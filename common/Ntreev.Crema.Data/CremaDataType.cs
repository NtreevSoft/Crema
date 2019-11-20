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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.Schema;
using System.Threading;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Data.Xml;
using Ntreev.Crema.Data;
using Ntreev.Library;
using Ntreev.Crema.Data.Properties;
using System.IO;
using Ntreev.Library.IO;
using System.Data;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Ntreev.Library.ObjectModel;
using System.Text;
using System.Collections;
using Ntreev.Library.Linq;
using System.Security.Cryptography;

namespace Ntreev.Crema.Data
{
    [Serializable]
    public class CremaDataType : IListSource, IXmlSerializable, ISerializable, INotifyPropertyChanged
    {
        private static readonly CremaDataTypeMember[] emptyMembers = new CremaDataTypeMember[] { };

        private bool isEventsAttached;

        private readonly InternalDataType type;
        internal readonly CremaDataTypeMemberBuilder builder;
        private readonly CremaDataTypeMemberCollection members;
        private readonly CremaAttributeCollection attributes;

        public CremaDataType()
            : this(string.Empty)
        {

        }

        public CremaDataType(string name)
            : this(name, PathUtility.Separator)
        {

        }

        public CremaDataType(string name, string categoryPath)
        {
            this.builder = new CremaDataTypeMemberBuilder(this);
            this.type = new InternalDataType(this, name, categoryPath);
            this.attributes = new CremaAttributeCollection(this.type);
            this.members = new CremaDataTypeMemberCollection(this.type);

            this.AttachEventHandlers();
        }

        protected CremaDataType(SerializationInfo info, StreamingContext context)
            : this((TypeInfo)info.GetValue("Info", typeof(TypeInfo)))
        {

        }

        internal CremaDataType(InternalDataType type)
        {
            this.type = type;
            this.builder = new CremaDataTypeMemberBuilder(this);
            this.attributes = new CremaAttributeCollection(this.type);
            this.members = new CremaDataTypeMemberCollection(this.type);

            this.AttachEventHandlers();
        }

        private CremaDataType(TypeInfo typeInfo)
        {
            this.builder = new CremaDataTypeMemberBuilder(this);
            this.type = new InternalDataType(this, typeInfo.Name, typeInfo.CategoryPath)
            {
                IsFlag = typeInfo.IsFlag,
                Comment = typeInfo.Comment,
                Tags = typeInfo.Tags,
            };
            this.attributes = new CremaAttributeCollection(this.type);
            this.members = new CremaDataTypeMemberCollection(this.type);
            this.type.CreationInfo = typeInfo.CreationInfo;
            this.type.ModificationInfo = typeInfo.ModificationInfo;

            foreach (var item in typeInfo.Members)
            {
                var member = this.NewMember();
                member.Name = item.Name;
                member.Value = item.Value;
                member.Comment = item.Comment;
                member.IsEnabled = item.IsEnabled;
                member.Tags = item.Tags;
                this.Members.Add(member);
            }
            this.type.AcceptChanges();
            this.AttachEventHandlers();
        }

        public static CremaDataType Create(SignatureDateProvider modificationProvider)
        {
            var dataType = new CremaDataType() { SignatureDateProvider = modificationProvider };
            dataType.InternalCreationInfo = dataType.InternalModificationInfo = modificationProvider.Provide();
            return dataType;
        }

        public static string GenerateHashValue(params TypeMemberInfo[] members)
        {
            return InternalDataType.GenerateHashValue(members);
        }

        /// <summary>
        /// 2개 단위로 인자를 설정해야 함
        /// name, value
        /// </summary>
        public static string GenerateHashValue(params object[] args)
        {
            return InternalDataType.GenerateHashValue(args);
        }

        public override string ToString()
        {
            return this.type.ToString();
        }

        public void Clear()
        {
            this.type.Clear();
        }

        public CremaDataTypeMember NewMember()
        {
            var dataRow = this.type.NewRow() as InternalDataTypeMember;
            return dataRow.Target;
        }

        public CremaDataTypeMember AddMember(string name, long value)
        {
            return this.AddMember(name, value, string.Empty);
        }

