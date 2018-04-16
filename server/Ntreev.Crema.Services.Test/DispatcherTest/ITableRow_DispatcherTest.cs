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
    public class ITableRow_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IDataBase dataBase;
        private static ITableContent content;
        private static ITableRow row;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(ITableRow_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                dataBase = cremaHost.DataBases.Random();
                dataBase.Load(authentication);
                dataBase.Enter(authentication);
                dataBase.Initialize(authentication);
                content = dataBase.TableContext.Tables.Random(item => item.Parent == null).Content;
                content.BeginEdit(authentication);
                content.EnterEdit(authentication);
                if (content.Count == 0)
                    content.AddRandomRows(authentication);
                row = content.Random();
            });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                content.LeaveEdit(authentication);
                dataBase.Unload(authentication);
                cremaHost.Stop(authentication);
            });
            app.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Delete()
        {
            row.Delete(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetIsEnabled()
        {
            row.SetIsEnabled(authentication, false);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetTags()
        {
            row.SetTags(authentication, TagInfo.All);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetField()
        {
            row.SetField(authentication, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Indexer()
        {
            Console.Write(row[string.Empty]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Tags()
        {
            Console.Write(row.Tags);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IsEnabled()
        {
            Console.Write(row.IsEnabled);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Content()
        {
            Console.Write(row.Content);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RelationID()
        {
            Console.Write(row.RelationID);
        }
    }
}
