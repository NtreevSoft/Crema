using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Ntreev.Library.Random;
using System.Reflection;
using System.Linq;
using Ntreev.Crema.ServiceModel;
using System.Collections.Generic;

namespace Ntreev.Crema.ServerService.Test
{
    [TestClass]
    public class CremaRandomTest
    {
        private static ICremaHost cremaHost;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            var solutionDir = Path.GetDirectoryName(Path.GetDirectoryName(context.TestDir));
            var path = Path.Combine(solutionDir, "crema_repo", "test_all");
            cremaHost = TestCrema.GetInstance(path);
        }

        [TestInitialize()]
        public void Initialize()
        {
            cremaHost.Open();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            cremaHost.Dispatcher.Invoke(() => cremaHost.Close());
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            cremaHost.Dispose();
        }

        [TestMethod]
        public void TestTest()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                var authentication = cremaHost.Login("Admin", "admin");

                var category = cremaHost.PrimaryDataBase.TypeContext.Root.AddNewCategory(authentication, "sub");
                var category1 = cremaHost.PrimaryDataBase.TypeContext.Root.AddNewCategory(authentication, "other");

                var t_category_sub = cremaHost.PrimaryDataBase.TableContext.Root.AddNewCategory(authentication, "sub");
                var t_category_sub_wow = t_category_sub.AddNewCategory(authentication, "wow");
                var t_category_other = cremaHost.PrimaryDataBase.TableContext.Root.AddNewCategory(authentication, "other");
                var t_category_other_hehe = cremaHost.PrimaryDataBase.TableContext.Root.AddNewCategory(authentication, "hehe");

                var typeTemplate = cremaHost.PrimaryDataBase.TypeContext.Root.NewType(authentication);
                typeTemplate.EndEdit(authentication);

                var type = typeTemplate.Type;

                var tableTemplate = t_category_sub_wow.NewTable(authentication);
                tableTemplate.AddKey(authentication, "key", "int");
                tableTemplate.AddColumn(authentication, "value", type.Path);
                tableTemplate.EndEdit(authentication);

                var table = tableTemplate.Table;
                {
                    var childTemplate = table.NewTable(authentication);
                    childTemplate.AddKey(authentication, "key", "int");
                    childTemplate.AddColumn(authentication, "value", type.Path);
                    childTemplate.EndEdit(authentication);
                }

                {
                    var childTemplate = table.NewTable(authentication);
                    childTemplate.AddKey(authentication, "key", "int");
                    childTemplate.AddColumn(authentication, "value", type.Path);
                    childTemplate.EndEdit(authentication);
                }

                table.Inherit(authentication, "table2", t_category_other.Path, false);

                for (int i = 0; i < 100; i++)
                {
                    CremaHostUtility.TableMoveTest(cremaHost, authentication);
                    CremaHostUtility.TableRenameTest(cremaHost, authentication);
                    CremaHostUtility.TableCategoryMoveTest(cremaHost, authentication);
                    CremaHostUtility.TableCategoryRenameTest(cremaHost, authentication);
                }


                //table.Childs.ToArray()[1].Delete(authentication);
                //table.Childs.ToArray()[0].Delete(authentication);

