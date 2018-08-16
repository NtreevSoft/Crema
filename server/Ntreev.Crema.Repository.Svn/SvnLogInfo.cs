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
using Ntreev.Library.IO;

namespace Ntreev.Crema.Repository.Svn
{
    struct SvnLogInfo
    {
        private static readonly int defaultMaxCount = 50;

        private const string propertyPrefix = "prop:";

        public string Author { get; private set; }

        public string Revision { get; internal set; }

        public SvnChangeInfo[] ChangedPaths { get; internal set; }

        public string Comment { get; private set; }

        public DateTime DateTime { get; private set; }

        public SvnPropertyValue[] Properties { get; internal set; }

        public static SvnLogInfo[] Read(string text)
        {
            var logItemList = new List<SvnLogInfo>();
            using (var sr = new StringReader(text))
            {
                var doc = XDocument.Load(sr);
                var logItems = doc.XPathSelectElements("/log/logentry");

                foreach (var item in logItems)
                {
                    var logItem = SvnLogInfo.Parse(item);
                    logItemList.Add(logItem);
                }
            }
            return logItemList.ToArray();
        }

        public static SvnLogInfo GetLatestLog(string path)
        {
            var logCommand = new SvnCommand("log")
            {
                (SvnPath)path,
                SvnCommandItem.FromRevision($"head:1"),
                SvnCommandItem.Xml,
                SvnCommandItem.Verbose,
                SvnCommandItem.FromMaxCount(1),
                SvnCommandItem.WithAllRevprops
            };
            return SvnLogInfo.Read(logCommand.Run()).First();
        }

        public static SvnLogInfo[] GetLogs(string path, string revision)
        {
            if (revision == null)
            {
                var logCommand = new SvnCommand("log")
                {
                    (SvnPath)path,
                    SvnCommandItem.FromRevision($"head:1"),
                    SvnCommandItem.Xml,
                    SvnCommandItem.Verbose,
                    SvnCommandItem.FromMaxCount(MaxLogCount),
                    SvnCommandItem.WithAllRevprops
                };
                return SvnLogInfo.Read(logCommand.Run());
            }
            else
            {
                var logCommand = new SvnCommand("log")
                {
                    (SvnPath)path,
                    SvnCommandItem.FromRevision($"{revision}:1"),
                    SvnCommandItem.Xml,
                    SvnCommandItem.Verbose,
                    SvnCommandItem.FromMaxCount(MaxLogCount),
                    SvnCommandItem.WithAllRevprops
                };
                return SvnLogInfo.Read(logCommand.Run());
            }
        }

        public static SvnLogInfo[] GetLogs(string[] paths, string revision)
        {
            var logCommand = new SvnCommand("log")
            {
                SvnCommandItem.FromRevision($"{revision ?? "head"}:1"),
                SvnCommandItem.Xml,
                SvnCommandItem.Verbose,
                SvnCommandItem.FromMaxCount(MaxLogCount),
                SvnCommandItem.WithAllRevprops,
            };
            foreach (var item in paths)
            {
                logCommand.Add((SvnPath)item);
            }
            return SvnLogInfo.Read(logCommand.Run());
        }

        public static SvnLogInfo[] Run(string path, string minRevision, string maxRevision, int count)
        {
            var logCommand = new SvnCommand("log")
            {
                (SvnPath)path,
                SvnCommandItem.FromRevision($"{maxRevision}:{minRevision}"),
                SvnCommandItem.Xml,
                SvnCommandItem.Verbose,
                SvnCommandItem.FromMaxCount(count),
                SvnCommandItem.WithAllRevprops,
            };
            return SvnLogInfo.Read(logCommand.Run());
        }

        public static SvnLogInfo[] RunForGetBranch(Uri uri)
        {
            var logCommand = new SvnCommand("log")
            {
                (SvnPath)uri,
                SvnCommandItem.Xml,
                SvnCommandItem.Verbose,
                new SvnCommandItem("stop-on-copy")
            };
            return SvnLogInfo.Read(logCommand.Run());
        }

