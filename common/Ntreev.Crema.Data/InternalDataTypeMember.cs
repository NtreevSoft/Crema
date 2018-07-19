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
using System;
using System.Data;

namespace Ntreev.Crema.Data
{
    class InternalDataTypeMember : InternalRowBase<InternalDataType, InternalDataTypeMember>
    {
        public InternalDataTypeMember(CremaDataTypeMemberBuilder builder, InternalDataType table)
            : base(table, builder.InternalBuilder)
        {
            this.Table = table;
            base.Target = builder.NewMember(this);
        }

        public override string ToString()
        {
            if (this.RowState == DataRowState.Deleted)
                return string.Empty;
            return $"{this[CremaSchema.Name]}";
        }

        public static explicit operator CremaDataTypeMember(InternalDataTypeMember typeMebmer)
        {
            if (typeMebmer == null)
                return null;
            return typeMebmer.Target;
        }

        public static explicit operator InternalDataTypeMember(CremaDataTypeMember typeMebmer)
        {
            if (typeMebmer == null)
                return null;
            return typeMebmer.InternalObject;
        }

        public new void AcceptChanges()
        {
            using (this.Table.AcceptChangesStack.Set(true))
            {
                base.AcceptChanges();
            }
        }

        public new CremaDataTypeMember Target => base.Target as CremaDataTypeMember;

        public new InternalDataType Table { get; }

        public Guid ID
        {
            get => this.Field<Guid>(this.Table.attributeID);
            set => this.SetField<Guid>(this.Table.attributeID, value);
        }

        public TagInfo Tags
        {
            get => new TagInfo(this.Field<string>(CremaSchema.Tags));
            set => this.SetField<string>(CremaSchema.Tags, value.ToString());
        }

        public TagInfo DerivedTags => this.Tags & this.Table.Tags;

        public bool IsEnabled
        {
            get => this.Field<bool>(CremaSchema.Enable);
            set => this.SetField(CremaSchema.Enable, value);
        }

        public string Name
        {
            get => this.Field<string>(this.Table.columnName) ?? string.Empty;
            set => this.SetField(this.Table.columnName, value);
        }

        public long Value
        {
            get => this.Field<long>(this.Table.columnValue);
            set => this.SetField(this.Table.columnValue, value);
        }

        public string Comment
        {
            get => this.Field<string>(this.Table.columnComment) ?? string.Empty;
            set => this.SetField(this.Table.columnComment, value);
        }

        public TypeMemberInfo TypeMemberInfo
        {
            get
            {
                return new TypeMemberInfo()
                {
                    ID = this.ID,
                    Tags = this.Tags,
                    DerivedTags = this.DerivedTags,
                    IsEnabled = this.IsEnabled,
                    Name = this.Name,
                    Value = this.Value,
                    Comment = this.Comment,
                    CreationInfo = this.CreationInfo,
                    ModificationInfo = this.ModificationInfo
                };
            }
        }
    }
}
