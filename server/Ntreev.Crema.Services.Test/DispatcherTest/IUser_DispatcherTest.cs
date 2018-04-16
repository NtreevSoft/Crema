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
    public class IUser_DispatcherTest
    {
        private static CremaBootstrapper app;
        private static ICremaHost cremaHost;
        private static Authentication authentication;
        private static IUserContext userContext;
        private static IUser user;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            app = new CremaBootstrapper();
            app.Initialize(context, nameof(IUser_DispatcherTest));
            cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            cremaHost.Dispatcher.Invoke(() =>
            {
                authentication = cremaHost.Start();
                userContext = cremaHost.GetService(typeof(IUserContext)) as IUserContext;
            });
            user = userContext.Dispatcher.Invoke(() => userContext.Users.Random());
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
        public void Move()
        {
            user.Move(authentication, PathUtility.Separator);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Delete()
        {
            user.Delete(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ChangeUserInfo()
        {
            user.ChangeUserInfo(authentication, null, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SendMessage()
        {
            user.SendMessage(authentication, RandomUtility.NextString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Kick()
        {
            user.Kick(authentication, RandomUtility.NextString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Ban()
        {
            user.Ban(authentication, RandomUtility.NextString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Unban()
        {
            user.Unban(authentication);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ID()
        {
            Console.Write(user.ID);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserName()
        {
            Console.Write(user.UserName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Path()
        {
            Console.Write(user.Path);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Authority()
        {
            Console.Write(user.Authority);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Category()
        {
            Console.Write(user.Category);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserInfo()
        {
            Console.Write(user.UserInfo);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserState()
        {
            Console.Write(user.UserState);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BanInfo()
        {
            Console.Write(user.BanInfo);
        }

        [TestMethod]
        public void ExtendedProperties()
        {
            Console.Write(user.ExtendedProperties);
        }

        [TestMethod]
        public void Dispatcher()
        {
            Console.Write(user.Dispatcher);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Renamed()
        {
            user.Renamed += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Moved()
        {
            user.Moved += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deleted()
        {
            user.Deleted += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserInfoChanged()
        {
            user.UserInfoChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserStateChanged()
        {
            user.UserStateChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserBanInfoChanged()
        {
            user.UserBanInfoChanged += (s, e) => Assert.Inconclusive();
        }

        [TestMethod]
        public void GetService()
        {
            Console.Write(user.GetService(typeof(ICremaHost)));
        }
    }
}
