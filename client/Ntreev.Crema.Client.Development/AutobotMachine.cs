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

using Ntreev.Crema.Services;
using Ntreev.Crema.Services.Random;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Development
{
    [Export(typeof(AutobotMachine))]
    class AutobotMachine
    {
        private readonly ICremaHost cremaHost;
        private readonly Authenticator authenticator;
        private CancellationTokenSource tokenSource;
        private Task task;

        [ImportingConstructor]
        public AutobotMachine(ICremaHost cremaHost, Authenticator authenticator)
        {
            this.cremaHost = cremaHost;
            this.authenticator = authenticator;
        }

        public void Start()
        {
            this.tokenSource = new CancellationTokenSource();
            this.task = new Task(() => this.TestAll(this.cremaHost, this.tokenSource.Token));
            this.task.Start();
            this.OnStarted(EventArgs.Empty);
        }

        public void Stop()
        {
            if (this.tokenSource == null)
                return;

            this.tokenSource.Cancel();
            this.tokenSource = null;

            this.task.Wait();
            this.task = null;

            this.OnStopped(EventArgs.Empty);
        }

        public bool IsProcessing
        {
            get { return this.task != null; }
        }

        public event EventHandler Started;

        public event EventHandler Stopped;

        protected virtual void OnStarted(EventArgs e)
        {
            if (this.Started != null)
            {
                this.Started(this, e);
            }
        }

        protected virtual void OnStopped(EventArgs e)
        {
            if (this.Stopped != null)
            {
                this.Stopped(this, e);
            }
        }

        private void TestAll(ICremaHost cremaHost, CancellationToken token)
        {
            var authentication = (Authentication)this.authenticator;

            while (true)
            {
                if (token.IsCancellationRequested == true)
                    break;

                try
                {
                    if (token.IsCancellationRequested == false)
                    {
                        cremaHost.RandomTask(authentication);
                    }
                }
                catch (PermissionDeniedException)
                {

                }
                catch (Exception e)
                {
                    if (e.InnerException == null || e.InnerException is PermissionDeniedException == false)
                        throw e;
                }

                try
                {
                    //cremaHost.TableContentEditTest(authentication);
                }
                catch (PermissionDeniedException)
                {

                }
            }
        }
    }
}
