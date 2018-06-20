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
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Ntreev.Crema.Repository.Git
{
    [Export(typeof(IRepositoryProvider))]
    class GitRepositoryProvider : IRepositoryProvider
    {
        public const string keepExtension = ".keep";
        private const string emptyBranch = "__empty__";
        private const string commentHeader = "# revision properties";

        private static readonly Serializer propertySerializer = new SerializerBuilder().Build();
        private static readonly Deserializer propertyDeserializer = new Deserializer();

        [ImportMany]
        private IEnumerable<Lazy<ILogService>> logServices = null;

        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        public string Name => "git";

        private readonly Dictionary<Uri, string> cacheRepositories = new Dictionary<Uri, string>();

        public void CopyRepository(string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties)
        {
            throw new NotImplementedException();
        }

        public IRepository CreateInstance(RepositorySettings settings)
        {
            var baseUri = new Uri(settings.BasePath);
            var branchName = this.GetBranchName(settings.RepositoryName);
            //var logService = this.logServices.FirstOrDefault(item => item.Value.Name == "repository");

            if (Directory.Exists(settings.WorkingPath) == false)
            {
                GitServerHost.Run("clone", baseUri.ToString().ToGitPath(), "-b", branchName, settings.WorkingPath.ToGitPath(), "--single-branch");
            }
            else
            {
                //GitHost.Run("update", workingPath.ToGitPath());
            }
            //var transactionPath = Path.Combine(this.CremaHost.GetPath(CremaPath.Transactions), Path.GetFileName(workingPath));
            var repositoryInfo = this.GetRepositoryInfo(settings.BasePath, settings.RepositoryName);
            return new GitRepository(this, settings.LogService, settings.WorkingPath, settings.TransactionPath, repositoryInfo);
        }

        public void CreateRepository(string basePath, string initPath, string comment, params LogPropertyInfo[] properties)
        {
            throw new NotImplementedException();
        }

        public void DeleteRepository(string basePath, string repositoryName, string comment, params LogPropertyInfo[] properties)
        {
            throw new NotImplementedException();
        }

        public LogInfo[] GetLog(string basePath, string repositoryName, int count)
        {
            var baseUri = new Uri(basePath);
            var branchName = this.GetBranchName(repositoryName);
            var repositoryPath = baseUri.LocalPath;
            var logs = GitLogInfo.RunOnBranch(repositoryPath, branchName, $"--max-count={count}");
            return logs.Select(item => (LogInfo)item).ToArray();
        }

        public string[] GetRepositories(string basePath)
        {

            //GitItemStatusInfo.Run(@"C:\Users\s2quake\AppData\Local\Temp\tmp1D24");

            var baseUri = new Uri(basePath);
            var repositoryPath = baseUri.LocalPath;
            var text = GitHost.Run(repositoryPath, "branch", "--list");
            var matches = Regex.Matches(text, "^[*]*\\s*(\\S+)", RegexOptions.Multiline);
            var itemList = new List<string>(matches.Count);
            for (var i = 0; i < matches.Count; i++)
            {
                var item = matches[i];
                var branchName = item.Groups[1].Value;
                var repositoryName = this.GetRepositoryName(branchName);
                if (repositoryName != emptyBranch)
                    itemList.Add(repositoryName);
            }

            return itemList.ToArray();
        }

        public RepositoryInfo GetRepositoryInfo(string basePath, string repositoryName)
        {
            var baseUri = new Uri(basePath);
            var branchName = this.GetBranchName(repositoryName);
            var repositoryInfo = new RepositoryInfo();
            var repositoryPath = baseUri.LocalPath;
            var text = GitHost.Run(repositoryPath, "log", $"{branchName}", "--pretty=fuller", "--max-count=1");
            var logItems = GitLogInfo.ParseMany(text);

            var firstLog = logItems.Last();
            var latestLog = logItems.First();

            repositoryInfo.Name = repositoryName;
            repositoryInfo.Comment = latestLog.Comment;
            repositoryInfo.Revision = latestLog.CommitID;
            repositoryInfo.CreationInfo = new SignatureDate(firstLog.Author, firstLog.AuthorDate);
            repositoryInfo.ModificationInfo = new SignatureDate(latestLog.Author, latestLog.AuthorDate);

            try
            {
                var description = GitHost.Run(repositoryPath, "config", $"branch.{branchName}.description");
                var info = propertyDeserializer.Deserialize<GitBranchDescription>(description);
                repositoryInfo.ID = info.ID;
            }
            catch
            {
                var info = new GitBranchDescription()
                {
                    ID = Guid.NewGuid(),
                };
                var props = propertySerializer.Serialize(info);
                GitHost.Run(repositoryPath, "config", $"branch.{branchName}.description", props.ToGitPath());
                repositoryInfo.ID = info.ID;
            }

            //var reflog = GitHost.Run(tempPath, "reflog show", $"refs/remotes/origin/{branchName}", "--no-abbrev");

            return repositoryInfo;
        }

        public string[] GetRepositoryItemList(string basePath, string repositoryName)
        {
            var baseUri = new Uri(basePath);
            var branchName = this.GetBranchName(repositoryName);
            var repositoryPath = baseUri.LocalPath;
            var text = GitHost.Run(repositoryPath, "ls-tree", "-r", "--name-only", $"{branchName}");
            var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var itemList = new List<string>(lines.Length);

            foreach (var item in lines)
            {
                if (item.EndsWith(keepExtension) == true)
                {
                    itemList.Add(PathUtility.Separator + item.Substring(0, item.Length - keepExtension.Length));
                }
                else
                {
                    itemList.Add(PathUtility.Separator + item);
                }
            }
            return itemList.ToArray();
        }

        public string GetRevision(string basePath, string repositoryName)
        {
            throw new NotImplementedException();
        }

        public void InitializeRepository(string basePath, string repositoryPath)
        {
            GitServerHost.Run("init", basePath.ToGitPath());
            GitHost.Run(basePath, "config receive.denyCurrentBranch ignore");
            GitHost.Run(basePath, "commit --allow-empty -m \"root commit\"");
            GitHost.Run(basePath, $"branch {emptyBranch}");
            DirectoryUtility.Copy(repositoryPath, basePath);

            foreach (var item in GetEmptyDirectories(basePath))
            {
                File.WriteAllText(Path.Combine(item, keepExtension), string.Empty);
            }

            var query = from item in DirectoryUtility.GetAllFiles(basePath, "*", true)
                        select item.ToGitPath();

            var argList = new List<object>()
            {
                "add",
            };
            argList.AddRange(query);
            GitHost.Run(basePath, argList.ToArray());
            GitHost.Run(basePath, "commit -m \"first commit\"");
            
        }

        public void RenameRepository(string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties)
        {
            throw new NotImplementedException();
        }

        private string[] GetEmptyDirectories(string path)
        {
            var items = DirectoryUtility.GetAllDirectories(path, "*", true);
            var itemList = new List<string>(items.Length);
            foreach (var item in items)
            {
                if (Directory.GetFiles(item).Length == 0)
                {
                    itemList.Add(item);
                }
            }
            return itemList.ToArray();
        }

        private string GetBranchName(string repositoryName)
        {
            return repositoryName == "default" ? "master" : repositoryName;
        }

        private string GetRepositoryName(string branchName)
        {
            return branchName == "master" ? "default" : branchName;
        }

        private ICremaHost CremaHost => this.cremaHost.Value;

        //private string Fetch(Uri baseUri)
        //{
        //    if (this.cacheRepositories.ContainsKey(baseUri) == false)
        //    {
        //        var id = GuidUtility.FromName(baseUri.ToString());
        //        var repositoryPath = this.CremaHost.GetPath(CremaPath.Caches, "git-cache", $"{id}");
        //        this.cacheRepositories.Add(baseUri, repositoryPath);
        //        DirectoryUtility.Delete(repositoryPath);
        //        GitServerHost.Run("init", repositoryPath.ToGitPath());
        //        GitHost.Run(repositoryPath, "remote add origin", baseUri);
        //    }
        //    {
        //        var repositoryPath = this.cacheRepositories[baseUri];
        //        GitHost.Run(repositoryPath, "fetch");
        //        return repositoryPath;
        //    }
        //}

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
    }
}
