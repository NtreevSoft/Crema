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
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Services.Data
{
    abstract class TableTemplateBase : ITableTemplate, IDomainHost
    {
        private TableTemplateDomain domain;
        private DataTable table;

        private readonly List<TableColumn> columns = new List<TableColumn>();

        private EventHandler editBegun;
        private EventHandler editEnded;
        private EventHandler editCanceled;
        private EventHandler changed;

        private bool isModified;

        public TableColumn AddNew(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                return new TableColumn(this, this.TemplateSource.View.Table);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void EndNew(Authentication authentication, TableColumn column)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.table.RowChanged -= Table_RowChanged;
                try
                {
                    column.EndNew(authentication);
                    this.columns.Add(column);
                }
                finally
                {
                    this.table.RowChanged += Table_RowChanged;
                }
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void BeginEdit(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(BeginEdit));
                this.ValidateBeginEdit(authentication);
                this.Sign(authentication);
                this.OnBeginEdit(authentication);
                this.OnEditBegun(EventArgs.Empty);
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
                this.CremaHost.DebugMethod(authentication, this, nameof(EndEdit));
                this.ValidateEndEdit(authentication);
                this.Sign(authentication);
                this.OnEndEdit(authentication, this.TemplateSource);
                this.OnEditEnded(EventArgs.Empty);
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
                this.CremaHost.DebugMethod(authentication, this, nameof(CancelEdit));
                this.ValidateCancelEdit(authentication);
                this.Sign(authentication);
                this.OnCancelEdit(authentication);
                this.OnEditCanceled(EventArgs.Empty);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetTableName(Authentication authentication, string value)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.domain.Dispatcher.Invoke(() => this.domain.SetProperty(authentication, CremaSchema.TableName, value));
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetTags(Authentication authentication, TagInfo value)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.domain.Dispatcher.Invoke(() => this.domain.SetProperty(authentication, CremaSchema.Tags, value.ToString()));
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetComment(Authentication authentication, string value)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.domain.Dispatcher.Invoke(() => this.domain.SetProperty(authentication, CremaSchema.Comment, value));
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public abstract Type GetType(string typeName);

        public bool Contains(string columnName)
        {
            this.Dispatcher?.VerifyAccess();
            return this.columns.Any(item => item.Name == columnName);
        }

        public bool IsBeingEdited => this.domain != null;

        public bool IsNew { get; set; }

        public Domain Domain
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.domain;
            }
        }

        public abstract IPermission Permission { get; }

        public int Count
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.columns.Count;
            }
        }

        public abstract DomainContext DomainContext { get; }

        public abstract string ItemPath { get; }

        public abstract CremaDispatcher Dispatcher { get; }

        public abstract CremaHost CremaHost { get; }

        public abstract ITable Table { get; }

        public abstract DataBase DataBase { get; }

        public string TableName
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.TemplateSource.TableName;
            }
        }

        public TagInfo Tags
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.TemplateSource.Tags;
            }
        }

        public string Comment
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.TemplateSource.Comment;
            }
        }

        public TableColumn this[string columnName]
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.columns.FirstOrDefault(item => item.Name == columnName);
            }
        }

        public string[] SelectableTypes
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.TemplateSource.Types;
            }
        }

        public IEnumerable<TableColumn> PrimaryKey
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                foreach (var item in this.columns)
                {
                    if (item.IsNew == true)
                        continue;

                    if (item.IsKey == true)
                        yield return item;
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

        protected virtual void OnBeginEdit(Authentication authentication)
        {
            this.TemplateSource = this.CreateSource(authentication);
            this.domain = new TableTemplateDomain(authentication, this.TemplateSource, this.DataBase, this.ItemPath, this.GetType().Name)
            {
                IsNew = this.IsNew,
                Host = this
            };

            this.table = this.TemplateSource.View.Table;
            for (var i = 0; i < this.table.Rows.Count; i++)
            {
                var item = this.table.Rows[i];
                this.columns.Add(new TableColumn(this, item));
            }
            this.table.RowDeleted += Table_RowDeleted;
            this.table.RowChanged += Table_RowChanged;

            this.DomainContext.Domains.Add(authentication, this.domain);
            this.domain.Dispatcher.Invoke(() =>
            {
                this.AttachDomainEvent();
                this.domain.AddUser(authentication, DomainAccessType.ReadWrite);
            });
        }

        protected virtual void OnEndEdit(Authentication authentication, CremaTemplate template)
        {
            this.domain?.Dispatcher?.Invoke(() =>
            {
                this.DetachDomainEvent();
                this.domain.Dispose(authentication, false);
            });
            this.domain = null;
            if (this.table != null)
            {
                this.table.RowDeleted -= Table_RowDeleted;
                this.table.RowChanged -= Table_RowChanged;
            }
            this.isModified = false;
            this.table = null;
            this.columns.Clear();
        }

        protected virtual void OnCancelEdit(Authentication authentication)
        {
            this.domain?.Dispatcher?.Invoke(() =>
            {
                this.DetachDomainEvent();
                this.domain.Dispose(authentication, true);
            });
            this.domain = null;
            if (this.table != null)
            {
                this.table.RowDeleted -= Table_RowDeleted;
                this.table.RowChanged -= Table_RowChanged;
            }
            this.isModified = false;
            this.table = null;
            this.columns.Clear();
        }

        protected virtual void OnRestore(Domain domain)
        {
            this.TemplateSource = domain.Source as CremaTemplate;
            this.domain = domain as TableTemplateDomain;

            if (this.TemplateSource != null)
            {
                this.table = this.TemplateSource.View.Table;
                for (var i = 0; i < this.table.Rows.Count; i++)
                {
                    var item = this.table.Rows[i];
                    this.columns.Add(new TableColumn(this, item));
                }
                this.table.RowDeleted += Table_RowDeleted;
                this.table.RowChanged += Table_RowChanged;
            }
            this.domain.Dispatcher.Invoke(() =>
            {
                this.isModified = this.domain.IsModified;
                this.AttachDomainEvent();
            });
        }

        protected virtual void OnDetach()
        {
            this.domain.Dispatcher.Invoke(() =>
            {
                this.DetachDomainEvent();
            });
            this.domain = null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ValidateBeginEdit(Authentication authentication)
        {
            if (this.domain != null)
                throw new InvalidOperationException(Resources.Exception_ItIsAlreadyBeingEdited);
            this.OnValidateBeginEdit(authentication, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ValidateEndEdit(Authentication authentication)
        {
            if (this.domain == null)
                throw new InvalidOperationException(Resources.Exception_TableTemplateIsNotBeingEdited);
            this.OnValidateEndEdit(authentication, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ValidateCancelEdit(Authentication authentication)
        {
            if (this.domain == null)
                throw new InvalidOperationException(Resources.Exception_TableTemplateIsNotBeingEdited);
            this.OnValidateCancelEdit(authentication, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateBeginEdit(Authentication authentication, object target)
        {

        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateEndEdit(Authentication authentication, object target)
        {
            if (target == this)
            {
                if (this.TemplateSource.Columns.Any() == false)
                    throw new InvalidOperationException(Resources.Exception_AtLeastOneColumnInTable);
                if (this.TemplateSource.TargetTable.PrimaryKey.Any() == false)
                    throw new InvalidOperationException(Resources.Exception_AtLeastOneKeyInTable);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateCancelEdit(Authentication authentication, object target)
        {

        }

        protected CremaTemplate TemplateSource { get; private set; }

        protected abstract CremaTemplate CreateSource(Authentication authentication);

        protected virtual void OnDomainDeleted(Authentication authentication)
        {
            this.domain = null;
            this.table.RowDeleted -= Table_RowDeleted;
            this.table.RowChanged -= Table_RowChanged;
            this.isModified = false;
            this.table = null;
            this.columns.Clear();
            this.OnEditCanceled(EventArgs.Empty);
        }

        protected void Sign(Authentication authentication)
        {
            authentication.Sign();
        }

        private void Table_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            var column = this.columns.FirstOrDefault(item => item.Row == e.Row);
            this.columns.Remove(column);
        }

        private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
            {
                this.Dispatcher.InvokeAsync(() => this.columns.Add(new TableColumn(this, e.Row)));
            }
        }

        private void Domain_Deleted(object sender, DomainDeletedEventArgs e)
        {
            var isCanceled = e.IsCanceled;
            this.Dispatcher?.InvokeAsync(() =>
            {
                if (isCanceled == false)
                {
                    this.OnEndEdit(e.Authentication, this.TemplateSource);
                    this.OnEditEnded(e);
                }
                else
                {
                    this.OnCancelEdit(e.Authentication);
                    this.OnEditCanceled(e);
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

        #region ITableTemplate

        IType ITableTemplate.GetType(string typeName)
        {
            this.Dispatcher?.VerifyAccess();
            return this.GetType(typeName);
        }

        ITableColumn ITableTemplate.AddNew(Authentication authentication)
        {
            return this.AddNew(authentication);
        }

        void ITableTemplate.EndNew(Authentication authentication, ITableColumn column)
        {
            if (column == null)
                throw new ArgumentNullException();
            if (column is TableColumn == false)
                throw new ArgumentException(nameof(column));
            this.EndNew(authentication, column as TableColumn);
        }

        ITable ITableTemplate.Table => this.Table;

        IDomain ITableTemplate.Domain => this.Domain;

        ITableColumn ITableTemplate.this[string columnName] => this[columnName];

        IEnumerable<ITableColumn> ITableTemplate.PrimaryKey => this.PrimaryKey;

        #endregion

        #region IDomainHost

        void IDomainHost.Restore(Domain domain)
        {
            this.OnRestore(domain);
            this.OnEditBegun(EventArgs.Empty);
        }

        void IDomainHost.Detach()
        {
            this.OnDetach();
        }

        void IDomainHost.ValidateDelete(Authentication authentication, bool isCanceled)
        {
            if (isCanceled == false)
            {
                this.Dispatcher.Invoke(() => this.ValidateEndEdit(authentication));
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator<ITableColumn> IEnumerable<ITableColumn>.GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            return this.columns.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            return this.columns.GetEnumerator();
        }

        #endregion
    }
}
