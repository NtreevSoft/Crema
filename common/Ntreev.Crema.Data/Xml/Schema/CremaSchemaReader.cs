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
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Xml.Schema;
using System.Data;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using Ntreev.Crema.Data;
using System.Text.RegularExpressions;
using Ntreev.Library;
using System.Collections;
using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;

namespace Ntreev.Crema.Data.Xml.Schema
{
    public class CremaSchemaReader
    {
        private readonly CremaDataSet dataSet;
        private readonly CremaDataTable dataTable;
        private readonly CremaDataType type;
        private readonly ItemName itemName;
        private readonly Dictionary<string, CremaDataTable> tables = new Dictionary<string, CremaDataTable>();
        private Version version = new Version();
        private string hashValue;
        private XmlSchema schema;

        public CremaSchemaReader(CremaDataSet dataSet)
            : this(dataSet, null)
        {

        }

        public CremaSchemaReader(CremaDataSet dataSet, ItemName itemName)
        {
            this.dataSet = dataSet ?? throw new ArgumentNullException();
            this.itemName = itemName;
        }

        public CremaSchemaReader(CremaDataTable dataTable, ItemName itemName)
        {
            if (dataTable == null)
                throw new ArgumentNullException();
            if (dataTable.DataSet != null)
                this.dataSet = dataTable.DataSet;
            else
                this.dataTable = dataTable;
            this.itemName = itemName;
        }

        public CremaSchemaReader(CremaDataType type)
        {
            this.type = type ?? throw new ArgumentNullException();
        }

        public void Read(string filename)
        {
            if (this.hashValue == null)
            {
                this.hashValue = HashUtility.GetHashValueFromFile(filename);
            }
            using (var stream = File.OpenRead(filename))
            {
                var schema = XmlSchema.Read(stream, CremaSchema.SchemaValidationEventHandler);
                schema.SourceUri = $"{new Uri(filename)}";
                this.Read(schema, new CremaXmlResolver(filename));
            }
        }

        public void Read(string filename, XmlResolver resolver)
        {
            if (this.hashValue == null)
            {
                this.hashValue = HashUtility.GetHashValueFromFile(filename);
            }
            using (var stream = File.OpenRead(filename))
            {
                var schema = XmlSchema.Read(stream, CremaSchema.SchemaValidationEventHandler);
                schema.SourceUri = $"{new Uri(filename)}";
                this.Read(schema, resolver);
            }
        }

        public void Read(TextReader reader)
        {
            this.Read(reader, new CremaXmlResolver(this.dataSet));
        }

        public void Read(Stream stream)
        {
            if (this.hashValue == null && stream.CanSeek == true)
            {
                this.hashValue = HashUtility.GetHashValue(stream);
                stream.Seek(0, SeekOrigin.Begin);
            }
            this.Read(stream, new CremaXmlResolver(this.dataSet));
        }

        public void Read(XmlReader reader)
        {
            this.Read(reader, new CremaXmlResolver(this.dataSet));
        }

        public void Read(Stream stream, XmlResolver resolver)
        {
            if (this.hashValue == null && stream.CanSeek == true)
            {
                this.hashValue = HashUtility.GetHashValue(stream);
                stream.Seek(0, SeekOrigin.Begin);
            }

            this.Read(XmlSchema.Read(stream, CremaSchema.SchemaValidationEventHandler), resolver);
        }

        public void Read(TextReader reader, XmlResolver resolver)
        {
            using (var xmlReader = XmlReader.Create(reader))
            {
                this.Read(xmlReader, resolver);
            }
        }

        public void Read(XmlReader reader, XmlResolver resolver)
        {
            this.Read(XmlSchema.Read(reader, CremaSchema.SchemaValidationEventHandler), resolver);
            if (reader.NodeType == XmlNodeType.EndElement)
                reader.ReadEndElement();
        }

        public CremaDataSet DataSet
        {
            get { return this.dataSet; }
        }

        private void Read(XmlSchema schema, XmlResolver resolver)
        {
            var schemaSet = new XmlSchemaSet()
            {
                XmlResolver = resolver
            };
            schemaSet.Add(schema);
            schemaSet.Compile();

            if (Version.TryParse(schema.Version, out this.version) == false)
            {
                this.version = new Version(2, 0);
            }
            this.schema = schema;
            if (this.type != null)
            {
                this.ReadType(schema);
            }
            else if (this.dataTable != null)
            {
                var element = schema.Elements.Values.OfType<XmlSchemaElement>().First();
                if (element.Name != CremaDataSet.DefaultDataSetName)
                    throw new CremaDataException();
                this.ReadDataTable(element);
            }
            else if (this.dataSet != null)
            {
                var element = schema.Elements.Values.OfType<XmlSchemaElement>().First();
                if (element.Name != CremaDataSet.DefaultDataSetName)
                    throw new CremaDataException();

                this.ReadType(schema);
                this.ReadDataSet(element);
            }
        }

