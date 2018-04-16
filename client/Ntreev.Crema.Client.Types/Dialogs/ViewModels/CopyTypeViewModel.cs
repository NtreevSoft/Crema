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

using System.Linq;
using Ntreev.Crema.Client.Types.Properties;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Crema.Client.Framework;
using System.Threading.Tasks;
using System;
using Ntreev.ModernUI.Framework;
using Ntreev.Library.ObjectModel;

namespace Ntreev.Crema.Client.Types.Dialogs.ViewModels
{
    public class CopyTypeViewModel : ModalDialogBase
    {
        private readonly Authentication authentication;
        private readonly IType type;
        private readonly ITypeCollection types;
        private readonly ITypeCategoryCollection categories;
        private bool isValid;

        private string[] categoryPaths;
        private string categoryPath;
        private string typeName;
        private string newName;

        private CopyTypeViewModel(Authentication authentication, IType type)
        {
            this.authentication = authentication;
            this.type = type;
            this.type.Dispatcher.VerifyAccess();
            this.types = type.GetService(typeof(ITypeCollection)) as ITypeCollection;
            this.categories = type.GetService(typeof(ITypeCategoryCollection)) as ITypeCategoryCollection;
            this.categoryPaths = this.categories.Select(item => item.Path).ToArray();
            this.categoryPath = this.type.Category.Path;
            this.typeName = type.Name;
            this.NewName = type.Name;
            this.DisplayName = Resources.Title_CopyType;
        }

        public static Task<CopyTypeViewModel> CreateInstanceAsync(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is IType type)
            {
                return type.Dispatcher.InvokeAsync(() =>
                {
                    return new CopyTypeViewModel(authentication, type);
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
                this.BeginProgress(Resources.Message_CopingType);
                await this.type.Dispatcher.InvokeAsync(() => this.type.Copy(this.authentication, this.NewName, this.CategoryPath));
                this.EndProgress();
                this.TryClose(true);
                AppMessageBox.Show(Resources.Message_TypeCopied);
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
            }
        }

        public string[] CategoryPaths
        {
            get { return this.categoryPaths; }
        }

        public string NewName
        {
            get { return this.newName ?? string.Empty; }
            set
            {
                this.newName = value;
                this.NotifyOfPropertyChange(nameof(this.NewName));
                this.VerifyCopy(this.VerifyAction);
            }
        }

        public string CategoryPath
        {
            get { return this.categoryPath ?? string.Empty; }
            set
            {
                this.categoryPath = value;
                this.NotifyOfPropertyChange(nameof(this.CategoryPath));
                this.VerifyCopy(this.VerifyAction);
            }
        }

        public string TypeName
        {
            get { return this.typeName; }
        }

        public bool CanCopy
        {
            get
            {
                if (this.IsProgressing == true)
                    return false;

                if (this.CategoryPath == string.Empty)
                    return false;

                if (NameValidator.VerifyName(this.NewName) == false)
                    return false;

                return this.isValid;
            }
        }

        private async void VerifyCopy(Action<bool> isValid)
        {
            var result = await this.type.Dispatcher.InvokeAsync(() =>
            {
                if (this.types.Contains(this.NewName) == true)
                    return false;

                return this.categories.Contains(this.CategoryPath) == true;
            });
            isValid(result);
        }

        protected override void OnProgress()
        {
            base.OnProgress();
            this.NotifyOfPropertyChange(nameof(this.CanCopy));
        }

        private void VerifyAction(bool isValid)
        {
            this.isValid = isValid;
            this.NotifyOfPropertyChange(nameof(this.CanCopy));
        }
    }
}
