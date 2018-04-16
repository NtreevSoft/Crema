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
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Client.Framework;
using Ntreev.ModernUI.Framework;
using System.ComponentModel;
using Ntreev.Crema.Data;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Converters.Dialogs.ViewModels
{
    class TableCategoryTreeViewItemViewModel : ExportTreeViewItemViewModel
    {
        private readonly Authentication authentication;
        private readonly TableCategoryDescriptor descriptor;

        public TableCategoryTreeViewItemViewModel(Authentication authentication, TableCategoryDescriptor descriptor)
        {
            this.authentication = authentication;
            this.descriptor = descriptor;

            foreach (var item in descriptor.Categories)
            {
                var viewModel = new TableCategoryTreeViewItemViewModel(authentication, item)
                {
                    Parent = this
                };
            }

            foreach (var item in descriptor.Tables)
            {
                var viewModel = new TableTreeViewItemViewModel(authentication, item)
                {
                    Parent = this
                };
            }
        }

        public override async Task PreviewAsync(CremaDataSet dataSet)
        {
            foreach (var item in this.Items.OfType<TableTreeViewItemViewModel>())
            {
                if (item.IsChecked == false)
                    continue;

                await item.PreviewAsync(dataSet);
            }
        }

        public override string DisplayName
        {
            get { return this.Name; }
        }

        public override bool CanCheck => this.descriptor.AccessType >= AccessType.Guest;

        public override string Path => this.descriptor.Path;

        public string Name => this.descriptor.Name;
    }
}
