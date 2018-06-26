using Ntreev.Crema.Services.Users.Serializations;
using Ntreev.Library;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    class RepositoryUpgrader
    {
        private const string nameString = "svn";
        private const string trunkString = "trunk";
        private const string tagsString = "tags";
        private const string branchesString = "branches";
        private const string defaultString = "default";

        //private readonly IServiceProvider serviceProvider;
        private readonly string basePath;
        private readonly IRepositoryUpgrader repositoryUpgrader;

        private RepositoryUpgrader(IRepositoryUpgrader repositoryUpgrader, string basePath)
        {
            this.repositoryUpgrader = repositoryUpgrader;
            this.basePath = basePath;
        }

        public static void Upgrade(IRepositoryUpgrader repositoryUpgrader, string basePath)
        {
            var obj = new RepositoryUpgrader(repositoryUpgrader, basePath);
            obj.Upgrade();
        }

        private void Upgrade()
        {
            var repositoryProvider = this.repositoryUpgrader.RepositoryProvider;
            var svnPath = Path.Combine(this.basePath, nameString);
            var repositoryPath = DirectoryUtility.Prepare(this.basePath, CremaString.Repository);

            try
            {
                var repositoryPathInfo = new DirectoryInfo(repositoryPath)
                {
                    Attributes = FileAttributes.Directory | FileAttributes.Hidden
                };

                var usersPath = this.UpgradeUsers(repositoryProvider, svnPath, repositoryPath);
                var databasesPath = this.UpgradeDataBases(svnPath, repositoryPath);

                var destPath = this.repositoryUpgrader.Upgrade(databasesPath);
                if (destPath != null)
                {
                    var tempPath = databasesPath + "_bak";
                    DirectoryUtility.Move(databasesPath, tempPath);
                    DirectoryUtility.Move(destPath, databasesPath);
                }
                //if (this.IsGit == true)
                //{
                //    this.ConvertToGit(usersPath);
                //    this.ConvertToGit(databasesPath);
                //}

                var repoModulePath = FileUtility.WriteAllText(repositoryProvider.Name, repositoryPath, CremaString.Repo);
                new FileInfo(repoModulePath).Attributes |= FileAttributes.ReadOnly;
                var fileTypePath = FileUtility.WriteAllText("xml", repositoryPath, CremaString.File);
                new FileInfo(fileTypePath).Attributes |= FileAttributes.ReadOnly;
            }
            catch (Exception e)
            {
                DirectoryUtility.Delete(repositoryPath);
            }
        }

        private string UpgradeUsers(IRepositoryProvider repositoryProvider, string svnPath, string repositoryPath)
        {
            var usersPath = Path.Combine(repositoryPath, CremaString.Users);
            //return usersPath;
            var svnUri = new Uri(svnPath);
            var userUri = UriUtility.Combine(svnUri, $"{CremaString.Users}.xml");
            var userPath = Path.Combine(this.basePath, $"{CremaString.Users}.xml");

            this.Run("export", userUri, this.basePath.WrapQuot(), "--force");

            var userContext = Ntreev.Library.Serialization.DataContractSerializerUtility.Read<UserContextSerializationInfo>(userPath);
            var tempPath = PathUtility.GetTempPath(true);
            try
            {
                userContext.WriteToDirectory(tempPath);
                repositoryProvider.InitializeRepository(usersPath, tempPath);
                return usersPath;
            }
            finally
            {
                FileUtility.Delete(userPath);
                DirectoryUtility.Delete(tempPath);
            }
        }

        private string UpgradeDataBases(string svnPath, string repositoryPath)
        {
            var dataBasesPath = Path.Combine(repositoryPath, CremaString.DataBases);
            //return dataBasesPath;
            var dataBasesUri = new Uri(dataBasesPath);

            DirectoryUtility.Copy(svnPath, dataBasesPath);
            this.PrepareBranches(dataBasesUri);
            this.MoveTagsToBranches(dataBasesUri);

            return dataBasesPath;
        }

        private void MoveTagsToBranches(Uri dataBasesUri)
        {
            var tagsUri = UriUtility.Combine(dataBasesUri, tagsString);
            var branchesUri = UriUtility.Combine(dataBasesUri, branchesString);
            var text = this.Run("list", tagsUri);
            var list = this.GetLines(text);

            foreach (var item in list)
            {
                if (item.EndsWith(PathUtility.Separator) == true)
                {
                    var name = item.Remove(item.Length - PathUtility.Separator.Length);

                    var sourceUri = UriUtility.Combine(tagsUri, name);
                    var destUri = UriUtility.Combine(branchesUri, name);

                    this.Run("mv", $"\"{sourceUri}\"", $"\"{destUri}\"", "-m", $"\"Upgrade: move {name} from tags to branches\"");
                }
            }
        }

        private void PrepareBranches(Uri dataBasesUri)
        {
            var text = this.Run("list", dataBasesUri);
            var list = this.GetLines(text);
            if (list.Contains($"{branchesString}{PathUtility.Separator}") == false)
            {
                var branchesUri = UriUtility.Combine(dataBasesUri, branchesString);
                this.Run("mkdir", branchesUri, "-m", "\"Upgrade: create branches\"");
            }
        }

        private string[] GetLines(string text)
        {
            var sr = new StringReader(text);
            var line = null as string;
            var lineList = new List<string>();
            while ((line = sr.ReadLine()) != null)
            {
                lineList.Add(line);
            }
            return lineList.ToArray();
        }

        private string Run(params object[] args)
        {
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "svn";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = string.Join(" ", args);
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.OutputDataReceived += (s, e) =>
            {
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
