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

using Ntreev.Library;
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ntreev.Crema.Client.Users.Dialogs.Views
{
    partial class NewUserView : UserControl
    {
        private static DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(SecureString), typeof(NewUserView));

        private BindingExpressionBase passwordBinding;

        public NewUserView()
        {
            this.InitializeComponent();

            this.passwordBinding = BindingOperations.SetBinding(this, PasswordProperty, new Binding("Password")
            {
                Mode = BindingMode.OneWayToSource,
                UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
            });
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ID.Focus();
        }

        private void ID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.ID.Text != string.Empty && IdentifierValidator.Verify(this.ID.Text) == false)
            {
                //this.blinker.HasError = true;
            }
        }

        private SecureString Password
        {
            get { return (SecureString)this.GetValue(PasswordProperty); }
        }

        private void PasswordBox1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.UpdatePassword();
        }

        private void PasswordBox2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.UpdatePassword();
        }

        private async void UpdatePassword()
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                var isValid = PasswordBoxUtility.GetIsValid(this.passwordBox1);
                if (isValid == true)
                    this.SetValue(PasswordProperty, this.passwordBox1.SecurePassword);
                else
                    this.SetValue(PasswordProperty, null);
                this.passwordBinding.UpdateSource();
            });
        }
    }
}
