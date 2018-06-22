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
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;

namespace Ntreev.Crema.Runtime.Serialization
{
    [Serializable]
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct SerializationTable
    {
        public SerializationTable(CremaDataTable dataTable)
            : this()
        {
            this.Tags = dataTable.Tags;
            this.DerivedTags = dataTable.DerivedTags;
            this.CategoryPath = dataTable.CategoryPath;
            this.Name = dataTable.Name;
            this.TableName = dataTable.TableName;
            this.Parent = dataTable.ParentName;
            this.TemplatedParent = dataTable.TemplatedParentName;
            this.Comment = dataTable.Comment;

            var columns = dataTable.Columns.ToArray();
            var columnList = columns.Select(item => new SerializationColumn(item)).ToList();

            if (dataTable.Childs.Any() == true)
            {
                columnList.Add(new SerializationColumn()
                {
                    Index = columnList.Count,
                    Tags = TagInfo.All,
                    DerivedTags = TagInfo.All,
                    Name = CremaSchema.__RelationID__,
                    DataType = typeof(string).GetTypeName(),
                    IsKey = false,
                });
            }

            if (dataTable.Parent != null)
            {
                columnList.Add(new SerializationColumn()
                {
                    Index = columnList.Count,
                    Tags = TagInfo.All,
                    DerivedTags = TagInfo.All,
                    Name = CremaSchema.__ParentID__,
                    DataType = typeof(string).GetTypeName(),
                    IsKey = false,
                });
            }

            this.Columns = columnList.ToArray();
            this.Rows = dataTable.Rows.Where(item => item.IsDerivedEnabled == true)
                                      .Select(item => new SerializationRow(item, columns)).ToArray();
            this.HashValue = this.GetHashValue();
        }

        [IgnoreDataMember]
        public TagInfo Tags { get; set; }

        [IgnoreDataMember]
        public TagInfo DerivedTags { get; set; }

        [DataMember]
        public string HashValue { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string TableName { get; set; }

        [DataMember]
        public string TemplatedParent { get; set; }

        [DataMember]
        public string Parent { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public SerializationColumn[] Columns { get; set; }

        [DataMember]
        public SerializationRow[] Rows { get; set; }

        [DataMember]
        public string CategoryPath { get; set; }

        internal SerializationTable Filter(TagInfo tags)
        {
            var table = this;
            var columnList = new List<SerializationColumn>(table.Columns.Length);
            for (var i = 0; i < table.Columns.Length; i++)
            {
                var column = table.Columns[i];
                if ((column.DerivedTags & tags) == TagInfo.Unused)
                    continue;

                columnList.Add(column);
            }
            table.Columns = columnList.ToArray();

            var rowList = new List<SerializationRow>(this.Rows.Length);
            for (var i = 0; i < table.Rows.Length; i++)
            {
                var row = table.Rows[i];
                if ((row.DerivedTags & tags) == TagInfo.Unused)
                    continue;

                rowList.Add(row.Filter(tags, this.Columns));
            }
            table.Rows = rowList.ToArray();

            table.HashValue = table.GetHashValue();
            return table;
        }

        internal bool ContainsType(string dataType)
        {
            foreach (var item in this.Columns)
            {
                if (item.DataType == dataType)
                    return true;
            }
            return false;
        }

        private string GetHashValue()
        {
            var argList = new List<object>(this.Columns.Length * 4);
            foreach (var item in this.Columns)
            {
                if (item.Name == CremaSchema.__ParentID__ || item.Name == CremaSchema.__RelationID__)
                    continue;
                argList.Add(item.DataType);
                argList.Add(item.Name);
                argList.Add(item.IsKey);
                argList.Add(item.IsUnique);
            }
            return CremaDataTable.GenerateHashValue(argList.ToArray());
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