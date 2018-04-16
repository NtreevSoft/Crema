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

using Ntreev.Library.IO;
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
    struct ResxInfo
    {
        public string Name { get; set; }

        public bool IsPublic { get; set; }

        public string FileName { get; set; }

        public string ResgenFileName { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public void Parse(XElement element, XmlNamespaceManager namespaceManager)
        {
            var attr = element.Attribute(XName.Get("Include", string.Empty));

            var match = Regex.Match(attr.Value, "(.+)[.]([^.]+)([.]resx)$");
            if (match.Success == true)
            {
                this.CultureInfo = CultureInfo.GetCultureInfo(match.Groups[2].Value);
                this.Name = match.Groups[1].Value + match.Groups[3].Value;
                this.FileName = attr.Value;
            }
            else
            {
                this.Name = attr.Value;
                this.FileName = attr.Value;
            }

            var e1 = element.XPathSelectElement("./xs:Generator", namespaceManager);
            if (e1 != null)
            {
                this.IsPublic = e1.Value == "PublicResXFileCodeGenerator";
            }

            var e2 = element.XPathSelectElement("./xs:LastGenOutput", namespaceManager);
            if (e2 != null)
            {
                this.ResgenFileName = Path.Combine(Path.GetDirectoryName(this.FileName), e2.Value);
            }
            else
            {
                this.ResgenFileName = string.Empty;
            }


            this.Name = this.Name.Replace(Path.DirectorySeparatorChar, PathUtility.SeparatorChar);
            this.FileName = this.FileName.Replace(Path.DirectorySeparatorChar, PathUtility.SeparatorChar);
            this.ResgenFileName = this.ResgenFileName.Replace(Path.DirectorySeparatorChar, PathUtility.SeparatorChar);
        }
    }
}
