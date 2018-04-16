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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System.Threading;
using System.Windows;

namespace Ntreev.Crema.Client.Framework
{
    public abstract class BackgroundTaskBase : PropertyChangedBase, IBackgroundTask
    {
        private string displayName;
        private bool isBusy;
        private bool isAlive = true;
        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();

        public BackgroundTaskBase()
        {
            Application.Current.Exit += Current_Exit;
        }

        public string DisplayName
        {
            get { return this.displayName ?? this.ToString(); }
            set
            {
                this.displayName = value;
                this.NotifyOfPropertyChange(nameof(this.DisplayName));
            }
        }

        public bool IsBusy => this.isBusy;

        public event ProgressChangedEventHandler ProgressChanged;

        public void Cancel()
        {
            this.cancellation.Cancel();
            this.NotifyOfPropertyChange(nameof(this.CanCancel));
        }

        void IBackgroundTask.RunAsync()
        {
            this.RunAsync();
        }

        public Task RunAsync()
        {
            var progress = new Progress();
            progress.Changed += Progress_Changed;
            return Task.Run(() =>
            {
                try
                {
                    this.isBusy = true;
                    this.OnRun(progress, this.cancellation.Token);
                    progress.Complete();
                }
                catch (Exception e)
                {
                    progress.Fail(e.Message);
                }
                finally
                {
                    this.isBusy = false;
                }
            });
        }

        public bool CanCancel
        {
            get { return this.cancellation.IsCancellationRequested == false; }
        }

        public bool IsCancellationRequested => this.cancellation.IsCancellationRequested;

        protected abstract void OnRun(IProgress progress, CancellationToken cancellation);

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            this.ProgressChanged?.Invoke(this, e);
        }

        private void Progress_Changed(object sender, ProgressChangedEventArgs e)
        {
            if (this.isAlive == true)
            {
                this.Dispatcher.InvokeAsync(() => this.OnProgressChanged(e));
            }
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            this.isAlive = false;
        }
    }
}
