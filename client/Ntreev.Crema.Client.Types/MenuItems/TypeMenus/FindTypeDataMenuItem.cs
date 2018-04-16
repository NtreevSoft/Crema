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
using Ntreev.Crema.Client.Types.Properties;
using Ntreev.Library.IO;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Ntreev.Crema.Client.Types.MenuItems.TypeMenus
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(TypeMenuItem))]
    class FindTypeDataMenuItem : MenuItemBase
    {
        private readonly IShell shell;

        [Import]
        private TypeServiceViewModel typeService = null;
        [Import]
        private Authenticator authenticator = null;

        [ImportingConstructor]
        public FindTypeDataMenuItem(IShell shell)
        {
            this.shell = shell;
            this.shell.ServiceChanged += Shell_ServiceChanged;
            this.InputGesture = new KeyGesture(Key.F, ModifierKeys.Control | ModifierKeys.Shift);
            this.DisplayName = Resources.MenuItem_FindAll;
            this.HideOnDisabled = true;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return this.shell.SelectedService == this.typeService;
        }

        protected override void OnExecute(object parameter)
        {
            this.typeService.DocumentService.AddFinder(this.authenticator, PathUtility.Separator);
        }

        private void Shell_ServiceChanged(object sender, EventArgs e)
        {
            this.InvokeCanExecuteChangedEvent();
        }
    }
}
