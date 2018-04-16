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
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Crema.Client.Base.Properties;

namespace Ntreev.Crema.Client.Base.MenuItems
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(DataBaseMenuItem))]
    class CopyDataBaseMenuItem : MenuItemBase
    {
        private readonly ICremaAppHost cremaAppHost;
        [Import]
        private Authenticator authenticator = null;

        [ImportingConstructor]
        public CopyDataBaseMenuItem(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Opened += this.InvokeCanExecuteChangedEvent;
            this.cremaAppHost.Closed += this.InvokeCanExecuteChangedEvent;
            this.DisplayName = Resources.MenuItem_CopyDataBase;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return this.cremaAppHost.IsOpened == true && this.authenticator.Authority == Authority.Admin;
        }

        protected async override void OnExecute(object parameter)
        {
            var dialog = new SelectDataBaseViewModel(this.authenticator, this.cremaAppHost);
            if (dialog.ShowDialog() != true)
                return;

            await DataBaseUtility.CopyAsync(this.authenticator, dialog.SelectedItem);
        }
    }
}
