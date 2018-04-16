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

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.DataBaseService;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

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
            var categoryName = new CategoryName(parentPath, name);
            var result = this.Service.NewTypeCategory(categoryName);
            result.Validate(authentication);
            var category = this.Prepare(categoryName.Path);
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
        }

        public void InvokeCategoryRename(Authentication authentication, TypeCategory category, string name)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryRename), category, name);
        }

        public void InvokeCategoryMove(Authentication authentication, TypeCategory category, string parentPath)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryMove), category, parentPath);
        }

        public void InvokeCategoryDelete(Authentication authentication, TypeCategory category)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryDelete), category);
        }

        public void InvokeCategoriesCreatedEvent(Authentication authentication, TypeCategory[] categories)
        {
            var args = categories.Select(item => (object)null).ToArray();
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesCreatedEvent), categories);
            var comment = EventMessageBuilder.CreateTypeCategory(authentication, categories);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnCategoriesCreated(new ItemsCreatedEventArgs<ITypeCategory>(authentication, categories, args));
            this.Context.InvokeItemsCreatedEvent(authentication, categories, args);
        }

        public void InvokeCategoriesRenamedEvent(Authentication authentication, TypeCategory[] categories, string[] oldNames, string[] oldPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesRenamedEvent), categories, oldNames, oldPaths);
            var comment = EventMessageBuilder.RenameTypeCategory(authentication, categories, oldNames);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnCategoriesRenamed(new ItemsRenamedEventArgs<ITypeCategory>(authentication, categories, oldNames, oldPaths));
            this.Context.InvokeItemsRenamedEvent(authentication, categories, oldNames, oldPaths);
        }

        public void InvokeCategoriesMovedEvent(Authentication authentication, TypeCategory[] categories, string[] oldPaths, string[] oldParentPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesMovedEvent), categories, oldPaths, oldParentPaths);
            var comment = EventMessageBuilder.MoveTypeCategory(authentication, categories, oldParentPaths);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnCategoriesMoved(new ItemsMovedEventArgs<ITypeCategory>(authentication, categories, oldPaths, oldParentPaths));
            this.Context.InvokeItemsMovedEvent(authentication, categories, oldPaths, oldParentPaths);
        }

        public void InvokeCategoriesDeletedEvent(Authentication authentication, TypeCategory[] categories, string[] categoryPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesDeletedEvent), categories, categoryPaths);
            var comment = EventMessageBuilder.DeleteTypeCategory(authentication, categories);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnCategoriesDeleted(new ItemsDeletedEventArgs<ITypeCategory>(authentication, categories, categoryPaths));
            this.Context.InvokeItemsDeleteEvent(authentication, categories, categoryPaths);
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
