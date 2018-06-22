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

using Ntreev.Crema.Data;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Runtime.Serialization
{
    [Serializable]
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct SerializationColumn
    {
        public SerializationColumn(CremaDataColumn dataColumn)
            : this()
        {
            this.Index = dataColumn.Index;
            this.Tags = dataColumn.Tags;
            this.DerivedTags = dataColumn.DerivedTags;
            this.Name = dataColumn.ColumnName;
            this.DataType = dataColumn.DataTypeName;
            this.Comment = dataColumn.Comment;
            this.IsKey = dataColumn.IsKey;
            this.IsUnique = dataColumn.Unique;
            this.AllowNull = dataColumn.AllowDBNull;
            this.AutoIncrement = dataColumn.AutoIncrement;
            this.ReadOnly = dataColumn.ReadOnly;
        }

        [DataMember]
        public int Index { get; set; }

        [IgnoreDataMember]
        public TagInfo Tags { get; set; }

        [IgnoreDataMember]
        public TagInfo DerivedTags { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DataType { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public bool IsKey { get; set; }

        [DataMember]
        public bool IsUnique { get; set; }

        [DataMember]
        public bool AllowNull { get; set; }

        [DataMember]
        public bool AutoIncrement { get; set; }

        [DataMember]
        public bool ReadOnly { get; set; }

        #region Invisibles

        [DataMember(Name = nameof(Tags))]
        public string TagsMember
        {
            get => (string)this.Tags;
            set => this.Tags = (TagInfo)value;
        }

        [DataMember(Name = nameof(DerivedTags))]
        public string DerivedTagsMember
        {
            get => (string)this.DerivedTags;
            set => this.DerivedTags = (TagInfo)value;
        }

        #endregion
    }
}
