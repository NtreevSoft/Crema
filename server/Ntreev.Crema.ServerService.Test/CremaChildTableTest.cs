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
    static class CremaChildTableTest
    {
        public static void RenameFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    var parent = table.Parent;
                    var newName = NameUtility.GenerateNewName(RandomUtility.NextIdentifier(), parent.Childs.Select(item => item.Name));
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
            Assert.AreNotEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    var categories = table.GetService(typeof(ITableCategoryCollection)) as ITableCategoryCollection;
                    var parent = categories.RandomOrDefault(item => item.VerifyWrite(authentication));
                    table.Move(authentication, parent.Path);
                    Assert.Fail("MoveFailTest");
                }
                catch (T)
                {
                }
            });
        }

        public static void DeleteFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, table.Parent);
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

        public static void NewChildFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, table.Parent);
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
            Assert.AreNotEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.Content.BeginEdit(authentication);
                    Assert.Fail("Content.BeginEdit");
                }
                catch (T)
                {
                }
            });
        }

        public static void TemplateEditFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.Template.BeginEdit(authentication);
                    Assert.Fail("Template.BeginEdit");
                }
                catch (T)
                {
                }
            });
        }

        public static void LockFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.Lock(authentication, string.Empty);
                    Assert.Fail("Lock");
                }
                catch (T)
                {
                }
            });
        }

        public static void UnlockFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.Unlock(authentication);
                    Assert.Fail("Unlock");
                }
                catch (T)
                {
                }
            });
        }

        public static void SetPrivateFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.SetPrivate(authentication);
                    Assert.Fail("SetPrivate");
                }
                catch (T)
                {
                }
            });
        }

        public static void SetPublicFailTest<T>(ITable table, Authentication authentication) where T : Exception
        {
            Assert.AreNotEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.SetPublic(authentication);
                    Assert.Fail("SetPublic");
                }
                catch (T)
                {
                }
            });
        }

        public static void AddAccessMemberFailTest<T>(ITable table, Authentication authentication, string memberID) where T : Exception
        {
            Assert.AreNotEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.AddAccessMember(authentication, memberID, AccessType.ReadWrite);
                    Assert.Fail("AddAccessMember");
                }
                catch (T)
                {
                }
            });
        }

        public static void RemoveAccessMemberFailTest<T>(ITable table, Authentication authentication, string memberID) where T : Exception
        {
            Assert.AreNotEqual(null, table.Parent);
            table.Dispatcher.Invoke(() =>
            {
                try
                {
                    table.RemoveAccessMember(authentication, memberID);
                    Assert.Fail("RemoveAccessMember");
                }
                catch (T)
                {
                }
            });
        }
    }
}
