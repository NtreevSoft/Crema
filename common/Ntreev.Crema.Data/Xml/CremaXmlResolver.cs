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

using Ntreev.Crema.Data.Xml.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Ntreev.Crema.Data.Xml
{
    public class CremaXmlResolver : XmlResolver
    {
        private readonly string filename;
        private readonly CremaDataSet dataSet;
        private Dictionary<string, SchemaBuilder> nameToBuilder = new Dictionary<string, SchemaBuilder>();

        public static CremaXmlResolver Default = new CremaXmlResolver();

        public CremaXmlResolver()
        {

        }

        public CremaXmlResolver(CremaDataSet dataSet)
        {
            this.dataSet = dataSet;
        }

        public CremaXmlResolver(string filename)
        {
            this.filename = filename;
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            return File.OpenRead(absoluteUri.LocalPath);
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if (this.filename == null)
                return base.ResolveUri(baseUri, relativeUri);
            var uri = base.ResolveUri(new Uri(this.filename), relativeUri);
#if DEBUG
            if (File.Exists(uri.LocalPath) == false)
                throw new FileNotFoundException();
#endif
            return uri;
        }

        #region classes

        class SchemaBuilder
        {
            XmlSchema schema;

            public XmlSchema GetSchema(string path)
            {
                lock (this)
                {
                    if (this.schema != null)
                        return this.schema;

                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        this.schema = XmlSchema.Read(stream, CremaSchema.SchemaValidationEventHandler);
                    }
                    return this.schema;
                }
            }
        }

        #endregion
    }
}
