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
    public class SendMessageViewModel : ModalDialogAppBase
    {
        private readonly Authentication authentication;
        private readonly IUser user;
        private readonly string userID;
        private string message;

        public SendMessageViewModel(Authentication authentication, IUser user)
        {
            this.authentication = authentication;
            this.user = user;
            this.user.Dispatcher.VerifyAccess();
            this.userID = user.ID;
            this.DisplayName = Resources.Title_SendMessage;
        }

        public static Task<SendMessageViewModel> CreateInstanceAsync(Authentication authentication, IUserDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is IUser user)
            {
                return user.Dispatcher.InvokeAsync(() =>
                {
                    return new SendMessageViewModel(authentication, user);
                });
            }
            else
            {
                throw new ArgumentException("ServiceProvider does not provide IUserContext", nameof(descriptor));
            }
        }

        public string Message
        {
            get { return this.message ?? string.Empty; }
            set
            {
                this.message = value;
                this.NotifyOfPropertyChange(nameof(this.Message));
                this.NotifyOfPropertyChange(nameof(this.CanSend));
            }
        }

        public string UserID
        {
            get { return this.userID ?? string.Empty; }
        }

        public async void Send()
        {
            try
            {
                this.BeginProgress(Resources.Message_SendMessage);
                await this.user.Dispatcher.InvokeAsync(() =>
                {
                    user.SendMessage(this.authentication, this.Message);
                });
                this.EndProgress();
                this.TryClose(true);
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
            }
        }

        public bool CanSend
        {
            get
            {
                if (this.IsProgressing == true)
                    return false;
                if (this.Message == string.Empty)
                    return false;
                return true;
            }
        }
    }
}
