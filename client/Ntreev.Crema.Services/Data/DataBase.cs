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

using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.DataBaseService;
using Ntreev.Crema.Services.Domains;
using Ntreev.Crema.Services.Properties;
using Ntreev.Crema.Services.Users;
using Ntreev.Library;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Timers;

namespace Ntreev.Crema.Services.Data
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    class DataBase : DataBaseBase<Type, TypeCategory, TypeCollection, TypeCategoryCollection, TypeContext, Table, TableCategory, TableCollection, TableCategoryCollection, TableContext>,
        IDataBaseServiceCallback, IDataBase, ICremaService, IInfoProvider, IStateProvider
    {
        private readonly CremaHost cremaHost;
        private readonly UserContext userContext;
        private CremaDispatcher dispatcher;

        private TableContext tableContext;
        private TypeContext typeContext;
        private DataBaseServiceClient service;
        private CremaDispatcher serviceDispatcher;
        private Timer timer;

        private EventHandler<AuthenticationEventArgs> authenticationEntered;
        private EventHandler<AuthenticationEventArgs> authenticationLeft;

        private readonly HashSet<AuthenticationToken> authentications = new HashSet<AuthenticationToken>();
        private bool isResetting;

        public DataBase(CremaHost cremaHost, DataBaseInfo dataBaseInfo)
        {
            this.cremaHost = cremaHost;
            this.dispatcher = cremaHost.Dispatcher;
            this.userContext = cremaHost.UserContext;
            base.Name = dataBaseInfo.Name;
            base.DataBaseInfo = dataBaseInfo;
        }

        public DataBase(CremaHost cremaHost, DataBaseMetaData metaData)
        {
            this.cremaHost = cremaHost;
            this.dispatcher = cremaHost.Dispatcher;
            this.userContext = cremaHost.UserContext;
            base.Name = metaData.DataBaseInfo.Name;
            base.DataBaseInfo = metaData.DataBaseInfo;
            base.DataBaseState = metaData.DataBaseState;
            base.AccessInfo = metaData.AccessInfo;
            base.LockInfo = metaData.LockInfo;

            foreach (var item in metaData.Authentications)
            {
                if (this.userContext.Authenticate(item) is Authentication authentication)
                {
                    this.authentications.Add(authentication);
                }
            }
        }

        public override string ToString()
        {
            return base.Name;
        }

        public AccessType GetAccessType(Authentication authentication)
        {
            return base.GetAccessType(authentication);
        }

        public void SetPublic(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(SetPublic), this);
            this.DataBases.InvokeDataBaseSetPublic(authentication, this);
            base.SetPublic(authentication);
            this.DataBases.InvokeItemsSetPublicEvent(authentication, new IDataBase[] { this });
        }

        public void SetPrivate(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(SetPrivate), this);
            this.DataBases.InvokeDataBaseSetPrivate(authentication, this);
            base.SetPrivate(authentication);
            this.DataBases.InvokeItemsSetPrivateEvent(authentication, new IDataBase[] { this });
        }

        public void AddAccessMember(Authentication authentication, string memberID, AccessType accessType)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(AddAccessMember), this, memberID, accessType);
            this.DataBases.InvokeDataBaseAddAccessMember(authentication, this, memberID, accessType);
            base.AddAccessMember(authentication, memberID, accessType);
            this.DataBases.InvokeItemsAddAccessMemberEvent(authentication, new IDataBase[] { this }, new string[] { memberID, }, new AccessType[] { accessType, });
        }

        public void SetAccessMember(Authentication authentication, string memberID, AccessType accessType)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(SetAccessMember), this, memberID, accessType);
            this.DataBases.InvokeDataBaseSetAccessMember(authentication, this, memberID, accessType);
            base.SetAccessMember(authentication, memberID, accessType);
            this.DataBases.InvokeItemsSetAccessMemberEvent(authentication, new IDataBase[] { this }, new string[] { memberID, }, new AccessType[] { accessType, });
        }

        public void RemoveAccessMember(Authentication authentication, string memberID)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(RemoveAccessMember), this, memberID);
            this.DataBases.InvokeDataBaseRemoveAccessMember(authentication, this, memberID);
            base.RemoveAccessMember(authentication, memberID);
            this.DataBases.InvokeItemsRemoveAccessMemberEvent(authentication, new IDataBase[] { this }, new string[] { memberID, });
        }

        public void Lock(Authentication authentication, string comment)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(Lock), this);
            this.DataBases.InvokeDataBaseLock(authentication, this, comment);
            base.Lock(authentication, comment);
            this.DataBases.InvokeItemsLockedEvent(authentication, new IDataBase[] { this }, new string[] { comment, });
        }

        public void Unlock(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(Unlock), this);
            this.DataBases.InvokeDataBaseUnlock(authentication, this);
            base.Unlock(authentication);
            this.DataBases.InvokeItemsUnlockedEvent(authentication, new IDataBase[] { this });
        }

        public void Load(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.DataBases.LoadDataBase(authentication, this);
        }

        public void Unload(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.DataBases.UnloadDataBase(authentication, this);
            this.authentications.Clear();
            this.tableContext?.Dispose();
            this.tableContext = null;
            this.typeContext?.Dispose();
            this.typeContext = null;
            base.DataBaseState = DataBaseState.None;
        }

        public void Enter(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.OnEnter(authentication);
        }

        public void Leave(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.OnLeave(authentication);
        }

        public void Rename(Authentication authentication, string name)
        {
            this.ValidateDispatcher();
            this.DataBases.RenameDataBase(authentication, this, name);
        }

        public void Delete(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.DataBases.DeleteDataBase(authentication, this);
        }

        public bool Contains(Authentication authentication)
        {
            return this.authentications.Contains(authentication);
        }

        public LogInfo[] GetLog(Authentication authentication)
        {
            this.ValidateDispatcher();
            return this.DataBases.GetLog(authentication, this);
        }

        public void Revert(Authentication authentication, long revision)
        {
            this.ValidateDispatcher();
            this.DataBases.Revert(authentication, this, revision);
        }

        public CremaDataSet GetDataSet(Authentication authentication, long revision)
        {
            this.ValidateAsyncBeginInDataBase(authentication);
            var result = this.Service.GetDataSet(revision);
            result.Validate(authentication);
            return result.Value;
        }

        public DataBaseTransaction BeginTransaction(Authentication authentication)
        {
            this.ValidateDispatcher();
            var result = this.DataBases.Service.BeginTransaction(this.Name);
            this.Sign(authentication, result);
            if (this.IsLocked == false)
            {
                base.Lock(authentication, $"{this.ID}");
                this.DataBases.InvokeItemsLockedEvent(authentication, new IDataBase[] { this }, new string[] { $"{this.ID}", });
            }
            var transaction = new DataBaseTransaction(authentication, this, this.DataBases.Service);
            transaction.Disposed += (s, e) =>
            {
                this.dispatcher.InvokeAsync(() =>
                {
                    if (this.LockInfo.Comment == $"{this.ID}" && this.IsLocked == true)
                    {
                        base.Unlock(authentication);
                        this.DataBases.InvokeItemsUnlockedEvent(authentication, new IDataBase[] { this });
                    }
                });
            };
            return transaction;
        }

        public void ValidateBeginInDataBase(Authentication authentication)
        {
            this.ValidateDispatcher();
            if (authentication != Authentication.System && this.authentications.Contains(authentication) == false)
                throw new InvalidOperationException(Resources.Exception_NotInDataBase);
        }

        public void ValidateAsyncBeginInDataBase(Authentication authentication)
        {
            if (this.Dispatcher == null)
                throw new InvalidOperationException(Resources.Exception_InvalidObject);
            this.Dispatcher.Invoke(() =>
            {
                if (authentication != Authentication.System && this.authentications.Contains(authentication) == false)
                    throw new InvalidOperationException(Resources.Exception_NotInDataBase);
            });
        }

        public bool VerifyAccess(Authentication authentication)
        {
            return this.authentications.Contains(authentication);
        }

        public DataBaseMetaData GetMetaData(Authentication authentication)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            this.ValidateDispatcher();

            var metaData = new DataBaseMetaData()
            {
                DataBaseInfo = this.DataBaseInfo,
                DataBaseState = this.DataBaseState,
                AccessInfo = this.AccessInfo,
                LockInfo = this.LockInfo,
                Authentications = this.AuthenticationInfos,
            };
            if (this.tableContext != null)
            {
                var query = from TableCategory item in this.tableContext.Categories
                            orderby item.Path
                            select item.MetaData;

                metaData.TableCategories = query.ToArray();
            }
            if (this.tableContext != null)
            {
                var query = from Table item in this.tableContext.Tables
                            orderby item.Path
                            orderby item.TemplatedParent != null
                            select item.MetaData;

                metaData.Tables = query.ToArray();
            }
            if (this.typeContext != null)
            {
                var query = from TypeCategory item in this.typeContext.Categories
                            orderby item.Path
                            select item.MetaData;

                metaData.TypeCategories = query.ToArray();
            }
            if (this.typeContext != null)
            {
                var query = from Type item in this.typeContext.Types
                            orderby item.Path
                            select item.MetaData;

                metaData.Types = query.ToArray();
            }
            return metaData;
        }

        public void SetLoaded(Authentication authentication)
        {
            base.DataBaseState = DataBaseState.IsLoaded;
            base.Load(authentication);
        }

        public void SetUnloaded(Authentication authentication)
        {
            if (this.serviceDispatcher != null)
            {
                this.DetachDomainHost();
                this.ReleaseService();
            }
            this.authentications.Clear();
            this.tableContext?.Dispose();
            this.tableContext = null;
            this.typeContext?.Dispose();
            this.typeContext = null;
            base.DataBaseState = DataBaseState.None;
            base.Unload(authentication);
        }

        public void SetResetting(Authentication authentication)
        {
            System.Diagnostics.Trace.WriteLine("Resetting");
            this.typeContext?.Dispose();
            this.tableContext?.Dispose();
            this.isResetting = true;
            base.ResettingDataBase(authentication);
            this.DataBases.InvokeItemsResettingEvent(authentication, new IDataBase[] { this, });

            if (this.serviceDispatcher != null)
            {
                this.DetachDomainHost();
            }

            if (this.GetService(typeof(DomainContext)) is DomainContext domainContext)
            {
                var domains = domainContext.Domains.Where<Domain>(item => item.DataBaseID == this.ID).ToArray();
                foreach (var item in domains)
                {
                    item.Dispatcher.Invoke(() => item.Dispose(authentication, true));
                }
            }
        }

        public void SetReset(Authentication authentication, DomainMetaData[] metaDatas)
        {
            var domains = metaDatas.Where(item => item.DomainInfo.DataBaseID == this.ID).ToArray();
            this.cremaHost.DomainContext.AddDomains(domains);
            if (this.serviceDispatcher != null)
            {
                var result = this.service.GetMetaData();
                this.typeContext = new TypeContext(this, result.Value);
                this.tableContext = new TableContext(this, result.Value);
                this.AttachDomainHost();
                base.UpdateLockParent();
                base.UpdateAccessParent();
            }
            
            this.isResetting = false;
            base.ResetDataBase(authentication);
            this.DataBases.InvokeItemsResetEvent(authentication, new IDataBase[] { this, });
            System.Diagnostics.Trace.WriteLine("Reset");
        }

        public void SetAuthenticationEntered(Authentication authentication)
        {
            this.authentications.Add(authentication);
            this.authenticationEntered?.Invoke(this, new AuthenticationEventArgs(authentication.AuthenticationInfo));
            this.DataBases.InvokeItemsAuthenticationEnteredEvent(authentication, new IDataBase[] { this });
        }

        public void SetAuthenticationLeft(Authentication authentication)
        {
            this.authentications.Remove(authentication);
            this.authenticationLeft?.Invoke(this, new AuthenticationEventArgs(authentication.AuthenticationInfo));
            this.DataBases.InvokeItemsAuthenticationLeftEvent(authentication, new IDataBase[] { this });
        }

        public void SetDataBaseInfo(DataBaseInfo dataBaseInfo)
        {
            base.DataBaseInfo = dataBaseInfo;
        }

        public void SetDataBaseState(DataBaseState dataBaseState)
        {
            base.DataBaseState = dataBaseState;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetAccessInfo(AccessChangeType changeType, AccessInfo accessInfo)
        {
            if (changeType != AccessChangeType.Public)
                base.AccessInfo = accessInfo;
            else
                base.AccessInfo = AccessInfo.Empty;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetAccessInfo(AccessInfo accessInfo)
        {
            base.AccessInfo = accessInfo;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetLockInfo(LockChangeType changeType, LockInfo lockInfo)
        {
            if (this.isResetting == true)
                return;
            if (changeType == LockChangeType.Lock)
                base.LockInfo = lockInfo;
            else
                base.LockInfo = LockInfo.Empty;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetLockInfo(LockInfo lockInfo)
        {
            base.LockInfo = lockInfo;
        }

        public void Invoke(Action action)
        {
            this.Dispatcher.Invoke(action);
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
                this.serviceDispatcher.Dispose();
                this.serviceDispatcher = null;
            });
        }

        public void Delete()
        {
            this.dispatcher = null;
            base.DataBaseState = DataBaseState.None;
            this.tableContext = null;
            this.typeContext = null;
            this.OnDeleted(EventArgs.Empty);
        }

        public IDomainHost FindDomainHost(Domain domain)
        {
            var itemPath = domain.DomainInfo.ItemPath;
            var itemType = domain.DomainInfo.ItemType;

            if (itemType == nameof(TableContent))
            {
                var table = this.tableContext[itemPath] as Table;
                return table.Content;
            }
            else if (itemType == nameof(NewTableTemplate))
            {
                var category = this.tableContext[itemPath] as TableCategory;
                return new NewTableTemplate(category);
            }
            else if (itemType == nameof(NewChildTableTemplate))
            {
                var table = this.tableContext[itemPath] as Table;
                return new NewChildTableTemplate(table);
            }
            else if (itemType == nameof(TableTemplate))
            {
                var table = this.tableContext[itemPath] as Table;
                return table.Template;
            }
            else if (itemType == nameof(NewTypeTemplate))
            {
                var category = this.typeContext[itemPath] as TypeCategory;
                return new NewTypeTemplate(category);
            }
            else if (itemType == nameof(TypeTemplate))
            {
                var type = this.typeContext[itemPath] as Type;
                return type.Template;
            }

            return null;
        }

        public object GetService(System.Type serviceType)
        {
            if (serviceType == typeof(TableContext))
                return this.tableContext;
            else if (serviceType == typeof(TableCategoryCollection))
                return this.tableContext.Categories;
            else if (serviceType == typeof(TableCollection))
                return this.tableContext.Tables;
            else if (serviceType == typeof(TypeContext))
                return this.typeContext;
            else if (serviceType == typeof(TypeCategoryCollection))
                return this.typeContext.Categories;
            else if (serviceType == typeof(TypeCollection))
                return this.typeContext.Types;
            else if (serviceType == typeof(DomainContext))
                return this.CremaHost.DomainContext;
            else
                return this.CremaHost.GetService(serviceType);
        }

        public CremaHost CremaHost
        {
            get { return this.cremaHost; }
        }

        public DataBaseCollection DataBases
        {
            get { return this.CremaHost.DataBases; }
        }

        public TableContext TableContext
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.tableContext;
            }
        }

        public TypeContext TypeContext
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.typeContext;
            }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.dispatcher; }
        }

        public IDataBaseService Service
        {
            get { return this.service; }
        }

        public new DataBaseInfo DataBaseInfo
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.DataBaseInfo;
            }
        }

        public new DataBaseState DataBaseState
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.DataBaseState;
            }
        }

        public AuthenticationInfo[] AuthenticationInfos
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.authentications.Select(item => ((Authentication)item).AuthenticationInfo).ToArray();
            }
        }

        public override TypeCategoryBase<Type, TypeCategory, TypeCollection, TypeCategoryCollection, TypeContext> TypeCategory
        {
            get { return this.typeContext?.Root; }
        }

        public override TableCategoryBase<Table, TableCategory, TableCollection, TableCategoryCollection, TableContext> TableCategory
        {
            get { return this.tableContext?.Root; }
        }

        public new string Name
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        public bool IsLoaded
        {
            get { return this.DataBaseState.HasFlag(DataBaseState.IsLoaded); }
        }

        public bool IsResetting
        {
            get { return this.isResetting; }
        }

        public new bool IsLocked
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.IsLocked;
            }
        }

        public new bool IsPrivate
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.IsPrivate;
            }
        }

        public Guid ID
        {
            get { return base.DataBaseInfo.ID; }
        }

        public new event EventHandler Renamed
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Renamed += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Renamed -= value;
            }
        }

        public new event EventHandler Deleted
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Deleted += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Deleted -= value;
            }
        }

        public new event EventHandler Loaded
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Loaded += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Loaded -= value;
            }
        }

        public new event EventHandler Unloaded
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Unloaded += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Unloaded -= value;
            }
        }

        public event EventHandler<AuthenticationEventArgs> AuthenticationEntered
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.authenticationEntered += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.authenticationEntered -= value;
            }
        }

        public event EventHandler<AuthenticationEventArgs> AuthenticationLeft
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.authenticationLeft += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.authenticationLeft -= value;
            }
        }

        public new event EventHandler Resetting
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Resetting += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Resetting -= value;
            }
        }

        public new event EventHandler Reset
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Reset += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Reset -= value;
            }
        }

        public new event EventHandler DataBaseInfoChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.DataBaseInfoChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.DataBaseInfoChanged -= value;
            }
        }

        public new event EventHandler DataBaseStateChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.DataBaseStateChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.DataBaseStateChanged -= value;
            }
        }

        private void AttachDomainHost()
        {
            var domainContext = this.CremaHost.DomainContext;
            var domains = domainContext.Domains.Where<Domain>(item => item.DataBaseID == this.ID)
                                               .ToArray();

            foreach (var item in domains)
            {
                try
                {
                    var target = this.FindDomainHost(item);
                    target.Restore(item);
                    target.OnRestoredEvent(item);
                    item.Host = target;
                    item.AttachUser();
                }
                catch (Exception e)
                {
                    this.CremaHost.Error(e);
                }
            }
        }

        private void DetachDomainHost()
        {
            var domainContext = this.CremaHost.DomainContext;
            var domains = domainContext.Domains.Where<Domain>(item => item.DataBaseID == this.ID).ToArray();

            foreach (var item in domains)
            {
                item.Host?.Detach();
                item.Host = null;
                item.DetachUser();
            }
        }

        private void ValidateDispatcher()
        {
            if (this.Dispatcher == null)
                throw new InvalidOperationException(Resources.Exception_InvalidObject);
            this.Dispatcher?.VerifyAccess();
        }

        private SignatureDate ReleaseService()
        {
            var signatureDate = this.serviceDispatcher.Invoke(() =>
            {
                var result = this.service.Unsubscribe();
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    this.service.Close();
                else
                    this.service.Abort();
                this.timer?.Dispose();
                this.timer = null;
                this.service = null;
                this.serviceDispatcher.Dispose();
                this.serviceDispatcher = null;
                result.Validate();
                return result.SignatureDate;
            });
            this.cremaHost.RemoveService(this);
            return signatureDate;
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
                this.cremaHost.RemoveService(this);
                this.OnUnloaded(EventArgs.Empty);
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
            this.timer?.Stop();
            try
            {
                await this.serviceDispatcher.InvokeAsync(() => this.service.IsAlive());
                this.timer?.Start();
            }
            catch
            {

            }
        }

        private void OnEnter(Authentication authentication)
        {
            if (this.IsLoaded == false)
                throw new InvalidOperationException(Resources.Exception_CannotEnter);
            this.authentications.Add(authentication);
            if (this.authentications.Any(item => ((Authentication)item).ID == authentication.ID) && this.serviceDispatcher == null)
            {
                this.serviceDispatcher = new CremaDispatcher(this);
                var metaData = this.serviceDispatcher.Invoke(() =>
                {
                    this.service = DataServiceFactory.CreateServiceClient(this.cremaHost.IPAddress, this.cremaHost.ServiceInfos[nameof(DataBaseService)], this);
                    this.service.Open();
                    if (this.service is ICommunicationObject service)
                    {
                        service.Faulted += Service_Faulted;
                    }
                    var result = this.service.Subscribe(this.cremaHost.AuthenticationToken, base.Name);
                    result.Validate(authentication);
#if !DEBUG
                    this.timer = new Timer(30000);
                    this.timer.Elapsed += Timer_Elapsed;
                    this.timer.Start();
#endif
                    return result.Value;
                });
                this.typeContext = new TypeContext(this, metaData);
                this.tableContext = new TableContext(this, metaData);
                this.AttachDomainHost();
                this.cremaHost.AddService(this);
                base.UpdateAccessParent();
                base.UpdateLockParent();
                this.authenticationEntered?.Invoke(this, new AuthenticationEventArgs(authentication.AuthenticationInfo));
                this.DataBases.InvokeItemsAuthenticationEnteredEvent(authentication, new IDataBase[] { this });
            }
        }

        private void OnLeave(Authentication authentication)
        {
            this.authentications.Remove(authentication);

            if (this.authentications.Any(item => ((Authentication)item).ID == authentication.ID) == false && this.serviceDispatcher != null)
            {
                var signatureDate = this.ReleaseService();
                authentication.SignatureDate = signatureDate;
                this.DetachDomainHost();
                this.typeContext.Dispose();
                this.typeContext = null;
                this.tableContext.Dispose();
                this.tableContext = null;
                this.authenticationLeft?.Invoke(this, new AuthenticationEventArgs(authentication.AuthenticationInfo));
                this.DataBases.InvokeItemsAuthenticationLeftEvent(authentication, new IDataBase[] { this });
            }
        }

        private void Sign(Authentication authentication, ResultBase result)
        {
            result.Validate(authentication);
        }

        private void Sign<T>(Authentication authentication, ResultBase<T> result)
        {
            result.Validate(authentication);
        }

