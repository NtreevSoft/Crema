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
using Ntreev.Library;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Services.Properties;
using Ntreev.Crema.Services;
using System.Security;
using System.Runtime.InteropServices;
using Ntreev.Library.Linq;
using System.Data;

namespace Ntreev.Crema.Services.Users
{
    class User : UserBase<User, UserCategory, UserCollection, UserCategoryCollection, UserContext>,
        IUser, IUserItem, IInfoProvider, IStateProvider
    {
        private Authentication authentication;
        private SecureString password;

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
            this.ValidateMove(authentication, categoryPath);
            this.Sign(authentication);
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
            this.ValidateDelete(authentication);
            this.Sign(authentication);
            var items = EnumerableUtility.One(this).ToArray();
            var oldPaths = items.Select(item => item.Path).ToArray();
            var container = this.Container;
            container.InvokeUserDelete(authentication, this);
            base.Delete(authentication);
            container.InvokeUsersDeletedEvent(authentication, items, oldPaths);
        }

        public Authentication Login(SecureString password)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(Authentication.System, this, nameof(Login), this);
            this.ValidateLogin(password);

            var users = new User[] { this };
            var authentication = new Authentication(new UserAuthenticationProvider(this), Guid.NewGuid());
            this.Sign(authentication);

            if (this.Authentication != null)
            {
                var message = "다른 기기에서 동일한 아이디로 접속하였습니다.";
                var closeInfo = new CloseInfo() { Reason = CloseReason.Reconnected, Message = message };
                this.Authentication.InvokeExpiredEvent(this.ID, message);
                this.Container.InvokeUsersLoggedOutEvent(this.Authentication, users, closeInfo);
            }

