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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Services.Properties;
using Ntreev.Crema.Services;
using System.Collections;
using System.Text.RegularExpressions;
using Ntreev.Library.Serialization;
using System.Collections.Specialized;
using System.Security;
using Ntreev.Crema.Data.Xml.Schema;

namespace Ntreev.Crema.Services.Users
{
    class UserCollection : ItemContainer<User, UserCategory, UserCollection, UserCategoryCollection, UserContext>,
        IUserCollection
    {
        private ItemsCreatedEventHandler<IUser> usersCreated;
        private ItemsRenamedEventHandler<IUser> usersRenamed;
        private ItemsMovedEventHandler<IUser> usersMoved;
        private ItemsDeletedEventHandler<IUser> usersDeleted;
        private ItemsEventHandler<IUser> usersStateChanged;
        private ItemsEventHandler<IUser> usersChanged;
        private ItemsEventHandler<IUser> usersLoggedIn;
        private ItemsEventHandler<IUser> usersLoggedOut;
        private ItemsEventHandler<IUser> usersKicked;
        private ItemsEventHandler<IUser> usersBanChanged;
        private EventHandler<MessageEventArgs> messageReceived;
        private EventHandler<MessageEventArgs2> messageReceived2;

        public UserCollection()
        {

        }

        public User AddNew(string name, string categoryPath)
        {
            try
            {
                return this.BaseAddNew(name, categoryPath, null);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public User AddNew(Authentication authentication, string userID, string categoryPath, SecureString password, string userName, Authority authority)
        {
            try
            {
                this.ValidateUserCreate(authentication, userID, categoryPath, password);
                this.Sign(authentication);
                var designedInfo = new SignatureDate(authentication.ID, DateTime.UtcNow);
                var userInfo = new UserSerializationInfo()
                {
                    ID = userID,
                    Password = UserContext.SecureStringToString(password).Encrypt(),
                    Name = userName,
                    Authority = authority,
                    CategoryPath = categoryPath,
                    CreationInfo = designedInfo,
                    ModificationInfo = designedInfo,
                };

                this.InvokeUserCreate(authentication, userInfo);

                var user = this.BaseAddNew(userID, categoryPath, null);
                user.Initialize((UserInfo)userInfo, BanInfo.Empty);
                user.Password = UserContext.StringToSecureString(userInfo.Password);
                this.InvokeUsersCreatedEvent(authentication, new User[] { user });
                return user;
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void InvokeUserCreate(Authentication authentication, UserSerializationInfo userInfo)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeUserCreate), userInfo.ID, userInfo.Authority, userInfo.CategoryPath);
            try
            {
                var categories = from UserCategory item in this.Context.Categories
                                 select item.Path;

                var users = from User item in this.Context.Users select item.SerializationInfo;
                var userList = users.ToList();
                userList.Add(userInfo);

                var serializationInfo = new UserContextSerializationInfo()
                {
                    Version = CremaSchema.VersionValue,
                    Categories = categories.ToArray(),
                    Users = userList.ToArray(),
                };

                var xml = DataContractSerializerUtility.GetString(serializationInfo, true);

                this.Repository.Modify(this.Context.UserFilePath, xml);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeUserRename(Authentication authentication, User user, string newName)
        {
            throw new NotImplementedException();
        }

        public void InvokeUserMove(Authentication authentication, User user, string categoryPath)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeUserMove), user, categoryPath);

            try
            {
                var userInfo = user.SerializationInfo;
                userInfo.CategoryPath = categoryPath;
                userInfo.ModificationInfo = new SignatureDate(authentication.ID, DateTime.UtcNow);

                var categories = from UserCategory item in this.Context.Categories
                                 select item.Path;

                var users = from User item in this.Context.Users
                            select userInfo.ID == item.ID ? userInfo : item.SerializationInfo;

                var serializationInfo = new UserContextSerializationInfo()
                {
                    Version = CremaSchema.VersionValue,
                    Categories = categories.ToArray(),
                    Users = users.ToArray(),
                };

                var xml = DataContractSerializerUtility.GetString(serializationInfo, true);
                this.Repository.Modify(this.Context.UserFilePath, xml);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeUserDelete(Authentication authentication, User user)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeUserDelete), user);

            try
            {
                var categories = from UserCategory item in this.Context.Categories
                                 select item.Path;

                var users = from User item in this.Context.Users
                            where item.ID != user.ID
                            select item.SerializationInfo;

                var serializationInfo = new UserContextSerializationInfo()
                {
                    Version = CremaSchema.VersionValue,
                    Categories = categories.ToArray(),
                    Users = users.ToArray(),
                };

                var xml = DataContractSerializerUtility.GetString(serializationInfo, true);
                this.Repository.Modify(this.Context.UserFilePath, xml);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeUserChange(Authentication authentication, User user, UserSerializationInfo userInfo)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeUserChange), user);

