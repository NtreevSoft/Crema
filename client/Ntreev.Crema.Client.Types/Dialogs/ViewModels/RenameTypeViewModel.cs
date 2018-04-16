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
using Ntreev.Crema.Client.Framework;
using System.Threading.Tasks;
using System;
using Ntreev.ModernUI.Framework;
using System.Windows.Threading;
using Ntreev.Library.ObjectModel;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;

namespace Ntreev.Crema.Client.Types.Dialogs.ViewModels
{
    public class RenameTypeViewModel : RenameAsyncAppViewModel
    {
        private readonly Authentication authentication;
        private readonly IType type;

        private RenameTypeViewModel(Authentication authentication, IType type)
            : base(type.Name)
        {
            this.authentication = authentication;
            this.type = type;
            this.type.Dispatcher.VerifyAccess();
            this.DisplayName = Resources.Title_RenameType;
        }

        public static Task<RenameTypeViewModel> CreateInstanceAsync(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is IType type)
            {
                return type.Dispatcher.InvokeAsync(() =>
                {
                    return new RenameTypeViewModel(authentication, type);
                });
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        protected async override void VerifyRename(string newName, Action<bool> isVerify)
        {
            var result = await this.type.Dispatcher.InvokeAsync(() =>
            {
                var types = this.type.GetService(typeof(ITypeCollection)) as ITypeCollection;
                return types.Contains(newName) == false;
            });
            isVerify(result);
        }

        protected override Task RenameAsync(string newName)
        {
            return this.type.Dispatcher.InvokeAsync(() => this.type.Rename(this.authentication, newName));
        }
    }
}
