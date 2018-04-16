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

using Ntreev.Library.Linq;
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

namespace Ntreev.Crema.Presentation.Controls
{
    [TemplatePart(Name = "PART_Line1", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_Line2", Type = typeof(FrameworkElement))]
    public class DiffVertScrollBar : ScrollBar
    {
        public static DependencyProperty GridControl1Property =
            DependencyProperty.Register(nameof(GridControl1), typeof(DiffDataGridControl), typeof(DiffVertScrollBar),
                new FrameworkPropertyMetadata(null, GridControl1PropertyChangedCallback));

        public static DependencyProperty GridControl2Property =
            DependencyProperty.Register(nameof(GridControl2), typeof(DiffDataGridControl), typeof(DiffVertScrollBar),
                new FrameworkPropertyMetadata(null, GridControl2PropertyChangedCallback));

        private static readonly DependencyPropertyKey LinePosition1PropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(LinePosition1), typeof(double), typeof(DiffVertScrollBar),
                new FrameworkPropertyMetadata(0.0, LinePosition1PropertyChangedCallback, LinePositionPropertyCoerceValueCallback));
        public static readonly DependencyProperty LinePosition1Property = LinePosition1PropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey LinePosition2PropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(LinePosition2), typeof(double), typeof(DiffVertScrollBar),
                new FrameworkPropertyMetadata(0.0, LinePosition2PropertyChangedCallback, LinePositionPropertyCoerceValueCallback));
        public static readonly DependencyProperty LinePosition2Property = LinePosition2PropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey LinePositionVisibility1PropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(LinePositionVisibility1), typeof(Visibility), typeof(DiffVertScrollBar),
                new FrameworkPropertyMetadata(Visibility.Hidden));
        public static readonly DependencyProperty LinePositionVisibility1Property = LinePositionVisibility1PropertyKey.DependencyProperty;

