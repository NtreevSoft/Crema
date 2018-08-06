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
    public class IType_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IDataBase dataBase;
        private static IType type;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(IType_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                dataBase = cremaHost.DataBases.Random();
                dataBase.Load(authentication);
                dataBase.Enter(authentication);
                dataBase.TypeContext.AddRandomItems(authentication);
                type = dataBase.TypeContext.Types.Random();
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
            type.Rename(authentication, RandomUtility.NextIdentifier());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Move()
        {
            type.Move(authentication, PathUtility.Separator);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Delete()
        {
            type.Delete(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Copy()
        {
            type.Copy(authentication, RandomUtility.NextIdentifier(), PathUtility.Separator);
        }

        [TestMethod]
        public void Preview()
        {
            type.GetDataSet(authentication, null);
        }

        [TestMethod]
        public void GetLog()
        {
            type.GetLog(authentication, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Name()
        {
            Console.Write(type.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Path()
        {
            Console.Write(type.Path);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IsLocked()
        {
            Console.Write(type.IsLocked);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IsPrivate()
        {
            Console.Write(type.IsPrivate);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LockInfo()
        {
            Console.Write(type.LockInfo);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AccessInfo()
        {
            Console.Write(type.AccessInfo);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TypeInfo()
        {
            Console.Write(type.TypeInfo);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TypeState()
        {
            Console.Write(type.TypeState);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Category()
        {
            Console.Write(type.Category);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Template()
        {
            Console.Write(type.Template);
        }

        [TestMethod]
        public void ExtendedProperties()
        {
            Console.Write(type.ExtendedProperties);
        }

        [TestMethod]
        public void Dispatcher()
        {
            Console.Write(type.Dispatcher);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Renamed()
        {
            type.Renamed += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Moved()
        {
            type.Moved += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deleted()
        {
            type.Deleted += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LockChanged()
        {
            type.LockChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AccessChanged()
        {
            type.AccessChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TypeInfoChanged()
        {
            type.TypeInfoChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TypeStateChanged()
        {
            type.TypeStateChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetPublic()
        {
            type.SetPublic(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetPrivate()
        {
            type.SetPrivate(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddAccessMember()
        {
            type.AddAccessMember(authentication, "admin", ServiceModel.AccessType.Owner);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveAccessMember()
        {
            type.RemoveAccessMember(authentication, "admin");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Lock()
        {
            type.Lock(authentication, RandomUtility.NextString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Unlock()
        {
            type.Unlock(authentication);
        }

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void VerifyWrite()
        //{
        //    type.VerifyWrite(authentication);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void VerifyRead()
        //{
        //    type.VerifyRead(authentication);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void VerifyOwner()
        //{
        //    type.VerifyOwner(authentication);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void VerifyMember()
        //{
        //    type.VerifyMember(authentication);
        //}

        [TestMethod]
        public void GetService()
        {
            Console.Write(type.GetService(typeof(ICremaHost)));
        }
    }
}
