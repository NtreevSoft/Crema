using Ntreev.Crema.Services;
using Ntreev.Library;
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
    [Export(typeof(IRepositoryUpgrader))]
    class GitRepositoryUpgrader : IRepositoryUpgrader
    {
        private readonly GitRepositoryProvider repositoryProvider;

        [ImportingConstructor]
        public GitRepositoryUpgrader(GitRepositoryProvider repositoryProvider)
        {
            this.repositoryProvider = repositoryProvider;
        }

        public IRepositoryProvider RepositoryProvider => this.repositoryProvider;

        public string Name => this.repositoryProvider.Name;

        public string Upgrade(string sourcePath)
        {
            var repositoryPath2 = sourcePath + "_Temp";

            


            var repositoryUri = new Uri(sourcePath).ToString();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                repositoryUri = Regex.Replace(repositoryUri, "(file:///\\w):(.+)", "$1$2");
            }

            this.Run("svn clone", repositoryUri, repositoryPath2.WrapQuot(), "--stdlayout");

            var branches = GitBranchCollection.GetRemoteBranches(repositoryPath2);
            var b = GitBranchCollection.Run(repositoryPath2);

            foreach (var item in branches)
            {
                if (item != "trunk" && b.Contains(item) == false)
                {
                    GitHost.Run(repositoryPath2, "checkout", "-b", item, $"remotes/origin/{item}");
                }
            }


            return repositoryPath2;
            return repositoryPath2;
        }

        private string Run(params object[] args)
        {
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "git";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = string.Join(" ", args);
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.OutputDataReceived += (s, e) =>
            {
                Console.WriteLine(e.Data);
                outputBuilder.AppendLine(e.Data);
            };
            process.ErrorDataReceived += (s, e) =>
            {
                errorBuilder.AppendLine(e.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception(errorBuilder.ToString());

            return outputBuilder.ToString();
        }
    }
}
