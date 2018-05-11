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
using Ntreev.Crema.Data.Xml;
using Ntreev.Crema.Data.Xml.Schema;
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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ntreev.Crema.Services.Data
{
    class TableCategoryCollection : CategoryContainer<Table, TableCategory, TableCollection, TableCategoryCollection, TableContext>,
        ITableCategoryCollection
    {
        private ItemsCreatedEventHandler<ITableCategory> categoriesCreated;
        private ItemsRenamedEventHandler<ITableCategory> categoriesRenamed;
        private ItemsMovedEventHandler<ITableCategory> categoriesMoved;
        private ItemsDeletedEventHandler<ITableCategory> categoriesDeleted;

        public TableCategoryCollection()
        {

        }

        public TableCategory AddNew(Authentication authentication, string name, string parentPath)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.ValidateAddNew(authentication, name, parentPath);
            this.Sign(authentication);
            this.InvokeCategoryCreate(authentication, name, parentPath);
            var category = this.BaseAddNew(name, parentPath, authentication);
            var items = EnumerableUtility.One(category).ToArray();
            this.InvokeCategoriesCreatedEvent(authentication, items);
            return category;
        }

        public object GetService(System.Type serviceType)
        {
            return this.DataBase.GetService(serviceType);
        }

        public void InvokeCategoryCreate(Authentication authentication, string name, string parentPath)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryCreate), name, parentPath);

            var parent = this[parentPath];
            parent.ValidateAccessType(authentication, AccessType.Master);

            var path = this.Context.GenerateCategoryPath(parentPath, name);
            if (Directory.Exists(path) == true)
                throw new InvalidOperationException(Resources.Exception_SameNamePathExists);

            try
            {
                Directory.CreateDirectory(path);
                this.Repository.Add(path);
                this.Context.InvokeTableItemCreate(authentication, parentPath + name);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                if (Directory.Exists(path) == true)
                    Directory.Delete(path);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeCategoryRename(Authentication authentication, TableCategory category, string name, CremaDataSet dataSet)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryRename), category, name);

            var categoryName = new CategoryName(category.Path) { Name = name, };
            var dataTables = new DataTableCollection(dataSet, this.DataBase);

            foreach (var item in dataTables)
            {
                var dataTable = item.Key;
                var table = item.Value;
                if (table.Path.StartsWith(category.Path) == false)
                    continue;

                dataTable.CategoryPath = Regex.Replace(dataTable.CategoryPath, "^" + category.Path, categoryName.Path);
            }

            try
            {
                foreach (var item in dataTables)
                {
                    var dataTable = item.Key;
                    var table = item.Value;

                    if (table.TemplatedParent == null)
                    {
                        this.Repository.Modify(table.SchemaPath, dataTable.GetXmlSchema());
                    }
                    this.Repository.Modify(table.XmlPath, dataTable.GetXml());
                }

                this.Repository.Move(category.LocalPath, this.Context.GenerateCategoryPath(categoryName.Path));
                this.Context.InvokeTableItemRename(authentication, category, name);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeCategoryMove(Authentication authentication, TableCategory category, string parentPath, CremaDataSet dataSet)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryMove), category, parentPath);

            var categoryName = new CategoryName(parentPath, category.Name);
            var dataTables = new DataTableCollection(dataSet, this.DataBase);

            foreach (var item in dataTables)
            {
                var dataTable = item.Key;
                var table = item.Value;
                if (table.Path.StartsWith(category.Path) == false)
                    continue;

                dataTable.CategoryPath = Regex.Replace(dataTable.CategoryPath, "^" + category.Path, categoryName.Path);
            }

            try
            {
                foreach (var item in dataTables)
                {
                    var dataTable = item.Key;
                    var table = item.Value;

                    if (table.TemplatedParent == null)
                    {
                        this.Repository.Modify(table.SchemaPath, dataTable.GetXmlSchema());
                    }
                    this.Repository.Modify(table.XmlPath, dataTable.GetXml());
                }

                this.Repository.Move(category.LocalPath, this.Context.GenerateCategoryPath(categoryName));
                this.Context.InvokeTableItemMove(authentication, category, parentPath);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeCategoryDelete(Authentication authentication, TableCategory category)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryDelete), category);

            try
            {
                this.Repository.Delete(category.LocalPath);
                this.Context.InvokeTableItemDelete(authentication, category);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeCategoriesCreatedEvent(Authentication authentication, TableCategory[] categories)
        {
            var args = categories.Select(item => (object)null).ToArray();
            var dataSet = CremaDataSet.Create(new SignatureDateProvider(authentication.ID));
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesCreatedEvent), categories);
            var comment = EventMessageBuilder.CreateTableCategory(authentication, categories);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnCategoriesCreated(new ItemsCreatedEventArgs<ITableCategory>(authentication, categories, args));
            this.Context.InvokeItemsCreatedEvent(authentication, categories, args, dataSet);
        }
        
        public void InvokeCategoriesRenamedEvent(Authentication authentication, TableCategory[] categories, string[] oldNames, string[] oldPaths, CremaDataSet dataSet)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesRenamedEvent), categories, oldNames, oldPaths);
            var comment = EventMessageBuilder.RenameTableCategory(authentication, categories, oldNames);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnCategoriesRenamed(new ItemsRenamedEventArgs<ITableCategory>(authentication, categories, oldNames, oldPaths, dataSet));
            this.Context.InvokeItemsRenamedEvent(authentication, categories, oldNames, oldPaths, dataSet);
        }
        
        public void InvokeCategoriesMovedEvent(Authentication authentication, TableCategory[] categories, string[] oldPaths, string[] oldParentPaths, CremaDataSet dataSet)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesMovedEvent), categories, oldPaths, oldParentPaths);
            var comment = EventMessageBuilder.MoveTableCategory(authentication, categories, oldParentPaths);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnCategoriesMoved(new ItemsMovedEventArgs<ITableCategory>(authentication, categories, oldPaths, oldParentPaths, dataSet));
            this.Context.InvokeItemsMovedEvent(authentication, categories, oldPaths, oldParentPaths, dataSet);
        }
        
        public void InvokeCategoriesDeletedEvent(Authentication authentication, TableCategory[] categories, string[] categoryPaths)
        {
            var dataSet = CremaDataSet.Create(new SignatureDateProvider(authentication.ID));
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesDeletedEvent), categoryPaths);
            var comment = EventMessageBuilder.DeleteTableCategory(authentication, categories);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnCategoriesDeleted(new ItemsDeletedEventArgs<ITableCategory>(authentication, categories, categoryPaths, dataSet));
            this.Context.InvokeItemsDeletedEvent(authentication, categories, categoryPaths, dataSet);
        }

        public DataBaseRepositoryHost Repository
        {
            get { return this.DataBase.Repository; }
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

        public event ItemsCreatedEventHandler<ITableCategory> CategoriesCreated
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.categoriesCreated += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.categoriesCreated -= value;
            }
        }

        public event ItemsRenamedEventHandler<ITableCategory> CategoriesRenamed
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.categoriesRenamed += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.categoriesRenamed -= value;
            }
        }

        public event ItemsMovedEventHandler<ITableCategory> CategoriesMoved
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.categoriesMoved += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.categoriesMoved -= value;
            }
        }

        public event ItemsDeletedEventHandler<ITableCategory> CategoriesDeleted
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.categoriesDeleted += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.categoriesDeleted -= value;
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

        protected virtual void OnCategoriesCreated(ItemsCreatedEventArgs<ITableCategory> e)
        {
            this.categoriesCreated?.Invoke(this, e);
        }

        protected virtual void OnCategoriesRenamed(ItemsRenamedEventArgs<ITableCategory> e)
        {
            this.categoriesRenamed?.Invoke(this, e);
        }

        protected virtual void OnCategoriesMoved(ItemsMovedEventArgs<ITableCategory> e)
        {
            this.categoriesMoved?.Invoke(this, e);
        }

        protected virtual void OnCategoriesDeleted(ItemsDeletedEventArgs<ITableCategory> e)
        {
            this.categoriesDeleted?.Invoke(this, e);
        }

        private void Sign(Authentication authentication)
        {
            authentication.Sign();
        }

        private void ValidateAddNew(Authentication authentication, string name, string parentPath)
        {
            base.ValidateAddNew(name, parentPath, null);
            var parent = this[parentPath];
            parent.ValidateAccessType(authentication, AccessType.Master);
        }

        #region ITableCategoryCollection

        bool ITableCategoryCollection.Contains(string categoryPath)
        {
            this.Dispatcher?.VerifyAccess();
            return this.Contains(categoryPath);
        }

        ITableCategory ITableCategoryCollection.Root
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Root;
            }
        }

        ITableCategory ITableCategoryCollection.this[string categoryPath]
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this[categoryPath];
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator<ITableCategory> IEnumerable<ITableCategory>.GetEnumerator()
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
