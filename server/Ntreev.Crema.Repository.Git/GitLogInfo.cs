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

using Ntreev.Library.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Ntreev.Library;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Ntreev.Crema.ServiceModel;
using YamlDotNet.Serialization;

namespace Ntreev.Crema.Repository.Git
{
    struct GitLogInfo
    {
        private const string notesPattern = "^Notes:(?<notes>.*)$";
        private const string commitIDPattern = "commit\\s*(?<commitID>[a-f0-9]{40})";
        private const string mergePattern = "Merge:\\s*(?<mergeFrom>[a-f0-9]{7})\\s(?<mergeTo>[a-f0-9]{7})";
        private const string reflogPattern = "Reflog:\\s*(?<reflog>.+)\\s[(](?<author>.+)\\s<(?<authorEMail>.*)[)]";
        private const string reflogMessagePattern = "Reflog message:(?<message>.*)";
        private const string authorPattern = "Author:\\s*(?<author>.+)\\s<(?<authorEMail>.*)>";
        private const string authorDatePattern = "AuthorDate:\\s*(?<authorDate>.+)";
        private const string commitPattern = "Commit:\\s*(?<commit>.+)\\s<(?<commitEMail>.*)>";
        private const string commitDatePattern = "CommitDate:\\s*(?<commitDate>.+)";

        private const string dateTimeFormat = "ddd MMM d HH:mm:ss yyyy K";

        private static readonly int defaultMaxCount = 50;

        private static readonly Deserializer propertyDeserializer = new Deserializer();

        public override string ToString()
        {
            return this.Comment ?? base.ToString();
        }

        public string CommitID { get; set; }

        public string MergeTo { get; set; }

        public string MergeFrom { get; set; }

        public string Reflog { get; set; }

        public string ReflogMessage { get; set; }

        public string Author { get; set; }

        public DateTime AuthorDate { get; set; }

        public string Commit { get; set; }

        public DateTime CommitDate { get; set; }

        public string Comment { get; set; }

        public GitPropertyValue[] Properties { get; internal set; }

        public static GitLogInfo GetLatestLog(string repositoryPath)
        {
            var logCommand = new GitCommand(repositoryPath, "show")
            {
                GitCommandItem.FromPretty("fuller"),
                GitCommandItem.ShowNotes,
                GitCommandItem.NoPatch,
            };
            return ParseMany(logCommand.Run()).First();
        }

        public static GitLogInfo[] GetLogs(string repositoryPath, string revision, string[] paths)
        {
            var logCommand = new GitCommand(repositoryPath, "log")
            {
                revision ?? "head",
                GitCommandItem.FromPretty("fuller"),
                GitCommandItem.ShowNotes,
                new GitCommandItem(string.Empty)
            };
            foreach (var item in paths)
            {
                logCommand.Add((GitPath)item);
            }
            return ParseMany(logCommand.Run());
        }

        public static GitLogInfo[] GetRepositoryLogs(string repositoryPath, string repositoryName, string revision)
        {
            if (revision == null)
            {
                var logCommand = new GitCommand(repositoryPath, "log")
                {
                    $"{repositoryName}",
                    GitCommandItem.FromPretty("fuller"),
                    GitCommandItem.ShowNotes,
                    GitCommandItem.FromMaxCount(MaxLogCount),
                };
                return ParseMany(logCommand.Run());
            }
            else
            {
                var revisions = GetRepositoryRevisions(repositoryPath, repositoryName);
                var index = revisions.IndexOf(revision);
                if (index < 0)
                    throw new ArgumentException($"not found revision: {revision}");

                var logCommand = new GitCommand(repositoryPath, "log")
                {
                    $"{revision}",
                    GitCommandItem.FromPretty("fuller"),
                    GitCommandItem.ShowNotes,
                    GitCommandItem.FromMaxCount(MaxLogCount),
                };
                return ParseMany(logCommand.Run());
            }
        }

        public static GitLogInfo GetRepositoryLatestLog(string repositoryPath, string repositoryName)
        {
            var logCommand = new GitCommand(repositoryPath, "show")
            {
                $"{repositoryName}",
                GitCommandItem.FromPretty("fuller"),
                GitCommandItem.ShowNotes,
                GitCommandItem.NoPatch,
            };
            return ParseMany(logCommand.Run()).First();
        }

        public static string[] GetRepositoryRevisions(string repositoryPath, string repositoryName)
        {
            var logCommand = new GitCommand(repositoryPath, "log")
            {
                $"{repositoryName}",
                GitCommandItem.FromPretty("format:%H")
            };
            return logCommand.ReadLines();
        }

        public static GitLogInfo Parse(string text)
        {
            var logInfo = new GitLogInfo();
            ParseNotes(ref text, ref logInfo);
            ParseCommitID(ref text, ref logInfo);
            ParseMerge(ref text, ref logInfo);
            ParseReflog(ref text, ref logInfo);
            ParseReflogMessage(ref text, ref logInfo);
            ParseAuthor(ref text, ref logInfo);
            ParseAuthorDate(ref text, ref logInfo);
            ParseCommit(ref text, ref logInfo);
            ParseCommitDate(ref text, ref logInfo);
            ParseComment(ref text, ref logInfo);
            return logInfo;
        }

