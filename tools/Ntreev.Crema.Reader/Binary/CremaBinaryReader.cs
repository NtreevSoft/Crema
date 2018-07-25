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
using System.IO;
using System.Linq;
using System.Text;

using Ntreev.Crema.Reader.IO;
using Ntreev.Crema.Reader.Internal;

namespace Ntreev.Crema.Reader.Binary
{
    class CremaBinaryReader : IDataSet
    {
        private Stream stream;
        private TableIndex[] tableIndexes;
        private ReadOptions options;
        private CremaBinaryTableCollection tables;
        private int version;
        private string revision;
        private string name;
        private string typesHashValue;
        private string tablesHashValue;
        private string tags;

        public CremaBinaryReader()
        {

        }

        public ITableCollection Tables
        {
            get { return this.tables; }
        }

        public ReadOptions Options
        {
            get { return this.options; }
        }

        public bool CaseSensitive
        {
            get { return (this.options & ReadOptions.CaseSensitive) == ReadOptions.CaseSensitive; }
        }

        public string Revision
        {
            get { return this.revision; }
        }

        public int Version
        {
            get { return this.version; }
        }

        public string TypesHashValue
        {
            get { return this.typesHashValue; }
        }

        public string TablesHashValue
        {
            get { return this.tablesHashValue; }
        }

        public string Tags
        {
            get { return this.tags; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public CremaBinaryTable ReadTable(string tableName)
        {
            int index = -1;

            for (int i = 0; i < this.tableIndexes.Length; i++)
            {
                TableIndex tableIndex = this.tableIndexes[i];

                if (StringResource.Equals(tableIndex.TableName, tableName) == true)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                throw new KeyNotFoundException("테이블을 찾을수 없습니다.");

            CremaBinaryTable table = this.ReadTable(new BinaryReader(this.stream), this.tableIndexes[index].Offset);
            this.tables[index] = table;
            return table;
        }

        public CremaBinaryTable ReadTable(int index)
        {
            TableIndex table_index = this.tableIndexes[index];
            CremaBinaryTable table = this.ReadTable(new BinaryReader(this.stream), table_index.Offset);
            this.tables[index] = table;
            return table;
        }

        public void Read(Stream stream, ReadOptions options)
        {
            this.ReadCore(stream, options);
        }

        protected void ReadCore(Stream stream, ReadOptions options)
        {
            this.stream = stream;
            this.options = options;

            var reader = new BinaryReader(stream);

            stream.Seek(0, SeekOrigin.Begin);
            var fileHeader = reader.ReadValue<FileHeader>();
            this.tableIndexes = reader.ReadValues<TableIndex>(fileHeader.TableCount);
            this.version = fileHeader.MagicValue;
            
            stream.Seek(fileHeader.StringResourcesOffset, SeekOrigin.Begin);
            StringResource.Read(reader);
            this.revision = StringResource.GetString(fileHeader.Revision);
            this.name = StringResource.GetString(fileHeader.Name);
            this.tables = new CremaBinaryTableCollection(this, this.tableIndexes);
            this.typesHashValue = StringResource.GetString(fileHeader.TypesHashValue);
            this.tablesHashValue = StringResource.GetString(fileHeader.TablesHashValue);
            this.tags = StringResource.GetString(fileHeader.Tags);

            for (var i = 0; i < this.tableIndexes.Length; i++)
            {
                var tableIndex = this.tableIndexes[i];
                var table = this.ReadTable(reader, tableIndex.Offset);
                this.tables[i] = table;
            }
        }

        private CremaBinaryTable ReadTable(BinaryReader reader, long offset)
        {
            TableHeader tableHeader;
            TableInfo tableInfo;

            reader.Seek(offset, SeekOrigin.Begin);
            reader.ReadValue(out tableHeader);

            reader.Seek(tableHeader.TableInfoOffset + offset, SeekOrigin.Begin);
            reader.ReadValue(out tableInfo);

            var table = new CremaBinaryTable(this, tableInfo.RowCount, this.options);

            reader.Seek(tableHeader.StringResourcesOffset + offset, SeekOrigin.Begin);
            StringResource.Read(reader);

            reader.Seek(tableHeader.ColumnsOffset + offset, SeekOrigin.Begin);
            this.ReadColumns(reader, table, tableInfo.ColumnCount);

            reader.Seek(tableHeader.RowsOffset + offset, SeekOrigin.Begin);
            this.ReadRows(reader, table, tableInfo.RowCount);

            table.Name = StringResource.GetString(tableInfo.TableName);
            table.Category = StringResource.GetString(tableInfo.CategoryName);
            table.HashValue = StringResource.GetString(tableHeader.HashValue);

            return table;
        }

        private void ReadRows(BinaryReader reader, CremaBinaryTable table, int rowCount)
        {
            for (var i = 0; i < rowCount; i++)
            {
                var dataRow = table.Rows[i];
                var length = reader.ReadInt32();
                dataRow.fieldbytes = reader.ReadBytes(length);
                dataRow.Table = table;
            }
        }

        private void ReadColumns(BinaryReader reader, CremaBinaryTable table, int columnCount)
        {
            var keys = new List<IColumn>();
            var columns = new CremaBinaryColumnCollection(table, columnCount, this.CaseSensitive);
            for (var i = 0; i < columnCount; i++)
            {
                var columninfo = reader.ReadValue<ColumnInfo>();
                var columnName = StringResource.GetString(columninfo.ColumnName);
                var typeName = StringResource.GetString(columninfo.DataType);
                var isKey = columninfo.Iskey == 0 ? false : true;

                var column = new CremaBinaryColumn(columnName, Utility.NameToType(typeName), isKey);
                columns.Add(column);
                if (isKey == true)
                    keys.Add(column);

                column.Table = table;
            }
            table.Columns = columns;
            table.Keys = keys.ToArray();
        }
    }
}
