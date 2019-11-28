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

using Microsoft.WindowsAPICodePack.Dialogs;
using Ntreev.Crema.Spreadsheet;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Crema.Spreadsheet.Excel;

namespace Ntreev.Crema.Comparer.Tables.ViewModels
{
    [Export]
    class TableBrowserViewModel : TreeViewViewModel
    {
        [Import]
        private Lazy<IServiceProvider> serviceProvider = null;
        private IShell shell;
        [Import]
        private Lazy<TablePropertyViewModel> property = null;
        private TableTreeViewItemViewModel tableViewModel;

        [ImportingConstructor]
        public TableBrowserViewModel(IShell shell)
        {
            this.shell = shell;
            this.shell.Loaded += Shell_Loaded;
            this.shell.Unloaded += Shell_Unloaded;

            if (this.shell.DataSet != null)
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    this.UpdateItemsSource();
                });
            }
        }

        public void ExportToExcel()
        {
            var dialog = new CommonSaveFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("excel file", "*.xlsx"));
            dialog.DefaultExtension = "xlsx";

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;

            try
            {
                var dataTable = this.tableViewModel.Source.ExportTable2();
                var writer = new SpreadsheetWriter(dataTable.DataSet);
                writer.Write(dialog.FileName);
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
        }

        public bool CanExportToExcel
        {
            get
            {
                return this.tableViewModel != null && this.tableViewModel.IsResolved && this.tableViewModel.Parent is TableTreeViewItemViewModel == false;
            }
        }

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);

            this.property.Value.SelectedObject = this.SelectedItem;

            if (this.tableViewModel != null)
                this.tableViewModel.PropertyChanged -= TableViewModel_PropertyChanged;
            this.tableViewModel = this.SelectedItem as TableTreeViewItemViewModel;
            if (this.tableViewModel != null)
                this.tableViewModel.PropertyChanged += TableViewModel_PropertyChanged;
            this.NotifyOfPropertyChange(nameof(this.CanExportToExcel));
        }

        private void TableViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TableTreeViewItemViewModel.IsResolved))
                this.NotifyOfPropertyChange(nameof(this.CanExportToExcel));
        }

        private void UpdateItemsSource()
        {
            var dataSet = this.shell.DataSet;
            var compositionService = this.ServiceProvider.GetService(typeof(ICompositionService)) as ICompositionService;

            foreach (var item in dataSet.Tables)
            {
                var viewModel = new TableTreeViewItemViewModel(item)
                {
                    Header1 = dataSet.Header1,
                    Header2 = dataSet.Header2
                };
                compositionService.SatisfyImportsOnce(viewModel);
                this.Items.Add(viewModel);
            }
        }

        private void Shell_Loaded(object sender, EventArgs e)
        {
            this.UpdateItemsSource();
        }

        private void Shell_Unloaded(object sender, EventArgs e)
        {
            this.Items.Clear();
        }

        private void Import(TableTreeViewItemViewModel viewModel)
        {






            //var dataBaseName = this.cremaAppHost.DataBaseName;
            //var dataBase = await this.cremaHost.Dispatcher.InvokeAsync(() => this.cremaHost.DataBases[dataBaseName]);

            //var comment = this.GetComment();
            //if (comment == null)
            //    return;

            //var dialog = new ProgressViewModel();
            //dialog.ShowDialog(() =>
            //{
            //    this.cremaHost.Dispatcher.Invoke(() => dataBase.TableContext.Import(this.authenticator, dataTable.DataSet, comment));
            //});
        }

        private IServiceProvider ServiceProvider => this.serviceProvider.Value;
    }
}
