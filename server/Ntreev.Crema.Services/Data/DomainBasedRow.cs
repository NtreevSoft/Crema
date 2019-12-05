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
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Data
{
    abstract class DomainBasedRow
    {
        private readonly Domain domain;
        private readonly DataTable table;
        private DataRow row;
        private Dictionary<string, object> fields;

        protected DomainBasedRow(Domain domain, DataRow row)
        {
            this.domain = domain ?? throw new ArgumentNullException(nameof(domain));
            this.row = row;
            this.table = row.Table;
        }

        protected DomainBasedRow(Domain domain, DataTable table)
        {
            this.domain = domain ?? throw new ArgumentNullException(nameof(domain));
            this.table = table;
            this.Backup(this.table.NewRow());
        }

        protected DomainBasedRow(Domain domain, DataTable table, string parentID)
            : this(domain, table)
        {
            if (parentID != null)
                this.fields[CremaSchema.__ParentID__] = parentID;
        }

        public void Delete(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            if (this.row == null)
                throw new InvalidOperationException();
            this.domain.Dispatcher.Invoke(() =>
            {
                var keys = this.row.GetKeys();
                this.domain.RemoveRow(authentication, this.table.TableName, keys);
            });
        }

        public void EndNew(Authentication authentication)
        {
            if (this.row != null)
                throw new NotImplementedException();

            var tableName = this.table is InternalDataTable internalDataTable
                ? internalDataTable.LocalName
                : this.table.TableName;
            var fields = this.fields.Values.ToArray();
            var keys = this.domain.Dispatcher.Invoke(() => this.domain.NewRow(authentication, tableName, fields));
            this.row = this.table.Rows.Find(keys);
            this.fields = null;
        }

        public bool IsNew
        {
            get { return this.row == null; }
        }

        public DataRow Row
        {
            get { return this.row; }
        }

        public abstract CremaDispatcher Dispatcher
        {
            get;
        }

        public abstract DataBase DataBase
        {
            get;
        }

        protected T GetField<T>(string columnName)
        {
            if (this.fields != null)
            {
                return (T)this.fields[columnName];
            }
            return this.row.Field<T>(columnName);
        }

        protected void SetField<T>(Authentication authentication, string columnName, T value)
        {
            if (this.fields != null)
            {
                if (this.fields.ContainsKey(columnName) == false)
                    throw new KeyNotFoundException(columnName);
                this.fields[columnName] = value;
            }
            else
            {
                this.domain.Dispatcher.Invoke(() =>
                {
                    var keys = this.row.GetKeys();
                    var fields = new object[this.table.Columns.Count];
                    var column = this.table.Columns[columnName];
                    fields[column.Ordinal] = value;
                    this.domain.SetRow(authentication, this.table.TableName, keys, fields);
                });
            }
        }

        private void Backup(DataRow row)
        {
            this.fields = new Dictionary<string, object>(this.table.Columns.Count);

            for (var i = 0; i < this.table.Columns.Count; i++)
            {
                var column = this.table.Columns[i];
                var field = row[column];
                this.fields.Add(column.ColumnName, field == DBNull.Value ? null : field);
            }
        }
    }
}
