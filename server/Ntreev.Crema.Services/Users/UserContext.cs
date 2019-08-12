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

using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.Properties;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace Ntreev.Crema.Services.Users
{
    class UserContext : ItemContext<User, UserCategory, UserCollection, UserCategoryCollection, UserContext>,
        IUserContext, IServiceProvider, IDisposable
    {
        public const string usersFileName = "users.xml";

        private readonly CremaHost cremaHost;
        private readonly CremaDispatcher dispatcher;
        private readonly RepositoryHost repository;
        private readonly string userFilePath;

        private ItemsCreatedEventHandler<IUserItem> itemsCreated;
        private ItemsRenamedEventHandler<IUserItem> itemsRenamed;
        private ItemsMovedEventHandler<IUserItem> itemsMoved;
        private ItemsDeletedEventHandler<IUserItem> itemsDeleted;
        private ItemsEventHandler<IUserItem> itemsChanged;
        private EventHandler<MessageEventArgs> messageReceived;
        private ItemsEventHandler<AuthenticationInfo> usersLoggedIn;
        private ItemsEventHandler<AuthenticationInfo> usersLoggedOut;
        private ItemsEventHandler<IUser> usersKicked;
        private ItemsEventHandler<IUserAuthentication> userAuthenticationsKicked;
        private ItemsEventHandler<IUser> usersBanChanged;

        public UserContext(CremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
            this.cremaHost.Debug(Resources.Message_UserContextInitialize);
            this.userFilePath = GenerateUsersFilePath(cremaHost.RepositoryPath);
            this.repository = new RepositoryHost(cremaHost.Repository, cremaHost.RepositoryDispatcher, this.userFilePath);
            this.dispatcher = new CremaDispatcher(this);
            this.dispatcher.Invoke(() =>
            {
                this.Items.MessageReceived += Users_MessageReceived;
                this.Items.UsersLoggedIn += Users_UsersLoggedIn;
                this.Items.UsersLoggedOut += Users_UsersLoggedOut;
                this.Items.UsersKicked += Users_UsersKicked;
                this.Items.UserAuthenticationsKicked += Users_UserAuthenticationsKicked;
                this.Items.UsersBanChanged += Users_UsersBanChanged;
                
            });
            this.cremaHost.Debug(Resources.Message_UserContextIsCreated);
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

        public Authentication Login(string userID, SecureString password)
        {
            this.Dispatcher.VerifyAccess();
            this.ValidateLogin(userID, password);

            var user = this.Users[userID];
            return user.Login(password);
        }

        public void Logout(Authentication authentication)
        {
            this.Dispatcher.VerifyAccess();
            this.ValidateLogout(authentication);
            var user = this.Users[authentication.ID];
            user.Logout(authentication);
        }

        public void NotifyMessage(Authentication authentication, string[] userIDs, string message)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(NotifyMessage), this, userIDs, message);
            this.ValidateSendMessage(authentication, userIDs, message);
            var users = userIDs == null ? new User[] { } : userIDs.Select(item => this.Users[item]).ToArray();
            authentication.Sign();
            this.Users.InvokeNotifyMessageEvent(authentication, users, message);
        }

        public Authentication Authenticate(Guid authenticationToken)
        {
            this.Dispatcher.VerifyAccess();
            
            var query = from User item in this.Users
                let authentications = item.Authentications
                where authentications.ContainsKey(authenticationToken)
                select item;

            if (query.Any() == true)
                return query.First().Authentications[authenticationToken].Authentication;

            if (authenticationToken == Authentication.System.Token)
                return Authentication.System;

            return null;
        }

        public Authentication Authenticate(AuthenticationInfo authenticationInfo)
        {
            var user = this.Users[authenticationInfo.ID];
            if (user != null)
            {
                var auth = user.Authentications[authenticationInfo.Token];
                return auth.Authentication;
            }
            return null;
        }

        public bool IsAuthenticated(string userID, Guid token)
        {
            this.Dispatcher.VerifyAccess();

            if (this.Users.Contains(userID) == false)
                return false;

            var user = this.Users[userID];

            if (!user.Authentications.ContainsKey(token))
                return false;

            return user.Authentications[token] != null;
        }

        public bool IsOnlineUser(string userID, SecureString password)
        {
            this.Dispatcher.VerifyAccess();
            if (this.Users.Contains(userID) == false)
                return false;

            var user = this.Users[userID];

            if (user.VerifyPassword(password) == false)
                return false;

            return user.IsOnline;
        }

        public UserContextMetaData GetMetaData(Authentication authentication)
        {
            this.Dispatcher.VerifyAccess();
            var metaData = new UserContextMetaData();

            {
                var query = from UserCategory item in this.Categories
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
                                UserInfo = new UserInfo
                                {
                                    ID = item.UserInfo.ID,
                                    Name = item.UserInfo.Name,
                                    AllowMultiLogin = item.UserInfo.AllowMultiLogin,
                                    CreationInfo = item.UserInfo.CreationInfo,
                                    ModificationInfo = item.UserInfo.ModificationInfo,
                                    CategoryPath = item.UserInfo.CategoryPath,
                                    CategoryName = item.UserInfo.CategoryName,
                                    Authority = item.UserInfo.Authority,
                                    AuthenticationInfos = item.Authentications.Values.Select(o => o.Authentication.AuthenticationInfo).ToArray()
                                },
                                UserState = item.UserState,
                                BanInfo = item.BanInfo,
                            };
                metaData.Users = query.ToArray();
            }

            metaData.AuthenticationToken = authentication.Token;
            return metaData;
        }

        public static string GenerateUsersFilePath(string basePath)
        {
            return Path.Combine(basePath, usersFileName);
        }

        public static void GenerateDefaultUserInfos(string repositoryPath)
        {
            var filename = UserContext.GenerateUsersFilePath(repositoryPath);
            var designedInfo = new SignatureDate(Authentication.SystemID, DateTime.UtcNow);
            var administrator = new UserSerializationInfo()
            {
                ID = Authentication.AdminID,
                Name = Authentication.AdminName,
                CategoryName = string.Empty,
                Authority = Authority.Admin,
                Password = Authentication.AdminID.Encrypt(),
                CreationInfo = designedInfo,
                ModificationInfo = designedInfo,
                BanInfo = (BanSerializationInfo)BanInfo.Empty,
            };

#if DEBUG
            var users = new List<UserSerializationInfo>
            {
                administrator
            };
            for (var i = 0; i < 0; i++)
            {
                var admin = new UserSerializationInfo()
                {
                    ID = "admin" + i,
                    Name = "관리자" + i,
                    CategoryName = "Administrators",
                    Authority = Authority.Admin,
                    Password = "admin".Encrypt(),
                    CreationInfo = designedInfo,
                    ModificationInfo = designedInfo,
                    BanInfo = (BanSerializationInfo)BanInfo.Empty,
                };

                var member = new UserSerializationInfo()
                {
                    ID = "member" + i,
                    Name = "구성원" + i,
                    CategoryName = "Members",
                    Authority = Authority.Member,
                    Password = "member".Encrypt(),
                    CreationInfo = designedInfo,
                    ModificationInfo = designedInfo,
                    BanInfo = (BanSerializationInfo)BanInfo.Empty,
                };

                var guest = new UserSerializationInfo()
                {
                    ID = "guest" + i,
                    Name = "손님" + i,
                    CategoryName = "Guests",
                    Authority = Authority.Guest,
                    Password = "guest".Encrypt(),
                    CreationInfo = designedInfo,
                    ModificationInfo = designedInfo,
                    BanInfo = (BanSerializationInfo)BanInfo.Empty,
                };

                users.Add(admin);
                users.Add(member);
                users.Add(guest);
            }

            var serializationInfo = new UserContextSerializationInfo()
            {
                Version = CremaSchema.VersionValue,
                Categories = new string[] { "/Administrators/", "/Members/", "/Guests/" },
                Users = users.ToArray(),
            };
#else
            var serializationInfo = new UserContextSerializationInfo()
            {
                Version = CremaSchema.VersionValue,
                Categories = new string[] { },
                Users = new UserSerializationInfo[] { administrator},
            };
#endif
            //foreach (var item in serializationInfo.Categories)
            //{
            //    var basePath = Path.Combine(Path.GetDirectoryName(filename), "users");
            //    var localPath = PathUtility.ConvertFromUri(basePath + item);
            //    DirectoryUtility.Prepare(localPath);
            //}

            //foreach (var item in serializationInfo.Users)
            //{
            //    var basePath = Path.Combine(Path.GetDirectoryName(filename), "users");
            //    var localPath = PathUtility.ConvertFromUri(basePath + item.CategoryPath + item.ID + ".xml");
            //    FileUtility.WriteAllText(DataContractSerializerUtility.GetString(item), localPath);
            //}

            DataContractSerializerUtility.Write(filename, serializationInfo, true);
        }

        public static string SecureStringToString(SecureString value)
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

        public static SecureString StringToSecureString(string value)
        {
            var secureString = new SecureString();
            foreach (var item in value)
            {
                secureString.AppendChar(item);
            }
            return secureString;
        }

        public void Initialize()
        {
            this.cremaHost.Debug("Load user data...");

            if (File.Exists(this.userFilePath) == true)
            {
                var serializationInfo = DataContractSerializerUtility.Read<UserContextSerializationInfo>(this.userFilePath);

                foreach (var item in serializationInfo.Categories)
                {
                    if (item == this.Root.Path)
                        continue;
                    this.Categories.Prepare(item);
                }

                for (var i = 0; i < serializationInfo.Users.Length; i++)
                {
                    var item = serializationInfo.Users[i];
                    if (serializationInfo.Version == null)
                        item.BanInfo = (BanSerializationInfo)BanInfo.Empty;
                    var user = this.Users.AddNew(item.ID, item.CategoryPath);
                    user.Initialize((UserInfo)item, (BanInfo)item.BanInfo);
                    user.Password = UserContext.StringToSecureString(item.Password);
                }
            }
            else
            {
                var user = this.Users.AddNew(Authentication.AdminID, PathUtility.Separator);
                var userInfo = new UserInfo()
                {
                    ID = Authentication.AdminID,
                    Name = Authentication.AdminName,
                    CategoryPath = PathUtility.Separator,
                    Authority = Authority.Admin,
                };
                user.Initialize(userInfo, BanInfo.Empty);
                user.Password = UserContext.StringToSecureString(Authentication.AdminID.Encrypt());
            }

            this.cremaHost.Debug("Loading complete!");
        }

        public void Dispose()
        {
            this.dispatcher.Dispose();
        }

        public new void Clear()
        {
            foreach (var item in this.Users)
            {
                foreach (var auth in item.Authentications.Values.ToArray())
                {
                    auth.Authentication.InvokeExpiredEvent(Authentication.System.ID);
                }
                item.Authentications.Clear();
            }
            base.Clear();
        }

        public RepositoryHost Repository
        {
            get { return this.repository; }
        }

        public string UserFilePath
        {
            get { return this.userFilePath; }
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
            get { return this.dispatcher; }
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

        public event ItemsEventHandler<AuthenticationInfo> UsersLoggedIn
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

        public event ItemsEventHandler<AuthenticationInfo> UsersLoggedOut
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

        public event ItemsEventHandler<IUserAuthentication> UserAuthenticationsKicked
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.userAuthenticationsKicked += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.userAuthenticationsKicked -= value;
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

        private void ValidateLogin(string userID, SecureString password)
        {
            var user = this.Users[userID];
            if (user == null || user.VerifyPassword(password) == false)
                throw new ArgumentException(Resources.Exception_WrongIDOrPassword);
        }

        private void ValidateLogout(Authentication authentication)
        {
            var user = this.Users[authentication.ID];
            if (user == null)
                throw new UserNotFoundException(authentication.ID);
        }

        private void ValidateSendMessage(Authentication authentication, string[] userIDs, string message)
        {
            if (authentication.IsAdmin == false && authentication.IsSystem == false)
                throw new PermissionDeniedException();
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (message == string.Empty)
                throw new ArgumentException(Resources.Exception_EmptyStringCannotSend, nameof(message));
        }

        private void WriteUsers()
        {
            var categories = from UserCategory item in this.Categories
                             where item != this.Root
                             select item.Path;
            var users = from User item in this.Users
                        select item.SerializationInfo;

            var serializationInfo = new UserContextSerializationInfo()
            {
                Version = CremaSchema.VersionValue,
                Categories = categories.ToArray(),
                Users = users.ToArray(),
            };

            var xml = DataContractSerializerUtility.GetString(serializationInfo, true);

        }

        private void Users_MessageReceived(object sender, MessageEventArgs e)
        {
            this.messageReceived?.Invoke(this, e);
        }

        private void Users_UsersLoggedIn(object sender, ItemsEventArgs<AuthenticationInfo> e)
        {
            this.usersLoggedIn?.Invoke(this, e);
        }

        private void Users_UsersLoggedOut(object sender, ItemsEventArgs<AuthenticationInfo> e)
        {
            this.usersLoggedOut?.Invoke(this, e);
        }

        private void Users_UsersKicked(object sender, ItemsEventArgs<IUser> e)
        {
            this.usersKicked?.Invoke(this, e);
        }

        private void Users_UserAuthenticationsKicked(object sender, ItemsEventArgs<IUserAuthentication> e)
        {
            this.userAuthenticationsKicked?.Invoke(this, e);
        }

        private void Users_UsersBanChanged(object sender, ItemsEventArgs<IUser> e)
        {
            this.usersBanChanged?.Invoke(this, e);
        }

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
