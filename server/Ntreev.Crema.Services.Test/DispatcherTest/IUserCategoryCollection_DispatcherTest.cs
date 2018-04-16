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
using System.Collections.Generic;
using System.Collections;

namespace Ntreev.Crema.Services.Test.DispatcherTest
{
    [TestClass]
    public class IUserCategoryCollection_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IUserCategoryCollection categories;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(IUserCategoryCollection_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                categories = cremaHost.GetService(typeof(IUserCategoryCollection)) as IUserCategoryCollection;
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
        public void Contains()
        {
            categories.Contains(PathUtility.Separator);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Root()
        {
            Console.Write(categories.Root);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Indexer()
        {
            Console.Write(categories[PathUtility.Separator]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CategoriesCreated()
        {
            categories.CategoriesCreated += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CategoriesRenamed()
        {
            categories.CategoriesRenamed += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CategoriesMoved()
        {
            categories.CategoriesMoved += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CategoriesDeleted()
        {
            categories.CategoriesDeleted += (s, e) => Assert.Inconclusive();
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Count()
        {
            Console.WriteLine(categories.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetEnumerator()
        {
            foreach (var item in categories as IEnumerable)
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetEnumeratorGeneric()
        {
            foreach (var item in categories as IEnumerable<IUserCategory>)
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CollectionChanged()
        {
            categories.CollectionChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void GetService()
        {
            Console.Write(categories.GetService(typeof(ICremaHost)));
        }
    }
}
