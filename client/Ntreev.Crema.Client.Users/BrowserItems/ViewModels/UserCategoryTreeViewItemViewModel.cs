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
using Ntreev.Crema.Client.Users.Dialogs.ViewModels;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework;
using Ntreev.Library.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Windows.Input;
using Ntreev.Crema.Client.Users.Properties;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace Ntreev.Crema.Client.Users.BrowserItems.ViewModels
{
    public class UserCategoryTreeViewItemViewModel : UserCategoryTreeItemBase
    {
        private ICommand deleteCommand;

        public UserCategoryTreeViewItemViewModel(Authentication authentication, IUserCategory category, object owner)
            : this(authentication, new UserCategoryDescriptor(authentication, category, DescriptorTypes.All, owner), owner)
        {

        }

        private UserCategoryTreeViewItemViewModel(Authentication authentication, UserCategoryDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {
            this.deleteCommand = new DelegateCommand(async item => await this.DeleteAsync(), item => this.CanDelete);
        }

        public async Task NewUserAsync()
        {
            await UserCategoryUtility.NewUserAsync(this.authentication, this.descriptor);
        }

        public async Task NewFolderAsync()
        {
            if (await UserCategoryUtility.NewFolderAsync(this.authentication, this.descriptor) is string categoryName)
            {
                await this.Dispatcher.InvokeAsync(() =>
                {
                    var viewModel = this.Items.First(item => item.DisplayName == categoryName);
                    this.IsExpanded = true;
                    viewModel.IsSelected = true;
                });
            }
        }

        public async Task RenameAsync()
        {
            await UserCategoryUtility.RenameAsync(this.authentication, this.descriptor);
        }

        public async Task MoveAsync()
        {
            if (await UserCategoryUtility.MoveAsync(this.authentication, this.descriptor) == true)
            {
                await this.Dispatcher.InvokeAsync(() =>
                {
                    this.Parent.IsExpanded = true;
                    this.IsSelected = true;
                });
            }
        }

        public async Task DeleteAsync()
        {
            await UserCategoryUtility.DeleteAsync(this.authentication, this.descriptor);
        }

        public bool CanRename => UserCategoryUtility.CanRename(this.authentication, this.descriptor);

        public bool CanMove => UserCategoryUtility.CanMove(this.authentication, this.descriptor);

        public bool CanDelete => UserCategoryUtility.CanDelete(this.authentication, this.descriptor);

        public ICommand DeleteCommand => this.deleteCommand;

        public override string DisplayName => this.descriptor.Name == string.Empty ? Resources.Title_Users : this.descriptor.Name;

        protected override UserCategoryTreeItemBase CreateInstance(Authentication authentication, UserCategoryDescriptor descriptor, object owner)
        {
            return new UserCategoryTreeViewItemViewModel(authentication, descriptor, owner);
        }

        protected override UserTreeItemBase CreateInstance(Authentication authentication, UserDescriptor descriptor, object owner)
        {
            return new UserTreeViewItemViewModel(authentication, descriptor, owner);
        }
    }
}
