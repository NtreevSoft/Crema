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
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.Domains;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Services.Data
{
    abstract class DomainBasedRow
    {
        private readonly Domain domain;
        private readonly DataTable table;
        private Dictionary<string, object> fields;

        protected DomainBasedRow(Domain domain, DataRow row)
        {
            this.domain = domain ?? throw new ArgumentNullException(nameof(domain));
            this.Row = row;
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
            try
            {
            this.DataBase.ValidateBeginInDataBase(authentication);
            if (this.Row == null)
                    throw new InvalidOperationException();
            this.domain.Dispatcher.Invoke(() =>
            {
                var keys = this.Row.GetKeys();
                this.domain.RemoveRow(authentication, this.table.TableName, keys);
            });
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void EndNew(Authentication authentication)
        {
            try
        {
            if (this.Row != null)
                throw new InvalidOperationException();

            var fields = this.fields.Values.ToArray();
            var keys = this.domain.Dispatcher.Invoke(() => this.domain.NewRow(authentication, this.table.TableName, fields));
            this.Row = this.table.Rows.Find(keys);
            this.fields = null;
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public bool IsNew => this.Row == null;

        public DataRow Row { get; private set; }

        public abstract CremaDispatcher Dispatcher { get; }

        public abstract DataBase DataBase { get; }

        public abstract CremaHost CremaHost { get; }

        protected T GetField<T>(string columnName)
        {
            if (this.fields != null)
            {
                return (T)this.fields[columnName];
            }
            return this.Row.Field<T>(columnName);
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
                    var keys = this.Row.GetKeys();
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
