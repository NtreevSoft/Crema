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

using Ntreev.Library.Linq;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Diff;
using Ntreev.Crema.Presentation.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Views;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Presentation.Controls.Actions;
using Ntreev.ModernUI.Framework.Controls;
using System.Windows.Threading;

namespace Ntreev.Crema.Presentation.Controls
{
    [TemplatePart(Name = PART_DataGrid1String, Type = typeof(Control))]
    [TemplatePart(Name = PART_DataGrid2String, Type = typeof(Control))]
    [TemplatePart(Name = PART_MiddleScrollBarString, Type = typeof(DiffVertScrollBar))]
    [TemplatePart(Name = PART_HorzScrollBar1String, Type = typeof(DiffHorzScrollBar))]
    [TemplatePart(Name = PART_HorzScrollBar2String, Type = typeof(DiffHorzScrollBar))]
    [TemplatePart(Name = PART_VertMinimap1String, Type = typeof(DiffVertMinimap))]
    [TemplatePart(Name = PART_VertMinimap2String, Type = typeof(DiffVertMinimap))]
    [TemplatePart(Name = PART_HorzMinimap1String, Type = typeof(DiffHorzMinimap))]
    [TemplatePart(Name = PART_HorzMinimap2String, Type = typeof(DiffHorzMinimap))]
    public class DiffDataControl : Control
    {
        public const string PART_DataGrid1String = "PART_DataGrid1";
        public const string PART_DataGrid2String = "PART_DataGrid2";
        public const string PART_MiddleScrollBarString = "PART_MiddleScrollBar";
        public const string PART_HorzScrollBar1String = "PART_HorzScrollBar1";
        public const string PART_HorzScrollBar2String = "PART_HorzScrollBar2";
        public const string PART_VertMinimap1String = "PART_VertMinimap1";
        public const string PART_VertMinimap2String = "PART_VertMinimap2";
        public const string PART_HorzMinimap1String = "PART_HorzMinimap1";
        public const string PART_HorzMinimap2String = "PART_HorzMinimap2";

        public static readonly DependencyProperty DataContentTemplateProperty =
            DependencyProperty.Register(nameof(DataContentTemplate), typeof(DataTemplate), typeof(DiffDataControl),
                new FrameworkPropertyMetadata(null));

        //public static readonly DependencyProperty UndoServiceProperty =
        //    DependencyProperty.RegisterAttached(nameof(UndoService), typeof(IUndoService), typeof(DiffDataTypeControl),
        //        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        internal static readonly DependencyPropertyKey ReadOnly1PropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ReadOnly1), typeof(bool), typeof(DiffDataControl),
                new UIPropertyMetadata(false));
        public static readonly DependencyProperty ReadOnly1Property = ReadOnly1PropertyKey.DependencyProperty;

