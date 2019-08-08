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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.Client.Framework
{
    public class UserAuthenticationTreeItemBase : DescriptorTreeItemBase<UserAuthenticationDescriptor>, IUserAuthenticationDescriptor
    {
        public UserAuthenticationTreeItemBase(Authentication authentication, IUserAuthentication userAuthentication, bool isSubscriptable, object owner)
            : base(authentication, new UserAuthenticationDescriptor(authentication, userAuthentication, isSubscriptable ? DescriptorTypes.All : DescriptorTypes.IsRecursive, owner), owner)
        {

        }

        internal protected UserAuthenticationTreeItemBase(Authentication authentication, UserAuthenticationDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {
        }

        public UserInfo UserInfo => this.Descriptor.UserInfo;

        public bool IsOnline => this.Descriptor.IsOnline;

        public IUser User => this.Descriptor.User;

        IUserAuthentication IUserAuthenticationDescriptor.Target => this.Descriptor.Target as IUserAuthentication;
    }
}