            try
            {
                var categories = from UserCategory item in this.Context.Categories
                                 select item.Path;

                var users = from User item in this.Context.Users
                            select item.ID == userInfo.ID ? userInfo : item.SerializationInfo;

                var serializationInfo = new UserContextSerializationInfo()
                {
                    Version = CremaSchema.VersionValue,
                    Categories = categories.ToArray(),
                    Users = users.ToArray(),
                };

                var xml = DataContractSerializerUtility.GetString(serializationInfo, true);
                this.Repository.Modify(this.Context.UserFilePath, xml);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeUserBan(Authentication authentication, User user, BanInfo banInfo)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeUserBan), user, banInfo.Comment);

            try
            {
                var userInfo = user.SerializationInfo;
                userInfo.BanInfo = (BanSerializationInfo)banInfo;

                var categories = from UserCategory item in this.Context.Categories
                                 select item.Path;

                var users = from User item in this.Context.Users
                            select userInfo.ID == item.ID ? userInfo : item.SerializationInfo;

                var serializationInfo = new UserContextSerializationInfo()
                {
                    Version = CremaSchema.VersionValue,
                    Categories = categories.ToArray(),
                    Users = users.ToArray(),
                };

                var xml = DataContractSerializerUtility.GetString(serializationInfo, true);
                this.Repository.Modify(this.Context.UserFilePath, xml);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeUserUnban(Authentication authentication, User user)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeUserUnban), user);

            try
            {
                var userInfo = user.SerializationInfo;
                userInfo.BanInfo = (BanSerializationInfo)BanInfo.Empty;

                var categories = from UserCategory item in this.Context.Categories
                                 select item.Path;

                var users = from User item in this.Context.Users
                            select userInfo.ID == item.ID ? userInfo : item.SerializationInfo;

                var serializationInfo = new UserContextSerializationInfo()
                {
                    Version = CremaSchema.VersionValue,
                    Categories = categories.ToArray(),
                    Users = users.ToArray(),
                };

                var xml = DataContractSerializerUtility.GetString(serializationInfo, true);
                this.Repository.Modify(this.Context.UserFilePath, xml);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeUserKick(Authentication authentication, User user, string comment)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeUserKick), user, comment);
        }

        public void InvokeUsersCreatedEvent(Authentication authentication, User[] users)
        {
            var args = users.Select(item => (object)item.UserInfo).ToArray();
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeUsersCreatedEvent), users);
            var comment = EventMessageBuilder.CreateUser(authentication, users);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnUsersCreated(new ItemsCreatedEventArgs<IUser>(authentication, users, args));
            this.Context.InvokeItemsCreatedEvent(authentication, users, args);
        }

        public void InvokeUsersRenamedEvent(Authentication authentication, User[] users, string[] oldNames, string[] oldPaths)
        {
            throw new NotImplementedException();
        }

        public void InvokeUsersMovedEvent(Authentication authentication, User[] users, string[] oldPaths, string[] oldCategoryPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeUsersMovedEvent), users, oldPaths, oldCategoryPaths);
            var comment = EventMessageBuilder.MoveUser(authentication, users, oldCategoryPaths);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnUsersMoved(new ItemsMovedEventArgs<IUser>(authentication, users, oldPaths, oldCategoryPaths));
            this.Context.InvokeItemsMovedEvent(authentication, users, oldPaths, oldCategoryPaths);
        }

        public void InvokeUsersDeletedEvent(Authentication authentication, User[] users, string[] itemPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeUsersDeletedEvent), itemPaths);
            var comment = EventMessageBuilder.DeleteUser(authentication, users);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnUsersDeleted(new ItemsDeletedEventArgs<IUser>(authentication, users, itemPaths));
            this.Context.InvokeItemsDeleteEvent(authentication, users, itemPaths);
        }

        public void InvokeUsersChangedEvent(Authentication authentication, User[] users)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeUsersChangedEvent), users);
            var comment = EventMessageBuilder.ChangeUserInfo(authentication, users);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnUsersChanged(new ItemsEventArgs<IUser>(authentication, users));
            this.Context.InvokeItemsChangedEvent(authentication, users);
        }

        public void InvokeUsersStateChangedEvent(Authentication authentication, User[] users)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeUsersStateChangedEvent), users);
            this.OnUsersStateChanged(new ItemsEventArgs<IUser>(authentication, users));
        }

        public void InvokeUsersLoggedInEvent(Authentication authentication, User[] users)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeUsersLoggedInEvent), users);
            this.CremaHost.Info(EventMessageBuilder.LoginUser(authentication, users));
            this.OnUsersLoggedIn(new ItemsEventArgs<IUser>(authentication, users));
        }

        public void InvokeUsersLoggedOutEvent(Authentication authentication, User[] users, CloseInfo closeInfo)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeUsersLoggedOutEvent), users, closeInfo.Reason, closeInfo.Message);
            var comment = EventMessageBuilder.LogoutUser(authentication, users);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnUsersLoggedOut(new ItemsEventArgs<IUser>(authentication, users, closeInfo));
        }

        public void InvokeUsersKickedEvent(Authentication authentication, User[] users, string[] comments)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeUsersKickedEvent), users, comments);
            var comment = EventMessageBuilder.KickUser(authentication, users, comments);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnUsersKicked(new ItemsEventArgs<IUser>(authentication, users, comments));
        }

        public void InvokeUsersBannedEvent(Authentication authentication, User[] users, string[] comments)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeUsersBannedEvent), users, comments);
            var comment = EventMessageBuilder.BanUser(authentication, users, comments);
            var metaData = EventMetaDataBuilder.Build(users, BanChangeType.Ban, comments);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnUsersBanChanged(new ItemsEventArgs<IUser>(authentication, users, metaData));
            this.Context.InvokeItemsChangedEvent(authentication, users);
        }

        public void InvokeUsersUnbannedEvent(Authentication authentication, User[] users)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeUsersUnbannedEvent), users);
            var comment = EventMessageBuilder.UnbanUser(authentication, users);
            var metaData = EventMetaDataBuilder.Build(users, BanChangeType.Unban);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnUsersBanChanged(new ItemsEventArgs<IUser>(authentication, users, metaData));
            this.Context.InvokeItemsChangedEvent(authentication, users);
        }

        public void InvokeSendMessageEvent(Authentication authentication, User user, string message)
        {
            var eventLog = EventLogBuilder.Build(authentication, this, nameof(InvokeSendMessageEvent), user, message);
            var comment = EventMessageBuilder.SendMessage(authentication, user, message);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnMessageReceived(new MessageEventArgs(authentication, new IUser[] { user, }, message, MessageType.None));
        }

        public void InvokeNotifyMessageEvent(Authentication authentication, User[] users, string message)
        {
            var target = users.Any() == false ? "all users" : string.Join(",", users.Select(item => item.ID).ToArray());
            var eventLog = EventLogBuilder.Build(authentication, this, nameof(InvokeNotifyMessageEvent), target, message);
            var comment = EventMessageBuilder.NotifyMessage(authentication, users, message);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnMessageReceived(new MessageEventArgs(authentication, users, message, MessageType.Notification));
        }

        public void InvokeNotifyMessageEvent2(Authentication authentication, User[] users, string message, NotifyMessageType notifyMessageType)
        {
            var target = users.Any() == false ? "all users" : string.Join(",", users.Select(item => item.ID).ToArray());
            var eventLog = EventLogBuilder.Build(authentication, this, nameof(InvokeNotifyMessageEvent), target, message);
            var comment = EventMessageBuilder.NotifyMessage(authentication, users, message);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnMessageReceived2(new MessageEventArgs2(authentication, users, message, MessageType.Notification, notifyMessageType));
        }

        public RepositoryHost Repository
        {
            get { return this.Context.Repository; }
        }

        public CremaHost CremaHost
        {
            get { return this.Context.CremaHost; }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.Context.Dispatcher; }
        }

        public new int Count
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.Count;
            }
        }

        public event ItemsCreatedEventHandler<IUser> UsersCreated
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.usersCreated += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.usersCreated -= value;
            }
        }

        public event ItemsRenamedEventHandler<IUser> UsersRenamed
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.usersRenamed += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.usersRenamed -= value;
            }
        }

        public event ItemsMovedEventHandler<IUser> UsersMoved
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.usersMoved += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.usersMoved -= value;
            }
        }

        public event ItemsDeletedEventHandler<IUser> UsersDeleted
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.usersDeleted += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.usersDeleted -= value;
            }
        }

        public event ItemsEventHandler<IUser> UsersStateChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.usersStateChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.usersStateChanged -= value;
            }
        }

        public event ItemsEventHandler<IUser> UsersChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.usersChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.usersChanged -= value;
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

        public event EventHandler<MessageEventArgs2> MessageReceived2
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.messageReceived2 += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.messageReceived2 -= value;
            }
        }

        public new event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                base.CollectionChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                base.CollectionChanged -= value;
            }
        }

        protected virtual void OnUsersCreated(ItemsCreatedEventArgs<IUser> e)
        {
            this.usersCreated?.Invoke(this, e);
        }

        protected virtual void OnUsersRenamed(ItemsRenamedEventArgs<IUser> e)
        {
            this.usersRenamed?.Invoke(this, e);
        }

        protected virtual void OnUsersMoved(ItemsMovedEventArgs<IUser> e)
        {
            this.usersMoved?.Invoke(this, e);
        }

        protected virtual void OnUsersDeleted(ItemsDeletedEventArgs<IUser> e)
        {
            this.usersDeleted?.Invoke(this, e);
        }

        protected virtual void OnUsersStateChanged(ItemsEventArgs<IUser> e)
        {
            this.usersStateChanged?.Invoke(this, e);
        }

        protected virtual void OnUsersChanged(ItemsEventArgs<IUser> e)
        {
            this.usersChanged?.Invoke(this, e);
        }

        protected virtual void OnUsersLoggedIn(ItemsEventArgs<IUser> e)
        {
            this.usersLoggedIn?.Invoke(this, e);
        }

        protected virtual void OnUsersLoggedOut(ItemsEventArgs<IUser> e)
        {
            this.usersLoggedOut?.Invoke(this, e);
        }

        protected virtual void OnUsersKicked(ItemsEventArgs<IUser> e)
        {
            this.usersKicked?.Invoke(this, e);
        }

        protected virtual void OnUsersBanChanged(ItemsEventArgs<IUser> e)
        {
            this.usersBanChanged?.Invoke(this, e);
        }

        protected virtual void OnMessageReceived(MessageEventArgs e)
        {
            this.messageReceived?.Invoke(this, e);
        }

        protected virtual void OnMessageReceived2(MessageEventArgs2 e)
        {
            this.messageReceived2?.Invoke(this, e);
        }

        private void ValidateUserCreate(Authentication authentication, string userID, string categoryPath, SecureString password)
        {
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();

            if (password == null)
                throw new ArgumentNullException(nameof(password), Resources.Exception_InvalidPassword);

            var category = this.GetCategory(categoryPath);
            if (category == null)
                throw new CategoryNotFoundException(categoryPath);

            if (this.Contains(userID) == true)
                throw new ArgumentException(Resources.Exception_UserIDisAlreadyResitered, nameof(userID));

            if (VerifyName(userID) == false)
                throw new ArgumentException(Resources.Exception_InvalidUserID, nameof(userID));
        }

        private static bool VerifyName(string name)
        {
            if (NameValidator.VerifyName(name) == false)
                return false;
            return IdentifierValidator.Verify(name);
        }

        private void Sign(Authentication authentication)
        {
            authentication.Sign();
        }

        #region IUserCollection

        bool IUserCollection.Contains(string userID)
        {
            this.Dispatcher.VerifyAccess();
            return base.Contains(userID);
        }

        IUser IUserCollection.this[string userID]
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this[userID];
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator<IUser> IEnumerable<IUser>.GetEnumerator()
        {
            this.Dispatcher.VerifyAccess();
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.Dispatcher.VerifyAccess();
            return this.GetEnumerator();
        }

        #endregion

        #region IServiceProvider

        object IServiceProvider.GetService(System.Type serviceType)
        {
            return (this.Context as IUserContext).GetService(serviceType);
        }

        #endregion
    }
}
