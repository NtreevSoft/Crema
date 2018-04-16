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
    public class CremaDataTypeMember
    {
        private readonly CremaDataType type;
        private readonly InternalDataTypeMember member;

        public CremaDataTypeMember(CremaDataTypeMemberBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException();
            this.type = builder.Type;
            this.member = builder.DataRow;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", this.Name ?? "(null)", this.Value);
        }

        public void AcceptChanges()
        {
            this.member.AcceptChanges();
        }

        public void BeginEdit()
        {
            this.member.BeginEdit();
        }

        public void EndEdit()
        {
            this.member.EndEdit();
        }

        public void CancelEdit()
        {
            this.member.CancelEdit();
        }

        public void Delete()
        {
            this.member.Delete();
        }

        public void RejectChanges()
        {
            this.member.RejectChanges();
        }

        public void SetAdded()
        {
            this.member.SetAdded();
        }

        public void SetModified()
        {
            this.member.SetModified();
        }

        public int Index
        {
            get { return this.member.Index; }
            set { this.member.Index = value; }
        }

        public TagInfo Tags
        {
            get { return this.member.Tags; }
            set { this.member.Tags = value; }
        }

        public TagInfo DerivedTags
        {
            get { return this.member.DerivedTags; }
        }

        public bool IsEnabled
        {
            get { return this.member.IsEnabled; }
            set { this.member.IsEnabled = value; }
        }

        public string Name
        {
            get { return this.member.Name; }
            set { this.member.Name = value; }
        }

        public long Value
        {
            get { return this.member.Value; }
            set { this.member.Value = value; }
        }

        public string Comment
        {
            get { return this.member.Comment; }
            set { this.member.Comment = value; }
        }

        [Obsolete]
        public DataRowState MemberState
        {
            get { return this.member.RowState; }
        }

        public DataRowState ItemState
        {
            get { return this.member.RowState; }
        }

        public SignatureDate CreationInfo
        {
            get { return this.member.CreationInfo; }
            internal set { this.member.CreationInfo = value; }
        }

        public SignatureDate ModificationInfo
        {
            get { return this.member.ModificationInfo; }
            internal set { this.member.ModificationInfo = value; }
        }

        public Guid MemberID
        {
            get { return this.member.ID; }
            internal set { this.member.ID = value; }
        }

        public TypeMemberInfo TypeMemberInfo
        {
            get { return this.member.TypeMemberInfo; }
        }

        public CremaDataType Type
        {
            get { return this.type; }
        }

        [Obsolete]
        public bool IsValid
        {
            get
            {
                return this.member.RowState != DataRowState.Deleted && this.member.RowState != DataRowState.Detached;
            }
        }

        public object GetAttribute(string attributeName)
        {
            if (this.Type.Attributes.Contains(attributeName) == false)
                throw new CremaDataException(string.Format(Resources.Exception_NotFoundAttribute_Format, attributeName));
            return this.member[attributeName];
        }

        public void SetAttribute(string attributeName, object value)
        {
            if (this.Type.Attributes.Contains(attributeName) == false)
                throw new CremaDataException(string.Format(Resources.Exception_NotFoundAttribute_Format, attributeName));
            this.member[attributeName] = value;
        }

        public void SetFieldError(string fieldName, string error)
        {
            this.member.SetColumnError(fieldName, error);
        }

        internal InternalDataTypeMember InternalObject
        {
            get { return this.member; }
        }
    }
}
