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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ntreev.Crema.Client.Framework
{
    public class DomainUserTreeItemBase : DescriptorTreeItemBase<DomainUserDescriptor>, IDomainUserDescriptor
    {
        private Brush foreground;

        public DomainUserTreeItemBase(Authentication authentication, IDomainUser domainUser, bool isSubscriptable, object owner)
            : base(authentication, new DomainUserDescriptor(authentication, domainUser, isSubscriptable == true ? DescriptorTypes.All : DescriptorTypes.IsRecursive, owner), owner)
        {

        }

        public DomainUserTreeItemBase(Authentication authentication, DomainUserDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {

        }

        public string UserID => this.descriptor.UserID;

        public DomainUserInfo DomainUserInfo => this.descriptor.DomainUserInfo;

        public DomainUserState DomainUserState => this.descriptor.DomainUserState;

        public bool IsOnline => this.descriptor.IsOnline;

        public bool IsModified => this.descriptor.IsModified;

        public bool IsOwner => this.descriptor.IsOwner;

        public bool CanWrite => this.descriptor.CanWrite;

        public bool CanRead => this.descriptor.CanRead;

        public override string DisplayName => this.descriptor.DisplayName;

        public Brush Foreground
        {
            get
            {
                if (this.foreground == null)
                {
                    this.foreground = new SolidColorBrush(IDomainUserExtensions.GetColor(this.descriptor.DomainUserInfo));
                }
                return this.foreground;
            }
        }
                
        #region IDomainUserDescriptor

        IDomainUser IDomainUserDescriptor.Target => this.descriptor.Target as IDomainUser;

        #endregion
    }
}
