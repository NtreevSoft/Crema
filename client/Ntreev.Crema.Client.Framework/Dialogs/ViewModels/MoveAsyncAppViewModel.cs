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

using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels
{
    public abstract class MoveAsyncAppViewModel : MoveAsyncViewModel, IPartImportsSatisfiedNotification
    {
        [Import]
        private ICremaAppHost cremaAppHost = null;

        protected MoveAsyncAppViewModel(string currentPath, string[] targetPaths)
            : base(currentPath, targetPaths)
        {

        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close == true)
            {
                if (this.cremaAppHost != null)
                {
                    this.cremaAppHost.Closed -= CremaAppHost_Closed;
                    this.cremaAppHost.Unloaded -= CremaAppHost_Unloaded;
                }
                this.cremaAppHost = null;
            }
        }

        protected virtual void OnCancel()
        {
            this.TryClose();
        }

        protected virtual ModalDialogAppScope Scope
        {
            get { return ModalDialogAppScope.Loaded; }
        }

        private void CremaAppHost_Unloaded(object sender, EventArgs e)
        {
            if (this.cremaAppHost != null && this.Scope == ModalDialogAppScope.Loaded)
            {
                this.Dispatcher.InvokeAsync(this.OnCancel);
            }
        }

        private void CremaAppHost_Closed(object sender, EventArgs e)
        {
            if (this.cremaAppHost != null)
            {
                this.Dispatcher.InvokeAsync(this.OnCancel);
            }
        }

        #region IPartImportsSatisfiedNotification

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            if (this.cremaAppHost != null)
            {
                this.cremaAppHost.Closed += CremaAppHost_Closed;
                this.cremaAppHost.Unloaded += CremaAppHost_Unloaded;
            }
        }

        #endregion
    }
}
