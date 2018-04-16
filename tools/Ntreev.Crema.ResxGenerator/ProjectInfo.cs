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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Ntreev.Crema.ResxGenerator
{
    class ProjectInfo
    {
        private readonly XDocument doc;

        private readonly string rootNamespace;
        private readonly ResxInfo[] resxInfos;

        public ProjectInfo(string xml)
        {
            using (var sr = new StringReader(xml))
            using (var reader = XmlReader.Create(sr))
            {
                this.doc = XDocument.Load(reader);
                var ns = this.doc.Root.GetDefaultNamespace();
                var namespaceManager = new XmlNamespaceManager(reader.NameTable);
                namespaceManager.AddNamespace("xs", ns.NamespaceName);

                {
                    var element = this.doc.Root.XPathSelectElement($"/xs:Project/xs:PropertyGroup/xs:RootNamespace", namespaceManager);
                    this.rootNamespace = element.Value;
                }

                {
                    var elements = this.doc.Root.XPathSelectElements($"/xs:Project/xs:ItemGroup/xs:EmbeddedResource", namespaceManager);

                    var resxInfoList = new List<ResxInfo>();
                    foreach (var item in elements)
                    {
                        var resxInfo = new ResxInfo();
                        resxInfo.Parse(item, namespaceManager);
                        resxInfoList.Add(resxInfo);
                    }
                    this.resxInfos = resxInfoList.ToArray();
                }
            }
        }

        public ResxInfo[] ResxInfos
        {
            get { return this.resxInfos; }
        }

        public string RootNamespace
        {
            get { return this.rootNamespace; }
        }
    }
}
