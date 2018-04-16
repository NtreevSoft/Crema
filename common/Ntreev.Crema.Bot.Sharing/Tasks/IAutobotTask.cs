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
    [Export(typeof(IAutobotTask))]
    [TaskClass]
    class IAutobotTask : ITaskProvider
    {
        private readonly CremaDispatcher dispatcher;

        private Dictionary<Authentication, AutobotBase> autobots = new Dictionary<Authentication, AutobotBase>();

        public IAutobotTask()
        {
            this.dispatcher = new CremaDispatcher(this, System.Windows.Threading.Dispatcher.CurrentDispatcher);
        }

        public void InvokeTask(TaskContext context)
        {
            if (context.Target is AutobotBase autobot)
            {
                if (autobot.IsOnline == true)
                {
                    if (RandomUtility.Within(75) == true)
                    {
                        var dataBase = autobot.CremaHost.Dispatcher.Invoke(() => autobot.CremaHost.DataBases.Random());
                        context.Push(dataBase);
                    }
                    else if (RandomUtility.Within(75) == true)
                    {
                        var userContext = autobot.CremaHost.GetService(typeof(IUserContext)) as IUserContext;
                        if (RandomUtility.Within(75) == true)
                        {
                            var user = userContext.Dispatcher.Invoke(() => userContext.Users.Random());
                            context.Push(user);
                        }
                        else
                        {
                            var category = userContext.Dispatcher.Invoke(() => userContext.Categories.Random());
                            context.Push(category);
                        }
                    }
                    else if (RandomUtility.Within(10) == true)
                    {
                        var dataBase = autobot.CremaHost.Dispatcher.Invoke(() => autobot.CremaHost.DataBases);
                        context.Push(dataBase);
                    }
                }
            }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.dispatcher; }
        }

        public Type TargetType
        {
            get { return typeof(AutobotBase); }
        }

        public bool IsEnabled
        {
            get { return true; }
        }

        [TaskMethod]
        public void Login(AutobotBase autobot, TaskContext context)
        {
            if (autobot.IsOnline == false)
            {
                this.autobots[autobot.Login()] = autobot;
            }
        }

        [TaskMethod(Weight = 10)]
        public void Logout(AutobotBase autobot, TaskContext context)
        {
            if (autobot.IsOnline == true && context.IsCompleted(autobot) == true)
            {
                var authentication = context.Authentication;
                autobot.Logout();
                this.autobots.Remove(authentication);
            }
        }

        [TaskMethod]
        public void Wait(AutobotBase autobot, TaskContext context)
        {

        }

        [TaskMethod(Weight = 50)]
        public void AddNewAutoBot(Autobot autobot, TaskContext context)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (autobot.IsOnline == true)
                {
                    autobot.Service.AddAutobot(context.Authentication);
                }
            });
        }
    }
}
