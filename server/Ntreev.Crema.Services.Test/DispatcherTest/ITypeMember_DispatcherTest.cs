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
    public class ITypeMember_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IDataBase dataBase;
        private static ITypeTemplate template;
        private static ITypeMember member;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(ITypeMember_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                dataBase = cremaHost.DataBases.Random();
                dataBase.Load(authentication);
                dataBase.Enter(authentication);
                dataBase.TypeContext.AddRandomItems(authentication);
                template = dataBase.TypeContext.Types.Random().Template;
                template.BeginEdit(authentication);
                member = template.Random();
            });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                template.CancelEdit(authentication);
                dataBase.Unload(authentication);
                cremaHost.Stop(authentication);
            });
            app.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Delete()
        {
            member.Delete(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetIndex()
        {
            member.SetIndex(authentication, RandomUtility.Next(int.MaxValue));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetName()
        {
            member.SetName(authentication, RandomUtility.NextIdentifier());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetValue()
        {
            member.SetValue(authentication, RandomUtility.NextLong(long.MaxValue));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetComment()
        {
            member.SetComment(authentication, RandomUtility.NextString());
        }
               
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Index()
        {
            Console.Write(member.Index);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Name()
        {
            Console.Write(member.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Value()
        {
            Console.Write(member.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Comment()
        {
            Console.Write(member.Comment);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Template()
        {
            Console.Write(member.Template);
        }
    }
}
