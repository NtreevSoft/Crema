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
    public class ITable_Deleted_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IDataBase dataBase;
        private static ITable table;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(ITable_Deleted_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                dataBase = cremaHost.DataBases.Random();
                dataBase.Load(authentication);
                dataBase.Enter(authentication);
                dataBase.Initialize(authentication);
                table = dataBase.TableContext.Tables.Random();
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
            table.Rename(authentication, RandomUtility.NextIdentifier());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Move()
        {
            table.Move(authentication, PathUtility.Separator);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Delete()
        {
            table.Delete(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Copy()
        {
            table.Copy(authentication, RandomUtility.NextIdentifier(), PathUtility.Separator, true);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Inherit()
        {
            table.Inherit(authentication, RandomUtility.NextIdentifier(), PathUtility.Separator, true);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NewTable()
        {
            table.NewTable(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Preview()
        {
            table.GetDataSet(authentication, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetLog()
        {
            table.GetLog(authentication, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Find()
        {
            table.Find(authentication, "1", ServiceModel.FindOptions.None);
        }

        [TestMethod]
        public void Parent()
        {
            Console.Write(table.Parent);
        }

        [TestMethod]
        public void Name()
        {
            Console.Write(table.Name);
        }

        [TestMethod]
        public void TableName()
        {
            Console.Write(table.TableName);
        }

        [TestMethod]
        public void Path()
        {
            Console.Write(table.Path);
        }

        [TestMethod]
        public void IsLocked()
        {
            Console.Write(table.IsLocked);
        }

        [TestMethod]
        public void IsPrivate()
        {
            Console.Write(table.IsPrivate);
        }

        [TestMethod]
        public void LockInfo()
        {
            Console.Write(table.LockInfo);
        }

        [TestMethod]
        public void AccessInfo()
        {
            Console.Write(table.AccessInfo);
        }

        [TestMethod]
        public void TableInfo()
        {
            Console.Write(table.TableInfo);
        }

        [TestMethod]
        public void TableState()
        {
            Console.Write(table.TableState);
        }

        [TestMethod]
        public void Category()
        {
            Console.Write(table.Category);
        }

        [TestMethod]
        public void Childs()
        {
            Console.Write(table.Childs);
        }

        [TestMethod]
        public void DerivedTables()
        {
            Console.Write(table.DerivedTables);
        }

        [TestMethod]
        public void TemplatedParent()
        {
            Console.Write(table.TemplatedParent);
        }

        [TestMethod]
        public void Template()
        {
            Console.Write(table.Template);
        }

        [TestMethod]
        public void Content()
        {
            Console.Write(table.Content);
        }

        [TestMethod]
        public void ExtendedProperties()
        {
            Console.Write(table.ExtendedProperties);
        }

        [TestMethod]
        public void Dispatcher()
        {
            Assert.IsNull(table.Dispatcher);
        }

        [TestMethod]
        public void Renamed()
        {
            table.Renamed += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void Moved()
        {
            table.Moved += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void Deleted()
        {
            table.Deleted += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void LockChanged()
        {
            table.LockChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void AccessChanged()
        {
            table.AccessChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void TableInfoChanged()
        {
            table.TableInfoChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void TableStateChanged()
        {
            table.TableStateChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetPublic()
        {
            table.SetPublic(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetPrivate()
        {
            table.SetPrivate(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddAccessMember()
        {
            table.AddAccessMember(authentication, "admin", ServiceModel.AccessType.Owner);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveAccessMember()
        {
            table.RemoveAccessMember(authentication, "admin");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Lock()
        {
            table.Lock(authentication, RandomUtility.NextString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Unlock()
        {
            table.Unlock(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAccessType()
        {
            table.GetAccessType(authentication);
        }

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void VerifyRead()
        //{
        //    table.VerifyRead(authentication);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void VerifyOwner()
        //{
        //    table.VerifyOwner(authentication);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void VerifyMember()
        //{
        //    table.VerifyMember(authentication);
        //}

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetService()
        {
            Console.Write(table.GetService(typeof(ICremaHost)));
        }
    }
}
