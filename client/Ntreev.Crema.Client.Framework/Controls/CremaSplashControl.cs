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
using System.Windows.Media;

namespace Ntreev.Crema.Client.Framework.Controls
{
    public class CremaSplashControl : Control
    {
        private static Color DefaultColor = Color.FromArgb(0xff, 0x1b, 0xa1, 0xe2);

        private static DependencyPropertyKey ThemePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Theme), typeof(Brush), typeof(CremaSplashControl),
                new PropertyMetadata(new SolidColorBrush(DefaultColor)));
        public static DependencyProperty ThemeProperty = ThemePropertyKey.DependencyProperty;

        public static DependencyProperty ThemeColorProperty =
            DependencyProperty.Register(nameof(ThemeColor), typeof(Color), typeof(CremaSplashControl),
                new PropertyMetadata(DefaultColor, ThemeColorPropertyChangedCallback));

        public static DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(CremaSplashControl),
                new PropertyMetadata("Crema"));

        public CremaSplashControl()
        {

        }

        public Color ThemeColor
        {
            get { return (Color)this.GetValue(ThemeColorProperty); }
            set { this.SetValue(ThemeColorProperty, value); }
        }

        public Brush Theme
        {
            get { return (Brush)this.GetValue(ThemeProperty); }
        }

        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        private static void ThemeColorPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(ThemePropertyKey, new SolidColorBrush((Color)e.NewValue));
        }
    }
}
