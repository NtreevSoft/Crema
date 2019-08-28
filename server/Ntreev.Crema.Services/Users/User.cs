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
        private SecureString password;

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

        public void Rename(Authentication authentication, string newName)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                throw new NotSupportedException();
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Move(Authentication authentication, string categoryPath)
        {
            try
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
            catch (Exception e)
            {

                throw;
            }
        }

        public void Delete(Authentication authentication)
        {
            try
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
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public Authentication Login(SecureString password)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(Authentication.System, this, nameof(Login), this);
                this.ValidateLogin(password);

                if (!this.UserInfo.AllowMultiLogin && this.Authentications.Any())
                {
                    var message = "다른 기기에서 동일한 아이디로 접속하였습니다.";
                    var closeInfo = new CloseInfo() { Reason = CloseReason.Reconnected, Message = message };
                    foreach (var auth in this.Authentications.Values.ToArray())
                    {
                        auth.Authentication.InvokeExpiredEvent(this.ID, message);
                        this.Container.InvokeUsersLoggedOutEvent(auth.Authentication, new[] { auth.Authentication.AuthenticationInfo }, closeInfo);
                    }
                    this.Authentications.Clear();

                }

                var authenticationToken = GetAuthenticationToken();

                var authentication = new Authentication(new UserAuthenticationProvider(this), authenticationToken);
                this.Sign(authentication);

                this.Authentications.Add(authentication.Token, new UserAuthentication(this, authentication));
                var authenticationInfos = new[] { authentication.AuthenticationInfo };
                this.Container.InvokeUsersLoggedInEvent(authentication, authenticationInfos);
                this.Container.InvokeUsersStateChangedEvent(authentication, new IUser[] { this });
                return authentication;
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        private Guid GetAuthenticationToken()
        {
            var domainContext = this.CremaHost.GetService(typeof(IDomainContext)) as IDomainContext;
            var userID = this.ID;
            var authenticationToken = domainContext.Dispatcher.Invoke(() =>
            {
                var domainContextMetaData = domainContext.GetMetaData(Authentication.System);
                var containsDomain = domainContextMetaData.Domains.Any(metaData =>
                {
                    return metaData.DomainInfo.ItemType != "TableContent" && metaData.Users.Any(user => user.DomainUserInfo.UserID == userID);
                });
                if (containsDomain)
                {
                    foreach (var domainMetaData in domainContextMetaData.Domains)
                    {
                        if (domainMetaData.Users.Any(user => user.DomainUserInfo.UserID == userID) == false) continue;

                        var domainUserInfo = domainMetaData.Users.First(user => user.DomainUserInfo.UserID == userID);
                        var token = domainUserInfo.DomainUserInfo.Token;

                        return this.Authentications.ContainsKey(token) ? Guid.NewGuid() : token;
                    }
                }

                return Guid.NewGuid();
            });
            return authenticationToken;
        }

        public void Logout(Authentication authentication)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(Logout), this);
                this.ValidateLogout(authentication);
                this.Sign(authentication);
                var auth = this.Authentications[authentication.Token];
                auth.Authentication.InvokeExpiredEvent(authentication.ID, string.Empty);
                this.Authentications.Remove(auth.Authentication.Token);
                var authenticationInfos = new[] {auth.Authentication.AuthenticationInfo};
                this.Container.InvokeUsersStateChangedEvent(authentication, new IUser[]{this});
                this.Container.InvokeUsersLoggedOutEvent(authentication, authenticationInfos, CloseInfo.Empty);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Ban(Authentication authentication, string comment)
        {
            try
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
                this.Container.InvokeUsersBannedEvent(authentication, users, comments);
                if (isOnline == true)
                {
                    foreach (var auth in this.Authentications.Values.ToArray())
                    {
                        auth.Authentication.InvokeExpiredEvent(authentication.ID, comment);
                    }

                    var authenticationInfos = users.SelectMany(user => user.Authentications.Select(auth => auth.Value.Authentication.AuthenticationInfo)).ToArray();
                    this.Authentications.Clear();
                    this.Container.InvokeUsersStateChangedEvent(authentication, users);
                    this.Container.InvokeUsersLoggedOutEvent(authentication, authenticationInfos, new CloseInfo(CloseReason.Banned, comment));
                }
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Unban(Authentication authentication)
        {
            try
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
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Kick(Authentication authentication, string comment)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(Kick), this, comment);
                this.ValidateKick(authentication, comment);
                this.Sign(authentication);
                var users = new User[] { this };
                var comments = Enumerable.Repeat(comment, users.Length).ToArray();
                this.Container.InvokeUserKick(authentication, this, comment);
                foreach (var auth in this.Authentications.Values.ToArray())
                {
                    if (auth.Authentication.Token == authentication.Token) continue;

                    auth.Authentication.InvokeExpiredEvent(authentication.ID, comment);
                }
                var authenticationInfos = users.SelectMany(user => user.Authentications.Select(auth => auth.Value.Authentication.AuthenticationInfo)).ToArray();
                this.Authentications.Clear();

                this.Container.InvokeUsersKickedEvent(authentication, users, comments);
                this.Container.InvokeUsersStateChangedEvent(authentication, users);
                this.Container.InvokeUsersLoggedOutEvent(authentication, authenticationInfos, new CloseInfo(CloseReason.Kicked, comment));
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void KickAuthentication(Authentication authentication, Guid userToken, string comment)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(Kick), this, comment);
                this.ValidateKickUserAuthentication(authentication, comment);
                this.Sign(authentication);
                var users = new User[] { this };
                var userAuthentications = new IUserAuthentication[] { this.Authentications[userToken] };
                var comments = Enumerable.Repeat(comment, userAuthentications.Length).ToArray();
                for(var i=0; i < userAuthentications.Length; i++)
                {
                    this.Container.InvokeUserKickAuthentication(authentication, userAuthentications[i], comments[i]);
                }
                foreach (var auth in this.Authentications.Values.ToArray())
                {
                    if (auth.Authentication.Token == authentication.Token) continue;

                    auth.Authentication.InvokeExpiredEvent(authentication.ID, comment);
                }
                var authenticationInfos = userAuthentications.Select(user => user.Authentication.AuthenticationInfo).ToArray();
                this.Authentications.Remove(userToken);

                this.Container.InvokeUserAuthenticationsKickedEvent(authentication, userAuthentications, comments);
                this.Container.InvokeUsersStateChangedEvent(authentication, users);
                this.Container.InvokeUsersLoggedOutEvent(authentication, authenticationInfos, new CloseInfo(CloseReason.Kicked, comment));
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void ChangeUserInfo(Authentication authentication, SecureString password, SecureString newPassword, string userName, Authority? authority, bool? allowMultiLogin)
        {
            try
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
                if (allowMultiLogin != null)
                    serializationInfo.AllowMultiLogin = allowMultiLogin.Value;
                if (object.Equals(serializationInfo, this.SerializationInfo) == true)
                    return;
                var items = EnumerableUtility.One(this).ToArray();
                serializationInfo.ModificationInfo = new SignatureDate(authentication.ID, authentication.Token);
                this.Container.InvokeUserChange(authentication, this, serializationInfo);
                if (newPassword != null)
                    this.password = UserContext.StringToSecureString(serializationInfo.Password);
                base.UpdateUserInfo((UserInfo)serializationInfo);
                this.Container.InvokeUsersChangedEvent(authentication, items);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SendMessage(Authentication authentication, string message)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.ValidateSendMessage(authentication, message);
                this.Sign(authentication);
                this.Container.InvokeSendMessageEvent(authentication, this, message);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public bool VerifyPassword(SecureString password)
        {
            return UserContext.SecureStringToString(this.password) == UserContext.SecureStringToString(password).Encrypt();
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

        protected override void InitializeUserInfo(UserInfo userInfo)
        {
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
                    throw new InvalidOperationException(Resources.Exception_AdminCanChangeAdminInfo);

                if (authority.HasValue == true && this.IsOnline == true)
                    throw new InvalidOperationException(Resources.Exception_OnlineUserAuthorityCannotChanged);

                if (newPassword != null)
                {
                    if (isAdmin == false)
                    {
                        if (this.VerifyPassword(password) == false)
                            throw new ArgumentException(Resources.Exception_IncorrectPassword, nameof(password));
                        //if (this.VerifyPassword(newPassword) == true)
                        //    throw new ArgumentException(Resources.Exception_CannotChangeToOldPassword, nameof(newPassword));
                    }
                    else
                    {
                        //if (this.VerifyPassword(newPassword) == true)
                        //    throw new ArgumentException(Resources.Exception_CannotChangeToOldPassword, nameof(newPassword));
                    }
                }
            }
            else
            {
                if (newPassword != null)
                {
                    if (newPassword == null || this.VerifyPassword(password) == false)
                        throw new ArgumentException(Resources.Exception_IncorrectPassword, nameof(password));
                    //if (this.VerifyPassword(newPassword) == true)
                    //    throw new ArgumentException(Resources.Exception_CannotChangeToOldPassword, nameof(newPassword));
                }
                if (authority.HasValue == true)
                    throw new ArgumentException(Resources.Exception_CannotChangeYourAuthority);
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

            if (this.Authentications.Values.All(o => o.Authentication != authentication))
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
                throw new InvalidOperationException(Resources.Exception_CannotDeleteYourself);

            if (base.UserInfo.ID == Authentication.AdminID)
                throw new InvalidOperationException(Resources.Exception_AdminCannotDeleted);

            base.ValidateDelete();
        }

        private void ValidateLogin(SecureString password)
        {
            if (this.IsBanned == true)
                throw new InvalidOperationException(Resources.Exception_BannedUserCannotLogin);
        }

        private void ValidateLogout(Authentication authentication)
        {
            if (this.IsOnline == false)
                throw new InvalidOperationException(Resources.Exception_UserIsNotLoggedIn);
            if (authentication.ID != this.ID)
                throw new PermissionDeniedException();
        }

        private void ValidateBan(Authentication authentication, string comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));
            if (comment == string.Empty)
                throw new ArgumentNullException(nameof(comment), Resources.Exception_EmptyStringIsNotAllowed);
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();
            if (this.IsBanned == true)
                throw new InvalidOperationException(Resources.Exception_UserIsAlreadyBanned);
            if (this.Authority == Authority.Admin)
                throw new PermissionDeniedException(Resources.Exception_AdminCannotBanned);
            if (authentication.ID == this.ID)
                throw new PermissionDeniedException(Resources.Exception_CannotBanYourself);
        }

        private void ValidateUnban(Authentication authentication)
        {
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();
            if (this.IsBanned == false)
                throw new InvalidOperationException(Resources.Exception_UserIsNotBanned);
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
                throw new InvalidOperationException(Resources.Exception_OfflineUserCannotKicked);
        }

        private void ValidateKickUserAuthentication(Authentication authentication, string comment)
        {
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
