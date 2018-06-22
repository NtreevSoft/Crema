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
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Runtime.Serialization
{
    [Serializable]
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct SerializationSet
    {
        public SerializationSet(CremaDataSet dataSet)
            : this()
        {
            this.Types = dataSet.Types.Select(item => new SerializationType(item)).ToArray();
            this.Tables = dataSet.Tables.Select(item => new SerializationTable(item)).ToArray();
            this.Name = dataSet.DataSetName;
            this.Revision = null;
            this.TypesHashValue = GetTypesHashValue(this);
            this.TablesHashValue = GetTablesHashValue(this);
            this.Tags = TagInfo.All;
        }

        [DataMember]
        public SerializationType[] Types { get; set; }

        [DataMember]
        public SerializationTable[] Tables { get; set; }

        [DataMember]
        public string Name { get; set; }

        [IgnoreDataMember]
        public TagInfo Tags { get; set; }

        [DataMember]
        public string Revision { get; set; }

        [DataMember]
        public string TypesHashValue { get; set; }

        [DataMember]
        public string TablesHashValue { get; set; }

        public SerializationSet Filter(TagInfo tags)
        {
            var metaSet = this;
            var tableList = new List<SerializationTable>(this.Tables.Length);
            for (var i = 0; i < this.Tables.Length; i++)
            {
                var table = this.Tables[i];
                if (tags != TagInfo.All && (table.DerivedTags & tags) == TagInfo.Unused)
                    continue;

                tableList.Add(table.Filter(tags));
            }
            metaSet.Tables = tableList.ToArray();

            var typeList = new List<SerializationType>(this.Types.Length);
            for (var i = 0; i < this.Types.Length; i++)
            {
                var type = this.Types[i];
                //if (tags != TagInfo.All && (type.DerivedTags & tags) == TagInfo.Unused)
                //    continue;

                typeList.Add(type);
            }
            metaSet.Types = typeList.ToArray();
            metaSet.TypesHashValue = GetTypesHashValue(metaSet);
            metaSet.TablesHashValue = GetTablesHashValue(metaSet);
            metaSet.Tags = tags;
            return metaSet;
        }

        public SerializationSet Filter(string filterExpression)
        {
            var metaSet = this;
            var tableList = new List<SerializationTable>(metaSet.Tables.Length);
            for (var i = 0; i < metaSet.Tables.Length; i++)
            {
                var table = metaSet.Tables[i];
                if (StringUtility.GlobMany(table.Name, filterExpression) == false)
                    continue;

                tableList.Add(table);
            }
            metaSet.Tables = tableList.ToArray();

            var typeList = new List<SerializationType>(metaSet.Types.Length);
            for (var i = 0; i < metaSet.Types.Length; i++)
            {
                var type = metaSet.Types[i];
                if (Contains() == true)
                    typeList.Add(type);

                bool Contains()
                {
                    foreach (var item in metaSet.Tables)
                    {
                        if (item.ContainsType(type.CategoryPath + type.Name) == true)
                            return true;
                    }
                    return false;
                }
            }
            metaSet.Types = typeList.ToArray();
            metaSet.TypesHashValue = GetTypesHashValue(metaSet);
            metaSet.TablesHashValue = GetTablesHashValue(metaSet);
            return metaSet;
        }

        private static string GetTypesHashValue(SerializationSet dataSet)
        {
            var args = dataSet.Types.Select(item => item.HashValue).ToArray();
            using (var algorithm = HashAlgorithm.Create("SHA1"))
            {
                return HashUtility.GetHashValue(algorithm, args);
            }
        }

        private static string GetTablesHashValue(SerializationSet dataSet)
        {
            var args = dataSet.Tables.Select(item => item.HashValue).ToArray();
            using (var algorithm = HashAlgorithm.Create("SHA1"))
            {
                return HashUtility.GetHashValue(algorithm, args);
            }
        }

        #region Invisibles

        [DataMember(Name = nameof(Tags))]
        public string TagsMember
        {
            get => (string)this.Tags;
            set => this.Tags = (TagInfo)value;
        }

        #endregion
    }
}