        private void ReadType(XmlSchema schema)
        {
            if (this.version < new Version(3, 0))
            {
                var query = from item in schema.GetSimpleTypes()
                            where item.Name.EndsWith("_Flags") == false
                            select item;

                foreach (var item in query)
                {
                    this.ReadType(item);
                }
            }
            else
            {
                var query = from item in schema.GetSimpleTypes()
                            where item.Name.EndsWith(CremaSchema.FlagExtension) == false
                            select item;

                foreach (var item in query)
                {
                    if (item.QualifiedName.Name == typeof(Guid).GetTypeName() && item.QualifiedName.Namespace == schema.TargetNamespace)
                        continue;
                    this.ReadType(item);
                }
            }
        }

        /// <summary>
        /// for version 2.0
        /// </summary>
        private void ReadExtendedProperties(XmlSchemaAnnotated annotated, PropertyCollection properties)
        {
            var annotation = annotated.Annotation;
            if (annotation == null)
                return;

            for (var i = 0; i < annotation.Items.Count; i++)
            {
                var item = annotation.Items[i];
                if (item is XmlSchemaAppInfo == true)
                {
                    var appInfo = item as XmlSchemaAppInfo;

                    foreach (XmlNode xmlNode in appInfo.Markup)
                    {
                        if (xmlNode.Name != CremaSchema.ExtendedProperty)
                            continue;

                        var keyAttr = xmlNode.Attributes["key"];
                        if (keyAttr == null)
                            keyAttr = xmlNode.Attributes["name"];
                        var keyTypeAttr = xmlNode.Attributes["keyType"];
                        var valueTypeAttr = xmlNode.Attributes["valueType"];
                        if (valueTypeAttr == null)
                            valueTypeAttr = xmlNode.Attributes["type"];
                        var valueAttr = xmlNode.Attributes["value"];

                        try
                        {
                            var keyType = typeof(string);
                            if (keyTypeAttr != null)
                            {
                                keyType = Type.GetType(keyTypeAttr.Value);
                            }

                            var valueType = typeof(string);
                            if (valueTypeAttr != null)
                            {
                                valueType = Type.GetType(valueTypeAttr.Value);
                            }

                            object key = null;
                            object value = null;

                            if (keyType == typeof(string))
                            {
                                key = keyAttr.Value;
                            }
                            else
                            {
                                var converter = TypeDescriptor.GetConverter(keyType);
                                key = converter.ConvertFromString(keyAttr.Value);
                            }

                            if (valueType == typeof(string))
                            {
                                value = valueAttr.Value;
                            }
                            else
                            {
                                var converter = TypeDescriptor.GetConverter(valueType);
                                value = converter.ConvertFromString(valueAttr.Value);
                            }

                            properties.Add(key, value);
                        }
                        catch
                        {
                            properties.Add(keyAttr.Value, valueAttr.Value);
                        }
                    }
                }
            }
        }

        private void ReadKey(XmlSchemaKey key)
        {
            var dataTable = this.GetTable(key, CremaSchema.KeyTypeNameExtension);

            lock (CremaSchema.lockobj)
            {
                foreach (var item in key.GetFields())
                {
                    var columnName = item.XPath.Replace(CremaSchema.TableTypePrefix + ":", string.Empty);
                    var dataColumn = dataTable.Columns[columnName];

                    dataColumn.InternalIsKey = true;
                }
            }
        }

        private void ReadUnique(XmlSchemaUnique unique)
        {
            var dataTable = this.GetTable(unique, CremaSchema.UniqueTypeNameExtension);

            lock (CremaSchema.lockobj)
            {
                foreach (var item in unique.GetFields())
                {
                    var columnName = item.XPath.Replace(CremaSchema.TableTypePrefix + ":", string.Empty);
                    var dataColumn = dataTable.Columns[columnName];

                    dataColumn.InternalUnique = true;
                }
            }
        }

        private void ReadTableInfo(XmlSchemaComplexType complexType, CremaDataTable dataTable)
        {
            dataTable.InternalCreationInfo = complexType.ReadAppInfoAsSigunatureDate(CremaSchema.TableInfo, CremaSchema.Creator, CremaSchema.CreatedDateTime);
            dataTable.InternalModificationInfo = complexType.ReadAppInfoAsSigunatureDate(CremaSchema.TableInfo, CremaSchema.Modifier, CremaSchema.ModifiedDateTime);
            dataTable.InternalTableID = complexType.ReadAppInfoAsGuid(CremaSchema.TableInfo, CremaSchema.ID);
            dataTable.InternalTags = complexType.ReadAppInfoAsTagInfo(CremaSchema.TableInfo, CremaSchema.Tags);
            dataTable.InternalComment = complexType.ReadDescription();
        }

