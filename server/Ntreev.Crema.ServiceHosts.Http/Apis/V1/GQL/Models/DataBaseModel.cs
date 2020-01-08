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
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.GQL.Models
{
    public class DataBaseModel
    {
        public IDataBase DataBase { get; private set; }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public long Revision { get; set; }
        public string Tags { get; set; }
        public string TypesHashValue { get; set; }
        public string TablesHashValue { get; set; }
        public string[] Paths { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifiedDateTime { get; set; }

        public static  DataBaseModel ConvertFrom(IDataBase dataBase, DataBaseInfo info)
        {
            return new DataBaseModel
            {
                DataBase = dataBase,
                Id = info.ID,
                Name = info.Name,
                Comment = info.Comment,
                Revision = info.Revision,
                Tags = info.Tags.ToString(),
                TypesHashValue = info.TypesHashValue,
                TablesHashValue = info.TablesHashValue,
                Paths = info.Paths,
                Creator = info.CreationInfo.ID,
                CreatedDateTime = info.CreationInfo.DateTime,
                Modifier = info.ModificationInfo.ID,
                ModifiedDateTime = info.ModificationInfo.DateTime
            };
        }
    }
}
