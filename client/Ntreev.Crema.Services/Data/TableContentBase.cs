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

using Ntreev.Crema.Services.Domains;
using Ntreev.Crema.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Services.Data
{
    abstract class TableContentBase
    {
        private readonly Dictionary<DataRow, TableRow> rows = new Dictionary<DataRow, TableRow>();

        protected void Add(TableRow row)
        {
            this.rows.Add(row.Row, row);
        }

        protected void Clear()
        {
            this.rows.Clear();
        }

        public TableRow Find(Authentication authentication, params object[] keys)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            var table = this.DataTable.DefaultView.Table;
            var row = table.Rows.Find(keys);
            if (row == null)
                return null;
            if (this.rows.ContainsKey(row) == false)
            {
                this.rows.Add(row, new TableRow(this, row));
            }
            return this.rows[row];
        }

        public TableRow[] Select(Authentication authentication, string filterExpression)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            var table = this.DataTable.DefaultView.Table;
            var rows = table.Select(filterExpression);
            foreach (var item in rows)
            {
                if (this.rows.ContainsKey(item) == false)
                {
                    this.rows.Add(item, new TableRow(this, item));
                }
            }
            return rows.Select(item => this.rows[item]).ToArray();
        }

        public IEnumerator<TableRow> GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            if (this.DataTable != null)
            {
                foreach (DataRow item in this.DataTable.DefaultView.Table.Rows)
                {
                    if (this.rows.ContainsKey(item) == false)
                    {
                        this.rows.Add(item, new TableRow(this, item));
                    }

                    yield return this.rows[item];
                }
            }
        }

        public abstract Domain Domain { get; }

        public abstract DataBase DataBase { get; }

        public abstract CremaDataTable DataTable { get; }

        public abstract CremaDispatcher Dispatcher { get; }

        public abstract CremaHost CremaHost { get; }
    }
}
