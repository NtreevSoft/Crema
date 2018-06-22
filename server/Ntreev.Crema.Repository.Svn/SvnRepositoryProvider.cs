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

namespace Ntreev.Crema.Repository.Svn
{
    [Export(typeof(IRepositoryProvider))]
    [Export(typeof(IConfigurationPropertyProvider))]
    class SvnRepositoryProvider : IRepositoryProvider, IConfigurationPropertyProvider
    {
        public const string remoteName = "svn";
        public const string trunkName = "trunk";
        public const string tagsName = "tags";
        public const string defaultName = "default";
        private const string commentHeader = "# revision properties";
        private static readonly Serializer propertySerializer = new SerializerBuilder().Build();
        private static readonly Deserializer propertyDeserializer = new Deserializer();

        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        [ImportingConstructor]
        public SvnRepositoryProvider()
        {

        }

        public string Name
        {
            get { return "svn"; }
        }

        public IRepository CreateInstance(RepositorySettings settings)
        {
            var baseUri = new Uri(settings.BasePath);
            var repositoryName = settings.RepositoryName == string.Empty ? defaultName : settings.RepositoryName;
            var url = repositoryName == defaultName ? UriUtility.Combine(baseUri, trunkName) : UriUtility.Combine(baseUri, "branches", settings.RepositoryName);

            if (Directory.Exists(settings.WorkingPath) == false)
            {
                SvnClientHost.Run("checkout", url.ToString().ToSvnPath(), settings.WorkingPath.ToSvnPath());
            }
            else
            {
                SvnClientHost.Run("update", settings.WorkingPath.ToSvnPath());
            }

            var repositoryInfo = this.GetRepositoryInfo(settings.BasePath, repositoryName);
            return new SvnRepository(this, settings.LogService, settings.WorkingPath, settings.TransactionPath, repositoryInfo);
        }

