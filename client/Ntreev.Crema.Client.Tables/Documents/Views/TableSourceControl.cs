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
using Ntreev.Crema.Presentation.Controls;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework.Controls;
using Ntreev.ModernUI.Framework.DataGrid.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Ntreev.Crema.Client.Tables.Documents.Views
{
    [TemplatePart(Name = "PART_SearchBox", Type = typeof(SearchBox))]
    class TableSourceControl : CremaDataTableItemControl
    {
        public static readonly DependencyProperty DomainProperty =
            DependencyProperty.Register(nameof(Domain), typeof(IDomain), typeof(TableSourceControl));

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(TableSourceControl),
                new UIPropertyMetadata(string.Empty, SearchTextPropertyChangedCallback, SearchTextCoerceValueCallback));

        private ModernDataGridControl gridControl;
        private SearchBox searchBox;

        public TableSourceControl()
        {
            this.CommandBindings.Add(new CommandBinding(SearchBox.ShowCommand, ShowCommand_Execute, ShowCommand_CanExecute));
            this.CommandBindings.Add(new CommandBinding(SearchBox.HideCommand, HideCommand_Execute, HideCommand_CanExecute));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.gridControl = this.Template.FindName("PART_DataGridControl", this) as ModernDataGridControl;
            this.searchBox = this.Template.FindName("PART_SearchBox", this) as SearchBox;

            if (this.searchBox != null)
            {
                this.searchBox.Visibility = Visibility.Collapsed;
                this.searchBox.IsVisibleChanged += SearchBox_IsVisibleChanged;

                BindingOperations.SetBinding(this, SearchTextProperty, new Binding(SearchBox.TextProperty.Name)
                {
                    Source = this.searchBox,
                    Mode = BindingMode.TwoWay,
                });
            }
            if (this.gridControl != null)
            {
                for (var i = 0; i < this.gridControl.CommandBindings.Count; i++)
                {
                    var commandBinding = this.gridControl.CommandBindings[i];
                    if (commandBinding.Command == ModernDataGridCommands.NextMatchedItem || commandBinding.Command == ModernDataGridCommands.PrevMatchedItem)
                    {
                        this.CommandBindings.Add(commandBinding);
                    }
                }
            }
        }

        public IDomain Domain
        {
            get { return (IDomain)GetValue(DomainProperty); }
            set { SetValue(DomainProperty, value); }
        }

        public string SearchText
        {
            get { return (string)this.GetValue(SearchTextProperty); }
            set { this.SetValue(SearchTextProperty, value); }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (this.gridControl != null && this.gridControl.IsBeingEdited == false && this.searchBox.IsVisible == true)
                {
                    this.searchBox.Visibility = Visibility.Collapsed;
                    this.gridControl.Focus();
                    e.Handled = true;
                }
            }
        }

        private static void SearchTextPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TableSourceControl self && self.gridControl != null)
            {
                self.gridControl.SearchText = self.SearchText;
            }
        }

        private static object SearchTextCoerceValueCallback(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
                return string.Empty;
            return baseValue;
        }

        private void SearchBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.searchBox.IsVisible == false)
            {
                if (this.gridControl != null)
                {
                    this.gridControl.SearchText = string.Empty;
                    this.gridControl.Focus();
                }
            }
            else
            {
                if (this.gridControl != null)
                {
                    this.gridControl.SearchText = this.SearchText;
                    if (this.SearchText != string.Empty)
                    {
                        this.searchBox.SelectAll();
                    }
                }
            }
        }

        private void ShowCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.searchBox != null)
            {
                if (this.searchBox.IsVisible == false)
                    this.searchBox.Visibility = Visibility.Visible;
                this.searchBox.Focus();
            }
        }

        private void ShowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void HideCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.searchBox != null)
            {
                this.searchBox.Visibility = Visibility.Collapsed;
            }
        }

        private void HideCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.searchBox != null && this.searchBox.IsVisible == true)
            {
                e.CanExecute = true;
            }
        }
    }
}
