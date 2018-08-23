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

using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.DataGrid.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Configuration;
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

namespace Ntreev.Crema.Designer.Types.Views
{
    /// <summary>
    /// Interaction logic for TypeViewerView.xaml
    /// </summary>
    public partial class TypeViewerView : UserControl, IDisposable
    {
        [Import]
        private IAppConfiguration configs = null;

        //[Import]
        //private QuickFindTypeDataMenuItemViewModel menuItem = null;

        private ModernDataGridControl gridControl;

        public static RoutedCommand ShowFilterCommand = new RoutedCommand("ShowFilter", typeof(TypeViewerView));

        public TypeViewerView()
        {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(ShowFilterCommand, ShowFilter_Execute, ShowFilter_CanExecute));
        }

        public void Dispose()
        {
            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.dataTypeControl.ApplyTemplate();
            this.gridControl = this.dataTypeControl.Template.FindName("PART_DataGridControl", this.dataTypeControl) as ModernDataGridControl;

            if (this.gridControl != null)
            {
                this.gridControl.ItemsSourceChangeCompleted += GridControl_ItemsSourceChangeCompleted;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (this.gridControl != null && this.gridControl.IsBeingEdited == false && this.gridFilter.Visibility == Visibility.Visible)
                {
                    this.HideFilter();
                    this.gridControl.Focus();
                    e.Handled = true;
                }
            }
        }

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);

            if ((bool)e.NewValue == true)
            {
                //var tableBrowser = this.shell.GetService(typeof(ITypeBrowser)) as ITypeBrowser;
                //var tableItemViewModel = this.DataContext as ITypeDocument;
                //var table = tableItemViewModel.Target;

                //var viewModel = table.ExtendedProperties[tableBrowser];

                //if (viewModel != null)
                //    await this.Dispatcher.InvokeAsync(() => tableBrowser.SelectedItem = viewModel);
            }
            //this.menuItem.Refresh();
        }

        private void ShowFilter_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.ShowFilter();
        }

        private void ShowFilter_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.gridFilter.Visibility != Visibility.Visible;
        }

        private void GridControl_ItemsSourceChangeCompleted(object sender, EventArgs e)
        {
            var typeID = this.dataTypeControl.Source.TypeID;
            //if (this.configs.TryParse<Xceed.Wpf.DataGrid.Settings.SettingsRepository>(this.GetType(), typeID.ToString(), out var settings) == true)
            //{
            //    this.gridControl.LoadUserSettings(settings, Xceed.Wpf.DataGrid.Settings.UserSettings.All);
            //}
        }

        //private ICremaConfiguration Configs
        //{
        //    get { return this.shell.Configs; }
        //}

        private void CloseFilter_Click(object sender, RoutedEventArgs e)
        {
            this.HideFilter();
        }

        private void ShowFilter()
        {
            this.gridFilter.Visibility = Visibility.Visible;
            this.PART_Filter.Focus();
            BindingOperations.SetBinding(this.PART_Filter, TextBox.TextProperty, new Binding("Filter")
            {
                Mode = BindingMode.TwoWay,
                Source = this.gridControl,
                Delay = 300,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });
        }

        private void HideFilter()
        {
            this.gridFilter.Visibility = Visibility.Collapsed;
            this.gridControl.SearchText = string.Empty;
            BindingOperations.ClearBinding(this.PART_Filter, TextBox.TextProperty);
        }
    }
}
