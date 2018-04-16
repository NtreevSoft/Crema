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
    public class IUserCategory_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IUserContext userContext;
        private static IUserCategory category;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(IUserCategory_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                userContext = cremaHost.GetService(typeof(IUserContext)) as IUserContext;
                
            });
            category = userContext.Dispatcher.Invoke(() => userContext.Categories.Random());
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
            category.Rename(authentication, RandomUtility.NextIdentifier());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Move()
        {
            category.Move(authentication, PathUtility.Separator);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Delete()
        {
            category.Delete(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddNewCategory()
        {
            category.AddNewCategory(authentication, RandomUtility.NextIdentifier());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddNewUser()
        {
            category.AddNewUser(authentication, null, null, null, ServiceModel.Authority.Admin);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Name()
        {
            Console.Write(category.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Path()
        {
            Console.Write(category.Path);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Parent()
        {
            Console.Write(category.Parent);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Categories()
        {
            Console.Write(category.Categories);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Users()
        {
            Console.Write(category.Users);
        }

        [TestMethod]
        public void ExtendedProperties()
        {
            Console.Write(category.ExtendedProperties);
        }

        [TestMethod]
        public void Dispatcher()
        {
            Console.Write(category.Dispatcher);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Renamed()
        {
            category.Renamed += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Moved()
        {
            category.Moved += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deleted()
        {
            category.Deleted += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void GetService()
        {
            Console.Write(category.GetService(typeof(ICremaHost)));
        }
    }
}
