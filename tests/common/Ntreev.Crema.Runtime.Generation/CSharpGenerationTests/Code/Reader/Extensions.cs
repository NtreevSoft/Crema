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
using System.Text;

namespace Ntreev.Crema.Code.Reader
{
    public static class ReaderExtensions
    {
        public const string __RelationID__ = "__RelationID__";
        public const string __ParentID__ = "__ParentID__";

        public static IRow Find(this IRowCollection rows, params object[] keyValues)
        {
            var keys = rows.Table.Keys;
            if (keys.Length != keyValues.Length)
                throw new ArgumentException("키 갯수가 맞지 않습니다.", "keyValues");

            var query = from item in rows
                        where IsEqual(item.GetKeys(), keyValues)
                        select item;

            return query.Single();
        }

        public static IRow GetParentRow(this IRow row)
        {
            var parentTable = row.Table.GetParentTable();
            if (parentTable == null)
                return null;

            var query = from item in parentTable.Rows
                        where item.ToInt32(__RelationID__) == row.ToInt32(__ParentID__)
                        select item;

            return query.Single();
        }

        public static IEnumerable<IRow> GetChildRows(this IRow row, ITable childTable)
        {
            if (row.Table != childTable.GetParentTable())
                throw new ArgumentException("잘못된 자식 테이블입니다.");

            return from item in childTable.Rows
                   where row.ToInt32(__RelationID__) == item.ToInt32(__ParentID__)
                   select item;
        }

        public static IEnumerable<IRow> GetChildRows(this IRow row, string childTableName)
        {
            if (row.Table.DataSet.Tables.Contains(childTableName) == false)
                throw new ArgumentException("존재하지 않은 테이블입니다.");

            var childTable = row.Table.DataSet.Tables[childTableName];
            if (row.Table != childTable.GetParentTable())
                throw new ArgumentException("잘못된 자식 테이블입니다.");

            return from item in childTable.Rows
                   where row.ToInt32(__RelationID__) == item.ToInt32(__ParentID__)
                   select item;
        }

        public static IEnumerable<ITable> GetChildTables(this ITable table)
        {
            return from item in table.DataSet.Tables
                   where GetParentName(item.Name) == table.Name
                   select item;
        }

        public static ITable GetParentTable(this ITable table)
        {
            var parentName = GetParentName(table.Name);
            if (parentName == string.Empty)
                return null;
            return table.DataSet.Tables[parentName];
        }

        public static T Field<T>(this IRow row, string columnName)
        {
            return row.Field<T>(row.Table.Columns[columnName]);
        }

        public static T Field<T>(this IRow row, int columnIndex)
        {
            return row.Field<T>(row.Table.Columns[columnIndex]);
        }

        public static T Field<T>(this IRow row, IColumn column)
        {
            return (T)row[column];
        }

        public static object GetValue(this IRow row, int columnIndex)
        {
            return row[columnIndex];
        }

        public static object GetValue(this IRow row, IColumn column)
        {
            return row[column];
        }

        public static object GetValue(this IRow row, string columnName)
        {
            return row[columnName];
        }

        public static bool ToBoolean(this IRow row, string columnName)
        {
            return (bool)row[columnName];
        }

        public static bool ToBoolean(this IRow row, int columnIndex)
        {
            return (bool)row[columnIndex];
        }

        public static bool ToBoolean(this IRow row, IColumn column)
        {
            return (bool)row[column];
        }

        public static string ToString(this IRow row, string columnName)
        {
            return (string)row[columnName];
        }

        public static string ToString(this IRow row, int columnIndex)
        {
            return (string)row[columnIndex];
        }

        public static string ToString(this IRow row, IColumn column)
        {
            return (string)row[column];
        }

        public static float ToSingle(this IRow row, string columnName)
        {
            return (float)row[columnName];
        }

        public static float ToSingle(this IRow row, int columnIndex)
        {
            return (float)row[columnIndex];
        }

        public static float ToSingle(this IRow row, IColumn column)
        {
            return (float)row[column];
        }

        public static double ToDouble(this IRow row, string columnName)
        {
            return (double)row[columnName];
        }

        public static double ToDouble(this IRow row, int columnIndex)
        {
            return (double)row[columnIndex];
        }

        public static double ToDouble(this IRow row, IColumn column)
        {
            return (double)row[column];
        }

        public static sbyte ToInt8(this IRow row, string columnName)
        {
            return (sbyte)row[columnName];
        }

