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

namespace Ntreev.Crema.Runtime.Generation
{
    [Serializable]
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct GenerationSet
    {
        [DataMember]
        public TypeInfo[] Types { get; set; }

        [DataMember]
        public TableInfo[] Tables { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Tags { get; set; }

        [DataMember]
        public string Revision { get; set; }

        [DataMember]
        public string TablesHashValue { get; set; }

        [DataMember]
        public string TypesHashValue { get; set; }

        [DataMember]
        public string HashValue { get; set; }

        public GenerationSet(TypeInfo[] types, TableInfo[] tables)
            : this()
        {
            this.Types = types;
            this.Tables = tables;
            this.Revision = null;
            this.TypesHashValue = GetTypesHashValue(this);
            this.TablesHashValue = GetTablesHashValue(this);
            this.Tags = string.Empty;
        }

        public GenerationSet Filter(TagInfo tags)
        {
            var metaSet = this;
            var tableList = new List<TableInfo>(metaSet.Tables.Length);
            for (var i = 0; i < metaSet.Tables.Length; i++)
            {
                var table = this.Tables[i];
                if ((table.DerivedTags & tags) == TagInfo.Unused)
                    continue;

                tableList.Add(table.Filter(tags));
            }
            metaSet.Tables = tableList.ToArray();

            var typeList = new List<TypeInfo>(metaSet.Types.Length);
            for (var i = 0; i < metaSet.Types.Length; i++)
            {
                var type = this.Types[i];
                //if ((type.DerivedTags & tags) == TagInfo.Unused)
                //    continue;

                typeList.Add(type);
            }
            metaSet.Types = typeList.ToArray();
            metaSet.TypesHashValue = GetTypesHashValue(metaSet);
            metaSet.TablesHashValue = GetTablesHashValue(metaSet);
            metaSet.Tags = (string)tags;
            return metaSet;
        }

        public GenerationSet Filter(string filterExpression)
        {
            var metaSet = this;
            var tableList = new List<TableInfo>(metaSet.Tables.Length);
            for (var i = 0; i < metaSet.Tables.Length; i++)
            {
                var table = metaSet.Tables[i];
                if (StringUtility.GlobMany(table.Name, filterExpression) == false)
                    continue;

                tableList.Add(table);
            }
            metaSet.Tables = tableList.ToArray();

            var typeList = new List<TypeInfo>(metaSet.Types.Length);
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

        private static string GetTypesHashValue(GenerationSet dataSet)
        {
            var args = dataSet.Types.Select(item => item.HashValue).ToArray();
            using (var algorithm = HashAlgorithm.Create("SHA1"))
            {
                return HashUtility.GetHashValue(algorithm, args);
            }
        }

        private static string GetTablesHashValue(GenerationSet dataSet)
        {
            var args = dataSet.Tables.Select(item => item.HashValue).ToArray();
            using (var algorithm = HashAlgorithm.Create("SHA1"))
            {
                return HashUtility.GetHashValue(algorithm, args);
            }
        }
    }
}
