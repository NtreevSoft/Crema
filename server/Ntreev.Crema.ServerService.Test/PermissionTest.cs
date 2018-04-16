using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.Random;
using System.Collections.Generic;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;

namespace Ntreev.Crema.ServerService.Test
{
    [TestClass]
    public class PermissionTest
    {
        private static ICremaHost cremaHost;
        private readonly static OnlineUserCollection users = new OnlineUserCollection();

        private TestContext testContext;

        private Authentication admin;
        private Authentication member;
        private Authentication someone;
        private Authentication guest;
        private IDataBase dataBase;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            var solutionDir = Path.GetDirectoryName(Path.GetDirectoryName(context.TestDir));
            var tempDir = Path.Combine(solutionDir, "crema_repo", "permission_test", typeof(PermissionTest).Name);
            var empty = DirectoryUtility.Exists(tempDir) == false || DirectoryUtility.IsEmpty(tempDir);

            cremaHost = TestCrema.GetInstance(tempDir);
            cremaHost.Open();

            if (empty == true)
            {
                CremaSimpleGenerator.Generate(cremaHost, 10);
            }

            users.Initialize(cremaHost);
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            cremaHost.Dispatcher.Invoke(() => cremaHost.Close());
            cremaHost.Dispose();
        }

        [TestInitialize()]
        public void Initialize()
        {
            this.dataBase = cremaHost.PrimaryDataBase;
            this.admin = users.RandomAuthentication(Authority.Admin);
            this.member = users.RandomAuthentication(Authority.Member);
            this.someone = users.RandomAuthentication(Authority.Member);
            this.guest = users.RandomAuthentication(Authority.Guest);
        }

        [TestCleanup()]
        public void Cleanup()
        {
            this.admin = null;
            this.guest = null;
            this.member = null;
            this.someone = null;
            this.dataBase = null;
        }

        [TestMethod]
        public void TableFailTest()
        {
            var tableContext = this.dataBase.TableContext;
            var table = tableContext.Tables.RandomSample();
            var child = table.Childs.Random();
            var category = EnumerableUtility.Ancestors(table as IItem).Random(item => item.Parent != null) as ITableCategory;

            this.InvokeFail<PermissionException>(() => table.SetPublic(this.guest));
            this.InvokeFail<PermissionDeniedException>(() => table.SetPrivate(this.guest));
            this.InvokeFail<PermissionException>(() => table.AddAccessMember(this.guest, this.someone.UserID, AccessType.ReadWrite));
            this.InvokeFail<PermissionException>(() => table.RemoveAccessMember(this.guest, this.someone.UserID));
            this.InvokeFail<PermissionException>(() => table.Lock(this.guest, RandomUtility.NextString()));
            this.InvokeFail<PermissionException>(() => table.Unlock(this.guest));
            this.InvokeFail<PermissionDeniedException>(() => table.RenameTest(this.guest));
            this.InvokeFail<PermissionDeniedException>(() => table.MoveTest(this.guest));
            this.InvokeFail<PermissionDeniedException>(() => table.Delete(this.guest));
            this.InvokeFail<PermissionDeniedException>(() => table.NewTable(this.guest));
            this.InvokeFail<PermissionDeniedException>(() => table.Revert(this.guest, 0));

            this.InvokeFail<CremaException>(() => category.SetPublic(this.guest));
            this.InvokeFail<CremaException>(() => category.SetPrivate(this.guest));
            this.InvokeFail<CremaException>(() => category.AddAccessMember(this.guest, this.someone.UserID, AccessType.ReadWrite));
            this.InvokeFail<CremaException>(() => category.RemoveAccessMember(this.guest, this.someone.UserID));
            this.InvokeFail<CremaException>(() => category.Lock(this.guest, RandomUtility.NextString()));
            this.InvokeFail<CremaException>(() => category.Unlock(this.guest));
            this.InvokeFail<CremaException>(() => category.RenameTest(this.guest));
            this.InvokeFail<CremaException>(() => category.MoveTest(this.guest));
            this.InvokeFail<CremaException>(() => category.Delete(this.guest));

            this.InvokeFail<PermissionException>(() => child.SetPublic(this.guest));
            this.InvokeFail<PermissionDeniedException>(() => child.SetPrivate(this.guest));
            this.InvokeFail<PermissionException>(() => child.AddAccessMember(this.guest, this.someone.UserID, AccessType.ReadWrite));
            this.InvokeFail<PermissionException>(() => child.RemoveAccessMember(this.guest, this.someone.UserID));
            this.InvokeFail<PermissionException>(() => child.Lock(this.guest, RandomUtility.NextString()));
            this.InvokeFail<PermissionException>(() => child.Unlock(this.guest));
            this.InvokeFail<PermissionDeniedException>(() => child.RenameTest(this.guest));
            this.InvokeFail<PermissionDeniedException>(() => child.MoveTest(this.guest));
            this.InvokeFail<PermissionDeniedException>(() => child.Delete(this.guest));
        }

