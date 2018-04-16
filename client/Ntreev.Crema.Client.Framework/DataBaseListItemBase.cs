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
using Ntreev.Crema.Services;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Framework
{
    public class DataBaseListItemBase : DescriptorListItemBase<DataBaseDescriptor>, IDataBaseDescriptor, IPermissionDescriptor, ILockableDescriptor, IAccessibleDescriptor
    {
        public DataBaseListItemBase(Authentication authentication, IDataBase dataBase, bool IsSubscriptable, object owner)
            : base(authentication, new DataBaseDescriptor(authentication, dataBase, IsSubscriptable == true ? DescriptorTypes.IsSubscriptable : DescriptorTypes.None, owner), owner)
        {

        }

        public DataBaseListItemBase(Authentication authentication, DataBaseDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {

        }

        public string Name => this.descriptor.Name;

        public Guid DataBaseID => this.descriptor.DataBaseID;

        public DataBaseInfo DataBaseInfo => this.descriptor.DataBaseInfo;

        public DataBaseState DataBaseState => this.descriptor.DataBaseState;

        public AuthenticationInfo[] AuthenticationInfos => this.descriptor.AuthenticationInfos;

        public LockInfo LockInfo => this.descriptor.LockInfo;

        public AccessInfo AccessInfo => this.descriptor.AccessInfo;

        public AccessType AccessType => this.descriptor.AccessType;

        public bool IsLoaded => this.descriptor.IsLoaded;

        public bool IsLocked => this.descriptor.IsLocked;

        public bool IsLockInherited => this.descriptor.IsLockInherited;

        public bool IsLockOwner => this.descriptor.IsLockOwner;

        public bool IsPrivate => this.descriptor.IsPrivate;

        public bool IsAccessInherited => this.descriptor.IsAccessInherited;

        public bool IsAccessOwner => this.descriptor.IsAccessOwner;

        public bool IsAccessMember => this.descriptor.IsAccessMember;

        public override string DisplayName => this.descriptor.Name;

        #region IDataBaseDescriptor

        IDataBase IDataBaseDescriptor.Target => this.descriptor.Target as IDataBase;

        #endregion

        #region ILockableDescriptor

        ILockable ILockableDescriptor.Target => this.descriptor.Target as ILockable;

        #endregion

        #region IAccessibleDescriptor

        IAccessible IAccessibleDescriptor.Target => this.descriptor.Target as IAccessible;

        #endregion

        #region IAccessibleDescriptor

        IPermission IPermissionDescriptor.Target => this.descriptor.Target as IPermission;

        #endregion
    }
}
