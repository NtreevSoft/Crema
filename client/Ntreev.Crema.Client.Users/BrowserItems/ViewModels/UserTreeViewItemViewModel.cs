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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Ntreev.Crema.Services;
using System.Windows.Input;
using Ntreev.Crema.Client.Framework;
using Caliburn.Micro;
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Users.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using System.Windows;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Threading.Tasks;
using System.ComponentModel;
using Ntreev.Crema.Client.Users.MenuItems;

namespace Ntreev.Crema.Client.Users.BrowserItems.ViewModels
{
    public class UserTreeViewItemViewModel : UserTreeItemBase
    {
        private readonly ICommand modifyCommand;
        private readonly ICommand deleteCommand;

        public UserTreeViewItemViewModel(Authentication authentication, IUser user, object owner)
            : this(authentication, new UserDescriptor(authentication, user, DescriptorTypes.All, owner), owner)
        {


        }

        internal protected UserTreeViewItemViewModel(Authentication authentication, UserDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {
            this.modifyCommand = new DelegateCommand(async item => await this.ChangeAsync(), item => this.CanChange);
            this.deleteCommand = new DelegateCommand(async item => await this.DeleteAsync(), item => this.CanDelete);
        }

        public async Task ChangeAsync()
        {
            if (await UserUtility.ChangeAsync(this.authentication, this) == true)
            {
                this.NotifyOfPropertyChange(nameof(this.DisplayName));
            }
        }

        public async Task MoveAsync()
        {
            if (await UserUtility.MoveAsync(this.authentication, this) == true)
            {
                await this.Dispatcher.InvokeAsync(() =>
                {
                    this.Parent.IsExpanded = true;
                    this.IsSelected = true;
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        public async Task DeleteAsync()
        {
            await UserUtility.DeleteAsync(this.authentication, this);
        }

        public async Task SendMessageAsync()
        {
            await UserUtility.SendMessageAsync(this.authentication, this);
        }

        public async Task KickAsync()
        {
            await UserUtility.KickAsync(this.authentication, this);
        }

        public async Task BanAsync()
        {
            var dialog = await BanViewModel.CreateInstanceAsync(this.authentication, this);
            dialog?.ShowDialog();
        }

        public async Task UnbanAsync()
        {
            await UserUtility.UnbanAsync(this.authentication, this);
        }

        public override string DisplayName
        {
            get { return this.descriptor.UserInfo.ID + " [" + this.descriptor.UserInfo.Name + "]"; }
        }

        public ICommand ModifyCommand => this.modifyCommand;

        public ICommand DeleteCommand => this.deleteCommand;

        public IUserDescriptor UserDescriptor => this.descriptor;

        public bool CanSendMessage => UserUtility.CanSendMessage(this.authentication, this);

        public bool CanChange => UserUtility.CanChange(this.authentication, this);

        public bool CanDelete => UserUtility.CanDelete(this.authentication, this);

        public bool CanKick => UserUtility.CanKick(this.authentication, this);

        public bool CanBan => UserUtility.CanBan(this.authentication, this);

        public bool CanUnban => UserUtility.CanUnban(this.authentication, this);
    }
}
