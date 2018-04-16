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

using Ntreev.Crema.Reader.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Crema.Reader.Binary
{
    class CremaBinaryRow : IRow
    {
        public byte[] fieldbytes;

        public CremaBinaryTable Table { get; set; }

        public object this[IColumn column]
        {
            get
            {
                int offset = BitConverter.ToInt32(this.fieldbytes, sizeof(int) * column.Index);

                if (column.DataType == typeof(string))
                {
                    if (offset == 0)
                        return string.Empty;
                    int id = BitConverter.ToInt32(this.fieldbytes, offset);
                    return StringResource.GetString(id);
                }
                else
                {
                    if (offset == 0)
                        return null;
                    if (column.DataType == typeof(bool))
                        return BitConverter.ToBoolean(this.fieldbytes, offset);
                    else if (column.DataType == typeof(sbyte))
                        return (sbyte)this.fieldbytes[offset];
                    else if (column.DataType == typeof(byte))
                        return this.fieldbytes[offset];
                    else if (column.DataType == typeof(short))
                        return BitConverter.ToInt16(this.fieldbytes, offset);
                    else if (column.DataType == typeof(ushort))
                        return BitConverter.ToUInt16(this.fieldbytes, offset);
                    else if (column.DataType == typeof(int))
                        return BitConverter.ToInt32(this.fieldbytes, offset);
                    else if (column.DataType == typeof(uint))
                        return BitConverter.ToUInt32(this.fieldbytes, offset);
                    else if (column.DataType == typeof(long))
                        return BitConverter.ToInt64(this.fieldbytes, offset);
                    else if (column.DataType == typeof(ulong))
                        return BitConverter.ToUInt64(this.fieldbytes, offset);
                    else if (column.DataType == typeof(char))
                        return BitConverter.ToChar(this.fieldbytes, offset);
                    else if (column.DataType == typeof(float))
                        return BitConverter.ToSingle(this.fieldbytes, offset);
                    else if (column.DataType == typeof(double))
                        return BitConverter.ToDouble(this.fieldbytes, offset);
                    else if (column.DataType == typeof(DateTime))
                        return new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(Convert.ToDouble(BitConverter.ToInt64(this.fieldbytes, offset)));
                    else if (column.DataType == typeof(TimeSpan))
                    {
                        if (this.Table.Reader.Version == FileHeader.defaultMagicValue)
                            return new TimeSpan(BitConverter.ToInt64(this.fieldbytes, offset));
                        return new TimeSpan(BitConverter.ToInt32(this.fieldbytes, offset));
                    }
                }
                throw new Exception();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool HasValue(IColumn column)
        {
            int offset = BitConverter.ToInt32(this.fieldbytes, sizeof(int) * column.Index);
            return offset != 0;
        }

        #region IRow

        ITable IRow.Table
        {
            get { return this.Table; }
        }

        object IRow.this[string columnName]
        {
            get
            {
                return this[this.Table.Columns[columnName]];
            }
        }

        object IRow.this[int columnIndex]
        {
            get
            {
                return this[this.Table.Columns[columnIndex]];
            }
        }

        bool IRow.HasValue(string columnName)
        {
            IColumn column = this.Table.Columns[columnName];
            if (column == null)
                throw new KeyNotFoundException(string.Format("'{0}'은(는) '{1}'에 존재하지 않는 열입니다.", columnName, this.Table.Name));
            return this.HasValue(column);
        }

        bool IRow.HasValue(int columnIndex)
        {
            IColumn column = this.Table.Columns[columnIndex];
            if (column == null)
                throw new ArgumentOutOfRangeException(string.Format("'{0}'번째 열은 '{1}'에 존재하지 않습니다.", columnIndex, this.Table.Name));
            return this.HasValue(column);
        }

        #endregion
    }
}
