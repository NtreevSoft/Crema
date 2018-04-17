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

using Ntreev.Crema.Services.DataBaseCollectionService;
using Ntreev.Crema.Services.Users;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library;
using System.Timers;

namespace Ntreev.Crema.Services.Data
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    class DataBaseCollection : ContainerBase<DataBase>, IDataBaseCollection, IDataBaseCollectionServiceCallback, ICremaService
    {
        private readonly CremaHost cremaHost;
        private readonly UserContext userContext;
        private CremaDispatcher serviceDispatcher;
        private Timer timer;
        private DataBaseCollectionServiceClient service;

        private ItemsCreatedEventHandler<IDataBase> itemsCreated;
        private ItemsRenamedEventHandler<IDataBase> itemsRenamed;
        private ItemsDeletedEventHandler<IDataBase> itemsDeleted;
        private ItemsEventHandler<IDataBase> itemsLoaded;
        private ItemsEventHandler<IDataBase> itemsUnloaded;
        private ItemsEventHandler<IDataBase> itemsAuthenticationEntered;
        private ItemsEventHandler<IDataBase> itemsAuthenticationLeft;
        private ItemsEventHandler<IDataBase> itemsInfoChanged;
        private ItemsEventHandler<IDataBase> itemsStateChanged;
        private ItemsEventHandler<IDataBase> itemsAccessChanged;
        private ItemsEventHandler<IDataBase> itemsLockChanged;

        public DataBaseCollection(CremaHost cremaHost, string address, ServiceInfo serviceInfo)
        {
            this.cremaHost = cremaHost;
            this.userContext = cremaHost.UserContext;

            this.serviceDispatcher = new CremaDispatcher(this);
            var metaData = this.serviceDispatcher.Invoke(() =>
            {
                this.service = DataBaseCollectionServiceFactory.CreateServiceClient(address, serviceInfo, this);
                this.service.Open();
                if (this.service is ICommunicationObject service)
                {
                    service.Faulted += Service_Faulted;
                }
                var result = this.service.Subscribe(cremaHost.AuthenticationToken);
                result.Validate();
                this.timer = new Timer(30000);
                this.timer.Elapsed += Timer_Elapsed;
                this.timer.Start();
                return result.Value;
            });

            this.Initialize(metaData);
            this.CremaHost.AddService(this);
        }

        public LockInfo InvokeDataBaseLock(Authentication authentication, DataBase dataBase, string comment)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseLock), dataBase, comment);
            var result = this.service.Lock(dataBase.Name, comment);
            result.Validate(authentication);
            return result.Value;
        }

        public void InvokeDataBaseUnlock(Authentication authentication, DataBase dataBase)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseUnlock), dataBase);
            var result = this.service.Unlock(dataBase.Name);
            result.Validate(authentication);
        }

        public AccessInfo InvokeDataBaseSetPrivate(Authentication authentication, DataBase dataBase)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseSetPrivate), dataBase);
            var result = this.service.SetPrivate(dataBase.Name);
            result.Validate(authentication);
            return result.Value;
        }

        public void InvokeDataBaseSetPublic(Authentication authentication, DataBase dataBase)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseSetPrivate), dataBase);
            var result = this.service.SetPublic(dataBase.Name);
            result.Validate(authentication);
        }

        public AccessMemberInfo InvokeDataBaseAddAccessMember(Authentication authentication, DataBase dataBase, string memberID, AccessType accessType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseAddAccessMember), dataBase, memberID, accessType);
            var result = this.service.AddAccessMember(dataBase.Name, memberID, accessType);
            result.Validate(authentication);
            return result.Value;
        }

        public AccessMemberInfo InvokeDataBaseSetAccessMember(Authentication authentication, DataBase dataBase, string memberID, AccessType accessType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseSetAccessMember), dataBase, memberID, accessType);
            var result = this.service.SetAccessMember(dataBase.Name, memberID, accessType);
            result.Validate(authentication);
            return result.Value;
        }

        public void InvokeDataBaseRemoveAccessMember(Authentication authentication, DataBase dataBase, string memberID)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseRemoveAccessMember), dataBase, memberID);
            var result = this.service.RemoveAccessMember(dataBase.Name, memberID);
            result.Validate(authentication);
        }

        public DataBase AddNewDataBase(Authentication authentication, string dataBaseName, string comment)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(AddNewDataBase), dataBaseName, comment);

            var result = this.service.Create(dataBaseName, comment);
            result.Validate(authentication);
            var dataBaseInfo = result.Value;
            var dataBase = new DataBase(this.cremaHost, dataBaseInfo);
            this.AddBase(dataBase.Name, dataBase);
            authentication.SignatureDate = dataBaseInfo.CreationInfo;
            this.InvokeItemsCreateEvent(authentication, new DataBase[] { dataBase });
            return dataBase;
        }

        public void LoadDataBase(Authentication authentication, DataBase dataBase)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(LoadDataBase), dataBase);
            var result = this.service.Load(dataBase.Name);
            result.Validate(authentication);
            dataBase.SetLoaded(authentication);
            this.InvokeItemsLoadedEvent(authentication, new IDataBase[] { dataBase, });
        }

        public void UnloadDataBase(Authentication authentication, DataBase dataBase)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(UnloadDataBase), dataBase);
            var result = this.service.Unload(dataBase.Name);
            result.Validate(authentication);
            dataBase.SetUnloaded(authentication);
            this.InvokeItemsUnloadedEvent(authentication, new IDataBase[] { dataBase, });
        }

        public void RenameDataBase(Authentication authentication, DataBase dataBase, string newDataBaseName)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(RenameDataBase), dataBase, newDataBaseName);

            var dataBaseName = dataBase.Name;
            var result = this.service.Rename(dataBase.Name, newDataBaseName);
            result.Validate(authentication);
            this.ReplaceKeyBase(dataBaseName, newDataBaseName);
            dataBase.Name = newDataBaseName;
            this.InvokeItemsRenamedEvent(authentication, new DataBase[] { dataBase }, new string[] { dataBaseName });
        }

        public void DeleteDataBase(Authentication authentication, DataBase dataBase)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(DeleteDataBase), dataBase);

            var dataBaseName = dataBase.Name;
            var result = this.service.Delete(dataBase.Name);
            result.Validate(authentication);
            this.RemoveBase(dataBase.Name);
            dataBase.Delete();
            this.InvokeItemsDeletedEvent(authentication, new DataBase[] { dataBase }, new string[] { dataBaseName, });
        }

        public DataBase CopyDataBase(Authentication authentication, DataBase dataBase, string newDataBaseName, string comment, bool force)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(CopyDataBase), dataBase, newDataBaseName, comment, force);

            var result = this.service.Copy(dataBase.Name, newDataBaseName, comment, force);
            result.Validate(authentication);
            var dataBaseInfo = result.Value;
            var newDataBase = new DataBase(this.cremaHost, dataBaseInfo);
            this.AddBase(newDataBase.Name, newDataBase);
            authentication.SignatureDate = dataBaseInfo.CreationInfo;
            this.InvokeItemsCreateEvent(authentication, new DataBase[] { newDataBase });
            return newDataBase;
        }

        public LogInfo[] GetLog(Authentication authentication, DataBase dataBase)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(GetLog), dataBase);

            var result = this.service.GetLog(dataBase.Name);
            result.Validate(authentication);
            return result.Value ?? new LogInfo[] { };
        }

        public void Revert(Authentication authentication, DataBase dataBase, long revision)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Revert), dataBase);

            var result = this.service.Revert(dataBase.Name, revision);
            result.Validate(authentication);
        }

        public DataBaseCollectionMetaData GetMetaData(Authentication authentication)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            this.Dispatcher.VerifyAccess();
            return new DataBaseCollectionMetaData()
            {
                DataBases = (from DataBase item in this select item.GetMetaData(authentication)).ToArray(),
            };
        }

        public void InvokeItemsCreateEvent(Authentication authentication, IDataBase[] items)
        {
            var args = items.Select(item => (object)item.DataBaseInfo).ToArray();
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsCreateEvent), items);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(EventMessageBuilder.CreateDataBase(authentication, items));
            this.OnItemsCreated(new ItemsCreatedEventArgs<IDataBase>(authentication, items, args, null));
        }

        public void InvokeItemsRenamedEvent(Authentication authentication, IDataBase[] items, string[] oldNames)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsRenamedEvent), items, oldNames);
            var comment = EventMessageBuilder.RenameDataBase(authentication, items, oldNames);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsRenamed(new ItemsRenamedEventArgs<IDataBase>(authentication, items, oldNames, oldNames));
        }

        public void InvokeItemsDeletedEvent(Authentication authentication, IDataBase[] items, string[] paths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsDeletedEvent), paths);
            var comment = EventMessageBuilder.DeleteDataBase(authentication, paths);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsDeleted(new ItemsDeletedEventArgs<IDataBase>(authentication, items, paths));
        }

        public void InvokeItemsLoadedEvent(Authentication authentication, IDataBase[] items)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeItemsLoadedEvent), items);
            this.CremaHost.Info(EventMessageBuilder.LoadDataBase(authentication, items));
            this.OnItemsLoaded(new ItemsEventArgs<IDataBase>(authentication, items));
        }

        public void InvokeItemsAuthenticationEnteredEvent(Authentication authentication, IDataBase[] items)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeItemsAuthenticationEnteredEvent), items);
            this.CremaHost.Info(EventMessageBuilder.EnterDataBase(authentication, items));
            this.OnItemsEntered(new ItemsEventArgs<IDataBase>(authentication, items, authentication.AuthenticationInfo));
        }

        public void InvokeItemsAuthenticationLeftEvent(Authentication authentication, IDataBase[] items)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeItemsAuthenticationLeftEvent), items);
            this.CremaHost.Info(EventMessageBuilder.LeaveDataBase(authentication, items));
            this.OnItemsLeft(new ItemsEventArgs<IDataBase>(authentication, items, authentication.AuthenticationInfo));
        }

        public void InvokeItemsUnloadedEvent(Authentication authentication, IDataBase[] items)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeItemsUnloadedEvent), items);
            this.CremaHost.Info(EventMessageBuilder.UnloadDataBase(authentication, items));
            this.OnItemsUnloaded(new ItemsEventArgs<IDataBase>(authentication, items));
        }

        public void InvokeItemsInfoChangedEvent(Authentication authentication, IDataBase[] items)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeItemsInfoChangedEvent), items);
            this.OnItemsInfoChanged(new ItemsEventArgs<IDataBase>(authentication, items));
        }

        public void InvokeItemsStateChangedEvent(Authentication authentication, IDataBase[] items)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeItemsStateChangedEvent), items);
            this.OnItemsStateChanged(new ItemsEventArgs<IDataBase>(authentication, items));
        }

        public void InvokeItemsSetPublicEvent(Authentication authentication, IDataBase[] items)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsSetPublicEvent), items);
            var comment = EventMessageBuilder.SetPublicDataBase(authentication, items);
            var metaData = new object[] { AccessChangeType.Public, new string[] { string.Empty, }, new AccessType[] { AccessType.None, }, };
            this.CremaHost.Debug(eventLog);
            //this.repository.Commit(accessInfoPath, comment, authentication, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<IDataBase>(authentication, items, metaData));
        }

        public void InvokeItemsSetPrivateEvent(Authentication authentication, IDataBase[] items)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsSetPrivateEvent), items);
            var comment = EventMessageBuilder.SetPrivateDataBase(authentication, items);
            var metaData = new object[] { AccessChangeType.Private, new string[] { string.Empty, }, new AccessType[] { AccessType.None, }, };
            this.CremaHost.Debug(eventLog);
            //this.repository.Commit(accessInfoPath, comment, authentication, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<IDataBase>(authentication, items, metaData));
        }

        public void InvokeItemsAddAccessMemberEvent(Authentication authentication, IDataBase[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsAddAccessMemberEvent), items, memberIDs, accessTypes);
            var comment = EventMessageBuilder.AddAccessMemberToDataBase(authentication, items, memberIDs, accessTypes);
            var metaData = new object[] { AccessChangeType.Add, memberIDs, accessTypes, };
            this.CremaHost.Debug(eventLog);
            //this.repository.Commit(accessInfoPath, comment, authentication, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<IDataBase>(authentication, items, metaData));
        }

        public void InvokeItemsSetAccessMemberEvent(Authentication authentication, IDataBase[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsSetAccessMemberEvent), items, memberIDs, accessTypes);
            var comment = EventMessageBuilder.SetAccessMemberOfDataBase(authentication, items, memberIDs, accessTypes);
            var metaData = new object[] { AccessChangeType.Set, memberIDs, accessTypes, };
            this.CremaHost.Debug(eventLog);
            //this.repository.Commit(accessInfoPath, comment, authentication, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<IDataBase>(authentication, items, metaData));
        }

        public void InvokeItemsRemoveAccessMemberEvent(Authentication authentication, IDataBase[] items, string[] memberIDs)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsRemoveAccessMemberEvent), items, memberIDs);
            var comment = EventMessageBuilder.RemoveAccessMemberFromDataBase(authentication, items, memberIDs);
            var metaData = new object[] { AccessChangeType.Remove, memberIDs, new AccessType[] { AccessType.None, }, };
            this.CremaHost.Debug(eventLog);
            //this.repository.Commit(accessInfoPath, comment, authentication, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<IDataBase>(authentication, items, new object[] { AccessChangeType.Remove, memberIDs, }));
        }

        public void InvokeItemsLockedEvent(Authentication authentication, IDataBase[] items, string[] comments)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsLockedEvent), items, comments);
            var comment = EventMessageBuilder.LockDataBase(authentication, items, comments);
            var metaData = new object[] { LockChangeType.Lock, new string[] { comment, }, };
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsLockChanged(new ItemsEventArgs<IDataBase>(authentication, items, metaData));
        }

        public void InvokeItemsUnlockedEvent(Authentication authentication, IDataBase[] items)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsUnlockedEvent), items);
            var comment = EventMessageBuilder.UnlockDataBase(authentication, items);
            var metaData = new object[] { LockChangeType.Unlock, new string[] { string.Empty, }, };
            this.CremaHost.Info(EventMessageBuilder.UnlockDataBase(authentication, items));
            this.OnItemsLockChanged(new ItemsEventArgs<IDataBase>(authentication, items, metaData));
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

        public ResultBase LoadDataBase(DataBase dataBase)
        {
            return this.service.Load(dataBase.Name);
        }

        public ResultBase UnloadDataBase(DataBase dataBase)
        {
            return this.service.Unload(dataBase.Name);
        }

        public new DataBase this[string dataBaseName]
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base[dataBaseName];
            }
        }

        public DataBase this[Guid dataBaseID]
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.FirstOrDefault<DataBase>(item => item.ID == dataBaseID);
            }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.CremaHost.Dispatcher; }
        }

        public CremaHost CremaHost
        {
            get { return this.cremaHost; }
        }

        public new int Count
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.Count;
            }
        }

        public event ItemsCreatedEventHandler<IDataBase> ItemsCreated
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

        public event ItemsRenamedEventHandler<IDataBase> ItemsRenamed
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

        public event ItemsDeletedEventHandler<IDataBase> ItemsDeleted
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

        public event ItemsEventHandler<IDataBase> ItemsLoaded
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsLoaded += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsLoaded -= value;
            }
        }

        public event ItemsEventHandler<IDataBase> ItemsUnloaded
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsUnloaded += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsUnloaded -= value;
            }
        }

        public event ItemsEventHandler<IDataBase> ItemsAuthenticationEntered
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsAuthenticationEntered += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsAuthenticationEntered -= value;
            }
        }

        public event ItemsEventHandler<IDataBase> ItemsAuthenticationLeft
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsAuthenticationLeft += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsAuthenticationLeft -= value;
            }
        }


        public event ItemsEventHandler<IDataBase> ItemsInfoChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsInfoChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsInfoChanged -= value;
            }
        }

        public event ItemsEventHandler<IDataBase> ItemsStateChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsStateChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsStateChanged -= value;
            }
        }

        public event ItemsEventHandler<IDataBase> ItemsAccessChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsAccessChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsAccessChanged -= value;
            }
        }

        public event ItemsEventHandler<IDataBase> ItemsLockChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsLockChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsLockChanged -= value;
            }
        }

        protected virtual void OnItemsCreated(ItemsCreatedEventArgs<IDataBase> e)
        {
            this.itemsCreated?.Invoke(this, e);
        }

        protected virtual void OnItemsRenamed(ItemsRenamedEventArgs<IDataBase> e)
        {
            this.itemsRenamed?.Invoke(this, e);
        }

        protected virtual void OnItemsDeleted(ItemsDeletedEventArgs<IDataBase> e)
        {
            this.itemsDeleted?.Invoke(this, e);
        }

        protected virtual void OnItemsLoaded(ItemsEventArgs<IDataBase> e)
        {
            this.itemsLoaded?.Invoke(this, e);
        }

        protected virtual void OnItemsUnloaded(ItemsEventArgs<IDataBase> e)
        {
            this.itemsUnloaded?.Invoke(this, e);
        }

        protected virtual void OnItemsEntered(ItemsEventArgs<IDataBase> e)
        {
            this.itemsAuthenticationEntered?.Invoke(this, e);
        }

        protected virtual void OnItemsLeft(ItemsEventArgs<IDataBase> e)
        {
            this.itemsAuthenticationLeft?.Invoke(this, e);
        }

        protected virtual void OnItemsInfoChanged(ItemsEventArgs<IDataBase> e)
        {
            this.itemsInfoChanged?.Invoke(this, e);
        }

        protected virtual void OnItemsStateChanged(ItemsEventArgs<IDataBase> e)
        {
            this.itemsStateChanged?.Invoke(this, e);
        }

        protected virtual void OnItemsAccessChanged(ItemsEventArgs<IDataBase> e)
        {
            this.itemsAccessChanged?.Invoke(this, e);
        }

        protected virtual void OnItemsLockChanged(ItemsEventArgs<IDataBase> e)
        {
            this.itemsLockChanged?.Invoke(this, e);
        }

        private void Initialize(DataBaseCollectionMetaData metaData)
        {
            for (var i = 0; i < metaData.DataBases.Length; i++)
            {
                var dataBaseInfo = metaData.DataBases[i];
                var dataBase = new DataBase(cremaHost, dataBaseInfo);
                this.AddBase(dataBase.Name, dataBase);
            }
        }

        private async void InvokeAsync(Action action, string callbackName)
        {
            try
            {
                await this.Dispatcher.InvokeAsync(action);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(callbackName);
                this.CremaHost.Error(e);
            }
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.timer.Stop();
            try
            {
                await this.serviceDispatcher.InvokeAsync(() => this.service.IsAlive());
                this.timer?.Start();
            }
            catch
            {
                
            }
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
                this.CremaHost.RemoveService(this);
            }, nameof(Service_Faulted));
        }

        #region IDataBaseCollectionServiceCallback

        void IDataBaseCollectionServiceCallback.OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo)
        {
            this.service.Abort();
            this.service = null;
            this.timer?.Dispose();
            this.timer = null;
            this.serviceDispatcher.Dispose();
            this.serviceDispatcher = null;
            this.InvokeAsync(() =>
            {
                this.CremaHost.RemoveService(this);
            }, nameof(IDataBaseCollectionServiceCallback.OnServiceClosed));
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesCreated(SignatureDate signatureDate, string[] dataBaseNames, DataBaseInfo[] dataBaseInfos)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var dataBases = new DataBase[dataBaseNames.Length];
                for (var i = 0; i < dataBaseNames.Length; i++)
                {
                    var dataBaseName = dataBaseNames[i];
                    var dataBaseInfo = dataBaseInfos[i];
                    var dataBase = new DataBase(this.cremaHost, dataBaseInfo);
                    this.AddBase(dataBase.Name, dataBase);
                    dataBases[i] = dataBase;
                }
                this.InvokeItemsCreateEvent(authentication, dataBases);
            }, nameof(IDataBaseCollectionServiceCallback.OnDataBasesCreated));
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesRenamed(SignatureDate signatureDate, string[] dataBaseNames, string[] newDataBaseNames)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var dataBases = new DataBase[dataBaseNames.Length];
                for (var i = 0; i < dataBaseNames.Length; i++)
                {
                    var dataBaseName = dataBaseNames[i];
                    var newDataBaseName = newDataBaseNames[i];
                    var dataBase = this[dataBaseName];
                    this.ReplaceKeyBase(dataBaseName, newDataBaseName);
                    dataBase.Name = newDataBaseName;
                    dataBases[i] = dataBase;
                }
                this.InvokeItemsRenamedEvent(authentication, dataBases, dataBaseNames);
            }, nameof(IDataBaseCollectionServiceCallback.OnDataBasesRenamed));
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesDeleted(SignatureDate signatureDate, string[] dataBaseNames)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var dataBases = new DataBase[dataBaseNames.Length];
                for (var i = 0; i < dataBaseNames.Length; i++)
                {
                    var dataBaseName = dataBaseNames[i];
                    var dataBase = this[dataBaseName];
                    this.RemoveBase(dataBaseName);
                    dataBases[i] = dataBase;
                }

                this.InvokeItemsDeletedEvent(authentication, dataBases, dataBaseNames);
            }, nameof(IDataBaseCollectionServiceCallback.OnDataBasesDeleted));
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesLoaded(SignatureDate signatureDate, string[] dataBaseNames)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var dataBases = new DataBase[dataBaseNames.Length];
                for (var i = 0; i < dataBaseNames.Length; i++)
                {
                    var dataBaseName = dataBaseNames[i];
                    var dataBase = this[dataBaseName];
                    dataBase.SetLoaded(authentication);
                    dataBases[i] = dataBase;
                }
                this.InvokeItemsLoadedEvent(authentication, dataBases);
            }, nameof(IDataBaseCollectionServiceCallback.OnDataBasesLoaded));
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesUnloaded(SignatureDate signatureDate, string[] dataBaseNames)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var dataBases = new DataBase[dataBaseNames.Length];
                for (var i = 0; i < dataBaseNames.Length; i++)
                {
                    var dataBaseName = dataBaseNames[i];
                    var dataBase = this[dataBaseName];
                    dataBase.SetUnloaded(authentication);
                    dataBases[i] = dataBase;
                }
                this.InvokeItemsUnloadedEvent(authentication, dataBases);
            }, nameof(IDataBaseCollectionServiceCallback.OnDataBasesUnloaded));
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesAuthenticationEntered(SignatureDate signatureDate, string[] dataBaseNames, AuthenticationInfo authenticationInfo)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var dataBases = new DataBase[dataBaseNames.Length];
                for (var i = 0; i < dataBaseNames.Length; i++)
                {
                    var dataBaseName = dataBaseNames[i];
                    var dataBase = this[dataBaseName];
                    dataBase.SetAuthenticationEntered(authentication);
                    dataBases[i] = dataBase;
                }
                this.InvokeItemsAuthenticationEnteredEvent(authentication, dataBases);
            }, nameof(IDataBaseCollectionServiceCallback.OnDataBasesAuthenticationEntered));
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesAuthenticationLeft(SignatureDate signatureDate, string[] dataBaseNames, AuthenticationInfo authenticationInfo)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var dataBases = new DataBase[dataBaseNames.Length];
                for (var i = 0; i < dataBaseNames.Length; i++)
                {
                    var dataBaseName = dataBaseNames[i];
                    var dataBase = this[dataBaseName];
                    dataBase.SetAuthenticationLeft(authentication);
                    dataBases[i] = dataBase;
                }
                this.InvokeItemsAuthenticationLeftEvent(authentication, dataBases);
            }, nameof(IDataBaseCollectionServiceCallback.OnDataBasesAuthenticationLeft));
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesInfoChanged(SignatureDate signatureDate, DataBaseInfo[] dataBaseInfos)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var dataBases = new DataBase[dataBaseInfos.Length];
                for (var i = 0; i < dataBaseInfos.Length; i++)
                {
                    var dataBaseInfo = dataBaseInfos[i];
                    var dataBase = this[dataBaseInfo.Name];
                    dataBase.SetDataBaseInfo(dataBaseInfo);
                    dataBases[i] = dataBase;
                }
                this.InvokeItemsInfoChangedEvent(authentication, dataBases);
            }, nameof(IDataBaseCollectionServiceCallback.OnDataBasesInfoChanged));
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesStateChanged(SignatureDate signatureDate, string[] dataBaseNames, DataBaseState[] dataBaseStates)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var dataBases = new DataBase[dataBaseNames.Length];
                for (var i = 0; i < dataBaseNames.Length; i++)
                {
                    var dataBaseName = dataBaseNames[i];
                    var dataBaseState = dataBaseStates[i];
                    var dataBase = this[dataBaseName];
                    dataBase.SetDataBaseState(dataBaseState);
                    dataBases[i] = dataBase;
                }
                this.InvokeItemsStateChangedEvent(authentication, dataBases);
            }, nameof(IDataBaseCollectionServiceCallback.OnDataBasesStateChanged));
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesAccessChanged(SignatureDate signatureDate, AccessChangeType changeType, AccessInfo[] accessInfos, string[] memberIDs, AccessType[] accessTypes)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var dataBases = new DataBase[accessInfos.Length];
                for (var i = 0; i < accessInfos.Length; i++)
                {
                    var accessInfo = accessInfos[i];
                    var dataBase = this[accessInfo.Path];
                    dataBase.SetAccessInfo(changeType, accessInfo);
                    dataBases[i] = dataBase;
                }
                switch (changeType)
                {
                    case AccessChangeType.Public:
                        this.InvokeItemsSetPublicEvent(authentication, dataBases);
                        break;
                    case AccessChangeType.Private:
                        this.InvokeItemsSetPrivateEvent(authentication, dataBases);
                        break;
                    case AccessChangeType.Add:
                        this.InvokeItemsAddAccessMemberEvent(authentication, dataBases, memberIDs, accessTypes);
                        break;
                    case AccessChangeType.Set:
                        this.InvokeItemsSetAccessMemberEvent(authentication, dataBases, memberIDs, accessTypes);
                        break;
                    case AccessChangeType.Remove:
                        this.InvokeItemsRemoveAccessMemberEvent(authentication, dataBases, memberIDs);
                        break;
                }
            }, nameof(IDataBaseCollectionServiceCallback.OnDataBasesAccessChanged));
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesLockChanged(SignatureDate signatureDate, LockChangeType changeType, LockInfo[] lockInfos, string[] comments)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var dataBases = new DataBase[lockInfos.Length];
                for (var i = 0; i < lockInfos.Length; i++)
                {
                    var lockInfo = lockInfos[i];
                    var dataBase = this[lockInfo.Path];
                    dataBase.SetLockInfo(changeType, lockInfo);
                    dataBases[i] = dataBase;
                }
                switch (changeType)
                {
                    case LockChangeType.Lock:
                        this.InvokeItemsLockedEvent(authentication, dataBases, comments);
                        break;
                    case LockChangeType.Unlock:
                        this.InvokeItemsUnlockedEvent(authentication, dataBases);
                        break;
                }
            }, nameof(IDataBaseCollectionServiceCallback.OnDataBasesLockChanged));
        }

        #endregion

        #region IDataBaseCollection

        IDataBase IDataBaseCollection.AddNewDataBase(Authentication authentication, string dataBaseName, string comment)
        {
            return this.AddNewDataBase(authentication, dataBaseName, comment);
        }

        bool IDataBaseCollection.Contains(string dataBaseName)
        {
            this.Dispatcher.VerifyAccess();
            return this.ContainsKey(dataBaseName);
        }

        IDataBase IDataBaseCollection.this[string dataBaseName]
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this[dataBaseName];
            }
        }

        IDataBase IDataBaseCollection.this[Guid dataBaseID]
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this[dataBaseID];
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator<IDataBase> IEnumerable<IDataBase>.GetEnumerator()
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
    }
}
