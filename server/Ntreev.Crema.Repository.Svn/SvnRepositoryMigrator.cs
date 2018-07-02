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
            return null;
        }

        private void MoveTagsToBranches(string dataBasesPath)
        {

            var dataBaseUrl = new Uri(dataBasesPath);
            var tagsUrl = UriUtility.Combine(dataBaseUrl, SvnString.Tags);
            var branchesUri = UriUtility.Combine(dataBaseUrl, SvnString.Branches);
            var text = SvnClientHost.Run($"list \"{tagsUrl}\"");
            var list = this.GetLines(text);

            foreach (var item in list)
            {
                if (item.EndsWith(PathUtility.Separator) == true)
                {
                    var name = item.Remove(item.Length - PathUtility.Separator.Length);
                    var sourceUri = UriUtility.Combine(tagsUrl, name);
                    var destUri = UriUtility.Combine(branchesUri, name);

                    var log = SvnLogEventArgs.Run(sourceUri.ToString(), null, 1).First();
                    var propText = string.Join(" ", log.Properties.Select(i => $"--with-revprop \"{i.Prefix}{i.Key}={i.Value}\""));

                    SvnClientHost.Run($"mv \"{sourceUri}\" \"{destUri}\" -m \"Migrate: move {name} from tags to branches\"", propText);
                }
            }
        }

        private void PrepareBranches(string dataBasesPath)
        {
            var dataBaseUrl = new Uri(dataBasesPath);
            var text = SvnClientHost.Run($"list \"{dataBaseUrl}\"");
            var list = this.GetLines(text);
            if (list.Contains($"{SvnString.Branches}{PathUtility.Separator}") == false)
            {
                var branchesUrl = UriUtility.Combine(dataBaseUrl, SvnString.Branches);
                SvnClientHost.Run($"mkdir \"{branchesUrl}\" -m \"Migrate: create branches\"");
            }
        }

        private void DeleteUsers(string dataBasesPath)
        {
            var usersUrl = UriUtility.Combine(new Uri(dataBasesPath), "users.xml");
            SvnClientHost.Run($"rm \"{usersUrl}\" -m \"Migrate: delete users\"");
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
