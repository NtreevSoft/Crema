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
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Base.MenuItems
{
    [Export(typeof(IMenuItem))]
    [Category("wer")]
    [ParentType(typeof(FileMenuItem))]
    class LogoutMenuItem : MenuItemBase
    {
        private readonly CremaAppHostViewModel cremaAppHost;

        [ImportingConstructor]
        public LogoutMenuItem(CremaAppHostViewModel cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.PropertyChanged += CremaAppHost_PropertyChanged;
            this.DisplayName = Resources.MenuItem_Logout;
        }

        protected override void OnExecute(object parameter)
        {
            if (AppMessageBox.ShowQuestion(Resources.Message_Logout) == true)
            {
                this.cremaAppHost.Logout();
            }
        }

        protected override bool OnCanExecute(object parameter)
        {
            return this.cremaAppHost.IsOpened == true && this.cremaAppHost.IsProgressing == false;
        }

        private void CremaAppHost_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CremaAppHostViewModel.IsProgressing) || e.PropertyName == string.Empty)
            {
                this.Refresh();
            }
        }
    }
}
