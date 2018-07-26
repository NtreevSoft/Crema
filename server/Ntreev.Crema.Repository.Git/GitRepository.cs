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
using YamlDotNet.Serialization;

namespace Ntreev.Crema.Repository.Git
{
    class GitRepository : IRepository
    {
        private static readonly Serializer propertySerializer = new SerializerBuilder().Build();
        private static readonly Deserializer propertyDeserializer = new Deserializer();

        private readonly string repositoryPath;
        private readonly string transactionPath;
        private readonly GitRepositoryProvider repositoryProvider;
        private readonly ILogService logService;
        private readonly GitCommand resetCommand;
        private readonly GitCommand cleanCommand;
        private string transactionAuthor;
        private string transactionName;
        private List<string> transactionMessageList;
        private List<LogPropertyInfo> transactionPropertyList;
        private string transactionPatchPath;
        private RepositoryInfo repositoryInfo;

        public GitRepository(GitRepositoryProvider repositoryProvider, ILogService logService, string repositoryPath, string transactionPath, RepositoryInfo repositoryInfo)
        {
            this.repositoryProvider = repositoryProvider;
            this.logService = logService;
            this.repositoryPath = repositoryPath;
            this.transactionPath = transactionPath;
            this.repositoryInfo = repositoryInfo;
            this.resetCommand = new GitCommand(this.repositoryPath, "reset")
            {
                new GitCommandItem("hard")
            };
            this.cleanCommand = new GitCommand(this.repositoryPath, "clean")
            {
                new GitCommandItem('f'),
                new GitCommandItem('d')
            };

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
                addCommand.Run(this.logService);
        }

        public void BeginTransaction(string author, string name)
        {
            this.transactionAuthor = author;
            this.transactionName = name;
            this.transactionMessageList = new List<string>();
            this.transactionPropertyList = new List<LogPropertyInfo>();
            this.transactionPatchPath = Path.Combine(this.transactionPath, this.transactionName + ".patch");
        }

        public void EndTransaction()
        {
            var transactionMessage = string.Join(Environment.NewLine, this.transactionMessageList);
            var messagePath = FileUtility.WriteAllText(transactionMessage, Encoding.UTF8, PathUtility.GetTempFileName());
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
                        GitCommandItem.FromFile(messagePath),
                        GitCommandItem.FromAuthor(this.transactionAuthor),
                    };
                    var result = commitCommand.Run(this.logService);
                    this.logService?.Debug(result);
                    var log = GitLogInfo.Run(this.repositoryPath, 1).First();
                    this.repositoryInfo.Revision = log.CommitID;
                    this.repositoryInfo.ModificationInfo = new SignatureDate(this.transactionAuthor, log.CommitDate);
                    this.SetNotes(this.transactionPropertyList.ToArray());
                    FileUtility.Delete(this.transactionPatchPath);
                    this.transactionAuthor = null;
                    this.transactionName = null;
                    this.transactionMessageList = null;
                    this.transactionPropertyList = null;
                    this.transactionPatchPath = null;
                    this.Pull();
                    this.Push();
                    this.PushNotes();
                }
                else
                {
                    this.logService?.Debug("repository has no changes.");
                }
            }
            finally
            {
                FileUtility.Delete(messagePath);
            }
        }

        public void CancelTransaction()
        {
            this.resetCommand.Run(this.logService);
            this.cleanCommand.Run(this.logService);
            FileUtility.Delete(this.transactionPatchPath);
            this.transactionAuthor = null;
            this.transactionName = null;
            this.transactionMessageList = null;
            this.transactionPropertyList = null;
            this.transactionPatchPath = null;
        }

        public void Commit(string author, string comment, params LogPropertyInfo[] properties)
        {
            if (this.transactionName != null)
            {
                var diffCommand = new GitCommand(this.repositoryPath, "diff")
                {
                    "HEAD",
                    new GitCommandItem("stat"),
                    new GitCommandItem("binary")
                };
                diffCommand.WriteAllText(this.transactionPatchPath);
                this.transactionMessageList.Add(comment);
                this.transactionPropertyList.AddRange(properties);
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
                    var result = commitCommand.Run(this.logService);
                    this.logService?.Debug(result);
                    var log = GitLogInfo.Run(this.repositoryPath, 1).First();
                    this.repositoryInfo.Revision = log.CommitID;
                    this.repositoryInfo.ModificationInfo = new SignatureDate(author, log.CommitDate);

                    this.SetNotes(properties);
                    this.Pull();
                    this.Push();
                    this.PushNotes();
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
                copyCommand.Run(this.logService);
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
            removeCommand.Run(this.logService);
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
                if (Directory.Exists(exportPath) == false)
                    Directory.CreateDirectory(exportPath);
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
                archiveCommand.Run(this.logService);
                ZipFile.ExtractToDirectory(tempPath, exportPath);
                var exportUri = new Uri(UriUtility.Combine(exportPath, relativePath));
                return exportUri.LocalPath;
            }
            finally
            {
                FileUtility.Delete(tempPath);
            }
        }

        public LogInfo[] GetLog(string[] paths, string revision)
        {
            var logs = GitLogInfo.RunWithPaths(this.repositoryPath, revision, paths);
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
            moveCommand.Run(this.logService);
        }

        public void Revert()
        {
            this.resetCommand.Run(this.logService);
            this.cleanCommand.Run(this.logService);

            if (File.Exists(this.transactionPatchPath) == true)
            {
                var applyCommand = new GitCommand(this.repositoryPath, "apply")
                {
                    (GitPath)transactionPatchPath,
                    new GitCommandItem("index")
                };
                applyCommand.Run(this.logService);
            }
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
            pullCommand.Run(this.logService);
        }

        private void Push()
        {
            var pushCommand = new GitCommand(this.repositoryPath, "push");
            pushCommand.Run(this.logService);
        }

        private void SetNotes(params LogPropertyInfo[] properties)
        {
            var props = properties.Select(item => (GitPropertyValue)item).ToArray();
            var propText = propertySerializer.Serialize(props);
            var notesCommand = new GitCommand(this.repositoryPath, "notes")
            {
                "add",
                GitCommandItem.FromMessage(propText),
            };
            notesCommand.Run(this.logService);
        }

        private void PushNotes()
        {
            var pushNotesCommand = new GitCommand(this.repositoryPath, "push")
            {
                "origin",
                "refs/notes/commits",
            };
            pushNotesCommand.Run(this.logService);
        }
    }
}
