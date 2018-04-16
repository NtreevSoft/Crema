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
using Ntreev.Crema.Client.Base.Properties;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Client.Base.Services.ViewModels;

namespace Ntreev.Crema.Client.Base.ToolBarItems
{
    [Export(typeof(IToolBarItem))]
    [ParentType(typeof(DataBaseListViewModel))]
    class CreateDataBaseToolBarItem : ToolBarItemBase
    {
        private readonly ICremaAppHost cremaAppHost;
        [Import]
        private Authenticator authenticator = null;
        [Import]
        private IServiceProvider serviceProvider = null;

        [ImportingConstructor]
        public CreateDataBaseToolBarItem(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Opened += this.InvokeCanExecuteChangedEvent;
            this.cremaAppHost.Closed += this.InvokeCanExecuteChangedEvent;
            this.Icon = "/Ntreev.Crema.Client.Base;component/Images/add.png";
            this.DisplayName = Resources.MenuItem_CreateDataBase;
            this.HideOnDisabled = true;
        }

        protected override bool OnCanExecute(object parameter)
        {
            if (this.cremaAppHost.IsOpened == true && parameter is DataBaseListViewModel listViewModel)
            {
                return DataBaseUtility.CanCreate(this.authenticator);
            }
            return false;
        }

        protected async override void OnExecute(object parameter)
        {
            if (this.cremaAppHost.IsOpened == true && parameter is DataBaseListViewModel listViewModel)
            {
                await DataBaseUtility.CreateAsync(this.authenticator, this.serviceProvider);
            }
        }
    }
}
