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
using Ntreev.Crema.Services.Random;

namespace Ntreev.Crema.Services.Test
{
    [TestClass]
    public class CremaHost_AssemblyInitializer
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            AppUtility.ProductName = "CremaTest";
            AppUtility.ProductVersion = "1.0.0.0";
            AppUtility.UserAppDataPath = Path.Combine(context.TestRunResultsDirectory, "AppData");

            TypeContextExtensions.MinTypeCount = 1;
            TypeContextExtensions.MaxTypeCount = 3;
            TypeContextExtensions.MinTypeCategoryCount = 1;
            TypeContextExtensions.MaxTypeCategoryCount = 3;

            TableContextExtensions.MinRowCount = 1;
            TableContextExtensions.MaxRowCount = 10;
            TableContextExtensions.MinTableCount = 1;
            TableContextExtensions.MaxTableCount = 3;
            TableContextExtensions.MinTableCategoryCount = 1;
            TableContextExtensions.MaxTableCategoryCount = 3;

            TableTemplateExtensions.MinColumnCount = 2;
            TableTemplateExtensions.MaxColumnCount = 5;
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(CremaHost_AssemblyInitializer));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            authentication = cremaHost.Dispatcher.Invoke(() => cremaHost.Start());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            cremaHost.Dispatcher.Invoke(() => cremaHost.Stop(authentication));
            app.Dispose();
        }

        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DataBaseTest()
        {
            var dataBases = cremaHost.DataBases;
        }
    }
}
