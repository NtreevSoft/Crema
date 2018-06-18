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

#pragma warning disable 0612
using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ntreev.Crema.Services.Data
{
    sealed class DataBaseRepositoryHost : RepositoryHost
    {
        private readonly DataBase dataBase;
        private readonly CremaSettings settings;

        public DataBaseRepositoryHost(DataBase dataBase, IRepository repository)
            : base(repository, dataBase.CremaHost.RepositoryDispatcher)
        {
            this.dataBase = dataBase;
            this.settings = this.dataBase.GetService(typeof(CremaSettings)) as CremaSettings;
        }

        public void Commit(Authentication authentication, string comment)
        {
            var props = new List<LogPropertyInfo>
            {
                new LogPropertyInfo() { Key = LogPropertyInfo.BranchRevisionKey, Value = $"{this.RepositoryInfo.BranchRevision}"},
                new LogPropertyInfo() { Key = LogPropertyInfo.BranchSourceKey, Value = $"{this.RepositoryInfo.BranchSource}"},
                new LogPropertyInfo() { Key = LogPropertyInfo.BranchSourceRevisionKey, Value = $"{this.RepositoryInfo.BranchSourceRevision}"},
            };

#if DEBUG
            if (this.settings.ValidationMode == true)
            {
                try
                {
                    var dataSet = Ntreev.Crema.Data.CremaDataSet.ReadFromDirectory(this.dataBase.BasePath);
                }
                catch (Exception e)
                {
                    this.dataBase.CremaHost.Error("DataSet Error");
                    this.dataBase.CremaHost.Error(e);
                }
            }
#endif
            base.Commit(authentication, comment, props.ToArray());
        }

        public CremaDataSet GetTypeData(IObjectSerializer serializer, string itemPath, string revision)
        {
            var tempPath = PathUtility.GetTempPath(true);
            try
            {
                var itemList = new List<string>();
                var files = serializer.GetPath(itemPath, typeof(CremaDataType), ObjectSerializerSettings.Empty);
                foreach (var item in files)
                {
                    var exportPath = this.ExportItem(item, tempPath, revision);
                    itemList.Add(FileUtility.RemoveExtension(exportPath));
                }

                var referencedFiles = serializer.GetReferencedPath(itemPath, typeof(CremaDataType), ObjectSerializerSettings.Empty);
                foreach (var item in referencedFiles)
                {
                    this.ExportItem(item, tempPath, revision);
                }

                var props = new CremaDataSetSerializerSettings(itemList.ToArray(), null);
                return serializer.Deserialize(tempPath, typeof(CremaDataSet), props) as CremaDataSet;
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }

        public CremaDataSet GetTypeCategoryData(IObjectSerializer serializer, string itemPath, string revision)
        {
            var tempPath = PathUtility.GetTempPath(true);
            try
            {
                var revisionValue = revision ?? this.RepositoryInfo.Revision;
                var repoUri = this.GetUri(this.RepositoryPath, revisionValue);
                var categoryUri = this.GetUri(itemPath, revisionValue);
                var categoryPath = this.Export(categoryUri, tempPath);
                var baseUri = this.GetDataBaseUri($"{repoUri}", $"{categoryUri}");
                var items = serializer.GetItemPaths(categoryPath, typeof(CremaDataType), ObjectSerializerSettings.Empty);
                var props = new CremaDataSetSerializerSettings(items, null);
                return serializer.Deserialize(tempPath, typeof(CremaDataSet), ObjectSerializerSettings.Empty) as CremaDataSet;
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }

        public CremaDataSet GetTableData(IObjectSerializer serializer, string itemPath, string templateItemPath, string revision)
        {
            var tempPath = PathUtility.GetTempPath(true);
            try
            {
                var props = new CremaDataTableSerializerSettings(itemPath, templateItemPath);
                var files = serializer.GetPath(itemPath, typeof(CremaDataTable), props);
                foreach (var item in files)
                {
                    this.ExportItem(item, tempPath, revision);
                }

                var referencedFiles = serializer.GetReferencedPath(itemPath, typeof(CremaDataTable), ObjectSerializerSettings.Empty);
                foreach (var item in referencedFiles)
                {
                    this.ExportItem(item, tempPath, revision);
                }

                return serializer.Deserialize(tempPath, typeof(CremaDataSet), ObjectSerializerSettings.Empty) as CremaDataSet;
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }

        public CremaDataSet GetTableCategoryData(IObjectSerializer serializer, string itemPath, string revision)
        {
            var tempPath = PathUtility.GetTempPath(true);
            try
            {
                var revisionValue = revision ?? this.RepositoryInfo.Revision;
                var repoUri = this.GetUri(this.RepositoryPath, revisionValue);
                var categoryUri = this.GetUri(itemPath, revisionValue);
                var categoryPath = this.Export(categoryUri, tempPath);
                var baseUri = this.GetDataBaseUri($"{repoUri}", $"{categoryUri}");

                var items = serializer.GetItemPaths(categoryPath, typeof(CremaDataTable), ObjectSerializerSettings.Empty);
                foreach (var item in items)
                {
                    var files = serializer.GetPath(item, typeof(CremaDataTable), ObjectSerializerSettings.Empty);
                    foreach (var f in files)
                    {
                        this.ExportRevisionItem(f, tempPath, revision);
                    }

                    var referencedFiles = serializer.GetReferencedPath(item, typeof(CremaDataTable), ObjectSerializerSettings.Empty);
                    foreach (var f in referencedFiles)
                    {
                        this.ExportRevisionItem(f, tempPath, revision);
                    }
                }

                return serializer.Deserialize(tempPath, typeof(CremaDataSet), ObjectSerializerSettings.Empty) as CremaDataSet;
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }

        private string ExportItem(string path, string exportPath, string revision)
        {
            var revisionValue = revision ?? this.RepositoryInfo.Revision;
            var relativeItemUri = UriUtility.MakeRelativeOfDirectory(this.RepositoryPath, path);
            var itemUri = UriUtility.Combine(exportPath, relativeItemUri);
            var itemTempPath = new Uri(itemUri).LocalPath;
            if (File.Exists(itemTempPath) == false)
            {
                var itemRevisionUri = this.GetUri(path, revisionValue);
                return this.Export(itemRevisionUri, exportPath);
            }
            return null;
        }

        private string ExportRevisionItem(string path, string exportPath, string revision)
        {
            var revisionValue = revision ?? this.RepositoryInfo.Revision;
            var relativeItemUri = UriUtility.MakeRelativeOfDirectory(exportPath, path);
            var itemUri = UriUtility.Combine(exportPath, relativeItemUri);
            var itemTempPath = new Uri(itemUri).LocalPath;
            if (File.Exists(itemTempPath) == false)
            {
                var itemRevisionUri = new Uri(UriUtility.Combine(this.RepositoryPath, $"{relativeItemUri}@{revisionValue}"));
                return this.Export(itemRevisionUri, exportPath);
            }
            return null;
        }
    }
}
