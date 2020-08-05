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
using Ntreev.Library;
using Ntreev.ModernUI.Framework.DataGrid.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Threading;
using System.Xml.Linq;
using Ntreev.Library.Linq;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Views;

namespace Ntreev.Crema.Presentation.Controls
{
    /// <summary>
    /// CremaDataTableItemControl.xaml에 대한 상호 작용 논리
    /// </summary>
    [TemplatePart(Name = "PART_DataGridControl", Type = typeof(ModernDataGridControl))]
    public class CremaDataTableItemControl : Control
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(CremaDataTable), typeof(CremaDataTableItemControl));

        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register(nameof(ReadOnly), typeof(bool), typeof(CremaDataTableItemControl));

        public static readonly DependencyProperty IsKeyProperty = DependencyProperty.RegisterAttached("IsKey", typeof(bool), typeof(CremaDataTableItemControl), new PropertyMetadata(false));
        public static readonly DependencyProperty IsUniqueProperty = DependencyProperty.RegisterAttached("IsUnique", typeof(bool), typeof(CremaDataTableItemControl), new PropertyMetadata(false));
        public static readonly DependencyProperty FieldTypeProperty = DependencyProperty.RegisterAttached("FieldType", typeof(Type), typeof(CremaDataTableItemControl), new PropertyMetadata(typeof(string)));
        public static readonly DependencyProperty TagsProperty = DependencyProperty.RegisterAttached("Tags", typeof(TagInfo), typeof(CremaDataTableItemControl));
        private static readonly DependencyPropertyKey HasTagColorPropertyKey = DependencyProperty.RegisterAttachedReadOnly("HasTagColor", typeof(bool), typeof(CremaDataTableItemControl), new PropertyMetadata(false));
        public static readonly DependencyProperty HasTagColorProperty = HasTagColorPropertyKey.DependencyProperty;
        public static readonly DependencyProperty CommentProperty = DependencyProperty.RegisterAttached("Comment", typeof(string), typeof(CremaDataTableItemControl));
        public static readonly DependencyProperty ReferenceProperty = DependencyProperty.RegisterAttached("Reference", typeof(object), typeof(CremaDataTableItemControl));
        public static readonly DependencyProperty CremaTypeMembersProperty = DependencyProperty.RegisterAttached("CremaTypeMembers", typeof(IEnumerable), typeof(CremaDataTableItemControl));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(CremaDataTableItemControl),
                new FrameworkPropertyMetadata(SelectedItemPropertyChangedCallback));

        public static readonly DependencyProperty SelectedItemIndexProperty =
            DependencyProperty.Register(nameof(SelectedItemIndex), typeof(int), typeof(CremaDataTableItemControl),
                new FrameworkPropertyMetadata(SelectedItemIndexPropertyChangedCallback));

        public static readonly DependencyProperty SelectedColumnProperty =
            DependencyProperty.Register(nameof(SelectedColumn), typeof(object), typeof(CremaDataTableItemControl),
                new FrameworkPropertyMetadata(SelectedColumnPropertyChangedCallback));

        public static readonly DependencyProperty IsVerticalScrollBarOnLeftSideProperty =
            DependencyProperty.Register(nameof(IsVerticalScrollBarOnLeftSide), typeof(bool), typeof(CremaDataTableItemControl));

        public static readonly RoutedEvent SourceChangedEvent = EventManager.RegisterRoutedEvent(nameof(SourceChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CremaDataTableItemControl));

        private ModernDataGridControl dataGridControl;
        private TableView view;
        private DataGridCollectionViewSource viewSource = new DataGridCollectionViewSource();

        public CremaDataTableItemControl()
        {

        }

        public static bool GetIsKey(ColumnBase obj)
        {
            return (bool)obj.GetValue(IsKeyProperty);
        }

        public static void SetIsKey(ColumnBase obj, bool value)
        {
            obj.SetValue(IsKeyProperty, value);
        }

        public static bool GetIsUnique(ColumnBase obj)
        {
            return (bool)obj.GetValue(IsUniqueProperty);
        }

        public static void SetIsUnique(ColumnBase obj, bool value)
        {
            obj.SetValue(IsUniqueProperty, value);
        }

        public static Type GetFieldType(ColumnBase obj)
        {
            return (Type)obj.GetValue(FieldTypeProperty);
        }

        public static void SetFieldType(ColumnBase obj, Type value)
        {
            obj.SetValue(FieldTypeProperty, value);
        }

        public static TagInfo GetTags(ColumnBase obj)
        {
            return (TagInfo)obj.GetValue(TagsProperty);
        }

        public static void SetTags(ColumnBase obj, TagInfo value)
        {
            obj.SetValue(TagsProperty, value);
        }

        public static bool GetHasTagColor(ColumnBase obj)
        {
            return (bool)obj.GetValue(HasTagColorProperty);
        }

        private static void SetHasTagColor(ColumnBase obj, bool value)
        {
            obj.SetValue(HasTagColorPropertyKey, value);
        }

        public static string GetComment(ColumnBase obj)
        {
            return (string)obj.GetValue(CommentProperty);
        }

        public static void SetComment(ColumnBase obj, string value)
        {
            obj.SetValue(CommentProperty, value);
        }

        public static object GetReference(ColumnBase obj)
        {
            return (object)obj.GetValue(ReferenceProperty);
        }

        public static void SetReference(ColumnBase obj, object value)
        {
            obj.SetValue(ReferenceProperty, value);
        }

        public static object GetReference(DetailConfiguration obj)
        {
            return (object)obj.GetValue(ReferenceProperty);
        }

        public static void SetReference(DetailConfiguration obj, object value)
        {
            obj.SetValue(ReferenceProperty, value);
        }

        public static IEnumerable GetCremaTypeMembers(ColumnBase obj)
        {
            return (IEnumerable)obj.GetValue(CremaTypeMembersProperty);
        }

        public static void SetCremaTypeMembers(ColumnBase obj, IEnumerable value)
        {
            obj.SetValue(CremaTypeMembersProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.dataGridControl = this.Template.FindName("PART_DataGridControl", this) as ModernDataGridControl;

            if (this.dataGridControl == null)
                return;

            this.view = this.dataGridControl.View as TableView;

            {
                try
                {
                    this.dataGridControl.Columns.Add(new Column() { FieldName = CremaSchema.Index, ReadOnly = true, Width = 50, Title = nameof(CremaSchema.Index) });
                }
                catch
                {

                }
            }
            //if (this.dataGridControl.Columns.Count == 0)
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
            this.dataGridControl.Columns.CollectionChanged += Columns_CollectionChanged;

            BindingOperations.SetBinding(this.viewSource, CollectionViewSource.SourceProperty, new Binding("Source.DefaultView") { Source = this, });
            BindingOperations.SetBinding(this.dataGridControl, ItemsControl.ItemsSourceProperty, new Binding() { Source = this.viewSource, });
            BindingOperations.SetBinding(this.dataGridControl, ModernDataGridControl.IsVerticalScrollBarOnLeftSideProperty, new Binding("IsVerticalScrollBarOnLeftSide") { Source = this, });
        }

        private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        public new bool Focus()
        {
            var viewer = this.dataGridControl.Template.FindName("PART_ScrollViewer", this.dataGridControl) as ScrollViewer;
            if (viewer == null)
                return false;
            return viewer.Focus();
        }

        public CremaDataTable Source
        {
            get { return (CremaDataTable)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        public object SelectedItem
        {
            get { return (object)this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        public int SelectedItemIndex
        {
            get { return (int)this.GetValue(SelectedItemIndexProperty); }
            set { this.SetValue(SelectedItemIndexProperty, value); }
        }

        public string SelectedColumn
        {
            get { return (string)this.GetValue(SelectedColumnProperty); }
            set { this.SetValue(SelectedColumnProperty, value); }
        }

        public bool ReadOnly
        {
            get { return (bool)this.GetValue(ReadOnlyProperty); }
            set { this.SetValue(ReadOnlyProperty, value); }
        }

        public bool IsVerticalScrollBarOnLeftSide
        {
            get { return (bool)this.GetValue(IsVerticalScrollBarOnLeftSideProperty); }
            set { this.SetValue(IsVerticalScrollBarOnLeftSideProperty, value); }
        }

        public event RoutedEventHandler SourceChanged
        {
            add { AddHandler(SourceChangedEvent, value); }
            remove { RemoveHandler(SourceChangedEvent, value); }
        }

        protected virtual void OnItemsSourceChanged(RoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        private void DataGridControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentItem")
            {
                this.SelectedItem = this.dataGridControl.CurrentItem;

                if (this.dataGridControl.CurrentItem != null)
                {
                    this.SelectedItemIndex = this.dataGridControl.Items.IndexOf(this.dataGridControl.CurrentItem);
                }
                else
                {
                    this.SelectedItemIndex = -1;
                }
            }
            else if (e.PropertyName == "CurrentColumn")
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
            if (this.Source == null)
                return;

            foreach (var item in this.Source.Columns)
            {
                var column = this.dataGridControl.Columns[item.ColumnName];
                if (column == null)
                    continue;

                this.InitializeColumn(item, column);
            }
#if DEBUG
            if (this.dataGridControl.DetailConfigurations.Any() == true)
            {
                this.dataGridControl.Columns.Add(new Column() { FieldName = CremaSchema.__RelationID__, ReadOnly = true, });
            }
#endif

            foreach (var item in this.dataGridControl.DetailConfigurations)
            {
                var childName = CremaSchema.GetChildNameFromRelationName(item.RelationName);
                item.Title = childName;
                item.TitleTemplate = this.FindResource("DetailConfiguration_Title_Template") as DataTemplate;
                var childTable = this.Source.Childs[childName];
                this.FillColumns(item, childTable);
                SetReference(item, childTable);
            }

            var index = 0;
            this.dataGridControl.Columns[CremaSchema.Index].VisiblePosition = index++;
#if DEBUG
            if (this.dataGridControl.DetailConfigurations.Any() == true)
            {
                this.dataGridControl.Columns[CremaSchema.__RelationID__].VisiblePosition = index++;
            }
#endif
            this.dataGridControl.Columns[CremaSchema.Tags].VisiblePosition = index++;
            this.dataGridControl.Columns[CremaSchema.Enable].VisiblePosition = index++;

            foreach (var item in this.Source.Columns)
            {
                var column = this.dataGridControl.Columns[item.ColumnName];
                if (column == null)
                    continue;
                column.VisiblePosition = index++;
            }
            this.dataGridControl.Columns[CremaSchema.Modifier].VisiblePosition = index++;
            this.dataGridControl.Columns[CremaSchema.ModifiedDateTime].VisiblePosition = index++;
            this.dataGridControl.Columns[CremaSchema.Creator].VisiblePosition = index++;
            this.dataGridControl.Columns[CremaSchema.CreatedDateTime].VisiblePosition = index++;

            if (this.view != null)
            {
                var firstColumnIndex = FirstPrimaryKeyColumnIndex(this.dataGridControl.VisibleColumns, this.Source.PrimaryKey);
                if (firstColumnIndex >= 0)
                {
                    this.view.FixedColumnCount = firstColumnIndex + 1;
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

        private static int FirstPrimaryKeyColumnIndex(IEnumerable<ColumnBase> visibleColumns, IReadOnlyCollection<CremaDataColumn> columns)
        {
            if (columns == null) return -1;
            if (columns.Count == 0) return -1;

            var firstPrimaryKeyIndex = columns.FirstOrDefault();
            if (firstPrimaryKeyIndex == null) return -1;

            var firstColumnIndex = visibleColumns.IndexOf(column => column.FieldName == firstPrimaryKeyIndex.ColumnName);
            return firstColumnIndex;
        }

        private void InitializeColumn(CremaDataColumn dataColumn, ColumnBase column)
        {
            if (dataColumn.CremaType != null)
            {
                if (dataColumn.CremaType.IsFlag == true)
                    column.CellEditor = this.FindResource("CremaFlagTypeSelector") as CellEditor;
                else
                    column.CellEditor = this.FindResource("CremaTypeSelector") as CellEditor;
                SetCremaTypeMembers(column, dataColumn.CremaType.Members);
            }

            CremaDataTableItemControl.SetIsKey(column, dataColumn.IsKey);
            CremaDataTableItemControl.SetIsUnique(column, dataColumn.Unique);
            CremaDataTableItemControl.SetFieldType(column, dataColumn.DataType);
            CremaDataTableItemControl.SetTags(column, dataColumn.Tags);
            CremaDataTableItemControl.SetComment(column, dataColumn.Comment);
            CremaDataTableItemControl.SetReference(column, dataColumn);

            column.TitleTemplate = this.FindResource("Title_Template") as DataTemplate;

            if (dataColumn.DerivedTags.Color != null)
            {
                CremaDataTableItemControl.SetHasTagColor(column, true);
                column.SetValue(TextElement.ForegroundProperty, new BrushConverter().ConvertFrom(dataColumn.DerivedTags.Color));
            }
        }

        private void FillColumns(DetailConfiguration detail, CremaDataTable table)
        {
            try
            {
                detail.Columns.Add(new Column() { FieldName = CremaSchema.Index, ReadOnly = true, Width = 50, Title = nameof(CremaSchema.Index) });
#if DEBUG
                detail.Columns.Add(new Column() { FieldName = CremaSchema.__ParentID__, ReadOnly = true, });
#endif
                detail.Columns.Add(this.FindResource("tagColumn") as ColumnBase);
                detail.Columns.Add(this.FindResource("enableColumn") as ColumnBase);

                foreach (var item in table.Columns)
                {
                    var column = new Column() { FieldName = item.ColumnName, };
                    this.InitializeColumn(item, column);
                    detail.Columns.Add(column);
                }

                detail.Columns.Add(this.FindResource("modifierColumn") as ColumnBase);
                detail.Columns.Add(this.FindResource("modifiedDateTimeColumn") as ColumnBase);
                detail.Columns.Add(this.FindResource("creatorColumn") as ColumnBase);
                detail.Columns.Add(this.FindResource("createdDateTimeColumn") as ColumnBase);

                var firstPrimaryKeyColumnIndex = FirstPrimaryKeyColumnIndex(detail.VisibleColumns, table.PrimaryKey);
                if (firstPrimaryKeyColumnIndex >= 0)
                {
                    detail.SetValue(TableView.FixedColumnCountProperty, firstPrimaryKeyColumnIndex + 1);
                }
            }
            catch
            {
                throw new InvalidOperationException();
            }
        }

        private static void SelectedItemPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as CremaDataTableItemControl;
            var gridControl = self.dataGridControl;
            var item = e.NewValue;

            if (gridControl.CurrentItem != item && gridControl.Items.Contains(item) == true)
            {
                gridControl.CurrentItem = item;
                if (gridControl.GetContainerFromItem(item) is ModernDataRow dataRow)
                {
                    dataRow.BringIntoView();
                }
                else
                {
                    gridControl.BringItemIntoView(item);
                }
            }
        }

        private static void SelectedItemIndexPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as CremaDataTableItemControl;
            var gridControl = self.dataGridControl;

            if (e.NewValue is int index)
            {
                if (index >= 0 && index < gridControl.Items.Count)
                {
                    var item = gridControl.Items.GetItemAt(index);
                    if (self.GetValue(SelectedItemProperty) != item)
                    {
                        self.SetValue(SelectedItemProperty, item);
                    }
                }
                else
                {
                    if (self.GetValue(SelectedItemProperty) != null)
                    {
                        self.SetValue(SelectedItemProperty, null);
                    }
                }
            }
        }

        private static void SelectedColumnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as CremaDataTableItemControl;
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
