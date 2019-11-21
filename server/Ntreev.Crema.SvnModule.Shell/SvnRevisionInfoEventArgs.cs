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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Ntreev.Library;

namespace Ntreev.Crema.SvnModule
{
    struct SvnRevisionInfoEventArgs
    {
        public string Name { get; private set; }
        public long Revision { get; private set; }

        public static SvnRevisionInfoEventArgs[] Run(string path, long revision = -1)
        {
            var revisionString = revision == -1 ? "head" : revision.ToString();

            var text = SvnClientHost.Run("list", "-R", "-r", revisionString, "--xml", path.WrapQuot());
            return Parse(text);
        }

        private static SvnRevisionInfoEventArgs[] Parse(string text)
        {
            var list = new List<SvnRevisionInfoEventArgs>(100);
            using (var reader = new StringReader(text))
            {
                var doc = XDocument.Load(reader);
                foreach (var element in doc.XPathSelectElements("/lists/list/entry[@kind='file']"))
                {
                    var name = element.Element("name")?.Value;
                    var revision = long.Parse(element.Element("commit")?.Attribute("revision")?.Value ?? "0");

                    list.Add(new SvnRevisionInfoEventArgs
                    {
                        Name = name,
                        Revision = revision
                    });
                }
            }

            return list.ToArray();
        }
    }
}