        private void ReadAttribute(XmlSchemaAttribute schemaAttribute, CremaDataTable dataTable)
        {
            if (schemaAttribute.Name == CremaSchema.RelationID || schemaAttribute.Name == CremaSchema.ParentID)
                return;

            var attributeName = schemaAttribute.Name == CremaSchemaObsolete.DataLocation ? CremaSchema.Tags : schemaAttribute.Name;
            var attribute = dataTable.Attributes[attributeName];

            if (attribute == null)
                attribute = dataTable.Attributes.Add(attributeName);

            if (schemaAttribute.Use == XmlSchemaUse.Required)
                attribute.AllowDBNull = false;

            if (string.IsNullOrEmpty(schemaAttribute.DefaultValue) == false)
                attribute.DefaultValue = CremaXmlConvert.ToValue(schemaAttribute.DefaultValue, attribute.DataType);

            attribute.AutoIncrement = schemaAttribute.ReadAppInfoAsBoolean(CremaSchema.AttributeInfo, CremaSchema.AutoIncrement);
            attribute.Comment = schemaAttribute.ReadAppInfoAsString(CremaSchema.AttributeInfo, CremaSchema.Comment);
        }

        private void ReadColumn(XmlSchemaElement element, CremaDataTable dataTable)
        {
            var dataColumn = new CremaDataColumn()
            {
                InternalColumnName = element.Name,
                InternalComment = element.ReadDescription(),
            };

            this.ReadColumnDataType(element.ElementSchemaType as XmlSchemaSimpleType, dataColumn);

            if (element.MinOccursString == null)
            {
                dataColumn.InternalAllowDBNull = false;
            }

            if (string.IsNullOrEmpty(element.DefaultValue) == false)
            {
                dataColumn.InternalDefaultValue = CremaXmlConvert.ToValue(element.DefaultValue, dataColumn.DataType);
            }

            if (this.version >= new Version(3, 0))
                this.ReadColumnInfo(element, dataColumn);
            else
                this.ReadColumnInfoVersion2(element, dataColumn);

            dataTable.Columns.Add(dataColumn);
        }

        private void ReadColumnInfo(XmlSchemaAnnotated annotated, CremaDataColumn dataColumn)
        {
            dataColumn.InternalCreationInfo = annotated.ReadAppInfoAsSigunatureDate(CremaSchema.ColumnInfo, CremaSchema.Creator, CremaSchema.CreatedDateTime);
            dataColumn.InternalModificationInfo = annotated.ReadAppInfoAsSigunatureDate(CremaSchema.ColumnInfo, CremaSchema.Modifier, CremaSchema.ModifiedDateTime);
            dataColumn.InternalAutoIncrement = annotated.ReadAppInfoAsBoolean(CremaSchema.ColumnInfo, CremaSchema.AutoIncrement);
            dataColumn.InternalColumnID = annotated.ReadAppInfoAsGuid(CremaSchema.ColumnInfo, CremaSchema.ID);
            dataColumn.InternalTags = annotated.ReadAppInfoAsTagInfo(CremaSchema.ColumnInfo, CremaSchema.Tags);
            dataColumn.InternalReadOnly = annotated.ReadAppInfoAsBoolean(CremaSchema.ColumnInfo, CremaSchema.ReadOnly);
        }

