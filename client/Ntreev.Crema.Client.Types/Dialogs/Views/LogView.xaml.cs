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

using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Ntreev.Library;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Ntreev.Crema.Client.Types.Dialogs.Views
{
    public partial class LogView : UserControl
    {
        [Import]
        private IAppConfiguration configs = null;

        public LogView()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.configs.Update(this);
            if (this.Settings != null)
                this.gridControl.LoadUserSettings(this.Settings, Xceed.Wpf.DataGrid.Settings.UserSettings.All);
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Settings = new Xceed.Wpf.DataGrid.Settings.SettingsRepository();
            this.gridControl.SaveUserSettings(this.Settings, Xceed.Wpf.DataGrid.Settings.UserSettings.All);
            this.configs.Commit(this);
        }

        [ConfigurationProperty("settings")]
        private Xceed.Wpf.DataGrid.Settings.SettingsRepository Settings
        {
            get; set;
        }

        private void ModernDataRow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is FrameworkElement fe && fe.DataContext is ICommand command)
            {
                if (command.CanExecute(fe.DataContext) == true)
                {
                    command.Execute(fe.DataContext);
                    e.Handled = true;
                }
            }
        }
    }
}
