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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Ntreev.Crema.Services;
using System.Windows;
using System.Windows.Threading;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.SmartSet.Properties;
using Ntreev.Crema.Client.Tables.BrowserItems.ViewModels;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.SmartSet.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.ModernUI.Framework.ViewModels;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;

namespace Ntreev.Crema.Client.SmartSet.BrowserItems.ViewModels
{
    class SmartSetTreeViewItemViewModel : TreeViewItemViewModel
    {
        private readonly ISmartSet smartSet;
        private readonly SmartSetBrowserViewModel browser;
        private readonly Dictionary<object, TreeViewItemViewModel> items = new Dictionary<object, TreeViewItemViewModel>();

        public SmartSetTreeViewItemViewModel(ISmartSet smartSet, SmartSetBrowserViewModel browser)
        {
            this.smartSet = smartSet;
            this.browser = browser;
            this.Target = smartSet;

            foreach (var item in this.smartSet.Items)
            {
                this.AddViewModel(item);
            }

            if (this.smartSet.Items is INotifyCollectionChanged)
            {
                (this.smartSet.Items as INotifyCollectionChanged).CollectionChanged += Items_CollectionChanged;
            }
            this.smartSet.Renamed += SmartSet_Renamed;
        }

        public override int CompareTo(object obj)
        {
            if (object.ReferenceEquals(this, obj) == true)
                return 0;

            else if (obj is SmartSetCategoryTreeViewItemViewModel == true)
                return 1;

            return base.CompareTo(obj);
        }

        public override string DisplayName
        {
            get { return this.smartSet.Name; }
        }

        public ISmartSet SmartSet
        {
            get { return this.smartSet; }
        }

        public void Rename()
        {
            var dialog = new RenameViewModel(this.smartSet.Name, item => this.smartSet.Category.Items.ContainsKey(item) == false);
            if (dialog.ShowDialog() == true)
            {
                this.smartSet.Name = dialog.NewName;
            }
        }

        public void Edit()
        {
            var dialog = new SmartSetEditViewModel(this.SmartSet.RuleItems, this.browser.Rules)
            {
                SmartSetName = this.smartSet.Name,
            };

            if (dialog.ShowDialog() == true)
            {
                this.smartSet.RuleItems = dialog.RuleItems;
            }
        }

        public void Delete()
        {
            if (AppMessageBox.ConfirmDelete() == false)
                return;

            this.smartSet.Dispose();
        }

        private void SmartSet_Renamed(object sender, EventArgs e)
        {
            var isSelected = this.IsSelected;
            this.NotifyOfPropertyChange(nameof(this.DisplayName));
            this.Parent.Items.Reposition(this);
            this.IsSelected = isSelected;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            this.AddViewModel(item);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            this.RemoveViewModel(item);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.Items.Clear();
                    }
                    break;
            }
        }

        private void AddViewModel(object target)
        {
            var viewModel = this.browser.CreateTreeViewItemViewModel(this, target);
            viewModel.Parent = this;
            this.items.Add(target, viewModel);
        }

        private void RemoveViewModel(object target)
        {
            if (this.items.ContainsKey(target) == true)
            {
                var viewModel = this.items[target];
                viewModel.Parent = null;
                viewModel.Dispose();
                this.items.Remove(target);
            }
        }
    }
}
