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
using System.Xml;

namespace Ntreev.Crema.Data
{
    public class CremaDataTable : IListSource, ISerializable, INotifyPropertyChanged
    {
        private readonly InternalDataTable table;
        internal readonly CremaDataRowBuilder builder;
        private readonly CremaDataColumnCollection columns;
        private readonly CremaAttributeCollection attributes;
        private readonly CremaDataRowCollection rows;
        private readonly CremaChildTableCollection childs;

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
            this.table = new InternalDataTable(this, name, categoryPath);
            this.columns = new CremaDataColumnCollection(this.table);
            this.attributes = new CremaAttributeCollection(this.table);
            this.rows = new CremaDataRowCollection(this.table);
            this.childs = new CremaChildTableCollection(this.table);

            this.AttachEventHandlers();
        }

        internal CremaDataTable(InternalDataTable table)
        {
            this.table = table;
            this.builder = new CremaDataRowBuilder(this);
            this.columns = new CremaDataColumnCollection(this.table);
            this.attributes = new CremaAttributeCollection(this.table);
            this.rows = new CremaDataRowCollection(this.table);
            this.childs = new CremaChildTableCollection(this.table);

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
            return this.Inherit(tableName, false);
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

            var schema = this.GetXmlSchema();
            var xml = copyData == true ? this.GetXml() : null;
            var tableNamespace = this.DataSet.TableNamespace + itemName.Path;
            this.DataSet.ReadXmlSchemaString(schema, itemName);
            if (xml != null)
                this.DataSet.ReadXmlString(xml, itemName);
            var dataTable = this.DataSet.Tables[itemName.Name, itemName.CategoryPath];
            var signatureDate = this.SignatureDateProvider.Provide();
            dataTable.TemplatedParent = this;
            dataTable.InternalTableID = Guid.NewGuid();
            dataTable.InternalCreationInfo = signatureDate;
            foreach (var item in dataTable.Childs)
            {
                item.TemplatedParent = this.Childs[item.TableName];
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

            var schema = this.GetXmlSchema();
            var xml = copyData == true ? this.GetXml() : null;

            if (this.DataSet != null)
            {
                this.DataSet.ReadXmlSchemaString(schema, itemName);
                if (xml != null)
                    this.DataSet.ReadXmlString(xml, itemName);

                var dataTable = this.DataSet.Tables[itemName.Name, itemName.CategoryPath];
                var signatureDate = this.SignatureDateProvider.Provide();
                dataTable.DetachTemplatedParent();
                foreach (var item in EnumerableUtility.Friends(dataTable, dataTable.childs))
                {
                    item.InternalTableID = Guid.NewGuid();
                    item.CreationInfo = signatureDate;
                }
                dataTable.UpdateRevision(this.Revision);
                return dataTable;
            }

            using (var sr = new StringReader(schema))
            using (var reader = XmlReader.Create(sr))
            {
                var dataTable = CremaDataTable.ReadSchema(reader, itemName);
                if (xml != null)
                    dataTable.ReadXmlString(xml);

                dataTable.UpdateRevision(this.Revision);
                return dataTable;
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
            this.table.BeginLoadData();
            try
            {
                this.table.ImportRow(row.InternalObject);

                foreach (var item in row.Table.childs)
                {
                    var childRows = row.GetChildRows(item);

                    var thisChild = this.childs[item.TableName];
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
                this.table.EndLoadData();
            }
        }

        public void AcceptChanges()
        {
            //if (this.Parent == null && this.Rows.Count > 0 && this.PrimaryKey.Any() == false)
            //{
            //    this.table.RefreshRelationID();
            //}
            this.table.AcceptChanges();
        }

        public bool HasChanges()
        {
            return this.HasChanges(false);
        }

        public bool HasChanges(bool isComparable)
        {
            for (var i = 0; i < this.table.Rows.Count; i++)
            {
                var dataRow = this.table.Rows[i];
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
            this.table.Clear();
        }

        public CremaDataRow[] GetErrors()
        {
            return this.table.GetErrors().Select(item => (item as InternalDataRow).Target).ToArray();
        }

        public CremaDataRow NewRow()
        {
            var dataRow = this.table.NewRow() as InternalDataRow;
            if (this.table.IsLoading == false && this.table.ColumnRelation != null)
                dataRow.RelationID = this.table.GenerateRelationID(dataRow);
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
            this.rows.Add(dataRow);
            return dataRow;
        }

        public CremaDataRow AddRow(params object[] args)
        {
            var dataRow = this.NewRow();
            for (var i = 0; i < args.Length; i++)
            {
                dataRow[i] = args[i];
            }
            this.rows.Add(dataRow);
            return dataRow;
        }

        public void UpdateRevision(long revision)
        {
            this.table.Revision = revision;
        }

        public void RejectChanges()
        {
            this.table.RejectChanges();
            //if (this.contentsInfo2 != null)
            //{
            //    this.contentsInfo = this.contentsInfo2.Value;
            //    this.contentsInfo2 = null;
            //}
        }

        public CremaDataRow[] Select()
        {
            return this.table.Select().Cast<InternalDataRow>().Select(item => item.Target).ToArray();
        }

        public CremaDataRow[] Select(string filterExpression)
        {
            return this.table.Select(filterExpression).Cast<InternalDataRow>().Select(item => item.Target).ToArray();
        }

        public CremaDataRow[] Select(string filterExpression, string sort)
        {
            return this.table.Select(filterExpression, sort).Cast<InternalDataRow>().Select(item => item.Target).ToArray();
        }

        public CremaDataRow[] Select(string filterExpression, string sort, DataViewRowState recordStates)
        {
            return this.table.Select(filterExpression, sort, recordStates).Cast<InternalDataRow>().Select(item => item.Target).ToArray();
        }

        public void WriteXml(Stream stream)
        {
            var xmlWriter = new CremaXmlWriter(this);
            xmlWriter.Write(stream);
        }

        public void WriteXml(string fileName)
        {
            var xmlWriter = new CremaXmlWriter(this);
            xmlWriter.Write(fileName);
        }

        public void WriteXml(TextWriter writer)
        {
            var xmlWriter = new CremaXmlWriter(this);
            xmlWriter.Write(writer);
        }

        public void WriteXml(XmlWriter writer)
        {
            var xmlWriter = new CremaXmlWriter(this);
            xmlWriter.Write(writer);
        }

        public void WriteXmlSchema(Stream stream)
        {
            var schemaWriter = new CremaSchemaWriter(this);
            schemaWriter.Write(stream);
        }

        public void WriteXmlSchema(string fileName)
        {
            var schemaWriter = new CremaSchemaWriter(this);
            schemaWriter.Write(fileName);
        }

        public void WriteXmlSchema(TextWriter writer)
        {
            var schemaWriter = new CremaSchemaWriter(this);
            schemaWriter.Write(writer);
        }

        public void WriteXmlSchema(XmlWriter writer)
        {
            var schemaWriter = new CremaSchemaWriter(this);
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
            using (var sw = new Utf8StringWriter())
            {
                this.WriteXml(sw);
                return sw.ToString();
            }
        }

        public string GetXmlSchema()
        {
            using (var sw = new Utf8StringWriter())
            {
                this.WriteXmlSchema(sw);
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
            get { return this.table.CreationInfo; }
            set { this.table.InternalCreationInfo = value; }
        }

        public SignatureDate ModificationInfo
        {
            get { return this.table.ModificationInfo; }
            set { this.table.InternalModificationInfo = value; }
        }

        public SignatureDate ContentsInfo
        {
            get { return this.table.ContentsInfo; }
            set { this.table.InternalContentsInfo = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CremaDataColumnCollection Columns
        {
            get { return this.columns; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CremaAttributeCollection Attributes
        {
            get { return this.attributes; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CremaDataSet DataSet
        {
            get
            {
                if (this.table.DataSet is InternalDataSet dataSet)
                    return dataSet.Target;
                return null;
            }
        }

        [Browsable(false)]
        public DataView DefaultView
        {
            get { return this.table.DefaultView; }
        }

        [Browsable(false)]
        public PropertyCollection ExtendedProperties
        {
            get { return this.table.ExtendedProperties; }
        }

        [Browsable(false)]
        public bool HasErrors
        {
            get { return this.table.HasErrors; }
        }

        [DefaultValue(50)]
        public int MinimumCapacity
        {
            get { return this.table.MinimumCapacity; }
            set { this.table.MinimumCapacity = value; }
        }

        public string Namespace
        {
            get { return this.table.Namespace; }
        }

        [Browsable(false)]
        public CremaDataRowCollection Rows
        {
            get { return this.rows; }
        }

        public string TableName
        {
            get { return this.table.LocalName; }
            set
            {
                if (this.table.LocalName == value)
                    return;
                this.table.LocalName = value;
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
                if (this.table.Parent is InternalDataTable dataTable)
                    return dataTable.Target;
                return null;
            }
            internal set
            {
                if (value != null)
                    this.table.Parent = value.InternalObject;
                else
                    this.table.Parent = null;
            }
        }

        public CremaChildTableCollection Childs
        {
            get { return this.childs; }
        }

        public string Comment
        {
            get { return this.table.Comment; }
            set { this.table.Comment = value; }
        }

        public string CategoryPath
        {
            get { return this.table.CategoryPath; }
            set { this.table.CategoryPath = value; }
        }

        public string TemplateNamespace
        {
            get { return this.table.TemplateNamespace; }
        }

        public CremaDataTable TemplatedParent
        {
            get
            {
                if (this.table.TemplatedParent is InternalDataTable templatedParent)
                    return (CremaDataTable)templatedParent;
                return null;
            }
            internal set
            {
                this.table.TemplatedParent = (InternalDataTable)value;
            }
        }

        public string ParentName
        {
            get
            {
                if (this.Parent != null)
                    return this.Parent.Name;
                return string.Empty;
            }
        }

        public string TemplatedParentName
        {
            get { return this.table.TemplatedParentName; }
        }

        public IEnumerable<CremaDataTable> DerivedTables
        {
            get
            {
                foreach (var item in this.table.DerivedItems)
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
                return this.table.PrimaryKey.Cast<InternalDataColumn>().Select(item => item.Target).ToArray();
            }
            set
            {
                if (value == null)
                {
                    this.table.PrimaryKey = null;
                }
                else
                {

                }
            }
        }

        public bool ReadOnly
        {
            get { return this.table.ReadOnly; }
            set { this.table.ReadOnly = value; }
        }

        public TagInfo Tags
        {
            get { return this.table.Tags; }
            set { this.table.Tags = value; }
        }

        public TagInfo DerivedTags
        {
            get { return this.table.DerivedTags; }
        }

        public TableInfo TableInfo
        {
            get
            {
                return this.table.TableInfo;
            }
        }

        public TableDetailInfo GetTableDetailInfo()
        {
            var cremaDataTable = this.DataSet.Tables[this.Name, this.CategoryPath].Rows;
            var columnInfos = this.TableInfo.Columns;
            var tableDetailInfo = new TableDetailInfo
            {
                Name = this.Name,
                CategoryPath = this.CategoryPath,
                TableRowsCount = cremaDataTable.Count(),
                TableEnabledRowsCount = cremaDataTable.Count(o => o.IsEnabled),
                TableDisabledRowsCount = cremaDataTable.Count(o => !o.IsEnabled),
                TableAllTagRowsCount = cremaDataTable.Count(o => o.Tags == TagInfo.All),
                TableServerTagRowsCount = cremaDataTable.Count(o => o.Tags == TagInfoUtility.Server),
                TableClientTagRowsCount = cremaDataTable.Count(o => o.Tags == TagInfoUtility.Client),
                TableUnusedTagRowsCount = cremaDataTable.Count(o => o.Tags == TagInfo.Unused),
                ColumnsCount = columnInfos.Count(),
                ColumnsAllTagCount = columnInfos.Count(o => o.Tags == TagInfo.All),
                ColumnsServerTagCount = columnInfos.Count(o => o.Tags == TagInfoUtility.Server),
                ColumnsClientTagCount = columnInfos.Count(o => o.Tags == TagInfoUtility.Client),
                ColumnsUnusedTagCount = columnInfos.Count(o => o.Tags == TagInfo.Unused)
            };
            return tableDetailInfo;
        }

        public long Revision => this.table.Revision;

        public bool IgnoreCaseSensitive
        {
            get { return this.table.IgnoreCaseSensitive; }
            set { this.table.IgnoreCaseSensitive = value; }
        }

        public SignatureDateProvider SignatureDateProvider
        {
            get { return this.table.SignatureDateProvider; }
            set { this.table.SignatureDateProvider = value; }
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

            var columns = EnumerableUtility.Friends(this, this.Childs).SelectMany(item => item.columns);

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

            var schemaWriter = GetSchemaWriter();
            var action = new Func<string>(() =>
            {
                using (var sw = new Utf8StringWriter())
                {
                    schemaWriter.Write(sw);
                    return sw.ToString();
                }
            });

            var schema = action();
            var xml = copyData == true ? this.GetXml() : null;
            var itemName = new ItemName(this.CategoryPath, this.Name);

            dataSet.ReadXmlSchemaString(schema, itemName);
            if (xml != null)
                dataSet.ReadXmlString(xml, itemName);

            var targetTable = dataSet.Tables[this.Name, this.CategoryPath];

            if (this.TemplateNamespace != string.Empty)
            {
                targetTable.AttachTemplatedParent(this.TemplateNamespace);
            }

            targetTable.UpdateRevision(this.Revision);

            return targetTable;

            CremaSchemaWriter GetSchemaWriter()
            {
                if (this.TemplatedParent != null)
                    return new CremaSchemaWriter(this.TemplatedParent);
                if (this.TemplateNamespace == string.Empty)
                    return new CremaSchemaWriter(this);
                var t1 = InternalDataSet.GetTableName((InternalDataSet)this.DataSet, this.TemplateNamespace);
                var c1 = InternalDataSet.GetTableCategoryPath((InternalDataSet)this.DataSet, this.TemplateNamespace);
                return new CremaSchemaWriter(this, new ItemName(c1, t1));
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

        private void UpdateSignatureDate(DataRowChangeEventArgs e)
        {
            var row = e.Row as InternalDataRow;
            var omitSignatureDate = this.table.OmitSignatureDate;
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
                        //if (omitSignatureDate == false)
                        //    this.ContentsInfo = this.SignatureDateProvider.Provide();
                    }
                    break;
            }
        }

        private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            this.UpdateSignatureDate(e);
            this.OnRowChanged(new CremaDataRowChangeEventArgs(e));
        }

        private void Table_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            //if (this.IsLoading == false && e.Action == DataRowAction.Add)
            //{
            //    foreach (var item in this.columns)
            //    {
            //        if (item.CremaDataType != null && item.Unique == true)
            //        {
            //            item.CremaDataType.ValidateInsertRow(item.InternalColumn, e.Row.Field<string>(item.InternalColumn));
            //        }
            //    }
            //}
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

        internal void CopyTo(CremaDataTable dest)
        {
            foreach (CremaDataColumn item in this.Columns)
            {
                item.CopyTo(dest.Columns[item.ColumnName]);
            }

            dest.CategoryPath = this.CategoryPath;
        }

        internal InternalRelation RelationColumn
        {
            get { return this.table.ColumnRelation; }
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
            if (this.Parent != null)
                return this.TableName;

            var baseNamespace = this.DataSet != null ? this.DataSet.Namespace : CremaSchema.BaseNamespace;
            if (targetNamespace == this.Namespace || targetNamespace == baseNamespace)
                return this.TableName;

            if (this.TemplateNamespace != string.Empty)
                return this.DataSet.GetTableName(this.TemplateNamespace);


            return this.TableName;
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

        public CremaDataSet Filter(CremaDataSet srcDataSet, CremaDataSet destDataSet, TagInfo tags)
        {
            CremaDataTable parentDestDataTable = null;
            if (this.Parent != null)
            {
                parentDestDataTable = new CremaDataTable(this.ParentName, this.Parent.CategoryPath);
                destDataSet.Tables.Add(parentDestDataTable);
            }

            var destDataTable = new CremaDataTable(this.TableName, this.CategoryPath);
            destDataSet.Tables.Add(destDataTable);
            destDataTable.Parent = parentDestDataTable;

            foreach (var column in this.Columns)
            {
                if ((column.DerivedTags & tags) != TagInfo.Unused)
                {
                    var newColumn = new CremaDataColumn(column.ColumnName, column.DataType);
                    destDataTable.Columns.Add(newColumn);
                }
            }

            var columns = destDataTable.Columns.ToArray();
            foreach (var row in this.Rows)
            {
                if ((row.DerivedTags & tags) == TagInfo.Unused) continue;

                destDataTable.AddRow(row.Filter(columns, tags));
            }

            return destDataSet;
        }

        internal InternalDataTable InternalObject
        {
            get { return this.table; }
        }

        internal IEnumerable<CremaDataColumnCollection> DeriviedColumns
        {
            get
            {
                foreach (var item in this.DerivedTables)
                {
                    yield return item.columns;
                }
            }
        }

        internal IEnumerable<CremaAttributeCollection> DerivedAttributes
        {
            get
            {
                foreach (var item in this.DerivedTables)
                {
                    yield return item.attributes;
                }
            }
        }

        internal CremaDataRow InvokeNewRowFromBuilder(CremaDataRowBuilder builder)
        {
            return this.NewRowFromBuilder(builder);
        }

        internal void AttachEventHandlers()
        {
            this.table.ColumnChanging += Table_ColumnChanging;
            this.table.ColumnChanged += Table_ColumnChanged;
            this.table.RowChanging += Table_RowChanging;
            this.table.RowChanged += Table_RowChanged;
            this.table.RowDeleted += Table_RowDeleted;
            this.table.RowDeleting += Table_RowDeleting;
            this.table.TableCleared += Table_TableCleared;
            this.table.TableClearing += Table_TableClearing;
            this.table.TableNewRow += Table_TableNewRow;
        }

        internal void DetachEventHandlers()
        {
            this.table.ColumnChanging -= Table_ColumnChanging;
            this.table.ColumnChanged -= Table_ColumnChanged;
            this.table.RowChanging -= Table_RowChanging;
            this.table.RowChanged -= Table_RowChanged;
            this.table.RowDeleted -= Table_RowDeleted;
            this.table.RowDeleting -= Table_RowDeleting;
            this.table.TableCleared -= Table_TableCleared;
            this.table.TableClearing -= Table_TableClearing;
            this.table.TableNewRow -= Table_TableNewRow;
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

        public void AttachTemplatedParent(CremaDataTable templatedParent)
        {
            this.TemplatedParent = templatedParent;
        }

        internal void AttachTemplatedParent(string templateNamespace)
        {
            this.table.TemplateNamespace = templateNamespace;
        }

        internal void DetachTemplatedParent()
        {
            this.table.TemplatedParent = null;
        }

        internal string InternalName
        {
            get { return this.table.InternalName; }
            set { this.table.InternalName = value; }
        }

        internal TagInfo InternalTags
        {
            get { return this.table.InternalTags; }
            set { this.table.InternalTags = value; }
        }

        internal string InternalComment
        {
            get { return this.table.InternalComment; }
            set { this.table.InternalComment = value; }
        }

        internal string InternalCategoryPath
        {
            get { return this.table.InternalCategoryPath; }
            set { this.table.InternalCategoryPath = value; }
        }

        internal SignatureDate InternalCreationInfo
        {
            get { return this.table.InternalCreationInfo; }
            set { this.table.InternalCreationInfo = value; }
        }

        internal SignatureDate InternalModificationInfo
        {
            get { return this.table.InternalModificationInfo; }
            set { this.table.InternalModificationInfo = value; }
        }

        internal SignatureDate InternalContentsInfo
        {
            get { return this.table.InternalContentsInfo; }
            set { this.table.InternalContentsInfo = value; }
        }

        internal Guid InternalTableID
        {
            get { return this.table.InternalTableID; }
            set { this.table.InternalTableID = value; }
        }

        internal bool IsDiffMode
        {
            get { return this.table.IsDiffMode; }
            set { this.table.IsDiffMode = value; }
        }

        #region IListSource

        bool IListSource.ContainsListCollection
        {
            get { return (this.table as IListSource).ContainsListCollection; }
        }

        IList IListSource.GetList()
        {
            return (this.table as IListSource).GetList();
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
