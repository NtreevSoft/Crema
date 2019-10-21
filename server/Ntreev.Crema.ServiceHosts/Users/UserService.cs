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
using Ntreev.Library.ObjectModel;
using Ntreev.Library;
using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Security;
using System.Runtime.InteropServices;
using Ntreev.Crema.ServiceHosts.Properties;

namespace Ntreev.Crema.ServiceHosts.Users
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    class UserService : CremaServiceItemBase<IUserEventCallback>, IUserService, ICremaServiceItem
    {
        private readonly ICremaHost cremaHost;
        private readonly CremaService cremaService;
        private readonly ILogService logService;
        private readonly IUserContext userContext;

        private Authentication authentication;
        private Version clientVersion;

        public UserService(ICremaHost cremaHost, CremaService cremaService)
            : base(cremaHost.GetService(typeof(ILogService)) as ILogService)
        {
            this.cremaHost = cremaHost;
            this.cremaService = cremaService;
            this.logService = cremaHost.GetService(typeof(ILogService)) as ILogService;
            this.userContext = cremaHost.GetService(typeof(IUserContext)) as IUserContext;
            this.logService.Debug($"{nameof(UserService)} Constructor");
        }

        public ResultBase DefinitionType(SignatureDate p1)
        {
            return new ResultBase();
        }

        public ResultBase<UserContextMetaData> Subscribe(string userID, byte[] password, string version, string platformID, string culture)
        {
            var result = new ResultBase<UserContextMetaData>();
            try
            {
                result.Value = this.userContext.Dispatcher.Invoke(() =>
                {
                    var serverVersion = new Version(AppUtility.ProductVersion);
                    this.clientVersion = new Version(version);

                    if (this.clientVersion.Major < serverVersion.Major && this.clientVersion.Minor < serverVersion.Minor)
                        throw new ArgumentException(Resources.Exception_LowerVersion, nameof(version));

                    this.authentication = this.userContext.Login(userID, ToSecureString(userID, password));
                    this.authentication.AddRef(this, (a) =>
                    {
                        this.userContext.Dispatcher.Invoke(() => this.userContext.Logout(a));
                    });

                    this.OwnerID = this.authentication.ID;
                    this.AttachEventHandlers();
                    this.logService.Debug($"[{this.OwnerID}] {nameof(UserService)} {nameof(Subscribe)}");

                    return this.userContext.GetMetaData(this.authentication);
                });
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                this.OwnerID = $"{userID} - failed";
                result.Fault = new CremaFault(e);
            }
            return result;
        }

        private void AuthenticationUtility_Disconnected(object sender, EventArgs e)
        {
            var authentication = sender as Authentication;
            if (this.authentication != null && this.authentication == authentication)
            {
                this.userContext.Dispatcher.Invoke(() =>
                {
                    this.userContext.Logout(this.authentication);
                });
            }
        }

        public ResultBase Unsubscribe()
        {
            var result = new ResultBase();
            try
            {
                this.userContext.Dispatcher.Invoke(() =>
                {
                    this.DetachEventHandlers();
                    this.authentication.RemoveRef(this);
                    this.userContext.Logout(this.authentication);
                    this.authentication = null;
                    this.logService.Debug($"[{this.OwnerID}] {nameof(UserService)} {nameof(Unsubscribe)}");
                });
                result.SignatureDate = new SignatureDateProvider(this.OwnerID).Provide();
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault(e);
            }
            return result;
        }

        public ResultBase Shutdown(int milliseconds, ShutdownType shutdownType, string message)
        {
            var result = new ResultBase();
            try
            {
                this.cremaHost.Shutdown(this.authentication, milliseconds, shutdownType, message);
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault() { ExceptionType = e.GetType().Name, Message = e.Message };
            }
            return result;
        }

        public ResultBase CancelShutdown()
        {
            var result = new ResultBase();
            try
            {
                this.cremaHost.CancelShutdown(this.authentication);
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault() { ExceptionType = e.GetType().Name, Message = e.Message };
            }
            return result;
        }

        public ResultBase<UserInfo> NewUser(string userID, string categoryPath, byte[] password, string userName, Authority authority)
        {
            return this.Invoke(() =>
            {
                var category = this.GetCategory(categoryPath);
                var user = category.AddNewUser(this.authentication, userID, ToSecureString(userID, password), userName, authority);
                return user.UserInfo;
            });
        }

        public ResultBase NewUserCategory(string categoryPath)
        {
            return this.Invoke(() =>
            {
                var categoryName = new Ntreev.Library.ObjectModel.CategoryName(categoryPath);
                var category = this.GetCategory(categoryName.ParentPath);
                category.AddNewCategory(this.authentication, categoryName.Name);
            });
        }

        public ResultBase RenameUserItem(string itemPath, string newName)
        {
            return this.Invoke(() =>
            {
                var item = this.GetUserItem(itemPath);
                item.Rename(this.authentication, newName);
            });
        }

        public ResultBase MoveUserItem(string itemPath, string parentPath)
        {
            return this.Invoke(() =>
            {
                var item = this.GetUserItem(itemPath);
                item.Move(this.authentication, parentPath);
            });
        }

        public ResultBase DeleteUserItem(string itemPath)
        {
            return this.Invoke(() =>
            {
                var item = this.GetUserItem(itemPath);
                item.Delete(this.authentication);
            });
        }

        public ResultBase<UserInfo> ChangeUserInfo(string userID, byte[] password, byte[] newPassword, string userName, Authority? authority)
        {
            return this.Invoke(() =>
            {
                var user = this.GetUser(userID);
                var p1 = password == null ? null : ToSecureString(userID, password);
                var p2 = newPassword == null ? null : ToSecureString(userID, newPassword);
                user.ChangeUserInfo(this.authentication, p1, p2, userName, authority);
                return user.UserInfo;
            });
        }

        public ResultBase Kick(string userID, string comment)
        {
            return this.Invoke(() =>
            {
                var user = this.GetUser(userID);
                user.Kick(this.authentication, comment);
            });
        }

        public ResultBase<BanInfo> Ban(string userID, string comment)
        {
            return this.Invoke(() =>
            {
                var user = this.GetUser(userID);
                user.Ban(this.authentication, comment);
                return user.BanInfo;
            });
        }

        public ResultBase Unban(string userID)
        {
            return this.Invoke(() =>
            {
                var user = this.GetUser(userID);
                user.Unban(this.authentication);
            });
        }

        public ResultBase SendMessage(string userID, string message)
        {
            return this.Invoke(() =>
            {
                var user = this.GetUser(userID);
                user.SendMessage(this.authentication, message);
            });
        }

        public ResultBase NotifyMessage(string[] userIDs, string message)
        {
            return this.Invoke(() =>
            {
                this.userContext.NotifyMessage(this.authentication, userIDs, message);
            });
        }

        public ResultBase NotifyMessage2(string[] userIDs, string message, NotifyMessageType notifyMessageType)
        {
            return this.Invoke(() =>
            {
                this.userContext.NotifyMessage2(this.authentication, userIDs, message, notifyMessageType);
            });
        }

        public bool IsAlive()
        {
            if (this.authentication == null)
                return false;
            this.logService.Debug($"[{this.authentication}] {nameof(UserService)}.{nameof(IsAlive)} : {DateTime.Now}");
            this.authentication.Ping();
            return true;
        }

        protected override void OnDisposed(EventArgs e)
        {
            base.OnDisposed(e);
            this.userContext.Dispatcher.Invoke(() =>
            {
                if (this.authentication != null)
                {
                    this.DetachEventHandlers();
                    if (this.authentication.RemoveRef(this) == 0)
                    {
                        this.userContext.Logout(this.authentication);
                    }
                    this.authentication = null;
                }
            });
        }

        protected override void OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo)
        {
            this.Callback.OnServiceClosed(signatureDate, closeInfo);
        }

        private void AttachEventHandlers()
        {
            this.userContext.Dispatcher.VerifyAccess();

            this.userContext.Users.UsersStateChanged += Users_UsersStateChanged;
            this.userContext.Users.UsersChanged += Users_UsersChanged;
            this.userContext.ItemsCreated += UserContext_ItemsCreated;
            this.userContext.ItemsRenamed += UserContext_ItemsRenamed;
            this.userContext.ItemsMoved += UserContext_ItemsMoved;
            this.userContext.ItemsDeleted += UserContext_ItemsDeleted;
            this.userContext.UsersLoggedIn += UserContext_UsersLoggedIn;
            this.userContext.UsersLoggedOut += UserContext_UsersLoggedOut;
            this.userContext.UsersKicked += UserContext_UsersKicked;
            this.userContext.UsersBanChanged += UserContext_UsersBanChanged;
            this.userContext.MessageReceived += UserContext_MessageReceived;
            this.userContext.MessageReceived2 += UserContext_MessageReceived2;

            this.logService.Debug($"[{this.OwnerID}] {nameof(UserService)} {nameof(AttachEventHandlers)}");
        }

        private void DetachEventHandlers()
        {
            this.userContext.Dispatcher.VerifyAccess();

            this.userContext.Users.UsersStateChanged -= Users_UsersStateChanged;
            this.userContext.Users.UsersChanged -= Users_UsersChanged;
            this.userContext.ItemsCreated -= UserContext_ItemsCreated;
            this.userContext.ItemsRenamed -= UserContext_ItemsRenamed;
            this.userContext.ItemsMoved -= UserContext_ItemsMoved;
            this.userContext.ItemsDeleted -= UserContext_ItemsDeleted;
            this.userContext.UsersLoggedIn -= UserContext_UsersLoggedIn;
            this.userContext.UsersLoggedOut -= UserContext_UsersLoggedOut;
            this.userContext.UsersKicked -= UserContext_UsersKicked;
            this.userContext.UsersBanChanged -= UserContext_UsersBanChanged;
            this.userContext.MessageReceived -= UserContext_MessageReceived;
            this.userContext.MessageReceived2 -= UserContext_MessageReceived2;

            this.logService.Debug($"[{this.OwnerID}] {nameof(UserService)} {nameof(DetachEventHandlers)}");
        }

        private void Users_UsersStateChanged(object sender, Services.ItemsEventArgs<IUser> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var userIDs = e.Items.Select(item => item.ID).ToArray();
            var states = e.Items.Select(item => item.UserState).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnUsersStateChanged(e.SignatureDate, userIDs, states));
        }

        private void Users_UsersChanged(object sender, Services.ItemsEventArgs<IUser> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var values = e.Items.Select(item => item.UserInfo).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnUsersChanged(signatureDate, values));
        }

        private void UserContext_ItemsCreated(object sender, Services.ItemsCreatedEventArgs<IUserItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var itemPaths = e.Items.Select(item => item.Path).ToArray();
            var arguments = e.Arguments.Select(item => item is UserInfo userInfo ? (UserInfo?)userInfo : null).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnUserItemsCreated(signatureDate, itemPaths, arguments));
        }

        private void UserContext_ItemsRenamed(object sender, Services.ItemsRenamedEventArgs<IUserItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var oldPaths = e.OldPaths;
            var itemNames = e.Items.Select(item => item.Name).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnUserItemsRenamed(signatureDate, oldPaths, itemNames));
        }

        private void UserContext_ItemsMoved(object sender, Services.ItemsMovedEventArgs<IUserItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var oldPaths = e.OldPaths;
            var parentPaths = e.Items.Select(item => item.Parent.Path).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnUserItemsMoved(signatureDate, oldPaths, parentPaths));
        }

        private void UserContext_ItemsDeleted(object sender, Services.ItemsDeletedEventArgs<IUserItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var itemPaths = e.ItemPaths;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnUserItemsDeleted(signatureDate, itemPaths));
        }

        private void UserContext_MessageReceived(object sender, MessageEventArgs e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var userIDs = e.Items.Select(item => item.ID).ToArray();
            var message = e.Message;
            var messageType = e.MessageType;

            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnMessageReceived(signatureDate, userIDs, message, messageType));
        }

        private void UserContext_MessageReceived2(object sender, MessageEventArgs2 e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var userIDs = e.Items.Select(item => item.ID).ToArray();
            var message = e.Message;
            var messageType = e.MessageType;
            var notifyMessageType = e.NotifyMessageType;

            this.InvokeEvent(userID, exceptionUserID, () =>
            {
                if (notifyMessageType == NotifyMessageType.Toast && CremaFeatures.SupportsToastMessage(this.clientVersion))
                {
                    this.Callback.OnMessageReceived2(signatureDate, userIDs, message, messageType, notifyMessageType);
                }
                else
                {
                    this.Callback.OnMessageReceived(signatureDate, userIDs, message, messageType);
                }
            });
        }

        private void UserContext_UsersLoggedIn(object sender, Services.ItemsEventArgs<IUser> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var userIDs = e.Items.Select(item => item.ID).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnUsersLoggedIn(signatureDate, userIDs));
        }

        private void UserContext_UsersLoggedOut(object sender, Services.ItemsEventArgs<IUser> e)
        {
            var actionUserID = e.UserID;
            var contains = e.Items.Any(item => item.ID == this.authentication.ID);
            var closeInfo = (CloseInfo)e.MetaData;
            if (actionUserID != this.authentication.ID && contains == true)
            {
                var signatureDate = e.SignatureDate;
                this.DetachEventHandlers();
                this.InvokeEvent(null, null, () => this.Callback.OnServiceClosed(signatureDate, closeInfo));
            }
            else
            {
                var userID = this.authentication.ID;
                var exceptionUserID = e.UserID;
                var signatureDate = e.SignatureDate;
                var userIDs = e.Items.Select(item => item.ID).ToArray();
                this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnUsersLoggedOut(signatureDate, userIDs));
            }
        }

        private void UserContext_UsersKicked(object sender, ItemsEventArgs<IUser> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var userIDs = e.Items.Select(item => item.ID).ToArray();
            var comments = e.MetaData as string[];
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnUsersKicked(signatureDate, userIDs, comments));
        }

        private void UserContext_UsersBanChanged(object sender, ItemsEventArgs<IUser> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var values = new BanInfo[e.Items.Length];
            for (var i = 0; i < e.Items.Length; i++)
            {
                var item = e.Items[i];
                var lockInfo = item.BanInfo;
                if (item.BanInfo.Path != item.Path)
                {
                    lockInfo = BanInfo.Empty;
                    lockInfo.Path = item.Path;
                }
                values[i] = lockInfo;
            }
            var metaData = e.MetaData as object[];
            var changeType = (BanChangeType)metaData[0];
            var comments = metaData[1] as string[];
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnUsersBanChanged(signatureDate, values, changeType, comments));
        }

        private static SecureString ToSecureString(string userID, byte[] password)
        {
            var text = Encoding.UTF8.GetString(password);
            return StringToSecureString(StringUtility.Decrypt(text, userID));
        }

        private static SecureString StringToSecureString(string value)
        {
            var secureString = new SecureString();
            foreach (var item in value)
            {
                secureString.AppendChar(item);
            }
            return secureString;
        }

        private ResultBase<T> Invoke<T>(Func<T> func)
        {
            var result = new ResultBase<T>();
            try
            {
                result.Value = this.userContext.Dispatcher.Invoke(func);
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault() { ExceptionType = e.GetType().Name, Message = e.Message };
            }
            return result;
        }

        private ResultBase Invoke(Action action)
        {
            var result = new ResultBase();
            try
            {
                this.userContext.Dispatcher.Invoke(action);
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault() { ExceptionType = e.GetType().Name, Message = e.Message };
            }
            return result;
        }

        private IUserItem GetUserItem(string itemPath)
        {
            var item = this.userContext[itemPath];
            if (item == null)
                throw new ItemNotFoundException(itemPath);
            return item;
        }

        private IUser GetUser(string userID)
        {
            var user = this.userContext.Users[userID];
            if (user == null)
                throw new UserNotFoundException(userID);
            return user;
        }

        private IUserCategory GetCategory(string categoryPath)
        {
            var category = this.userContext.Categories[categoryPath];
            if (category == null)
                throw new CategoryNotFoundException(categoryPath);
            return category;
        }

        #region ICremaServiceItem

        void ICremaServiceItem.Abort(bool disconnect)
        {
            this.userContext.Dispatcher.Invoke(() =>
            {
                this.DetachEventHandlers();
                this.authentication = null;
            });

            CremaService.Dispatcher.Invoke(() =>
            {
                if (disconnect == false)
                {
                    this.Callback?.OnServiceClosed(SignatureDate.Empty, CloseInfo.Empty);
                    try
                    {
                        this.Channel?.Close(TimeSpan.FromSeconds(10));
                    }
                    catch
                    {
                        this.Channel?.Abort();
                    }
                }
                else
                {
                    this.Channel?.Abort();
                }
            });
        }

        #endregion
    }
}
