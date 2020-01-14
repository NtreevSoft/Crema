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
    class TableContent : TableContentBase, ITableContent, IDomainHost
    {
        private readonly Table table;
        private readonly TableContentCollection childs;
        private Domain domain;
        private CremaDataSet dataSet;
        private CremaDataTable dataTable;

        private EventHandler editBegun;
        private EventHandler editEnded;
        private EventHandler editCanceled;
        private EventHandler changed;

        private bool isModified;

        private Guid masterUserToken;

        public TableContent(Table table)
        {
            this.table = table;
            this.childs = new TableContentCollection(table);
        }

        public override string ToString()
        {
            return this.table.ToString();
        }

        public void BeginEdit(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(BeginEdit), this.table);
            var result = this.table.Service.BeginTableContentEdit(this.table.Name);
            this.Sign(authentication, result);
            if (this.domain == null)
            {
                this.domain = this.DomainContext.Create(authentication, result.Value);
                this.domain.Host = this;
                this.domain.Dispatcher.Invoke(() =>
                {
                    this.AttachDomainEvent();
                });
            }
            this.BeginContent(authentication, this.domain);
            this.InvokeEditBegunEvent(EventArgs.Empty);
        }

        public void EndEdit(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(EndEdit), this.table);
            var result = this.table.Service.EndTableContentEdit(this.table.Name);
            this.Sign(authentication, result);
            if (this.domain != null)
            {
                this.masterUserToken = this.domain.Users.OwnerToken;
                this.DetachDomainEvent();
                this.domain.Dispose(authentication, false);
            }
            else
            {
                this.DomainContext.Delete(authentication, this.table.DataBase.ID, this.table.Path, nameof(TableContent), false);
            }
            this.EndContent(authentication, result.Value);
            this.InvokeEditEndedEvent(EventArgs.Empty);
        }

        public void CancelEdit(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(CancelEdit), this.table);
            var result = this.table.Service.CancelTableContentEdit(this.table.Name);
            this.Sign(authentication, result);
            if (this.domain != null)
            {
                this.DetachDomainEvent();
                this.domain.Dispose(authentication, true);
            }
            else
            {
                this.DomainContext.Delete(authentication, this.table.DataBase.ID, this.table.Path, nameof(TableContent), true);
            }
            this.CancelContent(authentication);
            this.InvokeEditCanceledEvent(EventArgs.Empty);
        }

        public void EnterEdit(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(EnterEdit), this.table);
            var result = this.table.Service.EnterTableContentEdit(this.table.Name);
            this.Sign(authentication, result);
            this.domain.Dispatcher.Invoke(() => this.domain.Initialize(authentication, result.Value));
            this.EnterContent(authentication, this.domain);
        }

        public void LeaveEdit(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(LeaveEdit), this.table);
            var result = this.table.Service.LeaveTableContentEdit(this.table.Name);
            this.Sign(authentication, result);
            this.domain.Dispatcher.Invoke(() => this.domain.Release(authentication, result.Value));
            this.LeaveContent(authentication);
        }

        public void Clear(Authentication authentication)
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

        public TableRow AddNew(Authentication authentication, string relationID)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            if (this.domain == null)
                throw new InvalidOperationException(Resources.Exception_NotBeingEdited);
            var view = this.dataTable.DefaultView;
            return new TableRow(this, view.Table, relationID);
        }

        public void EndNew(Authentication authentication, TableRow row)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            if (this.domain == null)
                throw new InvalidOperationException(Resources.Exception_NotBeingEdited);
            row.EndNew(authentication);
            this.Add(row);
        }

        public override Domain Domain
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.domain;
            }
        }

        public IPermission Permission
        {
            get { return this.table; }
        }

        public Table Table
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.table;
            }
        }

        public CremaHost CremaHost
        {
            get { return this.table.CremaHost; }
        }

        public override DataBase DataBase
        {
            get { return this.table.DataBase; }
        }

        public override CremaDispatcher Dispatcher
        {
            get { return this.table.Dispatcher; }
        }

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

        public CremaDataSet DataSet
        {
            get { return this.dataSet; }
        }

        public override CremaDataTable DataTable
        {
            get { return this.dataTable; }
        }

        public DomainContext DomainContext
        {
            get { return this.table.CremaHost.DomainContext; }
        }

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

        private void Domain_Deleted(object sender, DomainDeletedEventArgs e)
        {
            var isCanceled = e.IsCanceled;
            this.Dispatcher?.InvokeAsync(() =>
            {
                if (isCanceled == false)
                {
                    this.EndContent(e.Authentication, null);
                    this.InvokeEditEndedEvent(e);
                }
                else
                {
                    this.CancelContent(e.Authentication);
                    this.InvokeEditCanceledEvent(e);
                }
            });
        }

        private void Domain_RowAdded(object sender, DomainRowEventArgs e)
        {
            this.isModified = this.domain.IsModified;
            this.Dispatcher.InvokeAsync(() => this.OnChanged(e));
        }

        private void Domain_RowChanged(object sender, DomainRowEventArgs e)
        {
            this.isModified = this.domain.IsModified;
            this.Dispatcher.InvokeAsync(() => this.OnChanged(e));
        }

        private void Domain_RowRemoved(object sender, DomainRowEventArgs e)
        {
            this.isModified = this.domain.IsModified;
            this.Dispatcher.InvokeAsync(() => this.OnChanged(e));
        }

        private void Domain_PropertyChanged(object sender, DomainPropertyEventArgs e)
        {
            this.isModified = this.domain.IsModified;
            this.Dispatcher.InvokeAsync(() => this.OnChanged(e));
        }

        private void AttachDomainEvent()
        {
            this.domain.Deleted += Domain_Deleted;
            this.domain.RowAdded += Domain_RowAdded;
            this.domain.RowChanged += Domain_RowChanged;
            this.domain.RowRemoved += Domain_RowRemoved;
            this.domain.PropertyChanged += Domain_PropertyChanged;
            this.domain.UserChanged += Domain_UserChanged;
        }

        private void DetachDomainEvent()
        {
            this.domain.Deleted -= Domain_Deleted;
            this.domain.RowAdded -= Domain_RowAdded;
            this.domain.RowChanged -= Domain_RowChanged;
            this.domain.RowRemoved -= Domain_RowRemoved;
            this.domain.PropertyChanged -= Domain_PropertyChanged;
            this.domain.UserChanged -= Domain_UserChanged;
        }

        private void Domain_UserChanged(object sender, DomainUserEventArgs e)
        {
            if (this.masterUserToken == this.domain.Users.OwnerToken)
                return;

            this.masterUserToken = this.domain.Users.OwnerToken;
            var items = EnumerableUtility.Friends(this, this.Childs);
            foreach (var item in items)
            {
                var tableState = item.table.TableState;
                if (this.masterUserToken == this.CremaHost.Token)
                    tableState |= TableState.IsOwner;
                else
                    tableState &= ~TableState.IsOwner;
                item.table.SetTableState(tableState);
            }
            Authentication.System.Sign();
            this.Container.InvokeTablesStateChangedEvent(Authentication.System, items.Select(i => i.table).ToArray());
        }

        private void InvokeEditBegunEvent(EventArgs e)
        {
            var items = EnumerableUtility.Friends(this, this.Childs);
            foreach (var item in items)
            {
                item.OnEditBegun(e);
            }
        }

        private void InvokeEditEndedEvent(EventArgs e)
        {
            var items = EnumerableUtility.Friends(this, this.Childs);
            foreach (var item in items)
            {
                item.OnEditEnded(e);
            }
        }

        private void InvokeEditCanceledEvent(EventArgs e)
        {
            var items = EnumerableUtility.Friends(this, this.Childs);
            foreach (var item in items)
            {
                item.OnEditCanceled(e);
            }
        }

        private void BeginContent(Authentication authentication, Domain domain)
        {
            var items = EnumerableUtility.Friends(this, this.Childs);
            foreach (var item in items)
            {
                item.domain = domain;
                item.table.SetTableState(TableState.IsBeingEdited);
            }
            this.Container.InvokeTablesStateChangedEvent(authentication, items.Select(i => i.table).ToArray());
        }

        private void EndContent(Authentication authentication, TableInfo[] tableInfos)
        {
            var items = EnumerableUtility.Friends(this.table, this.table.Childs);
            foreach (var item in items.Select(i => i.Content))
            {
                item.domain = null;
                item.table.SetTableState(TableState.None);
                if (tableInfos != null)
                    item.table.UpdateContent(tableInfos.First(i => i.Name == item.table.Name));
                item.dataSet = null;
                item.dataTable = null;
            }
            this.Container.InvokeTablesContentChangedEvent(authentication, this, items.ToArray());
            this.Container.InvokeTablesStateChangedEvent(authentication, items.ToArray());
        }

        private void CancelContent(Authentication authentication)
        {
            var items = EnumerableUtility.Friends(this.table, this.table.Childs);
            foreach (var item in items.Select(i => i.Content))
            {
                item.domain = null;
                item.dataSet = null;
                item.dataTable = null;
                item.table.SetTableState(TableState.None);
            }
            this.Container.InvokeTablesStateChangedEvent(authentication, items.ToArray());
        }

        private void EnterContent(Authentication authentication, Domain domain)
        {
            var items = EnumerableUtility.Friends(this, this.Childs);
            foreach (var item in items)
            {
                var dataSet = domain.Source as CremaDataSet;
                var tableState = item.table.TableState;
                item.dataSet = dataSet;
                item.dataTable = dataSet?.Tables[item.table.Name, item.table.Category.Path];
                if (dataSet != null)
                    tableState |= TableState.IsMember;
                if (this.masterUserToken == authentication.Token)
                    tableState |= TableState.IsOwner;
                item.table.SetTableState(tableState);
            }

            this.Container.InvokeTablesStateChangedEvent(authentication, items.Select(i => i.table).ToArray());
        }

        private void LeaveContent(Authentication authentication)
        {
            var items = EnumerableUtility.Friends(this.table, this.table.Childs);
            foreach (var item in items.Select(i => i.Content))
            {
                item.dataSet = null;
                item.dataTable = null;
                item.table.SetTableState(item.table.TableState & ~TableState.IsMember);
            }

            this.Container.InvokeTablesStateChangedEvent(authentication, items.ToArray());
        }

        private void Sign(Authentication authentication, ResultBase result)
        {
            result.Validate(authentication);
        }

        private void Sign<T>(Authentication authentication, ResultBase<T> result)
        {
            result.Validate(authentication);
        }

        private TableCollection Container
        {
            get { return this.table.Container; }
        }

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

        IDomain ITableContent.Domain
        {
            get { return this.Domain; }
        }

        ITable ITableContent.Table
        {
            get { return this.Table; }
        }

        ITableContent ITableContent.Parent
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                if (this.table.Parent != null)
                {
                    return this.table.Parent.Content;
                }
                return null;
            }
        }

        ITableContentCollection ITableContent.Childs
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.childs;
            }
        }

        Domain IDomainHost.Domain => throw new NotImplementedException();

        #endregion

        #region IDomainHost

        void IDomainHost.Restore(Domain domain)
        {
            this.domain = domain;
            Authentication.System.Sign();
            this.BeginContent(Authentication.System, this.domain);
            if (this.domain.Source != null)
                this.EnterContent(Authentication.System, this.domain);
            this.masterUserToken = this.domain.Users.OwnerToken;
            this.domain.Dispatcher.Invoke(() =>
            {
                this.isModified = this.domain.IsModified;
                this.AttachDomainEvent();
            });
        }

        void IDomainHost.OnRestoredEvent(Domain domain)
        {
            this.Dispatcher.Invoke(() => {
                this.InvokeEditBegunEvent(EventArgs.Empty);
            });
        }

        void IDomainHost.Detach()
        {
            this.domain.Dispatcher.Invoke(() =>
            {
                this.DetachDomainEvent();
            });
            this.domain = null;
        }

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
