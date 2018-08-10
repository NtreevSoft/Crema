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

using Ntreev.Crema.Data.Properties;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Security.Cryptography;

namespace Ntreev.Crema.Data
{
    class InternalDataType : InternalTableBase<InternalDataType, InternalDataTypeMember>
    {
        internal readonly InternalAttribute attributeID;
        internal readonly InternalAttribute attributeTag;
        internal readonly InternalAttribute attributeEnable;
        internal readonly DataColumn columnName;
        internal readonly DataColumn columnValue;
        internal readonly DataColumn columnComment;
        private string oldNameCache;
        private long? oldValueCache;

        private string[] namesCache;
        private ulong[] valuesCache;
        private KeyValuePair<string, long> deletedMember;
        private SignatureDate? modificationInfoCurrent;

        private readonly ObservableCollection<InternalDataType> childList = new ObservableCollection<InternalDataType>();

        public InternalDataType()
            : this(null, null, null)
        {

        }

        public InternalDataType(string name)
            : this(null, name, PathUtility.Separator)
        {

        }

        public InternalDataType(string name, string categoryPath)
            : this(null, name, categoryPath)
        {

        }

        public InternalDataType(CremaDataType target, string name, string categoryPath)
            : base(name, categoryPath)
        {
            this.ChildItems = new ReadOnlyObservableCollection<InternalDataType>(this.childList);
            if (base.ChildItems is INotifyCollectionChanged childTables)
            {
                childTables.CollectionChanged += ChildTables_CollectionChanged;
            }

            this.attributeID = new InternalAttribute(CremaSchema.ID, typeof(Guid))
            {
                AllowDBNull = false,
                ColumnMapping = MappingType.Hidden,
                ReadOnly = true,
                DefaultValue = Guid.NewGuid(),
            };
            this.Columns.Add(this.attributeID);

            this.attributeTag = new InternalAttribute(CremaSchema.Tags, typeof(string))
            {
                ColumnMapping = MappingType.Attribute,
                DefaultValue = $"{TagInfo.All}",
            };
            this.Columns.Add(this.attributeTag);

            this.attributeEnable = new InternalAttribute(CremaSchema.Enable, typeof(bool))
            {
                ColumnMapping = MappingType.Attribute,
                DefaultValue = true,
            };
            this.Columns.Add(this.attributeEnable);

            this.columnName = new DataColumn(CremaSchema.Name, typeof(string))
            {
                DefaultValue = "Member1",
                AllowDBNull = false,
            };
            this.Columns.Add(this.columnName);

            this.columnValue = new DataColumn(CremaSchema.Value, typeof(long))
            {
                DefaultValue = 0,
                AllowDBNull = false,
            };
            this.Columns.Add(this.columnValue);

            this.columnComment = new DataColumn(CremaSchema.Comment, typeof(string))
            {
                DataType = typeof(string),
            };
            this.Columns.Add(this.columnComment);

            base.Target = target ?? new CremaDataType(this);
            this.PrimaryKey = new DataColumn[] { this.columnName };
            this.DefaultView.Sort = $"{this.attributeIndex.AttributeName} ASC";
            this.CreationInfo = this.ModificationInfo = SignatureDateProvider.Default.Provide();
        }

        public static string GenerateHashValue(params TypeMemberInfo[] members)
        {
            var argList = new List<object>(members.Length * 2);
            foreach (var item in members)
            {
                argList.Add(item.Name);
                argList.Add(item.Value);
            }

            return GenerateHashValue(argList.ToArray());
        }

