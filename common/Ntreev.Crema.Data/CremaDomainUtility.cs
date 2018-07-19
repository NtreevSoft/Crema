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
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Data
{
    public static class CremaDomainUtility
    {
        static CremaDomainUtility()
        {

        }

        public static DataRowView AddNew(DataView view, object[] fields)
        {
            var rowView = view.AddNew();
            var table = view.Table;

            try
            {
                for (var i = 0; i < table.Columns.Count; i++)
                {
                    var field = fields[i];
                    if (field == null)
                        continue;

                    rowView[i] = field;
                }

                rowView.EndEdit();
            }
            catch (Exception e)
            {
                rowView.CancelEdit();
                throw e;
            }

            return rowView;
        }

        public static void Delete(DataView view, object[] keys)
        {
            var table = view.Table;
            var row = table.Rows.Find(keys);
            if (row != null)
                row.Delete();
        }

        public static object[] GetFields(DataRowView rowView)
        {
            var row = rowView.Row;
            var table = row.Table;

            var fields = new object[table.Columns.Count];
            for (var i = 0; i < table.Columns.Count; i++)
            {
                var field = row[i];
                if (field == DBNull.Value)
                    continue;

                fields[i] = field;
            }

            return fields;
        }

        public static object[] GetFields(DataRowView rowView, string fieldName, object value)
        {
            var row = rowView.Row;
            var table = row.Table;

            var fields = new object[table.Columns.Count];
            var column = table.Columns[fieldName];
            fields[column.Ordinal] = value;

            return fields;
        }

        public static object[] GetInsertionFields(DataRowView parentRowView, string tableName)
        {
            var parentRow = parentRowView.Row;
            var parentTable = parentRow.Table;
            var dataSet = parentTable.DataSet;
            var table = dataSet.Tables[tableName];

            var items = new object[table.Columns.Count];

            foreach (DataRelation relation in dataSet.Relations)
            {
                if (relation.ChildTable == table)
                {
                    for (var i = 0; i < relation.ParentColumns.Length; i++)
                    {
                        var f = parentRow[relation.ParentColumns[i]];
                        var childColumn = relation.ChildColumns[i];
                        items[childColumn.Ordinal] = f;
                    }
                }
            }

            return items;
        }

        public static object[] GetResetFields(DataRowView rowView, string[] fieldNames)
        {
            var row = rowView.Row;
            var table = row.Table;

            var fields = new object[table.Columns.Count];
            for (var i = 0; i < fieldNames.Length; i++)
            {
                var column = table.Columns[fieldNames[i]];
                fields[column.Ordinal] = DBNull.Value;
            }

            return fields;
        }

        public static object[] SetFields(DataView view, object[] keys, object[] fields)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));
            if (fields.Length != view.Table.Columns.Count)
                throw new ArgumentException("필드 갯수가 테이블의 열의 갯수와 일치하지 않습니다");
            var table = view.Table;
            var row = table.Rows.Find(keys);

            if (row == null)
                throw new ArgumentException($"{string.Join(",", keys)} 값의 행을 찾을 수 없습니다.");

            var originalFields = new object[row.ItemArray.Length];
            row.ItemArray.CopyTo(originalFields, 0);

            row.BeginEdit();
            for (var i = 0; i < table.Columns.Count; i++)
            {
                var field = fields[i];
                if (field == null)
                    continue;

                var column = table.Columns[i];
                if (field == DBNull.Value && column.AllowDBNull == false)
                    throw new System.Data.NoNullAllowedException($"'{column.ColumnName} 열에 null 을 사용할 수 없습니다.");

                row[i] = field;
            }
            row.EndEdit();

            for (var i = 0; i < table.Columns.Count; i++)
            {
                var column = table.Columns[i];
                fields[i] = row[column];
            }
            return fields;
        }

        public static void SetFieldsForce(DataView view, object[] keys, object[] fields)
        {
            var table = view.Table;
            var row = table.Rows.Find(keys);

            if (fields.Length != table.Columns.Count)
                throw new CremaDataException();

            row.BeginEdit();
            for (var i = 0; i < table.Columns.Count; i++)
            {
                if (fields[i] == null)
                    continue;

                var column = table.Columns[i];
                var field = fields[i];

                if (object.Equals(row[column], field) == true)
                    continue;

                if (column.ReadOnly == true)
                {
                    row.SetReadOnlyField(column, field);
                }
                else
                {
                    row[column] = field;
                }
            }
            row.EndEdit();
        }

        public static object[] GetKeys(DataRowView rowView)
        {
            var row = rowView.Row;
            var table = row.Table;

            return (from item in table.PrimaryKey select row[item]).ToArray();
        }

        public static object[] GetKeys(this DataRow row)
        {
            var table = row.Table;
            var keys = from DataColumn item in table.PrimaryKey select row[item];
            return keys.ToArray();
        }
    }
}
