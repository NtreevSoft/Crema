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
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.Domains;
using Ntreev.Crema.Services.Properties;
using Ntreev.Library.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ntreev.Crema.Services.Data
{
    partial class TableContent : TableContentBase, ITableContent
    {
        private readonly Table table;
        private Domain domain;
        private CremaDataTable dataTable;

        private EventHandler editBegun;
        private EventHandler editEnded;
        private EventHandler editCanceled;
        private EventHandler changed;

        private bool isModified;

        public TableContent(Table table)
        {
            this.table = table;
        }

        public override string ToString()
        {
            return this.table.ToString();
        }

        public void BeginEdit(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(BeginEdit), this.table);
                var result = this.table.Service.BeginTableContentEdit(this.table.Name);
                this.Sign(authentication, result);
                if (this.domain == null)
                {
                    this.domain = this.DomainContext.Create(authentication, result.Value);
                    this.domain.Host = new TableContentDomainHost(this.Container, this.domain, this.domain.DomainInfo.ItemPath);
                    this.domain.Dispatcher.Invoke(this.domainHost.AttachDomainEvent);
                }
                this.domainHost.BeginContent(authentication, this.domain);
                this.domainHost.InvokeEditBegunEvent(EventArgs.Empty);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void EndEdit(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(EndEdit), this.table);
                var result = this.table.Service.EndTableContentEdit(this.table.Name);
                this.Sign(authentication, result);
                if (this.domain != null)
                {
                    this.domain.Dispatcher.Invoke(() =>
                    {
                        this.domainHost.DetachDomainEvent();
                        this.domain.Dispose(authentication, false);
                    });
                }
                else
                {
                    this.DomainContext.Delete(authentication, this.table.DataBase.ID, this.table.Path, nameof(TableContent), false);
                }
                this.domainHost.EndContent(authentication, result.Value);
                this.domainHost.InvokeEditEndedEvent(EventArgs.Empty);
                this.domainHost = null;
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void CancelEdit(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(CancelEdit), this.table);
                var result = this.table.Service.CancelTableContentEdit(this.table.Name);
                this.Sign(authentication, result);
                if (this.domain != null)
                {
                    this.domain.Dispatcher.Invoke(() =>
                    {
                        this.domainHost.DetachDomainEvent();
                        this.domain.Dispose(authentication, true);
                    });
                }
                else
                {
                    this.DomainContext.Delete(authentication, this.table.DataBase.ID, this.table.Path, nameof(TableContent), true);
                }
                this.domainHost.CancelContent(authentication);
                this.domainHost.InvokeEditCanceledEvent(EventArgs.Empty);
                this.domainHost = null;
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void EnterEdit(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(EnterEdit), this.table);
                var result = this.table.Service.EnterTableContentEdit(this.table.Name);
                this.Sign(authentication, result);
                this.domain.Dispatcher.Invoke(() => this.domain.Initialize(authentication, result.Value));
                this.domainHost.EnterContent(authentication, this.domain);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void LeaveEdit(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(LeaveEdit), this.table);
                var result = this.table.Service.LeaveTableContentEdit(this.table.Name);
                this.Sign(authentication, result);
                this.domain.Dispatcher.Invoke(() => this.domain.Release(authentication, result.Value));
                this.domainHost.LeaveContent(authentication);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Clear(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(Clear), this.table);
                var rowInfo = new DomainRowInfo()
                {
                    TableName = this.table.Name,
                    Keys = DomainRowInfo.ClearKey,
                };
                this.domain.Dispatcher.Invoke(() => this.domain.RemoveRow(authentication, new DomainRowInfo[] { rowInfo, }));
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public TableRow AddNew(Authentication authentication, string relationID)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                if (this.domain == null)
                    throw new InvalidOperationException(Resources.Exception_NotBeingEdited);
                var view = this.dataTable.DefaultView;
                return new TableRow(this, view.Table);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void EndNew(Authentication authentication, TableRow row)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                if (this.domain == null)
                    throw new InvalidOperationException(Resources.Exception_NotBeingEdited);
                row.EndNew(authentication);
                this.Add(row);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public override Domain Domain
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.domain;
            }
        }

        public IPermission Permission => this.table;

        public Table Table
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.table;
            }
        }

        public override CremaHost CremaHost => this.table.CremaHost;

        public override DataBase DataBase => this.table.DataBase;

        public override CremaDispatcher Dispatcher => this.table.Dispatcher;

        public int Count
        {
            get
            {
                if (this.Dispatcher == null)
                    return 0;
                this.Dispatcher?.VerifyAccess();
                return this.dataTable.Rows.Count;
            }
        }

        public override CremaDataTable DataTable => this.dataTable;

        public DomainContext DomainContext => this.table.CremaHost.DomainContext;

        public IEnumerable<TableContent> Childs
        {
            get
            {
                if (this.table != null)
                {
                    foreach (var item in this.table.Childs)
                    {
                        yield return item.Content;
                    }
                }
            }
        }

        public bool IsModified
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.isModified;
            }
        }

        public event EventHandler EditBegun
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.editBegun += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.editBegun -= value;
            }
        }

        public event EventHandler EditEnded
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.editEnded += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.editEnded -= value;
            }
        }

        public event EventHandler EditCanceled
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.editCanceled += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.editCanceled -= value;
            }
        }

        public event EventHandler Changed
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.changed += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.changed -= value;
            }
        }

        protected virtual void OnEditBegun(EventArgs e)
        {
            this.editBegun?.Invoke(this, e);
        }

        protected virtual void OnEditEnded(EventArgs e)
        {
            this.editEnded?.Invoke(this, e);
        }

        protected virtual void OnEditCanceled(EventArgs e)
        {
            this.editCanceled?.Invoke(this, e);
        }

        protected virtual void OnChanged(EventArgs e)
        {
            this.changed?.Invoke(this, e);
        }

        private void Sign(Authentication authentication, ResultBase result)
        {
            result.Validate(authentication);
        }

        private void Sign<T>(Authentication authentication, ResultBase<T> result)
        {
            result.Validate(authentication);
        }

        private TableCollection Container => this.table.Container;

        #region ITableContent

        ITableRow ITableContent.AddNew(Authentication authentication, string relationID)
        {
            return this.AddNew(authentication, relationID);
        }

        void ITableContent.EndNew(Authentication authentication, ITableRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            if (row is TableRow == false)
                throw new ArgumentException(Resources.Exception_InvalidObject, nameof(row));
            this.EndNew(authentication, row as TableRow);
        }

        ITableRow ITableContent.Find(Authentication authentication, params object[] keys)
        {
            return this.Find(authentication, keys);
        }

        ITableRow[] ITableContent.Select(Authentication authentication, string filterExpression)
        {
            return this.Select(authentication, filterExpression);
        }

        IDomain ITableContent.Domain => this.Domain;

        ITable ITableContent.Table => this.Table;

        ITable[] ITableContent.Tables => this.domainHost != null ? this.domainHost.Tables : new ITable[] { };

        #endregion

        #region IEnumerable

        IEnumerator<ITableRow> IEnumerable<ITableRow>.GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            return this.GetEnumerator();
        }

        #endregion
    }
}