                //type.Rename(authentication, "wow");
                //type.Move(authentication, category.Path);
                //category.Rename(authentication, "sub1");
                //category.Move(authentication, category1.Path);
                //category.Rename(authentication, "sub");
                //category1.Rename(authentication, "other1");
                ////type.Move(authentication, cremaHost.PrimaryDataBase.TypeContext.Root.Path);
                //type.Category.Delete(authentication);
            });
        }

        [TestMethod]
        public void TestAll()
        {
            var methods = typeof(CremaHostUtility).GetMethods();
            var users = new Dictionary<string, Authentication>();

            CreateBase(cremaHost);

            //while(true)
            for (var i = 0; i < 100; i++)
            {
                var method = methods.Random(Predicate);
                if (method.Name.IndexOf("Delete") >= 0)
                    continue;

                if (users.Any() == false || RandomUtility.Within(20) == true)
                {
                    LogIn(cremaHost, users);
                }

                var authentication = users.Random().Value;

                try
                {
                    method.Invoke(null, new object[] { cremaHost, authentication, });
                }
                catch(PermissionDeniedException)
                {
                    
                }

                CremaHostUtility.TableContentEditTest(cremaHost, authentication);

                if (users.Any() == true || RandomUtility.Within(10) == true)
                {
                    Logout(cremaHost, users);
                }
            }
        }

        [TestMethod]
        public void TestOne()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                var authentication = cremaHost.Login("Admin", "admin");
                var transaction = cremaHost.PrimaryDataBase.BeginTransaction(authentication);

                 //Table_thorny.Child2
                var table = cremaHost.PrimaryDataBase.TableContext.Tables["Table_thorny"];
                var template = table.Childs["Child2"].Template;
                template.BeginEdit(authentication);

                var column = template.AddNew(authentication);
                column.SetName(authentication, "wow");
                column.SetIsKey(authentication, true);
                template.EndNew(authentication, column);

                template.EndEdit(authentication);

                //transaction.Rollback();

            });
        }

        private static void CreateBase(ICremaHost cremaHost)
        {
            var authentication = cremaHost.Dispatcher.Invoke(() =>
            {
                var userContext = cremaHost.GetService<IUserContext>();
                var user = userContext.Users.Random(item => item.Authority == Authority.Admin);
                return cremaHost.Login(user.ID, "admin");
            });

            if (cremaHost.PrimaryDataBase.TypeContext.Types.Any() == false)
            {
                for (int i = 0; i < 10; i++)
                {
                    CremaHostUtility.TypeCategoryCreateTest(cremaHost, authentication);
                }

                for (int i = 0; i < 10; i++)
                {
                    CremaHostUtility.TypeCreateTest(cremaHost, authentication);
                }
            }

            if (cremaHost.PrimaryDataBase.TableContext.Tables.Any() == false)
            {
                for (int i = 0; i < 10; i++)
                {
                    CremaHostUtility.TableCategoryCreateTest(cremaHost, authentication);
                }

                for (int i = 0; i < 10; i++)
                {
                    CremaHostUtility.TableCreateTest(cremaHost, authentication);
                }

                for (int i = 0; i < 10; i++)
                {
                    CremaHostUtility.ChildTableCreateTest(cremaHost, authentication);
                }

                for (int i = 0; i < 10; i++)
                {
                    CremaHostUtility.TableInheritTest(cremaHost, authentication);
                }

                for (int i = 0; i < 10; i++)
                {
                    CremaHostUtility.ChildTableCreateTest(cremaHost, authentication);
                }
            }

            cremaHost.Dispatcher.Invoke(() =>
            {
                var userContext = cremaHost.GetService<IUserContext>();
                userContext.Logout(authentication, authentication.UserID);
            });
        }

        private static void LogIn(ICremaHost cremaHost, Dictionary<string, Authentication> users)
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                var userContext = cremaHost.GetService<IUserContext>();
                var user = userContext.Users.RandomOrDefault(item => item.Authority != Authority.Guest && item.UserState == UserState.None);
                if (user != null)
                {
                    var authentication = cremaHost.Login(user.ID, user.Authority.ToString().ToLower());
                    users.Add(user.ID, authentication);
                }
            });
        }

        private static void Logout(ICremaHost cremaHost, Dictionary<string, Authentication> users)
        {
            if (users.Any() == false)
                return;

            cremaHost.Dispatcher.Invoke(() =>
            {
                var userContext = cremaHost.GetService<IUserContext>();
                var user = users.Random();
                userContext.Logout(user.Value, user.Key);
                users.Remove(user.Key);
            });
        }

        private static bool Predicate(MethodInfo methodInfo)
        {
            if (methodInfo.IsStatic == false)
                return false;

            var parameters = methodInfo.GetParameters();
            if (parameters.Count() != 2)
                return false;

            return parameters[0].ParameterType == typeof(ICremaHost) && parameters[1].ParameterType == typeof(Authentication);
        }
    }
}