        public static SvnLogInfo GetFirstLog(string path)
        {
            var info = SvnInfo.Run(path);
            var revision = info.LastChangedRevision;
            var localPath = PathUtility.Separator + UriUtility.MakeRelativeOfDirectory(info.RepositoryRoot, info.Uri);

            while (revision != "1")
            {
                var logs = SvnLogInfo.Run(info.Uri.ToString(), "1", revision, 100);
                foreach (var item in logs)
                {
                    var renamed = false;
                    foreach (var changedPath in item.ChangedPaths)
                    {
                        if (changedPath.Action == "A" && changedPath.Path == localPath)
                        {
                            localPath = changedPath.CopyFromPath;
                            renamed = true;
                            break;
                        }
                    }

                    var deleted = false;
                    if (renamed == true)
                    {
                        foreach (var changedPath in item.ChangedPaths)
                        {
                            if (changedPath.Action == "D" && changedPath.Path == localPath)
                            {
                                deleted = true;
                                break;
                            }
                        }

                        if (deleted == false)
                        {
                            return item;
                        }
                    }
                }
                if (logs.Count() == 1)
                    return logs.First();
                else
                    revision = logs.Last().Revision;
            }

            return SvnLogInfo.GetLogs(info.Uri.ToString(), "1").First();
        }

        public static explicit operator LogInfo(SvnLogInfo value)
        {
            var userID = $"svn:{value.Author}";
            foreach (var item in value.Properties)
            {
                if (item.Key == LogPropertyInfo.UserIDKey)
                {
                    userID = item.Value;
                }
            }

            var obj = new LogInfo()
            {
                UserID = userID,
                Revision = value.Revision,
                Comment = value.Comment,
                DateTime = value.DateTime,
                Properties = value.Properties.Select(item => (LogPropertyInfo)item).ToArray(),
            };

            return obj;
        }

        public static int MaxCount { get; set; }

        internal string GetPropertyString(string key)
        {
            if (this.Properties == null)
                return null;

            var query = from item in this.Properties
                        where item.Key == key
                        select item;
            if (query.Any() == true)
                return query.First().Value;
            return null;
        }

        private static SvnLogInfo Parse(XElement element)
        {
            var commentValue = element.XPathSelectElement("msg").Value;
            var comment = null as string;
            var props = null as LogPropertyInfo[];

            var obj = new SvnLogInfo()
            {
                Author = element.XPathSelectElement("author").Value,
                Revision = element.Attribute("revision").Value,
                Comment = comment ?? commentValue,
                DateTime = XmlConvert.ToDateTime(element.XPathSelectElement("date").Value, XmlDateTimeSerializationMode.Utc)
            };
            var pathItems = element.XPathSelectElements("paths/path").ToArray();
            var changedItemList = new List<SvnChangeInfo>();
            foreach (var item in pathItems)
            {
                var changedItem = SvnChangeInfo.Parse(item);
                changedItemList.Add(changedItem);
            }
            obj.ChangedPaths = changedItemList.ToArray();

            if (props == null)
            {
                var propItems = element.XPathSelectElements("revprops/property").ToArray();
                var propItemList = new List<SvnPropertyValue>();
                foreach (var item in propItems)
                {
                    var propItem = SvnPropertyValue.Parse(item);
                    propItemList.Add(propItem);

                    if (propItem.Prefix == propertyPrefix && propItem.Key == LogPropertyInfo.UserIDKey)
                    {
                        obj.Author = propItem.Value;
                    }
                }
                obj.Properties = propItemList.ToArray();
            }
            else
            {
                var propList = new List<SvnPropertyValue>();
                foreach (var item in props)
                {
                    propList.Add((SvnPropertyValue)item);
                }
                obj.Properties = propList.ToArray();
            }

            return obj;
        }

        private static int MaxLogCount => MaxCount == 0 ? defaultMaxCount : MaxCount;
    }
}