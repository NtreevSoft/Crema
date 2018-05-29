using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
