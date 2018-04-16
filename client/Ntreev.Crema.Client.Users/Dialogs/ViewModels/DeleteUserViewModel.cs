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
using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using System.Windows;
using Ntreev.Crema.Services;
using System.Threading.Tasks;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using Ntreev.Crema.Client.Users.Properties;

namespace Ntreev.Crema.Client.Users.Dialogs.ViewModels
{
    public class DeleteUserViewModel : DeleteAsyncViewModel
    {
        private readonly Authentication authentication;
        private readonly IUser user;

        private DeleteUserViewModel(Authentication authentication, IUser user)
        {
            this.authentication = authentication;
            this.authentication.Expired += Authentication_Expired;
            this.user = user;
            this.user.Dispatcher.VerifyAccess();
            this.DisplayName = Resources.Title_DeleteUser;
        }

        public static Task<DeleteUserViewModel> CreateInstanceAsync(Authentication authentication, IUserDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is IUser user)
            {
                return user.Dispatcher.InvokeAsync(() =>
                {
                    return new DeleteUserViewModel(authentication, user);
                });
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        protected override Task DeleteAsync()
        {
            return this.user.Dispatcher.InvokeAsync(() => this.user.Delete(this.authentication));
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close == true)
            {
                this.authentication.Expired -= Authentication_Expired;
            }
        }

        private void Authentication_Expired(object sender, EventArgs e)
        {
            this.Dispatcher.InvokeAsync(() => this.TryClose());
        }
    }
}
