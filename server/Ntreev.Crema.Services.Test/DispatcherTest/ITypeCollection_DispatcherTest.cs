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
using System.Collections;
using System.Collections.Generic;

namespace Ntreev.Crema.Services.Test.DispatcherTest
{
    [TestClass]
    public class ITypeCollection_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IDataBase dataBase;
        private static ITypeCollection types;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(ITypeCollection_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                dataBase = cremaHost.DataBases.Random();
                dataBase.Load(authentication);
                dataBase.Enter(authentication);
                dataBase.TypeContext.AddRandomItems(authentication);
                types = dataBase.TypeContext.Types;
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
        public void Contains()
        {
            types.Contains(string.Empty);
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Indexer()
        {
            Console.Write(types[string.Empty]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TypesStateChanged()
        {
            types.TypesStateChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TypesChanged()
        {
            types.TypesChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TypesCreated()
        {
            types.TypesCreated += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TypesMoved()
        {
            types.TypesMoved += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TypesRenamed()
        {
            types.TypesRenamed += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TypesDeleted()
        {
            types.TypesDeleted += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Count()
        {
            Console.Write(types.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetEnumerator()
        {
            foreach (var item in types as IEnumerable)
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetEnumeratorGeneric()
        {
            foreach (var item in types as IEnumerable<IType>)
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CollectionChanged()
        {
            types.CollectionChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void GetService()
        {
            Console.Write(types.GetService(typeof(ICremaHost)));
        }
    }
}