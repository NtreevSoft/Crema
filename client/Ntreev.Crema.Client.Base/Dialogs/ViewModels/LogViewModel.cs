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
using Ntreev.Crema.ServiceModel;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Base.Properties;
using Ntreev.ModernUI.Framework;
using System.Collections;
using System.Windows.Input;

namespace Ntreev.Crema.Client.Base.Dialogs.ViewModels
{
    public class LogViewModel : ModalDialogAppBase, ISelector
    {
        private readonly Authentication authentication;
        private readonly IDataBase dataBase;
        private LogInfoViewModel[] itemsSource;
        private LogInfoViewModel selectedItem;
        //private readonly ICommand previewCommand;

        private LogViewModel(Authentication authentication, IDataBase dataBase)
        {
            this.authentication = authentication;
            this.dataBase = dataBase;
            this.DisplayName = Resources.Title_ViewLog;
            //this.previewCommand = new DelegateCommand((p) => this.Preview(), (p) => this.CanPreview);
        }

        public static async Task<LogViewModel> ShowDialogAsync(Authentication authentication, IDataBaseDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is IDataBase dataBase)
            {
                try
                {
                    var dialog = await dataBase.Dispatcher.InvokeAsync(() => new LogViewModel(authentication, dataBase));
                    if (dialog.ShowDialog() == true)
                        return dialog;
                    return null;
                }
                catch (Exception e)
                {
                    CremaLog.Error(e);
                    return null;
                }
            }
            throw new NotImplementedException();
        }

        public void Close()
        {
            this.TryClose(true);
        }

        //public void Preview()
        //{
        //    this.selectedItem.Preview();
        //}

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

        public IEnumerable<LogInfoViewModel> ItemsSource
        {
            get { return this.itemsSource; }
        }

        //public ICommand PreviewCommand
        //{
        //    get { return this.previewCommand; }
        //}

        protected async override void OnInitialize()
        {
            base.OnInitialize();

            try
            {
                this.BeginProgress(Resources.Message_ReceivingInfo);
                this.itemsSource = await this.dataBase.Dispatcher.InvokeAsync(() =>
                {
                    var logs = this.dataBase.GetLog(this.authentication);
                    var logList = new List<LogInfoViewModel>(logs.Length);
                    foreach (var item in logs)
                    {
                        logList.Add(new LogInfoViewModel(this.authentication, this.dataBase, item));
                    }
                    return logList.ToArray();
                });
                this.selectedItem = null;
                this.EndProgress();
                this.NotifyOfPropertyChange(nameof(this.SelectedItem));
                this.NotifyOfPropertyChange(nameof(this.ItemsSource));
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
                this.TryClose();
            }
        }

        #region ISelector

        object ISelector.SelectedItem
        {
            get => this.SelectedItem;
            set
            {
                if (value is LogInfoViewModel viewModel)
                    this.SelectedItem = viewModel;
                else
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
