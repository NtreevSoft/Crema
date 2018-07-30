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

        public void CopyRepository(string author, string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties)
        {
            var baseUri = new Uri(basePath);
            var repositoryPath = baseUri.LocalPath;
            var branchCommand = new GitCommand(repositoryPath, "branch")
            {
                newRepositoryName,
                repositoryName,
            };
            branchCommand.Run();
            this.SetID(repositoryPath, newRepositoryName, Guid.NewGuid());
            this.SetDescription(repositoryPath, newRepositoryName, comment);
            this.SetCreationInfo(repositoryPath, newRepositoryName, new SignatureDate(author));
        }

        public IRepository CreateInstance(RepositorySettings settings)
        {
            var baseUri = new Uri(settings.BasePath);
            var repositoryName = settings.RepositoryName == string.Empty ? "master" : settings.RepositoryName;

            if (Directory.Exists(settings.WorkingPath) == false)
            {
                var cloneCommand = new GitCommand(null, "clone")
                {
                    (GitPath)baseUri,
                    new GitCommandItem('b'),
                    repositoryName,
                    (GitPath)settings.WorkingPath,
                    new GitCommandItem("single-branch")
                };
                cloneCommand.Run();

                var fetchCommand = new GitCommand(settings.WorkingPath, "fetch")
                {
                    "origin",
                    "refs/notes/*:refs/notes/*",
                };
                fetchCommand.Run();
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
            var checkoutCommand = new GitCommand(repositoryPath, "checkout")
            {
                new GitCommandItem("orphan"), repositoryName
            };
            checkoutCommand.Run();
            var removeCommand = new GitCommand(repositoryPath, "rm")
            {
                "-rf", "."
            };
            removeCommand.Run();

            DirectoryUtility.Copy(initPath, repositoryPath);
            foreach (var item in GetEmptyDirectories(repositoryPath))
            {
                File.WriteAllText(Path.Combine(item, KeepExtension), string.Empty);
            }

            var statusItems = GitItemStatusInfo.Run(repositoryPath);
            var addCommand = new GitCommand(repositoryPath, "add");
            foreach (var item in statusItems)
            {
                if (item.Status == RepositoryItemStatus.Untracked)
                {
                    addCommand.Add((GitPath)item.Path);
                }
            }
            addCommand.Run();

            var commitCommand = new GitCommitCommand(repositoryPath, author, comment);
            commitCommand.Run();
            this.SetID(repositoryPath, repositoryName, Guid.NewGuid());
            this.SetDescription(repositoryPath, repositoryName, comment);
        }

        public void RenameRepository(string author, string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties)
        {
            var baseUri = new Uri(basePath);
            var repositoryPath = baseUri.LocalPath;
            var renameCommand = new GitCommand(repositoryPath, "branch")
            {
                new GitCommandItem('m'),
                repositoryName,
                newRepositoryName
            };

            var repositoryID = this.GetID(repositoryPath, repositoryName);
            renameCommand.Run();
            this.SetID(repositoryPath, newRepositoryName, repositoryID);
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

            var deleteCommand = new GitCommand(repositoryPath, "branch")
            {
                new GitCommandItem('D'),
                branchName
            };
            deleteCommand.Run();
            this.UnsetID(repositoryPath, repositoryName);
        }

        public void RevertRepository(string author, string basePath, string repositoryName, string revision, string comment)
        {
            var baseUri = new Uri(basePath);
            var repositoryPath = baseUri.LocalPath;
            
            this.CheckoutBranch(repositoryPath, repositoryName);

            try
            {
                var revisionsCommand = new GitCommand(repositoryPath, "log")
                {
                    GitCommandItem.FromPretty("format:%H"),
                };
                var revisions = revisionsCommand.ReadLines();
                foreach (var item in revisions)
                {
                    if (item == revision)
                        break;
                    var revertCommand = new GitCommand(repositoryPath, "revert")
                    {
                        new GitCommandItem('n'),
                        item,
                    };
                    revertCommand.Run();
                }
                var statusCommand = new GitCommand(repositoryPath, "status")
                {
                    new GitCommandItem('s')
                };
                var items = statusCommand.ReadLines(true);
                if (items.Length != 0)
                {
                    var commitCommand = new GitCommitCommand(basePath, author, comment);
                    commitCommand.Run();
                }
                else
                {
                    throw new InvalidOperationException("nothing to revert.");
                }
            }
            catch
            {
                var abortCommand = new GitCommand(repositoryPath, "revert")
                {
                    new GitCommandItem("abort"),
                };
                abortCommand.Run();
                throw;
            }
        }

        public LogInfo[] GetLog(string basePath, string repositoryName)
        {
            var baseUri = new Uri(basePath);
            var repositoryPath = baseUri.LocalPath;
            var logs = GitLogInfo.GetBranchLogs(repositoryPath, repositoryName);
            return logs.Select(item => (LogInfo)item).ToArray();
        }

        public string[] GetRepositories(string basePath)
        {
            var baseUri = new Uri(basePath);
            var repositoryPath = baseUri.LocalPath;
            var branchCommand = new GitCommand(repositoryPath, "branch")
            {
                new GitCommandItem("list")
            };
            var lines = branchCommand.ReadLines();
            var itemList = new List<string>(lines.Length);
            foreach (var line in lines)
            {
                var match = Regex.Match(line, "^[*]*\\s*(\\S+)");
                if (match.Success)
                {
                    var branchName = match.Groups[1].Value;
                    itemList.Add(branchName);
                }
            }

            return itemList.ToArray();
        }

        public RepositoryInfo GetRepositoryInfo(string basePath, string repositoryName)
        {
            var baseUri = new Uri(basePath);
            var repositoryInfo = new RepositoryInfo();
            var repositoryPath = baseUri.LocalPath;
            var latestLog = GitLogInfo.GetBranchLog(repositoryPath, $"{repositoryName}");
            var refLogItems = GitLogInfo.GetReflogs(basePath, repositoryName);

            var firstLog = refLogItems.Last();

            repositoryInfo.Name = repositoryName;
            repositoryInfo.Revision = latestLog.CommitID;
            repositoryInfo.CreationInfo = new SignatureDate(firstLog.Author, firstLog.AuthorDate);
            repositoryInfo.ModificationInfo = new SignatureDate(latestLog.Author, latestLog.AuthorDate);

            if (this.HasID(repositoryPath, repositoryName) == false)
            {
                this.SetID(repositoryPath, repositoryName, Guid.NewGuid());
            }
            repositoryInfo.ID = this.GetID(repositoryPath, repositoryName);

            if (this.HasDescription(repositoryPath, repositoryName) == true)
            {
                repositoryInfo.Comment = this.GetDescription(repositoryPath, repositoryName);
            }
            else
            {
                repositoryInfo.Comment = string.Empty;
            }

            if (this.HasCreationInfo(repositoryPath, repositoryName) == true)
            {
                repositoryInfo.CreationInfo = this.GetCreationInfo(repositoryPath, repositoryName);
            }

            return repositoryInfo;
        }

        public string[] GetRepositoryItemList(string basePath, string repositoryName)
        {
            var baseUri = new Uri(basePath);
            var repositoryPath = baseUri.LocalPath;
            var listCommand = new GitCommand(repositoryPath, "ls-tree")
            {
                new GitCommandItem('r'),
                new GitCommandItem("name-only"),
                repositoryName
            };
            var lines = listCommand.ReadLines(true);
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
            var revparseCommand = new GitCommand(basePath, "rev-parse")
            {
                repositoryName,
            };
            return revparseCommand.ReadLine();
        }

        public void InitializeRepository(string basePath, string repositoryPath)
        {
            var initCommand = new GitCommand(null, "init")
            {
                (GitPath)basePath
            };
            initCommand.Run();

            var configCommand = new GitCommand(basePath, "config")
            {
                "receive.denyCurrentBranch",
                "ignore"
            };
            configCommand.Run();

            DirectoryUtility.Copy(repositoryPath, basePath);
            foreach (var item in GetEmptyDirectories(basePath))
            {
                File.WriteAllText(Path.Combine(item, KeepExtension), string.Empty);
            }

            var query = from item in DirectoryUtility.GetAllFiles(basePath, "*", true)
                        select item;

            var addCommand = new GitCommand(basePath, "add");
            foreach (var item in DirectoryUtility.GetAllFiles(basePath, "*", true))
            {
                addCommand.Add((GitPath)item);
            }
            addCommand.Run();

            var commitCommand = new GitCommitCommand(basePath, Environment.UserName, "first commit");
            commitCommand.Run();

            this.SetID(basePath, "master", Guid.NewGuid());
        }

        public string Name => "git";

        private void SetID(string repositoryPath, string repositoryName, Guid guid)
        {
            GitConfig.SetValue(repositoryPath, $"branch.{repositoryName}.id", $"{guid}");
        }

        private void UnsetID(string repositoryPath, string repositoryName)
        {
            GitConfig.UnsetValue(repositoryPath, $"branch.{repositoryName}.id");
        }

        private bool HasID(string repositoryPath, string repositoryName)
        {
            return GitConfig.HasValue(repositoryPath, $"branch.{repositoryName}.id");
        }

        private Guid GetID(string repositoryPath, string repositoryName)
        {
            return Guid.Parse(GitConfig.GetValue(repositoryPath, $"branch.{repositoryName}.id"));
        }

        private void SetCreationInfo(string repositoryPath, string repositoryName, SignatureDate signatureDate)
        {
            GitConfig.SetValue(repositoryPath, $"branch.{repositoryName}.createdDateTime", $"{signatureDate}");
        }

        private void UnsetCreationInfo(string repositoryPath, string repositoryName)
        {
            GitConfig.UnsetValue(repositoryPath, $"branch.{repositoryName}.createdDateTime");
        }

        private bool HasCreationInfo(string repositoryPath, string repositoryName)
        {
            return GitConfig.HasValue(repositoryPath, $"branch.{repositoryName}.createdDateTime");
        }

        private SignatureDate GetCreationInfo(string repositoryPath, string repositoryName)
        {
            return SignatureDate.Parse(GitConfig.GetValue(repositoryPath, $"branch.{repositoryName}.createdDateTime"));
        }

        private void SetDescription(string repositoryPath, string repositoryName, string description)
        {
            GitConfig.SetValue(repositoryPath, $"branch.{repositoryName}.description", $"{description}");
        }

        private void UnsetDescription(string repositoryPath, string repositoryName)
        {
            GitConfig.UnsetValue(repositoryPath, $"branch.{repositoryName}.description");
        }

        private bool HasDescription(string repositoryPath, string repositoryName)
        {
            return GitConfig.HasValue(repositoryPath, $"branch.{repositoryName}.description");
        }

        private string GetDescription(string repositoryPath, string repositoryName)
        {
            return GitConfig.GetValue(repositoryPath, $"branch.{repositoryName}.description");
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
            var resetCommand = new GitCommand(repositoryPath, "reset")
            {
                new GitCommandItem("hard")
            };
            resetCommand.Run();
            var checkoutCommand = new GitCommand(repositoryPath, "checkout")
            {
                branchName
            };
            checkoutCommand.Run();
        }
    }
}
