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
    abstract class InternalRowBase : DataRow
    {
        private readonly InternalTableBase table;

        protected InternalRowBase(InternalTableBase table, DataRowBuilder builder)
            : base(builder)
        {
            this.table = table;
            this.table.RowChanged += Table_RowChanged;
            this.table.RowDeleted += Table_RowDeleted;
            if (this.table.ColumnRelation != null)
                this[this.table.ColumnRelation] = this.table.GenerateRelationID(this);
        }

        public SignatureDate CreationInfo
        {
            get => new SignatureDate(this.Field<string>(this.table.attributeCreator) ?? string.Empty, this.Field<DateTime>(this.table.attributeCreatedDateTime));
            internal set
            {
                if (this.table.attributeCreator.ReadOnly == false)
                {
                    this.SetField(this.table.attributeCreator, value.ID);
                }
                else
                {
                    this.SetReadOnlyField(this.table.attributeCreator, value.ID);
                }

                if (this.table.attributeCreatedDateTime.ReadOnly == false)
                {
                    this.SetField(this.table.attributeCreatedDateTime, value.DateTime);
                }
                else
                {
                    this.SetReadOnlyField(this.table.attributeCreatedDateTime, value.DateTime);
                }
            }
        }

        public SignatureDate ModificationInfo
        {
            get => new SignatureDate(this.Field<string>(this.table.attributeModifier) ?? string.Empty, this.Field<DateTime>(this.table.attributeModifiedDateTime));
            internal set
            {
                if (this.table.attributeModifier.ReadOnly == false)
                {
                    this.SetField(this.table.attributeModifier, value.ID);
                }
                else
                {
                    this.SetReadOnlyField(this.table.attributeModifier, value.ID);
                }

                if (this.table.attributeModifiedDateTime.ReadOnly == false)
                {
                    this.SetField(this.table.attributeModifiedDateTime, value.DateTime);
                }
                else
                {
                    this.SetReadOnlyField(this.table.attributeModifiedDateTime, value.DateTime);
                }
            }
        }

        public object Target { get; set; }

        public string RelationID
        {
            get
            {
                if (this.table.ColumnRelation == null)
                    return null;
                return this.Field<string>(this.table.ColumnRelation);
            }
            set
            {
                if (this.table.ColumnRelation == null)
                    return;

                if (this[this.table.ColumnRelation] is DBNull == false)
                {
                    if (this.RowState != DataRowState.Detached)
                        throw new InvalidOperationException();
                }

                this.SetField(this.table.ColumnRelation, value);
            }
        }

        public string ParentID
        {
            get
            {
                if (this.table.ParentRelation == null)
                    return null;
                return this.Field<string>(this.table.ParentRelation);
            }
            set
            {
                if (this.table.ParentRelation == null)
                    return;

                if (this[this.table.ParentRelation] is DBNull == false)
                {
                    if (this.RowState != DataRowState.Detached)
                        throw new InvalidOperationException();
                }
                this.SetField(this.table.ParentRelation, value);
            }
        }

        public abstract int Index { get; set; }

        private void Table_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            this.Index = this.table.Rows.Count;
        }

        private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
            {
                if (e.Row == this)
                {
                    this.table.RowChanged -= Table_RowChanged;
                    this.table.RowDeleted -= Table_RowDeleted;
                }
                else
                {
                    this.Index = this.table.Rows.Count;
                }
            }
        }
    }

    class InternalRowBase<TableBase, RowBase> : InternalRowBase
        where TableBase : InternalTableBase<TableBase, RowBase>
        where RowBase : InternalRowBase<TableBase, RowBase>
    {
        private readonly TableBase table;
        protected InternalRowBase(TableBase table, DataRowBuilder builder)
            : base(table, builder)
        {
            this.table = table;
        }

        public override int Index
        {
            get => this.InternalIndex;
            set => this.InternalIndex = value;
        }

        public int InternalIndex
        {
            get
            {
                if (this.RowState == DataRowState.Deleted || this.RowState == DataRowState.Detached)
                    return -1;
                return this.Field<int>(this.table.attributeIndex);
            }
            set { this.SetField(this.table.attributeIndex, value); }
        }

        public RowBase ParentRow
        {
            get
            {
                if (this.table.Parent == null)
                    return null;
                var relationName = InternalSetBase.GenerateRelationName(this.table.Parent, this.table);
                return this.GetParentRow(relationName) as RowBase;
            }
        }
    }
}