        public static readonly DependencyPropertyKey LinePositionVisibility2PropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(LinePositionVisibility2), typeof(Visibility), typeof(DiffVertScrollBar),
                new FrameworkPropertyMetadata(Visibility.Hidden));
        public static readonly DependencyProperty LinePositionVisibility2Property = LinePositionVisibility2PropertyKey.DependencyProperty;

        private FrameworkElement line1;
        private FrameworkElement line2;

        public DiffVertScrollBar()
        {
            this.CommandBindings.Add(new CommandBinding(DiffScrollBarCommands.ScrollToPoint, ScrollToPoint_Executed));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.line1 = this.Template.FindName("PART_Line1", this) as FrameworkElement;
            this.line2 = this.Template.FindName("PART_Line2", this) as FrameworkElement;

            if (this.line1 != null)
            {
                BindingOperations.SetBinding(this.line1, FrameworkElement.VisibilityProperty, new Binding(nameof(this.LinePositionVisibility1)) { Source = this, });
            }

            if (this.line2 != null)
            {
                BindingOperations.SetBinding(this.line2, FrameworkElement.VisibilityProperty, new Binding(nameof(this.LinePositionVisibility2)) { Source = this, });
            }
        }

        public DiffDataGridControl GridControl1
        {
            get { return (DiffDataGridControl)this.GetValue(GridControl1Property); }
            set { this.SetValue(GridControl1Property, value); }
        }

        public DiffDataGridControl GridControl2
        {
            get { return (DiffDataGridControl)this.GetValue(GridControl2Property); }
            set { this.SetValue(GridControl2Property, value); }
        }

        public double LinePosition1
        {
            get { return (double)this.GetValue(LinePosition1Property); }
        }

        public double LinePosition2
        {
            get { return (double)this.GetValue(LinePosition2Property); }
        }

        public Visibility LinePositionVisibility1
        {
            get { return (Visibility)this.GetValue(LinePositionVisibility1Property); }
        }

        public Visibility LinePositionVisibility2
        {
            get { return (Visibility)this.GetValue(LinePositionVisibility2Property); }
        }

        private void ScrollToPoint_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var point = Mouse.GetPosition(this);
            var value = this.Track.ValueFromPoint(point);
            this.Value = value;
            this.RaiseEvent(new ScrollEventArgs(ScrollEventType.ThumbPosition, value));
        }

        private static object LinePositionPropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            var value = (double)baseValue;
            if (value > 1.0)
                return 1.0;
            if (value < 0)
                return 0;
            return baseValue;
        }

        private static void LinePosition1PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiffVertScrollBar self)
            {
                self.SetLinePosition(self.line1, self.LinePosition1);
            }
        }

        private static void LinePosition2PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiffVertScrollBar self)
            {
                self.SetLinePosition(self.line2, self.LinePosition2);
            }
        }

        private void SetLinePosition(FrameworkElement border, double linePosition)
        {
            if (border == null)
                return;

            var thumb = this.Track.Thumb;
            var b = thumb.BorderThickness;

            if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                var m = b.Top + b.Bottom;
                var y = (int)((this.ActualHeight - (border.ActualHeight + m)) * linePosition) + b.Top;
                border.Margin = new Thickness(b.Left, y, b.Right, 0);
            }
            else
            {
                var m = b.Left + b.Right;
                var x = (int)((this.ActualWidth - (border.ActualWidth + m)) * linePosition) + b.Left;
                border.Margin = new Thickness(x, b.Top, 0, b.Bottom);
            }
        }

        private static void GridControl1PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiffVertScrollBar self)
            {
                if (e.OldValue is DiffDataGridControl oldControl)
                {
                    oldControl.PropertyChanged -= self.Control1_PropertyChanged;
                }

                if (e.NewValue is DiffDataGridControl newControl)
                {
                    newControl.PropertyChanged += self.Control1_PropertyChanged;
                }
            }
        }

        private void Control1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is DiffDataGridControl gridControl)
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    if (e.PropertyName == nameof(this.GridControl1.GlobalCurrentItem))
                    {
                        var gridContext = DataGridControl.GetDataGridContext(this.GridControl1);
                        var items = DiffItemInfo.GetVisibleItems(gridContext);
                        var index = items.IndexOf(i => i.Item == this.GridControl1.GlobalCurrentItem);
                        if (index == -1)
                        {
                            this.SetValue(LinePositionVisibility1PropertyKey, Visibility.Hidden);
                        }
                        else
                        {
                            var s = (1.0 / (double)(items.Count()));
                            this.SetValue(LinePositionVisibility1PropertyKey, Visibility.Visible);
                            this.SetValue(LinePosition1PropertyKey, s * index + s * 0.5);
                        }
                    }
                });
            }
        }

        private static void GridControl2PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiffVertScrollBar self)
            {
                if (e.OldValue is DiffDataGridControl oldControl)
                {
                    oldControl.PropertyChanged -= self.Control2_PropertyChanged;
                }

                if (e.NewValue is DiffDataGridControl newControl)
                {
                    newControl.PropertyChanged += self.Control2_PropertyChanged;
                }
            }
        }

        private void Control2_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is DiffDataGridControl gridControl)
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    if (e.PropertyName == nameof(this.GridControl2.GlobalCurrentItem))
                    {
                        var gridContext = DataGridControl.GetDataGridContext(this.GridControl2);
                        var items = DiffItemInfo.GetVisibleItems(gridContext);
                        var index = items.IndexOf(i => i.Item == this.GridControl2.GlobalCurrentItem);
                        if (index == -1)
                        {
                            this.SetValue(LinePositionVisibility2PropertyKey, Visibility.Hidden);
                        }
                        else
                        {
                            var s = (1.0 / (double)(items.Count()));
                            this.SetValue(LinePositionVisibility2PropertyKey, Visibility.Visible);
                            this.SetValue(LinePosition2PropertyKey, s * index + s * 0.5);
                        }
                    }
                });
            }
        }
    }
}
