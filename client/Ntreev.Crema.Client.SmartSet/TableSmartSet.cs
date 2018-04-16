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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Tables.BrowserItems.ViewModels;
using System.Collections.Specialized;
using System.Xml.Schema;
using System.Xml;
using Ntreev.Crema.Client.Framework;
using System.Collections;

namespace Ntreev.Crema.Client.SmartSet
{
    class TableSmartSet : ItemBase<TableSmartSet, TableSmartSetCategory, TableSmartSetCollection, TableSmartSetCategoryCollection, TableSmartSetContext>,
        IXmlSerializable, ISmartSet
    {
        private IRuleItem[] ruleItems = new IRuleItem[] { };
        private ObservableCollection<ITableDescriptor> items = new ObservableCollection<ITableDescriptor>();

        public TableSmartSet()
        {

        }

        public IRuleItem[] RuleItems
        {
            get { return this.ruleItems; }
            set
            {
                this.ruleItems = value;
                this.Refresh();
            }
        }

        public ObservableCollection<ITableDescriptor> Items
        {
            get { return this.items; }
        }

        public void Refresh()
        {
            var descriptors = this.Context.GetDescriptors();

            var query = from item in descriptors
                        where this.Verify(item)
                        select item;
            var items = query.ToArray();

            var exceptedItems = this.items.Except(items).ToArray();
            var addedItems = items.Except(this.items).ToArray();

            foreach (var item in exceptedItems)
            {
                this.items.Remove(item);
            }

            foreach (var item in addedItems)
            {
                this.items.Add(item);
            }

            this.OnChanged(EventArgs.Empty);
        }

        public event EventHandler Changed;

        protected virtual void OnChanged(EventArgs e)
        {
            this.Changed?.Invoke(this, e);
        }

        private bool Verify(ITableDescriptor descriptor)
        {
            foreach (var item in this.ruleItems)
            {
                if (this.Context.Verify(descriptor, item) == false)
                    return false;
            }
            return true;
        }

        #region IXmlSerializable

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            var ruleItemList = new List<IRuleItem>();
            reader.ReadStartElement(this.GetType().Name);

            reader.MoveToContent();
            if (reader.IsEmptyElement == false)
            {
                reader.ReadStartElement("RuleItems");
                while (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "RuleItem")
                {
                    var ruleName = reader.GetAttribute("RuleName");
                    reader.ReadStartElement("RuleItem");

                    var rule = this.Context.Rules.FirstOrDefault(item => item.Name == ruleName);
                    if (rule == null)
                    {
                        reader.Skip();
                    }
                    else
                    {
                        var ruleItem = rule.CreateItem();
                        reader.MoveToContent();
                        ruleItem.ReadXml(reader);
                        ruleItemList.Add(ruleItem);
                        reader.MoveToContent();
                        reader.ReadEndElement();
                    }
                }
                reader.MoveToContent();
                reader.ReadEndElement();
            }
            else
            {
                reader.Skip();
            }
            this.ruleItems = ruleItemList.ToArray();

            reader.MoveToContent();
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(this.GetType().Name);

            writer.WriteStartElement("RuleItems");
            foreach (var item in this.ruleItems)
            {
                writer.WriteStartElement("RuleItem");
                writer.WriteAttributeString("RuleName", item.RuleName);
                item.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        #endregion

        #region ISmartSet

        string ISmartSet.CategoryPath
        {
            get { return this.Category.Path; }
            set
            {
                this.Category = this.Context.Categories[value];
            }
        }

        ISmartSetCategory ISmartSet.Category
        {
            get { return this.Category; }
        }

        IEnumerable ISmartSet.Items
        {
            get { return this.items; }
        }

        #endregion

        #region IServiceProvider

        public object GetService(Type serviceType)
        {
            return this.Context.GetService(serviceType);
        }

        #endregion
    }
}
