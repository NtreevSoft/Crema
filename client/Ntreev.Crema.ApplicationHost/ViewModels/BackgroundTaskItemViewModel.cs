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

using Ntreev.Crema.Client.Framework;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ApplicationHost.ViewModels
{
    class BackgroundTaskItemViewModel : PropertyChangedBase
    {
        private readonly IBackgroundTask task;
        private double progressValue;
        private string progressMessage;

        public BackgroundTaskItemViewModel(IBackgroundTask task)
        {
            this.task = task;
            this.task.ProgressChanged += Task_ProgressChanged;
            if (this.task.IsBusy == false)
                this.task.RunAsync();
        }

        private void Task_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressValue = e.Value * 100;
            this.progressMessage = e.Message;
            this.Dispatcher.InvokeAsync(() =>
            {
                this.NotifyOfPropertyChange(nameof(this.ProgressValue));
                this.NotifyOfPropertyChange(nameof(this.ProgressMessage));
                if (e.State == ProgressChangeState.Completed || e.State == ProgressChangeState.Failed)
                {
                    this.OnDispopsed(EventArgs.Empty);
                }
            });
        }

        public string DisplayName
        {
            get { return this.task.DisplayName; }
        }

        public double ProgressValue
        {
            get { return this.progressValue; }
        }

        public string ProgressMessage
        {
            get { return this.progressMessage; }
        }

        public event EventHandler Disposed;

        protected virtual void OnDispopsed(EventArgs e)
        {
            this.Disposed?.Invoke(this, e);
        }
    }
}