        public CremaDataTypeMember AddMember(string name, long value, string comment)
        {
            var member = (this.type.NewRow() as InternalDataTypeMember).Target;
            member.Name = name;
            member.Value = value;
            member.Comment = comment;

            this.members.Add(member);
            return member;
        }

        public void ImportMember(CremaDataTypeMember member)
        {
            this.type.ImportRow((InternalDataTypeMember)member);
        }

        public void AcceptChanges()
        {
            this.type.AcceptChanges();
        }

        public void RejectChanges()
        {
            this.type.RejectChanges();
        }

        public bool HasChanges()
        {
            return this.HasChanges(false);
        }

        public bool HasChanges(bool isComparable)
        {
            for (var i = 0; i < this.type.Rows.Count; i++)
            {
                var dataRow = this.type.Rows[i];
                if (dataRow.RowState == DataRowState.Unchanged)
                    continue;
                if (isComparable == false)
                    return true;
                if (CompareDataRow(dataRow) == true)
                    return true;
            }

            return false;

            bool CompareDataRow(DataRow dataRow)
            {
                if (dataRow.HasVersion(DataRowVersion.Original) == false)
                    return false;

                for (var i = 0; i < dataRow.Table.Columns.Count; i++)
                {
                    if (object.Equals(dataRow[i], dataRow[i, DataRowVersion.Original]) == false)
                        return true;
                }
                return false;
            }
        }

        public string ConvertToString(long value)
        {
            return this.type.ConvertToString(value);
        }

        public long ConvertFromString(string textValue)
        {
            return this.type.ConvertFromString(textValue);
        }

        public bool VerifyValue(string textValue)
        {
            if (this.IsFlag == true)
            {
                var items = StringUtility.Split(textValue);
                foreach (var item in items)
                {
                    if (this.members.Contains(item) == false)
                        return false;
                }
            }
            else
            {
                if (this.members.Contains(textValue) == false)
                    return false;
            }
            return true;
        }

        public void ValidateValue(string textValue)
        {
            if (this.IsFlag == true)
            {
                var items = StringUtility.Split(textValue);

                foreach (var item in items)
                {
                    if (this.members.Contains(item) == false)
                        throw new FormatException(string.Format(Resources.Exception_CannotParseEnum_Format, item, this.TypeName));
                }
            }
            else
            {
                if (this.type.columnName.AllowDBNull == false && this.members.Contains(textValue) == false)
                    throw new FormatException(string.Format(Resources.Exception_CannotParseEnum_Format, textValue, this.TypeName));
            }
        }

        public CremaDataTypeMember[] Select()
        {
            return this.type.Select().Cast<InternalDataTypeMember>().Select(item => item.Target).ToArray();
        }

        public CremaDataTypeMember[] Select(string filterExpression)
        {
            return this.type.Select(filterExpression).Cast<InternalDataTypeMember>().Select(item => item.Target).ToArray();
        }

        public CremaDataTypeMember[] Select(string filterExpression, string sort)
        {
            return this.type.Select(filterExpression, sort).Cast<InternalDataTypeMember>().Select(item => item.Target).ToArray();
        }

        public CremaDataTypeMember[] Select(string filterExpression, string sort, DataViewRowState recordStates)
        {
            return this.type.Select(filterExpression, sort, recordStates).Cast<InternalDataTypeMember>().Select(item => item.Target).ToArray();
        }

        public void Write(Stream stream)
        {
            var writer = new CremaSchemaWriter(this);
            writer.Write(stream);
        }

        public void Write(TextWriter textWriter)
        {
            var writer = new CremaSchemaWriter(this);
            writer.Write(textWriter);
        }

        public void Write(XmlWriter xmlWriter)
        {
            var writer = new CremaSchemaWriter(this);
            writer.Write(xmlWriter);
        }

        public void Write(string filename)
        {
            var writer = new CremaSchemaWriter(this);
            writer.Write(filename);
        }

        public CremaDataType CopyTo(CremaDataSet dataSet)
        {
            this.ValidateCopyTo(dataSet);

            var xmlSchema = this.GetXmlSchema();
            using (var sr = new StringReader(xmlSchema))
            {
                dataSet.ReadType(sr);
            }
            return dataSet.Types[this.TypeName, this.CategoryPath];
        }

