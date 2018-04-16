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

using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Tables.Documents.ViewModels
{
    abstract class TableDocumentBase : DocumentBase, ITableDocument, IPartImportsSatisfiedNotification
    {
        private readonly ObservableCollection<TableItemViewModel> tables = new ObservableCollection<TableItemViewModel>();
        private TableItemViewModel selectedTable;

        [Import]
        private ICompositionService compositionService = null;

        protected TableDocumentBase()
        {
            this.tables.CollectionChanged += Tables_CollectionChanged;
        }

        public object Target
        {
            get;
            protected set;
        }

        public ObservableCollection<TableItemViewModel> Tables
        {
            get { return this.tables; }
        }

        public TableItemViewModel SelectedTable
        {
            get { return this.selectedTable; }
            set
            {
                if (this.selectedTable != null)
                {
                    this.selectedTable.IsSelected = false;
                }
                this.selectedTable = value;
                if (this.selectedTable != null)
                {
                    this.selectedTable.IsSelected = true;
                }
                this.NotifyOfPropertyChange(nameof(this.SelectedTable));
                this.OnSelectionChanged(EventArgs.Empty);
            }
        }

        public event EventHandler SelectionChanged;

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            this.SelectionChanged?.Invoke(this, e);
        }

        private void Tables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            this.compositionService?.SatisfyImportsOnce(item);
                        }
                    }
                    break;
            }
        }

        #region ITableDocument

        IEnumerable<ITableDocumentItem> ITableDocument.TableItems => this.tables;

        ITableDocumentItem ITableDocument.SelectedItem
        {
            get => this.selectedTable;
            set
            {
                if (value is TableItemViewModel viewModel)
                {
                    this.SelectedTable = viewModel;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        ITable ITableDocument.Target => this.Target as ITable;

        #endregion

        #region IPartImportsSatisfiedNotification

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            foreach (var item in this.tables)
            {
                this.compositionService.SatisfyImportsOnce(item);
            }
        }

        #endregion
    }
}
