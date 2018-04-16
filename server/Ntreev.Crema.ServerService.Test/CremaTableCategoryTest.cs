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
    static class CremaTableCategoryTest
    {
        public static void RenameFailTest<T>(ITableCategory category, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, category.Parent);
            category.Dispatcher.Invoke(() =>
            {
                try
                {
                    var parent = category.Parent;
                    var newName = NameUtility.GenerateNewName(RandomUtility.NextIdentifier(), parent.Categories.Select(item => item.Name));
                    category.Rename(authentication, newName);
                    Assert.Fail("RenameFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void MoveFailTest<T>(ITableCategory category, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, category.Parent);
            category.Dispatcher.Invoke(() =>
            {
                try
                {
                    var categories = category.GetService(typeof(ITableCategoryCollection)) as ITableCategoryCollection;
                    var descendants = EnumerableUtility.Descendants(category, item => item.Categories);
                    var target = categories.Random(item => descendants.Contains(item) == false && item != category.Parent);
                    category.Move(authentication, target.Path);
                    Assert.Fail("MoveFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void DeleteFailTest<T>(ITableCategory category, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, category.Parent);
            category.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Delete(authentication);
                    Assert.Fail("DeleteFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void LockFailTest<T>(ITableCategory category, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, category.Parent);
            category.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Lock(authentication, RandomUtility.NextString());
                    Assert.Fail("LockFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void UnlockFailTest<T>(ITableCategory category, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, category.Parent);
            category.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Unlock(authentication);
                    Assert.Fail("UnlockFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void SetPrivateFailTest<T>(ITableCategory category, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, category.Parent);
            category.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.SetPrivate(authentication);
                    Assert.Fail("SetPrivateFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void SetPublicFailTest<T>(ITableCategory category, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, category.Parent);
            category.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.SetPublic(authentication);
                    Assert.Fail("SetPublicFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void AddAccessMemberFailTest<T>(ITableCategory category, Authentication authentication, string memberID) where T : Exception
        {
            Assert.AreNotEqual(null, category.Parent);
            category.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.AddAccessMember(authentication, memberID, AccessType.ReadWrite);
                    Assert.Fail("AddAccessMemberFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void RemoveAccessMemberFailTest<T>(ITableCategory category, Authentication authentication, string memberID) where T : Exception
        {
            Assert.AreNotEqual(null, category.Parent);
            category.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.RemoveAccessMember(authentication, memberID);
                    Assert.Fail("RemoveAccessMemberFailTest");
                }
                catch (T)
                {
                }
            });
        }
    }
}
