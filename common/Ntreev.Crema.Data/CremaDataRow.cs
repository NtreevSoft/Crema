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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Xml;
using System.IO;
using Ntreev.Crema.Data.Properties;
using Ntreev.Crema.Data.Xml;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;

namespace Ntreev.Crema.Data
{
    public class CremaDataRow
    {
        private readonly InternalDataRow row;

        public CremaDataRow(CremaDataRowBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException();
            this.row = builder.DataRow;
        }

        public void AcceptChanges()
        {
            this.row.AcceptChanges();
        }

        public void BeginEdit()
        {
            this.row.BeginEdit();
        }

        public void CancelEdit()
        {
            this.row.CancelEdit();
        }

        public void ClearErrors()
        {
            this.row.ClearErrors();
        }

        public void Delete()
        {
            this.row.Delete();
        }

        public void EndEdit()
        {
            this.row.EndEdit();
        }

        public CremaDataRow[] GetChildRows(CremaDataTable childTable)
        {
            if (childTable.Parent != this.Table)
                return null;

            var relationName = InternalSetBase.GenerateRelationName(this.Table.TableName, childTable.TableName, this.Table.Namespace);

            return this.row.GetChildRows(relationName).Select(item => (item as InternalDataRow).Target).ToArray();
        }

        public CremaDataRow[] GetChildRows(string childTableName)
        {
            var relationName = InternalSetBase.GenerateRelationName(this.Table.TableName, childTableName, this.Table.Namespace);

            return this.row.GetChildRows(relationName).Select(item => (item as InternalDataRow).Target).ToArray();
        }

        public string GetColumnError(CremaDataColumn cremaColumn)
        {
            return this.row.GetColumnError(cremaColumn.InternalObject);
        }

        public string GetColumnError(int columnIndex)
        {
            return this.row.GetColumnError(columnIndex);
        }

        public string GetColumnError(string columnName)
        {
            return this.row.GetColumnError(columnName);
        }

        public CremaDataColumn[] GetColumnsInError()
        {
            return this.row.GetColumnsInError().Select(item =>
            {
                var column = item as InternalColumnBase;
                return column.Target as CremaDataColumn;
            }).ToArray();
        }

        public bool HasVersion(DataRowVersion version)
        {
            return this.row.HasVersion(version);
        }

        public bool IsNull(CremaDataColumn column)
        {
            return this.row.IsNull(column.InternalObject);
        }

        public bool IsNull(int columnIndex)
        {
            return this.row.IsNull(columnIndex);
        }

        public bool IsNull(string columnName)
        {
            return this.row.IsNull(columnName);
        }

        public bool IsNull(CremaDataColumn column, DataRowVersion version)
        {
            return this.row.IsNull(column.InternalObject, version);
        }

        public void RejectChanges()
        {
            this.row.RejectChanges();
        }

        public void SetAdded()
        {
            this.row.SetAdded();
        }

        public void SetColumnError(CremaDataColumn column, string error)
        {
            this.row.SetColumnError(column.InternalObject, error);
        }

        public void SetColumnError(int columnIndex, string error)
        {
            this.row.SetColumnError(columnIndex, error);
        }

        public void SetColumnError(string columnName, string error)
        {
            this.row.SetColumnError(columnName, error);
        }

        public void SetAttributeError(CremaAttribute attribute, string error)
        {
            this.row.SetColumnError(attribute.InternalAttribute, error);
        }

        public void SetAttributeError(string attributeName, string error)
        {
            this.row.SetColumnError(attributeName, error);
        }

        public void SetModified()
        {
            this.row.SetModified();
        }

        public object this[CremaDataColumn column]
        {
            get { return this.row[column.InternalObject]; }
            set { this.row[column.InternalObject] = value; }
        }

        public object this[CremaDataColumn column, DataRowVersion version]
        {
            get { return this.row[column.InternalObject, version]; }
        }

        public object this[int columnIndex]
        {
            get { return this.row[this.Table.Columns[columnIndex].InternalObject]; }
            set { this.row[this.Table.Columns[columnIndex].InternalObject] = value; }
        }

