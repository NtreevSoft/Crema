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

//using Ntreev.Library;
//using Ntreev.Crema.Data;
//using Ntreev.Crema.Data.Xml;
//using Ntreev.Crema.Data.Xml.Schema;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Xml;
//using System.Data;

//namespace Ntreev.Crema.RuntimeService.Xml
//{
//    [Obsolete]
//    public class DataTypeXmlFormatter : IFormatter
//    {
//        private SerializationBinder binder;
//        private StreamingContext context;
//        private ISurrogateSelector surrogateSelector;

//        private static XmlWriterSettings settings = new XmlWriterSettings()
//        {
//            Encoding = Encoding.UTF8,
//            OmitXmlDeclaration = false,
//        };

//        public CremaDataSet Deserialize(Stream stream)
//        {
//            throw new NotImplementedException();
//        }

//        public void Serialize(Stream stream, CremaDataType dataType)
//        {
//            using (XmlWriter writer = XmlWriter.Create(stream, settings))
//            {
//                WriteCore(writer, dataType);
//            }
//        }

//        public void Serialize(TextWriter textWriter, CremaDataType dataType)
//        {
//            using (XmlWriter writer = XmlWriter.Create(textWriter, settings))
//            {
//                WriteCore(writer, dataType);
//            }
//        }

//        public void Serialize(string filename, CremaDataType dataType)
//        {
//            using (XmlWriter writer = XmlWriter.Create(filename, settings))
//            {
//                WriteCore(writer, dataType);
//            }
//        }

//        private static void WriteCore(XmlWriter writer, CremaDataType dataType)
//        {
//            writer.WriteStartElement("DataType");

//            writer.WriteAttributeString("Name", dataType.Name);
//            writer.WriteAttributeString("IsFlag", dataType.IsFlag.ToString());
//            writer.WriteAttributeString("Comment", dataType.Comment);

//            if (string.IsNullOrEmpty(dataType.CreationInfo.ID) == false)
//                writer.WriteAttributeString(CremaSchema.Creator, dataType.CreationInfo.ID);
//            if (dataType.CreationInfo.DateTime != DateTime.MinValue)
//                writer.WriteAttributeString(CremaSchema.CreatedDateTime, dataType.CreationInfo.DateTime.ToXmlString());
//            if (string.IsNullOrEmpty(dataType.ModificationInfo.ID) == false)
//                writer.WriteAttributeString(CremaSchema.Modifier, dataType.ModificationInfo.ID);
//            if (dataType.ModificationInfo.DateTime != DateTime.MinValue)
//                writer.WriteAttributeString(CremaSchema.ModifiedDateTime, dataType.ModificationInfo.DateTime.ToXmlString());

//            if (dataType.Members.Any() == true)
//            {
//                writer.WriteAttributeString("Members", dataType.Members.Count.ToString());
//                WriteDataColumns(writer, dataType.Members.ToArray());
//            }

//            writer.WriteEndElement();
//        }

//        private static void WriteDataColumns(XmlWriter writer, CremaDataTypeMember[] typeMembers)
//        {
//            writer.WriteStartElement("Members");

//            foreach (CremaDataTypeMember item in typeMembers)
//            {
//                writer.WriteStartElement("Member");
//                writer.WriteAttributeString("Name", item.Name);
//                writer.WriteAttributeString("Value", item.Value.ToString());
//                if (string.IsNullOrEmpty(item.CreationInfo.ID) == false)
//                    writer.WriteAttributeString(CremaSchema.Creator, item.CreationInfo.ID);
//                if (item.CreationInfo.DateTime != DateTime.MinValue)
//                    writer.WriteAttributeString(CremaSchema.CreatedDateTime, item.CreationInfo.DateTime.ToXmlString());
//                if (item.ModificationInfo.ID != null)
//                    writer.WriteAttributeString(CremaSchema.Modifier, item.ModificationInfo.ID);
//                if (item.ModificationInfo.DateTime != DateTime.MinValue)
//                    writer.WriteAttributeString(CremaSchema.ModifiedDateTime, item.ModificationInfo.DateTime.ToXmlString());
//                writer.WriteEndElement();
//            }

//            writer.WriteEndElement();
//        }

//        public SerializationBinder Binder
//        {
//            get { return this.binder; }
//            set { this.binder = value; }
//        }

//        public StreamingContext Context
//        {
//            get { return this.context; }
//            set { this.context = value; }
//        }

//        object IFormatter.Deserialize(Stream serializationStream)
//        {
//            throw new NotImplementedException();
//        }

//        void IFormatter.Serialize(Stream serializationStream, object graph)
//        {
//            this.Serialize(serializationStream, (CremaDataType)graph);
//        }

//        public ISurrogateSelector SurrogateSelector
//        {
//            get { return this.surrogateSelector; }
//            set { this.surrogateSelector = value; }
//        }
//    }
//}
