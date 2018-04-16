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
using Ntreev.Crema.Client.Users.Properties;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using System;
using System.Threading.Tasks;
using Ntreev.Library.ObjectModel;
using System.Windows.Threading;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework;

namespace Ntreev.Crema.Client.Users.Dialogs.ViewModels
{
    public class RenameCategoryViewModel : RenameAsyncViewModel
    {
        private readonly Authentication authentication;
        private readonly IUserCategory category;
        private readonly IUserContext userContext;

        private RenameCategoryViewModel(Authentication authentication, IUserCategory category)
            : base(category.Name)
        {
            this.authentication = authentication;
            this.authentication.Expired += Authentication_Expired;
            this.category = category;
            this.category.Dispatcher.VerifyAccess();
            this.userContext = category.GetService(typeof(IUserContext)) as IUserContext;
            this.DisplayName = Resources.Title_RenameUserFolder;
        }

        public static Task<RenameCategoryViewModel> CreateInstanceAsync(Authentication authentication, IUserCategoryDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is IUserCategory category)
            {
                return category.Dispatcher.InvokeAsync(() =>
                {
                    return new RenameCategoryViewModel(authentication, category);
                });
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        protected async override void VerifyRename(string newName, Action<bool> isValid)
        {
            var result = await this.category.Dispatcher.InvokeAsync(() =>
            {
                var path = this.GeneratePath(newName);
                var category = this.userContext.Categories[path];
                return category == null || category == this.category;
            });
            isValid(result);
        }

        protected override Task RenameAsync(string newName)
        {
            return this.category.Dispatcher.InvokeAsync(() => this.category.Rename(this.authentication, newName));
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

        private string GeneratePath(string newName)
        {
            var categoryName = new CategoryName(this.category.Parent.Path, newName);
            return categoryName.Path;
        }
    }
}
