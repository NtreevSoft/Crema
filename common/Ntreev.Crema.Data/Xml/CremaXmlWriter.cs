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
using Ntreev.Library;
using Ntreev.Library.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Ntreev.Crema.Data.Xml
{
    public class CremaXmlWriter
    {
        private CremaDataSet dataSet;
        private CremaDataTable dataTable;
        private string targetNamespace;

        private static XmlWriterSettings settings = new XmlWriterSettings()
        {
            Encoding = Encoding.UTF8,
            Indent = true,
            NewLineOnAttributes = true,
        };

        public CremaXmlWriter(CremaDataSet dataSet)
        {
            this.dataSet = dataSet;
            this.targetNamespace = dataSet.Namespace;
        }

        public CremaXmlWriter(CremaDataTable dataTable)
        {
            this.dataTable = dataTable;
            if (dataTable.TemplateNamespace != string.Empty)
                this.targetNamespace = dataTable.TemplateNamespace;
            else
                this.targetNamespace = dataTable.Namespace;
        }

        public void Write(string filename)
        {
            using (XmlWriter writer = XmlWriter.Create(filename, CremaXmlWriter.settings))
            {
                this.Write(writer);
            }
        }

        public void Write(Stream stream)
        {
            using (XmlWriter writer = XmlWriter.Create(stream, CremaXmlWriter.settings))
            {
                this.Write(writer);
            }
        }

        public void Write(TextWriter writer)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(writer, CremaXmlWriter.settings))
            {
                this.Write(xmlWriter);
            }
        }

        public void Write(XmlWriter writer)
        {
            if (this.dataSet != null)
            {
                writer.WriteStartElement(this.dataSet.DataSetName, this.targetNamespace);
                this.WriteTables(writer, this.dataSet.Tables.OrderBy(item => item.Name).OrderBy(item => item.TemplateNamespace));
                writer.WriteEndElement();
            }
            else if (this.dataTable != null)
            {
                if (this.dataTable.TableName == string.Empty)
                    throw new CremaDataException("table without name cannot serialize");
                var tables = this.IsRecursive == true ? EnumerableUtility.Friends(this.dataTable, this.dataTable.Childs) : Enumerable.Repeat(this.dataTable, 1);
                writer.WriteStartElement(CremaDataSet.DefaultDataSetName, this.targetNamespace);
                this.WriteTables(writer, tables);
                writer.WriteEndElement();
            }
            else
            {
                throw new CremaDataException("there is no table to write");
            }
        }

        public bool IsRecursive { get; set; }

        private void WriteAttribute(XmlWriter writer, InternalDataRow dataRow, InternalAttribute dataColumn)
        {
            var field = dataRow[dataColumn];

            if (field == DBNull.Value || object.Equals(field, dataColumn.DefaultValue) == true)
                return;

            var textValue = CremaXmlConvert.ToString(field, dataColumn.DataType);

            if (string.IsNullOrEmpty(textValue) == false)
            {
                writer.WriteStartAttribute(dataColumn.AttributeName);
                writer.WriteValue(textValue);
                writer.WriteEndAttribute();
            }
        }

        private static void WriteField(XmlWriter writer, CremaDataRow dataRow, CremaDataColumn dataColumn)
        {
            WriteField(writer, dataRow, dataColumn, false);
        }

        private static void WriteField(XmlWriter writer, CremaDataRow dataRow, CremaDataColumn dataColumn, bool ignoreDefaultValue)
        {
            var dataType = dataColumn.DataType;
            var value = dataRow[dataColumn];

            if (value == DBNull.Value)
                return;

            var converter = TypeDescriptor.GetConverter(dataColumn.DataType);
            var textValue = CremaXmlConvert.ToString(value, dataType);

            if (object.Equals(value, dataColumn.DefaultValue) == true)
            {
                writer.WriteStartElement(dataColumn.ColumnName);
                writer.WriteEndElement();
            }
            else
            {
                writer.WriteStartElement(dataColumn.ColumnName);
                writer.WriteValue(textValue);
                writer.WriteEndElement();
            }
        }

        private void WriteDataRow(XmlWriter writer, CremaDataRow dataRow)
        {
            var dataTable = dataRow.Table;
            writer.WriteStartElement(dataTable.GetXmlName(this.targetNamespace));

            foreach (var item in dataTable.Attributes)
            {
                if (item.IsVisible == false)
                    continue;
                this.WriteAttribute(writer, dataRow.InternalObject, item.InternalAttribute);
            }

            if (dataTable.ColumnRelation != null)
            {
                writer.WriteStartAttribute(CremaSchema.RelationID);
                writer.WriteValue(dataRow.Field<string>(dataTable.ColumnRelation));
                writer.WriteEndAttribute();
            }

            if (dataTable.ParentRelation != null)
            {
                writer.WriteStartAttribute(CremaSchema.ParentID);
                writer.WriteValue(dataRow.Field<string>(dataTable.ParentRelation));
                writer.WriteEndAttribute();
            }

            foreach (var item in dataTable.Columns)
            {
                WriteField(writer, dataRow, item);
            }

            writer.WriteEndElement();
        }

        private void WriteTables(XmlWriter writer, IEnumerable<CremaDataTable> tables)
        {
            writer.WriteAttribute(CremaSchema.Version, CremaSchema.VersionValue);

            if (this.dataTable != null)
            {
                if (this.dataTable.TemplateNamespace != string.Empty)
                {
                    var relativeUri = UriUtility.MakeRelative(this.dataTable.Namespace, this.dataTable.TemplateNamespace) + CremaSchema.SchemaExtension;
                    var value = string.Format("{0} {1}", this.dataTable.Namespace, relativeUri);
                    writer.WriteAttributeString(CremaSchema.InstancePrefix, "schemaLocation", XmlSchema.InstanceNamespace, value);
                }
                else
                {
                    var value = string.Format("{0} {1}", this.dataTable.Namespace, this.dataTable.Name + CremaSchema.SchemaExtension);
                    writer.WriteAttributeString(CremaSchema.InstancePrefix, "schemaLocation", XmlSchema.InstanceNamespace, value);
                }
            }

            foreach (var item in tables)
            {
                if (item.TemplateNamespace != string.Empty)
                {
                    if (this.dataSet != null)
                    {
                        var name = item.Name;
                        var user = name + CremaSchema.CreatorExtension;
                        var dateTime = name + CremaSchema.CreatedDateTimeExtension;
                        writer.WriteAttribute(user, dateTime, item.CreationInfo);
                    }
                    else
                    {
                        var name = item.TemplatedParentName;
                        var user = name + CremaSchema.CreatorExtension;
                        var dateTime = name + CremaSchema.CreatedDateTimeExtension;
                        writer.WriteAttribute(user, dateTime, item.CreationInfo);
                    }
                }
            }

            foreach (var item in tables)
            {
                this.WriteHeaderInfo(writer, item);
            }

            foreach (var item in tables)
            {
                this.WriteRows(writer, item);
            }
        }

        private void WriteHeaderInfo(XmlWriter writer, CremaDataTable dataTable)
        {
            var name = dataTable.GetXmlPath(this.targetNamespace);
            var user = name + CremaSchema.ModifierExtension;
            var dateTime = name + CremaSchema.ModifiedDateTimeExtension;
            var count = name + CremaSchema.CountExtension;
            var id = name + CremaSchema.IDExtension;
            writer.WriteAttribute(user, dateTime, dataTable.ContentsInfo);
            writer.WriteAttribute(count, dataTable.Rows.Count);
            writer.WriteAttribute(id, dataTable.TableID.ToString());
        }

        private void WriteRows(XmlWriter writer, CremaDataTable dataTable)
        {
            foreach (var item in dataTable.Rows)
            {
                if (item.RowState == DataRowState.Deleted)
                    continue;
                this.WriteDataRow(writer, item);
            }
        }
    }
}
