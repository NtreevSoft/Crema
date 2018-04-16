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

using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Tables.BrowserItems.ViewModels;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using Ntreev.ModernUI.Framework.ViewModels;
using Ntreev.Crema.Data;
using Ntreev.Library;
using System.Windows.Input;
using System.Windows.Threading;
using System.ComponentModel.Composition;

namespace Ntreev.Crema.Client.Tables.PropertyItems.ViewModels
{
    public class TableListBoxItemViewModel : TableListItemBase
    {
        private readonly ICommand selectInBrowserCommand;

        [Import]
        private TableBrowserViewModel browser = null;
        [Import]
        private IShell shell = null;
        [Import]
        private TableServiceViewModel service = null;

        public TableListBoxItemViewModel(Authentication authentication, ITable table, object owner)
            : base(authentication, table, owner)
        {
            this.selectInBrowserCommand = new DelegateCommand(async item => await this.SelectInBrowserAsync());
        }

        public TableListBoxItemViewModel(Authentication authentication, ITableDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {
            this.selectInBrowserCommand = new DelegateCommand(async item => await this.SelectInBrowserAsync());
        }

        public async Task SelectInBrowserAsync()
        {
            if (this.browser != null && this.shell != null && this.service != null)
            {
                await this.Dispatcher.InvokeAsync(() => shell.SelectedService = this.service);
                await this.Dispatcher.InvokeAsync(() => browser.Select(this.descriptor));
            }
        }

        public ICommand SelectInBrowserCommand => this.selectInBrowserCommand;
    }
}
