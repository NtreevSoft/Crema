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

using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using Ntreev.Library.IO;

namespace Ntreev.Crema.Repository.Svn
{
    class SvnRepository : IRepository
    {
        public const string propertyPrefix = "prop:";
        private const string patchExtension = ".patch";

        private readonly string repositoryPath;
        private readonly string transactionPath;
        private readonly SvnRepositoryProvider repositoryProvider;
        private readonly ILogService logService;
        private readonly Dictionary<string, string> transactions = new Dictionary<string, string>();
        private readonly Dictionary<string, string> transactionMessages = new Dictionary<string, string>();
        private bool needToUpdate;
        private Uri repositoryRoot;
        private RepositoryInfo repositoryInfo;

        public SvnRepository(SvnRepositoryProvider repositoryProvider, ILogService logService, string repositoryPath, string transactionPath, RepositoryInfo repositoryInfo)
        {
            this.repositoryProvider = repositoryProvider;
            this.logService = logService;
            this.repositoryPath = repositoryPath;
            this.transactionPath = transactionPath;
            this.repositoryInfo = repositoryInfo;

            var items = this.Run("stat", this.repositoryPath.WrapQuot(), "-q").Trim();

            if (items != string.Empty)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Repository is dirty. Please fix the problem before running the service.");
                sb.AppendLine();
                sb.AppendLine(items);
                throw new Exception($"{sb}");
            }

            this.Run("log", this.repositoryPath.WrapQuot(), "-l 1");

            var info = SvnInfoEventArgs.Run(this.repositoryPath);
            this.repositoryRoot = info.RepositoryRoot;
            //var repositoryInfo = SvnInfoEventArgs.Run($"{info.Uri}");
            //this.revision = repositoryInfo.Revision;
        }

        public string Name
        {
            get { return "svn"; }
        }

        public RepositoryInfo RepositoryInfo => this.repositoryInfo;

        public void Add(string path)
        {
            if (path.EndsWith(Path.DirectorySeparatorChar.ToString()) == true)
                path = path.TrimEnd(Path.DirectorySeparatorChar);
            if (path.EndsWith(Path.AltDirectorySeparatorChar.ToString()) == true)
                path = path.TrimEnd(Path.AltDirectorySeparatorChar);
            this.Run("add", "--depth files", path.WrapQuot());
        }

        public void Add(string path, string contents)
        {
            File.WriteAllText(path, contents, Encoding.UTF8);
            this.Run("add", "--depth files", path.WrapQuot());
        }

        public void BeginTransaction(string path, string name)
        {
            this.logService?.Debug("repository begin transaction \"{0}\" \"{1}\"", path, name);
            this.transactions.Add(path, name);
            this.transactionMessages.Add(path, string.Empty);
        }

        public void EndTransaction(string path)
        {
            this.logService?.Debug("repository end transaction \"{0}\"", path);
            var patchPath = Path.Combine(this.transactionPath, this.transactions[path] + patchExtension);
            var message = this.transactionMessages[path];
            this.transactions.Remove(path);
            this.transactionMessages.Remove(path);
            if (File.Exists(patchPath) == true)
            {
                this.Run("patch", patchPath.WrapQuot(), path.WrapQuot());
                this.Commit(path, "Transaction" + Environment.NewLine + message, new LogPropertyInfo[] { });
                FileUtility.Delete(patchPath);
            }
        }

        public void CancelTransaction(string path)
        {
            this.logService?.Debug("repository cancel transaction \"{0}\"", path);
            var patchPath = Path.Combine(this.transactionPath, this.transactions[path] + patchExtension);
            this.transactions.Remove(path);
            this.transactionMessages.Remove(path);
            //DirectoryUtility.Delete(path);
            SvnClientHost.Run("revert", path.WrapQuot(), "-R");
            FileUtility.Delete(patchPath);
        }

        public void Commit(string path, string comment, params LogPropertyInfo[] properties)
        {
            var commentMessage = this.repositoryProvider.GenerateComment(comment, properties);
            if (this.transactions.ContainsKey(path) == true)
            {
                var patchPath = Path.Combine(this.transactionPath, this.transactions[path] + ".patch");
                var text = this.Run("diff", path.WrapQuot(), "--patch-compatible");
                FileUtility.WriteAllText(text, patchPath);
                this.transactionMessages[path] = this.transactionMessages[path] + comment + Environment.NewLine;
                //return DateTime.UtcNow;
            }

            //var propText = string.Join(" ", properties.Select(item => $"--with-revprop \"{propertyPrefix}{item.Key}={item.Value}\""));

            this.logService?.Debug($"repository committing {path.WrapQuot()}");
            var result = string.Empty;
            var commentPath = PathUtility.GetTempFileName();
            try
            {
                if (this.needToUpdate == true)
                    this.Run("update", path.WrapQuot());

                File.WriteAllText(commentPath, commentMessage);
                result = this.Run("commit", path.WrapQuot(), "--file", $"\"{commentPath}\"");
            }
            catch (Exception e)
            {
                this.logService?.Warn(e);
                this.Run("update", path.WrapQuot());
                result = this.Run("commit", path.WrapQuot(), "--file", $"\"{commentPath}\"");
            }
            finally
            {
                this.needToUpdate = false;
                FileUtility.Delete(commentPath);
            }

            if (result.Trim() != string.Empty)
            {
                this.logService?.Debug($"repository committed {path.WrapQuot()}");
                var match = Regex.Match(result, @"Committed revision (?<revision>\d+)[.]", RegexOptions.ExplicitCapture | RegexOptions.Multiline);
                var revision = match.Groups["revision"].Value;
                this.repositoryInfo.Revision = revision;
                var log = SvnLogEventArgs.Run(path, revision).First();
                var userID = properties.FirstOrDefault(item => item.Key == LogPropertyInfo.UserIDKey).Value;
                this.repositoryInfo.ModificationInfo = new SignatureDate(userID, log.DateTime);
            }
            else
            {
                this.logService?.Debug("repository no changes. \"{0}\"", path);
            }
        }

