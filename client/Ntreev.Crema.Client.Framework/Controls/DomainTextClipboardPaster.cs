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
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.DataGrid.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Xceed.Wpf.DataGrid;
using Ntreev.Crema.Client.Framework.Properties;

namespace Ntreev.Crema.Client.Framework.Controls
{
    public class DomainTextClipboardPaster
    {
        private readonly DataGridContext gridContext;
        private readonly ColumnBase[] columns;
        private readonly object[] items;

        private readonly string tableName;
        private readonly string[] columnNames;
        private readonly PropertyDescriptorCollection props;

        private object[] selectedItems;
        private ColumnBase[] selectedColumns;
        private DomainRowInfo[] domainRows;

        public DomainTextClipboardPaster(DataGridContext gridContext)
        {
            this.gridContext = gridContext;
            this.columns = gridContext.GetSelectedColumns().ToArray();
            this.items = gridContext.GetSelectedItems().ToArray();

            var typedList = gridContext.Items.SourceCollection as ITypedList;
            if (typedList == null)
            {
                var source = (gridContext.Items.SourceCollection as CollectionView).SourceCollection;
                typedList = source as ITypedList;
            }

            this.props = typedList.GetItemProperties(null);
            this.tableName = CremaDataTable.GetTableName(typedList.GetListName(null));
            this.columnNames = CremaDataRowUtility.GetColumnNames(typedList);
        }

        public void Parse(string[][] rows)
        {
            if (this.gridContext.HasRectangularSelection() == false)
                throw new Exception("여러 범위");

            var targetColumns = this.InitializeTargetColumns(rows.First());
            var targetItems = this.InitializeTargetItems(rows);
            var itemCount = this.items.Length % rows.Length == 0 ? this.items.Length : rows.Length;
            var columnCount = this.columns.Length % rows.First().Length == 0 ? this.columns.Length : rows.First().Length;
            var rowInfos = new List<DomainRowInfo>(itemCount);

            for (var i = 0; i < targetItems.Length; i++)
            {
                var item = targetItems[i];
                var textFields = rows[i % rows.Length];

                var rowInfo = new DomainRowInfo
                {
                    Fields = this.Fill(targetColumns, textFields),
                    TableName = this.tableName,
                    Keys = CremaDataRowUtility.GetKeys(item)
                };
                rowInfos.Add(rowInfo);
            }

            this.domainRows = rowInfos.ToArray();
            this.selectedItems = targetItems;
            this.selectedColumns = targetColumns;
        }

        public DomainRowInfo[] DomainRows
        {
            get { return this.domainRows; }
        }

        public void SelectRange()
        {
            var itemIndex = this.gridContext.Items.IndexOf(this.selectedItems.First());
            var columnIndex = this.gridContext.VisibleColumns.IndexOf(this.selectedColumns.First());
            var itemRange = new SelectionRange(itemIndex, itemIndex + this.selectedItems.Length - 1);
            var columnRange = new SelectionRange(columnIndex, columnIndex + this.selectedColumns.Length - 1);
            this.gridContext.SelectedCellRanges.Clear();
            this.gridContext.SelectedCellRanges.Add(new SelectionCellRange(itemRange, columnRange));
        }

        private ColumnBase[] InitializeTargetColumns(string[] textFields)
        {
            var firstColumn = this.columns.First();
            var firstColumnIndex = this.gridContext.VisibleColumns.IndexOf(firstColumn);
            var count = this.columns.Length % textFields.Length == 0 ? this.columns.Length : textFields.Length;
            var columnList = new List<ColumnBase>();

            for (var i = 0; i < count; i++)
            {
                var column = this.gridContext.VisibleColumns[firstColumnIndex + i];
                if (column.ReadOnly == true)
                    throw new ArgumentException(string.Format(Resources.Exception_ItsReadOnly_Format, column.FieldName), nameof(textFields));
                columnList.Add(column);
            }

            return columnList.ToArray();
        }

        private object[] InitializeTargetItems(string[][] rows)
        {
            var firstItem = this.items.First();
            var firstItemIndex = this.gridContext.Items.IndexOf(firstItem);
            var count = this.items.Length % rows.Length == 0 ? this.items.Length : rows.Length;
            var itemList = new List<object>();

            for (var i = 0; i < count; i++)
            {
                var item = this.gridContext.Items.GetItemAt(firstItemIndex + i);
                itemList.Add(item);
            }

            return itemList.ToArray();
        }

        private object[] Fill(ColumnBase[] columns, string[] textFields)
        {
            var fields = new object[this.columnNames.Length];

            for (var i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                var textField = textFields[i % textFields.Length];
                var prop = props[column.FieldName];
                var field = this.ConvertFrom(prop, textField);

                for (var j = 0; j < this.columnNames.Length; j++)
                {
                    if (this.columnNames[j] == column.FieldName)
                    {
                        fields[j] = field;
                        break;
                    }
                }
            }

            return fields;
        }

        private object ConvertFrom(PropertyDescriptor prop, string textField)
        {
            if (string.IsNullOrEmpty(textField) == true)
                return DBNull.Value;
            return prop.Converter.ConvertFrom(textField);
        }
    }
}