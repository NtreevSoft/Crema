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

using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Ntreev.Crema.Services.Users.Serializations
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct UserContextSerializationInfo
    {
        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public UserCategorySerializationInfoList CategoriesList { get; set; }

        [DataMember]
        public UserSerializationInfo[] Users { get; set; }

        [IgnoreDataMember]
        public string[] Categories
        {
            get { return this.CategoriesList?.ToArray(); }
            set
            {
                if (value == null)
                {
                    this.CategoriesList = null;
                }
                else
                {
                    if (this.CategoriesList == null)
                        this.CategoriesList = new UserCategorySerializationInfoList();
                    else
                        this.CategoriesList.Clear();
                    this.CategoriesList.AddRange(value);
                }
            }
        }

        public void WriteToDirectory(string path)
        {
            this.WriteToDirectory(path, XmlObjectSerializer.Default);
        }

        public void WriteToDirectory(string path, IObjectSerializer serializer)
        {
            var uri = new Uri(path);
            foreach (var item in this.Categories)
            {
                var pathList = new List<string>() { path };
                var segments = StringUtility.Split(item, PathUtility.SeparatorChar, true);
                pathList.AddRange(segments);
                DirectoryUtility.Prepare(pathList.ToArray());
            }

            foreach (var item in this.Users)
            {
                var pathList = new List<string>() { path };
                var segments = StringUtility.Split(item.CategoryPath, PathUtility.SeparatorChar, true);
                pathList.AddRange(segments);
                pathList.Add(item.ID);
                var itemPath = FileUtility.Prepare(pathList.ToArray());
                serializer.Serialize(itemPath, item, ObjectSerializerSettings.Empty);
            }
        }

        public static UserContextSerializationInfo ReadFromDirectory(string path)
        {
            var instance = new UserContextSerializationInfo()
            {
                CategoriesList = new UserCategorySerializationInfoList(),
            };

            var directories = DirectoryUtility.GetAllDirectories(path, "*", true);
            foreach (var item in directories)
            {
                var relativePath = UriUtility.MakeRelativeOfDirectory(path, item);
                var categoryPath = relativePath.WrapSeparator();
                instance.CategoriesList.Add(categoryPath);
            }

            var files = DirectoryUtility.GetAllFiles(path, "*.xml");
            var userList = new List<UserSerializationInfo>(files.Length);
            foreach (var item in files)
            {
                var fileInfo = new FileInfo(item);
                if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden) == true)
                    continue;
                try
                {
                    var user = DataContractSerializerUtility.Read<UserSerializationInfo>(item);
                    userList.Add(user);
                }
                catch
                {

                }
            }
            instance.Users = userList.ToArray();

            return instance;
        }
    }
}
