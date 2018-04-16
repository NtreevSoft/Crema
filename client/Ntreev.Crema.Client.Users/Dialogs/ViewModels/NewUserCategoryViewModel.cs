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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using Ntreev.Crema.Client.Users.Properties;
using Ntreev.Crema.Client.Framework;

namespace Ntreev.Crema.Client.Users.Dialogs.ViewModels
{
    public class NewUserCategoryViewModel : NewCategoryAsyncViewModel
    {
        private readonly Authentication authentication;
        private readonly IUserCategory category;
        
        private NewUserCategoryViewModel(Authentication authentication, IUserCategory category)
            : base(category.Path)
        {
            this.authentication = authentication;
            this.authentication.Expired += Authentication_Expired;
            this.category = category;
            this.category.Dispatcher.VerifyAccess();
            this.DisplayName = Resources.Title_NewUserFolder;
        }

        public static Task<NewUserCategoryViewModel> CreateInstanceAsync(Authentication authentication, IUserCategoryDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is IUserCategory category)
            {
                return category.Dispatcher.InvokeAsync(() =>
                {
                    return new NewUserCategoryViewModel(authentication, category);
                });
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        protected async override void VerifyName(string categoryName, Action<bool> isValid)
        {
            var result = await this.category.Dispatcher.InvokeAsync(() =>
            {
                if (this.category.Users.ContainsKey(categoryName) == true)
                    return false;

                return this.category.Categories.ContainsKey(categoryName) == false;
            });
            isValid(result);
        }

        protected override Task CreateAsync(string categoryName)
        {
            return this.category.Dispatcher.InvokeAsync(() => this.category.AddNewCategory(this.authentication, categoryName));
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
