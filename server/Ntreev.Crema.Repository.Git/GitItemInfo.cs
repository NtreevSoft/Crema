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

using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Git
{
    struct GitItemInfo
    {
        private const string commitIDPattern = "(?<commitID>[a-f0-9]{40})";
        private const string nameStatusPattern = "(?<status>[A|C|D|M|R|T|U|X|B])(?<percent>\\d*)\\s+(?<name1>\\S+)\\s*(?<name2>\\S*)";
        private static readonly string newLinePattern = Environment.NewLine.Replace("\n", "\\n").Replace("\r", "\\r");

        public static GitItemInfo[] Parse(string text)
        {
            var pattern = commitIDPattern + newLinePattern + nameStatusPattern + newLinePattern + newLinePattern;
            var matches = Regex.Matches(text, pattern);
            var itemList = new List<GitItemInfo>(matches.Count);
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var commitID = match.Groups["commitID"].Value;
                var status = match.Groups["status"].Value;
                var percent = match.Groups["percent"].Value;
                var name1 = match.Groups["name1"].Value;
                var name2 = match.Groups["name2"].Value;

                itemList.Add(new GitItemInfo()
                {
                    CommitID = commitID,
                    Status = status,
                    Percent = percent == string.Empty ? 0 : long.Parse(percent),
                    Path1 = name1,
                    Path2 = name2,
                });
            }

            return itemList.ToArray();
        }

        //public static GitItemInfo[] Run(string repositoryPath, string filename)
        //{
        //    var logCommand = new GitCommand()
        //    var text = GitHost.Run(repositoryPath, "git log --follow --pretty=format:%H --name-status", filename.ToGitPath());
        //    return Parse(text);
        //}

        public string Status { get; set; }

        public long Percent { get; set; }

        public string CommitID { get; set; }

        public string Path1 { get; set; }

        public string Path2 { get; set; }
    }
}
