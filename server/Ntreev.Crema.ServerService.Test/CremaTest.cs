//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Ntreev.Library.Random;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Ntreev.Library;
//using Ntreev.Crema.ServiceModel;
//using Ntreev.Library.Linq;
//using System.IO;

//namespace Ntreev.Crema.ServerService.Test
//{
//    [TestClass]
//    public class CremaTest
//    {
//        private static ICremaHost cremaHost;
//        private int repeatCount = 1;

//        [ClassInitialize()]
//        public static void ClassInit(TestContext context)
//        {
//            var solutionDir = Path.GetDirectoryName(Path.GetDirectoryName(context.TestDir));
//            var path = Path.Combine(solutionDir, "crema_repo", "test_all");
//            cremaHost = TestCrema.GetInstance(path);
//            cremaHost.Open();
//        }

//        [TestInitialize()]
//        public void Initialize()
//        {

//        }

//        [TestCleanup()]
//        public void Cleanup()
//        {

//        }

//        [ClassCleanup()]
//        public static void ClassCleanup()
//        {
//            cremaHost.Dispatcher.Invoke(() => cremaHost.Close());
//            cremaHost.Dispose();
//        }

//        [TestMethod]
//        public void UserCreateTest()
//        {
//            cremaHost.UserCreateTest();
//        }

//        [TestMethod]
//        public void TypeCreateTest()
//        {
//            for (int i = 0; i < repeatCount; i++)
//            {
//                cremaHost.TypeCreateTest();
//            }
//        }

//        [TestMethod]
//        public void TypeRenameTest()
//        {
//            for (int i = 0; i < repeatCount; i++)
//            {
//                cremaHost.TypeRenameTest();
//            }
//        }

//        [TestMethod]
//        public void TypeMoveTest()
//        {
//            cremaHost.TypeMoveTest();
//        }

//        [TestMethod]
//        public void TypeEditTest()
//        {
//            cremaHost.TypeTemplateEditTest();
//        }

//        //[TestMethod]
//        //public void TypeMemberCreateTest()
//        //{
//        //    cremaHost.TypeMemberCreateTest();
//        //}

//        //[TestMethod]
//        //public void TypeMemberDeleteTest()
//        //{
//        //    cremaHost.TypeMemberDeleteTest();
//        //}

//        //[TestMethod]
//        //public void TypeMemberChangeTest()
//        //{
//        //    cremaHost.TypeMemberChangeTest();
//        //}

//        [TestMethod]
//        public void TypeCategoryCreateTest()
//        {
//            for (int i = 0; i < repeatCount; i++)
//            {
//                cremaHost.TypeCategoryCreateTest();
//            }
//        }

//        [TestMethod]
//        public void TypeCategoryRenameTest()
//        {
//            for (int i = 0; i < repeatCount; i++)
//            {
//                cremaHost.TypeCategoryRenameTest();
//            }
//        }

//        [TestMethod]
//        public void TypeCategoryMoveTest()
//        {
//            for (int i = 0; i < repeatCount; i++)
//            {
//                cremaHost.TypeCategoryMoveTest();
//            }
//        }

//        [TestMethod]
//        public void TypeCategoryDeleteTest()
//        {
//            for (int i = 0; i < repeatCount; i++)
//            {
//                cremaHost.TypeCategoryDeleteTest();
//            }
//        }

//        [TestMethod]
//        public void TableCreateTest()
//        {
//            for (int i = 0; i < repeatCount; i++)
//            {
//                cremaHost.TableCreateTest();
//            }
//        }

//        [TestMethod]
//        public void ChildTableCreateTest()
//        {
//            for (int i = 0; i < repeatCount; i++)
//            {
//                cremaHost.ChildTableCreateTest();
//            }
//        }

//        [TestMethod]
//        public void TableRenameTest()
//        {
//            for (int i = 0; i < repeatCount; i++)
//            {
//                cremaHost.TableRenameTest();
//            }
//        }

//        [TestMethod]
//        public void TableTemplateEditTest()
//        {
//            cremaHost.TableTemplateEditTest();
//        }

//        [TestMethod]
//        public void ChildTableRenameTest()
//        {
//            cremaHost.ChildTableRenameTest();
//        }

//        [TestMethod]
//        public void TableMoveTest()
//        {
//            for (int i = 0; i < repeatCount; i++)
//            {
//                cremaHost.TableMoveTest();
//            }
//        }

//        [TestMethod]
//        public void TableCategoryCreateTest()
//        {
//            for (int i = 0; i < repeatCount; i++)
//            {
//                cremaHost.TableCategoryCreateTest();
//            }
//        }

//        [TestMethod]
//        public void TableCategoryRenameTest()
//        {
//            for (int i = 0; i < repeatCount; i++)
//            {
//                cremaHost.TableCategoryRenameTest();
//            }
//        }

//        [TestMethod]
//        public void TableCategoryMoveTest()
//        {
//            for (int i = 0; i < repeatCount; i++)
//            {
//                cremaHost.TableCategoryMoveTest();
//            }
//        }

//        [TestMethod]
//        public void TableInheritTest()
//        {
//            for (int i = 0; i < repeatCount; i++)
//            {
//                cremaHost.TableInheritTest();
//            }
//        }
//    }
//}
