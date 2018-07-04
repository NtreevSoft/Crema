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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ntreev.Crema.Repository.Svn
{
    struct SvnChangeInfo
    {
        public string Path { get; internal set; }

        public string CopyFromPath { get; internal set; }

        public string CopyFromRevision { get; private set; }

        public string Action { get; private set; }

        public static SvnChangeInfo Parse(XElement element)
        {
            var obj = new SvnChangeInfo()
            {
                Action = element.Attribute("action").Value,
                Path = element.Value,
            };

            {
                var attr = element.Attribute("copyfrom-path");
                if (attr != null)
                {
                    obj.CopyFromPath = attr.Value;
                }
            }
            {
                var attr = element.Attribute("copyfrom-rev");
                if (attr != null)
                {
                    obj.CopyFromRevision = attr.Value;
                }
            }

            return obj;
        }
    }
}
