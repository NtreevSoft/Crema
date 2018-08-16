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

using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ntreev.Crema.Repository.Git
{
    struct GitPropertyValue
    {
        //public string Prefix { get; private set; }

        public string Key { get; private set; }

        public string Value { get; private set; }

        //public static GitPropertyValue Parse(XElement element)
        //{
        //    var propName = element.Attribute("name").Value;
        //    var match = Regex.Match(propName, @"(?<prefix>[^:]+:)(?<key>.+)", RegexOptions.ExplicitCapture);
        //    var prefix = match.Groups["prefix"].Value.Trim();
        //    var name = match.Groups["key"].Value.Trim();

        //    var obj = new GitPropertyValue()
        //    {
        //        Value = element.Value,
        //    };

        //    if (prefix == string.Empty)
        //    {
        //        obj.Prefix = string.Empty;
        //        obj.Key = propName;
        //    }
        //    else
        //    {
        //        obj.Prefix = prefix;
        //        obj.Key = name;
        //    }

        //    return obj;
        //}

        public static explicit operator LogPropertyInfo(GitPropertyValue value)
        {
            return new LogPropertyInfo()
            {
                Key = value.Key,
                Value = value.Value,
            };
        }

        public static explicit operator GitPropertyValue(LogPropertyInfo value)
        {
            return new GitPropertyValue()
            {
                Key = value.Key,
                Value = value.Value,
            };
        }
    }
}
