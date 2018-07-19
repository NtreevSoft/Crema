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
using Ntreev.Library.IO;
using System;
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Data
{
    class InternalTemplate : InternalTableBase<InternalTemplate, InternalTemplateColumn>
    {
        private readonly CremaTemplateColumnBuilder builder;

        internal readonly InternalAttribute columnID;
        internal readonly InternalAttribute columnTags;

        internal readonly DataColumn columnIsKey;
        internal readonly DataColumn columnColumnName;
        internal readonly DataColumn columnDataType;
        internal readonly DataColumn columnComment;
        internal readonly DataColumn columnDefaultValue;
        internal readonly DataColumn columnAllowNull;
        internal readonly DataColumn columnReadOnly;
        internal readonly DataColumn columnIsUnique;
        internal readonly DataColumn columnAutoIncrement;

        public InternalTemplate(CremaTemplate template, CremaTemplateColumnBuilder builder)
            : base("TableTemplate", PathUtility.Separator)
        {
            base.Target = template;
            this.builder = builder;

            this.columnID = new InternalAttribute(CremaSchema.ID, typeof(Guid))
            {
                ColumnMapping = MappingType.Hidden,
                AllowDBNull = false,
                DefaultValue = Guid.NewGuid()
            };
            this.Columns.Add(this.columnID);

            this.columnTags = new InternalAttribute(CremaSchema.Tags, typeof(string))
            {
                ColumnMapping = MappingType.Attribute,
                DefaultValue = $"{TagInfo.All}"
            };
            this.Columns.Add(this.columnTags);

            this.columnIsKey = new DataColumn(CremaSchema.IsKey, typeof(bool))
            {
                DefaultValue = false,
                AllowDBNull = false
            };
            this.Columns.Add(this.columnIsKey);

            this.columnColumnName = new DataColumn(CremaSchema.ColumnName)
            {
                DefaultValue = "Column1",
                AllowDBNull = false
            };
            this.Columns.Add(this.columnColumnName);

            this.columnDataType = new DataColumn(CremaSchema.DataType)
            {
                DefaultValue = typeof(string).GetTypeName(),
                AllowDBNull = false
            };
            this.Columns.Add(this.columnDataType);

            this.columnComment = new DataColumn(CremaSchema.Comment);
            this.Columns.Add(this.columnComment);

            this.columnDefaultValue = new DataColumn(CremaSchema.DefaultValue);
            this.Columns.Add(this.columnDefaultValue);

            this.columnAllowNull = new DataColumn(CremaSchema.AllowNull, typeof(bool))
            {
                DefaultValue = true,
                AllowDBNull = false
            };
            this.Columns.Add(this.columnAllowNull);

            this.columnReadOnly = new DataColumn(CremaSchema.ReadOnly, typeof(bool))
            {
                DefaultValue = false,
                AllowDBNull = false
            };
            this.Columns.Add(this.columnReadOnly);

            this.columnIsUnique = new DataColumn(CremaSchema.IsUnique, typeof(bool))
            {
                DefaultValue = false,
                AllowDBNull = false
            };
            this.Columns.Add(this.columnIsUnique);

            this.columnAutoIncrement = new DataColumn(CremaSchema.AutoIncrement, typeof(bool))
            {
                DefaultValue = false,
                AllowDBNull = false
            };
            this.Columns.Add(this.columnAutoIncrement);

            this.PrimaryKey = new DataColumn[] { this.columnColumnName };
            this.DefaultView.Sort = $"{CremaSchema.Index} ASC";
        }

        public void AddColumn(InternalTemplateColumn column)
        {
            this.ValidateAddColumn(column);
            this.Rows.Add(column);
        }

        public void RemoveColumn(InternalTemplateColumn column)
        {
            this.ValidateRemoveColumn(column);
            using (this.AcceptChangesStack.Set(true))
            {
                this.Rows.Remove(column);
            }
        }

        public void ClearColumns()
        {
            this.ValidateClearColumns();
            this.Rows.Clear();
        }

        public void ValidateAddColumn(InternalTemplateColumn column)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));
            if (column.RowState != DataRowState.Detached && column.Table == this)
                throw new ArgumentException($"이 항목은 이미 현재 DataType에 속해 있습니다.", nameof(column));
            if (column.Table != null && column.Table != this)
                throw new ArgumentException($"'{column}' 항목은 이미 다른 DataType에 속해 있습니다.", nameof(column));
            if (this.ReadOnly == true)
                throw new ArgumentException($"읽기 전용인 타입에는 항목을 추가할 수 없습니다.", nameof(column));
        }

        public void ValidateRemoveColumn(InternalTemplateColumn column)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));
            if (column.Table == null)
                throw new ArgumentException($"이 타입에 속하지 않는 항목은 제거할 수 없습니다.", nameof(column));
            if (column.Table != null && column.Table != this)
                throw new ArgumentException($"이 타입에 속하지 않는 항목은 제거할 수 없습니다.", nameof(column));
            if (this.ReadOnly == true)
                throw new ArgumentException($"읽기 전용인 타입에는 항목을 제거할 수 없습니다.", nameof(column));
        }

        public void ValidateClearColumns()
        {
            //foreach (var item in this.referenceList)
            //{
            //    this.ValidateClearColumns(item);
            //}
        }

        public bool ContainsColumn(string columnName)
        {
            foreach (var item in this.Rows)
            {
                if (item is InternalTemplateColumn column)
                {
                    if (column.RowState == DataRowState.Deleted)
                        continue;
                    if (column.Field<string>(CremaSchema.ColumnName) == columnName)
                        return true;
                }
            }
            return false;
        }

        public bool ContainsColumn(Guid columnID)
        {
            foreach (var item in this.Rows)
            {
                if (item is InternalTemplateColumn column)
                {
                    if (column.RowState == DataRowState.Deleted)
                        continue;
                    if (column[CremaSchema.ID] is Guid id && id == columnID)
                        return true;
                }
            }
            return false;
        }

        public new CremaTemplate Target
        {
            get { return base.Target as CremaTemplate; }
        }

        public override bool IsLoading
        {
            get { return this.IsDiffMode; }
        }

        public InternalDataTable TargetTable
        {
            get { return this.InternalTargetTable; }
            set
            {
                this.InternalTypes = null;
                this.InternalTargetTable = value;

                if (this.InternalTargetTable != null)
                {
                    this.SignatureDateProvider = this.InternalTargetTable.SignatureDateProvider;
                    this.OmitSignatureDate = true;
                    foreach (var item in this.InternalTargetTable.Columns)
                    {
                        if (item is InternalDataColumn column)
                        {
                            if (this.NewRow() is InternalTemplateColumn row)
                            {
                                row.AttachTarget(column);
                                this.Rows.Add(row);
                            }
                        }
                    }
                    this.OmitSignatureDate = false;
                    this.AcceptChanges();
                }
            }
        }

        public InternalDataSet TargetSet
        {
            get
            {
                if (this.InternalTargetTable == null)
                    return null;
                return this.InternalTargetTable.DataSet;
            }
        }

        public string[] Types
        {
            get
            {
                if (this.InternalTypes == null)
                {
                    if (this.TargetSet != null)
                    {
                        var types1 = CremaDataTypeUtility.GetBaseTypeNames().OrderBy(item => item);
                        var types2 = from DataTable item in this.TargetSet.Tables
                                     let type = item as InternalDataType
                                     where type != null
                                     orderby type.Name
                                     select type.CategoryPath + type.Name;

                        this.InternalTypes = types1.Concat(types2).ToArray();
                    }
                    else
                    {
                        this.InternalTypes = CremaDataTypeUtility.GetBaseTypeNames().OrderBy(item => item).ToArray();
                    }
                }
                return this.InternalTypes;
            }
        }

        public InternalDataTable InternalTargetTable { get; set; }

        public string[] InternalTypes { get; set; }

        protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
        {
            this.builder.InternalBuilder = builder;
            return new InternalTemplateColumn(this.builder, this);
        }

        protected override void OnColumnChanging(DataColumnChangeEventArgs e)
        {
            base.OnColumnChanging(e);

            if (e.Row is InternalTemplateColumn columnItem && columnItem.TargetColumn != null && this.RowEventStack.Any() == false && this.IsDiffMode == false)
            {
                var targetColumn = columnItem.TargetColumn;
                if (e.Column == this.attributeIndex)
                {
                    targetColumn.ValidateSetIndex((int)e.ProposedValue);
                }
                else if (e.Column == this.columnTags)
                {
                    targetColumn.ValidateSetTags((TagInfo)(string)e.ProposedValue);
                }
                else if (e.Column == this.columnIsKey)
                {
                    targetColumn.ValidateSetIsKey((bool)e.ProposedValue);
                }
                else if (e.Column == this.columnColumnName)
                {
                    targetColumn.ValidateSetColumnName(e.ProposedValue as string);
                }
                else if (e.Column == this.columnDataType)
                {
                    targetColumn.ValidateSetDataTypeName(e.ProposedValue as string);
                }
                else if (e.Column == this.columnComment)
                {
                    targetColumn.ValidateSetComment(e.ProposedValue as string);
                }
                else if (e.Column == this.columnDefaultValue)
                {
                    targetColumn.ValidateSetDefaultValue(e.ProposedValue);
                }
                else if (e.Column == this.columnAllowNull)
                {
                    targetColumn.ValidateSetAllowDBNull((bool)e.ProposedValue);
                }
                else if (e.Column == this.columnReadOnly)
                {
                    targetColumn.ValidateSetReadOnly((bool)e.ProposedValue);
                }
                else if (e.Column == this.columnIsUnique)
                {
                    targetColumn.ValidateSetUnique((bool)e.ProposedValue);
                }
                else if (e.Column == this.columnAutoIncrement)
                {
                    targetColumn.ValidateSetAutoIncrement((bool)e.ProposedValue);
                }
            }
        }

        protected override void OnColumnChanged(DataColumnChangeEventArgs e)
        {
            base.OnColumnChanged(e);

            if (e.Row is InternalTemplateColumn columnItem && columnItem.TargetColumn != null && this.RowEventStack.Any() == false)
            {
                var targetColumn = columnItem.TargetColumn;
                if (this.IsDiffMode == false)
                {
                    if (e.Column == this.columnTags)
                    {
                        targetColumn.Tags = columnItem.Tags;
                    }
                    else if (e.Column == this.columnIsKey)
                    {
                        targetColumn.IsKey = columnItem.IsKey;
                    }
                    else if (e.Column == this.columnColumnName)
                    {
                        targetColumn.ColumnName = columnItem.ColumnName;
                    }
                    else if (e.Column == this.columnDataType)
                    {
                        targetColumn.DataTypeName = columnItem.DataTypeName;
                    }
                    else if (e.Column == this.columnComment)
                    {
                        targetColumn.Comment = columnItem.Comment;
                    }
                    else if (e.Column == this.columnDefaultValue)
                    {
                        targetColumn.DefaultValue = columnItem.DefaultValue;
                    }
                    else if (e.Column == this.columnAllowNull)
                    {
                        targetColumn.AllowDBNull = columnItem.AllowNull;
                    }
                    else if (e.Column == this.columnReadOnly)
                    {
                        targetColumn.ReadOnly = columnItem.ReadOnly;
                    }
                    else if (e.Column == this.columnIsUnique)
                    {
                        targetColumn.Unique = columnItem.Unique;
                    }
                    else if (e.Column == this.columnAutoIncrement)
                    {
                        targetColumn.AutoIncrement = columnItem.AutoIncrement;
                    }
                }
            }
        }

        protected override void OnRowChanging(DataRowChangeEventArgs e)
        {
            if (this.RowEventStack.Any() == true)
            {
                if (this.RowEventStack.Peek() == e.Row)
                    base.OnRowChanging(new InternalDataRowChangeEventArgs(e));
                else
                    base.OnRowChanging(e);
            }
            else
            {
                var row = e.Row as InternalTemplateColumn;
                switch (e.Action)
                {
                    case DataRowAction.Add:
                        {
                            if (this.IsLoading == false)
                                CremaDataSet.ValidateName(row.ColumnName);

                            if (this.InternalTargetTable != null && this.IsDiffMode == false)
                                InternalDataSet.ValidateSetDataTypeName(this.InternalTargetTable.DataSet, row.DataTypeName);

                            if (e.Row is InternalTemplateColumn c && c.TargetColumn == null && this.InternalTargetTable != null)
                            {
                                var dataColumn = new InternalDataColumn()
                                {
                                    ColumnName = c.ColumnName,

                                    IsKey = c.IsKey,
                                    Unique = c.Unique,
                                    Comment = c.Comment,
                                    DefaultValue = c.DefaultValue ?? (object)DBNull.Value,
                                    AutoIncrement = c.AutoIncrement,
                                    AllowDBNull = c.AllowNull,
                                    Tags = c.Tags,
                                    ReadOnly = c.ReadOnly,
                                };
                                this.TargetTable.ValidateAddColumn(dataColumn);
                            }
                        }
                        break;
                }
                base.OnRowChanging(e);
            }
        }

        protected override void OnRowChanged(DataRowChangeEventArgs e)
        {
            if (this.RowEventStack.Any() == false)
            {
                base.OnRowChanged(e);
                if (e.Action == DataRowAction.Add)
                {
                    if (e.Row is InternalTemplateColumn c && c.TargetColumn == null && this.InternalTargetTable != null)
                    {
                        var dataColumn = new InternalDataColumn()
                        {
                            ColumnName = c.ColumnName,

                            IsKey = c.IsKey,
                            Unique = c.Unique,
                            Comment = c.Comment,
                            DefaultValue = c.DefaultValue ?? (object)DBNull.Value,
                            AutoIncrement = c.AutoIncrement,
                            AllowDBNull = c.AllowNull,
                            Tags = c.Tags,
                            ReadOnly = c.ReadOnly,
                        };

                        this.TargetTable.AddColumn(dataColumn);
                        dataColumn.DataTypeName = c.DataTypeName;
                        dataColumn.CreationInfo = c.CreationInfo;
                        dataColumn.ModificationInfo = c.ModificationInfo;
                        c.TargetColumn = dataColumn;
                        this.UpdateRow(e.Row);
                    }
                }
                else if (e.Action == DataRowAction.Change)
                {
                    if (e.Row is InternalTemplateColumn c && this.IsDiffMode == false)
                    {
                        c.TargetColumn.Index = c.Index;
                        c.TargetColumn.ModificationInfo = c.ModificationInfo;
                    }
                    if (this.IsDiffMode == false)
                    {
                        this.UpdateRow(e.Row);
                    }
                }

                if (this.IsLoading == false)
                {
                    this.columnColumnName.DefaultValue = this.GenerateColumnName();
                    this.columnID.DefaultValue = Guid.NewGuid();
                }
            }
            else
            {
                base.OnRowChanged(new InternalDataRowChangeEventArgs(e));
            }
        }

        protected override void OnRowDeleting(DataRowChangeEventArgs e)
        {
            //이 열은 기본 키의 일부이므로 제거할 수 없습니다.'
            if (e.Row is InternalTemplateColumn column)
            {
                this.TargetTable.ValidateRemoveColumn(column.TargetColumn);
            }

            base.OnRowDeleting(e);
        }

        protected override void OnRowDeleted(DataRowChangeEventArgs e)
        {
            base.OnRowDeleted(e);
            if (e.Row is InternalTemplateColumn column)
            {
                this.TargetTable.RemoveColumn(column.TargetColumn);
            }
        }

        protected override void OnTableCleared(DataTableClearEventArgs e)
        {
            base.OnTableCleared(e);
        }

        protected override void OnSetNormalMode()
        {
            base.OnSetNormalMode();

            this.columnID.AllowDBNull = false;
            this.columnID.ColumnMapping = MappingType.Hidden;
            this.columnID.DefaultValue = Guid.NewGuid();
            this.columnID.ReadOnly = true;

            this.columnTags.ColumnMapping = MappingType.Attribute;
            this.columnTags.DefaultValue = $"{TagInfo.All}";

            this.columnIsKey.DefaultValue = false;
            this.columnIsKey.AllowDBNull = false;

            this.columnColumnName.DefaultValue = "Column1";
            this.columnColumnName.AllowDBNull = false;
            this.columnColumnName.Unique = true;

            this.columnDataType.DefaultValue = typeof(string).GetTypeName();
            this.columnDataType.AllowDBNull = false;

            this.columnAllowNull.DefaultValue = true;
            this.columnAllowNull.AllowDBNull = false;

            this.columnReadOnly.DefaultValue = false;
            this.columnReadOnly.AllowDBNull = false;

            this.columnIsUnique.DefaultValue = false;
            this.columnIsUnique.AllowDBNull = false;

            this.columnAutoIncrement.DefaultValue = false;
            this.columnAutoIncrement.AllowDBNull = false;

            this.PrimaryKey = new DataColumn[] { this.columnColumnName };
            this.DefaultView.Sort = $"{CremaSchema.Index} ASC";
        }

        protected override void OnSetDiffMode()
        {
            base.OnSetDiffMode();

            this.columnID.AllowDBNull = true;
            this.columnID.ReadOnly = false;
            this.columnID.ColumnMapping = MappingType.Attribute;
            this.columnID.DefaultValue = DBNull.Value;

            this.columnTags.ColumnMapping = MappingType.Attribute;

            this.columnIsKey.DefaultValue = DBNull.Value;
            this.columnIsKey.AllowDBNull = true;

            this.columnColumnName.DefaultValue = DBNull.Value;
            this.columnColumnName.AllowDBNull = true;

            this.columnDataType.DefaultValue = DBNull.Value;
            this.columnDataType.AllowDBNull = true;

            this.columnAllowNull.DefaultValue = DBNull.Value;
            this.columnAllowNull.AllowDBNull = true;

            this.columnReadOnly.DefaultValue = DBNull.Value;
            this.columnReadOnly.AllowDBNull = true;

            this.columnIsUnique.DefaultValue = DBNull.Value;
            this.columnIsUnique.AllowDBNull = true;

            this.columnAutoIncrement.DefaultValue = DBNull.Value;
            this.columnAutoIncrement.AllowDBNull = true;

            this.PrimaryKey = null;
        }

        protected override string BaseNamespace => CremaSchema.TemplateNamespace1;

        private string GenerateColumnName()
        {
            var index = 1;
            var name = "Column";
            var columnName = name + index++;

            while (FindName())
            {
                columnName = name + index++;
            }

            return columnName;

            bool FindName()
            {
                foreach (var item in this.Rows)
                {
                    if (item is DataRow dataRow)
                    {
                        if (dataRow.RowState != DataRowState.Deleted && dataRow.Field<string>(this.columnColumnName) == columnName)
                            return true;
                    }
                }
                return false;
            }
        }

        private void UpdateRow(DataRow row)
        {
            if (this.RowEventStack.Any() == true)
                throw new Exception();

            if (this.IsDiffMode == true)
                return;

            this.RowEventStack.Push(row);
            var omitSignatureDate = this.OmitSignatureDate;
            this.OmitSignatureDate = true;

            var query = from DataRow item in this.Rows
                        where item.RowState != DataRowState.Deleted
                        select item as InternalTemplateColumn;

            foreach (var item in query)
            {
                var targetColumn = item.TargetColumn;

                if (item.InternalIsKey != targetColumn.InternalIsKey)
                    item.InternalIsKey = targetColumn.InternalIsKey;
                if (item.InternalUnique != targetColumn.InternalUnique)
                    item.InternalUnique = targetColumn.InternalUnique;
                if (item.InternalColumnName != targetColumn.InternalColumnName)
                    item.InternalColumnName = targetColumn.InternalColumnName;
                if (item.InternalDataTypeName != targetColumn.InternalDataTypeName)
                    item.InternalDataTypeName = targetColumn.InternalDataTypeName;
                if (item.InternalComment != targetColumn.InternalComment)
                    item.InternalComment = targetColumn.InternalComment;
                if (object.Equals(item.InternalDefaultValue, targetColumn.InternalDefaultValue) == false)
                    item.InternalDefaultValue = targetColumn.InternalDefaultValue;
                if (item.InternalAllowNull != targetColumn.InternalAllowDBNull)
                    item.InternalAllowNull = targetColumn.InternalAllowDBNull;
                if (item.InternalReadOnly != targetColumn.InternalReadOnly)
                    item.InternalReadOnly = targetColumn.InternalReadOnly;
                if (item.InternalAutoIncrement != targetColumn.InternalAutoIncrement)
                    item.InternalAutoIncrement = targetColumn.InternalAutoIncrement;
            }

            this.OmitSignatureDate = omitSignatureDate;
            this.RowEventStack.Pop();
        }
    }
}
