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

using Ntreev.Crema.ApplicationHost.Properties;
using Ntreev.Crema.ApplicationHost.Views;
using Ntreev.Crema.Client.Framework;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Ntreev.Crema.ApplicationHost.MenuItems.ViewModels
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(IToolMenuItem))]
    class LogViewMenuItemViewModel : MenuItemBase, IPartImportsSatisfiedNotification
    {
        [Import]
        private Lazy<ShellView> shellView = null;

        public LogViewMenuItemViewModel()
        {
            this.InputGesture = new KeyGesture(Key.F12, ModifierKeys.Shift);
        }

        protected override void OnExecute(object parameter)
        {
            this.shellView.Value.IsLogVisible = !this.shellView.Value.IsLogVisible;
        }

        private void RefreshDisplayName()
        {
            if (this.shellView.Value.IsLogVisible == true)
            {
                this.DisplayName = Resources.MenuItem_HideLogWindow;
            }
            else
            {
                this.DisplayName = Resources.MenuItem_ShowLogWindow;
            }
        }

        private void ShellView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.shellView.Value.IsLogVisible))
            {
                this.RefreshDisplayName();
            }
        }

        async void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.shellView.Value.PropertyChanged += ShellView_PropertyChanged;
                this.RefreshDisplayName();
            }, DispatcherPriority.ContextIdle);
        }
    }
}
