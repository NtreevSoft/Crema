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
using Caliburn.Micro;
using Ntreev.Crema.Services;
using System.Collections.ObjectModel;
using Ntreev.Crema.Services.DataBaseService;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Types.Properties;
using Ntreev.ModernUI.Framework;
using System.Collections;
using System.Windows.Input;
using System.ComponentModel.Composition;

namespace Ntreev.Crema.Client.Types.Dialogs.ViewModels
{
    public class LogViewModel : ModalDialogBase
    {
        private readonly Authentication authentication;
        private readonly ITypeItem typeItem;
        private LogInfoViewModel[] itemsSource;
        private LogInfoViewModel selectedItem;
        private readonly ICommand previewCommand;
        [Import]
        private ICompositionService compositionService = null;

        public LogViewModel(Authentication authentication, ITypeItem typeItem)
        {
            this.authentication = authentication;
            this.typeItem = typeItem;
            this.DisplayName = Resources.Title_ViewLog;
            this.previewCommand = new DelegateCommand((p) => this.Preview(), (p) => this.CanPreview);
            this.Initialize();
        }

        public void Close()
        {
            this.TryClose(true);
        }

        public void Preview()
        {
            this.selectedItem.Preview();
        }

        public LogInfoViewModel SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                this.selectedItem = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedItem));
                this.NotifyOfPropertyChange(nameof(this.CanPreview));
            }
        }

        public bool CanPreview
        {
            get
            {
                if (this.IsProgressing == true)
                    return false;
                return this.selectedItem != null;
            }
        }

        public IEnumerable<LogInfoViewModel> Items
        {
            get { return this.itemsSource; }
        }

        public ICommand PreviewCommand
        {
            get { return this.previewCommand; }
        }

        private async void Initialize()
        {
            try
            {
                this.BeginProgress(Resources.Message_ReceivingInfo);
                var query = from item in await Task.Run(() => this.typeItem.GetLog(this.authentication))
                            select new LogInfoViewModel(this.authentication, this.typeItem, item);
                this.itemsSource = query.ToArray();
                foreach (var item in this.itemsSource)
                {
                    this.compositionService?.SatisfyImportsOnce(item);
                }
                this.selectedItem = null;
                this.EndProgress();
                this.NotifyOfPropertyChange(nameof(this.SelectedItem));
                this.NotifyOfPropertyChange(nameof(this.Items));
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
                this.TryClose();
            }
        }
    }
}