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
    public class ITypeItem_Deleted_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IDataBase dataBase;
        private static ITypeItem typeItem;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(ITypeItem_Deleted_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                dataBase = cremaHost.DataBases.Random();
                dataBase.Load(authentication);
                dataBase.Enter(authentication);
                dataBase.TypeContext.AddRandomItems(authentication);
                typeItem = dataBase.TypeContext.Random();
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
            typeItem.Rename(authentication, RandomUtility.NextIdentifier());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Move()
        {
            typeItem.Move(authentication, PathUtility.Separator);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Delete()
        {
            typeItem.Delete(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetLog()
        {
            typeItem.GetLog(authentication, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Find()
        {
            typeItem.Find(authentication, "1", ServiceModel.FindOptions.None);
        }

        [TestMethod]
        public void Name()
        {
            Console.Write(typeItem.Name);
        }

        [TestMethod]
        public void Path()
        {
            Console.Write(typeItem.Path);
        }

        [TestMethod]
        public void IsLocked()
        {
            Console.Write(typeItem.IsLocked);
        }

        [TestMethod]
        public void IsPrivate()
        {
            Console.Write(typeItem.IsPrivate);
        }

        [TestMethod]
        public void Parent()
        {
            Console.Write(typeItem.Parent);
        }

        [TestMethod]
        public void Childs()
        {
            foreach (var item in typeItem.Childs)
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        public void LockInfo()
        {
            Console.Write(typeItem.LockInfo);
        }

        [TestMethod]
        public void AccessInfo()
        {
            Console.Write(typeItem.AccessInfo);
        }

        [TestMethod]
        public void ExtendedProperties()
        {
            Console.Write(typeItem.ExtendedProperties);
        }

        [TestMethod]
        public void Dispatcher()
        {
            Assert.IsNull(typeItem.Dispatcher);
        }

        [TestMethod]
        public void Renamed()
        {
            typeItem.Renamed += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void Moved()
        {
            typeItem.Moved += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void Deleted()
        {
            typeItem.Deleted += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void LockChanged()
        {
            typeItem.LockChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void AccessChanged()
        {
            typeItem.AccessChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetPublic()
        {
            typeItem.SetPublic(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetPrivate()
        {
            typeItem.SetPrivate(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddAccessMember()
        {
            typeItem.AddAccessMember(authentication, "admin", ServiceModel.AccessType.Owner);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveAccessMember()
        {
            typeItem.RemoveAccessMember(authentication, "admin");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Lock()
        {
            typeItem.Lock(authentication, RandomUtility.NextString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Unlock()
        {
            typeItem.Unlock(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAccessType()
        {
            typeItem.GetAccessType(authentication);
        }

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void VerifyRead()
        //{
        //    typeItem.VerifyRead(authentication);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void VerifyOwner()
        //{
        //    typeItem.VerifyOwner(authentication);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void VerifyMember()
        //{
        //    typeItem.VerifyMember(authentication);
        //}
    }
}
