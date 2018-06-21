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
using Ntreev.Crema.Services.Properties;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;

namespace Ntreev.Crema.Services.Users
{
    class UserCategory : UserCategoryBase<User, UserCategory, UserCollection, UserCategoryCollection, UserContext>,
        IUserCategory, IUserItem
    {
        public void Rename(Authentication authentication, string name)
        {
            try
            {
                this.Dispatcher?.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(Rename), this, name);
                this.ValidateRename(authentication, name);
                this.Sign(authentication);
                var items = EnumerableUtility.One(this).ToArray();
                var oldNames = items.Select(item => item.Name).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                this.Container.InvokeCategoryRename(authentication, this, name);
                base.Name = name;
                this.Container.InvokeCategoriesRenamedEvent(authentication, items, oldNames, oldPaths);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Move(Authentication authentication, string parentPath)
        {
            try
            {
                this.Dispatcher?.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(Move), this, parentPath);
                this.ValidateMove(authentication, parentPath);
                this.Sign(authentication);
                var items = EnumerableUtility.One(this).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                var oldParentPaths = items.Select(item => item.Parent.Path).ToArray();
                this.Container.InvokeCategoryMove(authentication, this, parentPath);
                this.Parent = this.Container[parentPath];
                this.Container.InvokeCategoriesMovedEvent(authentication, items, oldPaths, oldParentPaths);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Delete(Authentication authentication)
        {
            try
            {
                this.Dispatcher?.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(Delete), this);
                this.ValidateDelete(authentication);
                this.Sign(authentication);
                var items = EnumerableUtility.One(this).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                var container = this.Container;
                container.InvokeCategoryDelete(authentication, this);
                this.Dispose();
                container.InvokeCategoriesDeletedEvent(authentication, items, oldPaths);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public UserCategory AddNewCategory(Authentication authentication, string name)
        {
            try
            {
                this.Dispatcher?.VerifyAccess();
                return this.Container.AddNew(authentication, name, base.Path);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public User AddNewUser(Authentication authentication, string userID, SecureString password, string userName, Authority authority)
        {
            try
            {
                this.Dispatcher?.VerifyAccess();
                return this.Context.Users.AddNew(authentication, userID, base.Path, password, userName, authority);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void ValidateRename(Authentication authentication, string name)
        {
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();
            if (base.Name == name)
                throw new ArgumentException(Resources.Exception_SameName, nameof(name));
            base.ValidateRename(name);
        }

        public void ValidateMove(Authentication authentication, string parentPath)
        {
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
                throw new PermissionDeniedException();
            if (this.Parent.Path == parentPath)
                throw new ArgumentException(Resources.Exception_CannotMoveToSamePath, nameof(parentPath));
            var parent = this.Container[parentPath];
            if (parent == null)
                throw new CategoryNotFoundException(parentPath);

            base.ValidateMove(parent);

        }

        public void ValidateDelete(Authentication authentication)
        {
            if (authentication.Types.HasFlag(AuthenticationType.Administrator) == false)
            {
                throw new PermissionDeniedException();
            }

            base.ValidateDelete();

            if (EnumerableUtility.Descendants<IItem, IUser>(this as IItem, item => item.Childs).Any() == true)
                throw new InvalidOperationException(Resources.Exception_CannotDeletePathWithItems);
        }

        public CremaDispatcher Dispatcher => this.Context?.Dispatcher;

        public CremaHost CremaHost => this.Context.CremaHost;

        public string ItemPath => this.Context.GenerateCategoryPath(base.Path);

        public new string Name
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.Name;
            }
        }

        public new string Path
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.Path;
            }
        }

        public new event EventHandler Renamed
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Renamed += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Renamed -= value;
            }
        }

        public new event EventHandler Moved
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Moved += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Moved -= value;
            }
        }

        public new event EventHandler Deleted
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Deleted += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Deleted -= value;
            }
        }

        private void Sign(Authentication authentication)
        {
            authentication.Sign();
        }

        #region IUserCategory

        IUserCategory IUserCategory.AddNewCategory(Authentication authentication, string name)
        {
            return this.AddNewCategory(authentication, name);
        }

        IUser IUserCategory.AddNewUser(Authentication authentication, string userID, SecureString password, string userName, Authority authority)
        {
            return this.AddNewUser(authentication, userID, password, userName, authority);
        }

        IUserCategory IUserCategory.Parent
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Parent;
            }
        }

        IContainer<IUser> IUserCategory.Users
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Items;
            }
        }

        IContainer<IUserCategory> IUserCategory.Categories
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Categories;
            }
        }

        #endregion

        #region IUserItem

        IUserItem IUserItem.Parent
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Parent;
            }
        }

        IEnumerable<IUserItem> IUserItem.Childs
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                foreach (var item in this.Categories)
                {
                    yield return item;
                }
                foreach (var item in this.Items)
                {
                    yield return item;
                }
            }
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
