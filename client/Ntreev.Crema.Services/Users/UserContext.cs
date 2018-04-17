//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.UserService;
using Ntreev.Library;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Timers;

namespace Ntreev.Crema.Services.Users
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    class UserContext : ItemContext<User, UserCategory, UserCollection, UserCategoryCollection, UserContext>,
        IUserServiceCallback, IUserContext, ICremaService
    {
        private readonly CremaHost cremaHost;
        private readonly Guid authenticationToken;
        private UserServiceClient service;
        private CremaDispatcher serviceDispatcher;
        private Timer timer;

        private ItemsCreatedEventHandler<IUserItem> itemsCreated;
        private ItemsRenamedEventHandler<IUserItem> itemsRenamed;
        private ItemsMovedEventHandler<IUserItem> itemsMoved;
        private ItemsDeletedEventHandler<IUserItem> itemsDeleted;
        private ItemsEventHandler<IUserItem> itemsChanged;
        private EventHandler<MessageEventArgs> messageReceived;
        private ItemsEventHandler<IUser> usersLoggedIn;
        private ItemsEventHandler<IUser> usersLoggedOut;
        private ItemsEventHandler<IUser> usersKicked;
        private ItemsEventHandler<IUser> usersBanChanged;

        private readonly Dictionary<string, Authentication> customAuthentications = new Dictionary<string, Authentication>();

        public UserContext(CremaHost cremaHost, string address, ServiceInfo serviceInfo, string userID, SecureString password)
        {
            this.cremaHost = cremaHost;

            this.serviceDispatcher = new CremaDispatcher(this);
            var metaData = this.serviceDispatcher.Invoke(() =>
            {
                this.service = UserServiceFactory.CreateServiceClient(address, serviceInfo, this);
                this.service.Open();
                if (this.service is ICommunicationObject service)
                {
                    service.Faulted += Service_Faulted;
                }
                var version = typeof(CremaHost).Assembly.GetName().Version;
                try
                {
                    var result = this.service.Subscribe(userID, UserContext.Encrypt(userID, password), $"{version}", $"{Environment.OSVersion.Platform}", $"{CultureInfo.CurrentCulture}");
                    result.Validate();
                    this.timer = new Timer(30000);
                    this.timer.Elapsed += Timer_Elapsed;
                    this.timer.Start();
                    return result.Value;
                }
                catch
                {
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                        this.service.Close();
                    else
                        this.service.Abort();
                    this.serviceDispatcher.Dispose();
                    this.serviceDispatcher = null;
                    throw;
                }
            });

            this.authenticationToken = metaData.AuthenticationToken;
            this.Initialize(metaData);
            this.Items.MessageReceived += Users_MessageReceived;
            this.Items.UsersLoggedIn += Users_UsersLoggedIn;
            this.Items.UsersLoggedOut += Users_UsersLoggedOut;
            this.Items.UsersKicked += Users_UsersKicked;
            this.Items.UsersBanChanged += Users_UsersBanChanged;
            this.cremaHost.AddService(this);
        }

        public static byte[] Encrypt(string userID, SecureString value)
        {
            return StringToSecureString(SecureStringToString());

            string SecureStringToString()
            {
                var valuePtr = IntPtr.Zero;
                try
                {
                    valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                    return Marshal.PtrToStringUni(valuePtr);
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
                }
            }

            byte[] StringToSecureString(string text)
            {
                return Encoding.UTF8.GetBytes(StringUtility.Encrypt(text, userID));
            }
        }

        public Authentication Authenticate(SignatureDate signatureDate)
        {
            if (signatureDate.ID == Authentication.SystemID)
            {
                Authentication.System.SignatureDate = signatureDate;
                return Authentication.System;
            }

            var user = this.Users[signatureDate.ID];
            if (user != null)
            {
                user.Authentication.SignatureDate = signatureDate;
                return user.Authentication;
            }

            if (this.customAuthentications.ContainsKey(signatureDate.ID) == false)
            {
                this.customAuthentications.Add(signatureDate.ID, new Authentication(new AuthenticationProvider(signatureDate.ID)));
            }

            var authentication = this.customAuthentications[signatureDate.ID];
            authentication.SignatureDate = signatureDate;
            return authentication;
        }

        public Authentication Authenticate(AuthenticationInfo authenticationInfo)
        {
            var user = this.Users[authenticationInfo.ID];
            if (user != null)
                return user.Authentication;
            return null;
        }

        public void InvokeItemsCreatedEvent(Authentication authentication, IUserItem[] items, object[] args)
        {
            this.OnItemsCreated(new ItemsCreatedEventArgs<IUserItem>(authentication, items, args));
        }

        public void InvokeItemsRenamedEvent(Authentication authentication, IUserItem[] items, string[] oldNames, string[] oldPaths)
        {
            this.OnItemsRenamed(new ItemsRenamedEventArgs<IUserItem>(authentication, items, oldNames, oldPaths));
        }

        public void InvokeItemsMovedEvent(Authentication authentication, IUserItem[] items, string[] oldPaths, string[] oldParentPaths)
        {
            this.OnItemsMoved(new ItemsMovedEventArgs<IUserItem>(authentication, items, oldPaths, oldParentPaths));
        }

        public void InvokeItemsDeleteEvent(Authentication authentication, IUserItem[] items, string[] itemPaths)
        {
            this.OnItemsDeleted(new ItemsDeletedEventArgs<IUserItem>(authentication, items, itemPaths));
        }

        public void InvokeItemsChangedEvent(Authentication authentication, IUserItem[] items)
        {
            this.OnItemsChanged(new ItemsEventArgs<IUserItem>(authentication, items));
        }

        public UserContextMetaData GetMetaData(Authentication authentication)
        {
            this.Dispatcher.VerifyAccess();
            var metaData = new UserContextMetaData();

            {
                var query = from UserCategory item in this.Categories
                            where item != this.Root
                            orderby item.Path
                            select item.Path;

                metaData.Categories = query.ToArray();
            }

            {
                var query = from User item in this.Users
                            orderby item.Category.Path
                            select new UserMetaData()
                            {
                                Path = item.Path,
                                UserInfo = item.UserInfo,
                                UserState = item.UserState,
                                BanInfo = item.BanInfo,
                            };
                metaData.Users = query.ToArray();
            }

            return metaData;
        }

        public void NotifyMessage(Authentication authentication, string[] userIDs, string message)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(NotifyMessage), this, userIDs, message);
            var result = this.service.NotifyMessage(userIDs, message);
            result.Validate(authentication);
            var users = userIDs == null ? new User[] { } : userIDs.Select(item => this.Users[item]).ToArray();
            this.Users.InvokeNotifyMessageEvent(authentication, users, message);
        }

        public void Close(CloseInfo closeInfo)
        {
            this.serviceDispatcher?.Invoke(() =>
            {
                this.timer?.Dispose();
                this.timer = null;
                if (this.service != null)
                {
                    try
                    {
                        if (closeInfo.Reason != CloseReason.NoResponding)
                        {
                            this.service.Unsubscribe();
                            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                                this.service.Close();
                            else
                                this.service.Abort();
                        }
                        else
                        {
                            this.service.Abort();
                        }
                    }
                    catch
                    {
                        this.service.Abort();
                    }
                    this.service = null;
                }
                this.serviceDispatcher?.Dispose();
                this.serviceDispatcher = null;
            });
        }

        public Guid AuthenticationToken
        {
            get { return this.authenticationToken; }
        }

        public IUserService Service
        {
            get { return this.service; }
        }

        public UserCollection Users
        {
            get { return this.Items; }
        }

        public CremaHost CremaHost
        {
            get { return this.cremaHost; }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.cremaHost.Dispatcher; }
        }

        public event ItemsCreatedEventHandler<IUserItem> ItemsCreated
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsCreated += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsCreated -= value;
            }
        }

        public event ItemsRenamedEventHandler<IUserItem> ItemsRenamed
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsRenamed += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsRenamed -= value;
            }
        }

        public event ItemsMovedEventHandler<IUserItem> ItemsMoved
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsMoved += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsMoved -= value;
            }
        }

        public event ItemsDeletedEventHandler<IUserItem> ItemsDeleted
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsDeleted += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsDeleted -= value;
            }
        }

        public event ItemsEventHandler<IUserItem> ItemsChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsChanged -= value;
            }
        }

        public event EventHandler<MessageEventArgs> MessageReceived
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.messageReceived += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.messageReceived -= value;
            }
        }

        public event ItemsEventHandler<IUser> UsersLoggedIn
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.usersLoggedIn += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.usersLoggedIn -= value;
            }
        }

        public event ItemsEventHandler<IUser> UsersLoggedOut
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.usersLoggedOut += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.usersLoggedOut -= value;
            }
        }

        public event ItemsEventHandler<IUser> UsersKicked
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.usersKicked += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.usersKicked -= value;
            }
        }

        public event ItemsEventHandler<IUser> UsersBanChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.usersBanChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.usersBanChanged -= value;
            }
        }

        protected virtual void OnItemsCreated(ItemsCreatedEventArgs<IUserItem> e)
        {
            this.itemsCreated?.Invoke(this, e);
        }

        protected virtual void OnItemsRenamed(ItemsRenamedEventArgs<IUserItem> e)
        {
            this.itemsRenamed?.Invoke(this, e);
        }

        protected virtual void OnItemsMoved(ItemsMovedEventArgs<IUserItem> e)
        {
            this.itemsMoved?.Invoke(this, e);
        }

        protected virtual void OnItemsDeleted(ItemsDeletedEventArgs<IUserItem> e)
        {
            this.itemsDeleted?.Invoke(this, e);
        }

        protected virtual void OnItemsChanged(ItemsEventArgs<IUserItem> e)
        {
            this.itemsChanged?.Invoke(this, e);
        }

        private void Initialize(UserContextMetaData metaData)
        {
            foreach (var item in metaData.Categories)
            {
                if (item == this.Root.Path)
                    continue;
                this.Categories.Prepare(item);
            }

            foreach (var item in metaData.Users)
            {
                var itemName = new ItemName(item.Path);
                var user = this.Users.BaseAddNew(itemName.Name, itemName.CategoryPath);
                user.Initialize(item.UserInfo, item.BanInfo);
                user.SetUserState(item.UserState);
            }
        }

        private void Service_Faulted(object sender, EventArgs e)
        {
            this.serviceDispatcher.Invoke(() =>
            {
                try
                {
                    this.service.Abort();
                    this.service = null;
                }
                catch
                {

                }
                this.timer?.Dispose();
                this.timer = null;
                this.serviceDispatcher.Dispose();
                this.serviceDispatcher = null;
            });
            this.InvokeAsync(() =>
            {
                this.cremaHost.RemoveService(this, new CloseInfo(CloseReason.Faulted, "서버와의 연결이 끊어졌습니다."));
            }, nameof(Service_Faulted));
        }

        private async void InvokeAsync(Action action, string callbackName)
        {
            try
            {
                await this.Dispatcher.InvokeAsync(action);
            }
            catch (Exception e)
            {
                this.cremaHost.Error(callbackName);
                this.cremaHost.Error(e);
            }
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.timer.Stop();
            try
            {
                await this.serviceDispatcher.InvokeAsync(() => this.service.IsAlive());
                this.timer?.Start();
            }
            catch
            {
                this.cremaHost.Dispatcher.Invoke(() =>
                {
                    this.cremaHost.InvokeClose(new CloseInfo(CloseReason.NoResponding, "서버가 응답하질 않습니다."));
                });
            }
        }

        private void Users_MessageReceived(object sender, MessageEventArgs e)
        {
            this.messageReceived?.Invoke(this, e);
        }

        private void Users_UsersLoggedIn(object sender, ItemsEventArgs<IUser> e)
        {
            this.usersLoggedIn?.Invoke(this, e);
        }

        private void Users_UsersLoggedOut(object sender, ItemsEventArgs<IUser> e)
        {
            this.usersLoggedOut?.Invoke(this, e);
        }

        private void Users_UsersKicked(object sender, ItemsEventArgs<IUser> e)
        {
            this.usersKicked?.Invoke(this, e);
        }

        private void Users_UsersBanChanged(object sender, ItemsEventArgs<IUser> e)
        {
            this.usersBanChanged?.Invoke(this, e);
        }

        #region IUserServiceCallback

        void IUserServiceCallback.OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo)
        {
            this.service.Abort();
            this.service = null;
            this.timer?.Dispose();
            this.timer = null;
            this.serviceDispatcher?.Dispose();
            this.serviceDispatcher = null;
            this.InvokeAsync(() =>
            {
                this.cremaHost.RemoveService(this, closeInfo);
            }, nameof(IUserServiceCallback.OnServiceClosed));
        }

        void IUserServiceCallback.OnUsersChanged(SignatureDate signatureDate, UserInfo[] userInfos)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.Authenticate(signatureDate);
                var users = new User[userInfos.Length];
                for (var i = 0; i < userInfos.Length; i++)
                {
                    var userInfo = userInfos[i];
                    var user = this.Users[userInfo.ID];
                    user.SetUserInfo(userInfo);
                    users[i] = user;
                }
                this.Users.InvokeUsersChangedEvent(authentication, users);
            }, nameof(IUserServiceCallback.OnUsersChanged));
        }

        void IUserServiceCallback.OnUsersStateChanged(SignatureDate signatureDate, string[] userIDs, UserState[] states)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.Authenticate(signatureDate);
                var users = new User[userIDs.Length];
                for (var i = 0; i < userIDs.Length; i++)
                {
                    var user = this.Users[userIDs[i]];
                    user.SetUserState(states[i]);
                    users[i] = user;
                }
                this.Users.InvokeUsersStateChangedEvent(authentication, users);
            }, nameof(IUserServiceCallback.OnUsersStateChanged));
        }

        void IUserServiceCallback.OnUserItemsCreated(SignatureDate signatureDate, string[] itemPaths, UserInfo?[] args)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.Authenticate(signatureDate);
                var userItems = new IUserItem[itemPaths.Length];
                var categories = new List<UserCategory>(itemPaths.Length);
                var users = new List<User>(itemPaths.Length);

                for (var i = 0; i < itemPaths.Length; i++)
                {
                    var itemPath = itemPaths[i];
                    if (NameValidator.VerifyCategoryPath(itemPath) == true)
                    {
                        var categoryName = new CategoryName(itemPath);
                        var category = this.Categories.Prepare(itemPath);
                        categories.Add(category);
                        userItems[i] = category;
                    }
                    else
                    {
                        var userInfo = (UserInfo)args[i];
                        var user = this.Users.BaseAddNew(userInfo.ID, userInfo.CategoryPath);
                        user.Initialize(userInfo, BanInfo.Empty);
                        users.Add(user);
                        userItems[i] = user;
                    }
                }

                if (categories.Any() == true)
                {
                    this.Categories.InvokeCategoriesCreatedEvent(authentication, categories.ToArray());
                }

                if (users.Any() == true)
                {
                    this.Users.InvokeUsersCreatedEvent(authentication, users.ToArray());
                }
            }, nameof(IUserServiceCallback.OnUserItemsCreated));
        }

        void IUserServiceCallback.OnUserItemsRenamed(SignatureDate signatureDate, string[] itemPaths, string[] newNames)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.Authenticate(signatureDate);

                {
                    var items = new List<UserCategory>(itemPaths.Length);
                    var oldNames = new List<string>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var userItem = this[itemPaths[i]];
                        if (userItem is UserCategory == false)
                            continue;

                        var category = userItem as UserCategory;
                        items.Add(category);
                        oldNames.Add(category.Name);
                        oldPaths.Add(category.Path);
                    }

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var userItem = this[itemPaths[i]];
                        if (userItem is UserCategory == false)
                            continue;

                        var category = userItem as UserCategory;
                        category.InternalSetName(newNames[i]);
                    }

                    this.Categories.InvokeCategoriesRenamedEvent(authentication, items.ToArray(), oldNames.ToArray(), oldPaths.ToArray());
                }

                {
                    var items = new List<User>(itemPaths.Length);
                    var oldNames = new List<string>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var userItem = this[itemPaths[i]];
                        if (userItem is User == false)
                            continue;

                        var user = userItem as User;
                        items.Add(user);
                        oldNames.Add(user.Name);
                        oldPaths.Add(user.Path);
                    }

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var userItem = this[itemPaths[i]];
                        if (userItem is User == false)
                            continue;

                        var user = userItem as User;
                        user.Name = newNames[i];
                    }

                    this.Users.InvokeUsersRenamedEvent(authentication, items.ToArray(), oldNames.ToArray(), oldPaths.ToArray());
                }

            }, nameof(IUserServiceCallback.OnUserItemsRenamed));
        }

        void IUserServiceCallback.OnUserItemsMoved(SignatureDate signatureDate, string[] itemPaths, string[] parentPaths)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.Authenticate(signatureDate);

                {
                    var items = new List<UserCategory>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);
                    var oldParentPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var userItem = this[itemPaths[i]];
                        if (userItem is UserCategory == false)
                            continue;

                        var category = userItem as UserCategory;
                        items.Add(category);
                        oldPaths.Add(category.Path);
                        oldParentPaths.Add(category.Parent.Path);
                    }

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var userItem = this[itemPaths[i]];
                        if (userItem is UserCategory == false)
                            continue;

                        var category = userItem as UserCategory;
                        category.Parent = this.Categories[parentPaths[i]];
                    }

                    this.Categories.InvokeCategoriesMovedEvent(authentication, items.ToArray(), oldPaths.ToArray(), oldParentPaths.ToArray());
                }

                {
                    var items = new List<User>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);
                    var oldParentPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var userItem = this[itemPaths[i]];
                        if (userItem is User == false)
                            continue;

                        var user = userItem as User;
                        items.Add(user);
                        oldPaths.Add(user.Path);
                        oldParentPaths.Add(user.Category.Path);
                    }

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var userItem = this[itemPaths[i]];
                        if (userItem is User == false)
                            continue;

                        var user = userItem as User;
                        user.Category = this.Categories[parentPaths[i]];
                    }

                    this.Users.InvokeUsersMovedEvent(authentication, items.ToArray(), oldPaths.ToArray(), oldParentPaths.ToArray());
                }
            }, nameof(IUserServiceCallback.OnUserItemsMoved));
        }

        void IUserServiceCallback.OnUserItemsDeleted(SignatureDate signatureDate, string[] itemPaths)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.Authenticate(signatureDate);

                {
                    var items = new List<UserCategory>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var userItem = this[itemPaths[i]];
                        if (userItem is UserCategory == false)
                            continue;

                        var category = userItem as UserCategory;
                        items.Add(category);
                        oldPaths.Add(category.Path);
                    }

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var userItem = this[itemPaths[i]];
                        if (userItem is UserCategory == false)
                            continue;

                        var category = userItem as UserCategory;
                        category.Dispose();
                    }

                    this.Categories.InvokeCategoriesDeletedEvent(authentication, items.ToArray(), oldPaths.ToArray());
                }

                {
                    var items = new List<User>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var userItem = this[itemPaths[i]];
                        if (userItem is User == false)
                            continue;

                        var user = userItem as User;
                        items.Add(user);
                        oldPaths.Add(user.Path);
                    }

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var userItem = this[itemPaths[i]];
                        if (userItem is User == false)
                            continue;

                        var user = userItem as User;
                        user.Dispose();
                    }

                    this.Users.InvokeUsersDeletedEvent(authentication, items.ToArray(), oldPaths.ToArray());
                }
            }, nameof(IUserServiceCallback.OnUserItemsDeleted));
        }

        void IUserServiceCallback.OnUsersLoggedIn(SignatureDate signatureDate, string[] userIDs)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.Authenticate(signatureDate);
                var users = new User[userIDs.Length];
                for (var i = 0; i < userIDs.Length; i++)
                {
                    var user = this.Users[userIDs[i]];
                    user.IsOnline = true;
                    users[i] = user;
                }
                this.Users.InvokeUsersLoggedInEvent(authentication, users);
            }, nameof(IUserServiceCallback.OnUsersLoggedIn));
        }

        void IUserServiceCallback.OnUsersLoggedOut(SignatureDate signatureDate, string[] userIDs)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.Authenticate(signatureDate);
                var users = new User[userIDs.Length];
                for (var i = 0; i < userIDs.Length; i++)
                {
                    var user = this.Users[userIDs[i]];
                    user.IsOnline = false;
                    users[i] = user;
                }
                this.Users.InvokeUsersLoggedOutEvent(authentication, users, CloseInfo.Empty);
            }, nameof(IUserServiceCallback.OnUsersLoggedOut));
        }

        void IUserServiceCallback.OnUsersKicked(SignatureDate signatureDate, string[] userIDs, string[] comments)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.Authenticate(signatureDate);
                var users = new User[userIDs.Length];
                for (var i = 0; i < userIDs.Length; i++)
                {
                    var user = this.Users[userIDs[i]];
                    user.IsOnline = false;
                    users[i] = user;
                }
                this.Users.InvokeUsersKickedEvent(authentication, users, comments);
            }, nameof(IUserServiceCallback.OnUsersKicked));
        }

        void IUserServiceCallback.OnUsersBanChanged(SignatureDate signatureDate, BanInfo[] banInfos, BanChangeType changeType, string[] comments)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.Authenticate(signatureDate);
                var users = new User[banInfos.Length];
                for (var i = 0; i < banInfos.Length; i++)
                {
                    var banInfo = banInfos[i];
                    var user = this[banInfo.Path] as User;
                    user.SetBanInfo(changeType, banInfo);
                    users[i] = user;
                }

                switch (changeType)
                {
                    case BanChangeType.Ban:
                        this.Users.InvokeUsersBannedEvent(authentication, users, comments);
                        break;
                    case BanChangeType.Unban:
                        this.Users.InvokeUsersUnbannedEvent(authentication, users);
                        break;
                }

            }, nameof(IUserServiceCallback.OnUsersBanChanged));
        }

        void IUserServiceCallback.OnMessageReceived(SignatureDate signatureDate, string[] userIDs, string message, MessageType messageType)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.Authenticate(signatureDate);
                if (messageType == MessageType.None)
                {
                    foreach (var item in userIDs)
                    {
                        var user = this.Users[item];
                        this.Users.InvokeSendMessageEvent(authentication, user, message);
                    }
                }
                else if (messageType == MessageType.Notification)
                {
                    var users = new User[userIDs.Length];
                    for (var i = 0; i < userIDs.Length; i++)
                    {
                        var user = this.Users[userIDs[i]];
                        users[i] = user;
                    }
                    this.Users.InvokeNotifyMessageEvent(authentication, users, message);
                }
            }, nameof(IUserServiceCallback.OnMessageReceived));
        }

        bool IUserServiceCallback.OnPing()
        {
            return true;
        }

        #endregion

        #region IUserContext

        bool IUserContext.Contains(string itemPath)
        {
            this.Dispatcher.VerifyAccess();
            return this.Contains(itemPath);
        }

        IUserCollection IUserContext.Users
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Users;
            }
        }

        IUserCategoryCollection IUserContext.Categories
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Categories;
            }
        }

        IUserItem IUserContext.this[string itemPath]
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this[itemPath] as IUserItem;
            }
        }

        IUserCategory IUserContext.Root
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Root;
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator<IUserItem> IEnumerable<IUserItem>.GetEnumerator()
        {
            this.Dispatcher.VerifyAccess();
            foreach (var item in this)
            {
                yield return item as IUserItem;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            this.Dispatcher.VerifyAccess();
            foreach (var item in this)
            {
                yield return item as IUserItem;
            }
        }

        #endregion

        #region IServiceProvider

        object IServiceProvider.GetService(System.Type serviceType)
        {
            return (this.CremaHost as ICremaHost).GetService(serviceType);
        }

        #endregion
    }
}
