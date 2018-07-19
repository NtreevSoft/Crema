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
using System;
using System.ComponentModel;
using System.Data;

namespace Ntreev.Crema.Data
{
    public class CremaDataColumn : INotifyPropertyChanged
    {
        public CremaDataColumn()
            : this(string.Empty, typeof(string))
        {

        }

        public CremaDataColumn(string columnName)
            : this(columnName, typeof(string))
        {

        }

        public CremaDataColumn(string columnName, Type dataType)
        {
            this.InternalObject = new InternalDataColumn(this, columnName, dataType);
        }

        public CremaDataColumn(string columnName, CremaDataType dataType)
        {
            this.InternalObject = new InternalDataColumn(this, columnName, (InternalDataType)dataType);
        }

        internal CremaDataColumn(InternalDataColumn dataColumn)
        {
            this.InternalObject = dataColumn;
        }

        public override string ToString()
        {
            return this.InternalObject.ToString();
        }

        public bool VerifyUnique()
        {
            return this.InternalObject.VerifyUnique();
        }

        public bool VerifyNotAllowNull()
        {
            return this.InternalObject.VerifyNotAllowNull();
        }

        public object ConvertFromString(string textValue)
        {
            return this.InternalObject.ConvertFromString(textValue);
        }

        public static void ValidateColumnName(string columnName)
        {
            if (VerifyColumnName(columnName) == false)
            {
                throw new ArgumentException(string.Format(Resources.Exception_NameCannotBeUsed_Format, columnName));
            }
        }

        public static bool VerifyColumnName(string columnName)
        {
            if (CremaDataSet.VerifyName(columnName) == false)
                return false;

            foreach (var item in CremaSchema.ReservedNames)
            {
                if (item == columnName)
                    return false;
            }

            return true;
        }

        public bool AllowDBNull
        {
            get => this.InternalObject.AllowDBNull;
            set
            {
                this.InternalObject.AllowDBNull = value;
                this.InvokePropertyChangedEvent(nameof(this.AllowDBNull));
            }
        }

        public bool AutoIncrement
        {
            get => this.InternalObject.AutoIncrement;
            set
            {
                this.InternalObject.AutoIncrement = value;
                this.InvokePropertyChangedEvent(nameof(this.AutoIncrement));
            }
        }

        public string ColumnName
        {
            get => this.InternalObject.ColumnName;
            set
            {
                this.InternalObject.ColumnName = value;
                this.InvokePropertyChangedEvent(nameof(this.ColumnName));
            }
        }

        public Type DataType
        {
            get => this.InternalObject.DataType;
            set
            {
                this.InternalObject.DataType = value;
                this.InvokePropertyChangedEvent(nameof(this.CremaType));
                this.InvokePropertyChangedEvent(nameof(this.DataType));
                this.InvokePropertyChangedEvent(nameof(this.DataTypeName));
            }
        }

        public CremaDataType CremaType
        {
            get => (CremaDataType)this.InternalObject.CremaType;
            set
            {
                this.InternalObject.CremaType = (InternalDataType)value;
                this.InvokePropertyChangedEvent(nameof(this.CremaType));
                this.InvokePropertyChangedEvent(nameof(this.DataType));
                this.InvokePropertyChangedEvent(nameof(this.DataTypeName));
            }
        }

        public object DefaultValue
        {
            get => this.InternalObject.DefaultValue;
            set
            {
                this.InternalObject.DefaultValue = value;
                this.InvokePropertyChangedEvent(nameof(this.DefaultValue));
            }
        }

        public string Expression
        {
            get => this.InternalObject.Expression;
            set
            {
                this.InternalObject.Expression = value;
                this.InvokePropertyChangedEvent(nameof(this.Expression));
            }
        }

        public string Validation
        {
            get => this.InternalObject.Validation;
            set
            {
                this.InternalObject.Validation = value;
                this.InvokePropertyChangedEvent(nameof(this.Validation));
            }
        }

        public PropertyCollection ExtendedProperties => this.InternalObject.ExtendedProperties;

        public bool Unique
        {
            get => this.InternalObject.Unique;
            set
            {
                this.InternalObject.Unique = value;
                this.InvokePropertyChangedEvent(nameof(this.Unique));
            }
        }

        public bool ReadOnly
        {
            get => this.InternalObject.ReadOnly;
            set
            {
                this.InternalObject.ReadOnly = value;
                this.InvokePropertyChangedEvent(nameof(this.ReadOnly));
            }
        }

        public string Comment
        {
            get => this.InternalObject.Comment;
            set
            {
                this.InternalObject.Comment = value;
                this.InvokePropertyChangedEvent(nameof(this.Comment));
            }
        }

        public bool IsKey
        {
            get => this.InternalObject.IsKey;
            set
            {
                this.InternalObject.IsKey = value;
                this.InvokePropertyChangedEvent(nameof(this.IsKey));
            }
        }

        public CremaDataTable Table
        {
            get
            {
                if (this.InternalObject.Table == null)
                    return null;
                return (this.InternalObject.Table as InternalDataTable).Target;
            }
        }

        public SignatureDate CreationInfo
        {
            get => this.InternalObject.CreationInfo;
            internal set => this.InternalObject.CreationInfo = value;
        }

        public SignatureDate ModificationInfo
        {
            get => this.InternalObject.ModificationInfo;
            internal set
            {
                if (this.Table != null)
                {
                    this.InternalObject.ModificationInfo = value;
                    this.Table.InternalModificationInfo = value;
                }
            }
        }

