using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServerService.Test
{
    static class CremaTableFailTest
    {
        public static void RenameFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    var tables = table.GetService(typeof(ITableCollection)) as ITableCollection;
                    var newName = NameUtility.GenerateNewName(RandomUtility.NextIdentifier(), tables.Select(item => item.Name));
                    table.Rename(authentication, newName);
                    Assert.Fail("RenameFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void MoveFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    var categories = table.GetService(typeof(ITableCategoryCollection)) as ITableCategoryCollection;
                    var category = categories.RandomOrDefault(item => item != table.Category);
                    table.Move(authentication, category.Path);
                    Assert.Fail("MoveFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void DeleteFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.Delete(authentication);
                    Assert.Fail("DeleteFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void NewChildFailTest<T>(ITable table, Authentication authentication)  where T : Exception
        {
            Assert.AreEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.NewTable(authentication);
                    Assert.Fail("NewChildFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void ContentEditFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.Content.BeginEdit(authentication);
                    Assert.Fail("ContentEditFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void TemplateEditFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.Template.BeginEdit(authentication);
                    Assert.Fail("TemplateEditFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void LockFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.Lock(authentication, string.Empty);
                    Assert.Fail("LockFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void UnlockFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.Unlock(authentication);
                    Assert.Fail("UnlockFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void SetPrivateFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.SetPrivate(authentication);
                    Assert.Fail("SetPrivateFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void SetPublicFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.SetPublic(authentication);
                    Assert.Fail("SetPublicFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void AddAccessMemberFailTest<T>(ITable table, Authentication authentication, string memberID) where T :Exception
        {
            Assert.AreEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.AddAccessMember(authentication, memberID, AccessType.ReadWrite);
                    Assert.Fail("AddAccessMemberFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void RemoveAccessMemberFailTest<T>(ITable table, Authentication authentication, string memberID) where T : Exception
        {
            Assert.AreEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.RemoveAccessMember(authentication, memberID);
                    Assert.Fail("RemoveAccessMemberFailTest");
                }
                catch (T)
                {
                }
            });
        }
    }
}
