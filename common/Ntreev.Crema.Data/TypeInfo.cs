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
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Ntreev.Crema.Data
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    [Serializable]
    public struct TypeInfo
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public TypeMemberInfo[] Members { get; set; }

        [DataMember]
        public string CategoryPath { get; set; }

        [DataMember]
        public bool IsFlag { get; set; }

        [DataMember]
        public TagInfo Tags { get; set; }

        [DataMember]
        public TagInfo DerivedTags { get; set; }

        [DataMember]
        public SignatureDate CreationInfo { get; set; }

        [DataMember]
        public SignatureDate ModificationInfo { get; set; }

        [DataMember]
        public string HashValue
        {
            get; set;
        }

        public override bool Equals(object obj)
        {
            if (obj is TypeInfo == false)
                return false;

            TypeInfo dest = (TypeInfo)obj;

            return ((this.Name == dest.Name) &&
                    (this.Comment == dest.Comment) &&
                    (HashUtility.Equals(this.Members, dest.Members)) &&
                    (this.CategoryPath == dest.CategoryPath) &&
                    (this.IsFlag == dest.IsFlag) &&
                    (this.CreationInfo == dest.CreationInfo) &&
                    (this.ModificationInfo == dest.ModificationInfo));
        }

        public override string ToString()
        {
            return this.Name ?? string.Empty;
        }

        public override int GetHashCode()
        {
            return HashUtility.GetHashCode(this.Name) ^
                   HashUtility.GetHashCode(this.Comment) ^
                   HashUtility.GetHashCode(this.Members) ^
                   HashUtility.GetHashCode(this.CategoryPath) ^
                   HashUtility.GetHashCode(this.IsFlag) ^
                   HashUtility.GetHashCode(this.CreationInfo) ^
                   HashUtility.GetHashCode(this.ModificationInfo);
        }

        public IDictionary<string, object> ToDictionary()
        {
            var props = new Dictionary<string, object>
            {
                { nameof(this.ID), this.ID },
                { nameof(this.Name), this.Name },
                { nameof(this.Comment), this.Comment },
                { nameof(this.Tags), $"{this.Tags}" },
                { nameof(this.IsFlag), this.IsFlag },
                { nameof(this.CategoryPath), this.CategoryPath },
                { CremaSchema.Creator, this.CreationInfo.ID },
                { CremaSchema.CreatedDateTime, this.CreationInfo.DateTime },
                { CremaSchema.Modifier, this.ModificationInfo.ID },
                { CremaSchema.ModifiedDateTime, this.ModificationInfo.DateTime },
                { nameof(this.Members), this.GetMembersInfo(this.Members) }
            };

            return props;
        }

        [Obsolete("타입에는 태그 기능이 아직 적용되어 있지 않음")]
        public TypeInfo Filter(TagInfo tags)
        {
            var typeInfo = this;
            typeInfo.Members = typeInfo.Members.Where(item => (item.DerivedTags & tags) != TagInfo.Unused).ToArray();
            typeInfo.HashValue = CremaDataType.GenerateHashValue(typeInfo.Members);
            return typeInfo;
        }

        public string ConvertToString(long value)
        {
            if (this.IsFlag == true)
            {
                List<string> texts = new List<string>();

                if (value == 0)
                {
                    foreach (var item in this.Members)
                    {
                        if (item.Value == 0)
                        {
                            texts.Add(item.Name);
                            break;
                        }
                    }
                }
                else
                {
                    long v = value;
                    foreach (var item in this.Members.OrderByDescending(i => (ulong)i.Value))
                    {
                        if ((v & item.Value) == item.Value)
                        {
                            texts.Add(item.Name);
                            v &= ~item.Value;
                        }
                    }
                }
                if (texts.Any() == false)
                    return value.ToString();
                return string.Join(" ", texts);
            }
            else
            {
                foreach (var item in this.Members)
                {
                    if (item.Value == value)
                        return item.Name;
                }

                return value.ToString();
            }
        }

        public string CategoryName
        {
            get
            {
                return this.CategoryPath.Trim(PathUtility.SeparatorChar);
            }
        }

        public static readonly TypeInfo Empty;

        public static readonly TypeInfo Default = new TypeInfo()
        {
            Name = string.Empty,
            CategoryPath = PathUtility.Separator,
            Members = Enumerable.Empty<TypeMemberInfo>().ToArray(),
        };

        private object[] GetMembersInfo(TypeMemberInfo[] members)
        {
            var items = new object[members.Length];
            for (var i = 0; i < members.Length; i++)
            {
                items[i] = members[i].ToDictionary();
            }
            return items;
        }
    }
}
