using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    [Export(typeof(IRepositoryMigrator))]
    class GitRepositoryMigrator : IRepositoryMigrator
    {
        private readonly GitRepositoryProvider repositoryProvider;

        [ImportingConstructor]
        public GitRepositoryMigrator(GitRepositoryProvider repositoryProvider)
        {
            this.repositoryProvider = repositoryProvider;
        }

        public IRepositoryProvider RepositoryProvider => this.repositoryProvider;

        public string Name => this.repositoryProvider.Name;

        public string Migrate(string sourcePath)
        {
            var repositoryPath2 = PathUtility.GetTempPath(false);
            var repositoryUri = new Uri(sourcePath).ToString();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                repositoryUri = Regex.Replace(repositoryUri, "(file:///\\w):(.+)", "$1$2");
            }

            var cloneCommand = new GitCommand(null, "svn clone")
            {
                (GitPath)repositoryUri,
                (GitPath)repositoryPath2,
                new GitCommandItem('T', "trunk"),
                new GitCommandItem('b', "branches"),
                new GitCommandItem('b', "tags")
            };
            cloneCommand.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
            cloneCommand.Run();

            var remoteBranches = GitBranchCollection.GetRemoteBranches(repositoryPath2);
            var branches = GitBranchCollection.Run(repositoryPath2);

            foreach (var item in remoteBranches)
            {
                if (item != "trunk" && branches.Contains(item) == false)
                {
                    var checkoutCommand = new GitCommand(repositoryPath2, "checkout")
                    {
                        new GitCommandItem('b'),
                        item,
                        $"remotes/origin/{item}"
                    };
                    checkoutCommand.Run();
                }
            }

            var configCommand = new GitCommand(repositoryPath2, "config")
            {
                "receive.denyCurrentBranch",
                "ignore"
            };
            configCommand.Run();
            return repositoryPath2;
        }
    }
}
