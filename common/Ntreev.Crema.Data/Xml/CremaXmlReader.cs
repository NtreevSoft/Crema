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
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Ntreev.Crema.Data.Xml
{
    public class CremaXmlReader
    {
        private readonly CremaDataSet dataSet;
        private readonly CremaDataTable dataTable;
        private readonly ItemName itemName;
        private readonly Version version40 = new Version(4, 0);
        private Dictionary<string, CremaDataTable> tables;
        private Version version = new Version();

        public CremaXmlReader(CremaDataSet dataSet)
        {
            this.dataSet = dataSet;
        }

        public CremaXmlReader(CremaDataSet dataSet, ItemName itemName)
        {
            this.dataSet = dataSet;
            this.itemName = itemName;
        }

        public CremaXmlReader(CremaDataTable dataTable)
        {
            this.dataTable = dataTable;
            this.dataSet = dataTable.DataSet;
        }

        public void Read(string filename)
        {
            using (var reader = XmlReader.Create(filename))
            {
                this.Read(reader);
            }
        }

        public void Read(Stream stream)
        {
            using (var xmlReader = XmlReader.Create(stream))
            {
                this.Read(xmlReader);
            }
        }

        public void Read(TextReader reader)
        {
            using (var xmlReader = XmlReader.Create(reader))
            {
                this.Read(xmlReader);
            }
        }

        public void Read(XmlReader reader)
        {
            reader.MoveToContent();
            var text = reader.GetAttribute(CremaSchema.Version);
            if (string.IsNullOrEmpty(text) == false)
            {
                Version.TryParse(text, out this.version);
            }

            this.InitializeTables(reader);

            this.BeginLoad();
            this.ReadHeaderInfo(reader);

            if (reader.IsEmptyElement == false && this.OmitContent == false)
            {
                reader.ReadStartElement();
                reader.MoveToContent();
                while (reader.NodeType == XmlNodeType.Element)
                {
                    this.ReadNode(reader, null);
                }
                reader.MoveToContent();
                reader.ReadEndElement();
            }
            else
            {
                reader.Skip();
            }
            this.EndLoad();
        }

        public void ReadModifyInfo(TextReader textReader)
        {
            using (var reader = new XmlTextReader(textReader))
            {
                this.ReadModifyInfo(reader);
            }
        }

        public void ReadModifyInfo(XmlReader xmlReader)
        {
            var doc = XDocument.Load(xmlReader, LoadOptions.PreserveWhitespace);
            this.ReadModifyInfo(doc);
        }

        public bool OmitContent { get; set; }

        protected virtual void OnRowInsertAction(CremaDataTable dataTable, CremaDataRow dataRow)
        {
            dataTable.Rows.Add(dataRow);
        }

        private void BeginLoad()
        {
            if (this.dataSet != null)
            {
                this.dataSet.BeginLoad();
            }
            else
            {
                this.dataTable.BeginLoad();
            }
        }

        private void EndLoad()
        {
            if (this.dataSet != null)
            {
                this.dataSet.EndLoad();
            }
            else
            {
                this.dataTable.EndLoad();
            }
        }

        private CremaDataTable GetTable(XmlReader reader, CremaDataRow parentRow)
        {
            var tableName = reader.Name;

            if (parentRow != null)
            {
                return parentRow.Table.Childs[tableName];
            }
            else if (reader.NamespaceURI == CremaSchema.BaseNamespace)
            {
                return this.tables[tableName];
            }
            else if (this.itemName == null)
            {
                return this.tables[tableName];
            }
            else
            {
                return this.tables[this.itemName.Name];
            }
        }

        private void InitializeTables(XmlReader reader)
        {
            if (this.dataSet != null)
            {
                this.tables = new Dictionary<string, CremaDataTable>(this.dataSet.Tables.Count);

                if (itemName != null)
                {
                    var dataTable = this.dataSet.Tables[itemName.Name, itemName.CategoryPath];
                    var items = EnumerableUtility.Friends(dataTable, dataTable.Childs);
                    foreach (var item in items)
                    {
                        this.tables.Add(item.Name, item);
                    }
                }
                else if (reader.NamespaceURI == this.dataSet.Namespace)
                {
                    this.tables = new Dictionary<string, CremaDataTable>(this.dataSet.Tables.Count);
                    foreach (var item in this.dataSet.Tables)
                    {
                        this.tables.Add(item.Name, item);
                    }
                }
                else
                {
                    var categoryPath = CremaDataSet.GetTableCategoryPath(this.dataSet, reader.NamespaceURI);
                    var tableName = CremaDataSet.GetTableName(this.dataSet, reader.NamespaceURI);
                    var dataTable = this.dataSet.Tables[tableName, categoryPath];
                    var items = EnumerableUtility.Friends(dataTable, dataTable.Childs);
                    foreach (var item in items)
                    {
                        this.tables.Add(item.Name, item);
                    }
                }
            }
            else
            {
                var items = EnumerableUtility.Friends(this.dataTable, this.dataTable.Childs);
                this.tables = new Dictionary<string, CremaDataTable>(items.Count());
                foreach (var item in items)
                {
                    this.tables.Add(item.Name, item);
                }
            }
        }

        private void ReadHeaderInfo(XmlReader reader)
        {
            if (this.version < new Version(3, 0))
            {
                this.ReadHeaderInfoVersion2(reader);
                return;
            }

            var baseNamespace = this.dataSet != null ? this.dataSet.Namespace : CremaSchema.BaseNamespace;

            if (reader.NamespaceURI == baseNamespace)
            {
                foreach (var item in this.dataSet.Tables)
                {
                    var user = item.Name + CremaSchema.ModifierExtension;
                    var dateTime = item.Name + CremaSchema.ModifiedDateTimeExtension;
                    var count = item.Name + CremaSchema.CountExtension;
                    var id = item.Name + CremaSchema.IDExtension;
                    item.MinimumCapacity = reader.GetAttributeAsInt32(count);
                    item.InternalContentsInfo = reader.GetAttributeAsSignatureDate(user, dateTime);
                    item.InternalTableID = reader.GetAttributeAsGuid(id);
                }
            }
            else if (this.itemName == null)
            {
                var categoryPath = CremaDataSet.GetTableCategoryPath(this.dataSet, reader.NamespaceURI);
                var tableName = CremaDataSet.GetTableName(this.dataSet, reader.NamespaceURI);
                var dataTable = this.dataTable ?? this.dataSet.Tables[tableName, categoryPath];
                var items = EnumerableUtility.Friends(dataTable, dataTable.Childs);
                foreach (var item in items)
                {
                    var user = item.Name + CremaSchema.ModifierExtension;
                    var dateTime = item.Name + CremaSchema.ModifiedDateTimeExtension;
                    var count = item.Name + CremaSchema.CountExtension;
                    var id = item.Name + CremaSchema.IDExtension;
                    if (reader.TryGetAttributeAsInt32(count, out var countValue) == true)
                    {
                        item.MinimumCapacity = countValue;
                    }
                    if (reader.TryGetAttributeAsSignatureDate(user, dateTime, out var dateTimeValue) == true)
                    {
                        item.InternalContentsInfo = dateTimeValue;
                    }
                    if (reader.TryGetAttributeAsGuid(id, out var idValue))
                    {
                        item.InternalTableID = idValue;
                    }
                }
            }
            else
            {
                var tableNamespace = CremaSchema.TableNamespace + this.itemName.CategoryPath + this.itemName.Name;
                var dataTable = this.dataTable ?? this.dataSet.Tables[this.itemName.Name, this.itemName.CategoryPath];
                if (reader.NamespaceURI != tableNamespace)
                {

                    var name = dataTable.TemplatedParentName;
                    var user = name + CremaSchema.CreatorExtension;
                    var dateTime = name + CremaSchema.CreatedDateTimeExtension;
                    if (reader.TryGetAttributeAsSignatureDate(user, dateTime, out var dateTimeValue) == true)
                    {
                        dataTable.InternalCreationInfo = dateTimeValue;
                    }
                }
                var items = this.version < new Version(4, 0) ? EnumerableUtility.FamilyTree(dataTable, item => item.Childs) : Enumerable.Repeat(dataTable, 1);
                foreach (var item in items)
                {
                    var serializableName = item.GetXmlPath(reader.NamespaceURI);
                    var user = serializableName + CremaSchema.ModifierExtension;
                    var dateTime = serializableName + CremaSchema.ModifiedDateTimeExtension;
                    var count = serializableName + CremaSchema.CountExtension;
                    var id = serializableName + CremaSchema.IDExtension;
                    if (reader.TryGetAttributeAsInt32(count, out var countValue) == true)
                    {
                        item.MinimumCapacity = countValue;
                    }
                    if (reader.TryGetAttributeAsSignatureDate(user, dateTime, out var dateTimeValue) == true)
                    {
                        item.InternalContentsInfo = dateTimeValue;
                    }
                    if (reader.TryGetAttributeAsGuid(id, out var idValue))
                    {
                        item.InternalTableID = idValue;
                    }
                }
            }
        }

        //[Obsolete("for 2.0")]
        private void ReadHeaderInfoVersion2(XmlReader reader)
        {
            if (this.itemName != null)
            {
                var dataTable = this.dataSet.Tables[this.itemName.Name, this.itemName.CategoryPath];
                dataTable.InternalContentsInfo = reader.GetAttributeAsSignatureDate(CremaSchema.Modifier, CremaSchema.ModifiedDateTime);
            }
            else
            {
                var categoryPath = this.dataSet.GetTableCategoryPath(reader.NamespaceURI);
                var tableName = this.dataSet.GetTableName(reader.NamespaceURI);
                var dataTable = this.dataSet.Tables[tableName, categoryPath];
                dataTable.InternalContentsInfo = reader.GetAttributeAsSignatureDate(CremaSchema.Modifier, CremaSchema.ModifiedDateTime);
            }
        }

        //[Obsolete("for 2.0")]
        private void ReadModifiedDateTimeVersion2(CremaDataTable dataTable, CremaDataRow dataRow)
        {
            if (this.version >= new Version(3, 0))
                return;

            var dateTimeValue = dataRow.GetAttribute(CremaSchema.ModifiedDateTime);
            if (dateTimeValue == DBNull.Value)
                return;

            var dateTime = (DateTime)dateTimeValue;
            var userValue = dataRow.GetAttribute(CremaSchema.Modifier);
            var user = userValue is DBNull ? string.Empty : userValue as string;

            if (dateTime > dataTable.ContentsInfo.DateTime)
            {
                dataTable.InternalContentsInfo = new SignatureDate(user, dateTime);
            }
        }

        private void ReadModifyInfo(XDocument doc)
        {
            var ns = doc.Root.GetDefaultNamespace();

            foreach (var item in this.dataSet.Tables)
            {
                var xPath = null as string;
                if (item.Parent == null)
                {
                    xPath = string.Format("./*[local-name()='{0}']", item.TableName);
                }
                else
                {
                    xPath = string.Format("./*/*[local-name()='{0}']", item.TableName);
                }

                var elements = doc.Root.XPathSelectElements(xPath);
                var modifiedDateTime = DateTime.MinValue;
                var modifier = string.Empty;
                var capacity = 0;

                System.Threading.Tasks.Parallel.ForEach(elements, element =>
                {
                    capacity++;
                    if (element.HasAttributes == false)
                        return;

                    var attr1 = element.Attribute(ns + CremaSchema.ModifiedDateTime);
                    if (attr1 == null)
                    {
                        attr1 = element.Attribute(CremaSchema.ModifiedDateTime);
                        if (attr1 == null)
                            return;
                    }

                    var dateTime = XmlConvert.ToDateTime(attr1.Value, XmlDateTimeSerializationMode.Local);
                    var user = string.Empty;
                    var attr2 = element.Attribute(ns + CremaSchema.Modifier);
                    if (attr2 == null)
                        attr2 = element.Attribute(CremaSchema.Modifier);

                    if (attr2 != null)
                    {
                        user = attr2.Value;
                    }

                    lock (item)
                    {
                        if (modifiedDateTime < dateTime)
                        {
                            modifiedDateTime = dateTime;
                            modifier = user;
                        }
                    }
                });

                item.MinimumCapacity = capacity;
                item.InternalContentsInfo = new SignatureDate(modifier, modifiedDateTime);
            }
        }

        private void ReadNode(XmlReader reader, CremaDataRow parentRow)
        {
            var dataTable = this.GetTable(reader, parentRow);
            var dataRow = dataTable.NewRow();
            var relationXmls = new List<string>();

            // <a /> 는 기본값을 의미하고
            // 태그 조차도 없는것은 null값을 의미 하기 때문
            // <a /> 처럼 기본값이 있는 태그가 존재하면 read할때 한번 호출이 되기 때문에 그때 기본값을 입력하고 지금은 다 초기화 시킴
            // 3.5에서는 이부분을 수정했지만 이하 버전에서는 구분이 되어 있지 않았음.
            if (this.version >= new Version(3, 5))
            {
                foreach (var item in dataTable.Columns)
                {
                    if (item.AllowDBNull == true && object.Equals(item.DefaultValue, DBNull.Value) == false)
                        dataRow[item] = DBNull.Value;
                }
            }

            this.ReadNodeAttributes(reader, dataTable, dataRow);

            reader.MoveToContent();
            if (reader.IsEmptyElement == false)
            {
                reader.ReadStartElement();
                reader.MoveToContent();

                while (reader.NodeType == XmlNodeType.Element)
                {
                    var dataColumn = dataTable.Columns[reader.Name];
                    if (dataColumn != null)
                    {
                        this.ReadValue(reader, dataRow, dataColumn);
                    }
                    else if (this.version < this.version40 && dataTable.Childs.Contains(reader.Name) == true)
                    {
                        if (dataRow.RowState == DataRowState.Detached)
                        {
                            this.OnRowInsertAction(dataTable, dataRow);
                        }
                        this.ReadNode(reader, dataRow);
                        reader.MoveToContent();
                    }
                    else
                    {
                        reader.Skip();
                        reader.MoveToContent();
                    }
                }
                reader.MoveToContent();
                reader.ReadEndElement();
            }
            else
            {
                reader.Skip();
            }

            reader.MoveToContent();

            if (parentRow != null)
            {
                dataRow.ParentID = parentRow.RelationID;
            }

            if (dataRow.RowState == DataRowState.Detached)
            {
                this.OnRowInsertAction(dataTable, dataRow);
            }
        }

        private void ReadNodeAttributes(XmlReader reader, CremaDataTable dataTable, CremaDataRow dataRow)
        {
            for (var i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);

                var columnName = reader.Name == CremaSchemaObsolete.DataLocation ? CremaSchema.Tags : reader.Name;
                if (columnName == CremaSchema.RelationID)
                {
                    dataRow.RelationID = reader.Value;
                }
                else if (columnName == CremaSchema.ParentID)
                {
                    dataRow.ParentID = reader.Value;
                }
                else if (columnName != "xmlns")
                {
                    var dataColumn = dataTable.Attributes[columnName].InternalAttribute;
                    this.ReadValue(dataRow, dataColumn, reader.Value);
                }

                this.ReadModifiedDateTimeVersion2(dataTable, dataRow);
            }
        }

        private void ReadValue(XmlReader reader, CremaDataRow dataRow, CremaDataColumn dataColumn)
        {
            if (reader.IsEmptyElement == false)
            {
                reader.ReadStartElement();
                reader.MoveToContent();
                if (reader.HasValue == true)
                {
                    this.ReadValue(dataRow, dataColumn, reader.ReadContentAsString());
                }
                reader.MoveToContent();
                reader.ReadEndElement();
            }
            else
            {
                if (dataColumn.AllowDBNull == true && object.Equals(dataColumn.DefaultValue, DBNull.Value) == false)
                {
                    dataRow[dataColumn] = dataColumn.DefaultValue;
                }
                else if (object.Equals(dataColumn.DefaultValue, DBNull.Value) == true)
                {
                    this.ReadValue(dataRow, dataColumn, string.Empty);
                }
                reader.Skip();
            }
            reader.MoveToContent();
        }

        private void ReadValue(CremaDataRow dataRow, CremaDataColumn dataColumn, string textValue)
        {
            var dataType = dataColumn.DataType;
            var value = textValue as object;

            try
            {
                value = CremaXmlConvert.ToValue(textValue, dataType);
                if (dataColumn.CremaType != null)
                {
                    if (long.TryParse(textValue, out long v) == true)
                    {
                        value = dataColumn.CremaType.ConvertToString(v);
                    }
                }
                dataRow.SetField(dataColumn, value);
            }
            catch (Exception e)
            {
                dataRow.SetField(dataColumn, textValue);
                dataRow.SetColumnError(dataColumn, e.Message);
            }
        }

        private void ReadValue(CremaDataRow dataRow, DataColumn dataColumn, string textValue)
        {
            var dataType = dataColumn.DataType;
            var value = textValue as object;

            try
            {
                value = CremaXmlConvert.ToValue(textValue, dataType);
                dataRow.SetField(dataColumn.ColumnName, value);
            }
            catch (Exception e)
            {
                dataRow.SetField(dataColumn.ColumnName, textValue);
                dataRow.SetColumnError(dataColumn.ColumnName, e.Message);
            }
        }
    }
}