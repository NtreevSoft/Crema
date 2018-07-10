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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Services;
using Ntreev.Crema.Services.Properties;
using Ntreev.Crema.Services.Users;
using System.Windows.Threading;
using Ntreev.Crema.Services.Data;
using Ntreev.Library.IO;

namespace Ntreev.Crema.Services.Domains
{
    class DomainContext : ItemContext<Domain, DomainCategory, DomainCollection, DomainCategoryCollection, DomainContext>,
        IDomainContext, IServiceProvider, IDisposable
    {
        private readonly UserContext userContext;
        private ItemsCreatedEventHandler<IDomainItem> itemsCreated;
        private ItemsRenamedEventHandler<IDomainItem> itemsRenamed;
        private ItemsMovedEventHandler<IDomainItem> itemsMoved;
        private ItemsDeletedEventHandler<IDomainItem> itemsDeleted;

        public DomainContext(CremaHost cremaHost, UserContext userContext)
        {
            this.CremaHost = cremaHost;
            this.CremaHost.Debug(Resources.Message_DomainContextInitialize);
            this.userContext = userContext;
            this.BasePath = cremaHost.GetPath(CremaPath.Domains);
            this.CremaHost.Opened += CremaHost_Opened;
            this.CremaHost.Debug(Resources.Message_DomainContextIsCreated);

            foreach (var item in this.CremaHost.DataBases)
            {
                var categoryName = CategoryName.Create(item.Name);
                var category = this.Categories.AddNew(categoryName);
                category.DataBase = item;
            }
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

        public object GetService(System.Type serviceType)
        {
            return this.CremaHost.GetService(serviceType);
        }

        public DomainContextMetaData GetMetaData(Authentication authentication)
        {
            this.Dispatcher.VerifyAccess();

            return new DomainContextMetaData()
            {
                DomainCategories = this.Categories.GetMetaData(authentication),
                Domains = this.Domains.GetMetaData(authentication),
            };
        }

        public DomainCollection Domains
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Items;
            }
        }

        public CremaHost CremaHost { get; }

        public string BasePath { get; }

        public CremaDispatcher Dispatcher => this.CremaHost.Dispatcher;

        public IObjectSerializer Serializer => this.CremaHost.Serializer;

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

        public void Restore(Authentication authentication, DataBase dataBase)
        {
            var succeededCount = 0;
            var failedCount = 0;
            var path = Path.Combine(this.BasePath, dataBase.ID.ToString());
            if (Directory.Exists(path) == false)
                return;

            var domainList = new List<string>();
            foreach (var item in Directory.GetDirectories(path))
            {
                var dirInfo = new DirectoryInfo(item);
                if (Guid.TryParse(dirInfo.Name, out Guid domainID) == true)
                {
                    domainList.Add(item);
                }
            }

            if (domainList.Any() == true)
            {
                this.CremaHost.Info(Resources.Message_DomainRestorationMessage);
                foreach (var item in domainList)
                {
                    var domainID = Guid.Parse(Path.GetFileName(item));
                    try
                    {
                        DomainRestorer.Restore(authentication, this, item);
                        this.CremaHost.Debug(Resources.Message_DomainIsRestored_Format, domainID);
                        succeededCount++;
                    }
                    catch (Exception e)
                    {
                        this.CremaHost.Error(e.Message);
                        this.CremaHost.Error(Resources.Message_DomainRestorationIsFailed_Format, domainID);
                        failedCount++;
                    }
                }

                this.CremaHost.Debug(string.Format(Resources.Message_RestoreResult_Format, succeededCount, failedCount));
            }
            else
            {
                this.CremaHost.Debug(Resources.Message_NotFoundDomainsToRestore);
            }
        }

        public void Dispose()
        {
            foreach (var item in this.Domains.ToArray<Domain>())
            {
                item.Dispatcher.Invoke(() => item.Dispose(this));
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

        private void CremaHost_Opened(object sender, EventArgs e)
        {
            this.CremaHost.DataBases.ItemsCreated += DataBases_ItemsCreated;
            this.CremaHost.DataBases.ItemsRenamed += DataBases_ItemsRenamed;
            this.CremaHost.DataBases.ItemsDeleted += DataBases_ItemDeleted;
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
                categoryList.Add(category);
                category.DataBase = dataBase;
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
                var localPath = Path.Combine(this.BasePath, $"{item.ID}");
                DirectoryUtility.Delete(localPath);
                category.Dispose();
                categoryList.Add(category);
                categoryPathList.Add(categoryPath);
            }
            Authentication.System.Sign();
            this.Categories.InvokeCategoriesDeletedEvent(Authentication.System, categoryList.ToArray(), categoryPathList.ToArray());
        }

        private void DeleteDomains(IDataBase dataBase)
        {
            foreach (var item in this.Domains.ToArray<Domain>())
            {
                if (item.DataBaseID == dataBase.ID)
                {
                    item.Dispatcher.Invoke(() => item.Dispose(this));
                }
            }

            DirectoryUtility.Delete(this.BasePath, dataBase.ID.ToString());
        }

        #region IDomainContext

        bool IDomainContext.Contains(string itemPath)
        {
            this.Dispatcher.VerifyAccess();
            return this.Contains(itemPath);
        }

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

        #endregion

        #region IServiceProvider 

        object IServiceProvider.GetService(System.Type serviceType)
        {
            return (this.CremaHost as ICremaHost).GetService(serviceType);
        }

        #endregion
    }
}
