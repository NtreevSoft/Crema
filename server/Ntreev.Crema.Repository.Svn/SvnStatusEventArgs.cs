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
    struct SvnStatusEventArgs
    {
        public IDictionary<string, string> Status { get; private set; }

        public static SvnStatusEventArgs Run(string path)
        {
            var text = SvnClientHost.Run("status", path.WrapQuot(), "--xml");
            return Parse(text);
        }

        public static SvnStatusEventArgs Parse(string text)
        {
            using (var sr = new StringReader(text))
            {
                var doc = XDocument.Load(sr);
                var dictionary = new Dictionary<string, string>();

                foreach (var item in doc.XPathSelectElements("/status/target/entry"))
                {
                    var path = item.Attribute("path").Value;
                    var status = item.XPathSelectElement("wc-status").Attribute("item").Value;
                    dictionary.Add(path, status);
                }

                return new SvnStatusEventArgs() { Status = dictionary };
            }
        }
    }
}