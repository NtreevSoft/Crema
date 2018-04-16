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

using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework;
using Ntreev.Library.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Ntreev.Crema.Client.Framework
{
    public class DomainCategoryDescriptor : DescriptorBase, IDomainCategoryDescriptor, IDomainItemDescriptor
    {
        private readonly IDomainCategory category;
        private readonly object owner;
        private readonly ObservableCollection<DomainCategoryDescriptor> categories = new ObservableCollection<DomainCategoryDescriptor>();
        private readonly ReadOnlyObservableCollection<DomainCategoryDescriptor> categoriesReadonly;
        private readonly ObservableCollection<DomainDescriptor> domains = new ObservableCollection<DomainDescriptor>();
        private readonly ReadOnlyObservableCollection<DomainDescriptor> domainsReadonly;

        private string categoryName;
        private string categoryPath;
        private bool isActivated;

        public DomainCategoryDescriptor(Authentication authentication, IDomainCategory category, DescriptorTypes descriptorTypes, object owner)
            : base(authentication, category, descriptorTypes)
        {
            this.category = category;
            this.owner = owner ?? this;
            this.category.Dispatcher.VerifyAccess();
            this.categoryName = category.Name;
            this.categoryPath = category.Path;

            this.domainsReadonly = new ReadOnlyObservableCollection<DomainDescriptor>(this.domains);
            this.categoriesReadonly = new ReadOnlyObservableCollection<DomainCategoryDescriptor>(this.categories);

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
            {
                this.category.Renamed += Category_Renamed;
            }

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsRecursive) == true)
            {
                if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                {
                    this.category.Domains.CollectionChanged += Domains_CollectionChanged;
                    this.category.Categories.CollectionChanged += Categories_CollectionChanged;
                }

                foreach (var item in this.category.Categories)
                {
                    var descriptor = new DomainCategoryDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                    item.ExtendedProperties[this.owner] = descriptor;
                    this.categories.Add(descriptor);
                }

                foreach (var item in this.category.Domains)
                {
                    var descriptor = new DomainDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                    item.ExtendedProperties[this.owner] = descriptor;
                    this.domains.Add(descriptor);
                }
            }

            if (this.category.GetService(typeof(IDataBase)) is IDataBase dataBase)
            {
                this.isActivated = dataBase.IsLoaded;
                if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                {
                    dataBase.Loaded += DataBase_Loaded;
                    dataBase.Unloaded += DataBase_Unloaded;
                }
            }
        }

        [DescriptorProperty]
        public string Name => this.categoryName;

        [DescriptorProperty]
        public string Path => this.categoryPath;

        [DescriptorProperty]
        public bool IsActivated => this.isActivated;

        public ReadOnlyObservableCollection<DomainCategoryDescriptor> Categories => this.categoriesReadonly;

        public ReadOnlyObservableCollection<DomainDescriptor> Domains => this.domainsReadonly;

        protected async override void OnDisposed(EventArgs e)
        {
            if (this.referenceTarget == null && this.category != null)
            {
                await this.category.Dispatcher.InvokeAsync(() =>
                {
                    if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                    {
                        this.category.Renamed -= Category_Renamed;
                        
                    }
                    if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                    {
                        if (this.descriptorTypes.HasFlag(DescriptorTypes.IsRecursive) == true)
                        {
                            this.category.Domains.CollectionChanged -= Domains_CollectionChanged;
                            this.category.Categories.CollectionChanged -= Categories_CollectionChanged;
                        }
                    }
                    if (this.category.GetService(typeof(IDataBase)) is IDataBase dataBase)
                    {
                        if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                        {
                            dataBase.Loaded -= DataBase_Loaded;
                            dataBase.Unloaded -= DataBase_Unloaded;
                        }
                    }
                });
            }
            base.OnDisposed(e);
        }

        private async void Category_Renamed(object sender, EventArgs e)
        {
            this.categoryName = this.category.Name;
            this.categoryPath = this.category.Path;
            await this.RefreshAsync();
        }

        private void Domains_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var descriptorList = new List<DomainDescriptor>(e.NewItems.Count);
                        foreach (IDomain item in e.NewItems)
                        {
                            if (item.ExtendedProperties.ContainsKey(this.owner) == true)
                            {
                                var descriptor = item.ExtendedProperties[this.owner] as DomainDescriptor;
                                descriptorList.Add(descriptor);
                            }
                            else
                            {
                                var descriptor = new DomainDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                                item.ExtendedProperties[this.owner] = descriptor;
                                descriptorList.Add(descriptor);
                            }

                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.domains.Add(item);
                            }
                        });
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var descriptorList = new List<DomainDescriptor>(e.OldItems.Count);
                        foreach (IDomain item in e.OldItems)
                        {
                            var descriptor = item.ExtendedProperties[this.owner] as DomainDescriptor;
                            descriptorList.Add(descriptor);
                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.domains.Remove(item);
                            }
                        });
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    {

                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            this.categories.Clear();
                            this.domains.Clear();
                        });
                    }
                    break;
            }
        }

        private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var descriptorList = new List<DomainCategoryDescriptor>(e.NewItems.Count);
                        foreach (IDomainCategory item in e.NewItems)
                        {
                            if (item.ExtendedProperties.ContainsKey(this.owner) == true)
                            {
                                var descriptor = item.ExtendedProperties[this.owner] as DomainCategoryDescriptor;
                                descriptorList.Add(descriptor);
                            }
                            else
                            {
                                var descriptor = new DomainCategoryDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                                item.ExtendedProperties[this.owner] = descriptor;
                                descriptorList.Add(descriptor);
                            }
                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.categories.Add(item);
                            }
                        });
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var descriptorList = new List<DomainCategoryDescriptor>(e.OldItems.Count);
                        foreach (IDomainCategory item in e.OldItems)
                        {
                            var descriptor = item.ExtendedProperties[this.owner] as DomainCategoryDescriptor;
                            descriptorList.Add(descriptor);
                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.categories.Remove(item);
                            }
                        });
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    {

                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            this.categories.Clear();
                        });
                    }
                    break;
            }
        }

        private async void DataBase_Loaded(object sender, EventArgs e)
        {
            this.isActivated = true;
            await this.RefreshAsync();
        }

        private async void DataBase_Unloaded(object sender, EventArgs e)
        {
            this.isActivated = false;
            await this.RefreshAsync();
        }

        #region IDomainCategoryDescriptor

        IDomainCategory IDomainCategoryDescriptor.Target => this.category;

        #endregion

        #region IDomainItemDescriptor

        IDomainItem IDomainItemDescriptor.Target => this.category as IDomainItem;

        #endregion
    }
}
