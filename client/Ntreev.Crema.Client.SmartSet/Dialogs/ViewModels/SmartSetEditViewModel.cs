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

using System.Collections.ObjectModel;
using System.Linq;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System;
using Ntreev.Crema.Data;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Ntreev.Crema.Client.Framework;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.SmartSet.Dialogs.Views;
using Ntreev.Crema.Client.SmartSet.Properties;

namespace Ntreev.Crema.Client.SmartSet.Dialogs.ViewModels
{
    [View(typeof(SmartSetEditView))]
    class SmartSetEditViewModel : ModalDialogBase
    {
        private readonly IEnumerable<IRule> rules;
        private readonly ObservableCollection<RuleListItemViewModel> itemsSource = new ObservableCollection<RuleListItemViewModel>();

        private string smartSetName;
        private IRuleItem[] ruleItems;
        private bool canRename;

        public SmartSetEditViewModel(IEnumerable<IRule> rules)
        {
            this.rules = rules;
            this.DisplayName = Resources.Title_NewSmartCollection;
            this.canRename = true;
            this.itemsSource.Add(new RuleListItemViewModel(this.itemsSource, rules, null));
        }

        public SmartSetEditViewModel(IEnumerable<IRuleItem> ruleItems, IEnumerable<IRule> rules)
        {
            this.rules = rules;
            this.DisplayName = Resources.Title_EditSmartCollection;

            foreach (var item in ruleItems)
            {
                this.itemsSource.Add(new RuleListItemViewModel(this.itemsSource, this.rules, item));
            }
        }

        public string SmartSetName
        {
            get { return this.smartSetName; }
            set
            {
                this.smartSetName = value;
                this.NotifyOfPropertyChange(nameof(this.SmartSetName));
                this.NotifyOfPropertyChange(nameof(this.CanSave));
            }
        }

        public IEnumerable<RuleListItemViewModel> ItemsSource
        {
            get { return this.itemsSource; }
        }

        public IEnumerable<IRule> Rules
        {
            get { return this.rules; }
        }

        public IRuleItem[] RuleItems
        {
            get { return this.ruleItems; }
        }

        public bool CanRename
        {
            get { return this.canRename; }
        }

        public bool CanSave
        {
            get
            {
                return !string.IsNullOrEmpty(this.SmartSetName);
            }
        }

        public void Save()
        {
            try
            {
                this.ruleItems = this.ItemsSource.Select(item => item.RuleItem).ToArray();
                this.TryClose(true);
            }
            catch (Exception ex)
            {
                AppMessageBox.ShowError(ex.Message);
            }
        }

        public void Cancel()
        {
            this.TryClose();
        }
    }
}