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
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.DataGrid;

namespace Ntreev.Crema.Client.Framework.Controls
{
    class DomainInsertionCell : InsertionCell
    {
        public static readonly DependencyProperty EditingContentProperty =
            DependencyProperty.Register(nameof(EditingContent), typeof(object), typeof(DomainInsertionCell),
                new FrameworkPropertyMetadata(null));

        public DomainInsertionCell(DomainInsertionRow parentRow)
        {
            this.CommandBindings.Add(new CommandBinding(DataGridCommands.EndEdit, this.EndEdit_Execute, this.EndEdit_CanExecute));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, this.Reset_Execute, this.Reset_CanExecute));
            this.CommandBindings.Insert(0, new CommandBinding(ModernDataGridCommands.MoveToNextColumn, this.MoveToNextColumn_Execute, this.MoveToNextColumn_CanExecute));
            this.CommandBindings.Insert(0, new CommandBinding(ModernDataGridCommands.MoveToPrevColumn, this.MoveToPrevColumn_Execute, this.MoveToPrevColumn_CanExecute));
            parentRow.Inserted += ParentRow_Inserted;
        }

        public DomainDataGridControl GridControl
        {
            get { return (DomainDataGridControl)this.GridContext.DataGridControl; }
        }

        public DataGridContext GridContext
        {
            get { return (DataGridContext)this.GetValue(DataGridControl.DataGridContextProperty); }
        }

        public object EditingContent
        {
            get { return (object)GetValue(EditingContentProperty); }
            set { SetValue(EditingContentProperty, value); }
        }

        public bool CanReset
        {
            get
            {
                if (this.ReadOnly == true)
                    return false;

                if (this.IsBeingEdited == true)
                    return false;

                return this.IsCurrent;
            }
        }

        protected override void OnEditEnding(CancelRoutedEventArgs e)
        {
            base.OnEditEnding(e);
            this.Content = this.EditingContent;
        }

        protected override void OnEditEnded()
        {
            base.OnEditEnded();
            var parentRow = this.ParentRow as DomainInsertionRow;
            if (parentRow.IsBeginEnding == false)
            {
                parentRow.EndEdit();
                parentRow.SetField(this.FieldName, this.Content);
            }
        }

        protected override void OnEditCanceled()
        {
            base.OnEditCanceled();
            this.EditingContent = this.Content;
            var parentRow = this.ParentRow as DomainInsertionRow;
            if (parentRow.IsBeginEnding == false)
                this.ParentRow.CancelEdit();
            this.Focus();
        }

        protected override void PrepareContainer(DataGridContext dataGridContext, object item)
        {
            try
            {
                var parentRow = this.ParentRow as DomainInsertionRow;
                var field = parentRow.GetField(this.FieldName);

                if (this.ParentRow.IsBeingEdited == false)
                {
                    base.PrepareContainer(dataGridContext, item);
                    this.Content = field;
                    this.EditingContent = field;
                }
            }
            catch { };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
        }

        private void EndEdit_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.EndEdit();
        }

        private void EndEdit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.IsBeingEdited;
        }

        private void Reset_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.CanReset;
        }

        private void Reset_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (AppMessageBox.ShowQuestion(Properties.Resources.Message_ConfirmToResetField) == false)
                return;

            var parentRow = this.ParentRow as DomainInsertionRow;
            parentRow.ResetField(this.FieldName);
            this.Content = this.EditingContent = parentRow.GetField(this.FieldName);
        }

        private void ParentRow_Inserted(object sender, EventArgs e)
        {
            this.EditingContent = null;
            this.Content = null;
        }

        private void MoveToNextColumn_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MoveToNextColumn_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var gridContext = DataGridControl.GetDataGridContext(this);
            var index = gridContext.VisibleColumns.IndexOf(this.ParentColumn);
            if (gridContext.VisibleColumns.Count != (index + 1))
            {
                var newColumn = gridContext.VisibleColumns[index + 1];
                gridContext.CurrentColumn = newColumn;
                gridContext.FocusCurrent();
            }
        }

        private void MoveToPrevColumn_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MoveToPrevColumn_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var gridContext = DataGridControl.GetDataGridContext(this);
            var index = gridContext.VisibleColumns.IndexOf(this.ParentColumn);
            if (index != 0)
            {
                var newColumn = gridContext.VisibleColumns[index - 1];
                gridContext.CurrentColumn = newColumn;
                gridContext.FocusCurrent();
            }
        }
    }
}