            this.Authentication = authentication;
            this.IsOnline = true;
            this.Container.InvokeUsersStateChangedEvent(this.Authentication, users);
            this.Container.InvokeUsersLoggedInEvent(this.Authentication, users);
            return this.Authentication;
        }

        public void Logout(Authentication authentication)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Logout), this);
            this.ValidateLogout(authentication);
            this.Sign(authentication);
            var users = new User[] { this };
            this.Authentication.InvokeExpiredEvent(authentication.ID, string.Empty);
            this.Authentication = null;
            this.IsOnline = false;
            this.Container.InvokeUsersStateChangedEvent(authentication, users);
            this.Container.InvokeUsersLoggedOutEvent(authentication, users, CloseInfo.Empty);
        }

        public void Ban(Authentication authentication, string comment)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Ban), this, comment);
            this.ValidateBan(authentication, comment);
            this.Sign(authentication);
            var users = new User[] { this };
            var comments = Enumerable.Repeat(comment, users.Length).ToArray();
            var banInfo = new BanInfo() { Path = this.Path, Comment = comment, SignatureDate = authentication.SignatureDate };
            var isOnline = this.IsOnline;
            this.Container.InvokeUserBan(authentication, this, banInfo);
            base.Ban(authentication, banInfo);
            this.IsOnline = false;
            this.Container.InvokeUsersBannedEvent(authentication, users, comments);
            if (isOnline == true)
            {
                this.Authentication.InvokeExpiredEvent(authentication.ID, comment);
                this.Authentication = null;
                this.Container.InvokeUsersStateChangedEvent(authentication, users);
                this.Container.InvokeUsersLoggedOutEvent(authentication, users, new CloseInfo(CloseReason.Banned, comment));
            }
        }

        public void Unban(Authentication authentication)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Unban), this);
            this.ValidateUnban(authentication);
            this.Sign(authentication);
            var banInfo = BanInfo.Empty;
            this.Container.InvokeUserUnban(authentication, this);
            base.Unban(authentication);
            this.Container.InvokeUsersUnbannedEvent(authentication, new User[] { this });
        }

        public void Kick(Authentication authentication, string comment)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Kick), this, comment);
            this.ValidateKick(authentication, comment);
            this.Sign(authentication);
            var users = new User[] { this };
            var comments = Enumerable.Repeat(comment, users.Length).ToArray();
            this.Container.InvokeUserKick(authentication, this, comment);
            this.IsOnline = false;
            this.Authentication.InvokeExpiredEvent(authentication.ID, comment);
            this.Authentication = null;
            this.Container.InvokeUsersKickedEvent(authentication, users, comments);
            this.Container.InvokeUsersStateChangedEvent(authentication, users);
            this.Container.InvokeUsersLoggedOutEvent(authentication, users, new CloseInfo(CloseReason.Kicked, comment));
        }

        public void ChangeUserInfo(Authentication authentication, SecureString password, SecureString newPassword, string userName, Authority? authority)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Delete), this, password, newPassword, userName, authority);
            this.ValidateUserInfoChange(authentication, password, newPassword, userName, authority);
            this.Sign(authentication);
            var serializationInfo = this.SerializationInfo;
            if (newPassword != null)
                serializationInfo.Password = UserContext.SecureStringToString(newPassword).Encrypt();
            if (userName != null)
                serializationInfo.Name = userName;
            if (authority.HasValue)
                serializationInfo.Authority = authority.Value;
            if (object.Equals(serializationInfo, this.SerializationInfo) == true)
                return;
            var items = EnumerableUtility.One(this).ToArray();
            serializationInfo.ModificationInfo = new SignatureDate(authentication.ID);
            this.Container.InvokeUserChange(authentication, this, serializationInfo);
            if (newPassword != null)
                this.password = UserContext.StringToSecureString(serializationInfo.Password);
            base.UpdateUserInfo((UserInfo)serializationInfo);
            this.Container.InvokeUsersChangedEvent(authentication, items);
        }

        public void SendMessage(Authentication authentication, string message)
        {
            this.Dispatcher.VerifyAccess();
            this.ValidateSendMessage(authentication, message);
            this.Sign(authentication);
            this.Container.InvokeSendMessageEvent(authentication, this, message);
        }

        public bool VerifyPassword(SecureString password)
        {
            return UserContext.SecureStringToString(this.password) == UserContext.SecureStringToString(password).Encrypt();
        }

        public Authentication Authentication
        {
            get { return authentication; }
            set { this.authentication = value; }
        }

        public string ID
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.Name;
            }
        }

        public string UserName
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.UserInfo.Name;
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

        public new UserInfo UserInfo
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.UserInfo;
            }
        }

        public new UserState UserState
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.UserState;
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

        public SecureString Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        public UserSerializationInfo SerializationInfo
        {
            get
            {
                var userInfo = (UserSerializationInfo)base.UserInfo;
                userInfo.Password = UserContext.SecureStringToString(this.password);
                userInfo.BanInfo = (BanSerializationInfo)base.BanInfo;
                return userInfo;
            }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.Context?.Dispatcher; }
        }

        public CremaHost CremaHost
        {
            get { return this.Context.CremaHost; }
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

        protected override void OnDeleted(EventArgs e)
        {
            if (this.IsOnline == true)
            {
                throw new InvalidOperationException(Resources.Exception_UserIDIsConnecting);
            }

            base.OnDeleted(e);
        }

        private void ValidateUpdate(Authentication authentication)
        {
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
            {
                throw new PermissionDeniedException();
            }
        }

        private void ValidateUserInfoChange(Authentication authentication, SecureString password, SecureString newPassword, string userName, Authority? authority)
        {
            if (this.ID != authentication.ID)
            {
                var isAdmin = authentication.Types.HasFlag(AuthenticationType.Administrator);

                if (base.UserInfo.ID == Authentication.AdminID)
                    throw new CremaException("관리자의 계정 정보는 관리자만 변경할 수 있습니다.");

                if (authority.HasValue == true && this.IsOnline == true)
                    throw new CremaException("로그인된 사용자의 권한을 변경할 수 없습니다..");

                if (newPassword != null)
                {
                    if (isAdmin == false)
                    {
                        if (this.VerifyPassword(password) == false)
                            throw new ArgumentException(Resources.Exception_IncorrectPassword, nameof(password));
                        if (this.VerifyPassword(newPassword) == true)
                            throw new ArgumentException(Resources.Exception_CannotChangeToOldPassword, nameof(newPassword));
                    }
                    else
                    {
                        if (this.VerifyPassword(newPassword) == true)
                            throw new ArgumentException(Resources.Exception_CannotChangeToOldPassword, nameof(newPassword));
                    }
                }
            }
            else
            {
                if (newPassword != null)
                {
                    if (newPassword == null || this.VerifyPassword(password) == false)
                        throw new ArgumentException(Resources.Exception_IncorrectPassword, nameof(password));
                    if (this.VerifyPassword(newPassword) == true)
                        throw new ArgumentException(Resources.Exception_CannotChangeToOldPassword, nameof(newPassword));
                }
                if (authority.HasValue == true)
                    throw new CremaException("자신의 권한을 변경할 수 없습니다.");
            }
        }

        private void ValidateSendMessage(Authentication authentication, string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (message == string.Empty)
                throw new ArgumentException(Resources.Exception_EmptyStringCannotSend, nameof(message));
            if (this.IsOnline == false)
                throw new InvalidOperationException(Resources.Exception_CannotSendMessageToOfflineUser);
        }

        private void ValidateMove(Authentication authentication, string categoryPath)
        {
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();

            if (this.Authentication != authentication)
            {
                if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                {
                    throw new PermissionDeniedException();
                }
            }

            base.ValidateMove(authentication, categoryPath);
        }

        private void ValidateDelete(Authentication authentication)
        {
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();

            if (this.IsOnline == true)
                throw new InvalidOperationException(Resources.Exception_LoggedInUserCannotDelete);

            if (this.ID == authentication.ID)
                throw new InvalidOperationException("자기 자신을 삭제할 수 없습니다.");

            if (base.UserInfo.ID == Authentication.AdminID)
                throw new InvalidOperationException("관리자 계정은 삭제할 수 없습니다..");

            base.ValidateDelete();
        }

        private void ValidateLogin(SecureString password)
        {
            if (this.IsBanned == true)
                throw new CremaException("해당 사용자는 차단되어 있어 접속할 수 없습니다.");
        }

        private void ValidateLogout(Authentication authentication)
        {
            if (this.IsOnline == false)
                throw new CremaException("로그인 되어 있지 않은 사용자입니다.");
            if (authentication.ID != this.ID)
                throw new PermissionDeniedException();
        }

        private void ValidateBan(Authentication authentication, string comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));
            if (comment == string.Empty)
                throw new ArgumentNullException(nameof(comment), "빈 문자열은 사용할 수 없습니다.");
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();
            if (this.IsBanned == true)
                throw new CremaException("이미 차단되어 있는 사용자입니다.");
            if (this.Authority == Authority.Admin)
                throw new PermissionDeniedException("관리자는 차단할 수 없습니다.");
            if (authentication.ID == this.ID)
                throw new PermissionDeniedException("자기 자신을 차단할 수 없습니다.");
        }

        private void ValidateUnban(Authentication authentication)
        {
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();
            if (this.IsBanned == false)
                throw new CremaException("차단되어 있지 않은 사용자입니다.");
        }

        private void ValidateKick(Authentication authentication, string comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));
            if (comment == string.Empty)
                throw new ArgumentNullException(nameof(comment), Resources.Exception_EmptyStringIsNotAllowed);
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();
            if (this.IsOnline == false)
                throw new CremaException("접속 되지 않은 사용자는 추방할 수 없습니다.");
            if (authentication.ID == this.ID)
                throw new PermissionDeniedException("자기 자신을 추방할 수 없습니다.");
        }

        private void Sign(Authentication authentication)
        {
            authentication.Sign();
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