        public void Copy(string srcPath, string toPath)
        {
            this.Run("copy", srcPath.WrapQuot(), toPath.WrapQuot());
        }

        public void Delete(params string[] paths)
        {
            var items = new List<object>() { "delete", "--force" };
            items.AddRange(paths.Select(item => item.WrapQuot()));

            foreach (var item in paths)
            {
                if (DirectoryUtility.IsDirectory(item) == true)
                    this.Run("update", item.WrapQuot());
            }

            this.Run(items.ToArray());
        }

        public IDictionary<string, string> Status(string path)
        {
            var args = SvnStatusEventArgs.Run(path);
            return args.Status;
        }

        public string Export(Uri uri, string exportPath)
        {
            var pureUrl = new Uri(Regex.Replace($"{uri}", "@\\d+$", string.Empty));
            var relativeUri = this.repositoryRoot.MakeRelativeUri(pureUrl);
            var uriTarget = $"{uri}";
            var filename = FileUtility.Prepare(exportPath, $"{relativeUri}");
            this.Run("export", uriTarget, filename.WrapQuot());
            return new FileInfo(Path.Combine(exportPath, $"{relativeUri}")).FullName;
        }

        public void GetBranchInfo(string path, out string revision, out string source, out string sourceRevision)
        {
            var info = SvnInfoEventArgs.Run(path);
            this.GetBranchRevision(info.RepositoryRoot, info.Uri, out revision, out source, out sourceRevision);
        }

        public LogInfo[] GetLog(string path, string revision, int count)
        {
            var logs = SvnLogEventArgs.Run(path, revision, count);
            return logs.Select(item => (LogInfo)item).ToArray();
        }

        public string GetRevision(string path)
        {
            var info = SvnInfoEventArgs.Run(path);
            var repositoryInfo = SvnInfoEventArgs.Run($"{info.Uri}");
            return repositoryInfo.LastChangeRevision;
        }

        public Uri GetUri(string path, string revision)
        {
            var revisionValue = revision ?? this.repositoryInfo.Revision;
            var info = SvnInfoEventArgs.Run(path, revisionValue);
            return new Uri($"{info.Uri}@{revisionValue}");
        }

        public void Modify(string path, string contents)
        {
            File.WriteAllText(path, contents, Encoding.UTF8);
        }

        public void Move(string srcPath, string toPath)
        {
            if (DirectoryUtility.IsDirectory(srcPath) == true)
                this.Run("update", srcPath.WrapQuot());
            this.Run("move", srcPath.WrapQuot(), toPath.WrapQuot());
        }

        public void Revert(string path)
        {
            try
            {
                this.Run("revert", "-R", path.WrapQuot());
            }
            catch
            {
                this.Run("cleanup", path.WrapQuot());
                this.Run("revert", "-R", path.WrapQuot());
            }
        }

        public void Revert(string path, string revision)
        {
            this.Run("update", path.WrapQuot());
            this.Run("merge", "-r", $"head:{revision}", path.WrapQuot(), path.WrapQuot());
        }

        private string GetOriginPath(Uri repoUri, SvnLogEventArgs[] logs, string revision)
        {
            var repoPath = PathUtility.SeparatorChar + repoUri.OriginalString;

            foreach (var log in logs)
            {
                if (log.Revision == revision)
                    continue;

                foreach (var changedPath in log.ChangedPaths)
                {
                    if (changedPath.CopyFromPath == null)
                        continue;
                    if (repoPath.StartsWith(changedPath.Path) == true)
                    {
                        repoPath = Regex.Replace(repoPath, "^" + changedPath.Path, changedPath.CopyFromPath);
                        break;
                    }
                }
            }

            return repoPath;
        }

        private string Run(params object[] args)
        {
            try
            {
                return SvnClientHost.Run(args);
            }
            catch (Exception e)
            {
                this.logService.Error(e);
                throw e;
            }
        }

        private void GetBranchRevision(Uri repositoryRoot, Uri uri, out string revision, out string source, out string sourceRevision)
        {
            var log = SvnLogEventArgs.Run($"{uri}", "--xml -v --stop-on-copy").Last();
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

        public void Dispose()
        {
            DirectoryUtility.Delete(this.repositoryPath);
        }
    }
}
