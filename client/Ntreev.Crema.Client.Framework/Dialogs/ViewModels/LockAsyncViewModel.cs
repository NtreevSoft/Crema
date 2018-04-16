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
using System.Windows;
using Ntreev.Crema.Services;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Framework.Properties;
using Ntreev.ModernUI.Framework;
using System.Windows.Threading;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Client.Framework.Dialogs.Views;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels
{
    public class LockAsyncViewModel : LockViewModel
    {
        private bool isValid;

        public LockAsyncViewModel()
        {
            this.PropertyChanged += LockAsyncViewModel_PropertyChanged;
        }

        public async void Lock()
        {
            try
            {
                this.BeginProgress(Resources.Message_Locking);
                await this.LockAsync(this.Comment);
                this.EndProgress();
                this.TryClose(true);
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
            }
        }

        protected sealed override bool VerifyLock(string comment)
        {
            if (base.VerifyLock(comment) == false)
                return false;
            return this.isValid;
        }

        protected virtual void VerifyLock(string comment, Action<bool> isValid)
        {
            isValid(true);
        }

        protected virtual Task LockAsync(string comment)
        {
            return Task.Delay(1);
        }

        protected override void OnProgress()
        {
            base.OnProgress();
            this.NotifyOfPropertyChange(nameof(this.CanLock));
        }

        private void VerifyAction(bool isValid)
        {
            this.isValid = isValid;
            this.NotifyOfPropertyChange(nameof(this.CanLock));
        }

        private void LockAsyncViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.Comment))
            {
                this.VerifyLock(this.Comment, this.VerifyAction);
            }
        }
    }
}
