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
using Ntreev.Library.ObjectModel;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

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

        public void InvokeDomainCreatedEvent(Authentication authentication, Domain domain)
        {
            var args = new DomainEventArgs(authentication, domain);
            var eventLog = EventLogBuilder.Build(authentication, this, nameof(InvokeDomainCreatedEvent), domain);
            var comment = EventMessageBuilder.BeginDomain(authentication, domain);
            var domainInfo = domain.DomainInfo;
            this.Dispatcher.InvokeAsync(() =>
            {
                this.CremaHost.Debug(eventLog);
                this.CremaHost.Info(comment);
                this.OnDomainCreated(args);
                this.Context.InvokeItemsCreatedEvent(authentication, new IDomainItem[] { domain }, new object[] { domainInfo });
            });
        }

        public void InvokeDomainDeletedEvent(Authentication authentication, Domain domain, bool isCanceled)
        {
            var args = new DomainDeletedEventArgs(authentication, domain, isCanceled);
            var eventLog = EventLogBuilder.Build(authentication, this, nameof(InvokeDomainDeletedEvent), domain, isCanceled);
            var comment = isCanceled == false ? EventMessageBuilder.EndDomain(authentication, domain) : EventMessageBuilder.CancelDomain(authentication, domain);
            this.Dispatcher.InvokeAsync(() =>
            {
                this.CremaHost.Debug(eventLog);
                this.CremaHost.Info(comment);
                this.OnDomainDeleted(args);
                this.Context.InvokeItemsDeleteEvent(authentication, new IDomainItem[] { domain }, new string[] { domain.Path });
            });
        }

        public void InvokeDomainRowAddedEvent(Authentication authentication, Domain domain, DomainRowInfo[] rows)
        {
            var args = new DomainRowEventArgs(authentication, domain, rows);
            this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDomainRowAdded(args);
            });
        }

        public void InvokeDomainRowChangedEvent(Authentication authentication, Domain domain, DomainRowInfo[] rows)
        {
            var args = new DomainRowEventArgs(authentication, domain, rows);
            this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDomainRowChanged(args);
            });
        }

        public void InvokeDomainRowRemovedEvent(Authentication authentication, Domain domain, DomainRowInfo[] rows)
        {
            var args = new DomainRowEventArgs(authentication, domain, rows);
            this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDomainRowChanged(args);
            });
        }

        public void InvokeDomainPropertyChangedEvent(Authentication authentication, Domain domain, string propertyName, object value)
        {
            var args = new DomainPropertyEventArgs(authentication, domain, propertyName, value);
            this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDomainPropertyChanged(args);
            });
        }

        public void InvokeDomainUserAddedEvent(Authentication authentication, Domain domain, DomainUser domainUser)
        {
            var args = new DomainUserEventArgs(authentication, domain, domainUser);
            var eventLog = EventLogBuilder.Build(authentication, this, nameof(InvokeDomainUserAddedEvent), domain, domainUser);
            var comment = EventMessageBuilder.EnterDomainUser(authentication, domain);
            this.Dispatcher.InvokeAsync(() =>
            {
                this.CremaHost.Debug(eventLog);
                this.CremaHost.Info(comment);
                this.OnDomainUserAdded(args);
            });
        }

        public void InvokeDomainUserRemovedEvent(Authentication authentication, Domain domain, DomainUser domainUser, RemoveInfo removeInfo)
        {
            var args = new DomainUserRemovedEventArgs(authentication, domain, domainUser, removeInfo);
            var eventLog = EventLogBuilder.Build(authentication, this, nameof(InvokeDomainUserRemovedEvent), domain, domainUser, removeInfo.Reason, removeInfo.Message);
            var comment = removeInfo.Reason == RemoveReason.Kick
                ? EventMessageBuilder.KickDomainUser(authentication, domain, domainUser)
                : EventMessageBuilder.LeaveDomainUser(authentication, domain);
            this.Dispatcher.InvokeAsync(() =>
            {
                this.CremaHost.Debug(eventLog);
                this.CremaHost.Info(comment);
                this.OnDomainUserRemoved(args);
            });
        }

        public void InvokeDomainUserChangedEvent(Authentication authentication, Domain domain, DomainUser domainUser)
        {
            var args = new DomainUserEventArgs(authentication, domain, domainUser);
            this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDomainUserChanged(args);
            });
        }

        public void InvokeDomainInfoChangedEvent(Authentication authentication, Domain domain)
        {
            var args = new DomainEventArgs(authentication, domain);
            this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDomainInfoChanged(args);
            });
        }

        public void InvokeDomainStateChangedEvent(Authentication authentication, Domain domain)
        {
            var args = new DomainEventArgs(authentication, domain);
            this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDomainStateChanged(args);
            });
        }

        public void Restore(Authentication authentication, Domain domain)
        {
            try
            {
                authentication.Sign();
                var dataBase = this.Context.CremaHost.DataBases[domain.DataBaseID];
                var categoryName = CategoryName.Create(dataBase.Name, domain.DomainInfo.ItemType);
                var category = this.Context.Categories.Prepare(categoryName);
                domain.Category = category;
                domain.Dispatcher = new CremaDispatcher(domain);
                domain.Dispatcher.InvokeAsync(() => this.InvokeDomainCreatedEvent(authentication, domain));
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Add(Authentication authentication, Domain domain)
        {
            try
            {
                var dataBase = this.Context.CremaHost.DataBases[domain.DataBaseID];
                var categoryName = CategoryName.Create(dataBase.Name, domain.DomainInfo.ItemType);
                var category = this.Context.Categories.Prepare(categoryName);
                domain.Category = category;
                domain.Dispatcher = new CremaDispatcher(domain);
                domain.Logger = new DomainLogger(domain);
                domain.Dispatcher.InvokeAsync(() => this.InvokeDomainCreatedEvent(authentication, domain));
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public bool Contains(Guid domainID)
        {
            this.Dispatcher.VerifyAccess();
            return this.Contains(domainID.ToString());
        }

        public DomainMetaData[] GetMetaData(Authentication authentication)
        {
            try
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
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
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
