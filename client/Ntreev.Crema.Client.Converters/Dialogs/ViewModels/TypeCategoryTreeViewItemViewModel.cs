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

using System;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Data;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Converters.Dialogs.ViewModels
{
    public class TypeCategoryTreeViewItemViewModel : ExportTreeViewItemViewModel
    {
        private readonly Authentication authentication;
        private readonly TypeCategoryDescriptor descriptor;

        public TypeCategoryTreeViewItemViewModel(Authentication authentication, TypeCategoryDescriptor descriptor)
        {
            this.authentication = authentication;
            this.descriptor = descriptor;

            foreach (var item in descriptor.Categories)
            {
                var viewModel = new TypeCategoryTreeViewItemViewModel(authentication, item)
                {
                    Parent = this
                };
            }

            foreach (var item in descriptor.Types)
            {
                var viewModel = new TypeTreeViewItemViewModel(authentication, item)
                {
                    Parent = this
                };
            }
        }

        public override Task PreviewAsync(CremaDataSet dataSet)
        {
            throw new NotSupportedException();
        }

        public override string DisplayName => this.Name;

        public override bool CanCheck => this.descriptor.AccessType >= AccessType.Guest;

        public override string Path => this.descriptor.Path;

        public string Name => this.descriptor.Name;
    }
}