        private void ReadTable(XmlSchemaElement element, CremaDataTable dataTable)
        {
            var complexType = element.ElementSchemaType as XmlSchemaComplexType;
            if (dataTable.InternalName != string.Empty && dataTable.InternalName != element.Name)
                throw new CremaDataException("대상 테이블과 스키마 이름이 일치하지 않습니다.");
            dataTable.InternalName = element.Name;

            if (this.itemName != null)
            {
                dataTable.InternalName = this.itemName.Name;
                dataTable.InternalCategoryPath = this.itemName.CategoryPath;
            }
            else
            {
                if (this.version >= new Version(3, 0))
                {
                    if (element.QualifiedName.Namespace != CremaSchema.BaseNamespace)
                    {
                        dataTable.InternalName = CremaDataSet.GetTableName(this.dataSet, element.QualifiedName.Namespace);
                        dataTable.InternalCategoryPath = CremaDataSet.GetTableCategoryPath(this.dataSet, element.QualifiedName.Namespace);
                    }
                    else if (this.version == new Version(3, 0))
                    {
                        var categoryName = complexType.ReadAppInfoAsString(CremaSchema.TableInfo, CremaSchema.Category) ?? string.Empty;
                        var categoryPath = categoryName == string.Empty ? PathUtility.Separator : categoryName.WrapSeparator();
                        dataTable.InternalName = element.Name;
                        dataTable.InternalCategoryPath = categoryPath;
                    }
                    else
                    {
                        dataTable.InternalName = element.Name;
                        dataTable.InternalCategoryPath = complexType.ReadAppInfoAsString(CremaSchema.TableInfo, CremaSchema.CategoryPath, PathUtility.Separator);
                    }
                }
                else
                {
                    dataTable.InternalName = CremaDataSet.GetTableName(this.dataSet, element.QualifiedName.Namespace);
                    dataTable.InternalCategoryPath = CremaDataSet.GetTableCategoryPath(this.dataSet, element.QualifiedName.Namespace);
                }
            }

            dataTable.BeginLoadInternal();
            this.ReadTable(element.ElementSchemaType as XmlSchemaComplexType, dataTable);
            this.ReadChildTables(element.ElementSchemaType as XmlSchemaComplexType, dataTable);
            dataTable.EndLoadInternal();

            this.tables.Add(dataTable.Name, dataTable);
            foreach (var item in dataTable.Childs)
            {
                this.tables.Add(item.Name, item);
            }

            if (this.dataSet != null)
            {
                lock (CremaSchema.lockobj)
                {
                    this.dataSet.Tables.Add(dataTable);

                    if (complexType.ContentModel != null)
                    {
                        var contentModel = complexType.ContentModel as XmlSchemaComplexContent;
                        var content = contentModel.Content as XmlSchemaComplexContentExtension;
                        var templatedParentName = content.BaseTypeName.Name.Substring(0, content.BaseTypeName.Name.Length - CremaSchema.ComplexTypeNameExtension.Length);
                        var baseComplexType = content.GetBaseType();
                        var templateCategoryPath = PathUtility.Separator;
                        if (this.version <= new Version(3, 0))
                            templateCategoryPath = (baseComplexType.ReadAppInfoAsString(CremaSchema.TableInfo, CremaSchema.Category) ?? string.Empty).Wrap(PathUtility.SeparatorChar);
                        else
                            templateCategoryPath = baseComplexType.ReadAppInfoAsString(CremaSchema.TableInfo, CremaSchema.CategoryPath) ?? PathUtility.Separator;
                        var templateNamespace = this.dataSet.TableNamespace + templateCategoryPath + templatedParentName;

                        var templatedParent = this.dataSet.Tables[templatedParentName, templateCategoryPath];
                        if (templatedParent != null)
                        {
                            dataTable.AttachTemplatedParent(templatedParent);
                        }
                        else
                        {
                            dataTable.AttachTemplatedParent(templateNamespace);
                        }
                    }
                    else if (this.itemName != null)
                    {
                        var tableName = this.dataSet.GetTableName(element.QualifiedName.Namespace);
                        var categoryPath = this.dataSet.GetTableCategoryPath(element.QualifiedName.Namespace);
                        var templatedParent = this.dataSet.Tables[tableName, categoryPath];

                        if (dataTable != templatedParent)
                        {
                            if (templatedParent != null)
                            {
                                dataTable.AttachTemplatedParent(templatedParent);
                            }
                            else
                            {
                                dataTable.AttachTemplatedParent(element.QualifiedName.Namespace);
                            }
                        }
                    }
                    else if (complexType.ReadAppInfoAsString(CremaSchema.TableInfo, CremaSchema.TemplateNamespace) != null)
                    {
                        var templateNamespace = complexType.ReadAppInfoAsString(CremaSchema.TableInfo, CremaSchema.TemplateNamespace);
                        dataTable.AttachTemplatedParent(templateNamespace);
                    }
                    else
                    {
                        var query = from item in this.dataSet.Tables
                                    where item.Parent == null
                                    where item != dataTable
                                    where item.TemplateNamespace == dataTable.Namespace
                                    select item;

                        foreach (var item in query.ToArray())
                        {
                            item.AttachTemplatedParent(dataTable);
                        }
                    }

                    if (this.dataSet.Tables.Contains(dataTable.ParentName) == true)
                    {
                        dataTable.Parent = this.dataSet.Tables[dataTable.ParentName];
                    }

                    foreach (var item in this.dataSet.Tables)
                    {
                        if (item.ParentName == dataTable.Name && item.Parent == null)
                        {
                            item.Parent = dataTable;
                        }
                    }
                }
            }
        }

        private void ReadDataSet(XmlSchemaElement element)
        {
            var complexType = element.ElementSchemaType as XmlSchemaComplexType;

            foreach (var item in complexType.GetSequenceElements())
            {
                this.ReadTable(item, new CremaDataTable());
            }

            foreach (var item in element.GetKeyConstraints())
            {
                this.ReadKey(item);
            }

            foreach (var item in element.GetUniqueConstraints())
            {
                this.ReadUnique(item);
            }
        }

        private void ReadDataTable(XmlSchemaElement element)
        {
            var complexType = element.ElementSchemaType as XmlSchemaComplexType;

            var tableElement = complexType.GetSequenceElements().Single();

            this.ReadTable(tableElement, this.dataTable);

            foreach (var item in element.GetKeyConstraints())
            {
                this.ReadKey(item);
            }

            foreach (var item in element.GetUniqueConstraints())
            {
                this.ReadUnique(item);
            }
        }

