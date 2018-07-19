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
using System.Data;

namespace Ntreev.Crema.Data
{
    class InternalTemplateColumn : InternalRowBase<InternalTemplate, InternalTemplateColumn>
    {
        private readonly InternalTemplate table;

        public InternalTemplateColumn(CremaTemplateColumnBuilder builder, InternalTemplate table)
            : base(table, builder.InternalBuilder)
        {
            this.table = table;
            base.Target = builder.NewColumn(this);
        }

        public void AttachTarget(InternalDataColumn targetColumn)
        {
            this.ColumnID = targetColumn.ColumnID;
            this.IsKey = targetColumn.IsKey;
            this.Unique = targetColumn.Unique;
            this.ColumnName = targetColumn.ColumnName;
            this.DataTypeName = targetColumn.DataTypeName;
            this.DefaultValue = targetColumn.DefaultValue;
            this.AutoIncrement = targetColumn.AutoIncrement;
            this.AllowNull = targetColumn.AllowDBNull;
            this.ReadOnly = targetColumn.ReadOnly;
            this.Comment = targetColumn.Comment;
            this.Tags = targetColumn.Tags;
            this.CreationInfo = targetColumn.CreationInfo;
            this.ModificationInfo = targetColumn.ModificationInfo;
            this.TargetColumn = targetColumn;
        }

        public static explicit operator CremaTemplateColumn(InternalTemplateColumn column)
        {
            if (column == null)
                return null;
            return column.Target;
        }

        public static explicit operator InternalTemplateColumn(CremaTemplateColumn column)
        {
            if (column == null)
                return null;
            return column.InternalObject;
        }

        public new CremaTemplateColumn Target => base.Target as CremaTemplateColumn;

        public Guid ColumnID
        {
            get => this.InternalColumnID;
            set => this.InternalColumnID = value;
        }

        public bool IsKey
        {
            get => this.InternalIsKey;
            set => this.InternalIsKey = value;
        }

        public bool Unique
        {
            get => this.InternalUnique;
            set => this.InternalUnique = value;
        }

        public string ColumnName
        {
            get => this.InternalColumnName ?? string.Empty;
            set
            {
                if (this.RowState != DataRowState.Detached && value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.RowState != DataRowState.Detached && value == string.Empty)
                    throw new ArgumentException(nameof(value));
                this.InternalColumnName = value;
            }
        }

        public string DataTypeName
        {
            get => this.InternalDataTypeName;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value == string.Empty)
                    throw new ArgumentException(nameof(value));
                this.InternalDataTypeName = value;
            }
        }

        public object DefaultValue
        {
            get
            {
                var value = this.InternalDefaultValue;
                if (value == DBNull.Value)
                    return value;
                if (CremaDataTypeUtility.IsBaseType(this.InternalDataTypeName) == true)
                    return CremaConvert.ChangeType(value, CremaDataTypeUtility.GetType(this.InternalDataTypeName));
                return value;
            }
            set => this.InternalDefaultValue = CremaConvert.ToString(value);
        }

        public bool AutoIncrement
        {
            get => this.InternalAutoIncrement;
            set => this.InternalAutoIncrement = value;
        }

        public string Comment
        {
            get => this.InternalComment ?? string.Empty;
            set => this.InternalComment = value;
        }

        public TagInfo Tags
        {
            get => this.InternalTags;
            set => this.InternalTags = value;
        }

        public bool AllowNull
        {
            get => this.InternalAllowNull;
            set => this.InternalAllowNull = value;
        }

        public bool ReadOnly
        {
            get => this.InternalReadOnly;
            set => this.InternalReadOnly = value;
        }

        public InternalDataColumn TargetColumn { get; set; }

        public bool CanDelete
        {
            get
            {
                if (this.TargetColumn == null)
                    return false;
                if (this.TargetColumn.Table == null)
                    return false;
                return this.TargetColumn.Table.Columns.CanRemove(this.TargetColumn);
            }
        }

        public Guid InternalColumnID
        {
            get => this.Field<Guid>(this.table.columnID);
            internal set => this.SetField(this.table.columnID, value);
        }

        public bool InternalIsKey
        {
            get => this.Field<bool>(this.table.columnIsKey);
            set => this.SetField(this.table.columnIsKey, value);
        }

        public bool InternalUnique
        {
            get => this.Field<bool>(this.table.columnIsUnique);
            set => this.SetField(this.table.columnIsUnique, value);
        }

        public string InternalColumnName
        {
            get => this.Field<string>(this.table.columnColumnName);
            set => this.SetField(this.table.columnColumnName, value);
        }

        public string InternalDataTypeName
        {
            get => this.Field<string>(this.table.columnDataType);
            set => this.SetField(this.table.columnDataType, value);
        }

        public object InternalDefaultValue
        {
            get => this[this.table.columnDefaultValue];
            set => this[this.table.columnDefaultValue] = value;
        }

        public bool InternalAutoIncrement
        {
            get => this.Field<bool>(this.table.columnAutoIncrement);
            set => this.SetField(this.table.columnAutoIncrement, value);
        }

        public string InternalComment
        {
            get => this.Field<string>(this.table.columnComment);
            set => this.SetField(this.table.columnComment, value);
        }

        public TagInfo InternalTags
        {
            get => new TagInfo(this.Field<string>(this.table.columnTags));
            set => this.SetField(this.table.columnTags, value.ToString());
        }

        public bool InternalAllowNull
        {
            get => this.Field<bool>(this.table.columnAllowNull);
            set => this.SetField(this.table.columnAllowNull, value);
        }

        public bool InternalReadOnly
        {
            get => this.Field<bool>(this.table.columnReadOnly);
            set => this.SetField(this.table.columnReadOnly, value);
        }
    }
}
