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

namespace Ntreev.Crema.SvnModule
{
    struct SvnLogEventArgs
    {
        public string Author { get; private set; }

        public string Revision { get; internal set; }

        public SvnChangeItem[] ChangedPaths { get; internal set; }

        public string Comment { get; private set; }

        public DateTime DateTime { get; private set; }

        public SvnPropertyValue[] Properties { get; internal set; }

        public static SvnLogEventArgs[] Read(string text)
        {
            var logItemList = new List<SvnLogEventArgs>();
            using (var sr = new StringReader(text))
            {
                var doc = XDocument.Load(sr);
                var logItems = doc.XPathSelectElements("/log/logentry").ToArray();

                foreach (var item in logItems)
                {
                    var logItem = SvnLogEventArgs.Parse(item);
                    logItemList.Add(logItem);
                }
            }
            return logItemList.ToArray();
        }

        public static SvnLogEventArgs[] Run(string path, string revision)
        {
            var text = SvnClientHost.Run("log", path.WrapQuot(), "-r", $"head:{revision}", "--xml", "-v");
            return SvnLogEventArgs.Read(text);
        }

        public static SvnLogEventArgs[] Run(string path, string revision, int count)
        {
            var text = SvnClientHost.Run("log", path.WrapQuot(), "-r", $"{revision??"head"}:1", "--xml", "-v", "-l", count, "--with-all-revprops");
            return SvnLogEventArgs.Read(text);
        }

        public static SvnLogEventArgs[] Runa(string path, string arguments)
        {
            if (arguments.IndexOf("--xml") < 0)
                arguments += " --xml";
            var text = SvnClientHost.Run("log", path.WrapQuot(), arguments);
            return SvnLogEventArgs.Read(text);
        }

        public static explicit operator LogInfo(SvnLogEventArgs value)
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

        private static SvnLogEventArgs Parse(XElement element)
        {
            var commentValue = element.XPathSelectElement("msg").Value;
            var comment = null as string;
            var props = null as LogPropertyInfo[];
            SvnRepositoryProvider.ParseComment(commentValue, out comment, out props);

            var obj = new SvnLogEventArgs()
            {
                Author = element.XPathSelectElement("author").Value,
                Revision = element.Attribute("revision").Value,
                Comment = comment ?? commentValue,
                DateTime = XmlConvert.ToDateTime(element.XPathSelectElement("date").Value, XmlDateTimeSerializationMode.Utc)
            };
            var pathItems = element.XPathSelectElements("paths/path").ToArray();
            var changedItemList = new List<SvnChangeItem>();
            foreach (var item in pathItems)
            {
                var changedItem = SvnChangeItem.Parse(item);
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
    }
}