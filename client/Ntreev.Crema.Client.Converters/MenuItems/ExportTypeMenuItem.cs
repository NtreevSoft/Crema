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

using System.ComponentModel.Composition;
using Ntreev.Crema.Client.Converters.Dialogs.ViewModels;
using Ntreev.Crema.Client.Converters.Properties;
using Ntreev.Crema.Client.Framework;
using Ntreev.ModernUI.Framework;

namespace Ntreev.Crema.Client.Converters.MenuItems
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(IToolMenuItem))]
    class ExportTypeMenuItem : MenuItemBase
    {
        [Import] private Authenticator authenticator = null;
        private ICremaAppHost cremaAppHost;

        [ImportingConstructor]
        public ExportTypeMenuItem(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Loaded += InvokeCanExecuteChangedEvent;
            this.cremaAppHost.Unloaded += InvokeCanExecuteChangedEvent;
            this.DisplayName = Resources.MenuItem_ExportType;
        }

        protected override async void OnExecute(object parameter)
        {
            var dialog = await ExportTypeViewModel.CreateInstanceAsync(this.authenticator, this.cremaAppHost);
            dialog?.ShowDialog();
        }

        protected override bool OnCanExecute(object parameter)
        {
            return this.cremaAppHost.IsLoaded;
        }
    }
}
