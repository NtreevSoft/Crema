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
    public struct TypeMemberInfo
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public long Value { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public TagInfo Tags { get; set; }

        [DataMember]
        public TagInfo DerivedTags { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public SignatureDate CreationInfo { get; set; }

        [DataMember]
        public SignatureDate ModificationInfo { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TypeMemberInfo == false)
                return false;

            var member = (TypeMemberInfo)obj;
            return ((member.ID == this.ID) &&
                    (member.Name == this.Name) &&
                    (member.Value == this.Value) &&
                    (member.Comment == this.Comment) &&
                    (member.CreationInfo == this.CreationInfo) &&
                    (member.ModificationInfo == this.ModificationInfo));
        }

        public override int GetHashCode()
        {
            return HashUtility.GetHashCode(this.ID) ^
                   HashUtility.GetHashCode(this.Name) ^
                   HashUtility.GetHashCode(this.Value) ^
                   HashUtility.GetHashCode(this.Comment) ^
                   HashUtility.GetHashCode(this.CreationInfo) ^
                   HashUtility.GetHashCode(this.ModificationInfo);
        }

        public IDictionary<string, object> ToDictionary()
        {
            var props = new Dictionary<string, object>
            {
                { nameof(this.ID), this.ID },
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

        public override string ToString()
        {
            return string.Format("{0}, {1}", this.Name ?? "(null)", this.Value);
        }
    }
}
