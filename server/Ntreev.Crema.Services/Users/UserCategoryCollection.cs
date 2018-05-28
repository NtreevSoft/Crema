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
using System.Linq;
using System.Text;
using Ntreev.Crema.ServiceModel;
using System.IO;
using System.Xml.Serialization;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Services;
using System.Collections;
using Ntreev.Library.Serialization;
using System.Text.RegularExpressions;
using Ntreev.Crema.Services.Properties;
using System.Collections.Specialized;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.IO;

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
            var category = this.BaseAddNew(name, parentPath, authentication);
            authentication.Sign();
            this.InvokeCategoriesCreatedEvent(authentication, new UserCategory[] { category });
            return category;
        }

        public void InvokeCategoryCreate(Authentication authentication, string name, string parentPath)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryCreate), name, parentPath);

            try
            {
                var categoryPath = this.Context.GenerateCategoryPath(parentPath, name);
                DirectoryUtility.Prepare(categoryPath);
                this.Repository.Add(categoryPath);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeCategoriesCreatedEvent(Authentication authentication, UserCategory[] categories)
        {
            var args = categories.Select(item => (object)null).ToArray();
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesCreatedEvent), categories);
            var comment = EventMessageBuilder.CreateUserCategory(authentication, categories);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment);
            this.CremaHost.Info(comment);
            this.OnCategoriesCreated(new ItemsCreatedEventArgs<IUserCategory>(authentication, categories, args));
            this.Context.InvokeItemsCreatedEvent(authentication, categories, args);
        }

        public void InvokeCategoryRename(Authentication authentication, UserCategory category, string name)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryRename), category, name);

            try
            {
                //var categoryPath = (string)new CategoryName(category.Parent.Path, name);

                //var categories = from UserCategory item in this.Context.Categories
                //                 where item != this.Root
                //                 select item.Path;

                var categoryPath = this.Context.GenerateCategoryPath(category.Path);
                var newCategoryPath = this.Context.GenerateCategoryPath(category.Parent.Path, name); ;
                var path = this.Context.GenerateCategoryPath(categoryPath);
                var newPath = this.Context.GenerateCategoryPath(newCategoryPath);

                var query = from User item in this.Context.Users
                            select item.SerializationInfo;

                //var categoryArray = categories.ToArray();
                var users = query.ToArray();

                //for (var i = 0; i < categoryArray.Length; i++)
                //{
                //    categoryArray[i] = Regex.Replace(categoryArray[i], "^" + category.Path, categoryPath);
                //}


                for (var i = 0; i < users.Length; i++)
                {
                    var item = users[i];
                    var filename = this.Context.GenerateUserPath(item.CategoryPath, item.ID);
                    item.CategoryPath = Regex.Replace(item.CategoryPath, "^" + categoryPath, newCategoryPath);
                    var content = DataContractSerializerUtility.GetString(item, true);
                    this.Repository.Modify(filename, content);
                    users[i] = item;
                }
                this.Repository.Move(path, newPath);
                //var serializationInfo = new UserContextSerializationInfo()
                //{
                //    Version = CremaSchema.VersionValue,
                //    Categories = categoryArray,
                //    Users = userArray,
                //};

                //var xml = DataContractSerializerUtility.GetString(serializationInfo, true);
                //this.Repository.Modify(this.Context.UserFilePath, xml);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeCategoriesRenamedEvent(Authentication authentication, UserCategory[] categories, string[] oldNames, string[] oldPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesRenamedEvent), categories, oldNames, oldPaths);
            var comment = EventMessageBuilder.RenameUserCategory(authentication, categories, oldNames);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment);
            this.CremaHost.Info(comment);
            this.OnCategoriesRenamed(new ItemsRenamedEventArgs<IUserCategory>(authentication, categories, oldNames, oldPaths));
            this.Context.InvokeItemsRenamedEvent(authentication, categories, oldNames, oldPaths);
        }

        public void InvokeCategoryMove(Authentication authentication, UserCategory category, string parentPath)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryMove), category, parentPath);

            try
            {
                var categoryPath = category.Path;
                var newCategoryPath = new CategoryName(parentPath, category.Name);
                var path = this.Context.GenerateCategoryPath(categoryPath);
                var newPath = this.Context.GenerateCategoryPath(newCategoryPath);

                var query = from User item in this.Context.Users
                            select item.SerializationInfo;

                //var categoryArray = categories.ToArray();
                var users = query.ToArray();

                //for (var i = 0; i < categoryArray.Length; i++)
                //{
                //    categoryArray[i] = Regex.Replace(categoryArray[i], "^" + category.Path, categoryPath);
                //}


                for (var i = 0; i < users.Length; i++)
                {
                    var item = users[i];
                    var filename = this.Context.GenerateUserPath(item.CategoryPath, item.ID);
                    item.CategoryPath = Regex.Replace(item.CategoryPath, "^" + categoryPath, newCategoryPath);
                    var content = DataContractSerializerUtility.GetString(item, true);
                    this.Repository.Modify(filename, content);
                    users[i] = item;
                }
                this.Repository.Move(path, newPath);

                //var categoryPath = (string)new CategoryName(parentPath, category.Name);

                //var categories = from UserCategory item in this.Context.Categories
                //                 where item != this.Root
                //                 select item.Path;

                //var users = from User item in this.Context.Users
                //            select item.SerializationInfo;

                //var categoryArray = categories.ToArray();
                //var userArray = users.ToArray();

                //for (var i = 0; i < categoryArray.Length; i++)
                //{
                //    categoryArray[i] = Regex.Replace(categoryArray[i], "^" + category.Path, categoryPath);
                //}

                //for (var i = 0; i < userArray.Length; i++)
                //{
                //    userArray[i].CategoryPath = Regex.Replace(userArray[i].CategoryPath, "^" + category.Path, categoryPath);
                //}

                //var serializationInfo = new UserContextSerializationInfo()
                //{
                //    Version = CremaSchema.VersionValue,
                //    Categories = categoryArray,
                //    Users = userArray,
                //};

                //var xml = DataContractSerializerUtility.GetString(serializationInfo, true);
                //this.Repository.Modify(this.Context.UserFilePath, xml);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeCategoriesMovedEvent(Authentication authentication, UserCategory[] categories, string[] oldPaths, string[] oldParentPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesMovedEvent), categories, oldPaths, oldParentPaths);
            var comment = EventMessageBuilder.MoveUserCategory(authentication, categories, oldParentPaths);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment);
            this.CremaHost.Info(comment);
            this.OnCategoriesMoved(new ItemsMovedEventArgs<IUserCategory>(authentication, categories, oldPaths, oldParentPaths));
            this.Context.InvokeItemsMovedEvent(authentication, categories, oldPaths, oldParentPaths);
        }

        public void InvokeCategoryDelete(Authentication authentication, UserCategory category)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeCategoryDelete), category);

            try
            {
                var path = this.Context.GenerateCategoryPath(category.Path);
                this.Repository.Delete(path);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeCategoriesDeletedEvent(Authentication authentication, UserCategory[] categories, string[] categoryPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeCategoriesDeletedEvent), categoryPaths);
            var comment = EventMessageBuilder.DeleteUserCategory(authentication, categories);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment);
            this.CremaHost.Info(comment);
            this.OnCategoriesDeleted(new ItemsDeletedEventArgs<IUserCategory>(authentication, categories, categoryPaths));
            this.Context.InvokeItemsDeleteEvent(authentication, categories, categoryPaths);
        }

        public RepositoryHost Repository
        {
            get { return this.Context.Repository; }
        }

        public CremaHost CremaHost
        {
            get { return this.Context.CremaHost; }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.Context.Dispatcher; }
        }

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