        /// <summary>
        /// 2개 단위로 인자를 설정해야 함
        /// name, value
        /// </summary>
        public static string GenerateHashValue(params object[] args)
        {
            if (args.Length % 2 != 0)
                throw new ArgumentException("Invalid args", nameof(args));
            var key = string.Empty;
            var list = new List<KeyValuePair<string, long>>();

            for (var i = 0; i < args.Length; i++)
            {
                switch (i % 2)
                {
                    case 0:
                        if (args[i] is string name)
                        {
                            key = name;
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                        break;
                    case 1:
                        if (args[i] is long value)
                        {
                            list.Add(new KeyValuePair<string, long>(key, value));
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                        break;
                }
            }

            var argList = new List<object>();
            foreach (var item in list.OrderBy(item => item.Key))
            {
                argList.Add(item.Key);
                argList.Add(item.Value);
            }

            using (var algorithm = HashAlgorithm.Create("SHA1"))
            {
                return HashUtility.GetHashValue(algorithm, argList.ToArray());
            }
        }

        public new void AcceptChanges()
        {
            this.oldNameCache = null;
            this.oldValueCache = null;

            base.AcceptChanges();

            this.attributeID.DefaultValue = Guid.NewGuid();
            this.columnName.DefaultValue = this.GenerateMemberName();
            this.columnValue.DefaultValue = this.GenerateMemberValue();

            var tables = this.ReferenceList.Select(item => item.Table).Distinct().ToArray();
            foreach (var item in tables)
            {
                lock (item)
                {
                    item.AcceptChanges();
                    item.Editor = null;
                }
            }
        }

        public new void RejectChanges()
        {
            base.BeginLoadData();
            base.RejectChanges();
            base.EndLoadData();

            var tables = this.ReferenceList.Select(item => item.Table).Distinct().ToArray();
            foreach (var item in tables)
            {
                item.RejectChanges();
                item.Editor = null;
            }
        }

        public override void EndLoadData()
        {
            base.EndLoadData();
            this.namesCache = null;
            this.valuesCache = null;
            //this.GetNamesValues(out this.namesCache, out this.valuesCache, false);
            this.attributeID.DefaultValue = Guid.NewGuid();
            this.columnName.DefaultValue = this.GenerateMemberName();
            this.columnValue.DefaultValue = this.GenerateMemberValue();
        }

        public void AddMember(InternalDataTypeMember member)
        {
            this.ValidateAddMember(member);
            this.Rows.Add(member);
        }

        public void AddMember(TypeMemberInfo memberInfo)
        {
            this.ValidateAddMember(memberInfo);
            var member = this.NewRow() as InternalDataTypeMember;
            member.ID = memberInfo.ID;
            member.Tags = memberInfo.Tags;
            member.IsEnabled = memberInfo.IsEnabled;
            member.Name = memberInfo.Name;
            member.Value = memberInfo.Value;
            member.Comment = memberInfo.Comment;
            this.Rows.Add(member);
            member.CreationInfo = memberInfo.CreationInfo;
            member.ModificationInfo = memberInfo.ModificationInfo;
        }

        public void RemoveMember(InternalDataTypeMember member)
        {
            this.ValidateRemoveMember(member);
            using (this.AcceptChangesStack.Set(true))
            {
                this.Rows.Remove(member);
            }
        }

        public void ClearMembers()
        {
            this.ValidateClearMembers();
            this.Rows.Clear();
        }

        public void ValidateAddMember(InternalDataTypeMember member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            if (member.RowState != DataRowState.Detached && member.Table == this)
                throw new ArgumentException(string.Format(Resources.Exception_AlreadyExistedItem_Format, member), nameof(member));
            if (member.Table != null && member.Table != this)
                throw new ArgumentException($"'{member}' 멤버는 이미 다른 DataType에 속해 있습니다.", nameof(member));
            if (this.ReadOnly == true)
                throw new ArgumentException($"읽기 전용인 타입에는 멤버를 추가할 수 없습니다.", nameof(member));
        }

        public void ValidateAddMember(TypeMemberInfo memberInfo)
        {
            if (this.ReadOnly == true)
                throw new ArgumentException($"읽기 전용인 타입에는 멤버를 추가할 수 없습니다.", nameof(memberInfo));
        }

        public void ValidateRemoveMember(InternalDataTypeMember member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            if (member.Table == null)
                throw new ArgumentException($"이 타입에 속하지 않는 멤버는 제거할 수 없습니다.", nameof(member));
            if (member.Table != null && member.Table != this)
                throw new ArgumentException($"이 타입에 속하지 않는 멤버는 제거할 수 없습니다.", nameof(member));
            if (this.ReadOnly == true)
                throw new ArgumentException($"읽기 전용인 타입에는 멤버를 제거할 수 없습니다.", nameof(member));
        }

        public void ValidateClearMembers()
        {
            foreach (var item in this.ReferenceList)
            {
                this.ValidateClearMembers(item);
            }
        }

        public void ValidateName()
        {
            for (var i = 0; i < this.Rows.Count; i++)
            {
                var item = this.Rows[i];
                if (item.RowState != DataRowState.Deleted)
                {
                    var name = item.Field<string>(this.columnName);
                    if (name == null && this.columnName.AllowDBNull == true)
                        continue;
                    CremaDataSet.ValidateName(name);
                }
            }
        }

        public bool ContainsMember(string memberName)
        {
            foreach (var item in this.Rows)
            {
                if (item is InternalDataTypeMember member)
                {
                    if (member.RowState == DataRowState.Deleted)
                        continue;
                    if (member.Field<string>(CremaSchema.Name) == memberName)
                        return true;
                }
            }
            return false;
        }

        public bool ContainsMember(Guid memberID)
        {
            foreach (var item in this.Rows)
            {
                if (item is InternalDataTypeMember member)
                {
                    if (member.RowState == DataRowState.Deleted)
                        continue;
                    if (member[CremaSchema.ID] is Guid id && id == memberID)
                        return true;
                }
            }
            return false;
        }

        public string ConvertToString(long value)
        {
            if(this.namesCache == null)
            {
                this.GetNamesValues(out this.namesCache, out this.valuesCache, false);
            }
            return EnumUtility.ConvertToString(this.namesCache, this.valuesCache, this.IsFlag, (ulong)value);
        }

        public long ConvertFromString(string textValue)
        {
            if (this.IsFlag == true)
            {
                var ss = StringUtility.Split(textValue);
                var value = (long)0;
                foreach (var item in ss)
                {
                    if (this.ContainsMember(item) == false)
                        throw new InvalidOperationException(string.Format(Resources.Exception_CannotParseEnum_Format, item, this.LocalName));
                    value |= this.GetMember(item).Value;
                }
                return value;
            }
            else
            {
                if (this.columnName.AllowDBNull == false && this.ContainsMember(textValue) == false)
                    throw new InvalidOperationException(string.Format(Resources.Exception_CannotParseEnum_Format, textValue, this.LocalName));
                return this.GetMember(textValue).Value;
            }
        }

        public InternalDataTypeMember GetMember(string memberName)
        {
            foreach (var item in this.Rows)
            {
                if (item is InternalDataTypeMember member)
                {
                    if (member.RowState == DataRowState.Deleted)
                        continue;
                    if (member.Field<string>(CremaSchema.Name) == memberName)
                        return member;
                }
            }
            return null;
        }

        public InternalDataTypeMember GetMember(Guid memberID)
        {
            foreach (var item in this.Rows)
            {
                if (item is InternalDataTypeMember member)
                {
                    if (member.RowState == DataRowState.Deleted)
                        continue;
                    if (member[CremaSchema.ID] is Guid id && id == memberID)
                        return member;
                }
            }
            return null;
        }

        public bool VerifyValue(string textValue)
        {
            if (this.IsFlag == true)
            {
                var items = StringUtility.Split(textValue);

                foreach (var item in items)
                {
                    if (this.ContainsMember(item) == false)
                        return false;
                }
            }
            else
            {
                if (this.ContainsMember(textValue) == false)
                    return false;
            }
            return true;
        }

        public void ValidateValue(string value)
        {
            if (this.IsFlag == true)
            {
                var items = StringUtility.Split(value);

                foreach (var item in items)
                {
                    if (this.ContainsMember(item) == false)
                        throw new ArgumentException(string.Format(Resources.Exception_CannotParseEnum_Format, item, this.Target.TypeName), nameof(value));
                }
            }
            else
            {
                if (this.columnName.AllowDBNull == false && this.ContainsMember(value) == false)
                    throw new ArgumentException(string.Format(Resources.Exception_CannotParseEnum_Format, value, this.Target.TypeName), nameof(value));
            }
        }

        public void AddReference(InternalDataColumn column)
        {
            lock (this)
            {
                if (this.ReferenceList.Contains(column) == true)
                    throw new CremaDataException();
                this.ReferenceList.Add(column);
            }
        }

        public void RemoveReference(InternalDataColumn column)
        {
            lock (this)
            {
                if (this.ReferenceList.Contains(column) == false)
                    throw new CremaDataException();
                this.ReferenceList.Remove(column);
            }
        }

        public new CremaDataType Target => base.Target as CremaDataType;

        public List<InternalDataColumn> ReferenceList { get; } = new List<InternalDataColumn>();

        public new ReadOnlyObservableCollection<InternalDataType> ChildItems { get; }

        public new IEnumerable<InternalDataType> FamilyItems
        {
            get
            {
                yield return this;
                foreach (var item in this.ChildItems)
                {
                    yield return item;
                }
            }
        }

        public new IEnumerable<InternalDataType> DerivedItems
        {
            get
            {
                foreach (var item in base.DerivedItems)
                {
                    if (item is InternalDataType dataTable)
                        yield return dataTable;
                }
            }
        }

        public new IEnumerable<InternalDataType> SiblingItems
        {
            get
            {
                yield return this;
                foreach (var item in this.DerivedItems)
                {
                    yield return item;
                }
            }
        }

        public Guid TypeID => this.InternalTypeID;

        public bool IsFlag
        {
            get => this.InternalIsFlag;
            set
            {
                if (this.InternalIsFlag == value)
                    return;
                this.ValidateSetIsFlag(value);
                this.InternalIsFlag = value;
            }
        }

        public string Comment
        {
            get => this.InternalComment ?? string.Empty;
            set
            {
                if (this.InternalComment == value)
                    return;
                this.ValidateSetComment(value);
                this.InternalComment = value;
            }
        }

        public TagInfo Tags { get; set; } = TagInfo.All;

        public TagInfo DerivedTags
        {
            get => this.Tags;
            set => this.Tags = value;
        }

        public new InternalDataSet DataSet => base.DataSet as InternalDataSet;

        public SignatureDate CreationInfo { get; set; } = SignatureDate.Empty;

        public SignatureDate ModificationInfo { get; set; } = SignatureDate.Empty;

        public TypeInfo TypeInfo
        {
            get
            {
                var typeInfo = new TypeInfo()
                {
                    ID = this.TypeID,
                    Tags = this.Tags,
                    DerivedTags = this.DerivedTags,
                    Name = this.Name,
                    Comment = this.Comment,
                    IsFlag = this.IsFlag,
                    CategoryPath = this.CategoryPath,
                    CreationInfo = this.CreationInfo,
                    ModificationInfo = this.ModificationInfo,
                    Members = GetMembers(),
                };
                typeInfo.HashValue = GenerateHashValue(typeInfo.Members);
                return typeInfo;

                TypeMemberInfo[] GetMembers()
                {
                    var query = from item in this.RowList
                                where item[this.attributeID] is Guid
                                select (item as InternalDataTypeMember).TypeMemberInfo;

                    return query.ToArray();
                }
            }
        }

        public bool InternalIsFlag { get; set; }

        public TagInfo InternalTags
        {
            get => this.Tags;
            set
            {
                this.Tags = value;
                this.attributeTag.DefaultValue = (string)value;
            }
        }

        public string InternalComment { get; set; }

        public Guid InternalTypeID { get; set; } = Guid.NewGuid();

        public SignatureDate InternalCreationInfo
        {
            get => this.CreationInfo;
            set => this.CreationInfo = value;
        }

        public SignatureDate InternalModificationInfo
        {
            get => this.ModificationInfo;
            set => this.ModificationInfo = value;
        }

        public static explicit operator CremaDataType(InternalDataType dataType)
        {
            if (dataType == null)
                return null;
            return dataType.Target;
        }

        public static explicit operator InternalDataType(CremaDataType dataType)
        {
            if (dataType == null)
                return null;
            return dataType.InternalObject;
        }

        protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
        {
            this.Target.builder.InternalBuilder = builder;
            return new InternalDataTypeMember(this.Target.builder, this);
        }

        protected override void OnColumnChanging(DataColumnChangeEventArgs e)
        {
            var column = e.Column;
            var row = e.Row as InternalDataTypeMember;

            this.ValidateChangeName(e);
            this.ValidateChangeID(e);

            foreach (var item in this.ReferenceList)
            {
                var table = item.Table;
                if (table.Editor == null && table.HasChanges == true)
                {
                    throw new ArgumentException("커밋되지 않은 테이블이 있기 때문에 편집할 수 없습니다.");
                }
            }

            if (column == this.columnValue && row.IsIncluded() && column.AllowDBNull == false)
            {
                var value = (long)Convert.ChangeType(e.ProposedValue, typeof(long));
            }

            base.OnColumnChanging(e);

            if (column == this.columnName)
            {
                this.oldNameCache = row.Name;
            }
            else if (column == this.columnValue)
            {
                if (e.ProposedValue is long value)
                    this.oldValueCache = value;
                else
                    this.oldValueCache = null;
            }

            foreach (var item in this.ReferenceList)
            {
                item.Table.Editor = this;
            }
        }

        protected override void OnColumnChanged(DataColumnChangeEventArgs e)
        {
            base.OnColumnChanged(e);
        }

        protected override void OnRowChanging(DataRowChangeEventArgs e)
        {
            if (this.RowEventStack.Any() == true)
            {
                if (this.RowEventStack.Peek() == e.Row)
                    base.OnRowChanging(new InternalDataRowChangeEventArgs(e));
                else
                    base.OnRowChanging(e);
            }
            else
            {
                var row = e.Row as InternalDataTypeMember;
                switch (e.Action)
                {
                    case DataRowAction.Add:
                        {
                            if (this.IsLoading == false && this.IsDiffMode == false)
                                CremaDataSet.ValidateName(row.Name);
                        }
                        break;
                    case DataRowAction.Change:
                        {
                            if (this.oldValueCache != null)
                            {
                                if (row[this.columnName] is string && row[this.columnValue] is long)
                                    this.ValidateChangeMemberValue(row.Name, row.Value);
                            }


                        }
                        break;
                }
                base.OnRowChanging(e);
            }
        }

        protected override void OnRowChanged(DataRowChangeEventArgs e)
        {
            if (this.RowEventStack.Any() == true)
            {
                if (this.RowEventStack.Peek() == e.Row)
                    base.OnRowChanged(new InternalDataRowChangeEventArgs(e));
                else
                    base.OnRowChanged(e);
            }
            else
            {
                base.OnRowChanged(e);
                this.UpdateSignatureDate(e);
                this.namesCache = null;
                this.valuesCache = null;
                //if (this.IsLoading == false)
                //    this.GetNamesValues(out this.namesCache, out this.valuesCache, false);
                this.UpdateTableData(e);

                if (this.IsLoading == false && this.IsDiffMode == false)
                {
                    if (e.Action != DataRowAction.Commit)
                    {
                        this.attributeID.DefaultValue = Guid.NewGuid();
                        this.columnName.DefaultValue = this.GenerateMemberName();
                        this.columnValue.DefaultValue = this.GenerateMemberValue();
                    }
                }
            }
        }

        protected override void OnRowDeleting(DataRowChangeEventArgs e)
        {
            var row = e.Row as InternalDataTypeMember;

            if (this.columnValue.AllowDBNull == false)
                this.ValidateDeleteMember(row.Name, row.Value);

            base.OnRowDeleting(e);
            if (this.columnValue.AllowDBNull == false)
                this.deletedMember = new KeyValuePair<string, long>(row.Name, row.Value);
        }

        protected override void OnRowDeleted(DataRowChangeEventArgs e)
        {
            base.OnRowDeleted(e);

            this.namesCache = null;
            this.valuesCache = null;
            //this.GetNamesValues(out this.namesCache, out this.valuesCache, false);
            this.DeleteMember(this.deletedMember.Key, this.deletedMember.Value);
            if (this.OmitSignatureDate == false)
            {
                this.ModificationInfo = this.SignatureDateProvider.Provide();
            }
        }

        protected override void OnTableClearing(DataTableClearEventArgs e)
        {
            this.ValidateClearMembers();
            base.OnTableClearing(e);
        }

        protected override void OnTableCleared(DataTableClearEventArgs e)
        {
            this.attributeIndex.DefaultValue = (int)0;
            this.ModificationInfo = this.SignatureDateProvider.Provide();
            this.modificationInfoCurrent = null;
            base.OnTableCleared(e);
            this.ClearTableData();
        }

        protected override void OnSetNormalMode()
        {
            base.OnSetNormalMode();

            this.attributeID.AllowDBNull = false;
            this.attributeID.ColumnMapping = MappingType.Hidden;
            this.attributeID.DefaultValue = Guid.NewGuid();
            this.attributeID.ReadOnly = true;

            this.attributeTag.ColumnMapping = MappingType.Attribute;
            this.attributeTag.DefaultValue = $"{TagInfo.All}";

            this.attributeEnable.ColumnMapping = MappingType.Attribute;
            this.attributeEnable.DefaultValue = true;

            this.columnName.DefaultValue = "Member1";
            this.columnName.AllowDBNull = false;

            this.columnValue.DefaultValue = 0;
            this.columnValue.AllowDBNull = false;

            this.PrimaryKey = new DataColumn[] { this.columnName };
            this.DefaultView.Sort = $"{CremaSchema.Index} ASC";
        }

        protected override void OnSetDiffMode()
        {
            base.OnSetDiffMode();

            this.attributeID.AllowDBNull = true;
            this.attributeID.ReadOnly = false;
            this.attributeID.ColumnMapping = MappingType.Attribute;
            this.attributeID.DefaultValue = DBNull.Value;

            this.attributeTag.DefaultValue = DBNull.Value;

            this.attributeEnable.DefaultValue = DBNull.Value;

            this.columnName.DefaultValue = DBNull.Value;
            this.columnName.AllowDBNull = true;

            this.columnValue.DefaultValue = DBNull.Value;
            this.columnValue.AllowDBNull = true;

            this.PrimaryKey = null;
        }

        protected override string BaseNamespace
        {
            get
            {
                if (this.DataSet == null)
                    return CremaSchema.TypeNamespace;
                return this.DataSet.TypeNamespace;
            }
        }

        private void ValidateChangeID(DataColumnChangeEventArgs e)
        {
            var column = e.Column;
            var row = e.Row as InternalDataTypeMember;

            if (Verify() == true)
            {
                if (e.ProposedValue is Guid id)
                {
                    var member = this.GetMember(id);
                    if (member != null && member != row)
                        throw new Exception($"'{id}' 은(는) 이미 존재합니다.");
                }
            }

            bool Verify()
            {
                if (column == this.attributeID && e.Row.RowState != DataRowState.Detached && this.IsLoading == false)
                    return true;
                return false;
            }
        }

        private void ValidateChangeName(DataColumnChangeEventArgs e)
        {
            var column = e.Column;
            var row = e.Row as InternalDataTypeMember;

            if (Verify() == true)
            {
                if (e.ProposedValue is string text)
                {
                    CremaDataSet.ValidateName(text);
                    var member = this.GetMember(text);
                    if (member != null && member != row)
                        throw new Exception($"'{text}' 은(는) 이미 존재합니다.");
                }
            }

            bool Verify()
            {
                if (column == this.columnName && e.Row.RowState != DataRowState.Detached && this.IsLoading == false)
                    return true;
                return false;
            }
        }

        private string GenerateMemberName()
        {
            var index = 1;
            var name = "Member";
            var memberName = name + index++;

            while (FindName())
            {
                memberName = name + index++;
            }

            return memberName;

            bool FindName()
            {
                foreach (var item in this.Rows)
                {
                    if (item is DataRow dataRow && dataRow.RowState != DataRowState.Deleted)
                    {
                        if (dataRow.Field<string>(this.columnName) == memberName)
                            return true;
                    }
                }
                return false;
            }
        }

        private long GenerateMemberValue()
        {
            long value = 0;

            while (this.Select($"{this.columnValue}={value}").Any())
            {
                if (this.Target.IsFlag == true)
                {
                    if (value == (long)1 << 63)
                        break;
                    if (value == 0)
                        value = 1;
                    else
                        value <<= 1;
                }
                else
                {
                    if (value == -1)
                        break;
                    value++;
                }
            }

            return value;
        }

        private void UpdateTableData(DataRowChangeEventArgs e)
        {
            var row = e.Row as InternalDataTypeMember;
            var newName = GetName();

            if (this.oldNameCache != null && newName != this.oldNameCache)
            {
                try
                {
                    foreach (var item in this.ReferenceList)
                    {
                        this.UpdateTableData(item, this.oldNameCache, newName);
                    }
                }
                finally
                {

                }
            }

            string GetName()
            {
                if (row.RowState == DataRowState.Detached)
                    return string.Empty;
                if (row[this.columnName] is string name)
                    return name;
                return string.Empty;
            }
        }

        private void UpdateTableData(InternalDataColumn dataColumn, string oldValue, string newValue)
        {
            var dataTable = dataColumn.Table;
            dataTable.Editor = this;
            foreach (var item in dataTable.Rows)
            {
                if (item is DataRow dataRow)
                    ChangeField(dataRow);
            }

            if (dataColumn.DefaultString == oldValue)
            {
                dataColumn.DefaultValue = newValue;
            }

            void ChangeField(DataRow dataRow)
            {
                if (dataRow[dataColumn] is string textValue)
                {
                    if (this.IsFlag == true)
                    {
                        var ss = textValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var isChanged = false;
                        for (var i = 0; i < ss.Length; i++)
                        {
                            if (ss[i] == oldValue)
                            {
                                ss[i] = newValue;
                                isChanged = true;
                            }
                        }
                        if (isChanged == true)
                        {
                            dataRow.SetForceField(dataColumn, string.Join(" ", ss), this);
                        }
                    }
                    else
                    {
                        if (textValue == oldValue)
                        {
                            dataRow.SetForceField(dataColumn, newValue, this);
                        }
                    }
                }
            }
        }

        private void UpdateSignatureDate(DataRowChangeEventArgs e)
        {
            if (this.OmitSignatureDate == true)
                return;

            var row = e.Row as InternalDataTypeMember;
            var action = e.Action;

            switch (action)
            {
                case DataRowAction.Add:
                    {
                        this.ModificationInfo = row.ModificationInfo;
                    }
                    break;
                case DataRowAction.Commit:
                    {

                    }
                    break;
                case DataRowAction.Change:
                    {
                        this.ModificationInfo = row.ModificationInfo;
                    }
                    break;
                case DataRowAction.Delete:
                    {
                        this.ModificationInfo = this.SignatureDateProvider.Provide();
                    }
                    break;
                case DataRowAction.Rollback:
                    {
                        if (this.modificationInfoCurrent.HasValue == true)
                            this.ModificationInfo = this.modificationInfoCurrent.Value;
                    }
                    break;
            }
        }

        private void GetNamesValues(out string[] names, out ulong[] values, bool isBegineEdited)
        {
            var members = new List<DataRow>(this.Rows.Count);
            for (var i = 0; i < this.Rows.Count; i++)
            {
                var dataRow = this.Rows[i];
                if (dataRow.RowState == DataRowState.Deleted)
                    continue;
                if (isBegineEdited == true)
                {
                    if (dataRow.HasVersion(DataRowVersion.Original) == true && dataRow[CremaSchema.Name, DataRowVersion.Original] is DBNull)
                        continue;
                    if (dataRow.HasVersion(DataRowVersion.Current) == true && dataRow[CremaSchema.Name, DataRowVersion.Current] is DBNull)
                        continue;
                }
                else
                {
                    if (dataRow[CremaSchema.Name] == DBNull.Value || dataRow[CremaSchema.Value] == DBNull.Value)
                        continue;
                }
                members.Add(dataRow);
            }
            EnumUtility.GetEnumData(members, NameSelector, ValueSelector, out names, out values);

            string NameSelector(DataRow member)
            {
                if (isBegineEdited == true)
                {
                    if (member.HasVersion(DataRowVersion.Current) == true)
                        return (string)member[CremaSchema.Name, DataRowVersion.Current];
                }
                return (string)member[CremaSchema.Name];
            }

            ulong ValueSelector(DataRow member)
            {
                if (isBegineEdited == true)
                {
                    if (member.HasVersion(DataRowVersion.Current) == true)
                        return (ulong)(long)member[CremaSchema.Value, DataRowVersion.Current];
                }
                return (ulong)(long)member[CremaSchema.Value];
            }
        }

        private void DeleteMember(string name, long value)
        {
            var members = new List<InternalDataTypeMember>(this.Rows.Count);
            foreach (var item in this.Rows)
            {
                if (item is InternalDataTypeMember member)
                {
                    if (member.RowState == DataRowState.Deleted || member.RowState == DataRowState.Detached)
                        continue;
                    if (member[this.columnName] is DBNull)
                        continue;
                    if (member.Name == name)
                        continue;
                    members.Add(member);
                }
            }
            EnumUtility.GetEnumData(members, item => item.Name, item => (ulong)item.Value, out string[] names, out ulong[] values);

            foreach (var item in this.ReferenceList)
            {
                this.DeleteMember(item, name, value, names, values);
            }
        }

        private void DeleteMember(InternalDataColumn column, string name, long value, string[] names, ulong[] values)
        {
            var table = column.Table;
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var dataRow = table.Rows[i];
                var oldValue = dataRow.Field<string>(column);
                var curValue = dataRow.Field<string>(column);

                if (curValue != null)
                {
                    var replaceValue = string.Empty;
                    for (var j = 0; j < names.Length; j++)
                    {
                        if (values[j] == (ulong)value)
                        {
                            replaceValue = names[j];
                            break;
                        }
                    }

                    var ss = StringUtility.Split(curValue);
                    for (var j = 0; j < ss.Length; j++)
                    {
                        if (ss[j] == name)
                        {
                            ss[j] = replaceValue;
                            break;
                        }
                    }

                    curValue = string.Join(" ", ss.Where(item => item != string.Empty));
                    if (curValue == string.Empty)
                        curValue = null;
                }

                if (oldValue != curValue)
                {
                    dataRow.SetForceField(column, curValue, this);
                }
            }
        }

        private void ClearTableData()
        {
            foreach (var item in this.ReferenceList)
            {
                this.ClearTableData(item);
            }
        }

        private void ClearTableData(InternalDataColumn column)
        {
            var table = column.Table;
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var dataRow = table.Rows[i];
                var oldValue = dataRow.Field<string>(column);

                if (oldValue != null)
                    dataRow.SetField<string>(column, null);
            }
        }

