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

using Ntreev.Crema.Client.Base.Dialogs.ViewModels;
using Ntreev.Crema.Client.Base.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ntreev.Crema.Client.Base.Dialogs.Views
{
    /// <summary>
    /// ConnectionItemEditView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ConnectionItemEditView : UserControl
    {
        private readonly static DependencyProperty PasswordProperty =
            DependencyProperty.Register(nameof(ConnectionItemViewModel.Password), typeof(string), typeof(ConnectionItemEditView),
                new PropertyMetadata(string.Empty, PasswordPropertyChangedCallback));

        public ConnectionItemEditView()
        {
            InitializeComponent();

            BindingOperations.SetBinding(this, PasswordProperty, new Binding($"{nameof(ConnectionItemEditViewModel.ConnectionInfo)}.{nameof(ConnectionItemViewModel.Password)}")
            {
                Mode = BindingMode.TwoWay,
            });
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.SetValue(PasswordProperty, this.passwordBox.Password);
        }

        private static void PasswordPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ConnectionItemEditView view && e.NewValue is string password)
            {
                if (view.passwordBox.Password != password)
                {
                    view.passwordBox.Password = password;
                }
            }
        }
    }
}
