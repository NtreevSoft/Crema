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
using Ntreev.Crema.Data.Diff;
using Ntreev.Crema.Presentation.Controls.Actions;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Views;

namespace Ntreev.Crema.Presentation.Controls
{
    public class DiffDataTypeControl : DiffDataControl
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(DiffDataType), typeof(DiffDataTypeControl),
                new FrameworkPropertyMetadata(null, SourcePropertyChangedCallback));

        public static readonly DependencyProperty ResolveCommandProperty =
            DependencyProperty.Register(nameof(ResolveCommand), typeof(ICommand), typeof(DiffDataTypeControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ResolveCommandParameterProperty =
            DependencyProperty.Register(nameof(ResolveCommandParameter), typeof(object), typeof(DiffDataTypeControl),
                new PropertyMetadata(null));

        public DiffDataTypeControl()
        {
            this.CommandBindings.Add(new CommandBinding(DiffCommands.CopyPropertyToRightSide, CopyPropertyToRightSide_Execute, CopyPropertyToRightSide_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.CopyPropertyToLeftSide, CopyPropertyToLeftSide_Execute, CopyPropertyToLeftSide_CanExecute));

            this.CommandBindings.Add(new CommandBinding(DiffCommands.Merge, Merge_Execute, Merge_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.Resolve, Resolve_Execute, Resolve_CanExecute));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindingOperations.SetBinding(this.DataControl1, CremaDataTypeControl.SourceProperty, new Binding($"{nameof(Source)}.{nameof(DiffDataType.SourceItem1)}") { Source = this, });
            BindingOperations.SetBinding(this.DataControl2, CremaDataTypeControl.SourceProperty, new Binding($"{nameof(Source)}.{nameof(DiffDataType.SourceItem2)}") { Source = this, });
        }

        public new void Merge()
        {
            try
            {
                base.Merge();
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
                return;
            }
        }

        public override bool CanMerge
        {
            get
            {
                if (this.ReadOnly == true)
                    return false;
                if (this.Source == null)
                    return false;
                return this.Source.DiffState != DiffState.Unchanged && this.Source.IsResolved == false;
            }
        }

        public void Resolve()
        {
            try
            {
                var undoService = DiffUndoService.GetUndoService(this);
                this.Source.Resolve();
                undoService.Clear();
                this.InvalidateMeasure();
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
                return;
            }
        }

        public bool CanResolve
        {
            get
            {
                if (this.ReadOnly == true)
                    return false;
                if (this.Source != null)
                    return this.Source.IsResolved == false;
                return false;
            }
        }

        public DiffDataType Source
        {
            get { return (DiffDataType)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        public ICommand ResolveCommand
        {
            get { return (ICommand)this.GetValue(ResolveCommandProperty); }
            set { this.SetValue(ResolveCommandProperty, value); }
        }

        public object ResolveCommandParameter
        {
            get { return (object)this.GetValue(ResolveCommandParameterProperty); }
            set { this.SetValue(ResolveCommandParameterProperty, value); }
        }

        protected override void RefreshDiffState()
        {
            base.RefreshDiffState();
            this.Source.Refresh();
        }

        private void CopyPropertyToRightSide_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            if (this.Source != null && e.Parameter is string propertyName)
            {
                if (propertyName == nameof(CremaDataType.Name))
                {
                    if (this.Source.ItemName2 != this.Source.ItemName1)
                    {
                        var oldValue = this.Source.ItemName2;
                        var redoAction = new Action(() => this.Source.ItemName2 = this.Source.ItemName1);
                        var undoAction = new Action(() => this.Source.ItemName2 = oldValue);
                        undoService.Execute(new DelegateUndoAction(nameof(CremaDataType.Name), redoAction, undoAction));
                    }
                }
                else if (propertyName == nameof(CremaDataType.IsFlag))
                {
                    if (this.Source.IsFlag2 != this.Source.IsFlag1)
                    {
                        var oldValue = this.Source.IsFlag2;
                        var redoAction = new Action(() => this.Source.IsFlag2 = this.Source.IsFlag1);
                        var undoAction = new Action(() => this.Source.IsFlag2 = oldValue);
                        undoService.Execute(new DelegateUndoAction(nameof(CremaDataType.IsFlag), redoAction, undoAction));
                    }
                }
                else if (propertyName == nameof(CremaDataType.Comment))
                {
                    if (this.Source.Comment2 != this.Source.Comment1)
                    {
                        var oldValue = this.Source.Comment2;
                        var redoAction = new Action(() => this.Source.Comment2 = this.Source.Comment1);
                        var undoAction = new Action(() => this.Source.Comment2 = oldValue);
                        undoService.Execute(new DelegateUndoAction(nameof(CremaDataType.Comment), redoAction, undoAction));
                    }
                }
            }
        }

        private void CopyPropertyToRightSide_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.Source != null && e.Parameter is string propertyName)
            {
                if (propertyName == nameof(CremaDataType.Name))
                {
                    if (this.Source.SourceItem2.TypeName != this.Source.SourceItem1.TypeName)
                    {
                        e.CanExecute = this.Source.MergeType != DiffMergeTypes.ReadOnly2;
                    }
                }
                else if (propertyName == nameof(CremaDataType.IsFlag))
                {
                    if (this.Source.SourceItem2.IsFlag != this.Source.SourceItem1.IsFlag)
                    {
                        e.CanExecute = this.Source.MergeType != DiffMergeTypes.ReadOnly2;
                    }
                }
                else if (propertyName == nameof(CremaDataType.Comment))
                {
                    if (this.Source.SourceItem2.Comment != this.Source.SourceItem1.Comment)
                    {
                        e.CanExecute = this.Source.MergeType != DiffMergeTypes.ReadOnly2;
                    }
                }
            }
        }

        private void CopyPropertyToLeftSide_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var undoService = DiffUndoService.GetUndoService(this);
            if (this.Source != null && e.Parameter is string propertyName)
            {
                if (propertyName == nameof(CremaDataType.Name))
                {
                    if (this.Source.ItemName1 != this.Source.ItemName2)
                    {
                        var oldValue = this.Source.ItemName1;
                        var redoAction = new Action(() => this.Source.ItemName1 = this.Source.ItemName2);
                        var undoAction = new Action(() => this.Source.ItemName1 = oldValue);
                        undoService.Execute(new DelegateUndoAction(nameof(CremaDataType.Name), redoAction, undoAction));
                    }
                }
                else if (propertyName == nameof(CremaDataType.IsFlag))
                {
                    if (this.Source.IsFlag1 != this.Source.IsFlag1)
                    {
                        var oldValue = this.Source.IsFlag1;
                        var redoAction = new Action(() => this.Source.IsFlag1 = this.Source.IsFlag2);
                        var undoAction = new Action(() => this.Source.IsFlag1 = oldValue);
                        undoService.Execute(new DelegateUndoAction(nameof(CremaDataType.IsFlag), redoAction, undoAction));
                    }
                }
                else if (propertyName == nameof(CremaDataType.Comment))
                {
                    if (this.Source.Comment1 != this.Source.Comment2)
                    {
                        var oldValue = this.Source.Comment1;
                        var redoAction = new Action(() => this.Source.Comment1 = this.Source.Comment2);
                        var undoAction = new Action(() => this.Source.Comment1 = oldValue);
                        undoService.Execute(new DelegateUndoAction(nameof(CremaDataType.Comment), redoAction, undoAction));
                    }
                }
            }
        }

        private void CopyPropertyToLeftSide_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.Source != null && e.Parameter is string propertyName)
            {
                if (propertyName == nameof(CremaDataType.Name))
                {
                    if (this.Source.SourceItem1.TypeName != this.Source.SourceItem2.TypeName)
                    {
                        e.CanExecute = this.Source.MergeType != DiffMergeTypes.ReadOnly1;
                    }
                }
                else if (propertyName == nameof(CremaDataType.IsFlag))
                {
                    if (this.Source.SourceItem1.IsFlag != this.Source.SourceItem2.IsFlag)
                    {
                        e.CanExecute = this.Source.MergeType != DiffMergeTypes.ReadOnly1;
                    }
                }
                else if (propertyName == nameof(CremaDataType.Comment))
                {
                    if (this.Source.SourceItem1.Comment != this.Source.SourceItem2.Comment)
                    {
                        e.CanExecute = this.Source.MergeType != DiffMergeTypes.ReadOnly1;
                    }
                }
            }
        }

        private void Merge_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.Merge();
        }

        private void Merge_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.CanMerge;
        }

        private void Resolve_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ResolveCommand != null)
                this.ResolveCommand.Execute(this.ResolveCommandParameter);
            else
                this.Resolve();
        }

        private void Resolve_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.ResolveCommand != null && this.ReadOnly == false)
                e.CanExecute = this.ResolveCommand.CanExecute(this.ResolveCommandParameter);
            else
                e.CanExecute = this.CanResolve;
        }

        private static void SourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiffDataTypeControl control && e.NewValue is DiffDataType source)
            {
                control.Dispatcher.InvokeAsync(() =>
                {
                    var gridControl1 = control.GetGridControl(control.DataControl1);
                    var gridContext = DataGridControl.GetDataGridContext(gridControl1);
                    TableView.SetFixedColumnCount(gridContext, 2);
                    control.SetValue(ReadOnly1PropertyKey, source.DiffState != DiffState.Modified || source.MergeType == DiffMergeTypes.ReadOnly1 || source.SourceItem1.ReadOnly);
                    control.SetValue(ReadOnly2PropertyKey, source.DiffState != DiffState.Modified || source.MergeType == DiffMergeTypes.ReadOnly2 || source.SourceItem2.ReadOnly);
                });
            }
        }
    }
}
