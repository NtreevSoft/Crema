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
using Ntreev.ModernUI.Framework.DataGrid.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.DataGrid;
using System.Collections;
using Xceed.Wpf.DataGrid.Views;
using System.Collections.Specialized;
using Ntreev.Crema.Presentation.Controls;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Presentation.Controls.Actions;
using Ntreev.Library.Linq;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ntreev.Crema.Presentation.Controls
{
    public class DiffDataGridControl : ModernDataGridControl
    {
        public static readonly DependencyProperty DestControlProperty =
            DependencyProperty.Register(nameof(DestControl), typeof(DiffDataGridControl), typeof(DiffDataGridControl),
                new PropertyMetadata(null, DestControlPropertyChangedCallback));

        public static readonly DependencyProperty VerticalSyncScrollBarProperty =
            DependencyProperty.Register(nameof(VerticalSyncScrollBar), typeof(ScrollBar), typeof(DiffDataGridControl),
                new PropertyMetadata(null, VerticalSyncScrollBarPropertyChangedCallback));

        public static readonly DependencyProperty HorizontalSyncScrollBarProperty =
            DependencyProperty.Register(nameof(HorizontalSyncScrollBar), typeof(ScrollBar), typeof(DiffDataGridControl),
                new PropertyMetadata(null, HorizontalSyncScrollBarPropertyChangedCallback));

        private ScrollViewer scrollViewer;
        private ScrollViewer destScrollViewer;
        private FieldStack<bool> preventEvent = new FieldStack<bool>();
        private FieldStack<bool> preventSelection = new FieldStack<bool>();

        static DiffDataGridControl()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) == false)
                TableView.FixedColumnCountProperty.OverrideMetadata(typeof(DataGridContext), new UIPropertyMetadata(FixedColumnCountPropertyChangedCallback));
        }

        public DiffDataGridControl()
        {
            this.Loaded += DiffDataGridControl_Loaded;
            this.IsDeleteCommandEnabled = false;
        }

        public void RefreshSelection()
        {
            if (this.DestControl != null)
            {
                this.SyncSelection(this.DestControl);
            }
        }

        public void RefreshFocus()
        {
            if (this.DestControl != null)
            {
                this.SyncCurrentItem(this.DestControl);
                this.SyncCurrentColumn(this.DestControl);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.scrollViewer = this.Template.FindName("PART_ScrollViewer", this) as ScrollViewer;
            this.scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
        }

        public DiffDataGridControl DestControl
        {
            get { return (DiffDataGridControl)this.GetValue(DestControlProperty); }
            set { this.SetValue(DestControlProperty, value); }
        }

        public ScrollBar VerticalSyncScrollBar
        {
            get { return (ScrollBar)this.GetValue(VerticalSyncScrollBarProperty); }
            set { this.SetValue(VerticalSyncScrollBarProperty, value); }
        }

        public ScrollBar HorizontalSyncScrollBar
        {
            get { return (ScrollBar)this.GetValue(HorizontalSyncScrollBarProperty); }
            set { this.SetValue(HorizontalSyncScrollBarProperty, value); }
        }

        public ScrollViewer DestScrollViewer
        {
            get
            {
                if (this.DestControl == null)
                    return null;
                if (this.destScrollViewer == null)
                    this.destScrollViewer = this.DestControl.Template.FindName("PART_ScrollViewer", this.DestControl) as ScrollViewer;
                return this.destScrollViewer;
            }
        }

        protected override void OnExpandGroupExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            base.OnExpandGroupExecuted(sender, e);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DiffDataContainer;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            foreach (var item in this.Columns)
            {
                item.PropertyChanged += Column_PropertyChanged;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DiffDataContainer();
        }

        protected override void OnRowDrag(ModernDragEventArgs e)
        {
            base.OnRowDrag(e);
        }

        protected override void OnRowDrop(ModernDragEventArgs e)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            if (undoService != null)
            {
                var action = new SetIndexAction(e.Item)
                {
                    Selector = DiffUndoService.GetSelector(this),
                    SelectedItem = this.DataContext
                };
                base.OnRowDrop(e);
                if (e.Handled == true)
                {
                    action.Complete();
                    undoService.Push(action);
                }
            }
            else
            {
                base.OnRowDrop(e);
            }
        }

        private ColumnBase FindDestColumn(ColumnBase column)
        {
            var destControl = this.DestControl;
            if (destControl == null)
                return null;

            if (this.Columns.Contains(column) == true)
            {
                var index = this.Columns.IndexOf(column);
                return destControl.Columns[index];
            }
            else
            {
                throw new NotImplementedException();
            }
            return null;
        }

        private void DiffDataGridControl_Loaded(object sender, RoutedEventArgs e)
        {
            var vertScrollBar = this.VerticalSyncScrollBar;
            var horzScrollBar = this.HorizontalSyncScrollBar;
            var scrollViewer = this.scrollViewer;
            if (vertScrollBar != null)
            {
                var vs = scrollViewer.Template.FindName("PART_VerticalScrollBar", scrollViewer) as ScrollBar;
                BindingOperations.SetBinding(vertScrollBar, ScrollBar.MaximumProperty, new Binding("Maximum") { Source = vs, Mode = BindingMode.OneWay });
                BindingOperations.SetBinding(vertScrollBar, ScrollBar.MinimumProperty, new Binding("Minimum") { Source = vs, Mode = BindingMode.OneWay });
                BindingOperations.SetBinding(vertScrollBar, ScrollBar.LargeChangeProperty, new Binding("LargeChange") { Source = vs, Mode = BindingMode.OneWay });
                BindingOperations.SetBinding(vertScrollBar, ScrollBar.SmallChangeProperty, new Binding("SmallChange") { Source = vs, Mode = BindingMode.OneWay });
                BindingOperations.SetBinding(vertScrollBar, ScrollBar.ViewportSizeProperty, new Binding("ViewportSize") { Source = vs, Mode = BindingMode.OneWay });
                BindingOperations.SetBinding(vertScrollBar, ScrollBar.VisibilityProperty, new Binding("Visibility") { Source = vs, Mode = BindingMode.OneWay });
            }
            if (horzScrollBar != null)
            {
                var vs = scrollViewer.Template.FindName("PART_HorizontalScrollBar", scrollViewer) as ScrollBar;
                BindingOperations.SetBinding(horzScrollBar, ScrollBar.MaximumProperty, new Binding("Maximum") { Source = vs, Mode = BindingMode.OneWay });
                BindingOperations.SetBinding(horzScrollBar, ScrollBar.MinimumProperty, new Binding("Minimum") { Source = vs, Mode = BindingMode.OneWay });
                BindingOperations.SetBinding(horzScrollBar, ScrollBar.LargeChangeProperty, new Binding("LargeChange") { Source = vs, Mode = BindingMode.OneWay });
                BindingOperations.SetBinding(horzScrollBar, ScrollBar.SmallChangeProperty, new Binding("SmallChange") { Source = vs, Mode = BindingMode.OneWay });
                BindingOperations.SetBinding(horzScrollBar, ScrollBar.ViewportSizeProperty, new Binding("ViewportSize") { Source = vs, Mode = BindingMode.OneWay });
                BindingOperations.SetBinding(horzScrollBar, ScrollBar.VisibilityProperty, new Binding("Visibility") { Source = vs, Mode = BindingMode.OneWay });
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var destViewer = this.DestScrollViewer;
            if (destViewer == null)
                return;

            this.scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
            if (destViewer.VerticalOffset != this.scrollViewer.VerticalOffset)
                destViewer.ScrollToVerticalOffset(this.scrollViewer.VerticalOffset);
            if (destViewer.HorizontalOffset != this.scrollViewer.HorizontalOffset)
                destViewer.ScrollToHorizontalOffset(this.scrollViewer.HorizontalOffset);

            if (this.VerticalSyncScrollBar != null && this.VerticalSyncScrollBar.Value != this.scrollViewer.VerticalOffset)
                this.VerticalSyncScrollBar.Value = this.scrollViewer.VerticalOffset;
            if (this.HorizontalSyncScrollBar != null && this.HorizontalSyncScrollBar.Value != this.scrollViewer.HorizontalOffset)
                this.HorizontalSyncScrollBar.Value = this.scrollViewer.HorizontalOffset;

            this.scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
        }

        private void Column_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var column = sender as ColumnBase;
            if (e.PropertyName == nameof(column.Visible))
            {
                var destColumn = this.FindDestColumn(column);
                if (destColumn != null)
                    destColumn.Visible = column.Visible;
            }
            else if (e.PropertyName == nameof(column.Width))
            {
                var destColumn = this.FindDestColumn(column);
                if (destColumn != null)
                    destColumn.Width = column.Width;
            }
        }

        private void DiffDataGridControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.DestControl == null)
                return;

            //if (this.GlobalCurrentItem != null && e.PropertyName == nameof(this.GlobalCurrentItem))
            //{
            //    this.SyncCurrentItem();
            //}

            //if (this.GlobalCurrentColumn != null && e.PropertyName == nameof(this.GlobalCurrentColumn))
            //{
            //    this.SyncCurrentColumn();
            //}
        }

        private void DestControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.preventEvent == true)
                return;

            if (sender is DiffDataGridControl destControl)
            {
                if (e.PropertyName == nameof(destControl.GlobalCurrentItem))
                {
                    this.SyncCurrentItem(destControl);
                }
                else if (e.PropertyName == nameof(destControl.GlobalCurrentColumn))
                {
                    this.SyncCurrentColumn(destControl);
                }
            }
        }

        private void DestControl_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            if (this.preventSelection == true)
                return;

            if (sender is DiffDataGridControl destControl)
            {
                this.SyncSelection(destControl);
            }
        }

        private void SyncSelection(DiffDataGridControl destControl)
        {
            using (destControl.preventSelection.Set(true))
            {
                {
                    this.SelectedItemRanges.Clear();
                    foreach (var item in DataGridControl.GetDataGridContext(this).GetChildContexts())
                    {
                        item.SelectedItemRanges.Clear();
                    }
                }

                if (destControl.GlobalCurrentItem != null)
                {
                    var destContext = DataGridControl.GetDataGridContext(destControl);
                    Select(destContext);

                    foreach (var item in destContext.GetChildContexts())
                    {
                        Select(item);
                    }
                }
            }

            void Select(DataGridContext destContext)
            {
                var gridContext = this.FindContext(destContext);
                var query = from item in destContext.GetSelectedItems()
                            let obj = this.FindItem(gridContext.Items, DiffUtility.GetItemKey(item))
                            where obj != null
                            select obj;

                if (query.Count() == destContext.GetSelectedItems().Count())
                {
                    var selectionRanges = gridContext.GenerateSelectionRanges(query);
                    foreach (var item in selectionRanges)
                    {
                        gridContext.SelectedItemRanges.Add(item);
                    }
                }
            }
        }

        private void SyncCurrentColumn(DiffDataGridControl destControl)
        {
            var destCurrentColumn = destControl.GlobalCurrentColumn;
            if (destCurrentColumn == null)
                return;

            var dataContext = this.FindContext(destControl.CurrentContext);
            if (dataContext == null)
                return;

            using (destControl.preventEvent.Set(true))
            {
                if (destCurrentColumn == null)
                    dataContext.CurrentColumn = null;
                else
                    dataContext.CurrentColumn = dataContext.Columns[destCurrentColumn.FieldName];
            }
        }

        private void SyncCurrentItem(DiffDataGridControl destControl)
        {
            var destCurrentItem = destControl.GlobalCurrentItem;
            if (destCurrentItem == null)
                return;

            var dataContext = this.FindContext(destControl.CurrentContext);
            if (dataContext == null)
                return;

            using (destControl.preventEvent.Set(true))
            {
                dataContext.CurrentItem = this.FindItem(dataContext.Items, DiffUtility.GetItemKey(destControl.GlobalCurrentItem));
            }
        }

        private static void FixedColumnCountPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gridContext = d as DataGridContext;
            var gridControl = gridContext.DataGridControl as DiffDataGridControl;
            if (gridControl == null)
                return;
            var destControl = gridControl.DestControl;
            if (destControl == null)
                return;

            if (gridContext.ParentDataGridContext == null)
            {
                var destContext = DataGridControl.GetDataGridContext(destControl);
                destContext.SetValue(TableView.FixedColumnCountProperty, (int)e.NewValue);

                gridControl.OnNotifyPropertyChanged(new PropertyChangedEventArgs(TableView.FixedColumnCountProperty.Name));
                destControl.OnNotifyPropertyChanged(new PropertyChangedEventArgs(TableView.FixedColumnCountProperty.Name));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void DestControlPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as DiffDataGridControl;
            self.destScrollViewer = null;

            if (d is DiffDataGridControl gridControl)
            {
                if (e.OldValue is DiffDataGridControl oldControl)
                {
                    oldControl.SelectionChanged -= gridControl.DestControl_SelectionChanged;
                    oldControl.PropertyChanged -= gridControl.DestControl_PropertyChanged;
                }

                if (e.NewValue is DiffDataGridControl newControl)
                {
                    newControl.SelectionChanged += gridControl.DestControl_SelectionChanged;
                    newControl.PropertyChanged += gridControl.DestControl_PropertyChanged;
                }
            }
        }

        private static void VerticalSyncScrollBarPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ScrollBar oldScrollBar)
            {
                oldScrollBar.Scroll -= VerticalSyncScrollBar_Scroll;
                oldScrollBar.Tag = null;
            }

            if (e.NewValue is ScrollBar scrollBar)
            {
                scrollBar.Scroll += VerticalSyncScrollBar_Scroll;
                scrollBar.Tag = d;
            }
        }

        private static void HorizontalSyncScrollBarPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ScrollBar oldScrollBar)
            {
                oldScrollBar.Scroll -= HorizontalSyncScrollBar_Scroll;
                oldScrollBar.Tag = null;
            }

            if (e.NewValue is ScrollBar scrollBar)
            {
                scrollBar.Scroll += HorizontalSyncScrollBar_Scroll;
                scrollBar.Tag = d;
            }
        }

        private static void VerticalSyncScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            var scrollBar = sender as ScrollBar;
            var gridControl = scrollBar.Tag as DiffDataGridControl;
            scrollBar.Value = (double)((int)scrollBar.Value);

            var scrollViewer = gridControl.scrollViewer;
            if (scrollViewer != null)
                scrollViewer.ScrollToVerticalOffset((double)((int)scrollBar.Value));
        }

        private static void HorizontalSyncScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            var scrollBar = sender as ScrollBar;
            var gridControl = scrollBar.Tag as DiffDataGridControl;
            scrollBar.Value = (double)((int)scrollBar.Value);

            var scrollViewer = gridControl.scrollViewer;
            if (scrollViewer != null)
                scrollViewer.ScrollToHorizontalOffset((double)((int)scrollBar.Value));
        }

        public object FindItem(CollectionView items, object key)
        {
            if (key != null)
            {
                foreach (var item in items)
                {
                    var key2 = DiffUtility.GetItemKey(item);
                    if (object.Equals(key, key2) == true)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public DataGridContext FindContext(DataGridContext destContext)
        {
            if (destContext.ParentDataGridContext == null)
            {
                return DataGridControl.GetDataGridContext(this);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public DataGridContext FindContext(object dataContext)
        {
            var gridContext = DataGridControl.GetDataGridContext(this);
            var gridContexts = EnumerableUtility.Friends(gridContext, gridContext.GetChildContexts());

            foreach (var item in gridContexts)
            {
                if (item.Items.IndexOf(dataContext) >= 0)
                {
                    return item;
                }
            }
            return null;
        }

        internal void InvokeDetailsToggledEvent()
        {
            this.DetailsToggled?.Invoke(this, EventArgs.Empty);
        }

        internal event EventHandler DetailsToggled;

        #region classes

        struct DetailsDescriptor
        {
            public string Title { get; set; }

            public DiffState DiffState { get; set; }
        }

        #endregion
    }
}