#region IDataBase

        IDataBase IDataBase.Copy(Authentication authentication, string newDataBaseName, string comment, bool force)
        {
            this.ValidateDispatcher();
            return this.DataBases.CopyDataBase(authentication, this, newDataBaseName, comment, force);
        }

        ITransaction IDataBase.BeginTransaction(Authentication authentication)
        {
            return this.BeginTransaction(authentication);
        }

        ITableContext IDataBase.TableContext
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                if (this.IsLoaded == false)
                    return null;
                return this.TableContext;
            }
        }

        ITypeContext IDataBase.TypeContext
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                if (this.IsLoaded == false)
                    return null;
                return this.TypeContext;
            }
        }

#endregion

#region IServiceProvider

        object IServiceProvider.GetService(System.Type serviceType)
        {
            if (serviceType == typeof(IDataBase))
                return this;

            if (base.DataBaseState.HasFlag(DataBaseState.IsLoaded) == true && this.serviceDispatcher != null)
            {
                if (serviceType == typeof(ITableContext))
                    return this.tableContext;
                else if (serviceType == typeof(ITableCategoryCollection))
                    return this.tableContext.Categories;
                else if (serviceType == typeof(ITableCollection))
                    return this.tableContext.Tables;
                else if (serviceType == typeof(ITypeContext))
                    return this.typeContext;
                else if (serviceType == typeof(ITypeCategoryCollection))
                    return this.typeContext.Categories;
                else if (serviceType == typeof(ITypeCollection))
                    return this.typeContext.Types;
            }

            return this.CremaHost.GetService(serviceType);
        }

