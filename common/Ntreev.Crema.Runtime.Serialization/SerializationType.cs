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

namespace Ntreev.Crema.Runtime.Serialization
{
    [Serializable]
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct SerializationType
    {
        public SerializationType(CremaDataType dataType)
            : this()
        {
            this.Tags = dataType.Tags;
            this.DerivedTags = dataType.DerivedTags;
            this.CategoryPath = dataType.CategoryPath;
            this.Name = dataType.Name;
            this.IsFlag = dataType.IsFlag;
            this.Comment = dataType.Comment;
            this.HashValue = dataType.TypeInfo.HashValue;
            this.Members = dataType.Members.Select(item => new SerializationTypeMember(item)).ToArray();
        }

        [IgnoreDataMember]
        public TagInfo Tags { get; set; }

        [IgnoreDataMember]
        public TagInfo DerivedTags { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsFlag { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public SerializationTypeMember[] Members { get; set; }

        [DataMember]
        public string CategoryPath { get; set; }

        [DataMember]
        public string HashValue { get; set; }

        [Obsolete("타입에는 태그 기능이 아직 적용되어 있지 않음")]
        internal SerializationType Filter(TagInfo tags)
        {
            var type = this;
            var memberList = new List<SerializationTypeMember>(type.Members.Length);
            for (var i = 0; i < type.Members.Length; i++)
            {
                var member = type.Members[i];
                if ((member.DerivedTags & tags) == TagInfo.Unused)
                    continue;

                memberList.Add(member);
            }

            type.Members = memberList.ToArray();
            type.HashValue = type.GetHashValue();
            return type;
        }

        public long ConvertFromString(string textValue)
        {
            var members = this.Members.ToDictionary(item => item.Name, item => item.Value);
            if (this.IsFlag == true)
            {
                var ss = StringUtility.Split(textValue);

                long value = 0;
                foreach (var item in ss)
                {
                    if (members.ContainsKey(item) == true)
                    {
                        value |= members[item];
                    }
                }
                return value;
            }
            else
            {
                if (members.ContainsKey(textValue) == true)
                {
                    return members[textValue];
                }
                return 0;
            }
        }

        private string GetHashValue()
        {
            var argList = new List<object>(this.Members.Length * 2);
            foreach (var item in this.Members)
            {
                argList.Add(item.Name);
                argList.Add(item.Value);
            }
            return CremaDataType.GenerateHashValue(argList.ToArray());
        }

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