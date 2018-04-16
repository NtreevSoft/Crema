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

using Caliburn.Micro;
using Ntreev.Crema.Data;
using Ntreev.Crema.Designer.ViewModels;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.Crema.Designer.Tables.ViewModels
{
    class TableEditorViewModel : DocumentBase, IPartImportsSatisfiedNotification
    {
        [Import]
        private IServiceProvider serviceProvider = null;
        private readonly CremaDataTable dataTable;

        private ObservableCollection<TableItemViewModel> tableItems;
        private TableItemViewModel selectedItem;
        private string selectedTableName;
        private bool isLoaded;

        private bool isFinderVisible;

        public TableEditorViewModel(CremaDataTable dataTable)
        {
            this.dataTable = dataTable;
        }

        public CremaDataTable Table
        {
            get { return this.dataTable; }
        }

        public ObservableCollection<TableItemViewModel> Tables
        {
            get { return this.tableItems; }
        }

        public string SelectedTableName
        {
            get
            {
                if (this.selectedItem == null)
                    return string.Empty;
                return this.selectedItem.ToString();
            }
            set
            {
                if (this.tableItems == null)
                {
                    this.selectedTableName = value;
                    return;
                }

                var query = from item in this.tableItems
                            where item.ToString() == value
                            select item;

                if (query.Any() == false)
                    return;

                this.SelectedItem = query.First();
            }
        }

        public TableItemViewModel SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                if (this.selectedItem != null)
                {
                    this.selectedItem.IsVisible = false;
                }
                this.selectedItem = value;
                if (this.selectedItem != null)
                {
                    this.selectedItem.IsVisible = true;

                    var browser = this.serviceProvider.GetService(typeof(TableBrowserViewModel)) as TableBrowserViewModel;
                    var viewModel = this.selectedItem.Table.ExtendedProperties[browser] as TableTreeViewItemViewModel;
                    browser.SelectedItem = viewModel;
                }
                this.NotifyOfPropertyChange(nameof(this.SelectedItem));
                this.OnSelectionChanged(EventArgs.Empty);
            }
        }

        public bool IsLoaded
        {
            get { return this.isLoaded; }
            set
            {
                this.isLoaded = value;
                this.NotifyOfPropertyChange(nameof(this.IsLoaded));
            }
        }

        public bool IsFinderVisible
        {
            get { return this.isFinderVisible; }
            set
            {
                this.isFinderVisible = value;
                this.NotifyOfPropertyChange(nameof(this.IsFinderVisible));
            }
        }

        public event EventHandler SelectionChanged;

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            this.SelectionChanged?.Invoke(this, e);
        }

        protected override Task CloseAsync()
        {
            return Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in this.tableItems)
                {
                    item.Dispose();
                }
                this.tableItems = null;
            }).Task;
        }

        private void Initialize()
        {
            try
            {
                this.BeginProgress();
                this.DisplayName = this.dataTable.Name;

                var tableItemList = new List<TableItemViewModel>
                {
                    new TableItemViewModel(this.dataTable)
                };
                foreach (var item in this.dataTable.Childs)
                {
                    tableItemList.Add(new TableItemViewModel(item));
                }

                var compositionService = this.serviceProvider.GetService(typeof(ICompositionService)) as ICompositionService;
                foreach (var item in tableItemList)
                {
                    compositionService.SatisfyImportsOnce(item);
                }

                this.tableItems = new ObservableCollection<TableItemViewModel>(tableItemList);
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e.Message);
                this.EndProgress();
                this.TryClose();
                return;
            }

            if (this.selectedTableName != null)
                this.selectedItem = this.tableItems.Where(item => item.Name == this.selectedTableName).First();
            else
                this.selectedItem = this.tableItems.First();

            this.selectedItem.IsVisible = true;
            this.selectedTableName = null;

            this.EndProgress();
            this.IsLoaded = true;
            this.NotifyOfPropertyChange(nameof(this.Tables));
            this.NotifyOfPropertyChange(nameof(this.SelectedItem));
            this.NotifyOfPropertyChange(nameof(this.IsProgressing));
        }

        #region IPartImportsSatisfiedNotification

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            this.Initialize();
        }

        #endregion

        //#region ITableDocument

        //IEnumerable<ITableDocumentItem> ITableDocument.TableItems
        //{
        //    get { return this.tableItems; }
        //}

        //ITableDocumentItem ITableDocument.SelectedItem
        //{
        //    get { return this.selectedItem; }
        //}

        //#endregion
    }
}
