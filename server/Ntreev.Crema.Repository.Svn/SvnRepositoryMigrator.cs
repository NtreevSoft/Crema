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

namespace Ntreev.Crema.Repository.Svn
{
    [Export(typeof(IRepositoryMigrator))]
    class SvnRepositoryMigrator : IRepositoryMigrator
    {
        private readonly SvnRepositoryProvider repositoryProvider;

        [ImportingConstructor]
        public SvnRepositoryMigrator(SvnRepositoryProvider repositoryProvider)
        {
            this.repositoryProvider = repositoryProvider;
        }

        public IRepositoryProvider RepositoryProvider => this.repositoryProvider;

        public string Name => this.repositoryProvider.Name;

        public string Migrate(string sourcePath)
        {
            this.PrepareBranches(sourcePath);
            this.MoveTagsToBranches(sourcePath);
            this.DeleteUsers(sourcePath);
            this.Pack(sourcePath);
            return null;
        }

        private void Pack(string sourcePath)
        {
            var info = SvnInfo.Run(sourcePath);
            var rootPath = info.RepositoryRoot.LocalPath;
            if (rootPath.EndsWith($"{Path.DirectorySeparatorChar}") == true)
                rootPath = Path.GetDirectoryName(rootPath);
            var packCommand = new SvnAdminCommand("pack") { (SvnPath)rootPath };
            packCommand.Run();
        }

        private void MoveTagsToBranches(string dataBasesPath)
        {
            var dataBaseUrl = new Uri(dataBasesPath);
            var tagsUrl = UriUtility.Combine(dataBaseUrl, SvnString.Tags);
            var branchesUri = UriUtility.Combine(dataBaseUrl, SvnString.Branches);
            var listCommand = new SvnCommand("list") { (SvnPath)tagsUrl };
            var list = listCommand.ReadLines();

            foreach (var item in list)
            {
                if (item.EndsWith(PathUtility.Separator) == true)
                {
                    var name = item.Remove(item.Length - PathUtility.Separator.Length);
                    var sourceUri = UriUtility.Combine(tagsUrl, name);
                    var destUri = UriUtility.Combine(branchesUri, name);
                    //var log = SvnLogInfo.Run(sourceUri.ToString(), null, 1).First();
                    var moveCommand = new SvnCommand("mv")
                    {
                        (SvnPath)sourceUri,
                        (SvnPath)destUri,
                        SvnCommandItem.FromMessage($"Migrate: move {name} from tags to branches"),
                        SvnCommandItem.FromUsername(nameof(SvnRepositoryMigrator)),
                    };
                    moveCommand.Run();
                    //var propText = string.Join(" ", log.Properties.Select(i => $"--with-revprop \"{i.Prefix}{i.Key}={i.Value}\""));
                    //SvnClientHost.Run($"mv \"{sourceUri}\" \"{destUri}\" -m \"Migrate: move {name} from tags to branches\"", propText, $"--username {nameof(SvnRepositoryMigrator)}");
                }
            }
        }

        private void PrepareBranches(string dataBasesPath)
        {
            var dataBaseUrl = new Uri(dataBasesPath);
            var listCommand = new SvnCommand("list") { (SvnPath)dataBaseUrl };
            var list = listCommand.ReadLines();
            if (list.Contains($"{SvnString.Branches}{PathUtility.Separator}") == false)
            {
                var branchesUrl = UriUtility.Combine(dataBaseUrl, SvnString.Branches);
                var mkdirCommand = new SvnCommand("mkdir")
                {
                    (SvnPath)branchesUrl,
                    SvnCommandItem.FromMessage("Migrate: create branches"),
                    SvnCommandItem.FromUsername(nameof(SvnRepositoryMigrator)),
                };
                mkdirCommand.Run();
            }
        }

        private void DeleteUsers(string dataBasesPath)
        {
            var usersUrl = UriUtility.Combine(new Uri(dataBasesPath), "users.xml");
            var deleteCommand = new SvnCommand("rm")
            {
                (SvnPath)usersUrl,
                SvnCommandItem.FromMessage("Migrate: delete users"),
                SvnCommandItem.FromUsername(nameof(SvnRepositoryMigrator)),
            };
            deleteCommand.Run();
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
    }
}