        private void ReadTable(XmlSchemaComplexType complexType, CremaDataTable dataTable)
        {
            if (this.version >= new Version(3, 0))
                this.ReadTableInfo(complexType, dataTable);
            else
                this.ReadTableInfoVersion2(complexType, dataTable);

            foreach (var item in complexType.GetAttributes())
            {
                this.ReadAttribute(item, dataTable);
            }

            foreach (var item in complexType.GetSequenceElements())
            {
                if (item.ElementSchemaType is XmlSchemaSimpleType)
                {
                    this.ReadColumn(item, dataTable);
                }
            }

            dataTable.SchemaHashValue = this.hashValue;
        }

        private void ReadChildTables(XmlSchemaComplexType complexType, CremaDataTable dataTable)
        {
            foreach (var item in complexType.GetSequenceElements())
            {
                if (item.ElementSchemaType is XmlSchemaSimpleType == false)
                {
                    var tableName = item.Name;
                    var childTable = new CremaDataTable(tableName, dataTable.CategoryPath);
                    childTable.BeginLoadInternal();
                    this.ReadTable(item.ElementSchemaType as XmlSchemaComplexType, childTable);
                    childTable.EndLoadInternal();
                    childTable.Parent = dataTable;
                }
            }
        }

        private void ReadColumnDataType(XmlSchemaSimpleType simpleType, CremaDataColumn column)
        {
            var typeName = simpleType.QualifiedName.Name;

            if (simpleType.QualifiedName.Namespace == XmlSchema.Namespace)
            {
                column.InternalDataType = CremaDataTypeUtility.GetType(typeName) ?? typeof(string);
            }
            else if (simpleType.QualifiedName.Name == typeof(Guid).GetTypeName() && simpleType.QualifiedName.Namespace == simpleType.GetSchema().TargetNamespace)
            {
                column.InternalDataType = typeof(Guid);
            }
            else
            {
                var categoryPath = PathUtility.Separator;

                if (simpleType.QualifiedName.Namespace == CremaSchema.BaseNamespace)
                {
                    if (this.version >= new Version(3, 5))
                    {
                        categoryPath = simpleType.ReadAppInfoAsString(CremaSchema.TypeInfo, CremaSchema.CategoryPath, PathUtility.Separator);
                    }
                    else
                    {
                        var xmlRestriction = simpleType.Content as XmlSchemaSimpleTypeRestriction;
                        if (xmlRestriction == null)
                        {
                            if (simpleType.Content is XmlSchemaSimpleTypeList == true)
                            {
                                simpleType = (simpleType.Content as XmlSchemaSimpleTypeList).BaseItemType;
                            }
                        }
                        var categoryName = simpleType.ReadAppInfoAsString(CremaSchema.TypeInfo, CremaSchema.Category) ?? string.Empty;
                        categoryPath = categoryName == string.Empty ? PathUtility.Separator : categoryName.WrapSeparator();
                    }
                }
                else
                {
                    if (this.version >= new Version(3, 0))
                    {
                        categoryPath = this.dataSet.GetTypeCategoryPath(simpleType.QualifiedName.Namespace);
                    }
                    else
                    {
                        categoryPath = PathUtility.Separator;
                    }
                }

                if (this.dataSet.Types.Contains(typeName, categoryPath) == false)
                {
                    this.ReadType(simpleType);
                }

                column.InternalCremaType = (InternalDataType)this.dataSet.Types[typeName, categoryPath];
            }
        }

        private void ReadType(XmlSchemaSimpleType simpleType)
        {
            if (this.dataSet != null && this.dataSet.Types.Contains(simpleType.Name) == true)
                return;

            var contentType = simpleType;
            var restriction = simpleType.Content as XmlSchemaSimpleTypeRestriction;
            var dataType = this.type ?? new CremaDataType();
            dataType.InternalName = simpleType.Name;
            dataType.BeginLoadData();

            if (restriction == null && simpleType.Content is XmlSchemaSimpleTypeList == true)
            {
                contentType = (simpleType.Content as XmlSchemaSimpleTypeList).BaseItemType;
                restriction = contentType.Content as XmlSchemaSimpleTypeRestriction;
                dataType.IsFlag = true;
            }

            this.ReadTypeMembers(restriction, dataType);
            this.ReadTypeInfo(simpleType, dataType);

            dataType.EndLoadData();
            dataType.AcceptChanges();

            if (simpleType.GetSchema() != this.schema)
            {
                if (Uri.TryCreate(simpleType.GetSchema().SourceUri, UriKind.Absolute, out Uri sourceUri))
                {
                    if (File.Exists(sourceUri.LocalPath) == true)
                        dataType.HashValue = HashUtility.GetHashValueFromFile(sourceUri.LocalPath);
                }
            }
            else
            {
                dataType.HashValue = this.hashValue;
            }

            if (this.dataSet != null)
            {
                this.dataSet.Types.Add(dataType);
            }
        }

