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
    public static class CremaDataTableExtensions
    {
        static CremaDataTableExtensions()
        {
            MinColumnCount = 1;
            MaxColumnCount = 20;
            MinRowCount = 1;
            MaxRowCount = 50;
            ChildPrefix = string.Empty;
            ChildPostfix = "Child";
        }

        public static CremaDataTable CreateRandomTable()
        {
            return CreateRandomTable(string.Empty, string.Empty);
        }

        public static CremaDataTable CreateRandomTable(string prefix, string postfix)
        {
            var dataTable = new CremaDataTable(IdentifierUtility.Next(prefix, postfix));
            dataTable.AddRandomColumns(RandomUtility.Next(MinColumnCount, MaxColumnCount));
            dataTable.AddRandomRows(RandomUtility.Next(MinRowCount, MaxRowCount));
            return dataTable;
        }

        public static CremaDataTable CreateDerivedTable(this CremaDataTable dataTable, string prefix, string postfix)
        {
            var tableName = string.Empty;
            do
            {
                tableName = IdentifierUtility.Next(prefix, postfix);
            }
            while (dataTable.DataSet.Tables.Contains(tableName) == true);

            var derivedTable = dataTable.Inherit(tableName);
            derivedTable.AddRandomRows(RandomUtility.Next(MinRowCount, MaxRowCount));
            return derivedTable;
        }

        public static CremaDataRow AddRandomRow(this CremaDataTable dataTable)
        {
            return AddRandomRow(dataTable, true);
        }

        public static CremaDataRow AddRandomRow(this CremaDataTable dataTable, bool throwOnError)
        {
            var dataRow = null as CremaDataRow;
            if (dataTable.Parent != null)
            {
                var parentRow = dataTable.Parent.Rows.RandomOrDefault();
                if (parentRow == null)
                    return null;
                try
                {
                    dataRow = dataTable.NewRow(parentRow);
                }
                catch (OverflowException)
                {
                    return null;
                }
            }
            else
            {
                try
                {
                    dataRow = dataTable.NewRow();
                }
                catch (OverflowException)
                {
                    return null;
                }
            }

            if (dataRow == null)
                return null;

            if (dataRow.InitializeRandom() == false)
                return null;

            if (RandomUtility.Within(5) == true)
                dataRow.IsEnabled = RandomUtility.NextBoolean();

            try
            {
                dataTable.Rows.Add(dataRow);
            }
            catch
            {
                if (throwOnError == true)
                    throw;
                return null;
            }
            return dataRow;
        }

        public static void AddRandomRows(this CremaDataTable dataTable, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                try
                {
                    AddRandomRow(dataTable);
                }
                catch
                {

                }
            }
        }

        public static void ModifyRandomRow(this CremaDataTable dataTable)
        {
            if (RandomUtility.Within(90) == true)
            {
                var dataRow = dataTable.Rows.RandomOrDefault();
                var dataColumn = dataTable.Columns.RandomOrDefault();
                if (dataRow != null && dataColumn != null)
                {
                    dataRow.SetRandomValue(dataColumn);
                    if (RandomUtility.Within(5) == true)
                        dataRow.IsEnabled = RandomUtility.NextBoolean();
                }
            }
            else if (RandomUtility.Within(75) == true)
            {
                AddRandomRow(dataTable);
            }
            else
            {
                var dataRow = dataTable.Rows.RandomOrDefault();
                if (dataRow != null)
                {
                    if (RandomUtility.Within(50) == true)
                        dataRow.Delete();
                    else
                        dataTable.Rows.Remove(dataRow);
                }
            }
        }

        public static void ModifyRandomRows(this CremaDataTable dataTable, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                try
                {
                    ModifyRandomRow(dataTable);
                }
                catch
                {

                }
            }
        }

        public static CremaAttribute AddRandomAttribute(this CremaDataTable dataTable)
        {
            var attribute = dataTable.Attributes.Add();
            attribute.InitializeRandom();
            return attribute;
        }

        public static void AddRandomAttributes(this CremaDataTable dataTable, int count)
        {
            for (var i = 0; i < count; i++)
            {
                AddRandomAttribute(dataTable);
            }
        }

        public static CremaDataColumn AddRandomColumn(this CremaDataTable dataTable)
        {
            var column = dataTable.Columns.Add();
            column.InitializeRandom();
            return column;
        }

        public static CremaDataColumn AddRandomColumn(this CremaDataTable dataTable, string columnName)
        {
            var column = dataTable.Columns.Add(columnName);
            column.InitializeRandom();
            return column;
        }

        public static void AddRandomColumns(this CremaDataTable dataTable)
        {
            AddRandomColumns(dataTable, RandomUtility.Next(MinColumnCount, MaxColumnCount));
        }

        public static void AddRandomColumns(this CremaDataTable dataTable, int count)
        {
            while (dataTable.Columns.Count < count)
            {
                try
                {
                    AddRandomColumn(dataTable);
                }
                catch
                {

                }
            }
        }

        public static CremaDataTable AddRandomChild(this CremaDataTable dataTable)
        {
            var childTable = dataTable.Childs.Add(IdentifierUtility.Next(ChildPrefix, ChildPostfix));
            childTable.AddRandomColumns(RandomUtility.Next(CremaDataTableExtensions.MinColumnCount, CremaDataTableExtensions.MaxColumnCount));
            childTable.AddRandomRows(RandomUtility.Next(MinRowCount, MaxRowCount));
            return childTable;
        }

        public static void AddRandomChilds(this CremaDataTable dataTable, int count)
        {
            while (dataTable.Childs.Count < count)
            {
                try
                {
                    AddRandomChild(dataTable);
                }
                catch
                {

                }
            }
        }

        public static int MinColumnCount { get; set; }

        public static int MaxColumnCount { get; set; }

        public static int MinRowCount { get; set; }

        public static int MaxRowCount { get; set; }

        public static string ChildPrefix { get; set; }

        public static string ChildPostfix { get; set; }
    }
}
