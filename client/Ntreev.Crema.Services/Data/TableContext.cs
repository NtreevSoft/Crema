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

using Ntreev.Crema.Services;
using Ntreev.Crema.Services.DataBaseService;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Data.Xml;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.IO;
using System.Collections.Generic;
using Ntreev.Library;
using Ntreev.Crema.Services.Properties;

namespace Ntreev.Crema.Services.Data
{
    class TableContext : ItemContext<Table, TableCategory, TableCollection, TableCategoryCollection, TableContext>,
        ITableContext
    {
        private DataBase dataBase;

        private ItemsCreatedEventHandler<ITableItem> itemsCreated;
        private ItemsRenamedEventHandler<ITableItem> itemsRenamed;
        private ItemsMovedEventHandler<ITableItem> itemsMoved;
        private ItemsDeletedEventHandler<ITableItem> itemsDeleted;
        private ItemsEventHandler<ITableItem> itemsChanged;
        private ItemsEventHandler<ITableItem> itemsAccessChanged;
        private ItemsEventHandler<ITableItem> itemsLockChanged;

        public TableContext(DataBase dataBase, DataBaseMetaData metaData)
        {
            this.dataBase = dataBase;
            this.Initialize(metaData);
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
        }

        public void InvokeTableItemSetPublic(Authentication authentication, ITableItem tableItem, AccessInfo accessInfo)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemSetPrivate), tableItem);
        }

        public void InvokeTableItemAddAccessMember(Authentication authentication, ITableItem tableItem, AccessInfo accessInfo, string memberID, AccessType accessType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemAddAccessMember), tableItem, memberID, accessType);
        }

        public void InvokeTableItemSetAccessMember(Authentication authentication, ITableItem tableItem, AccessInfo accessInfo, string memberID, AccessType accessType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemSetAccessMember), tableItem, memberID, accessType);
        }

        public void InvokeTableItemRemoveAccessMember(Authentication authentication, ITableItem tableItem, AccessInfo accessInfo, string memberID)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableItemRemoveAccessMember), tableItem, memberID);
        }

        public void InvokeTableItemCreate(Authentication authentication, string path)
        {

        }

        public void InvokeTableItemRename(Authentication authentication, ITableItem tableItem, string newName)
        {

        }

        public void InvokeTableItemMove(Authentication authentication, ITableItem tableItem, string newCategoryPath)
        {

        }

        public void InvokeTableItemDelete(Authentication authentication, ITableItem tableItem)
        {

        }

        public void InvokeTableItemChange(Authentication authentication, ITableItem tableItem)
        {

        }

        public void Import(Authentication authentication, CremaDataSet dataSet, string comment)
        {
            this.Dispatcher?.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Import), comment);

            var result = this.Service.ImportTables(dataSet, comment);
            result.Validate(authentication);
        }

        public void Dispose()
        {
            this.dataBase = null;
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

        public void InvokeItemsCreatedEvent(Authentication authentication, ITableItem[] items, object[] args)
        {
            this.OnItemsCreated(new ItemsCreatedEventArgs<ITableItem>(authentication, items, args));
        }

        public void InvokeItemsRenamedEvent(Authentication authentication, ITableItem[] items, string[] oldNames, string[] oldPaths)
        {
            this.OnItemsRenamed(new ItemsRenamedEventArgs<ITableItem>(authentication, items, oldNames, oldPaths));
        }

        public void InvokeItemsMovedEvent(Authentication authentication, ITableItem[] items, string[] oldPaths, string[] oldParentPaths)
        {
            this.OnItemsMoved(new ItemsMovedEventArgs<ITableItem>(authentication, items, oldPaths, oldParentPaths));
        }

        public void InvokeItemsDeleteEvent(Authentication authentication, ITableItem[] items, string[] itemPaths)
        {
            this.OnItemsDeleted(new ItemsDeletedEventArgs<ITableItem>(authentication, items, itemPaths));
        }

        public void InvokeItemsChangedEvent(Authentication authentication, ITableItem[] items)
        {
            this.OnItemsChanged(new ItemsEventArgs<ITableItem>(authentication, items));
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

        public IDataBaseService Service => this.dataBase.Service;

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

        private void Initialize(DataBaseMetaData metaData)
        {
            foreach (var item in metaData.TableCategories)
            {
                if (item.Path == this.Root.Path)
                    continue;
                this.Categories.Prepare(item.Path);
            }

            foreach (var item in metaData.Tables)
            {
                var tableInfo = item.TableInfo;
                var tableState = item.TableState;
                var table = this.Tables.AddNew(Authentication.System, tableInfo.Name, tableInfo.CategoryPath);
                table.Initialize(tableInfo);
                table.SetTableState(tableState);
            }

            foreach (var item in metaData.Tables)
            {
                var tableInfo = item.TableInfo;
                if (tableInfo.TemplatedParent == string.Empty)
                    continue;
                var table = this.Tables[tableInfo.Name];
                table.TemplatedParent = this.Tables[tableInfo.TemplatedParent];
                if (table.TemplatedParent == null)
                {
                    throw new Exception();
                }
            }

            foreach (var item in metaData.TableCategories)
            {
                var category = this.Categories[item.Path];

                if (item.AccessInfo.IsPrivate == true && item.AccessInfo.IsInherited == false)
                {
                    category.SetAccessInfo(item.AccessInfo);
                }

                if (item.LockInfo.IsLocked == true && item.LockInfo.IsInherited == false)
                {
                    category.SetLockInfo(item.LockInfo);
                }
            }

            foreach (var item in metaData.Tables)
            {
                var itemName = new ItemName(item.Path);
                var table = this.Tables[itemName.Name];

                if (item.AccessInfo.IsPrivate == true && item.AccessInfo.IsInherited == false)
                {
                    table.SetAccessInfo(item.AccessInfo);
                }

                if (item.LockInfo.IsLocked == true && item.LockInfo.IsInherited == false)
                {
                    table.SetLockInfo(item.LockInfo);
                }
            }
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
