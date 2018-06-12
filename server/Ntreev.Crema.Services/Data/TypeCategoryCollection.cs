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
    class TypeCategoryCollection : CategoryContainer<Type, TypeCategory, TypeCollection, TypeCategoryCollection, TypeContext>,
        ITypeCategoryCollection
    {
        private ItemsCreatedEventHandler<ITypeCategory> categoriesCreated;
        private ItemsRenamedEventHandler<ITypeCategory> categoriesRenamed;
        private ItemsMovedEventHandler<ITypeCategory> categoriesMoved;
        private ItemsDeletedEventHandler<ITypeCategory> categoriesDeleted;

        public TypeCategoryCollection()
        {

        }

        public TypeCategory AddNew(Authentication authentication, string name, string parentPath)
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
            var categoryName = new CategoryName(parentPath, name);
            var path = this.Context.GenerateCategoryPath(parentPath, name);
            var message = EventMessageBuilder.CreateTypeCategory(authentication, categoryName);
            try
            {
                Directory.CreateDirectory(path);
                this.Repository.Add(path);
                this.Context.InvokeTypeItemCreate(authentication, parentPath + name);
                this.Repository.Commit(authentication, message);
            }
            catch (Exception e)
            {
                DirectoryUtility.Delete(path);
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeCategoryRename(Authentication authentication, TypeCategory category, string name, CremaDataSet dataSet)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryRename), category, name);
            var categoryName = new CategoryName(category.Path) { Name = name, };
            var dataTypes = new DataTypeCollection(dataSet, this.DataBase);
            var dataTables = new DataTableCollection(dataSet, this.DataBase);
            var message = EventMessageBuilder.RenameTypeCategory(authentication, category.Path, categoryName);
            try
            {
                dataTypes.SetCategoryPath(category.Path, categoryName);
                dataTypes.Modify(this.Serializer);
                dataTables.Modify(this.Serializer);
                this.Repository.Move(category.LocalPath, this.Context.GenerateCategoryPath(categoryName.Path));
                this.Context.InvokeTypeItemRename(authentication, category, name);
                this.Repository.Commit(authentication, message);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeCategoryMove(Authentication authentication, TypeCategory category, string parentPath, CremaDataSet dataSet)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryMove), category, parentPath);
            var categoryName = new CategoryName(parentPath, category.Name);
            var dataTypes = new DataTypeCollection(dataSet, this.DataBase);
            var dataTables = new DataTableCollection(dataSet, this.DataBase);
            var message = EventMessageBuilder.MoveTypeCategory(authentication, category.Path, category.Parent.Path, parentPath);
            try
            {
                dataTypes.SetCategoryPath(category.Path, categoryName);
                dataTypes.Modify(this.Serializer);
                dataTables.Modify(this.Serializer);
                this.Repository.Move(category.LocalPath, this.Context.GenerateCategoryPath(categoryName));
                this.Repository.Commit(authentication, message);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeCategoryDelete(Authentication authentication, TypeCategory category, CremaDataSet dataSet)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryDelete), category);
            var message = EventMessageBuilder.DeleteTypeCategory(authentication, category.Path);
            try
            {
                this.Repository.Delete(category.LocalPath);
                this.Context.InvokeTypeItemDelete(authentication, category);
                this.Repository.Commit(authentication, message);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeCategoriesCreatedEvent(Authentication authentication, TypeCategory[] categories)
        {
            var args = categories.Select(item => (object)null).ToArray();
            var dataSet = CremaDataSet.Create(new SignatureDateProvider(authentication.ID));
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesCreatedEvent), categories);
            var message = EventMessageBuilder.CreateTypeCategory(authentication, categories);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnCategoriesCreated(new ItemsCreatedEventArgs<ITypeCategory>(authentication, categories, args, dataSet));
            this.Context.InvokeItemsCreatedEvent(authentication, categories, args, dataSet);
        }

        public void InvokeCategoriesRenamedEvent(Authentication authentication, TypeCategory[] categories, string[] oldNames, string[] oldPaths, CremaDataSet dataSet)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesRenamedEvent), categories, oldNames, oldPaths);
            var message = EventMessageBuilder.RenameTypeCategory(authentication, categories, oldPaths);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnCategoriesRenamed(new ItemsRenamedEventArgs<ITypeCategory>(authentication, categories, oldNames, oldPaths, dataSet));
            this.Context.InvokeItemsRenamedEvent(authentication, categories, oldNames, oldPaths, dataSet);
        }

        public void InvokeCategoriesMovedEvent(Authentication authentication, TypeCategory[] categories, string[] oldPaths, string[] oldParentPaths, CremaDataSet dataSet)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesMovedEvent), categories, oldPaths, oldParentPaths);
            var message = EventMessageBuilder.MoveTypeCategory(authentication, categories, oldPaths, oldParentPaths);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnCategoriesMoved(new ItemsMovedEventArgs<ITypeCategory>(authentication, categories, oldPaths, oldParentPaths, dataSet));
            this.Context.InvokeItemsMovedEvent(authentication, categories, oldPaths, oldParentPaths, dataSet);
        }

        public void InvokeCategoriesDeletedEvent(Authentication authentication, TypeCategory[] categories, string[] categoryPaths, CremaDataSet dataSet)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesDeletedEvent), categories, categoryPaths);
            var message = EventMessageBuilder.DeleteTypeCategory(authentication, categories);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnCategoriesDeleted(new ItemsDeletedEventArgs<ITypeCategory>(authentication, categories, categoryPaths, dataSet));
            this.Context.InvokeItemsDeleteEvent(authentication, categories, categoryPaths, dataSet);
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

        public IObjectSerializer Serializer => this.DataBase.Serializer;

        public new int Count
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.Count;
            }
        }

        public event ItemsCreatedEventHandler<ITypeCategory> CategoriesCreated
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

        public event ItemsRenamedEventHandler<ITypeCategory> CategoriesRenamed
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

        public event ItemsMovedEventHandler<ITypeCategory> CategoriesMoved
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

        public event ItemsDeletedEventHandler<ITypeCategory> CategoriesDeleted
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

        protected virtual void OnCategoriesCreated(ItemsCreatedEventArgs<ITypeCategory> e)
        {
            this.categoriesCreated?.Invoke(this, e);
        }

        protected virtual void OnCategoriesRenamed(ItemsRenamedEventArgs<ITypeCategory> e)
        {
            this.categoriesRenamed?.Invoke(this, e);
        }

        protected virtual void OnCategoriesMoved(ItemsMovedEventArgs<ITypeCategory> e)
        {
            this.categoriesMoved?.Invoke(this, e);
        }

        protected virtual void OnCategoriesDeleted(ItemsDeletedEventArgs<ITypeCategory> e)
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

            var path = this.Context.GenerateCategoryPath(parentPath, name);
            if (Directory.Exists(path) == true)
                throw new InvalidOperationException(Resources.Exception_SameNamePathExists);
        }

        #region ITypeCategoryCollection

        bool ITypeCategoryCollection.Contains(string categoryPath)
        {
            this.Dispatcher?.VerifyAccess();
            return this.Contains(categoryPath);
        }

        ITypeCategory ITypeCategoryCollection.Root
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Root;
            }
        }

        ITypeCategory ITypeCategoryCollection.this[string categoryPath]
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this[categoryPath];
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator<ITypeCategory> IEnumerable<ITypeCategory>.GetEnumerator()
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
