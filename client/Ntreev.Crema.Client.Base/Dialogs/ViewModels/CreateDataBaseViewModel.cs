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
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Client.Base.Properties;

namespace Ntreev.Crema.Client.Base.Dialogs.ViewModels
{
    public class CreateDataBaseViewModel : ModalDialogAppBase
    {
        private readonly ICremaHost cremaHost;
        private readonly Authentication authentication;
        private string dataBaseName;
        private string comment;

        private CreateDataBaseViewModel(Authentication authentication, ICremaHost cremaHost)
        {
            this.authentication = authentication;
            this.cremaHost = cremaHost;
            this.cremaHost.Dispatcher.VerifyAccess();
            this.DisplayName = Resources.Title_CreateDataBase;
        }

        public static Task<CreateDataBaseViewModel> CreateInstanceAsync(Authentication authentication, IServiceProvider serviceProvider)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            if (serviceProvider.GetService(typeof(ICremaHost)) is ICremaHost cremaHost)
            {
                return cremaHost.Dispatcher.InvokeAsync(() =>
                {
                    return new CreateDataBaseViewModel(authentication, cremaHost);
                });
            }
            else
            {
                throw new ArgumentException("ServiceProvider does not provide IUserContext", nameof(serviceProvider));
            }
        }

        public async void Create()
        {
            try
            {
                this.BeginProgress(Resources.Message_CreatingNewDataBase);
                await this.cremaHost.Dispatcher.InvokeAsync(() => this.cremaHost.DataBases.AddNewDataBase(this.authentication, this.DataBaseName, this.Comment));
                this.EndProgress();
                this.TryClose(true);
                AppMessageBox.Show(Resources.Message_CreatedNewDataBase);
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
                this.EndProgress();
            }
        }

        public string DataBaseName
        {
            get { return this.dataBaseName ?? string.Empty; }
            set
            {
                if (this.dataBaseName == value)
                    return;

                this.dataBaseName = value;
                this.NotifyOfPropertyChange(nameof(this.DataBaseName));
                this.NotifyOfPropertyChange(nameof(this.CanCreate));
            }
        }

        public string Comment
        {
            get { return this.comment ?? string.Empty; }
            set
            {
                if (this.comment == value)
                    return;

                this.comment = value;
                this.NotifyOfPropertyChange(nameof(this.Comment));
                this.NotifyOfPropertyChange(nameof(this.CanCreate));
            }
        }

        public bool CanCreate
        {
            get
            {
                if (this.Comment == string.Empty)
                    return false;
                if (this.DataBaseName == string.Empty)
                    return false;

                if (this.DataBaseName.Contains(' '))
                    return false;

                return NameValidator.VerifyName(this.DataBaseName);
            }
        }
    }
}
