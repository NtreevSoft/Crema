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

namespace Ntreev.Crema.Data
{
    static class InternalExtensions
    {
        public static void SetReadOnlyField<T>(this DataRow row, string columnName, T value)
        {
            var column = row.Table.Columns[columnName];
            if (column.ColumnName != columnName)
                throw new ArgumentException(nameof(columnName));
            row.SetReadOnlyField<T>(column, value);
        }

        public static void SetReadOnlyField<T>(this DataRow row, DataColumn column, T value)
        {
            if (column.ReadOnly == false)
                throw new ArgumentException();
            try
            {
                column.ReadOnly = false;
                row.SetField<T>(column, value);
            }
            finally
            {
                column.ReadOnly = true;
            }
        }

        public static void SetForceField<T>(this DataRow row, InternalDataColumn column, T value, object editor)
        {
            if (column.Editor != null)
                throw new ArgumentException();

            try
            {
                column.Editor = editor;
                row.SetField(column, value);
            }
            finally
            {
                column.Editor = null;
            }
        }

        public static bool HasChanges(this DataRow row, DataColumn column)
        {
            if (row.HasVersion(DataRowVersion.Original) == false)
                return false;

            var oldValue = row[column, DataRowVersion.Original];
            var newValue = row[column];

            return object.Equals(oldValue, newValue) == false;
        }

        public static bool IsIncluded(this DataRow dataRow)
        {
            return dataRow.RowState != DataRowState.Deleted && dataRow.RowState != DataRowState.Detached;
        }

        public static bool IsDeleted(this DataRow dataRow)
        {
            return dataRow.RowState == DataRowState.Deleted || dataRow.RowState == DataRowState.Detached;
        }
    }
}
