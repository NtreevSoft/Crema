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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace Ntreev.Crema.Data
{
    public static class CremaDataRowUtility
    {
        public static bool GetEnabled(object component)
        {
            if (component == null)
                return true;

            if (component is DataRowView dataRowView && dataRowView.Row is InternalDataRow dataRow)
                return dataRow.IsEnabled;
            throw new ArgumentException(nameof(component));
        }

        public static TagInfo GetTags(object component)
        {
            if (component == null)
                return TagInfo.All;

            if (component is DataRowView dataRowView && dataRowView.Row is InternalDataRow dataRow)
                return dataRow.Tags;
            throw new ArgumentException(nameof(component));
        }

        public static TagInfo GetParentTags(object component)
        {
            if (component == null)
                return TagInfo.All;

            if (component is DataRowView dataRowView && dataRowView.Row is InternalDataRow dataRow)
            {
                if (dataRow.ParentRow == null)
                    return TagInfo.All;
                return dataRow.ParentRow.Tags;
            }
            throw new ArgumentException(nameof(component));
        }

        public static string GetTableName(object component)
        {
            if (component is DataRowView == false)
                throw new ArgumentException();

            var rowView = component as DataRowView;
            return CremaDataTable.GetTableName(rowView.Row.Table.TableName);
        }

        public static object[] GetKeys(object component)
        {
            if (component is DataRowView == false)
                throw new ArgumentException();

            var rowView = component as DataRowView;
            return CremaDomainUtility.GetKeys(rowView);
        }

        public static object[] GetFields(object component)
        {
            if (component is DataRowView == false)
                throw new ArgumentException();

            return CremaDomainUtility.GetFields(component as DataRowView);
        }

        public static object[] GetFields(object component, string fieldName, object value)
        {
            if (component is DataRowView == false)
                throw new ArgumentException();

            return CremaDomainUtility.GetFields(component as DataRowView, fieldName, value);
        }

        public static object[] GetInsertionFields(object parentComponent, string tableName)
        {
            if (parentComponent is DataRowView == false)
                throw new ArgumentException();

            return CremaDomainUtility.GetInsertionFields(parentComponent as DataRowView, tableName);
        }

        public static object[] GetResetFields(object component, string[] fieldNames)
        {
            if (component is DataRowView == false)
                throw new ArgumentException();

            return CremaDomainUtility.GetResetFields(component as DataRowView, fieldNames);
        }

        public static string[] GetColumnNames(ITypedList typedList)
        {
            var props = typedList.GetItemProperties(null);
            var columnList = new List<string>(props.Count);
            for (var i = 0; i < props.Count; i++)
            {
                if (props[i].PropertyType == typeof(IBindingList))
                    continue;
                columnList.Add(props[i].Name);
            }
            return columnList.ToArray();
        }
    }
}
