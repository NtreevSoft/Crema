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

using Ntreev.Crema.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Ntreev.Library.IO;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.ObjectModel;

namespace Ntreev.Crema.Commands.Consoles
{
    [Export(typeof(IConsoleDrive))]
    public sealed class UsersConsoleDrive : ConsoleDriveBase
    {
        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        private string path;

        internal UsersConsoleDrive()
            : base("users")
        {

        }

        public override string[] GetPaths()
        {
            return this.UserContext.Dispatcher.Invoke(() => this.UserContext.Select(item => item.Path).ToArray());
        }

        public override object GetObject(Authentication authentication, string path)
        {
            return this.GetObject(path);
        }

        public IUser GetUser(string userID)
        {
            return this.UserContext.Dispatcher.Invoke(() => this.UserContext.Users[userID]);
        }

        public string[] GetUserList()
        {
            return this.UserContext.Dispatcher.Invoke(() => this.UserContext.Users.Select(item => item.ID).ToArray());
        }

        protected override void OnCreate(Authentication authentication, string path, string name)
        {
            var category = this.GetObject(path) as IUserCategory;
            this.UserContext.Dispatcher.Invoke(Create);

            void Create()
            {
                if (category == null)
                    throw new CategoryNotFoundException(path);
                category.AddNewCategory(authentication, name);
            }
        }

        protected override void OnMove(Authentication authentication, string path, string newPath)
        {
            var sourceObject = this.GetObject(path);

            if (sourceObject is IUser sourceUser)
            {
                this.MoveUser(authentication, sourceUser, newPath);
            }
            else if (sourceObject is IUserCategory sourceUserCategory)
            {
                this.MoveUserCategory(authentication, sourceUserCategory, newPath);
            }
            else
            {
                throw new ItemNotFoundException(path);
            }
        }

        protected override void OnDelete(Authentication authentication, string path)
        {
            var userItem = this.GetObject(path) as IUserItem;
            this.UserContext.Dispatcher.Invoke(Delete);

            void Delete()
            {
                if(userItem == null)
                    throw new ItemNotFoundException(path);
                userItem.Delete(authentication);
            }
        }

        protected override void OnSetPath(Authentication authentication, string path)
        {
            this.path = path;
        }
        
        private void MoveUserCategory(Authentication authentication, IUserCategory sourceCategory, string destPath)
        {
            var destObject = this.GetObject(destPath);
            this.UserContext.Dispatcher.Invoke(MoveUserCategory);

            void MoveUserCategory()
            {
                //var dataBase = sourceCategory.GetService(typeof(IDataBase)) as IDataBase;
                var users = sourceCategory.GetService(typeof(IUserCollection)) as IUserCollection;

                //if (destPath.DataBaseName != dataBase.Name)
                //    throw new InvalidOperationException($"cannot move to : {destPath}");
                //if (destPath.Context != CremaSchema.UserDirectory)
                //    throw new InvalidOperationException($"cannot move to : {destPath}");
                if (destObject is IUser)
                    throw new InvalidOperationException($"cannot move to : {destPath}");

                if (destObject is IUserCategory destCategory)
                {
                    if (sourceCategory.Parent != destCategory)
                        sourceCategory.Move(authentication, destCategory.Path);
                }
                else
                {
                    if (NameValidator.VerifyCategoryPath(destPath) == true)
                        throw new InvalidOperationException($"cannot move to : {destPath}");
                    var itemName = new ItemName(destPath);
                    var categories = sourceCategory.GetService(typeof(IUserCategoryCollection)) as IUserCategoryCollection;
                    if (categories.Contains(itemName.CategoryPath) == false)
                        throw new InvalidOperationException($"cannot move to : {destPath}");
                    if (sourceCategory.Name != itemName.Name && users.Contains(itemName.Name) == true)
                        throw new InvalidOperationException($"cannot move to : {destPath}");
                    if (sourceCategory.Parent.Path != itemName.CategoryPath)
                        sourceCategory.Move(authentication, itemName.CategoryPath);
                    if (sourceCategory.Name != itemName.Name)
                        sourceCategory.Rename(authentication, itemName.Name);
                }
            }
        }

        private void MoveUser(Authentication authentication, IUser sourceUser, string destPath)
        {
            var destObject = this.GetObject(destPath);
            this.UserContext.Dispatcher.Invoke(MoveUser);

            void MoveUser()
            {
                var users = sourceUser.GetService(typeof(IUserCollection)) as IUserCollection;

                //if (destPath.DataBaseName != dataBase.Name)
                //    throw new InvalidOperationException($"cannot move to : {destPath}");
                //if (destPath.Context != CremaSchema.UserDirectory)
                //    throw new InvalidOperationException($"cannot move to : {destPath}");
                //if (destObject is IUser)
                //    throw new InvalidOperationException($"cannot move to : {destPath}");

                if (destObject is IUserCategory destCategory)
                {
                    if (sourceUser.Category != destCategory)
                        sourceUser.Move(authentication, destCategory.Path);
                }
                else
                {
                    if (NameValidator.VerifyCategoryPath(destPath) == true)
                        throw new InvalidOperationException($"cannot move to : {destPath}");
                    var itemName = new ItemName(destPath);
                    var categories = sourceUser.GetService(typeof(IUserCategoryCollection)) as IUserCategoryCollection;
                    if (categories.Contains(itemName.CategoryPath) == false)
                        throw new InvalidOperationException($"cannot move to : {destPath}");
                    if (sourceUser.ID != itemName.Name)
                        throw new InvalidOperationException($"cannot move to : {destPath}");
                    if (sourceUser.Category.Path != itemName.CategoryPath)
                        sourceUser.Move(authentication, itemName.CategoryPath);
                }
            };
        }

        private IUserItem GetObject(string path)
        {
            return this.UserContext.Dispatcher.Invoke(GetObject);

            IUserItem GetObject()
            {
                if (NameValidator.VerifyCategoryPath(path) == true)
                {
                    return this.UserContext[path];
                }
                else
                {
                    var itemName = new ItemName(path);
                    var category = this.UserContext.Categories[itemName.CategoryPath];
                    if (category.Categories.ContainsKey(itemName.Name) == true)
                        return category.Categories[itemName.Name] as IUserItem;
                    if (category.Users.ContainsKey(itemName.Name) == true)
                        return category.Users[itemName.Name] as IUserItem;
                    return null;
                }
            }
        }

        private IUserContext UserContext => this.cremaHost.Value.GetService(typeof(IUserContext)) as IUserContext;
    }
}
