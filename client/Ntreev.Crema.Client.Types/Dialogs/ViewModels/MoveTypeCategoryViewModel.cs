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
using System.Collections.Generic;
using System.Collections;
using System;
using System.Threading.Tasks;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;

namespace Ntreev.Crema.Client.Types.Dialogs.ViewModels
{
    public class MoveTypeCategoryViewModel : MoveAsyncAppViewModel
    {
        private readonly Authentication authentication;
        private readonly ITypeCategory category;

        private MoveTypeCategoryViewModel(Authentication authentication, ITypeCategory category, string[] targetPaths)
            : base(category.Path, targetPaths)
        {
            this.authentication = authentication;
            this.category = category;
            this.category.Dispatcher.VerifyAccess();
            this.DisplayName = Resources.Title_MoveTypeFolder;
        }

        public static Task<MoveTypeCategoryViewModel> CreateInstanceAsync(Authentication authentication, ITypeCategoryDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is ITypeCategory category)
            {
                return category.Dispatcher.InvokeAsync(() =>
                {
                    var categories = category.GetService(typeof(ITypeCategoryCollection)) as ITypeCategoryCollection;
                    var targetPaths = categories.Select(item => item.Path).ToArray();
                    return new MoveTypeCategoryViewModel(authentication, category, targetPaths);
                });
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        protected async override void VerifyMove(string targetPath, Action<bool> isVerify)
        {
            var result = await this.category.Dispatcher.InvokeAsync(() =>
            {
                var categories = this.category.GetService(typeof(ITypeCategoryCollection)) as ITypeCategoryCollection;
                var targetCategory = categories[targetPath];
                if (targetCategory == null)
                    return false;
                if (targetCategory.Categories.ContainsKey(this.category.Name) == true)
                    return false;
                return targetCategory.Types.ContainsKey(this.category.Name) == false;
            });
            isVerify(result);
        }

        protected override Task MoveAsync(string targetPath)
        {
            return this.category.Dispatcher.InvokeAsync(() => this.category.Move(this.authentication, targetPath));
        }
    }
}
