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

        private readonly string basePath;
        private readonly Uri sourceUrl;
        private readonly string sourceRelativeUrl;
        private readonly Uri sourceRootUrl;
        private readonly IRepositoryUpgrader repositoryUpgrader;
        private readonly ILogService logService;

        private RepositoryUpgrader(IRepositoryUpgrader repositoryUpgrader, string basePath, Uri repositoryUrl)
        {
            this.repositoryUpgrader = repositoryUpgrader;
            this.basePath = basePath;
            if (repositoryUrl == null)
            {
                this.sourceUrl = new Uri(Path.Combine(this.basePath, nameString));
            }
            else if (repositoryUrl.IsAbsoluteUri)
            {
                this.sourceUrl = repositoryUrl;
            }
            else
            {
                var svnUri = new Uri(Path.Combine(this.basePath, nameString));
                this.sourceUrl = UriUtility.Combine(svnUri, repositoryUrl.ToString());
            }


            this.sourceRootUrl = new Uri(this.Run("info", $"{this.sourceUrl}", "--show-item repos-root-url").Trim());
            this.sourceRelativeUrl = UriUtility.MakeRelativeString(this.sourceRootUrl, this.sourceUrl);

            this.logService = new LogServiceHost(typeof(RepositoryUpgrader).FullName, CremaHost.GetPath(basePath, CremaPath.Logs));
        }

        public static void Upgrade(IRepositoryUpgrader repositoryUpgrader, string basePath, string repositoryUrl)
        {
            new RepositoryUpgrader(repositoryUpgrader, basePath, repositoryUrl == null ? null : new Uri(repositoryUrl, UriKind.RelativeOrAbsolute)).Upgrade();
        }

        private void Upgrade()
        {
            var repositoryProvider = this.repositoryUpgrader.RepositoryProvider;
            var svnPath = Path.Combine(this.basePath, nameString);
            var repositoryPath = DirectoryUtility.Prepare(this.basePath, CremaString.Repository);

            try
            {
                //this.UpgradeUsers(repositoryProvider, svnPath, repositoryPath);
                var dataBasesPath = this.UpgradeDataBases(svnPath, repositoryPath);
                var dataBaseUri = UriUtility.Combine(new Uri(dataBasesPath), this.sourceRelativeUrl);
                var destPath = this.repositoryUpgrader.Upgrade(dataBaseUri.ToString());
                if (destPath != null)
                {
                    DirectoryUtility.Backup(dataBasesPath);
                    DirectoryUtility.Move(destPath, dataBasesPath);
                    DirectoryUtility.Clean(dataBasesPath);
                }

                var repoModulePath = FileUtility.WriteAllText(repositoryProvider.Name, repositoryPath, CremaString.Repo);
                var fileTypePath = FileUtility.WriteAllText("xml", repositoryPath, CremaString.File);

                FileUtility.SetReadOnly(repoModulePath, true);
                FileUtility.SetReadOnly(fileTypePath, true);
                DirectoryUtility.SetVisible(repositoryPath, false);
            }
            catch (Exception e)
            {
                logService.Error(e);
                DirectoryUtility.SetVisible(repositoryPath, true);
                DirectoryUtility.Delete(repositoryPath);
                throw e;
            }
        }

        private void UpgradeUsers(IRepositoryProvider repositoryProvider, string svnPath, string repositoryPath)
        {
            this.logService.Info(nameof(UpgradeUsers));
            var usersPath = Path.Combine(repositoryPath, CremaString.Users);
            //var svnUri = new Uri(svnPath);
            var userUri = UriUtility.Combine(this.sourceUrl, $"{CremaString.Users}.xml");
            var userPath = Path.Combine(this.basePath, $"{CremaString.Users}.xml");

            this.Run("export", userUri, this.basePath.WrapQuot(), "--force");

            var userContext = Ntreev.Library.Serialization.DataContractSerializerUtility.Read<UserContextSerializationInfo>(userPath);
            var tempPath = PathUtility.GetTempPath(true);
            try
            {
                userContext.WriteToDirectory(tempPath);
                repositoryProvider.InitializeRepository(usersPath, tempPath);
            }
            finally
            {
                FileUtility.Delete(userPath);
                DirectoryUtility.Delete(tempPath);
            }
        }

        private string UpgradeDataBases(string svnPath, string repositoryPath)
        {
            this.logService.Info(nameof(UpgradeDataBases));
            var dataBasesPath = Path.Combine(repositoryPath, CremaString.DataBases);
            return dataBasesPath;
            //var dataBasesUri = new Uri(UriUtility.Combine(dataBasesPath, this.relativeUrl));

            DirectoryUtility.Copy(svnPath, dataBasesPath);
            this.PrepareBranches(dataBasesPath);
            this.MoveTagsToBranches(dataBasesPath);

            return dataBasesPath;
        }

        private void MoveTagsToBranches(string dataBasesPath)
        {
            var dataBaseUri = UriUtility.Combine(new Uri(dataBasesPath), this.sourceRelativeUrl);
            var tagsUri = UriUtility.Combine(dataBaseUri, tagsString);
            var branchesUri = UriUtility.Combine(dataBaseUri, branchesString);
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

        private void PrepareBranches(string dataBasesPath)
        {
            var dataBaseUri = UriUtility.Combine(new Uri(dataBasesPath), this.sourceRelativeUrl);
            var text = this.Run("list", dataBaseUri);
            var list = this.GetLines(text);
            if (list.Contains($"{branchesString}{PathUtility.Separator}") == false)
            {
                var branchesUri = UriUtility.Combine(dataBaseUri, branchesString);
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
