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

//using Ntreev.Crema.Data.Xml;
//using Ntreev.Crema.ServiceModel;
//using Ntreev.Crema.Data;
//using Ntreev.Crema.Data.Xml.Schema;
//using Ntreev.Library;
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
//    public class CremaXmlFormatter : IFormatter
//    {
//        private SerializationBinder binder;
//        private StreamingContext context;
//        private ISurrogateSelector surrogateSelector;

//        public CremaDataSet Deserialize(Stream stream)
//        {
//            throw new NotImplementedException();
//        }

//        public void Serialize(Stream stream, CremaDataTable dataTable)
//        {
//            using (XmlWriter writer = XmlWriter.Create(stream, CremaXmlSerializer.settings))
//            {
//                CremaXmlFormatter.WriteCore(writer, dataTable);
//            }
//        }

//        public void Serialize(TextWriter textWriter, CremaDataTable dataTable)
//        {
//            using (XmlWriter writer = XmlWriter.Create(textWriter, CremaXmlSerializer.settings))
//            {
//                CremaXmlFormatter.WriteCore(writer, dataTable);
//            }
//        }

//        public void Serialize(string filename, CremaDataTable dataTable)
//        {
//            using (XmlWriter writer = XmlWriter.Create(filename, CremaXmlSerializer.settings))
//            {
//                CremaXmlFormatter.WriteCore(writer, dataTable);
//            }
//        }

//        private static void WriteCore(XmlWriter xmlWriter, CremaDataTable dataTable)
//        {
//            xmlWriter.WriteStartElement("Table");

//            xmlWriter.WriteAttributeString("Name", dataTable.Name);
//            xmlWriter.WriteAttributeString("Category", dataTable.CategoryName);
//            xmlWriter.WriteAttributeString("Columns", dataTable.Columns.Count.ToString());
//            xmlWriter.WriteAttributeString("Rows", dataTable.Rows.Count.ToString());

//            xmlWriter.WriteAttribute(CremaSchema.Creator, CremaSchema.CreatedDateTime, dataTable.CreationInfo);
//            xmlWriter.WriteAttribute(CremaSchema.Modifier, CremaSchema.ModifiedDateTime, dataTable.ModificationInfo);

//            CremaDataColumn[] dataColumns = dataTable.Columns.ToArray();

//            CremaXmlFormatter.WriteDataColumns(xmlWriter, dataTable, dataColumns);
//            CremaXmlFormatter.WriteDataRows(xmlWriter, dataTable, dataColumns);

//            xmlWriter.WriteEndElement();
//        }

//        private static void WriteDataColumns(XmlWriter xmlWriter, CremaDataTable dataTable, CremaDataColumn[] dataColumns)
//        {
//            xmlWriter.WriteStartElement("Columns");

//            foreach (CremaDataColumn item in dataColumns)
//            {
//                xmlWriter.WriteStartElement("Column");
//                xmlWriter.WriteAttributeString("Name", item.ColumnName);
//                xmlWriter.WriteAttributeString("DataType", item.DataType.GetTypeName());
//                xmlWriter.WriteAttributeString("IsKey", item.IsKey.ToString());

//                if (dataTable.Parent != null)
//                {
//                    xmlWriter.WriteAttributeString("ParentColumn", CremaSchema.GenerateRelationID(dataTable.Parent.TableName));
//                }

//                xmlWriter.WriteAttribute("Default", CremaXmlConvert.ToString(item.DefaultValue));
//                xmlWriter.WriteAttribute(CremaSchema.Creator, item.CreationInfo.ID);
//                xmlWriter.WriteAttribute(CremaSchema.CreatedDateTime, item.CreationInfo.DateTime);
//                xmlWriter.WriteAttribute(CremaSchema.Modifier, item.ModificationInfo.ID);
//                xmlWriter.WriteAttribute(CremaSchema.ModifiedDateTime, item.ModificationInfo.DateTime);

//                xmlWriter.WriteEndElement();
//            }

//            xmlWriter.WriteEndElement();
//        }

//        private static void WriteDataRows(XmlWriter xmlWriter, CremaDataTable dataTable, CremaDataColumn[] dataColumns)
//        {
//            xmlWriter.WriteStartElement("Rows");

//            foreach (var item in dataTable.Rows)
//            {
//                WriteDataRow(xmlWriter, item, dataColumns);
//            }

//            xmlWriter.WriteEndElement();
//        }

//        private static void WriteDataRow(XmlWriter writer, CremaDataRow dataRow, IEnumerable<CremaDataColumn> columns)
//        {
//            writer.WriteStartElement("Row");

//            foreach (var item in columns)
//            {
//                object value = dataRow[item];
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
//            this.Serialize(serializationStream, graph as CremaDataTable);
//        }

//        public ISurrogateSelector SurrogateSelector
//        {
//            get { return this.surrogateSelector; }
//            set { this.surrogateSelector = value; }
//        }
//    }
//}
