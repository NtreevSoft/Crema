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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntreev.Library;
using System.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Data.Properties;

namespace Ntreev.Crema.Data
{
    public class CremaTemplateColumn
    {
        private readonly CremaTemplate template;
        private readonly InternalTemplateColumn row;

        public CremaTemplateColumn(CremaTemplateColumnBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException();
            this.template = builder.Template;
            this.row = builder.DataRow;
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
            this.row.Delete();
        }

        public void SetDataType(Type dataType)
        {
            this.DataTypeName = dataType.GetTypeName();
        }

        public object GetAttribute(string attributeName)
        {
            if (this.template.Attributes.Contains(attributeName) == false)
                throw new CremaDataException(string.Format(Resources.Exception_NotFoundAttribute_Format, attributeName));
            return this.row[attributeName];
        }

        public void SetAttribute(string attributeName, object value)
        {
            if (this.template.Attributes.Contains(attributeName) == false)
                throw new CremaDataException(string.Format(Resources.Exception_NotFoundAttribute_Format, attributeName));
            this.row[attributeName] = value;
        }

        public int Index
        {
            get { return this.row.Index; }
            set { this.row.Index = value; }
        }

        public Guid ColumnID
        {
            get { return this.row.ColumnID; }
            set { this.row.ColumnID = value; }
        }

        public string Name
        {
            get { return this.row.ColumnName; }
            set { this.row.ColumnName = value; }
        }

        public bool IsKey
        {
            get { return this.row.IsKey; }
            set { this.row.IsKey = value; }
        }

        public bool Unique
        {
            get { return this.row.Unique; }
            set { this.row.Unique = value; }
        }

        public bool AutoIncrement
        {
            get { return this.row.AutoIncrement; }
            set { this.row.AutoIncrement = value; }
        }

        public string DataTypeName
        {
            get { return this.row.DataTypeName; }
            set { this.row.DataTypeName = value; }
        }

        public object DefaultValue
        {
            get { return this.row.DefaultValue; }
            set { this.row.DefaultValue = value; }
        }

        public string Comment
        {
            get { return this.row.Comment ?? string.Empty; }
            set { this.row.Comment = value; }
        }

        [Obsolete]
        public DataRowState ColumnState
        {
            get { return this.row.RowState; }
        }

        public DataRowState ItemState
        {
            get { return this.row.RowState; }
        }

        public TagInfo Tags
        {
            get { return this.row.Tags; }
            set { this.row.Tags = value; }
        }

        public bool ReadOnly
        {
            get { return this.row.ReadOnly; }
            set { this.row.ReadOnly = value; }
        }

        public bool AllowNull
        {
            get { return this.row.AllowNull; }
            set { this.row.AllowNull = value; }
        }

        public bool IsDeleted
        {
            get { return this.row.RowState == DataRowState.Deleted || this.row.RowState == DataRowState.Detached; }
        }

        public bool CanDelete
        {
            get
            {
                if (this.IsDeleted == true)
                    return false;
                return this.row.CanDelete;
            }
        }

        public DataRowState State
        {
            get { return this.row.RowState; }
        }

        public CremaTemplate Template
        {
            get { return this.template; }
        }

        public SignatureDate CreationInfo
        {
            get { return this.row.CreationInfo; }
            internal set { this.row.CreationInfo = value; }
        }

        public SignatureDate ModificationInfo
        {
            get { return this.row.ModificationInfo; }
            internal set { this.row.ModificationInfo = value; }
        }
        
        internal InternalTemplateColumn InternalObject
        {
            get { return this.row; }
        }

        internal CremaDataColumn TargetColumn
        {
            get { return (CremaDataColumn)this.row.TargetColumn; }
            set { this.row.TargetColumn = (InternalDataColumn)value; }
        }
    }
}
