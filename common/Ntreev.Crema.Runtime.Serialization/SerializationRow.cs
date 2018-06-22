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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library;
using System.Xml.Serialization;

namespace Ntreev.Crema.Runtime.Serialization
{
    [Serializable]
    [DataContract(Namespace = SchemaUtility.Namespace)]
    [KnownType(typeof(DBNull))]
    [XmlInclude(typeof(DBNull))]
    public struct SerializationRow
    {
        public SerializationRow(CremaDataRow dataRow)
            : this(dataRow, dataRow.Table.Columns.Where(item => item.DerivedTags != TagInfo.Unused).ToArray())
        {

        }

        public SerializationRow(CremaDataRow dataRow, CremaDataColumn[] columns)
            : this()
        {
            var columnCount = columns.Length;

            if (dataRow.Table.Childs.Any())
                columnCount++;
            if (dataRow.Table.Parent != null)
                columnCount++;

            var fields = new object[columnCount];

            var i = 0;
            for (; i < columns.Length; i++)
            {
                var field = dataRow[columns[i]];
                if (field is TimeSpan timeSpan)
                    fields[i] = timeSpan.Ticks;
                else
                    fields[i] = field;
            }

            if (dataRow.Table.Childs.Any())
                fields[i++] = dataRow.RelationID;
            if (dataRow.Table.Parent != null)
                fields[i++] = dataRow.ParentID;

            this.Tags = dataRow.Tags;
            this.DerivedTags = dataRow.DerivedTags;
            this.Fields = fields;
            this.RelationID = dataRow.RelationID;
            this.ParentID = dataRow.ParentID;
        }

        [IgnoreDataMember]
        public TagInfo Tags { get; set; }

        [IgnoreDataMember]
        public TagInfo DerivedTags { get; set; }

        [IgnoreDataMember]
        public object[] Fields { get; set; }

        [DataMember]
        public string RelationID { get; set; }

        [DataMember]
        public string ParentID { get; set; }

        internal SerializationRow Filter(TagInfo tags, SerializationColumn[] columns)
        {
            var fieldList = new List<object>(this.Fields.Length);

            for (var i = 0; i < columns.Length; i++)
            {
                if ((columns[i].DerivedTags & tags) != TagInfo.Unused)
                {
                    fieldList.Add(this.Fields[i]);
                }
            }

            var row = this;
            row.Fields = fieldList.ToArray();
            return row;
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

        [DataMember(Name = nameof(Fields))]
        public SerializationField[] FieldInfos
        {
            get { return this.Fields != null ? this.Fields.Select(item => new SerializationField(item)).ToArray() : new SerializationField[] { }; }
            set { this.Fields = value?.Select(item => item.ToValue()).ToArray(); }
        }

        #endregion
    }
}
