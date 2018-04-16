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
using Ntreev.Crema.Reader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Tools.View.ViewModels
{
    class ItemViewModel : PropertyChangedBase
    {
        private readonly ITable table;
        private IEnumerable itemsSource;

        public ItemViewModel(ITable table)
        {
            this.table = table;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public string Name
        {
            get { return this.table.Name; }
        }

        public IEnumerable ItemsSource
        {
            get
            {
                if (this.itemsSource == null)
                {
                    Task.Run(() => this.Initialize());
                }

                return this.itemsSource;
            }
        }

        private void Initialize()
        {
            var table = new DataTable();

            foreach (var item in this.table.Columns)
            {
                table.Columns.Add(item.Name);
            }

            foreach (var item in this.table.Rows)
            {
                var row = table.NewRow();
                foreach (var c in this.table.Columns)
                {
                    var value = item[c];
                    if (value != null)
                        row[c.Name] = value.ToString();
                }
                table.Rows.Add(row);
            }

            this.itemsSource = table.DefaultView;
            this.NotifyOfPropertyChange(() => this.ItemsSource);
        }
    }
}
