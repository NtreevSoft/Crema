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

using Ntreev.Crema.Client.Development.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Users.BrowserItems.ViewModels;
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

namespace Ntreev.Crema.Client.Development.MenuItems.Users
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(UserCategoryTreeViewItemViewModel))]
    class ManyUserCreateMenuItemViewModel : MenuItemBase
    {
        private readonly ICremaHost cremaHost;
        private Authority authority;

        [ImportingConstructor]
        public ManyUserCreateMenuItemViewModel(ICremaHost cremaHost)
        {
            this.DisplayName = "사용자 일괄 생성";
            this.cremaHost = cremaHost;
            this.cremaHost.Opened += CremaHost_Opened;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return this.cremaHost.IsOpened == true && this.authority == Authority.Admin;
        }

        protected override void OnExecute(object parameter)
        {
            var viewModel = parameter as UserCategoryTreeViewItemViewModel;
            if (viewModel.Target is IUserCategory category)
            {
                var dialog = new BatchUserCreationViewModel(category);
                dialog.ShowDialog();
            }
        }

        private void CremaHost_Opened(object sender, EventArgs e)
        {
            this.authority = this.cremaHost.Authority;
        }
    }
}
