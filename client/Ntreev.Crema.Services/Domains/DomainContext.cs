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
using Ntreev.Crema.Services.Data;
using Ntreev.Crema.Services.DomainService;
using Ntreev.Crema.Services.Users;
using Ntreev.Library;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Timers;

namespace Ntreev.Crema.Services.Domains
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    class DomainContext : ItemContext<Domain, DomainCategory, DomainCollection, DomainCategoryCollection, DomainContext>,
        IDomainServiceCallback, IDomainContext, IServiceProvider, ICremaService
    {
        private readonly UserContext userContext;
        private DomainServiceClient service;
        private CremaDispatcher serviceDispatcher;
        private Timer timer;

        private ItemsCreatedEventHandler<IDomainItem> itemsCreated;
        private ItemsRenamedEventHandler<IDomainItem> itemsRenamed;
        private ItemsMovedEventHandler<IDomainItem> itemsMoved;
        private ItemsDeletedEventHandler<IDomainItem> itemsDeleted;

        public DomainContext(CremaHost cremaHost, string address, ServiceInfo serviceInfo)
        {
            this.CremaHost = cremaHost;
            this.userContext = cremaHost.UserContext;

            this.serviceDispatcher = new CremaDispatcher(this);
            var metaData = this.serviceDispatcher.Invoke(() =>
            {
                var binding = CremaHost.CreateBinding(serviceInfo);

                var endPointAddress = new EndpointAddress($"net.tcp://{address}:{serviceInfo.Port}/DomainService");
                var instanceContext = new InstanceContext(this);
                if (Environment.OSVersion.Platform != PlatformID.Unix)
                    instanceContext.SynchronizationContext = System.Threading.SynchronizationContext.Current;

                this.service = new DomainServiceClient(instanceContext, binding, endPointAddress);
                this.service.Open();
                if (this.service is ICommunicationObject service)
                {
                    service.Faulted += Service_Faulted;
                }
                var result = this.service.Subscribe(this.CremaHost.AuthenticationToken);
                result.Validate();
#if !DEBUG
                this.timer = new Timer(30000);
                this.timer.Elapsed += Timer_Elapsed;
                this.timer.Start();
#endif
                return result.Value;
            });

            this.Initialize(metaData);
            this.CremaHost.DataBases.ItemsCreated += DataBases_ItemsCreated;
            this.CremaHost.DataBases.ItemsRenamed += DataBases_ItemsRenamed;
            this.CremaHost.DataBases.ItemsDeleted += DataBases_ItemDeleted;
            this.CremaHost.AddService(this);
        }

        public DomainMetaData[] Restore(DataBase dataBase)
        {
            var result = this.service.GetMetaData();
            result.Validate();
            var metaData = result.Value;

            var metaDataList = new List<DomainMetaData>();
            foreach (var item in metaData.Domains)
            {
                if (item.DomainInfo.DataBaseID == dataBase.ID)
                {
                    metaDataList.Add(item);
                }
            }
            return metaDataList.ToArray();
        }

        public void InvokeItemsCreatedEvent(Authentication authentication, IDomainItem[] items, object[] args)
        {
            this.OnItemsCreated(new ItemsCreatedEventArgs<IDomainItem>(authentication, items, args));
        }

        public void InvokeItemsRenamedEvent(Authentication authentication, IDomainItem[] items, string[] oldNames, string[] oldPaths)
        {
            this.OnItemsRenamed(new ItemsRenamedEventArgs<IDomainItem>(authentication, items, oldNames, oldPaths));
        }

        public void InvokeItemsMovedEvent(Authentication authentication, IDomainItem[] items, string[] oldPaths, string[] oldParentPaths)
        {
            this.OnItemsMoved(new ItemsMovedEventArgs<IDomainItem>(authentication, items, oldPaths, oldParentPaths));
        }

        public void InvokeItemsDeleteEvent(Authentication authentication, IDomainItem[] items, string[] itemPaths)
        {
            this.OnItemsDeleted(new ItemsDeletedEventArgs<IDomainItem>(authentication, items, itemPaths));
        }

        public Domain Create(Authentication authentication, DomainMetaData metaData)
        {
            return this.Domains.Create(authentication, metaData);
        }

        public void Delete(Authentication authentication, Guid dataBaseID, string itemPath, string itemType, bool isCanceled)
        {
            this.Domains.Delete(authentication, dataBaseID, itemPath, itemType, isCanceled);
        }

        public Domain AddDomain(Authentication authentication, DomainInfo domainInfo)
        {
            return this.Domains.AddDomain(authentication, domainInfo);
        }

        public void AddDomains(DomainMetaData[] metaDatas)
        {
            System.Diagnostics.Trace.WriteLine(metaDatas.Length);
            foreach (var item in metaDatas)
            {
                System.Diagnostics.Trace.WriteLine(item.DomainID);
                var domain = this.Domains.AddDomain(null, item.DomainInfo);
                if (domain == null)
                    continue;

                domain.Initialize(Authentication.System, item);
            }
        }

        public void Close(CloseInfo closeInfo)
        {
            this.serviceDispatcher?.Invoke(() =>
            {
                this.timer?.Dispose();
                this.timer = null;
                if (this.service != null)
                {
                    try
                    {
                        if (closeInfo.Reason != CloseReason.NoResponding)
                        {
                            this.service.Unsubscribe();
                            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                                this.service.Close();
                            else
                                this.service.Abort();
                        }
                        else
                        {
                            this.service.Abort();
                        }
                    }
                    catch
                    {
                        this.service.Abort();
                    }
                    this.service = null;
                }
                this.serviceDispatcher.Dispose();
                this.serviceDispatcher = null;
            });
        }

        public DomainContextMetaData GetMetaData(Authentication authentication)
        {
            this.Dispatcher.VerifyAccess();

            var domains = this.Domains.ToArray<Domain>();
            var metaDataList = new List<DomainMetaData>(domains.Length);
            foreach (var item in domains)
            {
                var metaData = item.Dispatcher.Invoke(() => item.GetMetaData(authentication));
                metaDataList.Add(metaData);
            }

            var categoryPathList = new List<string>(this.Categories.Count);
            foreach (var item in this.Categories)
            {
                categoryPathList.Add(item.Path);
            }

            return new DomainContextMetaData()
            {
                DomainCategories = categoryPathList.ToArray(),
                Domains = metaDataList.ToArray(),
            };
        }

        public CremaHost CremaHost { get; }

        public DomainCollection Domains => this.Items;

        public CremaDispatcher Dispatcher => this.CremaHost.Dispatcher;

        public IDomainService Service => this.service;

        public event ItemsCreatedEventHandler<IDomainItem> ItemsCreated
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsCreated += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsCreated -= value;
            }
        }

        public event ItemsRenamedEventHandler<IDomainItem> ItemsRenamed
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsRenamed += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsRenamed -= value;
            }
        }

        public event ItemsMovedEventHandler<IDomainItem> ItemsMoved
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsMoved += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsMoved -= value;
            }
        }

        public event ItemsDeletedEventHandler<IDomainItem> ItemsDeleted
        {
            add
            {
                this.Dispatcher.VerifyAccess();
                this.itemsDeleted += value;
            }
            remove
            {
                this.Dispatcher.VerifyAccess();
                this.itemsDeleted -= value;
            }
        }

        protected virtual void OnItemsCreated(ItemsCreatedEventArgs<IDomainItem> e)
        {
            this.itemsCreated?.Invoke(this, e);
        }

        protected virtual void OnItemsRenamed(ItemsRenamedEventArgs<IDomainItem> e)
        {
            this.itemsRenamed?.Invoke(this, e);
        }

        protected virtual void OnItemsMoved(ItemsMovedEventArgs<IDomainItem> e)
        {
            this.itemsMoved?.Invoke(this, e);
        }

        protected virtual void OnItemsDeleted(ItemsDeletedEventArgs<IDomainItem> e)
        {
            this.itemsDeleted?.Invoke(this, e);
        }

        private void Initialize(DomainContextMetaData metaData)
        {
            var dataBases = this.CremaHost.DataBases;
            foreach (var item in metaData.DomainCategories)
            {
                if (item != this.Root.Path)
                {
                    var category = this.Categories.AddNew(item);
                    if (category.Parent == this.Root)
                    {
                        category.DataBase = this.CremaHost.DataBases[category.Name];
                    }
                }
            }

            foreach (var item in metaData.Domains)
            {
                var domain = this.Domains.AddDomain(null, item.DomainInfo);
                if (domain == null)
                    continue;

                domain.Initialize(Authentication.System, item);
            }
        }

        private void DataBases_ItemsCreated(object sender, ItemsCreatedEventArgs<IDataBase> e)
        {
            var categoryList = new List<DomainCategory>(e.Items.Length);
            var categoryNameList = new List<string>(e.Items.Length);
            var categoryPathList = new List<string>(e.Items.Length);
            for (var i = 0; i < e.Items.Length; i++)
            {
                var dataBase = e.Items[i];
                var categoryName = CategoryName.Create(dataBase.Name);
                var category = this.Categories.AddNew(categoryName);
                category.DataBase = dataBase;
                categoryList.Add(category);
            }
            Authentication.System.Sign();
            this.Categories.InvokeCategoriesCreatedEvent(Authentication.System, categoryList.ToArray());
        }

        private void DataBases_ItemsRenamed(object sender, ItemsRenamedEventArgs<IDataBase> e)
        {
            var categoryList = new List<DomainCategory>(e.Items.Length);
            var categoryNameList = new List<string>(e.Items.Length);
            var categoryPathList = new List<string>(e.Items.Length);
            for (var i = 0; i < e.Items.Length; i++)
            {
                var oldName = e.OldNames[i];
                var newName = e.Items[i].Name;
                var category = this.Root.Categories[oldName];
                var categoryName = category.Name;
                var categoryPath = category.Path;
                category.Name = newName;
                categoryList.Add(category);
                categoryNameList.Add(categoryName);
                categoryPathList.Add(categoryPath);
            }
            Authentication.System.Sign();
            this.Categories.InvokeCategoriesRenamedEvent(Authentication.System, categoryList.ToArray(), categoryNameList.ToArray(), categoryPathList.ToArray());
        }

        private void DataBases_ItemDeleted(object sender, ItemsDeletedEventArgs<IDataBase> e)
        {
            var categoryList = new List<DomainCategory>(e.Items.Length);
            var categoryPathList = new List<string>(e.Items.Length);
            foreach (var item in e.Items)
            {
                this.DeleteDomains(item);
                var category = this.Root.Categories[item.Name];
                var categoryPath = category.Path;
                category.Dispose();
                categoryList.Add(category);
                categoryPathList.Add(categoryPath);
            }
            Authentication.System.Sign();
            this.Categories.InvokeCategoriesDeletedEvent(Authentication.System, categoryList.ToArray(), categoryPathList.ToArray());
        }

        private void DeleteDomains(IDataBase dataBase)
        {
            var domainList = new List<Domain>();
            var domainPathList = new List<string>();
            foreach (var item in this.Domains.ToArray<Domain>())
            {
                if (item.DataBaseID == dataBase.ID)
                {
                    var path = item.Path;
                    item.Dispose();
                    domainList.Add(item);
                    domainPathList.Add(path);
                }
            }

            //this.Domains.InvokeDomainsDeletedEvent(Authentication.System, domainList.ToArray(), domainPathList.ToArray());
        }

        private void Service_Faulted(object sender, EventArgs e)
        {
            this.serviceDispatcher.Invoke(() =>
            {
                try
                {
                    this.service.Abort();
                    this.service = null;
                }
                catch
                {

                }
                this.timer?.Dispose();
                this.timer = null;
                this.serviceDispatcher.Dispose();
                this.serviceDispatcher = null;
            });
            this.InvokeAsync(() =>
            {
                this.CremaHost.RemoveService(this);
            }, nameof(Service_Faulted));
        }

        private async void InvokeAsync(Action action, string callbackName)
        {
            var count = 0;
            _Invoke:
            try
            {

                await this.Dispatcher.InvokeAsync(action);
            }
            catch (NullReferenceException e)
            {
                await Task.Delay(1);
                if (count == 0)
                {
                    count++;
                    goto _Invoke;
                }
                this.CremaHost.Error(callbackName);
                this.CremaHost.Error(e);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(callbackName);
                this.CremaHost.Error(e);
            }
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.timer?.Stop();
            try
            {
                await this.serviceDispatcher.InvokeAsync(() => this.service.IsAlive());
                this.timer?.Start();
            }
            catch
            {

            }
        }

        #region IDomainServiceCallback

        void IDomainServiceCallback.OnDomainCreated(SignatureDate signatureDate, DomainInfo domainInfo, DomainState domainState)
        {
            this.InvokeAsync(() =>
            {
                var authentication = this.userContext.Authenticate(signatureDate);
                var domain = this.Domains.AddDomain(authentication, domainInfo);
                domain.InvokeDomainStateChanged(authentication, domainState);
            }, nameof(IDomainServiceCallback.OnDomainCreated));
        }

        void IDomainServiceCallback.OnDomainDeleted(SignatureDate signatureDate, Guid domainID, bool isCanceled)
        {
            this.InvokeAsync(() =>
            {
                var domain = this.Domains[domainID];
                var authentication = this.userContext.Authenticate(signatureDate);
                domain.Dispose(authentication, isCanceled);
            }, nameof(IDomainServiceCallback.OnDomainDeleted));
        }

        void IDomainServiceCallback.OnDomainInfoChanged(SignatureDate signatureDate, Guid domainID, DomainInfo domainInfo)
        {
            this.InvokeAsync(() =>
            {
                var domain = this.Domains[domainID];
                var authentication = this.userContext.Authenticate(signatureDate);
                domain.InvokeDomainInfoChanged(authentication, domainInfo);
            }, nameof(IDomainServiceCallback.OnDomainInfoChanged));
        }

        void IDomainServiceCallback.OnDomainStateChanged(SignatureDate signatureDate, Guid domainID, DomainState domainState)
        {
            this.InvokeAsync(() =>
            {
                var domain = this.Domains[domainID];
                if (domain == null)
                    return;
                var authentication = this.userContext.Authenticate(signatureDate);
                domain.InvokeDomainStateChanged(authentication, domainState);
            }, nameof(IDomainServiceCallback.OnDomainStateChanged));
        }

        void IDomainServiceCallback.OnUserAdded(SignatureDate signatureDate, Guid domainID, DomainUserInfo domainUserInfo, DomainUserState domainUserState)
        {
            this.InvokeAsync(() =>
            {
                var domain = this.Domains[domainID];
                var authentication = this.userContext.Authenticate(signatureDate);
                domain.InvokeUserAdded(authentication, domainUserInfo, domainUserState);
            }, nameof(IDomainServiceCallback.OnUserAdded));
        }

        void IDomainServiceCallback.OnUserRemoved(SignatureDate signatureDate, Guid domainID, DomainUserInfo domainUserInfo, RemoveInfo removeInfo)
        {
            this.InvokeAsync(() =>
            {
                var domain = this.Domains[domainID];
                var authentication = this.userContext.Authenticate(signatureDate);
                domain.InvokeUserRemoved(authentication, domainUserInfo, removeInfo);
            }, nameof(IDomainServiceCallback.OnUserRemoved));
        }

        void IDomainServiceCallback.OnUserChanged(SignatureDate signatureDate, Guid domainID, DomainUserInfo domainUserInfo, DomainUserState domainUserState)
        {
            this.InvokeAsync(() =>
            {
                var domain = this.Domains[domainID];
                var authentication = this.userContext.Authenticate(signatureDate);
                domain.InvokeUserChanged(authentication, domainUserInfo, domainUserState);
            }, nameof(IDomainServiceCallback.OnUserChanged));
        }

        void IDomainServiceCallback.OnRowAdded(SignatureDate signatureDate, Guid domainID, DomainRowInfo[] rows)
        {
            this.InvokeAsync(() =>
            {
                var domain = this.Domains[domainID];
                var authentication = this.userContext.Authenticate(signatureDate);
                domain.InvokeRowAdded(authentication, rows);
            }, nameof(IDomainServiceCallback.OnRowAdded));
        }

        void IDomainServiceCallback.OnRowChanged(SignatureDate signatureDate, Guid domainID, DomainRowInfo[] rows)
        {
            this.InvokeAsync(() =>
            {
                var domain = this.Domains[domainID];
                var authentication = this.userContext.Authenticate(signatureDate);
                domain.InvokeRowChanged(authentication, rows);
            }, nameof(IDomainServiceCallback.OnRowChanged));
        }

        void IDomainServiceCallback.OnRowRemoved(SignatureDate signatureDate, Guid domainID, DomainRowInfo[] rows)
        {
            this.InvokeAsync(() =>
            {
                var domain = this.Domains[domainID];
                var authentication = this.userContext.Authenticate(signatureDate);
                domain.InvokeRowRemoved(authentication, rows);
            }, nameof(IDomainServiceCallback.OnRowRemoved));
        }

        void IDomainServiceCallback.OnPropertyChanged(SignatureDate signatureDate, Guid domainID, string propertyName, object value)
        {
            this.InvokeAsync(() =>
            {
                var domain = this.Domains[domainID];
                var authentication = this.userContext.Authenticate(signatureDate);
                domain.InvokePropertyChanged(authentication, propertyName, value);
            }, nameof(IDomainServiceCallback.OnPropertyChanged));
        }

        void IDomainServiceCallback.OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo)
        {
            this.service.Abort();
            this.service = null;
            this.timer?.Dispose();
            this.timer = null;
            this.serviceDispatcher.Dispose();
            this.serviceDispatcher = null;
            this.InvokeAsync(() =>
            {
                this.CremaHost.RemoveService(this);
            }, nameof(IDomainServiceCallback.OnServiceClosed));
        }

        #endregion

        #region IDomainContext

        IDomainCollection IDomainContext.Domains
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Domains;
            }
        }

        IDomainCategoryCollection IDomainContext.Categories
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Categories;
            }
        }

        IDomainItem IDomainContext.this[string itemPath]
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this[itemPath] as IDomainItem;
            }
        }

        IDomainCategory IDomainContext.Root
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Root;
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator<IDomainItem> IEnumerable<IDomainItem>.GetEnumerator()
        {
            this.Dispatcher.VerifyAccess();
            foreach (var item in this)
            {
                yield return item as IDomainItem;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            this.Dispatcher.VerifyAccess();
            foreach (var item in this)
            {
                yield return item as IDomainItem;
            }
        }

        #endregion

        #region IServiceProvider 

        object IServiceProvider.GetService(System.Type serviceType)
        {
            return (this.CremaHost as ICremaHost).GetService(serviceType);
        }

        #endregion
    }
}
