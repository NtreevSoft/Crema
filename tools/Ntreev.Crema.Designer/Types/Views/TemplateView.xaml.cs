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
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.DataGrid.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

namespace Ntreev.Crema.Designer.Types.Views
{
    /// <summary>
    /// TemplateView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TemplateView : UserControl
    {
        [Import]
        private IAppConfiguration configs = null;

        public TemplateView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.TypeName.Focus();
        }

        private void TypeName_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                this.Change.IsEnabled = false;
            }
            else if (e.Action == ValidationErrorEventAction.Removed)
            {
                this.Change.IsEnabled = true;
            }
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void PART_DataGridControl_Loaded(object sender, RoutedEventArgs e)
        {
            //var gridControl = sender as ModernDataGridControl;
            //this.configs.Update(this);
            //if (this.Settings != null)
            //    gridControl.LoadUserSettings(this.Settings, Xceed.Wpf.DataGrid.Settings.UserSettings.All);
        }

        private void PART_DataGridControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //var gridControl = sender as ModernDataGridControl;
            //this.Settings = new Xceed.Wpf.DataGrid.Settings.SettingsRepository();
            //gridControl.SaveUserSettings(this.Settings, Xceed.Wpf.DataGrid.Settings.UserSettings.All);
            //this.configs.Commit(this);
        }
    }
}
