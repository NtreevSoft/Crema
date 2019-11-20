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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Runtime.Serialization;
using Ntreev.Library;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.Responses.CremaDev
{
    public class SerializationSetResponse
    {
        public SerializationTypeResponse[] Types { get; set; }
        public SerializationTableResponse[] Tables { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public long Revision { get; set; }
        public string TypesHashValue { get; set; }
        public string TablesHashValue { get; set; }

        public static SerializationSetResponse ConvertFrom(SerializationSet serializationSet)
        {
            return new SerializationSetResponse
            {
                Name = serializationSet.Name,
                Tags = serializationSet.Tags,
                Revision = serializationSet.Revision,
                TypesHashValue = serializationSet.TypesHashValue,
                TablesHashValue = serializationSet.TablesHashValue,
                Types = serializationSet.Types.Select(SerializationTypeResponse.ConvertFrom).ToArray(),
                Tables = serializationSet.Tables.Select(SerializationTableResponse.ConvertFrom).ToArray()
            };
        }
    }

    public class SerializationTableResponse
    {
        public TagInfo Tags { get; set; }
        public TagInfo DerivedTags { get; set; }
        public string HashValue { get; set; }
        public string Name { get; set; }
        public string TableName { get; set; }
        public string TemplatedParent { get; set; }
        public string Parent { get; set; }
        public string Comment { get; set; }
        public long Revision { get; set; }
        public SerializationColumnResponse[] Columns { get; set; }
        public SerializationRowResponse[] Rows { get; set; }
        public string CategoryPath { get; set; }

        public static SerializationTableResponse ConvertFrom(SerializationTable serializationTable)
        {
            return new SerializationTableResponse
            {
                Tags = serializationTable.Tags,
                DerivedTags = serializationTable.DerivedTags,
                HashValue = serializationTable.HashValue,
                Name = serializationTable.Name,
                TableName = serializationTable.TableName,
                TemplatedParent = serializationTable.TemplatedParent,
                Parent = serializationTable.Parent,
                Comment = serializationTable.Comment,
                Revision = serializationTable.Revision,
                CategoryPath = serializationTable.CategoryPath,
                Columns = serializationTable.Columns.Select(SerializationColumnResponse.ConvertFrom).ToArray(),
                Rows = serializationTable.Rows.Select(SerializationRowResponse.ConvertFrom).ToArray()
            };
        }
    }

    public class SerializationTypeResponse
    {
        public TagInfo Tags { get; set; }
        public TagInfo DerivedTags { get; set; }
        public string Name { get; set; }
        public bool IsFlag { get; set; }
        public string Comment { get; set; }
        public SerializationTypeMemberResponse[] Members { get; set; }
        public string CategoryPath { get; set; }
        public string HashValue { get; set; }
        public long Revision { get; set; }

        public static SerializationTypeResponse ConvertFrom(SerializationType serializationType)
        {
            return new SerializationTypeResponse
            {
                Tags = serializationType.Tags,
                DerivedTags = serializationType.DerivedTags,
                Name = serializationType.Name,
                IsFlag = serializationType.IsFlag,
                Comment = serializationType.Comment,
                CategoryPath = serializationType.CategoryPath,
                HashValue = serializationType.HashValue,
                Revision = serializationType.Revision,
                Members = serializationType.Members.Select(SerializationTypeMemberResponse.ConvertFrom).ToArray()
            };
        }
    }

    public class SerializationTypeMemberResponse
    {
        public TagInfo Tags { get; set; }
        public TagInfo DerivedTags { get; set; }
        public string Name { get; set; }
        public long Value { get; set; }
        public string Comment { get; set; }

        public static SerializationTypeMemberResponse ConvertFrom(SerializationTypeMember serializationTypeMember)
        {
            return new SerializationTypeMemberResponse
            {
                Tags = serializationTypeMember.Tags,
                DerivedTags = serializationTypeMember.DerivedTags,
                Name = serializationTypeMember.Name,
                Value = serializationTypeMember.Value,
                Comment = serializationTypeMember.Comment
            };
        }
    }

    public class SerializationColumnResponse
    {
        public int Index { get; set; }
        public TagInfo Tags { get; set; }
        public TagInfo DerivedTags { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public string Comment { get; set; }
        public bool IsKey { get; set; }
        public bool IsUnique { get; set; }
        public bool AllowNull { get; set; }
        public bool AutoIncrement { get; set; }
        public bool ReadOnly { get; set; }

        public static SerializationColumnResponse ConvertFrom(SerializationColumn serializationColumn)
        {
            return new SerializationColumnResponse
            {
                Index = serializationColumn.Index,
                Tags = serializationColumn.Tags,
                DerivedTags = serializationColumn.DerivedTags,
                Name = serializationColumn.Name,
                DataType = serializationColumn.DataType,
                Comment = serializationColumn.Comment,
                IsKey = serializationColumn.IsKey,
                IsUnique = serializationColumn.IsUnique,
                AllowNull = serializationColumn.AllowNull,
                AutoIncrement = serializationColumn.AutoIncrement,
                ReadOnly = serializationColumn.ReadOnly
            };
        }
    }

    public class SerializationRowResponse
    {
        public TagInfo Tags { get; set; }
        public TagInfo DerivedTags { get; set; }
        public object[] Fields { get; set; }
        public string RelationID { get; set; }
        public string ParentID { get; set; }

        public static SerializationRowResponse ConvertFrom(SerializationRow serializationRow)
        {
            return new SerializationRowResponse
            {
                Tags = serializationRow.Tags,
                DerivedTags = serializationRow.DerivedTags,
                Fields = serializationRow.Fields,
                RelationID = serializationRow.RelationID,
                ParentID = serializationRow.ParentID
            };
        }
    }
}
