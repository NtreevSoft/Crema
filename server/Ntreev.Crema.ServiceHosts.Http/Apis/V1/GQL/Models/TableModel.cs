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
    public class TableModel
    {
        public IDataBase DataBase { get; private set; }
        public ITable Table { get; private set; }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TableName { get; set; }
        public string Tags { get; set; }
        public string DerivedTags { get; set; }
        public string Comment { get; set; }
        public string TemplatedParent { get; set; }
        public string ParentName { get; set; }
        public string CategoryPath { get; set; }
        public string HashValue { get; set; }
        public long Revision { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ContentsModifier { get; set; }
        public DateTime ContentsModifiedDateTime { get; set; }
        public ColumnModel[] Columns { get; set; }

        public static TableModel ConvertFrom(IDataBase dataBase, ITable table, TableInfo tableInfo)
        {
            return new TableModel
            {
                DataBase = dataBase,
                Table = table,
                Id = tableInfo.ID,
                Name = tableInfo.Name,
                TableName = tableInfo.TableName,
                Tags = tableInfo.Tags.ToString(),
                DerivedTags = tableInfo.DerivedTags.ToString(),
                Comment = tableInfo.Comment,
                TemplatedParent = tableInfo.TemplatedParent,
                ParentName = tableInfo.ParentName,
                CategoryPath = tableInfo.CategoryPath,
                HashValue = tableInfo.HashValue,
                Revision = tableInfo.Revision,
                Creator = tableInfo.CreationInfo.ID,
                CreatedDateTime = tableInfo.CreationInfo.DateTime,
                Modifier = tableInfo.ModificationInfo.ID,
                ModifiedDateTime = tableInfo.ModificationInfo.DateTime,
                ContentsModifier = tableInfo.ContentsInfo.ID,
                ContentsModifiedDateTime = tableInfo.ContentsInfo.DateTime,
                Columns = tableInfo.Columns.Select(ColumnModel.ConvertFrom).ToArray()
            };
        }
    }
}
