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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data
{
    class InternalDataTypeMember : InternalRowBase<InternalDataType, InternalDataTypeMember>
    {
        private readonly InternalDataType table;

        public InternalDataTypeMember(CremaDataTypeMemberBuilder builder, InternalDataType table)
            : base(table, builder.InternalBuilder)
        {
            this.table = table;
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
            using (this.table.AcceptChangesStack.Set(true))
            {
                base.AcceptChanges();
            }
        }

        public new CremaDataTypeMember Target
        {
            get { return base.Target as CremaDataTypeMember; }
        }

        public new InternalDataType Table
        {
            get { return this.table; }
        }

        public Guid ID
        {
            get { return this.Field<Guid>(this.table.attributeID); }
            set { this.SetField<Guid>(this.table.attributeID, value); }
        }

        public TagInfo Tags
        {
            get { return new TagInfo(this.Field<string>(CremaSchema.Tags)); }
            set { this.SetField<string>(CremaSchema.Tags, value.ToString()); }
        }

        public TagInfo DerivedTags
        {
            get { return this.Tags & this.Table.Tags; }
        }

        public bool IsEnabled
        {
            get { return this.Field<bool>(CremaSchema.Enable); }
            set { this.SetField(CremaSchema.Enable, value); }
        }

        public string Name
        {
            get { return this.Field<string>(this.table.columnName) ?? string.Empty; }
            set { this.SetField(this.table.columnName, value); }
        }

        public long Value
        {
            get { return this.Field<long>(this.table.columnValue); }
            set { this.SetField(this.table.columnValue, value); }
        }

        public string Comment
        {
            get { return this.Field<string>(this.table.columnComment) ?? string.Empty; }
            set { this.SetField(this.table.columnComment, value); }
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
