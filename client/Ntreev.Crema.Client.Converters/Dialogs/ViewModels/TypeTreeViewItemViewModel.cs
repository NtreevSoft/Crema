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
using Ntreev.Crema.Data;
using System;
using System.Threading.Tasks;
using Ntreev.Crema.Client.Framework;

namespace Ntreev.Crema.Client.Converters.Dialogs.ViewModels
{
    class TypeTreeViewItemViewModel : ExportTreeViewItemViewModel
    {
        private readonly Authentication authentication;
        private readonly TypeDescriptor descriptor;
        private TypeDescriptor parent;

        public TypeTreeViewItemViewModel(Authentication authentication, TypeDescriptor descriptor)
        {
            this.authentication = authentication;
            this.descriptor = descriptor;
        }

        public override Task PreviewAsync(CremaDataSet dataSet)
        {
            throw new NotSupportedException();

        }

        public override bool IsThreeState => false;

        public override bool DependsOnChilds => false;

        public override bool DependsOnParent => this.parent != null;

        public override string DisplayName => this.TypeName;

        public string Name => this.descriptor.Name;

        public string TypeName => this.descriptor.TypeName;

        public override string Path => this.descriptor.Path;

        public TypeInfo TypeInfo => this.descriptor.TypeInfo;
    }
}
