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
using Ntreev.Library.Random;
using System.IO;
using Ntreev.Library.IO;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Services.Test.Deleted_DispatcherTest
{
    [TestClass]
    public class IDataBase_Deleted_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IDataBase dataBase;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(IDataBase_Deleted_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                dataBase = cremaHost.DataBases.AddNewDataBase(authentication, RandomUtility.NextIdentifier(), RandomUtility.NextString());
                dataBase.Delete(authentication);
            });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            cremaHost.Dispatcher.Invoke(() => cremaHost.Stop(authentication));
            app.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Load()
        {
            dataBase.Load(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Unload()
        {
            dataBase.Unload(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Enter()
        {
            dataBase.Enter(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Leave()
        {
            dataBase.Leave(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Rename()
        {
            dataBase.Rename(authentication, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Delete()
        {
            dataBase.Delete(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Copy()
        {
            dataBase.Copy(authentication, null, null, false);
        }

        [TestMethod]
        public void TypeContext()
        {
            Assert.IsNull(dataBase.TypeContext);
        }

        [TestMethod]
        public void TableContext()
        {
            Assert.IsNull(dataBase.TableContext);
        }

        [TestMethod]
        public void Name()
        {
            Console.WriteLine(dataBase.Name);
        }

        [TestMethod]
        public void IsLoaded()
        {
            Assert.IsFalse(dataBase.IsLoaded);
        }

        [TestMethod]
        public void ID()
        {
            Console.WriteLine(dataBase.ID);
        }

        [TestMethod]
        public void DataBaseInfo()
        {
            Console.WriteLine(dataBase.DataBaseInfo);
        }

        [TestMethod]
        public void DataBaseState()
        {
            Assert.AreEqual(ServiceModel.DataBaseState.None, dataBase.DataBaseState);
        }

        [TestMethod]
        public void Dispatcher()
        {
            Assert.IsNull(dataBase.Dispatcher);
        }

        [TestMethod]
        public void Renamed()
        {
            dataBase.Renamed += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void Deleted()
        {
            dataBase.Deleted += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void Loaded()
        {
            dataBase.Loaded += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void Unloaded()
        {
            dataBase.Unloaded += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void Changed()
        {
            dataBase.DataBaseInfoChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void StateChanged()
        {
            dataBase.DataBaseStateChanged += (s, e) => Assert.Inconclusive();
        }
    }
}
