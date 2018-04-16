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
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Designer.ViewModels
{
    class TabItemViewModel : PropertyChangedBase
    {
        private readonly CremaDataTable table;
        private bool isVisible;
        private CremaDataTable itemsSource;
        //private IEnumerable itemsSource;

        public TabItemViewModel(CremaDataTable table)
        {
            this.table = table;
            this.isVisible = true;
            this.itemsSource = table;
            //this.itemsSource = new TableAsyncBindingListView(table.TableInfo, table.DefaultView);
            //this.itemsSource = table.DefaultView;
        }

        public void Fill()
        {
            //this.itemsSource.Fill();
        }

        public CremaDataTable ItemsSource
        {
            get { return this.itemsSource; }
        }

        public TableInfo TableInfo
        {
            get { return this.table.TableInfo; }
        }

        public string Name
        {
            get { return this.table.TableName; }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                this.isVisible = value;
                this.NotifyOfPropertyChange(() => this.IsVisible);
            }
        }
    }
}
