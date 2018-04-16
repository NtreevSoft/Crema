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

using Ntreev.Crema.Data.Diff;
using Ntreev.Crema.Presentation.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Views;

namespace Ntreev.Crema.Presentation.Controls
{
    [TemplatePart(Name = PART_Grid, Type = typeof(Grid))]
    public class DiffVertMinimap : Control
    {
        public const string PART_Grid = "PART_Grid";

        public static DependencyProperty GridControlProperty =
            DependencyProperty.Register(nameof(GridControl), typeof(DiffDataGridControl), typeof(DiffVertMinimap),
                new FrameworkPropertyMetadata(null, GridControlPropertyChangedCallback));

        private Grid grid;
        private object currentItem;
        private DataGridContext currentContext;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.grid = this.Template.FindName(PART_Grid, this) as Grid;
        }

        public DiffDataGridControl GridControl
        {
            get { return (DiffDataGridControl)this.GetValue(GridControlProperty); }
            set { this.SetValue(GridControlProperty, value); }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var size = base.MeasureOverride(constraint);
            this.ResetVerticalMinimap();
            return size;
        }

        private static void GridControlPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiffVertMinimap self)
            {
                if (e.OldValue is DiffDataGridControl oldControl)
                {
                    oldControl.PropertyChanged -= self.Control_PropertyChanged;
                    oldControl.DetailsToggled += self.Control_DetailsToggled;
                }

                if (e.NewValue is DiffDataGridControl newControl)
                {
                    newControl.PropertyChanged += self.Control_PropertyChanged;
                    newControl.DetailsToggled += self.Control_DetailsToggled;
                }
            }
        }

        private void Control_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is DiffDataGridControl gridControl)
            {
                if (e.PropertyName == nameof(DiffDataGridControl.GlobalCurrentItem))
                {
                    if (this.currentItem != null)
                    {
                        if (this.currentItem is INotifyPropertyChanged rowView)
                        {
                            rowView.PropertyChanged -= RowView_PropertyChanged;
                        }
                    }

                    this.currentItem = gridControl.GlobalCurrentItem;
                    this.currentContext = gridControl.CurrentContext;

                    if (this.currentItem != null)
                    {
                        if (this.currentItem is INotifyPropertyChanged rowView)
                        {
                            rowView.PropertyChanged += RowView_PropertyChanged;
                        }
                    }
                    this.InvalidateMeasure();
                }
            }
        }

        private void RowView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.InvalidateMeasure();
        }

        private void Control_DetailsToggled(object sender, EventArgs e)
        {
            this.InvalidateMeasure();
        }

        private void ResetVerticalMinimap()
        {
            if (this.grid == null || this.GridControl == null)
                return;

            var gridContext = DataGridControl.GetDataGridContext(this.GridControl);
            var items = DiffMinimapItemInfo.CollectMinimapItems(gridContext);
            this.grid.Children.Clear();
            this.grid.RowDefinitions.Clear();
            this.grid.ColumnDefinitions.Clear();
            this.grid.ColumnDefinitions.Add(new ColumnDefinition());
            this.grid.ColumnDefinitions.Add(new ColumnDefinition());
            this.grid.ColumnDefinitions.Add(new ColumnDefinition());

            foreach (var item in items)
            {
                var brush = item.Background;
                var border = new Border() { BorderThickness = new Thickness(0), Background = brush, };
                Grid.SetRow(border, this.grid.RowDefinitions.Count);
                Grid.SetColumn(border, 0);
                Grid.SetColumnSpan(border, 3);
                if (item.GridContext.ParentItem != null)
                {
                    if (item.DiffState == DiffMinimapItemState.Null)
                    {
                        border.Width = 1;
                    }
                    else
                    {
                        Grid.SetColumn(border, 1);
                        Grid.SetColumnSpan(border, 1);
                    }
                }
                this.grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength((double)item.Count / items.Length, GridUnitType.Star), });
                this.grid.Children.Add(border);
            }
        }
    }
}