        [TestMethod]
        public void LockedTableFailTest()
        {
            var tableContext = this.dataBase.TableContext;
            var table = tableContext.Tables.RandomSample();
            var child = table.Childs.Random();
            var category = EnumerableUtility.Ancestors(table as IItem).Random(item => item.Parent != null) as ITableCategory;

            table.Lock(this.admin, string.Empty);

            try
            {
                this.InvokeFail<PermissionException>(() => table.SetPublic(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.SetPrivate(this.member));
                this.InvokeFail<PermissionException>(() => table.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionException>(() => table.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionException>(() => table.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionDeniedException>(() => table.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.Delete(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.NewTable(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.Revert(this.member, 0));

                this.InvokeFail<CremaException>(() => category.SetPublic(this.member));
                this.InvokeFail<CremaException>(() => category.SetPrivate(this.member));
                this.InvokeFail<CremaException>(() => category.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<CremaException>(() => category.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<CremaException>(() => category.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<CremaException>(() => category.Unlock(this.member));
                this.InvokeFail<CremaException>(() => category.RenameTest(this.member));
                this.InvokeFail<CremaException>(() => category.MoveTest(this.member));
                this.InvokeFail<CremaException>(() => category.Delete(this.member));

                this.InvokeFail<PermissionException>(() => child.SetPublic(this.member));
                this.InvokeFail<PermissionDeniedException>(() => child.SetPrivate(this.member));
                this.InvokeFail<PermissionException>(() => child.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionException>(() => child.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionException>(() => child.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionException>(() => child.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => child.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => child.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => child.Delete(this.member));
            }
            finally
            {
                table.Unlock(this.admin);
            }
        }

        [TestMethod]
        public void PrivateTableFailTest()
        {
            var tableContext = this.dataBase.TableContext;
            var table = tableContext.Tables.RandomSample();
            var child = table.Childs.Random();
            var category = EnumerableUtility.Ancestors(table as IItem).Random(item => item.Parent != null) as ITableCategory;

            table.SetPrivate(this.admin);

            try
            {
                this.InvokeFail<PermissionDeniedException>(() => table.SetPublic(this.member));
                this.InvokeFail<PermissionException>(() => table.SetPrivate(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionDeniedException>(() => table.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionDeniedException>(() => table.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionException>(() => table.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.Delete(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.NewTable(this.member));

                //this.InvokeFail<CremaException>(() => category.SetPublic(this.member));
                //this.InvokeFail<CremaException>(() => category.SetPrivate(this.member));
                //this.InvokeFail<CremaException>(() => category.AddAccessMember(this.member, this.someone.UserID, ItemAccess.ReadWrite));
                //this.InvokeFail<CremaException>(() => category.RemoveAccessMember(this.member, this.someone.UserID));
                //this.InvokeFail<CremaException>(() => category.Lock(this.member, RandomUtility.NextString()));
                //this.InvokeFail<CremaException>(() => category.Unlock(this.member));
                //this.InvokeFail<CremaException>(() => category.RenameTest(this.member));
                //this.InvokeFail<CremaException>(() => category.MoveTest(this.member));
                //this.InvokeFail<CremaException>(() => category.Delete(this.member));

                //this.InvokeFail<PermissionException>(() => child.SetPublic(this.member));
                //this.InvokeFail<PermissionDeniedException>(() => child.SetPrivate(this.member));
                //this.InvokeFail<PermissionException>(() => child.AddAccessMember(this.member, this.someone.UserID, ItemAccess.ReadWrite));
                //this.InvokeFail<PermissionException>(() => child.RemoveAccessMember(this.member, this.someone.UserID));
                //this.InvokeFail<PermissionException>(() => child.Lock(this.member, RandomUtility.NextString()));
                //this.InvokeFail<PermissionException>(() => child.Unlock(this.member));
                //this.InvokeFail<PermissionDeniedException>(() => child.RenameTest(this.member));
                //this.InvokeFail<PermissionDeniedException>(() => child.MoveTest(this.member));
                //this.InvokeFail<PermissionDeniedException>(() => child.Delete(this.member));
            }
            finally
            {
                table.SetPublic(this.admin);
            }
        }

        [TestMethod]
        public void LockedTableCategoryFailTest()
        {
            var tableContext = this.dataBase.TableContext;
            var category = tableContext.Categories.RandomSample();
            var table = category.Tables.Random(item => item.TemplatedParent == null && item.Childs.Any());
            var child = table.Childs.Random();

            category.Lock(this.admin, string.Empty);

            try
            {
                this.InvokeFail<PermissionException>(() => category.SetPublic(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.SetPrivate(this.member));
                this.InvokeFail<PermissionException>(() => category.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionException>(() => category.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionException>(() => category.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionDeniedException>(() => category.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.Delete(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.AddNewCategory(this.member));

                this.InvokeFail<PermissionException>(() => table.SetPublic(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.SetPrivate(this.member));
                this.InvokeFail<PermissionException>(() => table.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionException>(() => table.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionDeniedException>(() => table.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionException>(() => table.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.Delete(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.NewTable(this.member));

                this.InvokeFail<PermissionException>(() => child.SetPublic(this.member));
                this.InvokeFail<PermissionDeniedException>(() => child.SetPrivate(this.member));
                this.InvokeFail<PermissionException>(() => child.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionException>(() => child.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionDeniedException>(() => child.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionException>(() => child.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => child.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => child.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => child.Delete(this.member));
            }
            finally
            {
                category.Unlock(this.admin);
            }
        }

        [TestMethod]
        public void PrivateTableCategoryFailTest()
        {
            var tableContext = this.dataBase.TableContext;
            var category = tableContext.Categories.RandomSample();
            var table = category.Tables.Random(item => item.TemplatedParent == null && item.Childs.Any());
            var child = table.Childs.Random();

            category.SetPrivate(this.admin);

            try
            {
                this.InvokeFail<PermissionDeniedException>(() => category.SetPublic(this.member));
                this.InvokeFail<PermissionException>(() => category.SetPrivate(this.member));
                this.InvokeFail<PermissionException>(() => category.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionException>(() => category.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionDeniedException>(() => category.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionException>(() => category.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.Delete(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.AddNewCategory(this.member));

                this.InvokeFail<PermissionException>(() => table.SetPublic(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.SetPrivate(this.member));
                this.InvokeFail<PermissionException>(() => table.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionException>(() => table.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionDeniedException>(() => table.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionException>(() => table.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.Delete(this.member));
                this.InvokeFail<PermissionDeniedException>(() => table.NewTable(this.member));

                this.InvokeFail<PermissionException>(() => child.SetPublic(this.member));
                this.InvokeFail<PermissionDeniedException>(() => child.SetPrivate(this.member));
                this.InvokeFail<PermissionException>(() => child.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionException>(() => child.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionDeniedException>(() => child.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionException>(() => child.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => child.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => child.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => child.Delete(this.member));
            }
            finally
            {
                category.SetPublic(this.admin);
            }
        }

        [TestMethod]
        public void LockedTypeFailTest()
        {
            var typeContext = this.dataBase.TypeContext;
            var type = typeContext.Types.RandomSample();
            var category = EnumerableUtility.Ancestors(type as IItem).Random(item => item.Parent != null) as ITypeCategory;

            type.Lock(this.admin, string.Empty);

            try
            {
                this.InvokeFail<PermissionException>(() => type.SetPublic(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.SetPrivate(this.member));
                this.InvokeFail<PermissionException>(() => type.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionException>(() => type.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionException>(() => type.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionDeniedException>(() => type.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.Delete(this.member));

                this.InvokeFail<CremaException>(() => category.SetPublic(this.member));
                this.InvokeFail<CremaException>(() => category.SetPrivate(this.member));
                this.InvokeFail<CremaException>(() => category.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<CremaException>(() => category.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<CremaException>(() => category.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<CremaException>(() => category.Unlock(this.member));
                this.InvokeFail<CremaException>(() => category.RenameTest(this.member));
                this.InvokeFail<CremaException>(() => category.MoveTest(this.member));
                this.InvokeFail<CremaException>(() => category.Delete(this.member));
            }
            finally
            {
                type.Unlock(this.admin);
            }
        }

        [TestMethod]
        public void PrivateTypeFailTest()
        {
            var typeContext = this.dataBase.TypeContext;
            var type = typeContext.Types.RandomSample();
            var category = EnumerableUtility.Ancestors(type as IItem).Random(item => item.Parent != null) as ITypeCategory;

            type.SetPrivate(this.admin);

            try
            {
                this.InvokeFail<PermissionDeniedException>(() => type.SetPublic(this.member));
                this.InvokeFail<PermissionException>(() => type.SetPrivate(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionDeniedException>(() => type.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionDeniedException>(() => type.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionException>(() => type.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.Delete(this.member));
            }
            finally
            {
                type.SetPublic(this.admin);
            }
        }

        [TestMethod]
        public void LockedTypeCategoryFailTest()
        {
            var typeContext = this.dataBase.TypeContext;
            var category = typeContext.Categories.RandomSample();
            var type = category.Types.Random();

            category.Lock(this.admin, string.Empty);

            try
            {
                this.InvokeFail<PermissionException>(() => category.SetPublic(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.SetPrivate(this.member));
                this.InvokeFail<PermissionException>(() => category.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionException>(() => category.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionException>(() => category.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionDeniedException>(() => category.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.Delete(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.AddNewCategory(this.member));

                this.InvokeFail<PermissionException>(() => type.SetPublic(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.SetPrivate(this.member));
                this.InvokeFail<PermissionException>(() => type.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionException>(() => type.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionDeniedException>(() => type.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionException>(() => type.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.Delete(this.member));
            }
            finally
            {
                category.Unlock(this.admin);
            }
        }

        [TestMethod]
        public void PrivateTypeCategoryFailTest()
        {
            var typeContext = this.dataBase.TypeContext;
            var category = typeContext.Categories.RandomSample();
            var type = category.Types.Random();

            category.SetPrivate(this.admin);

            try
            {
                this.InvokeFail<PermissionDeniedException>(() => category.SetPublic(this.member));
                this.InvokeFail<PermissionException>(() => category.SetPrivate(this.member));
                this.InvokeFail<PermissionException>(() => category.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionException>(() => category.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionDeniedException>(() => category.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionException>(() => category.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.Delete(this.member));
                this.InvokeFail<PermissionDeniedException>(() => category.AddNewCategory(this.member));

                this.InvokeFail<PermissionException>(() => type.SetPublic(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.SetPrivate(this.member));
                this.InvokeFail<PermissionException>(() => type.AddAccessMember(this.member, this.someone.UserID, AccessType.ReadWrite));
                this.InvokeFail<PermissionException>(() => type.RemoveAccessMember(this.member, this.someone.UserID));
                this.InvokeFail<PermissionDeniedException>(() => type.Lock(this.member, RandomUtility.NextString()));
                this.InvokeFail<PermissionException>(() => type.Unlock(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.RenameTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.MoveTest(this.member));
                this.InvokeFail<PermissionDeniedException>(() => type.Delete(this.member));
            }
            finally
            {
                category.SetPublic(this.admin);
            }
        }

        public TestContext TestContext
        {
            get { return this.testContext; }
            set { this.testContext = value; }
        }

        private void InvokeFail<T>(Action action) where T : Exception
        {
            try
            {
                action();
                Assert.Fail();
            }
            catch (T)
            {

            }
        }
    }
}
