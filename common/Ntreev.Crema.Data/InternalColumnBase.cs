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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Data
{
    abstract class InternalColumnBase : DataColumn, INotifyPropertyChanged
    {
        private bool isKey;

        public InternalColumnBase(string columnName)
            : base(columnName)
        {

        }

        public InternalColumnBase(string columnName, Type dataType)
                : base(columnName, dataType)
        {

        }

        protected InternalColumnBase()
        {

        }

        public void ValidateSetIsKey(bool value)
        {
            this.ValidateSetProperty(nameof(this.IsKey));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                item.OnValidateSetIsKey(value);
            }
        }

        public void ValidateSetUnique(bool value)
        {
            this.ValidateSetProperty(nameof(this.Unique));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                item.OnValidateSetUnique(value);
            }
        }

        public void ValidateSetColumnName(string value)
        {
            this.ValidateSetProperty(nameof(this.ColumnName));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                this.OnValidateSetColumnName(value);
            }
        }

        public void ValidateSetDataType(Type value)
        {
            this.ValidateSetProperty(nameof(this.DataType));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                this.OnValidateSetDataType(value);
            }
        }

        public void ValidateSetReadOnly(bool value)
        {
            this.ValidateSetProperty(nameof(this.ReadOnly));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                this.OnValidateSetReadOnly(value);
            }
        }

        public void ValidateSetExpression(string value)
        {
            this.ValidateSetProperty(nameof(this.Expression));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                this.OnValidateSetExpression(value);
            }
        }

        public void ValidateSetDefaultValue(object value)
        {
            this.ValidateSetProperty(nameof(this.DefaultValue));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                this.OnValidateSetDefaultValue(value);
            }
        }

        public void ValidateSetAutoIncrement(bool value)
        {
            this.ValidateSetProperty(nameof(this.AutoIncrement));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                this.OnValidateSetAutoIncrement(value);
            }
        }

        public void ValidateSetAllowDBNull(bool value)
        {
            this.ValidateSetProperty(nameof(this.AllowDBNull));
            foreach (var item in this.SiblingColumns.ToArray())
            {
                this.OnValidateSetAllowDBNull(value);
            }
        }

        public object Target { get; protected set; }

        public bool IsDiffMode
        {
            get
            {
                if (this.Table is InternalTableBase table)
                    return table.IsDiffMode;
                return false;
            }
        }

        public bool IsKey
        {
            get => this.InternalIsKey;
            set
            {
                if (this.InternalIsKey == value)
                    return;
                this.ValidateSetIsKey(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalIsKey = value;
                }
            }
        }

        public new bool ReadOnly
        {
            get => this.InternalReadOnly;
            set
            {
                if (this.InternalReadOnly == value)
                    return;
                this.ValidateSetReadOnly(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalReadOnly = value;
                }
            }
        }

        public new string Expression
        {
            get => this.InternalExpression;
            set
            {
                if (this.InternalExpression == value)
                    return;
                this.ValidateSetExpression(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalExpression = value;
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
                if (value != null && value != DBNull.Value)
                    value = CremaConvert.ChangeType(value, this.DataType);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalDefaultValue = value;
                }
            }
        }

        public new Type DataType
        {
            get => this.InternalDataType;
            set
            {
                if (this.InternalDataType == value)
                    return;
                this.ValidateSetDataType(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalDataType = value;
                }
            }
        }

        public new bool Unique
        {
            get => this.InternalUnique;
            set
            {
                if (this.InternalUnique == value)
                    return;
                this.ValidateSetUnique(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalUnique = value;
                }
            }
        }

        public new bool AutoIncrement
        {
            get => this.InternalAutoIncrement;
            set
            {
                if (this.InternalAutoIncrement == value)
                    return;
                this.ValidateSetAutoIncrement(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalAutoIncrement = value;
                }
            }
        }

        public new bool AllowDBNull
        {
            get => this.InternalAllowDBNull;
            set
            {
                if (this.InternalAllowDBNull == value)
                    return;
                this.ValidateSetAllowDBNull(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalAllowDBNull = value;
                }
            }
        }

        public new string ColumnName
        {
            get => this.InternalColumnName;
            set
            {
                if (this.InternalColumnName == value)
                    return;
                this.ValidateSetColumnName(value);
                foreach (var item in this.SiblingColumns.ToArray())
                {
                    item.InternalColumnName = value;
                }
            }
        }

        public new InternalTableBase Table => base.Table as InternalTableBase;

        public IEnumerable<InternalColumnBase> DerivedColumns
        {
            get
            {
                if (this.Table is InternalTableBase table)
                {
                    foreach (var item in table.DerivedItems)
                    {
                        var column = item.Columns[this.ColumnName];
                        if (column is InternalColumnBase == false || column.ColumnName != this.ColumnName)
                            throw new InvalidOperationException();
                        yield return column as InternalColumnBase;
                    }
                }
            }
        }

        public IEnumerable<InternalColumnBase> SiblingColumns
        {
            get
            {
                yield return this;
                foreach (var item in this.DerivedColumns)
                {
                    yield return item;
                }
            }
        }

        public bool InternalIsKey
        {
            get => this.isKey;
            set
            {
                if (this.Table != null)
                {
                    if (value == true)
                        this.Table.AddKey(this);
                    else
                        this.Table.RemoveKey(this);
                }
                this.isKey = value;
                this.InvokePropertyChangedEvent(nameof(this.IsKey));
            }
        }

        public bool InternalReadOnly
        {
            get => base.ReadOnly;
            set
            {
                base.ReadOnly = value;
                this.InvokePropertyChangedEvent(nameof(this.ReadOnly));
            }
        }

        public string InternalExpression
        {
            get => base.Expression;
            set
            {
                base.Expression = value;
                this.InvokePropertyChangedEvent(nameof(this.Expression));
            }
        }

        public object InternalDefaultValue
        {
            get => base.DefaultValue;
            set
            {
                if (value != null && value != DBNull.Value && value.GetType() != base.DataType)
                    throw new ArgumentException(nameof(value));
                base.DefaultValue = value;
                this.InvokePropertyChangedEvent(nameof(this.DefaultValue));
            }
        }

        public Type InternalDataType
        {
            get => base.DataType;
            set
            {
                var prop = typeof(DataColumn).GetProperty("HasData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var hasData = (bool)prop.GetValue(this);

                if (this.Table != null && hasData == false)
                {
                    var defaultValue = base.DefaultValue;
                    base.DefaultValue = DBNull.Value;
                    base.DataType = value;
                    if (defaultValue != DBNull.Value)
                        base.DefaultValue = CremaConvert.ChangeType(defaultValue, base.DataType);
                }
                else if (this.Table is InternalTableBase table)
                {
                    var oldType = base.DataType;
                    var fieldList = new List<object>(table.Rows.Count);
                    foreach (DataRow item in table.Rows)
                    {
                        fieldList.Add(item[this.ColumnName]);
                    }

                    var oldName = this.ColumnName;
                    var defaultValue = this.DefaultValue;
                    var ordinal = this.Ordinal;
                    var keys = table.PrimaryKey;
                    var unique = this.Unique;
                    var isReadonly = this.ReadOnly;
                    var isKey = this.isKey;

                    this.IsInternalAction = true;
                    table.PrimaryKey = null;
                    base.Unique = false;
                    base.ReadOnly = false;

                    table.Columns.Remove(this);
                    base.DefaultValue = DBNull.Value;
                    base.DataType = value;

                    table.BeginLoadData();
                    table.Columns.Add(this);
                    table.PrimaryKey = keys;
                    base.SetOrdinal(ordinal);
                    for (var i = 0; i < fieldList.Count; i++)
                    {
                        if (fieldList[i] == DBNull.Value)
                            table.Rows[i][this.ColumnName] = fieldList[i];
                        else
                            table.Rows[i][this.ColumnName] = CremaConvert.ChangeType(fieldList[i], value);
                    }
                    base.Unique = unique;
                    base.ReadOnly = isReadonly;
                    table.EndLoadData();
                    if (defaultValue != DBNull.Value)
                        base.DefaultValue = CremaConvert.ChangeType(defaultValue, base.DataType);
                    this.IsInternalAction = false;
                }
                else if (this.Table == null)
                {
                    var defaultValue = base.DefaultValue;
                    base.DefaultValue = DBNull.Value;
                    base.DataType = value;
                    if (defaultValue != DBNull.Value)
                        base.DefaultValue = CremaConvert.ChangeType(defaultValue, base.DataType);
                }
                else
                {
                    throw new NotImplementedException();
                }
                this.InvokePropertyChangedEvent(nameof(this.DataType));
            }
        }

        public bool InternalUnique
        {
            get => base.Unique;
            set
            {
                base.Unique = value;
                this.InvokePropertyChangedEvent(nameof(this.Unique));
            }
        }

        public bool InternalAutoIncrement
        {
            get => base.AutoIncrement;
            set
            {
                base.AutoIncrement = value;
                this.InvokePropertyChangedEvent(nameof(this.AutoIncrement));
            }
        }

        public bool InternalAllowDBNull
        {
            get => base.AllowDBNull;
            set
            {
                base.AllowDBNull = value;
                this.InvokePropertyChangedEvent(nameof(this.AllowDBNull));
            }
        }

        public string InternalColumnName
        {
            get => base.ColumnName;
            set
            {
                base.ColumnName = value;
                this.InvokePropertyChangedEvent(nameof(this.ColumnName));
            }
        }

        public bool IsInternalAction { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChangedEvent(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnValidateSetColumnName(string value)
        {
            if (this.Table != null && value == null)
                throw new ArgumentNullException(nameof(value));
            if (this.Table != null || string.IsNullOrEmpty(value) == false)
                CremaDataColumn.ValidateColumnName(value);
            if (this.Table != null && this.ColumnName != value)
            {
                if (this.Table.Columns[value] != null && this.Table.Columns[value].ColumnName == value)
                    throw new ArgumentException(nameof(value));
            }
        }

        protected virtual void OnValidateSetIsKey(bool value)
        {
            if (this.Table != null && value == true)
                this.CheckNotAllowNull();
        }

        protected virtual void OnValidateSetUnique(bool value)
        {
            if (this.Table != null && value == true)
                this.CheckUnique();
        }

        protected virtual void OnValidateSetDataType(Type value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (CremaDataTypeUtility.IsBaseType(value) == false)
                throw new ArgumentException(string.Format(Resources.Exception_TypeCannotBeUsed_Format, value.Name), nameof(value));
            if (this.Table != null && this.Table.Rows.Count > 0 && this.AutoIncrement == true && CremaDataTypeUtility.CanUseAutoIncrement(value) == false)
                throw new ArgumentException($"AutoIncrement 를 사용중인 열에서 '{value.GetTypeName()}' 타입은 사용할 수 없습니다.");

            var index = 0;
            if (this.Table != null)
            {
                foreach (DataRow item in this.Table.Rows)
                {
                    var field = item[this];
                    if (field == DBNull.Value)
                        continue;
                    if (CremaConvert.VerifyChangeType(field, value) == false)
                        throw new FormatException($"{index}번째 값 \"{field}\"은(는) {value.GetTypeName()} 타입으로 변환할 수 없습니다.");
                    index++;
                }
            }

            if (this.DefaultValue != DBNull.Value && CremaConvert.VerifyChangeType(this.DefaultValue, value) == false)
                throw new FormatException($"기본값 \"{this.DefaultValue}\"은(는) {value.GetTypeName()} 타입으로 변환할 수 없습니다.");
        }

        protected virtual void OnValidateSetReadOnly(bool value)
        {

        }

        protected virtual void OnValidateSetExpression(string value)
        {

        }

        protected virtual void OnValidateSetDefaultValue(object value)
        {
            if (value != null && value != DBNull.Value)
            {
                if (this.AutoIncrement == true)
                    throw new ArgumentException($"AutoIncrement 열에 DefaultValue를 설정할 수 없습니다.");
                if (CremaConvert.VerifyChangeType(value, this.DataType) == false)
                    throw new ArgumentException($"기본값 \"{value}\"은(는) {this.DataType.GetTypeName()} 타입으로 변환할 수 없습니다.");

            }
        }

        protected virtual void OnValidateSetAutoIncrement(bool value)
        {
            if (value == true && this.Table != null && this.Table.Rows.Count > 0 && CremaDataTypeUtility.CanUseAutoIncrement(this.DataType) == false)
                throw new ArgumentException($"{this.DataType.GetTypeName()} 은 {nameof(AutoIncrement)}가 지원되지 않는 타입입니다.");
        }

        protected virtual void OnValidateSetAllowDBNull(bool value)
        {
            if (this.IsDiffMode == false && this.IsKey && value == true)
                throw new Exception();
            if (this.Table != null && value == false)
                this.CheckNotAllowNull();
        }

        protected virtual void OnSetNormalMode()
        {

        }

        protected virtual void OnSetDiffMode()
        {

        }

        protected void ValidateSetProperty(string propertyName)
        {
            if (this.Table != null && this.Table.TemplateNamespace != string.Empty)
                throw new ArgumentException($"상속받은 테이블의 열의 속성 {propertyName}(는) 변경할 수 없습니다.");
        }

        internal void SetNormalMode()
        {
            this.OnSetNormalMode();
        }

        internal void SetDiffMode()
        {
            this.OnSetDiffMode();
        }
    }
}
