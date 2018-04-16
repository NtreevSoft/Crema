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

using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Diff;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ntreev.Crema.Comparer.Tables.ViewModels
{
    class TableTreeViewItemViewModel : TreeViewItemViewModel, IPartImportsSatisfiedNotification
    {
        private readonly DiffDataTable diffTable;
        private string header1;
        private string header2;
        private ICommand viewCommand;

        [Import]
        private ICompositionService compositionService = null;
        [Import]
        private TableDocumentServiceViewModel documentService = null;

        public TableTreeViewItemViewModel(DiffDataTable diffTable)
        {
            this.diffTable = diffTable;
            this.diffTable.PropertyChanged += DiffType_PropertyChanged;
            this.diffTable.SourceItem1.PropertyChanged += DataTable1_PropertyChanged;
            this.diffTable.SourceItem2.PropertyChanged += DataTable2_PropertyChanged;
            this.viewCommand = new DelegateCommand(this.View);
            this.Target = diffTable;

            foreach (var item in diffTable.Childs)
            {
                this.Items.Add(new TableTreeViewItemViewModel(item));
            }
        }

        public override string ToString()
        {
            return this.DisplayName;
        }

        public void View()
        {
            this.documentService.View(this);
        }

        public override string DisplayName
        {
            get
            {
                if (this.diffTable.ItemName1 == this.diffTable.ItemName2)
                    return this.diffTable.ItemName1;
                return $"{this.diffTable.ItemName1} => {this.diffTable.ItemName2}";
            }
        }

        public DiffState DiffState
        {
            get { return this.diffTable.DiffState; }
        }

        public bool IsResolved
        {
            get { return this.diffTable.IsResolved; }
        }

        public DiffDataTable Source
        {
            get { return this.diffTable; }
        }

        public CremaDataTable Source1
        {
            get { return this.diffTable.SourceItem1; }
        }

        public CremaDataTable Source2
        {
            get { return this.diffTable.SourceItem2; }
        }

        public IEnumerable<object> UnresolvedItems
        {
            get { return this.diffTable.UnresolvedItems; }
        }

        public string Header1
        {
            get { return this.header1 ?? string.Empty; }
            set
            {
                this.header1 = value;
                this.NotifyOfPropertyChange(() => this.Header1);
            }
        }

        public string Header2
        {
            get { return this.header2; }
            set
            {
                this.header2 = value;
                this.NotifyOfPropertyChange(() => this.Header2);
            }
        }

        public ICommand ViewCommand
        {
            get { return this.viewCommand; }
        }

        public bool IsInherited
        {
            get { return this.Source2.TemplatedParentName != string.Empty; }
        }

        public bool IsBaseTemplate
        {
            get { return this.Source2.DerivedTables.Any();  }
        }

        private void DiffType_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.diffTable.DiffState) || e.PropertyName == string.Empty)
            {
                this.NotifyOfPropertyChange(nameof(this.DiffState));
            }

            if (e.PropertyName == nameof(this.diffTable.IsResolved))
            {
                this.NotifyOfPropertyChange(nameof(this.IsResolved));
            }
        }

        private void DataTable1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CremaDataTable.TableName))
            {
                this.NotifyOfPropertyChange(nameof(this.DisplayName));
            }
        }

        private void DataTable2_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CremaDataTable.TableName))
            {
                this.NotifyOfPropertyChange(nameof(this.DisplayName));
            }
        }

        #region IPartImportsSatisfiedNotification\

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            foreach (var item in this.Items)
            {
                this.compositionService.SatisfyImportsOnce(item);
            }
        }

        #endregion
    }
}
