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
    public class ITableColumn_Deleted_DispatcherTest
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
            app.Initialize(context, nameof(ITableColumn_Deleted_DispatcherTest));
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
                column = template.Random();
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void Delete()
        {
            column.Delete(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetIndex()
        {
            column.SetIndex(authentication, RandomUtility.Next(int.MaxValue));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetIsKey()
        {
            column.SetIsKey(authentication, RandomUtility.NextBoolean());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetIsUnique()
        {
            column.SetIsUnique(authentication, RandomUtility.NextBoolean());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetName()
        {
            column.SetName(authentication, RandomUtility.NextIdentifier());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetDataType()
        {
            column.SetDataType(authentication, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetDefaultValue()
        {
            column.SetDefaultValue(authentication, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetComment()
        {
            column.SetComment(authentication, RandomUtility.NextString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetAutoIncrement()
        {
            column.SetAutoIncrement(authentication, RandomUtility.NextBoolean());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetTags()
        {
            column.SetTags(authentication, RandomUtility.NextTags());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetIsReadOnly()
        {
            column.SetIsReadOnly(authentication, RandomUtility.NextBoolean());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetAllowNull()
        {
            column.SetAllowNull(authentication, RandomUtility.NextBoolean());
        }

        [TestMethod]
        public void Index()
        {
            Console.Write(column.Index);
        }

        [TestMethod]
        public void IsKey()
        {
            Console.Write(column.IsKey);
        }

        [TestMethod]
        public void IsUnique()
        {
            Console.Write(column.IsUnique);
        }

        [TestMethod]
        public void Name()
        {
            Console.Write(column.Name);
        }

        [TestMethod]
        public void DataType()
        {
            Console.Write(column.DataType);
        }

        [TestMethod]
        public void DefaultValue()
        {
            Console.Write(column.DefaultValue);
        }

        [TestMethod]
        public void Comment()
        {
            Console.Write(column.Comment);
        }

        [TestMethod]
        public void AutoIncrement()
        {
            Console.Write(column.AutoIncrement);
        }

        [TestMethod]
        public void Tags()
        {
            Console.Write(column.Tags);
        }

        [TestMethod]
        public void IsReadOnly()
        {
            Console.Write(column.IsReadOnly);
        }

        [TestMethod]
        public void AllowNull()
        {
            Console.Write(column.AllowNull);
        }

        [TestMethod]
        public void Template()
        {
            Console.Write(column.Template);
        }
    }
}
