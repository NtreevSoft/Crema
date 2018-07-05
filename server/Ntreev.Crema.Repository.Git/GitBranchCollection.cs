using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    class GitBranchCollection : IReadOnlyList<string>
    {
        private readonly List<string> branchList;

        private GitBranchCollection(IEnumerable<string> branchList, string currentBranch)
        {
            this.branchList = new List<string>(branchList);
            this.CurrentBranch = currentBranch;
        }

        public static GitBranchCollection Run(string repositoryPath)
        {
            var listCommand = new GitCommand(repositoryPath, "branch")
            {
                new GitCommandItem("list")
            };
            var lines = listCommand.ReadLines(true);
            var itemList = new List<string>(lines.Length);
            var currentBranch = string.Empty;
            foreach (var line in lines)
            {
                var match = Regex.Match(line, "^(?<current>[*])*\\s*(?<branch>\\S+)");
                if (match.Success == true)
                {
                    var isCurrent = match.Groups["current"].Value == "*";
                    var branchName = match.Groups["branch"].Value;

                    if (isCurrent == true)
                        currentBranch = branchName;
                    itemList.Add(branchName);
                }
            }

            return new GitBranchCollection(itemList, currentBranch);
        }

        public static GitBranchCollection GetRemoteBranches(string repositoryPath)
        {
            var listCommand = new GitCommand(repositoryPath, "branch")
            {
                new GitCommandItem('a')
            };
            var lines = listCommand.ReadLines(true);
            var itemList = new List<string>(lines.Length);
            var currentBranch = string.Empty;
            foreach (var line in lines)
            {
                var match = Regex.Match(line, "remotes/origin/(?<branch>[^/]+)$");
                if (match.Success == true)
                {
                    var branchName = match.Groups["branch"].Value.Trim();
                    itemList.Add(branchName);
                }
            }

            return new GitBranchCollection(itemList, currentBranch);
        }

        public string this[int index] => this.branchList[index];

        public string CurrentBranch { get; }

        public int Count => this.branchList.Count;

        #region IEnumerable

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            foreach (var item in this.branchList)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in this.branchList)
            {
                yield return item;
            }
        }

        #endregion
    }
}