        public static sbyte ToInt8(this IRow row, int columnIndex)
        {
            return (sbyte)row[columnIndex];
        }

        public static sbyte ToInt8(this IRow row, IColumn column)
        {
            return (sbyte)row[column];
        }

        public static byte ToUInt8(this IRow row, string columnName)
        {
            return (byte)row[columnName];
        }

        public static byte ToUInt8(this IRow row, int columnIndex)
        {
            return (byte)row[columnIndex];
        }

        public static byte ToUInt8(this IRow row, IColumn column)
        {
            return (byte)row[column];
        }

        public static short ToInt16(this IRow row, string columnName)
        {
            return (short)row[columnName];
        }

        public static short ToInt16(this IRow row, int columnIndex)
        {
            return (short)row[columnIndex];
        }

        public static short ToInt16(this IRow row, IColumn column)
        {
            return (short)row[column];
        }

        public static ushort ToUInt16(this IRow row, string columnName)
        {
            return (ushort)row[columnName];
        }

        public static ushort ToUInt16(this IRow row, int columnIndex)
        {
            return (ushort)row[columnIndex];
        }

        public static ushort ToUInt16(this IRow row, IColumn column)
        {
            return (ushort)row[column];
        }

        public static int ToInt32(this IRow row, string columnName)
        {
            return (int)row[columnName];
        }

        public static int ToInt32(this IRow row, int columnIndex)
        {
            return (int)row[columnIndex];
        }

        public static int ToInt32(this IRow row, IColumn column)
        {
            return (int)row[column];
        }

        public static uint ToUInt32(this IRow row, string columnName)
        {
            return (uint)row[columnName];
        }

        public static uint ToUInt32(this IRow row, int columnIndex)
        {
            return (uint)row[columnIndex];
        }

        public static uint ToUInt32(this IRow row, IColumn column)
        {
            return (uint)row[column];
        }

        public static long ToInt64(this IRow row, string columnName)
        {
            return (long)row[columnName];
        }

        public static long ToInt64(this IRow row, int columnIndex)
        {
            return (long)row[columnIndex];
        }

        public static long ToInt64(this IRow row, IColumn column)
        {
            return (long)row[column];
        }

        public static ulong ToUInt64(this IRow row, string columnName)
        {
            return (ulong)row[columnName];
        }

        public static ulong ToUInt64(this IRow row, int columnIndex)
        {
            return (ulong)row[columnIndex];
        }

        public static ulong ToUInt64(this IRow row, IColumn column)
        {
            return (ulong)row[column];
        }

        public static DateTime ToDateTime(this IRow row, string columnName)
        {
            return (DateTime)row[columnName];
        }

        public static DateTime ToDateTime(this IRow row, int columnIndex)
        {
            return (DateTime)row[columnIndex];
        }

        public static DateTime ToDateTime(this IRow row, IColumn column)
        {
            return (DateTime)row[column];
        }

        public static TimeSpan ToDuration(this IRow row, string columnName)
        {
            return (TimeSpan)row[columnName];
        }

        public static TimeSpan ToDuration(this IRow row, int columnIndex)
        {
            return (TimeSpan)row[columnIndex];
        }

        public static TimeSpan ToDuration(this IRow row, IColumn column)
        {
            return (TimeSpan)row[column];
        }

        public static Guid ToGuid(this IRow row, int columnIndex)
        {
            return new Guid(row[columnIndex].ToString());
        }

        public static Guid ToGuid(this IRow row, string column)
        {
            return new Guid(row[column].ToString());
        }

        public static IEnumerable<object> GetKeys(this IRow row)
        {
            return row.Table.Keys.Select(item => row[item]).ToArray();
        }

        public static DateTime FromTotalSeconds(long seconds)
        {
            var delta = TimeSpan.FromSeconds(Convert.ToDouble(seconds));
            return new DateTime(1970, 1, 1) + delta;
        }

        private static bool IsEqual(IEnumerable<object> k1, object[] k2)
        {
            return k1.SequenceEqual(k2);
        }

        public static string GetPureName(this ITable table)
        {
            var tableName = table.Name;
            var value = tableName.Split('.');
            if (value.Length == 1)
                return tableName;
            return value[1];
        }

        private static string GetParentName(string name)
        {
            var value = name.Split('.');
            if (value.Length == 1)
                return string.Empty;
            return value[0];
        }
    }
}
