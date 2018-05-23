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
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.ModernUI.Framework.DataGrid.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Views;

namespace Ntreev.Crema.Presentation.Controls
{
    [TemplatePart(Name = "PART_DataGridControl", Type = typeof(ModernDataGridControl))]
    public class CremaDataTypeControl : Control
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(CremaDataType), typeof(CremaDataTypeControl));

        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register(nameof(ReadOnly), typeof(bool), typeof(CremaDataTypeControl));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(CremaDataTypeControl),
                new FrameworkPropertyMetadata(SelectedItemPropertyChangedCallback));

        public static readonly DependencyProperty SelectedColumnProperty =
            DependencyProperty.Register(nameof(SelectedColumn), typeof(object), typeof(CremaDataTypeControl),
                new FrameworkPropertyMetadata(SelectedColumnPropertyChangedCallback));

        public static readonly DependencyProperty IsVerticalScrollBarOnLeftSideProperty =
            DependencyProperty.Register(nameof(IsVerticalScrollBarOnLeftSide), typeof(bool), typeof(CremaDataTypeControl));

        private readonly DataGridCollectionViewSource viewSource = new DataGridCollectionViewSource();
        private ModernDataGridControl dataGridControl;

        public CremaDataTypeControl()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.dataGridControl = this.Template.FindName("PART_DataGridControl", this) as ModernDataGridControl;

            if (this.dataGridControl == null)
                return;

            //if (this.dataGridControl.Columns.Count == 0)
            {
                try
                {
#if DEBUG
                    this.dataGridControl.Columns.Add(new Column() { FieldName = CremaSchema.Index, Title = CremaSchema.Index, });
#endif
                    this.dataGridControl.Columns.Add(new Column() { FieldName = CremaSchema.ID, Title = CremaSchema.ID, });
                }
                catch
                {

                }
            }

            if (this.dataGridControl.AutoCreateColumns == false)
            {
                try
                {
                    var columnNames = new string[] { "tagColumn", "enableColumn", "modifierColumn", "modifiedDateTimeColumn", "creatorColumn", "createdDateTimeColumn" };
                    foreach (var item in columnNames)
                    {
                        if (this.TryFindResource(item) is ColumnBase column)
                            this.dataGridControl.Columns.Add(column);
                    }
                }
                catch
                {

                }
            }

            this.dataGridControl.RowDrag += DataGridControl_RowDrag;
            this.dataGridControl.RowDrop += DataGridControl_RowDrop;
            this.dataGridControl.ItemsSourceChangeCompleted += DataGridControl_ItemsSourceChangeCompleted;
            BindingOperations.SetBinding(this.viewSource, DataGridCollectionViewSource.SourceProperty, new Binding($"{nameof(Source)}.{nameof(CremaDataType.View)}") { Source = this, });
            BindingOperations.SetBinding(this.dataGridControl, ModernDataGridControl.ItemsSourceProperty, new Binding() { Source = this.viewSource, });
            BindingOperations.SetBinding(this.dataGridControl, ModernDataGridControl.IsVerticalScrollBarOnLeftSideProperty, new Binding(nameof(IsVerticalScrollBarOnLeftSide)) { Source = this, });
            this.dataGridControl.IsDeleteCommandEnabled = true;
        }

        public CremaDataType Source
        {
            get { return (CremaDataType)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        public object SelectedItem
        {
            get { return (object)this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        public string SelectedColumn
        {
            get { return (string)this.GetValue(SelectedColumnProperty); }
            set { this.SetValue(SelectedColumnProperty, value); }
        }

        public bool IsVerticalScrollBarOnLeftSide
        {
            get { return (bool)this.GetValue(IsVerticalScrollBarOnLeftSideProperty); }
            set { this.SetValue(IsVerticalScrollBarOnLeftSideProperty, value); }
        }

        public new bool Focus()
        {
            var viewer = this.dataGridControl.Template.FindName("PART_ScrollViewer", this.dataGridControl) as ScrollViewer;
            if (viewer == null)
                return false;
            return viewer.Focus();
        }

        private void DataGridControl_RowDrop(object sender, ModernDragEventArgs e)
        {
            var data = e.Data;
            var gridContext = e.GridContext as DataGridContext;
            if (data.GetDataPresent(typeof(int)) == true)
            {
                var index = (int)data.GetData(typeof(int));
                var item = e.Item;
                var prop = TypeDescriptor.GetProperties(item)[CremaSchema.Index];
                if (gridContext != null)
                {
                    gridContext.SelectedCellRanges.Clear();
                    gridContext.CurrentColumn = null;
                }
                prop.SetValue(item, index);
                e.Handled = true;
                this.Dispatcher.InvokeAsync(() =>
                {
                    if (gridContext != null)
                    {
                        gridContext.SelectItem(item);
                        gridContext.CurrentItem = item;
                    }
                }, DispatcherPriority.Render);
            }
        }

        private void DataGridControl_RowDrag(object sender, ModernDragEventArgs e)
        {
            var item = e.Item;
            var data = e.Data;
            var prop = TypeDescriptor.GetProperties(item)[CremaSchema.Index];
            var value = (int)prop.GetValue(item);
            data.SetData(value);
            e.Handled = true;
        }

        private void DataGridControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.dataGridControl.CurrentItem))
            {
                this.SelectedItem = this.dataGridControl.CurrentItem;
            }
            else if (e.PropertyName == nameof(this.dataGridControl.CurrentColumn))
            {
                if (this.dataGridControl.CurrentColumn != null)
                {
                    this.SelectedColumn = this.dataGridControl.CurrentColumn.FieldName;
                }
                else
                {
                    this.SelectedColumn = null;
                }
            }
        }

        private void DataGridControl_ItemsSourceChangeCompleted(object sender, EventArgs e)
        {
            if (this.dataGridControl.AutoCreateColumns == true)
            {
                var index = 0;

                this.dataGridControl.Columns[CremaSchema.Tags].MaxWidth = 40;
                this.dataGridControl.Columns[CremaSchema.Tags].MinWidth = 40;
                this.dataGridControl.Columns[CremaSchema.Tags].TitleTemplate = this.TryFindResource("EmptyTitle_DataTemplate") as DataTemplate;
                this.dataGridControl.Columns[CremaSchema.Tags].CellHorizontalContentAlignment = HorizontalAlignment.Center;
                this.dataGridControl.Columns[CremaSchema.Tags].CellEditor = this.TryFindResource("TagSelector") as CellEditor;
                this.dataGridControl.Columns[CremaSchema.Tags].CellContentTemplate = this.TryFindResource("TagSelector_ContentTemplate") as DataTemplate;
                this.dataGridControl.Columns[CremaSchema.Tags].VisiblePosition = index++;
                this.dataGridControl.Columns[CremaSchema.Tags].Visible = false;
                this.dataGridControl.Columns[CremaSchema.Enable].MaxWidth = 28;
                this.dataGridControl.Columns[CremaSchema.Enable].MinWidth = 28;
                this.dataGridControl.Columns[CremaSchema.Enable].TitleTemplate = this.TryFindResource("EmptyTitle_DataTemplate") as DataTemplate;
                this.dataGridControl.Columns[CremaSchema.Enable].VisiblePosition = index++;
                this.dataGridControl.Columns[CremaSchema.Enable].CellEditor = this.TryFindResource("EnableEditor") as CellEditor;
                this.dataGridControl.Columns[CremaSchema.Enable].CellContentTemplate = this.TryFindResource("EnableContentTemplate") as DataTemplate;
                this.dataGridControl.Columns[CremaSchema.Enable].Visible = false;
                this.dataGridControl.Columns[CremaSchema.ID].VisiblePosition = index++;
                this.dataGridControl.Columns[CremaSchema.Name].VisiblePosition = index++;
                this.dataGridControl.Columns[CremaSchema.Value].VisiblePosition = index++;
                this.dataGridControl.Columns[CremaSchema.Comment].VisiblePosition = index++;
                this.dataGridControl.Columns[CremaSchema.Creator].VisiblePosition = index++;
                this.dataGridControl.Columns[CremaSchema.CreatedDateTime].VisiblePosition = index++;
                this.dataGridControl.Columns[CremaSchema.Modifier].VisiblePosition = index++;
                this.dataGridControl.Columns[CremaSchema.ModifiedDateTime].VisiblePosition = index++;

                foreach (var item in this.dataGridControl.Columns)
                {
                    item.AllowSort = false;
                }
            }
            this.dataGridControl.PropertyChanged -= DataGridControl_PropertyChanged;
            this.dataGridControl.PropertyChanged += DataGridControl_PropertyChanged;
            this.dataGridControl.LayoutUpdated += DataGridControl_LayoutUpdated;
        }

        private void DataGridControl_LayoutUpdated(object sender, EventArgs e)
        {
            this.dataGridControl.LayoutUpdated -= DataGridControl_LayoutUpdated;

            var selectedItem = this.SelectedItem;
            var selectedColumn = this.SelectedColumn;

            if (this.dataGridControl.CurrentItem != selectedItem && this.dataGridControl.Items.Contains(selectedItem) == true)
            {
                this.dataGridControl.CurrentItem = selectedItem;

            }

            if (selectedColumn != null)
            {
                var column = this.dataGridControl.VisibleColumns.FirstOrDefault(item => item.FieldName == selectedColumn);

                if (this.dataGridControl.CurrentColumn != column)
                {
                    this.dataGridControl.CurrentColumn = column;
                }
            }
            this.Focus();
        }

        private static void SelectedItemPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as CremaDataTypeControl;
            var gridControl = self.dataGridControl;
            var item = e.NewValue;

            if (gridControl.CurrentItem != item && gridControl.Items.Contains(item) == true)
            {
                gridControl.CurrentItem = item;
            }
        }

        private static void SelectedColumnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as CremaDataTypeControl;
            var gridControl = self.dataGridControl;
            var columnName = (string)e.NewValue;

            var column = gridControl.VisibleColumns.FirstOrDefault(item => item.FieldName == columnName);

            if (column == null)
                return;

            if (gridControl.CurrentColumn != column)
            {
                gridControl.CurrentColumn = column;
            }
        }
    }
}
