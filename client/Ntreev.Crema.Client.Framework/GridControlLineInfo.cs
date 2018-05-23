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
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.DataGrid.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ntreev.Crema.Client.Framework
{
    public class GridControlLineInfo : PropertyChangedBase, ILineInfo
    {
        private readonly ModernDataGridControl gridControl;
        private string displayName;
        private string column;
        private string row;

        public GridControlLineInfo(ModernDataGridControl gridControl)
            : this(gridControl, null)
        {

        }

        public GridControlLineInfo(ModernDataGridControl gridControl, string displayName)
        {
            this.gridControl = gridControl;
            this.displayName = displayName;
            this.gridControl.PropertyChanged += GridControl_PropertyChanged;
        }

        public string DisplayName
        {
            get { return this.displayName ?? string.Empty; }
            set
            {
                this.displayName = value;
                this.NotifyOfPropertyChange(nameof(this.DisplayName));
            }
        }

        public object Row
        {
            get { return this.row; }
        }

        public object Column
        {
            get { return this.column; }
        }

        private void GridControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ModernDataGridControl.GlobalCurrentColumn))
            {
                var gridContext = this.gridControl.CurrentContext;
                if (gridContext.CurrentColumn != null)
                    this.column = gridContext.CurrentColumn.FieldName;
                else
                    this.column = null;
                this.NotifyOfPropertyChange(nameof(this.Column));
            }
            if (e.PropertyName == nameof(ModernDataGridControl.GlobalCurrentItem))
            {
                var gridContext = this.gridControl.CurrentContext;
                this.row = $"{gridContext.Items.IndexOf(gridContext.CurrentItem)}";
                this.NotifyOfPropertyChange(nameof(this.Row));
            }
            if (e.PropertyName == nameof(ModernDataGridControl.CurrentContext))
            {
                var gridContext = this.gridControl.CurrentContext;
                var typedList = gridContext.Items.SourceCollection as ITypedList;
                if (typedList == null && gridContext.Items.SourceCollection is CollectionView view)
                {
                    var source = view.SourceCollection;
                    typedList = source as ITypedList;
                }

                if (typedList != null)
                    this.displayName = typedList.GetListName(null);
            }
            this.NotifyOfPropertyChange(nameof(this.DisplayName));
        }
    }
}
