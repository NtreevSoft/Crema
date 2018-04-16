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
    class StringCompareRuleItemViewModel : RuleItem
    {
        private string value;
        private bool caseSensitive;
        private bool globPattern;

        public bool CaseSensitive
        {
            get { return this.caseSensitive; }
            set
            {
                this.caseSensitive = value;
                this.NotifyOfPropertyChange(nameof(this.CaseSensitive));
            }
        }

        public bool GlobPattern
        {
            get { return this.globPattern; }
            set
            {
                this.globPattern = value;
                this.NotifyOfPropertyChange(nameof(this.GlobPattern));
            }
        }

        public string Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                this.NotifyOfPropertyChange(nameof(this.Value));
            }
        }

        protected override void OnReadXml(System.Xml.XmlReader reader)
        {
            this.value = reader.ReadElementContentAsString();
            reader.MoveToContent();
            this.caseSensitive = reader.ReadElementContentAsBoolean();
        }

        protected override void OnWriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement(nameof(Value));
            writer.WriteValue(this.value);
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(CaseSensitive));
            writer.WriteValue(this.caseSensitive);
            writer.WriteEndElement();
        }
    }
}
