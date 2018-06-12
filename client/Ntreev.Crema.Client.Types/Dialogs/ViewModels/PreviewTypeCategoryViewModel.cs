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

using Ntreev.Crema.Client.Types.Properties;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.Data;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Collections.ObjectModel;
using Ntreev.Library.IO;
using System.Collections;

namespace Ntreev.Crema.Client.Types.Dialogs.ViewModels
{
    class PreviewTypeCategoryViewModel : ModalDialogBase
    {
        private readonly Authentication authentication;
        private readonly ITypeCategory category;
        private readonly string revision;

        private CremaDataSet source;
        private ObservableCollection<TreeViewItemViewModel> itemsSource = new ObservableCollection<TreeViewItemViewModel>();
        private PreviewDocumentViewModel documents = new PreviewDocumentViewModel();
        private object selectedItem;

        public PreviewTypeCategoryViewModel(Authentication authentication, ITypeCategory category, string revision)
        {
            this.authentication = authentication;
            this.category = category;
            this.revision = revision;
            this.Initialize();
        }

        public CremaDataSet Source
        {
            get { return this.source; }
            private set
            {
                var builder = new PreviewTreeViewItemViewModelBuilder(this.documents.ViewType);
                var itemPaths = value.Types.Select(item => item.CategoryPath + item.Name).ToArray();
                var items = builder.Create(itemPaths, false);

                foreach (var item in value.Types)
                {
                    items[item.CategoryPath + item.Name].Target = item;
                }

                this.itemsSource.Clear();
                this.itemsSource.Add(items[PathUtility.Separator]);
                this.source = value;
                this.NotifyOfPropertyChange(nameof(this.Source));
            }
        }

        public IEnumerable ItemsSource
        {
            get { return this.itemsSource; }
        }

        public object SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                this.selectedItem = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedItem));
            }
        }

        public IDocumentService Documents
        {
            get { return this.documents; }
        }

        private async void Initialize()
        {
            try
            {
                this.DisplayName = await this.category.Dispatcher.InvokeAsync(() => $"{this.category.Path} - {revision}");
                this.BeginProgress(Resources.Message_ReceivingInfo);
                this.Source = await Task.Run(() => this.category.GetDataSet(this.authentication, this.revision));
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
                this.TryClose();
            }
            finally
            {
                this.EndProgress();
            }
        }
    }
}
