//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ntreev.Library;
using System.IO;
using Ntreev.Library.IO;
using Ntreev.Library.Random;
using Ntreev.Crema.Services.Random;

namespace Ntreev.Crema.Services.Test.Deleted_DispatcherTest
{
    [TestClass]
    public class ITypeCategory_Deleted_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IDataBase dataBase;
        private static ITypeCategory category;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(ITypeCategory_Deleted_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                dataBase = cremaHost.DataBases.Random();
                dataBase.Load(authentication);
                dataBase.Enter(authentication);
                dataBase.TypeContext.AddRandomItems(authentication);
                category = dataBase.TypeContext.Categories.Random();
                dataBase.Leave(authentication);
                dataBase.Unload(authentication);
            });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                cremaHost.Stop(authentication);
            });
            app.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Rename()
        {
            category.Rename(authentication, RandomUtility.NextIdentifier());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Move()
        {
            category.Move(authentication, PathUtility.Separator);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Delete()
        {
            category.Delete(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddNewCategory()
        {
            category.AddNewCategory(authentication, RandomUtility.NextIdentifier());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NewType()
        {
            category.NewType(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Preview()
        {
            category.GetDataSet(authentication, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetLog()
        {
            category.GetLog(authentication, null);
        }

        [TestMethod]
        public void Name()
        {
            Console.Write(category.Name);
        }

        [TestMethod]
        public void Path()
        {
            Console.Write(category.Path);
        }

        [TestMethod]
        public void IsLocked()
        {
            Console.Write(category.IsLocked);
        }

        [TestMethod]
        public void IsPrivate()
        {
            Console.Write(category.IsPrivate);
        }

        [TestMethod]
        public void LockInfo()
        {
            Console.Write(category.LockInfo);
        }

        [TestMethod]
        public void AccessInfo()
        {
            Console.Write(category.AccessInfo);
        }

        [TestMethod]
        public void Parent()
        {
            Console.Write(category.Parent);
        }

        [TestMethod]
        public void Categories()
        {
            Console.Write(category.Categories);
        }

        [TestMethod]
        public void Types()
        {
            Console.Write(category.Types);
        }

        [TestMethod]
        public void ExtendedProperties()
        {
            Console.Write(category.ExtendedProperties);
        }

        [TestMethod]
        public void Dispatcher()
        {
            Console.Write(category.Dispatcher);
        }

        [TestMethod]
        public void Renamed()
        {
            category.Renamed += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void Moved()
        {
            category.Moved += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void Deleted()
        {
            category.Deleted += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void LockChanged()
        {
            category.LockChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void AccessChanged()
        {
            category.AccessChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetPublic()
        {
            category.SetPublic(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetPrivate()
        {
            category.SetPrivate(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddAccessMember()
        {
            category.AddAccessMember(authentication, "admin", ServiceModel.AccessType.Owner);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveAccessMember()
        {
            category.RemoveAccessMember(authentication, "admin");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Lock()
        {
            category.Lock(authentication, RandomUtility.NextString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Unlock()
        {
            category.Unlock(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAccessType()
        {
            category.GetAccessType(authentication);
        }

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void VerifyRead()
        //{
        //    category.VerifyRead(authentication);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void VerifyOwner()
        //{
        //    category.VerifyOwner(authentication);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void VerifyMember()
        //{
        //    category.VerifyMember(authentication);
        //}

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetService()
        {
            Console.Write(category.GetService(typeof(ICremaHost)));
        }
    }
}
