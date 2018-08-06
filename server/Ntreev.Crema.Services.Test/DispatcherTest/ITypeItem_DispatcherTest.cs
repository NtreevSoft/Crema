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

namespace Ntreev.Crema.Services.Test.DispatcherTest
{
    [TestClass]
    public class ITypeItem_DispatcherTest
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
            app.Initialize(context, nameof(ITypeItem_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                dataBase = cremaHost.DataBases.Random();
                dataBase.Load(authentication);
                dataBase.Enter(authentication);
                dataBase.TypeContext.AddRandomItems(authentication);
                typeItem = dataBase.TypeContext.Random();
            });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                dataBase.Unload(authentication);
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
        public void GetLog()
        {
            typeItem.GetLog(authentication, null);
        }

        [TestMethod]
        public void Find()
        {
            typeItem.Find(authentication, "1", ServiceModel.FindOptions.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Name()
        {
            Console.Write(typeItem.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Path()
        {
            Console.Write(typeItem.Path);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IsLocked()
        {
            Console.Write(typeItem.IsLocked);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IsPrivate()
        {
            Console.Write(typeItem.IsPrivate);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Parent()
        {
            Console.Write(typeItem.Parent);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Childs()
        {
            foreach(var item in typeItem.Childs)
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LockInfo()
        {
            Console.Write(typeItem.LockInfo);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
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
            Console.Write(typeItem.Dispatcher);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Renamed()
        {
            typeItem.Renamed += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Moved()
        {
            typeItem.Moved += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deleted()
        {
            typeItem.Deleted += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LockChanged()
        {
            typeItem.LockChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
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
