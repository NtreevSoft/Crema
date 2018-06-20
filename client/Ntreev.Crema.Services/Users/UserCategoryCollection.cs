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
using Ntreev.Crema.Services.UserService;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Ntreev.Crema.Services.Users
{
    class UserCategoryCollection : CategoryContainer<User, UserCategory, UserCollection, UserCategoryCollection, UserContext>,
        IUserCategoryCollection
    {
        private ItemsCreatedEventHandler<IUserCategory> categoriesCreated;
        private ItemsRenamedEventHandler<IUserCategory> categoriesRenamed;
        private ItemsMovedEventHandler<IUserCategory> categoriesMoved;
        private ItemsDeletedEventHandler<IUserCategory> categoriesDeleted;

        public UserCategoryCollection()
        {

        }
                
        public UserCategory AddNew(Authentication authentication, string name, string parentPath)
        {
            this.Dispatcher.VerifyAccess();
            var categoryName = new CategoryName(parentPath, name);
            var result = this.Context.Service.NewUserCategory(categoryName);
            result.Validate(authentication);
            var category = this.BaseAddNew(name, parentPath, authentication);
            this.InvokeCategoriesCreatedEvent(authentication, new UserCategory[] { category });
            return category;
        }

        public void InvokeCategoriesCreatedEvent(Authentication authentication, UserCategory[] categories)
        {
            var args = categories.Select(item => (object)null).ToArray();
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesCreatedEvent), categories);
            var message = EventMessageBuilder.CreateUserCategory(authentication, categories);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnCategoriesCreated(new ItemsCreatedEventArgs<IUserCategory>(authentication, categories, args));
            this.Context.InvokeItemsCreatedEvent(authentication, categories, null);
        }

        public void InvokeCategoriesRenamedEvent(Authentication authentication, UserCategory[] categories, string[] oldNames, string[] oldPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesRenamedEvent), categories, oldNames, oldPaths);
            var message = EventMessageBuilder.RenameUserCategory(authentication, categories, oldPaths);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnCategoriesRenamed(new ItemsRenamedEventArgs<IUserCategory>(authentication, categories, oldNames, oldPaths));
            this.Context.InvokeItemsRenamedEvent(authentication, categories, oldNames, oldPaths);
        }

        public void InvokeCategoriesMovedEvent(Authentication authentication, UserCategory[] categories, string[] oldPaths, string[] oldParentPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesMovedEvent), categories, oldPaths, oldParentPaths);
            var message = EventMessageBuilder.MoveUserCategory(authentication, categories, oldPaths, oldParentPaths);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnCategoriesMoved(new ItemsMovedEventArgs<IUserCategory>(authentication, categories, oldPaths, oldParentPaths));
            this.Context.InvokeItemsMovedEvent(authentication, categories, oldPaths, oldParentPaths);
        }

        public void InvokeCategoriesDeletedEvent(Authentication authentication, UserCategory[] categories, string[] categoryPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesDeletedEvent), categoryPaths);
            var message = EventMessageBuilder.DeleteUserCategory(authentication, categories);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnCategoriesDeleted(new ItemsDeletedEventArgs<IUserCategory>(authentication, categories, categoryPaths));
            this.Context.InvokeItemsDeleteEvent(authentication, categories, categoryPaths);
        }

        public IUserService Service => this.Context.Service;

        public CremaHost CremaHost => this.Context.CremaHost;

        public CremaDispatcher Dispatcher => this.CremaHost.Dispatcher;

        public new int Count
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.Count;
            }
        }

        public event ItemsCreatedEventHandler<IUserCategory> CategoriesCreated
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

        public event ItemsRenamedEventHandler<IUserCategory> CategoriesRenamed
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

        public event ItemsMovedEventHandler<IUserCategory> CategoriesMoved
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

        public event ItemsDeletedEventHandler<IUserCategory> CategoriesDeleted
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

        protected virtual void OnCategoriesCreated(ItemsCreatedEventArgs<IUserCategory> e)
        {
            this.categoriesCreated?.Invoke(this, e);
        }

        protected virtual void OnCategoriesRenamed(ItemsRenamedEventArgs<IUserCategory> e)
        {
            this.categoriesRenamed?.Invoke(this, e);
        }

        protected virtual void OnCategoriesMoved(ItemsMovedEventArgs<IUserCategory> e)
        {
            this.categoriesMoved?.Invoke(this, e);
        }

        protected virtual void OnCategoriesDeleted(ItemsDeletedEventArgs<IUserCategory> e)
        {
            this.categoriesDeleted?.Invoke(this, e);
        }

        #region IUserCategoryCollection

        bool IUserCategoryCollection.Contains(string categoryPath)
        {
            this.Dispatcher.VerifyAccess();
            return this.Contains(categoryPath);
        }

        IUserCategory IUserCategoryCollection.Root
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Root;
            }
        }

        IUserCategory IUserCategoryCollection.this[string categoryPath]
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this[categoryPath];
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator<IUserCategory> IEnumerable<IUserCategory>.GetEnumerator()
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

        object IServiceProvider.GetService(System.Type serviceType)
        {
            return (this.Context as IUserContext).GetService(serviceType);
        }

        #endregion
    }
}
