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

using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels
{
    public class BackgroundTaskViewModel : ModalDialogBase
    {
        [Import]
        private IStatusBarService statusBarService = null;
        private IBackgroundTask task;

        public BackgroundTaskViewModel(IBackgroundTask task)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            this.task.ProgressChanged += Task_ProgressChanged;
            this.DisplayName = this.task.DisplayName;
        }

        public void Hide()
        {
            this.TryClose();
        }

        public void Cancel()
        {
            this.task.Cancel();
            this.NotifyOfPropertyChange(nameof(this.CanCancel));
        }

        public bool CanCancel
        {
            get
            {
                return this.task.IsBusy && this.task.IsCancellationRequested == false;
            }
        }

        public bool CanHide
        {
            get { return true; }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            this.Dispatcher.InvokeAsync(() =>
            {
                this.statusBarService.AddTask(this.task);
                this.BeginProgress();
                this.Refresh();
            });
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (this.task != null && close == true)
            {
                this.task.ProgressChanged -= Task_ProgressChanged;
            }
        }

        private void Task_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ProgressMessage = e.Message;

            if (e.State == ProgressChangeState.Completed || e.State == ProgressChangeState.Failed)
            {
                this.EndProgress(e.Message);
                this.Refresh();
            }
        }
    }
}
