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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.Users.Serializations;

namespace Ntreev.Crema.Services.Users
{
    class UserRepositoryHost : RepositoryHost
    {
        private readonly UserContext userContext;

        public UserRepositoryHost(UserContext userContext, IRepository repository)
            : base(repository, userContext.CremaHost.RepositoryDispatcher)
        {
            this.userContext = userContext;
        }

        public void MoveCategory(Authentication authentication, UserCategory category, string newCategoryPath)
        {
            var categoryPath = category.Path;
            var query = from User item in this.userContext.Users
                        where item.Category.Path.StartsWith(category.Path)
                        select item.SerializationInfo;

            var users = query.ToArray();
            for (var i = 0; i < users.Length; i++)
            {
                var item = users[i];
                if (item.CategoryPath.StartsWith(categoryPath) == false)
                    continue;
                var itemPath = this.userContext.GenerateUserPath(item.CategoryPath, item.ID);
                item.CategoryPath = Regex.Replace(item.CategoryPath, "^" + categoryPath, newCategoryPath);
                this.Serializer.Serialize(itemPath, item, ObjectSerializerSettings.Empty);
            }

            var itemPath1 = category.ItemPath;
            var itemPath2 = this.userContext.GenerateCategoryPath(newCategoryPath);
            this.Move(itemPath1, itemPath2);
        }

        public void RenameCategory(Authentication authentication, UserCategory category, string newCategoryPath)
        {
            var categoryPath = category.Path;
            var query = from User item in this.userContext.Users
                        where item.Category.Path.StartsWith(category.Path)
                        select item.SerializationInfo;

            var users = query.ToArray();
            for (var i = 0; i < users.Length; i++)
            {
                var item = users[i];
                if (item.CategoryPath.StartsWith(categoryPath) == false)
                    continue;
                var itemPath = this.userContext.GenerateUserPath(item.CategoryPath, item.ID);
                item.CategoryPath = Regex.Replace(item.CategoryPath, "^" + categoryPath, newCategoryPath);
                this.Serializer.Serialize(itemPath, item, ObjectSerializerSettings.Empty);
            }

            var itemPath1 = category.ItemPath;
            var itemPath2 = this.userContext.GenerateCategoryPath(newCategoryPath);
            this.Move(itemPath1, itemPath2);
        }

        public void CreateUser(UserSerializationInfo userInfo)
        {
            var itemPath = this.userContext.GenerateUserPath(userInfo.CategoryPath, userInfo.ID);
            var itemPaths = this.Serializer.Serialize(itemPath, userInfo, ObjectSerializerSettings.Empty);
            this.AddRange(itemPaths);
        }

        public void MoveUser(Authentication authentication, User user, string categoryPath)
        {
            var itemPath = user.ItemPath;
            var userSerializationInfo = new UserSerializationInfo(user.SerializationInfo)
            {
                CategoryPath = categoryPath,
                ModificationInfo = authentication.SignatureDate
            };

            this.Serializer.Serialize(itemPath, userSerializationInfo, ObjectSerializerSettings.Empty);
            this.MoveRepositoryPath(userSerializationInfo, user);
        }

        public void DeleteUser(Authentication authentication, User user)
        {
            this.DeleteRepositoryPath(user);
        }

        public void ModifyUser(Authentication authentication, User user, UserSerializationInfo userSerializationInfo)
        {
            var itemPath = user.ItemPath;
            this.Serializer.Serialize(itemPath, userSerializationInfo, ObjectSerializerSettings.Empty);
        }

        private void MoveRepositoryPath(UserSerializationInfo userSerializationInfo, User user)
        {
            var files = this.userContext.GetFiles(user.ItemPath);

            for (var i = 0; i < files.Length; i++)
            {
                var path1 = files[i];
                var extension = Path.GetExtension(path1);
                var path2 = this.userContext.GeneratePath(userSerializationInfo.CategoryPath + userSerializationInfo.ID) + extension;
                this.Move(path1, path2);
            }
        }

        private void DeleteRepositoryPath(User user)
        {
            var files = this.userContext.GetFiles(user.ItemPath);
            this.DeleteRange(files);
        }

        private IObjectSerializer Serializer => this.userContext.Serializer;
    }
}
