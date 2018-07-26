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
using Ntreev.Crema.Services;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Data.Xml;
using Ntreev.Crema.Data;
using System.Collections.Generic;
using Ntreev.Library.Linq;
using System.Collections.Specialized;
using Ntreev.Library;

namespace Ntreev.Crema.ServiceHosts.Data
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    class DataBaseCollectionService : CremaServiceItemBase<IDataBaseCollectionEventCallback>, IDataBaseCollectionService, ICremaServiceItem
    {
        private readonly ICremaHost cremaHost;
        private readonly ILogService logService;
        private readonly IDataBaseCollection dataBases;
        private readonly IUserContext userContext;

        private Authentication authentication;

        public DataBaseCollectionService(ICremaHost cremaHost)
            : base(cremaHost.GetService(typeof(ILogService)) as ILogService)
        {
            this.cremaHost = cremaHost;
            this.logService = cremaHost.GetService(typeof(ILogService)) as ILogService;
            this.dataBases = cremaHost.GetService(typeof(IDataBaseCollection)) as IDataBaseCollection;
            this.userContext = cremaHost.GetService(typeof(IUserContext)) as IUserContext;

            this.logService.Debug($"{nameof(DataBaseCollectionService)} Constructor");
        }

        public ResultBase DefinitionType(LogInfo[] param1)
        {
            return new ResultBase();
        }

        public ResultBase<DataBaseCollectionMetaData> Subscribe(Guid authenticationToken)
        {
            var result = new ResultBase<DataBaseCollectionMetaData>();
            try
            {
                this.userContext.Dispatcher.Invoke(() =>
                {
                    this.authentication = this.userContext.Authenticate(authenticationToken);
                    this.authentication.AddRef(this);
                    this.OwnerID = this.authentication.ID;
                    this.userContext.Users.UsersLoggedOut += Users_UsersLoggedOut;
                });
                result.Value = this.cremaHost.Dispatcher.Invoke(() =>
                {
                    this.AttachEventHandlers();
                    this.logService.Debug($"[{this.OwnerID}] {nameof(DataBaseCollectionService)} {nameof(Subscribe)}");
                    return this.cremaHost.DataBases.GetMetaData(this.authentication);
                });
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault(e);
            }
            return result;
        }

        public ResultBase Unsubscribe()
        {
            var result = new ResultBase();
            try
            {
                this.cremaHost.Dispatcher.Invoke(() =>
                {
                    this.DetachEventHandlers();
                });
                this.userContext.Dispatcher.Invoke(() =>
                {
                    this.userContext.Users.UsersLoggedOut -= Users_UsersLoggedOut;
                    this.authentication.RemoveRef(this);
                    this.authentication = null;
                });
                result.SignatureDate = new SignatureDateProvider(this.OwnerID).Provide();
                this.logService.Debug($"[{this.OwnerID}] {nameof(DataBaseCollectionService)} {nameof(Unsubscribe)}");
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault(e);
            }
            return result;
        }

        public ResultBase SetPublic(string dataBaseName)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                dataBase.SetPublic(this.authentication);
            });
        }

        public ResultBase<AccessInfo> SetPrivate(string dataBaseName)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                dataBase.SetPrivate(this.authentication);
                return dataBase.AccessInfo;
            });
        }

        public ResultBase<AccessMemberInfo> AddAccessMember(string dataBaseName, string memberID, AccessType accessType)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                dataBase.AddAccessMember(this.authentication, memberID, accessType);
                return dataBase.AccessInfo.Members.Where(item => item.UserID == memberID).First();
            });
        }

        public ResultBase<AccessMemberInfo> SetAccessMember(string dataBaseName, string memberID, AccessType accessType)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                dataBase.SetAccessMember(this.authentication, memberID, accessType);
                return dataBase.AccessInfo.Members.Where(item => item.UserID == memberID).First();
            });
        }

        public ResultBase RemoveAccessMember(string dataBaseName, string memberID)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                dataBase.RemoveAccessMember(this.authentication, memberID);
            });
        }

        public ResultBase<LockInfo> Lock(string dataBaseName, string comment)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                dataBase.Lock(this.authentication, comment);
                return dataBase.LockInfo;
            });
        }

        public ResultBase Unlock(string dataBaseName)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                dataBase.Unlock(this.authentication);
            });
        }

        public ResultBase Load(string dataBaseName)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                dataBase.Load(this.authentication);
            });
        }

        public ResultBase Unload(string dataBaseName)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                dataBase.Unload(this.authentication);
            });
        }

        public ResultBase<DataBaseInfo> Create(string dataBaseName, string comment)
        {
            return this.Invoke(() =>
            {
                var dataBase = this.cremaHost.DataBases.AddNewDataBase(this.authentication, dataBaseName, comment);
                return dataBase.DataBaseInfo;
            });
        }

        public ResultBase Rename(string dataBaseName, string newDataBaseName)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                dataBase.Rename(this.authentication, newDataBaseName);
            });
        }

        public ResultBase Delete(string dataBaseName)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                dataBase.Delete(this.authentication);
            });
        }

        public ResultBase<DataBaseInfo> Copy(string dataBaseName, string newDataBaseName, string comment, bool force)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                var newDataBase = dataBase.Copy(this.authentication, newDataBaseName, comment, force);
                return newDataBase.DataBaseInfo;
            });
        }

        public ResultBase<LogInfo[]> GetLog(string dataBaseName)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                return dataBase.GetLog(this.authentication);
            });
        }

        public ResultBase<DataBaseInfo> Revert(string dataBaseName, string revision)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                dataBase.Revert(this.authentication, revision);
                return dataBase.DataBaseInfo;
            });
        }

        public ResultBase BeginTransaction(string dataBaseName)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                var transaction = dataBase.BeginTransaction(this.authentication);
                dataBase.ExtendedProperties[typeof(ITransaction)] = transaction;
            });
        }

        public ResultBase EndTransaction(string dataBaseName)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                var transaction = dataBase.ExtendedProperties[typeof(ITransaction)] as ITransaction;
                transaction.Commit(this.authentication);
            });
        }

        public ResultBase CancelTransaction(string dataBaseName)
        {
            return this.Invoke(() =>
            {
                var dataBase = GetDataBase(dataBaseName);
                var transaction = dataBase.ExtendedProperties[typeof(ITransaction)] as ITransaction;
                transaction.Rollback(this.authentication);
            });
        }

        public bool IsAlive()
        {
            if (this.authentication == null)
                return false;
            this.logService.Debug($"[{this.authentication}] {nameof(DataBaseCollectionService)}.{nameof(IsAlive)} : {DateTime.Now}");
            this.authentication.Ping();
            return true;
        }

        protected override void OnDisposed(EventArgs e)
        {
            base.OnDisposed(e);
            this.cremaHost.Dispatcher.Invoke(() =>
            {
                if (this.authentication != null)
                {
                    this.DetachEventHandlers();
                }
            });
            this.userContext.Dispatcher.Invoke(() =>
            {
                this.userContext.Users.UsersLoggedOut -= Users_UsersLoggedOut;
                if (this.authentication != null)
                {
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

        private void Users_UsersLoggedOut(object sender, ItemsEventArgs<IUser> e)
        {
            var actionUserID = e.UserID;
            var contains = e.Items.Any(item => item.ID == this.authentication.ID);
            var closeInfo = (CloseInfo)e.MetaData;
            if (actionUserID != this.authentication.ID && contains == true)
            {
                this.InvokeEvent(null, null, () => this.Callback.OnServiceClosed(e.SignatureDate, closeInfo));
            }
        }

        private void DataBases_ItemsCreated(object sender, ItemsCreatedEventArgs<IDataBase> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var dataBaseNames = e.Items.Select(item => item.Name).ToArray();
            var dataBaseInfos = e.Arguments.Select(item => (DataBaseInfo)item).ToArray();
            var comment = e.MetaData as string;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDataBasesCreated(signatureDate, dataBaseNames, dataBaseInfos, comment));
        }

        private void DataBases_ItemsRenamed(object sender, ItemsRenamedEventArgs<IDataBase> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var oldNames = e.OldNames;
            var itemNames = e.Items.Select(item => item.Name).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDataBasesRenamed(signatureDate, oldNames, itemNames));
        }

        private void DataBases_ItemsDeleted(object sender, ItemsDeletedEventArgs<IDataBase> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var itemPaths = e.ItemPaths;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDataBasesDeleted(signatureDate, itemPaths));
        }

        private void DataBases_ItemsLoaded(object sender, ItemsEventArgs<IDataBase> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var itemNames = e.Items.Select(item => item.Name).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDataBasesLoaded(signatureDate, itemNames));
        }

        private void DataBases_ItemsUnloaded(object sender, ItemsEventArgs<IDataBase> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var itemNames = e.Items.Select(item => item.Name).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDataBasesUnloaded(signatureDate, itemNames));
        }

        private void DataBases_ItemsResetting(object sender, ItemsEventArgs<IDataBase> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var itemNames = e.Items.Select(item => item.Name).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDataBasesResetting(signatureDate, itemNames));
        }

        private void DataBases_ItemsReset(object sender, ItemsEventArgs<IDataBase> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var itemNames = e.Items.Select(item => item.Name).ToArray();
            var metaDatas = e.MetaData as DomainMetaData[];
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDataBasesReset(signatureDate, itemNames, metaDatas));
        }

        private void DataBases_ItemsAuthenticationEntered(object sender, ItemsEventArgs<IDataBase> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var itemNames = e.Items.Select(item => item.Name).ToArray();
            var authenticationInfo = (AuthenticationInfo)e.MetaData;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDataBasesAuthenticationEntered(signatureDate, itemNames, authenticationInfo));
        }

        private void DataBases_ItemsAuthenticationLeft(object sender, ItemsEventArgs<IDataBase> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var itemNames = e.Items.Select(item => item.Name).ToArray();
            var authenticationInfo = (AuthenticationInfo)e.MetaData;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDataBasesAuthenticationLeft(signatureDate, itemNames, authenticationInfo));
        }

        private void DataBases_ItemsInfoChanged(object sender, ItemsEventArgs<IDataBase> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var dataBaseInfos = e.Items.Select(item => item.DataBaseInfo).ToArray();

            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDataBasesInfoChanged(signatureDate, dataBaseInfos));
        }

        private void DataBases_ItemsStateChanged(object sender, ItemsEventArgs<IDataBase> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var itemNames = e.Items.Select(item => item.Name).ToArray();
            var dataBaseStates = e.Items.Select(item => item.DataBaseState).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDataBasesStateChanged(signatureDate, itemNames, dataBaseStates));
        }

        private void DataBases_ItemsAccessChanged(object sender, ItemsEventArgs<IDataBase> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var values = new AccessInfo[e.Items.Length];
            for (var i = 0; i < e.Items.Length; i++)
            {
                var item = e.Items[i];
                var accessInfo = item.AccessInfo;
                if (item.AccessInfo.Path != item.Name)
                {
                    accessInfo = AccessInfo.Empty;
                    accessInfo.Path = item.Name;
                }
                values[i] = accessInfo;
            }
            var metaData = e.MetaData as object[];
            var changeType = (AccessChangeType)metaData[0];
            var memberIDs = metaData[1] as string[];
            var accessTypes = metaData[2] as AccessType[];

            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDataBasesAccessChanged(signatureDate, changeType, values, memberIDs, accessTypes));
        }

        private void DataBases_ItemsLockChanged(object sender, ItemsEventArgs<IDataBase> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var values = new LockInfo[e.Items.Length];
            for (var i = 0; i < e.Items.Length; i++)
            {
                var item = e.Items[i];
                var lockInfo = item.LockInfo;
                if (item.LockInfo.Path != item.Name)
                {
                    lockInfo = LockInfo.Empty;
                    lockInfo.Path = item.Name;
                }
                values[i] = lockInfo;
            }
            var metaData = e.MetaData as object[];
            var changeType = (LockChangeType)metaData[0];
            var comments = metaData[1] as string[];

            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDataBasesLockChanged(signatureDate, changeType, values, comments));
        }

        private void AttachEventHandlers()
        {
            this.cremaHost.Dispatcher.VerifyAccess();

            this.cremaHost.DataBases.ItemsCreated += DataBases_ItemsCreated;
            this.cremaHost.DataBases.ItemsRenamed += DataBases_ItemsRenamed;
            this.cremaHost.DataBases.ItemsDeleted += DataBases_ItemsDeleted;
            this.cremaHost.DataBases.ItemsLoaded += DataBases_ItemsLoaded;
            this.cremaHost.DataBases.ItemsUnloaded += DataBases_ItemsUnloaded;
            this.cremaHost.DataBases.ItemsResetting += DataBases_ItemsResetting;
            this.cremaHost.DataBases.ItemsReset += DataBases_ItemsReset;
            this.cremaHost.DataBases.ItemsAuthenticationEntered += DataBases_ItemsAuthenticationEntered;
            this.cremaHost.DataBases.ItemsAuthenticationLeft += DataBases_ItemsAuthenticationLeft;
            this.cremaHost.DataBases.ItemsInfoChanged += DataBases_ItemsInfoChanged;
            this.cremaHost.DataBases.ItemsStateChanged += DataBases_ItemsStateChanged;
            this.cremaHost.DataBases.ItemsAccessChanged += DataBases_ItemsAccessChanged;
            this.cremaHost.DataBases.ItemsLockChanged += DataBases_ItemsLockChanged;
            this.logService.Debug($"[{this.OwnerID}] {nameof(DataBaseCollectionService)} {nameof(AttachEventHandlers)}");
        }
        
        private void DetachEventHandlers()
        {
            this.cremaHost.Dispatcher.VerifyAccess();

            this.cremaHost.DataBases.ItemsCreated -= DataBases_ItemsCreated;
            this.cremaHost.DataBases.ItemsRenamed -= DataBases_ItemsRenamed;
            this.cremaHost.DataBases.ItemsDeleted -= DataBases_ItemsDeleted;
            this.cremaHost.DataBases.ItemsLoaded -= DataBases_ItemsLoaded;
            this.cremaHost.DataBases.ItemsUnloaded -= DataBases_ItemsUnloaded;
            this.cremaHost.DataBases.ItemsResetting -= DataBases_ItemsResetting;
            this.cremaHost.DataBases.ItemsReset -= DataBases_ItemsReset;
            this.cremaHost.DataBases.ItemsAuthenticationEntered -= DataBases_ItemsAuthenticationEntered;
            this.cremaHost.DataBases.ItemsAuthenticationLeft -= DataBases_ItemsAuthenticationLeft;
            this.cremaHost.DataBases.ItemsInfoChanged -= DataBases_ItemsInfoChanged;
            this.cremaHost.DataBases.ItemsStateChanged -= DataBases_ItemsStateChanged;
            this.cremaHost.DataBases.ItemsAccessChanged -= DataBases_ItemsAccessChanged;
            this.cremaHost.DataBases.ItemsLockChanged -= DataBases_ItemsLockChanged;
            this.logService.Debug($"[{this.OwnerID}] {nameof(DataBaseCollectionService)} {nameof(DetachEventHandlers)}");
        }

        private ResultBase Invoke(Action action)
        {
            var result = new ResultBase();
            try
            {
                this.cremaHost.Dispatcher.Invoke(action);
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault(e);
            }
            return result;
        }

        private ResultBase<T> Invoke<T>(Func<T> func)
        {
            var result = new ResultBase<T>();
            try
            {
                result.Value = this.cremaHost.Dispatcher.Invoke(func);
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault(e);
            }
            return result;
        }

        private IDataBase GetDataBase(string dataBaseName)
        {
            var dataBase = this.cremaHost.DataBases[dataBaseName];
            if (dataBase == null)
                throw new DataBaseNotFoundException(dataBaseName);
            return dataBase;
        }

        #region ICremaServiceItem

        void ICremaServiceItem.Abort(bool disconnect)
        {
            this.cremaHost.Dispatcher.Invoke(() =>
            {
                this.DetachEventHandlers();
            });
            this.userContext.Dispatcher.Invoke(() =>
            {
                this.userContext.Users.UsersLoggedOut -= Users_UsersLoggedOut;
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
