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
using Ntreev.Library.Random;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Ntreev.Crema.Services.Random
{
    public static class CremaDataRandomUtility
    {
        public static void GenerateTypes(this CremaDataSet dataSet, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                GenerateType(dataSet);
            }
        }

        public static bool GenerateType(this CremaDataSet dataSet)
        {
            var dataType = dataSet.Types.Add();

            if (RandomUtility.Within(25) == true)
                dataType.IsFlag = RandomUtility.NextBoolean();
            GenerateMembers(dataType, RandomUtility.Next(3, 10));
            if (RandomUtility.Within(25) == true)
                dataType.Comment = RandomUtility.NextString();

            return true;
        }

        public static void GenerateTables(this CremaDataSet dataSet, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                GenerateTable(dataSet);
            }
        }

        public static void GenerateTable(this CremaDataSet dataSet)
        {
            if (RandomUtility.Within(25) == true)
                InheritTable(dataSet);
            else if (RandomUtility.Within(25) == true)
                CopyTable(dataSet);
            else
                CreateTable(dataSet);
        }

        public static void CreateTable(this CremaDataSet dataSet)
        {
            //var category = dataSet.Categories.Random();

            var dataTable = dataSet.Tables.Add();
            GenerateColumns(dataTable, RandomUtility.Next(3, 10));
            if (RandomUtility.Within(25) == true)
                dataTable.Comment = RandomUtility.NextString();
            //dataTable.EndEdit();


            while (RandomUtility.Within(10))
            {
                var childTable = dataTable.Childs.Add();
                GenerateColumns(childTable, RandomUtility.Next(3, 10));
            }

            GenerateRows(dataTable, RandomUtility.Next(10, 100));

            foreach (var item in dataTable.Childs)
            {
                GenerateRows(item, RandomUtility.Next(10, 100));
            }
        }

        public static void InheritTable(this CremaDataSet dataSet)
        {
            var table = dataSet.Tables.RandomOrDefault();

            if (table == null)
                return;

            table.Inherit("Table_" + RandomUtility.NextIdentifier(), RandomUtility.NextBoolean());
        }

        public static void CopyTable(this CremaDataSet dataSet)
        {
            var table = dataSet.Tables.RandomOrDefault();

            if (table == null)
                return;

            table.Copy("Table_" + RandomUtility.NextIdentifier(), RandomUtility.NextBoolean());
        }

        public static void GenerateColumns(this CremaDataTable dataTable, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                CreateColumn(dataTable);
            }
        }

        public static bool CreateColumn(this CremaDataTable dataTable)
        {
            var columnName = RandomUtility.NextIdentifier();

            if (dataTable.Columns.Contains(columnName) == true)
                return false;

            var column = dataTable.Columns.Add(columnName);

            if (dataTable.PrimaryKey.Any() == false)
            {
                column.IsKey = true;
            }
            else if (RandomUtility.Within(10))
            {
                column.IsKey = true;
                column.Unique = RandomUtility.Within(75);
            }

            if (RandomUtility.Within(75) == true)
            {
                column.DataType = CremaDataTypeUtility.GetType(CremaDataTypeUtility.GetBaseTypeNames().Random());
            }
            else if (dataTable.DataSet.Types.Any())
            {
                column.CremaType = dataTable.DataSet.Types.Random();
            }

            if (RandomUtility.Within(25) == true)
            {
                column.Comment = RandomUtility.NextString();
            }

            if (CremaDataTypeUtility.CanUseAutoIncrement(column.DataType) == true)
            {
                column.AutoIncrement = RandomUtility.NextBoolean();
            }

            return true;
        }

        public static bool ChangeColumn(this CremaDataTable dataTable)
        {
            var column = dataTable.Columns.RandomOrDefault();

            if (column == null)
                return false;

            if (RandomUtility.Within(25) == true)
            {
                column.ColumnName = RandomUtility.NextIdentifier();
            }

            return true;
        }

        public static bool DeleteColumn(this CremaDataTable dataTable)
        {
            var column = dataTable.Columns.RandomOrDefault();

            if (column == null || column.IsKey == true)
                return false;

            try
            {
                dataTable.Columns.Remove(column);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static void EditColumn(this CremaDataTable dataTable, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                if (RandomUtility.Within(40) == true)
                    CreateColumn(dataTable);
                else if (RandomUtility.Within(50) == true)
                    ChangeColumn(dataTable);
                else if (RandomUtility.Within(25) == true)
                    DeleteColumn(dataTable);
            }
        }

        public static void EditRandom(this CremaDataTable dataTable)
        {
            EditRandom(dataTable, 1);
        }

        public static void EditRandom(this CremaDataTable dataTable, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                if (RandomUtility.Within(40) == true)
                    CreateRow(dataTable);
                else if (RandomUtility.Within(50) == true)
                    ChangeRow(dataTable);
                else if (RandomUtility.Within(25) == true)
                    DeleteRow(dataTable);
            }
        }

        public static bool CreateRow(this CremaDataTable dataTable)
        {
            var parent = dataTable.Parent;
            CremaDataRow parentRow = null;

            if (parent != null)
            {
                if (parent.Rows.Any() == true)
                    parentRow = parent.Rows.Random();
                else
                    return false;
            }

            var dataRow = dataTable.NewRow(parentRow);

            foreach (var item in dataTable.Columns)
            {
                dataRow.SetField(item.ColumnName, GetRandomValue(item));
            }

            try
            {
                dataTable.Rows.Add(dataRow);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool ChangeRow(this CremaDataTable dataTable)
        {
            var dataRow = dataTable.Rows.RandomOrDefault();

            if (dataRow == null)
                return false;

            foreach (var item in dataTable.Columns)
            {
                if (RandomUtility.Within(25) == false)
                    continue;

                try
                {
                    dataRow.SetField(item.ColumnName, GetRandomValue(item));
                }
                catch
                {

                }
            }

            return true;
        }

        public static bool DeleteRow(this CremaDataTable dataTable)
        {
            var row = dataTable.Rows.RandomOrDefault();
            if (row == null)
                return false;

            try
            {
                row.Delete();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static void GenerateRows(this CremaDataTable dataTable, int tryCount)
        {
            int failedCount = 0;
            for (int i = 0; i < tryCount; i++)
            {
                if (GenerateRow(dataTable) == true)
                    continue;

                failedCount++;
                if (failedCount > 5)
                    break;
            }
        }

        public static bool GenerateRow(this CremaDataTable dataTable)
        {
            var dataRow = NewRandomRow(dataTable);

            if (FillFields(dataRow) == true)
            {
                try
                {
                    dataTable.Rows.Add(dataRow);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public static bool FillFields(this CremaDataRow dataRow)
        {
            var changed = 0;

            foreach (var item in dataRow.Table.Columns)
            {
                if (item.AutoIncrement == true)
                    continue;
                if (dataRow[item.ColumnName] != DBNull.Value)
                    continue;
                if (FillField(dataRow, item) == false)
                    return false;
                changed++;
            }
            if (RandomUtility.Within(5) == true)
                dataRow.IsEnabled = RandomUtility.NextBoolean();

            return changed > 0;
        }

        public static bool FillField(this CremaDataRow dataRow, CremaDataColumn dataColumn)
        {
            for (int i = 0; i < 20; i++)
            {
                object value = GetRandomValue(dataColumn);

                if (dataColumn.AllowDBNull == false && value == DBNull.Value)
                    continue;

                //if (Contains(content, columnInfo.Name, value) == false)
                {
                    dataRow.SetField(dataColumn, value);
                    return true;
                }
            }
            return false;
        }

        public static void GenerateMembers(this CremaDataType dataType, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                CreateMember(dataType);
            }
        }

        public static CremaDataTypeMember CreateMember(this CremaDataType dataType)
        {
            var memberName = RandomUtility.NextIdentifier();
            if (dataType.Members.Contains(memberName) == true)
                return null;

            var member = dataType.NewMember();

            member.Name = memberName;
            if (dataType.IsFlag == true)
                member.Value = RandomUtility.NextBit();
            else
                member.Value = RandomUtility.Next(dataType.Members.Count);

            if (RandomUtility.Within(10) == true)
                member.Comment = RandomUtility.NextString();

            try
            {
                dataType.Members.Add(member);
                return member;
            }
            catch
            {
                return null;
            }
        }

        public static bool ChangeMember(this CremaDataType dataType)
        {
            var member = dataType.Members.RandomOrDefault();

            if (member == null)
                return false;

            try
            {
                if (RandomUtility.Within(25) == true)
                {
                    member.Name = RandomUtility.NextIdentifier();
                }

                if (RandomUtility.Within(25) == true)
                {
                    if (dataType.IsFlag == true)
                        member.Value = RandomUtility.NextBit();
                    else
                        member.Value = RandomUtility.Next(dataType.Members.Count * 3);
                }

                if (RandomUtility.Within(25) == true)
                {
                    member.Comment = RandomUtility.NextString();
                }

                if (RandomUtility.Within(25) == true)
                {
                    member.Index = RandomUtility.Next(dataType.Members.Count);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool DeleteMember(this CremaDataType dataType)
        {
            var member = dataType.Members.RandomOrDefault();

            if (member == null)
                return false;

            try
            {
                member.Delete();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static void EditRandom(this CremaDataType dataType, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                if (RandomUtility.Within(25) == true)
                    CreateMember(dataType);
                else if (RandomUtility.Within(50) == true)
                    ChangeMember(dataType);
                else if (RandomUtility.Within(25) == true)
                    DeleteMember(dataType);
            }
        }

        public static object GetRandomValue(CremaDataColumn dataColumn)
        {
            if (dataColumn.AllowDBNull == true && RandomUtility.Next(4) == 0)
            {
                return DBNull.Value;
            }
            else if (dataColumn.CremaType != null)
            {
                return GetRandomValue(dataColumn.CremaType);
            }
            else
            {
                return RandomUtility.Next(dataColumn.DataType);
            }
        }

        public static object GetRandomValue(this CremaDataType dataType)
        {
            var typeInfo = dataType.TypeInfo;
            if (typeInfo.Members.Length == 0)
                throw new Exception(dataType.Name);

            if (typeInfo.IsFlag == true)
            {
                long value = 0;
                int count = RandomUtility.Next(1, typeInfo.Members.Length);
                for (int i = 0; i < count; i++)
                {
                    var index = RandomUtility.Next(typeInfo.Members.Length);
                    value |= typeInfo.Members[index].Value;
                }
                var textvalue = typeInfo.ConvertToString(value);
                if (textvalue == string.Empty)
                    throw new Exception();
                return textvalue;
            }
            else
            {
                return typeInfo.Members.Random().Name;
            }
        }

        public static CremaDataRow NewRandomRow(this CremaDataTable dataTable)
        {
            if (dataTable.Parent != null)
            {
                var parentRow = dataTable.Parent.Rows.Random();
                if (parentRow == null)
                    return null;
                return dataTable.NewRow(parentRow);
            }
            return dataTable.NewRow();
        }

        public static void RandomTask(CremaTemplate template, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                if (RandomUtility.Within(3) == true)
                {
                    var column = template.Columns.Random();
                    if (column.CanDelete == true)
                        column.Delete();
                }
                else if (RandomUtility.Within(5) == true)
                {
                    var column = template.NewColumn();
                    column.Name = RandomUtility.NextIdentifier();
                    column.IsKey = RandomUtility.Within(5);
                    //column.IsUnique = RandomUtility.Within(10);
                    column.AutoIncrement = RandomUtility.Within(10);
                    column.DataTypeName = template.Types.Random();

                    try
                    {
                        template.Columns.Add(column);
                    }
                    catch { }
                }
                else
                {
                    var column = template.Columns.Random();
                    column.Name = RandomUtility.NextIdentifier();
                    column.Index = RandomUtility.Next(template.Columns.Count);

                    try
                    {
                        if (RandomUtility.Within(5) == true)
                            column.DataTypeName = template.Types.Random();
                    }
                    catch { }

                    try
                    {
                        if (RandomUtility.Within(5) == true && CremaDataTypeUtility.CanUseAutoIncrement(column.DataTypeName) == true)
                            column.AutoIncrement = RandomUtility.NextBoolean();
                    }
                    catch { }

                    try
                    {
                        if (RandomUtility.Within(5) == true)
                            column.Unique = RandomUtility.NextBoolean();
                    }
                    catch { }

                    if (RandomUtility.Within(5) == true)
                        column.Comment = RandomUtility.NextString();
                }
            }
        }

        private static bool Contains(CremaDataTable dataTable, string columnName, object value)
        {
            foreach (var item in dataTable.Rows)
            {
                if (object.Equals(item[columnName], value) == true)
                    return true;
            }
            return false;
        }

        private static int GetLevel<T>(T category, Func<T, T> parentFunc)
        {
            int level = 0;

            var parent = parentFunc(category);
            while (parent != null)
            {
                level++;
                parent = parentFunc(parent);
            }
            return level;
        }
    }
}
