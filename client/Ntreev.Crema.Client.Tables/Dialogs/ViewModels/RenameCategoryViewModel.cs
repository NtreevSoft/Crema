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
using Ntreev.Crema.Client.Tables.Properties;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using System;
using System.Threading.Tasks;
using Ntreev.Library.ObjectModel;
using System.Windows.Threading;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;

namespace Ntreev.Crema.Client.Tables.Dialogs.ViewModels
{
    public class RenameCategoryViewModel : RenameAsyncAppViewModel
    {
        private readonly Authentication authentication;
        private readonly ITableCategory category;

        private RenameCategoryViewModel(Authentication authentication, ITableCategory category)
            : base(category.Name)
        {
            this.authentication = authentication;
            this.category = category;
            this.category.Dispatcher.VerifyAccess();
            this.DisplayName = Resources.Title_RenameTableFolder;
        }

        public static Task<RenameCategoryViewModel> CreateInstanceAsync(Authentication authentication, ITableCategoryDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is ITableCategory category)
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

        protected async override void VerifyRename(string newName, Action<bool> isVerify)
        {
            var result = await this.category.Dispatcher.InvokeAsync(() =>
            {
                if (string.IsNullOrWhiteSpace(newName))
                    return false;

                if (newName.Contains(' '))
                    return false;

                var tableNames = this.category.Parent.Tables.Select(item => item.Name).ToArray();
                if (tableNames.Contains(newName, StringComparer.OrdinalIgnoreCase) == true)
                    return false;

                var path = this.GeneratePath(newName);
                var tableContext = this.category.GetService(typeof(ITableContext)) as ITableContext;
                var category = tableContext.Categories[path];
                return category == null || category == this.category;
            });

            isVerify(result);
        }

        protected override Task RenameAsync(string newName)
        {
            return this.category.Dispatcher.InvokeAsync(() => this.category.Rename(this.authentication, newName));
        }

        private string GeneratePath(string newName)
        {
            var categoryName = new CategoryName(this.category.Parent.Path, newName);
            return categoryName.Path;
        }
    }
}
