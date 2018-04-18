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
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections;
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
using Xceed.Wpf.DataGrid;
using Ntreev.Library;
using System.Windows.Threading;

namespace Ntreev.Crema.Presentation.Controls
{
    [TemplatePart(Name = "PART_DataGridControl", Type = typeof(ModernDataGridControl))]
    public class CremaTemplateControl : UserControl
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(CremaTemplate), typeof(CremaTemplateControl), 
                new PropertyMetadata(null, SourcePropertyChangedCallback));

        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register(nameof(ReadOnly), typeof(bool), typeof(CremaTemplateControl));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(CremaTemplateControl));

        public static readonly DependencyProperty SelectableTypesProperty =
            DependencyProperty.Register(nameof(SelectableTypes), typeof(IEnumerable), typeof(CremaTemplateControl), 
                new PropertyMetadata(CremaDataTypeUtility.GetBaseTypes()));

        public static readonly DependencyProperty IsVerticalScrollBarOnLeftSideProperty =
            DependencyProperty.Register(nameof(IsVerticalScrollBarOnLeftSide), typeof(bool), typeof(CremaTemplateControl));

        public static readonly DependencyProperty TypesProperty = DependencyProperty.RegisterAttached("Types", typeof(IEnumerable), typeof(CremaTemplateControl));

        public static readonly DependencyProperty TagsProperty = DependencyProperty.RegisterAttached("Tags", typeof(TagInfo), typeof(CremaTemplateControl));

        private DataGridCollectionViewSource viewSource = new DataGridCollectionViewSource();
        private ModernDataGridControl dataGridControl;

        public CremaTemplateControl()
        {

        }

        public static IEnumerable GetTypes(ColumnBase obj)
        {
            return (IEnumerable)obj.GetValue(TypesProperty);
        }

        public static void SetTypes(ColumnBase obj, IEnumerable value)
        {
            obj.SetValue(TypesProperty, value);
        }

        public static TagInfo GetTags(ModernDataRow obj)
        {
            return (TagInfo)obj.GetValue(TagsProperty);
        }

        public static void SetTags(ModernDataRow obj, TagInfo value)
        {
            obj.SetValue(TagsProperty, value);
        }

        public CremaTemplate Source
        {
            get { return (CremaTemplate)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        public IEnumerable SelectableTypes
        {
            get { return (IEnumerable)this.GetValue(SelectableTypesProperty);}
            set {this.SetValue(SelectableTypesProperty, value);}
        }

        public bool IsVerticalScrollBarOnLeftSide
        {
            get { return (bool)this.GetValue(IsVerticalScrollBarOnLeftSideProperty); }
            set { this.SetValue(IsVerticalScrollBarOnLeftSideProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.dataGridControl = this.Template.FindName("PART_DataGridControl", this) as ModernDataGridControl;

            if (this.dataGridControl == null)
                return;

            if (this.dataGridControl.Columns.Count == 0)
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

            this.dataGridControl.RowDrag += DataGridControl_RowDrag;
            this.dataGridControl.RowDrop += DataGridControl_RowDrop;
            this.dataGridControl.ItemsSourceChangeCompleted += DataGridControl_ItemsSourceChangeCompleted;
            BindingOperations.SetBinding(this.viewSource, DataGridCollectionViewSource.SourceProperty, new Binding("ItemsSource") { Source = this, });
            BindingOperations.SetBinding(this.dataGridControl, ModernDataGridControl.ItemsSourceProperty, new Binding() { Source = this.viewSource, });
            this.dataGridControl.IsDeleteCommandEnabled = true;
        }

        private void DataGridControl_ItemsSourceChangeCompleted(object sender, EventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                var index = 10000;

                {
                    var column = this.dataGridControl.Columns["DataType"];
                    if (column != null)
                    {
                        var typeSelector = this.FindResource("TypeSelector") as CellEditor;
                        column.CellEditor = typeSelector;
                        column.CellContentTemplate = this.FindResource("TypeSelector_Template") as DataTemplate;
                        SetTypes(column, this.SelectableTypes);
                    }
                }

                {
                    var column = this.dataGridControl.Columns[CremaSchema.Tags];
                    if (column != null)
                    {
                        column.CellEditor = this.FindResource("TagSelector") as CellEditor;
                        column.CellContentTemplate = this.FindResource("TagSelector_ContentTemplate") as DataTemplate;
                        column.CellHorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                    }
                }

                this.dataGridControl.Columns[CremaSchema.Modifier].VisiblePosition = index++;
                this.dataGridControl.Columns[CremaSchema.ModifiedDateTime].VisiblePosition = index++;
                this.dataGridControl.Columns[CremaSchema.Creator].VisiblePosition = index++;
                this.dataGridControl.Columns[CremaSchema.CreatedDateTime].VisiblePosition = index++;

                this.dataGridControl.Columns["ColumnName"].VisiblePosition = 0;
                this.dataGridControl.Columns["DataType"].VisiblePosition = 1;
                this.dataGridControl.Columns["IsKey"].VisiblePosition = 2;
                this.dataGridControl.Columns["Comment"].VisiblePosition = 3;

                foreach (var item in this.dataGridControl.Columns)
                {
                    item.AllowSort = false;
                }
            }, DispatcherPriority.Send);
        }

        private static void SourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is CremaTemplate cremaTemplate)
            {
                d.SetValue(ItemsSourceProperty, cremaTemplate.View);
                d.SetValue(SelectableTypesProperty, cremaTemplate.Types);
            }
            else
            {
                d.SetValue(ItemsSourceProperty, null);
                d.SetValue(SelectableTypesProperty, CremaDataTypeUtility.GetBaseTypes());
            }
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
    }
}
