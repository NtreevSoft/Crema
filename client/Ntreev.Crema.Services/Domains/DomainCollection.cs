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
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Ntreev.Crema.Services.Domains
{
    class DomainCollection : ItemContainer<Domain, DomainCategory, DomainCollection, DomainCategoryCollection, DomainContext>, 
        IDomainCollection
    {
        private EventHandler<DomainEventArgs> domainCreated;
        private EventHandler<DomainDeletedEventArgs> domainDeleted;
        private EventHandler<DomainEventArgs> domainInfoChanged;
        private EventHandler<DomainEventArgs> domainStateChanged;
        private EventHandler<DomainUserEventArgs> domainUserAdded;
        private EventHandler<DomainUserEventArgs> domainUserChanged;
        private EventHandler<DomainUserRemovedEventArgs> domainUserRemoved;
        private EventHandler<DomainRowEventArgs> domainRowAdded;
        private EventHandler<DomainRowEventArgs> domainRowChanged;
        private EventHandler<DomainRowEventArgs> domainRowRemoved;
        private EventHandler<DomainPropertyEventArgs> domainPropertyChanged;

        public async void InvokeDomainCreatedEvent(Authentication authentication, Domain domain)
        {
            var args = new DomainEventArgs(authentication, domain);
            var eventLog = EventLogBuilder.Build(authentication, this, nameof(InvokeDomainCreatedEvent), domain);
            var comment = EventMessageBuilder.BeginDomain(authentication, domain);
            var domainInfo = domain.DomainInfo;
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.CremaHost.Debug(comment);
                this.CremaHost.Info(comment);
                this.OnDomainCreated(args);
                this.Context.InvokeItemsCreatedEvent(authentication, new IDomainItem[] { domain }, new object[] { domainInfo });
            });
        }

        public async void InvokeDomainDeletedEvent(Authentication authentication, Domain domain, bool isCanceled)
        {
            var args = new DomainDeletedEventArgs(authentication, domain, isCanceled);
            var eventLog = EventLogBuilder.Build(authentication, this, nameof(InvokeDomainDeletedEvent), domain, isCanceled);
            var comment = isCanceled == false ? EventMessageBuilder.EndDomain(authentication, domain) : EventMessageBuilder.CancelDomain(authentication, domain);
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.CremaHost.Debug(comment);
                this.CremaHost.Info(comment);
                this.OnDomainDeleted(args);
                this.Context.InvokeItemsDeleteEvent(authentication, new IDomainItem[] { domain }, new string[] { domain.Path });
            });
        }

        public async void InvokeDomainRowAddedEvent(Authentication authentication, Domain domain, DomainRowInfo[] rows)
        {
            var args = new DomainRowEventArgs(authentication, domain, rows);
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDomainRowAdded(args);
            });
        }

        public async void InvokeDomainRowChangedEvent(Authentication authentication, Domain domain, DomainRowInfo[] rows)
        {
            var args = new DomainRowEventArgs(authentication, domain, rows);
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDomainRowChanged(args);
            });
        }

        public async void InvokeDomainRowRemovedEvent(Authentication authentication, Domain domain, DomainRowInfo[] rows)
        {
            var args = new DomainRowEventArgs(authentication, domain, rows);
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDomainRowChanged(args);
            });
        }

        public async void InvokeDomainPropertyChangedEvent(Authentication authentication, Domain domain, string propertyName, object value)
        {
            var args = new DomainPropertyEventArgs(authentication, domain, propertyName, value);
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDomainPropertyChanged(args);
            });
        }

        public async void InvokeDomainUserAddedEvent(Authentication authentication, Domain domain, DomainUser domainUser)
        {
            var args = new DomainUserEventArgs(authentication, domain, domainUser);
            var eventLog = EventLogBuilder.Build(authentication, this, nameof(InvokeDomainUserAddedEvent), domain, domainUser);
            var comment = EventMessageBuilder.EnterDomainUser(authentication, domain);
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.CremaHost.Debug(comment);
                this.CremaHost.Info(comment);
                this.OnDomainUserAdded(args);
            });
        }

        public async void InvokeDomainUserRemovedEvent(Authentication authentication, Domain domain, DomainUser domainUser, RemoveInfo removeInfo)
        {
            var args = new DomainUserRemovedEventArgs(authentication, domain, domainUser, removeInfo);
            var eventLog = EventLogBuilder.Build(authentication, this, nameof(InvokeDomainUserRemovedEvent), domain, domainUser, removeInfo.Reason, removeInfo.Message);
            var comment = removeInfo.Reason == RemoveReason.Kick
                ? EventMessageBuilder.KickDomainUser(authentication, domain, domainUser)
                : EventMessageBuilder.LeaveDomainUser(authentication, domain);
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.CremaHost.Debug(comment);
                this.CremaHost.Info(comment);
                this.OnDomainUserRemoved(args);
            });
        }

        public async void InvokeDomainUserChangedEvent(Authentication authentication, Domain domain, DomainUser domainUser)
        {
            var args = new DomainUserEventArgs(authentication, domain, domainUser);
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDomainUserChanged(args);
            });
        }

        public async void InvokeDomainInfoChangedEvent(Authentication authentication, Domain domain)
        {
            var args = new DomainEventArgs(authentication, domain);
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDomainInfoChanged(args);
            });
        }

        public async void InvokeDomainStateChangedEvent(Authentication authentication, Domain domain)
        {
            var args = new DomainEventArgs(authentication, domain);
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDomainStateChanged(args);
            });
        }

        //public void InvokeDomainsCreatedEvent(Authentication authentication, Domain[] domains)
        //{
        //    var args = domains.Select(item => (object)item.DomainInfo).ToArray();
        //    this.OnDomainsCreated(new ItemsCreatedEventArgs<IDomain>(authentication, domains, args));
        //    this.Context.InvokeItemsCreatedEvent(authentication, domains, args);
        //}

        //public void InvokeDomainsRenamedEvent(Authentication authentication, Domain[] domains, string[] oldNames, string[] oldPaths)
        //{
        //    throw new NotImplementedException();
        //}

        //public void InvokeDomainsMovedEvent(Authentication authentication, Domain[] domains, string[] oldPaths, string[] oldCategoryPaths)
        //{
        //    this.OnDomainsMoved(new ItemsMovedEventArgs<IDomain>(authentication, domains, oldPaths, oldCategoryPaths));
        //    this.Context.InvokeItemsMovedEvent(authentication, domains, oldPaths, oldCategoryPaths);
        //}

        //public void InvokeDomainsDeletedEvent(Authentication authentication, Domain[] domains, string[] itemPaths)
        //{
        //    this.OnDomainsDeleted(new ItemsDeletedEventArgs<IDomain>(authentication, domains, itemPaths));
        //    this.Context.InvokeItemsDeleteEvent(authentication, domains, itemPaths);
        //}

        public Domain Create(Authentication authentication, DomainMetaData metaData)
        {
            var domain = this[metaData.DomainID];

            if (domain == null)
            {
                if (metaData.DomainInfo.DomainType == typeof(TableContentDomain).Name)
                {
                    domain = new TableContentDomain(metaData.DomainInfo, this.Context.CremaHost.Dispatcher);
                }
                else if (metaData.DomainInfo.DomainType == typeof(TableTemplateDomain).Name)
                {
                    domain = new TableTemplateDomain(metaData.DomainInfo, this.Context.CremaHost.Dispatcher);
                }
                else if (metaData.DomainInfo.DomainType == typeof(TypeDomain).Name)
                {
                    domain = new TypeDomain(metaData.DomainInfo, this.Context.CremaHost.Dispatcher);
                }
                //this.AttachEventHandlers(domain);
                this.Add(domain);
                domain.Category = this.Context.Categories.Prepare(metaData.DomainInfo.CategoryPath);
                domain.Initialize(authentication, metaData);
                this.InvokeDomainCreatedEvent(authentication, domain);
            }
            else
            {
                domain.Initialize(authentication, metaData);
            }

            return domain;
        }

        public void Delete(Authentication authentication, Guid dataBaseID, string itemPath, string itemType, bool isCanceled)
        {
            foreach (var item in this.ToArray<Domain>())
            {
                if (item.DomainInfo.DataBaseID == dataBaseID && item.DomainInfo.ItemPath == itemPath && item.DomainInfo.ItemType == itemType)
                {
                    item.Dispose(authentication, isCanceled);
                    return;
                }
            }
        }

        public Domain AddDomain(Authentication authentication, DomainInfo domainInfo)
        {
            var domainType = domainInfo.DomainType;
            var domain = null as Domain;
            if (domainType == typeof(TableContentDomain).Name)
            {
                domain = new TableContentDomain(domainInfo, this.CremaHost.Dispatcher);
            }
            else if (domainType == typeof(TypeDomain).Name)
            {
                domain = new TypeDomain(domainInfo, this.CremaHost.Dispatcher);
            }
            else if (domainType == typeof(TableTemplateDomain).Name)
            {
                domain = new TableTemplateDomain(domainInfo, this.CremaHost.Dispatcher);
            }
            if (domain != null)
            {
                domain.Category = this.Context.Categories.Prepare(domainInfo.CategoryPath);
                
                foreach (var item in this.CremaHost.DataBases)
                {
                    var isLoaded = item.Service != null;
                    if (domain.DataBaseID == item.ID && isLoaded == true)
                    {
                        var target = item.FindDomainHost(domain);
                        if (target != null)
                        {
                            target.Restore(domain);
                            domain.Host = target;
                        }
                    }
                }
                if (authentication != null)
                    this.InvokeDomainCreatedEvent(authentication, domain);
            }
            return domain;
        }

        public bool Contains(Guid domainID)
        {
            this.Dispatcher.VerifyAccess();
            return this.Contains(domainID.ToString());
        }

        public DomainMetaData[] GetMetaData(Authentication authentication)
        {
            this.Dispatcher.VerifyAccess();

            var domains = this.ToArray<Domain>();
            var metaDataList = new List<DomainMetaData>(domains.Length);
            foreach (var item in domains)
            {
                var metaData = item.Dispatcher.Invoke(() => item.GetMetaData(authentication));
                metaDataList.Add(metaData);
            }
            return metaDataList.ToArray();
        }

        public Domain this[Guid domainID]
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this[domainID.ToString()];
            }
        }

        public CremaHost CremaHost
        {
            get { return this.Context.CremaHost; }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.Context.Dispatcher; }
        }

        public new int Count
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return base.Count;
            }
        }

        //public event ItemsCreatedEventHandler<IDomain> DomainsCreated
        //{
        //    add
        //    {
        //        this.Dispatcher.VerifyAccess();
        //        this.domainsCreated += value;
        //    }
        //    remove
        //    {
        //        this.Dispatcher.VerifyAccess();
        //        this.domainsCreated -= value;
        //    }
        //}

        //public event ItemsRenamedEventHandler<IDomain> DomainsRenamed
        //{
        //    add
        //    {
        //        this.Dispatcher.VerifyAccess();
        //        this.domainsRenamed += value;
        //    }
        //    remove
        //    {
        //        this.Dispatcher.VerifyAccess();
        //        this.domainsRenamed -= value;
        //    }
        //}

        //public event ItemsMovedEventHandler<IDomain> DomainsMoved
        //{
        //    add
        //    {
        //        this.Dispatcher.VerifyAccess();
        //        this.domainsMoved += value;
        //    }
        //    remove
        //    {
        //        this.Dispatcher.VerifyAccess();
        //        this.domainsMoved -= value;
        //    }
        //}

        //public event ItemsDeletedEventHandler<IDomain> DomainsDeleted
        //{
        //    add
        //    {
        //        this.Dispatcher.VerifyAccess();
        //        this.domainsDeleted += value;
        //    }
        //    remove
        //    {
        //        this.Dispatcher.VerifyAccess();
        //        this.domainsDeleted -= value;
        //    }
        //}

        public event EventHandler<DomainEventArgs> DomainCreated
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.domainCreated += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.domainCreated -= value;
            }
        }

        public event EventHandler<DomainDeletedEventArgs> DomainDeleted
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.domainDeleted += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.domainDeleted -= value;
            }
        }

        public event EventHandler<DomainEventArgs> DomainInfoChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.domainInfoChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.domainInfoChanged -= value;
            }
        }

        public event EventHandler<DomainEventArgs> DomainStateChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.domainStateChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.domainStateChanged -= value;
            }
        }

        public event EventHandler<DomainUserEventArgs> DomainUserAdded
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.domainUserAdded += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.domainUserAdded -= value;
            }
        }

        public event EventHandler<DomainUserEventArgs> DomainUserChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.domainUserChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.domainUserChanged -= value;
            }
        }

        public event EventHandler<DomainUserRemovedEventArgs> DomainUserRemoved
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.domainUserRemoved += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.domainUserRemoved -= value;
            }
        }

        public event EventHandler<DomainRowEventArgs> DomainRowAdded
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.domainRowAdded += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.domainRowAdded -= value;
            }
        }

        public event EventHandler<DomainRowEventArgs> DomainRowChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.domainRowChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.domainRowChanged -= value;
            }
        }

        public event EventHandler<DomainRowEventArgs> DomainRowRemoved
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.domainRowRemoved += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.domainRowRemoved -= value;
            }
        }

        public event EventHandler<DomainPropertyEventArgs> DomainPropertyChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.domainPropertyChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.domainPropertyChanged -= value;
            }
        }

        public new event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                base.CollectionChanged += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                base.CollectionChanged -= value;
            }
        }

        //protected virtual void OnDomainsCreated(ItemsCreatedEventArgs<IDomain> e)
        //{
        //    this.domainsCreated?.Invoke(this, e);
        //}

        //protected virtual void OnDomainsRenamed(ItemsRenamedEventArgs<IDomain> e)
        //{
        //    this.domainsRenamed?.Invoke(this, e);
        //}

        //protected virtual void OnDomainsMoved(ItemsMovedEventArgs<IDomain> e)
        //{
        //    this.domainsMoved?.Invoke(this, e);
        //}

        //protected virtual void OnDomainsDeleted(ItemsDeletedEventArgs<IDomain> e)
        //{
        //    this.domainsDeleted?.Invoke(this, e);
        //}

        protected virtual void OnDomainCreated(DomainEventArgs e)
        {
            this.domainCreated?.Invoke(this, e);
        }

        protected virtual void OnDomainDeleted(DomainDeletedEventArgs e)
        {
            this.domainDeleted?.Invoke(this, e);
        }

        protected virtual void OnDomainInfoChanged(DomainEventArgs e)
        {
            this.domainInfoChanged?.Invoke(this, e);
        }

        protected virtual void OnDomainStateChanged(DomainEventArgs e)
        {
            this.domainStateChanged?.Invoke(this, e);
        }

        protected virtual void OnDomainUserAdded(DomainUserEventArgs e)
        {
            this.domainUserAdded?.Invoke(this, e);
        }

        protected virtual void OnDomainUserChanged(DomainUserEventArgs e)
        {
            this.domainUserChanged?.Invoke(this, e);
        }

        protected virtual void OnDomainUserRemoved(DomainUserRemovedEventArgs e)
        {
            this.domainUserRemoved?.Invoke(this, e);
        }

        protected virtual void OnDomainRowAdded(DomainRowEventArgs e)
        {
            this.domainRowAdded?.Invoke(this, e);
        }

        protected virtual void OnDomainRowChanged(DomainRowEventArgs e)
        {
            this.domainRowChanged?.Invoke(this, e);
        }

        protected virtual void OnDomainRowRemoved(DomainRowEventArgs e)
        {
            this.domainRowRemoved?.Invoke(this, e);
        }

        protected virtual void OnDomainPropertyChanged(DomainPropertyEventArgs e)
        {
            this.domainPropertyChanged?.Invoke(this, e);
        }

        //private void AttachEventHandlers(Domain domain)
        //{
        //    domain.UserAdded += Domain_UserAdded;
        //    domain.UserChanged += Domain_UserChanged;
        //    domain.UserRemoved += Domain_UserRemoved;
        //    domain.RowAdded += Domain_RowAdded;
        //    domain.RowRemoved += Domain_RowRemoved;
        //    domain.RowChanged += Domain_RowChanged;
        //    domain.PropertyChanged += Domain_PropertyChanged;
        //    domain.Deleted += Domain_Deleted;
        //    domain.DomainStateChanged += Domain_DomainStateChanged;
        //}

        //private void DetachEventHandlers(Domain domain)
        //{
        //    domain.UserAdded -= Domain_UserAdded;
        //    domain.UserChanged -= Domain_UserChanged;
        //    domain.UserRemoved -= Domain_UserRemoved;
        //    domain.RowAdded -= Domain_RowAdded;
        //    domain.RowRemoved -= Domain_RowRemoved;
        //    domain.RowChanged -= Domain_RowChanged;
        //    domain.PropertyChanged -= Domain_PropertyChanged;
        //    domain.Deleted -= Domain_Deleted;
        //    domain.DomainStateChanged -= Domain_DomainStateChanged;
        //}

        //private async void Domain_Deleted(object sender, DomainDeletedEventArgs e)
        //{
        //    this.DetachEventHandlers(sender as Domain);
        //    await this.Dispatcher.InvokeAsync(() => this.OnDomainDeleted(e));
        //}

        //private async void Domain_UserAdded(object sender, DomainUserEventArgs e)
        //{
        //    await this.Dispatcher.InvokeAsync(() => this.OnDomainUserAdded(e));
        //}

        //private async void Domain_UserChanged(object sender, DomainUserEventArgs e)
        //{
        //    await this.Dispatcher.InvokeAsync(() => this.OnDomainUserChanged(e));
        //}

        //private async void Domain_UserRemoved(object sender, DomainUserRemovedEventArgs e)
        //{
        //    await this.Dispatcher.InvokeAsync(() => this.OnDomainUserRemoved(e));
        //}

        //private async void Domain_RowAdded(object sender, DomainRowEventArgs e)
        //{
        //    await this.Dispatcher.InvokeAsync(() => this.OnDomainRowAdded(e));
        //}

        //private async void Domain_RowChanged(object sender, DomainRowEventArgs e)
        //{
        //    await this.Dispatcher.InvokeAsync(() => this.OnDomainRowChanged(e));
        //}

        //private async void Domain_RowRemoved(object sender, DomainRowEventArgs e)
        //{
        //    await this.Dispatcher.InvokeAsync(() => this.OnDomainRowRemoved(e));
        //}

        //private async void Domain_PropertyChanged(object sender, DomainPropertyEventArgs e)
        //{
        //    await this.Dispatcher.InvokeAsync(() => this.OnDomainPropertyChanged(e));
        //}

        //private async void Domain_DomainStateChanged(object sender, DomainEventArgs e)
        //{
        //    await this.Dispatcher.InvokeAsync(() => this.OnDomainStateChanged(e));
        //}

        #region IDomainCollection

        IDomain IDomainCollection.this[Guid domainID]
        {
            get { return this[domainID]; }
        }

        IEnumerator<IDomain> IEnumerable<IDomain>.GetEnumerator()
        {
            this.Dispatcher.VerifyAccess();
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.Dispatcher.VerifyAccess();
            return this.GetEnumerator();
        }

        #endregion

        #region IServiceProvider

        object IServiceProvider.GetService(System.Type serviceType)
        {
            return (this.Context as IServiceProvider).GetService(serviceType);
        }

        #endregion
    }
}
