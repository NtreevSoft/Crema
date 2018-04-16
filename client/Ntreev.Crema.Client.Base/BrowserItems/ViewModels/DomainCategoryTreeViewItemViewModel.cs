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

using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Base.BrowserItems.ViewModels
{
    public class DomainCategoryTreeViewItemViewModel : DomainCategoryTreeItemBase
    {
        public DomainCategoryTreeViewItemViewModel(Authentication authentication, IDomainCategory category, object owner)
            : base(authentication, category, owner)
        {
            this.IsExpanded = true;
        }

        internal DomainCategoryTreeViewItemViewModel(Authentication authentication, DomainCategoryDescriptor descriptor, object owner)
            : base(authentication, descriptor, owner)
        {
            this.IsExpanded = true;
            this.RefreshVisible(false);
            this.Items.CollectionChanged += Items_CollectionChanged;
        }

        protected override DomainCategoryTreeItemBase CreateInstance(Authentication authentication, DomainCategoryDescriptor descriptor, object owner)
        {
            return new DomainCategoryTreeViewItemViewModel(authentication, descriptor, owner);
        }

        protected override DomainTreeItemBase CreateInstance(Authentication authentication, DomainDescriptor descriptor, object owner)
        {
            return new DomainTreeViewItemViewModel(authentication, descriptor, owner);
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RefreshVisible(true);
        }

        private void RefreshVisible(bool isRecursive)
        {
            this.IsVisible = this.Items.Any(item => item.IsVisible);

            if (this.Parent is DomainCategoryTreeViewItemViewModel parent)
            {
                parent.RefreshVisible(isRecursive);
            }
        }
    }
}
