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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceModel;
using System.Windows;
using Ntreev.ModernUI.Framework;
using System.Windows.Media;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using System.ComponentModel.Composition;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using Ntreev.ModernUI.Framework.ViewModels;

namespace Ntreev.Crema.Client.Framework
{
    public class DataBaseDescriptor : DescriptorBase, IDataBaseDescriptor, IPermissionDescriptor, ILockableDescriptor, IAccessibleDescriptor
    {
        private IDataBase dataBase;
        private readonly object owner;

        private DataBaseInfo dataBaseInfo = DataBaseInfo.Empty;
        private DataBaseState dataBaseState;
        private AuthenticationInfo[] authenticationInfos;
        private LockInfo lockInfo = LockInfo.Empty;
        private AccessInfo accessInfo = AccessInfo.Empty;
        private AccessType accessType;

        public DataBaseDescriptor(Authentication authentication, DataBaseInfo dataBaseInfo)
            : base(authentication, null, DescriptorTypes.None)
        {
            this.dataBaseInfo = dataBaseInfo;
            this.owner = this;
        }

        public DataBaseDescriptor(Authentication authentication, IDataBaseDescriptor descriptor, bool isSubscriptable, object owner)
            : base(authentication, descriptor.Target, descriptor, isSubscriptable)
        {
            this.dataBase = descriptor.Target;
            this.owner = owner ?? this;
        }

        public DataBaseDescriptor(Authentication authentication, IDataBase dataBase, DescriptorTypes descriptorTypes, object owner)
            : base(authentication, dataBase, descriptorTypes)
        {
            this.dataBase = dataBase;
            this.owner = owner ?? this;
            this.dataBase.Dispatcher.VerifyAccess();
            this.dataBaseInfo = dataBase.DataBaseInfo;
            this.dataBaseState = dataBase.DataBaseState;
            this.authenticationInfos = dataBase.AuthenticationInfos;
            this.lockInfo = dataBase.LockInfo;
            this.accessInfo = dataBase.AccessInfo;
            this.accessType = dataBase.GetAccessType(authentication);

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
            {
                this.dataBase.Renamed += DataBase_Renamed;
                this.dataBase.Deleted += DataBase_Deleted;
                this.dataBase.Loaded += DataBase_Loaded;
                this.dataBase.Unloaded += DataBase_Unloaded;
                this.dataBase.AuthenticationEntered += DataBase_AuthenticationEntered;
                this.dataBase.AuthenticationLeft += DataBase_AuthenticationLeft;
                this.dataBase.DataBaseInfoChanged += DataBase_DataBaseInfoChanged;
                this.dataBase.DataBaseStateChanged += DataBase_DataBaseStateChanged;
                this.dataBase.AccessChanged += DataBase_AccessChanged;
                this.dataBase.LockChanged += DataBase_LockChanged;
            }
        }

        [DescriptorProperty]
        public string Name => this.dataBaseInfo.Name;

        [DescriptorProperty]
        public string Path => this.dataBaseInfo.Name;

        [DescriptorProperty]
        public Guid DataBaseID => this.dataBaseInfo.ID;

        [DescriptorProperty(nameof(dataBaseInfo))]
        public DataBaseInfo DataBaseInfo => this.dataBaseInfo;

        [DescriptorProperty(nameof(dataBaseState))]
        public DataBaseState DataBaseState => this.dataBaseState;

        [DescriptorProperty(nameof(authenticationInfos))]
        public AuthenticationInfo[] AuthenticationInfos => this.authenticationInfos;

        [DescriptorProperty(nameof(lockInfo))]
        public LockInfo LockInfo => this.lockInfo;

        [DescriptorProperty(nameof(accessInfo))]
        public AccessInfo AccessInfo => this.accessInfo;

        [DescriptorProperty(nameof(accessType))]
        public AccessType AccessType => this.accessType;

        [DescriptorProperty]
        public bool IsLoaded => DataBaseDescriptorUtility.IsLoaded(Authenticator.Current, this);

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
            if (this.referenceTarget == null && this.dataBase != null)
            {
                await this.dataBase.Dispatcher.InvokeAsync(() =>
                {
                    if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                    {
                        this.dataBase.Renamed -= DataBase_Renamed;
                        this.dataBase.Deleted -= DataBase_Deleted;
                        this.dataBase.Loaded -= DataBase_Loaded;
                        this.dataBase.Unloaded -= DataBase_Unloaded;
                        this.dataBase.AuthenticationEntered -= DataBase_AuthenticationEntered;
                        this.dataBase.AuthenticationLeft -= DataBase_AuthenticationLeft;
                        this.dataBase.DataBaseInfoChanged -= DataBase_DataBaseInfoChanged;
                        this.dataBase.DataBaseStateChanged -= DataBase_DataBaseStateChanged;
                        this.dataBase.AccessChanged -= DataBase_AccessChanged;
                        this.dataBase.LockChanged -= DataBase_LockChanged;
                    }
                });
            }
            base.OnDisposed(e);
        }

        private async void DataBase_Renamed(object sender, EventArgs e)
        {
            this.dataBaseInfo = this.dataBase.DataBaseInfo;
            await this.RefreshAsync();
        }

        private void DataBase_Deleted(object sender, EventArgs e)
        {
            this.dataBase = null;
            this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDisposed(EventArgs.Empty);
            });
        }

        private async void DataBase_Loaded(object sender, EventArgs e)
        {
            this.dataBaseState = this.dataBase.DataBaseState;
            await this.RefreshAsync();
        }

        private async void DataBase_Unloaded(object sender, EventArgs e)
        {
            this.dataBaseState = this.dataBase.DataBaseState;
            this.authenticationInfos = this.dataBase.AuthenticationInfos;
            await this.RefreshAsync();
        }

        private async void DataBase_AuthenticationEntered(object sender, AuthenticationEventArgs e)
        {
            this.authenticationInfos = this.dataBase.AuthenticationInfos;
            await this.RefreshAsync();
        }

        private async void DataBase_AuthenticationLeft(object sender, AuthenticationEventArgs e)
        {
            this.authenticationInfos = this.dataBase.AuthenticationInfos;
            await this.RefreshAsync();
        }

        private async void DataBase_DataBaseInfoChanged(object sender, EventArgs e)
        {
            this.dataBaseInfo = this.dataBase.DataBaseInfo;
            await this.RefreshAsync();
        }

        private async void DataBase_DataBaseStateChanged(object sender, EventArgs e)
        {
            this.dataBaseState = this.dataBase.DataBaseState;
            await this.RefreshAsync();
        }

        private async void DataBase_LockChanged(object sender, EventArgs e)
        {
            this.lockInfo = this.dataBase.LockInfo;
            this.accessType = this.dataBase.GetAccessType(this.authentication);
            await this.RefreshAsync();
        }

        private async void DataBase_AccessChanged(object sender, EventArgs e)
        {
            this.accessInfo = this.dataBase.AccessInfo;
            this.accessType = this.dataBase.GetAccessType(this.authentication);
            await this.RefreshAsync();
        }

        #region IDataBaseDescriptor

        IDataBase IDataBaseDescriptor.Target => this.dataBase as IDataBase;

        #endregion

        #region ILockableDescriptor

        ILockable ILockableDescriptor.Target => this.dataBase as ILockable;

        #endregion

        #region IAccessibleDescriptor

        IAccessible IAccessibleDescriptor.Target => this.dataBase as IAccessible;

        #endregion

        #region IPermissionDescriptor

        IPermission IPermissionDescriptor.Target => this.dataBase as IPermission;

        #endregion
    }
}
