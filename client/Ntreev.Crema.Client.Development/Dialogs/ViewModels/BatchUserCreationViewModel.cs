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

using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.Random;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace Ntreev.Crema.Client.Development.Dialogs.ViewModels
{
    public class BatchUserCreationViewModel : ModalDialogBase
    {
        private readonly IUserCategory category;
        private readonly Authenticator authenticator;
        private int tryCount;

        public BatchUserCreationViewModel(IUserCategory category)
        {
            this.category = category;
            this.authenticator = category.GetService(typeof(Authenticator)) as Authenticator;
            this.tryCount = 10;
        }

        public int TryCount
        {
            get { return this.tryCount; }
            set
            {
                this.tryCount = value;
                this.NotifyOfPropertyChange(nameof(this.TryCount));
                this.NotifyOfPropertyChange(nameof(this.CanCreate));
            }
        }

        public async void Create()
        {
            try
            {
                this.BeginProgress();
                await this.category.Dispatcher.InvokeAsync(() =>
                {
                    for (var i = 0; i < this.TryCount; i++)
                    {
                        this.CreateUser(RandomUtility.NextEnum<Authority>());
                    }
                });

                this.TryClose(true);
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
            finally
            {
                this.EndProgress();
            }
        }

        public bool CanCreate
        {
            get
            {
                return this.tryCount > 0;
            }
        }

        private void CreateUser(Authority authority)
        {
            var identifier = RandomUtility.NextIdentifier();

            if (authority == Authority.Admin)
            {
                var newID = string.Format("Admin_{0}", identifier);
                var newName = string.Format("관리자_{0}", identifier);

                this.category.AddNewUser(this.authenticator, newID, StringToSecureString("admin"), newName, Authority.Admin);
            }
            else if (authority == Authority.Member)
            {
                var newID = string.Format("Member_{0}", identifier);
                var newName = string.Format("구성원_{0}", identifier);

                this.category.AddNewUser(this.authenticator, newID, StringToSecureString("member"), newName, Authority.Member);
            }
            else
            {
                var newID = string.Format("Guest_{0}", identifier);
                var newName = string.Format("손님_{0}", identifier);

                this.category.AddNewUser(this.authenticator, newID, StringToSecureString("guest"), newName, Authority.Guest);
            }
        }

        public static SecureString StringToSecureString(string value)
        {
            var secureString = new SecureString();
            foreach (var item in value)
            {
                secureString.AppendChar(item);
            }
            return secureString;
        }
    }
}
