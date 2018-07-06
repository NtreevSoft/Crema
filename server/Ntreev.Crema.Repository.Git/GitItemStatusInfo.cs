//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
        private const string nameStatusPattern = "^(?<X>[ |M|A|D|R|C|U|?|!])(?<Y>[ |M|A|D|R|C|U|?|!])\\s+(?<path1>\\S+)\\s*[-]*[>]*\\s*(?<path2>\\S*)$";

        public static GitItemStatusInfo[] Parse(string text)
        {
            var matches = Regex.Matches(text, nameStatusPattern, RegexOptions.Multiline);
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
            var statusCommand = new GitCommand(repositoryPath, "status")
            {
                new GitCommandItem('s'),
                new GitCommandItem(string.Empty)
            };
            foreach (var item in paths)
            {
                statusCommand.Add((GitPath)item);
            }
            return Parse(statusCommand.Run());
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
