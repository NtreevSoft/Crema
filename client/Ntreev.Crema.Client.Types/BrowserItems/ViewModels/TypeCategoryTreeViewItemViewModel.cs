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
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Client.Framework;
using Caliburn.Micro;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Types.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ntreev.ModernUI.Framework.ViewModels;
using Ntreev.Crema.Client.Types.Properties;

namespace Ntreev.Crema.Client.Types.BrowserItems.ViewModels
{
    public class TypeCategoryTreeViewItemViewModel : TypeCategoryTreeItemBase
    {
        private ICommand renameCommand;
        private ICommand deleteCommand;

        public TypeCategoryTreeViewItemViewModel(Authentication authentication, ITypeCategory category)
            : this(authentication, category, null)
        {

        }

        public TypeCategoryTreeViewItemViewModel(Authentication authentication, ITypeCategory category, object owner)
            : this(authentication, new TypeCategoryDescriptor(authentication, category, DescriptorTypes.All, owner), owner)
        {

        }

        public TypeCategoryTreeViewItemViewModel(Authentication authentication, TypeCategoryDescriptor descriptor)
            : this(authentication, descriptor, null)
        {

        }

        public TypeCategoryTreeViewItemViewModel(Authentication authentication, TypeCategoryDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {
            this.renameCommand = new DelegateCommand(async item => await this.RenameAsync(), item => this.CanRename);
            this.deleteCommand = new DelegateCommand(async item => await this.DeleteAsync(), item => this.CanDelete);
            this.Owner = owner;
        }

        public async Task NewTypeAsync()
        {
            if (await TypeCategoryUtility.NewTypeAsync(this.authentication, this.descriptor) is string typeName)
            {
                if (this.Owner is ISelector owner)
                {
                    var viewModel = this.Items.FirstOrDefault(item => item.DisplayName == typeName);
                    owner.SelectedItem = viewModel;
                }
            }
        }

        public async Task NewFolderAsync()
        {
            if (await TypeCategoryUtility.NewFolderAsync(this.authentication, this.descriptor) is string categoryName)
            {
                var viewModel = this.Items.First(item => item.DisplayName == categoryName);
                this.IsExpanded = true;
                viewModel.IsSelected = true;
            }
        }

        public async Task RenameAsync()
        {
            if (await TypeCategoryUtility.RenameAsync(this.authentication, this.descriptor) == false)
                return;

            if (this.Owner is ISelector owner)
                owner.SelectedItem = this;
        }

        public async Task MoveAsync()
        {
            if (await TypeCategoryUtility.MoveAsync(this.authentication, this.descriptor) == false)
                return;

            if (this.Owner is ISelector)
                this.ExpandAncestors();
        }

        public async Task DeleteAsync()
        {
            await TypeCategoryUtility.DeleteAsync(this.authentication, this.descriptor);
        }

        public async Task ViewLogAsync()
        {
            await TypeCategoryUtility.ViewLogAsync(this.authentication, this.descriptor);
        }

        public async Task FindAsync()
        {
            await TypeCategoryUtility.FindAsync(this.authentication, this.descriptor);
        }

        [DescriptorProperty]
        public bool CanNewType => TypeCategoryUtility.CanNewType(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanNewFolder => TypeCategoryUtility.CanNewFolder(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanRename => TypeCategoryUtility.CanRename(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanMove => TypeCategoryUtility.CanMove(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanDelete => TypeCategoryUtility.CanDelete(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanFind => TypeCategoryUtility.CanFind(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanViewLog => TypeCategoryUtility.CanViewLog(this.authentication, this.descriptor);

        public ICommand RenameCommand => this.renameCommand;

        public ICommand DeleteCommand => this.deleteCommand;

        public override int Order => 1;

        public override string DisplayName => this.descriptor.Name;

        protected override TypeCategoryTreeItemBase CreateInstance(Authentication authentication, TypeCategoryDescriptor descriptor, object owner)
        {
            return new TypeCategoryTreeViewItemViewModel(authentication, descriptor, owner);
        }

        protected override TypeTreeItemBase CreateInstance(Authentication authentication, TypeDescriptor descriptor, object owner)
        {
            return new TypeTreeViewItemViewModel(authentication, descriptor, owner);
        }

    }
}
