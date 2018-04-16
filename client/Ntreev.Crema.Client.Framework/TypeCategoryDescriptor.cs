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
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Client.Framework;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Collections.ObjectModel;

namespace Ntreev.Crema.Client.Framework
{
    public class TypeCategoryDescriptor : DescriptorBase, ITypeCategoryDescriptor, ITypeItemDescriptor, ILockableDescriptor, IPermissionDescriptor, IAccessibleDescriptor
    {
        private ITypeCategory category;
        private readonly object owner;
        private readonly ObservableCollection<TypeCategoryDescriptor> categories = new ObservableCollection<TypeCategoryDescriptor>();
        private readonly ReadOnlyObservableCollection<TypeCategoryDescriptor> categoriesReadonly;
        private readonly ObservableCollection<TypeDescriptor> types = new ObservableCollection<TypeDescriptor>();
        private readonly ReadOnlyObservableCollection<TypeDescriptor> typesReadonly;

        private string categoryName;
        private string categoryPath;
        private AccessInfo accessInfo = AccessInfo.Empty;
        private LockInfo lockInfo = LockInfo.Empty;
        private AccessType accessType;

        public TypeCategoryDescriptor(Authentication authentication, ITypeCategoryDescriptor descriptor, bool isSubscriptable, object owner)
            : base(authentication, descriptor.Target, descriptor, isSubscriptable)
        {
            this.category = descriptor.Target;
            this.owner = owner ?? this;
        }

        public TypeCategoryDescriptor(Authentication authentication, ITypeCategory category, DescriptorTypes descriptorTypes, object owner)
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

            this.typesReadonly = new ReadOnlyObservableCollection<TypeDescriptor>(this.types);
            this.categoriesReadonly = new ReadOnlyObservableCollection<TypeCategoryDescriptor>(this.categories);
            this.category.ExtendedProperties[this.owner] = this;

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
                    this.category.Types.CollectionChanged += Types_CollectionChanged;
                    this.category.Categories.CollectionChanged += Categories_CollectionChanged;
                }

                foreach (var item in this.category.Categories)
                {
                    var descriptor = new TypeCategoryDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                    this.categories.Add(descriptor);
                }

                foreach (var item in this.category.Types)
                {
                    var descriptor = new TypeDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                    this.types.Add(descriptor);
                }
            }
        }

        [DescriptorProperty(nameof(categoryName))]
        public string Name => this.categoryName ?? string.Empty;

        [DescriptorProperty(nameof(categoryName))]
        public string Path => this.categoryPath ?? string.Empty;

        [DescriptorProperty]
        public string DisplayName => this.categoryName ?? string.Empty;

        [DescriptorProperty(nameof(lockInfo))]
        public LockInfo LockInfo => this.lockInfo;

        [DescriptorProperty(nameof(accessInfo))]
        public AccessInfo AccessInfo => this.accessInfo;

        [DescriptorProperty(nameof(accessType))]
        public AccessType AccessType => this.accessType;

        public ReadOnlyObservableCollection<TypeCategoryDescriptor> Categories => this.categoriesReadonly;

        public ReadOnlyObservableCollection<TypeDescriptor> Types => this.typesReadonly;

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
                            this.category.Types.CollectionChanged -= Types_CollectionChanged;
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

        private void Types_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var descriptorList = new List<TypeDescriptor>(e.NewItems.Count);
                        foreach (IType item in e.NewItems)
                        {
                            if (item.ExtendedProperties.ContainsKey(this.owner) == true)
                            {
                                var descriptor = item.ExtendedProperties[this.owner] as TypeDescriptor;
                                descriptorList.Add(descriptor);
                            }
                            else
                            {
                                var descriptor = new TypeDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                                descriptorList.Add(descriptor);
                            }

                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.types.Add(item);
                            }
                        });
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var descriptorList = new List<TypeDescriptor>(e.OldItems.Count);
                        foreach (IType item in e.OldItems)
                        {
                            var descriptor = item.ExtendedProperties[this.owner] as TypeDescriptor;
                            descriptorList.Add(descriptor);
                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.types.Remove(item);
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
                            this.types.Clear();
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
                        var descriptorList = new List<TypeCategoryDescriptor>(e.NewItems.Count);
                        foreach (ITypeCategory item in e.NewItems)
                        {
                            if (item.ExtendedProperties.ContainsKey(this.owner) == true)
                            {
                                var descriptor = item.ExtendedProperties[this.owner] as TypeCategoryDescriptor;
                                descriptorList.Add(descriptor);
                            }
                            else
                            {
                                var descriptor = new TypeCategoryDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
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
                        var descriptorList = new List<TypeCategoryDescriptor>(e.OldItems.Count);
                        foreach (ITypeCategory item in e.OldItems)
                        {
                            var descriptor = item.ExtendedProperties[this.owner] as TypeCategoryDescriptor;
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

        #region ITypeItemDescriptor

        ITypeItem ITypeItemDescriptor.Target => this.category as ITypeItem;

        #endregion

        #region ITypeCategoryDescriptor

        ITypeCategory ITypeCategoryDescriptor.Target => this.category;

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
