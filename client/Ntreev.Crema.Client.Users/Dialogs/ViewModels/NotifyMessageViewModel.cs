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
    public class NotifyMessageViewModel : ModalDialogAppBase
    {
        private readonly Authentication authentication;
        private readonly IUserContext userContext;
        private string message;
        private string[] userIDs;

        private NotifyMessageViewModel(Authentication authentication, IUserContext userContext, string[] userIDs)
        {
            this.authentication = authentication;
            this.userContext = userContext;
            this.userContext.Dispatcher.VerifyAccess();
            this.userIDs = userIDs;
            this.DisplayName = Resources.Title_NotifyMessage;
        }

        public static Task<NotifyMessageViewModel> CreateInstanceAsync(Authentication authentication, IServiceProvider serviceProvider)
        {
            return CreateInstanceAsync(authentication, serviceProvider, null);
        }

        public static Task<NotifyMessageViewModel> CreateInstanceAsync(Authentication authentication, IServiceProvider serviceProvider, string[] userIDs)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            if (serviceProvider.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                return userContext.Dispatcher.InvokeAsync(() =>
                {
                    return new NotifyMessageViewModel(authentication, userContext, userIDs);
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
                this.NotifyOfPropertyChange(nameof(this.CanNotify));
            }
        }

        public string[] TargetUserIDs
        {
            get { return this.userIDs; }
        }

        public string TargetUserID
        {
            get
            {
                return this.userIDs.Any() == false ? "All Users" : string.Join(",", this.userIDs);
            }
        }

        public async void Notify()
        {
            try
            {
                this.BeginProgress(Resources.Message_SendMessage);
                await this.userContext.Dispatcher.InvokeAsync(() => this.userContext.NotifyMessage(this.authentication, this.userIDs, this.Message));
                this.EndProgress();
                this.TryClose(true);
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
            }
        }

        public bool CanNotify
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
