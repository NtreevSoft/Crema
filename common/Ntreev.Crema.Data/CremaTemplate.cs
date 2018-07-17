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

using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Data.Properties;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Ntreev.Crema.Data
{
    public sealed class CremaTemplate : IListSource, IXmlSerializable, INotifyPropertyChanged, IDisposable
    {
        private readonly InternalTemplate template;
        private readonly CremaTemplateColumnBuilder builder;
        private readonly CremaTemplateColumnCollection columns;
        private readonly CremaAttributeCollection attributes;

        private bool isModified;
        private bool isEventsAttached;

        public CremaTemplate()
            : this(new CremaDataTable())
        {

        }

        public CremaTemplate(CremaDataTable targetTable)
        {
            if (targetTable == null)
                throw new ArgumentNullException(nameof(targetTable));
            if (targetTable.TemplateNamespace != string.Empty)
                throw new ArgumentException(Resources.Exception_CannotEditInheritedTable, nameof(targetTable));

            this.builder = new CremaTemplateColumnBuilder(this);
            this.template = new InternalTemplate(this, this.builder) { TargetTable = (InternalDataTable)targetTable };
            this.attributes = new CremaAttributeCollection(this.template);
            this.columns = new CremaTemplateColumnCollection(this.template);

            this.AttachEventHandlers();
        }

        public static CremaTemplate Create(CremaDataSet dataSet, string tableName, string categoryPath)
        {
            var dataTable = dataSet.Tables.Add(tableName, categoryPath);
            return new CremaTemplate(dataTable);
        }

        public static CremaTemplate Create(CremaDataTable parentTable)
        {
            var dataTable = parentTable.Childs.Add();
            return new CremaTemplate(dataTable);
        }

        public CremaTemplateColumn NewColumn()
        {
            var dataRow = this.template.NewRow() as InternalTemplateColumn;
            return dataRow.Target;
        }

        public CremaTemplateColumn AddColumn(string columnName)
        {
            return this.AddColumn(columnName, typeof(string).GetTypeName(), null);
        }

        public CremaTemplateColumn AddColumn(string columnName, Type type)
        {
            return this.AddColumn(columnName, type.GetTypeName(), null);
        }

        public CremaTemplateColumn AddColumn(string columnName, Type type, string comment)
        {
            return this.AddColumn(columnName, type.GetTypeName(), comment);
        }

        public CremaTemplateColumn AddColumn(string columnName, string dataTypeName)
        {
            return this.AddColumn(columnName, dataTypeName, null);
        }

        public CremaTemplateColumn AddColumn(string columnName, string dataTypeName, string comment)
        {
            var column = (this.template.NewRow() as InternalTemplateColumn).Target;
            column.Name = columnName;
            column.DataTypeName = dataTypeName;
            column.Comment = comment;

            this.columns.Add(column);
            return column;
        }

        public void ImportColumn(CremaTemplateColumn column)
        {
            this.template.ImportRow((InternalTemplateColumn)column);
        }

        public void Dispose()
        {
            if (this.TargetTable != null)
            {
                this.DetachEventHandlers();
            }
        }

        public void AcceptChanges()
        {
            this.template.AcceptChanges();
        }

        public void RejectChanges()
        {
            this.template.RejectChanges();
        }

        public bool HasChanges()
        {
            return this.HasChanges(false);
        }

        public bool HasChanges(bool isComparable)
        {
            for (var i = 0; i < this.template.Rows.Count; i++)
            {
                var dataRow = this.template.Rows[i];
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

        internal void BeginLoadData()
        {
            this.template.BeginLoadData();
        }

        public void EndLoadData()
        {
            this.template.EndLoadData();
        }

        public void Validate()
        {
            if (this.columns.Any() == false)
                throw new CremaDataException(Resources.Exception_AtLeastOneColumn);
            if (this.columns.Any(item => item.IsKey) == false)
                throw new CremaDataException(Resources.Exception_AtLeastOneKey);
        }

        public string Comment
        {
            get { return this.TargetTable.Comment; }
            set { this.TargetTable.Comment = value; }
        }

        public string ParentName
        {
            get
            {
                if (this.TargetTable.Parent == null)
                    return string.Empty;
                return this.TargetTable.Parent.TableName;
            }
        }

        public string TableName
        {
            get { return this.TargetTable.TableName; }
            set
            {
                this.TargetTable.TableName = value;
                this.InvokePropertyChangedEvent(nameof(this.TableName), nameof(this.Name), nameof(this.Namespace));
            }
        }

        public string Name
        {
            get { return this.TargetTable.Name; }
        }

        public string Namespace
        {
            get { return this.TargetTable.Namespace; }
        }

        public string CategoryPath
        {
            get { return this.TargetTable.CategoryPath; }
            set { this.TargetTable.CategoryPath = value; }
        }

        public TagInfo Tags
        {
            get { return this.TargetTable.Tags; }
            set { this.TargetTable.Tags = value; }
        }

        public TagInfo DerivedTags
        {
            get { return this.TargetTable.DerivedTags; }
        }

        public Guid TableID
        {
            get { return this.TargetTable.TableID; }
        }

        public CremaDataTable TargetTable
        {
            get { return (CremaDataTable)this.template.TargetTable; }
            internal set
            {
                this.template.TargetTable = (InternalDataTable)value;
            }
        }

        public bool IsModified
        {
            get { return this.isModified; }
        }

        public CremaAttributeCollection Attributes
        {
            get { return this.attributes; }
        }

        public CremaTemplateColumnCollection Columns
        {
            get { return this.columns; }
        }

        public string[] Types
        {
            get { return this.template.Types; }
        }

        public DataView View
        {
            get { return this.template.DefaultView; }
        }

        public SignatureDate ModificationInfo
        {
            get { return this.TargetTable.ModificationInfo; }
        }

        public SignatureDateProvider SignatureDateProvider
        {
            get { return this.template.SignatureDateProvider; }
            set { this.template.SignatureDateProvider = value; }
        }

        public TagInfo ParentTags
        {
            get
            {
                if (this.TargetTable.Parent == null)
                    return TagInfo.All;
                return this.TargetTable.Parent.Tags;
            }
        }

        public TableInfo TableInfo
        {
            get { return this.TargetTable.TableInfo; }
        }

        public PropertyCollection ExtendedProperties
        {
            get { return this.InternalObject.ExtendedProperties; }
        }

        public bool ReadOnly
        {
            get { return this.template.ReadOnly; }
            set
            {
                this.template.ReadOnly = value;
                this.InvokePropertyChangedEvent(nameof(this.ReadOnly));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event CremaTemplateColumnChangeEventHandler ColumnChanging;

        public event CremaTemplateColumnChangeEventHandler ColumnChanged;

        public event CremaTemplateColumnChangeEventHandler ColumnDeleted;

        public event CremaTemplateColumnChangeEventHandler ColumnDeleting;

        public event CremaTemplateClearEventHandler Cleared;

        public event CremaTemplateClearEventHandler Clearing;

        public event CremaTemplateNewColumnEventHandler TemplateNewColumn;

        private CremaTemplateColumn NewTemplateColumnFromBuilder(CremaTemplateColumnBuilder builder)
        {
            return new CremaTemplateColumn(builder);
        }

        private void OnColumnChanged(CremaTemplateColumnChangeEventArgs e)
        {
            this.ColumnChanged?.Invoke(this, e);
        }

        private void OnColumnChanging(CremaTemplateColumnChangeEventArgs e)
        {
            this.ColumnChanging?.Invoke(this, e);
        }

        private void OnColumnDeleted(CremaTemplateColumnChangeEventArgs e)
        {
            this.ColumnDeleted?.Invoke(this, e);
        }

        private void OnColumnDeleting(CremaTemplateColumnChangeEventArgs e)
        {
            this.ColumnDeleting?.Invoke(this, e);
        }

        private void OnCleared(CremaTemplateClearEventArgs e)
        {
            this.Cleared?.Invoke(this, e);
        }

        private void OnClearing(CremaTemplateClearEventArgs e)
        {
            this.Clearing?.Invoke(this, e);
        }

        private void OnTemplateNewColumn(CremaTemplateNewColumnEventArgs e)
        {
            this.TemplateNewColumn?.Invoke(this, e);
        }

        private void Table_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            this.OnColumnChanged(new CremaTemplateColumnChangeEventArgs(e));
        }

        private void Table_ColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            this.OnColumnChanging(new CremaTemplateColumnChangeEventArgs(e));
        }

        private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            this.isModified = true;
            this.OnColumnChanged(new CremaTemplateColumnChangeEventArgs(e));
        }

        private void Table_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            this.OnColumnChanging(new CremaTemplateColumnChangeEventArgs(e));
        }

        private void Table_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            this.isModified = true;
            this.OnColumnDeleted(new CremaTemplateColumnChangeEventArgs(e));
        }

        private void Table_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            this.OnColumnDeleting(new CremaTemplateColumnChangeEventArgs(e));
        }

        private void Table_TableCleared(object sender, DataTableClearEventArgs e)
        {
            this.isModified = true;
            this.OnCleared(new CremaTemplateClearEventArgs(e));
        }

        private void Table_TableClearing(object sender, DataTableClearEventArgs e)
        {
            this.OnClearing(new CremaTemplateClearEventArgs(e));
        }

        private void Table_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            this.OnTemplateNewColumn(new CremaTemplateNewColumnEventArgs(e));
        }

        private void InvokePropertyChangedEvent(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        private void InvokePropertyChangedEvent(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                this.InvokePropertyChangedEvent(new PropertyChangedEventArgs(propertyName));
            }
        }

        internal CremaTemplateColumn InvokeNewTemplateColumnFromBuilder(CremaTemplateColumnBuilder builder)
        {
            return new CremaTemplateColumn(builder);
        }

        internal void AttachEventHandlers()
        {
            if (this.isEventsAttached == true)
                throw new Exception();
            this.template.ColumnChanging += Table_ColumnChanging;
            this.template.ColumnChanged += Table_ColumnChanged;
            this.template.RowChanging += Table_RowChanging;
            this.template.RowChanged += Table_RowChanged;
            this.template.RowDeleted += Table_RowDeleted;
            this.template.RowDeleting += Table_RowDeleting;
            this.template.TableCleared += Table_TableCleared;
            this.template.TableClearing += Table_TableClearing;
            this.template.TableNewRow += Table_TableNewRow;
            this.isEventsAttached = true;
        }

        internal void DetachEventHandlers()
        {
            if (this.isEventsAttached == false)
                throw new Exception();
            this.template.ColumnChanging -= Table_ColumnChanging;
            this.template.ColumnChanged -= Table_ColumnChanged;
            this.template.RowChanging -= Table_RowChanging;
            this.template.RowChanged -= Table_RowChanged;
            this.template.RowDeleted -= Table_RowDeleted;
            this.template.RowDeleting -= Table_RowDeleting;
            this.template.TableCleared -= Table_TableCleared;
            this.template.TableClearing -= Table_TableClearing;
            this.template.TableNewRow -= Table_TableNewRow;
            this.isEventsAttached = false;
        }

        internal InternalTemplate InternalObject
        {
            get { return this.template; }
        }

        internal bool IsDiffMode
        {
            get { return this.template.IsDiffMode; }
            set { this.template.IsDiffMode = value; }
        }

        internal CremaTemplateColumnCollection Items
        {
            get { return this.columns; }
        }

        #region IListSource

        bool IListSource.ContainsListCollection
        {
            get { return (this.template as IListSource).ContainsListCollection; }
        }

        System.Collections.IList IListSource.GetList()
        {
            return (this.template as IListSource).GetList();
        }

        #endregion

        #region IXmlSerializable

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var dataSet = new CremaDataSet();

            var name = reader.GetAttribute("Name");
            var categoryPath = reader.GetAttribute(nameof(CategoryPath));
            var baseNamespace = reader.GetAttribute("BaseNamespace");

            dataSet.Namespace = baseNamespace;

            reader.ReadStartElement();
            reader.MoveToContent();
            this.template.TargetTable = null;
            this.template.OmitSignatureDate = true;
            this.template.ReadXml(reader);
            this.template.OmitSignatureDate = false;
            reader.MoveToContent();
            dataSet.ReadXmlSchema(reader);
            reader.MoveToContent();
            dataSet.ReadXml(reader);
            reader.MoveToContent();
            this.template.InternalTypes = XmlSerializerUtility.Read<string[]>(reader);
            reader.MoveToContent();
            reader.ReadEndElement();

            this.template.InternalTargetTable = (InternalDataTable)dataSet.Tables[name, categoryPath];

            foreach (var item in this.template.Rows)
            {
                if (item is InternalTemplateColumn rowItem)
                {
                    if (rowItem.RowState == DataRowState.Deleted)
                        continue;
                    rowItem.TargetColumn = (InternalDataColumn)this.template.InternalTargetTable.Columns[rowItem.ColumnName];
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", this.TargetTable.Name);
            writer.WriteAttributeString("CategoryPath", this.TargetTable.CategoryPath);
            writer.WriteAttributeString("BaseNamespace", this.TargetTable.DataSet.Namespace);
            this.template.WriteXml(writer, XmlWriteMode.DiffGram);
            this.TargetTable.DataSet.WriteXmlSchema(writer);
            this.TargetTable.DataSet.WriteXml(writer);
            XmlSerializerUtility.Write(writer, this.Types);
        }

        #endregion
    }
}
