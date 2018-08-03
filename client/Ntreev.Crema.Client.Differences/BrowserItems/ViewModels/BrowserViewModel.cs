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

using Ntreev.Crema.Client.Differences.Documents.ViewModels;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Data.Diff;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ntreev.Crema.Client.Differences.BrowserItems.ViewModels
{
    class BrowserViewModel : TreeViewBase, IBrowserItem
    {
        private readonly DiffDataSet dataSet;
        private ICommand closeCommand;

        [Import]
        private Lazy<IServiceProvider> serviceProvider = null;
        [Import]
        private DocumentServiceViewModel documentService = null;
        [Import]
        private BrowserService browserService = null;
        [Import]
        private PropertyService propertyService = null;
        [Import]
        private ICremaAppHost cremaAppHost = null;
        [Import]
        private ICremaHost cremaHost = null;
        [Import]
        private Authenticator authenticator = null;
        private TableTreeViewItemViewModel selectedTable;

        public BrowserViewModel(DiffDataSet dataSet)
        {
            this.dataSet = dataSet;
            this.closeCommand = new DelegateCommand((p) => this.Close(), (p) => this.CanClose);
            this.DisplayName = dataSet.Header1;
            this.Dispatcher.InvokeAsync(() => this.AttachPropertyService(this.propertyService));
        }

        public async void Close()
        {
            if (AppMessageBox.ConfirmDelete() == false)
                return;
            this.BeginProgress();
            await this.CloseDocumentsAsync(false);
            this.EndProgress();
            this.browserService.ItemsSource.Remove(this);
        }

        public void Import()
        {
            if (this.SelectedItem is TableTreeViewItemViewModel viewModel)
            {
                this.Import(viewModel);
            }
        }

        public bool CanClose
        {
            get { return true; }
        }

        public bool CanImport
        {
            get
            {
                if (this.SelectedItem is TableTreeViewItemViewModel viewModel)
                {
                    if (viewModel.IsActivated == false)
                        return false;
                    return viewModel.IsResolved && viewModel.Parent is TableTreeViewItemViewModel == false;
                }
                return false;
            }
        }

        public ICommand CloseCommand
        {
            get { return this.closeCommand; }
        }

        private async Task CloseDocumentsAsync(bool save)
        {
            var query = from item in this.documentService.Documents
                        let document = item as DifferenceDocumentBase
                        where document != null && document.Target.Browser == this
                        select document;

            var documentList = query.ToList();
            foreach (var item in documentList.ToArray())
            {
                item.Disposed += (s, e) => documentList.Remove(item);
                await this.Dispatcher.InvokeAsync(() =>
                {
                    if (item.IsModified == true && save == false)
                        item.IsModified = false;
                    item.Dispose();
                });
                await Task.Delay(1);
            }

            while (documentList.Any())
            {
                await Task.Delay(1);
            }
        }

        public void UpdateItemsSource()
        {
            var compositionService = this.ServiceProvider.GetService(typeof(ICompositionService)) as ICompositionService;

            var typesCategory = new CategoryTreeViewItemViewModel("types");
            compositionService.SatisfyImportsOnce(typesCategory);
            foreach (var item in this.dataSet.Types)
            {
                var viewModel = new TypeTreeViewItemViewModel(this, item);
                compositionService.SatisfyImportsOnce(viewModel);
                typesCategory.Items.Add(viewModel);
            }
            this.Items.Add(typesCategory);

            var templatesCategory = new CategoryTreeViewItemViewModel("templates");
            compositionService.SatisfyImportsOnce(templatesCategory);
            foreach (var item in this.dataSet.Tables)
            {
                if (item.TemplatedParent != null)
                    continue;
                var viewModel = new TemplateTreeViewItemViewModel(this, item.Template);
                compositionService.SatisfyImportsOnce(viewModel);
                templatesCategory.Items.Add(viewModel);
            }
            this.Items.Add(templatesCategory);

            var tablesCategory = new CategoryTreeViewItemViewModel("tables");
            compositionService.SatisfyImportsOnce(tablesCategory);
            foreach (var item in this.dataSet.Tables)
            {
                var viewModel = new TableTreeViewItemViewModel(this, item);
                compositionService.SatisfyImportsOnce(viewModel);
                tablesCategory.Items.Add(viewModel);
            }
            this.Items.Add(tablesCategory);
        }

        public bool IsVisible => true;

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);

            if (this.selectedTable != null)
            {
                this.selectedTable.PropertyChanged -= SelectedTable_PropertyChanged;
            }
            this.propertyService.SelectedObject = this.SelectedItem;
            this.selectedTable = this.SelectedItem as TableTreeViewItemViewModel;
            if (this.selectedTable != null)
            {
                this.selectedTable.PropertyChanged += SelectedTable_PropertyChanged;
            }
            this.NotifyOfPropertyChange(nameof(this.CanImport));
        }

        private void Shell_Loaded(object sender, EventArgs e)
        {
            this.UpdateItemsSource();
        }

        private void Shell_Unloaded(object sender, EventArgs e)
        {
            this.Items.Clear();
        }

        private void SelectedTable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TableTreeViewItemViewModel.IsResolved))
            {
                this.NotifyOfPropertyChange(nameof(this.CanImport));
            }
        }

        private async void Import(TableTreeViewItemViewModel viewModel)
        {
            try
            {
                var dataTable = viewModel.Source.ExportTable2();
                var dataBaseName = this.cremaAppHost.DataBaseName;
                var dataBase = await this.cremaHost.Dispatcher.InvokeAsync(() => this.cremaHost.DataBases[dataBaseName]);

                var comment = this.GetComment(viewModel.DisplayName);
                if (comment == null)
                    return;

                var dialog = new ProgressViewModel();
                dialog.DisplayName = viewModel.DisplayName;
                dialog.ShowDialog(() =>
                {
                    this.cremaHost.Dispatcher.Invoke(() => dataBase.Import(this.authenticator, dataTable.DataSet, comment));
                });
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
        }

        private string GetComment(string displayName)
        {
            var dialog = new CommentViewModel()
            {
                DisplayName = displayName,
            };
            if (dialog.ShowDialog() == true)
                return dialog.Comment;
            return null;
        }

        private IServiceProvider ServiceProvider => this.serviceProvider.Value;
    }
}
