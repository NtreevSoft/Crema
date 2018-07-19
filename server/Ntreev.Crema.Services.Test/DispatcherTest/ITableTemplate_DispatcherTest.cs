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
    public class ITableTemplate_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IDataBase dataBase;
        private static ITableTemplate template;
        private static ITableColumn column;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(ITableTemplate_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                dataBase = cremaHost.DataBases.Random();
                dataBase.Load(authentication);
                dataBase.Enter(authentication);
                dataBase.Initialize(authentication);
                template = dataBase.TableContext.Tables.Random(item => item.TemplatedParent == null).Template;
                template.BeginEdit(authentication);
                column = template.AddNew(authentication);
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
        public void BeginEdit()
        {
            template.BeginEdit(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EndEdit()
        {
            template.EndEdit(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CancelEdit()
        {
            template.CancelEdit(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetTableName()
        {
            template.SetTableName(authentication, RandomUtility.NextIdentifier());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetTags()
        {
            template.SetTags(authentication, RandomUtility.NextTags());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetComment()
        {
            template.SetComment(authentication, RandomUtility.NextString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddNew()
        {
            template.AddNew(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EndNew()
        {
            template.EndNew(authentication, column);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Contains()
        {
            template.Contains(RandomUtility.NextIdentifier());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Domain()
        {
            Console.Write(template.Domain);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Table()
        {
            Console.Write(template.Target);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Count()
        {
            Console.Write(template.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Indexer()
        {
            Console.Write(template[RandomUtility.NextIdentifier()]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SelectableTypes()
        {
            Console.Write(template.SelectableTypes);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PrimaryKey()
        {
            foreach (var item in template.PrimaryKey)
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TableName()
        {
            Console.Write(template.TableName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Tags()
        {
            Console.Write(template.Tags);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Comment()
        {
            Console.Write(template.Comment);
        }

        [TestMethod]
        public void Dispatcher()
        {
            Console.Write(template.Dispatcher);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EditBegun()
        {
            template.EditBegun += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EditEnded()
        {
            template.EditEnded += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EditCanceled()
        {
            template.EditCanceled += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetEnumerator()
        {
            foreach (var item in template as IEnumerable)
            {
                Console.Write(item);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetEnumeratorGeneric()
        {
            foreach (var item in template as IEnumerable<ITableColumn>)
            {
                Console.Write(item);
            }
        }
    }
}
