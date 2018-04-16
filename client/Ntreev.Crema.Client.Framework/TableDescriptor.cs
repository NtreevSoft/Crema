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
using System.Linq;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Threading;
using Ntreev.Crema.Services;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.ServiceModel;
using System.Threading.Tasks;
using Ntreev.Library.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Data;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.Library.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Collections;
using Ntreev.Library;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Ntreev.Crema.Client.Framework
{
    public class TableDescriptor : DescriptorBase, ITableDescriptor, ITableItemDescriptor, ILockableDescriptor, IPermissionDescriptor, IAccessibleDescriptor
    {
        private ITable table;
        private readonly object owner;
        private readonly ObservableCollection<TableDescriptor> childs = new ObservableCollection<TableDescriptor>();
        private readonly ReadOnlyObservableCollection<TableDescriptor> childsReadonly;

        private TableInfo tableInfo = TableInfo.Default;
        private AccessInfo accessInfo = AccessInfo.Empty;
        private LockInfo lockInfo = LockInfo.Empty;
        private AccessType accessType;
        private TableState tableState;
        private TableAttribute tableAttribute;

        private TableTemplateDescriptor templateDescriptor;
        private TableContentDescriptor contentDescriptor;

        public TableDescriptor(Authentication authentication, ITableDescriptor descriptor)
            : this(authentication, descriptor, false)
        {

        }

        public TableDescriptor(Authentication authentication, ITableDescriptor descriptor, bool isSubscriptable)
            : this(authentication, descriptor, isSubscriptable, null)
        {

        }

        public TableDescriptor(Authentication authentication, ITableDescriptor descriptor, bool isSubscriptable, object owner)
            : base(authentication, descriptor.Target, descriptor, isSubscriptable)
        {
            this.table = descriptor.Target;
            this.owner = owner ?? this;
            this.childsReadonly = new ReadOnlyObservableCollection<TableDescriptor>(this.childs);
        }

        public TableDescriptor(Authentication authentication, ITable table, DescriptorTypes descriptorTypes, object owner)
            : base(authentication, table, descriptorTypes)
        {
            this.table = table;
            this.owner = owner ?? this;
            this.table.Dispatcher.VerifyAccess();
            this.tableInfo = table.TableInfo;
            this.tableState = table.TableState;
            this.tableAttribute = TableAttribute.None;
            if (this.table.DerivedTables.Any() == true)
                this.tableAttribute |= TableAttribute.BaseTable;
            if (this.table.TemplatedParent != null)
                this.tableAttribute |= TableAttribute.DerivedTable;
            if (this.table.Parent != null)
                this.tableAttribute |= TableAttribute.HasParent;
            this.lockInfo = table.LockInfo;
            this.accessInfo = table.AccessInfo;
            this.accessType = table.GetAccessType(this.authentication);
            this.templateDescriptor = new TableTemplateDescriptor(authentication, table.Template, descriptorTypes, owner);
            this.contentDescriptor = new TableContentDescriptor(authentication, table.Content, descriptorTypes, owner);
            this.childsReadonly = new ReadOnlyObservableCollection<TableDescriptor>(this.childs);
            this.table.ExtendedProperties[this.owner] = this;

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
            {
                this.table.Deleted += Table_Deleted;
                this.table.LockChanged += Table_LockChanged;
                this.table.AccessChanged += Table_AccessChanged;
                this.table.TableInfoChanged += Table_TableInfoChanged;
                this.table.TableStateChanged += Table_TableStateChanged;
                this.table.DerivedTables.CollectionChanged += DerivedTables_CollectionChanged;
            }

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsRecursive) == true)
            {
                if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                {
                    this.table.Childs.CollectionChanged += Childs_CollectionChanged;
                }

                foreach (var item in this.table.Childs)
                {
                    var descriptor = new TableDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                    this.childs.Add(descriptor);
                }
            }
        }

        [DescriptorProperty]
        public string Name => this.tableInfo.Name;

        [DescriptorProperty]
        public string TableName => this.tableInfo.TableName;

        [DescriptorProperty]
        public string Path => this.tableInfo.CategoryPath + this.tableInfo.Name;

        [DescriptorProperty]
        public string DisplayName => this.tableInfo.Name;

        public ReadOnlyObservableCollection<TableDescriptor> Childs => this.childsReadonly;

        [DescriptorProperty(nameof(tableInfo))]
        public TableInfo TableInfo => this.tableInfo;

        [DescriptorProperty(nameof(tableState))]
        public TableState TableState => this.tableState;

        [DescriptorProperty(nameof(tableAttribute))]
        public TableAttribute TableAttribute => this.tableAttribute;

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

        [DescriptorProperty]
        public bool IsBeingEdited => TableDescriptorUtility.IsBeingEdited(this.authentication, this);

        [DescriptorProperty]
        public bool IsBeingEditedClient => TableDescriptorUtility.IsBeingEditedClient(this.authentication, this);

        [DescriptorProperty]
        public bool IsBeingSetup => TableDescriptorUtility.IsBeingSetup(this.authentication, this);

        [DescriptorProperty]
        public bool IsBeingSetupClient => TableDescriptorUtility.IsBeingSetupClient(this.authentication, this);

        [DescriptorProperty]
        public bool IsInherited => TableDescriptorUtility.IsInherited(this.authentication, this);

        [DescriptorProperty]
        public bool IsBaseTemplate => TableDescriptorUtility.IsBaseTemplate(this.authentication, this);

        [DescriptorProperty]
        public TableTemplateDescriptor TemplateDescriptor => this.templateDescriptor;

        [DescriptorProperty]
        public TableContentDescriptor ContentDescriptor => this.contentDescriptor;

        protected async override void OnDisposed(EventArgs e)
        {
            if (this.referenceTarget == null && this.table != null)
            {
                await this.table.Dispatcher.InvokeAsync(() =>
                {
                    if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                    {
                        this.table.Deleted -= Table_Deleted;
                        this.table.LockChanged -= Table_LockChanged;
                        this.table.AccessChanged -= Table_AccessChanged;
                        this.table.TableInfoChanged -= Table_TableInfoChanged;
                        this.table.TableStateChanged -= Table_TableStateChanged;
                        this.table.DerivedTables.CollectionChanged -= DerivedTables_CollectionChanged;
                    }

                    if (this.descriptorTypes.HasFlag(DescriptorTypes.IsRecursive) == true)
                    {
                        if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                        {
                            this.table.Childs.CollectionChanged -= Childs_CollectionChanged;
                        }
                    }
                });
            }
            base.OnDisposed(e);
        }

        private void Table_Deleted(object sender, EventArgs e)
        {
            this.table = null;
            this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDisposed(EventArgs.Empty);
            });
        }

        private async void Table_LockChanged(object sender, EventArgs e)
        {
            this.lockInfo = this.table.LockInfo;
            this.accessType = this.table.GetAccessType(this.authentication);
            await this.RefreshAsync();
        }

        private async void Table_AccessChanged(object sender, EventArgs e)
        {
            this.accessInfo = this.table.AccessInfo;
            this.accessType = this.table.GetAccessType(this.authentication);
            await this.RefreshAsync();
        }

        private async void Table_TableInfoChanged(object sender, EventArgs e)
        {
            this.tableInfo = this.table.TableInfo;
            if (this.table.DerivedTables.Any() == true)
                this.tableAttribute |= TableAttribute.BaseTable;
            else
                this.tableAttribute &= ~TableAttribute.BaseTable;
            if (this.table.TemplatedParent != null)
                this.tableAttribute |= TableAttribute.DerivedTable;
            else
                this.tableAttribute &= ~TableAttribute.DerivedTable;
            await this.RefreshAsync();
        }

        private async void Table_TableStateChanged(object sender, EventArgs e)
        {
            this.tableState = this.table.TableState;
            await this.RefreshAsync();
        }

        private async void DerivedTables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.table.DerivedTables.Any() == true)
                this.tableAttribute |= TableAttribute.BaseTable;
            else
                this.tableAttribute &= ~TableAttribute.BaseTable;
            await this.RefreshAsync();
        }

        private void Childs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var descriptorList = new List<TableDescriptor>(e.NewItems.Count);
                        foreach (ITable item in e.NewItems)
                        {
                            if (item.ExtendedProperties.ContainsKey(this.owner) == true)
                            {
                                var descriptor = item.ExtendedProperties[this.owner] as TableDescriptor;
                                descriptorList.Add(descriptor);
                            }
                            else
                            {
                                var descriptor = new TableDescriptor(this.authentication, item, this.descriptorTypes, this.owner);
                                descriptorList.Add(descriptor);
                            }

                        }
                        this.Dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in descriptorList)
                            {
                                this.childs.Add(item);
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
                                this.childs.Remove(item);
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
                            this.childs.Clear();
                        });
                    }
                    break;
            }
        }

        #region ITableItemDescriptor

        ITableItem ITableItemDescriptor.Target => this.table as ITableItem;

        #endregion

        #region ITableDescriptor

        ITable ITableDescriptor.Target => this.table;

        #endregion

        #region ILockableDescriptor

        ILockable ILockableDescriptor.Target => this.table as ILockable;

        #endregion

        #region IAccessibleDescriptor

        IAccessible IAccessibleDescriptor.Target => this.table as IAccessible;

        #endregion

        #region IPermissionDescriptor

        IPermission IPermissionDescriptor.Target => this.table as IPermission;

        #endregion
    }
}
