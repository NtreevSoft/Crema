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

//using Ntreev.Crema.Data;
//using Ntreev.Library.IO;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Ntreev.Crema.Data.Xml;
//using Ntreev.Crema.ServiceModel;
//using System.Xml;
//using System.Runtime.Serialization;
//using Ntreev.Crema.RuntimeService.Binary;
//using System.Text.RegularExpressions;
//using System.Xml.Linq;
//using System.Diagnostics;

//namespace Ntreev.Crema.RuntimeService.Xml
//{
//    public class CremaXmlSerializer : CremaSerializerBase
//    {
//        private readonly IFormatter formatter = new CremaXmlFormatter();
//        private readonly IFormatter dataTypeFormatter = new DataTypeXmlFormatter();

//        internal static XmlWriterSettings settings = new XmlWriterSettings()
//        {
//            Encoding = Encoding.UTF8,
//            OmitXmlDeclaration = false,
//            NewLineChars = "\n",
//        };

//        private const string tableExtension = ".xml";
//        private const string typeExtension = ".typ";

//        public CremaXmlSerializer(CremaDataSet dataSet)
//            : base(dataSet)
//        {

//        }

//        protected override void Link(Stream stream, IEnumerable<string> files)
//        {
//            XmlReaderSettings settins = new XmlReaderSettings() { IgnoreWhitespace = true,};

//            var dataTypes = files.Where(item => Path.GetExtension(item) == CremaXmlSerializer.typeExtension).OrderBy(item => item);
//            var tables = files.Where(item => Path.GetExtension(item) == CremaXmlSerializer.tableExtension).OrderBy(item => item);

//            using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
//            {
//                xmlWriter.WriteStartElement("Crema");

//                xmlWriter.WriteAttributeString("DataTypes", dataTypes.Count().ToString());
//                xmlWriter.WriteAttributeString("Tables", tables.Count().ToString());

//                xmlWriter.WriteStartElement("DataTypes");
//                foreach (var item in dataTypes)
//                {
//                    using (XmlReader reader = XmlReader.Create(item, settins))
//                    {
//                        reader.MoveToContent();
//                        xmlWriter.WriteRaw(reader.ReadOuterXml());
//                    }
//                }
//                xmlWriter.WriteEndElement();

//                xmlWriter.WriteStartElement("Tables");
//                foreach (var item in tables)
//                {
//                    using (XmlReader reader = XmlReader.Create(item, settins))
//                    {
//                        reader.MoveToContent();
//                        xmlWriter.WriteRaw(reader.ReadOuterXml());
//                    }
//                }
//                xmlWriter.WriteEndElement();

//                xmlWriter.WriteEndElement();
//            }
//        }

//        protected override string TableExtension
//        {
//            get { return CremaXmlSerializer.tableExtension; }
//        }

//        protected override string TypeExtension
//        {
//            get { return CremaXmlSerializer.typeExtension; }
//        }

//        protected override void SerializeTable(Stream stream, CremaDataTable dataTable, long modifiedTime)
//        {
//            this.formatter.Serialize(stream, dataTable);
//        }

//        protected override void SerializeType(Stream stream, CremaDataType dataType, long modifiedTime)
//        {
//            this.dataTypeFormatter.Serialize(stream, dataType);
//        }
//    }
//}
