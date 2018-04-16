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
using Ntreev.Library.IO;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Random
{
    public static class CremaDataSetExtensions
    {
        static CremaDataSetExtensions()
        {
            TypePrefix = "e";
            TypePostfix = string.Empty;
            TablePrefix = string.Empty;
            TablePostfix = "Table";

            MinTableCount = 5;
            MaxTableCount = 15;
            MinTypeCount = 5;
            MaxTypeCount = 15;
        }

        public static void AddRandoms(this CremaDataSet dataSet, int tryCount)
        {
            var t1 = Math.Max(1, (int)(tryCount * 0.25));
            var t2 = Math.Max(1, (int)(tryCount * 0.50));
            var t3 = Math.Max(1, (int)(tryCount * 0.25));

            dataSet.AddRandomTypes(t1);
            dataSet.AddRandomTables(t2);
            for (var i = 0; i < t3; i++)
            {
                dataSet.Tables.Random(item => item.Parent == null).AddRandomChild();
            }
        }

        public static CremaDataTable AddRandomTable(this CremaDataSet dataSet)
        {
            var dataTable = CremaDataTableExtensions.CreateRandomTable(TablePrefix, TablePostfix);
            while (dataSet.Tables.Contains(dataTable.TableName) == true)
            {
                dataTable.TableName = IdentifierUtility.Next(TablePrefix, TablePostfix);
            }

            dataSet.Tables.Add(dataTable);
            return dataTable;
        }

        public static CremaDataTable AddDerivedTable(this CremaDataSet dataSet)
        {
            var dataTable = dataSet.Tables.Random(item => item.TemplateNamespace == string.Empty && item.Parent == null);
            return dataTable.CreateDerivedTable(TablePrefix, TablePostfix);
        }

        public static void AddRandomTables(this CremaDataSet dataSet, int count)
        {
            while (dataSet.Tables.Count < count)
            {
                try
                {
                    AddRandomTable(dataSet);
                }
                catch
                {

                }
            }
        }

        public static void AddDerivedTables(this CremaDataSet dataSet, int count)
        {
            while (dataSet.Tables.Where(item => item.TemplateNamespace != String.Empty).Count() < count)
            {
                try
                {
                    AddDerivedTable(dataSet);
                }
                catch
                {

                }
            }
        }

        public static void AddRandomChildTables(this CremaDataSet dataSet, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                var dataTable = dataSet.Tables.RandomOrDefault(item => item.Parent == null);
                if (dataTable != null)
                {
                    dataTable.AddRandomChild();
                }
            }
        }

        public static CremaDataTable AddRandomTable(this CremaDataSet dataSet, string tableName)
        {
            var table = dataSet.Tables.Add(tableName);
            table.AddRandomColumns(RandomUtility.Next(CremaDataTableExtensions.MinColumnCount, CremaDataTableExtensions.MaxColumnCount));
            return table;
        }

        public static CremaDataType AddRandomType(this CremaDataSet dataSet)
        {
            var dataType = CremaDataTypeExtensions.CreateRandomType(TypePrefix, TypePostfix);
            dataType.AcceptChanges();
            dataSet.Types.Add(dataType);
            return dataType;
        }

        public static CremaDataSet CreateRandomSet()
        {
            var dataSet = new CremaDataSet();
            dataSet.AddRandomTypes(RandomUtility.Next(MinTypeCount, MaxTypeCount));
            dataSet.AddRandomTables(RandomUtility.Next(MinTableCount, MaxTableCount));
            dataSet.AddRandomChildTables(RandomUtility.Next(MinTableCount, MaxTableCount));
            dataSet.AddDerivedTables(RandomUtility.Next(MinTableCount, MaxTableCount));

            return dataSet;
        }

        public static CremaDataSet CreateFromAssembly(Assembly assembly)
        {
            var dataSet = new CremaDataSet();
            var types = assembly.GetTypes().Where(item => item.IsEnum && item.IsPublic == true);
            foreach (var item in types)
            {
                dataSet.AddTypeFromEnumType(item);
            }

            CreateClassTable(dataSet, assembly);

            return dataSet;
        }

        private static void CreateClassTable(CremaDataSet dataSet, Assembly assembly)
        {
            var dataTable = dataSet.Tables.Add("ClassTable");
            var types = assembly.GetTypes().Where(item => item.IsClass && item.IsPublic == true);
            var indexColumn = dataTable.Columns.Add("Index");
            indexColumn.DataType = typeof(int);
            indexColumn.AutoIncrement = true;
            indexColumn.IsKey = true;
            var nameColumn = dataTable.Columns.Add("Name");
            var attributeColumn = dataTable.Columns.Add("Attributes", dataSet.Types[nameof(TypeAttributes)]);

            var propertyTable = dataTable.Childs.Add("Properties");
            var c1 = propertyTable.Columns.Add("Index");
            c1.DataType = typeof(int);
            c1.AutoIncrement = true;
            c1.IsKey = true;
            var c2 = propertyTable.Columns.Add("Name");

            foreach (var item in types)
            {
                var dataRow = dataTable.NewRow();
                dataRow[nameColumn] = item.Name;
                dataRow[attributeColumn] = item.Attributes.ToString().Replace(',', ' ');
                dataTable.Rows.Add(dataRow);

                CreatePropertyTable(propertyTable, dataRow, item);
            }
        }

        private static void CreatePropertyTable(CremaDataTable propertyTable, CremaDataRow classRow, Type type)
        {
            foreach (var item in type.GetProperties())
            {
                var dataRow = propertyTable.NewRow(classRow);
                dataRow["Name"] = item.Name;
                propertyTable.Rows.Add(dataRow);
            }
        }

        public static CremaDataType AddRandomEmptyType(this CremaDataSet dataSet)
        {
            var dataType = CremaDataTypeExtensions.CreateRandomEmptyType(TypePrefix, TypePostfix);
            dataSet.Types.Add(dataType);
            return dataType;
        }

        public static void AddRandomTypes(this CremaDataSet dataSet, int count)
        {
            while (dataSet.Types.Count < count)
            {
                try
                {
                    AddRandomType(dataSet);
                }
                catch
                {

                }
            }
        }

        public static void AddTypeFromEnumType(this CremaDataSet dataSet, Type enumType)
        {
            var summaryProvider = SummaryProvider.FromType(enumType);
            var categoryPath = enumType.Namespace.Replace(".", PathUtility.Separator).Wrap(PathUtility.SeparatorChar);
            var type = dataSet.Types.Add(enumType.Name, categoryPath);

            type.IsFlag = enumType.GetCustomAttributes(typeof(FlagsAttribute), true).Any();
            type.Comment = summaryProvider.GetEnumSummary(enumType.Namespace, enumType.Name);

            foreach (var item in Enum.GetNames(enumType))
            {
                var member = type.NewMember();

                member.Name = item;
                member.Value = Convert.ToInt64(Enum.Parse(enumType, item));
                member.Comment = summaryProvider.GetMemberSummary(enumType.Namespace, enumType.Name, item);

                type.Members.Add(member);
            }
        }

        public static string TypePrefix { get; set; }

        public static string TypePostfix { get; set; }

        public static string TablePrefix { get; set; }

        public static string TablePostfix { get; set; }

        public static int MinTableCount { get; set; }

        public static int MaxTableCount { get; set; }

        public static int MinTypeCount { get; set; }

        public static int MaxTypeCount { get; set; }

        private static void AddTableFromType(CremaDataSet dataSet, Type classType)
        {

        }
    }
}
