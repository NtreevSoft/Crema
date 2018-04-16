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
using System.Windows.Data;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Controls;
using System.Windows.Threading;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.IO;

namespace Ntreev.Crema.Presentation.Controls
{
    public class DiffDataTableItemControl : DiffDataControl
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(DiffDataTable), typeof(DiffDataTableItemControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsDateTimeIncludedProperty =
            DependencyProperty.Register(nameof(IsDateTimeIncluded), typeof(bool), typeof(DiffDataTableItemControl),
                new FrameworkPropertyMetadata(true, IsDateTimeIncludedPropertyChangedCallback));

        public static readonly DependencyProperty ResolveCommandProperty =
            DependencyProperty.Register(nameof(ResolveCommand), typeof(ICommand), typeof(DiffDataTableItemControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ResolveCommandParameterProperty =
            DependencyProperty.Register(nameof(ResolveCommandParameter), typeof(object), typeof(DiffDataTableItemControl),
                new PropertyMetadata(null));

        public DiffDataTableItemControl()
        {
            this.CommandBindings.Add(new CommandBinding(DiffCommands.Resolve, Resolve_Execute, Resolve_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.IncludeDateTime, IncludeDateTime_Execute, IncludeDateTime_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.ExportLeft, ExportLeft_Execute, ExportLeft_CanExecute));
            this.CommandBindings.Add(new CommandBinding(DiffCommands.ExportRight, ExportRight_Execute, ExportRight_CanExecute));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindingOperations.SetBinding(this.DataControl1, CremaDataTableItemControl.SourceProperty, new Binding($"{nameof(Source)}.{nameof(DiffDataTable.SourceItem1)}") { Source = this, });
            BindingOperations.SetBinding(this.DataControl2, CremaDataTableItemControl.SourceProperty, new Binding($"{nameof(Source)}.{nameof(DiffDataTable.SourceItem2)}") { Source = this, });
        }

        public void Resolve()
        {
            try
            {
                var undoService = DiffUndoService.GetUndoService(this);
                this.Source.Resolve();
                undoService.Clear();
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
                if (this.Source.DiffState == DiffState.Unchanged)
                    return false;
                if (this.Source.IsResolved == true)
                    return false;
                return this.Source.UnresolvedItems.Any() == false;
            }
        }

        public bool CanResolve
        {
            get
            {
                if (this.ReadOnly == true)
                    return false;
                if (this.Source != null)
                    return this.Source.IsResolved == false && this.Source.UnresolvedItems.Any() == false;
                return false;
            }
        }

        public DiffDataTable Source
        {
            get { return (DiffDataTable)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        public ICommand ResolveCommand
        {
            get { return (ICommand)this.GetValue(ResolveCommandProperty); }
            set { this.SetValue(ResolveCommandProperty, value); }
        }

        public bool IsDateTimeIncluded
        {
            get { return (bool)this.GetValue(IsDateTimeIncludedProperty); }
            set { this.SetValue(IsDateTimeIncludedProperty, value); }
        }

        public object ResolveCommandParameter
        {
            get { return (object)this.GetValue(ResolveCommandParameterProperty); }
            set { this.SetValue(ResolveCommandParameterProperty, value); }
        }

        private static void IsDateTimeIncludedPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiffDataTableItemControl self)
            {
                if (e.NewValue is bool IsDateTimeIncluded)
                {
                    var filterList = self.Source.Filters.ToList();
                    if (IsDateTimeIncluded == true)
                    {
                        filterList.Remove(CremaSchema.Creator);
                        filterList.Remove(CremaSchema.CreatedDateTime);
                        filterList.Remove(CremaSchema.Modifier);
                        filterList.Remove(CremaSchema.ModifiedDateTime);
                    }
                    else
                    {
                        filterList.Add(CremaSchema.Creator);
                        filterList.Add(CremaSchema.CreatedDateTime);
                        filterList.Add(CremaSchema.Modifier);
                        filterList.Add(CremaSchema.ModifiedDateTime);
                    }
                    self.Source.Filters = filterList.ToArray();
                }
            }
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

        private void IncludeDateTime_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.IsDateTimeIncluded = !this.IsDateTimeIncluded;
        }

        private void IncludeDateTime_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.Source != null && this.Source.IsResolved == false && this.Source.DiffState == DiffState.Modified && this.ReadOnly == false)
            {
                e.CanExecute = true;
            }
        }

        private void ExportLeft_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var dataTable = this.Source.ExportTable1();
                var dataSet = dataTable.DataSet;
                DirectoryUtility.Delete("Export1");
                dataSet.WriteToDirectory("Export1");
            }
            catch (Exception ex)
            {
                AppMessageBox.ShowError(ex);
            }
        }

        private void ExportLeft_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.Source != null && this.Source.UnresolvedItems.Any() == false)
            {
                e.CanExecute = true;
            }
        }

        private void ExportRight_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var dataTable = this.Source.ExportTable2();
                var dataSet = dataTable.DataSet;
                DirectoryUtility.Delete("Export2");
                dataSet.WriteToDirectory("Export2");
            }
            catch (Exception ex)
            {
                AppMessageBox.ShowError(ex);
            }
        }

        private void ExportRight_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.Source != null && this.Source.UnresolvedItems.Any() == false)
            {
                e.CanExecute = true;
            }
        }

        private DiffDataTable FindSource(DataGridContext gridContext)
        {
            if (gridContext.ParentDataGridContext != null)
            {
                var relationName = gridContext.SourceDetailConfiguration.RelationName;
                var childName = CremaSchema.GetChildNameFromRelationName(relationName);

                for (var i = 0; i < this.Source.Childs.Length; i++)
                {
                    if (this.Source.Childs[i].SourceItem1.TableName == childName)
                        return this.Source.Childs[i];
                }
                throw new NotImplementedException();
            }
            return this.Source;
        }
    }
}
