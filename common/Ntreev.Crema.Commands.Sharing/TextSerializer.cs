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

using Newtonsoft.Json;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace Ntreev.Crema.Commands
{
    public static class TextSerializer
    {
        private readonly static Serializer yamlSerializer;

        static TextSerializer()
        {
            yamlSerializer = new SerializerBuilder().DisableAliases().Build();
        }

        public static string Serialize(object obj)
        {
            return Serialize(obj, TextSerializerType.Yaml);
        }

        public static string Serialize(object obj, TextSerializerType type)
        {
            if (type == TextSerializerType.Yaml)
            {
                return yamlSerializer.Serialize(obj);
            }
            else if (type == TextSerializerType.Json)
            {
                return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
            else if (type == TextSerializerType.Xml)
            {
                return XmlSerializerUtility.GetString(obj, true);
            }
            throw new NotImplementedException();
        }
    }
}