        public static CremaDataType ReadSchema(TextReader reader)
        {
            var dataType = new CremaDataType();
            var schemaReader = new CremaSchemaReader(dataType);
            schemaReader.Read(reader);
            return dataType;
        }

        public static CremaDataType ReadSchema(XmlReader reader)
        {
            var dataType = new CremaDataType();
            var schemaReader = new CremaSchemaReader(dataType);
            schemaReader.Read(reader);
            return dataType;
        }

        public static CremaDataType ReadSchema(string filename)
        {
            var dataType = new CremaDataType();
            var schemaReader = new CremaSchemaReader(dataType);
            schemaReader.Read(filename);
            return dataType;
        }

        public static CremaDataType ReadSchema(Stream stream)
        {
            var dataType = new CremaDataType();
            var schemaReader = new CremaSchemaReader(dataType);
            schemaReader.Read(stream);
            return dataType;
        }

        public string GetXmlSchema()
        {
            using (var sw = new Utf8StringWriter())
            {
                this.Write(sw);
                return sw.ToString();
            }
        }

        public void BeginLoadData()
        {
            this.type.BeginLoadData();
        }

        public void EndLoadData()
        {
            this.type.ValidateName();
            this.type.EndLoadData();
        }

        public void UpdateRevision(long revision)
        {
            this.type.Revision = revision;
        }

        public bool IsFlag
        {
            get { return this.type.IsFlag; }
            set { this.type.IsFlag = value; }
        }

        public bool ReadOnly
        {
            get { return this.type.ReadOnly; }
            set
            {
                this.type.ReadOnly = value;
                this.InvokePropertyChangedEvent(nameof(this.ReadOnly));
            }
        }

        public TagInfo Tags
        {
            get { return this.type.Tags; }
            set { this.type.Tags = value; }
        }

        public TagInfo DerivedTags
        {
            get { return this.type.DerivedTags; }
            set { this.type.DerivedTags = value; }
        }

        public string Name
        {
            get { return this.TypeName; }
        }

        public string TypeName
        {
            get { return this.type.LocalName; }
            set
            {
                this.type.LocalName = value;
                this.InvokePropertyChangedEvent(nameof(this.TypeName), nameof(this.Name), nameof(this.CategoryPath), nameof(this.Namespace));
            }
        }

        public CremaAttributeCollection Attributes
        {
            get { return this.attributes; }
        }

        public CremaDataTypeMemberCollection Members
        {
            get { return this.members; }
        }

        public string Comment
        {
            get { return this.type.Comment; }
            set
            {
                this.type.Comment = value;
                this.InvokePropertyChangedEvent(nameof(this.Comment));
            }
        }

        public SignatureDate CreationInfo
        {
            get { return this.type.CreationInfo; }
            set
            {
                this.type.CreationInfo = value;
                this.InvokePropertyChangedEvent(nameof(this.CreationInfo));
            }
        }

        public SignatureDate ModificationInfo
        {
            get { return this.type.ModificationInfo; }
            set
            {
                this.type.ModificationInfo = value;
                this.InvokePropertyChangedEvent(nameof(this.ModificationInfo));
            }
        }

        public string CategoryPath
        {
            get { return this.type.CategoryPath; }
            set
            {
                this.type.CategoryPath = value;
                this.InvokePropertyChangedEvent(nameof(this.CategoryPath), nameof(this.Path), nameof(this.Namespace));
            }
        }

        public string Path
        {
            get { return this.CategoryPath + this.TypeName; }
        }

        [DefaultValue(50)]
        public int MinimumCapacity
        {
            get { return this.type.MinimumCapacity; }
            set { this.type.MinimumCapacity = value; }
        }

        public string Namespace
        {
            get { return this.type.Namespace; }
        }

        public TypeInfo TypeInfo
        {
            get { return this.type.TypeInfo; }
        }

        public long Revision => this.type.Revision;

