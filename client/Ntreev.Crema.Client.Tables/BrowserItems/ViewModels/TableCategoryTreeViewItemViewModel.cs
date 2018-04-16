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
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System.Collections.Specialized;
using Ntreev.Crema.Client.Tables.Properties;
using System.Windows.Input;
using System.Threading.Tasks;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Data;
using Ntreev.Crema.Client.Tables.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.Library.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using Ntreev.ModernUI.Framework.ViewModels;
using System.ComponentModel;

namespace Ntreev.Crema.Client.Tables.BrowserItems.ViewModels
{
    public class TableCategoryTreeViewItemViewModel : TableCategoryTreeItemBase
    {
        private ICommand renameCommand;
        private ICommand deleteCommand;

        public TableCategoryTreeViewItemViewModel(Authentication authentication, ITableCategory category)
            : this(authentication, category, null)
        {

        }

        public TableCategoryTreeViewItemViewModel(Authentication authentication, ITableCategory category, object owner)
            : this(authentication, new TableCategoryDescriptor(authentication, category, DescriptorTypes.All, owner), owner)
        {

        }

        public TableCategoryTreeViewItemViewModel(Authentication authentication, TableCategoryDescriptor descriptor)
            : this(authentication, descriptor, null)
        {

        }

        public TableCategoryTreeViewItemViewModel(Authentication authentication, TableCategoryDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {
            this.renameCommand = new DelegateCommand(async item => await this.RenameAsync(), item => this.CanRename);
            this.deleteCommand = new DelegateCommand(async item => await this.DeleteAsync(), item => this.CanDelete);
            this.Owner = owner;
        }

        public async Task NewTableAsync()
        {
            if (await TableCategoryUtility.NewTableAsync(this.authentication, this.descriptor) is string tableName)
            {
                if (this.Owner is ISelector owner)
                {
                    var viewModel = this.Items.FirstOrDefault(item => item.DisplayName == tableName);
                    owner.SelectedItem = viewModel;
                }
            }
        }

        public async Task NewFolderAsync()
        {
            if (await TableCategoryUtility.NewFolderAsync(this.authentication, this.descriptor) is string categoryName)
            {
                var viewModel = this.Items.First(item => item.DisplayName == categoryName);
                this.IsExpanded = true;
                viewModel.IsSelected = true;
            }
        }

        public async Task RenameAsync()
        {
            if (await TableCategoryUtility.RenameAsync(this.authentication, this.descriptor) == false)
                return;

            if (this.Owner is ISelector owner)
                owner.SelectedItem = this;
        }

        public async Task MoveAsync()
        {
            if (await TableCategoryUtility.MoveAsync(this.authentication, this.descriptor) == false)
                return;

            if (this.Owner is ISelector)
                this.ExpandAncestors();
        }

        public async Task DeleteAsync()
        {
            await TableCategoryUtility.DeleteAsync(this.authentication, this.descriptor);
        }

        public async Task ViewLogAsync()
        {
            await TableItemUtility.ViewLogAsync(this.authentication, this.descriptor);
        }

        public async Task FindAsync()
        {
            await TableItemUtility.FindAsync(this.authentication, this.descriptor);
        }

        [DescriptorProperty]
        public bool CanNewTable => TableCategoryUtility.CanNewTable(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanNewFolder => TableCategoryUtility.CanNewFolder(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanRename => TableCategoryUtility.CanRename(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanMove => TableCategoryUtility.CanMove(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanDelete => TableCategoryUtility.CanDelete(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanFind => TableItemUtility.CanFind(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanViewLog => TableItemUtility.CanViewLog(this.authentication, this.descriptor);

        public ICommand RenameCommand => this.renameCommand;

        public ICommand DeleteCommand => this.deleteCommand;

        public override int Order => 1;

        public override string DisplayName => this.descriptor.Name;

        protected override TableCategoryTreeItemBase CreateInstance(Authentication authentication, TableCategoryDescriptor descriptor, object owner)
        {
            return new TableCategoryTreeViewItemViewModel(authentication, descriptor, owner);
        }

        protected override TableTreeItemBase CreateInstance(Authentication authentication, TableDescriptor descriptor, object owner)
        {
            return new TableTreeViewItemViewModel(authentication, descriptor, owner);
        }
    }
}
