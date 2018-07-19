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
using System;
using System.Data;

namespace Ntreev.Crema.Data
{
    public class CremaTemplateColumn
    {
        public CremaTemplateColumn(CremaTemplateColumnBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException();
            this.Template = builder.Template;
            this.InternalObject = builder.DataRow;
        }

        public override string ToString()
        {
            return string.Format("{0}", this.Name);
        }

        public bool VerifyUnique()
        {
            if (this.TargetColumn == null)
                throw new InvalidOperationException();
            return this.TargetColumn.VerifyUnique();
        }

        public bool VerifyNotAllowNull()
        {
            if (this.TargetColumn == null)
                throw new InvalidOperationException();
            return this.TargetColumn.VerifyNotAllowNull();
        }

        public object ConvertFromString(string textValue)
        {
            if (this.TargetColumn == null)
                throw new InvalidOperationException();
            return this.TargetColumn.ConvertFromString(textValue);
        }

        public void Delete()
        {
            this.InternalObject.Delete();
        }

        public void SetDataType(Type dataType)
        {
            this.DataTypeName = dataType.GetTypeName();
        }

        public object GetAttribute(string attributeName)
        {
            if (this.Template.Attributes.Contains(attributeName) == false)
                throw new CremaDataException(string.Format(Resources.Exception_NotFoundAttribute_Format, attributeName));
            return this.InternalObject[attributeName];
        }

        public void SetAttribute(string attributeName, object value)
        {
            if (this.Template.Attributes.Contains(attributeName) == false)
                throw new CremaDataException(string.Format(Resources.Exception_NotFoundAttribute_Format, attributeName));
            this.InternalObject[attributeName] = value;
        }

        public int Index
        {
            get => this.InternalObject.Index;
            set => this.InternalObject.Index = value;
        }

        public Guid ColumnID
        {
            get => this.InternalObject.ColumnID;
            set => this.InternalObject.ColumnID = value;
        }

        public string Name
        {
            get => this.InternalObject.ColumnName;
            set => this.InternalObject.ColumnName = value;
        }

        public bool IsKey
        {
            get => this.InternalObject.IsKey;
            set => this.InternalObject.IsKey = value;
        }

        public bool Unique
        {
            get => this.InternalObject.Unique;
            set => this.InternalObject.Unique = value;
        }

        public bool AutoIncrement
        {
            get => this.InternalObject.AutoIncrement;
            set => this.InternalObject.AutoIncrement = value;
        }

        public string DataTypeName
        {
            get => this.InternalObject.DataTypeName;
            set => this.InternalObject.DataTypeName = value;
        }

        public object DefaultValue
        {
            get => this.InternalObject.DefaultValue;
            set => this.InternalObject.DefaultValue = value;
        }

        public string Comment
        {
            get => this.InternalObject.Comment ?? string.Empty;
            set => this.InternalObject.Comment = value;
        }

        [Obsolete]
        public DataRowState ColumnState => this.InternalObject.RowState;

        public DataRowState ItemState => this.InternalObject.RowState;

        public TagInfo Tags
        {
            get => this.InternalObject.Tags;
            set => this.InternalObject.Tags = value;
        }

        public bool ReadOnly
        {
            get => this.InternalObject.ReadOnly;
            set => this.InternalObject.ReadOnly = value;
        }

        public bool AllowNull
        {
            get => this.InternalObject.AllowNull;
            set => this.InternalObject.AllowNull = value;
        }

        public bool IsDeleted => this.InternalObject.RowState == DataRowState.Deleted || this.InternalObject.RowState == DataRowState.Detached;

        public bool CanDelete
        {
            get
            {
                if (this.IsDeleted == true)
                    return false;
                return this.InternalObject.CanDelete;
            }
        }

        public DataRowState State => this.InternalObject.RowState;

        public CremaTemplate Template { get; }

        public SignatureDate CreationInfo
        {
            get => this.InternalObject.CreationInfo;
            internal set => this.InternalObject.CreationInfo = value;
        }

        public SignatureDate ModificationInfo
        {
            get => this.InternalObject.ModificationInfo;
            internal set => this.InternalObject.ModificationInfo = value;
        }

        internal InternalTemplateColumn InternalObject { get; }

        internal CremaDataColumn TargetColumn
        {
            get => (CremaDataColumn)this.InternalObject.TargetColumn;
            set => this.InternalObject.TargetColumn = (InternalDataColumn)value;
        }
    }
}