        private void ReadTypeInfo(XmlSchemaSimpleType simpleType, CremaDataType dataType)
        {
            var contentType = simpleType;
            if (simpleType.Content as XmlSchemaSimpleTypeRestriction == null && simpleType.Content is XmlSchemaSimpleTypeList == true)
            {
                contentType = (simpleType.Content as XmlSchemaSimpleTypeList).BaseItemType;
            }

            if (this.version < new Version(3, 0))
            {
                dataType.Comment = contentType.ReadDescription();
                this.ReadTypeInfoVersion2(contentType, dataType);
                return;
            }
            else if (this.version == new Version(3, 0))
            {
                simpleType = contentType;
            }

            dataType.InternalCreationInfo = simpleType.ReadAppInfoAsSigunatureDate(CremaSchema.TypeInfo, CremaSchema.Creator, CremaSchema.CreatedDateTime);
            dataType.InternalModificationInfo = simpleType.ReadAppInfoAsSigunatureDate(CremaSchema.TypeInfo, CremaSchema.Modifier, CremaSchema.ModifiedDateTime);
            dataType.InternalTypeID = simpleType.ReadAppInfoAsGuidVersion2(CremaSchema.TypeInfo, CremaSchema.ID, dataType.TypeName);
            if (this.version == new Version(3, 0))
            {
                var category = simpleType.ReadAppInfoAsString(CremaSchema.TypeInfo, CremaSchema.Category);
                if (string.IsNullOrEmpty(category) == true)
                    dataType.InternalCategoryPath = PathUtility.Separator;
                else
                    dataType.InternalCategoryPath = category.Wrap(PathUtility.SeparatorChar);
            }
            else
            {
                dataType.InternalCategoryPath = (simpleType.ReadAppInfoAsString(CremaSchema.TypeInfo, CremaSchema.CategoryPath) ?? PathUtility.Separator);
            }
            dataType.InternalComment = simpleType.ReadDescription();

            if (this.version > new Version(3, 0))
            {
                dataType.InternalTags = simpleType.ReadAppInfoAsTagInfo(CremaSchema.TypeInfo, CremaSchema.Tags);
            }
        }

        private void ReadTypeMembers(XmlSchemaSimpleTypeRestriction restriction, CremaDataType dataType)
        {
            foreach (var item in restriction.Facets)
            {
                if (item is XmlSchemaEnumerationFacet)
                {
                    this.ReadTypeMember(item as XmlSchemaEnumerationFacet, dataType);
                }
            }
        }

        private void ReadTypeMember(XmlSchemaEnumerationFacet facet, CremaDataType dataType)
        {
            if (facet.Annotation == null)
                return;

            var member = dataType.NewMember();
            member.Name = facet.Value;
            member.Comment = facet.ReadDescription();
            member.Value = facet.ReadAppInfoComfortableAsInt64(CremaSchema.TypeInfo, CremaSchema.Value, CremaSchemaObsolete.ValueObsolete);
            member.CreationInfo = facet.ReadAppInfoAsSigunatureDate(CremaSchema.TypeInfo, CremaSchema.Creator, CremaSchema.CreatedDateTime);
            member.ModificationInfo = facet.ReadAppInfoAsSigunatureDate(CremaSchema.TypeInfo, CremaSchema.Modifier, CremaSchema.ModifiedDateTime);
            member.IsEnabled = facet.ReadAppInfoAsBoolean(CremaSchema.TypeInfo, CremaSchema.Enable, true);
            member.MemberID = facet.ReadAppInfoAsGuidVersion2(CremaSchema.TypeInfo, CremaSchema.ID, dataType.Name + "_" + member.Name);
            member.Tags = facet.ReadAppInfoAsTagInfo(CremaSchema.TypeInfo, CremaSchema.Tags);
            dataType.Members.Add(member);
        }

