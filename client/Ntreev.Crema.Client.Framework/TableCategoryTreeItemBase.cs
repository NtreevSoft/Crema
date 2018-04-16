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
using Caliburn.Micro;
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
using System.ComponentModel;

namespace Ntreev.Crema.Client.Framework
{
    public class TableCategoryTreeItemBase : DescriptorTreeItemBase<TableCategoryDescriptor>, ITableCategoryDescriptor, ITableItemDescriptor, ILockableDescriptor, IPermissionDescriptor, IAccessibleDescriptor
    {
        public TableCategoryTreeItemBase(Authentication authentication, ITableCategory category)
            : this(authentication, category, null)
        {

        }

        public TableCategoryTreeItemBase(Authentication authentication, ITableCategory category, object owner)
            : this(authentication, new TableCategoryDescriptor(authentication, category, DescriptorTypes.All, owner), owner)
        {

        }

        public TableCategoryTreeItemBase(Authentication authentication, TableCategoryDescriptor descriptor)
            : this(authentication, descriptor, null)
        {

        }

        public TableCategoryTreeItemBase(Authentication authentication, TableCategoryDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {
            foreach (var item in this.descriptor.Categories)
            {
                this.AddDescriptor(item);
            }

            foreach (var item in this.descriptor.Tables)
            {
                this.AddDescriptor(item);
            }

            if (this.descriptor.Categories is INotifyCollectionChanged categories)
            {
                categories.CollectionChanged += Categories_CollectionChanged;
            }

            if (this.descriptor.Tables is INotifyCollectionChanged tables)
            {
                tables.CollectionChanged += Tables_CollectionChanged;
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

        public bool CanLock => LockableDescriptorUtility.CanLock(this.authentication, this.descriptor);

        public bool CanUnlock => LockableDescriptorUtility.CanUnlock(this.authentication, this.descriptor);

        public bool CanSetPrivate => AccessibleDescriptorUtility.CanSetPrivate(this.authentication, this.descriptor);

        public bool CanSetPublic => AccessibleDescriptorUtility.CanSetPublic(this.authentication, this.descriptor);

        public bool CanSetAuthority => AccessibleDescriptorUtility.CanSetAuthority(this.authentication, this.descriptor);

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

        protected virtual TableCategoryTreeItemBase CreateInstance(Authentication authentication, TableCategoryDescriptor descriptor, object owner)
        {
            return new TableCategoryTreeItemBase(authentication, descriptor, owner);
        }

        protected virtual TableTreeItemBase CreateInstance(Authentication authentication, TableDescriptor descriptor, object owner)
        {
            return new TableTreeItemBase(authentication, descriptor, owner);
        }

        private void Descriptor_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
                            if (item is TableCategoryDescriptor descriptor)
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
                            if (item is TableCategoryDescriptor descriptor)
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

        private void Tables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is TableDescriptor descriptor)
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
                            if (item is TableDescriptor descriptor)
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

        private void AddDescriptor(TableCategoryDescriptor descriptor)
        {
            var viewModel = descriptor.Host == null ? this.CreateInstance(this.authentication, descriptor, this.Owner) : descriptor.Host as TableCategoryTreeItemBase;
            viewModel.Parent = this;
            descriptor.Host = viewModel;
        }

        private void RemoveDescriptor(TableCategoryDescriptor descriptor)
        {
            foreach (var item in this.Items.ToArray())
            {
                if (item is TableCategoryTreeItemBase viewModel && viewModel.Descriptor == descriptor)
                {
                    this.Items.Remove(viewModel);
                }
            }
        }

        private void AddDescriptor(TableDescriptor descriptor)
        {
            var viewModel = descriptor.Host == null ? this.CreateInstance(this.authentication, descriptor, this.Owner) : descriptor.Host as TableTreeItemBase;
            viewModel.Parent = this;
            descriptor.Host = viewModel;
        }

        private void RemoveDescriptor(TableDescriptor descriptor)
        {
            foreach (var item in this.Items.ToArray())
            {
                if (item is TableTreeItemBase viewModel && viewModel.Descriptor == descriptor)
                {
                    this.Items.Remove(viewModel);
                }
            }
        }

        #region ITableCategoryDescriptor

        ITableCategory ITableCategoryDescriptor.Target => this.descriptor.Target as ITableCategory;

        #endregion

        #region ITableItemDescriptor

        ITableItem ITableItemDescriptor.Target => this.descriptor.Target as ITableItem;

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
