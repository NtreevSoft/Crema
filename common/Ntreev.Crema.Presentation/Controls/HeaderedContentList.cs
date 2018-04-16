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
using System.Windows.Input;
using System.Windows.Media;

namespace Ntreev.Crema.Presentation.Controls
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(HeaderedContentListItem))]
    public class HeaderedContentList : ListBox
    {
        public static readonly DependencyProperty HeaderWidthProperty =
            DependencyProperty.Register(nameof(HeaderWidth), typeof(GridLength), typeof(HeaderedContentList),
                new FrameworkPropertyMetadata(GridLength.Auto, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty HeaderMinWidthProperty =
            DependencyProperty.Register(nameof(HeaderMinWidth), typeof(double), typeof(HeaderedContentList),
                new FrameworkPropertyMetadata(20.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        private static readonly DependencyPropertyKey HeaderActualWidthPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(HeaderActualWidth), typeof(double), typeof(HeaderedContentList),
                new PropertyMetadata(double.NaN));

        static HeaderedContentList()
        {
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(HeaderedContentList), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
        }

        public HeaderedContentList()
        {

        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is HeaderedContentListItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new HeaderedContentListItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }

        public double HeaderActualWidth
        {
            get { return (double)GetValue(HeaderActualWidthPropertyKey.DependencyProperty); }
        }

        [TypeConverter(typeof(GridLengthConverter))]
        public GridLength HeaderWidth
        {
            get { return (GridLength)this.GetValue(HeaderWidthProperty); }
            set { this.SetValue(HeaderWidthProperty, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double HeaderMinWidth
        {
            get { return (double)this.GetValue(HeaderMinWidthProperty); }
            set { this.SetValue(HeaderMinWidthProperty, value); }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (this.HeaderWidth.IsAuto == true)
            {
                var width = this.HeaderMinWidth;
                for (var i = 0; i < this.Items.Count; i++)
                {
                    if (this.ItemContainerGenerator.ContainerFromIndex(i) is DependencyObject d)
                    {
                        var headerPresenter = this.FindHeaderPresenter(d);
                        if (headerPresenter != null && headerPresenter.Content != null)
                        {
                            headerPresenter.Measure(constraint);
                            width = Math.Max(width, headerPresenter.DesiredSize.Width);
                        }
                    }
                }
                this.SetValue(HeaderActualWidthPropertyKey, width);
            }
            else if (this.HeaderWidth.IsStar == true)
            {
                var width = Math.Max(this.HeaderMinWidth, constraint.Width * this.HeaderWidth.Value);
                this.SetValue(HeaderActualWidthPropertyKey, Math.Max(this.HeaderMinWidth, width));
            }
            else
            {
                this.SetValue(HeaderActualWidthPropertyKey, Math.Max(this.HeaderMinWidth, this.HeaderWidth.Value));
            }

            return base.MeasureOverride(constraint);
        }

        private ContentPresenter FindHeaderPresenter(DependencyObject d)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);
                if (child is ContentPresenter contentPresenter && contentPresenter.ContentSource == nameof(HeaderedContentControl.Header))
                    return contentPresenter;
                var result = this.FindHeaderPresenter(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