        private CremaDataTable GetTable(XmlSchemaKey constraint, string extension)
        {
            if (this.version >= new Version(3, 0))
            {
                var keyName = constraint.Name.Substring(0, constraint.Name.Length - extension.Length);
                if (this.itemName == null)
                {
                    return this.tables[keyName];
                }
                else
                {
                    var tableName = CremaDataSet.GetTableName(this.dataSet, constraint.QualifiedName.Namespace);

                    if (keyName == tableName)
                    {
                        return this.tables[this.itemName.Name];
                    }
                    else
                    {
                        tableName = Regex.Replace(keyName, string.Format("(^{0})([.].*)", tableName), this.itemName.Name + "$2");
                        return this.tables[tableName];
                    }
                }
            }
            else
            {
                var keyName = GetTableNameObsolete(constraint);

                if (this.itemName == null)
                {
                    var tableName = CremaDataSet.GetTableName(CremaSchemaObsolete.TableNamespaceObsolete, constraint.QualifiedName.Namespace);
                    var categoryPath = CremaDataSet.GetTableCategoryPath(CremaSchemaObsolete.TableNamespaceObsolete, constraint.QualifiedName.Namespace);
                    if (keyName == tableName)
                    {
                        return this.dataSet.Tables[tableName, categoryPath];
                    }
                    else
                    {
                        return this.dataSet.Tables[tableName + "." + keyName, categoryPath];
                    }
                }
                else
                {
                    var tableName = CremaDataSet.GetTableName(CremaSchemaObsolete.TableNamespaceObsolete, constraint.QualifiedName.Namespace);
                    var categoryPath = CremaDataSet.GetTableCategoryPath(CremaSchemaObsolete.TableNamespaceObsolete, constraint.QualifiedName.Namespace);
                    if (keyName == tableName)
                    {
                        return this.dataSet.Tables[itemName.Name, itemName.CategoryPath];
                    }
                    else
                    {
                        return this.dataSet.Tables[itemName.Name + "." + keyName, itemName.CategoryPath];
                    }
                }
            }
            throw new CremaDataException();
        }

        private CremaDataTable GetTable(XmlSchemaUnique constraint, string extension)
        {
            if (this.version >= new Version(3, 0))
            {
                var keyName = constraint.Name.Substring(0, constraint.Name.Length - extension.Length);
                if (this.version >= new Version(3, 5)) // 3.5
                {
                    var items = StringUtility.Split(keyName, '.');
                    keyName = string.Join(".", items.Take(items.Length - 1));
                }

                if (this.itemName == null)
                {
                    return this.tables[keyName];
                }
                else
                {
                    var tableName = CremaDataSet.GetTableName(this.dataSet, constraint.QualifiedName.Namespace);

                    if (keyName == tableName)
                    {
                        return this.tables[this.itemName.Name];
                    }
                    else
                    {
                        tableName = Regex.Replace(keyName, string.Format("(^{0})([.].*)", tableName), this.itemName.Name + "$2");
                        return this.tables[tableName];
                    }
                }
            }
            else
            {
                var keyName = GetTableNameObsolete(constraint);

                if (this.itemName == null)
                {
                    var tableName = CremaDataSet.GetTableName(CremaSchemaObsolete.TableNamespaceObsolete, constraint.QualifiedName.Namespace);
                    var categoryPath = CremaDataSet.GetTableCategoryPath(CremaSchemaObsolete.TableNamespaceObsolete, constraint.QualifiedName.Namespace);
                    if (keyName == tableName)
                    {
                        return this.dataSet.Tables[tableName, categoryPath];
                    }
                    else
                    {
                        return this.dataSet.Tables[tableName + "." + keyName, categoryPath];
                    }
                }
                else
                {
                    var tableName = CremaDataSet.GetTableName(CremaSchemaObsolete.TableNamespaceObsolete, constraint.QualifiedName.Namespace);
                    var categoryPath = CremaDataSet.GetTableCategoryPath(CremaSchemaObsolete.TableNamespaceObsolete, constraint.QualifiedName.Namespace);
                    if (keyName == tableName)
                    {
                        return this.dataSet.Tables[itemName.Name, itemName.CategoryPath];
                    }
                    else
                    {
                        return this.dataSet.Tables[itemName.Name + "." + keyName, itemName.CategoryPath];
                    }
                }
            }
            throw new CremaDataException();
        }

        private string GetTableNameObsolete(XmlSchemaIdentityConstraint key)
        {
            var xPath = key.Selector.XPath;
            var strArray = xPath.Split(new char[] { '/', ':' });
            var name = strArray[strArray.Length - 1];
            if ((name == null) || (name.Length == 0))
            {
                throw new CremaDataException();
            }
            return XmlConvert.DecodeName(name);
        }

