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
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

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
            this.ValidateAddNew(authentication, name, parentPath);
            this.InvokeCategoryCreate(authentication, name, parentPath);
            this.Sign(authentication);
            var category = this.BaseAddNew(name, parentPath, authentication);
            this.InvokeCategoriesCreatedEvent(authentication, new UserCategory[] { category });
            return category;
        }

        public void InvokeCategoryCreate(Authentication authentication, string name, string parentPath)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryCreate), name, parentPath);
            var categoryName = new CategoryName(parentPath, name);
            var message = EventMessageBuilder.CreateUserCategory(authentication, categoryName);
            try
            {
                var categoryPath = this.Context.GenerateCategoryPath(parentPath, name);
                DirectoryUtility.Prepare(categoryPath);
                this.Repository.Add(categoryPath);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeCategoryRename(Authentication authentication, UserCategory category, string name)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryRename), category, name);
            var message = EventMessageBuilder.RenameUserCategory(authentication, category.Path, name);
            try
            {
                var categoryPath = category.Path;
                var newCategoryPath = new CategoryName(category.Parent.Path, name);
                var path = this.Context.GenerateCategoryPath(category.Path);
                var newPath = this.Context.GenerateCategoryPath(category.Parent.Path, name);

                var query = from User item in this.Context.Users
                            where item.Category.Path.StartsWith(category.Path)
                            select item.SerializationInfo;

                var users = query.ToArray();

                for (var i = 0; i < users.Length; i++)
                {
                    var item = users[i];
                    var itemPath = this.Context.GenerateUserPath(item.CategoryPath, item.ID);
                    item.CategoryPath = Regex.Replace(item.CategoryPath, "^" + categoryPath, newCategoryPath);
                    this.Serializer.Serialize(itemPath, item, ObjectSerializerSettings.Empty);
                }
                this.Repository.Move(path, newPath);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeCategoryMove(Authentication authentication, UserCategory category, string parentPath)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryMove), category, parentPath);
            var message = EventMessageBuilder.MoveUserCategory(authentication, category.Path, category.Parent.Path, parentPath);
            try
            {
                var categoryPath = category.Path;
                var newCategoryPath = new CategoryName(parentPath, category.Name);
                var path = this.Context.GenerateCategoryPath(categoryPath);
                var newPath = this.Context.GenerateCategoryPath(newCategoryPath);

                var query = from User item in this.Context.Users
                            where item.Category.Path.StartsWith(category.Path)
                            select item.SerializationInfo;

                var users = query.ToArray();
                for (var i = 0; i < users.Length; i++)
                {
                    var item = users[i];
                    var itemPath = this.Context.GenerateUserPath(item.CategoryPath, item.ID);
                    item.CategoryPath = Regex.Replace(item.CategoryPath, "^" + categoryPath, newCategoryPath);
                    this.Serializer.Serialize(itemPath, item, ObjectSerializerSettings.Empty);
                }
                this.Repository.Move(path, newPath);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeCategoryDelete(Authentication authentication, UserCategory category)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryDelete), category);
            var path = this.Context.GenerateCategoryPath(category.Path);
            var message = EventMessageBuilder.DeleteUserCategory(authentication, category.Path);
            try
            {
                this.Repository.Delete(path);
                this.Repository.Commit(authentication, message);
            }
            catch
            {
                this.Repository.Revert();
                throw;
            }
        }

        public void InvokeCategoriesCreatedEvent(Authentication authentication, UserCategory[] categories)
        {
            var args = categories.Select(item => (object)null).ToArray();
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesCreatedEvent), categories);
            var message = EventMessageBuilder.CreateUserCategory(authentication, categories);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(message);
            this.OnCategoriesCreated(new ItemsCreatedEventArgs<IUserCategory>(authentication, categories, args));
            this.Context.InvokeItemsCreatedEvent(authentication, categories, args);
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

        public RepositoryHost Repository => this.Context.Repository;

        public CremaHost CremaHost => this.Context.CremaHost;

        public CremaDispatcher Dispatcher => this.Context.Dispatcher;

        public IObjectSerializer Serializer => this.Context.Serializer;

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

        private void ValidateAddNew(Authentication authentication, string name, string parentPath)
        {
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();

            base.ValidateAddNew(name, parentPath, null);
        }

        private void Sign(Authentication authentication)
        {
            authentication.Sign();
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
