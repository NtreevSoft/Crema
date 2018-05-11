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

using Ntreev.Crema.Client.Framework.Controls;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.DataGrid;
using System.Windows.Threading;

namespace Ntreev.Crema.Client.Framework.Controls
{
    public class DomainInsertionRow : InsertionRow
    {
        private string tableName;
        private string[] columnNames;
        private object[] defaultArray;
        private object[] itemArray;
        private bool hasField;

        private DomainRowInfo[] results;

        public DomainInsertionRow()
        {
            this.InsertionMode = InsertionMode.Continuous;

            this.AttachInsertBinding();
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, InsertFromClipboard_Execute, InsertFromClipboard_CanExecute));
            this.CellEditorDisplayConditions = Xceed.Wpf.DataGrid.CellEditorDisplayConditions.CellIsCurrent;
        }

        public DomainDataGridControl GridControl
        {
            get { return (DomainDataGridControl)this.GridContext.DataGridControl; }
        }

        public DataGridContext GridContext
        {
            get { return (DataGridContext)this.GetValue(DataGridControl.DataGridContextProperty); }
        }

        public void InsertRowFromClipboard()
        {
            var gridContext = DataGridControl.GetDataGridContext(this);
            var gridControl = gridContext.DataGridControl as DomainDataGridControl;
            var inserter = new DomainTextClipboardInserter(gridContext);

            var textData = ClipboardUtility.GetData(true);

            if (textData.Length == 1)
            {
                var textLine = textData.First();
                var index = gridContext.CurrentColumn == null ? -1 : gridContext.VisibleColumns.IndexOf(gridContext.CurrentColumn);

                for (var i = 0; i < textLine.Length; i++)
                {
                    var text = textLine[i];
                    var column = gridContext.VisibleColumns[index + i];
                    if (this.Cells[column] is DomainInsertionCell cell)
                    {
                        cell.BeginEdit();
                        cell.EditingContent = text;
                        cell.EndEdit();
                    }
                }
            }
            else
            {
                inserter.Parse(textData);

                var domainRows = inserter.DomainRows;
                var domain = gridControl.Domain;
                var authenticator = domain.GetService(typeof(Authenticator)) as Authenticator;

                (gridContext.Items as INotifyCollectionChanged).CollectionChanged += Items_CollectionChanged;
                try
                {
                    this.results = domain.Dispatcher.Invoke(() => domain.NewRow(authenticator, domainRows));
                }
                catch (Exception e)
                {
                    this.results = null;
                    (gridContext.Items as INotifyCollectionChanged).CollectionChanged -= Items_CollectionChanged;
                    throw e;
                }
            }
        }

        public void InsertRow()
        {
            var gridContext = DataGridControl.GetDataGridContext(this);
            var gridControl = gridContext.DataGridControl as DomainDataGridControl;
            var fields = this.CreateFields();
            var domain = gridControl.Domain;
            var authenticator = domain.GetService(typeof(Authenticator)) as Authenticator;

            (gridContext.Items as INotifyCollectionChanged).CollectionChanged += Items_CollectionChanged;
            try
            {
                var keys = domain.Dispatcher.Invoke(() => domain.NewRow(authenticator, this.tableName, fields));
                this.results = new DomainRowInfo[] { new DomainRowInfo() { Keys = keys, }, };
            }
            catch (Exception e)
            {
                this.results = null;
                (gridContext.Items as INotifyCollectionChanged).CollectionChanged -= Items_CollectionChanged;
                throw e;
            }
        }

        public event EventHandler Inserted;

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        protected override Cell CreateCell(ColumnBase column)
        {
            return new DomainInsertionCell(this);
        }

        protected override void PrepareContainer(DataGridContext dataGridContext, object item)
        {
            base.PrepareContainer(dataGridContext, item);
            ModernDataGridControl.SetInsertionRow(dataGridContext, this);
        }

        protected override void SetDataContext(object item)
        {
            base.SetDataContext(item);
        }

        protected override void OnEditBeginning(CancelRoutedEventArgs e)
        {
            base.OnEditBeginning(e);
        }

        protected override void BeginEditCore()
        {
            try
            {
                this.IsBeginEnding = true;
                if (this.hasField == false)
                {
                    try
                    {
                        base.BeginEditCore();

                        var gridContext = DataGridControl.GetDataGridContext(this);
                        var typedList = gridContext.Items.SourceCollection as ITypedList;
                        if (typedList == null)
                        {
                            var source = (gridContext.Items.SourceCollection as CollectionView).SourceCollection;
                            typedList = source as ITypedList;
                        }

                        var props = typedList.GetItemProperties(null);


                        this.tableName = CremaDataTable.GetTableName(typedList.GetListName(null));
                        this.columnNames = CremaDataRowUtility.GetColumnNames(typedList);

                        this.defaultArray = new object[this.columnNames.Length];
                        this.itemArray = new object[this.columnNames.Length];
                        for (var i = 0; i < this.columnNames.Length; i++)
                        {
                            var prop = props[this.columnNames[i]];
                            if (prop.IsReadOnly == true)
                                continue;
                            var field = prop.GetValue(this.DataContext);
                            if (field != DBNull.Value)
                            {
                                this.defaultArray[i] = field;
                                this.itemArray[i] = field;
                            }

                            //if (gridContext.Columns[this.columnNames[i]] != null)
                            //{
                            //    this.fields.Add(this.columnNames[i], field);
                            //}
                        }

                        // 컬럼 가상화 사용시 보이지 않는 셀은 생성이 안되기 때문에 
                        // 아래 코드를 통해서 한번 쭉 돌려서 PrepareContainer 호출하게 하여 초기값을 설정하도록 했음. Release에서 동작함
                        //foreach (var item in this.Cells)
                        //{
                        //    var be = item.GetBindingExpression(Cell.ContentProperty);
                        //    be?.UpdateTarget();
                        //}

                        base.CancelEditCore();
                    }
                    finally
                    {
                        this.hasField = true;
                    }
                }
            }
            finally
            {
                this.IsBeginEnding = false;
            }
        }

        protected override void EndEditCore()
        {

        }

        protected override void CancelEditCore()
        {
            try
            {
                this.IsBeginEnding = true;
            }
            finally
            {
                this.IsBeginEnding = false;
            }
        }

        protected virtual void OnInserted(EventArgs e)
        {
            this.Inserted?.Invoke(this, e);
        }

        protected async override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);

            await this.Dispatcher.InvokeAsync(() =>
            {
                if (this.hasField == false && (bool)e.NewValue == true)
                {
                    var gridContext = DataGridControl.GetDataGridContext(this);
                    gridContext.BeginEdit();
                    gridContext.CancelEdit();
                    foreach (var item in this.Cells)
                    {
                        var be = item.GetBindingExpression(Cell.ContentProperty);
                        be?.UpdateTarget();
                    }
                }
            }, DispatcherPriority.Input);
        }

        private void Insert_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.IsBeingEdited == false)
            {
                e.CanExecute = true;
            }
        }

        private void Insert_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.RequestInsertRow();
        }

        private void InsertFromClipboard_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsText();
        }

        private void InsertFromClipboard_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.RequestInsertRowFromClipboard();
        }

        private void AttachInsertBinding()
        {
            for (var i = 0; i < this.CommandBindings.Count; i++)
            {
                var item = this.CommandBindings[i];
                if (item.Command == DataGridCommands.EndEdit)
                {
                    this.CommandBindings[i] = new CommandBinding(DataGridCommands.EndEdit, Insert_Execute, Insert_CanExecute);
                    return;
                }
            }
        }

        private void RequestInsertRow()
        {
            try
            {
                this.InsertRow();
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
        }

        private void RequestInsertRowFromClipboard()
        {
            try
            {
                this.InsertRowFromClipboard();
            }
            catch (Exception ex)
            {
                AppMessageBox.ShowError(ex);
            }
        }

        private void Select(DomainRowInfo[] domainRows)
        {
            var gridContext = DataGridControl.GetDataGridContext(this);
            var gridControl = gridContext.DataGridControl as DomainDataGridControl;

            var itemList = new List<object>(domainRows.Length);
            foreach (var domainRow in domainRows)
            {
                for (var i = gridContext.Items.Count - 1; i >= 0; i--)
                {
                    var item = gridContext.Items.GetItemAt(i);
                    var keys = CremaDataRowUtility.GetKeys(item);
                    if (keys.SequenceEqual(domainRow.Keys) == true)
                    {
                        itemList.Add(item);
                    }
                }
            }

            gridContext.SelectedCellRanges.Clear();
            foreach (var item in itemList)
            {
                var index = gridContext.Items.IndexOf(item);
                gridContext.SelectedCellRanges.Add(new SelectionCellRange(index, 0, index, gridContext.VisibleColumns.Count - 1));
            }
            gridContext.CurrentItem = itemList.FirstOrDefault();
            gridContext.FocusCurrent();
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset && sender is INotifyCollectionChanged items)
            {
                items.CollectionChanged -= Items_CollectionChanged;
            }

            this.Dispatcher.InvokeAsync(() =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {

                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    this.Select(this.results);
                    this.results = null;
                    this.hasField = false;
                    this.OnInserted(EventArgs.Empty);
                }
            });
        }

        private object[] CreateFields()
        {
            for (var i = 0; i < this.columnNames.Length; i++)
            {
                var cell = this.Cells[this.columnNames[i]];
                if (cell == null || cell.ReadOnly == true || cell.Content == null)
                    continue;

                this.itemArray[i] = cell.Content;
            }
            return this.itemArray;
        }

        internal void SetField(string fieldName, object content)
        {
            if (this.columnNames == null)
                return;
            for (var i = 0; i < this.columnNames.Length; i++)
            {
                if (this.columnNames[i] == fieldName)
                {
                    this.itemArray[i] = content;
                }
            }
        }

        internal object GetField(string fieldName)
        {
            if (this.columnNames == null || this.hasField == false)
                return null;
            for (var i = 0; i < this.columnNames.Length; i++)
            {
                if (this.columnNames[i] == fieldName)
                {
                    return this.itemArray[i];
                }
            }
            return null;
        }

        internal void ResetField(string fieldName)
        {
            if (this.columnNames == null)
                return;
            for (var i = 0; i < this.columnNames.Length; i++)
            {
                if (this.columnNames[i] == fieldName)
                {
                    this.itemArray[i] = this.defaultArray[i];
                }
            }
        }

        internal bool IsBeginEnding
        {
            get;
            set;
        }
    }
}
