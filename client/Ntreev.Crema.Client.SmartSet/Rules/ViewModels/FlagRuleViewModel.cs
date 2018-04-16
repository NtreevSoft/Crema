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

using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.SmartSet.Rules.ViewModels
{
    public abstract class FlagRuleViewModel : PropertyChangedBase, IRule
    {
        public FlagRuleViewModel()
        {

        }

        public string DisplayName
        {
            get;
            set;
        }

        public string Name
        {
            get { return this.GetType().Name; }
        }

        public abstract Type EnumType
        {
            get;
        }

        public abstract Type SupportType
        {
            get;
        }

        public IRuleItem CreateItem()
        {
            return new FlagRuleItemViewModel()
            {
                RuleName = this.Name,
                ItemsSource = this.ItemsSource.ToArray(),
                Value = this.ItemsSource.First().Value,
            };
        }

        public bool Verify(object target, IRuleItem ruleItem)
        {
            var value = (ruleItem as FlagRuleItemViewModel).Value;
            if (value.HasValue == false)
                return false;

            var targetValue = (Enum)this.GetTargetValue(target);
            var sourceValue = (Enum)Enum.ToObject(this.EnumType, value.Value);
            if (value.Value == 0)
                return targetValue == sourceValue;
            return targetValue.HasFlag(sourceValue);
        }

        protected abstract Enum GetTargetValue(object target);

        protected virtual Enum Parse(string name)
        {
            return (Enum)Enum.Parse(this.EnumType, name);
        }

        protected virtual IEnumerable<EnumMemberInfo> ItemsSource
        {
            get
            {
                foreach (var item in Enum.GetValues(this.EnumType))
                {
                    yield return new EnumMemberInfo(item.ToString(), (long)item);
                }
            }
        }
    }
}
