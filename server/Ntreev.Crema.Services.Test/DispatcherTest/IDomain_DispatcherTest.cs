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
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Services.Test.DispatcherTest
{
    [TestClass]
    public class IDomain_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IDataBase dataBase;
        private static ITable table;
        private static IDomain domain;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(IDomain_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                dataBase = cremaHost.DataBases.Random();
                dataBase.Load(authentication);
                dataBase.Enter(authentication);
                dataBase.TypeContext.AddRandomItems(authentication);
                dataBase.TableContext.AddRandomItems(authentication);
                table = dataBase.TableContext.Tables.Random(item => item.TemplatedParent == null);
                table.Template.BeginEdit(authentication);
                domain = table.Template.Domain;
            });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                table.Template.CancelEdit(authentication);
                dataBase.Unload(authentication);
                cremaHost.Stop(authentication);
            });
            app.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Delete()
        {
            domain.Delete(authentication, false);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BeginUserEdit()
        {
            domain.BeginUserEdit(authentication, DomainLocationInfo.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EndUserEdit()
        {
            domain.EndUserEdit(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NewRow()
        {
            domain.NewRow(authentication, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetRow()
        {
            domain.SetRow(authentication, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveRow()
        {
            domain.RemoveRow(authentication, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetProperty()
        {
            domain.SetProperty(authentication, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetUserLocation()
        {
            domain.SetUserLocation(authentication, DomainLocationInfo.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Kick()
        {
            domain.Kick(authentication, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetOwner()
        {
            domain.SetOwner(authentication, null);
        }

        [TestMethod]
        public void ID()
        {
            Console.Write(domain.ID);
        }

        [TestMethod]
        public void DataBaseID()
        {
            Console.Write(domain.DataBaseID);
        }

        [TestMethod]
        public void Source()
        {
            Console.Write(domain.Source);
        }

        [TestMethod]
        public void Host()
        {
            Console.Write(domain.Host);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DomainInfo()
        {
            Console.Write(domain.DomainInfo);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DomainState()
        {
            Console.Write(domain.DomainState);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Users()
        {
            Console.Write(domain.Users);
        }

        [TestMethod]
        public void Dispatcher()
        {
            Console.Write(domain.Dispatcher);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserAdded()
        {
            domain.UserAdded += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserChanged()
        {
            domain.UserChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserRemoved()
        {
            domain.UserRemoved += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RowAdded()
        {
            domain.RowAdded += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RowRemoved()
        {
            domain.RowRemoved += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RowChanged()
        {
            domain.RowChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PropertyChanged()
        {
            domain.PropertyChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deleted()
        {
            domain.Deleted += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DomainStateChanged()
        {
            domain.DomainStateChanged += (s, e) => Assert.Inconclusive();
        }

#if SERVER
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetMetaData()
        {
            domain.GetMetaData(authentication);
        }
#endif

        [TestMethod]
        public void GetService()
        {
            Console.Write(domain.GetService(typeof(ICremaHost)));
        }
    }
}
