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

using Ntreev.Crema.Data;
using Ntreev.Crema.Services;
using Ntreev.Library.Random;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Ntreev.Library.ObjectModel;

namespace Ntreev.Crema.Services.Random
{
    public static class TableRowExtensions
    {
        public static bool InitializeRandom(this ITableRow tableRow, Authentication authentication)
        {
            var content = tableRow.Content;
            var table = content.Table;
            foreach (var item in table.TableInfo.Columns)
            {
                if (tableRow[item.Name] != null && item.IsUnique == false)
                    continue;
                if (SetRandomValue(tableRow, authentication, item.Name) == false)
                    return false;
            }
            return true;
        }

        public static bool SetRandomValue(this ITableRow tableRow, Authentication authentication)
        {
            var content = tableRow.Content;
            var table = content.Table;
            var columnInfo = table.TableInfo.Columns.Random();
            var value = GetRandomValue(tableRow, columnInfo.Name);
            if (value == null)
                return false;
            tableRow.SetField(authentication, columnInfo.Name, value);
            return true;
        }

        public static bool SetRandomValue(this ITableRow tableRow, Authentication authentication, int tryCount)
        {
            var count = 0;
            for (var i = 0; i < tryCount; i++)
            {
                try
                {
                    if (SetRandomValue(tableRow, authentication) == true)
                        count++;
                }
                catch
                {

                }
            }
            return count > 0;
        }

        public static bool SetRandomValue(this ITableRow tableRow, Authentication authentication, string columnName)
        {
            var value = GetRandomValue(tableRow, columnName);
            if (value == null)
                return false;

            var domain = tableRow.Content.Domain;
            var table = tableRow.Content.Table;
            var dataSet = domain.Source as CremaDataSet;
            var dataTable = dataSet.Tables[table.Name, table.Category.Path];

            if (dataTable.Columns[columnName].Unique == true && value is TimeSpan == false)
            {
                var text = $"{value}";
                if (value is string || value is DateTime)
                    text = $"'{value}'";
                var items = dataTable.Select($"{columnName}={text}");
                if (items.Any() == true)
                    return false;
            }

            tableRow.SetField(authentication, columnName, value);
            return true;
        }

        private static object GetRandomValue(this ITableRow tableRow, string columnName)
        {
            var content = tableRow.Content;
            var table = content.Table;
            var columnInfo = table.TableInfo.Columns.Where(item => item.Name == columnName).First();

            if (CremaDataTypeUtility.IsBaseType(columnInfo.DataType) == true)
            {
                var type = CremaDataTypeUtility.GetType(columnInfo.DataType);
                return RandomUtility.Next(type);
            }
            else
            {
                var typeContext = table.GetService(typeof(ITypeContext)) as ITypeContext;
                var type = typeContext[columnInfo.DataType] as IType;
                return type.GetRandomValue();
            }
        }
    }
}