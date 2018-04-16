using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.Random;
using System.Collections.Generic;
using Ntreev.Library;
using Ntreev.Library.IO;

namespace Ntreev.Crema.ServerService.Test
{
    [TestClass]
    public class TypeGuestPermissionTest
    {
        private static ICremaHost cremaHost;

        private readonly static Dictionary<IUser, Authentication> users = new Dictionary<IUser, Authentication>();

        private TestContext testContext;

        private Authentication guest;
        private IType type;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            var solutionDir = Path.GetDirectoryName(Path.GetDirectoryName(context.TestDir));
            var tempDir = Path.Combine(solutionDir, "crema_repo", "permission_test", "type_guest");
            var empty = DirectoryUtility.Exists(tempDir) == false || DirectoryUtility.IsEmpty(tempDir);

            cremaHost = TestCrema.GetInstance(tempDir);
            cremaHost.Open();

            if (empty == true)
            {
                CremaSimpleGenerator.Generate(cremaHost, 10);
            }

            cremaHost.Dispatcher.Invoke(() =>
            {
                var userContext = cremaHost.GetService<IUserContext>();

                foreach (var item in userContext.Users)
                {
                    var authentication = cremaHost.Login(item.ID, item.Authority.ToString().ToLower());
                    users.Add(item, authentication);
                }
            });
        }

        [TestInitialize()]
        public void Initialize()
        {
            var dataBase = cremaHost.PrimaryDataBase;
            var typeContext = dataBase.TypeContext;
            this.guest = users.Where(item => item.Key.Authority == Authority.Guest).Random().Value;
            this.type = typeContext.Types.RandomOrDefault(item => item.IsLocked == false);
        }

        [TestCleanup()]
        public void Cleanup()
        {
            this.guest = null;
            this.type = null;
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            cremaHost.Dispatcher.Invoke(() => cremaHost.Close());
            cremaHost.Dispose();
        }

        [TestMethod]
        public void TypeFailTest()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    var types = this.type.GetService(typeof(ITypeCollection)) as ITypeCollection;
                    var type = types.RandomOrDefault(item => item.VerifyWrite(this.guest));
                    Assert.IsNull(type);
                }
                catch (PermissionDeniedException)
                {
                }
            });
        }

        [TestMethod]
        public void TypeRenameFailTest()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    var types = this.type.GetService(typeof(ITypeCollection)) as ITypeCollection;
                    var newName = NameUtility.GenerateNewName(RandomUtility.NextIdentifier(), types.Select(item => item.Name));
                    this.type.Rename(this.guest, newName);
                    Assert.Fail("Rename");
                }
                catch (PermissionDeniedException)
                {
                }
            });
        }

        [TestMethod]
        public void TypeMoveFailTest()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    var categories = this.type.GetService(typeof(ITypeCategoryCollection)) as ITypeCategoryCollection;
                    var category = categories.RandomOrDefault(item => item != this.type.Category);
                    this.type.Move(this.guest, category.Path);
                    Assert.Fail("Move");
                }
                catch (PermissionDeniedException)
                {
                }
            });
        }

        [TestMethod]
        public void TypeDeleteFailTest()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    this.type.Delete(this.guest);
                    Assert.Fail("Delete");
                }
                catch (PermissionDeniedException)
                {
                }
            });
        }

        [TestMethod]
        public void TypeTemplateEditFailTest()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    this.type.Template.BeginEdit(this.guest);
                    Assert.Fail("Template.BeginEdit");
                }
                catch (PermissionDeniedException)
                {
                }
            });
        }

        [TestMethod]
        public void TypeLockFailTest()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    this.type.Lock(this.guest, string.Empty);
                    Assert.Fail("Lock");
                }
                catch (PermissionDeniedException)
                {
                }
            });
        }

        [TestMethod]
        public void TypeUnlockFailTest()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    this.type.Unlock(this.guest);
                    Assert.Fail("Unlock");
                }
                catch (PermissionDeniedException)
                {
                }
            });
        }

        [TestMethod]
        public void TypeSetPrivateFailTest()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    this.type.SetPrivate(this.guest);
                    Assert.Fail("SetPrivate");
                }
                catch (PermissionDeniedException)
                {
                }
            });
        }

        [TestMethod]
        public void TypeSetPublicFailTest()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    this.type.SetPublic(this.guest);
                    Assert.Fail("SetPublic");
                }
                catch (PermissionDeniedException)
                {
                }
            });
        }

        [TestMethod]
        public void TypeAddAccessMemberFailTest()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    this.type.AddAccessMember(this.guest, users.Keys.Random().ID, AccessType.ReadWrite);
                    Assert.Fail("AddAccessMember");
                }
                catch (PermissionDeniedException)
                {
                }
            });
        }

        [TestMethod]
        public void TypeRemoveAccessMemberFailTest()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    this.type.RemoveAccessMember(this.guest, users.Keys.Random().ID);
                    Assert.Fail("RemoveAccessMember");
                }
                catch (PermissionDeniedException)
                {
                }
            });
        }

        public TestContext TestContext
        {
            get { return this.testContext; }
            set { this.testContext = value; }
        }
    }
}
