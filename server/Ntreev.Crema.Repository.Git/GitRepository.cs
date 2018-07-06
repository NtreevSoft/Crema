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
using Ntreev.Library;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    class GitRepository : IRepository
    {
        private readonly string repositoryPath;
        private readonly string transactionPath;
        private readonly GitRepositoryProvider repositoryProvider;
        private readonly ILogService logService;
        private string transactionAuthor;
        private string transactionName;
        private string transactionMessages;
        private RepositoryInfo repositoryInfo;

        public GitRepository(GitRepositoryProvider repositoryProvider, ILogService logService, string repositoryPath, string transactionPath, RepositoryInfo repositoryInfo)
        {
            this.repositoryProvider = repositoryProvider;
            this.logService = logService;
            this.repositoryPath = repositoryPath;
            this.transactionPath = transactionPath;
            this.repositoryInfo = repositoryInfo;

            var statusCommand = new GitCommand(this.repositoryPath, "status")
            {
                new GitCommandItem('s'),
            };
            var items = statusCommand.ReadLines(true);
            if (items.Length != 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Repository is dirty. Please fix the problem before running the service.");
                sb.AppendLine();
                foreach (var item in items)
                {
                    sb.AppendLine(item);
                }
                throw new Exception($"{sb}");
            }
        }

        public RepositoryInfo RepositoryInfo => this.repositoryInfo;

        public string BasePath => this.repositoryPath;

        public void Add(string path)
        {
            var addCommand = new GitCommand(this.repositoryPath, "add");
            if (DirectoryUtility.IsDirectory(path) == true)
            {
                var keepPath = Path.Combine(path, GitRepositoryProvider.KeepExtension);
                if (File.Exists(keepPath) == false)
                {
                    File.WriteAllText(keepPath, string.Empty);
                    addCommand.Add((GitPath)keepPath);
                }
            }
            else
            {
                addCommand.Add((GitPath)path);
            }

            if (addCommand.Items.Any() == true)
                addCommand.Run();
        }

        public void BeginTransaction(string author, string name)
        {
            this.transactionAuthor = author;
            this.transactionName = name;
            this.transactionMessages = string.Empty;
        }

        public void EndTransaction()
        {
            var resetCommand = new GitCommand(this.repositoryPath, "reset")
            {
                new GitCommandItem("hard")
            };
            resetCommand.Run();
            var patchPath = Path.Combine(this.transactionPath, this.transactionName + ".patch");
            var gitCommand = new GitCommand(this.repositoryPath, "apply")
            {
                (GitPath)patchPath,
            };
            gitCommand.Run();
            var items = GitItemStatusInfo.Run(this.repositoryPath);
            foreach (var item in items)
            {
                if (item.Status == RepositoryItemStatus.Untracked)
                {
                    this.Add(item.Path);
                }
            }
            var commitCommand = new GitCommand(this.repositoryPath, "commit")
            {
                new GitCommandItem('a'),
                GitCommandItem.FromMessage(this.transactionMessages),
                GitCommandItem.FromAuthor(this.transactionAuthor),
            };
            var result = commitCommand.Run();
            this.logService?.Debug(result);
            var log = GitLogInfo.Run(this.repositoryPath, 1).First();
            this.repositoryInfo.Revision = log.CommitID;
            this.repositoryInfo.ModificationInfo = new SignatureDate(this.transactionAuthor, log.CommitDate);
            this.transactionAuthor = null;
            this.transactionName = null;
            this.transactionMessages = null;
            this.Pull();
            this.Push();
        }

        public void CancelTransaction()
        {
            var patchPath = Path.Combine(this.transactionPath, this.transactionName + ".patch");
            var resetCommand = new GitCommand(this.repositoryPath, "reset")
            {
                new GitCommandItem("hard")
            };
            this.transactionAuthor = null;
            this.transactionName = null;
            this.transactionMessages = null;
            resetCommand.Run(this.logService);
            FileUtility.Delete(patchPath);
        }

        public void Commit(string author, string comment, params LogPropertyInfo[] properties)
        {
            if (this.transactionName != null)
            {
                var patchPath = Path.Combine(this.transactionPath, this.transactionName + ".patch");
                var diffCommand = new GitCommand(this.repositoryPath, "diff")
                {
                    new GitCommandItem("cached"),
                };
                var text = diffCommand.Run(this.logService);
                FileUtility.WriteAllText(text, Encoding.UTF8, patchPath);
                this.transactionMessages = this.transactionMessages + comment + Environment.NewLine;
                return;
            }

            try
            {
                var statusCommand = new GitCommand(this.repositoryPath, "status")
                {
                    new GitCommandItem('s')
                };
                var items = statusCommand.ReadLines(true);
                if (items.Length != 0)
                {
                    var commitCommand = new GitCommand(this.repositoryPath, "commit")
                    {
                        new GitCommandItem('a'),
                        GitCommandItem.FromMessage(comment),
                        GitCommandItem.FromAuthor(author),
                    };
                    var result = commitCommand.Run();
                    this.logService?.Debug(result);
                    var log = GitLogInfo.Run(this.repositoryPath, 1).First();
                    this.repositoryInfo.Revision = log.CommitID;
                    this.repositoryInfo.ModificationInfo = new SignatureDate(author, log.CommitDate);
                    this.Pull();
                    this.Push();
                }
                else
                {
                    this.logService?.Debug("repository no changes. \"{0}\"", this.repositoryPath);
                }
            }
            catch (Exception e)
            {
                this.logService?.Warn(e);
                throw;
            }
        }

        public void Copy(string srcPath, string toPath)
        {
            if (DirectoryUtility.IsDirectory(srcPath) == true)
            {
                throw new NotImplementedException();
            }
            else
            {
                var copyCommand = new GitCommand(this.repositoryPath, "add")
                {
                    (GitPath)toPath
                };
                File.Copy(srcPath, toPath);
                copyCommand.Run();
            }
        }

        public void Delete(string path)
        {
            var removeCommand = new GitCommand(this.repositoryPath, "rm")
            {
                (GitPath)path,
            };
            if (Directory.Exists(path) == true)
            {
                removeCommand.Add(new GitCommandItem('r'));
            }
            removeCommand.Run();
        }

        public void Dispose()
        {
            DirectoryUtility.Delete(this.repositoryPath);
        }

        public string Export(Uri uri, string exportPath)
        {
            var match = Regex.Match(uri.LocalPath, "(?<path>.+)@(?<keep>.*)(?<revision>[a-f0-9]{40})", RegexOptions.ExplicitCapture);
            var path = match.Groups["path"].Value;
            var keep = match.Groups["keep"].Value;
            var revision = match.Groups["revision"].Value;

            var tempPath = PathUtility.GetTempFileName();
            try
            {
                if (DirectoryUtility.IsEmpty(exportPath) == true)
                    new CremaDataSet().WriteToDirectory(exportPath);
                var relativePath = UriUtility.MakeRelativeOfDirectory(this.repositoryPath, path);
                var archiveCommand = new GitCommand(this.repositoryPath, "archive")
                {
                    new GitCommandItem($"output={(GitPath)tempPath}"),
                    new GitCommandItem("format=zip"),
                    revision,
                    GitCommandItem.Separator,
                    (GitPath)path,
                };
                archiveCommand.Run();
                ZipFile.ExtractToDirectory(tempPath, exportPath);
                var exportUri = new Uri(UriUtility.Combine(exportPath, relativePath));
                return exportUri.LocalPath;
            }
            finally
            {
                FileUtility.Delete(tempPath);
            }
        }

        public LogInfo[] GetLog(string[] paths, string revision, int count)
        {
            var logs = GitLogInfo.RunWithPaths(this.repositoryPath, revision, paths, count);
            return logs.Select(item => (LogInfo)item).ToArray();
        }

        public Uri GetUri(string path, string revision)
        {
            if (DirectoryUtility.IsDirectory(path) == true)
            {
                var uri = new Uri($"{path}@{revision ?? this.repositoryInfo.Revision}");
                var uriString = uri.ToString();
                var text = Regex.Replace(uriString, "file:///", "dir:///");
                return new Uri(text);
            }
            return new Uri($"{path}@{revision ?? this.repositoryInfo.Revision}");
        }

        public void Move(string srcPath, string toPath)
        {
            var moveCommand = new GitCommand(this.repositoryPath, "mv")
            {
                (GitPath)srcPath,
                (GitPath)toPath,
            };
            moveCommand.Run();
        }

        public void Revert()
        {
            var resetCommand = new GitCommand(this.repositoryPath, "reset")
            {
                new GitCommandItem("hard")
            };
            resetCommand.Run();
        }

        public void Revert(string revision)
        {
            throw new NotImplementedException();
        }

        public RepositoryItem[] Status(params string[] paths)
        {
            var items = GitItemStatusInfo.Run(this.repositoryPath, paths);
            var itemList = new List<RepositoryItem>(items.Length);
            foreach (var item in items)
            {
                var repositoryItem = new RepositoryItem()
                {
                    Path = new Uri(UriUtility.Combine(this.repositoryPath, item.Path)).LocalPath,
                    OldPath = new Uri(UriUtility.Combine(this.repositoryPath, item.OldPath)).LocalPath,
                    Status = item.Status,
                };

                itemList.Add(repositoryItem);
            }
            return itemList.ToArray();
        }

        private void Pull()
        {
            var pullCommand = new GitCommand(this.repositoryPath, "pull");
            pullCommand.Run();
        }

        private void Push()
        {
            var pushCommand = new GitCommand(this.repositoryPath, "push");
            pushCommand.Run();
        }
    }
}