        internal static readonly DependencyPropertyKey ReadOnly2PropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ReadOnly2), typeof(bool), typeof(DiffDataControl),
                new UIPropertyMetadata(false));
        public static readonly DependencyProperty ReadOnly2Property = ReadOnly2PropertyKey.DependencyProperty;

        public static readonly DependencyProperty Header1Property =
            DependencyProperty.Register(nameof(Header1), typeof(object), typeof(DiffDataControl));

        public static readonly DependencyProperty Header2Property =
            DependencyProperty.Register(nameof(Header2), typeof(object), typeof(DiffDataControl));

        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register(nameof(ReadOnly), typeof(bool), typeof(DiffDataControl));

        public static readonly DependencyProperty IsModifiedProperty =
            DependencyProperty.Register(nameof(IsModified), typeof(bool), typeof(DiffDataControl));

        private Control dataGrid1;
        private Control dataGrid2;
        private DiffVertScrollBar middleScrollBar;
        private DiffHorzScrollBar horzScrollBar1;
        private DiffHorzScrollBar horzScrollBar2;
        private DiffVertMinimap vertMinimap1;
        private DiffVertMinimap vertMinimap2;
        private DiffHorzMinimap horzMinimap1;
        private DiffHorzMinimap horzMinimap2;

        private DiffDataGridControl gridControl1;
        private DiffDataGridControl gridControl2;
        private List<DiffItemInfo> diffItems = new List<DiffItemInfo>();
        private List<object> visibleItems = new List<object>();

        public DiffDataControl()
        {
            this.CommandBindings.Add(new CommandBinding(DiffCommands.PrevDifferenceItem, PrevDifferenceItem_Execute, PrevDifferenceItem_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.NextDifferenceItem, NextDifferenceItem_Execute, NextDifferenceItem_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.PrevDifferenceField, PrevDifferenceField_Execute, PrevDifferenceField_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.NextDifferenceField, NextDifferenceField_Execute, NextDifferenceField_CanExecute));

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, Undo_Execute, Undo_CanExecute));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, Redo_Execute, Redo_CanExecute));

            this.CommandBindings.Add(new CommandBinding(DiffCommands.AddItemToRightSide, AddItemToRightSide_Execute, AddItemToRightSide_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.AddItemToLeftSide, AddItemToLeftSide_Execute, AddItemToLeftSide_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.CopyItemToRightSide, CopyItemToRightSide_Execute, CopyItemToRightSide_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.CopyItemToLeftSide, CopyItemToLeftSide_Execute, CopyItemToLeftSide_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.CopyFieldToRightSide, CopyFieldToRightSide_Execute, CopyFieldToRightSide_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.CopyFieldToLeftSide, CopyFieldToLeftSide_Execute, CopyFieldToLeftSide_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.DeleteItemOfRightSide, DeleteItemOfRightSide_Execute, DeleteItemOfRightSide_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.DeleteItemOfLeftSide, DeleteItemOfLeftSide_Execute, DeleteItemOfLeftSide_CanExecute));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.Dispatcher.InvokeAsync(() =>
            {
                if (DiffUndoService.GetUndoService(this) == null)
                {
                    DiffUndoService.SetUndoService(this, new UndoService());
                }
                var undoService = DiffUndoService.GetUndoService(this);
                undoService.Changed += UndoService_Changed;
            }, DispatcherPriority.DataBind);
        }

        //public static IUndoService GetUndoService(DependencyObject obj)
        //{
        //    return (IUndoService)obj.GetValue(UndoServiceProperty);
        //}

        //public static void SetUndoService(DependencyObject obj, IUndoService value)
        //{
        //    obj.SetValue(UndoServiceProperty, value);
        //}

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.dataGrid1 = this.Template.FindName(PART_DataGrid1String, this) as Control;
            this.dataGrid2 = this.Template.FindName(PART_DataGrid2String, this) as Control;
            this.middleScrollBar = this.Template.FindName(PART_MiddleScrollBarString, this) as DiffVertScrollBar;
            this.horzScrollBar1 = this.Template.FindName(PART_HorzScrollBar1String, this) as DiffHorzScrollBar;
            this.horzScrollBar2 = this.Template.FindName(PART_HorzScrollBar2String, this) as DiffHorzScrollBar;
            this.vertMinimap1 = this.Template.FindName(PART_VertMinimap1String, this) as DiffVertMinimap;
            this.vertMinimap2 = this.Template.FindName(PART_VertMinimap2String, this) as DiffVertMinimap;
            this.horzMinimap1 = this.Template.FindName(PART_HorzMinimap1String, this) as DiffHorzMinimap;
            this.horzMinimap2 = this.Template.FindName(PART_HorzMinimap2String, this) as DiffHorzMinimap;

            this.dataGrid1.ApplyTemplate();
            this.gridControl1 = this.GetGridControl(this.dataGrid1);
            this.dataGrid2.ApplyTemplate();
            this.gridControl2 = this.GetGridControl(this.dataGrid2);

            if (this.gridControl1 != null)
            {
                this.gridControl1.VerticalSyncScrollBar = this.middleScrollBar;
                this.gridControl1.HorizontalSyncScrollBar = this.horzScrollBar1;
                this.gridControl1.DestControl = this.gridControl2;
                this.horzMinimap1.GridControl = this.gridControl1;
                this.horzScrollBar1.GridControl = this.gridControl1;
                this.vertMinimap1.GridControl = this.gridControl1;
                this.middleScrollBar.GridControl1 = this.gridControl1;
            }
            if (this.gridControl2 != null)
            {
                this.gridControl2.HorizontalSyncScrollBar = this.horzScrollBar2;
                this.gridControl2.DestControl = this.gridControl1;
                this.horzMinimap2.GridControl = this.gridControl2;
                this.horzScrollBar2.GridControl = this.gridControl2;
                this.vertMinimap2.GridControl = this.gridControl2;
                this.middleScrollBar.GridControl2 = this.gridControl2;
            }
        }

        public Control DataControl1
        {
            get { return this.dataGrid1; }
        }

        public Control DataControl2
        {
            get { return this.dataGrid2; }
        }

        public DataTemplate DataContentTemplate
        {
            get { return (DataTemplate)this.GetValue(DataContentTemplateProperty); }
            set { this.SetValue(DataContentTemplateProperty, value); }
        }

        public bool ReadOnly
        {
            get { return (bool)this.GetValue(ReadOnlyProperty); }
            set { this.SetValue(ReadOnlyProperty, value); }
        }

        public bool ReadOnly1
        {
            get { return (bool)this.GetValue(ReadOnly1Property); }
        }

        public bool ReadOnly2
        {
            get { return (bool)this.GetValue(ReadOnly2Property); }
        }

        public object Header1
        {
            get { return (object)this.GetValue(Header1Property); }
            set { this.SetValue(Header1Property, value); }
        }

        public object Header2
        {
            get { return (object)this.GetValue(Header2Property); }
            set { this.SetValue(Header2Property, value); }
        }

        public bool IsModified
        {
            get { return (bool)this.GetValue(IsModifiedProperty); }
            set { this.SetValue(IsModifiedProperty, value); }
        }

        public virtual bool CanMerge
        {
            get { return false; }
        }

        protected void Merge()
        {
            var undoService = DiffUndoService.GetUndoService(this);
            var transaction = undoService.BeginTransaction("Merge");
            try
            {
                this.MergeInternal();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        protected virtual void RefreshDiffState()
        {

        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (this.middleScrollBar.InputHitTest(e.GetPosition(this.middleScrollBar)) != null)
            {
                if (e.Delta > 0)
                {
                    if (ScrollBar.LineUpCommand != null && ScrollBar.LineUpCommand.CanExecute(null, this.middleScrollBar) == true)
                    {
                        ScrollBar.LineUpCommand.Execute(null, this.middleScrollBar);
                    }
                }
                else if (e.Delta < 0)
                {
                    if (ScrollBar.LineDownCommand != null && ScrollBar.LineDownCommand.CanExecute(null, this.middleScrollBar) == true)
                    {
                        ScrollBar.LineDownCommand.Execute(null, this.middleScrollBar);
                    }
                }
            }
        }

        protected virtual DiffDataGridControl GetGridControl(Control dataContent)
        {
            return dataContent.Template.FindName("PART_DataGridControl", dataContent) as DiffDataGridControl;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.vertMinimap1?.InvalidateMeasure();
            this.vertMinimap2?.InvalidateMeasure();
            this.horzMinimap1?.InvalidateMeasure();
            this.horzMinimap2?.InvalidateMeasure();
            return base.MeasureOverride(constraint);
        }

        private void PrevDifferenceItem_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.gridControl1 != null && this.gridControl1.IsKeyboardFocusWithin)
            {
                Execute(this.gridControl1);
            }
            else if (this.gridControl2 != null && this.gridControl2.IsKeyboardFocusWithin)
            {
                Execute(this.gridControl2);
            }

            void Execute(DataGridControl gridControl)
            {
                var gridContext = DataGridControl.GetDataGridContext(gridControl);
                var items = DiffItemInfo.GetReverseItems(gridContext);
                var itemInfo = items.SkipWhile(i => i.Item != gridControl.GlobalCurrentItem)
                                    .Skip(1)
                                    .FirstOrDefault(i => DiffUtility.GetDiffState(i.Item) != DiffState.Unchanged);

                if (itemInfo.Item == null)
                    return;

                var item = itemInfo.Item;
                gridControl.SelectedItems.Clear();
                gridControl.SelectedItems.Add(item);
                itemInfo.GridContext.CurrentItem = item;
            }
        }

        private void PrevDifferenceItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.gridControl1 != null && this.gridControl1.IsKeyboardFocusWithin)
            {
                e.CanExecute = Verify(this.gridControl1);
            }
            else if (this.gridControl2 != null && this.gridControl2.IsKeyboardFocusWithin)
            {
                e.CanExecute = Verify(this.gridControl2);
            }

            bool Verify(DataGridControl gridControl)
            {
                var gridContext = DataGridControl.GetDataGridContext(gridControl);
                var items = DiffItemInfo.GetReverseItems(gridContext);
                var itemInfo = items.SkipWhile(i => i.Item != gridControl.GlobalCurrentItem)
                                    .Skip(1)
                                    .FirstOrDefault(i => DiffUtility.GetDiffState(i.Item) != DiffState.Unchanged);

                return itemInfo.Item != null;
            }
        }

        private void NextDifferenceItem_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.gridControl1 != null && this.gridControl1.IsKeyboardFocusWithin)
            {
                Execute(this.gridControl1);
            }
            else if (this.gridControl2 != null && this.gridControl2.IsKeyboardFocusWithin)
            {
                Execute(this.gridControl2);
            }

            void Execute(DataGridControl gridControl)
            {
                var gridContext = DataGridControl.GetDataGridContext(gridControl);
                var items = DiffItemInfo.GetItems(gridContext);
                var itemInfo = items.SkipWhile(i => i.Item != gridControl.GlobalCurrentItem)
                                    .Skip(1)
                                    .FirstOrDefault(i => DiffUtility.GetDiffState(i.Item) != DiffState.Unchanged);

                if (itemInfo.Item == null)
                    return;

                var item = itemInfo.Item;
                gridControl.SelectedItems.Clear();
                gridControl.SelectedItems.Add(item);
                itemInfo.GridContext.CurrentItem = item;
            }
        }

        private void NextDifferenceItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.gridControl1 != null && this.gridControl1.IsKeyboardFocusWithin)
            {
                e.CanExecute = Verify(this.gridControl1);
            }
            else if (this.gridControl2 != null && this.gridControl2.IsKeyboardFocusWithin)
            {
                e.CanExecute = Verify(this.gridControl2);
            }

            bool Verify(DataGridControl gridControl)
            {
                var gridContext = DataGridControl.GetDataGridContext(gridControl);
                var items = DiffItemInfo.GetItems(gridContext);
                var itemInfo = items.SkipWhile(i => i.Item != gridControl.GlobalCurrentItem)
                                    .Skip(1)
                                    .FirstOrDefault(i => DiffUtility.GetDiffState(i.Item) != DiffState.Unchanged);
                return itemInfo.Item != null;
            }
        }

        private void PrevDifferenceField_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.gridControl1 != null && this.gridControl1.IsKeyboardFocusWithin)
            {
                Execute(this.gridControl1);
            }
            else if (this.gridControl2 != null && this.gridControl2.IsKeyboardFocusWithin)
            {
                Execute(this.gridControl2);
            }

            void Execute(DataGridControl gridControl)
            {
                var gridContext = gridControl.CurrentContext;
                var diffFields = DiffUtility.GetDiffFields(gridContext.CurrentItem);
                var column = gridContext.VisibleColumns.Reverse().SkipWhile(i => i != gridContext.CurrentColumn)
                                        .Skip(1)
                                        .FirstOrDefault(i => diffFields.Contains(i.FieldName) == true);

                if (column == null)
                    return;

                gridContext.CurrentColumn = column;
            }
        }

        private void PrevDifferenceField_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.gridControl1 != null && this.gridControl1.IsKeyboardFocusWithin)
            {
                e.CanExecute = Verify(this.gridControl1);
            }
            else if (this.gridControl2 != null && this.gridControl2.IsKeyboardFocusWithin)
            {
                e.CanExecute = Verify(this.gridControl2);
            }

            bool Verify(DataGridControl gridControl)
            {
                var gridContext = gridControl.CurrentContext;
                var diffState = DiffUtility.GetDiffState(gridContext.CurrentItem);
                if (diffState != DiffState.Modified)
                    return false;
                var diffFields = DiffUtility.GetDiffFields(gridContext.CurrentItem);

                var column = gridContext.VisibleColumns.Reverse().SkipWhile(i => i != gridContext.CurrentColumn)
                                        .Skip(1)
                                        .FirstOrDefault(i => diffFields.Contains(i.FieldName) == true);
                return column != null;
            }
        }

        private void NextDifferenceField_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.gridControl1 != null && this.gridControl1.IsKeyboardFocusWithin)
            {
                Execute(this.gridControl1);
            }
            else if (this.gridControl2 != null && this.gridControl2.IsKeyboardFocusWithin)
            {
                Execute(this.gridControl2);
            }

            void Execute(DataGridControl gridControl)
            {
                var gridContext = gridControl.CurrentContext;
                var diffFields = DiffUtility.GetDiffFields(gridContext.CurrentItem);
                var column = gridContext.VisibleColumns.SkipWhile(i => i != gridContext.CurrentColumn)
                                        .Skip(1)
                                        .FirstOrDefault(i => diffFields.Contains(i.FieldName) == true);

                if (column == null)
                    return;

                gridContext.CurrentColumn = column;
            }
        }

        private void NextDifferenceField_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.gridControl1 != null && this.gridControl1.IsKeyboardFocusWithin)
            {
                e.CanExecute = Verify(this.gridControl1);
            }
            else if (this.gridControl2 != null && this.gridControl2.IsKeyboardFocusWithin)
            {
                e.CanExecute = Verify(this.gridControl2);
            }

            bool Verify(DataGridControl gridControl)
            {
                var gridContext = gridControl.CurrentContext;
                var diffState = DiffUtility.GetDiffState(gridContext.CurrentItem);
                if (diffState != DiffState.Modified)
                    return false;
                var diffFields = DiffUtility.GetDiffFields(gridContext.CurrentItem);

                var column = gridContext.VisibleColumns.SkipWhile(i => i != gridContext.CurrentColumn)
                                        .Skip(1)
                                        .FirstOrDefault(i => diffFields.Contains(i.FieldName) == true);
                return column != null;
            }
        }

        private void AddItemToRightSide_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                this.AddItemToRightSide(cell);
            }
        }

        private void AddItemToRightSide_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                e.CanExecute = this.VerifyAddItemToRightSide(cell);
            }
        }

        private void AddItemToLeftSide_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                this.AddItemToLeftSide(cell);
            }
        }

        private void AddItemToLeftSide_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                e.CanExecute = this.VerifyAddItemToLeftSide(cell);
            }
        }

        private void CopyItemToRightSide_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                this.CopyItemToRightSide(cell);
            }
        }

        private void CopyItemToRightSide_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                e.CanExecute = this.VerifyCopyItemToRightSide(cell);
            }
        }

        private void CopyItemToLeftSide_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                this.CopyItemToLeftSide(cell);
            }
        }

        private void CopyItemToLeftSide_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                e.CanExecute = this.VerifyCopyItemToLeftSide(cell);
            }
        }

        private void CopyFieldToRightSide_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                this.CopyFieldToRightSide(cell);
            }
        }

        private void CopyFieldToRightSide_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                e.CanExecute = this.VerifyCopyFieldToRightSide(cell);
            }
        }

        private void CopyFieldToLeftSide_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                this.CopyFieldToLeftSide(cell);
            }
        }

        private void CopyFieldToLeftSide_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                e.CanExecute = this.VerifyCopyFieldToLeftSide(cell);
            }
        }

        private void DeleteItemOfRightSide_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                this.DeleteItemOfRightSide(cell);
            }
        }

        private void DeleteItemOfRightSide_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                e.CanExecute = this.VerifyDeleteItemOfRightSide(cell);
            }
        }

        private void DeleteItemOfLeftSide_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                this.DeleteItemOfLeftSide(cell);
            }
        }

        private void DeleteItemOfLeftSide_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.OriginalSource is DiffDataCell cell)
            {
                e.CanExecute = this.VerifyDeleteItemOfLeftSide(cell);
            }
        }

        private void MergeInternal()
        {
            var undoService = DiffUndoService.GetUndoService(this);
            for (var i = this.gridControl2.Items.Count - 1; i >= 0; i--)
            {
                var dataItem = this.gridControl2.Items.GetItemAt(i);
                var diffState = DiffUtility.GetDiffState(dataItem);
                if (diffState == DiffState.Modified)
                {
                    var key = DiffUtility.GetItemKey(dataItem);
                    var destItem = this.gridControl1.FindItem(this.gridControl1.Items, key);
                    undoService.Execute(new CopyItemAction(dataItem, destItem));
                }
            }

            for (var i = 0; i < this.gridControl2.Items.Count; i++)
            {
                var dataItem = this.gridControl2.Items.GetItemAt(i);
                var diffState = DiffUtility.GetDiffState(dataItem);
                if (diffState == DiffState.Inserted)
                {
                    var destItem = this.gridControl1.Items.GetItemAt(i);
                    var destState = DiffUtility.GetDiffState(destItem);
                    if (destState == DiffState.Imaginary)
                    {
                        undoService.Execute(new CopyItemAction(dataItem, destItem));
                    }
                }
            }

            for (var i = 0; i < this.gridControl1.Items.Count; i++)
            {
                var dataItem = this.gridControl1.Items.GetItemAt(i);
                var diffState = DiffUtility.GetDiffState(dataItem);
                if (diffState == DiffState.Deleted)
                {
                    var destItem = this.gridControl2.Items.GetItemAt(i);
                    var destState = DiffUtility.GetDiffState(destItem);
                    if (destState == DiffState.Imaginary)
                    {
                        undoService.Execute(new DeleteItemAction(dataItem));
                    }
                }
            }
            this.InvalidateMeasure();
        }

        private void AddItemToRightSide(object dataItem)
        {
            this.ValidateAddItemToRightSide(dataItem);

            var gridControl = this.gridControl1;
            var destControl = this.gridControl2;
            var gridContext = gridControl.FindContext(dataItem);
            var destContext = destControl.FindContext(gridContext);

            var undoService = DiffUndoService.GetUndoService(this);
            var action = new AddItemAction(gridContext, destContext, dataItem)
            {
                Selector = DiffUndoService.GetSelector(this),
                SelectedItem = this.DataContext
            };
            undoService.Execute(action);
        }

        private void AddItemToLeftSide(object dataItem)
        {
            this.ValidateAddItemToLeftSide(dataItem);

            var gridControl = this.gridControl2;
            var destControl = this.gridControl1;
            var gridContext = gridControl.FindContext(dataItem);
            var destContext = destControl.FindContext(gridContext);

            var undoService = DiffUndoService.GetUndoService(this);
            var action = new AddItemAction(gridContext, destContext, dataItem)
            {
                Selector = DiffUndoService.GetSelector(this),
                SelectedItem = this.DataContext
            };
            undoService.Execute(action);
        }

        private void CopyItemToRightSide(object dataItem)
        {
            this.ValidateCopyItemToRightSide(dataItem);

            var gridControl = this.gridControl1;
            var destControl = this.gridControl2;
            var gridContext = gridControl.FindContext(dataItem);
            var index = gridContext.Items.IndexOf(dataItem);
            var destContext = destControl.FindContext(gridContext);
            var diffState = DiffUtility.GetDiffState(dataItem);
            var destItem = diffState == DiffState.Modified ? destControl.FindItem(destContext.Items, DiffUtility.GetItemKey(dataItem)) : destContext.Items.GetItemAt(index);

            var undoService = DiffUndoService.GetUndoService(this);
            var action = new CopyItemAction(dataItem, destItem)
            {
                Selector = DiffUndoService.GetSelector(this),
                SelectedItem = this.DataContext
            };
            undoService.Execute(action);
        }

        private void CopyItemToLeftSide(object dataItem)
        {
            this.ValidateCopyItemToLeftSide(dataItem);

            var gridControl = this.gridControl2;
            var destControl = this.gridControl1;
            var gridContext = gridControl.FindContext(dataItem);
            var index = gridContext.Items.IndexOf(dataItem);
            var destContext = destControl.FindContext(gridContext);
            var diffState = DiffUtility.GetDiffState(dataItem);
            var destItem = diffState == DiffState.Modified ? destControl.FindItem(destContext.Items, DiffUtility.GetItemKey(dataItem)) : destContext.Items.GetItemAt(index);

            var undoService = DiffUndoService.GetUndoService(this);
            var action = new CopyItemAction(dataItem, destItem)
            {
                Selector = DiffUndoService.GetSelector(this),
                SelectedItem = this.DataContext
            };
            undoService.Execute(action);
        }

        private void CopyFieldToRightSide(object dataItem, string fieldName)
        {
            this.ValidateCopyFieldToRightSide(dataItem, fieldName);

            var gridControl = this.gridControl1;
            var destControl = this.gridControl2;
            var gridContext = gridControl.FindContext(dataItem);
            var index = gridContext.Items.IndexOf(dataItem);
            var destContext = destControl.FindContext(gridContext);
            var diffState = DiffUtility.GetDiffState(dataItem);
            var destItem = diffState == DiffState.Modified ? destControl.FindItem(destContext.Items, DiffUtility.GetItemKey(dataItem)) : destContext.Items.GetItemAt(index);

            var undoService = DiffUndoService.GetUndoService(this);
            var action = new CopyFieldAction(dataItem, destItem, fieldName)
            {
                Selector = DiffUndoService.GetSelector(this),
                SelectedItem = this.DataContext
            };
            undoService.Execute(action);
        }

        private void CopyFieldToLeftSide(object dataItem, string fieldName)
        {
            this.ValidateCopyFieldToLeftSide(dataItem, fieldName);

            var gridControl = this.gridControl2;
            var destControl = this.gridControl1;
            var gridContext = gridControl.FindContext(dataItem);
            var index = gridContext.Items.IndexOf(dataItem);
            var destContext = destControl.FindContext(gridContext);
            var diffState = DiffUtility.GetDiffState(dataItem);
            var destItem = diffState == DiffState.Modified ? destControl.FindItem(destContext.Items, DiffUtility.GetItemKey(dataItem)) : destContext.Items.GetItemAt(index);

            var undoService = DiffUndoService.GetUndoService(this);
            var action = new CopyFieldAction(dataItem, destItem, fieldName)
            {
                Selector = DiffUndoService.GetSelector(this),
                SelectedItem = this.DataContext
            };
            undoService.Execute(action);
        }

        private void DeleteItemOfRightSide(object dataItem)
        {
            this.ValidateDeleteItemOfRightSide(dataItem);

            var undoService = DiffUndoService.GetUndoService(this);
            var action = new DeleteItemAction(dataItem)
            {
                Selector = DiffUndoService.GetSelector(this),
                SelectedItem = this.DataContext
            };
            undoService.Execute(action);
        }

        private void DeleteItemOfLeftSide(object dataItem)
        {
            this.ValidateDeleteItemOfLeftSide(dataItem);

            var undoService = DiffUndoService.GetUndoService(this);
            var action = new DeleteItemAction(dataItem)
            {
                Selector = DiffUndoService.GetSelector(this),
                SelectedItem = this.DataContext
            };
            undoService.Execute(action);
        }

        private void ValidateAddItemToRightSide(object dataItem)
        {
            if (dataItem is IEditableObject == false)
                throw new ArgumentException(nameof(dataItem));

            var gridContext = this.gridControl1.FindContext(dataItem);
            if (gridContext == null)
                throw new ArgumentException(nameof(dataItem));

            var diffState = DiffUtility.GetDiffState(dataItem);
            if (diffState != DiffState.Deleted)
                throw new ArgumentException(nameof(dataItem));
        }

        private void ValidateAddItemToLeftSide(object dataItem)
        {
            if (dataItem is IEditableObject == false)
                throw new ArgumentException(nameof(dataItem));

            var gridContext = this.gridControl2.FindContext(dataItem);
            if (gridContext == null)
                throw new ArgumentException(nameof(dataItem));

            var diffState = DiffUtility.GetDiffState(dataItem);
            if (diffState != DiffState.Inserted)
                throw new ArgumentException(nameof(dataItem));
        }

        private void ValidateCopyItemToRightSide(object dataItem)
        {
            if (dataItem is IEditableObject == false)
                throw new ArgumentException(nameof(dataItem));

            var gridContext = this.gridControl1.FindContext(dataItem);
            if (gridContext == null)
                throw new ArgumentException(nameof(dataItem));

            var index = gridContext.Items.IndexOf(dataItem);
            var destContext = this.gridControl2.FindContext(gridContext);
            var destItem = destContext.Items.GetItemAt(index);
            var diffState1 = DiffUtility.GetDiffState(dataItem);
            var diffState2 = DiffUtility.GetDiffState(destItem);

            if (diffState1 != DiffState.Modified && diffState1 != DiffState.Deleted && diffState1 != DiffState.Imaginary)
                throw new ArgumentException(nameof(dataItem));
            if (diffState1 == DiffState.Imaginary && diffState2 == DiffState.Modified)
                throw new ArgumentException(nameof(dataItem));
            if (diffState1 == DiffState.Deleted && diffState2 == DiffState.Modified)
                throw new ArgumentException(nameof(dataItem));
        }

        private void ValidateCopyItemToLeftSide(object dataItem)
        {
            if (dataItem is IEditableObject == false)
                throw new ArgumentException(nameof(dataItem));

            var gridContext = this.gridControl2.FindContext(dataItem);
            if (gridContext == null)
                throw new ArgumentException(nameof(dataItem));

            var index = gridContext.Items.IndexOf(dataItem);
            var destContext = this.gridControl1.FindContext(gridContext);
            var destItem = destContext.Items.GetItemAt(index);
            var diffState2 = DiffUtility.GetDiffState(dataItem);
            var diffState1 = DiffUtility.GetDiffState(destItem);

            if (diffState2 != DiffState.Modified && diffState2 != DiffState.Inserted && diffState2 != DiffState.Imaginary)
                throw new ArgumentException(nameof(dataItem));
            if (diffState2 == DiffState.Imaginary && diffState1 == DiffState.Modified)
                throw new ArgumentException(nameof(dataItem));
            if (diffState2 == DiffState.Inserted && diffState1 == DiffState.Modified)
                throw new ArgumentException(nameof(dataItem));
        }

        private void ValidateCopyFieldToRightSide(object dataItem, string fieldName)
        {
            if (dataItem == null)
                throw new ArgumentNullException(nameof(dataItem));
            if (fieldName == null)
                throw new ArgumentNullException(nameof(fieldName));
            if (dataItem is IEditableObject == false)
                throw new ArgumentException(nameof(dataItem));

            var gridContext = this.gridControl1.FindContext(dataItem);
            if (gridContext == null)
                throw new ArgumentException(nameof(dataItem));

            var index = gridContext.Items.IndexOf(dataItem);
            var destContext = this.gridControl2.FindContext(gridContext);
            var destItem = destContext.Items.GetItemAt(index);
            var diffState = DiffUtility.GetDiffState(dataItem);
            var destState = DiffUtility.GetDiffState(destItem);

            if (diffState != DiffState.Modified && destState != DiffState.Modified)
                throw new ArgumentException(nameof(dataItem));

            var diffFields = DiffUtility.GetDiffFields(dataItem);
            if (diffFields.Contains(fieldName) == false)
                throw new ArgumentException(nameof(fieldName));
        }

        private void ValidateCopyFieldToLeftSide(object dataItem, string fieldName)
        {
            if (dataItem == null)
                throw new ArgumentNullException(nameof(dataItem));
            if (fieldName == null)
                throw new ArgumentNullException(nameof(fieldName));
            if (dataItem is IEditableObject == false)
                throw new ArgumentException(nameof(dataItem));

            var gridContext = this.gridControl2.FindContext(dataItem);
            if (gridContext == null)
                throw new ArgumentException(nameof(dataItem));

            var index = gridContext.Items.IndexOf(dataItem);
            var destContext = this.gridControl1.FindContext(gridContext);
            var destItem = destContext.Items.GetItemAt(index);
            var diffState = DiffUtility.GetDiffState(dataItem);
            var destState = DiffUtility.GetDiffState(destItem);

            if (diffState != DiffState.Modified && destState != DiffState.Modified)
                throw new ArgumentException(nameof(dataItem));

            var diffFields = DiffUtility.GetDiffFields(dataItem);
            if (diffFields.Contains(fieldName) == false)
                throw new ArgumentException(nameof(fieldName));
        }

        private void ValidateDeleteItemOfLeftSide(object dataItem)
        {
            if (dataItem is IEditableObject == false)
                throw new ArgumentException(nameof(dataItem));

            var diffState = DiffUtility.GetDiffState(dataItem);
            if (diffState != DiffState.Deleted)
                throw new ArgumentException(nameof(dataItem));
        }

        private void ValidateDeleteItemOfRightSide(object dataItem)
        {
            if (dataItem is IEditableObject == false)
                throw new ArgumentException(nameof(dataItem));

            var diffState = DiffUtility.GetDiffState(dataItem);
            if (diffState != DiffState.Inserted)
                throw new ArgumentException(nameof(dataItem));
        }

        private async void AddItemToRightSide(DiffDataCell cell)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            var transaction = undoService.BeginTransaction(nameof(AddItemToRightSide));
            try
            {
                var gridContext = DataGridControl.GetDataGridContext(cell);
                foreach (var item in gridContext.GetSelectedItems().ToArray())
                {
                    this.AddItemToRightSide(item);
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                AppMessageBox.ShowError(e);
                return;
            }

            await this.Dispatcher.InvokeAsync(() =>
            {
                this.gridControl2.RefreshFocus();
                this.gridControl2.RefreshSelection();
            });
        }

        private async void AddItemToLeftSide(DiffDataCell cell)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            var transaction = undoService.BeginTransaction(nameof(AddItemToLeftSide));
            try
            {
                var gridContext = DataGridControl.GetDataGridContext(cell);
                foreach (var item in gridContext.GetSelectedItems().ToArray())
                {
                    this.AddItemToLeftSide(item);
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                AppMessageBox.ShowError(e);
                return;
            }

            await this.Dispatcher.InvokeAsync(() =>
            {
                this.gridControl1.RefreshFocus();
                this.gridControl1.RefreshSelection();
            });
        }

        private async void CopyItemToRightSide(DiffDataCell cell)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            var transaction = undoService.BeginTransaction(nameof(CopyItemToRightSide));
            try
            {
                var gridContext = DataGridControl.GetDataGridContext(cell);
                foreach (var item in gridContext.GetSelectedItems().ToArray())
                {
                    this.CopyItemToRightSide(item);
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                AppMessageBox.ShowError(e);
                return;
            }

            await this.Dispatcher.InvokeAsync(() =>
            {
                this.gridControl2.RefreshFocus();
                this.gridControl2.RefreshSelection();
            });
        }

        private async void CopyItemToLeftSide(DiffDataCell cell)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            var transaction = undoService.BeginTransaction(nameof(CopyItemToLeftSide));
            try
            {
                var gridContext = DataGridControl.GetDataGridContext(cell);
                foreach (var item in gridContext.GetSelectedItems().ToArray())
                {
                    this.CopyItemToLeftSide(item);
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                AppMessageBox.ShowError(e);
                return;
            }

            await this.Dispatcher.InvokeAsync(() =>
            {
                this.gridControl1.RefreshFocus();
                this.gridControl1.RefreshSelection();
            });
        }

        private async void CopyFieldToRightSide(DiffDataCell cell)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            var transaction = undoService.BeginTransaction(nameof(CopyFieldToRightSide));
            try
            {
                var gridContext = DataGridControl.GetDataGridContext(cell);
                foreach (var item in gridContext.GetSelectedItems().ToArray())
                {
                    this.CopyFieldToRightSide(item, cell.FieldName);
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                AppMessageBox.ShowError(e);
                return;
            }

            await this.Dispatcher.InvokeAsync(() =>
            {
                this.gridControl2.RefreshFocus();
                this.gridControl2.RefreshSelection();
            });
        }

        private async void CopyFieldToLeftSide(DiffDataCell cell)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            var transaction = undoService.BeginTransaction(nameof(CopyFieldToLeftSide));
            try
            {
                var gridContext = DataGridControl.GetDataGridContext(cell);
                foreach (var item in gridContext.GetSelectedItems().ToArray())
                {
                    this.CopyFieldToLeftSide(item, cell.FieldName);
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                AppMessageBox.ShowError(e);
                return;
            }

            await this.Dispatcher.InvokeAsync(() =>
            {
                this.gridControl1.RefreshFocus();
                this.gridControl1.RefreshSelection();
            });
        }

        private async void DeleteItemOfRightSide(DiffDataCell cell)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            var transaction = undoService.BeginTransaction(nameof(DeleteItemOfRightSide));
            try
            {
                var gridContext = DataGridControl.GetDataGridContext(cell);
                foreach (var item in gridContext.GetSelectedItems().Reverse().ToArray())
                {
                    this.DeleteItemOfRightSide(item);
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                AppMessageBox.ShowError(e);
                return;
            }

            await this.Dispatcher.InvokeAsync(() =>
            {
                this.gridControl1.RefreshFocus();
                this.gridControl1.RefreshSelection();
            });
        }

        private async void DeleteItemOfLeftSide(DiffDataCell cell)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            var transaction = undoService.BeginTransaction(nameof(DeleteItemOfLeftSide));
            try
            {
                var gridContext = DataGridControl.GetDataGridContext(cell);
                foreach (var item in gridContext.GetSelectedItems().Reverse().ToArray())
                {
                    this.DeleteItemOfLeftSide(item);
                }
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                AppMessageBox.ShowError(e);
                return;
            }

            await this.Dispatcher.InvokeAsync(() =>
            {
                this.gridControl2.RefreshFocus();
                this.gridControl2.RefreshSelection();
            });
        }

        private bool VerifyAddItemToRightSide(DiffDataCell cell)
        {
            if (this.CanMerge == false)
                return false;
            var gridContext = DataGridControl.GetDataGridContext(cell);
            foreach (var item in gridContext.GetSelectedItems())
            {
                var diffState = DiffUtility.GetDiffState(item);
                if (diffState != DiffState.Deleted)
                    return false;
                var index = gridContext.Items.IndexOf(item);
                var gridControl2 = this.GetGridControl(this.DataControl2);
                var gridContext2 = gridControl2.FindContext(gridContext);
                var destState = DiffUtility.GetDiffState(gridContext2.Items.GetItemAt(index));
                if (destState == DiffState.Imaginary)
                    return false;
            }
            return this.ReadOnly2 == false && this.ReadOnly1 == false;
        }

        private bool VerifyAddItemToLeftSide(DiffDataCell cell)
        {
            if (this.CanMerge == false)
                return false;
            var gridContext = DataGridControl.GetDataGridContext(cell);
            foreach (var item in gridContext.GetSelectedItems())
            {
                var diffState = DiffUtility.GetDiffState(item);
                if (diffState != DiffState.Inserted)
                    return false;
                var index = gridContext.Items.IndexOf(item);
                var gridControl1 = this.GetGridControl(this.DataControl1);
                var gridContext1 = gridControl1.FindContext(gridContext);
                var destState = DiffUtility.GetDiffState(gridContext1.Items.GetItemAt(index));
                if (destState == DiffState.Imaginary)
                    return false;
            }
            return this.ReadOnly1 == false && this.ReadOnly2 == false;
        }

        private bool VerifyCopyItemToRightSide(DiffDataCell cell)
        {
            if (this.CanMerge == false)
                return false;
            var gridContext = DataGridControl.GetDataGridContext(cell);
            var destControl = this.GetGridControl(this.DataControl2);

            foreach (var item in gridContext.GetSelectedItems())
            {
                var diffState = DiffUtility.GetDiffState(item);
                if (diffState != DiffState.Modified && diffState != DiffState.Deleted && diffState != DiffState.Imaginary)
                    return false;

                var destContext = destControl.FindContext(gridContext);
                var index = gridContext.Items.IndexOf(item);
                var destItem = destContext.Items.GetItemAt(index);
                var destState = DiffUtility.GetDiffState(destItem);
                if (diffState != DiffState.Modified && destState == DiffState.Modified)
                    return false;
            }
            return this.ReadOnly2 == false;
        }

        private bool VerifyCopyItemToLeftSide(DiffDataCell cell)
        {
            if (this.CanMerge == false)
                return false;
            var gridContext = DataGridControl.GetDataGridContext(cell);
            var destControl = this.GetGridControl(this.DataControl1);

            foreach (var item in gridContext.GetSelectedItems())
            {
                var diffState = DiffUtility.GetDiffState(item);
                if (diffState != DiffState.Modified && diffState != DiffState.Inserted && diffState != DiffState.Imaginary)
                    return false;

                var destContext = destControl.FindContext(gridContext);
                var index = gridContext.Items.IndexOf(item);
                var destItem = destContext.Items.GetItemAt(index);
                var destState = DiffUtility.GetDiffState(destItem);
                if (diffState != DiffState.Modified && destState == DiffState.Modified)
                    return false;
            }
            return this.ReadOnly1 == false;
        }

        private bool VerifyCopyFieldToRightSide(DiffDataCell cell)
        {
            if (this.CanMerge == false)
                return false;

            var gridContext = DataGridControl.GetDataGridContext(cell);
            var destControl = this.GetGridControl(this.DataControl2);

            var diffState = DiffUtility.GetDiffState(gridContext.CurrentItem);
            if (diffState != DiffState.Modified)
                return false;

            var diffFields = DiffUtility.GetDiffFields(gridContext.CurrentItem);
            if (diffFields.Contains(cell.FieldName) == false)
                return false;

            var destContext = destControl.FindContext(gridContext);
            var index = gridContext.Items.IndexOf(gridContext.CurrentItem);
            var destItem = destContext.Items.GetItemAt(index);
            var destState = DiffUtility.GetDiffState(destItem);
            if (destState != DiffState.Modified)
                return false;

            return this.ReadOnly2 == false;
        }

        private bool VerifyCopyFieldToLeftSide(DiffDataCell cell)
        {
            if (this.CanMerge == false)
                return false;

            var gridContext = DataGridControl.GetDataGridContext(cell);
            var destControl = this.GetGridControl(this.DataControl1);

            var diffState = DiffUtility.GetDiffState(gridContext.CurrentItem);
            if (diffState != DiffState.Modified)
                return false;

            var diffFields = DiffUtility.GetDiffFields(gridContext.CurrentItem);
            if (diffFields.Contains(cell.FieldName) == false)
                return false;

            var destContext = destControl.FindContext(gridContext);
            var index = gridContext.Items.IndexOf(gridContext.CurrentItem);
            var destItem = destContext.Items.GetItemAt(index);
            var destState = DiffUtility.GetDiffState(destItem);
            if (destState != DiffState.Modified)
                return false;

            return this.ReadOnly1 == false;
        }

        private bool VerifyDeleteItemOfRightSide(DiffDataCell cell)
        {
            if (this.CanMerge == false)
                return false;
            var gridContext = DataGridControl.GetDataGridContext(cell);
            foreach (var item in gridContext.GetSelectedItems())
            {
                var diffState = DiffUtility.GetDiffState(item);
                if (diffState != DiffState.Inserted)
                    return false;
            }
            return this.ReadOnly2 == false;
        }

        private bool VerifyDeleteItemOfLeftSide(DiffDataCell cell)
        {
            if (this.CanMerge == false)
                return false;
            var gridContext = DataGridControl.GetDataGridContext(cell);
            foreach (var item in gridContext.GetSelectedItems())
            {
                var diffState = DiffUtility.GetDiffState(item);
                if (diffState != DiffState.Deleted)
                    return false;
            }
            return this.ReadOnly1 == false;
        }

        private void Undo_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            if (undoService != null)
            {
                undoService.Undo();
            }
        }

        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            if (undoService != null)
            {
                e.CanExecute = undoService.CanUndo;
            }
        }

        private void Redo_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            if (undoService != null)
            {
                undoService.Redo();
            }
        }

        private void Redo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            if (undoService != null)
            {
                e.CanExecute = undoService.CanRedo;
            }
        }

        private void UndoService_Changed(object sender, EventArgs e)
        {
            if (sender is IUndoService undoService)
            {
                this.RefreshDiffState();
                this.InvalidateMeasure();
            }
        }
    }
}
