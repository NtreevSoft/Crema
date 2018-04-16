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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ntreev.Crema.Client.Framework.Controls
{
    public class PasswordValidator : FrameworkElement
    {
        public static readonly DependencyProperty Box1Property =
            DependencyProperty.Register(nameof(Box1), typeof(PasswordBox), typeof(PasswordValidator),
                new PropertyMetadata(Box1PropertyChangedCallback));

        public static readonly DependencyProperty Box2Property =
            DependencyProperty.Register(nameof(Box2), typeof(PasswordBox), typeof(PasswordValidator),
                new PropertyMetadata(Box2PropertyChangedCallback));

        private static Dictionary<PasswordBox, Brush> passwordBoxToBackground = new Dictionary<PasswordBox, Brush>();
        private static Dictionary<PasswordBox, Brush> passwordBoxToForeground = new Dictionary<PasswordBox, Brush>();

        public PasswordBox Box1
        {
            get { return (PasswordBox)this.GetValue(Box1Property); }
            set { this.SetValue(Box1Property, value); }
        }

        public PasswordBox Box2
        {
            get { return (PasswordBox)this.GetValue(Box2Property); }
            set { this.SetValue(Box2Property, value); }
        }

        private static void Box1PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pv = d as PasswordValidator;

            if (passwordBoxToBackground.ContainsKey(pv.Box1) == false)
            {
                passwordBoxToBackground[pv.Box1] = pv.Box1.Background;
                passwordBoxToForeground[pv.Box1] = pv.Box1.Foreground;
            }

            pv.Box1.KeyUp += (obj, evt) =>
            {
                RefreshPasswordBoxes(pv.Box1, pv.Box2);
            };
        }

        private static void Box2PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pv = d as PasswordValidator;

            if (passwordBoxToBackground.ContainsKey(pv.Box2) == false)
            {
                passwordBoxToBackground[pv.Box2] = pv.Box2.Background;
                passwordBoxToForeground[pv.Box2] = pv.Box2.Foreground;
            }

            pv.Box2.KeyUp += (obj, evt) =>
            {
                RefreshPasswordBoxes(pv.Box1, pv.Box2);
            };
        }

        private static void RefreshPasswordBoxes(PasswordBox box1, PasswordBox box2)
        {
            if (box1.Password != box2.Password)
            {
                box1.Background = new SolidColorBrush(Colors.Firebrick);
                box1.Foreground = new SolidColorBrush(Colors.White);
                box2.Background = new SolidColorBrush(Colors.Firebrick);
                box2.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                box1.Background = passwordBoxToBackground[box1];
                box1.Foreground = passwordBoxToForeground[box1];
                box2.Background = passwordBoxToBackground[box2];
                box2.Foreground = passwordBoxToForeground[box2];
            }
        }
    }
}
