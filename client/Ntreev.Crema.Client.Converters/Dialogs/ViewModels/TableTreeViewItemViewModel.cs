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
using Ntreev.Crema.Data;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Library;
using Ntreev.Crema.Client.Framework;

namespace Ntreev.Crema.Client.Converters.Dialogs.ViewModels
{
    class TableTreeViewItemViewModel : ExportTreeViewItemViewModel
    {
        private readonly Authentication authentication;
        private readonly TableDescriptor descriptor;
        private TableDescriptor parent;

        public TableTreeViewItemViewModel(Authentication authentication, TableDescriptor descriptor)
        {
            this.authentication = authentication;
            this.descriptor = descriptor;

            foreach (var item in descriptor.Childs)
            {
                var viewModel = new TableTreeViewItemViewModel(authentication, item)
                {
                    parent = descriptor,
                    Parent = this
                };
            }
        }

        public override async Task PreviewAsync(CremaDataSet dataSet)
        {
            var preview = await TableDescriptorUtility.GetDataAsync(authentication, this.descriptor, null);

            foreach (TableTreeViewItemViewModel item in this.Items)
            {
                if (item.IsChecked == true)
                    continue;

                var table = preview.Tables[item.TableName];
                preview.Tables.Remove(table);
            }

            foreach (var item in preview.Tables.Where(i => i.Parent == null))
            {
                item.CopyTo(dataSet);
            }
        }

        public override bool IsThreeState
        {
            get { return false; }
        }

        public override bool DependsOnChilds
        {
            get { return false; }
        }

        public override bool DependsOnParent
        {
            get { return this.parent != null; }
        }

        public override string DisplayName
        {
            get { return this.TableName; }
        }

        public string Name => this.descriptor.Name;

        public string TableName => this.descriptor.TableName;

        public override string Path => this.descriptor.Path;

        public TableInfo TableInfo => this.descriptor.TableInfo;

        public bool IsInherited => this.descriptor.IsInherited;

        public bool IsBaseTemplate => this.descriptor.IsBaseTemplate;
    }
}
