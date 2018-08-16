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

using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System.Windows;
using System.Windows.Input;
using Ntreev.ModernUI.Framework;
using Ntreev.Library.ObjectModel;
using System;
using System.Linq;
using System.Collections.Generic;
using Ntreev.ModernUI.Framework.ViewModels;
using Ntreev.Library.Linq;
using System.Collections;
using System.Windows.Threading;
using Ntreev.Crema.Client.Types.Properties;
using System.Threading.Tasks;
using Ntreev.Library;

namespace Ntreev.Crema.Client.Types.BrowserItems.ViewModels
{
    [Export(typeof(IBrowserItem))]
    [Export(typeof(ITypeBrowser))]
    [Export(typeof(TypeBrowserViewModel))]
    [RequiredAuthority(Authority.Guest)]
    [ParentType(typeof(BrowserService))]
    class TypeBrowserViewModel : TreeViewBase, ITypeBrowser, ISelector
    {
        private readonly ICremaAppHost cremaAppHost;
        private bool isVisible = true;
        private Guid dataBaseID;

        private DelegateCommand renameCommand;
        private DelegateCommand deleteCommand;

        [Import]
        private IPropertyService propertyService = null;
        [Import]
        private Authenticator authenticator = null;
        [Import]
        private ICompositionService compositionService = null;

        [ImportingConstructor]
        public TypeBrowserViewModel(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Loaded += CremaAppHost_Loaded;
            this.cremaAppHost.Unloaded += CremaAppHost_Unloaded;
            this.cremaAppHost.Resetting += CremaAppHost_Resetting;
            this.cremaAppHost.Reset += CremaAppHost_Reset;
            this.renameCommand = new DelegateCommand(this.Rename_Execute, this.Rename_CanExecute);
            this.deleteCommand = new DelegateCommand(this.Delete_Execute, this.Delete_CanExecute);
            this.DisplayName = Resources.Title_TypeBrowser;
            this.Dispatcher.InvokeAsync(() => this.AttachPropertyService(this.propertyService));
        }

        public void Select(ITypeDescriptor descriptor)
        {
            foreach (var item in this.Items.Descendants(item => item.Items))
            {
                if (item is TypeTreeViewItemViewModel viewModel && viewModel.Name == descriptor.TypeInfo.Name)
                {
                    viewModel.ExpandAncestors();
                    viewModel.IsSelected = true;
                    break;
                }
            }
        }

        public bool IsVisible
        {
            get { return this.isVisible; }
            set
            {
                this.isVisible = value;
                this.NotifyOfPropertyChange(nameof(this.IsVisible));
            }
        }

        public ICommand RenameCommand
        {
            get { return this.renameCommand; }
        }

        public ICommand DeleteCommand
        {
            get { return this.deleteCommand; }
        }

        private void CremaAppHost_Loaded(object sender, EventArgs e)
        {
            this.Initialize();
        }

        private void CremaAppHost_Unloaded(object sender, EventArgs e)
        {
            this.cremaAppHost.UserConfigs.Commit(this);
            this.Release();
        }

        private void CremaAppHost_Resetting(object sender, EventArgs e)
        {
            this.Release();
        }

        private void CremaAppHost_Reset(object sender, EventArgs e)
        {
            this.Initialize();
        }

        private void Delete_Execute(object parameter)
        {
            if (parameter is TypeTreeViewItemViewModel typeViewModel)
            {
                typeViewModel.DeleteCommand.Execute(parameter);
            }
            else if (parameter is TypeCategoryTreeViewItemViewModel categoryViewModel)
            {
                categoryViewModel.DeleteCommand.Execute(parameter);
            }
        }

        private bool Delete_CanExecute(object parameter)
        {
            if (parameter is TypeRootTreeViewItemViewModel)
                return false;

            if (parameter is TypeTreeViewItemViewModel typeViewModel)
                return typeViewModel.DeleteCommand.CanExecute(parameter);

            if (parameter is TypeCategoryTreeViewItemViewModel categoryViewModel)
                return categoryViewModel.DeleteCommand.CanExecute(parameter);

            return false;
        }

        private void Rename_Execute(object parameter)
        {
            if (parameter is TypeTreeViewItemViewModel typeViewModel)
            {
                typeViewModel.RenameCommand.Execute(parameter);
            }
            else if (parameter is TypeCategoryTreeViewItemViewModel categoryViewModel)
            {
                categoryViewModel.RenameCommand.Execute(parameter);
            }
        }

        private bool Rename_CanExecute(object parameter)
        {
            if (parameter is TypeRootTreeViewItemViewModel)
                return false;

            if (parameter is TypeTreeViewItemViewModel typeViewModel)
                return typeViewModel.RenameCommand.CanExecute(parameter);

            if (parameter is TypeCategoryTreeViewItemViewModel categoryViewModel)
                return categoryViewModel.RenameCommand.CanExecute(parameter);

            return false;
        }

        private async void Initialize()
        {
            if (this.cremaAppHost.GetService(typeof(IDataBase)) is IDataBase dataBase)
            {
                this.dataBaseID = dataBase.ID;
                var viewModel = await dataBase.Dispatcher.InvokeAsync(() =>
                {
                    return new TypeRootTreeViewItemViewModel(this.authenticator, dataBase, this);
                });
                this.compositionService.SatisfyImportsOnce(viewModel);
                this.Items.Add(viewModel);
            };

            this.cremaAppHost.UserConfigs.Update(this);
        }

        private void Release()
        {
            this.FilterExpression = string.Empty;
            this.Items.Clear();
        }

        [ConfigurationProperty(ScopeType = typeof(ICremaConfiguration))]
        private string[] Settings
        {
            get { return this.GetSettings(); }
            set
            {
                this.SetSettings(value);
            }
        }

        #region ITypeBrowser

        object ITypeBrowser.SelectedItem
        {
            get
            {
                return this.SelectedItem;
            }
            set
            {
                this.SelectedItem = value as TreeViewItemViewModel;
            }
        }

        IEnumerable ITypeBrowser.Items
        {
            get { return this.Items; }
        }

        #endregion
    }
}
