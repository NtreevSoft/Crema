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
using Ntreev.Library;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Data
{
    class InternalDataColumn : InternalColumnBase
    {
        private Guid columnID = Guid.NewGuid();
        private int index = -1;
        private InternalDataType cremaType;
        private TagInfo tags = TagInfo.All;
        private string validation;
        private string comment;

        private SignatureDate creationInfo = SignatureDate.Empty;
        private SignatureDate modificationInfo = SignatureDate.Empty;

        public InternalDataColumn()
        {
            base.Target = new CremaDataColumn(this);
            this.ColumnMapping = MappingType.Element;
        }

        public InternalDataColumn(CremaDataColumn target, string columnName, Type dataType)
            : base(columnName, dataType)
        {
            this.ValidateConstructor(columnName, dataType);
            base.Target = target;
            this.ColumnMapping = MappingType.Element;
        }

        public InternalDataColumn(CremaDataColumn target, string columnName, InternalDataType dataType)
            : base(columnName, typeof(string))
        {
            this.ValidateConstructor(columnName, dataType);
            base.Target = target;
            this.ColumnMapping = MappingType.Element;
            this.cremaType = dataType;
        }

        public bool VerifyUnique()
        {
            if (this.Table == null)
                throw new InvalidOperationException();

            var fieldList = new List<object>(this.Table.Rows.Count);

            for (var i = 0; i < this.Table.Rows.Count; i++)
            {
                var dataRow = this.Table.Rows[i];
                fieldList.Add(dataRow[this]);
            }
            return fieldList.Distinct().Count() == this.Table.Rows.Count;
        }

        public bool VerifyNotAllowNull()
        {
            if (this.Table == null)
                throw new InvalidOperationException();

            for (var i = 0; i < this.Table.Rows.Count; i++)
            {
                var dataRow = this.Table.Rows[i];
                if (dataRow[this] is DBNull == true)
                    return false;
            }
            return true;
        }

        public object ConvertFromString(string textValue)
        {
            if (string.IsNullOrEmpty(textValue) == true)
                return DBNull.Value;
            if (this.CremaType != null)
                return textValue;
            var converter = TypeDescriptor.GetConverter(this.DataType);
            return converter.ConvertFromString(textValue);
        }

        public void CopyTo(InternalDataColumn dest)
        {
            dest.InternalAllowDBNull = this.InternalAllowDBNull;
            dest.InternalAutoIncrement = this.InternalAutoIncrement;
            dest.InternalColumnName = this.InternalColumnName;
            dest.InternalDataType = this.InternalDataType;
            dest.InternalCremaType = this.InternalCremaType;
            dest.InternalDefaultValue = this.InternalDefaultValue;
            dest.InternalExpression = this.InternalExpression;
            dest.InternalValidation = this.InternalValidation;
            dest.InternalUnique = this.InternalUnique;
            dest.InternalReadOnly = this.InternalReadOnly;
            dest.InternalComment = this.InternalComment;
            dest.InternalIsKey = this.InternalIsKey;
            dest.InternalCreationInfo = this.InternalCreationInfo;
            dest.InternalModificationInfo = this.InternalModificationInfo;
            dest.InternalTags = this.InternalTags;
            dest.InternalColumnID = this.InternalColumnID;
        }

        public void ValidateSetColumnID(Guid value)
        {
            this.ValidateSetProperty(nameof(this.ColumnID));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                this.OnValidateSetColumnID(value);
            }
        }

        public void ValidateSetIndex(int value)
        {
            this.ValidateSetProperty(nameof(this.Index));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                item.OnValidateSetIndex(value);
            }
        }

        public void ValidateSetCremaType(InternalDataType value)
        {
            this.ValidateSetProperty(nameof(this.CremaType));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                item.OnValidateSetCremaType(value);
            }
        }

        public void ValidateSetDataTypeName(string value)
        {
            this.ValidateSetProperty(nameof(this.DataTypeName));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                item.OnValidateSetDataTypeName(value);
            }
        }

        public void ValidateSetTags(TagInfo value)
        {
            this.ValidateSetProperty(nameof(this.Tags));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                item.OnValidateSetTags(value);
            }
        }

        public void ValidateSetComment(string value)
        {
            this.ValidateSetProperty(nameof(this.Comment));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                item.OnValidateSetComment(value);
            }
        }

        public void ValidateSetValidation(string value)
        {
            this.ValidateSetProperty(nameof(this.Validation));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                item.OnValidateSetValidation(value);
            }
        }

        public new CremaDataColumn Target => base.Target as CremaDataColumn;

        public Guid ColumnID
        {
            get => this.InternalColumnID;
            set
            {
                if (this.InternalColumnID == value)
                    return;
                this.ValidateSetColumnID(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalColumnID = value;
                }
            }
        }

        public new Type DataType
        {
            get => this.InternalDataType;
            set
            {
                if (this.InternalCremaType == null && this.InternalDataType == value)
                    return;
                this.ValidateSetDataType(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalCremaType = null;
                    item.InternalDataType = value;
                }
            }
        }

        public InternalDataType CremaType
        {
            get => this.InternalCremaType;
            set
            {
                if (this.InternalCremaType == value)
                    return;
                this.ValidateSetCremaType(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalCremaType = value;
                }
            }
        }

        public new object DefaultValue
        {
            get => this.InternalDefaultValue;
            set
            {
                if (this.InternalDefaultValue == value)
                    return;
                this.ValidateSetDefaultValue(value);
                value = ConvertToCremaType();
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalDefaultValue = value;
                }

                object ConvertToCremaType()
                {
                    if (value == null || value == DBNull.Value)
                        return DBNull.Value;
                    if (this.CremaType != null && CremaConvert.VerifyChangeType(value, typeof(long)) == true)
                    {
                        var numericValue = (long)CremaConvert.ChangeType(value, typeof(long));
                        return this.CremaType.ConvertToString(numericValue);
                    }
                    else
                    {
                        return CremaConvert.ChangeType(value, this.DataType);
                    }
                }
            }
        }

        public int Index
        {
            get => this.InternalIndex;
            set
            {
                if (this.InternalIndex == value)
                    return;
                this.ValidateSetIndex(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalIndex = value;
                    item.Table.UpdateIndexInternal(item);
                }
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
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalComment = value;
                }
            }
        }

        public string Validation
        {
            get => this.InternalValidation ?? string.Empty;
            set
            {
                if (this.InternalValidation == value)
                    return;
                this.ValidateSetValidation(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalValidation = value;
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
                this.ValidateSetTags(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalTags = value;
                }
            }
        }

        public string DataTypeName
        {
            get => this.InternalDataTypeName;
            set
            {
                if (this.InternalDataTypeName == value)
                    return;
                this.ValidateSetDataTypeName(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalDataTypeName = value;
                }
            }
        }

        public SignatureDate CreationInfo
        {
            get => this.InternalCreationInfo;
            set
            {
                if (this.InternalCreationInfo == value)
                    return;
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalCreationInfo = value;
                }
            }
        }

        public SignatureDate ModificationInfo
        {
            get => this.InternalModificationInfo;
            set
            {
                this.InternalModificationInfo = value;
                if (this.InternalModificationInfo == value)
                    return;
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalModificationInfo = value;
                }
            }
        }

        public new InternalDataTable Table => base.Table as InternalDataTable;

        public string DefaultString
        {
            get
            {
                if (this.DefaultValue == null || this.DefaultValue == DBNull.Value)
                    return null;
                return CremaConvert.ChangeType(this.DefaultValue, typeof(string)) as string;
            }
        }

        public Guid InternalColumnID
        {
            get => this.columnID;
            set
            {
                this.columnID = value;
                this.InvokePropertyChangedEvent(nameof(this.ColumnID));
            }
        }

        public int InternalIndex
        {
            get => this.index;
            set
            {
                this.index = value;
                this.InvokePropertyChangedEvent(nameof(this.Index));
            }
        }

        public InternalDataType InternalCremaType
        {
            get => this.cremaType;
            set
            {
                if (this.Table != null && this.cremaType != null)
                {
                    this.cremaType.RemoveReference(this);
                }

                if (value != null)
                {
                    this.cremaType = value;
                    base.DataType = typeof(string);
                    if (value.VerifyValue(this.DefaultValue.ToString()) == false)
                        this.DefaultValue = DBNull.Value;
                }
                else
                {
                    this.cremaType = null;
                }

                if (this.Table != null && this.cremaType != null)
                {
                    this.cremaType.AddReference(this);
                }
                this.InvokePropertyChangedEvent(nameof(this.CremaType));
            }
        }

        public TagInfo InternalTags
        {
            get => this.tags;
            set
            {
                this.tags = value;
                this.InvokePropertyChangedEvent(nameof(this.Tags));
            }
        }

        public string InternalValidation
        {
            get => this.validation;
            set
            {
                this.validation = value;
                this.InvokePropertyChangedEvent(nameof(this.Validation));
            }
        }

        public string InternalComment
        {
            get => this.comment;
            set
            {
                this.comment = value;
                this.InvokePropertyChangedEvent(nameof(this.Comment));
            }
        }

        public string InternalDataTypeName
        {
            get
            {
                if (this.cremaType != null)
                {
                    var categoryPath = InternalDataSet.GetTypeCategoryPath(this.cremaType.DataSet, this.cremaType.Namespace);
                    var typeName = InternalDataSet.GetTypeName(this.cremaType.DataSet, this.cremaType.Namespace);
                    return categoryPath + typeName;
                }
                return base.DataType.GetTypeName();
            }
            set
            {
                if (CremaDataTypeUtility.IsBaseType(value) == true)
                {
                    this.InternalCremaType = null;
                    this.InternalDataType = CremaDataTypeUtility.GetType(value);
                }
                else
                {
                    if (NameValidator.VerifyItemPath(value) == false)
                    {
                        var dataType = InternalDataSet.GetType(this.Table.DataSet, value);
                        this.InternalCremaType = dataType;
                        this.InternalDataType = typeof(string);
                    }
                    else
                    {
                        var itemName = new ItemName(value);
                        var dataType = InternalDataSet.GetType(this.Table.DataSet, itemName.Name, itemName.CategoryPath);
                        this.InternalCremaType = dataType;
                        this.InternalDataType = typeof(string);
                    }
                }
            }
        }

        public SignatureDate InternalCreationInfo
        {
            get => this.creationInfo;
            set
            {
                this.creationInfo = value;
                this.InvokePropertyChangedEvent(nameof(this.CreationInfo));
            }
        }

        public SignatureDate InternalModificationInfo
        {
            get => this.modificationInfo;
            set
            {
                this.modificationInfo = value;
                this.InvokePropertyChangedEvent(nameof(this.ModificationInfo));
            }
        }

        public TagInfo DerivedTags
        {
            get
            {
                var tags = this.Tags;
                if (this.Table != null)
                {
                    if (this.IsKey == true)
                        tags = this.Table.DerivedTags;
                    else
                        tags = tags & this.Table.DerivedTags;
                }
                return tags;
            }
        }

        public ColumnInfo ColumnInfo
        {
            get
            {
                return new ColumnInfo()
                {
                    ID = this.ColumnID,
                    IsKey = this.IsKey,
                    IsUnique = this.Unique,
                    AllowNull = this.AllowDBNull,
                    DataType = this.DataTypeName,
                    Tags = this.Tags,
                    DerivedTags = this.DerivedTags,
                    DefaultValue = this.DefaultString,
                    AutoIncrement = this.AutoIncrement,
                    Comment = this.Comment,
                    Name = this.ColumnName,
                    CreationInfo = this.CreationInfo,
                    ModificationInfo = this.ModificationInfo,
                };
            }
        }

        public ColumnInfo DiffColumnInfo
        {
            get
            {
                return new ColumnInfo()
                {
                    ID = this.ColumnID,
                    IsKey = this.IsKey,
                    IsUnique = this.Unique,
                    AllowNull = this.AllowDBNull,
                    DataType = this.ExtendedProperties.ContainsKey(nameof(this.DataTypeName)) ? this.ExtendedProperties[nameof(this.DataTypeName)] as string : this.DataTypeName,
                    Tags = this.Tags,
                    DerivedTags = this.DerivedTags,
                    DefaultValue = this.DefaultString,
                    AutoIncrement = this.AutoIncrement,
                    Comment = this.Comment,
                    Name = this.ColumnName,
                    CreationInfo = this.CreationInfo,
                    ModificationInfo = this.ModificationInfo,
                };
            }
        }

        public object Editor { get; set; }

        public static explicit operator CremaDataColumn(InternalDataColumn dataColumn)
        {
            if (dataColumn == null)
                return null;
            return dataColumn.Target;
        }

        public static explicit operator InternalDataColumn(CremaDataColumn dataColumn)
        {
            if (dataColumn == null)
                return null;
            return dataColumn.InternalObject;
        }

        protected virtual void OnValidateSetColumnID(Guid value)
        {

        }

        protected virtual void OnValidateSetIndex(int value)
        {
            if (this.Table == null)
                throw new ArgumentException("열은 테이블에 속해야 합니다.");
            if (value < 0 || value >= this.Table.Columns.Count)
                throw new ArgumentOutOfRangeException($"인덱스 '{value}'이(가) 최대 수를 초과합니다.");
        }

        protected virtual void OnValidateSetCremaType(InternalDataType value)
        {
            if (value != null)
            {
                if (value.DataSet == null)
                    throw new ArgumentException("DataSet에 포함되지 않은 타입은 사용할 수 없습니다.");

                var dataTable = this.Table;

                if (dataTable != null)
                {
                    if (dataTable.DataSet == null)
                        throw new ArgumentException("크레마 타입을 사용하려면 Table이 DataSet내에 포함되어야 합니다.");
                    if (dataTable.DataSet != value.DataSet)
                        throw new ArgumentException("같은 DataSet내에 있지 않은 타입은 사용할 수 없습니다.");

                    for (var i = 0; i < this.Table.Rows.Count; i++)
                    {
                        var item = this.Table.Rows[i];
                        var field = item[this];
                        if (field == DBNull.Value)
                            continue;

                        var textValue = field.ToString();
                        if (value.VerifyValue(textValue) == false)
                            throw new ArgumentException($"'{dataTable.Name}'테이블에 '{i}'행의 '{field}'은(는) '{value}'타입에 정의되어 있지 않습니다.");
                    }
                    if (this.Unique == true)
                    {
                        try
                        {
                            this.ValidateSetUnique(value);
                        }
                        catch (Exception e)
                        {
                            throw new ArgumentException($"'{dataTable.Name}'테이블에 '{value}'타입 변경시 고유값이 생기게 되어 타입을 변경할 수 없습니다.", e);
                        }
                    }
                }
            }
        }

        protected virtual void OnValidateSetDataTypeName(string value)
        {
            var dataSet = this.Table?.DataSet;
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value is string == false)
                throw new ArgumentException(nameof(value));

            if (dataSet == null && CremaDataTypeUtility.IsBaseType(value) == false)
            {
                if (NameValidator.VerifyItemPath(value) == false)
                {
                    throw new ArgumentException(string.Format(Resources.Exception_TypeCannotBeUsed_Format, value), nameof(value));
                }
            }
            if (CremaDataTypeUtility.IsBaseType(value) == true)
                this.OnValidateSetDataType(CremaDataTypeUtility.GetType(value));

            if (dataSet != null && CremaDataTypeUtility.IsBaseType(value) == false)
            {
                if (NameValidator.VerifyItemPath(value) == false)
                {
                    if (InternalDataSet.ContainsType(this.Table?.DataSet, value) == false)
                        throw new ArgumentException(string.Format(Resources.Exception_TypeCannotBeUsed_Format, value), nameof(value));
                    var dataType = InternalDataSet.GetType(this.Table?.DataSet, value);
                    this.OnValidateSetCremaType(dataType);
                }
                else
                {
                    var itemName = new ItemName(value);
                    if (InternalDataSet.ContainsType(this.Table?.DataSet, itemName.Name, itemName.CategoryPath) == false)
                        throw new ArgumentException(string.Format(Resources.Exception_TypeCannotBeUsed_Format, value), nameof(value));
                    var dataType = InternalDataSet.GetType(this.Table?.DataSet, itemName.Name, itemName.CategoryPath);
                    this.OnValidateSetCremaType(dataType);
                }
            }
        }

        protected virtual void OnValidateSetTags(TagInfo value)
        {

        }

        protected virtual void OnValidateSetComment(string value)
        {

        }

        protected virtual void OnValidateSetValidation(string value)
        {

        }

        protected override void OnValidateSetDefaultValue(object value)
        {
            base.OnValidateSetDefaultValue(value);

            var cremaType = this.CremaType;
            if (cremaType != null)
            {
                if (value != null && value != DBNull.Value)
                {
                    if (value is string)
                    {
                        cremaType.ValidateValue(value as string);
                    }
                    else if (CremaConvert.VerifyChangeType(value, typeof(long)) == false)
                    {
                        throw new ArgumentException($"'{value}'은(는) 잘못된 기본값입니다.");
                    }
                    else
                    {
                        var n = (long)CremaConvert.ChangeType(value, typeof(long));
                        if (cremaType.ConvertToString(n) == $"{n}")
                            throw new ArgumentException($"'{value}'은(는) 잘못된 기본값입니다.");
                    }
                }
            }
        }

        protected override void OnValidateSetAutoIncrement(bool value)
        {
            base.OnValidateSetAutoIncrement(value);

            var cremaType = this.Target.CremaType;
            if (cremaType != null && value == true)
                throw new ArgumentException("CremaDataType 이 설정되어 있는 열의 AutoIncrement 속성은 변경할 수 없습니다.");
        }

        protected override void OnValidateSetColumnName(string value)
        {
            base.OnValidateSetColumnName(value);

            var dataTable = this.Table;
            if (dataTable == null && string.IsNullOrEmpty(value) == false && CremaDataSet.VerifyName(value) == false)
                throw new ArgumentException(Resources.Exception_InvalidName);
            if (dataTable != null && value == null)
                throw new ArgumentNullException(nameof(value));
            if (dataTable != null && CremaDataSet.VerifyName(value) == false)
                throw new ArgumentException(Resources.Exception_InvalidName);
            if (dataTable != null && dataTable.ChildItems.Where(item => item.LocalName == value).Any() == true)
                throw new ArgumentException($"{value} 은(는) 자식 테이블의 이름으로 사용되고 있으므로 사용할 수 없습니다.");
        }

        protected override void OnSetNormalMode()
        {
            base.OnSetNormalMode();

            //this.ExtendedProperties.Remove(nameof(this.ColumnName));

            this.InternalAllowDBNull = (bool)this.ExtendedProperties[nameof(this.AllowDBNull)];
            //this.ExtendedProperties.Remove(nameof(this.AllowDBNull));

            this.InternalUnique = (bool)this.ExtendedProperties[nameof(this.Unique)];
            //this.ExtendedProperties.Remove(nameof(this.Unique));

            //this.InternalDataTypeName = (string)this.ExtendedProperties[nameof(this.DataTypeName)];
            //this.ExtendedProperties.Remove(nameof(this.DataTypeName));

            //this.InternalDefaultValue = this.ExtendedProperties[nameof(this.DefaultValue)];
            //this.ExtendedProperties.Remove(nameof(this.DefaultValue));

            //this.InternalAutoIncrement = (bool)this.ExtendedProperties[nameof(this.AutoIncrement)];
            //this.ExtendedProperties.Remove(nameof(this.AutoIncrement));

            this.InternalReadOnly = (bool)this.ExtendedProperties[nameof(this.ReadOnly)];
            //this.ExtendedProperties.Remove(nameof(this.ReadOnly));
        }

        protected override void OnSetDiffMode()
        {
            base.OnSetDiffMode();

            this.ExtendedProperties[nameof(this.ColumnName)] = this.ColumnName;

            this.ExtendedProperties[nameof(this.IsKey)] = this.IsKey;

            this.ExtendedProperties[nameof(this.AllowDBNull)] = this.AllowDBNull;
            this.InternalAllowDBNull = true;

            this.ExtendedProperties[nameof(this.Unique)] = this.Unique;
            this.InternalUnique = false;

            this.ExtendedProperties[nameof(this.DefaultValue)] = this.DefaultValue;
            this.InternalDefaultValue = DBNull.Value;

            this.ExtendedProperties[nameof(this.DataTypeName)] = this.DataTypeName;
            this.InternalDataTypeName = typeof(string).GetTypeName();

            this.ExtendedProperties[nameof(this.AutoIncrement)] = this.AutoIncrement;
            this.InternalAutoIncrement = false;

            this.ExtendedProperties[nameof(this.ReadOnly)] = this.ReadOnly;
            this.InternalReadOnly = false;
        }

        private void ValidateSetUnique(InternalDataType dataType)
        {
            var valueList = new List<long?>(this.Table.Rows.Count);

            for (var i = 0; i < this.Table.Rows.Count; i++)
            {
                var dataRow = this.Table.Rows[i];
                var curValue = dataRow.Field<string>(this);

                if (curValue != null)
                {
                    var value = dataType.ConvertFromString(curValue);
                    valueList.Add(value);
                }
                else
                {
                    valueList.Add(null);
                }
            }

            if (valueList.Distinct().Count() != this.Table.Rows.Count)
            {
                throw new InvalidConstraintException(string.Format("'{0}'테이블에 {1}'열에서 중복값이 존재하여 고유 설정을 할 수 없습니다.", this.Table.Name, this.ColumnName));
            }
        }

        private void ValidateConstructor(string columnName, InternalDataType dataType)
        {
            if (string.IsNullOrEmpty(columnName) == false)
                CremaDataColumn.ValidateColumnName(columnName);
            if (dataType == null)
                throw new ArgumentNullException(nameof(dataType));
            if (dataType.DataSet == null)
                throw new ArgumentException("타입이 DataSet에 포함되어 있지 않기 때문에 사용할 수 없습니다.", nameof(dataType));
        }

        private void ValidateConstructor(string columnName, Type dataType)
        {
            if (string.IsNullOrEmpty(columnName) == false)
                CremaDataColumn.ValidateColumnName(columnName);
            if (CremaDataTypeUtility.IsBaseType(dataType) == false)
                throw new ArgumentException(string.Format(Resources.Exception_TypeCannotBeUsed_Format, dataType.Name), nameof(dataType));
        }

        private new IEnumerable<InternalDataColumn> SiblingColumns
        {
            get
            {
                yield return this;
                foreach (var item in this.DerivedColumns)
                {
                    if (item is InternalDataColumn column)
                        yield return column;
                }
            }
        }
    }
}
