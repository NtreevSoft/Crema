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
    [Export]
    [Export(typeof(IRepositoryProvider))]
    class GitRepositoryProvider : IRepositoryProvider
    {
        public const string KeepExtension = ".keep";
        private const string commentHeader = "# revision properties";

        private static readonly Serializer propertySerializer = new SerializerBuilder().Build();
        private static readonly Deserializer propertyDeserializer = new Deserializer();

        private readonly Dictionary<Uri, string> cacheRepositories = new Dictionary<Uri, string>();

        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        public void CopyRepository(string author, string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties)
        {
            throw new NotImplementedException();
        }

        public IRepository CreateInstance(RepositorySettings settings)
        {
            var baseUri = new Uri(settings.BasePath);
            var repositoryName = settings.RepositoryName == string.Empty ? "master" : settings.RepositoryName;

            if (Directory.Exists(settings.WorkingPath) == false)
            {
                GitServerHost.Run("clone", baseUri.ToString().ToGitPath(), "-b", repositoryName, settings.WorkingPath.ToGitPath(), "--single-branch");
            }
            else
            {
                //GitHost.Run("update", workingPath.ToGitPath());
            }

            var repositoryInfo = this.GetRepositoryInfo(settings.BasePath, repositoryName);
            return new GitRepository(this, settings.LogService, settings.WorkingPath, settings.TransactionPath, repositoryInfo);
        }

        public void CreateRepository(string author, string basePath, string initPath, string comment, params LogPropertyInfo[] properties)
        {
            var baseUri = new Uri(basePath);
            var repositoryPath = baseUri.LocalPath;
            var repositoryName = Path.GetFileName(initPath);
            GitHost.Run(repositoryPath, "checkout --orphan", repositoryName);
            GitHost.Run(repositoryPath, "rm -rf .");
            GitHost.Run(repositoryPath, "commit --allow-empty -m \"root commit\"");

            DirectoryUtility.Copy(initPath, repositoryPath);

            foreach (var item in GetEmptyDirectories(repositoryPath))
            {
                File.WriteAllText(Path.Combine(item, KeepExtension), string.Empty);
            }

            var query = from item in DirectoryUtility.GetAllFiles(repositoryPath, "*", true)
                        select item.ToGitPath();

            var argList = new List<object>()
            {
                "add",
            };
            argList.AddRange(query);
            GitHost.Run(repositoryPath, argList.ToArray());
            GitHost.Run(repositoryPath, "commit -m \"first commit\"");
        }

        public void DeleteRepository(string author, string basePath, string repositoryName, string comment, params LogPropertyInfo[] properties)
        {
            var baseUri = new Uri(basePath);
            var repositoryPath = baseUri.LocalPath;
            var branchName = repositoryName;
            var branchCollection = GitBranchCollection.Run(repositoryPath);
            if (branchCollection.Count <= 1)
                throw new InvalidOperationException();

            if (branchCollection.CurrentBranch == branchName)
            {
                var nextBranchName = branchCollection.First(item => item != branchCollection.CurrentBranch);
                this.CheckoutBranch(repositoryPath, nextBranchName);
            }

            GitHost.Run(repositoryPath, "branch -D", branchName);
        }

        public LogInfo[] GetLog(string basePath, string repositoryName, int count)
        {
            var baseUri = new Uri(basePath);
            var repositoryPath = baseUri.LocalPath;
            var logs = GitLogInfo.RunOnBranch(repositoryPath, repositoryName, $"--max-count={count}");
            return logs.Select(item => (LogInfo)item).ToArray();
        }

        public string[] GetRepositories(string basePath)
        {
            var baseUri = new Uri(basePath);
            var repositoryPath = baseUri.LocalPath;
            var text = GitHost.Run(repositoryPath, "branch", "--list");
            var matches = Regex.Matches(text, "^[*]*\\s*(\\S+)", RegexOptions.Multiline);
            var itemList = new List<string>(matches.Count);
            for (var i = 0; i < matches.Count; i++)
            {
                var item = matches[i];
                var branchName = item.Groups[1].Value;
                itemList.Add(branchName);
            }

            return itemList.ToArray();
        }

        public RepositoryInfo GetRepositoryInfo(string basePath, string repositoryName)
        {
            var baseUri = new Uri(basePath);
            var repositoryInfo = new RepositoryInfo();
            var repositoryPath = baseUri.LocalPath;
            var text = GitHost.Run(repositoryPath, "log", $"{repositoryName}", "--pretty=fuller", "--max-count=1");
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
                var description = GitHost.Run(repositoryPath, "config", $"branch.{repositoryName}.description");
                var info = propertyDeserializer.Deserialize<GitBranchDescription>(description);
                repositoryInfo.ID = info.ID;
            }
            catch
            {
                this.SetID(repositoryPath, repositoryName, Guid.NewGuid());
            }

            return repositoryInfo;
        }

        public string[] GetRepositoryItemList(string basePath, string repositoryName)
        {
            var baseUri = new Uri(basePath);
            var repositoryPath = baseUri.LocalPath;
            var text = GitHost.Run(repositoryPath, "ls-tree", "-r", "--name-only", $"{repositoryName}");
            var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var itemList = new List<string>(lines.Length);

            foreach (var item in lines)
            {
                if (item.EndsWith(KeepExtension) == true)
                {
                    itemList.Add(PathUtility.Separator + item.Substring(0, item.Length - KeepExtension.Length));
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
            DirectoryUtility.Copy(repositoryPath, basePath);

            foreach (var item in GetEmptyDirectories(basePath))
            {
                File.WriteAllText(Path.Combine(item, KeepExtension), string.Empty);
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

            this.SetID(basePath, "master", Guid.NewGuid());
        }

        public void RenameRepository(string author, string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties)
        {
            var baseUri = new Uri(basePath);
            var repositoryPath = baseUri.LocalPath;
            GitHost.Run(repositoryPath, "branch -m", repositoryName, newRepositoryName);
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

        public string Name => "git";

        private void SetID(string repositoryPath, string repositoryName, Guid guid)
        {
            var info = new GitBranchDescription()
            {
                ID = guid,
            };
            var props = propertySerializer.Serialize(info);
            GitHost.Run(repositoryPath, "config", $"branch.{repositoryName}.description", props.ToGitPath());
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

        private void CheckoutBranch(string repositoryPath, string branchName)
        {
            GitHost.Run(repositoryPath, "reset --hard");
            GitHost.Run(repositoryPath, "checkout", branchName);
        }

        private ICremaHost CremaHost => this.cremaHost.Value;
    }
}
