using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
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
        }

        public RepositoryInfo RepositoryInfo => throw new NotImplementedException();

        public void Add(string path)
        {
            throw new NotImplementedException();
        }

        public void Add(string path, string contents)
        {
            throw new NotImplementedException();
        }

        public void BeginTransaction(string path, string name)
        {
            throw new NotImplementedException();
        }

        public void CancelTransaction(string path)
        {
            throw new NotImplementedException();
        }

        public void Commit(string path, string message, params LogPropertyInfo[] properties)
        {
            throw new NotImplementedException();
        }

        public void Copy(string srcPath, string toPath)
        {
            throw new NotImplementedException();
        }

        public void Delete(params string[] paths)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void EndTransaction(string path)
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
            // git.exe archive --output="C:\Users\s2quake\Desktop\새 폴더 (6)\3f3b71ec45d869e282e6d3d000a30cc3a2b8bc4f.zip" --format=zip --verbose 3f3b71ec45d869e282e6d3d000a30cc3a2b8bc4f --


        }

        public LogInfo[] GetLog(string path, string revision, int count)
        {
            throw new NotImplementedException();
        }

        public string GetRevision(string path)
        {
            throw new NotImplementedException();
        }

        public Uri GetUri(string path, string revision)
        {
            if (DirectoryUtility.IsDirectory(path) == true)
                return new Uri($"{path}{PathUtility.Separator}.keep@{revision ?? this.repositoryInfo.Revision}");
            return new Uri($"{path}@{revision ?? this.repositoryInfo.Revision}");
        }

        public void Modify(string path, string contents)
        {
            throw new NotImplementedException();
        }

        public void Move(string srcPath, string toPath)
        {
            throw new NotImplementedException();
        }

        public void Revert(string path)
        {
            throw new NotImplementedException();
        }

        public void Revert(string path, string revision)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> Status(string path)
        {
            throw new NotImplementedException();
        }
    }
}
