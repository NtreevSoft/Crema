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
    public class CopyDataBaseViewModel : ModalDialogAppBase
    {
        private readonly IDataBase dataBase;
        private readonly Authentication authentication;
        private readonly string sourceDataBaseName;
        private string dataBaseName;
        private string comment;

        private CopyDataBaseViewModel(Authentication authentication, IDataBase dataBase)
        {
            this.authentication = authentication;
            this.dataBase = dataBase;
            this.dataBase.Dispatcher.VerifyAccess();
            this.sourceDataBaseName = this.dataBase.Name;
            this.DisplayName = Resources.Title_CopyDataBase;
        }

        public static Task<CopyDataBaseViewModel> CreateInstanceAsync(Authentication authentication, IDataBaseDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is IDataBase dataBase)
            {
                return dataBase.Dispatcher.InvokeAsync(() =>
                {
                    return new CopyDataBaseViewModel(authentication, dataBase);
                });
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        public async void Copy()
        {
            try
            {
                this.BeginProgress(Resources.Message_CopingDataBase);
                await this.dataBase.Dispatcher.InvokeAsync(() => this.dataBase.Copy(this.authentication, this.DataBaseName, this.Comment, false));
                this.EndProgress();
                this.TryClose(true);
                AppMessageBox.Show(Resources.Message_CopiedDataBase);
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
                this.NotifyOfPropertyChange(nameof(this.CanCopy));
            }
        }

        public string SourceDataBaseName
        {
            get { return this.sourceDataBaseName; }
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
                this.NotifyOfPropertyChange(nameof(this.CanCopy));
            }
        }

        public bool CanCopy
        {
            get
            {
                if (this.IsProgressing == true)
                    return false;
                if (this.Comment == string.Empty)
                    return false;
                if (this.DataBaseName == string.Empty)
                    return false;
                return NameValidator.VerifyName(this.DataBaseName);
            }
        }

        protected override void OnProgress()
        {
            base.OnProgress();
            this.NotifyOfPropertyChange(nameof(this.CanCopy));
        }
    }
}
