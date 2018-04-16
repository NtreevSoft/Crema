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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Ntreev.Crema.SvnModule
{
    struct SvnInfoEventArgs
    {
        public Uri RepositoryRoot { get; private set; }

        public Uri Uri { get; private set; }

        public string Path { get; private set; }

        public long Revision { get; private set; }

        public long LastChangeRevision { get; private set; }

        public static SvnInfoEventArgs Run(string path, long revision)
        {
            var text = SvnClientHost.Run("info", path.WrapQuot(), "-r", revision, "--xml");
            return Parse(text);
        }

        public static SvnInfoEventArgs Run(string path)
        {
            var text = SvnClientHost.Run("info", path.WrapQuot(), "--xml");
            return Parse(text);
        }

        public static SvnInfoEventArgs Parse(string text)
        {
            using (var sr = new StringReader(text))
            {
                var doc = XDocument.Load(sr);
                var obj = new SvnInfoEventArgs()
                {

                    Path = doc.XPathSelectElement("/info/entry").Attribute("path").Value,
                    RepositoryRoot = new Uri(doc.XPathSelectElement("/info/entry/repository/root").Value + "/"),
                    Revision = long.Parse(doc.XPathSelectElement("/info/entry").Attribute("revision").Value),
                    Uri = new Uri(doc.XPathSelectElement("/info/entry/url").Value, UriKind.RelativeOrAbsolute),
                    LastChangeRevision = long.Parse(doc.XPathSelectElement("/info/entry/commit").Attribute("revision").Value)
                };

                return obj;
            }
        }

        private static void Read(string text, Dictionary<string, string> props)
        {
            var match = Regex.Match(text, $"(.*): (.*)");
            props.Add(match.Groups[1].Value, match.Groups[2].Value);
        }
    }
}