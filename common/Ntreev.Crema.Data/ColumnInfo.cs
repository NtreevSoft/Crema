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

using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ntreev.Crema.Data
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    [Serializable]
    public struct ColumnInfo
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public bool IsKey { get; set; }

        [DataMember]
        public bool IsUnique { get; set; }

        [DataMember]
        public bool AllowNull { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DataType { get; set; }

        [DataMember]
        public string DefaultValue { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public bool AutoIncrement { get; set; }

        [DataMember]
        public TagInfo Tags { get; set; }

        [DataMember]
        public bool ReadOnly { get; set; }

        [DataMember]
        public TagInfo DerivedTags { get; set; }

        [DataMember]
        public SignatureDate CreationInfo { get; set; }

        [DataMember]
        public SignatureDate ModificationInfo { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ColumnInfo == false)
                return false;

            var columnInfo = (ColumnInfo)obj;
            return ((columnInfo.ID == this.ID) &&
                    (columnInfo.IsKey == this.IsKey) &&
                    (columnInfo.IsUnique == this.IsUnique) &&
                    (columnInfo.Name == this.Name) &&
                    (columnInfo.DataType == this.DataType) &&
                    (columnInfo.DefaultValue == this.DefaultValue) &&
                    (columnInfo.Comment == this.Comment) &&
                    (columnInfo.AutoIncrement == this.AutoIncrement) &&
                    (columnInfo.Tags == this.Tags) &&
                    (columnInfo.CreationInfo == this.CreationInfo) &&
                    (columnInfo.ModificationInfo == this.ModificationInfo));
        }

        public override int GetHashCode()
        {
            return HashUtility.GetHashCode(this.ID) ^
                   HashUtility.GetHashCode(this.IsKey) ^
                   HashUtility.GetHashCode(this.IsUnique) ^
                   HashUtility.GetHashCode(this.Name) ^
                   HashUtility.GetHashCode(this.DataType) ^
                   HashUtility.GetHashCode(this.DefaultValue) ^
                   HashUtility.GetHashCode(this.Comment) ^
                   HashUtility.GetHashCode(this.AutoIncrement) ^
                   HashUtility.GetHashCode(this.Tags) ^
                   HashUtility.GetHashCode(this.CreationInfo) ^
                   HashUtility.GetHashCode(this.ModificationInfo);
        }

        public IDictionary<string, object> ToDictionary()
        {
            var props = new Dictionary<string, object>
            {
                { nameof(this.ID), this.ID },
                { nameof(this.IsKey), this.IsKey },
                { nameof(this.IsUnique), this.IsUnique },
                { nameof(this.AllowNull), this.AllowNull },
                { nameof(this.Name), this.Name },
                { nameof(this.DataType), this.DataType },
                { nameof(this.DefaultValue), this.GetDefaultValue(this) },
                { nameof(this.Comment), this.Comment },
                { nameof(this.AutoIncrement), this.AutoIncrement },
                { nameof(this.ReadOnly), this.ReadOnly },
                { nameof(this.Tags), $"{this.Tags}" },
                { nameof(this.DerivedTags), $"{this.DerivedTags}" },
                { CremaSchema.Creator, this.CreationInfo.ID },
                { CremaSchema.CreatedDateTime, this.CreationInfo.DateTime },
                { CremaSchema.Modifier, this.ModificationInfo.ID },
                { CremaSchema.ModifiedDateTime, this.ModificationInfo.DateTime }
            };
            return props;
        }

        private object GetDefaultValue(ColumnInfo columnInfo)
        {
            if (columnInfo.DefaultValue != null && CremaDataTypeUtility.IsBaseType(columnInfo.DataType) == true)
            {
                var type = CremaDataTypeUtility.GetType(columnInfo.DataType);
                return CremaConvert.ChangeType(columnInfo.DefaultValue, type);
            }
            return columnInfo.DefaultValue;
        }
    }
}