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

using Ntreev.Crema.Client.Differences.BrowserItems.ViewModels;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Diff;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ntreev.Crema.Client.Differences.Documents.ViewModels
{
    class TableDocumentViewModel : DifferenceDocumentBase
    {
        private readonly TableTreeViewItemViewModel viewModel;
        private readonly List<TableDocumentItemViewModel> itemList = new List<TableDocumentItemViewModel>();
        private TableDocumentItemViewModel selectedItem;
        private UndoService undoService = new UndoService();
        private ICommand resolveCommand;

        public TableDocumentViewModel(TableTreeViewItemViewModel viewModel)
            : base(viewModel)
        {
            this.viewModel = viewModel;
            this.itemList.Add(new TableDocumentItemViewModel(viewModel));
            foreach (var item in viewModel.Items.OfType<TableTreeViewItemViewModel>())
            {
                this.itemList.Add(new TableDocumentItemViewModel(item));
            }
            foreach (var item in this.itemList)
            {
                item.PropertyChanged += DocumentItem_PropertyChanged;
            }
            this.undoService.Changed += UndoService_Changed;
            this.resolveCommand = new DelegateCommand((p) => this.Resolve(), (p) => this.CanResolve);
            this.SelectedItem = this.itemList.First();
            this.DisplayName = viewModel.DisplayName;
        }

        public void Resolve()
        {
            try
            {
                this.Source.ResolveAll();
                this.undoService.Clear();
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
        }

        public override string ToString()
        {
            return this.viewModel.ToString();
        }

        public DiffDataTable Source
        {
            get { return this.viewModel.Source; }
        }

        public IEnumerable<TableDocumentItemViewModel> ItemsSource
        {
            get { return this.itemList; }
        }

        public TableDocumentItemViewModel SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                this.selectedItem = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedItem));
            }
        }

        public string SelectedName
        {
            get { return $"{this.selectedItem}"; }
            set
            {
                var query = from item in this.itemList
                            where $"{item}" == value
                            select item;

                if (query.Any() == false)
                    return;

                this.SelectedItem = query.First();
            }
        }

        public ICommand ResolveCommand => this.resolveCommand;

        public IUndoService UndoService
        {
            get { return this.undoService; }
        }

        public bool CanResolve
        {
            get
            {
                return this.SelectedItem.Source.IsResolved == false && this.Source.UnresolvedItems.Any() == false;
            }
        }

        protected override Task CloseAsync()
        {
            return this.Dispatcher.InvokeAsync(() =>
            {
                if (this.IsModified == false)
                {
                    foreach (var item in this.itemList)
                    {
                        item.Source.RejectChanges();
                    }
                }
                else
                {
                    foreach (var item in this.itemList)
                    {
                        item.Source.AcceptChanges();
                    }
                }
            }).Task;
        }

        private void DocumentItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.IsModified = this.itemList.Where(item => item.IsModified).Any();
        }

        private void UndoService_Changed(object sender, EventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in this.itemList)
                {
                    item.RefreshModifiedState();
                }
            });
        }
    }
}
