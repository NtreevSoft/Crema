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

        public CremaDataSet GetTableData(string repositoryPath, string tableXmlPath, string tableSchemaPath, string revision)
        {
            var tempPath = PathUtility.GetTempPath(true);

            try
            {
                var revisionValue = revision ?? this.RepositoryInfo.Revision;
                var repoUri = this.GetUri(repositoryPath, revisionValue);
                var xmlUri = this.GetUri(tableXmlPath, revisionValue);
                var baseUri = this.GetDataBaseUri($"{repoUri}", $"{xmlUri}");

                var xmlPath = this.Export(xmlUri, tempPath);
                var xmlInfo = new CremaXmlReadInfo(xmlPath);

                var schemaUri = UriUtility.Combine(UriUtility.GetDirectoryName(xmlUri), xmlInfo.RelativeSchemaPath + "@" + revisionValue);
                var schemaPath = this.Export(schemaUri, tempPath);

                var xsdInfo = new CremaSchemaReadInfo(schemaPath);

                foreach (var item in xsdInfo.RelativeTypePaths)
                {
                    var typeUri = UriUtility.Combine(UriUtility.GetDirectoryName(schemaUri), $"{item}@{revisionValue}");
                    this.Export(typeUri, tempPath);
                }

                var pureBaseUri = $"{baseUri}".Replace($"@{revisionValue}", string.Empty);
                var pureTableXmlUri = $"{xmlUri}".Replace($"@{revisionValue}", string.Empty);

                var dataBaseRelativeUri = UriUtility.MakeRelativeOfDirectory(pureTableXmlUri, pureBaseUri);
                var dataBasePath = UriUtility.Combine(new Uri(xmlPath), dataBaseRelativeUri).LocalPath;
                return CremaDataSet.ReadFromDirectory(dataBasePath);
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }

        public CremaDataSet GetTableCategoryData(string repositoryPath, string localPath, string revision)
        {
            var tempPath = PathUtility.GetTempPath(true);

            try
            {
                var revisionValue = revision ?? this.RepositoryInfo.Revision;
                var repoUri = this.GetUri(repositoryPath, revisionValue);
                var categoryUri = this.GetUri(localPath, revisionValue);
                var categoryPath = this.Export(categoryUri, tempPath);
                var baseUri = this.GetDataBaseUri($"{repoUri}", $"{categoryUri}");

                var files = DirectoryUtility.GetAllFiles(categoryPath, "*" + CremaSchema.XmlExtension);
                foreach (var item in files)
                {
                    var relativeItemUri = UriUtility.MakeRelative(categoryPath, item);
                    var itemUri = UriUtility.Combine(UriUtility.GetDirectoryName(categoryUri), $"{relativeItemUri}@{revisionValue}");
                    var xmlInfo = new CremaXmlReadInfo(item);
                    var schemaUri = UriUtility.Combine(UriUtility.GetDirectoryName(itemUri), $"{xmlInfo.RelativeSchemaPath}@{revisionValue}");
                    var schemaPath = new Uri(UriUtility.Combine(Path.GetDirectoryName(item), xmlInfo.RelativeSchemaPath)).LocalPath;

                    if (File.Exists(schemaPath) == false)
                    {
                        this.Export(schemaUri, tempPath);
                    }
                    ExportTypes(schemaUri, schemaPath);
                }

                void ExportTypes(Uri schemaUri, string schemaPath)
                {
                    var xsdInfo = new CremaSchemaReadInfo(schemaPath);
                    foreach (var item in xsdInfo.RelativeTypePaths)
                    {
                        var typeUri = UriUtility.Combine(UriUtility.GetDirectoryName(schemaUri), $"{item}@{revisionValue}");
                        var typePath = new Uri(UriUtility.Combine(Path.GetDirectoryName(schemaPath), item)).LocalPath;
                        if (File.Exists(typePath) == false)
                        {
                            this.Export(typeUri, tempPath);
                        }
                    }
                }

                var pureBaseUri = $"{repoUri}".Replace($"@{revisionValue}", string.Empty);
                var pureCategoryUri = $"{categoryUri}".Replace($"@{revisionValue}", string.Empty);
                var dataBaseRelativeUri = UriUtility.MakeRelative(pureCategoryUri, pureBaseUri);
                var dataBasePath = UriUtility.Combine(new Uri(categoryPath), Path.GetDirectoryName(dataBaseRelativeUri)).LocalPath;
                return CremaDataSet.ReadFromDirectory(dataBasePath);
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }
    }
}
