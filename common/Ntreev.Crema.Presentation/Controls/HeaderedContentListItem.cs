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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ntreev.Crema.Presentation.Controls
{
    public class HeaderedContentListItem : ListBoxItem
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(HeaderedContentListItem),
                new FrameworkPropertyMetadata(null, HeaderPropertyChangedCallback));

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(HeaderedContentListItem),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty HeaderTemplateSelectorProperty =
            DependencyProperty.Register(nameof(HeaderTemplateSelector), typeof(DataTemplateSelector), typeof(HeaderedContentListItem),
                new FrameworkPropertyMetadata(null));

        public object Header
        {
            get { return (object)this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)this.GetValue(HeaderTemplateProperty); }
            set { this.SetValue(HeaderTemplateProperty, value); }
        }

        public DataTemplateSelector HeaderTemplateSelector
        {
            get { return (DataTemplateSelector)this.GetValue(HeaderTemplateSelectorProperty); }
            set { this.SetValue(HeaderTemplateSelectorProperty, value); }
        }

        private static void HeaderPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement fe && fe.Parent is FrameworkElement parent)
            {
                parent.InvalidateMeasure();
            }
        }
    }
}
