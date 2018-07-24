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

//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.Composition;
//using Caliburn.Micro;
//using Ntreev.Crema.Client.Framework;
//using Ntreev.Crema.Services;
//using Ntreev.Crema.ServiceModel;
//using System.Reflection;
//using Ntreev.ModernUI.Framework;
//using Ntreev.Crema.Client.SmartSet.PropertyItems.Views;
//using Ntreev.Crema.Client.SmartSet.BrowserItems.ViewModels;

//namespace Ntreev.Crema.Client.SmartSet.PropertyItems.ViewModels
//{
//    [Export(typeof(IPropertyItem))]
//    [RequiredAuthority(Authority.Guest)]
//    [View(typeof(SmartSetInformationView))]
//    [ParentType("Ntreev.Crema.Client.Tables.IPropertyService, Ntreev.Crema.Client.Tables, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null")]
//    class TableSmartSetInformationViewModel : PropertyItemBase
//    {
//        private readonly ICremaHost cremaHost;
//        private readonly IEnumerable<IRule> rules;
//        private SmartSetTreeViewItemViewModel viewModel;
//        private ISmartSet smartSet;
//        private string smartSetName;
//        private RuleItemViewModel[] ruleItems;

//        [ImportingConstructor]
//        public TableSmartSetInformationViewModel(ICremaHost cremaHost, [ImportMany]IEnumerable<IRule> rules)
//        {
//            this.DisplayName = "스마트 컬렉션";
//            this.cremaHost = cremaHost;
//            this.rules = rules.Where(item => item.SupportType == typeof(ITableDescriptor)).ToArray();
//        }

//        public override bool IsVisible
//        {
//            get { return this.viewModel != null; }
//        }

//        public string SmartSetName
//        {
//            get { return this.smartSetName; }
//            set
//            {
//                this.smartSetName = value;
//                this.NotifyOfPropertyChange(nameof(this.SmartSetName));
//            }
//        }

//        public RuleItemViewModel[] RuleItems
//        {
//            get { return this.ruleItems; }
//            set
//            {
//                this.ruleItems = value;
//                this.NotifyOfPropertyChange(nameof(this.RuleItems));
//            }
//        }

//        public override object SelectedObject
//        {
//            get { return this.smartSet; }
//        }

//        public override bool CanSupport(object obj)
//        {
//            return obj is SmartSetTreeViewItemViewModel;
//        }

//        public override void SelectObject(object obj)
//        {
//            if (this.viewModel != null)
//            {
//                this.SmartSetName = string.Empty;
//                this.RuleItems = null;
//            }

//            this.viewModel = obj as SmartSetTreeViewItemViewModel;
//            this.smartSet = null;

//            if (this.viewModel != null)
//            {
//                this.smartSet = this.viewModel.SmartSet;
//                this.SmartSetName = this.smartSet.Name;
//                this.RuleItems = this.smartSet.RuleItems.Select(item => this.MakeRuleItem(item)).ToArray();
//            }

//            this.NotifyOfPropertyChange(nameof(this.IsVisible));
//            this.NotifyOfPropertyChange(nameof(this.SelectedObject));
//        }

//        private RuleItemViewModel MakeRuleItem(IRuleItem ruleItem)
//        {
//            var rule = this.rules.First(item => item.Name == ruleItem.RuleName);
//            return new RuleItemViewModel(rule.DisplayName, rule.ToString());
//        }
//    }
//}
