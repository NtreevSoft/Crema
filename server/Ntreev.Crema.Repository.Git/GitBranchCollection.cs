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
            var text = GitHost.Run(repositoryPath, "branch", "--list");
            var matches = Regex.Matches(text, "^(?<current>[*])*\\s*(?<branch>\\S+)", RegexOptions.Multiline);
            var itemList = new List<string>(matches.Count);
            var currentBranch = string.Empty;
            for (var i = 0; i < matches.Count; i++)
            {
                var item = matches[i];
                var isCurrent = item.Groups["current"].Value == "*";
                var branchName = item.Groups["branch"].Value;

                if (isCurrent == true)
                    currentBranch = branchName;
                itemList.Add(branchName);
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
