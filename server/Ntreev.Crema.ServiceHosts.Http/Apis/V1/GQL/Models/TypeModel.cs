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
using Ntreev.Crema.Data;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.GQL.Models
{
    public class TypeModel
    {
        public IDataBase DataBase { get; private set; }
        public IType Type { get; private set; }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Tags { get; set; }
        public bool IsFlag { get; set; }
        public string CategoryPath { get; set; }
        public string HashValue { get; set; }
        public long Revision { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public MemberInfoModel[] Members { get; set; }

        public static TypeModel ConvertFrom(IDataBase dataBase, IType type, TypeInfo typeInfo)
        {
            return new TypeModel
            {
                DataBase = dataBase,
                Type = type,
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
                Revision = typeInfo.Revision,
                Members = typeInfo.Members.Select(MemberInfoModel.ConvertFrom).ToArray()
            };
        }
    }
}
