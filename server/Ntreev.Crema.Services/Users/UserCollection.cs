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
using Ntreev.Crema.Services.Properties;
using Ntreev.Library;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security;

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

        public UserCollection()
        {

        }

        public User AddNew(string name, string categoryPath)
        {
            return this.BaseAddNew(name, categoryPath, null);
        }

        public User AddNew(Authentication authentication, string userID, string categoryPath, SecureString password, string userName, Authority authority)
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

        public void InvokeUserCreate(Authentication authentication, UserSerializationInfo userInfo)
        {
            var message = EventMessageBuilder.CreateUser(authentication, userInfo.ID, userInfo.Name);
            try
            {
                this.Repository.CreateUser(userInfo);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeUserRename(Authentication authentication, User user, string newName)
        {
            throw new NotImplementedException();
        }

        public void InvokeUserMove(Authentication authentication, User user, string categoryPath)
        {
            var message = EventMessageBuilder.MoveUser(authentication, user.ID, user.UserName, user.Category.Path, categoryPath);
            try
            {
                this.Repository.MoveUser(authentication, user, categoryPath);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeUserDelete(Authentication authentication, User user)
        {
            var message = EventMessageBuilder.DeleteUser(authentication, user.ID);
            try
            {
                this.Repository.DeleteUser(authentication, user);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeUserChange(Authentication authentication, User user, UserSerializationInfo userInfo)
        {
            var message = EventMessageBuilder.ChangeUserInfo(authentication, userInfo.ID, userInfo.Name);
            try
            {
                this.Repository.ModifyUser(authentication, user, userInfo);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeUserBan(Authentication authentication, User user, UserSerializationInfo userSerializationInfo)
        {
            var message = EventMessageBuilder.BanUser(authentication, user.ID, user.UserName, userSerializationInfo.BanInfo.Comment);
            try
            {
                this.Repository.ModifyUser(authentication, user, userSerializationInfo);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeUserUnban(Authentication authentication, User user, UserSerializationInfo userSerializationInfo)
        {
            var message = EventMessageBuilder.UnbanUser(authentication, user.ID, user.UserName);
            try
            {
                this.Repository.ModifyUser(authentication, user, userSerializationInfo);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeUserKick(Authentication authentication, User user, string comment)
        {

        }

        public void InvokeUsersCreatedEvent(Authentication authentication, User[] users)
        {
            var args = users.Select(item => (object)item.UserInfo).ToArray();
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeUsersCreatedEvent), users);
            var message = EventMessageBuilder.CreateUser(authentication, users);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
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
            var message = EventMessageBuilder.MoveUser(authentication, users, oldCategoryPaths);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnUsersMoved(new ItemsMovedEventArgs<IUser>(authentication, users, oldPaths, oldCategoryPaths));
            this.Context.InvokeItemsMovedEvent(authentication, users, oldPaths, oldCategoryPaths);
        }

        public void InvokeUsersDeletedEvent(Authentication authentication, User[] users, string[] itemPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeUsersDeletedEvent), itemPaths);
            var message = EventMessageBuilder.DeleteUser(authentication, users);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnUsersDeleted(new ItemsDeletedEventArgs<IUser>(authentication, users, itemPaths));
            this.Context.InvokeItemsDeleteEvent(authentication, users, itemPaths);
        }

        public void InvokeUsersChangedEvent(Authentication authentication, User[] users)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeUsersChangedEvent), users);
            var message = EventMessageBuilder.ChangeUserInfo(authentication, users);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
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
            var message = EventMessageBuilder.BanUser(authentication, users, comments);
            var metaData = EventMetaDataBuilder.Build(users, BanChangeType.Ban, comments);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnUsersBanChanged(new ItemsEventArgs<IUser>(authentication, users, metaData));
            this.Context.InvokeItemsChangedEvent(authentication, users);
        }

        public void InvokeUsersUnbannedEvent(Authentication authentication, User[] users)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeUsersUnbannedEvent), users);
            var message = EventMessageBuilder.UnbanUser(authentication, users);
            var metaData = EventMetaDataBuilder.Build(users, BanChangeType.Unban);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
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

        public UserRepositoryHost Repository => this.Context.Repository;

        public CremaHost CremaHost => this.Context.CremaHost;

        public CremaDispatcher Dispatcher => this.Context.Dispatcher;

        public IObjectSerializer Serializer => this.Context.Serializer;

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
