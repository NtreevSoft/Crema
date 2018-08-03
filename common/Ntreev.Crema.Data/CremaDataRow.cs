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
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Data
{
    public class CremaDataRow
    {
        public CremaDataRow(CremaDataRowBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException();
            this.InternalObject = builder.DataRow;
        }

        public void AcceptChanges()
        {
            this.InternalObject.AcceptChanges();
        }

        public void BeginEdit()
        {
            this.InternalObject.BeginEdit();
        }

        public void CancelEdit()
        {
            this.InternalObject.CancelEdit();
        }

        public void ClearErrors()
        {
            this.InternalObject.ClearErrors();
        }

        public void Delete()
        {
            this.InternalObject.Delete();
        }

        public void EndEdit()
        {
            this.InternalObject.EndEdit();
        }

        public CremaDataRow[] GetChildRows(CremaDataTable childTable)
        {
            if (childTable.Parent != this.Table)
                return null;

            var relationName = InternalSetBase.GenerateRelationName(childTable.InternalObject);

            return this.InternalObject.GetChildRows(relationName).Select(item => (item as InternalDataRow).Target).ToArray();
        }

        public CremaDataRow[] GetChildRows(string childTableName)
        {
            var relationName = InternalSetBase.GenerateRelationName(childTableName, this.Table.Namespace);

            return this.InternalObject.GetChildRows(relationName).Select(item => (item as InternalDataRow).Target).ToArray();
        }

        public string GetColumnError(CremaDataColumn cremaColumn)
        {
            return this.InternalObject.GetColumnError(cremaColumn.InternalObject);
        }

        public string GetColumnError(int columnIndex)
        {
            return this.InternalObject.GetColumnError(columnIndex);
        }

        public string GetColumnError(string columnName)
        {
            return this.InternalObject.GetColumnError(columnName);
        }

        public CremaDataColumn[] GetColumnsInError()
        {
            return this.InternalObject.GetColumnsInError().Select(item =>
            {
                var column = item as InternalColumnBase;
                return column.Target as CremaDataColumn;
            }).ToArray();
        }

        public bool HasVersion(DataRowVersion version)
        {
            return this.InternalObject.HasVersion(version);
        }

        public bool IsNull(CremaDataColumn column)
        {
            return this.InternalObject.IsNull(column.InternalObject);
        }

        public bool IsNull(int columnIndex)
        {
            return this.InternalObject.IsNull(columnIndex);
        }

        public bool IsNull(string columnName)
        {
            return this.InternalObject.IsNull(columnName);
        }

        public bool IsNull(CremaDataColumn column, DataRowVersion version)
        {
            return this.InternalObject.IsNull(column.InternalObject, version);
        }

        public void RejectChanges()
        {
            this.InternalObject.RejectChanges();
        }

        public void SetAdded()
        {
            this.InternalObject.SetAdded();
        }

        public void SetColumnError(CremaDataColumn column, string error)
        {
            this.InternalObject.SetColumnError(column.InternalObject, error);
        }

        public void SetColumnError(int columnIndex, string error)
        {
            this.InternalObject.SetColumnError(columnIndex, error);
        }

        public void SetColumnError(string columnName, string error)
        {
            this.InternalObject.SetColumnError(columnName, error);
        }

        public void SetAttributeError(CremaAttribute attribute, string error)
        {
            this.InternalObject.SetColumnError(attribute.InternalAttribute, error);
        }

        public void SetAttributeError(string attributeName, string error)
        {
            this.InternalObject.SetColumnError(attributeName, error);
        }

        public void SetModified()
        {
            this.InternalObject.SetModified();
        }

        public object this[CremaDataColumn column]
        {
            get => this.InternalObject[column.InternalObject];
            set => this.InternalObject[column.InternalObject] = value;
        }

        public object this[CremaDataColumn column, DataRowVersion version] => this.InternalObject[column.InternalObject, version];

        public object this[int columnIndex]
        {
            get => this.InternalObject[this.Table.Columns[columnIndex].InternalObject];
            set => this.InternalObject[this.Table.Columns[columnIndex].InternalObject] = value;
        }

        public object this[int columnIndex, DataRowVersion version] => this.InternalObject[this.Table.Columns[columnIndex].InternalObject, version];

        public object this[string columnName]
        {
            get
            {
                if (this.Table.Columns.Contains(columnName) == false)
                    throw new CremaDataException(string.Format(Resources.Exception_NotFoundColumn_Format, columnName));
                return this.InternalObject[columnName];
            }
            set
            {
                if (this.Table.Columns.Contains(columnName) == false)
                    throw new CremaDataException(string.Format(Resources.Exception_NotFoundColumn_Format, columnName));
                this.InternalObject[columnName] = value;
            }
        }

        public object this[string columnName, DataRowVersion version]
        {
            get
            {
                if (this.Table.Columns.Contains(columnName) == false)
                    throw new CremaDataException(string.Format(Resources.Exception_NotFoundColumn_Format, columnName));
                return this.InternalObject[columnName, version];
            }
        }

        public object GetAttribute(string attributeName)
        {
            if (this.Table.Attributes.Contains(attributeName) == false)
                throw new CremaDataException(string.Format(Resources.Exception_NotFoundAttribute_Format, attributeName));
            return this.InternalObject[attributeName];
        }

        public void SetAttribute(string attributeName, object value)
        {
            if (this.Table.Attributes.Contains(attributeName) == false)
                throw new CremaDataException(string.Format(Resources.Exception_NotFoundAttribute_Format, attributeName));
            this.InternalObject[attributeName] = value;
        }

        public IDictionary<string, object> ToDictionary()
        {
            var dataTable = this.Table;
            var props = new Dictionary<string, object>();
            foreach (var item in dataTable.Columns)
            {
                var value = this[item];
                if (value == DBNull.Value)
                    props.Add(item.ColumnName, null);
                else
                    props.Add(item.ColumnName, value);
            }
            if (this.ParentID != null)
                props.Add(CremaSchema.__ParentID__, this.ParentID);
            if (this.RelationID != null)
                props.Add(CremaSchema.__RelationID__, this.RelationID);
            return props;
        }

        public TagInfo Tags
        {
            get => this.InternalObject.Tags;
            set => this.InternalObject.Tags = value;
        }

        public TagInfo DerivedTags
        {
            get
            {
                var tags = new TagInfo(this.InternalObject.Field<string>(CremaSchema.Tags));
                var dataTable = this.Table;
                if (dataTable != null)
                {
                    tags &= dataTable.DerivedTags;
                }

                var parentRow = this.Parent;
                if (parentRow != null)
                {
                    tags &= parentRow.DerivedTags;
                }

                return tags;
            }
        }

        public bool IsEnabled
        {
            get => this.InternalObject.IsEnabled;
            set => this.InternalObject.IsEnabled = value;
        }

        public bool IsDerivedEnabled
        {
            get
            {
                var parentRow = this.Parent;
                if (parentRow != null)
                {
                    if (parentRow.IsEnabled == false)
                        return false;
                }

                return this.InternalObject.Field<bool>(CremaSchema.Enable);
            }
        }

        public SignatureDate CreationInfo
        {
            get => this.InternalObject.CreationInfo;
            set => this.InternalObject.CreationInfo = value;
        }

        public SignatureDate ModificationInfo
        {
            get => this.InternalObject.ModificationInfo;
            set => this.InternalObject.ModificationInfo = value;
        }

        public string RelationID
        {
            get => this.InternalObject.RelationID;
            set => this.InternalObject.RelationID = value;
        }

        public string ParentID
        {
            get => this.InternalObject.ParentID;
            set => this.InternalObject.ParentID = value;
        }

        public int Index
        {
            get => this.InternalObject.Index;
            set => this.InternalObject.Index = value;
        }

        public bool HasErrors => this.InternalObject.HasErrors;

        public string RowError
        {
            get => this.InternalObject.RowError;
            set => this.InternalObject.RowError = value;
        }

        public DataRowState RowState => this.InternalObject.RowState;

        public CremaDataRow Parent
        {
            get
            {
                if (this.Table.Parent == null)
                    return null;

                var relationName = InternalSetBase.GenerateRelationName(this.InternalObject.Table);
                if (this.InternalObject.GetParentRow(relationName) is InternalDataRow parentRow)
                {
                    return parentRow.Target;
                }
                return null;
            }
            set => this.InternalObject.SetParentRow(value.InternalObject);
        }

        public CremaDataTable Table => this.InternalObject.Table.Target;

        protected void SetNull(CremaDataColumn column)
        {
            this.InternalObject.SetNull(column.InternalObject);
        }

        internal InternalDataRow InternalObject { get; }
    }
}
