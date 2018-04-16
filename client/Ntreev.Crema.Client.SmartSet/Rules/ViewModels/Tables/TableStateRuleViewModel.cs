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
using Ntreev.Crema.Client.SmartSet.Properties;
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.SmartSet.Rules.ViewModels.Tables
{
    [Export(typeof(IRule))]
    class TableStateRuleViewModel : FlagRuleViewModel
    {
        public TableStateRuleViewModel()
        {
            this.DisplayName = Resources.Title_TableStates;
        }

        public override Type EnumType
        {
            get { return typeof(TableState); }
        }

        public override Type SupportType
        {
            get { return typeof(ITableDescriptor); }
        }

        protected override Enum GetTargetValue(object target)
        {
            return (target as ITableDescriptor).TableState;
        }

        protected override IEnumerable<EnumMemberInfo> ItemsSource
        {
            get
            {
                yield return new EnumMemberInfo(Resources.Text_None, (long)TableState.None);
                yield return new EnumMemberInfo(Resources.Text_IsBeingEdited, (long)TableState.IsBeingEdited);
                yield return new EnumMemberInfo(Resources.Text_IsBeingSetup, (long)TableState.IsBeingSetup);
                yield return new EnumMemberInfo(Resources.Text_IsMember, (long)TableState.IsMember, Resources.Comment_IsMember);
            }
        }
    }
}
