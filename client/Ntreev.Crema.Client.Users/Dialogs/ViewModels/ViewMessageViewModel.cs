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
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Users.Dialogs.ViewModels
{
    public class ViewMessageViewModel : ModalDialogAppBase
    {
        private readonly Authentication authentication;
        private readonly IUserContext userContext;
        private string message;
        private string sendUserID;
        [Import]
        private IFlashService flashService = null;

        private ViewMessageViewModel(Authentication authentication, IUserContext userContext, string message, string sendUserID)
        {
            this.authentication = authentication;
            this.userContext = userContext;
            this.userContext.Dispatcher.VerifyAccess();
            this.message = message;
            this.sendUserID = sendUserID;
            this.DisplayName = Resources.Title_ViewMessage;
        }

        public static Task<ViewMessageViewModel> CreateInstanceAsync(Authentication authentication, IServiceProvider serviceProvider, string message, string sendUserID)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            if (serviceProvider.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                return userContext.Dispatcher.InvokeAsync(() =>
                {
                    return new ViewMessageViewModel(authentication, userContext, message, sendUserID);
                });
            }
            else
            {
                throw new ArgumentException("ServiceProvider does not provide IUserContext", nameof(serviceProvider));
            }
        }

        public string Message
        {
            get { return this.message ?? string.Empty; }
            set
            {
                this.message = value;
                this.NotifyOfPropertyChange(nameof(this.Message));
            }
        }

        public string SendUserID
        {
            get { return this.sendUserID ?? string.Empty; }
            set
            {
                this.sendUserID = value;
                this.NotifyOfPropertyChange(nameof(this.SendUserID));
            }
        }

        public async void Reply()
        {
            this.TryClose();

            var dialog = await this.userContext.Dispatcher.InvokeAsync(() =>
            {
                var user = this.userContext.Users[this.sendUserID];
                return new SendMessageViewModel(this.authentication, user);
            });
            dialog?.ShowDialog();
        }

        public bool CanReply
        {
            get
            {
                return this.SendUserID != string.Empty;
            }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.flashService?.Flash();
        }
    }
}
