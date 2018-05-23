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

using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Presentation.Controls;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.DataGrid.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.DataGrid;

namespace Ntreev.Crema.Presentation.Assets
{
    partial class CremaTemplateControl : ResourceDictionary
    {
        //private Point startPoint;
        //private RowSelector startSelector;
        //private bool isDragDrop;
        //private int scrollDirection;

        public CremaTemplateControl()
        {
            this.InitializeComponent();
        }

        //private void RowSelector_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    this.startSelector = sender as RowSelector;
        //    this.startPoint = e.GetPosition(null);
        //}

        //private void RowSelector_PreviewMouseMove(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Released || this.startSelector != sender || this.isDragDrop == true)
        //        return;

        //    var mousePos = e.GetPosition(null);
        //    var diff = this.startPoint - mousePos;

        //    if (e.LeftButton == MouseButtonState.Pressed &&
        //        Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
        //        Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
        //    {
        //        this.isDragDrop = true;
        //        var selector = sender as RowSelector;
        //        if (selector.DataContext is ModernDataRow)
        //        {
        //            var dataRow = selector.DataContext as ModernDataRow;
        //            var dataContext = dataRow.DataContext;
        //            var dragData = new DataObject(sender);
        //            var gridContext = DataGridControl.GetDataGridContext(selector);
        //            var gridControl = gridContext.DataGridControl;
        //            var scrollViewer = gridControl.Template.FindName("PART_ScrollViewer", gridControl) as ScrollViewer;

        //            scrollViewer.DragOver += ScrollViewer_DragOver;
        //            var timer = new Timer(200);
        //            timer.Elapsed += async (ss, ee) =>
        //            {
        //                await scrollViewer.Dispatcher.InvokeAsync(() =>
        //                {
        //                    if (this.scrollDirection == -1)
        //                        scrollViewer.LineUp();
        //                    else if (this.scrollDirection == 1)
        //                        scrollViewer.LineDown();
        //                });
        //            };
        //            timer.Start();
        //            var effects = DragDrop.DoDragDrop(sender as DependencyObject, dragData, DragDropEffects.Move | DragDropEffects.None);
        //            timer.Stop();
        //            scrollViewer.DragOver -= ScrollViewer_DragOver;

        //            if (effects == DragDropEffects.Move && dragData.GetDataPresent(typeof(int)) == true)
        //            {
        //                var index = (int)dragData.GetData(typeof(int));
        //                var prop = System.ComponentModel.TypeDescriptor.GetProperties(dataRow.DataContext)[CremaSchema.Index];
        //                try
        //                {
        //                    prop.SetValue(dataRow.DataContext, index);
        //                }
        //                catch (Exception ex)
        //                {
        //                    AppMessageBox.ShowError(ex);
        //                }
        //            }
        //        }
        //        this.isDragDrop = false;
        //    }
        //}

        //private void ScrollViewer_DragOver(object sender, DragEventArgs e)
        //{
        //    var scrollViewer = sender as ScrollViewer;
        //    var contentPresenter = scrollViewer.Template.FindName("PART_ScrollContentPresenter", scrollViewer) as ScrollContentPresenter;
        //    var position = e.GetPosition(contentPresenter);

        //    if (position.Y < 10)
        //    {
        //        this.scrollDirection = -1;
        //    }
        //    else if (position.Y > contentPresenter.ActualHeight - 10)
        //    {
        //        this.scrollDirection = 1;
        //    }
        //    else
        //    {
        //        this.scrollDirection = 0;
        //    }
        //}

        //private void RowSelector_Drop(object sender, DragEventArgs e)
        //{
        //    var selector = sender as Xceed.Wpf.DataGrid.RowSelector;
        //    if (this.startSelector == selector)
        //        return;

        //    var dataRow = selector.DataContext as ModernDataRow;
        //    var prop = System.ComponentModel.TypeDescriptor.GetProperties(dataRow.DataContext)[CremaSchema.Index];
        //    var index = (int)prop.GetValue(dataRow.DataContext);
        //    e.Data.SetData(index);
        //    dataRow.IsDragOver = false;
        //    ModernDataRow.SetIsDragOver(selector, dataRow.IsDragOver);
        //}

        //private void RowSelector_DragEnter(object sender, DragEventArgs e)
        //{
        //    var selector = sender as Xceed.Wpf.DataGrid.RowSelector;
        //    if (this.startSelector == selector)
        //        return;

        //    if (selector.DataContext is ModernDataRow)
        //    {
        //        var dataRow = selector.DataContext as ModernDataRow;
        //        dataRow.IsDragOver = true;
        //        ModernDataRow.SetIsDragOver(selector, dataRow.IsDragOver);
        //    }
        //}

        //private void RowSelector_DragOver(object sender, DragEventArgs e)
        //{
        //    var selector = sender as Xceed.Wpf.DataGrid.RowSelector;
        //    if (this.startSelector == selector)
        //        return;

        //    if (selector.DataContext is ModernDataRow)
        //    {
        //        var dataRow = selector.DataContext as ModernDataRow;
        //        e.Effects = DragDropEffects.Move;
        //    }
        //}

        //private void RowSelector_DragLeave(object sender, DragEventArgs e)
        //{
        //    var selector = sender as Xceed.Wpf.DataGrid.RowSelector;
        //    if (this.startSelector == selector)
        //        return;

        //    if (selector.DataContext is ModernDataRow)
        //    {
        //        var dataRow = selector.DataContext as ModernDataRow;
        //        dataRow.IsDragOver = false;
        //        ModernDataRow.SetIsDragOver(selector, dataRow.IsDragOver);
        //    }
        //}

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var cell = comboBox.Tag as ModernDataCell;
            comboBox.SelectionChanged -= ComboBox_SelectionChanged;
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            comboBox.SelectionChanged -= ComboBox_SelectionChanged;

            if (comboBox.Tag is ModernDataCell cell && cell.IsBeingEdited == true)
            {
                if (cell.Content != cell.EditingContent)
                    cell.EndEdit();
                else
                    cell.CancelEdit();
            }
        }

        private void ComboBox_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var fe = sender as Control;
            var textBox = fe.Template.FindName("PART_EditableTextBox", fe) as TextBox;
        }
    }
}