        public static GitLogInfo[] ParseMany(string text)
        {
            var matches = Regex.Matches(text, "^" + commitIDPattern, RegexOptions.Multiline);
            var itemList = new List<GitLogInfo>();
            var index = 0;
            var currentText = text;
            foreach (Match item in matches)
            {
                if (item.Index != index)
                {
                    var logItem = text.Substring(index, item.Index - index);
                    var logInfo = GitLogInfo.Parse(logItem);
                    itemList.Add(logInfo);
                    currentText = currentText.Substring(logItem.Length);
                }
                index = item.Index;
            }
            {
                var logInfo = GitLogInfo.Parse(currentText);
                itemList.Add(logInfo);
            }

            return itemList.ToArray();
        }

        public static GitLogInfo[] GetReflogs(string repositoryPath, string branchName)
        {
            var logCommand = new GitCommand(repositoryPath, "log")
            {
                new GitCommandItem("pretty=fuller"),
                new GitCommandItem('g'),
                GitCommandItem.ShowNotes,
                branchName,
            };
            return ParseMany(logCommand.Run());
        }

        public static explicit operator LogInfo(GitLogInfo value)
        {
            var obj = new LogInfo()
            {
                UserID = value.Author,
                Revision = value.CommitID,
                Comment = value.Comment,
                DateTime = value.CommitDate,
                Properties = value.Properties.Select(item => (LogPropertyInfo)item).ToArray(),
            };

            return obj;
        }

        public static int MaxCount { get; set; }

        private static void ParseNotes(ref string text, ref GitLogInfo logInfo)
        {
            var match = Regex.Match(text, notesPattern, RegexOptions.ExplicitCapture | RegexOptions.Multiline | RegexOptions.Singleline);
            if (match.Success == true)
            {
                var notes = match.Groups["notes"].Value;

                try
                {
                    logInfo.Properties = propertyDeserializer.Deserialize<GitPropertyValue[]>(notes);
                    text = text.Remove(text.Length - (match.Value.Length + Environment.NewLine.Length));
                    return;
                }
                catch
                {

                }
            }
            logInfo.Properties = new GitPropertyValue[] { };
        }

        private static void ParseCommitID(ref string text, ref GitLogInfo logInfo)
        {
            if (Match(ref text, commitIDPattern) is Match match)
            {
                logInfo.CommitID = match.Groups["commitID"].Value;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void ParseMerge(ref string text, ref GitLogInfo logInfo)
        {
            if (Match(ref text, mergePattern) is Match match)
            {
                logInfo.MergeFrom = match.Groups["mergeFrom"].Value;
                logInfo.MergeTo = match.Groups["mergeTo"].Value;
            }
            else
            {
                logInfo.MergeFrom = string.Empty;
                logInfo.MergeTo = string.Empty;
            }
        }

        private static void ParseReflog(ref string text, ref GitLogInfo logInfo)
        {
            if (Match(ref text, reflogPattern) is Match match)
            {
                logInfo.Reflog = match.Groups["reflog"].Value;
            }
            else
            {
                logInfo.Reflog = string.Empty;
            }
        }

        private static void ParseReflogMessage(ref string text, ref GitLogInfo logInfo)
        {
            if (Match(ref text, reflogMessagePattern) is Match match)
            {
                logInfo.ReflogMessage = match.Groups["message"].Value.TrimStart();
            }
            else
            {
                logInfo.ReflogMessage = string.Empty;
            }
        }

        private static string ParseAuthor(ref string text, ref GitLogInfo logInfo)
        {
            if (Match(ref text, authorPattern) is Match match)
            {
                logInfo.Author = match.Groups["author"].Value;
            }
            else
            {
                throw new NotImplementedException();
            }

            return text;
        }

        private static string ParseAuthorDate(ref string text, ref GitLogInfo logInfo)
        {
            if (Match(ref text, authorDatePattern) is Match match)
            {
                var value = match.Groups["authorDate"].Value;
                logInfo.AuthorDate = ParseDateTime(value);
            }
            else
            {
                throw new NotImplementedException();
            }

            return text;
        }

        private static string ParseCommit(ref string text, ref GitLogInfo logInfo)
        {
            if (Match(ref text, commitPattern) is Match match)
            {
                logInfo.Commit = match.Groups["commit"].Value;
            }
            else
            {
                throw new NotImplementedException();
            }

            return text;
        }

        private static string ParseCommitDate(ref string text, ref GitLogInfo logInfo)
        {
            if (Match(ref text, commitDatePattern) is Match match)
            {
                var value = match.Groups["commitDate"].Value;
                logInfo.CommitDate = ParseDateTime(value);
            }
            else
            {
                throw new NotImplementedException();
            }

            return text;
        }

        private static string ParseComment(ref string text, ref GitLogInfo logInfo)
        {
            text = text.Remove(0, Environment.NewLine.Length);
            text = text.Remove(0, "    ".Length);
            logInfo.Comment = text;
            return text;
        }

        private static Match Match(ref string input, string pattern)
        {
            var match = Regex.Match(input, pattern + Environment.NewLine, RegexOptions.ExplicitCapture | RegexOptions.Multiline);
            if (match.Success == true)
            {
                input = input.Substring(match.Length);
                return match;
            }
            return null;
        }

        private static DateTime ParseDateTime(string text)
        {
            return DateTime.ParseExact(text, dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }

        private static int MaxLogCount => MaxCount == 0 ? defaultMaxCount : MaxCount;
    }
}