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
using Caliburn.Micro;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ntreev.ModernUI.Framework.ViewModels;

namespace Ntreev.Crema.Client.Framework
{
    public class TypeCategoryTreeItemBase : DescriptorTreeItemBase<TypeCategoryDescriptor>, ITypeCategoryDescriptor, ITypeItemDescriptor, ILockableDescriptor, IPermissionDescriptor, IAccessibleDescriptor
    {
        public TypeCategoryTreeItemBase(Authentication authentication, ITypeCategory category)
            : this(authentication, category, null)
        {

        }

        public TypeCategoryTreeItemBase(Authentication authentication, ITypeCategory category, object owner)
        : this(authentication, new TypeCategoryDescriptor(authentication, category, DescriptorTypes.IsRecursive | DescriptorTypes.IsSubscriptable, owner), owner)
        {

        }

        public TypeCategoryTreeItemBase(Authentication authentication, TypeCategoryDescriptor descriptor)
            : this(authentication, descriptor, null)
        {

        }

        public TypeCategoryTreeItemBase(Authentication authentication, TypeCategoryDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {
            foreach (var item in this.descriptor.Categories)
            {
                this.AddDescriptor(item);
            }

            foreach (var item in this.descriptor.Types)
            {
                this.AddDescriptor(item);
            }

            if (this.descriptor.Categories is INotifyCollectionChanged categories)
            {
                categories.CollectionChanged += Categories_CollectionChanged;
            }

            if (this.descriptor.Types is INotifyCollectionChanged types)
            {
                types.CollectionChanged += Types_CollectionChanged;
            }
        }

        public async Task LockAsync()
        {
            await LockableDescriptorUtility.LockAsync(this.authentication, this.descriptor);
        }

        public async Task UnlockAsync()
        {
            await LockableDescriptorUtility.UnlockAsync(this.authentication, this.descriptor);
        }

        public async Task SetPrivateAsync()
        {
            await AccessibleDescriptorUtility.SetPrivateAsync(this.authentication, this.descriptor);
        }

        public async Task SetPublicAsync()
        {
            await AccessibleDescriptorUtility.SetPublicAsync(this.authentication, this.descriptor);
        }

        public async Task SetAuthorityAsync()
        {
            await AccessibleDescriptorUtility.SetAuthorityAsync(this.authentication, this.descriptor);
        }
        
        public bool CanLock => PermissionDescriptorUtility.CanLock(this.authentication, this.descriptor);

        public bool CanUnlock => PermissionDescriptorUtility.CanUnlock(this.authentication, this.descriptor);

        public bool CanSetPrivate => PermissionDescriptorUtility.CanSetPrivate(this.authentication, this.descriptor);

        public bool CanSetPublic => PermissionDescriptorUtility.CanSetPublic(this.authentication, this.descriptor);

        public bool CanSetAuthority => PermissionDescriptorUtility.CanSetAuthority(this.authentication, this.descriptor);

        public string Name => this.descriptor.Name;

        public string Path => this.descriptor.Path;

        public LockInfo LockInfo => this.descriptor.LockInfo;

        public AccessInfo AccessInfo => this.descriptor.AccessInfo;

        public AccessType AccessType => this.descriptor.AccessType;

        public bool IsLocked => this.descriptor.IsLocked;

        public bool IsLockInherited => this.descriptor.IsLockInherited;

        public bool IsLockOwner => this.descriptor.IsLockOwner;

        public bool IsPrivate => this.descriptor.IsPrivate;

        public bool IsAccessInherited => this.descriptor.IsAccessInherited;

        public bool IsAccessOwner => this.descriptor.IsAccessOwner;

        public bool IsAccessMember => this.descriptor.IsAccessMember;

        public override int Order => 1;

        public override string DisplayName => this.descriptor.Name;

        protected virtual TypeCategoryTreeItemBase CreateInstance(Authentication authentication, TypeCategoryDescriptor descriptor, object owner)
        {
            return new TypeCategoryTreeItemBase(authentication, descriptor, owner);
        }

        protected virtual TypeTreeItemBase CreateInstance(Authentication authentication, TypeDescriptor descriptor, object owner)
        {
            return new TypeTreeItemBase(authentication, descriptor, owner);
        }

        private void Descriptor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == string.Empty && this.Parent != null)
            {
                this.Parent.Items.Reposition(this);
            }
            this.NotifyOfPropertyChange(e.PropertyName);
        }

        private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is TypeCategoryDescriptor descriptor)
                            {
                                this.AddDescriptor(descriptor);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (item is TypeCategoryDescriptor descriptor)
                            {
                                this.RemoveDescriptor(descriptor);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        foreach (var item in this.Items.ToArray())
                        {
                            item.Parent = null;
                        }
                    }
                    break;
            }
        }

        private void Types_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is TypeDescriptor descriptor)
                            {
                                this.AddDescriptor(descriptor);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (item is TypeDescriptor descriptor)
                            {
                                this.RemoveDescriptor(descriptor);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        foreach (var item in this.Items.ToArray())
                        {
                            item.Parent = null;
                        }
                    }
                    break;
            }
        }

        private void AddDescriptor(TypeCategoryDescriptor descriptor)
        {
            var viewModel = descriptor.Host == null ? this.CreateInstance(this.authentication, descriptor, this.Owner) : descriptor.Host as TypeCategoryTreeItemBase;
            viewModel.Parent = this;
            descriptor.Host = viewModel;
        }

        private void RemoveDescriptor(TypeCategoryDescriptor descriptor)
        {
            foreach (var item in this.Items.ToArray())
            {
                if (item is TypeCategoryTreeItemBase viewModel && viewModel.Descriptor == descriptor)
                {
                    this.Items.Remove(viewModel);
                }
            }
        }

        private void AddDescriptor(TypeDescriptor descriptor)
        {
            var viewModel = descriptor.Host == null ? this.CreateInstance(this.authentication, descriptor, this.Owner) : descriptor.Host as TypeTreeItemBase;
            viewModel.Parent = this;
            descriptor.Host = viewModel;
        }

        private void RemoveDescriptor(TypeDescriptor descriptor)
        {
            foreach (var item in this.Items.ToArray())
            {
                if (item is TypeTreeItemBase viewModel && viewModel.Descriptor == descriptor)
                {
                    this.Items.Remove(viewModel);
                }
            }
        }

        #region ITypeCategoryDescriptor

        ITypeCategory ITypeCategoryDescriptor.Target => this.descriptor.Target as ITypeCategory;

        #endregion

        #region ITypeItemDescriptor

        ITypeItem ITypeItemDescriptor.Target => this.descriptor.Target as ITypeItem;

        #endregion

        #region ILockableDescriptor

        ILockable ILockableDescriptor.Target => this.descriptor.Target as ILockable;

        #endregion

        #region IAccessibleDescriptor

        IAccessible IAccessibleDescriptor.Target => this.descriptor.Target as IAccessible;

        #endregion

        #region IPermissionDescriptor

        IPermission IPermissionDescriptor.Target => this.descriptor.Target as IPermission;

        #endregion
    }
}
