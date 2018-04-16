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

using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Library;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Random
{
    public static class CremaRandomUtility
    {
        private static TagInfo[] tags = new TagInfo[] { TagInfo.All, TagInfoUtility.Server, TagInfoUtility.Client, TagInfo.Unused };

        public static void Generate(this IDataBase dataBase, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                if (RandomUtility.Within(50) == true)
                    dataBase.TypeContext.Generate(authentication);
                else
                    dataBase.TableContext.Generate(authentication);
            }
        }

        public static void Generate(this IUserContext context, Authentication authentication)
        {
            if (RandomUtility.Within(25) == true)
                context.GenerateCategory(authentication);
            else
                context.GenerateUser(authentication);
        }

        public static void Generate(this IUserContext context, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                context.Generate(authentication);
            }
        }

        public static void Generate(this ITypeContext context, Authentication authentication)
        {
            if (RandomUtility.Within(25) == true)
                context.AddRandomCategory(authentication);
            else
                context.AddRandomType(authentication);
        }

        public static void Generate(this ITypeContext context, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                context.Generate(authentication);
            }
        }

        public static void Generate(this ITableContext context, Authentication authentication)
        {
            if (RandomUtility.Within(25) == true)
                context.GenerateCategory(authentication);
            else
                context.GenerateTable(authentication);
        }

        public static void Generate(this ITableContext context, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                context.Generate(authentication);
            }
        }

        public static void GenerateCategories(this IUserContext context, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                context.GenerateCategory(authentication);
            }
        }

        public static bool GenerateCategory(this IUserContext context, Authentication authentication)
        {
            if (RandomUtility.Within(50) == true)
            {
                context.Root.AddNewCategory(authentication, RandomUtility.NextIdentifier());
            }
            else
            {
                var category = context.Categories.Random();
                if (GetLevel(category, (i) => i.Parent) > 4)
                    return false;
                category.AddNewCategory(authentication, RandomUtility.NextIdentifier());
            }
            return true;
        }

        public static void GenerateUsers(this IUserContext context, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                context.GenerateUser(authentication);
            }
        }

        public static bool GenerateUser(this IUserContext context, Authentication authentication)
        {
            var category = context.Categories.Random();
            var newID = NameUtility.GenerateNewName("Test", context.Users.Select(item => item.ID).ToArray());
            var newName = newID.Replace("Test", "테스트");
            category.AddNewUser(authentication, newID, null, newName, Authority.Member);

            return true;
        }

        public static void GenerateCategories(this ITableContext context, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                context.GenerateCategory(authentication);
            }
        }

        public static bool GenerateCategory(this ITableContext context, Authentication authentication)
        {
            var categoryName = RandomUtility.NextIdentifier();
            var category = RandomUtility.Within(50) == true ? context.Root : context.Categories.Random();

            if (category.VerifyAccessType(authentication, AccessType.Master) == false)
                return false;

            if (GetLevel(category, (i) => i.Parent) > 4)
                return false;

            if (category.Categories.ContainsKey(categoryName) == true)
                return false;

            category.AddNewCategory(authentication, categoryName);

            return true;
        }

        public static void GenerateTables(this ITableContext context, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                context.GenerateTable(authentication);
            }
        }

        public static void GenerateTable(this ITableContext context, Authentication authentication)
        {
            if (RandomUtility.Within(25) == true)
                InheritTable(context, authentication);
            else if (RandomUtility.Within(25) == true)
                CopyTable(context, authentication);
            else
                CreateTable(context, authentication);
        }

        public static void CreateTable(this ITableContext context, Authentication authentication)
        {
            var category = context.Categories.Random();

            var template = category.NewTable(authentication);
            GenerateColumns(template, authentication, RandomUtility.Next(3, 10));
            if (RandomUtility.Within(25) == true)
                template.SetComment(authentication, RandomUtility.NextString());
            template.EndEdit(authentication);

            var table = template.Table;

            while (RandomUtility.Within(10))
            {
                var childTemplate = table.NewTable(authentication);
                GenerateColumns(childTemplate, authentication, RandomUtility.Next(3, 10));
                childTemplate.EndEdit(authentication);
            }

            var content = table.Content;
            content.EnterEdit(authentication);

            GenerateRows(content, authentication, RandomUtility.Next(10, 100));

            foreach (var item in table.Childs)
            {
                GenerateRows(item.Content, authentication, RandomUtility.Next(10, 100));
            }

            content.LeaveEdit(authentication);
        }

        public static void InheritTable(this ITableContext context, Authentication authentication)
        {
            var category = context.Categories.Random();
            var table = context.Tables.RandomOrDefault();

            if (table == null)
                return;

            table.Inherit(authentication, "Table_" + RandomUtility.NextIdentifier(), category.Path, RandomUtility.NextBoolean());
        }

        public static void CopyTable(this ITableContext context, Authentication authentication)
        {
            var category = context.Categories.Random();
            var table = context.Tables.RandomOrDefault();

            if (table == null)
                return;

            table.Copy(authentication, "Table_" + RandomUtility.NextIdentifier(), category.Path, RandomUtility.NextBoolean());
        }

        public static void GenerateColumns(this ITableTemplate template, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                CreateColumn(template, authentication);
            }
        }

        public static bool CreateColumn(this ITableTemplate template, Authentication authentication)
        {
            var columnName = RandomUtility.NextIdentifier();

            if (template.Contains(columnName) == true)
                return false;

            var column = template.AddNew(authentication);
            column.SetName(authentication, columnName);

            if (template.PrimaryKey.Any() == false)
            {
                column.SetIsKey(authentication, true);
            }
            else if (template.Count == 0 && RandomUtility.Within(10))
            {
                column.SetIsKey(authentication, true);
                column.SetIsUnique(authentication, RandomUtility.Within(75));
            }

            if (RandomUtility.Within(75) == true)
            {
                column.SetTags(authentication, TagInfo.All);
            }
            else
            {
                column.SetTags(authentication, tags.Random());
            }

            if (RandomUtility.Within(75) == true)
            {
                column.SetDataType(authentication, CremaDataTypeUtility.GetBaseTypeNames().Random());
            }
            else
            {
                column.SetDataType(authentication, template.SelectableTypes.Random());
            }

            if (RandomUtility.Within(25) == true)
            {
                column.SetComment(authentication, RandomUtility.NextString());
            }

            if (CremaDataTypeUtility.CanUseAutoIncrement(column.DataType) == true)
            {
                column.SetAutoIncrement(authentication, RandomUtility.NextBoolean());
            }

            try
            {
                template.EndNew(authentication, column);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool ChangeColumn(this ITableTemplate template, Authentication authentication)
        {
            var column = template.RandomOrDefault();

            if (column == null)
                return false;

            if (RandomUtility.Within(25) == true)
            {
                column.SetName(authentication, RandomUtility.NextIdentifier());
            }

            if (RandomUtility.Within(75) == true)
            {
                column.SetTags(authentication, TagInfo.All);
            }
            else
            {
                column.SetTags(authentication, tags.Random());
            }

            if (RandomUtility.Within(25) == true)
            {
                column.SetComment(authentication, RandomUtility.NextString());
            }

            return true;
        }

        public static bool DeleteColumn(this ITableTemplate template, Authentication authentication)
        {
            var column = template.RandomOrDefault();

            if (column == null || column.IsKey == true)
                return false;

            try
            {
                column.Delete(authentication);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static void EditRandom(this ITableTemplate template, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                if (RandomUtility.Within(40) == true)
                    CreateColumn(template, authentication);
                else if (RandomUtility.Within(50) == true)
                    ChangeColumn(template, authentication);
                else if (RandomUtility.Within(25) == true)
                    DeleteColumn(template, authentication);
            }
        }

        public static void EditRandom(this ITableContent content, Authentication authentication)
        {
            EditRandom(content, authentication, 1);
        }

        public static void EditRandom(this ITableContent content, Authentication authentication, int tryCount)
        {
            var failedCount = 0;
            for (var i = 0; i < tryCount; i++)
            {
                try
                {
                    if (RandomUtility.Within(10) == true || content.Count < 5)
                        CreateRow(content, authentication);
                    else if (RandomUtility.Within(75) == true)
                        ChangeRow(content, authentication);
                    else if (RandomUtility.Within(10) == true)
                        DeleteRow(content, authentication);
                }
                catch
                {
                    failedCount++;
                }
                if (failedCount > 5)
                    break;
            }
        }

        public static void CreateRow(this ITableContent content, Authentication authentication)
        {
            var table = content.Table;
            var parent = content.Parent;
            string relationID = null;

            if (parent != null && parent.Any() == true)
            {
                relationID = parent.Random().RelationID;
            }

            var row = content.AddNew(authentication, relationID);

            var types = table.GetService(typeof(ITypeCollection)) as ITypeCollection;
            foreach (var item in table.TableInfo.Columns)
            {
                if (item.AutoIncrement == false)
                {
                    //row.SetField(authentication, item.Name, TypeContextExtensions.GetRandomValue(types, item));
                }
            }

            if (RandomUtility.Within(25) == true)
            {
                row.SetTags(authentication, tags.Random());
            }

            if (RandomUtility.Within(25) == true)
            {
                row.SetIsEnabled(authentication, RandomUtility.NextBoolean());
            }

            content.EndNew(authentication, row);
        }

        public static void ChangeRow(this ITableContent content, Authentication authentication)
        {
            var table = content.Table;
            var row = content.RandomOrDefault();

            if (row == null)
                return;

            var types = table.GetService(typeof(ITypeCollection)) as ITypeCollection;

            if (RandomUtility.Within(5) == true)
            {
                row.SetTags(authentication, tags.Random());
            }
            else if (RandomUtility.Within(5) == true)
            {
                row.SetIsEnabled(authentication, RandomUtility.NextBoolean());
            }
            else
            {
                var columnInfo = table.TableInfo.Columns.Random();
                //row.SetField(authentication, columnInfo.Name, TypeContextExtensions.GetRandomValue(types, columnInfo));
            }
        }

        public static void DeleteRow(this ITableContent content, Authentication authentication)
        {
            var row = content.RandomOrDefault();
            if (row == null)
                return;

            row.Delete(authentication);
        }

        public static void GenerateRows(this ITableContent content, Authentication authentication, int tryCount)
        {
            int failedCount = 0;
            for (var i = 0; i < tryCount; i++)
            {
                if (GenerateRow(content, authentication) == true)
                    continue;

                failedCount++;
                if (failedCount > 5)
                    break;
            }
        }

        public static bool GenerateRow(this ITableContent content, Authentication authentication)
        {
            var row = NewRandomRow(content, authentication);

            if (FillFields(row, authentication) == true)
            {
                if (RandomUtility.Within(25) == true)
                {
                    row.SetTags(authentication, tags.Random());
                }

                if (RandomUtility.Within(25) == true)
                {
                    row.SetIsEnabled(authentication, RandomUtility.NextBoolean());
                }

                try
                {
                    content.EndNew(authentication, row);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public static bool FillFields(this ITableRow row, Authentication authentication)
        {
            var table = row.Content.Table;
            var changed = 0;

            foreach (var item in table.TableInfo.Columns)
            {
                if (row[item.Name] != null)
                    continue;

                if (FillField(row, authentication, item) == false)
                    return false;
                changed++;
            }

            return changed > 0;
        }

        public static bool FillField(this ITableRow row, Authentication authentication, ColumnInfo columnInfo)
        {
            var content = row.Content;
            var types = content.Table.GetService(typeof(ITypeCollection)) as ITypeCollection;

            for (var i = 0; i < 20; i++)
            {
                //object value = TypeContextExtensions.GetRandomValue(types, columnInfo);

                //if (columnInfo.AllowNull == false && value == null)
                //    continue;

                ////if (Contains(content, columnInfo.Name, value) == false)
                //{
                //    row.SetField(authentication, columnInfo.Name, value);
                //    return true;
                //}
            }
            return false;
        }

        public static ITableRow NewRandomRow(this ITableContent content, Authentication authentication)
        {
            if (content.Parent != null)
            {
                var parentRow = content.Parent.Random();
                if (parentRow == null)
                    return null;
                return content.AddNew(authentication, parentRow.RelationID);
            }
            return content.AddNew(authentication, null);
        }

        public static ITable RandomSample(this ITableCollection tables)
        {
            return tables.Random(item =>
            {
                if (item.IsPrivate == true)
                    return false;
                if (item.IsLocked == true)
                    return false;
                if (item.Childs.Any() == false)
                    return false;
                if (item.TemplatedParent != null)
                    return false;
                if (item.Category.Parent == null)
                    return false;
                return true;
            });
        }

        public static ITableCategory RandomSample(this ITableCategoryCollection categories)
        {
            return categories.Random(item =>
            {
                if (item.Parent == null)
                    return false;
                if (item.IsPrivate == true)
                    return false;
                if (item.IsLocked == true)
                    return false;
                if (item.Tables.Any(i => i.TemplatedParent == null && i.Childs.Any()) == false)
                    return false;
                return true;
            });
        }

        public static IType RandomSample(this ITypeCollection types)
        {
            return types.Random(item =>
            {
                if (item.IsPrivate == true)
                    return false;
                if (item.IsLocked == true)
                    return false;
                if (item.Category.Parent == null)
                    return false;
                return true;
            });
        }

        public static ITypeCategory RandomSample(this ITypeCategoryCollection categories)
        {
            return categories.Random(item =>
            {
                if (item.Parent == null)
                    return false;
                if (item.IsPrivate == true)
                    return false;
                if (item.IsLocked == true)
                    return false;
                if (item.Types.Any() == false)
                    return false;
                return true;
            });
        }

        public static IDataBase RandomDataBase(this ICremaHost cremaHost)
        {
            var dataBase = cremaHost.DataBases.Random();
            //if (dataBase.IsLoaded == false)
            //    dataBase.Load();
            return dataBase;
        }

        public static TagInfo RandomTags()
        {
            return tags.Random();
        }

        private static bool Contains(ITableContent content, string columnName, object value)
        {
            foreach (var item in content)
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
