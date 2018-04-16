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

namespace Ntreev.Crema.Services.Test.DispatcherTest
{
    [TestClass]
    public class IDataBase_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IDataBase dataBase;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(IDataBase_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                dataBase = cremaHost.DataBases.Random();
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void TypeContext()
        {
            Console.WriteLine(dataBase.TypeContext);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TableContext()
        {
            Console.WriteLine(dataBase.TableContext);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Name()
        {
            Console.WriteLine(dataBase.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IsLoaded()
        {
            Console.WriteLine(dataBase.IsLoaded);
        }

        [TestMethod]
        public void ID()
        {
            Console.WriteLine(dataBase.ID);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DataBaseInfo()
        {
            Console.WriteLine(dataBase.DataBaseInfo);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DataBaseState()
        {
            Console.WriteLine(dataBase.DataBaseState);
        }

        [TestMethod]
        public void Dispatcher()
        {
            Console.WriteLine(dataBase.Dispatcher);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Renamed()
        {
            dataBase.Renamed += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deleted()
        {
            dataBase.Deleted += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Loaded()
        {
            dataBase.Loaded += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Unloaded()
        {
            dataBase.Unloaded += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Changed()
        {
            dataBase.DataBaseInfoChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void StateChanged()
        {
            dataBase.DataBaseStateChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void GetService()
        {
            Console.Write(dataBase.GetService(typeof(ICremaHost)));
        }
    }
}
