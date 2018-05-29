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
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    [Export(typeof(IRepositoryProvider))]
    class GitRepositoryProvider : IRepositoryProvider
    {
        private const string keepExtension = ".keep";

        [ImportMany]
        private IEnumerable<Lazy<ILogService>> logServices = null;

        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        public string Name => "git";



        public void CopyRepository(string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties)
        {
            throw new NotImplementedException();
        }

        public IRepository CreateInstance(string basePath, string repositoryName, string workingPath)
        {
            var baseUri = new Uri(basePath);
            var branchName = repositoryName == "default" ? "master" : repositoryName;
            var logService = this.logServices.FirstOrDefault(item => item.Value.Name == "repository");

            if (Directory.Exists(workingPath) == false)
            {
                GitServerHost.Run("clone", baseUri.ToString().WrapQuot(), "-b", branchName);
            }
            else
            {
                //GitHost.Run("update", workingPath.WrapQuot());
            }
            var transactionPath = Path.Combine(this.CremaHost.GetPath(CremaPath.Transactions), Path.GetFileName(workingPath));
            var repositoryInfo = this.GetRepositoryInfo(basePath, repositoryName);
            return new GitRepository(this, logService.Value, workingPath, transactionPath, repositoryInfo);
        }

        public void CreateRepository(string basePath, string initPath, string comment, params LogPropertyInfo[] properties)
        {
            throw new NotImplementedException();
        }

        public void DeleteRepository(string basePath, string repositoryName, string comment, params LogPropertyInfo[] properties)
        {
            throw new NotImplementedException();
        }

        public LogInfo[] GetLog(string basePath, string repositoryName, string revision, int count)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetRepositories(string basePath)
        {
            throw new NotImplementedException();
        }

        public RepositoryInfo GetRepositoryInfo(string basePath, string repositoryName)
        {
            var branchName = repositoryName == "default" ? "master" : repositoryName;
            throw new NotImplementedException();
        }

        public string[] GetRepositoryItemList(string basePath, string repositoryName)
        {
            throw new NotImplementedException();
        }

        public string GetRevision(string basePath, string repositoryName)
        {
            throw new NotImplementedException();
        }

        public void InitializeRepository(string basePath, string repositoryPath)
        {
            GitServerHost.Run("init", basePath.WrapQuot());
            GitHost.Run(basePath, "commit --allow-empty -m \"root commit\"");
            GitHost.Run(basePath, "branch __empty__");
            DirectoryUtility.Copy(repositoryPath, basePath);

            foreach (var item in GetEmptyDirectories(basePath))
            {
                File.WriteAllText(Path.Combine(item, keepExtension), string.Empty);
            }

            var query = from item in DirectoryUtility.GetAllFiles(basePath, "*", true)
                        select item.WrapQuot();

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

        private ICremaHost CremaHost => this.cremaHost.Value;
    }
}
