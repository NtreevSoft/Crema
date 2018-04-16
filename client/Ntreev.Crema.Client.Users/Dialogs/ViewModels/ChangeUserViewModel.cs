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

using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security;
using Ntreev.Crema.Client.Users.Properties;

namespace Ntreev.Crema.Client.Users.Dialogs.ViewModels
{
    public class ChangeUserViewModel : ModalDialogAppBase
    {
        private readonly Authentication authentication;
        private readonly IUser user;
        private string userID;
        private SecureString password;
        private string userName;
        private Authority authority;

        private string oldUserName;
        private Authority oldAuthority;

        private ChangeUserViewModel(Authentication authentication, IUser user)
        {
            this.authentication = authentication;
            this.user = user;
            this.user.Dispatcher.VerifyAccess();
            this.userID = user.ID;
            this.userName = this.oldUserName = user.UserName;
            this.authority = this.oldAuthority = user.Authority;
            this.DisplayName = Resources.Title_ChangeUserInfo;
        }

        public static Task<ChangeUserViewModel> CreateInstanceAsync(Authentication authentication, IUserDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is IUser user)
            {
                return user.Dispatcher.InvokeAsync(() =>
                {
                    return new ChangeUserViewModel(authentication, user);
                });
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        public string ID
        {
            get { return this.userID ?? string.Empty; }
            set
            {
                this.userID = value;

                this.NotifyOfPropertyChange(nameof(this.ID));
                this.NotifyOfPropertyChange(nameof(this.CanChange));
            }
        }

        public SecureString Password
        {
            get { return this.password; }
            set
            {
                this.password = value;

                this.NotifyOfPropertyChange(nameof(this.Password));
                this.NotifyOfPropertyChange(nameof(this.CanChange));
            }
        }
             
        public string UserName
        {
            get { return this.userName ?? string.Empty; }
            set
            {
                this.userName = value;

                this.NotifyOfPropertyChange(nameof(this.UserName));
                this.NotifyOfPropertyChange(nameof(this.CanChange));
            }
        }

        public Authority Authority
        {
            get { return this.authority; }
            set
            {
                this.authority = value;

                this.NotifyOfPropertyChange(nameof(this.Authority));
                this.NotifyOfPropertyChange(nameof(this.CanChange));
            }
        }

        public IEnumerable<Authority> Authorities
        {
            get
            {
                return Enum.GetValues(typeof(Authority)).Cast<Authority>();
            }
        }

        public async void Change()
        {
            try
            {
                this.BeginProgress(Resources.Message_Change);
                var authority = this.oldAuthority == this.authority ? null : (Authority?)this.authority;
                var userName = this.oldUserName == this.userName ? null : this.userName;
                var password = this.Password;
                await this.user.Dispatcher.InvokeAsync(() => this.user.ChangeUserInfo(this.authentication, password, password, userName, authority));
                this.EndProgress();
                this.TryClose(true);
                AppMessageBox.Show(Resources.Message_ChangeComplete);
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
            }
        }

        public bool CanChange
        {
            get
            {
                if (this.ID == string.Empty)
                    return false;
                if (this.UserName == string.Empty)
                    return false;
                if (this.UserName == this.oldUserName && this.Authority == this.oldAuthority && this.Password == null)
                    return false;
                return true;
            }
        }
    }
}
