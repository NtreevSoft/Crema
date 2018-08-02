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
using Ntreev.Library.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography;

namespace Ntreev.Crema.Data
{
    class InternalDataTable : InternalTableBase<InternalDataTable, InternalDataRow>
    {
        private const string childNameString = "Child";
        private const string columnNameString = "Column";
        private static readonly TypeConverter longConverter = TypeDescriptor.GetConverter(typeof(long));

        internal readonly InternalAttribute attributeTag;
        internal readonly InternalAttribute attributeEnable;

        private TagInfo tags = TagInfo.All;
        private bool[] autoIncrements;
        private readonly List<InternalDataColumn> columnList = new List<InternalDataColumn>();
        private readonly ObservableCollection<InternalDataTable> childList = new ObservableCollection<InternalDataTable>();

        public InternalDataTable()
            : this(null, null, null)
        {

        }

        public InternalDataTable(string name)
            : this(null, name, PathUtility.Separator)
        {

        }

        public InternalDataTable(string name, string categoryPath)
            : this(null, name, categoryPath)
        {

        }

        public InternalDataTable(CremaDataTable target, string name, string categoryPath)
            : base(name, categoryPath)
        {
            this.ChildItems = new ReadOnlyObservableCollection<InternalDataTable>(this.childList);
            if (base.ChildItems is INotifyCollectionChanged childTables)
            {
                childTables.CollectionChanged += ChildTables_CollectionChanged;
            }

            this.attributeTag = new InternalAttribute(CremaSchema.Tags, typeof(string))
            {
                AllowDBNull = false,
                DefaultValue = TagInfo.All.ToString(),
            };
            this.Columns.Add(this.attributeTag);

            this.attributeEnable = new InternalAttribute(CremaSchema.Enable, typeof(bool))
            {
                AllowDBNull = false,
                DefaultValue = true,
            };
            this.Columns.Add(this.attributeEnable);

            base.Target = target ?? new CremaDataTable(this);
            this.Columns.CollectionChanged += Columns_CollectionChanged;
            this.Constraints.CollectionChanged += Constraints_CollectionChanged;
            this.InternalCreationInfo = this.InternalModificationInfo = this.SignatureDateProvider.Provide();
        }

        public static string GenerateHashValue(params ColumnInfo[] columns)
        {
            var argList = new List<object>(columns.Length * 4);
            foreach (var item in columns)
            {
                argList.Add(item.DataType);
                argList.Add(item.Name);
                argList.Add(item.IsKey);
                argList.Add(item.IsUnique);
            }

            return GenerateHashValue(argList.ToArray());
        }

