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
using Ntreev.Crema.Services.Properties;
using Ntreev.Library.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Services.Data
{
    class TableContent : TableContentBase, ITableContent, IDomainHost
    {
        private readonly Table table;
        private readonly TableContentCollection childs;
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
            this.childs = new TableContentCollection(table);
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
                this.ValidateBeginEdit(authentication);
                this.Sign(authentication);
                if (this.domain == null)
                {
                    var dataSet = this.table.ReadEditableData(authentication);
                    //this.DataSet = dataSet;
                    //this.dataTable = this.DataSet.Tables[this.table.Name, this.table.Category.Path];
                    this.domain = new TableContentDomain(authentication, dataSet, this.table.DataBase, this.GetType().Name)
                    {
                        Host = this
                    };
                    this.DomainContext.Domains.Add(authentication, this.domain);
                    this.BeginContent(authentication, this.domain);
                    this.domain.Dispatcher.Invoke(() =>
                    {
                        this.AttachDomainEvent();
                    });
                }
                this.BeginContent(authentication, this.domain);
                this.InvokeEditBegunEvent(EventArgs.Empty);
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
                this.ValidateEndEdit(authentication);
                this.Sign(authentication);
                var isModifeid = this.domain.Dispatcher.Invoke(() =>
                {
                    this.DetachDomainEvent();
                    this.domain.Dispose(authentication, false);
                    return this.domain.IsModified;
                });
                this.EndContent(authentication, isModifeid);
                this.InvokeEditEndedEvent(EventArgs.Empty);
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
                this.ValidateCancelEdit(authentication);
                this.Sign(authentication);
                this.domain.Dispatcher.Invoke(() =>
                {
                    this.DetachDomainEvent();
                    this.domain.Dispose(authentication, true);
                });
                this.CancelContent(authentication);
                this.InvokeEditCanceledEvent(EventArgs.Empty);
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
                this.ValidateEnter(authentication);
                this.Sign(authentication);
                var accessType = this.GetAccessType(authentication);
                this.domain.Dispatcher.Invoke(() => this.domain.AddUser(authentication, accessType));
                this.EnterContent(authentication);
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
                this.ValidateLeave(authentication);
                this.Sign(authentication);
                this.domain.Dispatcher.Invoke(() => this.domain.RemoveUser(authentication));
                this.LeaveContent(authentication);
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
                this.domain.Dispatcher.Invoke(() => this.domain.RemoveRow(authentication, new DomainRowInfo[] { rowInfo }));
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
                    throw new InvalidOperationException(Resources.Exception_TableIsNotBeingEdited);
                var view = this.dataTable.DefaultView;
                return new TableRow(this, view.Table, relationID);
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
                    throw new InvalidOperationException(Resources.Exception_TableIsNotBeingEdited);
                row.EndNew(authentication);
                this.Add(row);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ValidateBeginEdit(Authentication authentication)
        {
            var isAdmin = authentication.Types.HasFlag(AuthenticationType.Administrator);
            var items = this.table.GetRelations().Distinct().OrderBy(item => item.Name);
            foreach (var item in items)
            {
                item.Content.OnValidateBeginEdit(authentication, this);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ValidateEndEdit(Authentication authentication)
        {
            if (this.table.Parent != null)
                throw new NotImplementedException();
            var isAdmin = authentication.Types.HasFlag(AuthenticationType.Administrator);
            if (this.domain == null)
                throw new NotImplementedException();
            this.domain.Dispatcher?.Invoke(() =>
            {
                var isOwner = this.domain.Users.OwnerUserID == authentication.ID;
                if (isAdmin == false && isOwner == false)
                    throw new NotImplementedException();
            });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ValidateEnter(Authentication authentication)
        {
            if (this.table.Parent != null)
                throw new NotImplementedException();

            var items = EnumerableUtility.Friends(this, this.Childs);
            foreach (var item in items)
            {
                item.OnValidateEnter(authentication, this);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ValidateLeave(Authentication authentication)
        {
            if (this.table.Parent != null)
                throw new NotImplementedException();

            var items = EnumerableUtility.Friends(this, this.Childs);
            foreach (var item in items)
            {
                item.OnValidateLeave(authentication, this);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ValidateCancelEdit(Authentication authentication)
        {
            if (this.table.Parent != null)
                throw new NotImplementedException();

            var isAdmin = authentication.Types.HasFlag(AuthenticationType.Administrator);
            if (this.domain == null)
                throw new NotImplementedException();
            this.domain.Dispatcher.Invoke(() =>
            {
                if (isAdmin == false && this.domain.Users.Owner.ID != authentication.ID)
                    throw new NotImplementedException();
            });

            var items = EnumerableUtility.Friends(this, this.Childs);
            foreach (var item in items)
            {
                item.OnValidateCancelEdit(authentication, this);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnValidateBeginEdit(Authentication authentication, object target)
        {
            this.table.ValidateAccessType(authentication, AccessType.Guest);

            if (this.table.DataBase.Version.Major != CremaSchema.MajorVersion || this.DataBase.Version.Minor != CremaSchema.MinorVersion)
                throw new InvalidOperationException("database version is low.");

            if (this.domain != null)
                throw new NotImplementedException();

            this.table.ValidateHasNotBeingEditedType();

            if (this.table.Template.IsBeingEdited == true)
                throw new InvalidOperationException(string.Format(Resources.Exception_TableIsBeingEdited_Format, this.table.Name));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnValidateEnter(Authentication authentication, object target)
        {
            this.table.ValidateAccessType(authentication, AccessType.Guest);

            if (this.domain == null)
                throw new NotImplementedException();

            this.table.ValidateHasNotBeingEditedType();

            if (this.table.Template.IsBeingEdited == true)
                throw new InvalidOperationException(string.Format(Resources.Exception_TableIsBeingEdited_Format, this.table.Name));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnValidateLeave(Authentication authentication, object target)
        {
            this.table.ValidateAccessType(authentication, AccessType.Guest);

            if (this.domain == null)
                throw new NotImplementedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnValidateCancelEdit(Authentication authentication, object target)
        {
            if (this.domain == null)
                throw new NotImplementedException();
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

        public CremaDataSet DataSet { get; private set; }

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

        private void Domain_Deleted(object sender, DomainDeletedEventArgs e)
        {
            var isCanceled = e.IsCanceled;
            this.Dispatcher?.InvokeAsync(() =>
            {
                if (isCanceled == false)
                {
                    this.EndContent(e.Authentication, this.isModified);
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
        }

        private void DetachDomainEvent()
        {
            this.domain.Deleted -= Domain_Deleted;
            this.domain.RowAdded -= Domain_RowAdded;
            this.domain.RowChanged -= Domain_RowChanged;
            this.domain.RowRemoved -= Domain_RowRemoved;
            this.domain.PropertyChanged -= Domain_PropertyChanged;
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
                item.DataSet = domain.Source as CremaDataSet;
                item.dataTable = DataSet.Tables[item.table.Name, item.table.Category.Path];
                item.table.SetTableState(TableState.IsBeingEdited);
            }

            this.Container.InvokeTablesStateChangedEvent(authentication, items.Select(i => i.table).ToArray());
        }

        private void EndContent(Authentication authentication, bool isUpdate)
        {
            if (isUpdate == true)
                this.Container.InvokeTableEndContentEdit(authentication, this.table, this.DataSet);
            var isModified = this.domain.IsModified;
            var dataSet = this.DataSet;
            var items = EnumerableUtility.Friends(this, this.Childs);
            foreach (var item in items)
            {
                if (isUpdate == true)
                    item.table.UpdateContent(item.dataTable.TableInfo);
                item.domain = null;
                item.isModified = false;
                item.DataSet = null;
                item.dataTable = null;
                item.table.SetTableState(TableState.None);
            }

            if (isModified == true)
                this.Container.InvokeTablesContentChangedEvent(authentication, this, items.Select(i => i.Table).ToArray(), dataSet);
        }

        private void CancelContent(Authentication authentication)
        {
            var items = EnumerableUtility.Friends(this.table, this.table.Childs);
            foreach (var item in items.Select(i => i.Content))
            {
                item.domain = null;
                item.isModified = false;
                item.DataSet = null;
                item.dataTable = null;
                item.table.SetTableState(TableState.None);
            }

            this.Container.InvokeTablesStateChangedEvent(authentication, items.ToArray());
        }

        private void EnterContent(Authentication authentication)
        {

        }

        private void LeaveContent(Authentication authentication)
        {

        }

        private DomainAccessType GetAccessType(Authentication authentication)
        {
            if (this.table.VerifyAccessType(authentication, AccessType.Editor))
                return DomainAccessType.ReadWrite;
            else if (this.table.VerifyAccessType(authentication, AccessType.Guest))
                return DomainAccessType.Read;
            throw new PermissionDeniedException();
        }

        protected void Sign(Authentication authentication)
        {
            authentication.Sign();
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

        #endregion

        #region IDomainHost

        void IDomainHost.Restore(Domain domain)
        {
            this.domain = domain;
            Authentication.System.Sign();
            this.BeginContent(Authentication.System, this.domain);
            this.InvokeEditBegunEvent(EventArgs.Empty);
            this.domain.Dispatcher.Invoke(() =>
            {
                this.isModified = this.domain.IsModified;
                this.AttachDomainEvent();
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

        void IDomainHost.ValidateDelete(Authentication authentication, bool isCanceled)
        {
            if (isCanceled == false)
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (this.table.Parent != null)
                        throw new NotImplementedException();
                });
            }
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
