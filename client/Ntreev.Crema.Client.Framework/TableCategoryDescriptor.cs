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
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Threading.Tasks;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Data;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.Library.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Collections.ObjectModel;

namespace Ntreev.Crema.Client.Framework
{
    public class TableCategoryDescriptor : DescriptorBase, ITableCategoryDescriptor, ITableItemDescriptor, ILockableDescriptor, IPermissionDescriptor, IAccessibleDescriptor
    {
        private ITableCategory category;
        private readonly object owner;
        private readonly ObservableCollection<TableCategoryDescriptor> categories = new ObservableCollection<TableCategoryDescriptor>();
        private readonly ReadOnlyObservableCollection<TableCategoryDescriptor> categoriesReadonly;
        private readonly ObservableCollection<TableDescriptor> tables = new ObservableCollection<TableDescriptor>();
        private readonly ReadOnlyObservableCollection<TableDescriptor> tablesReadonly;

        private string categoryName;
        private string categoryPath;
        private AccessInfo accessInfo = AccessInfo.Empty;
        private LockInfo lockInfo = LockInfo.Empty;
        private AccessType accessType;

        public TableCategoryDescriptor(Authentication authentication, ITableCategory category, DescriptorTypes descriptorTypes, object owner)
           : base(authentication, category, descriptorTypes)
        {
            this.category = category;
            this.owner = owner ?? this;
            this.category.Dispatcher.VerifyAccess();
            this.categoryName = category.Name;
            this.categoryPath = category.Path;
            this.accessInfo = category.AccessInfo;
            this.lockInfo = category.LockInfo;
            this.accessType = category.GetAccessType(authentication);

            this.tablesReadonly = new ReadOnlyObservableCollection<TableDescriptor>(this.tables);
            this.categoriesReadonly = new ReadOnlyObservableCollection<TableCategoryDescriptor>(this.categories);

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
            {
                this.category.Renamed += Category_Renamed;
                this.category.Moved += Category_Moved;
                this.category.Deleted += Category_Deleted;
                this.category.AccessChanged += Category_AccessChanged;
                this.category.LockChanged += Category_LockChanged;
            }

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsRecursive) == true)
            {
                if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                {
                    this.category.Tables.CollectionChanged += Tables_CollectionChanged;
                    this.category.Categories.CollectionChanged += Categories_CollectionChanged;
                }

                foreach (var item in this.category.Categories)
                {
                    var descriptor = new TableCategoryDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                    item.ExtendedProperties[this.owner] = descriptor;
                    this.categories.Add(descriptor);
                }

                foreach (var item in this.category.Tables)
                {
                    if (item.Parent != null)
                        continue;
                    var descriptor = new TableDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                    item.ExtendedProperties[this.owner] = descriptor;
                    this.tables.Add(descriptor);
                }
            }
        }

        public ReadOnlyObservableCollection<TableCategoryDescriptor> Categories => this.categoriesReadonly;

        public ReadOnlyObservableCollection<TableDescriptor> Tables => this.tablesReadonly;

        [DescriptorProperty(nameof(categoryName))]
        public string Name => this.categoryName ?? string.Empty;

        [DescriptorProperty(nameof(categoryPath))]
        public string Path => this.categoryPath ?? string.Empty;

        [DescriptorProperty]
        public string DisplayName => this.categoryName ?? string.Empty;

        [DescriptorProperty(nameof(lockInfo))]
        public LockInfo LockInfo => this.lockInfo;

        [DescriptorProperty(nameof(accessInfo))]
        public AccessInfo AccessInfo => this.accessInfo;

        [DescriptorProperty(nameof(accessType))]
        public AccessType AccessType => this.accessType;

        [DescriptorProperty]
        public bool IsLocked => LockableDescriptorUtility.IsLocked(this.authentication, this);

        [DescriptorProperty]
        public bool IsLockInherited => LockableDescriptorUtility.IsLockInherited(this.authentication, this);

        [DescriptorProperty]
        public bool IsLockOwner => LockableDescriptorUtility.IsLockOwner(this.authentication, this);

        [DescriptorProperty]
        public bool IsPrivate => AccessibleDescriptorUtility.IsPrivate(this.authentication, this);

        [DescriptorProperty]
        public bool IsAccessInherited => AccessibleDescriptorUtility.IsAccessInherited(this.authentication, this);

        [DescriptorProperty]
        public bool IsAccessOwner => AccessibleDescriptorUtility.IsAccessOwner(this.authentication, this);

        [DescriptorProperty]
        public bool IsAccessMember => AccessibleDescriptorUtility.IsAccessMember(this.authentication, this);

        protected async override void OnDisposed(EventArgs e)
        {
            if (this.referenceTarget == null && this.category != null)
            {
                await this.category.Dispatcher.InvokeAsync(() =>
                {
                    if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                    {
                        this.category.Renamed -= Category_Renamed;
                        this.category.Moved -= Category_Moved;
                        this.category.Deleted -= Category_Deleted;
                        this.category.AccessChanged -= Category_AccessChanged;
                        this.category.LockChanged -= Category_LockChanged;
                    }

                    if (this.descriptorTypes.HasFlag(DescriptorTypes.IsRecursive) == true)
                    {
                        if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                        {
                            this.category.Tables.CollectionChanged -= Tables_CollectionChanged;
                            this.category.Categories.CollectionChanged -= Categories_CollectionChanged;
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

        private async void Category_Moved(object sender, EventArgs e)
        {
            this.categoryName = this.category.Name;
            this.categoryPath = this.category.Path;
            await this.RefreshAsync();
        }

        private void Category_Deleted(object sender, EventArgs e)
        {
            this.category = null;
            this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDisposed(EventArgs.Empty);
            });
        }

        private async void Category_AccessChanged(object sender, EventArgs e)
        {
            this.accessInfo = this.category.AccessInfo;
            this.accessType = this.category.GetAccessType(this.authentication);
            await this.RefreshAsync();
        }

        private async void Category_LockChanged(object sender, EventArgs e)
        {
            this.lockInfo = this.category.LockInfo;
            this.accessType = this.category.GetAccessType(this.authentication);
            await this.RefreshAsync();
        }

        private void Tables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var descriptorList = new List<TableDescriptor>(e.NewItems.Count);
                        foreach (ITable item in e.NewItems)
                        {
                            if (item.Parent != null)
                                continue;
                            if (item.ExtendedProperties.ContainsKey(this.owner) == true)
                            {
                                var descriptor = item.ExtendedProperties[this.owner] as TableDescriptor;
                                descriptorList.Add(descriptor);
                            }
                            else
                            {
                                var descriptor = new TableDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                                item.ExtendedProperties[this.owner] = descriptor;
                                descriptorList.Add(descriptor);
                            }

                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.tables.Add(item);
                            }
                        });
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var descriptorList = new List<TableDescriptor>(e.OldItems.Count);
                        foreach (ITable item in e.OldItems)
                        {
                            var descriptor = item.ExtendedProperties[this.owner] as TableDescriptor;
                            descriptorList.Add(descriptor);
                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.tables.Remove(item);
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
                            this.tables.Clear();
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
                        var descriptorList = new List<TableCategoryDescriptor>(e.NewItems.Count);
                        foreach (ITableCategory item in e.NewItems)
                        {
                            if (item.ExtendedProperties.ContainsKey(this.owner) == true)
                            {
                                var descriptor = item.ExtendedProperties[this.owner] as TableCategoryDescriptor;
                                descriptorList.Add(descriptor);
                            }
                            else
                            {
                                var descriptor = new TableCategoryDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
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
                        var descriptorList = new List<TableCategoryDescriptor>(e.OldItems.Count);
                        foreach (ITableCategory item in e.OldItems)
                        {
                            var descriptor = item.ExtendedProperties[this.owner] as TableCategoryDescriptor;
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

        #region ITableItemDescriptor

        ITableItem ITableItemDescriptor.Target => this.category as ITableItem;

        #endregion

        #region ITableCategoryDescriptor

        ITableCategory ITableCategoryDescriptor.Target => this.category;

        #endregion

        #region ILockableDescriptor

        ILockable ILockableDescriptor.Target => this.category as ILockable;

        #endregion

        #region IAccessibleDescriptor

        IAccessible IAccessibleDescriptor.Target => this.category as IAccessible;

        #endregion

        #region IPermissionDescriptor

        IPermission IPermissionDescriptor.Target => this.category as IPermission;

        #endregion
    }
}
