using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServerService.Test
{
    static class CremaTableCategoryPermissionTest
    {
        public static void RenameFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Rename(authentication, RandomUtility.NextIdentifier());
                    Assert.Fail("Rename");
                }
                catch (T)
                {
                }
            });
        }

        public static void RenameParentFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Parent.Rename(authentication, RandomUtility.NextIdentifier());
                    Assert.Fail("Rename");
                }
                catch (T)
                {
                }
            });
        }

        public static void MoveFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    var categories = category.GetService(typeof(ITableCategoryCollection)) as ITableCategoryCollection;
                    category.Move(authentication, categories.Root.Path);
                    Assert.Fail("Move");
                }
                catch (T)
                {
                }
            });
        }

        public static void MoveParentFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    var categories = category.GetService(typeof(ITableCategoryCollection)) as ITableCategoryCollection;
                    category.Parent.Move(authentication, categories.Root.Path);
                    Assert.Fail("Move");
                }
                catch (T)
                {
                }
            });
        }

        public static void DeleteFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Delete(authentication);
                    Assert.Fail("Delete");
                }
                catch (T)
                {
                }
            });
        }

        public static void DeleteParentFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Parent.Delete(authentication);
                    Assert.Fail("Delete");
                }
                catch (T)
                {
                }
            });
        }

        public static void NewCategoryFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.AddNewCategory(authentication);
                    Assert.Fail("NewCategory");
                }
                catch (T)
                {
                }
            });
        }

        public static void NewTableFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.NewTable(authentication);
                    Assert.Fail("NewTable");
                }
                catch (T)
                {
                }
            });
        }

        public static void LockFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Lock(authentication, string.Empty);
                    Assert.Fail("Lock");
                }
                catch (T)
                {
                }
            });
        }

        public static void LockParentFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Parent.Lock(authentication, string.Empty);
                    Assert.Fail("Lock");
                }
                catch (T)
                {
                }
            });
        }

        public static void UnlockFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Unlock(authentication);
                    Assert.Fail("Unlock");
                }
                catch (T)
                {
                }
            });
        }

        public static void UnlockParentFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Parent.Unlock(authentication);
                    Assert.Fail("Unlock");
                }
                catch (T)
                {
                }
            });
        }

        public static void SetPrivateFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.SetPrivate(authentication);
                    Assert.Fail("SetPrivate");
                }
                catch (T)
                {
                }
            });
        }

        public static void SetPrivateParentFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Parent.SetPrivate(authentication);
                    Assert.Fail("SetPrivate");
                }
                catch (T)
                {
                }
            });
        }

        public static void SetPublicFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.SetPublic(authentication);
                    Assert.Fail("SetPublic");
                }
                catch (T)
                {
                }
            });
        }

        public static void SetPublicParentFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Parent.SetPublic(authentication);
                    Assert.Fail("SetPublic");
                }
                catch (T)
                {
                }
            });
        }

        public static void AddAccessMemberFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication, string memberID) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.AddAccessMember(authentication, memberID, AccessType.ReadWrite);
                    Assert.Fail("AddAccessMember");
                }
                catch (T)
                {
                }
            });
        }

        public static void AddAccessMemberParentFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication, string memberID) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Parent.AddAccessMember(authentication, memberID, AccessType.ReadWrite);
                    Assert.Fail("AddAccessMember");
                }
                catch (T)
                {
                }
            });
        }

        public static void RemoveAccessMemberFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication, string memberID) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.RemoveAccessMember(authentication, memberID);
                    Assert.Fail("RemoveAccessMember");
                }
                catch (T)
                {
                }
            });
        }

        public static void RemoveAccessMemberParentFailTest<T>(ICremaHost cremaHost, ITableCategory category, Authentication authentication, string memberID) where T : Exception
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    category.Parent.RemoveAccessMember(authentication, memberID);
                    Assert.Fail("RemoveAccessMember");
                }
                catch (T)
                {
                }
            });
        }
    }
}
