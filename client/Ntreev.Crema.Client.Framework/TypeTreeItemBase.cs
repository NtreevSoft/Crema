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
using System.Windows;
using Ntreev.Crema.Services;
using Ntreev.Crema.Client.Framework;
using System.Threading.Tasks;
using Caliburn.Micro;
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework;
using System.Linq;
using System.Windows.Input;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.Library.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using Ntreev.ModernUI.Framework.ViewModels;
using Ntreev.Crema.Data;
using Ntreev.Library;
using System.Windows.Threading;

namespace Ntreev.Crema.Client.Framework
{
    public class TypeTreeItemBase : DescriptorTreeItemBase<TypeDescriptor>, ITypeItemDescriptor, ITypeDescriptor, IPermissionDescriptor, ILockableDescriptor, IAccessibleDescriptor
    {
        public TypeTreeItemBase(Authentication authentication, IType type)
            : this(authentication, type, null)
        {

        }

        public TypeTreeItemBase(Authentication authentication, IType type, object owner)
            : this(authentication, new TypeDescriptor(authentication, type, DescriptorTypes.IsSubscriptable, owner), owner)
        {

        }

        public TypeTreeItemBase(Authentication authentication, TypeDescriptor descriptor)
            : this(authentication, descriptor, null)
        {

        }

        public TypeTreeItemBase(Authentication authentication, TypeDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {
            
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

        [DescriptorProperty]
        public bool CanLock => LockableDescriptorUtility.CanLock(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanUnlock => LockableDescriptorUtility.CanUnlock(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanSetPrivate => AccessibleDescriptorUtility.CanSetPrivate(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanSetPublic => AccessibleDescriptorUtility.CanSetPublic(this.authentication, this.descriptor);

        [DescriptorProperty]
        public bool CanSetAuthority => AccessibleDescriptorUtility.CanSetAuthority(this.authentication, this.descriptor);

        public string Name => this.descriptor.Name;

        public string TypeName => this.descriptor.TypeName;

        public string Path => this.descriptor.Path;

        public TypeInfo TypeInfo => this.descriptor.TypeInfo;

        public TypeState TypeState => this.descriptor.TypeState;

        public TypeAttribute TypeAttribute => this.descriptor.TypeAttribute;

        public bool IsBeingEdited => this.descriptor.IsBeingEdited;

        public bool IsBeingEditedClient => this.descriptor.IsBeingEditedClient;

        public bool IsFlag => this.descriptor.IsFlag;

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

        public override string DisplayName => this.descriptor.TypeName;

        #region ITypeItemDescriptor

        ITypeItem ITypeItemDescriptor.Target => this.descriptor.Target as ITypeItem;

        #endregion

        #region ITypeDescriptor

        IType ITypeDescriptor.Target => this.descriptor.Target as IType;

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
