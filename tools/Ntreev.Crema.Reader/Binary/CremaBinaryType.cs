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
using System.Runtime.InteropServices;
using System.Text;

namespace Ntreev.Crema.Reader.Binary
{
    struct TableHeader
    {
        public const int defaultMagicValue = 0x04000000;

        public int MagicValue { get; set; }
        public int HashValue { get; set; }
        public long ModifiedTime { get; set; }
        public long TableInfoOffset { get; set; }
        public long ColumnsOffset { get; set; }
        public long RowsOffset { get; set; }
        public long StringResourcesOffset { get; set; }
        public long UserOffset { get; set; }
    }

    struct TableInfo
    {
        public int TableName { get; set; }
        public int CategoryName { get; set; }
        public int ColumnCount { get; set; }
        public int RowCount { get; set; }
    }

    struct ColumnInfo
    {
        public int ColumnName { get; set; }
        public int DataType { get; set; }
        public int Iskey { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FileHeader
    {
        public const int defaultMagicValue = 0x04000000;

        public int MagicValue { get; set; }
        public int Revision { get; set; }
        public int TypesHashValue { get; set; }
        public int TablesHashValue { get; set; }
        public int Tags { get; set; }
        public int Reserved { get; set; }
        public int TableCount { get; set; }
        public int Name { get; set; }
        public long IndexOffset { get; set; }
        public long TablesOffset { get; set; }
        public long StringResourcesOffset { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct TableIndex
    {
        public int TableName { get; set; }
        public int Dummy { get; set; }
        public long Offset { get; set; }
    }
}
