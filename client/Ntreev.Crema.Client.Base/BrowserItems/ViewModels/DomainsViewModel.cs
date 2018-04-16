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

using Ntreev.Crema.Client.Base.Properties;
using Ntreev.Crema.Client.Base.Services.ViewModels;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Base.BrowserItems.ViewModels
{
    [Export(typeof(IBrowserItem))]
    [RequiredAuthority(Authority.Guest)]
    [ParentType(typeof(IBrowserService))]
    class DomainsViewModel : TreeViewBase, IBrowserItem
    {
        private readonly ICremaAppHost cremaAppHost;
        private DomainCategoryTreeViewItemViewModel root;

        [Import]
        private ICompositionService compositionService = null;
        [Import]
        private Authenticator authenticator = null;
        [Import]
        private IPropertyService propertyService = null;

        [ImportingConstructor]
        public DomainsViewModel(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Opened += CremaAppHost_Opened;
            this.cremaAppHost.Closed += CremaAppHost_Closed;
            this.DisplayName = Resources.Title_DomainList;
            this.Dispatcher.InvokeAsync(() => this.AttachPropertyService(this.propertyService));
        }

        public bool IsVisible => true;

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);
            this.Dispatcher.InvokeAsync(() => this.propertyService.SelectedObject = this.SelectedItem);
        }

        private async void CremaAppHost_Opened(object sender, EventArgs e)
        {
            if (this.cremaAppHost.GetService(typeof(IDomainContext)) is IDomainContext domainContext)
            {
                this.root = await domainContext.Dispatcher.InvokeAsync(() =>
                {
                    return new DomainCategoryTreeViewItemViewModel(this.authenticator, domainContext.Root, this);
                });

                await this.Dispatcher.InvokeAsync(() =>
                {
                    this.compositionService.SatisfyImportsOnce(this.root);
                    foreach (var item in this.root.Items)
                    {
                        this.Items.Add(item);
                    }
                    this.root.Items.CollectionChanged += ItemsSource_CollectionChanged;
                });
            }
        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is TreeViewItemViewModel viewModel)
                            {
                                this.Items.Add(viewModel);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (item is TreeViewItemViewModel viewModel)
                            {
                                this.Items.Remove(viewModel);
                            }
                        }
                    }
                    break;
            }
        }

        private void CremaAppHost_Closed(object sender, EventArgs e)
        {
            this.Items.Clear();
        }
    }
}
