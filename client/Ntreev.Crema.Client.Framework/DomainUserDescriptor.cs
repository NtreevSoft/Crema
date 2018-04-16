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
using System.ComponentModel;

namespace Ntreev.Crema.Client.Framework
{
    public class DomainUserDescriptor : DescriptorBase, IDomainUserDescriptor
    {
        private readonly IDomainUser domainUser;
        private readonly object owner;

        private DomainUserInfo domainUserInfo;
        private DomainUserState domainUserState;

        private Action disposeAction;

        public DomainUserDescriptor(Authentication authentication, IDomainUserDescriptor descriptor, DescriptorTypes descriptorTypes, object owner)
            : base(authentication, descriptor.Target, descriptorTypes)
        {
            this.domainUser = descriptor.Target;
            this.owner = owner ?? this;
            this.domainUser.Dispatcher.VerifyAccess();
            this.domainUserInfo = domainUser.DomainUserInfo;
            this.domainUserState = domainUser.DomainUserState;

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true && descriptor is INotifyPropertyChanged obj)
            {
                obj.PropertyChanged += Descriptor_PropertyChanged;
                this.disposeAction = new Action(() => obj.PropertyChanged -= Descriptor_PropertyChanged);
            }
        }

        public DomainUserDescriptor(Authentication authentication, IDomainUser domainUser, DescriptorTypes descriptorTypes, object owner)
            : base(authentication, domainUser, descriptorTypes)
        {
            this.domainUser = domainUser;
            this.owner = owner ?? this;
            this.domainUser.Dispatcher.VerifyAccess();
            this.domainUserInfo = domainUser.DomainUserInfo;
            this.domainUserState = domainUser.DomainUserState;

            if (this.descriptorTypes.HasFlag(DescriptorTypes.IsSubscriptable) == true)
            {
                this.domainUser.DomainUserInfoChanged += DomainUser_DomainUserInfoChanged;
                this.domainUser.DomainUserStateChanged += DomainUser_DomainUserStateChanged;
            }
        }

        [DescriptorProperty]
        public string UserID => this.domainUserInfo.UserID;

        [DescriptorProperty]
        public string DisplayName => this.domainUserInfo.UserID + " [" + this.domainUserInfo.UserName + "]";

        [DescriptorProperty(nameof(domainUserInfo))]
        public DomainUserInfo DomainUserInfo => this.domainUserInfo;

        [DescriptorProperty(nameof(domainUserState))]
        public DomainUserState DomainUserState => this.domainUserState;

        [DescriptorProperty]
        public bool IsOnline => DomainUserDescriptorUtility.IsOnline(this.authentication, this);

        [DescriptorProperty]
        public bool IsModified => DomainUserDescriptorUtility.IsModified(this.authentication, this);

        [DescriptorProperty]
        public bool IsOwner => DomainUserDescriptorUtility.IsOwner(this.authentication, this);

        [DescriptorProperty]
        public bool CanWrite => DomainUserDescriptorUtility.CanWrite(this.authentication, this);

        [DescriptorProperty]
        public bool CanRead => DomainUserDescriptorUtility.CanRead(this.authentication, this);

        private void DomainUser_DomainUserStateChanged(object sender, EventArgs e)
        {
            this.domainUserState = this.domainUser.DomainUserState;
            this.Dispatcher.InvokeAsync(this.RefreshAsync);
        }

        private void DomainUser_DomainUserInfoChanged(object sender, EventArgs e)
        {
            this.domainUserInfo = this.domainUser.DomainUserInfo;
            this.Dispatcher.InvokeAsync(this.RefreshAsync);
        }

        private void Descriptor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != string.Empty)
            {
                var prop1 = sender.GetType().GetProperty(e.PropertyName);
                var prop2 = this.GetType().GetProperty(e.PropertyName);

                if (prop1 != null && prop1.CanRead == true && prop2 != null && prop2.CanWrite == true)
                {
                    var value = prop1.GetValue(sender);
                    prop2.SetValue(this, value);
                }
            }
            else if (sender is IDomainUserDescriptor descriptor)
            {
                this.domainUserInfo = descriptor.DomainUserInfo;
                this.domainUserState = descriptor.DomainUserState;
            }
            this.NotifyOfPropertyChange(e.PropertyName);
        }

        #region IDomainDescriptor

        IDomainUser IDomainUserDescriptor.Target => this.domainUser;

        #endregion
    }
}