        public object this[int columnIndex, DataRowVersion version]
        {
            get { return this.row[this.Table.Columns[columnIndex].InternalObject, version]; }
        }

        public object this[string columnName]
        {
            get
            {
                if (this.Table.Columns.Contains(columnName) == false)
                    throw new CremaDataException(string.Format(Resources.Exception_NotFoundColumn_Format, columnName));
                return this.row[columnName];
            }
            set
            {
                if (this.Table.Columns.Contains(columnName) == false)
                    throw new CremaDataException(string.Format(Resources.Exception_NotFoundColumn_Format, columnName));
                this.row[columnName] = value;
            }
        }

        public object this[string columnName, DataRowVersion version]
        {
            get
            {
                if (this.Table.Columns.Contains(columnName) == false)
                    throw new CremaDataException(string.Format(Resources.Exception_NotFoundColumn_Format, columnName));
                return this.row[columnName, version];
            }
        }

        public object GetAttribute(string attributeName)
        {
            if (this.Table.Attributes.Contains(attributeName) == false)
                throw new CremaDataException(string.Format(Resources.Exception_NotFoundAttribute_Format, attributeName));
            return this.row[attributeName];
        }

        public void SetAttribute(string attributeName, object value)
        {
            if (this.Table.Attributes.Contains(attributeName) == false)
                throw new CremaDataException(string.Format(Resources.Exception_NotFoundAttribute_Format, attributeName));
            this.row[attributeName] = value;
        }

        public TagInfo Tags
        {
            get { return this.row.Tags; }
            set { this.row.Tags = value; }
        }

        public TagInfo DerivedTags
        {
            get
            {
                var tags = new TagInfo(this.row.Field<string>(CremaSchema.Tags));

                var cremaTable = this.Table;
                if (cremaTable != null)
                {
                    tags &= cremaTable.DerivedTags;
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
            get { return this.row.IsEnabled; }
            set { this.row.IsEnabled = value; }
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

                return this.row.Field<bool>(CremaSchema.Enable);
            }
        }

        public SignatureDate CreationInfo
        {
            get { return this.row.CreationInfo; }
            internal set { this.row.CreationInfo = value; }
        }

        public SignatureDate ModificationInfo
        {
            get { return this.row.ModificationInfo; }
            internal set { this.row.ModificationInfo = value; }
        }

        public string RelationID
        {
            get { return this.row.RelationID; }
            set { this.row.RelationID = value; }
        }

        public string ParentID
        {
            get { return this.row.ParentID; }
            set { this.row.ParentID = value; }
        }

        public int Index
        {
            get { return this.row.Index; }
            set { this.row.Index = value; }
        }

        public bool HasErrors
        {
            get { return this.row.HasErrors; }
        }

        public string RowError
        {
            get { return this.row.RowError; }
            set { this.row.RowError = value; }
        }

        public DataRowState RowState
        {
            get { return this.row.RowState; }
        }

        public CremaDataRow Parent
        {
            get
            {
                if (this.Table.Parent == null)
                    return null;

                var relationName = InternalSetBase.GenerateRelationName(this.row.Table.Parent, this.row.Table);
                if (this.row.GetParentRow(relationName) is InternalDataRow parentRow)
                {
                    return parentRow.Target;
                }
                return null;
            }
            set
            {
                this.row.SetParentRow(value.InternalObject);
            }
        }

        public CremaDataTable Table
        {
            get { return this.row.Table.Target; }
        }

        protected void SetNull(CremaDataColumn column)
        {
            this.row.SetNull(column.InternalObject);
        }

        internal InternalDataRow InternalObject
        {
            get { return this.row; }
        }

        public object[] Filter(CremaDataColumn[] columns, TagInfo tags)
        {
            var fields = new List<object>();

            for (var i = 0; i < columns.Length; i++)
            {
                fields.Add(this[columns[i].ColumnName]);
            }

            return fields.ToArray();
        }
    }
}
