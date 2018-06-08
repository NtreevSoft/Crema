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
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Xml;
using System.Linq;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.IO;
using System.Text;
using System.Collections.Generic;
using System.Xml.Schema;
using Ntreev.Library.Linq;

namespace Ntreev.Crema.Data.Xml
{
    public class CremaXmlWriter
    {
        private CremaDataSet dataSet;
        private CremaDataTable dataTable;
        private string targetNamespace;
        private bool isFamily = false;

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
            //this.isFamily = true;
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
                writer.WriteStartElement(CremaDataSet.DefaultDataSetName, this.targetNamespace);
                this.WriteTable(writer, this.dataTable);
                writer.WriteEndElement();
            }
            else
            {
                throw new CremaDataException("there is no table to write");
            }
        }

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

        internal static void WriteField(XmlWriter writer, CremaDataRow dataRow, CremaDataColumn dataColumn, bool ignoreDefaultValue)
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

            if (this.isFamily == true && dataTable.Childs.Count > 0)
            {
                var relationID = dataRow.Field<string>(dataTable.ColumnRelation);

                foreach (var child in dataTable.Childs)
                {
                    var rows = child.Select(string.Format("{0} = '{1}'", "__ParentID__", relationID));

                    foreach (var row in rows)
                    {
                        this.WriteDataRow(writer, row);
                    }
                }

            }

            writer.WriteEndElement();
        }

        private void WriteTable(XmlWriter writer, CremaDataTable dataTable)
        {
            writer.WriteAttribute(CremaSchema.Version, CremaSchema.VersionValue);

            if (dataTable.TemplateNamespace != string.Empty)
            {
                var relativeUri = UriUtility.MakeRelative(dataTable.Namespace, dataTable.TemplateNamespace) + CremaSchema.SchemaExtension;
                var value = string.Format("{0} {1}", dataTable.Namespace, relativeUri);
                writer.WriteAttributeString(CremaSchema.InstancePrefix, "schemaLocation", XmlSchema.InstanceNamespace, value);
                writer.WriteAttribute(CremaSchema.Creator, CremaSchema.CreatedDateTime, dataTable.CreationInfo);
            }
            else
            {
                var value = string.Format("{0} {1}", dataTable.Namespace, dataTable.Name + CremaSchema.SchemaExtension);
                writer.WriteAttributeString(CremaSchema.InstancePrefix, "schemaLocation", XmlSchema.InstanceNamespace, value);
            }

            this.WriteHeaderInfo(writer, dataTable);

            this.WriteRows(writer, dataTable);
        }

        private void WriteTables(XmlWriter writer, IEnumerable<CremaDataTable> tables)
        {
            writer.WriteAttribute(CremaSchema.Version, CremaSchema.VersionValue);

            foreach (var item in tables)
            {
                this.WriteHeaderInfo(writer, item);
            }

            foreach (var item in tables)
            {
                this.WriteRows(writer, item);
            }
        }

        private void WriteHeaderInfo(XmlWriter writer, CremaDataTable table)
        {
            var items = this.isFamily == true ? EnumerableUtility.Friends(table, table.Childs) : Enumerable.Repeat(table, 1);
            foreach (var item in items)
            {
                var name = item.GetXmlPath(this.targetNamespace);
                var user = name + CremaSchema.ModifierExtension;
                var dateTime = name + CremaSchema.ModifiedDateTimeExtension;
                var count = name + CremaSchema.CountExtension;
                var id = name + CremaSchema.IDExtension;
                writer.WriteAttribute(user, dateTime, item.ContentsInfo);
                writer.WriteAttribute(count, item.Rows.Count);
                writer.WriteAttribute(id, item.TableID.ToString());
            }
        }

        private void WriteRows(XmlWriter writer, CremaDataTable table)
        {
            foreach (var item in table.Rows)
            {
                if (item.RowState == DataRowState.Deleted)
                    continue;
                this.WriteDataRow(writer, item);
            }
        }
    }
}
