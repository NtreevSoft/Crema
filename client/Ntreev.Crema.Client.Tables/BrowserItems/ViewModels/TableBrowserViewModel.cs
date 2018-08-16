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
using Ntreev.Crema.Client.Framework;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Threading;
using Ntreev.Crema.Client.Tables.Properties;
using Ntreev.ModernUI.Framework;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using Ntreev.Library.ObjectModel;
using System.Windows;
using Ntreev.Library.Linq;
using Ntreev.Library;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Collections;
using System.Windows.Threading;

namespace Ntreev.Crema.Client.Tables.BrowserItems.ViewModels
{
    [Export(typeof(IBrowserItem))]
    [Export(typeof(ITableBrowser))]
    [InheritedExport(typeof(TableBrowserViewModel))]
    [RequiredAuthority(Authority.Guest)]
    [ParentType(typeof(BrowserService))]
    class TableBrowserViewModel : TreeViewBase, ITableBrowser, ISelector
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
        public TableBrowserViewModel(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Loaded += CremaAppHost_Loaded;
            this.cremaAppHost.Unloaded += CremaAppHost_Unloaded;
            this.cremaAppHost.Resetting += CremaAppHost_Resetting;
            this.cremaAppHost.Reset += CremaAppHost_Reset;
            this.renameCommand = new DelegateCommand(this.Rename_Execute, this.Rename_CanExecute);
            this.deleteCommand = new DelegateCommand(this.Delete_Execute, this.Delete_CanExecute);
            this.DisplayName = Resources.Title_TableBrowser;
            this.Dispatcher.InvokeAsync(() => this.AttachPropertyService(this.propertyService));
        }

        public void Select(ITableDescriptor descriptor)
        {
            foreach (var item in this.Items.Descendants(item => item.Items))
            {
                if (item is TableTreeViewItemViewModel viewModel && viewModel.Name == descriptor.TableInfo.Name)
                {
                    viewModel.ExpandAncestors();
                    viewModel.IsSelected = true;
                    break;
                }
            }
        }

        public ITableItemDescriptor GetDescriptor(ITableItemDescriptor descriptor)
        {
            foreach (var item in this.Items.FamilyTree(item => item.Items))
            {
                if (item is ITableItemDescriptor viewModel)
                {
                    if (viewModel == descriptor)
                        return viewModel;
                    if (viewModel.Target == descriptor.Target)
                        return viewModel;
                }
            }
            return null;
        }

        public ITableItemDescriptor GetDescriptor(string path)
        {
            foreach (var item in this.Items.FamilyTree(item => item.Items))
            {
                if (item is ITableItemDescriptor descriptor && descriptor.Path == path)
                {
                    return descriptor;
                }
            }
            return null;
        }

        public TableTreeViewItemViewModel GetViewModel(ITableDescriptor descriptor)
        {
            foreach (var item in this.Items.Descendants(item => item.Items))
            {
                if (item is TableTreeViewItemViewModel viewModel && viewModel.Name == descriptor.Name)
                {
                    return viewModel;
                }
            }
            return null;
        }

        public TableTreeViewItemViewModel GetTableViewModel(string name)
        {
            foreach (var item in this.Items.Descendants(item => item.Items))
            {
                if (item is TableTreeViewItemViewModel viewModel && viewModel.Name == name)
                {
                    return viewModel;
                }
            }
            return null;
        }

        public bool IsVisible
        {
            get { return this.isVisible; }
            set
            {
                this.isVisible = value;
                this.NotifyOfPropertyChange(nameof(IsVisible));
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
            if (parameter is TableTreeViewItemViewModel tableViewModel)
            {
                tableViewModel.DeleteCommand.Execute(parameter);
            }
            else if (parameter is TableCategoryTreeViewItemViewModel categoryViewModel)
            {
                categoryViewModel.DeleteCommand.Execute(parameter);
            }
        }

        private bool Delete_CanExecute(object parameter)
        {
            if (parameter is TableRootTreeViewItemViewModel)
                return false;

            if (parameter is TableTreeViewItemViewModel tableViewModel)
                return tableViewModel.DeleteCommand.CanExecute(parameter);

            if (parameter is TableCategoryTreeViewItemViewModel categoryViewModel)
                return categoryViewModel.DeleteCommand.CanExecute(parameter);

            return false;
        }

        private void Rename_Execute(object parameter)
        {
            if (parameter is TableTreeViewItemViewModel tableViewModel)
            {
                tableViewModel.RenameCommand.Execute(parameter);
            }
            else if (parameter is TableCategoryTreeViewItemViewModel categoryViewModel)
            {
                categoryViewModel.RenameCommand.Execute(parameter);
            }
        }

        private bool Rename_CanExecute(object parameter)
        {
            if (parameter is TableRootTreeViewItemViewModel)
                return false;

            if (parameter is TableTreeViewItemViewModel tableViewModel)
                return tableViewModel.RenameCommand.CanExecute(parameter);

            if (parameter is TableCategoryTreeViewItemViewModel categoryViewModel)
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
                    return new TableRootTreeViewItemViewModel(this.authenticator, dataBase, this);
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

        #region ITableBrowser

        object ISelector.SelectedItem
        {
            get
            {
                return this.SelectedItem;
            }
            set
            {
                if (value is TreeViewItemViewModel viewModel)
                    this.SelectedItem = viewModel;
                else
                    throw new NotImplementedException();
            }
        }

        IEnumerable ITableBrowser.Items
        {
            get { return this.Items; }
        }

        #endregion
    }
}
