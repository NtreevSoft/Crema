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

using Ntreev.Crema.Client.Types.Properties;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ntreev.Crema.Client.Framework;
using Ntreev.Library.ObjectModel;
using Ntreev.ModernUI.Framework;
using System.Windows.Threading;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;

namespace Ntreev.Crema.Client.Types.Dialogs.ViewModels
{
    public class NewTypeCategoryViewModel : NewCategoryAsyncViewModel
    {
        private readonly Authentication authentication;
        private readonly ITypeCategory category;

        private NewTypeCategoryViewModel(Authentication authentication, ITypeCategory category)
            : base(category.Path)
        {
            this.authentication = authentication;
            this.category = category;
            this.category.Dispatcher.VerifyAccess();
            this.DisplayName = Resources.Title_NewTypeFolder;
        }

        public static Task<NewTypeCategoryViewModel> CreateInstanceAsync(Authentication authentication, ITypeCategoryDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is ITypeCategory category)
            {
                return category.Dispatcher.InvokeAsync(() =>
                {
                    return new NewTypeCategoryViewModel(authentication, category);
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
                if (string.IsNullOrWhiteSpace(categoryName))
                    return false;

                if (categoryName.Contains(' '))
                    return false;

                if (this.category.Types.ContainsKey(categoryName) == true)
                    return false;

                return this.category.Categories.ContainsKey(categoryName) == false;
            });
            isValid(result);
        }

        protected override Task CreateAsync(string categoryName)
        {
            return this.category.Dispatcher.InvokeAsync(() => this.category.AddNewCategory(this.authentication, categoryName));
        }
    }
}