#endregion

#region IDataBaseServiceCallback

        void IDataBaseServiceCallback.OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo)
        {
            this.service.Abort();
            this.service = null;
            this.timer?.Dispose();
            this.timer = null;
            this.serviceDispatcher.Dispose();
            this.serviceDispatcher = null;
            this.InvokeAsync(() =>
            {
                base.DataBaseState = DataBaseState.None;
                this.cremaHost.RemoveService(this);
            }, nameof(IDataBaseServiceCallback.OnServiceClosed));
        }

        void IDataBaseServiceCallback.OnTablesChanged(SignatureDate signatureDate, TableInfo[] tableInfos)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var tables = new Table[tableInfos.Length];
                for (var i = 0; i < tableInfos.Length; i++)
                {
                    var tableInfo = tableInfos[i];
                    var table = this.tableContext.Tables[tableInfo.Name];
                    table.SetTableInfo(tableInfo);
                }
                this.tableContext.Tables.InvokeTablesTemplateChangedEvent(authentication, tables);
            }, nameof(IDataBaseServiceCallback.OnTablesChanged));
        }

        void IDataBaseServiceCallback.OnTablesStateChanged(SignatureDate signatureDate, string[] tableNames, TableState[] states)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var tables = new Table[tableNames.Length];
                for (var i = 0; i < tableNames.Length; i++)
                {
                    var table = this.tableContext.Tables[tableNames[i]];
                    var state = states[i];
                    table.SetTableState(state);
                }
                this.tableContext.Tables.InvokeTablesStateChangedEvent(authentication, tables);
            }, nameof(IDataBaseServiceCallback.OnTablesStateChanged));
        }

        void IDataBaseServiceCallback.OnTableItemsCreated(SignatureDate signatureDate, string[] itemPaths, TableInfo?[] args)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var tableItems = new ITableItem[itemPaths.Length];
                var categories = new List<TableCategory>();
                var tables = new List<Table>();

                for (var i = 0; i < itemPaths.Length; i++)
                {
                    var itemPath = itemPaths[i];
                    if (NameValidator.VerifyCategoryPath(itemPath) == true)
                    {
                        var categoryName = new CategoryName(itemPath);
                        var category = this.tableContext.Categories.Prepare(itemPath);
                        categories.Add(category);
                        tableItems[i] = category;
                    }
                    else
                    {
                        var tableInfo = (TableInfo)args[i];
                        var table = this.tableContext.Tables.AddNew(authentication, tableInfo.Name, tableInfo.CategoryPath);
                        table.Initialize(tableInfo);
                        tables.Add(table);
                        tableItems[i] = table;
                    }
                }

                if (categories.Any() == true)
                {
                    this.tableContext.Categories.InvokeCategoriesCreatedEvent(authentication, categories.ToArray());
                }

                if (tables.Any() == true)
                {
                    this.tableContext.Tables.InvokeTablesCreatedEvent(authentication, tables.ToArray());
                }
            }, nameof(IDataBaseServiceCallback.OnTableItemsCreated));
        }

        void IDataBaseServiceCallback.OnTableItemsRenamed(SignatureDate signatureDate, string[] itemPaths, string[] newNames)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);

                {
                    var items = new List<TableCategory>(itemPaths.Length);
                    var oldNames = new List<string>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var tableItem = this.tableContext[itemPaths[i]];
                        if (tableItem is TableCategory == false)
                            continue;

                        var category = tableItem as TableCategory;
                        items.Add(category);
                        oldNames.Add(category.Name);
                        oldPaths.Add(category.Path);
                    }

                    if (items.Any() == true)
                    {
                        for (var i = 0; i < itemPaths.Length; i++)
                        {
                            var tableItem = this.tableContext[itemPaths[i]];
                            if (tableItem is TableCategory == false)
                                continue;

                            var category = tableItem as TableCategory;
                            var categoryName = newNames[i];
                            category.SetName(categoryName);
                        }

                        this.tableContext.Categories.InvokeCategoriesRenamedEvent(authentication, items.ToArray(), oldNames.ToArray(), oldPaths.ToArray());
                    }
                }

                {
                    var items = new List<Table>(itemPaths.Length);
                    var oldNames = new List<string>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var tableItem = this.tableContext[itemPaths[i]];
                        if (tableItem is Table == false)
                            continue;

                        var table = tableItem as Table;
                        items.Add(table);
                        oldNames.Add(table.Name);
                        oldPaths.Add(table.Path);
                    }

                    if (items.Any() == true)
                    {
                        for (var i = 0; i < itemPaths.Length; i++)
                        {
                            var tableItem = this.tableContext[itemPaths[i]];
                            if (tableItem is Table == false)
                                continue;

                            var table = tableItem as Table;
                            var tableName = newNames[i];
                            table.SetName(tableName);
                        }

                        this.tableContext.Tables.InvokeTablesRenamedEvent(authentication, items.ToArray(), oldNames.ToArray(), oldPaths.ToArray());
                    }
                }
            }, nameof(IDataBaseServiceCallback.OnTableItemsRenamed));
        }

        void IDataBaseServiceCallback.OnTableItemsMoved(SignatureDate signatureDate, string[] itemPaths, string[] parentPaths)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);

                {
                    var items = new List<TableCategory>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);
                    var oldParentPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var tableItem = this.tableContext[itemPaths[i]];
                        if (tableItem is TableCategory == false)
                            continue;

                        var category = tableItem as TableCategory;
                        items.Add(category);
                        oldPaths.Add(category.Path);
                        oldParentPaths.Add(category.Parent.Path);
                    }

                    if (items.Any() == true)
                    {
                        for (var i = 0; i < itemPaths.Length; i++)
                        {
                            var tableItem = this.tableContext[itemPaths[i]];
                            if (tableItem is TableCategory == false)
                                continue;

                            var category = tableItem as TableCategory;
                            var parent = this.tableContext.Categories[parentPaths[i]];
                            category.SetParent(parent);
                        }

                        this.tableContext.Categories.InvokeCategoriesMovedEvent(authentication, items.ToArray(), oldPaths.ToArray(), oldParentPaths.ToArray());
                    }
                }

                {
                    var items = new List<Table>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);
                    var oldParentPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var tableItem = this.tableContext[itemPaths[i]];
                        if (tableItem is Table == false)
                            continue;

                        var table = tableItem as Table;
                        items.Add(table);
                        oldPaths.Add(table.Path);
                        oldParentPaths.Add(table.Category.Path);
                    }

                    if (items.Any() == true)
                    {
                        for (var i = 0; i < itemPaths.Length; i++)
                        {
                            var tableItem = this.tableContext[itemPaths[i]];
                            if (tableItem is Table == false)
                                continue;

                            var table = tableItem as Table;
                            var parent = this.tableContext.Categories[parentPaths[i]];
                            table.SetParent(parent);
                        }

                        this.tableContext.Tables.InvokeTablesMovedEvent(authentication, items.ToArray(), oldPaths.ToArray(), oldParentPaths.ToArray());
                    }
                }
            }, nameof(IDataBaseServiceCallback.OnTableItemsMoved));
        }

        void IDataBaseServiceCallback.OnTableItemsDeleted(SignatureDate signatureDate, string[] itemPaths)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);

                {
                    var items = new List<TableCategory>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var tableItem = this.tableContext[itemPaths[i]];
                        if (tableItem is TableCategory == false)
                            continue;

                        var category = tableItem as TableCategory;
                        items.Add(category);
                        oldPaths.Add(category.Path);
                    }

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var tableItem = this.tableContext[itemPaths[i]];
                        if (tableItem is TableCategory == false)
                            continue;

                        var category = tableItem as TableCategory;
                        category.Dispose();
                    }

                    this.tableContext.Categories.InvokeCategoriesDeletedEvent(authentication, items.ToArray(), oldPaths.ToArray());
                }

                {
                    var items = new List<Table>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var tableItem = this.tableContext[itemPaths[i]];
                        if (tableItem is Table == false)
                            continue;

                        var table = tableItem as Table;
                        items.Add(table);
                        oldPaths.Add(table.Path);
                    }

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var tableItem = this.tableContext[itemPaths[i]];
                        if (tableItem is Table == false)
                            continue;

                        var table = tableItem as Table;
                        table.Dispose();
                    }

                    this.tableContext.Tables.InvokeTablesDeletedEvent(authentication, items.ToArray(), oldPaths.ToArray());
                }
            }, nameof(IDataBaseServiceCallback.OnTableItemsDeleted));
        }

        void IDataBaseServiceCallback.OnTableItemsAccessChanged(SignatureDate signatureDate, AccessChangeType changeType, AccessInfo[] accessInfos, string[] memberIDs, AccessType[] accessTypes)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var tableItems = new ITableItem[accessInfos.Length];
                for (var i = 0; i < accessInfos.Length; i++)
                {
                    var accessInfo = accessInfos[i];
                    var tableItem = this.tableContext[accessInfo.Path];
                    if (tableItem is Table table)
                    {
                        table.SetAccessInfo(changeType, accessInfo);
                    }
                    else if (tableItem is TableCategory category)
                    {
                        category.SetAccessInfo(changeType, accessInfo);
                    }
                    else
                    {
                        throw new NotImplementedException(accessInfo.Path);
                    }
                    tableItems[i] = tableItem as ITableItem;
                }
                switch (changeType)
                {
                    case AccessChangeType.Public:
                        this.TableContext.InvokeItemsSetPublicEvent(authentication, tableItems);
                        break;
                    case AccessChangeType.Private:
                        this.TableContext.InvokeItemsSetPrivateEvent(authentication, tableItems);
                        break;
                    case AccessChangeType.Add:
                        this.TableContext.InvokeItemsAddAccessMemberEvent(authentication, tableItems, memberIDs, accessTypes);
                        break;
                    case AccessChangeType.Set:
                        this.TableContext.InvokeItemsSetAccessMemberEvent(authentication, tableItems, memberIDs, accessTypes);
                        break;
                    case AccessChangeType.Remove:
                        this.TableContext.InvokeItemsRemoveAccessMemberEvent(authentication, tableItems, memberIDs);
                        break;
                }
            }, nameof(IDataBaseServiceCallback.OnTableItemsAccessChanged));
        }

        void IDataBaseServiceCallback.OnTableItemsLockChanged(SignatureDate signatureDate, LockChangeType changeType, LockInfo[] lockInfos, string[] comments)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var tableItems = new ITableItem[lockInfos.Length];
                for (var i = 0; i < lockInfos.Length; i++)
                {
                    var lockInfo = lockInfos[i];
                    var tableItem = this.tableContext[lockInfo.Path];
                    if (tableItem is Table table)
                    {
                        table.SetLockInfo(changeType, lockInfo);
                    }
                    else if (tableItem is TableCategory category)
                    {
                        category.SetLockInfo(changeType, lockInfo);
                    }
                    else
                    {
                        throw new NotImplementedException(lockInfo.Path);
                    }
                    tableItems[i] = tableItem as ITableItem;
                }
                switch (changeType)
                {
                    case LockChangeType.Lock:
                        this.TableContext.InvokeItemsLockedEvent(authentication, tableItems, comments);
                        break;
                    case LockChangeType.Unlock:
                        this.TableContext.InvokeItemsUnlockedEvent(authentication, tableItems);
                        break;
                }
            }, nameof(IDataBaseServiceCallback.OnTableItemsLockChanged));
        }

        void IDataBaseServiceCallback.OnTypesChanged(SignatureDate signatureDate, TypeInfo[] typeInfos)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var types = new Type[typeInfos.Length];
                for (var i = 0; i < typeInfos.Length; i++)
                {
                    var typeInfo = typeInfos[i];
                    var type = this.typeContext.Types[typeInfo.Name];
                    type.SetTypeInfo(typeInfo);
                    types[i] = type;
                }
                this.typeContext.Types.InvokeTypesChangedEvent(authentication, types);
            }, nameof(IDataBaseServiceCallback.OnTypesChanged));
        }

        void IDataBaseServiceCallback.OnTypesStateChanged(SignatureDate signatureDate, string[] typeNames, TypeState[] states)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var types = new Type[typeNames.Length];
                for (var i = 0; i < typeNames.Length; i++)
                {
                    var type = this.typeContext.Types[typeNames[i]];
                    var state = states[i];
                    type.SetTypeState(state);
                    types[i] = type;
                }
                this.typeContext.Types.InvokeTypesStateChangedEvent(authentication, types);
            }, nameof(IDataBaseServiceCallback.OnTypesStateChanged));
        }

        void IDataBaseServiceCallback.OnTypeItemsCreated(SignatureDate signatureDate, string[] itemPaths, TypeInfo?[] args)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var typeItems = new ITypeItem[itemPaths.Length];
                var categories = new List<TypeCategory>();
                var types = new List<Type>();

                for (var i = 0; i < itemPaths.Length; i++)
                {
                    var itemPath = itemPaths[i];
                    if (NameValidator.VerifyCategoryPath(itemPath) == true)
                    {
                        var categoryName = new CategoryName(itemPath);
                        var category = this.typeContext.Categories.Prepare(itemPath);
                        categories.Add(category);
                        typeItems[i] = category;
                    }
                    else
                    {
                        var typeInfo = (TypeInfo)args[i];
                        var type = this.typeContext.Types.AddNew(authentication, typeInfo.Name, typeInfo.CategoryPath);
                        type.Initialize(typeInfo);
                        types.Add(type);
                        typeItems[i] = type;
                    }
                }

                if (categories.Any() == true)
                {
                    this.typeContext.Categories.InvokeCategoriesCreatedEvent(authentication, categories.ToArray());
                }

                if (types.Any() == true)
                {
                    this.typeContext.Types.InvokeTypesCreatedEvent(authentication, types.ToArray());
                }
            }, nameof(IDataBaseServiceCallback.OnTypeItemsCreated));
        }

        void IDataBaseServiceCallback.OnTypeItemsRenamed(SignatureDate signatureDate, string[] itemPaths, string[] newNames)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);

                {
                    var items = new List<TypeCategory>(itemPaths.Length);
                    var oldNames = new List<string>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var typeItem = this.typeContext[itemPaths[i]];
                        if (typeItem is TypeCategory == false)
                            continue;

                        var category = typeItem as TypeCategory;
                        items.Add(category);
                        oldNames.Add(category.Name);
                        oldPaths.Add(category.Path);
                    }

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var typeItem = this.typeContext[itemPaths[i]];
                        if (typeItem is TypeCategory == false)
                            continue;

                        var category = typeItem as TypeCategory;
                        category.SetName(newNames[i]);
                    }

                    this.typeContext.Categories.InvokeCategoriesRenamedEvent(authentication, items.ToArray(), oldNames.ToArray(), oldPaths.ToArray());
                }

                {
                    var items = new List<Type>(itemPaths.Length);
                    var oldNames = new List<string>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var typeItem = this.typeContext[itemPaths[i]];
                        if (typeItem is Type == false)
                            continue;

                        var type = typeItem as Type;
                        items.Add(type);
                        oldNames.Add(type.Name);
                        oldPaths.Add(type.Path);
                    }

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var typeItem = this.typeContext[itemPaths[i]];
                        if (typeItem is Type == false)
                            continue;

                        var type = typeItem as Type;
                        type.SetName(newNames[i]);
                    }

                    this.typeContext.Types.InvokeTypesRenamedEvent(authentication, items.ToArray(), oldNames.ToArray(), oldPaths.ToArray());
                }
            }, nameof(IDataBaseServiceCallback.OnTypeItemsRenamed));
        }

        void IDataBaseServiceCallback.OnTypeItemsMoved(SignatureDate signatureDate, string[] itemPaths, string[] parentPaths)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);

                {
                    var items = new List<TypeCategory>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);
                    var oldParentPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var typeItem = this.typeContext[itemPaths[i]];
                        if (typeItem is TypeCategory == false)
                            continue;

                        var category = typeItem as TypeCategory;
                        items.Add(category);
                        oldPaths.Add(category.Path);
                        oldParentPaths.Add(category.Parent.Path);
                    }

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var typeItem = this.typeContext[itemPaths[i]];
                        if (typeItem is TypeCategory == false)
                            continue;

                        var category = typeItem as TypeCategory;
                        var parent = this.typeContext.Categories[parentPaths[i]];
                        category.SetParent(parent);
                    }

                    this.typeContext.Categories.InvokeCategoriesMovedEvent(authentication, items.ToArray(), oldPaths.ToArray(), oldParentPaths.ToArray());
                }

                {
                    var items = new List<Type>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);
                    var oldParentPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var typeItem = this.typeContext[itemPaths[i]];
                        if (typeItem is Type == false)
                            continue;

                        var type = typeItem as Type;
                        items.Add(type);
                        oldPaths.Add(type.Path);
                        oldParentPaths.Add(type.Category.Path);
                    }

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var typeItem = this.typeContext[itemPaths[i]];
                        if (typeItem is Type == false)
                            continue;

                        var type = typeItem as Type;
                        var parent = this.typeContext.Categories[parentPaths[i]];
                        type.SetParent(parent);
                    }

                    this.typeContext.Types.InvokeTypesMovedEvent(authentication, items.ToArray(), oldPaths.ToArray(), oldParentPaths.ToArray());
                }
            }, nameof(IDataBaseServiceCallback.OnTypeItemsMoved));
        }

        void IDataBaseServiceCallback.OnTypeItemsDeleted(SignatureDate signatureDate, string[] itemPaths)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);

                {
                    var items = new List<TypeCategory>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var typeItem = this.typeContext[itemPaths[i]];
                        if (typeItem is TypeCategory == false)
                            continue;

                        var category = typeItem as TypeCategory;
                        items.Add(category);
                        oldPaths.Add(category.Path);
                    }

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var typeItem = this.typeContext[itemPaths[i]];
                        if (typeItem is TypeCategory == false)
                            continue;

                        var category = typeItem as TypeCategory;
                        category.Dispose();
                    }

                    this.typeContext.Categories.InvokeCategoriesDeletedEvent(authentication, items.ToArray(), oldPaths.ToArray());
                }

                {
                    var items = new List<Type>(itemPaths.Length);
                    var oldPaths = new List<string>(itemPaths.Length);

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var typeItem = this.typeContext[itemPaths[i]];
                        if (typeItem is Type == false)
                            continue;

                        var type = typeItem as Type;
                        items.Add(type);
                        oldPaths.Add(type.Path);
                    }

                    for (var i = 0; i < itemPaths.Length; i++)
                    {
                        var typeItem = this.typeContext[itemPaths[i]];
                        if (typeItem is Type == false)
                            continue;

                        var type = typeItem as Type;
                        type.Dispose();
                    }

                    this.typeContext.Types.InvokeTypesDeletedEvent(authentication, items.ToArray(), oldPaths.ToArray());
                }
            }, nameof(IDataBaseServiceCallback.OnTypeItemsDeleted));
        }

        void IDataBaseServiceCallback.OnTypeItemsAccessChanged(SignatureDate signatureDate, AccessChangeType changeType, AccessInfo[] accessInfos, string[] memberIDs, AccessType[] accessTypes)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var typeItems = new ITypeItem[accessInfos.Length];
                for (var i = 0; i < accessInfos.Length; i++)
                {
                    var accessInfo = accessInfos[i];
                    var typeItem = this.typeContext[accessInfo.Path];
                    if (typeItem is Type type)
                    {
                        type.SetAccessInfo(changeType, accessInfo);
                    }
                    else if (typeItem is TypeCategory category)
                    {
                        category.SetAccessInfo(changeType, accessInfo);
                    }
                    else
                    {
                        throw new NotImplementedException(accessInfo.Path);
                    }
                    typeItems[i] = typeItem as ITypeItem;
                }
                switch (changeType)
                {
                    case AccessChangeType.Public:
                        this.TypeContext.InvokeItemsSetPublicEvent(authentication, typeItems);
                        break;
                    case AccessChangeType.Private:
                        this.TypeContext.InvokeItemsSetPrivateEvent(authentication, typeItems);
                        break;
                    case AccessChangeType.Add:
                        this.TypeContext.InvokeItemsAddAccessMemberEvent(authentication, typeItems, memberIDs, accessTypes);
                        break;
                    case AccessChangeType.Set:
                        this.TypeContext.InvokeItemsSetAccessMemberEvent(authentication, typeItems, memberIDs, accessTypes);
                        break;
                    case AccessChangeType.Remove:
                        this.TypeContext.InvokeItemsRemoveAccessMemberEvent(authentication, typeItems, memberIDs);
                        break;
                }
            }, nameof(IDataBaseServiceCallback.OnTypeItemsAccessChanged));
        }

        void IDataBaseServiceCallback.OnTypeItemsLockChanged(SignatureDate signatureDate, LockChangeType changeType, LockInfo[] lockInfos, string[] comments)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var typeItems = new ITypeItem[lockInfos.Length];
                for (var i = 0; i < lockInfos.Length; i++)
                {
                    var lockInfo = lockInfos[i];
                    var typeItem = this.typeContext[lockInfo.Path];
                    if (typeItem is Type type)
                    {
                        type.SetLockInfo(changeType, lockInfo);
                    }
                    else if (typeItem is TypeCategory category)
                    {
                        category.SetLockInfo(changeType, lockInfo);
                    }
                    else
                    {
                        throw new NotImplementedException(lockInfo.Path);
                    }
                    typeItems[i] = typeItem as ITypeItem;
                }
                switch (changeType)
                {
                    case LockChangeType.Lock:
                        this.TypeContext.InvokeItemsLockedEvent(authentication, typeItems, comments);
                        break;
                    case LockChangeType.Unlock:
                        this.TypeContext.InvokeItemsUnlockedEvent(authentication, typeItems);
                        break;
                }
            }, nameof(IDataBaseServiceCallback.OnTypeItemsLockChanged));
        }

#endregion

#region IInfoProvider

        IDictionary<string, object> IInfoProvider.Info => this.DataBaseInfo.ToDictionary();

#endregion

#region IStateProvider

        object IStateProvider.State => this.DataBaseState;

#endregion
    }
}
