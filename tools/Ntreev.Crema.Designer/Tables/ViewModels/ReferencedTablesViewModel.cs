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

using Ntreev.Crema.Designer.Properties;
using Ntreev.Library;
using Ntreev.Library.Linq;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Designer.Tables.ViewModels
{
    [Export(typeof(IPropertyItem))]
    [Dependency(typeof(Types.ViewModels.TypeInfoViewModel))]
    [ParentType(typeof(Types.ViewModels.TypePropertyViewModel))]
    class ReferencedTablesViewModel : PropertyItemBase, ISelector
    {
        [Import]
        private Lazy<TableBrowserViewModel> tableBrowser = null;
        [Import]
        private ICompositionService compositionService = null;
        private ITypeDescriptor descriptor;
        private TableListBoxItemViewModel[] tables;
        private TableListBoxItemViewModel selectedTable;

        [ImportingConstructor]
        public ReferencedTablesViewModel()
        {
            this.DisplayName = Resources.Title_TablesBeingUsed;
        }

        public override bool CanSupport(object obj)
        {
            return obj is ITypeDescriptor;
        }

        public override void SelectObject(object obj)
        {
            this.Detach();
            this.descriptor = obj as ITypeDescriptor;
            this.Attach();
        }

        public override bool IsVisible
        {
            get
            {
                if (this.descriptor == null)
                    return false;
                return this.tables != null && this.tables.Any();
            }
        }

        public override object SelectedObject
        {
            get { return this.descriptor; }
        }

        public TableListBoxItemViewModel[] Tables
        {
            get { return this.tables; }
            set
            {
                this.tables = value;
                this.NotifyOfPropertyChange(nameof(this.Tables));
            }
        }

        public TableListBoxItemViewModel SelectedTable
        {
            get { return this.selectedTable; }
            set
            {
                this.selectedTable = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedTable));
            }
        }

        private void Descriptor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private void Attach()
        {
            if (this.descriptor != null)
            {
                if (this.descriptor is INotifyPropertyChanged)
                {
                    (this.descriptor as INotifyPropertyChanged).PropertyChanged += Descriptor_PropertyChanged;
                }

                var typePath = this.descriptor.TypeInfo.CategoryPath + this.descriptor.TypeInfo.Name;
                var tableBrowser = this.tableBrowser.Value;
                var tables = EnumerableUtility.Descendants<TreeViewItemViewModel, TableTreeViewItemViewModel>(tableBrowser.Items, item => item.Items)
                                              .Where(item => item.TableInfo.Columns.Any(i => i.DataType == typePath))
                                              .Select(item => new TableListBoxItemViewModel(item))
                                              .ToArray();

                foreach (var item in tables)
                {
                    this.compositionService.SatisfyImportsOnce(item);
                }

                this.Tables = tables;
            }

            this.NotifyOfPropertyChange(nameof(this.IsVisible));
            this.NotifyOfPropertyChange(nameof(this.SelectedObject));
        }

        private void Detach()
        {
            if (this.descriptor != null)
            {
                if (this.descriptor is INotifyPropertyChanged)
                {
                    (this.descriptor as INotifyPropertyChanged).PropertyChanged -= Descriptor_PropertyChanged;
                }
            }
        }

        #region ISelector

        object ISelector.SelectedItem
        {
            get { return this.SelectedTable; }
            set { this.SelectedTable = value as TableListBoxItemViewModel; }
        }

        #endregion
    }
}
