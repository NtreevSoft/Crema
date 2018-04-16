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

using Ntreev.Crema.Client.Differences.BrowserItems.ViewModels;
using Ntreev.Crema.Client.Differences.Properties;
using Ntreev.Crema.Client.Framework;
using Ntreev.Library;
using Ntreev.Library.Linq;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Differences.PropertyItems.ViewModels
{
    [Export(typeof(IPropertyItem))]
    [ParentType(typeof(PropertyService))]
    [Order(-1)]
    class TemplateUnresolvedItemsViewModel : PropertyItemBase
    {
        [Import]
        private ICompositionService compositionService = null;

        private TemplateTreeViewItemViewModel viewModel;
        private ObservableCollection<TemplateUnresolvedItemListBoxItemViewModel> itemList = new ObservableCollection<TemplateUnresolvedItemListBoxItemViewModel>();
        private TemplateUnresolvedItemListBoxItemViewModel selectedItem;

        public TemplateUnresolvedItemsViewModel()
        {
            this.DisplayName = Resources.Title_UnresolvedItems;
        }

        public override bool IsVisible => this.itemList.Any();

        public override object SelectedObject => this.viewModel;

        public override bool CanSupport(object obj)
        {
            return obj is TemplateTreeViewItemViewModel;
        }

        public ObservableCollection<TemplateUnresolvedItemListBoxItemViewModel> Items
        {
            get { return this.itemList; }
        }

        public override void SelectObject(object obj)
        {
            if (obj is TemplateTreeViewItemViewModel viewModel)
            {
                var query = from viewModelItem in this.GetViewModels(viewModel)
                            join unresolvedItem in viewModel.Source.UnresolvedItems on viewModelItem.Target equals unresolvedItem
                            select viewModelItem;

                this.itemList.Clear();
                foreach (var item in query)
                {
                    var itemViewModel = new TemplateUnresolvedItemListBoxItemViewModel(item);
                    this.itemList.Add(itemViewModel);
                    itemViewModel.PropertyChanged += ItemViewModel_PropertyChanged;
                }

                foreach (var item in itemList)
                {
                    this.compositionService.SatisfyImportsOnce(item);
                }

                this.viewModel = viewModel;
                this.selectedItem = this.itemList.FirstOrDefault();
            }
            else
            {
                this.itemList.Clear();
                this.viewModel = null;
            }
            this.NotifyOfPropertyChange(nameof(this.DisplayName));
            this.NotifyOfPropertyChange(nameof(this.IsVisible));
            this.NotifyOfPropertyChange(nameof(this.Items));
        }

        private void ItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is TemplateUnresolvedItemListBoxItemViewModel viewModel && e.PropertyName == nameof(TemplateUnresolvedItemListBoxItemViewModel.IsResolved))
            {
                if (viewModel.IsResolved == true)
                {
                    this.itemList.Remove(viewModel);
                    this.NotifyOfPropertyChange(nameof(this.IsVisible));
                }
            }
        }

        private IEnumerable<TreeViewItemViewModel> GetViewModels(TemplateTreeViewItemViewModel viewModel)
        {
            foreach (var item in EnumerableUtility.FamilyTree(viewModel.Browser.Items, i => i.Items))
            {
                yield return item;
            }
        }
    }
}
