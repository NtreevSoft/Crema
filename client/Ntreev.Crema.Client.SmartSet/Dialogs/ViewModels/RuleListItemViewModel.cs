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
using System.Reflection;
using System.ComponentModel;
using Ntreev.Crema.Client.Framework;
using Ntreev.ModernUI.Framework;

namespace Ntreev.Crema.Client.SmartSet.Dialogs.ViewModels
{
    class RuleListItemViewModel : PropertyChangedBase
    {
        private readonly ObservableCollection<RuleListItemViewModel> itemsSource;
        private readonly Dictionary<IRule, IRuleItem> ruleToItem = new Dictionary<IRule, IRuleItem>();
        private readonly IEnumerable<IRule> rules;
        private IRule rule;
        private IRuleItem ruleItem;

        public RuleListItemViewModel(ObservableCollection<RuleListItemViewModel> ruleItems, IEnumerable<IRule> rules, IRuleItem ruleItem)
        {
            this.itemsSource = ruleItems;
            this.itemsSource.CollectionChanged += itemsSource_CollectionChanged;
            this.rules = rules;
            if (ruleItem != null)
            {
                this.rule = rules.First(item => item.Name == ruleItem.RuleName);
                this.ruleItem = ruleItem;
            }
        }

        private void itemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.NotifyOfPropertyChange(nameof(this.CanDelete));
        }

        public string Name
        {
            get { return this.ruleItem.GetType().Name; }
        }

        public string Text
        {
            get
            {
                if (this.rule == null)
                    return "unknown";
                return this.rule.DisplayName;
            }

        }

        public IRule Rule
        {
            get { return this.rule; }
            set
            {
                this.rule = value;

                if (this.ruleToItem.ContainsKey(this.rule) == false)
                {
                    this.ruleToItem.Add(this.rule, this.rule.CreateItem());
                }
                this.ruleItem = this.ruleToItem[this.rule];
                this.NotifyOfPropertyChange(nameof(this.Rule));
                this.NotifyOfPropertyChange(nameof(this.RuleItem));
            }
        }

        public IRuleItem RuleItem
        {
            get { return this.ruleItem; }
        }

        public bool CanDelete
        {
            get { return this.itemsSource.Count != 1; }
        }

        public void Delete()
        {
            this.itemsSource.Remove(this);
        }

        public void Insert()
        {
            var index = this.itemsSource.IndexOf(this) + 1;
            this.itemsSource.Insert(index, new RuleListItemViewModel(this.itemsSource, this.rules, null));
        }

        public IEnumerable<IRule> Rules
        {
            get { return this.rules; }
        }
    }
}
