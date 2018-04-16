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

namespace Ntreev.Crema.Data.Random
{
    class SummaryProvider
    {
        private const string sourcePath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\ko";
        private readonly XElement docElement;


        public static SummaryProvider FromType(Type type)
        {
            var filename = Path.GetFileNameWithoutExtension(type.Assembly.CodeBase);
            var xmlPath = Path.Combine(sourcePath, filename + ".xml");
            return new SummaryProvider(xmlPath);
        }

        public SummaryProvider(string xmlPath)
        {
            if (File.Exists(xmlPath) == true)
                this.docElement = XElement.Load(xmlPath);
        }

        public string GetEnumSummary(string typeNamespace, string enumName)
        {
            if (this.docElement == null)
                return string.Empty;

            string xpath = string.Format("/members/member[@name='T:{0}.{1}']/summary", typeNamespace, enumName);
            var element = this.docElement.XPathSelectElement(xpath);

            if (element == null)
                return string.Empty;
            return element.Value;
        }

        public string GetMemberSummary(string typeNamespace, string enumName, string memberName)
        {
            if (this.docElement == null)
                return string.Empty;

            string xpath = string.Format("/members/member[@name='F:{0}.{1}.{2}']/summary", typeNamespace, enumName, memberName);
            var element = this.docElement.XPathSelectElement(xpath);

            if (element == null)
                return string.Empty;
            return element.Value;
        }
    }
}
