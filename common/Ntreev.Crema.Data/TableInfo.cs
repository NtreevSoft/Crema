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
using Ntreev.Crema.Data.Xml;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using System.Data;
using Ntreev.Library;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.IO;
using Ntreev.Crema.Data.Xml.Schema;

namespace Ntreev.Crema.Data
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    [Serializable]
    public struct TableInfo
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public TagInfo Tags { get; set; }

        [DataMember]
        public TagInfo DerivedTags { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public SignatureDate CreationInfo { get; set; }

        [DataMember]
        public SignatureDate ModificationInfo { get; set; }

        [DataMember]
        public SignatureDate ContentsInfo { get; set; }

        [DataMember]
        public string TemplatedParent { get; set; }

        [DataMember]
        public ColumnInfo[] Columns { get; set; }

        [DataMember]
        public string CategoryPath { get; set; }

        public string ParentName
        {
            get { return CremaDataTable.GetParentName(this.Name); }
        }

        public string CategoryName
        {
            get { return this.CategoryPath.Trim(PathUtility.SeparatorChar); }
        }

        public string TableName
        {
            get { return CremaDataTable.GetTableName(this.Name); }
        }

        [DataMember]
        public string HashValue
        {
            get; set;
        }

        public override bool Equals(object obj)
        {
            if (obj is TableInfo == false)
                return false;

            TableInfo tableInfo = (TableInfo)obj;

            return tableInfo.Name == this.Name &&
                   tableInfo.CategoryPath == this.CategoryPath &&
                   tableInfo.Tags == this.Tags &&
                   tableInfo.Comment == this.Comment &&
                   tableInfo.CreationInfo == this.CreationInfo &&
                   tableInfo.ModificationInfo == this.ModificationInfo &&
                   tableInfo.ContentsInfo == this.ContentsInfo &&
                   tableInfo.TemplatedParent == this.TemplatedParent &&
                   HashUtility.Equals(tableInfo.Columns, this.Columns);
        }

        public override int GetHashCode()
        {
            return HashUtility.GetHashCode(this.Name) ^
                   HashUtility.GetHashCode(this.CategoryPath) ^
                   HashUtility.GetHashCode(this.Tags) ^
                   HashUtility.GetHashCode(this.Comment) ^
                   HashUtility.GetHashCode(this.CreationInfo) ^
                   HashUtility.GetHashCode(this.ModificationInfo) ^
                   HashUtility.GetHashCode(this.ContentsInfo) ^
                   HashUtility.GetHashCode(this.TemplatedParent) ^
                   HashUtility.GetHashCode(this.Columns);
        }

        public IDictionary<string, object> ToDictionary()
        {
            return this.ToDictionary(false);
        }

        public IDictionary<string, object> ToDictionary(bool omitColumns)
        {
            var props = new Dictionary<string, object>
            {
                { nameof(this.ID), this.ID },
                { nameof(this.Name), this.Name },
                { nameof(this.TableName), this.TableName },
                { nameof(this.Tags), $"{this.Tags}" },
                { nameof(this.DerivedTags), $"{this.DerivedTags}" },
                { nameof(this.Comment), this.Comment },
                { nameof(this.TemplatedParent), this.TemplatedParent },
                { nameof(this.ParentName), this.ParentName },
                { nameof(this.CategoryPath), this.CategoryPath },
                { CremaSchema.Creator, this.CreationInfo.ID },
                { CremaSchema.CreatedDateTime, this.CreationInfo.DateTime },
                { CremaSchema.Modifier, this.ModificationInfo.ID },
                { CremaSchema.ModifiedDateTime, this.ModificationInfo.DateTime },
                { CremaSchema.ContentsModifier, this.ContentsInfo.ID },
                { CremaSchema.ContentsModifiedDateTime, this.ContentsInfo.DateTime },
            };

            if (omitColumns == false)
            {
                props.Add(nameof(this.Columns), this.GetColumnsInfo(this.Columns));
            }

            return props;
        }

        public bool ContainsType(string dataType)
        {
            foreach (var item in this.Columns)
            {
                if (item.DataType == dataType)
                    return true;
            }
            return false;
        }

        public TableInfo Filter(TagInfo tags)
        {
            var tableInfo = this;
            tableInfo.Columns = tableInfo.Columns.Where(item => (item.DerivedTags & tags) != TagInfo.Unused).ToArray();
            tableInfo.HashValue = CremaDataTable.GenerateHashValue(tableInfo.Columns);
            return tableInfo;
        }

        public static bool operator ==(TableInfo left, TableInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TableInfo left, TableInfo right)
        {
            return !(left == right);
        }

        public static readonly TableInfo Empty;

        public static readonly TableInfo Default = new TableInfo()
        {
            Name = string.Empty,
            CategoryPath = PathUtility.Separator,
            Columns = Enumerable.Empty<ColumnInfo>().ToArray(),
        };

        private object[] GetColumnsInfo(ColumnInfo[] columns)
        {
            var items = new object[columns.Length];
            for (var i = 0; i < columns.Length; i++)
            {
                items[i] = columns[i].ToDictionary();
            }
            return items;
        }
    }
}
