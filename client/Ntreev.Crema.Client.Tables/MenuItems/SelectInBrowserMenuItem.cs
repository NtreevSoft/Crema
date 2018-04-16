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

using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Tables.BrowserItems.ViewModels;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Tables.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Ntreev.Library;
using System.ComponentModel;

namespace Ntreev.Crema.Client.Tables.MenuItems
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(TableListItemBase))]
    [Order(int.MinValue)]
    [Category(nameof(CategoryAttribute.Default))]
    [DefaultMenu]
    class SelectInBrowserMenuItem : MenuItemBase
    {
        [Import]
        private TableBrowserViewModel browser = null;
        [Import]
        private IShell shell = null;
        [Import]
        private TableServiceViewModel service = null;

        public SelectInBrowserMenuItem()
        {
            this.DisplayName = Resources.MenuItem_SelectInTableBrowser;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return base.OnCanExecute(parameter);
        }

        protected async override void OnExecute(object parameter)
        {
            if (parameter is ITableDescriptor descriptor && this.browser != null && this.shell != null && this.service != null)
            {
                await this.Dispatcher.InvokeAsync(() => this.shell.SelectedService = this.service);
                await this.Dispatcher.InvokeAsync(() => this.browser.Select(descriptor));
            }
        }
    }
}
