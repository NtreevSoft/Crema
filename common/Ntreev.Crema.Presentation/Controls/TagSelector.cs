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
using Ntreev.Library;
using System;
using System.Collections.Generic;
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
    public class TagSelector : ButtonBase
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(TagInfo?), typeof(TagSelector),
                new UIPropertyMetadata(TagInfo.All, ValuePropertyChangedCallback, ValuePropertyCoerceValueCallback));

        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register(nameof(Filter), typeof(TagInfo), typeof(TagSelector),
                new UIPropertyMetadata(TagInfo.All, FilterPropertyChangedCallback));

        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register(nameof(IsPopupOpen), typeof(bool), typeof(TagSelector),
                new UIPropertyMetadata(false, IsPopupOpenPropertyChangedCallback));

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(TagSelector),
                new UIPropertyMetadata(false));

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TagSelector));

        private System.Windows.Controls.ContextMenu popup;

        static TagSelector()
        {

        }

        public TagSelector()
        {

        }

        public TagInfo? Value
        {
            get { return (TagInfo?)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        public TagInfo Filter
        {
            get { return (TagInfo)this.GetValue(FilterProperty); }
            set { this.SetValue(FilterProperty, value); }
        }

        public bool IsPopupOpen
        {
            get { return (bool)this.GetValue(IsPopupOpenProperty); }
            set { this.SetValue(IsPopupOpenProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)this.GetValue(IsReadOnlyProperty); }
            set { this.SetValue(IsReadOnlyProperty, value); }
        }


        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public event EventHandler PopupOpened;

        public event EventHandler PopupClosed;

        protected virtual void OnPopupOpened(EventArgs e)
        {
            this.PopupOpened?.Invoke(this, e);
        }

        protected virtual void OnPopupClosed(EventArgs e)
        {
            this.PopupClosed?.Invoke(this, e);
        }

        protected override void OnClick()
        {
            base.OnClick();

            if (this.popup != null)
            {
                this.popup.IsOpen = false;
            }
            else if (this.IsReadOnly == false)
            {
                var popup = new System.Windows.Controls.ContextMenu();
                var brushConverter = new BrushConverter();

                foreach (var item in TagInfoUtility.Names)
                {
                    var menuItem = new System.Windows.Controls.MenuItem() { Header = item, };
                    popup.Items.Add(menuItem);

                    var value = new TagInfo(item);
                    if (value.Color != null)
                        menuItem.Background = (Brush)brushConverter.ConvertFrom(value.Color);

                    menuItem.IsEnabled = (this.Filter & value) == value;
                    menuItem.IsChecked = this.Value == value;
                    menuItem.Click += MenuItem_Click;
                }

                popup.Closed += Popup_Closed;
                popup.PlacementTarget = this;
                popup.Placement = PlacementMode.Right;
                popup.IsOpen = true;
                this.popup = popup;
            }
        }

        private static object ValuePropertyCoerceValueCallback(DependencyObject d, object value)
        {
            if (value == null)
                return value;
            var tag = (TagInfo)value;
            var filter = (TagInfo)d.GetValue(TagSelector.FilterProperty);
            return tag & filter;
        }

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            }
        }

        private static void FilterPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(ValueProperty, d.GetValue(ValueProperty));
        }

        private static void IsPopupOpenPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tagSelector = d as TagSelector;
            if ((bool)e.NewValue == true)
            {
                tagSelector.OnPopupOpened(EventArgs.Empty);
            }
            else
            {
                if (tagSelector.popup != null)
                {
                    tagSelector.popup.IsOpen = false;
                }
            }
        }

        private void Selector_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Popup_Closed(object sender, RoutedEventArgs e)
        {
            this.popup.Closed -= Popup_Closed;
            this.popup = null;
            this.OnPopupClosed(EventArgs.Empty);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.IsChecked == false)
            {
                this.Value = new TagInfo(menuItem.Header as string);
            }
            //var item = sender as System.Windows.Controls.MenuItem;
            //if (item.IsChecked == true)
            //    return;
        }
    }
}