        public void InitializeRepository(string basePath, string initPath)
        {
            var baseUri = new Uri(basePath);
            var tempPath = PathUtility.GetTempPath(true);
            var tagsPath = DirectoryUtility.Prepare(tempPath, "tags");
            var branchesPath = DirectoryUtility.Prepare(tempPath, "branches");
            var trunkPath = DirectoryUtility.Prepare(tempPath, "trunk");

            SvnServerHost.Run("create", basePath.ToSvnPath(), "--fs-type", "fsfs");
            DirectoryUtility.Copy(initPath, trunkPath);
            SvnClientHost.Run("import", "-m", "first", tempPath.ToSvnPath(), baseUri.ToString().ToSvnPath());
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

        public void DeleteRepository(string basePath, string repositoryName, string comment, params LogPropertyInfo[] properties)
        {
            var commentPath = PathUtility.GetTempFileName();
            var uri = this.GetUrl(basePath, repositoryName);

            try
            {
                File.WriteAllText(commentPath, this.GenerateComment(comment, properties));
                var argList = new List<object>()
                {
                    "delete", "--file", $"\"{commentPath}\"", $"\"{uri}\""
                };

                SvnClientHost.Run(argList.ToArray());
            }
            finally
            {
                FileUtility.Delete(commentPath);
            }
        }

        public void ValidateRepository(string basePath, string repositoryPath)
        {
            if (DirectoryUtility.Exists(basePath) == false)
                throw new DirectoryNotFoundException($"base path does not exists :\"{basePath}\"");
            if (DirectoryUtility.Exists(repositoryPath) == false)
                throw new DirectoryNotFoundException($"repository path does not exists :\"{repositoryPath}\"");
        }

        public string[] GetRepositories(string basePath)
        {
            var uri = new Uri(basePath);
            var list = SvnClientHost.Run("list", $"{uri}".ToSvnPath());
            var sr = new StringReader(list);
            var line = string.Empty;
            var itemList = new List<string>();
            while ((line = sr.ReadLine()) != null)
            {
                if (line.EndsWith(PathUtility.Separator) == true)
                {
                    var name = line.Substring(0, line.Length - PathUtility.Separator.Length);
                    if (name == "trunk")
                    {
                        itemList.Add("default");
                    }
                    else if (name == "tags" || name == "branches")
                    {
                        var subPath = Path.Combine(basePath, name);
                        itemList.AddRange(this.GetRepositories(subPath));
                    }
                    else
                    {
                        itemList.Add(name);
                    }
                }
            }
            return itemList.ToArray();
        }

        public string GetRevision(string basePath, string repositoryName)
        {
            var uri = this.GetUrl(basePath, repositoryName);
            var args = SvnInfoEventArgs.Run($"{uri}".ToSvnPath());
            return $"{args.Revision}";
        }

        public RepositoryInfo GetRepositoryInfo(string basePath, string repositoryName)
        {
            var uri = this.GetUrl(basePath, repositoryName);
            var latestLog = SvnLogEventArgs.Run(uri.ToString(), null, 1).First();

            this.GetBranchInfo(uri.ToString(), out var branchRevision, out var branchSource, out var branchSourceRevision);

            var branchLog = SvnLogEventArgs.Run(uri.ToString(), branchRevision, 1).First();
            var branchUserID = branchLog.GetPropertyString(LogPropertyInfo.UserIDKey) ?? string.Empty;
            var latestUserID = latestLog.GetPropertyString(LogPropertyInfo.UserIDKey) ?? string.Empty;

            var repositoryInfo = new RepositoryInfo()
            {
                ID = GuidUtility.FromName(repositoryName + branchRevision),
                Name = repositoryName,
                Comment = latestLog.Comment,
                Revision = latestLog.Revision,
                BranchRevision = branchRevision,
                BranchSource = branchSource,
                BranchSourceRevision = branchSourceRevision,
                CreationInfo = new SignatureDate(branchUserID, branchLog.DateTime),
                ModificationInfo = new SignatureDate(latestUserID, latestLog.DateTime),
            };
            return repositoryInfo;
        }

        public string[] GetRepositoryItemList(string basePath, string repositoryName)
        {
            var uri = this.GetUrl(basePath, repositoryName);
            var text = SvnClientHost.Run("list", $"\"{uri}\"", "-R");
            var lines = text.Split(new string[] { Environment.NewLine, }, StringSplitOptions.RemoveEmptyEntries);
            return lines.Select(item => PathUtility.Separator + item).ToArray();
        }

        public LogInfo[] GetLog(string basePath, string repositoryName, int count)
        {
            var uri = $"\"{this.GetUrl(basePath, repositoryName)}\"";
            var logs = SvnLogEventArgs.Run(uri, null, count);
            return logs.Select(item => (LogInfo)item).ToArray();
        }

        public IEnumerable<KeyValuePair<string, Uri>> GetRepositoryPaths(string basePath)
        {
            var uri = new Uri(basePath);
            var list = SvnClientHost.Run("list", $"{uri}".ToSvnPath());
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

        public string GenerateComment(string comment, params LogPropertyInfo[] properties)
        {
            var propText = propertySerializer.Serialize(properties);
            var sb = new StringBuilder();
            sb.AppendLine(comment);
            sb.AppendLine();
            sb.AppendLine(commentHeader);
            sb.Append(propText);
            return sb.ToString();
        }

        public static void ParseComment(string message, out string comment, out LogPropertyInfo[] properties)
        {
            comment = string.Empty;
            properties = new LogPropertyInfo[] { };

            try
            {
                var index = message.IndexOf(commentHeader);
                if (index >= 0)
                {
                    var propText = message.Substring(index);
                    comment = message.Remove(index);

                    var sr = new StringReader(comment);
                    var lineList = new List<string>();
                    var line = null as string;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lineList.Add(line);
                    }

                    if (lineList.Last() == string.Empty)
                        lineList.RemoveAt(lineList.Count - 1);
                    comment = string.Join(Environment.NewLine, lineList);

                    properties = propertyDeserializer.Deserialize<LogPropertyInfo[]>(propText);
                }
            }
            catch
            {
                comment = null;
                properties = null;
            }
        }

        private Uri GetUrl(string basePath, string repositoryName)
        {
            var paths = this.GetRepositoryPaths(basePath).ToDictionary(item => item.Key, item => item.Value);
            return repositoryName == string.Empty ? paths[defaultName] : paths[repositoryName];
        }

        private Uri GenerateUrl(string basePath, string repositoryName)
        {
            var baseUri = new Uri(basePath);
            return UriUtility.Combine(baseUri, "branches", repositoryName);
        }

        private void GetBranchInfo(string path, out string revision, out string source, out string sourceRevision)
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

        private ICremaHost CremaHost => this.cremaHost.Value;
    }
}
