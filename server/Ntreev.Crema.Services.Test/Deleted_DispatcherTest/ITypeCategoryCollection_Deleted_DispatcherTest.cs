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

namespace Ntreev.Crema.Services.Test.Deleted_DispatcherTest
{
    [TestClass]
    public class ITypeCategoryCollection_Deleted_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IDataBase dataBase;
        private static ITypeCategoryCollection categories;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(ITypeCategoryCollection_Deleted_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                dataBase = cremaHost.DataBases.Random();
                dataBase.Load(authentication);
                dataBase.Enter(authentication);
                dataBase.TypeContext.AddRandomItems(authentication);
                categories = dataBase.TypeContext.Categories;
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
        public void Contains()
        {
            categories.Contains(PathUtility.Separator);
        }

        [TestMethod]
        public void Root()
        {
            Console.Write(categories.Root);
        }

        [TestMethod]
        public void Indexer()
        {
            Console.Write(categories[PathUtility.Separator]);
        }

        [TestMethod]
        public void CategoriesCreated()
        {
            categories.CategoriesCreated += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void CategoriesRenamed()
        {
            categories.CategoriesRenamed += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void CategoriesMoved()
        {
            categories.CategoriesMoved += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void CategoriesDeleted()
        {
            categories.CategoriesDeleted += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void Count()
        {
            Console.Write(categories.Count);
        }

        [TestMethod]
        public void GetEnumerator()
        {
            foreach (var item in categories as IEnumerable)
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        public void GetEnumeratorGeneric()
        {
            foreach (var item in categories as IEnumerable<ITypeCategory>)
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        public void CollectionChanged()
        {
            categories.CollectionChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetService()
        {
            Console.Write(categories.GetService(typeof(ICremaHost)));
        }
    }
}