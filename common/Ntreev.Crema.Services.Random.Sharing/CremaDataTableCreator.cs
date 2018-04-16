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

using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.Text;
using Ntreev.Crema.Data;
using Ntreev.Library.Random;

namespace Ntreev.Crema.Services.Random
{
    public static class CremaDataTableCreator
    {
        public static CremaDataTable CreateNormalTable(CremaDataSet dataSet, string tableName, string categoryPath)
        {
            var table = dataSet.Tables.Add(tableName, categoryPath);
            var column = table.Columns.Add("ID", typeof(int));
            column.IsKey = true;
            column.AutoIncrement = true;
            foreach (var item in CremaDataTypeUtility.GetBaseTypes())
            {
                table.Columns.Add($"column_{item.GetTypeName()}", item);
            }
            foreach (var item in dataSet.Types)
            {
                table.Columns.Add($"column_{item.Name}", item);
            }
            CreateNormalChild(table, "child_table");
            FillTable(table);
            return table;
        }

        public static CremaDataTable CreateNormalChild(CremaDataTable dataTable, string childName)
        {
            var child = dataTable.Childs.Add(childName);
            child.Columns.Add("ID", typeof(int)).IsKey = true;
            foreach (var item in CremaDataTypeUtility.GetBaseTypes())
            {
                child.Columns.Add($"column_{item.GetTypeName()}", item);
            }
            return child;
        }

        public static void FillTable(CremaDataTable dataTable)
        {
            for (var i = 0; i < 10000; i++)
            {
                var row = dataTable.NewRow();
                row.FillFields();
                dataTable.Rows.Add(row);
                
            }    
        }

        public static void FillRow(CremaDataRow dataRow)
        {
            var table = dataRow.Table;

            foreach (var item in table.Columns)
            {
                if (item.CremaType != null)
                {
                    dataRow[item] = item.CremaType.GetRandomValue();
                }
                else
                {
                    dataRow[item] = RandomUtility.Next(item.DataType);
                }
            }

        }

        
    }
}