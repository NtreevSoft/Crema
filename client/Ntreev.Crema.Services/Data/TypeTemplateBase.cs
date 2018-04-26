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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Ntreev.Library;
using Ntreev.Crema.Services.Properties;

namespace Ntreev.Crema.Services.Data
{
    abstract class TypeTemplateBase : ITypeTemplate, IDomainHost
    {
        private TypeDomain domain;
        private CremaDataType dataType;
        private DataTable table;

        private readonly List<TypeMember> members = new List<TypeMember>();

        private EventHandler editBegun;
        private EventHandler editEnded;
        private EventHandler editCanceled;
        private EventHandler changed;

        private bool isModified;

        public TypeMember AddNew(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            return new TypeMember(this, this.dataType.View.Table);
        }

        public void EndNew(Authentication authentication, TypeMember member)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.table.RowChanged -= Table_RowChanged;
            try
            {
                member.EndNew(authentication);
                this.members.Add(member);
            }
            finally
            {
                this.table.RowChanged += Table_RowChanged;
            }
        }

        public void BeginEdit(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(BeginEdit));
            var result = this.BeginDomain(authentication);
            this.Sign(authentication, result);
            this.OnBeginEdit(authentication, result.Value);
            this.OnEditBegun(EventArgs.Empty);
        }

        public void EndEdit(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(EndEdit));
            var result = this.EndDomain(authentication, this.domain.ID);
            this.Sign(authentication, result);
            this.OnEndEdit(authentication, result.Value);
            this.OnEditEnded(EventArgs.Empty);
        }

        public void CancelEdit(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(CancelEdit));
            var result = this.CancelDomain(authentication, this.domain.ID);
            this.Sign(authentication, result);
            this.OnCancelEdit(authentication);
            this.OnEditCanceled(EventArgs.Empty);
        }

        public void SetTypeName(Authentication authentication, string value)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.domain.Dispatcher.Invoke(() => this.domain.SetProperty(authentication, "TypeName", value));
        }

        public void SetIsFlag(Authentication authentication, bool value)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.domain.Dispatcher.Invoke(() => this.domain.SetProperty(authentication, "IsFlag", value));
        }

        public void SetComment(Authentication authentication, string value)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.domain.Dispatcher.Invoke(() => this.domain.SetProperty(authentication, "Comment", value));
        }

        public bool Contains(string memberName)
        {
            this.Dispatcher?.VerifyAccess();
            return this.members.Any(item => item.Name == memberName);
        }

        public bool IsNew { get; set; }

        public Domain Domain
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.domain;
            }
        }

        public abstract IPermission Permission
        {
            get;
        }

        public int Count
        {
            get
            {
                if (this.Dispatcher == null)
                    return 0;
                this.Dispatcher.VerifyAccess();
                return this.members.Count;
            }
        }

        public abstract DomainContext DomainContext
        {
            get;
        }

        public abstract CremaDispatcher Dispatcher
        {
            get;
        }

        public abstract CremaHost CremaHost
        {
            get;
        }

        public abstract IType Type
        {
            get;
        }

        public abstract DataBase DataBase
        {
            get;
        }

        public string TypeName
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.dataType.Name;
            }
        }

        public bool IsFlag
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.dataType.IsFlag;
            }
        }

        public string Comment
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.dataType.Comment;
            }
        }

        public TypeMember this[string memberName]
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.members.FirstOrDefault(item => item.Name == memberName);
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

        protected virtual void OnBeginEdit(Authentication authentication, DomainMetaData metaData)
        {
            this.domain = this.DomainContext.Create(authentication, metaData) as TypeDomain;
            this.domain.IsNew = this.IsNew;
            this.domain.Host = this;
            this.AttachDomainEvent();

            this.dataType = this.domain.Source as CremaDataType;
            this.table = this.dataType.View.Table;
            for (var i = 0; i < this.table.Rows.Count; i++)
            {
                var item = this.table.Rows[i];
                this.members.Add(new TypeMember(this, item));
            }
            this.table.RowDeleted += Table_RowDeleted;
            this.table.RowChanged += Table_RowChanged;
        }

        protected virtual void OnEndEdit(Authentication authentication, TypeInfo typeInfo)
        {
            this.domain.Dispatcher?.Invoke(() =>
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
            this.members.Clear();
        }

        protected virtual void OnCancelEdit(Authentication authentication)
        {
            this.domain?.Dispatcher?.Invoke(() =>
            {
                this.DetachDomainEvent();
                this.domain.Dispose(authentication, true);
            });
            this.isModified = false;
            this.domain = null;
            if (this.table != null)
            {
                this.table.RowDeleted -= Table_RowDeleted;
                this.table.RowChanged -= Table_RowChanged;
            }
            this.table = null;
            this.members.Clear();
        }

        protected virtual void OnRestore(Domain domain)
        {
            this.dataType = domain.Source as CremaDataType;
            this.domain = domain as TypeDomain;

            if (this.dataType != null)
            {
                this.table = this.dataType.View.Table;
                for (var i = 0; i < this.table.Rows.Count; i++)
                {
                    var item = this.table.Rows[i];
                    this.members.Add(new TypeMember(this, item));
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

        protected virtual void OnDetached()
        {
            this.domain.Dispatcher.Invoke(() =>
            {
                this.DetachDomainEvent();
            });
            this.domain = null;
        }

        protected CremaDataType TypeSource
        {
            get { return this.dataType; }
        }

        protected abstract ResultBase<DomainMetaData> BeginDomain(Authentication authentication);

        protected abstract ResultBase<TypeInfo> EndDomain(Authentication authentication, Guid domainID);

        protected abstract ResultBase CancelDomain(Authentication authentication, Guid domainID);

        private void Sign(Authentication authentication, ResultBase result)
        {
            result.Validate(authentication);
        }

        private void Sign<T>(Authentication authentication, ResultBase<T> result)
        {
            result.Validate(authentication);
        }

        private void Table_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            var column = this.members.FirstOrDefault(item => item.Row == e.Row);
            this.members.Remove(column);
        }

        private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
            {
                this.members.Add(new TypeMember(this, e.Row));
            }
        }

        private void Domain_Deleted(object sender, DomainDeletedEventArgs e)
        {
            var isCanceled = e.IsCanceled;
            this.Dispatcher?.InvokeAsync(() =>
            {
                if (isCanceled == false)
                {
                    this.OnEndEdit(e.Authentication, TypeInfo.Empty);
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

        #region ITypeTemplate

        ITypeMember ITypeTemplate.AddNew(Authentication authentication)
        {
            return this.AddNew(authentication);
        }

        void ITypeTemplate.EndNew(Authentication authentication, ITypeMember member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            if (member is TypeMember == false)
                throw new ArgumentException(Resources.Exception_InvalidObject, nameof(member));
            this.EndNew(authentication, member as TypeMember);
        }

        IType ITypeTemplate.Type
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Type;
            }
        }

        IDomain ITypeTemplate.Domain
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Domain;
            }
        }

        ITypeMember ITypeTemplate.this[string columnName]
        {
            get { return this[columnName]; }
        }

        #endregion

        #region IDomainHost

        void IDomainHost.Restore(Domain domain)
        {
            this.OnRestore(domain);
            this.OnEditBegun(EventArgs.Empty);
        }

        void IDomainHost.Detach()
        {
            this.OnDetached();
        }

        #endregion

        #region IEnumerable

        IEnumerator<ITypeMember> IEnumerable<ITypeMember>.GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            return this.members.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            return this.members.GetEnumerator();
        }

        #endregion
    }
}
