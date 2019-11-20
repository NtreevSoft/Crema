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
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Data.Xml;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Services;
using Ntreev.Crema.Services.Properties;
using Ntreev.Crema.Services.Users;
using Ntreev.Crema.Data;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Ntreev.Library.Serialization;
using Ntreev.Library;

namespace Ntreev.Crema.Services.Data
{
    class TableContext : ItemContext<Table, TableCategory, TableCollection, TableCategoryCollection, TableContext>,
        ITableContext
    {
        private DataBase dataBase;
        private readonly DataBaseRepositoryHost repository;
        private readonly string basePath;

        private ItemsCreatedEventHandler<ITableItem> itemsCreated;
        private ItemsRenamedEventHandler<ITableItem> itemsRenamed;
        private ItemsMovedEventHandler<ITableItem> itemsMoved;
        private ItemsDeletedEventHandler<ITableItem> itemsDeleted;
        private ItemsEventHandler<ITableItem> itemsChanged;
        private ItemsEventHandler<ITableItem> itemsAccessChanged;
        private ItemsEventHandler<ITableItem> itemsLockChanged;
        private ItemsEventHandler<ITableItem> itemsRevisionChanged;

        public TableContext(DataBase dataBase, IEnumerable<TableInfo> tableInfos)
        {
            this.dataBase = dataBase;
            this.repository = dataBase.Repository;
            this.CremaHost.Debug(Resources.Message_TableContextInitialize);
            this.basePath = Path.Combine(dataBase.BasePath, CremaSchema.TableDirectory);
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

        public void InvokeTableItemSetPrivate(Authentication authentication, ITableItem tableItem, AccessInfo accessInfo)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemSetPrivate), tableItem);
            var accessInfoPath = tableItem.GetAccessInfoPath();
            try
            {
                accessInfo.SetPrivate(tableItem.GetType().Name, authentication.SignatureDate);
                tableItem.WriteAccessInfo(accessInfoPath, accessInfo);
                this.repository.Add(accessInfoPath);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert();
                throw e;
            }
        }

        public void InvokeTableItemSetPublic(Authentication authentication, ITableItem tableItem, AccessInfo accessInfo)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemSetPrivate), tableItem);
            var accessInfoPath = tableItem.GetAccessInfoPath();
            try
            {
                accessInfo.SetPublic();
                this.repository.Delete(accessInfoPath);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert();
                throw e;
            }
        }

        public void InvokeTableItemAddAccessMember(Authentication authentication, ITableItem tableItem, AccessInfo accessInfo, string memberID, AccessType accessType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemAddAccessMember), tableItem, memberID, accessType);
            var accessInfoPath = tableItem.GetAccessInfoPath();
            try
            {
                accessInfo.Add(authentication.SignatureDate, memberID, accessType);
                tableItem.WriteAccessInfo(accessInfoPath, accessInfo);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert();
                throw e;
            }
        }

        public void InvokeTableItemSetAccessMember(Authentication authentication, ITableItem tableItem, AccessInfo accessInfo, string memberID, AccessType accessType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemSetAccessMember), tableItem, memberID, accessType);
            var accessInfoPath = tableItem.GetAccessInfoPath();
            try
            {
                accessInfo.Set(authentication.SignatureDate, memberID, accessType);
                tableItem.WriteAccessInfo(accessInfoPath, accessInfo);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert();
                throw e;
            }
        }

        public void InvokeTableItemRemoveAccessMember(Authentication authentication, ITableItem tableItem, AccessInfo accessInfo, string memberID)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemRemoveAccessMember), tableItem, memberID);
            var accessInfoPath = tableItem.GetAccessInfoPath();
            try
            {
                accessInfo.Remove(authentication.SignatureDate, memberID);
                tableItem.WriteAccessInfo(accessInfoPath, accessInfo);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert();
                throw e;
            }
        }

        public void InvokeTableItemCreate(Authentication authentication, string path)
        {

        }

        public void InvokeTableItemRename(Authentication authentication, ITableItem tableItem, string newName)
        {
            if (tableItem.AccessInfo.Path == tableItem.Path)
            {
                var accessInfoPath1 = tableItem.GetAccessInfoPath();
                var directoryName = Path.GetDirectoryName(accessInfoPath1);
                var extension = Path.GetExtension(accessInfoPath1);
                var accessInfoPath2 = Path.Combine(directoryName, newName + extension);
                if (File.Exists(accessInfoPath1) == true && accessInfoPath1 != accessInfoPath2)
                {
                    this.repository.Move(accessInfoPath1, accessInfoPath2);
                }
            }
        }

        public void InvokeTableItemMove(Authentication authentication, ITableItem tableItem, string newCategoryPath)
        {
            if (tableItem.AccessInfo.Path == tableItem.Path)
            {
                var accessInfoPath1 = tableItem.GetAccessInfoPath();
                var accessInfoPath2 = tableItem.GetAccessInfoPath(newCategoryPath);
                if (File.Exists(accessInfoPath1) == true)
                {
                    this.repository.Move(accessInfoPath1, accessInfoPath2);
                }
            }
        }

        public void InvokeTableItemDelete(Authentication authentication, ITableItem tableItem)
        {
            if (tableItem.AccessInfo.Path == tableItem.Path)
            {
                var accessInfoPath = tableItem.GetAccessInfoPath();
                if (File.Exists(accessInfoPath) == true)
                {
                    this.repository.Delete(accessInfoPath);
                }
            }
        }

        public void InvokeTableItemChange(Authentication authentication, ITableItem tableItem)
        {

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
                var targetSet = new CremaDataSet();
                foreach (var item in items.Where(i => i.Parent == null))
                {
                    var targetTable = item.ReadSchema(authentication, targetSet);

                    var dataTable = dataSet.Tables[targetTable.TableName];
                    if (dataTable == null)
                        continue;

                    foreach (var row in dataTable.Rows)
                    {
                        targetTable.ImportRow(row);
                    }
                    targetTable.ContentsInfo = dataTable.ContentsInfo;
                }

                var dataTables = new DataTableCollection(targetSet, this.DataBase);
                try
                {
                    dataTables.Modify(this.Repository);
                }
                catch
                {

                }

                foreach (var item in items)
                {
                    var dataTable = targetSet.Tables[item.Name, item.Category.Path];
                    item.UpdateContent(dataTable.TableInfo);
                }

                var eventLog = EventLogBuilder.Build(authentication, this, nameof(Import), comment);
                this.Repository.Commit(authentication, comment, eventLog);
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
            var comment = EventMessageBuilder.SetPublicTableItem(authentication, items);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Public);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsSetPrivateEvent(Authentication authentication, ITableItem[] items)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsSetPrivateEvent), items);
            var comment = EventMessageBuilder.SetPrivateTableItem(authentication, items);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Private);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsAddAccessMemberEvent(Authentication authentication, ITableItem[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsAddAccessMemberEvent), items, memberIDs, accessTypes);
            var comment = EventMessageBuilder.AddAccessMemberToTableItem(authentication, items, memberIDs, accessTypes);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Add, memberIDs, accessTypes);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsSetAccessMemberEvent(Authentication authentication, ITableItem[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsSetAccessMemberEvent), items, memberIDs, accessTypes);
            var comment = EventMessageBuilder.SetAccessMemberOfTableItem(authentication, items, memberIDs, accessTypes);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Set, memberIDs, accessTypes);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsRemoveAccessMemberEvent(Authentication authentication, ITableItem[] items, string[] memberIDs)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsRemoveAccessMemberEvent), items, memberIDs);
            var comment = EventMessageBuilder.RemoveAccessMemberFromTableItem(authentication, items, memberIDs);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Remove, memberIDs);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsLockedEvent(Authentication authentication, ITableItem[] items, string[] comments)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsLockedEvent), items, comments);
            var comment = EventMessageBuilder.LockTableItem(authentication, items, comments);
            var metaData = EventMetaDataBuilder.Build(items, LockChangeType.Lock, comments);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsLockChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsUnlockedEvent(Authentication authentication, ITableItem[] items)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsUnlockedEvent), items);
            var comment = EventMessageBuilder.UnlockTableItem(authentication, items);
            var metaData = EventMetaDataBuilder.Build(items, LockChangeType.Unlock);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsLockChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsCreatedEvent(Authentication authentication, ITableItem[] items, object[] args, object metaData)
        {
            this.OnItemsCreated(new ItemsCreatedEventArgs<ITableItem>(authentication, items, args, metaData));
        }

        public void InvokeItemsRenamedEvent(Authentication authentication, ITableItem[] items, string[] oldNames, string[] oldPaths, object metaData)
        {
            //this.WriteAccessInfos();
            this.OnItemsRenamed(new ItemsRenamedEventArgs<ITableItem>(authentication, items, oldNames, oldPaths, metaData));
        }

        public void InvokeItemsMovedEvent(Authentication authentication, ITableItem[] items, string[] oldPaths, string[] oldParentPaths, object metaData)
        {
            //this.WriteAccessInfos();
            this.OnItemsMoved(new ItemsMovedEventArgs<ITableItem>(authentication, items, oldPaths, oldParentPaths, metaData));
        }

        public void InvokeItemsDeletedEvent(Authentication authentication, ITableItem[] items, string[] itemPaths, object metaData)
        {
            //this.WriteAccessInfos();
            this.OnItemsDeleted(new ItemsDeletedEventArgs<ITableItem>(authentication, items, itemPaths, metaData));
        }

        public void InvokeItemsChangedEvent(Authentication authentication, ITableItem[] items, object metaData)
        {
            this.OnItemsChanged(new ItemsEventArgs<ITableItem>(authentication, items, metaData));
        }

        public void InvokeItemsRevisionChangedEvent(Authentication authentication, ITableItem[] items)
        {
            this.OnItemsRevisionChanged(new ItemsEventArgs<ITableItem>(authentication, items));
        }

        public LogInfo[] GetLog(string xmlPath, string schemaPath)
        {
            var revision = this.Repository.GetRevision(xmlPath);
            var schemaLogs = this.Repository.GetLog(schemaPath, revision, 100);
            var xmlLogs = this.Repository.GetLog(xmlPath, revision, 100);

            var logs = xmlLogs.ToList();
            foreach (var item in schemaLogs)
            {
                if (logs.Any(i => i.Revision == item.Revision))
                    continue;
                logs.Add(item);
            }

            return logs.OrderByDescending(item => item.Revision).Take(100).ToArray();
        }

        public LogInfo[] GetCategoryLog(string localPath)
        {
            var revision = this.Repository.GetRevision(localPath);
            return this.Repository.GetLog(localPath, revision, 100);
        }

        public string GenerateCategoryPath(string parentPath, string name)
        {
            var value = new CategoryName(parentPath, name);
            return this.GenerateCategoryPath(value.Path);
        }

        public string GenerateCategoryPath(string categoryPath)
        {
            NameValidator.ValidateCategoryPath(categoryPath);
            var baseUri = new Uri(this.basePath);
            var uri = new Uri(baseUri + categoryPath.TrimEnd(PathUtility.SeparatorChar));
            return uri.LocalPath;
        }

        public string GenerateTableXmlPath(string categoryPath, string name)
        {
            return Path.Combine(this.GenerateCategoryPath(categoryPath), name + CremaSchema.XmlExtension);
        }

        public string GenerateTableSchemaPath(string categoryPath, string name)
        {
            return Path.Combine(this.GenerateCategoryPath(categoryPath), name + CremaSchema.SchemaExtension);
        }

        public string GeneratePath(string parentPath, string name, string extension)
        {
            return Path.Combine(this.basePath, parentPath.Replace('/', '\\'), name + extension);
        }

        public void ValidateTableXmlPath(string categoryPath, string name)
        {
            this.DataBase.ValidateFileInfo(this.GenerateTableXmlPath(categoryPath, name));
        }

        public void ValidateTableSchemaPath(string categoryPath, string name)
        {
            this.DataBase.ValidateFileInfo(this.GenerateTableSchemaPath(categoryPath, name));
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

        public TableCollection Tables
        {
            get { return this.Items; }
        }

        public DataBase DataBase
        {
            get
            {
                if (this.dataBase == null)
                    throw new InvalidOperationException(Resources.Exception_InvalidObject);
                return this.dataBase;
            }
        }

        public CremaHost CremaHost
        {
            get { return this.DataBase.CremaHost; }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.dataBase?.Dispatcher; }
        }

        public string BasePath
        {
            get { return this.basePath; }
        }

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

        public event ItemsEventHandler<ITableItem> ItemsRevisionChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsRevisionChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsRevisionChanged -= value;
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

        protected virtual void OnItemsRevisionChanged(ItemsEventArgs<ITableItem> e)
        {
            this.itemsRevisionChanged?.Invoke(this, e);
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
            var directories = DirectoryUtility.GetAllDirectories(this.basePath);
            foreach (var item in directories)
            {
                var categoryName = CategoryName.Create(UriUtility.MakeRelativeOfDirectory(this.basePath, item));
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
            foreach (ITableItem item in this)
            {
                var accessInfoPath = item.GetAccessInfoPath();
                try
                {
                    item.ReadAccessInfo(accessInfoPath);
                }
                catch (Exception e)
                {
                    this.CremaHost.Error(e);
                }
            }
            this.CremaHost.Debug(Resources.Message_TableLoadingIsCompleted);
        }

        private void Sign(Authentication authentication)
        {
            authentication.Sign();
        }

        private DataBaseRepositoryHost Repository
        {
            get { return this.DataBase.Repository; }
        }

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
