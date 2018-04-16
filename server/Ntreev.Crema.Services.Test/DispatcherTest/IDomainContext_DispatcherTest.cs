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
    public class IDomainContext_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IDomainContext domainContext;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(IDomainContext_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                domainContext = cremaHost.GetService(typeof(IDomainContext)) as IDomainContext;
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
        public void Categories()
        {
            Console.Write(domainContext.Categories);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Domains()
        {
            Console.Write(domainContext.Domains);
        }

        [TestMethod]
        public void Dispatcher()
        {
            Console.Write(domainContext.Dispatcher);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DomainCreated()
        {
            domainContext.Domains.DomainCreated += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DomainDeleted()
        {
            domainContext.Domains.DomainDeleted += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DomainStateChanged()
        {
            domainContext.Domains.DomainStateChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DomainUserAdded()
        {
            domainContext.Domains.DomainUserAdded += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DomainUserChanged()
        {
            domainContext.Domains.DomainUserChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DomainUserRemoved()
        {
            domainContext.Domains.DomainUserRemoved += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DomainRowAdded()
        {
            domainContext.Domains.DomainRowAdded += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DomainRowChanged()
        {
            domainContext.Domains.DomainRowChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DomainRowRemoved()
        {
            domainContext.Domains.DomainRowRemoved += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DomainPropertyChanged()
        {
            domainContext.Domains.DomainPropertyChanged += (s, e) => Assert.Inconclusive();
        }

#if SERVER
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetMetaData()
        {
            domainContext.GetMetaData(authentication);
        }
#endif

        [TestMethod]
        public void GetService()
        {
            Console.Write(domainContext.GetService(typeof(ICremaHost)));
        }
    }
}