        public Guid ColumnID
        {
            get => this.InternalObject.ColumnID;
            internal set => this.InternalObject.ColumnID = value;
        }

        public TagInfo Tags
        {
            get => this.InternalObject.Tags;
            set
            {
                this.InternalObject.Tags = value;
                this.InvokePropertyChangedEvent(nameof(this.Tags));
            }
        }

        public TagInfo DerivedTags
        {
            get
            {
                var tags = this.InternalObject.Tags;
                var table = this.Table;
                if (table != null)
                {
                    if (this.IsKey == true)
                        tags = table.DerivedTags;
                    else
                        tags = tags & table.DerivedTags;
                }
                return tags;
            }
        }

        public string DefaultValueString => this.InternalObject.DefaultString;

        public string Namespace
        {
            get
            {
                if (this.Table != null && this.Table.Namespace != this.InternalObject.Namespace)
                    throw new CremaDataException();
                return this.InternalObject.Namespace;
            }
        }

        public ColumnInfo ColumnInfo => this.InternalObject.ColumnInfo;

        public int Index
        {
            get => this.InternalObject.Index;
            set
            {
                this.InternalObject.Index = value;
                this.InvokePropertyChangedEvent(nameof(this.Index));
            }
        }

        public string DataTypeName
        {
            get => this.InternalObject.DataTypeName;
            set
            {
                this.InternalObject.DataTypeName = value;
                this.InvokePropertyChangedEvent(nameof(this.DataTypeName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        private void InvokePropertyChangedEvent(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        internal void CopyTo(CremaDataColumn dest)
        {
            dest.InternalAllowDBNull = this.AllowDBNull;
            dest.InternalAutoIncrement = this.AutoIncrement;
            dest.InternalColumnName = this.ColumnName;
            dest.InternalDataType = this.DataType;
            dest.InternalCremaType = (InternalDataType)this.CremaType;
            dest.InternalDefaultValue = this.DefaultValue;
            dest.InternalExpression = this.Expression;
            dest.InternalValidation = this.Validation;
            dest.InternalUnique = this.Unique;
            dest.InternalReadOnly = this.ReadOnly;
            dest.InternalComment = this.Comment;
            dest.InternalIsKey = this.IsKey;
            dest.InternalCreationInfo = this.CreationInfo;
            dest.InternalModificationInfo = this.ModificationInfo;
            dest.InternalTags = this.Tags;
            dest.InternalColumnID = this.ColumnID;
        }

        internal Guid InternalColumnID
        {
            get => this.InternalObject.InternalColumnID;
            set => this.InternalObject.InternalColumnID = value;
        }

        internal bool InternalAllowDBNull
        {
            get => this.InternalObject.InternalAllowDBNull;
            set => this.InternalObject.InternalAllowDBNull = value;
        }

        internal bool InternalAutoIncrement
        {
            get => this.InternalObject.InternalAutoIncrement;
            set => this.InternalObject.InternalAutoIncrement = value;
        }

        internal string InternalColumnName
        {
            get => this.InternalObject.InternalColumnName;
            set => this.InternalObject.InternalColumnName = value;
        }

        internal Type InternalDataType
        {
            get => this.InternalObject.InternalDataType;
            set => this.InternalObject.InternalDataType = value;
        }

        internal InternalDataType InternalCremaType
        {
            get => this.InternalObject.InternalCremaType;
            set => this.InternalObject.InternalCremaType = value;
        }

        internal bool InternalIsKey
        {
            get => this.InternalObject.InternalIsKey;
            set => this.InternalObject.InternalIsKey = value;
        }

        internal bool InternalUnique
        {
            get => this.InternalObject.InternalUnique;
            set => this.InternalObject.InternalUnique = value;
        }

        internal string InternalExpression
        {
            get => this.InternalObject.InternalExpression;
            set => this.InternalObject.InternalExpression = value;
        }

        internal object InternalDefaultValue
        {
            get => this.InternalObject.InternalDefaultValue;
            set => this.InternalObject.InternalDefaultValue = value;
        }

        internal string InternalComment
        {
            get => this.InternalObject.InternalComment;
            set => this.InternalObject.InternalComment = value;
        }

        internal string InternalValidation
        {
            get => this.InternalObject.InternalValidation;
            set => this.InternalObject.InternalValidation = value;
        }

        internal bool InternalReadOnly
        {
            get => this.InternalObject.InternalReadOnly;
            set => this.InternalObject.InternalReadOnly = value;
        }

        internal int InternalIndex
        {
            get => this.InternalObject.InternalIndex;
            set => this.InternalObject.InternalIndex = value;
        }

        internal TagInfo InternalTags
        {
            get => this.InternalObject.InternalTags;
            set => this.InternalObject.InternalTags = value;
        }

        internal string InternalDataTypeName
        {
            get => this.InternalObject.InternalDataTypeName;
            set => this.InternalObject.InternalDataTypeName = value;
        }

        internal SignatureDate InternalCreationInfo
        {
            get => this.InternalObject.InternalCreationInfo;
            set => this.InternalObject.InternalCreationInfo = value;
        }

        internal SignatureDate InternalModificationInfo
        {
            get => this.InternalObject.InternalModificationInfo;
            set => this.InternalObject.InternalModificationInfo = value;
        }

        internal InternalDataColumn InternalObject { get; }
    }
}
