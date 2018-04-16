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
using Ntreev.Crema.Services.Domains;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ntreev.Crema.Services.Data
{
    class DataBaseCollection : ContainerBase<DataBase>, IDataBaseCollection
    {
        public const string DataBaseString = "database";
        private const string databaseExtension = ".database";
        private const string stateExtension = ".state";
        private readonly CremaHost cremaHost;
        private readonly IRepository repository;
        private readonly string cachePath;
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

        public DataBaseCollection(CremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
            this.cachePath = DirectoryUtility.Prepare(this.cremaHost.WorkingPath, DataBaseString);
            this.repository = cremaHost.Repository;

            var revision = this.repository.Revision;
            var caches = this.cremaHost.NoCache == true ? new Dictionary<string, DataBaseSerializationInfo>() : this.ReadCaches();

            foreach (var item in GetDataBases())
            {
                var dataBasePath = item.Value;
                var dataBaseName = item.Key;
                var dataBase = CreateDataBase();
                dataBase.Initialize();
                this.AddBase(dataBaseName, dataBase);

                DataBase CreateDataBase()
                {
                    if (caches.ContainsKey(dataBaseName) == false)
                        return new DataBase(this.cremaHost, dataBasePath, dataBaseName);
                    return new DataBase(this.cremaHost, dataBasePath, dataBaseName, caches[dataBaseName]);
                }
            }

            IEnumerable<KeyValuePair<string, string>> GetDataBases()
            {
                yield return new KeyValuePair<string, string>(DataBase.defaultName, this.cremaHost.TrunkPath);

                foreach (var item in Directory.GetDirectories(this.cremaHost.TagsPath))
                {
                    var dataBaseName = Path.GetFileName(item);
                    yield return new KeyValuePair<string, string>(dataBaseName, item);
                }
            }
        }

        public void RestoreState()
        {
            var stateCaches = this.ReadStateCaches();
            foreach (var item in this)
            {
                if (stateCaches.ContainsKey($"{item.ID}") == true)
                {
                    if (stateCaches[$"{item.ID}"] == DataBaseState.IsLoaded)
                    {
                        item.Load(Authentication.System);
                    }
                }
            }
        }

        public void InvokeDataBaseLock(Authentication authentication, DataBase dataBase, string comment)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseLock), dataBase, comment);
        }

        public void InvokeDataBaseUnlock(Authentication authentication, DataBase dataBase)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseUnlock), dataBase);
        }

        public void InvokeDataBaseSetPrivate(Authentication authentication, DataBase dataBase, AccessInfo accessInfo)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseSetPrivate), dataBase);
            var accessInfoPath = dataBase.GetAccessInfoPath();
            try
            {
                accessInfo.SetPrivate(dataBase.GetType().Name, authentication.SignatureDate);
                dataBase.WriteAccessInfo(accessInfoPath, accessInfo);
                this.repository.Add(accessInfoPath);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert(dataBase.BasePath);
                throw e;
            }
        }

        public void InvokeDataBaseSetPublic(Authentication authentication, DataBase dataBase, AccessInfo accessInfo)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseSetPrivate), dataBase);
            var accessInfoPath = dataBase.GetAccessInfoPath();
            try
            {
                accessInfo.SetPublic();
                this.repository.Delete(accessInfoPath);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert(dataBase.BasePath);
                throw e;
            }
        }

        public void InvokeDataBaseAddAccessMember(Authentication authentication, DataBase dataBase, AccessInfo accessInfo, string memberID, AccessType accessType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseAddAccessMember), dataBase, memberID, accessType);
            var accessInfoPath = dataBase.GetAccessInfoPath();
            try
            {
                accessInfo.Add(authentication.SignatureDate, memberID, accessType);
                dataBase.WriteAccessInfo(accessInfoPath, accessInfo);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert(dataBase.BasePath);
                throw e;
            }
        }

        public void InvokeDataBaseSetAccessMember(Authentication authentication, DataBase dataBase, AccessInfo accessInfo, string memberID, AccessType accessType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseSetAccessMember), dataBase, memberID, accessType);
            var accessInfoPath = dataBase.GetAccessInfoPath();
            try
            {
                accessInfo.Set(authentication.SignatureDate, memberID, accessType);
                dataBase.WriteAccessInfo(accessInfoPath, accessInfo);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert(dataBase.BasePath);
                throw e;
            }
        }

        public void InvokeDataBaseRemoveAccessMember(Authentication authentication, DataBase dataBase, AccessInfo accessInfo, string memberID)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseRemoveAccessMember), dataBase, memberID);
            var accessInfoPath = dataBase.GetAccessInfoPath();
            try
            {
                accessInfo.Remove(authentication.SignatureDate, memberID);
                dataBase.WriteAccessInfo(accessInfoPath, accessInfo);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert(dataBase.BasePath);
                throw e;
            }
        }

        public void InvokeDataBaseLoad(Authentication authentication, DataBase dataBase)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseLoad), dataBase);
        }

        public void InvokeDataBaseUnload(Authentication authentication, DataBase dataBase)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseUnload), dataBase);
        }

        public DataBase CreateDataBase(Authentication authentication, string dataBaseName, string comment)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(CreateDataBase), dataBaseName, comment);
            this.ValidateCreateDataBase(authentication, dataBaseName);

            var dataBasePath = DirectoryUtility.Prepare(this.cremaHost.TagsPath, dataBaseName);
            var typePath = DirectoryUtility.Prepare(dataBasePath, CremaSchema.TypeDirectory);
            var tablePath = DirectoryUtility.Prepare(dataBasePath, CremaSchema.TableDirectory);

            try
            {
                this.cremaHost.RepositoryDispatcher.Invoke(() =>
                {
                    this.repository.Add(dataBasePath);
                    this.repository.Add(typePath);
                    this.repository.Add(tablePath);
                });
            }
            catch (Exception e)
            {
                this.cremaHost.Error(e);
                DirectoryUtility.Delete(typePath);
                DirectoryUtility.Delete(tablePath);
                DirectoryUtility.Delete(dataBasePath);
                throw e;
            }

            var dataBase = this.AddFromPath(dataBasePath);
            authentication.Sign();
            this.InvokeItemsCreateEvent(authentication, new DataBase[] { dataBase, }, comment);
            return dataBase;
        }

        public DataBase CopyDataBase(Authentication authentication, DataBase dataBase, string newDataBaseName, string comment, bool force)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(CopyDataBase), dataBase, newDataBaseName, comment);
            this.ValidateCopyDataBase(authentication, dataBase, newDataBaseName, force);

            var basePath = Path.Combine(this.cremaHost.TagsPath, newDataBaseName);

            try
            {
                this.cremaHost.RepositoryDispatcher.Invoke(() =>
                {
                    this.repository.Copy(dataBase.BasePath, basePath);
                });
            }
            catch (Exception e)
            {
                this.cremaHost.Error(e);
                this.cremaHost.RepositoryDispatcher.Invoke(() =>
                {
                    this.repository.Revert(this.cremaHost.TagsPath);
                });
                throw e;
            }

            var newDataBase = new DataBase(this.cremaHost, basePath, Path.GetFileName(basePath));
            this.AddBase(newDataBase.Name, newDataBase);
            authentication.Sign();
            this.InvokeItemsCreateEvent(authentication, new DataBase[] { newDataBase, }, comment);
            return newDataBase;
        }

        public void InvokeDataBaseRename(Authentication authentication, DataBase dataBase, string newDataBaseName)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseRename), dataBase, newDataBaseName);
            this.ValidateRenameDataBase(authentication, dataBase, newDataBaseName);

            var dataBaseName = dataBase.Name;
            var dataBasePath = Path.Combine(this.cremaHost.TagsPath, dataBase.Name);
            var newDataBasePath = Path.Combine(this.cremaHost.TagsPath, newDataBaseName);

            try
            {
                this.cremaHost.RepositoryDispatcher.Invoke(() =>
                {
                    this.repository.Move(dataBasePath, newDataBasePath);
                });
            }
            catch (Exception e)
            {
                this.cremaHost.Error(e);
                this.cremaHost.RepositoryDispatcher.Invoke(() =>
                {
                    this.repository.Revert(this.cremaHost.TagsPath);
                });
                throw e;
            }
            this.ReplaceKeyBase(dataBaseName, newDataBaseName);
        }

        public void InvokeDataBaseDelete(Authentication authentication, DataBase dataBase)
        {
            this.Dispatcher.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeDataBaseDelete), dataBase);
            this.ValidateDeleteDataBase(authentication, dataBase);

            var dataBasePath = Path.Combine(this.cremaHost.TagsPath, dataBase.Name);

            try
            {
                this.cremaHost.RepositoryDispatcher.Invoke(() =>
                {
                    this.repository.Delete(dataBasePath);
                });
            }
            catch (Exception e)
            {
                this.cremaHost.Error(e);
                this.cremaHost.RepositoryDispatcher.Invoke(() =>
                {
                    this.repository.Revert(this.cremaHost.TagsPath);
                });
                throw e;
            }

            this.RemoveBase(dataBase.Name);
            //FileUtility.Delete(this.cachePath, $"{dataBase.ID}{databaseExtension}");
            //dataBase.DeleteInternal(authentication);
            //authentication.Sign();
            //this.InvokeItemsDeletedEvent(authentication, new DataBase[] { dataBase, }, new string[] { dataBase.Name, });
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

        public void InvokeItemsCreateEvent(Authentication authentication, DataBase[] items, string comment)
        {
            var args = items.Select(item => (object)item.DataBaseInfo).ToArray();
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsCreateEvent), items);
            this.CremaHost.Debug(eventLog);
            this.cremaHost.RepositoryDispatcher.Invoke(() =>
            {
                this.repository.Commit(this.cremaHost.TagsPath, comment, authentication, eventLog);
            });
            foreach (var item in items)
            {
                item.Initialize();
            }
            this.CremaHost.Info(EventMessageBuilder.CreateDataBase(authentication, items));
            this.OnItemsCreated(new ItemsCreatedEventArgs<IDataBase>(authentication, items, args, null));
        }

        public void InvokeItemsRenamedEvent(Authentication authentication, DataBase[] items, string[] oldNames)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsRenamedEvent), items, oldNames);
            var comment = EventMessageBuilder.RenameDataBase(authentication, items, oldNames);
            this.CremaHost.Debug(eventLog);
            this.cremaHost.RepositoryDispatcher.Invoke(() =>
            {
                this.repository.Commit(this.cremaHost.TagsPath, comment, authentication, eventLog);
            });
            this.CremaHost.Info(comment);
            this.OnItemsRenamed(new ItemsRenamedEventArgs<IDataBase>(authentication, items, oldNames, oldNames));
        }

        public void InvokeItemsDeletedEvent(Authentication authentication, IDataBase[] items, string[] paths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsDeletedEvent), paths);
            var comment = EventMessageBuilder.DeleteDataBase(authentication, paths);
            this.CremaHost.Debug(eventLog);
            this.cremaHost.RepositoryDispatcher.Invoke(() =>
            {
                this.repository.Commit(this.cremaHost.TagsPath, comment, authentication, eventLog);
            });
            this.CremaHost.Info(comment);
            this.OnItemsDeleted(new ItemsDeletedEventArgs<IDataBase>(authentication, items, paths));
        }

        public void InvokeItemsLoadedEvent(Authentication authentication, IDataBase[] items)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeItemsLoadedEvent), items);
            this.CremaHost.Info(EventMessageBuilder.LoadDataBase(authentication, items));
            this.OnItemsLoaded(new ItemsEventArgs<IDataBase>(authentication, items));
        }

        public void InvokeItemsUnloadedEvent(Authentication authentication, IDataBase[] items)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeItemsUnloadedEvent), items);
            this.CremaHost.Info(EventMessageBuilder.UnloadDataBase(authentication, items));
            this.OnItemsUnloaded(new ItemsEventArgs<IDataBase>(authentication, items));
        }

        public void InvokeItemsAuthenticationEnteredEvent(Authentication authentication, IDataBase[] items)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeItemsAuthenticationEnteredEvent), items);
            this.CremaHost.Info(EventMessageBuilder.EnterDataBase(authentication, items));
            this.OnItemsAuthenticationEntered(new ItemsEventArgs<IDataBase>(authentication, items, authentication.AuthenticationInfo));
        }

        public void InvokeItemsAuthenticationLeftEvent(Authentication authentication, IDataBase[] items)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeItemsAuthenticationLeftEvent), items);
            this.CremaHost.Info(EventMessageBuilder.LeaveDataBase(authentication, items));
            this.OnItemsAuthenticationLeft(new ItemsEventArgs<IDataBase>(authentication, items, authentication.AuthenticationInfo));
        }

        public void InvokeItemsChangedEvent(Authentication authentication, IDataBase[] items)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeItemsChangedEvent), items);
            this.OnItemsInfoChanged(new ItemsEventArgs<IDataBase>(authentication, items));
        }

        public void InvokeItemsStateChangedEvent(Authentication authentication, IDataBase[] items)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeItemsStateChangedEvent), items);
            this.OnItemsStateChanged(new ItemsEventArgs<IDataBase>(authentication, items));
        }

        public void InvokeItemsSetPublicEvent(Authentication authentication, string basePath, IDataBase[] items)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsSetPublicEvent), items);
            var comment = EventMessageBuilder.SetPublicDataBase(authentication, items);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Public);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(basePath, comment, authentication, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<IDataBase>(authentication, items, metaData));
        }

        public void InvokeItemsSetPrivateEvent(Authentication authentication, string basePath, IDataBase[] items)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsSetPrivateEvent), items);
            var comment = EventMessageBuilder.SetPrivateDataBase(authentication, items);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Private);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(basePath, comment, authentication, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<IDataBase>(authentication, items, metaData));
        }

        public void InvokeItemsAddAccessMemberEvent(Authentication authentication, string basePath, IDataBase[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsAddAccessMemberEvent), items, memberIDs, accessTypes);
            var comment = EventMessageBuilder.AddAccessMemberToDataBase(authentication, items, memberIDs, accessTypes);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Add, memberIDs, accessTypes);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(basePath, comment, authentication, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<IDataBase>(authentication, items, metaData));
        }

        public void InvokeItemsSetAccessMemberEvent(Authentication authentication, string basePath, IDataBase[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsSetAccessMemberEvent), items, memberIDs, accessTypes);
            var comment = EventMessageBuilder.SetAccessMemberOfDataBase(authentication, items, memberIDs, accessTypes);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Set, memberIDs, accessTypes);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(basePath, comment, authentication, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<IDataBase>(authentication, items, metaData));
        }

        public void InvokeItemsRemoveAccessMemberEvent(Authentication authentication, string basePath, IDataBase[] items, string[] memberIDs)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsRemoveAccessMemberEvent), items, memberIDs);
            var comment = EventMessageBuilder.RemoveAccessMemberFromDataBase(authentication, items, memberIDs);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Remove, memberIDs);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(basePath, comment, authentication, eventLog);
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
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsLockChanged(new ItemsEventArgs<IDataBase>(authentication, items, metaData));
        }

        public void Dispose()
        {
            foreach (var item in this.ToArray<DataBase>())
            {
                {
                    var dataBaseInfo = (DataBaseSerializationInfo)item.DataBaseInfo;
                    var filename = FileUtility.Prepare(this.cachePath, $"{item.ID}{databaseExtension}");
                    JsonSerializerUtility.Write(filename, dataBaseInfo, true);
                }
                {
                    var dataBaseState = item.DataBaseState & DataBaseState.IsLoaded;
                    var filename = FileUtility.Prepare(this.cachePath, $"{item.ID}{stateExtension}");
                    JsonSerializerUtility.Write(filename, dataBaseState, true);
                }
                item.Dispose();
            }
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

        public DataBase AddFromPath(string path)
        {
            var dataBase = new DataBase(this.cremaHost, path, Path.GetFileName(path));
            this.AddBase(dataBase.Name, dataBase);
            return dataBase;
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.cremaHost.Dispatcher; }
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

        protected virtual void OnItemsAuthenticationEntered(ItemsEventArgs<IDataBase> e)
        {
            this.itemsAuthenticationEntered?.Invoke(this, e);
        }

        protected virtual void OnItemsAuthenticationLeft(ItemsEventArgs<IDataBase> e)
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

        private void ValidateCopyDataBase(Authentication authentication, DataBase dataBase, string newDataBaseName, bool force)
        {
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();

            var dataBasePath = Path.Combine(this.cremaHost.TagsPath, newDataBaseName);
            if (DirectoryUtility.Exists(dataBasePath) == true)
                throw new CremaException("'{0}' 경로가 이미 존재합니다.", newDataBaseName);

            if (this.ContainsKey(newDataBaseName) == true)
                throw new CremaException("'{0}'은(는) 이미 존재하는 데이터베이스입니다.", newDataBaseName);

            if (force == true)
                return;

            if (dataBase.IsLoaded == true)
            {
                var types = dataBase.TypeContext.Types.Where<Type>(item => item.TypeState != TypeState.None).ToArray();
                if (types.Any() == true)
                    throw new CremaException("타입이 편집중입니다. : '{0}'", string.Join(", ", types.Select(item => item.Name)));

                var tables = dataBase.TableContext.Tables.Where<Table>(item => item.TableState != TableState.None).ToArray();
                if (tables.Any() == true)
                    throw new CremaException("테이블이 설정중이거나 편집중입니다. : '{0}'", string.Join(", ", tables.Select(item => item.Name)));

                var domainContext = dataBase.GetService(typeof(DomainContext)) as DomainContext;
                var domains = domainContext.Domains.Where<Domain>(item => item.DataBaseID == dataBase.ID).ToArray();
                if (domains.Any() == true)
                    throw new CremaException("아직 저장되지 않은 작업이 남아 있습니다. : '{0}'", string.Join(", ", domains.Select(item => item.Host)));
            }
        }

        private void ValidateCreateDataBase(Authentication authentication, string dataBaseName)
        {
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();

            var dataBasePath = Path.Combine(this.cremaHost.TagsPath, dataBaseName);
            if (DirectoryUtility.Exists(dataBasePath) == true)
                throw new CremaException("'{0}' 경로가 이미 존재합니다.", dataBaseName);

            if (this.ContainsKey(dataBaseName) == true)
                throw new CremaException("'{0}'은(는) 이미 존재하는 데이터베이스입니다.", dataBaseName);
        }

        private void ValidateRenameDataBase(Authentication authentication, DataBase dataBase, string newDataBaseName)
        {
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();

            if (dataBase.ID == DataBase.defaultID)
                throw new PermissionDeniedException("default 데이터 베이스는 이름을 변경할 수 없습니다.");

            if (dataBase.IsLoaded == true)
                throw new CremaException("데이터 베이스가 사용중입니다.");

            var dataBasePath = Path.Combine(this.cremaHost.TagsPath, newDataBaseName);
            if (DirectoryUtility.Exists(dataBasePath) == true)
                throw new CremaException("'{0}' 경로가 이미 존재합니다.", newDataBaseName);

            if (this.ContainsKey(newDataBaseName) == true)
                throw new CremaException("'{0}'은(는) 이미 존재하는 데이터베이스입니다.", newDataBaseName);
        }

        private void ValidateDeleteDataBase(Authentication authentication, DataBase dataBase)
        {
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();

            if (dataBase.ID == DataBase.defaultID)
                throw new PermissionDeniedException("default 데이터 베이스는 삭제할 수 없습니다.");

            if (dataBase.IsLoaded == true)
                throw new CremaException("데이터 베이스가 사용중입니다.");
        }

        private Dictionary<string, DataBaseSerializationInfo> ReadCaches()
        {
            var caches = new Dictionary<string, DataBaseSerializationInfo>();
            var files = Directory.GetFiles(cachePath, $"*{databaseExtension}");
            foreach (var item in files)
            {
                try
                {
                    var dataBaseInfo = JsonSerializerUtility.Read<DataBaseSerializationInfo>(item);
                    caches.Add(dataBaseInfo.Name, dataBaseInfo);
                }
                catch (Exception e)
                {
                    this.cremaHost.Error(e);
                }
            }

            return caches;
        }

        private Dictionary<string, DataBaseState> ReadStateCaches()
        {
            var caches = new Dictionary<string, DataBaseState>();
            var files = Directory.GetFiles(cachePath, $"*{stateExtension}");
            foreach (var item in files)
            {
                try
                {
                    var dataBaseState = JsonSerializerUtility.Read<DataBaseState>(item);
                    caches.Add(Path.GetFileNameWithoutExtension(item), dataBaseState);
                }
                catch (Exception e)
                {
                    this.cremaHost.Error(e);
                }
            }

            return caches;
        }

        //private void WriteAccessInfo(string accessInfoPath, AccessInfo accessInfo)
        //{
        //    JsonSerializerUtility.Write(accessInfoPath, (AccessSerializationInfo)accessInfo, true);
        //}

        #region IDataBaseCollection

        IDataBase IDataBaseCollection.AddNewDataBase(Authentication authentication, string dataBaseName, string comment)
        {
            return this.CreateDataBase(authentication, dataBaseName, comment);
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
