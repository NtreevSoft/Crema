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
using Ntreev.Crema.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Designer.Tables.ViewModels
{
    public class TableItemViewModel : PropertyChangedBase, IDisposable
    {
        //[Import]
        //private IServiceProvider serviceProvider = null;

        private CremaDataTable dataTable;
        private object selectedItem;
        private string selectedColumn;
        private TableInfo tableInfo;
        private bool isVisible;
        private bool isLoaded;
        private bool isFinderVisible;
        private bool isReadOnly;
        private string filter;

        public TableItemViewModel(CremaDataTable dataTable)
        {
            this.dataTable = dataTable;
            this.dataTable.ExtendedProperties[DataSetProperties.IsBeingEditedKey] = true;
            this.isLoaded = true;
            this.tableInfo = dataTable.TableInfo;
        }

        public void Focus(int row, string columnName)
        {
            int index = 0;
            foreach (var item in this.dataTable.DefaultView)
            {
                if (row == index)
                {
                    this.SelectedItem = item;
                    break;
                }
                index++;
            }
            this.SelectedColumn = columnName;
        }

        public string Name
        {
            get { return this.tableInfo.Name; }
        }

        public string DisplayName
        {
            get { return this.tableInfo.TableName; }
        }

        public void CloseFinder()
        {
            this.IsFinderVisible = false;
        }

        public bool IsVisible
        {
            get
            {
                if (this.isLoaded == false)
                    return true;
                return this.isVisible;
            }
            set
            {
                this.isVisible = value;
                this.NotifyOfPropertyChange(nameof(this.IsVisible));
            }
        }

        public bool IsReadOnly
        {
            get { return this.isReadOnly; }
            set
            {
                this.isReadOnly = value;
                this.NotifyOfPropertyChange(nameof(this.IsReadOnly));
            }
        }

        public CremaDataTable Table
        {
            get { return this.dataTable; }
        }

        public CremaDataTable Source
        {
            get { return this.dataTable; }
        }

        public TableInfo TableInfo
        {
            get { return this.tableInfo; }
        }

        public object SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                this.selectedItem = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedItem));
            }
        }

        public int SelectedItemIndex
        {
            set
            {
                if (value >= 0 && value < this.dataTable.DefaultView.Count)
                {
                    var item = this.dataTable.DefaultView[value];
                    this.SelectedItem = item;
                }
                else
                {
                    this.SelectedItem = null;
                }
            }
        }

        public string SelectedColumn
        {
            get { return this.selectedColumn; }
            set
            {
                this.selectedColumn = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedColumn));
            }
        }

        public bool IsFinderVisible
        {
            get { return this.isFinderVisible; }
            set
            {
                this.isFinderVisible = value;
                this.NotifyOfPropertyChange(nameof(this.IsFinderVisible));
            }
        }

        public string Filter
        {
            get { return this.filter ?? string.Empty; }
            set
            {
                this.filter = value;
                this.NotifyOfPropertyChange(nameof(this.Filter));
            }
        }

        public override string ToString()
        {
            return this.tableInfo.Name;
        }

        public void Dispose()
        {
            this.dataTable.ExtendedProperties[DataSetProperties.IsBeingEditedKey] = false;
            this.dataTable = null;

            this.NotifyOfPropertyChange(nameof(this.Table));
            this.NotifyOfPropertyChange(nameof(this.Source));
        }
    }
}
