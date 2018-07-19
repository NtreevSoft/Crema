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
using Ntreev.Library.IO;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Ntreev.Crema.Data.Xml
{
    public class CremaTypeXmlResolver : XmlResolver
    {
        private readonly CremaDataSet dataSet;
        private readonly string tableNamespace;

        public CremaTypeXmlResolver(CremaDataSet dataSet, string tableNamespace)
        {
            this.dataSet = dataSet;
            this.tableNamespace = tableNamespace;
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            var typeNamespace = UriUtility.RemoveExtension(absoluteUri);
            var typeName = Path.GetFileNameWithoutExtension(typeNamespace);
            var categoryPath = this.dataSet.GetTypeCategoryPath(typeNamespace);
            var type = this.dataSet.Types[typeName, categoryPath];

            if (type == null)
                return null;

            var bytes = Encoding.ASCII.GetBytes(type.GetXmlSchema());
            return new MemoryStream(bytes);
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            var tableNamespace = this.dataSet.TableNamespace;

            var ss = relativeUri.Split(PathUtility.SeparatorChar);
            for (var i = 0; i < ss.Length; i++)
            {
                var segment = ss[i];
                if (segment != "..")
                    break;

                tableNamespace += PathUtility.Separator + "dummy";
            }

            return new Uri(new Uri(tableNamespace), relativeUri);
        }
    }
}
