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
using System.Collections.Generic;
using System.Data;

namespace Ntreev.Crema.Data
{
    public class CremaDataTypeMember
    {
        public CremaDataTypeMember(CremaDataTypeMemberBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException();
            this.Type = builder.Type;
            this.InternalObject = builder.DataRow;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", this.Name ?? "(null)", this.Value);
        }

        public IDictionary<string, object> ToDictionary()
        {
            var props = new Dictionary<string, object>
            {
                { nameof(this.MemberID), this.MemberID },
                { nameof(this.Name), this.Name },
                { nameof(this.Value), this.Value },
                { nameof(this.Comment), this.Comment },
                { nameof(this.Tags), $"{this.Tags}" },
                { nameof(this.DerivedTags), $"{this.DerivedTags}" },
                { nameof(this.IsEnabled), this.IsEnabled },
                { CremaSchema.Creator, this.CreationInfo.ID },
                { CremaSchema.CreatedDateTime, this.CreationInfo.DateTime },
                { CremaSchema.Modifier, this.ModificationInfo.ID },
                { CremaSchema.ModifiedDateTime, this.ModificationInfo.DateTime }
            };
            return props;
        }

        public void AcceptChanges()
        {
            this.InternalObject.AcceptChanges();
        }

        public void BeginEdit()
        {
            this.InternalObject.BeginEdit();
        }

        public void EndEdit()
        {
            this.InternalObject.EndEdit();
        }

        public void CancelEdit()
        {
            this.InternalObject.CancelEdit();
        }

        public void Delete()
        {
            this.InternalObject.Delete();
        }

        public void RejectChanges()
        {
            this.InternalObject.RejectChanges();
        }

        public void SetAdded()
        {
            this.InternalObject.SetAdded();
        }

        public void SetModified()
        {
            this.InternalObject.SetModified();
        }

        public int Index
        {
            get => this.InternalObject.Index;
            set => this.InternalObject.Index = value;
        }

        public TagInfo Tags
        {
            get => this.InternalObject.Tags;
            set => this.InternalObject.Tags = value;
        }

        public TagInfo DerivedTags => this.InternalObject.DerivedTags;

        public bool IsEnabled
        {
            get => this.InternalObject.IsEnabled;
            set => this.InternalObject.IsEnabled = value;
        }

        public string Name
        {
            get => this.InternalObject.Name;
            set => this.InternalObject.Name = value;
        }

        public long Value
        {
            get => this.InternalObject.Value;
            set => this.InternalObject.Value = value;
        }

        public string Comment
        {
            get => this.InternalObject.Comment;
            set => this.InternalObject.Comment = value;
        }

        [Obsolete]
        public DataRowState MemberState => this.InternalObject.RowState;

        public DataRowState ItemState => this.InternalObject.RowState;

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

        public Guid MemberID
        {
            get => this.InternalObject.ID;
            internal set => this.InternalObject.ID = value;
        }

        public TypeMemberInfo TypeMemberInfo => this.InternalObject.TypeMemberInfo;

        public CremaDataType Type { get; }

        [Obsolete]
        public bool IsValid => this.InternalObject.RowState != DataRowState.Deleted && this.InternalObject.RowState != DataRowState.Detached;

        public object GetAttribute(string attributeName)
        {
            if (this.Type.Attributes.Contains(attributeName) == false)
                throw new CremaDataException(string.Format(Resources.Exception_NotFoundAttribute_Format, attributeName));
            return this.InternalObject[attributeName];
        }

        public void SetAttribute(string attributeName, object value)
        {
            if (this.Type.Attributes.Contains(attributeName) == false)
                throw new CremaDataException(string.Format(Resources.Exception_NotFoundAttribute_Format, attributeName));
            this.InternalObject[attributeName] = value;
        }

        public void SetFieldError(string fieldName, string error)
        {
            this.InternalObject.SetColumnError(fieldName, error);
        }

        internal InternalDataTypeMember InternalObject { get; }
    }
}
