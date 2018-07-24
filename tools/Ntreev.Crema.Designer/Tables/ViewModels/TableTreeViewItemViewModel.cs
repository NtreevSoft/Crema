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
using System.Linq;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Threading;
using System.Threading.Tasks;
using Ntreev.Library.ObjectModel;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Data;
using Ntreev.Library.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Collections;
using Ntreev.Library;
using System.ComponentModel;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.IO;

namespace Ntreev.Crema.Designer.Tables.ViewModels
{
    public class TableTreeViewItemViewModel : TreeViewItemViewModel, ITableDescriptor
    {
        [Import]
        private IServiceProvider serviceProvider = null;
        private CremaDataTable dataTable;
        private ISelector selector;

        private ICommand renameCommand;
        private ICommand deleteCommand;
        private ICommand editCommand;

        private TableInfo tableInfo;
        private TableAttribute tableAttribute;
        private TableState tableState;

        public TableTreeViewItemViewModel(CremaDataTable dataTable, ISelector selector)
        {
            this.Target = dataTable;
            this.dataTable = dataTable;
            this.dataTable.ExtendedProperties[selector] = this;
            this.selector = selector;
            this.tableInfo = dataTable.TableInfo;
            this.tableAttribute = TableAttribute.None;
            this.tableState = TableState.None;
            this.renameCommand = new DelegateCommand(item => this.Rename());
            this.deleteCommand = new DelegateCommand(item => this.Delete());
            this.editCommand = new DelegateCommand(item => this.EditContent());
            this.dataTable.PropertyChanged += DataTable_PropertyChanged;
        }

        public void UpdateCategoryPath()
        {

        }

        public void EditContent()
        {
            var service = this.serviceProvider.GetService(typeof(TableDocumentViewModel)) as TableDocumentViewModel;
            service.OpenTable(this.dataTable);
        }

        public void EditTemplate()
        {
            //var comment = await this.LockAsync("EditTemplate");
            //if (comment == null)
            //    return;

            var dataSet = new CremaDataSet();
            var dataTable = this.dataTable.CopyTo(dataSet);

            var dialog = new EditTemplateViewModel(new CremaTemplate(dataTable));
            if (dialog != null)
                dialog.ShowDialog();

            //await this.UnlockAsync(comment);
        }

        public void Delete()
        {
            var dialog = new DeleteViewModel();
            if (dialog.ShowDialog() != true)
                return;

            var dataSet = this.dataTable.DataSet;
            dataSet.Tables.Remove(this.dataTable);
            this.Parent = null;
        }

        public void Copy()
        {
            //var dialog = await this.table.Dispatcher.InvokeAsync(() => new CopyTableViewModel(this.authenticator, this.table));
            //if (dialog.ShowDialog() == true)
            //{

            //}
        }

        public void Inherit()
        {
            //var dialog = await this.table.Dispatcher.InvokeAsync(() => new CopyTableViewModel(this.authenticator, this.table));
            //dialog.UseTemplate = true;
            //dialog.ShowDialog();
        }

        public void Rename()
        {
            var dialog = new RenameTableViewModel(this.dataTable);
            if (dialog.ShowDialog() != true)
                return;

            this.dataTable.TableName = dialog.NewName;
        }

        public void Move()
        {
            var dataSet = this.dataTable.DataSet;
            var categoryPaths = CategoryTreeViewItemViewModel.GetAllCategoryPaths(this);
            var dialog = new MoveViewModel(this.dataTable.CategoryPath + this.dataTable.Name, categoryPaths);
            if (dialog.ShowDialog() != true)
                return;

            var targetViewModel = FindCategory(dialog.TargetPath);

            this.Parent = null;
            this.Parent = targetViewModel;

            if (this.selector != null)
                this.ExpandAncestors();
        }

        public void ViewLog()
        {
            //var dialog = new LogViewModel(this.authenticator, this.table);
            //dialog.ShowDialog();
        }

        public void AddChildTable()
        {
            //var comment = await this.LockAsync("AddChildTable");
            //if (comment == null)
            //    return;

            var dataSet = new CremaDataSet();
            var dataTable = this.dataTable.CopyTo(dataSet);
            var tempalte = CremaTemplate.Create(dataTable);
            var dialog = new NewChildTableViewModel(dataTable, tempalte);
            if (dialog != null)
                dialog.ShowDialog();

            //await this.UnlockAsync(comment);
        }

        public CremaDataTable Table => this.dataTable;

        public override string DisplayName
        {
            get
            {
                if (this.Parent is TableTreeViewItemViewModel)
                    return this.TableName;
                return this.TableInfo.Name;
            }
        }

        public string TableName => this.tableInfo.TableName;

        public string Path
        {
            get { return this.tableInfo.CategoryPath + this.tableInfo.Name; }
        }

        public TableInfo TableInfo
        {
            get { return this.tableInfo; }
        }

        public TableAttribute TableAttribute => this.tableAttribute;

        public TableState TableState => this.tableState;

        public TagInfo Tags
        {
            get { return this.tableInfo.DerivedTags; }
        }

        public bool IsInherited
        {
            get { return this.tableInfo.TemplatedParent != string.Empty; }
        }

        public bool IsBaseTemplate
        {
            get { return this.dataTable.DerivedTables.Any(); }
        }

        public ICommand RenameCommand
        {
            get { return this.renameCommand; }
        }

        public ICommand DeleteCommand
        {
            get { return this.deleteCommand; }
        }

        public ICommand EditCommand
        {
            get { return this.editCommand; }
        }

        protected override void OnDisposed(EventArgs e)
        {
            base.OnDisposed(e);
        }

        private void DataTable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CremaDataTable.TableName))
            {
                this.tableInfo = this.dataTable.TableInfo;
                this.Refresh();
            }
        }

        private CategoryTreeViewItemViewModel FindCategory(string categoryPath)
        {
            var viewModel = this as TreeViewItemViewModel;

            while (viewModel is DataBaseTreeViewItemViewModel == false)
            {
                viewModel = viewModel.Parent;
            }

            var segments = StringUtility.Split(categoryPath, PathUtility.SeparatorChar);

            foreach (var item in segments)
            {
                viewModel = viewModel.Items.Single(i => i.DisplayName == item);
            }

            return viewModel as CategoryTreeViewItemViewModel;
        }

        #region ITableDescriptor

        CremaDataTable ITableDescriptor.Target
        {
            get { return this.dataTable; }
        }

        ITableDescriptor ITableDescriptor.Parent
        {
            get { return this.Parent as ITableDescriptor; }
        }

        IEnumerable<ITableDescriptor> ITableDescriptor.Childs
        {
            get
            {
                foreach (var item in this.Items)
                {
                    if (item is ITableDescriptor == true)
                        yield return item as ITableDescriptor;
                }
            }
        }

        #endregion
    }
}
