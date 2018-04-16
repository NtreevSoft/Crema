using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.Library.Linq;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServerService.Test
{
    static class CremaTableTest
    {
        public static void RenameTest(this ITable table, Authentication authentication)
        {
            if (table.Parent == null)
            {
                var tables = table.GetService(typeof(ITableCollection)) as ITableCollection;
                var newName = NameUtility.GenerateNewName(RandomUtility.NextIdentifier(), tables.Select(item => item.Name));
                table.Rename(authentication, newName);
            }
            else
            {
                var parent = table.Parent;
                var newName = NameUtility.GenerateNewName(RandomUtility.NextIdentifier(), parent.Childs.Select(item => item.Name));
                table.Rename(authentication, newName);
            }
        }

        public static void MoveTest(this ITable table, Authentication authentication)
        {
            var categories = table.GetService(typeof(ITableCategoryCollection)) as ITableCategoryCollection;
            var category = categories.RandomOrDefault(item => item != table.Category);
            if (category == null)
            {
                Assert.Inconclusive();
            }
            table.Move(authentication, category.Path);
        }

        public static void RenameTest(this ITableCategory category, Authentication authentication)
        {
            Assert.AreNotEqual(null, category.Parent);
            var parent = category.Parent;
            var newName = NameUtility.GenerateNewName(RandomUtility.NextIdentifier(), parent.Categories.Select(item => item.Name));
            category.Rename(authentication, newName);
        }

        public static void MoveTest(this ITableCategory category, Authentication authentication)
        {
            Assert.AreNotEqual(null, category.Parent);
            var categories = category.GetService(typeof(ITableCategoryCollection)) as ITableCategoryCollection;
            var descendants = EnumerableUtility.Descendants(category, item => item.Categories);
            var target = categories.Random(item => descendants.Contains(item) == false && item != category && item != category.Parent);
            if (target == null)
            {
                Assert.Inconclusive();
            }
            category.Move(authentication, target.Path);
        }

        public static void RenameTest(this IType type, Authentication authentication)
        {
            var types = type.GetService(typeof(ITypeCollection)) as ITypeCollection;
            var newName = NameUtility.GenerateNewName(RandomUtility.NextIdentifier(), types.Select(item => item.Name));
            type.Rename(authentication, newName);
        }

        public static void MoveTest(this IType type, Authentication authentication)
        {
            var categories = type.GetService(typeof(ITypeCategoryCollection)) as ITypeCategoryCollection;
            var category = categories.RandomOrDefault(item => item != type.Category);
            if (category == null)
            {
                Assert.Inconclusive();
            }
            type.Move(authentication, category.Path);
        }

        public static void RenameTest(this ITypeCategory category, Authentication authentication)
        {
            Assert.AreNotEqual(null, category.Parent);
            var parent = category.Parent;
            var newName = NameUtility.GenerateNewName(RandomUtility.NextIdentifier(), parent.Categories.Select(item => item.Name));
            category.Rename(authentication, newName);
        }

        public static void MoveTest(this ITypeCategory category, Authentication authentication)
        {
            Assert.AreNotEqual(null, category.Parent);
            var categories = category.GetService(typeof(ITypeCategoryCollection)) as ITypeCategoryCollection;
            var descendants = EnumerableUtility.Descendants(category, item => item.Categories);
            var target = categories.Random(item => descendants.Contains(item) == false && item != category && item != category.Parent);
            if (target == null)
            {
                Assert.Inconclusive();
            }
            category.Move(authentication, target.Path);
        }
    }
}
