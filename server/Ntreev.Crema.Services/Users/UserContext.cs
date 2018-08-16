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
using Ntreev.Crema.Services.Users.Serializations;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace Ntreev.Crema.Services.Users
{
    class UserContext : ItemContext<User, UserCategory, UserCollection, UserCategoryCollection, UserContext>,
        IUserContext, IServiceProvider, IDisposable
    {
        private readonly string remotePath;
        private readonly string basePath;
        private readonly IObjectSerializer serializer;

        private ItemsCreatedEventHandler<IUserItem> itemsCreated;
        private ItemsRenamedEventHandler<IUserItem> itemsRenamed;
        private ItemsMovedEventHandler<IUserItem> itemsMoved;
        private ItemsDeletedEventHandler<IUserItem> itemsDeleted;
        private ItemsEventHandler<IUserItem> itemsChanged;

        public UserContext(CremaHost cremaHost)
        {
            this.CremaHost = cremaHost;
            this.CremaHost.Debug(Resources.Message_UserContextInitialize);

            this.remotePath = cremaHost.GetPath(CremaPath.RepositoryUsers);
            this.basePath = cremaHost.GetPath(CremaPath.Users);
            this.serializer = cremaHost.Serializer;

            this.Repository = new UserRepositoryHost(this, this.CremaHost.RepositoryProvider.CreateInstance(new RepositorySettings()
            {
                BasePath = this.remotePath,
                RepositoryName = string.Empty,
                WorkingPath = this.basePath,
                LogService = this.CremaHost
            }));

            this.Dispatcher = new CremaDispatcher(this);
            this.CremaHost.Debug(Resources.Message_UserContextIsCreated);
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
            try
            {
                this.Dispatcher.VerifyAccess();
                this.ValidateLogin(userID, password);
                var user = this.Users[userID];
                return user.Login(password);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Logout(Authentication authentication)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.ValidateLogout(authentication);
                var user = this.Users[authentication.ID];
                user.Logout(authentication);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void NotifyMessage(Authentication authentication, string[] userIDs, string message)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(NotifyMessage), this, userIDs, message);
                this.ValidateSendMessage(authentication, userIDs, message);
                var users = userIDs == null ? new User[] { } : userIDs.Select(item => this.Users[item]).ToArray();
                authentication.Sign();
                this.Users.InvokeNotifyMessageEvent(authentication, users, message);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public Authentication Authenticate(Guid authenticationToken)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                var query = from User item in this.Users
                            let authentication = item.Authentication
                            where authentication != null && authentication.Token == authenticationToken
                            select item;

                if (query.Any() == true)
                    return query.First().Authentication;

                if (authenticationToken == Authentication.System.Token)
                    return Authentication.System;

                return null;
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public bool IsAuthenticated(string userID)
        {
            this.Dispatcher.VerifyAccess();

            if (this.Users.Contains(userID) == false)
                return false;

            var user = this.Users[userID];

            if (user.IsOnline == false)
                return false;

            return user.Authentication != null;
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
                                UserInfo = item.UserInfo,
                                UserState = item.UserState,
                                BanInfo = item.BanInfo,
                            };
                metaData.Users = query.ToArray();
            }

            metaData.AuthenticationToken = authentication.Token;
            return metaData;
        }

        public static void GenerateDefaultUserInfos(string repositoryPath, IObjectSerializer serializer)
        {
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
            for (var i = 0; i < 10; i++)
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
            serializationInfo.WriteToDirectory(repositoryPath, serializer);
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

        public string GenerateCategoryPath(string parentPath, string name)
        {
            var value = new CategoryName(parentPath, name);
            return this.GenerateCategoryPath(value.Path);
        }

        public string GenerateCategoryPath(string categoryPath)
        {
            NameValidator.ValidateCategoryPath(categoryPath);
            var baseUri = new Uri(this.basePath);
            var uri = new Uri(baseUri + categoryPath);
            return uri.LocalPath;
        }

        public string GenerateUserPath(string categoryPath, string userID)
        {
            return Path.Combine(this.GenerateCategoryPath(categoryPath), userID);
        }

        public string GeneratePath(string path)
        {
            if (NameValidator.VerifyCategoryPath(path) == true)
                return this.GenerateCategoryPath(path);
            var itemName = new ItemName(path);
            return this.GenerateUserPath(itemName.CategoryPath, itemName.Name);
        }

        public string[] GetFiles(string itemPath)
        {
            var directoryName = Path.GetDirectoryName(itemPath);
            var name = Path.GetFileNameWithoutExtension(itemPath);
            var files = Directory.GetFiles(directoryName, $"{name}.*").Where(item => Path.GetFileNameWithoutExtension(item) == name).ToArray();
            return files;
        }

        public void Initialize()
        {
            this.CremaHost.Debug("Load user data...");

            var directories = DirectoryUtility.GetAllDirectories(this.BasePath, "*", true);
            foreach (var item in directories)
            {
                var relativeUri = UriUtility.MakeRelativeOfDirectory(this.basePath, item);
                var segments = StringUtility.Split(relativeUri, PathUtility.SeparatorChar, true);
                var categoryName = CategoryName.Create(relativeUri);
                this.Categories.Prepare(categoryName);
            }

            var settings = ObjectSerializerSettings.Empty;
            var itemPaths = this.Serializer.GetItemPaths(this.BasePath, typeof(UserSerializationInfo), settings);
            foreach (var item in itemPaths)
            {
                var userInfo = (UserSerializationInfo)this.Serializer.Deserialize(item, typeof(UserSerializationInfo), settings);
                var directory = Path.GetDirectoryName(item);
                var relativeUri = UriUtility.MakeRelativeOfDirectory(this.basePath, item);
                var segments = StringUtility.Split(relativeUri, PathUtility.SeparatorChar, true);
                var itemName = ItemName.Create(segments);
                var user = this.Users.AddNew(userInfo.ID, itemName.CategoryPath);
                user.Initialize((UserInfo)userInfo, (BanInfo)userInfo.BanInfo);
                user.Password = UserContext.StringToSecureString(userInfo.Password);
            }

            this.CremaHost.Debug("Loading complete!");
        }

        public void Dispose()
        {
            this.Repository.Dispose();
            this.Dispatcher.Dispose();
        }

        public new void Clear()
        {
            foreach (var item in this.Users)
            {
                if (item.Authentication != null)
                    item.Authentication.InvokeExpiredEvent(Authentication.System.ID);
            }
            base.Clear();
        }

        public UserRepositoryHost Repository { get; }

        public string BasePath => this.basePath;

        public UserCollection Users => this.Items;

        public CremaHost CremaHost { get; }

        public CremaDispatcher Dispatcher { get; }

        public IObjectSerializer Serializer => this.serializer;

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
