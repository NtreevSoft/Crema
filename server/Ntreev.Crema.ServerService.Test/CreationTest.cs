using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Ntreev.Library.IO;
using Ntreev.Library;
using System.Linq;
using Ntreev.Crema.SvnModule;
using Ntreev.Library.Random;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.ServerService.Test
{
    [TestClass]
    public class CreationTest
    {
        private static ICremaHost cremaHost;
        private static string tempDir;

        private TestContext testContext;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            var solutionDir = Path.GetDirectoryName(Path.GetDirectoryName(context.TestDir));
            tempDir = Path.Combine(solutionDir, "crema_repo", "unit_test");
        }

        [TestInitialize()]
        public void Initialize()
        {
            cremaHost = TestCrema.CreateInstance(tempDir);
            cremaHost.Open();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            cremaHost.Dispatcher.Invoke(() => cremaHost.Close());
            cremaHost.Dispose();
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            
        }

        [TestMethod]
        public void BaseTest()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                var userContext = cremaHost.GetService<IUserContext>();
                var user = userContext.Users.Random(item => item.Authority != Authority.Guest);
                var authentication = cremaHost.Login(user.ID, user.Authority.ToString().ToLower());
                var dataBase = cremaHost.PrimaryDataBase;

                var tableContext = dataBase.TableContext;

                var category = tableContext.Root.AddNewCategory(authentication, "Folder1");

                var category1 = category.AddNewCategory(authentication, "Folder1");

                var template = category1.NewTable(authentication);
                template.EndEdit(authentication);

                var table = template.Table;

                var childTemplate1 = table.NewTable(authentication);
                childTemplate1.EndEdit(authentication);

                var child = childTemplate1.Table;
                child.Rename(authentication, "Child_abc");

                var childTemplate2 = table.NewTable(authentication);
                childTemplate2.EndEdit(authentication);
            });
        }

        [TestMethod]
        public void GenerateStandardTest()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                var userContext = cremaHost.GetService<IUserContext>();
                var user = userContext.Users.Random(item => item.Authority != Authority.Guest);
                var authentication = cremaHost.Login(user.ID, user.Authority.ToString().ToLower());
                var dataBase = cremaHost.PrimaryDataBase;
                var tableContext = dataBase.TableContext;

                var time = DateTime.Now;
                
                var transaction = dataBase.BeginTransaction(authentication);
                dataBase.GenerateStandard(authentication);

                var diff = DateTime.Now - time;

                transaction.Commit();
            });
        }

        public TestContext TestContext
        {
            get { return this.testContext; }
            set { this.testContext = value; }
        }
    }
}