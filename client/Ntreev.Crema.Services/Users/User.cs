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

using Ntreev.Library;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Ntreev.Crema.Services.UserService;
using Ntreev.Library.Linq;
using System.Security;
using System.ComponentModel;
using System.IdentityModel.Tokens;

namespace Ntreev.Crema.Services.Users
{
    class User : UserBase<User, UserCategory, UserCollection, UserCategoryCollection, UserContext>,
        IUser, IUserItem, IInfoProvider, IStateProvider
    {
        public UserAuthenticationCollection Authentications { get; } = new UserAuthenticationCollection();

        public UserState UserState
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Authentications.Any() ? ServiceModel.UserState.Online : ServiceModel.UserState.None;
            }
        }

        public bool IsOnline
        {
            get { return this.UserState.HasFlag(UserState.Online); }
        }

        public User()
        {
            
        }

        public void Rename(Authentication authentication, string newName)
        {
            this.Dispatcher.VerifyAccess();
            throw new NotSupportedException();
        }

        public void Move(Authentication authentication, string categoryPath)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Move), this, categoryPath);
            var result = this.Service.MoveUserItem(this.Path, categoryPath);
            this.Sign(authentication, result);
            var items = EnumerableUtility.One(this).ToArray();
            var oldPaths = items.Select(item => item.Path).ToArray();
            var oldCategoryPaths = items.Select(item => item.Category.Path).ToArray();
            this.Container.InvokeUserMove(authentication, this, categoryPath);
            base.Move(authentication, categoryPath);
            this.Container.InvokeUsersMovedEvent(authentication, items, oldPaths, oldCategoryPaths);
        }

        public void Delete(Authentication authentication)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Delete), this);
            var result = this.Service.DeleteUserItem(this.Path);
            this.Sign(authentication, result);
            var items = EnumerableUtility.One(this).ToArray();
            var oldPaths = items.Select(item => item.Path).ToArray();
            var container = this.Container;
            container.InvokeUserDelete(authentication, this);
            base.Delete(authentication);
            container.InvokeUsersDeletedEvent(authentication, items, oldPaths);
        }

        public void Kick(Authentication authentication, string comment)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Kick), this, comment);
            var result = this.Service.Kick(this.ID, comment ?? string.Empty);
            this.Sign(authentication, result);
            var users = new User[] { this };
            var comments = Enumerable.Repeat(comment, users.Length).ToArray();
            this.Container.InvokeUserKick(authentication, this, comment);
            var authenticationInfos = users.SelectMany(user => user.Authentications.Select(auth => auth.Value.Authentication.AuthenticationInfo)).ToArray();
            this.Authentications.Clear();
            this.Container.InvokeUsersKickedEvent(authentication, users, comments);
            this.Container.InvokeUsersStateChangedEvent(authentication, users);
            this.Container.InvokeUsersLoggedOutEvent(authentication, authenticationInfos, new CloseInfo(CloseReason.Kicked, comment));
        }

        public void KickAuthentication(Authentication authentication, Guid userToken, string comment)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Kick), this, comment);
            var result = this.Service.KickAuthentication(this.ID, userToken, comment ?? string.Empty);
            this.Sign(authentication, result);
            var users = new User[] { this };
            var userAuthentications = new IUserAuthentication[] { this.Authentications[userToken] };
            var comments = Enumerable.Repeat(comment, users.Length).ToArray();
            for(var i=0; i < userAuthentications.Length; i++)
            {
                this.Container.InvokeUserAuthenticationsKick(authentication, userAuthentications[i], comments[i]);
            }
            var authenticationInfos = users.SelectMany(user => user.Authentications.Select(auth => auth.Value.Authentication.AuthenticationInfo)).ToArray();
            this.Authentications.Remove(userToken);

            this.Container.InvokeUserAuthenticationsKickedEvent(authentication, userAuthentications, comments);
            this.Container.InvokeUsersStateChangedEvent(authentication, users);
            this.Container.InvokeUsersLoggedOutEvent(authentication, authenticationInfos, new CloseInfo(CloseReason.Kicked, comment));
        }

        public void Ban(Authentication authentication, string comment)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Ban), this, comment);
            var result = this.Service.Ban(this.ID, comment ?? string.Empty);
            this.Sign(authentication, result);
            var users = new User[] { this };
            var comments = Enumerable.Repeat(comment, users.Length).ToArray();
            this.Container.InvokeUserBan(authentication, this, result.Value);
            base.Ban(authentication, result.Value);
            this.Container.InvokeUsersBannedEvent(authentication, users, comments);
            if (this.IsOnline == true)
            {
                var authenticationInfos = users.SelectMany(user => user.Authentications.Select(auth => auth.Value.Authentication.AuthenticationInfo)).ToArray();
                this.Authentications.Clear();
                this.Container.InvokeUsersStateChangedEvent(authentication, users);
                this.Container.InvokeUsersLoggedOutEvent(authentication, authenticationInfos, new CloseInfo(CloseReason.Banned, comment));
            }
        }

        public void Unban(Authentication authentication)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Unban), this);
            var result = this.Service.Unban(this.ID);
            this.Sign(authentication, result);
            var users = new User[] { this };
            this.Container.InvokeUserUnban(authentication, this);            
            base.Unban(authentication);
            this.Container.InvokeUsersUnbannedEvent(authentication, users);
            this.Container.InvokeUsersStateChangedEvent(authentication, users);
        }

        public void ChangeUserInfo(Authentication authentication, SecureString password, SecureString newPassword, string userName, Authority? authority, bool? allowMultiLogin)
        {
            this.Dispatcher.VerifyAccess();

            if (this.ID == authentication.ID && password == null && newPassword != null)
                throw new ArgumentNullException(nameof(password));
            if (newPassword == null && password != null)
                throw new ArgumentNullException(nameof(newPassword));

            var p1 = password == null ? null : UserContext.Encrypt(this.ID, password);
            var p2 = newPassword == null ? null : UserContext.Encrypt(this.ID, newPassword);
            var result = this.Service.ChangeUserInfo(this.UserInfo.ID, p1, p2, userName, authority, allowMultiLogin);
            this.Sign(authentication, result);
            this.Container.InvokeUserChange(authentication, this);
            base.UpdateUserInfo(result.Value);
            this.Container.InvokeUsersChangedEvent(authentication, new User[] { this });
        }

        public void SendMessage(Authentication authentication, string message)
        {
            this.Dispatcher.VerifyAccess();
            var result = this.Service.SendMessage(this.UserInfo.ID, message);
            this.Sign(authentication, result);
            this.Container.InvokeSendMessageEvent(authentication, this, message);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetUserInfo(UserInfo userInfo)
        {
            this.UpdateUserInfo(userInfo);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetBanInfo(BanChangeType changeType, BanInfo banInfo)
        {
            if (changeType == BanChangeType.Ban)
                base.BanInfo = banInfo;
            else
                base.BanInfo = BanInfo.Empty;
        }

        public string ID
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Name;
            }
        }

        public string UserName
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.UserInfo.Name;
            }
        }

        public new string Path
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.Path;
            }
        }

        public new Authority Authority
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.Authority;
            }
        }

        protected override void InitializeUserInfo(UserInfo userInfo)
        {
            if (userInfo.AuthenticationInfos == null) return;

            foreach (var auth in userInfo.AuthenticationInfos)
            {
                if (this.Authentications.ContainsKey(auth.Token)) continue;

                var authentication = new Authentication(new AuthenticationProvider(this), auth.Token);
                this.Authentications.Add(auth.Token, new UserAuthentication(this, authentication));
            }
        }

        public new UserInfo UserInfo
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.UserInfo;
            }
        }

        public new BanInfo BanInfo
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.BanInfo;
            }
        }

        public bool IsBanned
        {
            get { return this.BanInfo.Path != string.Empty; }
        }

        public IUserService Service
        {
            get { return this.Context.Service; }
        }

        public CremaHost CremaHost
        {
            get { return this.Context.CremaHost; }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.CremaHost.Dispatcher; }
        }

        public new event EventHandler Renamed
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                base.Renamed += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                base.Renamed -= value;
            }
        }

        public new event EventHandler Moved
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                base.Moved += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                base.Moved -= value;
            }
        }

        public new event EventHandler Deleted
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                base.Deleted += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                base.Deleted -= value;
            }
        }

        public new event EventHandler UserInfoChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                base.UserInfoChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                base.UserInfoChanged -= value;
            }
        }

        public new event EventHandler UserStateChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                base.UserStateChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                base.UserStateChanged -= value;
            }
        }

        public new event EventHandler UserBanInfoChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                base.UserBanInfoChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                base.UserBanInfoChanged -= value;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
        }

        private void Sign(Authentication authentication, ResultBase result)
        {
            result.Validate(authentication);
        }

        private void Sign<T>(Authentication authentication, ResultBase<T> result)
        {
            result.Validate(authentication);
        }

        #region IUser

        string IUser.ID
        {
            get { return this.ID; }
        }

        IUserCategory IUser.Category
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Category;
            }
        }

        #endregion

        #region IUserItem

        string IUserItem.Name
        {
            get { return this.ID; }
        }

        IUserItem IUserItem.Parent
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Category;
            }
        }

        IEnumerable<IUserItem> IUserItem.Childs
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return Enumerable.Empty<IUserItem>();
            }
        }

        #endregion

        #region IServiceProvider

        object IServiceProvider.GetService(System.Type serviceType)
        {
            return (this.Context as IUserContext).GetService(serviceType);
        }

        #endregion

        #region IInfoProvider

        IDictionary<string, object> IInfoProvider.Info => this.UserInfo.ToDictionary();

        #endregion

        #region IStateProvider

        object IStateProvider.State => this.UserState;

        #endregion
    }
}
