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
using Ntreev.Crema.Data;
using Ntreev.Crema.Services;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Random;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Ntreev.Library;

namespace Ntreev.Crema.Services.Random
{
    public static class TableContextExtensions
    {
        static TableContextExtensions()
        {
            MinTableCount = 10;
            MaxTableCount = 200;
            MinTableCategoryCount = 1;
            MaxTableCategoryCount = 20;
            MinRowCount = 100;
            MaxRowCount = 10000;
        }

        public static void AddRandomItems(this ITableContext tableContext, Authentication authentication)
        {
            AddRandomCategories(tableContext, authentication);
            AddRandomTables(tableContext, authentication);
            AddRandomChildTables(tableContext, authentication, 10);
            AddRandomDerivedTables(tableContext, authentication, 10);
        }

        public static void AddRandomCategories(this ITableContext tableContext, Authentication authentication)
        {
            AddRandomCategories(tableContext, authentication, RandomUtility.Next(MinTableCategoryCount, MaxTableCategoryCount));
        }

        public static void AddRandomCategories(this ITableContext tableContext, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                tableContext.AddRandomCategory(authentication);
            }
        }

        public static ITableCategory AddRandomCategory(this ITableCategory category, Authentication authentication)
        {
            var categoryName = RandomUtility.NextIdentifier();
            return category.AddNewCategory(authentication, categoryName);
        }

        public static ITableCategory AddRandomCategory(this ITableContext tableContext, Authentication authentication)
        {
            if (RandomUtility.Within(33) == true)
            {
                return tableContext.Root.AddRandomCategory(authentication);
            }
            else
            {
                var category = tableContext.Categories.Random();
                if (GetLevel(category, (i) => i.Parent) > 4)
                    return null;
                return category.AddRandomCategory(authentication);
            }
        }

        public static void AddRandomTables(this ITableContext tableContext, Authentication authentication)
        {
            AddRandomTables(tableContext, authentication, RandomUtility.Next(MinTableCount, MaxTableCount));
        }

        public static void AddRandomTables(this ITableContext tableContext, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                AddRandomTable(tableContext, authentication);
            }
        }

        public static ITable AddRandomTable(this ITableContext tableContext, Authentication authentication)
        {
            var category = tableContext.Categories.Random();
            return AddRandomTable(category, authentication);
        }

        public static ITable AddRandomTable(this ITableCategory category, Authentication authentication)
        {
            var template = category.NewTable(authentication);
            template.InitializeRandom(authentication);
            template.EndEdit(authentication);

            if (template.Target is ITable[] tables)
            {
                foreach (var item in tables)
                {
                    AddRandomRows(item, authentication, RandomUtility.Next(MinRowCount, MaxRowCount));
                }
                return tables.First();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static void AddRandomDerivedTables(this ITableContext tableContext, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                AddRandomDerivedTable(tableContext, authentication);
            }
        }

        public static ITable AddRandomDerivedTable(this ITableContext tableContext, Authentication authentication)
        {
            var category = tableContext.Categories.Random();
            return AddRandomDerivedTable(category, authentication);
        }

        public static ITable AddRandomDerivedTable(this ITableCategory category, Authentication authentication)
        {
            var tableName = RandomUtility.NextIdentifier();
            var copyData = RandomUtility.NextBoolean();
            var tableContext = category.GetService(typeof(ITableContext)) as ITableContext;
            var table = tableContext.Tables.Random(item => item.TemplatedParent == null && item.Parent == null);
            return table.Inherit(authentication, tableName, category.Path, copyData);
        }

        public static void AddRandomChildTables(this ITableContext tableContext, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                AddRandomChildTable(tableContext, authentication);
            }
        }

        public static ITable AddRandomChildTable(this ITableContext tableContext, Authentication authentication)
        {
            var table = tableContext.Tables.Random(item => item.TemplatedParent == null && item.Parent == null);
            return AddRandomChildTable(table, authentication);
        }

        public static ITable AddRandomChildTable(this ITable table, Authentication authentication)
        {
            var copyData = RandomUtility.NextBoolean();
            var template = table.NewTable(authentication);
            template.InitializeRandom(authentication);
            template.EndEdit(authentication);
            if (template.Target is ITable[] tables)
            {
                foreach (var item in tables)
                {
                    AddRandomRows(item, authentication, RandomUtility.Next(MinRowCount, MaxRowCount));
                }
                return tables.First();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static void AddRandomRows(this ITable table, Authentication authentication, int tryCount)
        {
            var target = table.Parent ?? table;
            var targetContent = target.Content;
            targetContent.BeginEdit(authentication);
            targetContent.EnterEdit(authentication);
            var failedCount = 0;
            for (var i = 0; i < tryCount; i++)
            {
                try
                {
                    var parentRow = target != table ? targetContent.RandomOrDefault() : null;
                    AddRandomRow(table, parentRow, authentication);
                }
                catch
                {
                    failedCount++;
                }
                if (failedCount > 3)
                    break;
            }
            targetContent.LeaveEdit(authentication);
            targetContent.EndEdit(authentication);
        }

        public static ITableRow AddRandomRow(this ITable table, ITableRow parentRow, Authentication authentication)
        {
            if (table.Parent != null && parentRow == null)
                return null;

            var tableRow = table.Content.AddNew(authentication, parentRow?.RelationID);
            tableRow.InitializeRandom(authentication);
            table.Content.EndNew(authentication, tableRow);
            return tableRow;
        }

        private static int GetLevel<T>(T category, Func<T, T> parentFunc)
        {
            var level = 0;

            var parent = parentFunc(category);
            while (parent != null)
            {
                level++;
                parent = parentFunc(parent);
            }
            return level;
        }

        public static int MinTableCount { get; set; }

        public static int MaxTableCount { get; set; }

        public static int MinTableCategoryCount { get; set; }

        public static int MaxTableCategoryCount { get; set; }

        public static int MinRowCount { get; set; }

        public static int MaxRowCount { get; set; }
    }
}
