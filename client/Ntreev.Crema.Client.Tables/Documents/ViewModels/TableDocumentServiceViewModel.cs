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

using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Tables.Documents.Views;
using Ntreev.Crema.Client.Tables.Properties;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using Ntreev.Crema.Client.Tables.BrowserItems.ViewModels;
using Ntreev.Library.Linq;

namespace Ntreev.Crema.Client.Tables.Documents.ViewModels
{
    [Export(typeof(ITableDocumentService))]
    [InheritedExport(typeof(TableDocumentServiceViewModel))]
    class TableDocumentServiceViewModel : DocumentServiceBase<IDocument>, ITableDocumentService
    {
        private readonly ICremaAppHost cremaAppHost;

        [Import]
        private ICompositionService compositionService = null;
        [Import]
        private Lazy<TableBrowserViewModel> browser = null;

        [ImportingConstructor]
        public TableDocumentServiceViewModel(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Unloaded += CremaAppHost_Unloaded;
            this.cremaAppHost.Resetting += CremaAppHost_Resetting;
            this.cremaAppHost.Reset += CremaAppHost_Reset;
            this.DisplayName = Resources.Title_Tables;
        }

        public TableDocumentBase Find(ITableDescriptor descriptor)
        {
            foreach (var document in this.Documents)
            {
                if (document is TableDocumentBase tableDocument)
                {
                    foreach (var documentItem in tableDocument.Tables)
                    {
                        if (documentItem.Target == descriptor.Target)
                        {
                            return tableDocument;
                        }
                    }
                }
            }
            return null;
        }

        public TableItemViewModel FindItem(ITableDescriptor descriptor)
        {
            foreach (var document in this.Documents)
            {
                if (document is TableDocumentBase tableDocument)
                {
                    foreach (var documentItem in tableDocument.Tables)
                    {
                        if (documentItem.Target == descriptor.Target)
                        {
                            return documentItem;
                        }
                    }
                }
            }
            return null;
        }

        public TableDataFinderViewModel AddFinder(Authentication authentication, ITableItemDescriptor descriptor)
        {
            var document = new TableDataFinderViewModel(authentication, this.Browser, descriptor);
            this.compositionService?.SatisfyImportsOnce(document);
            this.Items.Add(document);
            return document;
        }

