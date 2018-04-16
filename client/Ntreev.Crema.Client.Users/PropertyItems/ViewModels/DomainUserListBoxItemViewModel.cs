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
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Users.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using System.Windows;
using Ntreev.ModernUI.Framework.ViewModels;
using System.Windows.Media;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Users.PropertyItems.ViewModels
{
    class DomainUserListBoxItemViewModel : DomainUserListItemBase
    {
        public DomainUserListBoxItemViewModel(Authentication authentication, IDomainUser domainUser, object owner)
            : base(authentication, new DomainUserDescriptor(authentication, domainUser, DescriptorTypes.All, owner), owner)
        {

        }

        public async Task SendMessageAsync()
        {
            await DomainUserUtility.SendMessageAsync(this.authentication, this);
        }

        public async Task SetOwnerAsync()
        {
            await DomainUserUtility.SetOwnerAsync(this.authentication, this);
        }

        public async Task KickAsync()
        {
            await DomainUserUtility.KickAsync(this.authentication, this);
        }

        public bool CanSendMessage => DomainUserUtility.CanSendMessage(this.authentication, this);

        public bool CanSetOwner => DomainUserUtility.CanSetOwner(this.authentication, this);

        public bool CanKick => DomainUserUtility.CanKick(this.authentication, this);
    }
}