        private void ValidateSetIsFlag(bool value)
        {
            if (value == false)
            {
                foreach (var column in this.ReferenceList)
                {
                    ValidateRows(column);
                }

                void ValidateRows(DataColumn dataColumn)
                {
                    var table = dataColumn.Table;
                    foreach (var item in dataColumn.Table.Rows)
                    {
                        if (item is DataRow dataRow && dataRow[dataColumn] is string textValue)
                        {
                            if (StringUtility.Split(textValue).Length > 1)
                            {
                                throw new InvalidConstraintException(string.Format("'{0}'테이블에 {1}'열에서 항목이 중첩사용되고 있기 때문에 IsFlag 속성을 해제할 수 없습니다.", table.TableName, dataColumn.ColumnName));
                            }
                        }
                    }
                }
            }
        }

        private void ValidateSetComment(string comment)
        {

        }

        private void ValidateChangeMemberValue(string name, long value)
        {
            this.GetNamesValues(out string[] names, out ulong[] values, true);

            for (var i = 0; i < names.Length; i++)
            {
                if (names[i] == name)
                {
                    values[i] = (ulong)value;
                }
            }

            foreach (var item in this.ReferenceList)
            {
                this.ValidateChangeMemberValue(item, names, values);
            }
        }

        private void ValidateDeleteMember(string name, long value)
        {
            var members = new List<InternalDataTypeMember>(this.Rows.Count);
            foreach (var item in this.Rows)
            {
                if (item is InternalDataTypeMember member)
                {
                    if (member.RowState == DataRowState.Deleted || member.RowState == DataRowState.Detached)
                        continue;
                    if (member.Name == name)
                        continue;
                    members.Add(member);
                }
            }

            EnumUtility.GetEnumData(members, item => item.Name, item => (ulong)item.Value, out string[] names, out ulong[] values);

            foreach (var item in this.ReferenceList)
            {
                this.ValidateDeleteMember(item, name, value, names, values);
            }
        }

