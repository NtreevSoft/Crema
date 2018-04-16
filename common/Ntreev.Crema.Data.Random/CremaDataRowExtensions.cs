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

using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Random
{
    public static class CremaDataRowExtensions
    {
        static CremaDataRowExtensions()
        {

        }

        public static bool InitializeRandom(this CremaDataRow dataRow)
        {
            if (dataRow.RowState != System.Data.DataRowState.Detached)
                throw new ArgumentException();

            foreach (var item in dataRow.Table.Columns)
            {
                if (dataRow[item.ColumnName] != DBNull.Value && item.Unique == false && item.IsKey == false)
                    continue;
                if (SetRandomValue(dataRow, item) == false)
                    return false;
            }
            return true;
        }

        public static bool SetRandomValue(this CremaDataRow dataRow)
        {
            var dataColumn = dataRow.Table.Columns.Random();
            var value = GetRandomValue(dataRow, dataColumn);
            if (dataRow.RowState != System.Data.DataRowState.Detached && dataColumn.ReadOnly == true)
                return false;
            if (value == null)
                return false;
            dataRow.SetField(dataColumn, value);
            return true;
        }

        public static bool SetRandomValue(this CremaDataRow dataRow, int tryCount)
        {
            var count = 0;
            for (var i = 0; i < tryCount; i++)
            {
                try
                {
                    if (SetRandomValue(dataRow) == true)
                        count++;
                }
                catch
                {

                }
            }
            return count > 0;
        }

        public static bool SetRandomValue(this CremaDataRow dataRow, CremaDataColumn dataColumn)
        {
            if (dataRow.RowState != System.Data.DataRowState.Detached && dataColumn.ReadOnly == true)
                return true;
            var value = GetRandomValue(dataRow, dataColumn);
            if (value == null)
                return false;
            dataRow.SetField(dataColumn, value);
            return true;
        }

        private static object GetRandomValue(this CremaDataRow dataRow, CremaDataColumn dataColumn)
        {
            var value = dataColumn.GetRandomValue();

            if (dataColumn.Unique == true)
            {
                var tryCount = 0;
                while (tryCount < 5)
                {
                    //var textValue = CremaConvert.ChangeType(value, typeof(string));
                    //var expression = value == DBNull.Value ? $"[{dataColumn.ColumnName}] is null" : $"{dataColumn.ColumnName}='{textValue}'";
                    //var items = dataColumn.Table.Select(expression);
                    //if (items.Any() == false)
                    //    return value;
                    if (Exists() == false)
                        return value;
                    value = dataColumn.GetRandomValue();
                    tryCount++;

                    bool Exists()
                    {
                        foreach (var item in dataColumn.Table.Rows)
                        {
                            if (object.Equals(item[dataColumn], value) == true)
                                return true;
                        }
                        return false;
                    }
                }
                return null;
            }
            else
            {
                return value;
            }
        }
    }
}
