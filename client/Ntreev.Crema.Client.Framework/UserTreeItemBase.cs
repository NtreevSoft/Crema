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
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Ntreev.Crema.Services;
using System.Windows.Input;
using Ntreev.Crema.Client.Framework;
using Caliburn.Micro;
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using System.Windows;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Ntreev.Crema.Client.Framework
{
    public class UserTreeItemBase : DescriptorTreeItemBase<UserDescriptor>, IUserDescriptor, IUserItemDescriptor
    {
        public UserTreeItemBase(Authentication authentication, IUser user, bool isSubscriptable, object owner)
            : this(authentication, new UserDescriptor(authentication, user, isSubscriptable == true ? DescriptorTypes.All : DescriptorTypes.IsRecursive, owner), owner)
        {


        }

        internal protected UserTreeItemBase(Authentication authentication, UserDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {

        }

        public string UserID => this.descriptor.UserID;

        public override string DisplayName => this.descriptor.DisplayName;

        public UserInfo UserInfo => this.descriptor.UserInfo;

        public UserState UserState => this.descriptor.UserState;

        public BanInfo BanInfo => this.descriptor.BanInfo;

        public bool IsOnline => this.descriptor.IsOnline;

        public bool IsBanned => this.descriptor.IsBanned;

        public bool IsAdmin => this.descriptor.IsAdmin;

        public bool IsMember => this.descriptor.IsMember;

        public bool IsGuest => this.descriptor.IsGuest;

        #region IUserItemDescriptor

        IUserItem IUserItemDescriptor.Target => this.descriptor.Target as IUserItem;

        string IUserItemDescriptor.Name => this.descriptor.UserID;

        string IUserItemDescriptor.Path => this.descriptor.UserInfo.CategoryPath + this.descriptor.UserInfo.ID;

        #endregion

        #region IUserDescriptor

        IUser IUserDescriptor.Target => this.descriptor.Target as IUser;

        #endregion
    }
}
