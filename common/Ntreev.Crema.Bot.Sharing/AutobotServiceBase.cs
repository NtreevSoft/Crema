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

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Bot
{
    public abstract class AutobotServiceBase
    {
        private const string masterBotID = "Smith";
        private ICremaHost cremaHost;
        private object lockobj = new object();
        private IEnumerable<ITaskProvider> taskProviders;
        private List<AutobotBase> botList = new List<AutobotBase>();
        private List<Task> taskList = new List<Task>();
        private bool isPlaying;
        //private bool isStopping;

        protected AutobotServiceBase(ICremaHost cremaHost, IEnumerable<ITaskProvider> taskProviders)
        {
            this.cremaHost = cremaHost;
            this.cremaHost.Closing += CremaHost_Closing;
            this.taskProviders = taskProviders;
        }

        public void AddAutobot(Authentication authentication, string autobotID)
        {
            this.AddAutobot(authentication, autobotID, RandomUtility.NextEnum<Authority>());
        }

        public void AddAutobot(Authentication authentication, string autobotID, Authority authority)
        {
            lock (this.lockobj)
            {
                var userContext = this.cremaHost.GetService(typeof(IUserContext)) as IUserContext;

                if (this.botList.Any(item => item.AutobotID == autobotID) == true)
                    return;

                userContext.Dispatcher.Invoke(() =>
                {
                    if (userContext.Categories.Contains("/autobots/") == false)
                    {
                        userContext.Root.AddNewCategory(authentication, "autobots");
                    }

                    if (userContext.Users.Contains(autobotID) == false)
                    {
                        userContext.Categories["/autobots/"].AddNewUser(authentication, autobotID, StringUtility.ToSecureString("1111"), autobotID, authority);
                    }
                });

                var autobot = this.CreateInstance(autobotID);
                autobot.AllowException = this.AllowException;
                this.botList.Add(autobot);
                this.taskList.Add(autobot.ExecuteAsync(this.taskProviders));
            }
        }

        public void AddAutobot(Authentication authentication)
        {
            this.AddAutobot(authentication, $"Autobot{RandomUtility.Next(1000)}");
        }

        public void Start(Authentication authentication)
        {
            lock (this.lockobj)
            {
                if (this.isPlaying == true)
                    return;
                this.AddAutobot(authentication, masterBotID, Authority.Admin);
                this.isPlaying = true;
            }
        }

        public void Stop()
        {
            if (this.isPlaying == false)
                return;
            foreach (var item in this.botList.ToArray())
            {
                item.Cancel();
            }
            Task.WaitAll(this.taskList.ToArray());
            foreach (var item in this.botList.ToArray())
            {
                (item as IDisposable).Dispose();
            }
            this.taskList.Clear();
            this.isPlaying = false;
        }

        public bool IsPlaying
        {
            get { return this.isPlaying; }
        }

        public ITaskProvider[] TaskProviders
        {
            get { return this.taskProviders.ToArray(); }
        }

        public bool AllowException
        {
            get; set;
        }

        protected abstract AutobotBase CreateInstance(string autobotID);

        private void CremaHost_Closing(object sender, EventArgs e)
        {
            foreach (var item in this.botList)
            {
                item.Cancel();
            }
            Task.WaitAll(this.taskList.ToArray());
        }
    }
}
