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

using Ntreev.Crema.Client.Base.Controls;
using Ntreev.Crema.Client.Base.Services.ViewModels;
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Ntreev.Crema.Client.Base.Services.Views
{
    partial class CremaAppHostView : UserControl
    {
        [Import]
        private Lazy<CremaAppHostViewModel> cremaAppHost = null;

        public CremaAppHostView()
        {
            this.InitializeComponent();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (this.filterBox.IsKeyboardFocusWithin == true)
            {
                if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    if (e.Key == Key.Escape)
                    {
                        this.filterBox.Text = string.Empty;
                        this.serverList.Focus();
                        e.Handled = true;
                    }
                    else if (e.Key == Key.Enter)
                    {
                        if (this.filterBox.GetBindingExpression(FilterBox.TextProperty) is BindingExpression be)
                        {
                            be.UpdateSource();
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var cremaAppHost = this.cremaAppHost.Value;
            cremaAppHost.PropertyChanged += CremaAppHost_PropertyChanged;
            this.Dispatcher.InvokeAsync(() =>
            {
                if (this.serverList.SelectedItem != null)
                {
                    if (this.serverList.ItemContainerGenerator.ContainerFromItem(this.serverList.SelectedItem) is ListBoxItem container)
                    {
                        container.BringIntoView();
                    }
                }
            }, DispatcherPriority.Background);
        }

        private void CremaAppHost_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //var cremaAppHost = this.cremaAppHost.Value;
            //if (e.PropertyName == nameof(cremaAppHost.Password) == true)
            //{
            //    this.SetPassword(cremaAppHost.Password);
            //}
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var cremaAppHost = this.cremaAppHost.Value;
            cremaAppHost.SetPassword(passwordBox.Password, false);
        }

        private void SetPassword(string password)
        {
            //this.passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            //this.passwordBox.Password = password;
            //this.passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }
    }
}
