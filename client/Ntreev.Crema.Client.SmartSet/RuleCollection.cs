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
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Xml.Serialization;
//using System.Xml;
//using System.Reflection;

//namespace Ntreev.Crema.Client.SmartSet
//{
//    class RuleCollection : List<IRule>, IXmlSerializable
//    {
//        public System.Xml.Schema.XmlSchema GetSchema()
//        {
//            return null;
//        }

//        public void ReadXml(System.Xml.XmlReader reader)
//        {
//            bool wasEmpty = reader.IsEmptyElement;

//            if (wasEmpty)
//                return;

//            reader.Read();

//            while (reader.NodeType != XmlNodeType.EndElement)
//            {
//                string ruleType = "Ntreev.Crema.Client.SmartSet." + reader.Name;

//                Type type = Type.GetType(ruleType);
//                XmlSerializer serializer = XmlSerializer.FromTypes(new Type[] { type, })[0];
//                IRule rule = serializer.Deserialize(reader) as IRule;

//                if (rule != null)
//                {
//                    Add(rule);
//                }

//                reader.Read();
//                reader.MoveToContent();
//            }
//            reader.ReadEndElement();
//        }

//        public void WriteXml(System.Xml.XmlWriter writer)
//        {
//            foreach (IRule rule in this)
//            {
//                XmlSerializer serializer = XmlSerializer.FromTypes(new Type[] { rule.GetType(), })[0];
//                serializer.Serialize(writer, rule);
//            }
//        }
//    }
//}
