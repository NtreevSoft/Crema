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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Views;

namespace Ntreev.Crema.Presentation.Controls
{
    public class DiffHorzScrollBar : ScrollBar
    {
        private static readonly DependencyPropertyKey FixedSizePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(FixedSize), typeof(double), typeof(DiffHorzScrollBar),
                new UIPropertyMetadata((double)0));
        public static readonly DependencyProperty FixedSizeProperty = FixedSizePropertyKey.DependencyProperty;

        public static DependencyProperty GridControlProperty =
            DependencyProperty.Register(nameof(GridControl), typeof(DiffDataGridControl), typeof(DiffHorzScrollBar),
                new FrameworkPropertyMetadata(null, GridControlPropertyChangedCallback));

        public DiffHorzScrollBar()
        {
            this.CommandBindings.Add(new CommandBinding(DiffScrollBarCommands.ScrollToPoint, ScrollToPoint_Executed));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public double FixedSize
        {
            get { return (double)this.GetValue(FixedSizeProperty); }
        }

        public DiffDataGridControl GridControl
        {
            get { return (DiffDataGridControl)this.GetValue(GridControlProperty); }
            set { this.SetValue(GridControlProperty, value); }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var size = base.MeasureOverride(constraint);
            this.RefreshFixedSize();
            return size;
        }

        private void ScrollToPoint_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var point = Mouse.GetPosition(this);
            var value = this.Track.ValueFromPoint(point);
            this.Value = value;
            this.RaiseEvent(new ScrollEventArgs(ScrollEventType.ThumbPosition, value));
        }

        private static void GridControlPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiffHorzScrollBar self)
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
                if (e.PropertyName == TableView.FixedColumnCountProperty.Name || e.PropertyName == nameof(this.ActualWidth))
                {
                    this.InvalidateMeasure();
                }
            }
        }

        private void RefreshFixedSize()
        {
            var totalWidth = (double)0;
            var fixedSize = (double)0;
            var index = 0;

            if (this.GridControl != null)
            {
                var gridContext = DataGridControl.GetDataGridContext(this.GridControl);
                var fixedCount = (int)gridContext.GetValue(TableView.FixedColumnCountProperty);

                foreach (var item in this.GridControl.VisibleColumns)
                {
                    if (index < fixedCount)
                        fixedSize += item.ActualWidth;

                    totalWidth += item.ActualWidth;
                    index++;
                }
            }

            fixedSize = Math.Floor(this.ActualWidth * fixedSize / totalWidth);
            this.SetValue(FixedSizePropertyKey, fixedSize);
        }
    }
}