        public CremaDataSet DataSet
        {
            get
            {
                if (this.type.DataSet is InternalDataSet dataSet)
                    return dataSet.Target;
                return null;
            }
        }

        public SignatureDateProvider SignatureDateProvider
        {
            get { return this.type.SignatureDateProvider; }
            set { this.type.SignatureDateProvider = value; }
        }

        public CremaDataColumn[] ReferencedColumns
        {
            get { return this.type.ReferenceList.Select(item => item.Target).ToArray(); }
        }

        public bool IsDirty
        {
            get
            {
                foreach (var item in this.members)
                {
                    if (item.InternalObject.RowState != DataRowState.Unchanged)
                        return true;
                }
                return false;
            }
        }

        [Browsable(false)]
        public DataView View
        {
            get { return this.type.DefaultView; }
        }

        [Browsable(false)]
        public PropertyCollection ExtendedProperties
        {
            get { return this.type.ExtendedProperties; }
        }

        public Guid TypeID
        {
            get { return this.InternalTypeID; }
        }

        public string HashValue { get; internal set; }

        public static readonly string[] FieldNames = new string[] { CremaSchema.Name, CremaSchema.Value, CremaSchema.Comment };

        public event CremaTypeMemberChangeEventHandler MemberChanging;

        public event CremaTypeMemberChangeEventHandler MemberChanged;

        public event CremaTypeMemberChangeEventHandler MemberDeleted;

        public event CremaTypeMemberChangeEventHandler MemberDeleting;

        public event CremaTypeClearEventHandler Cleared;

        public event CremaTypeClearEventHandler Clearing;

        public event CremaDataTableNewRowEventHandler TypeNewMember;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual CremaDataTypeMember NewTypeMemberFromBuilder(CremaDataTypeMemberBuilder builder)
        {
            return new CremaDataTypeMember(builder);
        }

        protected virtual void OnMemberChanged(CremaDataTypeMemberChangeEventArgs e)
        {
            this.MemberChanged?.Invoke(this, e);
        }

        protected virtual void OnMemberChanging(CremaDataTypeMemberChangeEventArgs e)
        {
            this.MemberChanging?.Invoke(this, e);
        }

        protected virtual void OnMemberDeleted(CremaDataTypeMemberChangeEventArgs e)
        {
            this.MemberDeleted?.Invoke(this, e);
        }

        protected virtual void OnMemberDeleting(CremaDataTypeMemberChangeEventArgs e)
        {
            this.MemberDeleting?.Invoke(this, e);
        }

        protected virtual void OnCleared(CremaDataTypeClearEventArgs e)
        {
            this.Cleared?.Invoke(this, e);
        }

        protected virtual void OnClearing(CremaDataTypeClearEventArgs e)
        {
            this.Clearing?.Invoke(this, e);
        }

        protected virtual void OnTypeNewMember(CremaDataTableNewRowEventArgs e)
        {
            this.TypeNewMember?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        private void InvokePropertyChangedEvent(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Table_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            this.OnMemberChanged(new CremaDataTypeMemberChangeEventArgs(e));
        }

        private void Table_ColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            this.OnMemberChanging(new CremaDataTypeMemberChangeEventArgs(e));
        }

