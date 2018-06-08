using Ntreev.Crema.Services;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    struct GitItemStatusInfo
    {
        private const string nameStatusPattern = "(?<X>[ |M|A|D|R|C|U|?|!])(?<Y>[ |M|A|D|R|C|U|?|!])\\s+(?<path1>\\S+)\\s*[-]*[>]*\\s*(?<path2>\\S*)";
        private static readonly string newLinePattern = Environment.NewLine.Replace("\n", "\\n").Replace("\r", "\\r");

        public static GitItemStatusInfo[] Parse(string text)
        {
            var matches = Regex.Matches(text, nameStatusPattern + newLinePattern);
            var itemList = new List<GitItemStatusInfo>(matches.Count);
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var x = match.Groups["X"].Value;
                var y = match.Groups["Y"].Value;
                var path1 = match.Groups["path1"].Value;
                var path2 = match.Groups["path2"].Value;

                var item = new GitItemStatusInfo()
                {
                    X = x,
                    Y = y,
                    Path = path2 == string.Empty ? path1 : path2,
                    OldPath = path2 == string.Empty ? path1 : path1,
                };
                itemList.Add(item);
            }

            return itemList.ToArray();
        }

        public static GitItemStatusInfo[] Run(string repositoryPath, params string[] paths)
        {
            var argList = new List<object>()
            {
                "status", "-s", "--"
            };

            argList.AddRange(paths.ToArray());

            var text = GitHost.Run(repositoryPath, argList.ToArray());
            return Parse(text);
        }

        public string X { get; set; }

        public string Y { get; set; }

        public string Path { get; set; }

        public string OldPath { get; set; }

        public RepositoryItemStatus Status
        {
            get
            {
                if (this.X == "R")
                    return RepositoryItemStatus.Renamed;
                else if (this.X == "?")
                    return RepositoryItemStatus.Untracked;
                else if (this.X == "A")
                    return RepositoryItemStatus.Added;
                else if (this.X == "D")
                    return RepositoryItemStatus.Deleted;
                else if (this.Y == "M")
                    return RepositoryItemStatus.Modified;
                return RepositoryItemStatus.None;
            }
        }
    }
}
