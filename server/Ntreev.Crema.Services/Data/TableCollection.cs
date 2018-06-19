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
using Ntreev.Crema.Services.Properties;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;

#pragma warning disable 0612

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

        public Table AddNew(Authentication authentication, CremaTemplate template)
        {
            var dataSet = template.TargetTable.DataSet.Copy();
            var dataTable = dataSet.Tables[template.TableName, template.CategoryPath];
            var categoryPath = dataTable.CategoryPath;

            this.ValidateAddNew(dataTable.TableName, categoryPath, authentication);
            this.Sign(authentication);
            this.InvokeTableCreate(authentication, dataTable.TableName, dataSet, null);
            var table = this.AddNew(authentication, dataTable.TableName, categoryPath);
            table.Initialize(dataTable.TableInfo);
            this.InvokeTablesCreatedEvent(authentication, new Table[] { table }, dataSet);
            return table;
        }

        public Table Inherit(Authentication authentication, Table table, string newTableName, string categoryPath, bool copyContent)
        {
            this.ValidateInherit(authentication, table, newTableName, categoryPath, copyContent);
            this.Sign(authentication);
            var dataSet = table.ReadData(authentication);
            var dataTable = dataSet.Tables[table.Name, table.Category.Path];
            var itemName = new ItemName(categoryPath, newTableName);
            var newDataTable = dataTable.Inherit(itemName, copyContent);
            if (copyContent == false)
                newDataTable.Clear();
            newDataTable.CategoryPath = categoryPath;

            this.InvokeTableCreate(authentication, newTableName, dataSet, table);
            var newTable = this.AddNew(authentication, newTableName, categoryPath);
            newTable.TemplatedParent = table;
            newTable.Initialize(newDataTable.TableInfo);
            foreach (var item in newDataTable.Childs)
            {
                var childTable = this.AddNew(authentication, item.Name, categoryPath);
                childTable.TemplatedParent = table.Childs[item.TableName];
                childTable.Initialize(item.TableInfo);
            }
            var items = EnumerableUtility.Friends(newTable, newTable.Childs).ToArray();
            this.InvokeTablesCreatedEvent(authentication, items, dataTable.DataSet);
            return newTable;
        }

        public Table Copy(Authentication authentication, Table table, string newTableName, string categoryPath, bool copyContent)
        {
            this.ValidateCopy(authentication, table, newTableName, categoryPath, copyContent);
            this.Sign(authentication);
            var dataSet = table.ReadData(authentication);
            var dataTable = dataSet.Tables[table.Name, table.Category.Path];
            var itemName = new ItemName(categoryPath, newTableName);
            var newDataTable = dataTable.Copy(itemName, copyContent);
            if (copyContent == false)
                newDataTable.Clear();
            newDataTable.CategoryPath = categoryPath;

            this.InvokeTableCreate(authentication, newTableName, dataSet, table);
            var newTable = this.AddNew(authentication, newTableName, categoryPath);
            newTable.Initialize(newDataTable.TableInfo);
            foreach (var item in newDataTable.Childs)
            {
                var childTable = this.AddNew(authentication, item.Name, categoryPath);
                childTable.Initialize(item.TableInfo);
            }
            var items = EnumerableUtility.Friends(newTable, newTable.Childs).ToArray();
            this.InvokeTablesCreatedEvent(authentication, items, dataTable.DataSet);
            return newTable;
        }

        public object GetService(System.Type serviceType)
        {
            return this.DataBase.GetService(serviceType);
        }

        public void InvokeTableCreate(Authentication authentication, string tableName, CremaDataSet dataSet, Table sourceTable)
        {
            var message = EventMessageBuilder.CreateTable(authentication, tableName);
            try
            {
                this.Repository.CreateTable(dataSet, this.DataBase);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeTableRename(Authentication authentication, Table table, string newName, CremaDataSet dataSet)
        {
            var message = EventMessageBuilder.RenameTable(authentication, table.Name, newName);
            try
            {
                this.Repository.RenameTable(dataSet, table, newName);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeTableMove(Authentication authentication, Table table, string newCategoryPath, CremaDataSet dataSet)
        {
            var message = EventMessageBuilder.MoveTable(authentication, table.Name, newCategoryPath, table.Category.Path);
            try
            {
                this.Repository.MoveTable(dataSet, table, newCategoryPath);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeTableDelete(Authentication authentication, Table table, CremaDataSet dataSet)
        {
            var message = EventMessageBuilder.DeleteTable(authentication, table.Name);
            try
            {
                this.Repository.DeleteTable(dataSet, table);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeTableEndContentEdit(Authentication authentication, Table table, CremaDataSet dataSet)
        {
            var message = EventMessageBuilder.ChangeTableContent(authentication, table.Name);
            try
            {
                this.Repository.ModifyTable(dataSet, table);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeTableEndTemplateEdit(Authentication authentication, Table table, CremaTemplate template)
        {
            var dataTable = template.TargetTable;
            var dataSet = dataTable.DataSet;
            var message = EventMessageBuilder.ChangeTableTemplate(authentication, table.Name);
            try
            {
                this.Repository.ModifyTable(dataSet, table);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeTableSetTags(Authentication authentication, Table table, TagInfo tags, CremaDataSet dataSet)
        {
            var message = EventMessageBuilder.ChangeTableTemplate(authentication, table.Name);
            try
            {
                this.Repository.SetTableTags(dataSet, table, tags);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeTableSetComment(Authentication authentication, Table table, string comment, CremaDataSet dataSet)
        {
            var message = EventMessageBuilder.ChangeTableTemplate(authentication, table.Name);
            try
            {
                this.Repository.SetTableComment(dataSet, table, comment);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        //public void InvokeChildTableCreate(Authentication authentication, Table table, string childName, CremaDataSet dataSet)
        //{
        //    var message = EventMessageBuilder.CreateTable(authentication, childName);
        //    try
        //    {
        //        //var query = from item in dataSet.Tables
        //        //            where item.Name == childName || item.TemplatedParentName == childName
        //        //            select item;
        //        //foreach (var item in query)
        //        //{
        //        //    var props = new CremaDataTableSerializerSettings(item.Namespace, item.TemplateNamespace);
        //        //    var itemPath = this.Context.GenerateTablePath(item.CategoryPath, item.Name);
        //        //    var itemPaths = this.Serializer.Serialize(itemPath, item, props);
        //        //    this.Repository.AddRange(itemPaths);
        //        //}
        //        this.Repository.CreateTable(dataSet, this.DataBase);
        //        this.Repository.Commit(authentication, message);
        //        //this.CremaHost.Info(message);
        //    }
        //    catch
        //    {
        //        this.Repository.Revert();
        //        throw;
        //    }
        //}

        public void InvokeTablesCreatedEvent(Authentication authentication, Table[] tables, CremaDataSet dataSet)
        {
            var args = tables.Select(item => (object)item.TableInfo).ToArray();
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTablesCreatedEvent), tables);
            var message = EventMessageBuilder.CreateTable(authentication, tables);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnTablesCreated(new ItemsCreatedEventArgs<ITable>(authentication, tables, args, dataSet));
            this.Context.InvokeItemsCreatedEvent(authentication, tables, args, dataSet);
        }

        public void InvokeTablesRenamedEvent(Authentication authentication, Table[] tables, string[] oldNames, string[] oldPaths, CremaDataSet dataSet)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTablesRenamedEvent), tables, oldNames, oldPaths);
            var message = EventMessageBuilder.RenameTable(authentication, tables, oldNames);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnTablesRenamed(new ItemsRenamedEventArgs<ITable>(authentication, tables, oldNames, oldPaths, dataSet));
            this.Context.InvokeItemsRenamedEvent(authentication, tables, oldNames, oldPaths, dataSet);
        }

        public void InvokeTablesMovedEvent(Authentication authentication, Table[] tables, string[] oldPaths, string[] oldCategoryPaths, CremaDataSet dataSet)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTablesMovedEvent), tables, oldPaths, oldCategoryPaths);
            var message = EventMessageBuilder.MoveTable(authentication, tables, oldCategoryPaths);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnTablesMoved(new ItemsMovedEventArgs<ITable>(authentication, tables, oldPaths, oldCategoryPaths, dataSet));
            this.Context.InvokeItemsMovedEvent(authentication, tables, oldPaths, oldCategoryPaths, dataSet);
        }

        public void InvokeTablesDeletedEvent(Authentication authentication, Table[] tables, string[] oldPaths)
        {
            var dataSet = CremaDataSet.Create(new SignatureDateProvider(authentication.ID));
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTablesDeletedEvent), oldPaths);
            var message = EventMessageBuilder.DeleteTable(authentication, tables);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnTablesDeleted(new ItemsDeletedEventArgs<ITable>(authentication, tables, oldPaths, dataSet));
            this.Context.InvokeItemsDeletedEvent(authentication, tables, oldPaths, dataSet);
        }

        public void InvokeTablesStateChangedEvent(Authentication authentication, Table[] tables)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeTablesStateChangedEvent), tables);
            this.OnTablesStateChanged(new ItemsEventArgs<ITable>(authentication, tables));
        }

        public void InvokeTablesTemplateChangedEvent(Authentication authentication, Table[] tables, CremaDataSet dataSet)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTablesTemplateChangedEvent), tables);
            var message = EventMessageBuilder.ChangeTableTemplate(authentication, tables);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnTablesChanged(new ItemsEventArgs<ITable>(authentication, tables, dataSet));
            this.Context.InvokeItemsChangedEvent(authentication, tables, dataSet);
        }

        public void InvokeTablesContentChangedEvent(Authentication authentication, TableContent content, Table[] tables, CremaDataSet dataSet)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTablesContentChangedEvent), tables);
            var message = EventMessageBuilder.ChangeTableContent(authentication, tables);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnTablesChanged(new ItemsEventArgs<ITable>(authentication, tables));
            this.Context.InvokeItemsChangedEvent(authentication, tables, dataSet);
        }

        public DataBaseRepositoryHost Repository => this.DataBase.Repository;

        public CremaHost CremaHost => this.Context.CremaHost;

        public DataBase DataBase => this.Context.DataBase;

        public CremaDispatcher Dispatcher => this.Context?.Dispatcher;

        public IObjectSerializer Serializer => this.DataBase.Serializer;

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

        protected override void ValidateAddNew(string name, string categoryPath, object validation)
        {
            base.ValidateAddNew(name, categoryPath, validation);

            if (NameValidator.VerifyName(name) == false)
                throw new ArgumentException(string.Format(Resources.Exception_InvalidName_Format, name), nameof(name));

            if (validation is Authentication authentication)
            {
                var category = this.GetCategory(categoryPath);
                if (category == null)
                    throw new CategoryNotFoundException(categoryPath);
                category.ValidateAccessType(authentication, AccessType.Master);
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

        private void Sign(Authentication authentication)
        {
            authentication.Sign();
        }

        //private void ValidateCreate(Authentication authentication, string categoryPath)
        //{
        //    var category = this.GetCategory(categoryPath);

        //    if (category == null)
        //        throw new CategoryNotFoundException(categoryPath);

        //    category.ValidateAccessType(authentication, AccessType.Master);
        //}

        private void ValidateInherit(Authentication authentication, Table table, string newTableName, string categoryPath, bool copyXml)
        {
            table.ValidateAccessType(authentication, AccessType.Master);

            if (this.Contains(newTableName) == true)
                throw new ArgumentException(Resources.Exception_SameTableNameExist, nameof(newTableName));
            if (table.Parent != null)
                throw new InvalidOperationException(Resources.Exception_ChildTableCannotInherit);
            if (table.TemplatedParent != null)
                throw new InvalidOperationException(Resources.Exception_InheritedTableCannotInherit);

            NameValidator.ValidateCategoryPath(categoryPath);

            if (this.Context.Categories.Contains(categoryPath) == false)
                throw new CategoryNotFoundException(categoryPath);

            var category = this.Context.Categories[categoryPath];
            category.ValidateAccessType(authentication, AccessType.Master);

            if (copyXml == true)
            {
                foreach (var item in EnumerableUtility.Friends(table, table.Childs))
                {
                    item.ValidateHasNotBeingEditedType();
                }
            }
        }

        private void ValidateCopy(Authentication authentication, Table table, string newTableName, string categoryPath, bool copyXml)
        {
            table.ValidateAccessType(authentication, AccessType.Master);

            if (this.Contains(newTableName) == true)
                throw new ArgumentException(Resources.Exception_SameTableNameExist, nameof(newTableName));
            if (table.Parent != null)
                throw new InvalidOperationException(Resources.Exception_ChildTableCannotCopy);

            NameValidator.ValidateCategoryPath(categoryPath);

            if (this.Context.Categories.Contains(categoryPath) == false)
                throw new CategoryNotFoundException(categoryPath);

            var category = this.Context.Categories[categoryPath];
            category.ValidateAccessType(authentication, AccessType.Master);

            if (copyXml == true)
            {
                foreach (var item in EnumerableUtility.Friends(table, table.Childs))
                {
                    item.ValidateHasNotBeingEditedType();
                }
            }
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
