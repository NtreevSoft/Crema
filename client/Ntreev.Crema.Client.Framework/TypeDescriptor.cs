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
using System.ComponentModel;

namespace Ntreev.Crema.Client.Framework
{
    public class TypeDescriptor : DescriptorBase, ITypeItemDescriptor, ITypeDescriptor, IPermissionDescriptor, ILockableDescriptor, IAccessibleDescriptor
    {
        private IType type;
        private readonly object owner;

        private TypeInfo typeInfo = TypeInfo.Default;
        private AccessInfo accessInfo = AccessInfo.Empty;
        private LockInfo lockInfo = LockInfo.Empty;
        private AccessType accessType;
        private TypeState typeState;
        private TypeAttribute typeAttribute;

        private TypeTemplateDescriptor templateDescriptor;

        public TypeDescriptor(Authentication authentication, ITypeDescriptor descriptor, bool isSubscriptable, object owner)
            : base(authentication, descriptor.Target, descriptor, isSubscriptable)
        {
            this.type = descriptor.Target;
            this.owner = owner ?? this;
        }

        public TypeDescriptor(Authentication authentication, IType type, DescriptorTypes descriptorTypes, object owner)
            : base(authentication, type, descriptorTypes)
        {
            this.type = type;
            this.owner = owner ?? this;
            this.type.Dispatcher.VerifyAccess();
            this.typeInfo = type.TypeInfo;
            this.typeState = type.TypeState;
            if (this.type.TypeInfo.IsFlag == true)
                this.typeAttribute |= TypeAttribute.IsFlag;
            this.lockInfo = type.LockInfo;
            this.accessInfo = type.AccessInfo;
            this.accessType = type.GetAccessType(this.authentication);
            this.templateDescriptor = new TypeTemplateDescriptor(authentication, type.Template, descriptorTypes);

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
            {
                this.type.Deleted += Type_Deleted;
                this.type.LockChanged += Type_LockChanged;
                this.type.AccessChanged += Type_AccessChanged;
                this.type.TypeInfoChanged += Type_TypeInfoChanged;
                this.type.TypeStateChanged += Type_TypeStateChanged;
            }
        }

        [DescriptorProperty]
        public string Name => this.typeInfo.Name;

        [DescriptorProperty]
        public string TypeName => this.typeInfo.Name;

        [DescriptorProperty]
        public string Path => this.typeInfo.CategoryPath + this.typeInfo.Name;

        [DescriptorProperty]
        public string DisplayName => this.typeInfo.Name;

        [DescriptorProperty(nameof(typeInfo))]
        public TypeInfo TypeInfo => this.typeInfo;

        [DescriptorProperty(nameof(typeState))]
        public TypeState TypeState => this.typeState;

        [DescriptorProperty(nameof(typeAttribute))]
        public TypeAttribute TypeAttribute => this.typeAttribute;

        [DescriptorProperty]
        public bool IsBeingEdited => TypeDescriptorUtility.IsBeingEdited(this.authentication, this);

        [DescriptorProperty]
        public bool IsBeingEditedClient => TypeDescriptorUtility.IsBeingEditedClient(this.authentication, this);

        [DescriptorProperty]
        public bool IsFlag => TypeDescriptorUtility.IsFlag(this.authentication, this);

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
        public TypeTemplateDescriptor TemplateDescriptor => this.templateDescriptor;

        protected async override void OnDisposed(EventArgs e)
        {
            if (this.referenceTarget == null && this.type != null)
            {
                await this.type.Dispatcher.InvokeAsync(() =>
                {
                    if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
                    {
                        this.type.Deleted -= Type_Deleted;
                        this.type.LockChanged -= Type_LockChanged;
                        this.type.AccessChanged -= Type_AccessChanged;
                        this.type.TypeInfoChanged -= Type_TypeInfoChanged;
                        this.type.TypeStateChanged -= Type_TypeStateChanged;
                    }
                });
            }
            base.OnDisposed(e);
        }

        private void Type_Deleted(object sender, EventArgs e)
        {
            this.type = null;
            this.Dispatcher.InvokeAsync(() =>
            {
                this.OnDisposed(EventArgs.Empty);
            });
        }

        private async void Type_LockChanged(object sender, EventArgs e)
        {
            this.lockInfo = this.type.LockInfo;
            this.accessType = this.type.GetAccessType(this.authentication);
            await this.RefreshAsync();
        }

        private async void Type_AccessChanged(object sender, EventArgs e)
        {
            this.accessInfo = this.type.AccessInfo;
            this.accessType = this.type.GetAccessType(this.authentication);
            await this.RefreshAsync();
        }

        private async void Type_TypeInfoChanged(object sender, EventArgs e)
        {
            this.typeInfo = this.type.TypeInfo;
            if (this.typeInfo.IsFlag == true)
                this.typeAttribute |= TypeAttribute.IsFlag;
            else
                this.typeAttribute &= ~TypeAttribute.IsFlag;
            await this.RefreshAsync();
        }

        private async void Type_TypeStateChanged(object sender, EventArgs e)
        {
            this.typeState = this.type.TypeState;
            await this.RefreshAsync();
        }

        #region ITypeItemDescriptor

        ITypeItem ITypeItemDescriptor.Target => this.type as ITypeItem;

        #endregion

        #region ITypeDescriptor

        IType ITypeDescriptor.Target => this.type;

        #endregion

        #region ILockableDescriptor

        ILockable ILockableDescriptor.Target => this.type as ILockable;

        #endregion

        #region IAccessibleDescriptor

        IAccessible IAccessibleDescriptor.Target => this.type as IAccessible;

        #endregion

        #region IPermissionDescriptor

        IPermission IPermissionDescriptor.Target => this.type as IPermission;

        #endregion
    }
}