        /// <summary>
        /// 4개 단위로 인자를 설정해야 함
        /// dataType, columnName, isKey, isUnique 순으로
        /// </summary>
        public static string GenerateHashValue(params object[] args)
        {
            if (args.Length % 4 != 0)
                throw new ArgumentException("Invalid args", nameof(args));

            for (var i = 0; i < args.Length; i++)
            {
                switch (i % 4)
                {
                    case 0:
                        if (args[i] is string dataType)
                        {

                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                        break;
                    case 1:
                        if (args[i] is string name)
                        {

                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                        break;
                    case 2:
                        if (args[i] is bool isKey)
                        {

                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                        break;
                    case 3:
                        if (args[i] is bool isUnique)
                        {

                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                        break;
                }

            }
            using (var algorithm = HashAlgorithm.Create("SHA1"))
            {
                return HashUtility.GetHashValue(algorithm, args);
            }
        }

        public void AddRow(InternalDataRow row)
        {
            this.ValidateAddRow(row);
            if (this.IsLoading == true && this.ColumnRelation != null && row.RelationID == null)
            {
                row.RelationID = this.GenerateRelationID(row);
            }
            this.Rows.Add(row);
        }

        public void RemoveRow(InternalDataRow row)
        {
            this.ValidateRemoveRow(row);
            this.Rows.Remove(row);
        }

        public void RemoveRowAt(int index)
        {
            this.ValidateRemoveRowAt(index);
            var row = this.RowList[index];
            this.Rows.Remove(row);
        }

        public void AddColumn(InternalDataColumn dataColumn)
        {
            this.ValidateAddColumn(dataColumn);
            var signatureDate = this.Sign();
            this.AddColumnInternal(dataColumn);
            if (this.OmitSignatureDate == false)
                this.InternalModificationInfo = signatureDate;
            foreach (var item in this.DerivedItems.ToArray())
            {
                var derivedColumn = new InternalDataColumn();
                dataColumn.CopyTo(derivedColumn);
                item.AddColumnInternal(derivedColumn);
                if (item.OmitSignatureDate == false)
                    item.InternalModificationInfo = signatureDate;
            }
        }

        public InternalDataColumn AddColumn()
        {
            var columnNames = from DataColumn item in this.Columns select item.ColumnName;
            var columnName = NameUtility.GenerateNewName(columnNameString, columnNames.ToArray());
            return this.AddColumn(columnName);
        }

        public InternalDataColumn AddColumn(string columnName)
        {
            this.ValidateAddColumn(columnName);
            var signatureDate = this.Sign();
            if (this.OmitSignatureDate == false)
                this.InternalModificationInfo = signatureDate;
            var dataColumn = this.AddColumnInternal(columnName);
            foreach (var item in this.DerivedItems.ToArray())
            {
                item.AddColumnInternal(columnName);
                if (item.OmitSignatureDate == false)
                    item.InternalModificationInfo = signatureDate;
            }
            return dataColumn;
        }

        public InternalDataColumn AddColumn(string columnName, Type dataType)
        {
            this.ValidateAddColumn(columnName, dataType);
            var signatureDate = this.Sign();
            if (this.OmitSignatureDate == false)
                this.InternalModificationInfo = signatureDate;
            var dataColumn = this.AddColumnInternal(columnName, dataType);
            foreach (var item in this.DerivedItems.ToArray())
            {
                item.AddColumnInternal(columnName, dataType);
                if (item.OmitSignatureDate == false)
                    item.InternalModificationInfo = signatureDate;
            }
            return dataColumn;
        }

        public InternalDataColumn AddColumn(string columnName, InternalDataType dataType)
        {
            this.ValidateAddColumn(columnName, dataType);
            var signatureDate = this.Sign();
            if (this.OmitSignatureDate == false)
                this.InternalModificationInfo = signatureDate;
            var dataColumn = this.AddColumnInternal(columnName, dataType);
            foreach (var item in this.DerivedItems.ToArray())
            {
                item.AddColumnInternal(columnName, dataType);
                if (item.OmitSignatureDate == false)
                    item.InternalModificationInfo = signatureDate;
            }
            return dataColumn;
        }

        public InternalDataColumn AddColumn(ColumnInfo columnInfo)
        {
            this.ValidateAddColumn(columnInfo);
            var signatureDate = this.Sign();
            if (this.OmitSignatureDate == false)
                this.InternalModificationInfo = signatureDate;
            var dataColumn = this.AddColumnInternal(columnInfo);
            foreach (var item in this.DerivedItems.ToArray())
            {
                item.AddColumnInternal(columnInfo);
                if (item.OmitSignatureDate == false)
                    item.InternalModificationInfo = signatureDate;
            }
            return dataColumn;
        }

        public void RemoveColumn(InternalDataColumn dataColumn)
        {
            this.ValidateRemoveColumn(dataColumn);
            var signatureDate = this.Sign();
            if (this.OmitSignatureDate == false)
                this.InternalModificationInfo = signatureDate;
            this.RemoveColumnInternal(dataColumn);
            foreach (var item in this.DerivedItems.ToArray())
            {
                item.RemoveColumnInternal(dataColumn.ColumnName);
                if (item.OmitSignatureDate == false)
                    item.InternalModificationInfo = signatureDate;
            }
        }

        public void RemoveColumn(string columnName)
        {
            this.ValidateRemoveColumn(columnName);
            var signatureDate = this.Sign();
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.RemoveColumnInternal(columnName);
                if (item.OmitSignatureDate == false)
                    item.InternalModificationInfo = signatureDate;
            }
        }

        public bool CanRemoveColumn(InternalDataColumn dataColumn)
        {
            if (this.TemplateNamespace != string.Empty)
                return false;
            return this.Columns.CanRemove(dataColumn);
        }

        public void ClearColumns()
        {
            this.ValidateClearColumns();
            var signatureDate = this.Sign();
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.ClearColumnsInternal();
                if (item.OmitSignatureDate == false)
                    item.InternalModificationInfo = signatureDate;
            }
        }

        public void AddChild(InternalDataTable value)
        {
            this.ValidateAddChild(value);
        }

        public InternalDataTable AddChild()
        {
            var childNames = from item in this.ChildItems select item.LocalName;
            var childName = NameUtility.GenerateNewName(childNameString, EnumerableUtility.Friends(this.LocalName, childNames));
            return this.AddChild(childName);
        }

        public InternalDataTable AddChild(string localName)
        {
            this.ValidateAddChild(localName);
            this.Sign();

            var dataTable = this.AddChildInternal(localName);
            foreach (var item in this.DerivedItems.ToArray())
            {
                item.AddChildInternal(localName);
            }
            return dataTable;
        }

        public InternalDataTable AddChild(TableInfo tableInfo)
        {
            this.ValidateAddChild(tableInfo);
            this.Sign();

            var dataTable = this.AddChildInternal(tableInfo);
            foreach (var item in this.DerivedItems.ToArray())
            {
                item.AddChildInternal(tableInfo);
            }
            return dataTable;
        }

        public void RemoveChild(InternalDataTable dataTable)
        {
            this.ValidateRemoveChild(dataTable);
            this.Sign();
            this.RemoveChildInternal(dataTable);
            foreach (var item in this.DerivedItems.ToArray())
            {
                item.RemoveChildInternal(dataTable.LocalName);
            }
        }

        public bool CanRemoveChild(InternalDataTable dataTable)
        {
            if (this.TemplateNamespace != string.Empty)
                return false;
            return true;
        }

        public void ClearChilds()
        {
            throw new NotImplementedException();
        }

        public void BeginLoad()
        {
            foreach (var item in this.FamilyItems.ToArray())
            {
                item.BeginLoadData();
            }
        }

        public void EndLoad()
        {
            foreach (var item in this.FamilyItems.Reverse().ToArray())
            {
                item.EndLoadData();
            }
        }

        public void ValidateAddChild(InternalDataTable value)
        {
            this.ValidateInvoke(nameof(this.AddChild));
        }

        public void ValidateAddChild(string value)
        {
            this.ValidateInvoke(nameof(this.AddChild));
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateAddChild(value);
            }
        }

        public void ValidateAddChild(TableInfo tableInfo)
        {
            this.ValidateInvoke(nameof(this.AddChild));
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateAddChild(tableInfo);
            }
        }