        private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e is InternalDataRowChangeEventArgs == false)
            {
                this.OnMemberChanged(new CremaDataTypeMemberChangeEventArgs(e));
            }
        }

        private void Table_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            if (e is InternalDataRowChangeEventArgs == false)
            {
                this.OnMemberChanging(new CremaDataTypeMemberChangeEventArgs(e));
            }
        }

        private void Table_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            this.OnMemberDeleted(new CremaDataTypeMemberChangeEventArgs(e));
        }

        private void Table_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            this.OnMemberDeleting(new CremaDataTypeMemberChangeEventArgs(e));
        }

        private void Table_TableCleared(object sender, DataTableClearEventArgs e)
        {
            this.OnCleared(new CremaDataTypeClearEventArgs(e));
        }

        private void Table_TableClearing(object sender, DataTableClearEventArgs e)
        {
            this.OnClearing(new CremaDataTypeClearEventArgs(e));
        }

        private void ValidateCopyTo(CremaDataSet dataSet)
        {
            if (this.DataSet == dataSet)
                throw new CremaDataException(Resources.Exception_CannotCopyToSameDataSet);
            if (dataSet.Types.Contains(this.TypeName, this.CategoryPath) == true)
                throw new CremaDataException(this.TypeName);
        }

        private void ValidateSetTypeName(string value)
        {
            if (this.DataSet != null && CremaDataSet.VerifyName(value) == false)
                throw new CremaDataException(Resources.Exception_InvalidName);
            if (this.DataSet == null && string.IsNullOrEmpty(value) == false && CremaDataSet.VerifyName(value) == false)
                throw new CremaDataException(Resources.Exception_InvalidName);
            if (this.DataSet != null && this.DataSet.Types.Contains(value) == true)
                throw new CremaDataException(Resources.Exception_AreadyExistedDataType_Format, value);
        }

        internal void AttachEventHandlers()
        {
            if (this.isEventsAttached == true)
                throw new Exception();
            this.type.ColumnChanging += Table_ColumnChanging;
            this.type.ColumnChanged += Table_ColumnChanged;
            this.type.RowChanging += Table_RowChanging;
            this.type.RowChanged += Table_RowChanged;
            this.type.RowDeleted += Table_RowDeleted;
            this.type.RowDeleting += Table_RowDeleting;
            this.type.TableCleared += Table_TableCleared;
            this.type.TableClearing += Table_TableClearing;
            this.isEventsAttached = true;
        }

        internal void DetachEventHandlers()
        {
            if (this.isEventsAttached == false)
                throw new Exception();
            this.type.ColumnChanging -= Table_ColumnChanging;
            this.type.ColumnChanged -= Table_ColumnChanged;
            this.type.RowChanging -= Table_RowChanging;
            this.type.RowChanged -= Table_RowChanged;
            this.type.RowDeleted -= Table_RowDeleted;
            this.type.RowDeleting -= Table_RowDeleting;
            this.type.TableCleared -= Table_TableCleared;
            this.type.TableClearing -= Table_TableClearing;
            this.isEventsAttached = false;
        }

        internal bool HasReference
        {
            get { return this.type.ReferenceList.Any(); }
        }

        internal string FlagTypeName
        {
            get { return CremaSchema.GenerateFlagTypeName(this.TypeName); }
        }

        internal CremaDataTypeMember InvokeNewTypeMemberFromBuilder(CremaDataTypeMemberBuilder builder)
        {
            return this.NewTypeMemberFromBuilder(builder);
        }

        internal InternalDataType InternalObject
        {
            get { return this.type; }
        }

        internal string InternalName
        {
            get { return this.type.InternalName; }
            set { this.type.InternalName = value; }
        }

        internal TagInfo InternalTags
        {
            get { return this.type.InternalTags; }
            set { this.type.Tags = value; }
        }

        internal bool InternalIsFlag
        {
            get { return this.type.InternalIsFlag; }
            set { this.type.InternalIsFlag = value; }
        }

        internal string InternalComment
        {
            get { return this.type.InternalComment; }
            set { this.type.InternalComment = value; }
        }

        internal string InternalCategoryPath
        {
            get { return this.type.InternalCategoryPath; }
            set { this.type.InternalCategoryPath = value; }
        }

        internal SignatureDate InternalCreationInfo
        {
            get { return this.type.InternalCreationInfo; }
            set { this.type.InternalCreationInfo = value; }
        }

        internal SignatureDate InternalModificationInfo
        {
            get { return this.type.InternalModificationInfo; }
            set { this.type.InternalModificationInfo = value; }
        }

        internal Guid InternalTypeID
        {
            get { return this.type.InternalTypeID; }
            set { this.type.InternalTypeID = value; }
        }

        internal bool IsDiffMode
        {
            get { return this.type.IsDiffMode; }
            set { this.type.IsDiffMode = value; }
        }

        internal CremaDataTypeMemberCollection Items
        {
            get { return this.members; }
        }

        #region IListSource

        bool IListSource.ContainsListCollection
        {
            get { return (this.type as IListSource).ContainsListCollection; }
        }

        System.Collections.IList IListSource.GetList()
        {
            return (this.type as IListSource).GetList();
        }

        #endregion

        #region IXmlSerializable

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.DetachEventHandlers();
            this.type.BeginLoadData();
            this.InternalTypeID = Guid.Parse(reader.GetAttribute(CremaSchema.ID));
            this.InternalName = reader.GetAttribute(CremaSchema.Name);
            this.InternalCategoryPath = reader.GetAttribute(CremaSchema.CategoryPath);
            this.InternalComment = reader.GetAttribute(CremaSchema.Comment);
            this.InternalIsFlag = reader.GetAttributeAsBoolean("IsFlag");
            this.InternalTags = reader.GetAttributeAsTagInfo(CremaSchema.Tags);
            this.InternalCreationInfo = reader.GetAttributeAsModificationInfo(CremaSchema.Creator, CremaSchema.CreatedDateTime);
            this.InternalModificationInfo = reader.GetAttributeAsModificationInfo(CremaSchema.Modifier, CremaSchema.ModifiedDateTime);

            reader.ReadStartElement();
            reader.MoveToContent();

            if (reader.IsEmptyElement == false)
            {
                reader.ReadStartElement("Members");
                reader.MoveToContent();

                while (reader.NodeType == XmlNodeType.Element)
                {
                    var member = this.NewMember();
                    var creationInfo = reader.GetAttributeAsModificationInfo(CremaSchema.Creator, CremaSchema.CreatedDateTime);
                    var modificationInfo = reader.GetAttributeAsModificationInfo(CremaSchema.Modifier, CremaSchema.ModifiedDateTime);
                    var memberID = reader.GetAttributeAsGuid(CremaSchema.ID);

                    reader.ReadStartElement("Member");
                    member.MemberID = memberID;
                    member.Name = reader.ReadElementContentAsString();
                    member.Value = reader.ReadElementContentAsLong();
                    member.Comment = reader.ReadElementContentAsString();
                    member.CreationInfo = creationInfo;
                    member.ModificationInfo = modificationInfo;
                    reader.ReadEndElement();
                    reader.MoveToContent();

                    this.members.Add(member);
                }

                reader.MoveToContent();
                reader.ReadEndElement();
            }
            else
            {
                reader.Skip();
            }

            reader.MoveToContent();
            reader.ReadEndElement();

            this.type.EndLoadData();
            this.type.AcceptChanges();
            this.AttachEventHandlers();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute(CremaSchema.ID, this.TypeID.ToString());
            writer.WriteAttribute(CremaSchema.Name, this.TypeName);
            writer.WriteAttribute(CremaSchema.CategoryPath, this.CategoryPath);
            writer.WriteAttribute(CremaSchema.Comment, this.Comment);
            writer.WriteAttribute("IsFlag", this.IsFlag);
            writer.WriteAttribute(CremaSchema.Tags, this.Tags.ToString());
            writer.WriteAttribute(CremaSchema.Creator, this.CreationInfo.ID);
            writer.WriteAttribute(CremaSchema.CreatedDateTime, this.CreationInfo.DateTime);
            writer.WriteAttribute(CremaSchema.Modifier, this.ModificationInfo.ID);
            writer.WriteAttribute(CremaSchema.ModifiedDateTime, this.ModificationInfo.DateTime);

            writer.WriteStartElement("Members");
            foreach (var item in this.members)
            {
                writer.WriteStartElement("Member");
                writer.WriteAttribute(CremaSchema.Creator, CremaSchema.CreatedDateTime, item.CreationInfo);
                writer.WriteAttribute(CremaSchema.Modifier, CremaSchema.ModifiedDateTime, item.ModificationInfo);
                writer.WriteAttribute(CremaSchema.ID, $"{item.MemberID}");
                writer.WriteElementString(CremaSchema.Name, item.Name);
                writer.WriteElementString(CremaSchema.Value, item.Value.ToString());
                writer.WriteElementString(CremaSchema.Comment, item.Comment);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion

        #region ISerializable

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
            //info.AddValue("Info", this.TypeInfo);
        }

        #endregion
    }
}
