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
    public class ColumnModel
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
        public string Creator { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifiedDateTime { get; set; }

        public static ColumnModel ConvertFrom(ColumnInfo columnInfo)
        {
            return new ColumnModel
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
