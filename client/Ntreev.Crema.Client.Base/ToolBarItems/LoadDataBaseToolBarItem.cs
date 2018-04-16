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

using Ntreev.Crema.Client.Base.Properties;
using Ntreev.Crema.Client.Base.Services.ViewModels;
using Ntreev.Crema.Client.Framework;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Base.ToolBarItems
{
    [Export(typeof(IToolBarItem))]
    [ParentType(typeof(DataBaseListViewModel))]
    class LoadDataBaseToolBarItem : ToolBarItemBase
    {
        private readonly ICremaAppHost cremaAppHost;
        [Import]
        private Authenticator authenticator = null;
        private bool isLoaded;

        [ImportingConstructor]
        public LoadDataBaseToolBarItem(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Opened += this.InvokeCanExecuteChangedEvent;
            this.cremaAppHost.Loaded += this.InvokeCanExecuteChangedEvent;
            this.cremaAppHost.Unloaded += this.InvokeCanExecuteChangedEvent;
            this.cremaAppHost.Closed += this.InvokeCanExecuteChangedEvent;
            this.DisplayName = Resources.MenuItem_Load;
            this.Icon = typeof(LoadDataBaseIcon);
            this.HideOnDisabled = true;
        }

        public bool IsLoaded
        {
            get => this.isLoaded;
            set
            {
                this.isLoaded = value;
                this.DisplayName = this.isLoaded == true ? Resources.MenuItem_Unload : Resources.MenuItem_Load;
                this.NotifyOfPropertyChange(nameof(this.IsLoaded));
            }
        }

        protected override bool OnCanExecute(object parameter)
        {
            if (this.cremaAppHost.IsOpened == true && parameter is DataBaseListViewModel listViewModel && listViewModel.SelectedItem is IDataBaseDescriptor descriptor)
            {
                this.IsLoaded = descriptor.IsLoaded;
                if (this.IsLoaded == false)
                    return DataBaseUtility.CanLoad(this.authenticator, descriptor);
                return DataBaseUtility.CanUnload(this.authenticator, descriptor);
            }
            return false;
        }

        protected async override void OnExecute(object parameter)
        {
            if (this.cremaAppHost.IsOpened == true && parameter is DataBaseListViewModel listViewModel && listViewModel.SelectedItem is IDataBaseDescriptor descriptor)
            {
                if (descriptor.IsLoaded == false)
                    await DataBaseUtility.LoadAsync(this.authenticator, descriptor);
                else
                    await DataBaseUtility.UnloadAsync(this.authenticator, descriptor);
                this.IsLoaded = descriptor.IsLoaded;
            }
        }
    }
}
