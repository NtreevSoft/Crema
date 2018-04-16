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

using Caliburn.Micro;
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ntreev.Crema.Presentation.Controls
{
    class BrowserItemsControlItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var viewType = ViewLocator.LocateTypeForModelType(item.GetType(), null, null);
            if (viewType != null)
            {
                if (viewType.IsSubclassOf(typeof(BrowserExpander)) == true)
                {
                    var dataTemplate = new DataTemplate();
                    var element = new FrameworkElementFactory(viewType);
                    element.SetBinding(BrowserExpander.HeaderProperty, new Binding("DisplayName"));
                    element.SetBinding(BrowserExpander.IsExpandedProperty, new Binding()
                    {
                        Path = new PropertyPath(BrowserItemsControl.IsExpandedProperty),
                        Source = container,
                        Mode = BindingMode.TwoWay,
                    });
                    element.SetBinding(Bind.ModelProperty, new Binding());
                    element.AddHandler(BrowserExpander.ExpandedEvent, new RoutedEventHandler(BrowserExpander_RoutedEventHandler));
                    element.AddHandler(BrowserExpander.CollapsedEvent, new RoutedEventHandler(BrowserExpander_RoutedEventHandler));
                    dataTemplate.VisualTree = element;
                    return dataTemplate;
                }
                else
                {
                    var dataTemplate = new DataTemplate();
                    var element = new FrameworkElementFactory(typeof(BrowserExpander));
                    element.SetBinding(BrowserExpander.HeaderProperty, new Binding("DisplayName"));
                    element.SetBinding(BrowserExpander.IsExpandedProperty, new Binding()
                    {
                        Path = new PropertyPath(BrowserItemsControl.IsExpandedProperty),
                        Source = container,
                        Mode = BindingMode.TwoWay,
                    });
                    element.AddHandler(BrowserExpander.ExpandedEvent, new RoutedEventHandler(BrowserExpander_RoutedEventHandler));
                    element.AddHandler(BrowserExpander.CollapsedEvent, new RoutedEventHandler(BrowserExpander_RoutedEventHandler));
                    var contentElement = new FrameworkElementFactory(viewType);
                    contentElement.SetBinding(Bind.ModelProperty, new Binding());
                    element.AppendChild(contentElement);
                    dataTemplate.VisualTree = element;
                    return dataTemplate;
                }
            }

            return base.SelectTemplate(item, container);

            void BrowserExpander_RoutedEventHandler(object sender, RoutedEventArgs e)
            {
                if (sender is BrowserExpander expander)
                    BrowserItemsControl.SetIsExpanded(container, expander.IsExpanded);
            }
        }
    }
}