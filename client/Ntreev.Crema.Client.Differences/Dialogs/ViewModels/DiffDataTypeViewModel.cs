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

using Ntreev.Crema.Client.Differences.Properties;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Diff;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Ntreev.Crema.Client.Differences.Dialogs.ViewModels
{
    public class DiffDataTypeViewModel : ModalDialogBase
    {
        private DiffDataType diffType;
        private string header1;
        private string header2;

        [Import]
        private BrowserService browserService = null;
        [Import]
        private DifferencesServiceViewModel service = null;
        [Import]
        private IShell shell = null;

        internal DiffDataTypeViewModel(Task<DiffDataType> action)
        {
            this.Initialize(action);
        }

        public DiffDataTypeViewModel(DiffDataType diffType)
        {
            this.Source = diffType;
        }

        public void Merge()
        {
            this.TryClose();
            this.Dispatcher.InvokeAsync(() =>
            {
                this.browserService.Add(this.diffType.DiffSet);
                this.shell.SelectedService = this.service;
            });
        }

        public DiffDataType Source
        {
            get { return this.diffType; }
            private set
            {
                this.diffType = value;
                this.Header1 = this.diffType.Header1;
                this.Header2 = this.diffType.Header2;
                this.NotifyOfPropertyChange(nameof(this.Source));
            }
        }

        public string Header1
        {
            get { return this.header1 ?? string.Empty; }
            set
            {
                this.header1 = value;
                this.NotifyOfPropertyChange(nameof(this.Header1));
            }
        }

        public string Header2
        {
            get { return this.header2 ?? string.Empty; }
            set
            {
                this.header2 = value;
                this.NotifyOfPropertyChange(nameof(this.Header2));
            }
        }

        private async void Initialize(Task<DiffDataType> action)
        {
            try
            {
                this.BeginProgress(Resources.Message_ReceivingInfo);
                this.Source = await action;
            }
            catch (Exception e)
            {
                await this.Dispatcher.InvokeAsync(() =>
                {
                    AppMessageBox.ShowError(e);
                    this.TryClose();
                }, DispatcherPriority.ApplicationIdle);
            }
            finally
            {
                this.EndProgress();
            }
        }
    }
}

