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
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library;
using Ntreev.Crema.Data.Xml;
using System.IO;

namespace Ntreev.Crema.Services.Data
{
    sealed class DataBaseRepositoryHost : RepositoryHost
    {
        private readonly DataBase dataBase;
        private readonly CremaSettings settings;

        public DataBaseRepositoryHost(DataBase dataBase, IRepository repository)
            : base(repository, dataBase.CremaHost.RepositoryDispatcher, dataBase.BasePath)
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

        public CremaDataSet GetTypeData(string repositoryPath, string typeSchemaPath, string revision)
        {
            string tempPath = PathUtility.GetTempPath(true);

            try
            {
                var revisionValue = revision ?? this.RepositoryInfo.Revision;
                var repoUri = this.GetUri(repositoryPath, revisionValue);
                var schemaUri = this.GetUri(typeSchemaPath, revisionValue);
                var baseUri = this.GetDataBaseUri($"{repoUri}", $"{schemaUri}");
                var schemaPath = this.Export(schemaUri, tempPath);

                var pureBaseUri = $"{baseUri}".Replace($"@{revisionValue}", string.Empty);
                var pureSchemaUri = $"{schemaUri}".Replace($"@{revisionValue}", string.Empty);

                var dataBaseRelativeUri = UriUtility.MakeRelativeOfDirectory(pureSchemaUri, pureBaseUri);
                var dataBasePath = UriUtility.Combine(new Uri(schemaPath), dataBaseRelativeUri).LocalPath;
                return CremaDataSet.ReadFromDirectory(dataBasePath);
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }

        public CremaDataSet GetTypeCategoryData(string repositoryPath, string localPath, string revision)
        {
            var tempPath = PathUtility.GetTempPath(true);

            try
            {
                var revisionValue = revision ?? this.RepositoryInfo.Revision;
                var repoUri = this.GetUri(repositoryPath, revisionValue);
                var categoryUri = this.GetUri(localPath, revisionValue);
                var baseUri = this.GetDataBaseUri($"{repoUri}", $"{categoryUri}");
                var categoryPath = this.Export(categoryUri, tempPath);
                var pureBaseUri = $"{baseUri}".Replace($"@{revisionValue}", string.Empty);
                var pureCategoryUri = $"{categoryUri}".Replace($"@{revisionValue}", string.Empty);
                var dataBaseRelativeUri = UriUtility.MakeRelativeOfDirectory(pureCategoryUri, pureBaseUri);
                var dataBasePath = UriUtility.Combine(new Uri(categoryPath), dataBaseRelativeUri).LocalPath;
                return CremaDataSet.ReadFromDirectory(dataBasePath);
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }

        public CremaDataSet GetTableData(IObjectSerializer serializer, Table table, string revision)
        {
            var tempPath = PathUtility.GetTempPath(true);
            try
            {
                var itemPath = table.ItemPath;
                var templatedParent = table.TemplatedParent?.Path;
                var props = new RelativeSchemaPropertyCollection(table.ItemPath, table.TemplatedParent?.ItemPath);
                var files = serializer.GetPath(itemPath, typeof(CremaDataTable), props);
                foreach (var item in files)
                {
                    this.ExportItem(item, tempPath, revision);
                }

                var referencedfiles = serializer.GetReferencedPath(itemPath, typeof(CremaDataTable), SerializationPropertyCollection.Empty);
                foreach (var item in referencedfiles)
                {
                    this.ExportItem(item, tempPath, revision);
                }

                return serializer.Deserialize(tempPath, typeof(CremaDataSet), SerializationPropertyCollection.Empty) as CremaDataSet;
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }

        private void ExportItem(string path, string exportPath, string revision)
        {
            var revisionValue = revision ?? this.RepositoryInfo.Revision;
            var relativeItemUri = UriUtility.MakeRelativeOfDirectory(this.RepositoryPath, path);
            var itemUri = UriUtility.Combine(exportPath, relativeItemUri);
            var itemTempPath = new Uri(itemUri).LocalPath;
            if (File.Exists(itemTempPath) == false)
            {
                var itemRevisionUri = this.GetUri(path, revisionValue);
                var exportSchemaUri = this.Export(itemRevisionUri, exportPath);
            }
        }

        private void ExportItem2(string path, string exportPath, string revision)
        {
            var revisionValue = revision ?? this.RepositoryInfo.Revision;
            var relativeItemUri = UriUtility.MakeRelativeOfDirectory(exportPath, path);
            var itemUri = UriUtility.Combine(exportPath, relativeItemUri);
            var itemTempPath = new Uri(itemUri).LocalPath;
            if (File.Exists(itemTempPath) == false)
            {
                var itemRevisionUri = new Uri(UriUtility.Combine(this.RepositoryPath, relativeItemUri + $"@{revisionValue}"));
                var exportSchemaUri = this.Export(itemRevisionUri, exportPath);
            }
        }

        public CremaDataSet GetTableCategoryData(IObjectSerializer serializer, string localPath, string revision)
        {
            var tempPath = PathUtility.GetTempPath(true);
            try
            {
                var revisionValue = revision ?? this.RepositoryInfo.Revision;
                var repoUri = this.GetUri(this.RepositoryPath, revisionValue);
                var categoryUri = this.GetUri(localPath, revisionValue);
                var categoryPath = this.Export(categoryUri, tempPath);
                var baseUri = this.GetDataBaseUri($"{repoUri}", $"{categoryUri}");

                var items = serializer.GetItemPaths(categoryPath, typeof(CremaDataTable), SerializationPropertyCollection.Empty);
                foreach (var item in items)
                {
                    var files = serializer.GetPath(item, typeof(CremaDataTable), SerializationPropertyCollection.Empty);
                    foreach (var f in files)
                    {
                        this.ExportItem2(f, tempPath, revision);
                    }

                    var referencedfiles = serializer.GetReferencedPath(item, typeof(CremaDataTable), SerializationPropertyCollection.Empty);
                    foreach (var f in referencedfiles)
                    {
                        this.ExportItem2(f, tempPath, revision);
                    }
                }

                return serializer.Deserialize(tempPath, typeof(CremaDataSet), SerializationPropertyCollection.Empty) as CremaDataSet;
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }
    }
}
