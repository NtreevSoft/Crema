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

#pragma warning disable 0612
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace Ntreev.Crema.Data.Xml.Schema
{
    public static class CremaSchema
    {
        public const string BaseNamespace = "http://schemas.ntreev.com/crema";
        public const string TableDirectory = "tables";
        public const string TypeDirectory = "types";
        public const string TemplateDirectory = "templates";
        public static readonly string TypeNamespace = UriUtility.Combine(BaseNamespace, TypeDirectory);
        public static readonly string TableNamespace = UriUtility.Combine(BaseNamespace, TableDirectory);
        public static readonly string TemplateNamespace1 = UriUtility.Combine(BaseNamespace, TemplateDirectory);

        public const string SchemaExtension = ".xsd";
        public const string XmlExtension = ".xml";

        public const string Version = "Version";
        public static readonly int MajorVersion = 4;
        public static readonly int MinorVersion = 0;
        public static readonly string VersionValue = $"{MajorVersion}.{MinorVersion}";
        public const string TableInfo = "TableInfo";
        public const string TypeInfo = "TypeInfo";
        public const string ColumnInfo = "ColumnInfo";
        public const string AttributeInfo = "AttributeInfo";
        public const string ExtendedProperty = "ExtendedProperty";
        public const string CreatedDateTime = "CreatedDateTime";
        public const string Creator = "Creator";
        public const string ModifiedDateTime = "ModifiedDateTime";
        public const string Modifier = "Modifier";
        public const string Tags = "Tags";
        public const string Enable = "Enable";
        public const string TypeName = "TypeName";
        public const string IsFlag = "IsFlag";
        public const string Comment = "Comment";
        public const string ReadOnly = "ReadOnly";
        [Obsolete]
        public const string Category = "Category";
        public const string CategoryPath = "CategoryPath";
        public const string TemplateNamespace = "TemplateNamespace";
        public const string RelationID = "RelationID";
        public const string ParentID = "ParentID";
        [Obsolete]
        public const string Description = "Description";
        public const string Index = "__Index__";
        public const string Name = "Name";
        public const string ColumnName = "ColumnName";
        public const string TableName = "TableName";
        public const string Value = "Value";
        public const string DataType = "DataType";
        public const string DefaultValue = "DefaultValue";
        public const string AllowNull = "AllowNull";
        public const string IsKey = "IsKey";
        public const string IsUnique = "IsUnique";
        public const string AutoIncrement = "AutoIncrement";
        public const string ID = "ID";
        public const string Attributes = "Attributes";

        public const string InstancePrefix = "xsi";

        public const string __RelationID__ = "__RelationID__";
        public const string __ParentID__ = "__ParentID__";

        public const string ContentsModifiedDateTime = "ContentsModifiedDateTime";
        public const string ContentsModifier = "ContentsModifier";

        private readonly static string[] reservedNames = new string[]
        {
            CremaSchemaObsolete.DataLocation ,
            CremaSchema.RelationID,
        };

        internal readonly static object lockobj = new object();

        internal const string DataTypePrefix = "dt";
        internal const string TableTypePrefix = "t";

        internal const char NameSeparatorChar = '.';
        internal static readonly string NameSeparator = NameSeparatorChar.ToString();

        internal const string ComplexTypeNameExtension = ".Table";
        internal const string UniqueTypeNameExtension = ".Unique";
        internal const string KeyTypeNameExtension = ".Key";
        internal const string CountExtension = ".Count";
        internal const string TagsExtension = ".Tags";
        internal const string CommentExtension = ".Comment";
        internal const string IDExtension = ".ID";
        internal const string FlagExtension = ".Flag";
        internal static readonly string ModifierExtension = NameSeparator + Modifier;
        internal static readonly string ModifiedDateTimeExtension = NameSeparator + ModifiedDateTime;
        internal static readonly string CreatorExtension = NameSeparator + Creator;
        internal static readonly string CreatedDateTimeExtension = NameSeparator + CreatedDateTime;

        public static string GenerateTableTypeName(string name)
        {
            return name + ".Table";
        }

        public static string GenerateColumnTypeName(string name)
        {
            return name + ".Column";
        }

        public static string GenerateKeyName(string name)
        {
            return name + ".Key";
        }

        public static string GenerateUniqueName(string name)
        {
            return name + ".Unique";
        }

        public static string GenerateXPath(string prefix, params string[] names)
        {
            return string.Join("/", names.Select(item => prefix + ":" + item));
        }

        [Obsolete]
        internal static string GenerateXPathObsolete(string name)
        {
            return name;
        }

        public static string SplitNameFromXPath(string xpath)
        {
            int index = xpath.IndexOf(".//");
            if (index != 0)
                throw new CremaDataException("잘못된 xpath입니다.");
            return xpath.Substring(".//".Length);
        }

        public static string GenerateFlagTypeName(string typeName)
        {
            return typeName + CremaSchema.FlagExtension;
        }

        public static string GenerateSchemaTypeName(string typeName)
        {
            if (CremaDataTypeUtility.IsBaseType(typeName) == true)
                return string.Format("{0}:{1}", "xs", typeName);
            return typeName;
        }

        [Obsolete]
        public static string GenerateRelationID(string tableName)
        {
            return __RelationID__;
        }

        public static string[] SplitFromRelationName(string relationName)
        {
            var name = System.Xml.Linq.XName.Get(relationName);
            var names = new List<string>(name.LocalName.Split('.'))
            {
                name.NamespaceName
            };
            return names.ToArray();
        }

        public static string GetParentNameFromRelationName(string relationName)
        {
            return SplitFromRelationName(relationName)[0];
        }

        public static string GetChildNameFromRelationName(string relationName)
        {
            return SplitFromRelationName(relationName)[1];
        }

        public static string GetTableNamespaceFromRelationName(string relationName)
        {
            return SplitFromRelationName(relationName)[2];
        }

        public static void SchemaValidationEventHandler(object sender, ValidationEventArgs e)
        {
            Trace.WriteLine(e.Message);
        }

        public static string[] ReservedNames
        {
            get { return reservedNames; }
        }

        internal static void WriteAppInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, bool value, string ns)
        {
            WriteAppInfo(annotated, nodeName, attributeName, value, ns, false);
        }

        internal static void WriteAppInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, bool value, string ns, bool defaultValue)
        {
            WriteAppInfo(annotated, nodeName, attributeName, CremaXmlConvert.ToString(value), ns, CremaXmlConvert.ToString(defaultValue));
        }

        internal static void WriteAppInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, int value, string ns)
        {
            WriteAppInfo(annotated, nodeName, attributeName, value, ns, 0);
        }

        internal static void WriteAppInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, int value, string ns, int defaultValue)
        {
            WriteAppInfo(annotated, nodeName, attributeName, CremaXmlConvert.ToString(value), ns, CremaXmlConvert.ToString(defaultValue));
        }

        internal static void WriteAppInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, long value, string ns)
        {
            WriteAppInfo(annotated, nodeName, attributeName, value, ns, (long)0);
        }

        internal static void WriteAppInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, long value, string ns, long defaultValue)
        {
            WriteAppInfo(annotated, nodeName, attributeName, CremaXmlConvert.ToString(value), ns, CremaXmlConvert.ToString(defaultValue));
        }

        internal static void WriteAppInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, DateTime value, string ns)
        {
            WriteAppInfo(annotated, nodeName, attributeName, value, ns, DateTime.MinValue);
        }

        internal static void WriteAppInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, DateTime value, string ns, DateTime defaultValue)
        {
            WriteAppInfo(annotated, nodeName, attributeName, CremaXmlConvert.ToString(value), ns, CremaXmlConvert.ToString(defaultValue));
        }

        internal static void WriteAppInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, TagInfo value, string ns)
        {
            WriteAppInfo(annotated, nodeName, attributeName, value, ns, TagInfo.All);
        }

        internal static void WriteAppInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, TagInfo value, string ns, TagInfo defaultValue)
        {
            WriteAppInfo(annotated, nodeName, attributeName, value.ToString(), ns, defaultValue.ToString());
        }

        internal static void WriteAppInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, Guid value, string ns)
        {
            WriteAppInfo(annotated, nodeName, attributeName, value, ns, Guid.Empty);
        }

        internal static void WriteAppInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, Guid value, string ns, Guid defaultValue)
        {
            WriteAppInfo(annotated, nodeName, attributeName, CremaXmlConvert.ToString(value), ns, CremaXmlConvert.ToString(defaultValue));
        }

        internal static void WriteAppInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, string value, string ns)
        {
            WriteAppInfo(annotated, nodeName, attributeName, value, ns, null);
        }

        internal static void WriteAppInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, string value, string ns, string defaultValue)
        {
            if (value == defaultValue || string.IsNullOrEmpty(value) == true)
                return;

            if (annotated.Annotation == null)
            {
                annotated.Annotation = new XmlSchemaAnnotation();
            }

            var annotation = annotated.Annotation;
            {
                XmlSchemaAppInfo appInfo = null;

                foreach (XmlSchemaObject item in annotation.Items)
                {
                    if (item is XmlSchemaAppInfo == true)
                    {
                        appInfo = item as XmlSchemaAppInfo;
                        break;
                    }
                }

                if (appInfo == null)
                {
                    appInfo = new XmlSchemaAppInfo();
                    annotation.Items.Add(appInfo);
                }

                var doc = new XmlDocument();
                var root = doc.CreateElement("root", ns);
                doc.AppendChild(root);
                var element = doc.CreateElement(nodeName, ns);
                var valueAttr = doc.CreateAttribute(attributeName);
                valueAttr.Value = value as string;
                element.Attributes.Append(valueAttr);

                doc.DocumentElement.AppendChild(element);

                if (appInfo.Markup == null)
                {
                    appInfo.Markup = new XmlNode[1] { element };
                }
                else
                {
                    var nodes = new XmlNode[appInfo.Markup.Length + 1];
                    appInfo.Markup.CopyTo(nodes, 0);
                    nodes[appInfo.Markup.Length] = element;
                    appInfo.Markup = nodes;
                }
            }
        }

        internal static void WriteDescription(this XmlSchemaAnnotated annotated, string description)
        {
            if (string.IsNullOrEmpty(description) == true)
                return;

            if (annotated.Annotation == null)
            {
                annotated.Annotation = new XmlSchemaAnnotation();
            }

            var annotation = annotated.Annotation;
            {
                var documentation = new XmlSchemaDocumentation();
                {
                    var doc = new XmlDocument();
                    var textNode = doc.CreateTextNode(description);
                    documentation.Markup = new XmlNode[1] { textNode };
                }
                annotation.Items.Add(documentation);
            }
        }

        internal static string ReadDescription(this XmlSchemaAnnotated annotated)
        {
            var annotation = annotated.Annotation;
            if (annotation == null)
                return string.Empty;

            var stringList = new List<string>();
            foreach (XmlSchemaObject item in annotation.Items)
            {
                if (item is XmlSchemaDocumentation == true)
                {
                    var documentation = item as XmlSchemaDocumentation;

                    foreach (XmlNode node in documentation.Markup)
                    {
                        stringList.Add(node.Value);
                    }
                }
            }

            if (stringList.Count == 0)
                return string.Empty;

            var description = string.Empty;
            foreach (var item in stringList)
            {
                if (string.IsNullOrEmpty(description) == true)
                {
                    description = item;
                }
                else
                {
                    description += Environment.NewLine + item;
                }
            }
            return description;
        }

        internal static string ReadAppInfoComfortableAsString(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, params string[] comfortableNames)
        {
            var text = ReadAppInfoAsString(annotated, nodeName, attributeName);
            if (text == null)
            {
                foreach (var item in comfortableNames)
                {
                    text = ReadAppInfoAsString(annotated, nodeName, item);
                    if (text != null)
                        break;
                }
            }

            return text;
        }

        internal static int ReadAppInfoComfortableAsInt32(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, params string[] comfortableNames)
        {
            var text = ReadAppInfoComfortableAsString(annotated, nodeName, attributeName, comfortableNames);
            if (string.IsNullOrEmpty(text) == true)
                return 0;
            return int.Parse(text);
        }

        internal static long ReadAppInfoComfortableAsInt64(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, params string[] comfortableNames)
        {
            var text = ReadAppInfoComfortableAsString(annotated, nodeName, attributeName, comfortableNames);
            if (string.IsNullOrEmpty(text) == true)
                return 0;
            return long.Parse(text);
        }

        internal static bool ReadAppInfoAsBoolean(this XmlSchemaAnnotated annotated, string nodeName, string attributeName)
        {
            return ReadAppInfoAsBoolean(annotated, nodeName, attributeName, false);
        }

        internal static bool ReadAppInfoAsBoolean(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, bool defaultValue)
        {
            var text = ReadAppInfoAsString(annotated, nodeName, attributeName);
            if (text == null)
                return defaultValue;
            return CremaXmlConvert.ToBoolean(text);
        }

        internal static DateTime ReadAppInfoAsDateTime(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, XmlDateTimeSerializationMode mode)
        {
            return ReadAppInfoAsDateTime(annotated, nodeName, attributeName, mode, DateTime.MinValue);
        }

        internal static DateTime ReadAppInfoAsDateTime(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, XmlDateTimeSerializationMode mode, DateTime defaultValue)
        {
            var text = ReadAppInfoAsString(annotated, nodeName, attributeName);
            if (text == null)
                return defaultValue;
            return CremaXmlConvert.ToDateTime(text, mode);
        }

        internal static Guid ReadAppInfoAsGuid(this XmlSchemaAnnotated annotated, string nodeName, string attributeName)
        {
            return ReadAppInfoAsGuid(annotated, nodeName, attributeName, Guid.Empty);
        }

        internal static Guid ReadAppInfoAsGuid(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, Guid defaultValue)
        {
            var text = ReadAppInfoAsString(annotated, nodeName, attributeName);
            if (text == null)
                return defaultValue;
            return CremaXmlConvert.ToGuid(text);
        }

        internal static TagInfo ReadAppInfoAsTagInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName)
        {
            return ReadAppInfoAsTagInfo(annotated, nodeName, attributeName, TagInfo.All);
        }

        internal static TagInfo ReadAppInfoAsTagInfo(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, TagInfo defaultValue)
        {
            var text = ReadAppInfoAsString(annotated, nodeName, attributeName);
            if (text == null)
                return defaultValue;
            return (TagInfo)text;
        }

        internal static string ReadAppInfoAsString(this XmlSchemaAnnotated annotated, string nodeName, string attributeName)
        {
            return ReadAppInfoAsString(annotated, nodeName, attributeName, null);
        }

        internal static string ReadAppInfoAsString(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, string defaultValue)
        {
            var annotation = annotated.Annotation;
            if (annotation == null)
                return defaultValue;

            foreach (XmlSchemaObject item in annotation.Items)
            {
                if (item is XmlSchemaAppInfo == true)
                {
                    var appInfo = item as XmlSchemaAppInfo;

                    foreach (XmlNode xmlNode in appInfo.Markup)
                    {
                        if (xmlNode.Name != nodeName)
                            continue;

                        var attr = xmlNode.Attributes[attributeName];
                        if (attr != null)
                            return attr.Value;
                    }
                }
            }

            return defaultValue;
        }

        //[Obsolete("for 2.0")]
        internal static Guid ReadAppInfoAsGuidVersion2(this XmlSchemaAnnotated annotated, string nodeName, string attributeName, string nameToGenerate)
        {
            var text = ReadAppInfoAsString(annotated, nodeName, attributeName);
            if (string.IsNullOrEmpty(text) == true)
                return GuidUtility.FromName(nameToGenerate);
            return Guid.Parse(text);
        }

        internal static SignatureDate ReadAppInfoAsSigunatureDate(this XmlSchemaAnnotated annotated, string nodeName, string user, string dateTime)
        {
            return new SignatureDate()
            {
                ID = annotated.ReadAppInfoAsString(nodeName, user) ?? string.Empty,
                DateTime = annotated.ReadAppInfoAsDateTime(nodeName, dateTime, XmlDateTimeSerializationMode.Utc),
            };
        }

        internal static IEnumerable<XmlSchemaElement> GetSequenceElements(this XmlSchemaComplexType complexType)
        {
            if (complexType.ContentTypeParticle is XmlSchemaSequence sequence)
            {
                foreach (XmlSchemaObject item in sequence.Items)
                {
                    if (item is XmlSchemaElement == true)
                    {
                        yield return item as XmlSchemaElement;
                    }
                }
            }
        }

        internal static IEnumerable<XmlSchemaAttribute> GetAttributes(this XmlSchemaComplexType complexType)
        {
            foreach (var item in complexType.Attributes)
            {
                yield return item as XmlSchemaAttribute;
            }

            if (complexType.ContentModel is XmlSchemaComplexContent == true)
            {
                var contentModel = complexType.ContentModel as XmlSchemaComplexContent;
                if (contentModel.Content is XmlSchemaComplexContentExtension == true)
                {
                    var contentExtension = contentModel.Content as XmlSchemaComplexContentExtension;
                }
            }
        }

        internal static IEnumerable<XmlSchemaKey> GetKeyConstraints(this XmlSchemaElement element)
        {
            foreach (var item in element.Constraints)
            {
                if (item is XmlSchemaKey == true)
                {
                    yield return item as XmlSchemaKey;
                }
            }
        }

        internal static IEnumerable<XmlSchemaUnique> GetUniqueConstraints(this XmlSchemaElement element)
        {
            foreach (var item in element.Constraints)
            {
                if (item is XmlSchemaUnique == true)
                {
                    yield return item as XmlSchemaUnique;
                }
            }
        }

        internal static IEnumerable<XmlSchemaComplexType> GetComplexTypes(this XmlSchema schema)
        {
            foreach (var item in schema.Items)
            {
                if (item is XmlSchemaComplexType == true)
                    yield return item as XmlSchemaComplexType;
            }
        }

        internal static IEnumerable<XmlSchemaSimpleType> GetSimpleTypes(this XmlSchema schema)
        {
            foreach (var item in schema.Items)
            {
                if (item is XmlSchemaSimpleType == true)
                    yield return item as XmlSchemaSimpleType;
            }
        }

        internal static IEnumerable<XmlSchemaXPath> GetFields(this XmlSchemaIdentityConstraint constraint)
        {
            foreach (var item in constraint.Fields)
            {
                if (item is XmlSchemaXPath == true)
                    yield return item as XmlSchemaXPath;
            }
        }

        internal static XmlSchemaComplexType GetBaseType(this XmlSchemaComplexContentExtension extension)
        {
            var schema = extension.GetSchema();

            for (var i = 0; i < schema.Items.Count; i++)
            {
                var item = schema.Items[i];
                if (item is XmlSchemaComplexType == false)
                    continue;

                var complexType = item as XmlSchemaComplexType;

                if (complexType.QualifiedName == extension.BaseTypeName)
                    return complexType;
            }
            return null;
        }

        internal static XmlSchema GetSchema(this XmlSchemaObject schemaObject)
        {
            var parent = schemaObject.Parent;
            while (parent != null)
            {
                if (parent is XmlSchema)
                    return parent as XmlSchema;
                parent = parent.Parent;
            }
            return null;
        }

        internal static string GetTypeNameFromSchema(string schema)
        {
            throw new NotImplementedException();
        }

        internal static string GetSchemaLocationFromXml(string xml)
        {
            throw new NotImplementedException();
        }
    }
}
