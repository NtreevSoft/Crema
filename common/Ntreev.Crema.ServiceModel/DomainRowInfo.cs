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

using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Ntreev.Crema.Data.Xml;
using System.Collections.Generic;
using System;
using System.Linq;
using Ntreev.Library;
using System.Xml.Schema;
using System.Xml;
using Ntreev.Library.Serialization;
using System.Text;
using System.IO;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.ServiceModel
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct DomainRowInfo
    {
        private object[] fields;
        private object[] keys;

        [XmlElement]
        public string TableName { get; set; }

        [XmlIgnore]
        public object[] Fields
        {
            get { return this.fields; }
            set { this.fields = value; }
        }

        [XmlIgnore]
        public object[] Keys
        {
            get { return this.keys; }
            set { this.keys = value; }
        }

        public static readonly DomainRowInfo Empty;

        internal static void WriteFields(XmlWriter writer, string propertyName, object[] fields)
        {
            writer.WriteStartElement(propertyName);

            if (fields != null)
            {
                foreach (var item in fields)
                {
                    writer.WriteStartElement("item");
                    if (item is DBNull)
                    {
                        writer.WriteAttributeString("type", nameof(DBNull));
                    }
                    else if (item != null)
                    {
                        if (item is Guid)
                            writer.WriteAttributeString("type", nameof(Guid));
                        else
                            writer.WriteAttributeString("type", item.GetType().GetTypeName());
                        writer.WriteString(CremaXmlConvert.ToString(item));
                    }
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        internal static object[] ReadFields(XmlReader reader, string propertyName)
        {
            reader.MoveToContent();
            if (reader.IsEmptyElement == false)
            {
                var fieldList = new List<object>();
                reader.ReadStartElement(propertyName);

                while (reader.MoveToContent() == XmlNodeType.Element)
                {
                    var attr = reader.GetAttribute("type");
                    if (reader.IsEmptyElement == false)
                    {
                        var text = reader.ReadElementContentAsString();
                        var type = attr == nameof(Guid) ? typeof(Guid) : CremaDataTypeUtility.GetType(attr);
                        var field = CremaXmlConvert.ToValue(text, type);
                        fieldList.Add(field);
                    }
                    else if (attr == nameof(DBNull))
                    {
                        fieldList.Add(DBNull.Value);
                        reader.Skip();
                    }
                    else
                    {
                        fieldList.Add(null);
                        reader.Skip();
                    }
                }
                reader.ReadEndElement();
                return fieldList.ToArray();
            }
            else
            {
                reader.Skip();
                return null;
            }
        }

        internal static readonly string[] ClearKey = new string[] { "6B924529-8134-463D-A040-1632BCE6813A", "F1405371-7961-4A6D-9DDA-D66838617F41" };

        #region DataMember

        [XmlElement]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ItemXml
        {
            get
            {
                var settings = new XmlWriterSettings()
                {
                    ConformanceLevel = ConformanceLevel.Fragment,
                    Encoding = Encoding.UTF8,
                    Indent = true,
                };

                using (var sw = new Ntreev.Library.IO.Utf8StringWriter())
                using (var writer = XmlWriter.Create(sw, settings))
                {
                    WriteFields(writer, nameof(Fields), this.fields);
                    WriteFields(writer, nameof(Keys), this.keys);

                    writer.Close();
                    return sw.ToString();
                }
            }
            set
            {
                var settings = new XmlReaderSettings()
                {
                    ConformanceLevel = ConformanceLevel.Fragment,
                };

                using (var sr = new StringReader(value))
                using (var reader = XmlReader.Create(sr, settings))
                {
                    this.fields = ReadFields(reader, nameof(Fields));
                    this.keys = ReadFields(reader, nameof(Keys));
                }
            }
        }

        [DataMember]
        [XmlIgnore]
        private string Xml
        {
            get { return XmlSerializerUtility.GetString(this); }
            set { this = XmlSerializerUtility.ReadString(this, value); }
        }

        #endregion
    }
}
