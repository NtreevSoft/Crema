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
using Ntreev.Crema.Client.Users.Properties;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Users
{
    public static class UserUtility
    {
        public static bool CanRename(Authentication authentication, IUserDescriptor descriptor)
        {
            return false;
        }

        public static bool CanMove(Authentication authentication, IUserDescriptor descriptor)
        {
            return authentication.Authority == Authority.Admin;
        }

        public static bool CanDelete(Authentication authentication, IUserDescriptor descriptor)
        {
            if (authentication.ID == descriptor.UserInfo.ID)
                return false;
            return authentication.Authority == Authority.Admin;
        }

        public static bool CanSendMessage(Authentication authentication, IUserDescriptor descriptor)
        {
            if (authentication.ID == descriptor.UserInfo.ID)
                return false;
            return UserDescriptorUtility.IsOnline(authentication, descriptor);
        }

        public static bool CanChange(Authentication authentication, IUserDescriptor descriptor)
        {
            return authentication.Authority == Authority.Admin;
        }

        public static bool CanKick(Authentication authentication, IUserDescriptor descriptor)
        {
            if (authentication.ID == descriptor.UserInfo.ID)
                return false;
            if (authentication.Authority != Authority.Admin)
                return false;
            return UserDescriptorUtility.IsOnline(authentication, descriptor) == true;
        }

        public static bool CanUserAuthenticationKick(Authentication authentication, IUserAuthenticationDescriptor descriptor)
        {
            var canKick = descriptor.Target.User.Dispatcher.Invoke(() =>
            {
                if (authentication.ID == descriptor.UserInfo.ID && authentication.Token == descriptor.Target.Authentication.Token)
                    return false;
                if (authentication.ID == descriptor.UserInfo.ID)
                    return true;
                if (authentication.Authority == Authority.Admin)
                    return true;
                return false;
            });
            return canKick;
        }

        public static bool CanBan(Authentication authentication, IUserDescriptor descriptor)
        {
            if (authentication.ID == descriptor.UserInfo.ID)
                return false;
            if (authentication.Authority != Authority.Admin)
                return false;
            return UserDescriptorUtility.IsBanned(authentication, descriptor) == false;
        }

        public static bool CanUnban(Authentication authentication, IUserDescriptor descriptor)
        {
            if (authentication.ID == descriptor.UserInfo.ID)
                return false;
            if (authentication.Authority != Authority.Admin)
                return false;
            return UserDescriptorUtility.IsBanned(authentication, descriptor) == true;
        }

        public static async Task<bool> RenameAsync(Authentication authentication, IUserDescriptor descriptor)
        {
            return await Task.Run(() => false);
        }

        public static async Task<bool> MoveAsync(Authentication authentication, IUserDescriptor descriptor)
        {
            var dialog = await MoveUserViewModel.CreateInstanceAsync(authentication, descriptor);
            return dialog?.ShowDialog() == true;
        }

        public static async Task<bool> DeleteAsync(Authentication authentication, IUserDescriptor descriptor)
        {
            var dialog = await DeleteUserViewModel.CreateInstanceAsync(authentication, descriptor);
            return dialog?.ShowDialog() == true;
        }

        public static async Task<bool> SendMessageAsync(Authentication authentication, IUserDescriptor descriptor)
        {
            if (descriptor.Target is IUser user)
            {
                var dialog = await SendMessageViewModel.CreateInstanceAsync(authentication, descriptor);
                return dialog?.ShowDialog() == true;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static async Task<bool> ChangeAsync(Authentication authentication, IUserDescriptor descriptor)
        {
            var dialog = await ChangeUserViewModel.CreateInstanceAsync(authentication, descriptor);
            return dialog?.ShowDialog() == true;
        }

        public static async Task<bool> KickAsync(Authentication authentication, IUserDescriptor descriptor)
        {
            var dialog = await KickViewModel.CreateInstanceAsync(authentication, descriptor);
            return dialog?.ShowDialog() == true;
        }

        public static async Task<bool> KickUserAuthenticationAsync(Authentication authentication, IUserAuthenticationDescriptor descriptor)
        {
            var equalsUserID = descriptor.User.Dispatcher.Invoke(() => authentication.ID == descriptor.UserInfo.ID);
            if (equalsUserID)
            {
                return descriptor.User.Dispatcher.Invoke(() =>
                {
                    descriptor.User.KickAuthentication(authentication, descriptor.Target.Authentication.Token, Resources.Message_KickedByYourself);
                    return true;
                });
            }

            var dialog = await KickUserAuthenticationViewModel.CreateInstanceAsync(authentication, descriptor);
            return dialog?.ShowDialog() == true;
        }

        public static async Task<bool> BanAsync(Authentication authentication, IUserDescriptor descriptor)
        {
            var dialog = await BanViewModel.CreateInstanceAsync(authentication, descriptor);
            return dialog?.ShowDialog() == true;
        }

        public static async Task<bool> UnbanAsync(Authentication authentication, IUserDescriptor descriptor)
        {
            if (descriptor.Target is IUser user)
            {
                try
                {
                    await user.Dispatcher.InvokeAsync(() => user.Unban(authentication));
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
    }
}
