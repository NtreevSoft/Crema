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
    class InternalDataRow : InternalRowBase<InternalDataTable, InternalDataRow>
    {
        private InternalDataTable table;

        public InternalDataRow(CremaDataRowBuilder builder, InternalDataTable table)
            : base(table, builder.InternalBuilder)
        {
            this.table = table;
            base.Target = builder.NewRow(this);
        }

        public static explicit operator CremaDataRow(InternalDataRow dataRow)
        {
            if (dataRow == null)
                return null;
            return dataRow.Target;
        }

        public static explicit operator InternalDataRow(CremaDataRow dataRow)
        {
            if (dataRow == null)
                return null;
            return dataRow.InternalObject;
        }

        public void SetNull(InternalDataColumn column)
        {
            base.SetNull(column);
        }

        public new CremaDataRow Target
        {
            get => base.Target as CremaDataRow;
            set
            {
                if (base.Target != null)
                    throw new ArgumentException();
                base.Target = value;
            }
        }

        public new InternalDataTable Table => base.Table as InternalDataTable;

        public bool IsEnabled
        {
            get => this.Field<bool>(this.table.attributeEnable);
            set => this.SetField(this.table.attributeEnable, value);
        }

        public TagInfo Tags
        {
            get => new TagInfo(this.Field<string>(this.table.attributeTag));
            set => this.SetField(this.table.attributeTag, value.ToString());
        }
    }
}
