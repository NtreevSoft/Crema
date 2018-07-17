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

using Ntreev.Crema.Data.Properties;
using Ntreev.Crema.Data.Xml;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Ntreev.Crema.Data
{
    public class CremaDataTable : IListSource, ISerializable, INotifyPropertyChanged
    {
        internal readonly CremaDataRowBuilder builder;

        public CremaDataTable()
            : this(string.Empty)
        {

        }

        public CremaDataTable(string name)
            : this(name, PathUtility.Separator)
        {

        }

        public CremaDataTable(string name, string categoryPath)
        {
            this.builder = new CremaDataRowBuilder(this);
            this.InternalObject = new InternalDataTable(this, name, categoryPath);
            this.Columns = new CremaDataColumnCollection(this.InternalObject);
            this.Attributes = new CremaAttributeCollection(this.InternalObject);
            this.Rows = new CremaDataRowCollection(this.InternalObject);
            this.Childs = new CremaChildTableCollection(this.InternalObject);

            this.AttachEventHandlers();
        }

        internal CremaDataTable(InternalDataTable table)
        {
            this.InternalObject = table;
            this.builder = new CremaDataRowBuilder(this);
            this.Columns = new CremaDataColumnCollection(this.InternalObject);
            this.Attributes = new CremaAttributeCollection(this.InternalObject);
            this.Rows = new CremaDataRowCollection(this.InternalObject);
            this.Childs = new CremaChildTableCollection(this.InternalObject);

            this.AttachEventHandlers();
        }

        public static string GenerateHashValue(params ColumnInfo[] columns)
        {
            return InternalDataTable.GenerateHashValue(columns);
        }

        /// <summary>
        /// 4개 단위로 인자를 설정해야 함
        /// dataType, columnName, isKey, isUnique 순으로
        /// </summary>
        public static string GenerateHashValue(params object[] args)
        {
            return InternalDataTable.GenerateHashValue(args);
        }

        public void BeginLoad()
        {
            this.InternalObject.BeginLoad();
            this.DetachEventHandlers();
        }

        public void EndLoad()
        {
            this.AttachEventHandlers();
            this.InternalObject.EndLoad();
        }

        public void BeginLoadInternal()
        {
            this.InternalObject.BeginLoadData();
            this.DetachEventHandlers();
        }

        public void EndLoadInternal()
        {
            this.AttachEventHandlers();
            this.InternalObject.EndLoadData();
        }

        public CremaDataTable Inherit()
        {
            var tables = this.DataSet != null ? this.DataSet.Tables : Enumerable.Empty<CremaDataTable>();
            var tableName = NameUtility.GenerateNewName("DerivedTable", tables.Select(item => item.TableName));
            return this.Inherit(tableName);
        }

        public CremaDataTable Inherit(string tableName)
        {
            return this.Inherit(tableName, false);
        }

        public CremaDataTable Inherit(string tableName, string categoryPath)
        {
            return this.Inherit(tableName, categoryPath, false);
        }

        public CremaDataTable Inherit(string tableName, bool copyData)
        {
            return this.Inherit(tableName, this.CategoryPath, copyData);
        }

        public CremaDataTable Inherit(string tableName, string categoryPath, bool copyData)
        {
            return this.Inherit(new ItemName(categoryPath, tableName), copyData);
        }

        public CremaDataTable Inherit(ItemName itemName, bool copyData)
        {
            this.ValidateInerit(itemName);

            var names = StringUtility.Split(itemName.Name, '.');
            var items = EnumerableUtility.FamilyTree(this, item => item.Childs);
            foreach (var item in items)
            {
                var schema = item.GetXmlSchema();
                var xml = copyData == true ? item.GetXml() : null;
                var targetNames = StringUtility.Split(item.Name, '.');
                for (var i = 0; i < names.Length; i++)
                {
                    targetNames[i] = names[i];
                }
                var targetName = new ItemName(itemName.CategoryPath, string.Join(".", targetNames));
                this.DataSet.ReadXmlSchemaString(schema, targetName);
                if (xml != null)
                    this.DataSet.ReadXmlString(xml, targetName);
                var copiedTable = this.DataSet.Tables[targetName.Name];
                copiedTable.SourceTable = item;
            }

            var dataTable = this.DataSet.Tables[itemName.Name, itemName.CategoryPath];
            var signatureDate = this.SignatureDateProvider.Provide();
            dataTable.AttachTemplatedParent(this);
            dataTable.InternalTableID = Guid.NewGuid();
            dataTable.InternalCreationInfo = signatureDate;
            var descendants = EnumerableUtility.Descendants(dataTable, item => item.Childs);
            foreach (var item in descendants)
            {
                item.InternalTableID = Guid.NewGuid();
            }
            return dataTable;
        }

        public CremaDataTable CloneTo(CremaDataSet dataSet)
        {
            return this.CopyToCore(dataSet, false);
        }

        public CremaDataTable CopyTo(CremaDataSet dataSet)
        {
            return this.CopyToCore(dataSet, true);
        }

        public CremaDataTable Copy()
        {
            var tables = this.DataSet != null ? this.DataSet.Tables : Enumerable.Empty<CremaDataTable>();
            var tableName = NameUtility.GenerateNewName("CopiedTable", tables.Select(item => item.TableName));
            return this.Copy(tableName);
        }

        public CremaDataTable Copy(string name)
        {
            return this.Copy(name, false);
        }

        public CremaDataTable Copy(string name, string categoryPath)
        {
            return this.Copy(name, categoryPath, false);
        }

        public CremaDataTable Copy(string name, bool copyData)
        {
            return this.Copy(name, this.CategoryPath, copyData);
        }

        public CremaDataTable Copy(string name, string categoryPath, bool copyData)
        {
            return this.Copy(new ItemName(categoryPath, name), copyData);
        }

        public CremaDataTable Copy(ItemName itemName, bool copyData)
        {
            this.ValidateCopy(itemName);

            if (this.DataSet != null)
            {
                var names = StringUtility.Split(itemName.Name, '.');
                var items = EnumerableUtility.FamilyTree(this, item => item.Childs);
                foreach (var item in items)
                {
                    var schema = item.GetXmlSchema();
                    var xml = copyData == true ? item.GetXml() : null;
                    var targetNames = StringUtility.Split(item.Name, '.');
                    for (var i = 0; i < names.Length; i++)
                    {
                        targetNames[i] = names[i];
                    }
                    var targetName = new ItemName(itemName.CategoryPath, string.Join(".", targetNames));
                    this.DataSet.ReadXmlSchemaString(schema, targetName);
                    if (xml != null)
                        this.DataSet.ReadXmlString(xml, targetName);
                    var copiedTable = this.DataSet.Tables[targetName.Name];
                    copiedTable.SourceTable = item;
                }

                var dataTable = this.DataSet.Tables[itemName.Name, itemName.CategoryPath];
                var signatureDate = this.SignatureDateProvider.Provide();
                dataTable.DetachTemplatedParent();
                dataTable.InternalTableID = Guid.NewGuid();
                dataTable.InternalCreationInfo = signatureDate;
                var descendants = EnumerableUtility.Descendants(dataTable, item => item.Childs);
                foreach (var item in descendants)
                {
                    item.InternalTableID = Guid.NewGuid();
                    item.InternalCreationInfo = signatureDate;
                }
                return dataTable;
            }
            else
            {
                var schema = this.GetXmlSchema();
                var xml = copyData == true ? this.GetXml() : null;
                using (var sr = new StringReader(schema))
                using (var reader = XmlReader.Create(sr))
                {
                    var dataTable = CremaDataTable.ReadSchema(reader, itemName);
                    if (xml != null)
                        dataTable.ReadXmlString(xml);
                    return dataTable;
                }
            }
        }

        public void ReadXmlString(string xml)
        {
            this.ValidateReadXml();
            using (var reader = new StringReader(xml))
            {
                this.ReadXml(reader);
            }
        }

        public void ReadXmlSchemaString(string xmlSchema)
        {
            using (var sr = new StringReader(xmlSchema))
            {
                this.ReadXmlSchema(sr);
            }
        }

        public void ImportRow(CremaDataRow row)
        {
            this.InternalObject.BeginLoadData();
            try
            {
                this.InternalObject.ImportRow(row.InternalObject);

                foreach (var item in row.Table.Childs)
                {
                    var childRows = row.GetChildRows(item);

                    var thisChild = this.Childs[item.TableName];
                    if (thisChild == null)
                        continue;

                    foreach (var childRow in childRows)
                    {
                        thisChild.ImportRow(childRow);
                    }
                }
            }
            finally
            {
                this.InternalObject.EndLoadData();
            }
        }

        public void AcceptChanges()
        {
            this.InternalObject.AcceptChanges();
        }

        public bool HasChanges()
        {
            return this.HasChanges(false);
        }

        public bool HasChanges(bool isComparable)
        {
            for (var i = 0; i < this.InternalObject.Rows.Count; i++)
            {
                var dataRow = this.InternalObject.Rows[i];
                if (dataRow.RowState == DataRowState.Unchanged)
                    continue;
                if (isComparable == false)
                    return true;
                if (CompareDataRow(dataRow) == true)
                    return true;
            }

            return false;

            bool CompareDataRow(DataRow dataRow)
            {
                if (dataRow.HasVersion(DataRowVersion.Original) == false)
                    return false;

                for (var i = 0; i < dataRow.Table.Columns.Count; i++)
                {
                    if (object.Equals(dataRow[i], dataRow[i, DataRowVersion.Original]) == false)
                        return true;
                }
                return false;
            }
        }

        public void Clear()
        {
            foreach (var item in this.Childs)
            {
                item.Clear();
            }
            this.InternalObject.Clear();
        }

        public CremaDataRow[] GetErrors()
        {
            return this.InternalObject.GetErrors().Select(item => (item as InternalDataRow).Target).ToArray();
        }

        public CremaDataRow NewRow()
        {
            var dataRow = this.InternalObject.NewRow() as InternalDataRow;
            if (this.InternalObject.IsLoading == false && this.InternalObject.ColumnRelation != null)
                dataRow.RelationID = this.InternalObject.GenerateRelationID(dataRow);
            return dataRow.Target;
        }

        public CremaDataRow NewRow(CremaDataRow parentRow)
        {
            if (parentRow == null)
                throw new ArgumentNullException(nameof(parentRow));
            if (this.Parent == null)
                throw new CremaDataException(Resources.Exception_UsableOnChildTable);
            var newRow = this.NewRow();
            newRow.ParentID = parentRow.RelationID;
            return newRow;
        }

        public CremaDataRow AddRow(CremaDataRow parent, params object[] args)
        {
            var dataRow = this.NewRow(parent);
            for (var i = 0; i < args.Length; i++)
            {
                dataRow[i] = args[i];
            }
            this.Rows.Add(dataRow);
            return dataRow;
        }

        public CremaDataRow AddRow(params object[] args)
        {
            var dataRow = this.NewRow();
            for (var i = 0; i < args.Length; i++)
            {
                dataRow[i] = args[i];
            }
            this.Rows.Add(dataRow);
            return dataRow;
        }

        public void RejectChanges()
        {
            this.InternalObject.RejectChanges();
        }

        public CremaDataRow[] Select()
        {
            return this.InternalObject.Select().Cast<InternalDataRow>().Select(item => item.Target).ToArray();
        }

        public CremaDataRow[] Select(string filterExpression)
        {
            return this.InternalObject.Select(filterExpression).Cast<InternalDataRow>().Select(item => item.Target).ToArray();
        }

        public CremaDataRow[] Select(string filterExpression, string sort)
        {
            return this.InternalObject.Select(filterExpression, sort).Cast<InternalDataRow>().Select(item => item.Target).ToArray();
        }

        public CremaDataRow[] Select(string filterExpression, string sort, DataViewRowState recordStates)
        {
            return this.InternalObject.Select(filterExpression, sort, recordStates).Cast<InternalDataRow>().Select(item => item.Target).ToArray();
        }

        public void WriteXml(Stream stream)
        {
            this.WriteXml(stream, false);
        }

        public void WriteXml(Stream stream, bool isRecursive)
        {
            var xmlWriter = new CremaXmlWriter(this)
            {
                IsRecursive = isRecursive,
            };
            xmlWriter.Write(stream);
        }

        public void WriteXml(string filename)
        {
            this.WriteXml(filename, false);
        }

        public void WriteXml(string filename, bool isRecursive)
        {
            var xmlWriter = new CremaXmlWriter(this)
            {
                IsRecursive = isRecursive,
            };
            xmlWriter.Write(filename);
        }

        public void WriteXml(TextWriter writer)
        {
            this.WriteXml(writer, false);
        }

        public void WriteXml(TextWriter writer, bool isRecursive)
        {
            var xmlWriter = new CremaXmlWriter(this)
            {
                IsRecursive = isRecursive,
            };
            xmlWriter.Write(writer);
        }

        public void WriteXml(XmlWriter writer)
        {
            this.WriteXml(writer, false);
        }

        public void WriteXml(XmlWriter writer, bool isRecursive)
        {
            var xmlWriter = new CremaXmlWriter(this)
            {
                IsRecursive = isRecursive,
            };
            xmlWriter.Write(writer);
        }

        public void WriteXmlSchema(Stream stream)
        {
            this.WriteXmlSchema(stream, false);
        }

        public void WriteXmlSchema(Stream stream, bool isRecursive)
        {
            var schemaWriter = new CremaSchemaWriter(this)
            {
                IsRecursive = isRecursive,
            };
            schemaWriter.Write(stream);
        }

        public void WriteXmlSchema(string filename)
        {
            this.WriteXmlSchema(filename, false);
        }

        public void WriteXmlSchema(string filename, bool isRecursive)
        {
            var schemaWriter = new CremaSchemaWriter(this)
            {
                IsRecursive = isRecursive,
            };
            schemaWriter.Write(filename);
        }

        public void WriteXmlSchema(TextWriter writer)
        {
            this.WriteXmlSchema(writer, false);
        }

        public void WriteXmlSchema(TextWriter writer, bool isRecursvie)
        {
            var schemaWriter = new CremaSchemaWriter(this)
            {
                IsRecursive = isRecursvie,
            };
            schemaWriter.Write(writer);
        }

        public void WriteXmlSchema(XmlWriter writer)
        {
            this.WriteXmlSchema(writer, false);
        }

        public void WriteXmlSchema(XmlWriter writer, bool isRecursive)
        {
            var schemaWriter = new CremaSchemaWriter(this)
            {
                IsRecursive = isRecursive,
            };
            schemaWriter.Write(writer);
        }

        public void ReadXml(Stream stream)
        {
            var xmlReader = new CremaXmlReader(this);
            xmlReader.Read(stream);
        }

        public void ReadXml(string fileName)
        {
            var xmlReader = new CremaXmlReader(this);
            xmlReader.Read(fileName);
        }

        public void ReadXml(TextReader writer)
        {
            var xmlReader = new CremaXmlReader(this);
            xmlReader.Read(writer);
        }

        public void ReadXml(XmlReader writer)
        {
            var xmlReader = new CremaXmlReader(this);
            xmlReader.Read(writer);
        }

        public void ReadXmlSchema(Stream stream)
        {
            var itemName = this.TableName == string.Empty ? null : new ItemName(this.TableName, this.CategoryPath);
            var schemaReader = new CremaSchemaReader(this, itemName);
            schemaReader.Read(stream);
        }

        public void ReadXmlSchema(string fileName)
        {
            var itemName = this.TableName == string.Empty ? null : new ItemName(this.TableName, this.CategoryPath);
            var schemaReader = new CremaSchemaReader(this, itemName);
            schemaReader.Read(fileName);
        }

        public void ReadXmlSchema(TextReader writer)
        {
            var itemName = this.TableName == string.Empty ? null : new ItemName(this.TableName, this.CategoryPath);
            var schemaReader = new CremaSchemaReader(this, itemName);
            schemaReader.Read(writer);
        }

        public void ReadXmlSchema(XmlReader writer)
        {
            var itemName = this.TableName == string.Empty ? null : new ItemName(this.TableName, this.CategoryPath);
            var schemaReader = new CremaSchemaReader(this, itemName);
            schemaReader.Read(writer);
        }

        public string GetXml()
        {
            return this.GetXml(false);
        }

        public string GetXml(bool isRecursive)
        {
            using (var sw = new Utf8StringWriter())
            {
                this.WriteXml(sw, isRecursive);
                return sw.ToString();
            }
        }

        public string GetXmlSchema()
        {
            return this.GetXmlSchema(false);
        }

        public string GetXmlSchema(bool isRecursive)
        {
            using (var sw = new Utf8StringWriter())
            {
                this.WriteXmlSchema(sw, isRecursive);
                return sw.ToString();
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public static CremaDataTable ReadSchema(XmlReader reader)
        {
            var dataTable = new CremaDataTable();
            var schemaReader = new CremaSchemaReader(dataTable, null);
            schemaReader.Read(reader);
            return dataTable;
        }

        public static CremaDataTable ReadSchema(XmlReader reader, ItemName itemName)
        {
            var dataTable = new CremaDataTable();
            var schemaReader = new CremaSchemaReader(dataTable, itemName);
            schemaReader.Read(reader);
            return dataTable;
        }

        public SignatureDate CreationInfo
        {
            get { return this.InternalObject.CreationInfo; }
            set { this.InternalObject.InternalCreationInfo = value; }
        }

        public SignatureDate ModificationInfo
        {
            get { return this.InternalObject.ModificationInfo; }
            set { this.InternalObject.InternalModificationInfo = value; }
        }

        public SignatureDate ContentsInfo
        {
            get { return this.InternalObject.ContentsInfo; }
            set { this.InternalObject.InternalContentsInfo = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CremaDataColumnCollection Columns { get; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CremaAttributeCollection Attributes { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CremaDataSet DataSet
        {
            get
            {
                if (this.InternalObject.DataSet is InternalDataSet dataSet)
                    return dataSet.Target;
                return null;
            }
        }

        [Browsable(false)]
        public DataView DefaultView
        {
            get { return this.InternalObject.DefaultView; }
        }

        [Browsable(false)]
        public PropertyCollection ExtendedProperties
        {
            get { return this.InternalObject.ExtendedProperties; }
        }

        [Browsable(false)]
        public bool HasErrors
        {
            get { return this.InternalObject.HasErrors; }
        }

        [DefaultValue(50)]
        public int MinimumCapacity
        {
            get { return this.InternalObject.MinimumCapacity; }
            set { this.InternalObject.MinimumCapacity = value; }
        }

        public string Namespace
        {
            get { return this.InternalObject.Namespace; }
        }

        [Browsable(false)]
        public CremaDataRowCollection Rows { get; }

        public string TableName
        {
            get { return this.InternalObject.LocalName; }
            set
            {
                if (this.InternalObject.LocalName == value)
                    return;
                this.InternalObject.LocalName = value;
                this.InvokePropertyChangedEvent(nameof(this.TableName), nameof(this.Name), nameof(this.CategoryPath), nameof(this.Namespace));
            }
        }

        public Guid TableID
        {
            get { return this.InternalTableID; }
        }

        public CremaDataTable Parent
        {
            get
            {
                if (this.InternalObject.Parent is InternalDataTable dataTable)
                    return dataTable.Target;
                return null;
            }
            internal set
            {
                if (value != null)
                    this.InternalObject.Parent = value.InternalObject;
                else
                    this.InternalObject.Parent = null;
            }
        }

        public CremaChildTableCollection Childs { get; }

        public string Comment
        {
            get { return this.InternalObject.Comment; }
            set { this.InternalObject.Comment = value; }
        }

        public string CategoryPath
        {
            get { return this.InternalObject.CategoryPath; }
            set { this.InternalObject.CategoryPath = value; }
        }

        public string TemplateNamespace
        {
            get { return this.InternalObject.TemplateNamespace; }
        }

        public CremaDataTable TemplatedParent
        {
            get
            {
                if (this.InternalObject.TemplatedParent is InternalDataTable templatedParent)
                    return (CremaDataTable)templatedParent;
                return null;
            }
            internal set
            {
                this.InternalObject.TemplatedParent = (InternalDataTable)value;
            }
        }

        public string ParentName
        {
            get { return this.InternalObject.ParentName; }
        }

        public string TemplatedParentName
        {
            get { return this.InternalObject.TemplatedParentName; }
        }

        public CremaDataTable SourceTable
        {
            get; private set;
        }

        public IEnumerable<CremaDataTable> DerivedTables
        {
            get
            {
                foreach (var item in this.InternalObject.DerivedItems)
                {
                    if (item is InternalDataTable dataTable)
                        yield return dataTable.Target;
                }
            }
        }

        public string Name
        {
            get { return this.InternalName; }
        }

        public CremaDataColumn[] PrimaryKey
        {
            get
            {
                return this.InternalObject.PrimaryKey.Cast<InternalDataColumn>().Select(item => item.Target).ToArray();
            }
            set
            {
                if (value == null)
                {
                    this.InternalObject.PrimaryKey = null;
                }
                else
                {

                }
            }
        }

        public bool ReadOnly
        {
            get { return this.InternalObject.ReadOnly; }
            set { this.InternalObject.ReadOnly = value; }
        }

        public TagInfo Tags
        {
            get { return this.InternalObject.Tags; }
            set { this.InternalObject.Tags = value; }
        }

        public TagInfo DerivedTags
        {
            get { return this.InternalObject.DerivedTags; }
        }

        public TableInfo TableInfo
        {
            get
            {
                return this.InternalObject.TableInfo;
            }
        }

        public SignatureDateProvider SignatureDateProvider
        {
            get { return this.InternalObject.SignatureDateProvider; }
            set { this.InternalObject.SignatureDateProvider = value; }
        }

        public string SchemaHashValue { get; internal set; }

        public string ContentHashValue { get; internal set; }

        public static string GenerateName(string parentTableName, string tableName)
        {
            if (string.IsNullOrEmpty(parentTableName) == true)
                return tableName;
            return string.Format("{0}.{1}", parentTableName, tableName);
        }

        public static string GetTableName(string name)
        {
            return NameUtility.GetName(name);
        }

        public static string GetParentName(string name)
        {
            return NameUtility.GetParentName(name);
        }

        public event CremaDataColumnChangeEventHandler ColumnChanged;

        public event CremaDataColumnChangeEventHandler ColumnChanging;

        public event CremaDataRowChangeEventHandler RowChanging;

        public event CremaDataRowChangeEventHandler RowChanged;

        public event CremaDataRowChangeEventHandler RowDeleted;

        public event CremaDataRowChangeEventHandler RowDeleting;

        public event CremaDataTableClearEventHandler TableCleared;

        public event CremaDataTableClearEventHandler TableClearing;

        public event CremaDataTableNewRowEventHandler TableNewRow;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual CremaDataRow NewRowFromBuilder(CremaDataRowBuilder builder)
        {
            return new CremaDataRow(builder);
        }

        protected virtual void OnColumnChanged(CremaDataColumnChangeEventArgs e)
        {
            this.ColumnChanged?.Invoke(this, e);
        }

        protected virtual void OnColumnChanging(CremaDataColumnChangeEventArgs e)
        {
            this.ColumnChanging?.Invoke(this, e);
        }

        protected virtual void OnRowChanged(CremaDataRowChangeEventArgs e)
        {
            this.RowChanged?.Invoke(this, e);
        }

        protected virtual void OnRowChanging(CremaDataRowChangeEventArgs e)
        {
            this.RowChanging?.Invoke(this, e);
        }

        protected virtual void OnRowDeleted(CremaDataRowChangeEventArgs e)
        {
            this.RowDeleted?.Invoke(this, e);
        }

        protected virtual void OnRowDeleting(CremaDataRowChangeEventArgs e)
        {
            this.RowDeleting?.Invoke(this, e);
        }

        protected virtual void OnTableCleared(CremaDataTableClearEventArgs e)
        {
            this.TableCleared?.Invoke(this, e);
        }

        protected virtual void OnTableClearing(CremaDataTableClearEventArgs e)
        {
            this.TableClearing?.Invoke(this, e);
        }

        protected virtual void OnTableNewRow(CremaDataTableNewRowEventArgs e)
        {
            this.TableNewRow?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        private void ValidateReadXmlSchema()
        {

        }

        private void ValidateReadXml()
        {
            if (this.Rows.Count > 0)
                throw new CremaDataException();
        }

        private void InvokePropertyChangedEvent(params string[] propertyNames)
        {
            var modificationInfo = this.SignatureDateProvider.Provide();

            var tables = EnumerableUtility.Friends(this, this.DerivedTables);
            foreach (var item in tables)
            {
                item.InternalModificationInfo = modificationInfo;
            }

            foreach (var table in tables)
            {
                foreach (var propertyName in propertyNames)
                {
                    table.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        private CremaDataTable CopyToCore(CremaDataSet dataSet, bool copyData)
        {
            this.ValidateCopyTo(dataSet);

            var columns = EnumerableUtility.Friends(this, this.Childs).SelectMany(item => item.Columns);

            foreach (var item in columns)
            {
                var dataType = item.CremaType;
                if (dataType == null)
                    continue;

                if (dataSet.Types.Contains(dataType.TypeName, dataType.CategoryPath) == false)
                {
                    dataType.CopyTo(dataSet);
                }
            }

            var itemName = new ItemName(this.CategoryPath, this.Name);
            var items = EnumerableUtility.Friends(this, this.Childs);
            foreach (var item in items)
            {
                var schemaWriter = GetSchemaWriter(item);
                var action = new Func<string>(() =>
                {
                    using (var sw = new Utf8StringWriter())
                    {
                        schemaWriter.Write(sw);
                        return sw.ToString();
                    }
                });

                var schema = action();
                var xml = copyData == true ? item.GetXml() : null;
                var targetNames = StringUtility.Split(item.Name, '.');
                //for (var i = 0; i < names.Length; i++)
                //{
                //    targetNames[i] = names[i];
                //}
                var targetName = new ItemName(itemName.CategoryPath, string.Join(".", targetNames));
                dataSet.ReadXmlSchemaString(schema, targetName);
                if (xml != null)
                    dataSet.ReadXmlString(xml, targetName);
            }

            //var schema = action();
            //var xml = copyData == true ? this.GetXml() : null;
            //var itemName = new ItemName(this.CategoryPath, this.Name);

            //dataSet.ReadXmlSchemaString(schema, itemName);
            //if (xml != null)
            //    dataSet.ReadXmlString(xml, itemName);

            var targetTable = dataSet.Tables[this.Name, this.CategoryPath];

            if (this.TemplateNamespace != string.Empty)
            {
                targetTable.AttachTemplatedParent(this.TemplateNamespace);
            }

            return targetTable;

            CremaSchemaWriter GetSchemaWriter(CremaDataTable dataTable)
            {
                if (dataTable.TemplatedParent != null)
                    return new CremaSchemaWriter(dataTable.TemplatedParent);
                if (dataTable.TemplateNamespace == string.Empty)
                    return new CremaSchemaWriter(this);
                var t1 = InternalDataSet.GetTableName((InternalDataSet)dataTable.DataSet, dataTable.TemplateNamespace);
                var c1 = InternalDataSet.GetTableCategoryPath((InternalDataSet)dataTable.DataSet, dataTable.TemplateNamespace);
                return new CremaSchemaWriter(dataTable, new ItemName(c1, t1));
            }
        }

        private void UpdateSignatureDate(DataRowChangeEventArgs e)
        {
            var row = e.Row as InternalDataRow;
            var omitSignatureDate = this.InternalObject.OmitSignatureDate;
            var action = e.Action;

            switch (action)
            {
                case DataRowAction.Add:
                    {
                        if (omitSignatureDate == false)
                            this.InternalContentsInfo = row.ModificationInfo;

                    }
                    break;
                case DataRowAction.Commit:
                    {

                    }
                    break;
                case DataRowAction.Change:
                    {
                        if (omitSignatureDate == false)
                            this.InternalContentsInfo = row.ModificationInfo;
                    }
                    break;
                case DataRowAction.Delete:
                    {
                        if (omitSignatureDate == false)
                            this.InternalContentsInfo = this.SignatureDateProvider.Provide();
                    }
                    break;
                case DataRowAction.Rollback:
                    {

                    }
                    break;
            }
        }

        private void Table_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnMapping == MappingType.Element)
            {
                this.OnColumnChanged(new CremaDataColumnChangeEventArgs(e));
            }
        }

        private void Table_ColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnMapping == MappingType.Element)
            {
                this.OnColumnChanging(new CremaDataColumnChangeEventArgs(e));
            }
        }

        private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            this.UpdateSignatureDate(e);
            this.OnRowChanged(new CremaDataRowChangeEventArgs(e));
        }

        private void Table_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            this.OnRowChanging(new CremaDataRowChangeEventArgs(e));
        }

        private void Table_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            this.OnRowDeleted(new CremaDataRowChangeEventArgs(e));
        }

        private void Table_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            this.OnRowDeleting(new CremaDataRowChangeEventArgs(e));
        }

        private void Table_TableCleared(object sender, DataTableClearEventArgs e)
        {
            this.OnTableCleared(new CremaDataTableClearEventArgs(e));
        }

        private void Table_TableClearing(object sender, DataTableClearEventArgs e)
        {
            this.OnTableClearing(new CremaDataTableClearEventArgs(e));
        }

        private void Table_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            this.OnTableNewRow(new CremaDataTableNewRowEventArgs(e));
        }

        private void ValidateCopyTo(CremaDataSet dataSet)
        {
            if (this.DataSet == dataSet)
                throw new CremaDataException(Resources.Exception_CannotCopyToSameDataSet);

            if (dataSet.Tables.Contains(this.TableName, this.CategoryPath) == true)
                throw new CremaDataException(this.TableName);
        }

        private void ValidateInerit(ItemName itemName)
        {
            if (itemName == null)
                throw new ArgumentNullException(nameof(itemName));
            CremaDataSet.ValidateName(itemName.Name);

            if (this.DataSet == null)
                throw new InvalidOperationException(Resources.Exception_CannotInheritWithoutDataSet);
            if (this.DataSet != null && this.DataSet.Tables.Contains(itemName.Name, itemName.CategoryPath) == true)
                throw new ArgumentException(string.Format(Resources.Exception_AlreadyExistedItem_Format, itemName));
        }

        private void ValidateCopy(ItemName itemName)
        {
            if (itemName == null)
                throw new ArgumentNullException(nameof(itemName));

            var items = StringUtility.Split(itemName.Name, '.');
            foreach (var item in items)
            {
                CremaDataSet.ValidateName(item);
            }

            if (this.DataSet != null && this.DataSet.Tables.Contains(itemName.Name, itemName.CategoryPath) == true)
                throw new ArgumentException(string.Format(Resources.Exception_AlreadyExistedItem_Format, itemName));
        }

        internal void CopyTo(CremaDataTable dest)
        {
            foreach (CremaDataColumn item in this.Columns)
            {
                item.CopyTo(dest.Columns[item.ColumnName]);
            }

            dest.CategoryPath = this.CategoryPath;
        }

        internal InternalRelation ColumnRelation
        {
            get { return this.InternalObject.ColumnRelation; }
        }

        internal InternalRelation ParentRelation
        {
            get { return this.InternalObject.ParentRelation; }
        }

        internal string TableTypeName
        {
            get { return CremaSchema.GenerateTableTypeName(this.Name); }
        }

        internal string TemplateTypeName
        {
            get
            {
                if (this.TemplateNamespace == string.Empty)
                    throw new CremaDataException();

                var templateName = CremaDataSet.GetTableName(this.DataSet, this.TemplateNamespace);
                return CremaSchema.GenerateTableTypeName(templateName);
            }
        }

        internal string UniqueTypeName
        {
            get { return this.Name + CremaSchema.UniqueTypeNameExtension; }
        }

        internal string KeyTypeName
        {
            get { return this.Name + CremaSchema.KeyTypeNameExtension; }
        }

        internal string GetXmlName(string targetNamespace)
        {
            if (this.Namespace == targetNamespace)
                return this.Name;
            //if (this.Parent != null)
            //    return this.Name;

            var baseNamespace = this.DataSet != null ? this.DataSet.Namespace : CremaSchema.BaseNamespace;
            if (targetNamespace == this.Namespace || targetNamespace == baseNamespace)
                return this.Name;

            if (this.TemplateNamespace != string.Empty)
                return this.DataSet.GetTableName(this.TemplateNamespace);

            return this.Name;
        }

        internal string GetXmlPath(string targetNamespace)
        {
            var baseNamespace = this.DataSet != null ? this.DataSet.Namespace : CremaSchema.BaseNamespace;
            if (targetNamespace == this.Namespace || targetNamespace == baseNamespace)
                return this.Name;

            if (this.TemplateNamespace != string.Empty)
            {
                if (this.Parent == null)
                    return CremaDataSet.GetTableName(this.DataSet, this.TemplateNamespace);
                return CremaDataSet.GetTableName(this.DataSet, this.TemplateNamespace);
            }

            return this.Name;
        }

        internal string GetSelectorXPath(string prefix, string targetNamespace)
        {
            if (this.Namespace == targetNamespace)
            {
                return CremaSchema.GenerateXPath(prefix, this.Name);
            }
            if (this.Parent != null)
            {
                return CremaSchema.GenerateXPath(prefix, this.Parent.GetXmlName(targetNamespace), this.TableName);
            }
            else if (this.TemplateNamespace == string.Empty)
            {
                return CremaSchema.GenerateXPath(prefix, this.TableName);
            }
            return CremaSchema.GenerateXPath(prefix, this.GetXmlName(targetNamespace));
        }

        internal InternalDataTable InternalObject { get; }

        internal IEnumerable<CremaDataColumnCollection> DeriviedColumns
        {
            get
            {
                foreach (var item in this.DerivedTables)
                {
                    yield return item.Columns;
                }
            }
        }

        internal IEnumerable<CremaAttributeCollection> DerivedAttributes
        {
            get
            {
                foreach (var item in this.DerivedTables)
                {
                    yield return item.Attributes;
                }
            }
        }

        internal CremaDataRow InvokeNewRowFromBuilder(CremaDataRowBuilder builder)
        {
            return this.NewRowFromBuilder(builder);
        }

        internal void AttachEventHandlers()
        {
            this.InternalObject.ColumnChanging += Table_ColumnChanging;
            this.InternalObject.ColumnChanged += Table_ColumnChanged;
            this.InternalObject.RowChanging += Table_RowChanging;
            this.InternalObject.RowChanged += Table_RowChanged;
            this.InternalObject.RowDeleted += Table_RowDeleted;
            this.InternalObject.RowDeleting += Table_RowDeleting;
            this.InternalObject.TableCleared += Table_TableCleared;
            this.InternalObject.TableClearing += Table_TableClearing;
            this.InternalObject.TableNewRow += Table_TableNewRow;
        }

        internal void DetachEventHandlers()
        {
            this.InternalObject.ColumnChanging -= Table_ColumnChanging;
            this.InternalObject.ColumnChanged -= Table_ColumnChanged;
            this.InternalObject.RowChanging -= Table_RowChanging;
            this.InternalObject.RowChanged -= Table_RowChanged;
            this.InternalObject.RowDeleted -= Table_RowDeleted;
            this.InternalObject.RowDeleting -= Table_RowDeleting;
            this.InternalObject.TableCleared -= Table_TableCleared;
            this.InternalObject.TableClearing -= Table_TableClearing;
            this.InternalObject.TableNewRow -= Table_TableNewRow;
        }

        internal void AttachTemplatedParent(CremaDataTable templatedParent)
        {
            this.TemplatedParent = templatedParent;
        }

        internal void AttachTemplatedParent(string templateNamespace)
        {
            this.InternalObject.TemplateNamespace = templateNamespace;
        }

        internal void DetachTemplatedParent()
        {
            this.InternalObject.TemplatedParent = null;
        }

        internal void CreateRelationColumn()
        {
            this.InternalObject.CreateRelationColumn();
        }

        internal void CreateParentColumn()
        {
            this.InternalObject.CreateParentColumn();
        }

        internal string InternalName
        {
            get { return this.InternalObject.InternalName; }
            set { this.InternalObject.InternalName = value; }
        }

        internal TagInfo InternalTags
        {
            get { return this.InternalObject.InternalTags; }
            set { this.InternalObject.InternalTags = value; }
        }

        internal string InternalComment
        {
            get { return this.InternalObject.InternalComment; }
            set { this.InternalObject.InternalComment = value; }
        }

        internal string InternalCategoryPath
        {
            get { return this.InternalObject.InternalCategoryPath; }
            set { this.InternalObject.InternalCategoryPath = value; }
        }

        internal SignatureDate InternalCreationInfo
        {
            get { return this.InternalObject.InternalCreationInfo; }
            set { this.InternalObject.InternalCreationInfo = value; }
        }

        internal SignatureDate InternalModificationInfo
        {
            get { return this.InternalObject.InternalModificationInfo; }
            set { this.InternalObject.InternalModificationInfo = value; }
        }

        internal SignatureDate InternalContentsInfo
        {
            get { return this.InternalObject.InternalContentsInfo; }
            set { this.InternalObject.InternalContentsInfo = value; }
        }

        internal Guid InternalTableID
        {
            get { return this.InternalObject.InternalTableID; }
            set { this.InternalObject.InternalTableID = value; }
        }

        internal bool IsDiffMode
        {
            get { return this.InternalObject.IsDiffMode; }
            set { this.InternalObject.IsDiffMode = value; }
        }

        #region IListSource

        bool IListSource.ContainsListCollection
        {
            get { return (this.InternalObject as IListSource).ContainsListCollection; }
        }

        IList IListSource.GetList()
        {
            return (this.InternalObject as IListSource).GetList();
        }

        #endregion

        #region ISerializable

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
