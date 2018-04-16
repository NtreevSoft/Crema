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
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.DataGrid;
using System.Windows.Input;

namespace Ntreev.Crema.Presentation.Controls
{
    class DiffDataCell : ModernDataCell
    {
        private static readonly DependencyPropertyKey IsModifiedPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(IsModified), typeof(bool), typeof(DiffDataCell),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsModifiedProperty = IsModifiedPropertyKey.DependencyProperty;

        public bool IsModified
        {
            get { return (bool)this.GetValue(IsModifiedProperty); }
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
        }

        protected override void PrepareContainer(DataGridContext dataGridContext, object item)
        {
            if (this.DataContext is INotifyPropertyChanged oldRow)
            {
                oldRow.PropertyChanged -= DataRowView_PropertyChanged;
            }

            base.PrepareContainer(dataGridContext, item);

            if (this.ParentRow is DiffDataContainer row)
            {
                this.SetValue(IsModifiedPropertyKey, row.Contains(this.FieldName));
            }

            if (this.DataContext is INotifyPropertyChanged newRow)
            {
                newRow.PropertyChanged += DataRowView_PropertyChanged;
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (this.ParentRow is DiffDataContainer row)
            {
                this.SetValue(IsModifiedPropertyKey, row.Contains(this.FieldName));
            }
        }

        private void DataRowView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.DataContext == null)
                return;

            if (this.ParentRow is DiffDataContainer container)
            {
                if (e.PropertyName == DiffUtility.DiffFieldsKey)
                {
                    this.SetValue(IsModifiedPropertyKey, container.Contains(this.FieldName));
                }
            }
        }
    }
}