        public TableEditorViewModel OpenTable(Authentication authentication, ITableDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is ITable table)
            {
                var viewModel = this.Browser.GetViewModel(descriptor);
                if (viewModel.Parent is TableTreeViewItemViewModel parent)
                {
                    return this.OpenTable(authentication, parent.Descriptor, descriptor.TableName);
                }
                else
                {
                    return this.OpenTable(authentication, viewModel.Descriptor, descriptor.TableName);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public TableViewerViewModel ViewTable(Authentication authentication, ITableDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is ITable table)
            {
                var viewModel = this.Browser.GetViewModel(descriptor);
                if (viewModel.Parent is TableTreeViewItemViewModel parent)
                {
                    return this.ViewTable(authentication, parent.Descriptor, descriptor.TableName);
                }
                else
                {
                    return this.ViewTable(authentication, viewModel.Descriptor, descriptor.TableName);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void MoveToTable(Authentication authentication, ITableDescriptor descriptor, string columnName, int row)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            //if (descriptor.Target is ITable table)
            //{
            //    this.MoveToTable(authentication, table, columnName, row);
            //}
            //else
            //{
            //    throw new NotImplementedException();
            //}
        }

        //public TableDataFinderViewModel AddFinder(Authentication authentication, string itemPath)
        //{
        //    var document = new TableDataFinderViewModel(authentication, this.Browser, itemPath);
        //    this.compositionService?.SatisfyImportsOnce(document);
        //    this.Items.Add(document);
        //    return document;
        //}

        //public async void OpenTable(Authentication authentication, ITable table)
        //{
        //    var result = await table.Dispatcher.InvokeAsync(() =>
        //    {
        //        var targetTable = table.Parent ?? table;
        //        return Tuple.Create(targetTable, targetTable.Name, table.Name);
        //    });

        //    this.OpenTable(authentication, result.Item1, result.Item2, result.Item3);
        //}

        //public async void MoveToTable(Authentication authentication, ITable table, string columnName, int row)
        //{
        //    var result = await table.Dispatcher.InvokeAsync(() =>
        //    {
        //        var targetTable = table.Parent ?? table;
        //        return Tuple.Create(targetTable, targetTable.Name, table.Name);
        //    });

        //    var document = this.OpenTable(authentication, result.Item1, result.Item2, result.Item3);
        //    new SelectFieldStrategy(document, result.Item3, columnName, row);
        //}

        //public async void ViewTable(Authentication authentication, ITable table)
        //{
        //    var result = await table.Dispatcher.InvokeAsync(() =>
        //    {
        //        var targetTable = table.Parent ?? table;
        //        return Tuple.Create(targetTable, targetTable.Name, table.Name);
        //    });

        //    this.OpenTable(authentication, result.Item1, result.Item2, result.Item3);
        //}

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);

            if (this.SelectedDocument is TableEditorViewModel document && document.SelectedTable != null)
            {
                if (document.SelectedTable is ITableDescriptor descriptor)
                {
                    this.Browser.Select(descriptor);
                }
            }
        }

        private void CremaAppHost_Unloaded(object sender, EventArgs e)
        {
            this.Dispatcher.InvokeAsync(() => this.Items.Clear());
        }

        private void CremaAppHost_Resetting(object sender, EventArgs e)
        {
            this.Items.Clear();
        }

        private void CremaAppHost_Reset(object sender, EventArgs e)
        {
            
        }

        private TableEditorViewModel OpenTable(Authentication authentication, TableDescriptor descriptor, string tableName)
        {
            this.ValidateOpenTable(authentication, descriptor);
            var document = new TableEditorViewModel(authentication, descriptor);
            document.SelectedTable = document.Tables.FirstOrDefault(item => item.TableName == tableName);
            this.compositionService?.SatisfyImportsOnce(document);
            this.Items.Add(document);
            return document;
        }

        private void ValidateOpenTable(Authentication authentication, TableDescriptor descriptor)
        {
            foreach (var item in this.Items)
            {
                if (item is ITableDocument document && document.Target == descriptor.Target)
                {
                    throw new ArgumentException("이미 문서가 열려있습니다.");
                }
            }
        }

        private TableViewerViewModel ViewTable(Authentication authentication, TableDescriptor descriptor, string tableName)
        {
            this.ValidateViewTable(authentication, descriptor);
            var document = new TableViewerViewModel(authentication, descriptor);
            document.SelectedTable = document.Tables.FirstOrDefault(item => item.TableName == tableName);
            this.compositionService?.SatisfyImportsOnce(document);
            this.Items.Add(document);
            return document;
        }

        private void ValidateViewTable(Authentication authentication, TableDescriptor descriptor)
        {
            foreach (var item in this.Items)
            {
                if (item is ITableDocument document && document.Target == descriptor.Target)
                {
                    throw new ArgumentException("이미 문서가 열려있습니다.");
                }
            }
        }

        private TableBrowserViewModel Browser => this.browser.Value;

        #region ITableDocumentService

        IDocument ITableDocumentService.AddFinder(Authentication authentication, ITableItemDescriptor descriptor)
        {
            return this.AddFinder(authentication, descriptor);
        }

        ITableDocument ITableDocumentService.OpenTable(Authentication authentication, ITableDescriptor descriptor)
        {
            return this.OpenTable(authentication, descriptor);
        }

        ITableDocument ITableDocumentService.ViewTable(Authentication authentication, ITableDescriptor descriptor)
        {
            return this.ViewTable(authentication, descriptor);
        }

        #endregion
    }
}
