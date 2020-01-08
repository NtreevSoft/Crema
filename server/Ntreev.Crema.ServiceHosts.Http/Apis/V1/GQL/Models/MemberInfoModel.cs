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
using Ntreev.Crema.Data;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.GQL.Models
{
    public class MemberInfoModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public long Value { get; set; }
        public string Comment { get; set; }
        public string Tags { get; set; }
        public string DerivedTags { get; set; }
        public bool IsEnabled { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifiedDateTime { get; set; }

        public static MemberInfoModel ConvertFrom(TypeMemberInfo member)
        {
            return new MemberInfoModel
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