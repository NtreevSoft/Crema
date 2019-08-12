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
using Ntreev.Crema.Client.Users.Properties;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Users.Dialogs.ViewModels
{
    public class KickUserAuthenticationViewModel : KickViewModel
    {
        private readonly Authentication authentication;
        private readonly IUser user;
        private readonly Guid token;

        private KickUserAuthenticationViewModel(Authentication authentication, IUser user, Guid token) : base(authentication, user)
        {
            this.authentication = authentication;
            this.user = user;
            this.token = token;
        }

        public static Task<KickUserAuthenticationViewModel> CreateInstanceAsync(Authentication authentication, IUserAuthenticationDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is IUserAuthentication userAuthentication)
            {
                return userAuthentication.User.Dispatcher.InvokeAsync(() =>
                {
                    return new KickUserAuthenticationViewModel(authentication, userAuthentication.User, descriptor.Target.Authentication.Token);
                });
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        public override async void Kick()
        {
            try
            {
                this.BeginProgress();
                await this.user.Dispatcher.InvokeAsync(() => this.user.KickAuthentication(this.authentication, this.token, this.Comment));
                this.EndProgress();
                this.TryClose(true);
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
            }
        }
    }
}
