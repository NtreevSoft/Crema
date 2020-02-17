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
using Ntreev.Crema.Services.Properties;
using Ntreev.Library;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Ntreev.Crema.Services.Data
{
    class TableCollection : TableCollectionBase<Table, TableCategory, TableCollection, TableCategoryCollection, TableContext>, 
        ITableCollection
    {
        private ItemsCreatedEventHandler<ITable> tablesCreated;
        private ItemsRenamedEventHandler<ITable> tablesRenamed;
        private ItemsMovedEventHandler<ITable> tablesMoved;
        private ItemsDeletedEventHandler<ITable> tablesDeleted;
        private ItemsEventHandler<ITable> tablesChanged;
        private ItemsEventHandler<ITable> tablesStateChanged;

        public TableCollection()
        {
            
        }

        public Table AddNew(Authentication authentication, string name, string categoryPath)
        {
            if (NameValidator.VerifyName(name) == false)
                throw new ArgumentException(string.Format(Resources.Exception_InvalidName_Format, name), nameof(name));
            return this.BaseAddNew(name, categoryPath, authentication);
        }

        public Table AddNew(Authentication authentication, TableInfo tableInfo)
        {
            this.InvokeTableCreate(authentication, tableInfo.Name, tableInfo.CategoryPath, null);
            var table = this.AddNew(authentication, tableInfo.Name, tableInfo.CategoryPath);
            table.Initialize(tableInfo);
            this.InvokeTablesCreatedEvent(authentication, new Table[] { table });
            return table;
        }

        public Table Inherit(Authentication authentication, Table table, string newTableName, string categoryPath, bool copyContent)
        {
            var result = this.Service.InheritTable(table.Name, newTableName, categoryPath, copyContent);
            this.Sign(authentication, result);
            this.InvokeTableCreate(authentication, newTableName, categoryPath, table);
            var newTable = this.AddNew(authentication, newTableName, categoryPath);
            newTable.TemplatedParent = table;
            newTable.Initialize(result.Value.First());
            foreach (var item in result.Value.Skip(1))
            {
                var childTable = this.AddNew(authentication, item.Name, categoryPath);
                childTable.TemplatedParent = table.Childs[item.TableName];
                childTable.Initialize(item);
            }
            var items = EnumerableUtility.Friends(newTable, newTable.Childs).ToArray();
            this.InvokeTablesCreatedEvent(authentication, items);
            return newTable;
        }

        public Table Copy(Authentication authentication, Table table, string newTableName, string categoryPath, bool copyContent)
        {
            var result = this.Service.CopyTable(table.Name, newTableName, categoryPath, copyContent);
            this.Sign(authentication, result);
            this.InvokeTableCreate(authentication, newTableName, categoryPath, null);
            var newTable = this.AddNew(authentication, newTableName, categoryPath);
            newTable.Initialize(result.Value.First());
            foreach (var item in result.Value.Skip(1))
            {
                var childTable = this.AddNew(authentication, item.Name, categoryPath);
                childTable.Initialize(item);
            }
            var items = EnumerableUtility.Friends(newTable, newTable.Childs).ToArray();
            this.InvokeTablesCreatedEvent(authentication, items);
            return newTable;
        }

        public object GetService(System.Type serviceType)
        {
            return this.DataBase.GetService(serviceType);
        }

        public void InvokeTableCreate(Authentication authentication, string tableName, string categoryPath, Table sourceTable)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableCreate), tableName, categoryPath, sourceTable);
        }

        public void InvokeTableRename(Authentication authentication, Table table, string newName)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableRename), table, newName);
        }

        public void InvokeTableMove(Authentication authentication, Table table, string newCategoryPath)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableMove), table, newCategoryPath);
        }

        public void InvokeTableDelete(Authentication authentication, Table table)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableDelete), table);
        }

        public void InvokeEndContentEdit(Authentication authentication, Table table)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeEndContentEdit), table);
        }

        public void InvokeTableEndTemplateEdit(Authentication authentication, Table table)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableEndTemplateEdit), table);
        }

        public void InvokeTableSetTags(Authentication authentication, Table table, TagInfo tags)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableSetTags), table, tags);
        }

        public void InvokeTableSetComment(Authentication authentication, Table table, string comment)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTableSetComment), table, comment);
        }

        public void InvokeChildTableCreate(Authentication authentication, Table table)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeChildTableCreate), table);
        }

        public void InvokeTablesCreatedEvent(Authentication authentication, Table[] tables)
        {
            var args = tables.Select(item => (object)item.TableInfo).ToArray();
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTablesCreatedEvent), tables);
            var comment = EventMessageBuilder.CreateTable(authentication, tables);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.UpdateRevision(tables);
            this.UpdateTableDetailInfo(tables);
            this.OnTablesCreated(new ItemsCreatedEventArgs<ITable>(authentication, tables, args));
            this.Context.InvokeItemsCreatedEvent(authentication, tables, args);
        }

        public void InvokeTablesRenamedEvent(Authentication authentication, Table[] tables, string[] oldNames, string[] oldPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTablesRenamedEvent), tables, oldNames, oldPaths);
            var comment = EventMessageBuilder.RenameTable(authentication, tables, oldNames);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.UpdateRevision(tables);
            this.UpdateTableDetailInfo(tables);
            this.OnTablesRenamed(new ItemsRenamedEventArgs<ITable>(authentication, tables, oldNames, oldPaths));
            this.Context.InvokeItemsRenamedEvent(authentication, tables, oldNames, oldPaths);
        }

        public void InvokeTablesMovedEvent(Authentication authentication, Table[] tables, string[] oldPaths, string[] oldCategoryPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTablesMovedEvent), tables, oldPaths, oldCategoryPaths);
            var comment = EventMessageBuilder.MoveTable(authentication, tables, oldCategoryPaths);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.UpdateRevision(tables);
            this.UpdateTableDetailInfo(tables);
            this.OnTablesMoved(new ItemsMovedEventArgs<ITable>(authentication, tables, oldPaths, oldCategoryPaths));
            this.Context.InvokeItemsMovedEvent(authentication, tables, oldPaths, oldCategoryPaths);
        }

        public void InvokeTablesDeletedEvent(Authentication authentication, Table[] tables, string[] oldPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTablesDeletedEvent), oldPaths);
            var comment = EventMessageBuilder.DeleteTable(authentication, tables);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnTablesDeleted(new ItemsDeletedEventArgs<ITable>(authentication, tables, oldPaths));
            this.Context.InvokeItemsDeleteEvent(authentication, tables, oldPaths);
        }

        public void InvokeTablesStateChangedEvent(Authentication authentication, Table[] tables)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeTablesStateChangedEvent), tables);
            this.UpdateRevision(tables);
            this.OnTablesStateChanged(new ItemsEventArgs<ITable>(authentication, tables));
        }

        public void InvokeTablesTemplateChangedEvent(Authentication authentication, Table[] tables)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTablesTemplateChangedEvent), tables);
            var comment = EventMessageBuilder.ChangeTableTemplate(authentication, tables);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.UpdateRevision(tables);
            this.UpdateTableDetailInfo(tables);
            this.OnTablesChanged(new ItemsEventArgs<ITable>(authentication, tables));
            this.Context.InvokeItemsChangedEvent(authentication, tables);
        }

        public void InvokeTablesContentChangedEvent(Authentication authentication, TableContent content, Table[] tables)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTablesContentChangedEvent), tables);
            var comment = EventMessageBuilder.ChangeTableContent(authentication, tables);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.UpdateRevision(tables);
            this.UpdateTableDetailInfo(tables);
            this.OnTablesChanged(new ItemsEventArgs<ITable>(authentication, tables));
            this.Context.InvokeItemsChangedEvent(authentication, tables);
        }

        private void UpdateRevision(Table[] tables)
        {
            foreach (var table in tables)
            {
                if (table == null) continue;

                var revision = this.Service.GetTableRevision(table.Path);
                revision.Validate();

                table.UpdateRevision(revision.Value);
            }
        }

        internal void UpdateTableDetailInfo(Table[] tables)
        {
            foreach (var table in tables)
            {
                var tableDetailInfo = this.Service.GetTableDetailInfo(table.Name);
                tableDetailInfo.Validate();

                table.UpdateTableDetailInfo(tableDetailInfo.Value);
            }
        }

        public IDataBaseService Service
        {
            get { return this.Context.Service; }
        }

        public CremaHost CremaHost
        {
            get { return this.Context.CremaHost; }
        }

        public DataBase DataBase
        {
            get { return this.Context.DataBase; }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.Context?.Dispatcher; }
        }

        public new int Count
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.Count;
            }
        }

        public event ItemsCreatedEventHandler<ITable> TablesCreated
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.tablesCreated += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.tablesCreated -= value;
            }
        }

        public event ItemsRenamedEventHandler<ITable> TablesRenamed
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.tablesRenamed += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.tablesRenamed -= value;
            }
        }

        public event ItemsMovedEventHandler<ITable> TablesMoved
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.tablesMoved += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.tablesMoved -= value;
            }
        }

        public event ItemsDeletedEventHandler<ITable> TablesDeleted
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.tablesDeleted += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.tablesDeleted -= value;
            }
        }

        public event ItemsEventHandler<ITable> TablesChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.tablesChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.tablesChanged -= value;
            }
        }

        public event ItemsEventHandler<ITable> TablesStateChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.tablesStateChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.tablesStateChanged -= value;
            }
        }

        public new event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.CollectionChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.CollectionChanged -= value;
            }
        }

        protected virtual void OnTablesCreated(ItemsCreatedEventArgs<ITable> e)
        {
            this.tablesCreated?.Invoke(this, e);
        }

        protected virtual void OnTablesRenamed(ItemsRenamedEventArgs<ITable> e)
        {
            this.tablesRenamed?.Invoke(this, e);
        }

        protected virtual void OnTablesMoved(ItemsMovedEventArgs<ITable> e)
        {
            this.tablesMoved?.Invoke(this, e);
        }

        protected virtual void OnTablesDeleted(ItemsDeletedEventArgs<ITable> e)
        {
            this.tablesDeleted?.Invoke(this, e);
        }

        protected virtual void OnTablesStateChanged(ItemsEventArgs<ITable> e)
        {
            this.tablesStateChanged?.Invoke(this, e);
        }

        protected virtual void OnTablesChanged(ItemsEventArgs<ITable> e)
        {
            this.tablesChanged?.Invoke(this, e);
        }

        protected override Table NewItem(params object[] args)
        {
            return new Table();
        }

        private void Sign(Authentication authentication, ResultBase result)
        {
            result.Validate(authentication);
        }

        private void Sign<T>(Authentication authentication, ResultBase<T> result)
        {
            result.Validate(authentication);
        }

        #region ITableCollection

        bool ITableCollection.Contains(string tableName)
        {
            this.Dispatcher?.VerifyAccess();
            return this.Contains(tableName);
        }

        ITable ITableCollection.this[string tableName]
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this[tableName];
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator<ITable> IEnumerable<ITable>.GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            return this.GetEnumerator();
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
