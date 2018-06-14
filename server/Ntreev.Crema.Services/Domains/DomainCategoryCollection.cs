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
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Ntreev.Crema.Services.Domains
{
    class DomainCategoryCollection : CategoryContainer<Domain, DomainCategory, DomainCollection, DomainCategoryCollection, DomainContext>,
        IDomainCategoryCollection
    {
        private ItemsCreatedEventHandler<IDomainCategory> categoriesCreated;
        private ItemsRenamedEventHandler<IDomainCategory> categoriesRenamed;
        private ItemsMovedEventHandler<IDomainCategory> categoriesMoved;
        private ItemsDeletedEventHandler<IDomainCategory> categoriesDeleted;

        public DomainCategory AddNew(string categoryPath)
        {
            NameValidator.ValidateCategoryPath(categoryPath);
            var categoryName = new CategoryName(categoryPath);

            if (this.ContainsKey(categoryName.ParentPath) == false)
            {
                this.AddNew(categoryName.ParentPath);
            }

            return this.BaseAddNew(categoryName.Name, categoryName.ParentPath, null, null);
        }

        public void InvokeCategoriesCreatedEvent(Authentication authentication, DomainCategory[] categories)
        {
            var args = categories.Select(item => (object)null).ToArray();
            this.OnCategoriesCreated(new ItemsCreatedEventArgs<IDomainCategory>(authentication, categories, args));
            this.Context.InvokeItemsCreatedEvent(authentication, categories, args);
        }

        public void InvokeCategoriesRenamedEvent(Authentication authentication, DomainCategory[] categories, string[] oldNames, string[] oldPaths)
        {
            this.OnCategoriesRenamed(new ItemsRenamedEventArgs<IDomainCategory>(authentication, categories, oldNames, oldPaths));
            this.Context.InvokeItemsRenamedEvent(authentication, categories, oldNames, oldPaths);
        }

        public void InvokeCategoriesMovedEvent(Authentication authentication, DomainCategory[] categories, string[] oldPaths, string[] oldParentPaths)
        {
            this.OnCategoriesMoved(new ItemsMovedEventArgs<IDomainCategory>(authentication, categories, oldPaths, oldParentPaths));
            this.Context.InvokeItemsMovedEvent(authentication, categories, oldPaths, oldParentPaths);
        }

        public void InvokeCategoriesDeletedEvent(Authentication authentication, DomainCategory[] categories, string[] categoryPaths)
        {
            this.OnCategoriesDeleted(new ItemsDeletedEventArgs<IDomainCategory>(authentication, categories, categoryPaths));
            this.Context.InvokeItemsDeleteEvent(authentication, categories, categoryPaths);
        }

        public string[] GetMetaData(Authentication authentication)
        {
            this.Dispatcher.VerifyAccess();

            var categoryPathList = new List<string>(this.Count);
            foreach (var item in this)
            {
                categoryPathList.Add(item.Path);
            }
            return categoryPathList.ToArray();
        }

        public new int Count
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.Count;
            }
        }

        public event ItemsCreatedEventHandler<IDomainCategory> CategoriesCreated
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.categoriesCreated += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.categoriesCreated -= value;
            }
        }

        public event ItemsRenamedEventHandler<IDomainCategory> CategoriesRenamed
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.categoriesRenamed += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.categoriesRenamed -= value;
            }
        }

        public event ItemsMovedEventHandler<IDomainCategory> CategoriesMoved
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.categoriesMoved += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.categoriesMoved -= value;
            }
        }

        public event ItemsDeletedEventHandler<IDomainCategory> CategoriesDeleted
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.categoriesDeleted += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.categoriesDeleted -= value;
            }
        }

        public new event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                base.CollectionChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                base.CollectionChanged -= value;
            }
        }

        public CremaDispatcher Dispatcher => this.Context.Dispatcher;

        protected virtual void OnCategoriesCreated(ItemsCreatedEventArgs<IDomainCategory> e)
        {
            this.categoriesCreated?.Invoke(this, e);
        }

        protected virtual void OnCategoriesRenamed(ItemsRenamedEventArgs<IDomainCategory> e)
        {
            this.categoriesRenamed?.Invoke(this, e);
        }

        protected virtual void OnCategoriesMoved(ItemsMovedEventArgs<IDomainCategory> e)
        {
            this.categoriesMoved?.Invoke(this, e);
        }

        protected virtual void OnCategoriesDeleted(ItemsDeletedEventArgs<IDomainCategory> e)
        {
            this.categoriesDeleted?.Invoke(this, e);
        }

        #region IDomainCategoryCollection

        bool IDomainCategoryCollection.Contains(string categoryPath)
        {
            this.Dispatcher.VerifyAccess();
            return this.Contains(categoryPath);
        }

        IDomainCategory IDomainCategoryCollection.Root
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Root;
            }
        }

        IDomainCategory IDomainCategoryCollection.this[string categoryPath]
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this[categoryPath];
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator<IDomainCategory> IEnumerable<IDomainCategory>.GetEnumerator()
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

        #region IServiceProvider

        object IServiceProvider.GetService(Type serviceType)
        {
            return (this.Context as IServiceProvider).GetService(serviceType);
        }

        #endregion
    }
}
