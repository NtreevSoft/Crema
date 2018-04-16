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

using Ntreev.Crema.Client.Base.MenuItems;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Users.Dialogs.ViewModels;
using Ntreev.Crema.Client.Users.Properties;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.Crema.Client.Users.MenuItems.FileMenus
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(FileMenuItem))]
    class ChangePasswordMenuItem : MenuItemBase
    {
        private readonly ICremaAppHost cremaAppHost;
        [Import]
        private Authenticator authenticator = null;

        [ImportingConstructor]
        public ChangePasswordMenuItem(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Opened += this.InvokeCanExecuteChangedEvent;
            this.cremaAppHost.Closed += this.InvokeCanExecuteChangedEvent;
            this.DisplayName = Resources.MenuItem_ChangePassword;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return this.cremaAppHost.IsOpened;
        }

        protected async override void OnExecute(object parameter)
        {
            var dialog = await ChangePasswordViewModel.CreateInstanceAsync(this.authenticator, this.cremaAppHost, this.authenticator.ID);
            dialog?.ShowDialog();
        }
    }
}
