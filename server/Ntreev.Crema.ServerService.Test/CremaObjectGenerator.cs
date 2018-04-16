using Ntreev.Crema.ServiceModel.Data;
using Ntreev.Library;
using Ntreev.Library.Extensions;
using Ntreev.Library.Linq;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServerService.Test
{
    public static class CremaObjectGenerator
    {
        public static void GenerateStandard(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.TypeContext.GenerateStandard(authentication);
            dataBase.TableContext.GenerateStandard(authentication);
        }

        public static void GenerateStandard(this ITypeContext context, Authentication authentication)
        {
            var root = context.Root;
            {
                root.GenerateStandardType(authentication);
                root.GenerateStandardFlags(authentication);
            }

            var category = root.AddNewCategory(authentication);
            {
                category.GenerateStandardType(authentication);
                category.GenerateStandardFlags(authentication);
            }

            var subCategory = category.AddNewCategory(authentication);
            {
                subCategory.GenerateStandardType(authentication);
                subCategory.GenerateStandardFlags(authentication);
            }
        }

        public static void GenerateStandard(this ITableContext context, Authentication authentication)
        {
            var typeContext = context.GetService(typeof(ITypeContext)) as ITypeContext;
            var root = context.Root;
            var types1 = typeContext.Types.Select(item => item.Name).ToArray();
            var types2 = CremaTypeUtility.GetBaseTypes();
            var allTypes = types1.Concat(types2);
            var allKeyTypes = types1.Concat(types2).Where(item => item != typeof(bool).GetTypeName());

            {
                var category = root.AddNewCategory(authentication, "SingleKey");
                foreach (var item in allTypes)
                {
                    var table = category.GenerateStandardTable(authentication, "SingleKey", EnumerableUtility.AsEnumerable(item), allTypes.Where(i => i != item));
                    if (table == null)
                        continue;
                    table.GenerateStandardChild(authentication);
                    table.GenerateStandardContent(authentication);
                }

                var category1 = root.AddNewCategory(authentication, "SingleKeyRefs");
                {
                    foreach (var item in category.Tables)
                    {
                        var table1 = item.Inherit(authentication, "Ref_" + item.Name, category1.Path, false);
                        table1.GenerateStandardContent(authentication);
                    }
                }
            }

            {
                var category = root.AddNewCategory(authentication, "DoubleKey");
                var query = allKeyTypes.Permutations(2);
                for (int i = 0; i < allTypes.Count(); i++)
                {
                    var keys = query.Random();
                    var columns = allTypes.Except(keys);
                    var table = category.GenerateStandardTable(authentication, "DoubleKey", keys, columns);
                    if (table == null)
                        continue;
                    table.GenerateStandardChild(authentication);
                    table.GenerateStandardContent(authentication);
                }

                var category1 = root.AddNewCategory(authentication, "DoubleKeyRefs");
                {
                    foreach (var item in category.Tables)
                    {
                        var table1 = item.Inherit(authentication, "Ref_" + item.Name, category1.Path, false);
                        table1.GenerateStandardContent(authentication);
                    }
                }
            }

            {
                var category = root.AddNewCategory(authentication, "TripleKey");
                var query = allKeyTypes.Permutations(3);
                for (int i = 0; i < allTypes.Count(); i++)
                {
                    var keys = query.Random();
                    var columns = allTypes.Except(keys);
                    var table = category.GenerateStandardTable(authentication, "TripleKey", keys, columns);
                    if (table == null)
                        continue;
                    table.GenerateStandardChild(authentication);
                    table.GenerateStandardContent(authentication);
                }

                var category1 = root.AddNewCategory(authentication, "TripleKeyRefs");
                {
                    foreach (var item in category.Tables)
                    {
                        var table1 = item.Inherit(authentication, "Ref_" + item.Name, category1.Path, false);
                        table1.GenerateStandardContent(authentication);
                    }
                }
            }

            {
                var category = root.AddNewCategory(authentication, "QuadraKey");
                var query = allKeyTypes.Permutations(4);
                for (int i = 0; i < allTypes.Count(); i++)
                {
                    var keys = query.Random();
                    var columns = allTypes.Except(keys);
                    var table = category.GenerateStandardTable(authentication, "QuadraKey", keys, columns);
                    if (table == null)
                        continue;
                    table.GenerateStandardChild(authentication);
                    table.GenerateStandardContent(authentication);
                }

                var category1 = root.AddNewCategory(authentication, "QuadraKeyRefs");
                {
                    foreach (var item in category.Tables)
                    {
                        var table1 = item.Inherit(authentication, "Ref_" + item.Name, category1.Path, false);
                        table1.GenerateStandardContent(authentication);
                    }
                }
            }
        }

        public static ITable GenerateStandardTable(this ITableCategory category, Authentication authentication, string prefix, IEnumerable<string> keyTypes, IEnumerable<string> columnTypes)
        {
            var tables = category.GetService<ITableCollection>();
            var tableName = string.Join("_", EnumerableUtility.Friends(prefix, keyTypes));

            if (tables.Contains(tableName) == true)
                return null;

            var template = category.NewTable(authentication);
            template.SetTableName(authentication, tableName);

            foreach (var item in keyTypes)
            {
                template.AddKey(authentication, item, item);
            }

            foreach (var item in columnTypes)
            {
                template.AddColumn(authentication, item, item);
            }

            try
            {
                template.EndEdit(authentication);
                return template.Table;
            }
            catch
            {
                template.CancelEdit(authentication);
                return null;
            }
            
            //var table = template.Table;
            //var content = table.Content;

            //content.BeginEdit(authentication);
            //content.EnterEdit(authentication);
            //try
            //{
            //    content.GenerateRows(authentication, RandomUtility.Next(10, 1000));
            //    content.LeaveEdit(authentication);
            //    content.EndEdit(authentication);
            //}
            //catch
            //{
            //    content.CancelEdit(authentication);
            //}

            //return table;
        }

        public static ITable GenerateStandardChild(this ITable table, Authentication authentication, string prefix, IEnumerable<string> keyTypes, IEnumerable<string> columnTypes)
        {
            var typeCollection = table.GetService<ITypeCollection>();
            var tableName = string.Join("_", EnumerableUtility.Friends(prefix, keyTypes));

            if (table.Childs.ContainsKey(tableName) == true)
                return null;

            var template = table.NewTable(authentication);
            template.SetTableName(authentication, tableName);

            foreach (var item in keyTypes)
            {
                template.AddKey(authentication, item, item);
            }

            foreach (var item in columnTypes)
            {
                template.AddColumn(authentication, item, item);
            }

            try
            {
                template.EndEdit(authentication);
                return template.Table;
            }
            catch
            {
                template.CancelEdit(authentication);
                return null;
            }
        }

        public static void GenerateStandardChild(this ITable table, Authentication authentication)
        {
            var typeCollection = table.GetService<ITypeCollection>();
            var types1 = typeCollection.Select(item => item.Name).ToArray();
            var types2 = CremaTypeUtility.GetBaseTypes();
            var allTypes = types1.Concat(types2);
            var allKeyTypes = types1.Concat(types2).Where(item => item != typeof(bool).GetTypeName());

            var prefixes = new string[] { "SingleKey", "DoubleKey", "TripleKey", "QuadraKey", };

            for (int i = 0; i < prefixes.Length; i++)
            {
                var query = allKeyTypes.Permutations(i + 1);
                var keys = query.Random();
                var columns = allTypes.Except(keys);
                table.GenerateStandardChild(authentication, "Child_" + prefixes[i], keys, columns);
            }
        }

        //public static void GenerateStandardTable(this ITableCategory category, Authentication authentication)
        //{
        //    var types1 = category.DataBase.TypeContext.Types.Select(item => item.Name).ToArray();
        //    var types2 = CremaTypeUtility.GetBaseTypes();
        //    var allTypes = types1.Concat(types2);

        //    foreach (var item in allTypes)
        //    {
        //        var tableName = string.Join("_", "SingleKey", item);
        //        var template = category.NewTable(authentication);
        //        template.SetTableName(authentication, tableName);
        //        template.SetComment(authentication, string.Format("Single Key Table : {0}", item));

        //        template.AddKey(authentication, item, item);

        //        var extraTypes = allTypes.Where(i => i != item);

        //        foreach (var i in extraTypes)
        //        {
        //            template.AddColumn(authentication, i, i);
        //        }

        //        template.EndEdit(authentication);
        //    }
        //}

        public static void GenerateStandardType(this ITypeCategory category, Authentication authentication)
        {
            var template = category.NewType(authentication);
            template.SetIsFlag(authentication, false);
            template.SetComment(authentication, "Standard Type");

            var az = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (char)i).ToArray();

            template.AddMember(authentication, "None", 0, "None Value");
            for (int i = 0; i < az.Length; i++)
            {
                template.AddMember(authentication, az[i].ToString(), (long)i + 1, az[i] + " Value");
            }

            template.EndEdit(authentication);
        }

        public static void GenerateStandardFlags(this ITypeCategory category, Authentication authentication)
        {
            var newName = NameUtility.GenerateNewName("Flag", category.DataBase.TypeContext.Types.Select(item => item.Name).ToArray());
            var template = category.NewType(authentication);
            template.SetTypeName(authentication, newName);
            template.SetIsFlag(authentication, true);
            template.SetComment(authentication, "Standard Flag");

            template.AddMember(authentication, "None", 0, "None Value");
            template.AddMember(authentication, "A", 1, "A Value");
            template.AddMember(authentication, "B", 2, "B Value");
            template.AddMember(authentication, "C", 4, "C Value");
            template.AddMember(authentication, "D", 8, "D Value");
            template.AddMember(authentication, "AC", 1 | 4, "AC Value");
            template.AddMember(authentication, "ABC", 1 | 2 | 4, "AC Value");
            template.AddMember(authentication, "BD", 2 | 8, "AC Value");
            template.AddMember(authentication, "All", 1 | 2 | 4 | 8, "All Value");

            template.EndEdit(authentication);
        }

        private static void GenerateStandardContent(this ITable table, Authentication authentication)
        {
            var content = table.Content;

            content.BeginEdit(authentication);
            content.EnterEdit(authentication);

            content.GenerateRows(authentication, RandomUtility.Next(10, 1000));

            foreach (var item in content.Childs)
            {
                item.GenerateRows(authentication, RandomUtility.Next(10, 100));
            }

            try
            {
                content.LeaveEdit(authentication);
                content.EndEdit(authentication);
            }
            catch
            {
                content.CancelEdit(authentication);
            }
        }
    }
}
