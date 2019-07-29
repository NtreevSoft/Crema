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
using Ntreev.Crema.Client.Users.Dialogs.ViewModels;
using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Client.Users.PropertyItems.ViewModels;

namespace Ntreev.Crema.Client.Users
{
    public static class DomainUserUtility
    {
        public static bool CanSendMessage(Authentication authentication, IDomainUserDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            if (DomainUserDescriptorUtility.IsOnline(authentication, descriptor) == false)
                return false;
            return authentication.ID != descriptor.DomainUserInfo.UserID;
        }

        public static bool CanSetOwner(Authentication authentication, IDomainUserDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            if (DomainUserDescriptorUtility.IsOwner(authentication, descriptor) && authentication.Token == descriptor.Token)
                return false;
            if (authentication.Authority == Authority.Admin)
                return true;
            if (descriptor.Owner is EditorsViewModel editorsViewModel)
            {
                var isOwnerByUserId = editorsViewModel.Users.OfType<DomainUserTreeItemBase>().Any(user => user.UserID == descriptor.UserID && user.IsOwner);
                if (isOwnerByUserId) return true;
            }
            if (DomainUserDescriptorUtility.IsOwner(authentication, descriptor) == false)
                return false;
            return false;
        }

        public static bool CanKick(Authentication authentication, IDomainUserDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            if (DomainUserDescriptorUtility.IsOwner(authentication, descriptor) == true)
                return false;
            if (authentication.Authority == Authority.Admin)
                return true;
            if (authentication.ID == descriptor.UserID && authentication.Token != descriptor.Token)
                return true;
            return false;
        }

        public static async Task<bool> SendMessageAsync(Authentication authentication, IDomainUserDescriptor descriptor)
        {
            if (descriptor.Target is IDomainUser domainUser)
            {
                if (domainUser.GetService(typeof(IUserContext)) is IUserContext userContext)
                {
                    var user = await userContext.Dispatcher.InvokeAsync(() => userContext.Users[descriptor.DomainUserInfo.UserID]);
                    var dialog = await user.Dispatcher.InvokeAsync(() => new SendMessageViewModel(authentication, user));
                    return dialog?.ShowDialog() == true;
                }
                return false;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static async Task<bool> SetOwnerAsync(Authentication authentication, IDomainUserDescriptor descriptor)
        {
            if (descriptor.Target is IDomainUser domainUser)
            {
                try
                {

                    await domainUser.Dispatcher.InvokeAsync(() => domainUser.SetOwner(authentication));
                    return true;
                }
                catch (Exception e)
                {
                    AppMessageBox.ShowError(e);
                    return false;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static async Task<bool> KickAsync(Authentication authentication, IDomainUserDescriptor descriptor)
        {
            var dialog = await KickEditorViewModel.CreateInstanceAsync(authentication, descriptor);
            return dialog?.ShowDialog() == true;
        }
    }
}