        [Obsolete("for 1.0")]
        /// <summary>
        /// for 1.0
        /// </summary>
        private bool ReadTableInfoVersion1(XmlSchemaElement element, CremaDataTable dataTable)
        {
            var modifier = element.ReadAppInfoAsString(CremaSchema.TableInfo, CremaSchema.Modifier);
            if (modifier != null)
            {
                string modifiedDateTime = element.ReadAppInfoAsString(CremaSchema.TableInfo, CremaSchema.ModifiedDateTime);
                if (DateTime.TryParse(modifiedDateTime, out DateTime dateTime) == true)
                {
                    dataTable.InternalModificationInfo = new SignatureDate(modifier, dateTime);
                }
            }

            var creator = element.ReadAppInfoAsString(CremaSchema.TableInfo, CremaSchema.Creator);
            if (creator == null)
                creator = element.ReadAppInfoAsString(CremaSchema.TableInfo, CremaSchemaObsolete.CreatorObsolete);
            if (creator != null)
            {
                var createdDateTime = element.ReadAppInfoAsString(CremaSchema.TableInfo, CremaSchema.CreatedDateTime);
                if (DateTime.TryParse(createdDateTime, out DateTime dateTime) == true)
                {
                    dataTable.InternalCreationInfo = new SignatureDate(creator, dateTime);
                }
            }

            if (modifier != null || creator != null)
            {
                var properties = new PropertyCollection();
                this.ReadExtendedProperties(element, properties);
                if (properties.ContainsKey(CremaSchemaObsolete.DataLocation) == true)
                {
                    dataTable.InternalTags = new TagInfo(properties[CremaSchemaObsolete.DataLocation] as string);
                    properties.Remove(CremaSchemaObsolete.DataLocation);
                }
                foreach (DictionaryEntry item in properties)
                {
                    dataTable.ExtendedProperties.Add(item.Key, item.Value);
                }
            }

            return modifier != null || creator != null;
        }

        //[Obsolete("for 2.0")]
        /// <summary>
        /// for version 2.0
        /// </summary>
        private void ReadTableInfoVersion2(XmlSchemaComplexType complexType, CremaDataTable dataTable)
        {
            string textValue;

            dataTable.InternalCreationInfo = complexType.ReadAppInfoAsSigunatureDate(CremaSchema.TableInfo, CremaSchemaObsolete.CreatorObsolete, CremaSchema.CreatedDateTime);
            dataTable.InternalModificationInfo = complexType.ReadAppInfoAsSigunatureDate(CremaSchema.TableInfo, CremaSchema.Modifier, CremaSchema.ModifiedDateTime);

            textValue = complexType.ReadAppInfoAsString(CremaSchema.TableInfo, CremaSchema.ID);
            if (textValue != null)
                dataTable.InternalTableID = Guid.Parse(textValue);
            else
                dataTable.InternalTableID = GuidUtility.FromName(dataTable.Name);

            dataTable.InternalTags = complexType.ReadAppInfoAsTagInfo(CremaSchema.TableInfo, CremaSchema.Tags);
            dataTable.InternalComment = complexType.ReadDescription();

            var properties = new PropertyCollection();
            this.ReadExtendedProperties(complexType, properties);
            if (properties.ContainsKey(CremaSchemaObsolete.DataLocation) == true)
            {
                dataTable.InternalTags = new TagInfo(properties[CremaSchemaObsolete.DataLocation] as string);
                properties.Remove(CremaSchemaObsolete.DataLocation);
            }
        }

        /// <summary>
        /// for version 2.0
        /// </summary>
        private void ReadTypeInfoVersion2(XmlSchemaSimpleType simpleType, CremaDataType dataType)
        {
            dataType.InternalCreationInfo = simpleType.ReadAppInfoAsSigunatureDate(CremaSchema.TypeInfo, CremaSchemaObsolete.CreatorObsolete, CremaSchema.CreatedDateTime);
            dataType.InternalModificationInfo = simpleType.ReadAppInfoAsSigunatureDate(CremaSchema.TypeInfo, CremaSchema.Modifier, CremaSchema.ModifiedDateTime);
        }

        /// <summary>
        /// for version 2.0
        /// </summary>
        private void ReadColumnInfoVersion2(XmlSchemaAnnotated annotated, CremaDataColumn dataColumn)
        {
            dataColumn.InternalCreationInfo = annotated.ReadAppInfoAsSigunatureDate(CremaSchema.ColumnInfo, CremaSchemaObsolete.CreatorObsolete, CremaSchema.CreatedDateTime);
            dataColumn.InternalModificationInfo = annotated.ReadAppInfoAsSigunatureDate(CremaSchema.ColumnInfo, CremaSchema.Modifier, CremaSchema.ModifiedDateTime);
            dataColumn.InternalAutoIncrement = annotated.ReadAppInfoAsBoolean(CremaSchema.ColumnInfo, CremaSchema.AutoIncrement);
            dataColumn.InternalColumnID = annotated.ReadAppInfoAsGuidVersion2(CremaSchema.ColumnInfo, CremaSchema.ID, dataColumn.ColumnName);
            dataColumn.InternalTags = annotated.ReadAppInfoAsTagInfo(CremaSchema.ColumnInfo, CremaSchema.Tags);

            var properties = new PropertyCollection();
            this.ReadExtendedProperties(annotated, properties);
            if (properties.ContainsKey(CremaSchemaObsolete.DataLocation) == true)
            {
                dataColumn.InternalTags = new TagInfo(properties[CremaSchemaObsolete.DataLocation] as string);
                properties.Remove(CremaSchemaObsolete.DataLocation);
            }
        }
    }
}
