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

using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Crema.Services.Users;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Ntreev.Crema.SvnModule
{
    [Export(typeof(IRepositoryProvider))]
    [Export(typeof(IConfigurationPropertyProvider))]
    class SvnRepositoryProvider : IRepositoryProvider, IConfigurationPropertyProvider
    {
        public const string repositoryName = "crema";
        public const string remoteName = "svn";
        public const string trunkName = "trunk";
        public const string tagsName = "tags";
        private readonly Serializer propertySerializer = new SerializerBuilder().Build();

        [ImportMany]
        private IEnumerable<Lazy<ILogService>> logServices = null;

        [ImportingConstructor]
        public SvnRepositoryProvider()
        {

        }

        public string Name
        {
            get { return "svn"; }
        }

        public IRepository CreateInstance(string basePath, string repositoryName, string workingPath)
        {
            var baseUri = new Uri(basePath);
            var url = repositoryName == "default" ? UriUtility.Combine(baseUri, "trunk") : UriUtility.Combine(baseUri, "branches", repositoryName);
            //throw new NotImplementedException();
            var logService = this.logServices.FirstOrDefault(item => item.Value.Name == "repository");

            if (Directory.Exists(workingPath) == false)
            {
                SvnClientHost.Run("checkout", url.ToString().WrapQuot(), workingPath.WrapQuot());
            }
            else
            {
                SvnClientHost.Run("update", workingPath.WrapQuot());
            }

            return new SvnRepository(logService.Value, workingPath, null);
        }

        public void InitializeRepository(string basePath, string initPath)
        {
            var baseUri = new Uri(basePath);
            var tempPath = PathUtility.GetTempPath(true);
            var tagsPath = DirectoryUtility.Prepare(tempPath, "tags");
            var branchesPath = DirectoryUtility.Prepare(tempPath, "branches");
            var trunkPath = DirectoryUtility.Prepare(tempPath, "trunk");

            SvnServerHost.Run("create", basePath.WrapQuot(), "--fs-type", "fsfs");
            DirectoryUtility.Copy(initPath, trunkPath);
            SvnClientHost.Run("import", "-m", "first", tempPath.WrapQuot(), baseUri.ToString().WrapQuot());
        }

        public void CreateRepository(string basePath, string initPath, string comment, params LogPropertyInfo[] properties)
        {
            var commentPath = PathUtility.GetTempFileName();
            var uri = $"\"{UriUtility.Combine(new Uri(basePath), "branches")}\"";
            try
            {
                File.WriteAllText(commentPath, this.GenerateComment(comment, properties));
                SvnClientHost.Run("import", initPath, uri, "--file", $"\"{commentPath}\"", "--force");
            }
            finally
            {
                FileUtility.Delete(commentPath);
            }
        }

        public void CopyRepository(string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties)
        {
            var commentPath = PathUtility.GetTempFileName();
            var uri = $"\"{this.GetUrl(basePath, repositoryName)}\"";
            var newUri = $"\"{this.GenerateUrl(basePath, newRepositoryName)}\"";
            try
            {
                File.WriteAllText(commentPath, this.GenerateComment(comment, properties));
                SvnClientHost.Run("copy", "--file", $"\"{commentPath}\"", uri, newUri);
            }
            finally
            {
                FileUtility.Delete(commentPath);
            }
        }

        public void RenameRepository(string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties)
        {
            var commentPath = PathUtility.GetTempFileName();
            var uri = $"\"{this.GetUrl(basePath, repositoryName)}\"";
            var newUri = $"\"{this.GenerateUrl(basePath, newRepositoryName)}\"";
            try
            {
                File.WriteAllText(commentPath, this.GenerateComment(comment, properties));
                SvnClientHost.Run("move", "--file", $"\"{commentPath}\"", uri, newUri);
            }
            finally
            {
                FileUtility.Delete(commentPath);
            }
        }

        public void DeleteRepository(string basePath, string[] repositoryNames, string comment, params LogPropertyInfo[] properties)
        {
            var query = from item in repositoryNames
                        let uri = this.GetUrl(basePath, item)
                        select $"\"{uri}\"";

            var commentPath = PathUtility.GetTempFileName();
            try
            {
                File.WriteAllText(commentPath, this.GenerateComment(comment, properties));
                var argList = new List<object>()
                {
                    "delete", "--file", $"\"{commentPath}\"",
                };
                argList.AddRange(query);

                SvnClientHost.Run(argList.ToArray());
            }
            finally
            {
                FileUtility.Delete(commentPath);
            }
        }

        public IEnumerable<string> GetRepositories(string basePath)
        {
            var uri = new Uri(basePath);
            var list = SvnClientHost.Run("list", $"{uri}".WrapQuot());
            var sr = new StringReader(list);
            var line = string.Empty;

            while ((line = sr.ReadLine()) != null)
            {
                if (line.EndsWith(PathUtility.Separator) == true)
                {
                    var name = line.Substring(0, line.Length - PathUtility.Separator.Length);
                    if (name == "trunk")
                    {
                        yield return "default";
                    }
                    else if (name == "tags" || name == "branches")
                    {
                        var subPath = Path.Combine(basePath, name);
                        foreach (var item in this.GetRepositories(subPath))
                        {
                            yield return item;
                        }
                    }
                    else
                    {
                        yield return name;
                    }
                }
            }
        }

        public void ValidateRepository(string basePath, string repositoryPath)
        {
            if (DirectoryUtility.Exists(basePath) == false)
                throw new DirectoryNotFoundException($"base path does not exists :\"{basePath}\"");
            if (DirectoryUtility.Exists(repositoryPath) == false)
                throw new DirectoryNotFoundException($"repository path does not exists :\"{repositoryPath}\"");
        }

        public string GetRevision(string basePath, string repositoryName)
        {
            var uri = this.GetUrl(basePath, repositoryName);
            var args = SvnInfoEventArgs.Run($"{uri}".WrapQuot());
            return $"{args.Revision}";
        }

        public RepositoryInfo GetRepositoryInfo(string basePath, string repositoryName)
        {
            var uri = this.GetUrl(basePath, repositoryName);

            var latestLog = SvnLogEventArgs.Run(uri.ToString(), null, 1).First();
            int qwr = 0;

            this.GetBranchInfo(uri.ToString(), out var l, out var s, out var sl);

            var branchLog = SvnLogEventArgs.Run(uri.ToString(), l, 1).First();
            var branchUserID = branchLog.GetPropertyString(LogPropertyInfo.UserIDKey) ?? string.Empty;
            var latestUserID = latestLog.GetPropertyString(LogPropertyInfo.UserIDKey) ?? string.Empty;

            var repositoryInfo = new RepositoryInfo()
            {
                ID = GuidUtility.FromName(repositoryName + l),
                Name = repositoryName,
                Comment = latestLog.Comment,
                Revision = latestLog.Revision,
                BranchRevision = l,
                BranchSource = s,
                BranchSourceRevision = sl,
                CreationInfo = new SignatureDate(branchUserID, branchLog.DateTime),
                ModificationInfo = new SignatureDate(latestUserID, latestLog.DateTime),
            };
            return repositoryInfo;

            //var branchRevision = this.Repository.BranchRevision;
            //var branchLog = this.Repository.GetLog(this.basePath, branchRevision, 1).First();
            //var latestLog = this.Repository.GetLog(this.basePath, this.repositoryHost.Revision, 1).First();
            //var branchUserID = branchLog.GetPropertyString(LogPropertyInfo.UserIDKey) ?? string.Empty;
            //var latestUserID = latestLog.GetPropertyString(LogPropertyInfo.UserIDKey) ?? string.Empty;
            //var uri = this.Repository.GetUri(this.basePath, branchRevision);
            //var branchName = uri.Segments.Last();
            //base.DataBaseInfo = new DataBaseInfo()
            //{
            //    Name = base.Name,
            //    Revision = latestLog.Revision,
            //    Comment = branchLog.Comment.Decompress(),
            //    BranchRevision = this.Repository.BranchRevision,
            //    BranchSource = this.Repository.BranchSource,
            //    BranchSourceRevision = this.Repository.BranchSourceRevision,
            //    CreationInfo = new SignatureDate(branchUserID, branchLog.DateTime),
            //    ModificationInfo = new SignatureDate(latestUserID, latestLog.DateTime),
            //    ID = base.Name == DataBase.defaultName ? DataBase.defaultID : GuidUtility.FromName(branchName + branchLog.Revision.ToString())
            //};

            throw new NotImplementedException();
        }

        public string[] GetRepositoryItemList(string basePath, string repositoryName)
        {
            var uri = this.GetUrl(basePath, repositoryName);
            var text = SvnClientHost.Run("list", $"\"{uri}\"", "-R");
            var lines = text.Split(new string[] { Environment.NewLine, }, StringSplitOptions.RemoveEmptyEntries);
            return lines.Select(item => PathUtility.Separator + item).ToArray();
        }

        public LogInfo[] GetLog(string basePath, string repositoryName, string revision, int count)
        {
            var uri = $"\"{this.GetUrl(basePath, repositoryName)}\"";
            var logs = SvnLogEventArgs.Run(uri, revision, count);
            return logs.Select(item => (LogInfo)item).ToArray();
        }

        private Uri GetUrl(string basePath, string repositoryName)
        {
            var paths = this.GetRepositoryPaths(basePath).ToDictionary(item => item.Key, item => item.Value);
            var uri = paths[repositoryName];
            //var uri = repositoryName == "default" ? UriUtility.Combine(baseUri, "trunk") : UriUtility.Combine(baseUri, "tags", repositoryName);
            return uri;
        }

        private Uri GenerateUrl(string basePath, string repositoryName)
        {
            var baseUri = new Uri(basePath);
            return UriUtility.Combine(baseUri, "branches", repositoryName);
        }

        public void GetBranchInfo(string path, out string revision, out string source, out string sourceRevision)
        {
            var info = SvnInfoEventArgs.Run(path);
            this.GetBranchRevision(info.RepositoryRoot, info.Uri, out revision, out source, out sourceRevision);
        }

        private void GetBranchRevision(Uri repositoryRoot, Uri uri, out string revision, out string source, out string sourceRevision)
        {
            var log = SvnLogEventArgs.Runa($"{uri}", "--xml -v --stop-on-copy").Last();
            var relativeUri = repositoryRoot.MakeRelativeUri(uri);

            var localPath = $"/{relativeUri}";
            var oldPath = string.Empty;
            var oldRevision = null as string;

            revision = log.Revision;
            source = null;
            sourceRevision = log.Revision;
            foreach (var item in log.ChangedPaths)
            {
                if (item.Action == "A" && item.Path == localPath)
                {
                    oldPath = item.CopyFromPath;
                    oldRevision = item.CopyFromRevision;
                    source = Path.GetFileName(item.CopyFromPath);
                    sourceRevision = item.CopyFromRevision;
                }
            }

            if (oldPath == string.Empty)
                return;

            foreach (var item in log.ChangedPaths)
            {
                if (item.Action == "D" && item.Path == oldPath)
                {
                    var url = new Uri(repositoryRoot + item.Path.Substring(1) + "@" + oldRevision);
                    GetBranchRevision(repositoryRoot, url, out revision, out source, out sourceRevision);
                    return;
                }
            }
        }

        public IEnumerable<KeyValuePair<string, Uri>> GetRepositoryPaths(string basePath)
        {
            var uri = new Uri(basePath);
            var list = SvnClientHost.Run("list", $"{uri}".WrapQuot());
            var sr = new StringReader(list);
            var line = string.Empty;

            while ((line = sr.ReadLine()) != null)
            {
                if (line.EndsWith(PathUtility.Separator) == true)
                {
                    var name = line.Substring(0, line.Length - PathUtility.Separator.Length);
                    if (name == "trunk")
                    {
                        yield return new KeyValuePair<string, Uri>("default", UriUtility.Combine(uri, name));
                    }
                    else if (name == "tags" || name == "branches")
                    {
                        var subPath = Path.Combine(basePath, name);
                        foreach (var item in this.GetRepositoryPaths(subPath))
                        {
                            yield return item;
                        }
                    }
                    else
                    {
                        yield return new KeyValuePair<string, Uri>(name, UriUtility.Combine(uri, name));
                    }
                }
            }
        }

        private string GenerateComment(string comment, params LogPropertyInfo[] properties)
        {
            var commentInfo = new SvnCommentInfo()
            {
                Comment = comment,
                Properties = properties,
            };

            


            return this.propertySerializer.Serialize(commentInfo);
        }
    }
}
