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
        private RepositoryInfo repositoryInfo;

        public GitRepository(GitRepositoryProvider repositoryProvider, ILogService logService, string repositoryPath, string transactionPath, RepositoryInfo repositoryInfo)
        {
            this.repositoryProvider = repositoryProvider;
            this.logService = logService;
            this.repositoryPath = repositoryPath;
            this.transactionPath = transactionPath;
            this.repositoryInfo = repositoryInfo;

            var items = GitHost.Run(this.repositoryPath, "status", "-s").Trim();

            if (items != string.Empty)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Repository is dirty. Please fix the problem before running the service.");
                sb.AppendLine();
                sb.AppendLine(items);
                throw new Exception($"{sb}");
            }
        }

        public RepositoryInfo RepositoryInfo => this.repositoryInfo;

        public void Add(string path)
        {
            if (DirectoryUtility.IsDirectory(path) == true)
            {
                var keepPath = Path.Combine(path, GitRepositoryProvider.keepExtension);
                if (File.Exists(keepPath) == false)
                {
                    File.WriteAllText(keepPath, string.Empty);
                    GitHost.Run(this.repositoryPath, "add", keepPath.WrapQuot());
                }
            }
            else
            {
                GitHost.Run(this.repositoryPath, "add", path.WrapQuot());
            }
        }

        public void BeginTransaction(string name)
        {
            throw new NotImplementedException();
        }

        public void CancelTransaction()
        {
            throw new NotImplementedException();
        }

        public void Commit(string comment, params LogPropertyInfo[] properties)
        {
            var commentMessage = this.repositoryProvider.GenerateComment(comment, properties);
            //if (this.transactions.ContainsKey(path) == true)
            //{
            //    var patchPath = Path.Combine(this.transactionPath, this.transactions[path] + ".patch");
            //    var text = this.Run("diff", path.WrapQuot(), "--patch-compatible");
            //    FileUtility.WriteAllText(text, patchPath);
            //    this.transactionMessages[path] = this.transactionMessages[path] + comment + Environment.NewLine;
            //    //return DateTime.UtcNow;
            //}

            //var propText = string.Join(" ", properties.Select(item => $"--with-revprop \"{propertyPrefix}{item.Key}={item.Value}\""));

            this.logService?.Debug($"repository committing {this.repositoryPath.WrapQuot()}");
            var result = string.Empty;
            var commentPath = PathUtility.GetTempFileName();
            try
            {
                File.WriteAllText(commentPath, commentMessage);
                result = GitHost.Run(this.repositoryPath, "commit", "-a", "--file", $"\"{commentPath}\"");
                GitHost.Run(this.repositoryPath, "pull");
                GitHost.Run(this.repositoryPath, "push");
            }
            catch (Exception e)
            {
                this.logService?.Warn(e);
                //this.Run("update", this.repositoryPath.WrapQuot());
                //result = this.Run("commit", this.repositoryPath.WrapQuot(), "--file", $"\"{commentPath}\"");
            }
            finally
            {
                //this.needToUpdate = false;
                FileUtility.Delete(commentPath);
            }

            if (result.Trim() != string.Empty)
            {
                this.logService?.Debug($"repository committed {this.repositoryPath.WrapQuot()}");
                this.logService?.Debug(result);
                //var match = Regex.Match(result, @"Committed revision (?<revision>\d+)[.]", RegexOptions.ExplicitCapture | RegexOptions.Multiline);
                //var revision = match.Groups["revision"].Value;
                //this.repositoryInfo.Revision = revision;
                //var log = SvnLogEventArgs.Run(path, revision).First();
                var userID = properties.FirstOrDefault(item => item.Key == LogPropertyInfo.UserIDKey).Value;
                //this.repositoryInfo.ModificationInfo = new SignatureDate(userID, log.DateTime);
                var log = GitLogInfo.Run(this.repositoryPath, "--max-count=1").First();
                this.repositoryInfo.Revision = log.CommitID;
                this.repositoryInfo.ModificationInfo = new SignatureDate(userID, log.CommitDate);
            }
            else
            {
                this.logService?.Debug("repository no changes. \"{0}\"", this.repositoryPath);
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
                File.Copy(srcPath, toPath);
                GitHost.Run(this.repositoryPath, "add", toPath.WrapQuot());
            }
        }

        public void Delete(string path)
        {
            if (Directory.Exists(path) == true)
            {
                GitHost.Run(this.repositoryPath, "rm", path.WrapQuot(), "-r");
            }
            else
            {
                GitHost.Run(this.repositoryPath, "rm", path.WrapQuot());
            }
        }

        public void Dispose()
        {
            DirectoryUtility.Delete(this.repositoryPath);
        }

        public void EndTransaction()
        {
            throw new NotImplementedException();
        }

        public string Export(Uri uri, string exportPath)
        {
            var name = Path.GetFileName(uri.LocalPath);
            var match = Regex.Match(name, "(?<filename>.+)@(?<revision>[a-f0-9]{40})", RegexOptions.ExplicitCapture);
            var filename = match.Groups["filename"].Value;
            var revision = match.Groups["revision"].Value;

            if (filename == ".keep")
            {
                var tempPath = PathUtility.GetTempFileName();
                GitHost.Run(this.repositoryPath, "archive", $"--output=\"{tempPath}\"", "--format=zip", revision, "--");
                ZipFile.ExtractToDirectory(tempPath, exportPath);
                return exportPath;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public LogInfo[] GetLog(string[] paths, string revision, int count)
        {
            var logs = GitLogInfo.RunWithPaths(this.repositoryPath, revision, paths, $"--max-count={count}");
            return logs.Select(item => (LogInfo)item).ToArray();
        }

        //public string GetRevision(string path)
        //{
        //    throw new NotImplementedException();
        //}

        public Uri GetUri(string path, string revision)
        {
            // git log --follow --pretty=oneline --name-status .\tables\Table1.xml
            if (DirectoryUtility.IsDirectory(path) == true)
                return new Uri($"{path}{PathUtility.Separator}.keep@{revision ?? this.repositoryInfo.Revision}");
            return new Uri($"{path}@{revision ?? this.repositoryInfo.Revision}");
        }

        public void Move(string srcPath, string toPath)
        {
            GitHost.Run(this.repositoryPath, "mv", srcPath.WrapQuot(), toPath.WrapQuot());
        }

        public void Revert()
        {
            throw new NotImplementedException();
        }

        public void Revert(string revision)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> Status()
        {
            throw new NotImplementedException();
        }
    }
}
