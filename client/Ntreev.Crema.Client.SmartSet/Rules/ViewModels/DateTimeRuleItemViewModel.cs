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
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Ntreev.Crema.Client.SmartSet.Properties;

namespace Ntreev.Crema.Client.SmartSet.Rules.ViewModels
{
    class DateTimeRuleItemViewModel : OperatorRuleItemViewModel<DateTime>
    {
        private long selectedType;

        public long SelectedType
        {
            get { return this.selectedType; }
            set
            {
                this.selectedType = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedType));
                this.NotifyOfPropertyChange(nameof(this.CanSetValue));
            }
        }

        public IEnumerable<EnumMemberInfo> DateTimeTypes
        {
            get
            {
                yield return new EnumMemberInfo(Resources.Text_Now, 0);
                yield return new EnumMemberInfo(Resources.Text_Today, 1);
                yield return new EnumMemberInfo(Resources.Text_SpecificTime, 2);
            }
        }

        public bool CanSetValue
        {
            get { return this.selectedType == 2; }
        }

        public override DateTime Value
        {
            get
            {
                if (this.selectedType == 0)
                    return DateTime.Now;
                else if (this.selectedType == 1)
                    return DateTime.Today;
                return base.Value;
            }
            set
            {
                base.Value = value;
            }
        }

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);
            reader.MoveToContent();
            this.selectedType = reader.ReadElementContentAsLong();
            reader.MoveToContent();
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            writer.WriteStartElement("type");
            writer.WriteValue(this.selectedType);
            writer.WriteEndElement();
        }
    }
}
