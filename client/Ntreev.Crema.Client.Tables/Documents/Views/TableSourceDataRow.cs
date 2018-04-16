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
using Ntreev.Crema.Data.Xml.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.DataGrid;

namespace Ntreev.Crema.Client.Tables.Documents.Views
{
    class TableSourceDataRow : DomainDataRow
    {
        public static DependencyProperty IsItemEnabledProperty =
            DependencyProperty.Register(nameof(IsItemEnabled), typeof(bool), typeof(TableSourceDataRow));

        public static DependencyProperty TagBrushProperty =
            DependencyProperty.Register("TagBrush", typeof(Brush), typeof(TableSourceDataRow));

        public TableSourceDataRow()
        {

        }

        public bool IsItemEnabled
        {
            get { return (bool)this.GetValue(IsItemEnabledProperty); }
            set { this.SetValue(IsItemEnabledProperty, value); }
        }

        protected override Cell CreateCell(ColumnBase column)
        {
            if (column.FieldName == CremaSchema.Enable)
                return new TableSourceEnableDataCell();
            else if (column.FieldName == CremaSchema.Tags)
                return new TableSourceTagDataCell();
            return new TableSourceDataCell();
        }
    }
}
