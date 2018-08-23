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
using Ntreev.ModernUI.Framework.DataGrid.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ntreev.Crema.Services;
using System.ComponentModel.Composition;
using Ntreev.Crema.Client.Framework;
using Ntreev.Library;
using Ntreev.Crema.Client.Tables.MenuItems.TableMenus;

namespace Ntreev.Crema.Client.Tables.Documents.Views
{
    /// <summary>
    /// TableItemView.xaml에 대한 상호 작용 논리
    /// </summary>
    partial class TableItemView : UserControl, IDisposable
    {
        private ModernDataGridControl gridControl;

        [Import]
        private ICremaHost cremaHost = null;
        [Import]
        private QuickFindTableDataMenuItem menuItem = null;
        [Import]
        private IStatusBarService statusBarService = null;
        private ILineInfo lineInfo;

        private string columnsHashValue;

        public TableItemView()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.dataTableControl.ApplyTemplate();
            this.gridControl = this.dataTableControl.Template.FindName("PART_DataGridControl", this.dataTableControl) as ModernDataGridControl;
            if (this.gridControl != null)
            {
                this.gridControl.ItemsSourceChangeCompleted += GridControl_ItemsSourceChangeCompleted;
                this.lineInfo = new GridControlLineInfo(this.gridControl);
            }
        }

        protected async override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);

            if ((bool)e.NewValue == true && this.cremaHost != null)
            {
                var tableBrowser = this.cremaHost.GetService(typeof(ITableBrowser)) as ITableBrowser;
                var tableItemViewModel = this.DataContext as ITableDocumentItem;
                var table = tableItemViewModel.Target;

                if (table.ExtendedProperties[tableBrowser] is ITableDescriptor descriptor)
                    await this.Dispatcher.InvokeAsync(() => tableBrowser.Select(descriptor));
                this.statusBarService.LineInfo = this.lineInfo;
            }
            else
            {
                this.statusBarService.LineInfo = null;
            }
            this.menuItem.Refresh();
        }

        private void GridControl_ItemsSourceChangeCompleted(object sender, EventArgs e)
        {

        }

        private ICremaConfiguration Configs
        {
            get { return this.cremaHost.Configs; }
        }

        private void DataTableControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.dataTableControl.ApplyTemplate();
            {
                var gridControl = this.dataTableControl.Template.FindName("PART_DataGridControl", this.dataTableControl) as ModernDataGridControl;
                gridControl.PropertyChanged += GridControl_PropertyChanged;
            }
        }

        private void GridControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is ModernDataGridControl gridControl)
            {
                if (e.PropertyName == nameof(gridControl.GlobalCurrentItem) || 
                    e.PropertyName == nameof(gridControl.GlobalCurrentColumn) || 
                    (e.PropertyName == nameof(gridControl.IsBeingEdited) && gridControl.IsBeingEdited == false))
                {
                    if (gridControl.GlobalCurrentItem != null && gridControl.GlobalCurrentColumn != null)
                    {
                        if (gridControl.GetContainerFromItem(gridControl.GlobalCurrentItem) is ModernDataRow dataRow)
                        {
                            var cell = dataRow.Cells[gridControl.GlobalCurrentColumn];
                            this.currentField.Text = $"{cell.Content}";
                        }
                    }
                    else
                    {
                        this.currentField.Text = string.Empty;
                    }
                }
            }
        }
    }
}