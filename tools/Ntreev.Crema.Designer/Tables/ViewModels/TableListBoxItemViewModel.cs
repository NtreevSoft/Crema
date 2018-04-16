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

using Ntreev.Crema.Data;
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
using System.Windows.Input;
using System.Windows.Threading;

namespace Ntreev.Crema.Designer.Tables.ViewModels
{
    class TableListBoxItemViewModel : ListBoxItemViewModel, ITableDescriptor
    {
        private readonly TableTreeViewItemViewModel descriptor;
        private readonly ICommand selectInBrowserCommand;
        [Import]
        private IServiceProvider serviceProvider = null;

        public TableListBoxItemViewModel(TableTreeViewItemViewModel descriptor)
        {
            this.Target = descriptor;
            this.descriptor = descriptor;

            if (this.descriptor is INotifyPropertyChanged)
            {
                (this.descriptor as INotifyPropertyChanged).PropertyChanged += Descriptor_PropertyChanged;
            }
            this.selectInBrowserCommand = new DelegateCommand(item => this.SelectInBrowser());
        }

        public async void SelectInBrowser()
        {
            var table = this.descriptor.Target;
            var browser = this.serviceProvider.GetService(typeof(TableBrowserViewModel)) as TableBrowserViewModel;
            var shell = this.serviceProvider.GetService(typeof(IShell)) as IShell;
            var tableService = this.serviceProvider.GetService(typeof(TablesViewModel)) as TablesViewModel;
            await this.Dispatcher.InvokeAsync(() => shell.SelectedContent = tableService, DispatcherPriority.Background);
            await this.Dispatcher.InvokeAsync(() => browser.SelectedItem = this.descriptor, DispatcherPriority.Background);
        }

        public override string DisplayName
        {
            get { return this.TableInfo.Name; }
        }

        public TableAttribute TableAttribute
        {
            get { return this.descriptor.TableAttribute; }
        }

        public TableInfo TableInfo
        {
            get { return this.descriptor.TableInfo; }
        }

        public TableState TableState
        {
            get { return this.descriptor.TableState; }
        }

        public TagInfo Tags
        {
            get { return this.TableInfo.DerivedTags; }
        }

        public string Path
        {
            get { return this.TableInfo.CategoryPath + this.TableInfo.Name; }
        }

        public bool IsBeingEdited
        {
            get { return this.TableState.HasFlag(TableState.IsBeingEdited); }
        }

        public bool IsBeingSetup
        {
            get { return this.TableState.HasFlag(TableState.IsBeingSetup); }
        }

        public bool IsInherited
        {
            get { return this.TableInfo.TemplatedParent != string.Empty; }
        }

        public bool IsBaseTemplate
        {
            get { return this.TableAttribute.HasFlag(TableAttribute.BaseTable); }
        }

        public ICommand SelectInBrowserCommand
        {
            get { return this.selectInBrowserCommand; }
        }

        private void Descriptor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyOfPropertyChange(e.PropertyName);
        }

        #region  ITableDescriptor

        CremaDataTable ITableDescriptor.Target
        {
            get { return this.descriptor.Target as CremaDataTable; }
        }

        IEnumerable<ITableDescriptor> ITableDescriptor.Childs
        {
            get
            {
                foreach (var item in this.descriptor.Items)
                {
                    if (item is TableTreeViewItemViewModel viewModel)
                    {
                        yield return viewModel;
                    }
                }
            }
        }

        ITableDescriptor ITableDescriptor.Parent
        {
            get
            {
                if (this.descriptor.Parent is TableTreeViewItemViewModel viewModel)
                    return viewModel;
                return null;
            }
        }

        #endregion
    }
}
