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

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.DomainService;
using Ntreev.Crema.Services.Properties;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ntreev.Crema.Services.Domains
{
    abstract class Domain : DomainBase<Domain, DomainCategory, DomainCollection, DomainCategoryCollection, DomainContext>,
        IDomain, IDomainItem, IInfoProvider, IStateProvider
    {
        private bool initialized;
        private readonly HashSet<string> modifiedTableList = new HashSet<string>();

        private EventHandler<DomainUserEventArgs> userAdded;
        private EventHandler<DomainUserEventArgs> userChanged;
        private EventHandler<DomainUserRemovedEventArgs> userRemoved;
        private EventHandler<DomainRowEventArgs> rowAdded;
        private EventHandler<DomainRowEventArgs> rowRemoved;
        private EventHandler<DomainRowEventArgs> rowChanged;
        private EventHandler<DomainPropertyEventArgs> propertyChanged;
        private EventHandler<DomainDeletedEventArgs> deleted;

        protected Domain(DomainInfo domainInfo, CremaDispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
            this.Initialize(domainInfo); ;
            this.Name = domainInfo.DomainID.ToString();
            this.Users = new DomainUserCollection(this);
        }

        public void Delete(Authentication authentication, bool isCanceled)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(Delete), this);
                var container = this.Container;
                var result = this.Service.DeleteDomain(this.ID, isCanceled);
                this.Sign(authentication, result);
                this.Dispose();
                this.Dispatcher = null;
                this.OnDeleted(new DomainDeletedEventArgs(authentication, this, isCanceled));
                container.InvokeDomainDeletedEvent(authentication, this, isCanceled);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void BeginUserEdit(Authentication authentication, DomainLocationInfo location)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(BeginUserEdit), base.DomainInfo.ItemPath, base.DomainInfo.ItemType);
                var result = this.Service.BeginUserEdit(this.ID, location);
                this.Sign(authentication, result);
                this.InvokeBeginUserEdit(authentication, location, out var domainUser);
                this.OnUserChanged(new DomainUserEventArgs(authentication, this, domainUser));
                this.Container.InvokeDomainUserChangedEvent(authentication, this, domainUser);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void EndUserEdit(Authentication authentication)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                var result = this.Service.EndUserEdit(this.ID);
                this.Sign(authentication, result);
                this.InvokeEndUserEdit(authentication, out var domainUser);
                this.OnUserChanged(new DomainUserEventArgs(authentication, this, domainUser));
                this.Container.InvokeDomainUserChangedEvent(authentication, this, domainUser);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public DomainRowInfo[] NewRow(Authentication authentication, DomainRowInfo[] rows)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(NewRow), this.ID, rows.Length);
                var result = this.Service.NewRow(this.ID, rows);
                this.Sign(authentication, result);
                this.InvokeNewRow(authentication, result.Value);
                this.IsModified = true;
                base.UpdateModificationInfo(authentication.SignatureDate);
                this.OnRowAdded(new DomainRowEventArgs(authentication, this, result.Value));
                this.Container.InvokeDomainRowAddedEvent(authentication, this, result.Value);
                this.Container.InvokeDomainStateChangedEvent(authentication, this);
                this.Container.InvokeDomainInfoChangedEvent(authentication, this);
                return result.Value;
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public DomainRowInfo[] SetRow(Authentication authentication, DomainRowInfo[] rows)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(SetRow), this.ID, rows.Length);
                var result = this.Service.SetRow(this.ID, rows);
                this.Sign(authentication, result);
                this.InvokeSetRow(authentication, result.Value);
                this.IsModified = true;
                base.UpdateModificationInfo(authentication.SignatureDate);
                this.OnRowChanged(new DomainRowEventArgs(authentication, this, result.Value));
                this.Container.InvokeDomainRowChangedEvent(authentication, this, result.Value);
                this.Container.InvokeDomainStateChangedEvent(authentication, this);
                this.Container.InvokeDomainInfoChangedEvent(authentication, this);
                return result.Value;
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void RemoveRow(Authentication authentication, DomainRowInfo[] rows)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(RemoveRow), this.ID, rows.Length);
                var result = this.Service.RemoveRow(this.ID, rows);
                this.Sign(authentication, result);
                this.InvokeRemoveRow(authentication, rows);
                this.IsModified = true;
                base.UpdateModificationInfo(authentication.SignatureDate);
                this.OnRowRemoved(new DomainRowEventArgs(authentication, this, rows));
                this.Container.InvokeDomainRowRemovedEvent(authentication, this, rows);
                this.Container.InvokeDomainStateChangedEvent(authentication, this);
                this.Container.InvokeDomainInfoChangedEvent(authentication, this);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetProperty(Authentication authentication, string propertyName, object value)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(SetProperty), this.ID, propertyName, value);
                var result = this.Service.SetProperty(this.ID, propertyName, value);
                this.Sign(authentication, result);
                this.InvokeSetProperty(authentication, propertyName, value);
                this.OnPropertyChanged(new DomainPropertyEventArgs(authentication, this, propertyName, value));
                this.Container.InvokeDomainPropertyChangedEvent(authentication, this, propertyName, value);
                this.Container.InvokeDomainStateChangedEvent(authentication, this);
                this.Container.InvokeDomainInfoChangedEvent(authentication, this);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetUserLocation(Authentication authentication, DomainLocationInfo location)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(SetUserLocation), base.DomainInfo.ItemPath, base.DomainInfo.ItemType);
                var result = this.Service.SetUserLocation(this.ID, location);
                this.Sign(authentication, result);
                this.InvokeSetUserLocation(authentication, location, out var domainUser);
                this.OnUserChanged(new DomainUserEventArgs(authentication, this, domainUser));
                this.Container.InvokeDomainUserChangedEvent(authentication, this, domainUser);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public DomainUserInfo Kick(Authentication authentication, string userID, string comment)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(Kick), base.DomainInfo.ItemPath, base.DomainInfo.ItemType, userID, comment);
                var result = this.Service.Kick(this.ID, userID, comment);
                this.Sign(authentication, result);
                this.InvokeKick(authentication, userID, comment, out var domainUser, out var removeInfo);
                this.OnUserRemoved(new DomainUserRemovedEventArgs(authentication, this, domainUser, removeInfo));
                this.Container.InvokeDomainUserRemovedEvent(authentication, this, domainUser, removeInfo);
                return result.Value;
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetOwner(Authentication authentication, string userID)
        {
            try
            {
                this.Dispatcher.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(SetOwner), base.DomainInfo.ItemPath, base.DomainInfo.ItemType, userID);
                var result = this.Service.SetOwner(this.ID, userID);
                this.Sign(authentication, result);
                this.InvokeSetOwner(authentication, userID, out var oldOwner, out var newOwner);
                this.Users.Owner = newOwner;
                if (oldOwner != null)
                {
                    this.OnUserChanged(new DomainUserEventArgs(authentication, this, oldOwner));
                    this.Container.InvokeDomainUserChangedEvent(authentication, this, oldOwner);
                }
                this.OnUserChanged(new DomainUserEventArgs(authentication, this, newOwner));
                this.Container.InvokeDomainUserChangedEvent(authentication, this, newOwner);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public DomainMetaData GetMetaData(Authentication authentication)
        {
            this.Dispatcher.VerifyAccess();

            var metaData = new DomainMetaData()
            {
                DomainID = Guid.Parse(this.Name),
                DomainInfo = base.DomainInfo,
                Users = this.Users.Select<DomainUser, DomainUserMetaData>(item => item.GetMetaData(authentication)).ToArray(),
                DomainState = this.DomainState,
                ModifiedTables = this.modifiedTableList.ToArray(),
            };

            if (this.Users.Contains(authentication.ID) == true)
            {
                metaData.Data = this.SerializeSource();
            }

            return metaData;
        }

        public void Initialize(Authentication authentication, DomainMetaData metaData)
        {
            base.DomainState = metaData.DomainState;
            this.modifiedTableList.Clear();
            foreach (var item in metaData.ModifiedTables)
            {
                this.modifiedTableList.Add(item);
            }

            if (metaData.Data == null)
            {
                foreach (var item in metaData.Users)
                {
                    this.InvokeUserAdded(authentication, item.DomainUserInfo, item.DomainUserState);
                }
            }
            else
            {
                this.OnInitialize(metaData);
                this.initialized = true;
                foreach (var item in metaData.Users)
                {
                    if (this.Users.Contains(item.DomainUserInfo.UserID) == false)
                        this.InvokeUserAdded(authentication, item.DomainUserInfo, item.DomainUserState);
                }
            }
        }

        public void Release(Authentication authentication, DomainMetaData metaData)
        {
            this.OnRelease();
            this.initialized = false;

            foreach (var item in this.Users.ToArray<DomainUser>())
            {
                if (metaData.Users.Any(i => i.DomainUserInfo.UserID == item.DomainUserInfo.UserID) == false)
                {
                    this.InvokeUserRemoved(authentication, item.DomainUserInfo, RemoveInfo.Empty);
                }
            }

            foreach (var item in metaData.Users)
            {
                if (item.DomainUserState.HasFlag(DomainUserState.IsOwner) == true)
                {
                    var master = this.Users[item.DomainUserInfo.UserID];
                    this.Users.Owner = master;
                    this.InvokeUserChanged(authentication, item.DomainUserInfo, item.DomainUserState);
                }
            }
        }

        public void Dispose(Authentication authentication, bool isCanceled)
        {
            var container = this.Container;
            this.Dispose();
            this.Dispatcher = null;
            this.OnDeleted(new DomainDeletedEventArgs(authentication, this, isCanceled));
            container.InvokeDomainDeletedEvent(authentication, this, isCanceled);
        }

        public void AttachUser()
        {
            this.Dispatcher.VerifyAccess();

            if (this.Users.Contains(this.CremaHost.UserID) == true)
            {
                var domainUser = this.Users[this.CremaHost.UserID];
                domainUser.IsOnline = true;
            }
        }

        public void DetachUser()
        {
            this.Dispatcher.VerifyAccess();

            if (this.Users.Contains(this.CremaHost.UserID) == true)
            {
                var domainUser = this.Users[this.CremaHost.UserID];
                domainUser.IsOnline = false;
            }
        }

        public object GetService(System.Type serviceType)
        {
            return this.CremaHost.GetService(serviceType);
        }

        public void InvokeDomainInfoChanged(Authentication authentication, DomainInfo domainInfo)
        {
            base.UpdateDomainInfo(domainInfo);
            this.Container.InvokeDomainInfoChangedEvent(authentication, this);
        }

        public void InvokeDomainStateChanged(Authentication authentication, DomainState domainState)
        {
            base.DomainState = domainState;
            this.Container.InvokeDomainStateChangedEvent(authentication, this);
        }

        public void InvokeUserAdded(Authentication authentication, DomainUserInfo domainUserInfo, DomainUserState domainUserState)
        {
            this.Dispatcher.VerifyAccess();

            var domainUser = new DomainUser(this, domainUserInfo, domainUserState);
            this.Users.Add(domainUser);
            if (domainUser.IsOwner == true)
                this.Users.Owner = domainUser;
            this.OnUserAdded(new DomainUserEventArgs(authentication, this, domainUser));
            this.Container.InvokeDomainUserAddedEvent(authentication, this, domainUser);
        }

        public void InvokeUserChanged(Authentication authentication, DomainUserInfo domainUserInfo, DomainUserState domainUserState)
        {
            this.Dispatcher.VerifyAccess();

            var domainUser = this.Users[domainUserInfo.UserID];
            domainUser.SetDomainUserInfo(domainUserInfo);
            domainUser.SetDomainUserState(domainUserState);
            if (domainUser.IsOwner == true)
                this.Users.Owner = domainUser;
            this.OnUserChanged(new DomainUserEventArgs(authentication, this, domainUser));
            this.Container.InvokeDomainUserChangedEvent(authentication, this, domainUser);
        }

        public void InvokeUserRemoved(Authentication authentication, DomainUserInfo domainUserInfo, RemoveInfo removeInfo)
        {
            this.Dispatcher.VerifyAccess();

            var domainUser = this.Users[domainUserInfo.UserID];
            this.Users.Remove(domainUser.ID);
            if (domainUser.IsOwner == true)
                this.Users.Owner = null;
            this.OnUserRemoved(new DomainUserRemovedEventArgs(authentication, this, domainUser, removeInfo));
            this.Container.InvokeDomainUserRemovedEvent(authentication, this, domainUser, removeInfo);
        }

        public void InvokeRowAdded(Authentication authentication, DomainRowInfo[] rows)
        {
            this.Dispatcher.VerifyAccess();

            if (this.initialized == false)
                return;

            var domainUser = this.Users[authentication.ID];
            this.OnNewRow(domainUser, rows, authentication.SignatureDate);
            foreach (var item in rows)
            {
                this.modifiedTableList.Add(item.TableName);
            }
            domainUser.IsModified = true;
            this.IsModified = true;
            this.OnRowAdded(new DomainRowEventArgs(authentication, this, rows));
            this.Container.InvokeDomainRowAddedEvent(authentication, this, rows);
        }

        public void InvokeRowChanged(Authentication authentication, DomainRowInfo[] rows)
        {
            this.Dispatcher.VerifyAccess();

            if (this.initialized == false)
                return;

            var domainUser = this.Users[authentication.ID];
            this.OnSetRow(domainUser, rows, authentication.SignatureDate);
            foreach (var item in rows)
            {
                this.modifiedTableList.Add(item.TableName);
            }
            domainUser.IsModified = true;
            this.IsModified = true;
            this.OnRowChanged(new DomainRowEventArgs(authentication, this, rows));
            this.Container.InvokeDomainRowChangedEvent(authentication, this, rows);
        }

        public void InvokeRowRemoved(Authentication authentication, DomainRowInfo[] rows)
        {
            this.Dispatcher.VerifyAccess();

            if (this.initialized == false)
                return;

            var domainUser = this.Users[authentication.ID];
            this.OnRemoveRow(domainUser, rows, authentication.SignatureDate);
            foreach (var item in rows)
            {
                this.modifiedTableList.Add(item.TableName);
            }
            domainUser.IsModified = true;
            this.IsModified = true;
            this.OnRowRemoved(new DomainRowEventArgs(authentication, this, rows));
            this.Container.InvokeDomainRowRemovedEvent(authentication, this, rows);
        }

        public void InvokePropertyChanged(Authentication authentication, string propertyName, object value)
        {
            this.Dispatcher.VerifyAccess();

            if (this.initialized == false)
                return;

            var domainUser = this.Users[authentication.ID];
            this.OnSetProperty(domainUser, propertyName, value, authentication.SignatureDate);
            domainUser.IsModified = true;
            this.IsModified = true;
            this.OnPropertyChanged(new DomainPropertyEventArgs(authentication, this, propertyName, value));
            this.Container.InvokeDomainPropertyChangedEvent(authentication, this, propertyName, value);
        }

        public abstract object Source { get; }

        public IDomainHost Host { get; set; }

        public CremaHost CremaHost => this.Context.CremaHost;

        public Guid ID => Guid.Parse(this.Name);

        public Guid DataBaseID => base.DomainInfo.DataBaseID;

        public string DomainName => base.DomainInfo.ItemType;

        public DomainUserCollection Users { get; }

        public CremaDispatcher Dispatcher { get; private set; }

        public string[] ModifiedTables => this.modifiedTableList.OrderBy(item => item).ToArray();

        public event EventHandler<DomainUserEventArgs> UserAdded
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.userAdded += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.userAdded -= value;
            }
        }

        public event EventHandler<DomainUserEventArgs> UserChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.userChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.userChanged -= value;
            }
        }

        public event EventHandler<DomainUserRemovedEventArgs> UserRemoved
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.userRemoved += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.userRemoved -= value;
            }
        }

        public event EventHandler<DomainRowEventArgs> RowAdded
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.rowAdded += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.rowAdded -= value;
            }
        }

        public event EventHandler<DomainRowEventArgs> RowRemoved
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.rowRemoved += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.rowRemoved -= value;
            }
        }

        public event EventHandler<DomainRowEventArgs> RowChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.rowChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.rowChanged -= value;
            }
        }

        public event EventHandler<DomainPropertyEventArgs> PropertyChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.propertyChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.propertyChanged -= value;
            }
        }

        public new event EventHandler<DomainDeletedEventArgs> Deleted
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.deleted += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.deleted -= value;
            }
        }

        public new event EventHandler DomainInfoChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.DomainInfoChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.DomainInfoChanged -= value;
            }
        }

        public new event EventHandler DomainStateChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.DomainStateChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.DomainStateChanged -= value;
            }
        }

        protected virtual void OnInitialize(DomainMetaData metaData)
        {

        }

        protected virtual void OnRelease()
        {

        }

        protected virtual void OnNewRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDate signatureDate)
        {

        }

        protected virtual void OnSetRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDate signatureDate)
        {

        }

        protected virtual void OnRemoveRow(DomainUser domainUser, DomainRowInfo[] rows, SignatureDate signatureDate)
        {

        }

        protected virtual void OnSetProperty(DomainUser domainUser, string propertyName, object value, SignatureDate signatureDate)
        {

        }

        protected virtual void OnSetLocation(DomainUser domainUser, DomainLocationInfo location)
        {

        }

        protected virtual void OnBeginUserEdit(DomainUser domainUser, DomainLocationInfo location)
        {

        }

        protected virtual void OnEndUserEdit(DomainUser domainUser)
        {

        }

        protected abstract byte[] SerializeSource();

        protected abstract void DerializeSource(byte[] data);

        protected virtual void OnUserAdded(DomainUserEventArgs e)
        {
            this.userAdded?.Invoke(this, e);
        }

        protected virtual void OnUserChanged(DomainUserEventArgs e)
        {
            this.userChanged?.Invoke(this, e);
        }

        protected virtual void OnUserRemoved(DomainUserRemovedEventArgs e)
        {
            this.userRemoved?.Invoke(this, e);
        }

        protected virtual void OnRowAdded(DomainRowEventArgs e)
        {
            this.rowAdded?.Invoke(this, e);
        }

        protected virtual void OnRowChanged(DomainRowEventArgs e)
        {
            this.rowChanged?.Invoke(this, e);
        }

        protected virtual void OnRowRemoved(DomainRowEventArgs e)
        {
            this.rowRemoved?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(DomainPropertyEventArgs e)
        {
            this.propertyChanged?.Invoke(this, e);
        }

        protected virtual void OnDeleted(DomainDeletedEventArgs e)
        {
            this.deleted?.Invoke(this, e);
        }

        private void InvokeBeginUserEdit(Authentication authentication, DomainLocationInfo location, out DomainUser domainUser)
        {
            domainUser = this.GetDomainUser(authentication);
            this.OnBeginUserEdit(domainUser, location);
            domainUser.Location = location;
            domainUser.IsBeingEdited = true;
        }

        private void InvokeEndUserEdit(Authentication authentication, out DomainUser domainUser)
        {
            domainUser = this.GetDomainUser(authentication);
            this.OnEndUserEdit(domainUser);
            domainUser.IsBeingEdited = false;
        }

        private void InvokeNewRow(Authentication authentication, DomainRowInfo[] rows)
        {
            var domainUser = this.GetDomainUser(authentication);
            this.OnNewRow(domainUser, rows, authentication.SignatureDate);
            foreach (var item in rows)
            {
                this.modifiedTableList.Add(item.TableName);
            }
            domainUser.IsModified = true;
        }

        private void InvokeSetRow(Authentication authentication, DomainRowInfo[] rows)
        {
            var domainUser = this.GetDomainUser(authentication);
            this.OnSetRow(domainUser, rows, authentication.SignatureDate);
            foreach (var item in rows)
            {
                this.modifiedTableList.Add(item.TableName);
            }
            domainUser.IsModified = true;
        }

        private void InvokeRemoveRow(Authentication authentication, DomainRowInfo[] rows)
        {
            var domainUser = this.GetDomainUser(authentication);
            this.OnRemoveRow(domainUser, rows, authentication.SignatureDate);
            foreach (var item in rows)
            {
                this.modifiedTableList.Add(item.TableName);
            }
            domainUser.IsModified = true;
        }

        private void InvokeSetProperty(Authentication authentication, string propertyName, object value)
        {
            var domainUser = this.Users[authentication.ID];
            this.OnSetProperty(domainUser, propertyName, value, authentication.SignatureDate);
            domainUser.IsModified = true;
        }

        private void InvokeSetUserLocation(Authentication authentication, DomainLocationInfo location, out DomainUser domainUser)
        {
            domainUser = this.Users[authentication.ID];
            this.OnSetLocation(domainUser, location);
            domainUser.Location = location;
        }

        private void InvokeKick(Authentication authentication, string userID, string comment, out DomainUser domainUser, out RemoveInfo removeInfo)
        {
            removeInfo = new RemoveInfo(RemoveReason.Kick, comment);
            domainUser = this.Users[userID];
        }

        private void InvokeSetOwner(Authentication authentication, string userID, out DomainUser oldOwner, out DomainUser newOwner)
        {
            oldOwner = this.Users.Owner;
            newOwner = this.Users[userID];
        }

        private DomainUser GetDomainUser(Authentication authentication)
        {
            if (this.Dispatcher == null)
                throw new InvalidOperationException(Resources.Exception_InvalidObject);

            if (this.Users.Contains(authentication.ID) == false)
                throw new UserNotFoundException(authentication.ID);

            return this.Users[authentication.ID];
        }

        private void Sign(Authentication authentication, ResultBase result)
        {
            result.Validate(authentication);
        }

        private void Sign<T>(Authentication authentication, ResultBase<T> result)
        {
            result.Validate(authentication);
        }

        private IDomainService Service
        {
            get { return this.Context.Service; }
        }

        #region IDomain

        IDomainUserCollection IDomain.Users
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Users;
            }
        }

        DomainInfo IDomain.DomainInfo
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.DomainInfo;
            }
        }

        object IDomain.Host
        {
            get { return this.Host; }
        }

        #endregion

        #region IDomainItem

        IDomainItem IDomainItem.Parent
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Category;
            }
        }

        IEnumerable<IDomainItem> IDomainItem.Childs
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return Enumerable.Empty<IDomainItem>();
            }
        }

        #endregion

        #region IServiceProvider

        object IServiceProvider.GetService(System.Type serviceType)
        {
            if (serviceType == typeof(IDataBase) && this.Category != null)
            {
                return this.Category.DataBase;
            }
            return (this.Context as IServiceProvider).GetService(serviceType);
        }

        #endregion

        #region InternalSignatureDateProvider

        protected class InternalSignatureDateProvider : SignatureDateProvider
        {
            private readonly DateTime dateTime;

            public InternalSignatureDateProvider(SignatureDate signatureDate)
                : base(signatureDate.ID)
            {
                this.dateTime = signatureDate.DateTime;
            }

            public DateTime DateTime
            {
                get { return this.dateTime; }
            }

            protected override DateTime GetTime()
            {
                return this.dateTime;
            }
        }

        #endregion

        #region IInfoProvider

        IDictionary<string, object> IInfoProvider.Info => base.DomainInfo.ToDictionary();

        #endregion

        #region IStateProvider

        object IStateProvider.State => this.DomainState;

        #endregion
    }
}
