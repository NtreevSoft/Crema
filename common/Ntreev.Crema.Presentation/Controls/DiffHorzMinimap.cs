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
    public class DiffHorzMinimap : Control
    {
        public const string PART_Grid = "PART_Grid";

        public static DependencyProperty GridControlProperty =
            DependencyProperty.Register(nameof(GridControl), typeof(DiffDataGridControl), typeof(DiffHorzMinimap),
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
            if (this.currentContext != null)
                this.ResetHorizontalMinimap(this.grid, this.currentContext);
            return base.MeasureOverride(constraint);
        }

        private static void GridControlPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiffHorzMinimap self)
            {
                if (e.OldValue is DiffDataGridControl oldControl)
                {
                    oldControl.PropertyChanged -= self.Control_PropertyChanged;
                }

                if (e.NewValue is DiffDataGridControl newControl)
                {
                    newControl.PropertyChanged += self.Control_PropertyChanged;
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

        private void ResetHorizontalMinimap(Grid grid, DataGridContext gridContext)
        {
            if (gridContext.VisibleColumns.Any() == false || gridContext.CurrentItem == null)
            {
                grid.Visibility = Visibility.Hidden;
                return;
            }

            var diffState = DiffUtility.GetDiffState(gridContext.CurrentItem);
            var fixedCount = (int)gridContext.GetValue(TableView.FixedColumnCountProperty);
            var prevState = (int)DiffState.Unchanged;
            var count = (double)0;
            var list = new List<double>();
            var states = new List<int>();
            var fields = DiffUtility.GetDiffFields(gridContext.CurrentItem);
            var totalWidth = (double)0;
            var fixedSize = (double)0;

            foreach (var item in gridContext.VisibleColumns)
            {
                var rowState = (int)diffState;
                if (rowState == (int)DiffState.Modified)
                    rowState = fields.Contains(item.FieldName) == true ? (int)DiffState.Modified : (int)DiffState.Unchanged;

                if (gridContext.CurrentItem is IDataErrorInfo errorInfo && errorInfo[item.FieldName] != string.Empty)
                    rowState |= (1 << 16);


                if (item.Index < fixedCount)
                    fixedSize += item.ActualWidth;
                if (rowState != prevState)
                {
                    if (count != 0)
                    {
                        list.Add(count);
                        states.Add(prevState);
                    }
                    count = 0;
                    prevState = rowState;
                }
                else
                {

                }
                count += item.ActualWidth;
                totalWidth += item.ActualWidth;
            }

            list.Add(count);
            states.Add(prevState);

            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var state = states[i] & 0x0000ffff;
                var error = (states[i] & 0xffff0000) >> 16;
                var brush = DiffBrushes.GetBackgroundBrush((DiffState)state);
                var border = new Border() { BorderThickness = new Thickness(0, error != 0 ? 4 : 0, 0, 0), BorderBrush = Brushes.Red, Background = brush, };
                Grid.SetColumn(border, grid.ColumnDefinitions.Count);
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(item / totalWidth, GridUnitType.Star), });
                grid.Children.Add(border);
            }
            grid.Visibility = Visibility.Visible;
        }
    }
}
