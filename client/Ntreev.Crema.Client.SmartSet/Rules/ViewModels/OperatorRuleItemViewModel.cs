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
using Ntreev.Crema.Data.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Ntreev.Crema.Client.SmartSet.Rules.ViewModels
{
    public class OperatorRuleItemViewModel<T> : RuleItem where T : IComparable
    {
        private OperatorType operatorType;
        private T value;

        public virtual T Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                this.NotifyOfPropertyChange(nameof(this.Value));
            }
        }

        public OperatorType OperatorType
        {
            get { return this.operatorType; }
            set
            {
                this.operatorType = value;
                this.NotifyOfPropertyChange(nameof(this.OperatorType));
            }
        }

        public virtual IEnumerable<OperatorType> Operators
        {
            get
            {
                foreach (var item in Enum.GetValues(typeof(OperatorType)))
                {
                    yield return (OperatorType)item;
                }
            }
        }

        public virtual IEnumerable<KeyValuePair<string, OperatorType>> DisplayableOperators
        {
            get
            {
                foreach (var item in this.Operators)
                {
                    if (item == OperatorType.Equal)
                        yield return new KeyValuePair<string, OperatorType>("==", item);
                    if (item == OperatorType.GreaterThan)
                        yield return new KeyValuePair<string, OperatorType>(">", item);
                    if (item == OperatorType.GreaterThanEqual)
                        yield return new KeyValuePair<string, OperatorType>(">=", item);
                    if (item == OperatorType.LessThan)
                        yield return new KeyValuePair<string, OperatorType>("<", item);
                    if (item == OperatorType.LessThanEqual)
                        yield return new KeyValuePair<string, OperatorType>("<=", item);
                    if (item == OperatorType.NotEqual)
                        yield return new KeyValuePair<string, OperatorType>("~=", item);
                }
            }
        }

        protected override void OnReadXml(XmlReader reader)
        {
            var value1 = reader.ReadElementContentAsString();
            this.value = (T)CremaXmlConvert.ToValue(value1, typeof(T));
            reader.MoveToContent();
            var value2 = reader.ReadElementContentAsString();
            this.operatorType = (OperatorType)Enum.Parse(typeof(OperatorType), value2);
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(nameof(Value));
            writer.WriteValue(this.value);
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(OperatorType));
            writer.WriteValue(this.operatorType.ToString());
            writer.WriteEndElement();
        }
    }
}
