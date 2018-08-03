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

using Ntreev.Library;
using System;
using System.Xml;
using System.Xml.Linq;

namespace Ntreev.Crema.Data.Xml
{
    static class CremaXmlExtension
    {
        public static XmlAttribute AddAttribute(this XmlNode node, string name, string value)
        {
            var attribute = node.OwnerDocument.CreateAttribute(name);
            attribute.Value = value;
            node.Attributes.Append(attribute);
            return attribute;
        }

        public static XmlElement AddElement(this XmlNode node, string name)
        {
            return AddElement(node, name, string.Empty);
        }

        public static XmlElement AddElement(this XmlNode node, string name, string ns)
        {
            var element = node.OwnerDocument.CreateElement(name, ns);
            node.AppendChild(element);
            return element;
        }

        public static XmlElement AddRoot(this XmlDocument doc, string name, string ns)
        {
            var element = doc.CreateElement(name, ns);
            doc.AppendChild(element);
            return element;
        }

        public static string GetAttribute(this XElement element, string name)
        {
            var attr = element.Attribute(XName.Get(name, string.Empty));
            if (attr == null)
                return null;
            return attr.Value;
        }

        public static bool GetAttributeAsBoolean(this XmlReader reader, string name)
        {
            var text = reader.GetAttribute(name);
            if (string.IsNullOrEmpty(text) == true)
                return false;
            return CremaXmlConvert.ToBoolean(text);
        }

        public static DateTime GetAttributeAsDateTime(this XmlReader reader, string name, XmlDateTimeSerializationMode mode)
        {
            var text = reader.GetAttribute(name);
            if (string.IsNullOrEmpty(text) == true)
                return DateTime.MinValue;
            return CremaXmlConvert.ToDateTime(text, mode);
        }

        public static bool TryGetAttributeAsDateTime(this XmlReader reader, string name, XmlDateTimeSerializationMode mode, out DateTime value)
        {
            var text = reader.GetAttribute(name);
            if (string.IsNullOrEmpty(text) == true)
            {
                value = new DateTime();
                return false;
            }
            value = CremaXmlConvert.ToDateTime(text, mode);
            return true;
        }

        public static DateTime GetAttributeAsDateTime(this XElement element, string name, XmlDateTimeSerializationMode mode)
        {
            var text = element.GetAttribute(name);
            if (string.IsNullOrEmpty(text) == true)
                return DateTime.MinValue;
            return CremaXmlConvert.ToDateTime(text, mode);
        }

        public static T GetAttributeAsEnum<T>(this XmlReader reader, string name)
        {
            var text = reader.GetAttribute(name);
            if (string.IsNullOrEmpty(text) == true)
                return (T)Enum.Parse(typeof(T), "0");
            return (T)Enum.Parse(typeof(T), text);
        }

        public static Guid GetAttributeAsGuid(this XmlReader reader, string name)
        {
            var text = reader.GetAttribute(name);
            if (string.IsNullOrEmpty(text) == true)
                return Guid.Empty;
            return Guid.Parse(text);
        }

        public static bool TryGetAttributeAsGuid(this XmlReader reader, string name, out Guid value)
        {
            var text = reader.GetAttribute(name);
            if (string.IsNullOrEmpty(text) == true)
            {
                value = Guid.Empty;
                return false;
            }
            value = Guid.Parse(text);
            return true;
        }

        public static int GetAttributeAsInt32(this XmlReader reader, string name)
        {
            var text = reader.GetAttribute(name);
            if (string.IsNullOrEmpty(text) == true)
                return 0;
            return CremaXmlConvert.ToInt32(text);
        }

        public static bool TryGetAttributeAsInt32(this XmlReader reader, string name, out int value)
        {
            var text = reader.GetAttribute(name);
            if (string.IsNullOrEmpty(text) == true)
            {
                value = 0;
                return false;
            }
            value = CremaXmlConvert.ToInt32(text);
            return true;
        }

        public static int GetAttributeAsInt32(this XElement element, string name)
        {
            var text = element.GetAttribute(name);
            if (string.IsNullOrEmpty(text) == true)
                return 0;
            return CremaXmlConvert.ToInt32(text);
        }

        public static SignatureDate GetAttributeAsSignatureDate(this XmlReader reader, string user, string dateTime)
        {
            return new SignatureDate()
            {
                ID = reader.GetAttribute(user) ?? string.Empty,
                DateTime = reader.GetAttributeAsDateTime(dateTime, XmlDateTimeSerializationMode.Utc),
            };
        }

        public static bool TryGetAttributeAsSignatureDate(this XmlReader reader, string user, string dateTime, out SignatureDate value)
        {
            var id = reader.GetAttribute(user);
            var dateTimeValue = DateTime.MinValue;
            if (reader.TryGetAttributeAsDateTime(dateTime, XmlDateTimeSerializationMode.Utc, out dateTimeValue) == true || id != null)
            {
                value = new SignatureDate()
                {
                    ID = id ?? string.Empty,
                    DateTime = dateTimeValue,
                };
                return true;
            }
            value = SignatureDate.Empty;
            return false;
        }

        public static SignatureDate GetAttributeAsSignatureDate(this XElement element, string user, string dateTime)
        {
            return new SignatureDate()
            {
                ID = element.GetAttribute(user) ?? string.Empty,
                DateTime = element.GetAttributeAsDateTime(dateTime, XmlDateTimeSerializationMode.Utc),
            };
        }

        public static TagInfo GetAttributeAsTagInfo(this XmlReader reader, string name)
        {
            var text = reader.GetAttribute(name);
            if (string.IsNullOrEmpty(text) == true)
                return TagInfo.All;
            return new TagInfo(text);
        }

        public static void WriteAttribute(this XmlWriter writer, string localName, bool value)
        {
            if (value != false)
                writer.WriteAttributeString(localName, CremaXmlConvert.ToString(value));
        }

        public static void WriteAttribute(this XmlWriter writer, string localName, int value)
        {
            if (value != 0)
                writer.WriteAttributeString(localName, CremaXmlConvert.ToString(value));
        }

        public static void WriteAttribute(this XmlWriter writer, string localName, Enum value)
        {
            var name = Enum.GetName(value.GetType(), value);
            if (name == null || Convert.ToInt32(value) != 0)
            {
                writer.WriteAttributeString(localName, value.ToString());
            }
        }

        public static void WriteAttribute(this XmlWriter writer, string localName, string value)
        {
            if (string.IsNullOrEmpty(value) == false)
                writer.WriteAttributeString(localName, value);
        }

        public static void WriteAttribute(this XmlWriter writer, string localName, DateTime value)
        {
            if (value != DateTime.MinValue)
                writer.WriteAttributeString(localName, CremaXmlConvert.ToString(value));
        }

        public static void WriteAttribute(this XmlWriter writer, string user, string dateTime, SignatureDate value)
        {
            writer.WriteAttribute(user, value.ID);
            writer.WriteAttribute(dateTime, value.DateTime);
        }
    }
}
