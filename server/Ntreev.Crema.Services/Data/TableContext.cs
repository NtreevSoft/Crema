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
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.Data.Serializations;
using Ntreev.Crema.Services.Properties;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ntreev.Crema.Services.Data
{
    class TableContext : ItemContext<Table, TableCategory, TableCollection, TableCategoryCollection, TableContext>,
        ITableContext
    {
        private DataBase dataBase;
        private readonly DataBaseRepositoryHost repository;

        private ItemsCreatedEventHandler<ITableItem> itemsCreated;
        private ItemsRenamedEventHandler<ITableItem> itemsRenamed;
        private ItemsMovedEventHandler<ITableItem> itemsMoved;
        private ItemsDeletedEventHandler<ITableItem> itemsDeleted;
        private ItemsEventHandler<ITableItem> itemsChanged;
        private ItemsEventHandler<ITableItem> itemsAccessChanged;
        private ItemsEventHandler<ITableItem> itemsLockChanged;

        public TableContext(DataBase dataBase, IEnumerable<TableInfo> tableInfos)
        {
            this.dataBase = dataBase;
            this.repository = dataBase.Repository;
            this.CremaHost.Debug(Resources.Message_TableContextInitialize);
            this.BasePath = Path.Combine(dataBase.BasePath, CremaSchema.TableDirectory);
            this.Initialize(tableInfos);
            this.CremaHost.UserContext.Dispatcher.Invoke(() => this.CremaHost.UserContext.Users.UsersLoggedOut += Users_UsersLoggedOut);
            this.CremaHost.Debug(Resources.Message_TableContextIsCreated);
        }

        public void InvokeTableItemLock(Authentication authentication, ITableItem tableItem, string comment)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemLock), tableItem, comment);
        }

        public void InvokeTableItemUnlock(Authentication authentication, ITableItem tableItem)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemUnlock), tableItem);
        }

        public void InvokeTableItemSetPublic(Authentication authentication, ITableItem tableItem)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemSetPublic), tableItem);
            var message = EventMessageBuilder.SetPublicTableItem(authentication, tableItem.Path);
            try
            {
                var itemPath = this.GeneratePath(tableItem.Path);
                var itemPaths = this.Serializer.GetPath(itemPath, typeof(AccessSerializationInfo), AccessSerializationInfo.Settings);
                this.Repository.DeleteRange(itemPaths);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeTableItemSetPrivate(Authentication authentication, ITableItem tableItem, AccessInfo accessInfo)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemSetPrivate), tableItem);
            var message = EventMessageBuilder.SetPrivateTableItem(authentication, tableItem.Path);
            try
            {
                accessInfo.SetPrivate(tableItem.GetType().Name, authentication.SignatureDate);
                var itemPath = this.GeneratePath(tableItem.Path);
                var itemPaths = this.Serializer.Serialize(itemPath, (AccessSerializationInfo)accessInfo, AccessSerializationInfo.Settings);
                this.Repository.AddRange(itemPaths);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeTableItemAddAccessMember(Authentication authentication, ITableItem tableItem, AccessInfo accessInfo, string memberID, AccessType accessType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemAddAccessMember), tableItem, memberID, accessType);
            var message = EventMessageBuilder.AddAccessMemberToTableItem(authentication, tableItem.Path, memberID, accessType);
            try
            {
                accessInfo.Add(authentication.SignatureDate, memberID, accessType);
                var itemPath = this.GeneratePath(tableItem.Path);
                this.Serializer.Serialize(itemPath, (AccessSerializationInfo)accessInfo, AccessSerializationInfo.Settings);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeTableItemSetAccessMember(Authentication authentication, ITableItem tableItem, AccessInfo accessInfo, string memberID, AccessType accessType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemSetAccessMember), tableItem, memberID, accessType);
            var message = EventMessageBuilder.SetAccessMemberOfTableItem(authentication, tableItem.Path, memberID, accessType);
            try
            {
                accessInfo.Set(authentication.SignatureDate, memberID, accessType);
                var itemPath = this.GeneratePath(tableItem.Path);
                this.Serializer.Serialize(itemPath, (AccessSerializationInfo)accessInfo, AccessSerializationInfo.Settings);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeTableItemRemoveAccessMember(Authentication authentication, ITableItem tableItem, AccessInfo accessInfo, string memberID)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemRemoveAccessMember), tableItem, memberID);
            var message = EventMessageBuilder.RemoveAccessMemberFromTableItem(authentication, tableItem.Path, memberID);
            try
            {
                accessInfo.Remove(authentication.SignatureDate, memberID);
                var itemPath = this.GeneratePath(tableItem.Path);
                this.Serializer.Serialize(itemPath, (AccessSerializationInfo)accessInfo, AccessSerializationInfo.Settings);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void Import(Authentication authentication, CremaDataSet dataSet, string comment)
        {
            this.Dispatcher?.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Import), comment);
            this.ValidateImport(authentication, dataSet, comment);
            var query = from item in dataSet.Tables
                        let table = this.Tables[item.Name]
                        where table.LockInfo.Path != table.Path
                        select table;

            var items = query.ToArray();
            var comments = Enumerable.Repeat(comment, items.Length).ToArray();
            foreach (var item in items)
            {
                item.LockInternal(Authentication.System, comment);
            }
            this.InvokeItemsLockedEvent(authentication, items, comments);

            try
            {
                var targetSet = this.DataBase.GetDataSet(authentication, items, true);
                foreach (var item in targetSet.Tables)
                {
                    var dataTable = dataSet.Tables[item.Name];
                    if (dataTable == null)
                        continue;

                    foreach (var row in dataTable.Rows)
                    {
                        item.ImportRow(row);
                    }
                    item.ContentsInfo = dataTable.ContentsInfo;
                }
                try
                {
                    this.Repository.Modify(targetSet);
                    this.Repository.Commit(authentication, comment);
                }
                catch
                {
                    this.Repository.Revert();
                }

                foreach (var item in items)
                {
                    var dataTable = targetSet.Tables[item.Name, item.Category.Path];
                    item.UpdateContent(dataTable.TableInfo);
                }

                var eventLog = EventLogBuilder.Build(authentication, this, nameof(Import), comment);
                this.OnItemsChanged(new ItemsEventArgs<ITableItem>(Authentication.System, items, targetSet));
            }
            finally
            {
                foreach (var item in items)
                {
                    item.UnlockInternal(Authentication.System);
                }
                this.InvokeItemsUnlockedEvent(authentication, items);
            }
        }

        public void Dispose()
        {
            var userContext = this.CremaHost.UserContext;
            this.dataBase = null;
            userContext.Dispatcher.Invoke(() => userContext.Users.UsersLoggedOut -= Users_UsersLoggedOut);
        }

        public void InvokeItemsSetPublicEvent(Authentication authentication, ITableItem[] items)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsSetPublicEvent), items);
            var message = EventMessageBuilder.SetPublicTableItem(authentication, items);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Public);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsSetPrivateEvent(Authentication authentication, ITableItem[] items)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsSetPrivateEvent), items);
            var message = EventMessageBuilder.SetPrivateTableItem(authentication, items);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Private);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsAddAccessMemberEvent(Authentication authentication, ITableItem[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsAddAccessMemberEvent), items, memberIDs, accessTypes);
            var message = EventMessageBuilder.AddAccessMemberToTableItem(authentication, items, memberIDs, accessTypes);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Add, memberIDs, accessTypes);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsSetAccessMemberEvent(Authentication authentication, ITableItem[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsSetAccessMemberEvent), items, memberIDs, accessTypes);
            var message = EventMessageBuilder.SetAccessMemberOfTableItem(authentication, items, memberIDs, accessTypes);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Set, memberIDs, accessTypes);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsRemoveAccessMemberEvent(Authentication authentication, ITableItem[] items, string[] memberIDs)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsRemoveAccessMemberEvent), items, memberIDs);
            var message = EventMessageBuilder.RemoveAccessMemberFromTableItem(authentication, items, memberIDs);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Remove, memberIDs);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsLockedEvent(Authentication authentication, ITableItem[] items, string[] comments)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsLockedEvent), items, comments);
            var message = EventMessageBuilder.LockTableItem(authentication, items, comments);
            var metaData = EventMetaDataBuilder.Build(items, LockChangeType.Lock, comments);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnItemsLockChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsUnlockedEvent(Authentication authentication, ITableItem[] items)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsUnlockedEvent), items);
            var message = EventMessageBuilder.UnlockTableItem(authentication, items);
            var metaData = EventMetaDataBuilder.Build(items, LockChangeType.Unlock);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnItemsLockChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsCreatedEvent(Authentication authentication, ITableItem[] items, object[] args, object metaData)
        {
            this.OnItemsCreated(new ItemsCreatedEventArgs<ITableItem>(authentication, items, args, metaData));
        }

        public void InvokeItemsRenamedEvent(Authentication authentication, ITableItem[] items, string[] oldNames, string[] oldPaths, object metaData)
        {
            this.OnItemsRenamed(new ItemsRenamedEventArgs<ITableItem>(authentication, items, oldNames, oldPaths, metaData));
        }

        public void InvokeItemsMovedEvent(Authentication authentication, ITableItem[] items, string[] oldPaths, string[] oldParentPaths, object metaData)
        {
            this.OnItemsMoved(new ItemsMovedEventArgs<ITableItem>(authentication, items, oldPaths, oldParentPaths, metaData));
        }

        public void InvokeItemsDeletedEvent(Authentication authentication, ITableItem[] items, string[] itemPaths, object metaData)
        {
            this.OnItemsDeleted(new ItemsDeletedEventArgs<ITableItem>(authentication, items, itemPaths, metaData));
        }

        public void InvokeItemsChangedEvent(Authentication authentication, ITableItem[] items, object metaData)
        {
            this.OnItemsChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public LogInfo[] GetLog(string[] itemPaths)
        {
            return this.Repository.GetLog(itemPaths, null, 100);
        }

        public LogInfo[] GetCategoryLog(string localPath)
        {
            return this.Repository.GetLog(new string[] { localPath }, null, 100);
        }

        public string GenerateCategoryPath(string parentPath, string name)
        {
            var value = new CategoryName(parentPath, name);
            return this.GenerateCategoryPath(value.Path);
        }

        public string GenerateCategoryPath(string categoryPath)
        {
            NameValidator.ValidateCategoryPath(categoryPath);
            var baseUri = new Uri(this.BasePath);
            var uri = new Uri(baseUri + categoryPath);
            return uri.LocalPath;
        }

        public string GenerateTablePath(string categoryPath, string name)
        {
            return Path.Combine(this.GenerateCategoryPath(categoryPath), name);
        }

        public string GeneratePath(string path)
        {
            if (NameValidator.VerifyCategoryPath(path) == true)
                return this.GenerateCategoryPath(path);
            var itemName = new ItemName(path);
            return this.GenerateTablePath(itemName.CategoryPath, itemName.Name);
        }

        public string[] GetFiles(string itemPath)
        {
            var directoryName = Path.GetDirectoryName(itemPath);
            var name = Path.GetFileNameWithoutExtension(itemPath);
            var files = Directory.GetFiles(directoryName, $"{name}.*").Where(item => Path.GetFileNameWithoutExtension(item) == name).ToArray();
            return files;
        }

        public void ValidateTablePath(string categoryPath, string name, Table templatedParent)
        {
            var itemPath = this.GenerateTablePath(categoryPath, name);
            var settings = new CremaDataTableSerializerSettings(itemPath, templatedParent?.ItemPath);
            var itemPaths = this.Serializer.GetPath(itemPath, typeof(CremaDataTable), settings);
            foreach (var item in itemPaths)
            {
                this.DataBase.ValidateFileInfo(item);
            }
        }

        public void ValidateCategoryPath(string categoryPath)
        {
            this.DataBase.ValidateDirectoryInfo(this.GenerateCategoryPath(categoryPath));
        }

        public void LockItems(Authentication authentication, string[] itemPaths, string comment)
        {
            this.Dispatcher?.VerifyAccess();
            this.ValidateLockItems(authentication, itemPaths);
            var items = itemPaths.Select(item => this[item] as ITableItem).ToArray();

            foreach (var item in items)
            {
                item.Lock(authentication, comment);
            }

            authentication.Sign();
            this.OnItemsLockChanged(new ItemsEventArgs<ITableItem>(authentication, items));
        }

        public void UnlockItems(Authentication authentication, string userID)
        {
            this.Dispatcher?.VerifyAccess();
            this.ValidateUnlockItems(authentication, userID);
            this.Sign(authentication);
            var query = from ITableItem item in this
                        let lockInfo = item.LockInfo
                        where lockInfo.IsLocked == true && lockInfo.IsInherited == false && lockInfo.IsOwner(userID)
                        select item;

            var items = query.ToArray();
            if (items.Any() == false)
                return;

            foreach (var item in items)
            {
                item.Unlock(authentication);
            }

            var metaData = EventMetaDataBuilder.Build(items, LockChangeType.Unlock);
            this.OnItemsLockChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public object GetService(System.Type serviceType)
        {
            return this.DataBase.GetService(serviceType);
        }

        public TableCollection Tables => this.Items;

        public DataBase DataBase
        {
            get
            {
                if (this.dataBase == null)
                    throw new InvalidOperationException(Resources.Exception_InvalidObject);
                return this.dataBase;
            }
        }

        public CremaHost CremaHost => this.DataBase.CremaHost;

        public CremaDispatcher Dispatcher => this.dataBase?.Dispatcher;

        public IObjectSerializer Serializer => this.DataBase.Serializer;

        public string BasePath { get; }

        public event ItemsCreatedEventHandler<ITableItem> ItemsCreated
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsCreated += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsCreated -= value;
            }
        }

        public event ItemsRenamedEventHandler<ITableItem> ItemsRenamed
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsRenamed += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsRenamed -= value;
            }
        }

        public event ItemsMovedEventHandler<ITableItem> ItemsMoved
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsMoved += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsMoved -= value;
            }
        }

        public event ItemsDeletedEventHandler<ITableItem> ItemsDeleted

        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsDeleted += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsDeleted -= value;
            }
        }

        public event ItemsEventHandler<ITableItem> ItemsChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsChanged -= value;
            }
        }

        public event ItemsEventHandler<ITableItem> ItemsAccessChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsAccessChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsAccessChanged -= value;
            }
        }

        public event ItemsEventHandler<ITableItem> ItemsLockChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsLockChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsLockChanged -= value;
            }
        }

        protected virtual void OnItemsCreated(ItemsCreatedEventArgs<ITableItem> e)
        {
            this.itemsCreated?.Invoke(this, e);
        }

        protected virtual void OnItemsRenamed(ItemsRenamedEventArgs<ITableItem> e)
        {
            this.itemsRenamed?.Invoke(this, e);
        }

        protected virtual void OnItemsMoved(ItemsMovedEventArgs<ITableItem> e)
        {
            this.itemsMoved?.Invoke(this, e);
        }

        protected virtual void OnItemsDeleted(ItemsDeletedEventArgs<ITableItem> e)
        {
            this.itemsDeleted?.Invoke(this, e);
        }

        protected virtual void OnItemsChanged(ItemsEventArgs<ITableItem> e)
        {
            this.itemsChanged?.Invoke(this, e);
        }

        protected virtual void OnItemsAccessChanged(ItemsEventArgs<ITableItem> e)
        {
            this.itemsAccessChanged?.Invoke(this, e);
        }

        protected virtual void OnItemsLockChanged(ItemsEventArgs<ITableItem> e)
        {
            this.itemsLockChanged?.Invoke(this, e);
        }

        private void Users_UsersLoggedOut(object sender, ItemsEventArgs<IUser> e)
        {
            var userIDs = e.Items.Select(item => item.ID).ToArray();
            this.Dispatcher.InvokeAsync(() =>
            {
                if (this.dataBase == null)
                    return;
                foreach (var item in userIDs)
                {
                    this.UnlockItems(Authentication.System, item);
                }
            });
        }

        private void ValidateLockItems(Authentication authentication, string[] itemPaths)
        {
            var items = itemPaths.Select(item => this[item]);

            foreach (var item in items)
            {
                if (item is IPermission permission)
                {
                    if (permission.VerifyAccessType(authentication, AccessType.Master) == false)
                        throw new PermissionDeniedException();
                }

                {

                    var tableItem = item as ITableItem;
                    if (tableItem.IsLocked == true && tableItem.LockInfo.Path == tableItem.Path)
                        throw new PermissionException(string.Format(Resources.Exception_ItemIsAlreadyLocked_Format, item.Path));
                }
            }
        }

        private void ValidateUnlockItems(Authentication authentication, string userID)
        {
            foreach (ITableItem item in this)
            {
                if (item.LockInfo.UserID == userID && item.VerifyAccessType(authentication, AccessType.Master) == false)
                {
                    throw new PermissionDeniedException();
                }
            }
        }

        private void ValidateImport(Authentication authentication, CremaDataSet dataSet, string comment)
        {
            if (dataSet == null)
                throw new ArgumentNullException(nameof(dataSet));
            if (dataSet.Tables.Any() == false)
                throw new ArgumentException(Resources.Exception_EmptyDataSetCannotImport, nameof(dataSet));
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));
            if (comment == string.Empty)
                throw new ArgumentException(Resources.Exception_EmptyStringIsNotAllowed);
            var tableList = new List<Table>(dataSet.Tables.Count);
            foreach (var item in dataSet.Tables)
            {
                var table = this.Tables[item.Name];
                if (table == null)
                    throw new TableNotFoundException(item.Name);
                tableList.Add(table);
                table.ValidateAccessType(authentication, AccessType.Editor);
                table.ValidateHasNotBeingEditedType();
                table.ValidateNotBeingEdited();
            }

            var query = from item in tableList
                        where item.LockInfo.Path != item.Path
                        select item;

            foreach (var item in query)
            {
                item.ValidateLockInternal(authentication);
            }
        }

        private void Initialize(IEnumerable<TableInfo> tableInfos)
        {
            this.CremaHost.Debug(Resources.Message_LoadTables);
            var directories = DirectoryUtility.GetAllDirectories(this.BasePath);
            foreach (var item in directories)
            {
                var categoryName = CategoryName.Create(UriUtility.MakeRelativeOfDirectory(this.BasePath, item));
                this.Categories.Prepare(categoryName.Path);
            }

            foreach (var item in tableInfos.OrderBy(i => i.Name))
            {
                var table = this.Tables.AddNew(Authentication.System, item.Name, item.CategoryPath);
                table.Initialize(item);
            }

            foreach (var item in tableInfos.Where(i => i.TemplatedParent != string.Empty))
            {
                var table = this.Tables[item.Name];
                table.TemplatedParent = this.Tables[item.TemplatedParent];
                if (table.TemplatedParent == null)
                {
                    throw new Exception();
                }
            }

            var itemPaths = this.Serializer.GetItemPaths(this.BasePath, typeof(AccessSerializationInfo), AccessSerializationInfo.Settings);
            foreach (var item in itemPaths)
            {
                var accessInfo = (AccessSerializationInfo)this.Serializer.Deserialize(item, typeof(AccessSerializationInfo), AccessSerializationInfo.Settings);
                var tableItem = this.GetTableItemByItemPath(item);
                if (tableItem is Table table)
                {
                    table.SetAccessInfo((AccessInfo)accessInfo);
                }
                else if (tableItem is TableCategory category)
                {
                    category.SetAccessInfo((AccessInfo)accessInfo);
                }
            }
            this.CremaHost.Debug(Resources.Message_TableLoadingIsCompleted);
        }

        private void Sign(Authentication authentication)
        {
            authentication.Sign();
        }

        private ITableItem GetTableItemByItemPath(string itemPath)
        {
            var isCategory = itemPath.EndsWith($"{Path.DirectorySeparatorChar}");
            var directory = Path.GetDirectoryName(itemPath);
            var relativeUri = UriUtility.MakeRelativeOfDirectory(this.BasePath, itemPath);
            var segments = StringUtility.Split(relativeUri, PathUtility.SeparatorChar, true);
            var path = isCategory == true ? (string)CategoryName.Create(segments) : ItemName.Create(segments);
            return this[path] as ITableItem;
        }

        private DataBaseRepositoryHost Repository => this.DataBase.Repository;

        #region ITableContext

        bool ITableContext.Contains(string itemPath)
        {
            this.Dispatcher?.VerifyAccess();
            return this.Contains(itemPath);
        }

        ITableCollection ITableContext.Tables
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Tables;
            }
        }

        ITableCategoryCollection ITableContext.Categories
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Categories;
            }
        }

        ITableCategory ITableContext.Root
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Root;
            }
        }

        ITableItem ITableContext.this[string itemPath]
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this[itemPath] as ITableItem;
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator<ITableItem> IEnumerable<ITableItem>.GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            foreach (var item in this)
            {
                yield return item as ITableItem;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            foreach (var item in this)
            {
                yield return item as ITableItem;
            }
        }

        #endregion

        #region IServiceProvider

        object IServiceProvider.GetService(System.Type serviceType)
        {
            return (this.DataBase as IDataBase).GetService(serviceType);
        }

        #endregion
    }
}
