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
    public class GetTableInfoResponse
    {
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

        [JsonProperty(CremaSchema.Creator)]
        public string Creator { get; set; }

        [JsonProperty(CremaSchema.CreatedDateTime)]
        public DateTime CreatedDateTime { get; set; }

        [JsonProperty(CremaSchema.Modifier)]
        public string Modifier { get; set; }

        [JsonProperty(CremaSchema.ModifiedDateTime)]
        public DateTime ModifiedDateTime { get; set; }

        [JsonProperty(CremaSchema.ContentsModifier)]
        public string ContentsModifier { get; set; }

        [JsonProperty(CremaSchema.ContentsModifiedDateTime)]
        public DateTime ContentsModifiedDateTime { get; set; }

        public ColumnInfoResponse[] Columns { get; set; }

        public static GetTableInfoResponse ConvertFrom(TableInfo tableInfo)
        {
            return new GetTableInfoResponse
            {
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
                Creator = tableInfo.CreationInfo.ID,
                CreatedDateTime = tableInfo.CreationInfo.DateTime,
                Modifier = tableInfo.ModificationInfo.ID,
                ModifiedDateTime = tableInfo.ModificationInfo.DateTime,
                ContentsModifier = tableInfo.ContentsInfo.ID,
                ContentsModifiedDateTime = tableInfo.ContentsInfo.DateTime,
                Columns = tableInfo.Columns.Select(ColumnInfoResponse.ConvertFrom).ToArray()
            };
        }

        public class ColumnInfoResponse
        {
            public Guid Id { get; set; }
            public bool IsKey { get; set; }
            public bool IsUnique { get; set; }
            public bool AllowNull { get; set; }
            public string Name { get; set; }
            public string DataType { get; set; }
            public object DefaultValue { get; set; }
            public string Comment { get; set; }
            public bool AutoIncrement { get; set; }
            public bool ReadOnly { get; set; }
            public string Tags { get; set; }
            public string DerivedTags { get; set; }

            [JsonProperty(CremaSchema.Creator)]
            public string Creator { get; set; }

            [JsonProperty(CremaSchema.CreatedDateTime)]
            public DateTime CreatedDateTime { get; set; }

            [JsonProperty(CremaSchema.Modifier)]
            public string Modifier { get; set; }

            [JsonProperty(CremaSchema.ModifiedDateTime)]
            public DateTime ModifiedDateTime { get; set; }

            public static ColumnInfoResponse ConvertFrom(ColumnInfo columnInfo)
            {
                return new ColumnInfoResponse
                {
                    Id = columnInfo.ID,
                    IsKey = columnInfo.IsKey,
                    IsUnique = columnInfo.IsUnique,
                    AllowNull = columnInfo.AllowNull,
                    Name = columnInfo.Name,
                    DataType = columnInfo.DataType,
                    DefaultValue = GetDefaultValue(columnInfo),
                    Comment = columnInfo.Comment,
                    AutoIncrement = columnInfo.AutoIncrement,
                    ReadOnly = columnInfo.ReadOnly,
                    Tags = columnInfo.Tags.ToString(),
                    DerivedTags = columnInfo.DerivedTags.ToString(),
                    Creator = columnInfo.CreationInfo.ID,
                    CreatedDateTime = columnInfo.CreationInfo.DateTime,
                    Modifier = columnInfo.ModificationInfo.ID,
                    ModifiedDateTime = columnInfo.ModificationInfo.DateTime
                };
            }

            private static object GetDefaultValue(ColumnInfo columnInfo)
            {
                if (columnInfo.DefaultValue != null && CremaDataTypeUtility.IsBaseType(columnInfo.DataType))
                {
                    var type = CremaDataTypeUtility.GetType(columnInfo.DataType);
                    return CremaConvert.ChangeType(columnInfo.DefaultValue, type);
                }

                return columnInfo.DefaultValue;
            }
        }
    }
}