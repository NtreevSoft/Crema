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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Data;
using Ntreev.Library.IO;

namespace Ntreev.Crema.Runtime.Serialization.Binary
{
    [Export(typeof(IDataSerializer))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class OrderedBinaryDataSerializer : IDataSerializer
    {
        public const int formatterVersion = 0;

        private BinaryTableHeader tableHeader = new BinaryTableHeader();
        private BinaryTableInfo tableInfo = new BinaryTableInfo();
        private IDictionary strings = new OrderedDictionary();
        private IDictionary<string, int> stringsRef = new Dictionary<string, int>();
        private List<BinaryColumnInfo> columns;
        private int stringsIndex = 0;

        [ImportingConstructor]
        public OrderedBinaryDataSerializer()
        {

        }

        public string Name
        {
            get { return "seq-bin"; }
        }

        public void Serialize(Stream stream, SerializationSet dataSet)
        {
            this.stringsIndex = 0;

            var fileHeader = new BinaryFileHeader();
            var tables = dataSet.Tables;
            var tableIndexes = new List<BinaryTableIndex>(tables.Length);

            var writer = new BinaryWriter(stream);
            writer.WriteValue(fileHeader);

            fileHeader.MagicValue = BinaryFileHeader.DefaultMagicValue;
            fileHeader.IndexOffset = stream.Position;

            writer.Seek(Marshal.SizeOf(typeof(BinaryTableIndex)) * tables.Length, SeekOrigin.Current);
            fileHeader.TypesHashValue = this.GetStringID(dataSet.TypesHashValue);
            fileHeader.TablesHashValue = this.GetStringID(dataSet.TablesHashValue);
            fileHeader.Tags = this.GetStringID(dataSet.Tags.ToLower());
            fileHeader.TablesOffset = stream.Position;
            fileHeader.TableCount = tables.Length;
            fileHeader.Revision = (int)dataSet.Revision;
            fileHeader.Name = this.GetStringID(dataSet.Name);

            var t = new Dictionary<string, Stream>();

            tables.ToList().ForEach(item =>
            {
                var memory = new MemoryStream();
                var formatter = new OrderedBinaryDataSerializer();
                formatter.SerializeTable(memory, item, dataSet.Types);
                memory.Position = 0;
                lock (t)
                {
                    t.Add(item.Name, memory);
                }
            });

            foreach (var item in tables)
            {
                var tableIndex = new BinaryTableIndex()
                {
                    TableName = this.GetStringID(item.Name),
                    Offset = stream.Position
                };
                tableIndexes.Add(tableIndex);
                t[item.Name].CopyTo(stream);
            }

            fileHeader.StringResourcesOffset = stream.Position;
            writer.WriteResourceStrings(this.strings.ToArray<int, string>());

            writer.Seek(0, SeekOrigin.Begin);
            writer.WriteValue(fileHeader);

            writer.Seek((int)fileHeader.IndexOffset, SeekOrigin.Begin);
            writer.WriteArray(tableIndexes.ToArray());
        }

        private void SerializeTable(Stream stream, SerializationTable dataTable, SerializationType[] types)
        {
            var columns = dataTable.Columns;
            var rows = dataTable.Rows;
            this.tableHeader.MagicValue = (int)dataTable.Revision;
            this.tableHeader.HashValue = this.GetStringID(dataTable.HashValue);

            this.tableInfo.TableName = this.GetStringID(dataTable.Name);
            this.tableInfo.CategoryName = this.GetStringID(dataTable.CategoryPath);
            this.tableInfo.ColumnsCount = columns.Length;
            this.tableInfo.RowsCount = rows.Length;

            this.CollectColumns(columns);

            var writer = new BinaryWriter(stream);

            writer.WriteValue(this.tableHeader);

            this.tableHeader.TableInfoOffset = writer.GetPosition();
            writer.WriteValue(this.tableInfo);
            this.tableHeader.ColumnsOffset = writer.GetPosition();

            writer.WriteArray(this.columns.ToArray());

            this.tableHeader.RowsOffset = writer.GetPosition();
            this.WriteRows(writer, rows, columns, types);

            this.tableHeader.StringResourcesOffset = writer.GetPosition();
            writer.WriteResourceStrings(this.strings.ToArray<int, string>());

            this.tableHeader.UserOffset = writer.GetPosition();
            writer.Write((byte)0);

            var lastPosition = writer.GetPosition();
            writer.Seek(0, SeekOrigin.Begin);
            writer.WriteValue(this.tableHeader);
            writer.Seek((int)this.tableHeader.TableInfoOffset, SeekOrigin.Begin);
            writer.WriteValue(this.tableInfo);
            writer.SetPosition(lastPosition);
        }

        private void CollectColumns(SerializationColumn[] columns)
        {
            this.columns = new List<BinaryColumnInfo>(columns.Length);

            foreach (var item in columns)
            {
                var columnInfo = new BinaryColumnInfo()
                {
                    ColumnName = this.GetStringID(item.Name),
                    DataType = this.GetStringID(item.DataType),
                    IsKey = item.IsKey ? 1 : 0
                };
                this.columns.Add(columnInfo);
            }
        }

        private void AdjustSeek(BinaryWriter writer, Type type, ref int position)
        {
            if (type == typeof(bool))
            {
                position += sizeof(bool);
            }
            else if (type == typeof(short))
            {
                writer.Seek(position % 2, SeekOrigin.Current);
                position += sizeof(short);
            }
            else if (type == typeof(long))
            {
                writer.Seek(position % 8, SeekOrigin.Current);
                position += sizeof(short);
            }
            else
            {
                writer.Seek(position % 4, SeekOrigin.Current);
            }
        }

        private void WriteRows(BinaryWriter writer, SerializationRow[] rows, SerializationColumn[] columns, SerializationType[] types)
        {
            foreach (var item in rows)
            {
                this.WriteRow(writer, item, columns, types);
            }
        }

        private void WriteRow(BinaryWriter writer, SerializationRow dataRow, SerializationColumn[] columns, SerializationType[] types)
        {
            var headerPosition = writer.GetPosition();
            var valueOffsets = new int[columns.Length];
            var byteLength = 0;
            writer.Write(byteLength);
            var fieldPosition = writer.GetPosition();
            writer.WriteArray(valueOffsets);

            for (var i = 0; i < columns.Length; i++)
            {
                var dataColumn = columns[i];
                var value = dataRow.Fields[i];

                if (value == null || value == DBNull.Value)
                    continue;

                valueOffsets[i] = (int)(writer.GetPosition() - fieldPosition);
                this.WriteField(writer, dataColumn, value, types);
            }

            var lastPosition = writer.GetPosition();
            byteLength = (int)(lastPosition - fieldPosition);

            writer.SetPosition(headerPosition);
            writer.WriteValue(byteLength);
            writer.WriteArray(valueOffsets);
            writer.SetPosition(lastPosition);
        }

        private void WriteField(BinaryWriter writer, SerializationColumn dataColumn, object value, SerializationType[] types)
        {
            if (dataColumn.DataType == typeof(bool).GetTypeName())
            {
                writer.Write((bool)value);
            }
            else if (dataColumn.DataType == typeof(string).GetTypeName())
            {
                writer.Write(this.GetStringID(value as string));
            }
            else if (dataColumn.DataType == typeof(float).GetTypeName())
            {
                writer.Write((float)value);
            }
            else if (dataColumn.DataType == typeof(double).GetTypeName())
            {
                writer.Write((double)value);
            }
            else if (dataColumn.DataType == typeof(sbyte).GetTypeName())
            {
                writer.Write((sbyte)value);
            }
            else if (dataColumn.DataType == typeof(byte).GetTypeName())
            {
                writer.Write((byte)value);
            }
            else if (dataColumn.DataType == typeof(int).GetTypeName())
            {
                writer.Write((int)value);
            }
            else if (dataColumn.DataType == typeof(uint).GetTypeName())
            {
                writer.Write((uint)value);
            }
            else if (dataColumn.DataType == typeof(short).GetTypeName())
            {
                writer.WriteValue((short)value);
            }
            else if (dataColumn.DataType == typeof(ushort).GetTypeName())
            {
                writer.WriteValue((ushort)value);
            }
            else if (dataColumn.DataType == typeof(long).GetTypeName())
            {
                writer.Write((long)value);
            }
            else if (dataColumn.DataType == typeof(ulong).GetTypeName())
            {
                writer.Write((ulong)value);
            }
            else if (dataColumn.DataType == typeof(DateTime).GetTypeName())
            {
                var dateTime = (DateTime)value;
                var delta = dateTime - new DateTime(1970, 1, 1);
                var seconds = Convert.ToInt64(delta.TotalSeconds);
                writer.Write(seconds);
            }
            else if (dataColumn.DataType == typeof(TimeSpan).GetTypeName())
            {
                if (value is TimeSpan timeSpanValue)
                    writer.Write(timeSpanValue.Ticks);
                else if (value is long longValue)
                    writer.Write(longValue);
                else
                    throw new InvalidOperationException($"{value.GetType()} is invalid timespan type");
            }
            else if (dataColumn.DataType == typeof(string).GetTypeName())
            {
                writer.Write(this.GetStringID(value.ToString()));
            }
            else
            {
                var type = types.First(item => dataColumn.DataType == item.CategoryPath + item.Name);
                writer.Write((int)type.ConvertFromString(value.ToString()));
            }
        }

        private int GetStringID(string text)
        {
            lock (this.strings)
            {
                text = text.Replace(Environment.NewLine, "\n");
                if (this.stringsRef.ContainsKey(text) == false)
                {
                    this.stringsIndex++;
                    this.strings.Add(this.stringsIndex, text);
                    this.stringsRef.Add(text, this.stringsIndex);
                    return this.stringsIndex;
                }

                return this.stringsRef[text];
            }
        }
    }

    static class IDictionaryExtension
    {
        public static KeyValuePair<TKey, TValue>[] ToArray<TKey, TValue>(this IDictionary dictionary)
        {
            var index = 0;
            var entries = new KeyValuePair<TKey, TValue>[dictionary.Count];

            foreach (DictionaryEntry entry in dictionary)
            {
                entries[index++] = new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value);
            }

            return entries;
        }
    }
}
