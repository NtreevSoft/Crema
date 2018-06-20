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
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
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
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(Rename), this, name);
                var items = EnumerableUtility.One(this).ToArray();
                var oldNames = items.Select(item => item.Name).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                var result = this.Service.RenameUserItem(this.Path, name);
                this.Sign(authentication, result);
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
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(Move), this, parentPath);
                var items = EnumerableUtility.One(this).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                var oldParentPaths = items.Select(item => item.Parent.Path).ToArray();
                var result = this.Service.MoveUserItem(this.Path, parentPath);
                this.Sign(authentication, result);
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
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(Delete), this);
                var items = EnumerableUtility.One(this).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                var container = this.Container;
                var result = this.Service.DeleteUserItem(this.Path);
                this.Sign(authentication, result);
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
                return this.Container.AddNew(authentication, name, this.Path);
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
                return this.Context.Users.AddNew(authentication, userID, this.Path, password, userName, authority);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void InternalSetName(string name)
        {
            base.Name = name;
        }

        public IUserService Service => this.Context.Service;

        public CremaHost CremaHost => this.Context.CremaHost;

        public CremaDispatcher Dispatcher => this.Context.Dispatcher;

        public new string Name
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.Name;
            }
        }

        public new string Path
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.Path;
            }
        }

        public new event EventHandler Renamed
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                base.Renamed += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                base.Renamed -= value;
            }
        }

        public new event EventHandler Moved
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                base.Moved += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                base.Moved -= value;
            }
        }

        public new event EventHandler Deleted
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                base.Deleted += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                base.Deleted -= value;
            }
        }

        private void Sign(Authentication authentication, ResultBase result)
        {
            result.Validate(authentication);
        }

        private void Sign<T>(Authentication authentication, ResultBase<T> result)
        {
            result.Validate(authentication);
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
                this.Dispatcher.VerifyAccess();
                return this.Parent;
            }
        }

        IContainer<IUser> IUserCategory.Users
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Items;
            }
        }

        IContainer<IUserCategory> IUserCategory.Categories
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Categories;
            }
        }

        #endregion

        #region IUserItem

        IUserItem IUserItem.Parent
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Parent;
            }
        }

        IEnumerable<IUserItem> IUserItem.Childs
        {
            get
            {
                this.Dispatcher.VerifyAccess();
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
