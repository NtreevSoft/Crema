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
using Ntreev.Crema.Data;
using Ntreev.Crema.Client.Framework.Properties;

namespace Ntreev.Crema.Client.Framework.Controls
{
    public class DomainTextClipboardInserter
    {
        private readonly DataGridContext gridContext;

        private readonly string tableName;
        private readonly string[] columnNames;
        private readonly PropertyDescriptorCollection props;

        private ColumnBase[] selectedColumns;
        private DomainRowInfo[] domainRows;

        public DomainTextClipboardInserter(DataGridContext gridContext)
        {
            this.gridContext = gridContext;

            var typedList = gridContext.Items.SourceCollection as ITypedList;
            if (typedList == null)
            {
                var source = (gridContext.Items.SourceCollection as CollectionView).SourceCollection;
                typedList = source as ITypedList;
            }

            this.props = typedList.GetItemProperties(null);
            this.tableName = typedList.GetListName(null);
            this.columnNames = CremaDataRowUtility.GetColumnNames(typedList);
        }

        public void Parse(string[][] rows)
        {
            if (this.gridContext.HasRectangularSelection() == false)
                throw new Exception("여러 범위");

            var hasHeader = this.ExistsHeader(rows.First());
            var targetColumns = this.InitializeTargetColumns(rows.First());

            var rowInfos = new List<DomainRowInfo>();

            for (var i = hasHeader ? 1 : 0; i < rows.Length; i++)
            {
                var textFields = rows[i];

                var rowInfo = new DomainRowInfo
                {
                    Fields = this.Fill(targetColumns, textFields),
                    TableName = CremaDataTable.GetTableName(this.tableName),
                };
                rowInfos.Add(rowInfo);
            }

            this.domainRows = rowInfos.ToArray();
            this.selectedColumns = targetColumns;
        }

        public DomainRowInfo[] DomainRows
        {
            get { return this.domainRows; }
        }

        private bool ExistsHeader(string[] textFields)
        {
            foreach (var item in textFields)
            {
                var contains = false;
                foreach (var column in this.gridContext.Columns)
                {
                    if (column.FieldName == item)
                    {
                        contains = true;
                    }
                }

                if (contains == false)
                    return false;
            }

            return true;
        }

        private ColumnBase[] InitializeTargetColumns(string[] textFields)
        {
            var columnList = new List<ColumnBase>();
            if (this.ExistsHeader(textFields) == true)
            {
                foreach (var item in textFields)
                {
                    var column = this.gridContext.Columns[item];
                    columnList.Add(column);
                }
            }
            else
            {
                var index = this.gridContext.VisibleColumns.IndexOf(this.gridContext.CurrentColumn);
                if (index + textFields.Length > this.gridContext.VisibleColumns.Count)
                    throw new ArgumentException(Resources.Exception_ColumnRangeExceeded);
                for (var i = 0; i < textFields.Length; i++)
                {
                    var column = this.gridContext.VisibleColumns[i + index];
                    columnList.Add(column);
                }
            }

            foreach (var item in columnList)
            {
                if (item.ReadOnly == true)
                {
                    throw new ArgumentException(string.Format(Resources.Exception_ItsReadOnly_Format, item.FieldName), nameof(textFields));
                }
            }

            return columnList.ToArray();
        }

        private object[] Fill(ColumnBase[] columns, string[] textFields)
        {
            var fields = this.CreateFields();

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

        private object[] CreateFields()
        {
            if (this.gridContext.ParentItem != null)
                return CremaDataRowUtility.GetInsertionFields(this.gridContext.ParentItem, this.tableName);
            return new object[this.columnNames.Length];
        }

        private object ConvertFrom(PropertyDescriptor prop, string textField)
        {
            if (string.IsNullOrEmpty(textField) == true)
                return DBNull.Value;
            return prop.Converter.ConvertFrom(textField);
        }
    }
}