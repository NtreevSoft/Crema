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

using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Random;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Bot.Tasks
{
    [Export(typeof(ITaskProvider))]
    [Export(typeof(IUserTask))]
    [TaskClass(Weight = 10)]
    public class IUserTask : ITaskProvider
    {
        private readonly ICremaHost cremaHost;
        private IUserContext userContext;

        [ImportingConstructor]
        public IUserTask(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
            this.cremaHost.Opened += CremaHost_Opened;
            this.cremaHost.Closed += CremaHost_Closed;
        }

        public void InvokeTask(TaskContext context)
        {
            var user = context.Target as IUser;
            if (context.IsCompleted(user) == true)
            {
                context.Pop(user);
            }
        }

        public Type TargetType
        {
            get { return typeof(IUser); }
        }

        public bool IsEnabled
        {
            get { return true; }
        }

        [TaskMethod]
        public void Move(IUser user, TaskContext context)
        {
            user.Dispatcher.Invoke(() =>
            {
                var categories = user.GetService(typeof(IUserCategoryCollection)) as IUserCategoryCollection;
                var categoryPath = categories.Random().Path;
                if (Verify(categoryPath) == false)
                    return;
                user.Move(context.Authentication, categoryPath);
            });

            bool Verify(string categoryPath)
            {
                if (context.AllowException == true)
                    return true;
                if (user.Category.Path == categoryPath)
                    return false;
                return true;
            }
        }

        [TaskMethod(Weight = 1)]
        public void Delete(IUser user, TaskContext context)
        {
            user.Dispatcher.Invoke(() =>
            {
                //user.Delete(authentication);
            });
        }

        [TaskMethod]
        public void ChangeUserInfo(IUser user, TaskContext context)
        {
            user.Dispatcher.Invoke(() =>
            {
                //user.ChangeUserInfo(authentication);
            });
        }

        [TaskMethod]
        public void SendMessage(IUser user, TaskContext context)
        {
            user.Dispatcher.Invoke(() =>
            {
                var message = RandomUtility.NextString();
                if (Verify(message) == false)
                    return;
                user.SendMessage(context.Authentication, message);
            });

            bool Verify(string message)
            {
                if (context.AllowException == true)
                    return true;
                if (string.IsNullOrEmpty(message) == true)
                    return false;
                if (user.UserState != UserState.Online)
                    return false;
                return true;
            }
        }

        [TaskMethod]
        public void Kick(IUser user, TaskContext context)
        {
            user.Dispatcher.Invoke(() =>
            {
                var comment = RandomUtility.NextString();
                if (Verify(comment) == false)
                    return;
                user.Kick(context.Authentication, comment);
            });

            bool Verify(string comment)
            {
                if (context.AllowException == true)
                    return true;
                if (string.IsNullOrEmpty(comment) == true)
                    return false;
                if (user.Authority == Authority.Admin)
                    return false;
                if (user.UserState != UserState.Online)
                    return false;
                return true;
            }
        }

        [TaskMethod]
        public void Ban(IUser user, TaskContext context)
        {
            user.Dispatcher.Invoke(() =>
            {
                var comment = RandomUtility.NextString();
                if (Verify(comment) == false)
                    return;
                user.Ban(context.Authentication, comment);
            });

            bool Verify(string comment)
            {
                if (context.AllowException == true)
                    return true;
                if (string.IsNullOrEmpty(comment) == true)
                    return false;
                if (user.BanInfo.Path != string.Empty)
                    return false;
                if (user.Authority == Authority.Admin)
                    return false;
                return true;
            }
        }

        [TaskMethod]
        public void Unban(IUser user, TaskContext context)
        {
            user.Dispatcher.Invoke(() =>
            {
                if (Verify() == false)
                    return;
                user.Unban(context.Authentication);
            });

            bool Verify()
            {
                if (context.AllowException == true)
                    return true;
                if (user.BanInfo.Path != user.Path)
                    return false;
                return true;
            }
        }

        private void CremaHost_Opened(object sender, EventArgs e)
        {
            this.userContext = this.cremaHost.GetService(typeof(IUserContext)) as IUserContext;
        }

        private void CremaHost_Closed(object sender, EventArgs e)
        {
            this.userContext = null;
        }
    }
}
