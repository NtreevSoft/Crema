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
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.Crema.Tools.Framework.Dialogs.ViewModels
{
    public class DataBaseListViewModel : ModalDialogBase
    {
        private readonly string address;
        private DataBaseInfo? selectedItem;
        private string selectedValue;
        private ObservableCollection<DataBaseInfo> itemsSource;

        public DataBaseListViewModel(string address)
        {
            this.DisplayName = "데이터 베이스 선택";
            this.address = address;
        }

        public void OK()
        {
            this.TryClose(true);
        }

        public ObservableCollection<DataBaseInfo> ItemsSource
        {
            get { return this.itemsSource; }
            private set
            {
                this.itemsSource = value;
                this.NotifyOfPropertyChange(() => this.ItemsSource);
                this.NotifyOfPropertyChange(() => this.SelectedItem);
            }
        }

        public DataBaseInfo? SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                this.selectedItem = value;
                if (this.selectedItem.HasValue == true)
                {
                    this.selectedValue = this.selectedItem.Value.Name;
                }
                else
                {
                    this.selectedValue = null;
                }
                this.NotifyOfPropertyChange(() => this.SelectedItem);
                this.NotifyOfPropertyChange(() => this.SelectedValue);
                this.NotifyOfPropertyChange(() => this.CanOK);
            }
        }

        public string SelectedValue
        {
            get { return this.selectedValue ?? string.Empty; }
            set { this.selectedValue = value; }
        }

        public bool CanOK
        {
            get { return this.selectedItem != null; }
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await Application.Current.Dispatcher.InvokeAsync(() => this.RefreshLabelList());
        }

        private async void RefreshLabelList()
        {
            try
            {
                var service = DescriptorServiceFactory.CreateServiceClient(this.address);
                var labels = await service.GetDataBaseInfosAsync();
                service.Close();
                var selectedValue = this.selectedValue;
                this.ItemsSource = new ObservableCollection<DataBaseInfo>(labels);
                foreach (var item in labels)
                {
                    if (item.Name == selectedValue)
                    {
                        this.SelectedItem = item;
                    }
                }
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e.Message);
                this.TryClose();
            }
            finally
            {
                //this.EndProgress();
            }
        }
    }
}
