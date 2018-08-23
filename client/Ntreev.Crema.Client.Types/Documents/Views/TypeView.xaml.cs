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

using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework.Controls;
using Ntreev.ModernUI.Framework.DataGrid.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Ntreev.Library;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ntreev.Crema.Client.Types.MenuItems.TypeMenus;

namespace Ntreev.Crema.Client.Types.Documents.Views
{
    /// <summary>
    /// Interaction logic for TypeView.xaml
    /// </summary>
    public partial class TypeView : UserControl, IDisposable
    {
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(TypeView),
                new UIPropertyMetadata(string.Empty, SearchTextPropertyChangedCallback, SearchTextCoerceValueCallback));

        [Import]
        private ICremaHost cremaHost = null;
        [Import]
        private QuickFindTypeDataMenuItem menuItem = null;

        private ModernDataGridControl gridControl;

        public TypeView()
        {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(SearchBox.ShowCommand, ShowCommand_Execute, ShowCommand_CanExecute));
            this.CommandBindings.Add(new CommandBinding(SearchBox.HideCommand, HideCommand_Execute, HideCommand_CanExecute));
        }

        public void Dispose()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.dataTypeControl.ApplyTemplate();
            this.gridControl = this.dataTypeControl.Template.FindName("PART_DataGridControl", this.dataTypeControl) as ModernDataGridControl;

            this.searchBox.Visibility = Visibility.Collapsed;
            this.searchBox.IsVisibleChanged += SearchBox_IsVisibleChanged;

            BindingOperations.SetBinding(this, SearchTextProperty, new Binding(SearchBox.TextProperty.Name)
            {
                Source = this.searchBox,
                Mode = BindingMode.TwoWay,
            });

            if (this.gridControl != null)
            {
                this.gridControl.ItemsSourceChangeCompleted += GridControl_ItemsSourceChangeCompleted;

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

        protected async override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);

            if ((bool)e.NewValue == true)
            {
                var tableBrowser = this.cremaHost.GetService(typeof(ITypeBrowser)) as ITypeBrowser;
                var tableItemViewModel = this.DataContext as ITypeDocument;
                var table = tableItemViewModel.Target;

                var viewModel = table.ExtendedProperties[tableBrowser];

                if (viewModel != null)
                    await this.Dispatcher.InvokeAsync(() => tableBrowser.SelectedItem = viewModel);
            }
            this.menuItem.Refresh();
        }

        private static void SearchTextPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TypeView self && self.gridControl != null)
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

        private void GridControl_ItemsSourceChangeCompleted(object sender, EventArgs e)
        {
            
        }

        private ICremaConfiguration Configs
        {
            get { return this.cremaHost.Configs; }
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
            if (this.searchBox.IsVisible == false)
                this.searchBox.Visibility = Visibility.Visible;
            this.searchBox.Focus();
        }

        private void ShowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void HideCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.searchBox.Visibility = Visibility.Collapsed;
        }

        private void HideCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.searchBox.IsVisible == true)
            {
                e.CanExecute = true;
            }
        }
    }
}
