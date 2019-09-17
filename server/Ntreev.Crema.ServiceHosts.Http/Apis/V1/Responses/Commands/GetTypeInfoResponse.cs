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
using System.Linq;
using Newtonsoft.Json;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.Responses.Commands
{
    public class GetTypeInfoResponse
    {
        [JsonProperty("ID")]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Tags { get; set; }
        public bool IsFlag { get; set; }
        public string CategoryPath { get; set; }
        public string HashValue { get; set; }

        [JsonProperty(CremaSchema.Creator)]
        public string Creator { get; set; }

        [JsonProperty(CremaSchema.CreatedDateTime)]
        public DateTime CreatedDateTime { get; set; }

        [JsonProperty(CremaSchema.Modifier)]
        public string Modifier { get; set; }

        [JsonProperty(CremaSchema.ModifiedDateTime)]
        public DateTime ModifiedDateTime { get; set; }

        public MemberInfoResponse[] Members { get; set; }

        public static GetTypeInfoResponse ConvertFrom(TypeInfo typeInfo)
        {
            return new GetTypeInfoResponse
            {
                Id = typeInfo.ID,
                Name = typeInfo.Name,
                Comment = typeInfo.Comment,
                Tags = typeInfo.Tags.ToString(),
                IsFlag = typeInfo.IsFlag,
                CategoryPath = typeInfo.CategoryPath,
                HashValue = typeInfo.HashValue,
                Creator = typeInfo.CreationInfo.ID,
                CreatedDateTime = typeInfo.CreationInfo.DateTime,
                Modifier = typeInfo.ModificationInfo.ID,
                ModifiedDateTime = typeInfo.ModificationInfo.DateTime,
                Members = typeInfo.Members.Select(MemberInfoResponse.ConvertFrom).ToArray()
            };
        }

        public class MemberInfoResponse
        {
            [JsonProperty("ID")]
            public Guid Id { get; set; }
            public string Name { get; set; }
            public long Value { get; set; }
            public string Comment { get; set; }
            public string Tags { get; set; }
            public string DerivedTags { get; set; }
            public bool IsEnabled { get; set; }

            [JsonProperty(CremaSchema.Creator)]
            public string Creator { get; set; }

            [JsonProperty(CremaSchema.CreatedDateTime)]
            public DateTime CreatedDateTime { get; set; }

            [JsonProperty(CremaSchema.Modifier)]
            public string Modifier { get; set; }

            [JsonProperty(CremaSchema.ModifiedDateTime)]
            public DateTime ModifiedDateTime { get; set; }

            public static MemberInfoResponse ConvertFrom(TypeMemberInfo member)
            {
                return new MemberInfoResponse
                {
                    Id = member.ID,
                    Name = member.Name,
                    Value = member.Value,
                    Comment = member.Comment,
                    Tags = member.Tags.ToString(),
                    DerivedTags = member.DerivedTags.ToString(),
                    IsEnabled = member.IsEnabled,
                    Creator = member.CreationInfo.ID,
                    CreatedDateTime = member.CreationInfo.DateTime,
                    Modifier = member.ModificationInfo.ID,
                    ModifiedDateTime = member.ModificationInfo.DateTime
                };
            }
        }
    }
}