        private void ValidateChangeMemberValue(InternalDataColumn column, string[] names, ulong[] values)
        {
            if (column.Unique == false)
                return;

            var table = column.Table;
            var fields = new List<ulong?>(table.Rows.Count);

            for (var i = 0; i < table.Rows.Count; i++)
            {
                var dataRow = table.Rows[i];
                var curValue = dataRow.Field<string>(column);

                if (curValue != null)
                {
                    var field = EnumUtility.ConvertFromString(names, values, this.IsFlag, curValue);
                    fields.Add(field);
                }
                else
                {
                    fields.Add(null);
                }
            }

            if (fields.Distinct().Count() != table.Rows.Count)
            {
                throw new InvalidConstraintException(string.Format("'{0}'테이블에 {1}'열에서 값을 변경할 경우 중복 값이 발생하므로 변경할 수 없습니다.", table.Name, column.ColumnName));
            }
        }

        private void ValidateDeleteMember(InternalDataColumn column, string name, long value, string[] names, ulong[] values)
        {
            if (column.Unique == false && column.AllowDBNull == true)
                return;

            var table = column.Table;
            var textValues = new List<string>(table.Rows.Count);

            for (var i = 0; i < table.Rows.Count; i++)
            {
                var dataRow = table.Rows[i];
                var curValue = dataRow.Field<string>(column);

                if (curValue != null)
                {
                    var replaceValue = string.Empty;
                    for (var j = 0; j < names.Length; j++)
                    {
                        if (values[j] == (ulong)value)
                        {
                            replaceValue = names[j];
                            break;
                        }
                    }

                    var ss = StringUtility.Split(curValue);
                    for (var j = 0; j < ss.Length; j++)
                    {
                        if (ss[j] == name)
                        {
                            ss[j] = replaceValue;
                            break;
                        }
                    }

                    curValue = string.Join(" ", ss.Where(item => item != string.Empty));
                    if (curValue == string.Empty)
                        curValue = null;
                }

                if (column.AllowDBNull == false && curValue == null)
                    throw new InvalidConstraintException(string.Format("'{0}'테이블에 '{1}'열은 null 값이 허용되지 않기 때문에 '{2}'항목을 삭제할 수 없습니다.", table.Name, column.ColumnName, name));

                textValues.Add(curValue);
            }

            if (column.Unique == true)
            {
                if (textValues.Distinct().Count() != table.Rows.Count)
                {
                    throw new InvalidConstraintException(string.Format("'{0}'테이블에 {1}'열에서 '{2}'항목을 삭제할 경우 중복 값이 발생하므로 삭제할 수 없습니다.", table.Name, column.ColumnName, name));
                }
            }
        }

        private void ValidateClearMembers(InternalDataColumn column)
        {
            if (column.Unique == false && column.AllowDBNull == true)
                return;

            var table = column.Table;
            if (column.AllowDBNull == false)
                throw new InvalidConstraintException(string.Format("'{0}'테이블에 '{1}'열은 null 값이 허용되지 않기 때문에 모든 타입 항목을 삭제할 수 없습니다.", table.Name, column.ColumnName));

            if (column.Unique == true && table.Rows.Count > 1)
                throw new InvalidConstraintException(string.Format("'{0}'테이블에 {1}'열에서 모든 타입 항목을 삭제할 경우 중복 값이 발생하므로 모든 타입 항목을 삭제할 수 없습니다.", table.Name, column.ColumnName));
        }

        private void ChildTables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is InternalDataType typeItem)
                                this.childList.Add(typeItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (item is InternalDataType typeItem)
                                this.childList.Remove(typeItem);
                        }
                    }
                    break;
            }
        }
    }
}
