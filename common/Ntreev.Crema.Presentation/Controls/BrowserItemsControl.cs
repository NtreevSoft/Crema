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
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ntreev.Crema.Presentation.Controls
{
    public class BrowserItemsControl : ItemsControl
    {
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.RegisterAttached("IsExpanded", typeof(bool), typeof(BrowserItemsControl),
                new UIPropertyMetadata(true));

        public BrowserItemsControl()
        {

        }

        public static bool GetIsExpanded(DependencyObject d)
        {
            return (bool)d.GetValue(IsExpandedProperty);
        }

        public static void SetIsExpanded(DependencyObject d, bool value)
        {
            d.SetValue(IsExpandedProperty, value);
        }

        public void SaveSettings(IAppConfiguration configs)
        {
            foreach (var item in this.Items)
            {
                if (this.ItemContainerGenerator.ContainerFromItem(item) is DependencyObject container)
                {
                    var dataContext = container.GetValue(DataContextProperty);
                    if (dataContext != null)
                    {
                        configs.SetValue(typeof(BrowserItemsControl), dataContext.GetType(), nameof(BrowserExpander.IsExpanded), GetIsExpanded(container));
                    }
                }
            }
        }

        public void LoadSettings(IAppConfiguration configs)
        {
            foreach (var item in this.Items)
            {
                if (this.ItemContainerGenerator.ContainerFromItem(item) is DependencyObject container)
                {
                    var dataContext = container.GetValue(DataContextProperty);
                    if (dataContext != null && configs.TryGetValue<bool>(typeof(BrowserItemsControl), dataContext.GetType(), nameof(BrowserExpander.IsExpanded), out var isExpanded) == true)
                    {
                        SetIsExpanded(container, isExpanded);
                    }
                }
            }
        }
    }
}
