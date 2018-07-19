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
    public static class CremaCremaDataRowExtensions
    {
        public static T Field<T>(this CremaDataRow row, CremaDataColumn column)
        {
            return row.InternalObject.Field<T>(column.InternalObject);
        }

        public static T Field<T>(this CremaDataRow row, int columnIndex)
        {
            return row.InternalObject.Field<T>(columnIndex);
        }

        public static T Field<T>(this CremaDataRow row, string columnName)
        {
            return row.InternalObject.Field<T>(columnName);
        }

        public static T Field<T>(this CremaDataRow row, CremaDataColumn column, DataRowVersion version)
        {
            return row.InternalObject.Field<T>(column.InternalObject, version);
        }

        public static T Field<T>(this CremaDataRow row, int columnIndex, DataRowVersion version)
        {
            return row.InternalObject.Field<T>(columnIndex, version);
        }

        public static T Field<T>(this CremaDataRow row, string columnName, DataRowVersion version)
        {
            return row.InternalObject.Field<T>(columnName, version);
        }

        public static void SetField<T>(this CremaDataRow row, CremaDataColumn column, T value)
        {
            row.InternalObject.SetField<T>(column.InternalObject, value);
        }

        public static void SetField<T>(this CremaDataRow row, int columnIndex, T value)
        {
            row.InternalObject.SetField<T>(columnIndex, value);
        }

        public static void SetField<T>(this CremaDataRow row, string columnName, T value)
        {
            row.InternalObject.SetField<T>(columnName, value);
        }

        public static string FieldAsString(this CremaDataRow row, int columnIndex)
        {
            return row.FieldAsString(row.Table.Columns[columnIndex]);
        }

        public static string FieldAsString(this CremaDataRow row, string columnName)
        {
            return row.FieldAsString(row.Table.Columns[columnName]);
        }

        public static string FieldAsString(this CremaDataRow row, CremaDataColumn column)
        {
            object value = row[column];
            if (value == null || value == DBNull.Value)
                return null;
            return value.ToString();
        }

        public static string FieldAsString(this CremaDataRow row, int columnIndex, DataRowVersion version)
        {
            return row.FieldAsString(row.Table.Columns[columnIndex], version);
        }

        public static string FieldAsString(this CremaDataRow row, string columnName, DataRowVersion version)
        {
            return row.FieldAsString(row.Table.Columns[columnName], version);
        }

        public static string FieldAsString(this CremaDataRow row, CremaDataColumn column, DataRowVersion version)
        {
            object value = row[column, version];
            if (value == null || value == DBNull.Value)
                return null;
            return value.ToString();
        }

        public static void SetFieldAsString(this CremaDataRow row, int columnIndex, string textValue)
        {
            row.SetFieldAsString(row.Table.Columns[columnIndex], textValue);
        }

        public static void SetFieldAsString(this CremaDataRow row, string columnName, string textValue)
        {
            row.SetFieldAsString(row.Table.Columns[columnName], textValue);
        }

        public static void SetFieldAsString(this CremaDataRow row, CremaDataColumn column, string textValue)
        {
            row[column] = column.ConvertFromString(textValue);
        }

        internal static T Field<T>(this CremaDataRow row, InternalDataColumn column)
        {
            return row.InternalObject.Field<T>(column);
        }

        internal static T Field<T>(this CremaDataRow row, InternalRelation relation)
        {
            return row.InternalObject.Field<T>(relation);
        }

        internal static void SetFieldForce<T>(this CremaDataRow row, CremaDataColumn column, T value)
        {
            if (column.ReadOnly == true)
                row.InternalObject.SetReadOnlyField<T>(column.InternalObject, value);
            else
                row.InternalObject.SetField<T>(column.InternalObject, value);
        }
    }
}