        public void ValidateRemoveChild(InternalDataTable dataTable)
        {
            this.ValidateInvoke(nameof(this.RemoveChild));
            this.OnValidateRemoveChild(dataTable);
        }

        public void ValidateRemoveChild(string localName)
        {
            this.ValidateInvoke(nameof(this.RemoveChild));
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateRemoveChild(localName);
            }
        }

        public void ValidateAddRow(InternalDataRow dataRow)
        {

        }

        public void ValidateRemoveRow(InternalDataRow dataRow)
        {

        }

        public void ValidateRemoveRowAt(int index)
        {

        }

        public void ValidateAddColumn(InternalDataColumn dataColumn)
        {
            this.ValidateInvoke(nameof(this.AddColumn));
            this.OnValidateAddColumn(dataColumn);
        }

        public void ValidateAddColumn(string columnName)
        {
            this.ValidateInvoke(nameof(this.AddColumn));
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateAddColumn(columnName);
            }
        }

        public void ValidateAddColumn(string columnName, Type dataType)
        {
            this.ValidateInvoke(nameof(this.AddColumn));
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateAddColumn(columnName, dataType);
            }
        }

        public void ValidateAddColumn(string columnName, InternalDataType dataType)
        {
            this.ValidateInvoke(nameof(this.AddColumn));
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateAddColumn(columnName, dataType);
            }
        }

        public void ValidateAddColumn(string columnName, string dataTypeName)
        {
            this.ValidateInvoke(nameof(this.AddColumn));
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateAddColumn(columnName, dataTypeName);
            }
        }

        public void ValidateAddColumn(ColumnInfo columnInfo)
        {
            this.ValidateInvoke(nameof(this.AddColumn));
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateAddColumn(columnInfo);
            }
        }

        public void ValidateRemoveColumn(InternalDataColumn dataColumn)
        {
            this.ValidateInvoke(nameof(this.RemoveColumn));
            this.OnValidateRemoveColumn(dataColumn);
        }

        public void ValidateRemoveColumn(string columnName)
        {
            this.ValidateInvoke(nameof(this.RemoveColumn));
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateRemoveColumn(columnName);
            }
        }

        public void ValidateClearColumns()
        {
            this.ValidateInvoke(nameof(this.ClearColumns));
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateClearColumns();
            }
        }

        public void UpdateIndexInternal(InternalDataColumn dataColumn)
        {
            var query = from item in this.columnList
                        orderby item.Index
                        select item;

            var columns = query.ToList();
            if (dataColumn != null)
            {
                var index = dataColumn.Index;
                columns.Remove(dataColumn);
                columns.Insert(index, dataColumn);
            }
            var columnList = new List<InternalDataColumn>(columns.Count);
            for (var i = 0; i < columns.Count; i++)
            {
                var item = columns[i];
                if (item.Index != i)
                {
                    item.InternalIndex = i;
                    columnList.Add(item);
                }
                else if (item == dataColumn)
                {
                    columnList.Add(item);
                }
            }
            this.columnList.Clear();
            this.columnList.AddRange(columns);
        }

        public override void BeginLoadData()
        {
            this.autoIncrements = new bool[this.Columns.Count];
            for (var i = 0; i < this.Columns.Count; i++)
            {
                var column = this.Columns[i];
                this.autoIncrements[i] = column.AutoIncrement;
                column.AutoIncrement = false;
            }
            base.BeginLoadData();
        }

        public override void EndLoadData()
        {
            base.EndLoadData();
            if (this.autoIncrements != null)
            {
                for (var i = 0; i < this.autoIncrements.Length; i++)
                {
                    var column = this.Columns[i];
                    if (this.autoIncrements[i] == false || CremaDataTypeUtility.CanUseAutoIncrement(column.DataType) == false || column.DefaultValue != DBNull.Value)
                        continue;
                    column.AutoIncrement = true;
                }
                this.autoIncrements = null;
            }
        }

        public static explicit operator CremaDataTable(InternalDataTable dataTable)
        {
            if (dataTable == null)
                return null;
            return dataTable.Target as CremaDataTable;
        }

        public static explicit operator InternalDataTable(CremaDataTable dataTable)
        {
            if (dataTable == null)
                return null;
            return dataTable.InternalObject;
        }

        public new CremaDataTable Target => base.Target as CremaDataTable;

        public new InternalDataSet DataSet => base.DataSet as InternalDataSet;

        public IList<InternalDataColumn> ColumnList => this.columnList;

        public new ReadOnlyObservableCollection<InternalDataTable> ChildItems { get; }

        public new IEnumerable<InternalDataTable> FamilyItems
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

        public new IEnumerable<InternalDataTable> DerivedItems
        {
            get
            {
                foreach (var item in base.DerivedItems)
                {
                    if (item is InternalDataTable dataTable)
                        yield return dataTable;
                }
            }
        }

        public new IEnumerable<InternalDataTable> SiblingItems
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

        public TagInfo Tags
        {
            get => this.InternalTags;
            set
            {
                if (this.InternalTags == value)
                    return;

                var signatureDate = this.Sign();
                foreach (var item in this.SiblingItems.ToArray())
                {
                    item.InternalTags = value;
                    if (item.OmitSignatureDate == false)
                        item.InternalModificationInfo = signatureDate;
                }
            }
        }

        public TagInfo DerivedTags
        {
            get
            {
                if (this.Parent is InternalDataTable parent)
                {
                    return this.InternalTags & parent.Tags;
                }
                if (this.TemplatedParent is InternalDataTable templatedParent)
                {
                    return this.InternalTags & templatedParent.Tags;
                }
                return this.InternalTags;
            }
        }

        public TableInfo TableInfo
        {
            get
            {
                var tableInfo = new TableInfo()
                {
                    ID = this.TableID,
                    Name = this.Name,
                    CategoryPath = this.CategoryPath,
                    Tags = this.Tags,
                    DerivedTags = this.DerivedTags,
                    Comment = this.Comment,
                    CreationInfo = this.InternalCreationInfo,
                    ModificationInfo = this.InternalModificationInfo,
                    ContentsInfo = this.InternalContentsInfo,
                    Columns = this.columnList.Select(item => item.ColumnInfo).ToArray(),
                    TemplatedParent = this.TemplatedParentName,
                };
                tableInfo.HashValue = GenerateHashValue(tableInfo.Columns);
                return tableInfo;
            }
        }

        public string HashValue
        {
            get
            {
                var columns = this.columnList.Select(item => item.ColumnInfo).ToArray();
                return GenerateHashValue(columns);
            }
        }

        public Guid TableID { get; private set; } = Guid.NewGuid();

        public string Comment
        {
            get => this.InternalComment ?? string.Empty;
            set
            {
                if (this.InternalComment == value)
                    return;

                var signatureDate = this.Sign();
                foreach (var item in this.SiblingItems.ToArray())
                {
                    item.InternalComment = value;
                    if (item.OmitSignatureDate == false)
                        item.InternalModificationInfo = signatureDate;
                }
            }
        }

        public SignatureDate CreationInfo => this.InternalCreationInfo;

        public SignatureDate ModificationInfo => this.InternalModificationInfo;

        public SignatureDate ContentsInfo => this.InternalContentsInfo;

        public TagInfo InternalTags
        {
            get => this.tags;
            set
            {
                this.tags = value;
                this.attributeTag.InternalDefaultValue = (string)value;
            }
        }

        public string InternalComment { get; set; }

        public Guid InternalTableID
        {
            get => this.TableID;
            set => this.TableID = value;
        }

        public SignatureDate InternalCreationInfo { get; set; } = SignatureDate.Empty;

        public SignatureDate InternalModificationInfo { get; set; } = SignatureDate.Empty;

        public SignatureDate InternalContentsInfo { get; set; } = SignatureDate.Empty;

        protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
        {
            this.Target.builder.InternalBuilder = builder;
            return new InternalDataRow(this.Target.builder, this);
        }

        protected override void OnTableNewRow(DataTableNewRowEventArgs e)
        {
            base.OnTableNewRow(e);

            if (this.IsLoading == false)
            {
                foreach (DataColumn item in this.Columns)
                {
                    if (item.AutoIncrement == false)
                        continue;

                    var query = from DataRow i in this.Rows
                                where i.RowState != DataRowState.Deleted
                                where i[item] != DBNull.Value
                                select i;

                    if (query.Any() == false)
                    {
                        e.Row[item] = longConverter.ConvertTo(item.AutoIncrementSeed, item.DataType);
                    }
                    else
                    {
                        TypeConverter cc = TypeDescriptor.GetConverter(item.DataType);

                        long maxValue = query.Max(i => (long)cc.ConvertTo(i[item], typeof(long)));
                        e.Row[item] = longConverter.ConvertTo(maxValue + item.AutoIncrementStep, item.DataType);
                    }
                }
            }
        }

        protected override void OnColumnChanging(DataColumnChangeEventArgs e)
        {
            var column = e.Column as InternalDataColumn;
            var row = e.Row as InternalDataRow;

            if (this.IsDiffMode == false)
                this.ValidateCremaType(e);

            base.OnColumnChanging(e);
        }

        protected override void OnColumnChanged(DataColumnChangeEventArgs e)
        {
            base.OnColumnChanged(e);
        }

        protected override void OnRowChanging(DataRowChangeEventArgs e)
        {
            base.OnRowChanging(e);
        }

        protected override void OnRowChanged(DataRowChangeEventArgs e)
        {
            base.OnRowChanged(e);
        }

        protected override void OnRowDeleting(DataRowChangeEventArgs e)
        {
            this.Sign();
            base.OnRowDeleting(e);
        }

        protected override void OnRowDeleted(DataRowChangeEventArgs e)
        {
            base.OnRowDeleted(e);
            if (this.IsLoading == false)
            {
                this.InternalContentsInfo = this.SignatureDate;
            }
        }

        protected override void OnTableCleared(DataTableClearEventArgs e)
        {
            base.OnTableCleared(e);
            if (this.IsLoading == false)
            {
                this.InternalContentsInfo = this.SignatureDate;
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        protected override void OnSetNormalMode()
        {
            base.OnSetNormalMode();

            this.attributeTag.InternalDefaultValue = (string)TagInfo.All;
            this.attributeTag.InternalAllowDBNull = false;

            this.attributeEnable.InternalDefaultValue = true;
            this.attributeEnable.InternalAllowDBNull = false;
        }

        protected override void OnSetDiffMode()
        {
            base.OnSetDiffMode();

            lock (CremaSchema.lockobj)
            {
                this.PrimaryKey = null;
            }

            this.attributeTag.InternalDefaultValue = DBNull.Value;
            this.attributeTag.InternalAllowDBNull = true;

            this.attributeEnable.InternalDefaultValue = DBNull.Value;
            this.attributeEnable.InternalAllowDBNull = true;
        }

        protected virtual void OnValidateAddColumn(InternalDataColumn dataColumn)
        {
            if (this.ChildItems.Where(item => item.LocalName == dataColumn.ColumnName).Any() == true)
                throw new CremaDataException("{0} 은(는) 자식 테이블의 이름으로 사용되고 있으므로 사용할 수 없습니다.", dataColumn.ColumnName);
            if (dataColumn.IsKey == true && this.Rows.Count > 0)
                throw new CremaDataException("데이터가 있을때는 키로 설정된 열을 추가할 수 없습니다.");
            if (dataColumn.AllowDBNull == false && this.Rows.Count > 0)
                throw new CremaDataException("데이터가 있을때는 null값을 허용하지 않는 열을 추가할 수 없습니다.");
            if (dataColumn.Unique == true && this.Rows.Count > 0)
                throw new CremaDataException("데이터가 있을때는 고유 속성인 열을 추가할 수 없습니다.");

            var dataType = dataColumn.CremaType;
            if (dataType != null && this.DataSet != null && dataType.DataSet != this.DataSet)
                throw new CremaDataException("테이블이 DataSet에 속해있지 않기 때문에 타입 '{0}'을(를) 사용할 수 없습니다.", dataType);
        }

        protected virtual void OnValidateAddColumn(string columnName)
        {
            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));
            if (columnName == CremaSchema.__RelationID__)
                throw new CremaDataException("'{0}'은(는) 예약되어 있는 이름이기 때문에 사용할 수 없습니다.");
        }

        protected virtual void OnValidateAddColumn(string columnName, Type type)
        {
            this.OnValidateAddColumn(columnName);
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (CremaDataTypeUtility.IsBaseType(type) == false)
                throw new ArgumentException(string.Format(Resources.Exception_TypeCannotBeUsed_Format, type.Name), nameof(type));
        }

        protected virtual void OnValidateAddColumn(string columnName, InternalDataType type)
        {
            this.OnValidateAddColumn(columnName);
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (type.DataSet != this.DataSet)
                throw new CremaDataException("'{0}'은(는) 같은 CremaDataSet에 속하지 않은 타입입니다.", type.LocalName);
        }

        protected virtual void OnValidateAddColumn(string columnName, string dataTypeName)
        {
            this.OnValidateAddColumn(columnName);
        }

        protected virtual void OnValidateAddColumn(ColumnInfo columnInfo)
        {

        }

        protected virtual void OnValidateRemoveColumn(InternalDataColumn dataColumn)
        {
            if (dataColumn == null)
                throw new ArgumentNullException(nameof(dataColumn));
            if (this.Columns.IndexOf(dataColumn) < 0)
                throw new InvalidOperationException("이 테이블에 속하지 않는 열은 제거할 수 없습니다.");
            if (dataColumn.IsKey == true)
                throw new InvalidOperationException("이 열은 기본 키의 일부이므로 제거할 수 없습니다.'");
            if (dataColumn.Unique == true)
                throw new InvalidOperationException("이 열은  테이블에서 Constraint1 제약 조건의 일부이므로 제거할 수 없습니다.'");
        }

        protected virtual void OnValidateRemoveColumn(string columnName)
        {

        }

        protected virtual void OnValidateClearColumns()
        {

        }

        protected virtual void OnValidateAddChild(string value)
        {
            if (this.LocalName == string.Empty)
                throw new InvalidOperationException("부모 테이블의 이름이 없습니다.");

            foreach (var item in this.Columns)
            {
                if (item is InternalAttribute attributeItem && attributeItem.AttributeName == value)
                {
                    throw new ArgumentException($"'{value}' '{this.Name}'의 특성의 이름이기 때문에 사용할 수 없습니다.");
                }

                if (item is InternalDataColumn columnItem && columnItem.ColumnName == value)
                {
                    throw new ArgumentException($"'{value}' '{this.Name}'의 열의 이름이기 때문에 사용할 수 없습니다.");
                }

                if (item is InternalRelation relationItem && relationItem.RelationName == value)
                {
                    throw new ArgumentException($"'{value}' '{this.Name}'의 관계열의 이름이기 때문에 사용할 수 없습니다.");
                }
            }

            foreach (var item in this.ChildItems)
            {
                if (item.LocalName == value)
                {
                    throw new ArgumentException($"'{value}' '{this.Name}'의 자식의 이름이기 때문에 사용할 수 없습니다.");
                }
            }

            if (this.LocalName == value)
            {
                if (this.TemplatedParentName != string.Empty)
                    throw new ArgumentException($"'{value}' 상속된 '{this.Name}'의 이름이기 때문에 사용할 수 없습니다.");
                else
                    throw new ArgumentException($"'{value}' '{this.Name}'의 이름이기 때문에 사용할 수 없습니다.");
            }
        }

        protected virtual void OnValidateAddChild(TableInfo tableInfo)
        {
            if (tableInfo.ParentName != tableInfo.ParentName)
                throw new CremaDataException("부모가 다른 테이블은 추가할 수 없습니다.");
            if (tableInfo.TemplatedParent != string.Empty)
                throw new CremaDataException("상속받은 테이블은 추가할 수 없습니다.");
        }

        protected virtual void OnValidateRemoveChild(InternalDataTable dataTable)
        {
            if (this.childList.Contains(dataTable) == false)
                throw new CremaDataException("자식 목록에 있지 않은 테이블은 제거할 수 없습니다.");
        }

        protected virtual void OnValidateRemoveChild(string localName)
        {

        }

        protected override string BaseNamespace
        {
            get
            {
                if (this.DataSet == null)
                    return CremaSchema.TableNamespace;
                return this.DataSet.TableNamespace;
            }
        }

        private void ValidateCremaType(DataColumnChangeEventArgs e)
        {
            if (e.Column is InternalDataColumn column)
            {
                var proposedValue = e.ProposedValue;

                if (proposedValue == null || proposedValue == DBNull.Value)
                    return;

                if (column.CremaType == null)
                    return;

                var dataType = column.CremaType;
                if (column.Editor == null && dataType.HasChanges == true)
                    throw new ArgumentException("커밋되지 않은 타입은 사용할 수 없습니다.");

                var textValue = e.ProposedValue as string;
                if (long.TryParse(textValue, out long value) == true)
                {
                    textValue = dataType.ConvertToString(value);
                    e.ProposedValue = textValue;
                    return;
                }

                dataType.ValidateValue(textValue);
            }
        }

        private void ChildTables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is InternalDataTable tableItem)
                                this.childList.Add(tableItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (item is InternalDataTable tableItem)
                                this.childList.Remove(tableItem);
                        }
                    }
                    break;
            }
        }

        private void Constraints_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {

        }

        private void Columns_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    {
                        if (e.Element is InternalDataColumn dataColumn && dataColumn.IsInternalAction == false)
                        {
                            if (dataColumn.IsKey == true)
                            {
                                dataColumn.Table.AddKey(dataColumn);
                            }

                            if (dataColumn.CremaType != null)
                            {
                                dataColumn.CremaType.AddReference(dataColumn);
                            }

                            dataColumn.InternalIndex = this.columnList.Count;
                            this.columnList.Add(dataColumn);

                            if (this.OmitSignatureDate == false)
                            {
                                dataColumn.InternalCreationInfo = this.SignatureDate;
                                dataColumn.InternalModificationInfo = this.SignatureDate;
                            }
                        }
                    }
                    break;
                case CollectionChangeAction.Remove:
                    {
                        if (e.Element is InternalDataColumn dataColumn && dataColumn.IsInternalAction == false)
                        {
                            if (dataColumn.CremaType != null)
                            {
                                dataColumn.CremaType.RemoveReference(dataColumn);
                            }

                            this.columnList.Remove(dataColumn);

                            if (this.OmitSignatureDate == false)
                            {
                                dataColumn.InternalCreationInfo = SignatureDate.Empty;
                                dataColumn.InternalModificationInfo = SignatureDate.Empty;
                            }
                            dataColumn.InternalIndex = -1;
                            for (var i = 0; i < this.columnList.Count; i++)
                            {
                                this.columnList[i].InternalIndex = i;
                            }
                        }
                    }
                    break;
                case CollectionChangeAction.Refresh:
                    {
                        this.columnList.Clear();
                        foreach (var item in this.Columns)
                        {
                            if (item is InternalDataColumn dataColumn)
                            {
                                this.columnList.Add(dataColumn);
                            }
                        }

                        for (var i = 0; i < this.columnList.Count; i++)
                        {
                            this.columnList[i].InternalIndex = i;
                        }
                    }
                    break;
            }
        }

        private void AddColumnInternal(InternalDataColumn dataColumn)
        {
            this.Columns.Add(dataColumn);
        }

        private InternalDataColumn AddColumnInternal(string columnName)
        {
            var dataColumn = new InternalDataColumn()
            {
                InternalColumnName = columnName,
            };
            this.Columns.Add(dataColumn);
            return dataColumn;
        }

        private InternalDataColumn AddColumnInternal(string columnName, Type dataType)
        {
            var dataColumn = new InternalDataColumn()
            {
                InternalColumnName = columnName,
                InternalDataType = dataType,
            };
            this.Columns.Add(dataColumn);
            return dataColumn;
        }

        private InternalDataColumn AddColumnInternal(string columnName, InternalDataType dataType)
        {
            var dataColumn = new InternalDataColumn()
            {
                InternalColumnName = columnName,
                InternalCremaType = dataType,
            };
            this.Columns.Add(dataColumn);
            return dataColumn;
        }

        private InternalDataColumn AddColumnInternal(ColumnInfo columnInfo)
        {
            var dataColumn = new InternalDataColumn()
            {
                InternalColumnName = columnInfo.Name,
                InternalColumnID = columnInfo.ID,
                InternalIsKey = columnInfo.IsKey,
                InternalAllowDBNull = columnInfo.AllowNull,
                InternalTags = columnInfo.Tags,
                InternalDefaultValue = columnInfo.DefaultValue == null ? DBNull.Value : (object)columnInfo.DefaultValue,
                InternalUnique = columnInfo.IsUnique,
                InternalComment = columnInfo.Comment,
                InternalReadOnly = columnInfo.ReadOnly,
                InternalAutoIncrement = columnInfo.AutoIncrement,

            };
            this.Columns.Add(dataColumn);

            dataColumn.InternalDataTypeName = columnInfo.DataType;
            dataColumn.InternalCreationInfo = columnInfo.CreationInfo;
            dataColumn.InternalModificationInfo = columnInfo.ModificationInfo;

            return dataColumn;
        }

        private void RemoveColumnInternal(InternalDataColumn dataColumn)
        {
            this.Columns.Remove(dataColumn);
        }

        private void RemoveColumnInternal(string columnName)
        {
            this.Columns.Remove(columnName);
        }

        private void ClearColumnsInternal()
        {
            this.Columns.CollectionChanged -= Columns_CollectionChanged;
            this.PrimaryKey = null;
            foreach (var item in this.columnList)
            {
                if (item.CremaType != null)
                {
                    item.CremaType.RemoveReference(item);
                }
                this.Columns.Remove(item);
                item.InternalCreationInfo = SignatureDate.Empty;
                item.InternalModificationInfo = SignatureDate.Empty;
                item.InternalIndex = -1;
            }
            this.columnList.Clear();
            this.Columns.CollectionChanged += Columns_CollectionChanged;
        }

        private InternalDataTable AddChildInternal(string localName)
        {
            var childTable = new InternalDataTable(localName, this.CategoryPath)
            {
                InternalParent = this
            };

            if (this.DataSet != null)
            {
                var signatureDate = this.DataSet.Sign();
                this.DataSet.Tables.Add(childTable);
                childTable.InternalCreationInfo = signatureDate;
                childTable.InternalModificationInfo = signatureDate;
            }
            return childTable;
        }

        private InternalDataTable AddChildInternal(TableInfo tableInfo)
        {
            var dataTable = new InternalDataTable(tableInfo.TableName, tableInfo.CategoryPath)
            {
                InternalTableID = tableInfo.ID,
                InternalTags = tableInfo.Tags,
                InternalComment = tableInfo.Comment,
                InternalParent = this,
            };

            if (this.DataSet != null)
            {
                this.DataSet.Tables.Add(dataTable);
            }

            for (var i = 0; i < tableInfo.Columns.Length; i++)
            {
                dataTable.AddColumn(tableInfo.Columns[i]);
            }

            dataTable.InternalCreationInfo = tableInfo.CreationInfo;
            dataTable.InternalModificationInfo = tableInfo.ModificationInfo;
            dataTable.InternalContentsInfo = tableInfo.ContentsInfo;

            return dataTable;
        }

        private void RemoveChildInternal(InternalDataTable dataTable)
        {
            if (this.DataSet != null)
            {
                this.DataSet.Tables.Remove(dataTable);
            }
            dataTable.InternalParent = null;
        }

        private void RemoveChildInternal(string localName)
        {
            var dataTable = this.childList.Single(item => item.LocalName == localName);
            if (this.DataSet != null)
            {
                this.DataSet.Tables.Remove(dataTable);
            }
            dataTable.InternalParent = null;
        }
    }
}
