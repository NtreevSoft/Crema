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

using Ntreev.Crema.Services;
using Ntreev.Crema.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Ntreev.Crema.Data.Xml.Schema;
using System.Reflection;
using Ntreev.Library;
using Ntreev.Library.IO;

namespace Ntreev.Crema.Javascript.Methods
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class GetDataBaseInfoByTagsMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public GetDataBaseInfoByTagsMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, IDictionary<string, object>>(this.GetDataBaseInfoByTags);
        }

        private IDictionary<string, object> GetDataBaseInfoByTags(string dataBaseName, string tags)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            return dataBase.Dispatcher.Invoke(() =>
            {
                var dataBaseInfo = dataBase.DataBaseInfo;
                dataBaseInfo.Tags = (TagInfo)tags;
                dataBaseInfo.Paths = this.GetItems(dataBase, (TagInfo)tags).ToArray();
                dataBaseInfo.TypesHashValue = this.GetTypesHashValue(dataBase, (TagInfo)tags);
                dataBaseInfo.TablesHashValue = this.GetTablesHashValue(dataBase, (TagInfo)tags);

                var props = new Dictionary<string, object>
                {
                    { nameof(dataBaseInfo.ID), $"{dataBaseInfo.ID}" },
                    { nameof(dataBaseInfo.Name), dataBaseInfo.Name },
                    { nameof(dataBaseInfo.Comment), dataBaseInfo.Comment },
                    { nameof(dataBaseInfo.Revision), dataBaseInfo.Revision },
                    { nameof(dataBaseInfo.Tags), dataBaseInfo.Tags },
                    { nameof(dataBaseInfo.TypesHashValue), dataBaseInfo.TypesHashValue },
                    { nameof(dataBaseInfo.TablesHashValue), dataBaseInfo.TablesHashValue },
                    { nameof(dataBaseInfo.Paths), dataBaseInfo.Paths },
                    { CremaSchema.Creator, dataBaseInfo.CreationInfo.ID },
                    { CremaSchema.CreatedDateTime, dataBaseInfo.CreationInfo.DateTime },
                    { CremaSchema.Modifier, dataBaseInfo.ModificationInfo.ID },
                    { CremaSchema.ModifiedDateTime, dataBaseInfo.ModificationInfo.DateTime }
                };
                return props;
            });
        }

        private string GetTypesHashValue(IDataBase dataBase, TagInfo tags)
        {
            var typeInfoList = new List<Data.TypeInfo>();
            foreach (var item in dataBase.TypeContext.Types.OrderBy(item => item.Name))
            {
                if ((item.TypeInfo.Tags & tags) != TagInfo.Unused)
                {
                    var typeInfo = item.TypeInfo;
                    typeInfoList.Add(typeInfo);
                }
            }
            return CremaDataSet.GenerateHashValue(typeInfoList.ToArray());
        }

        private string GetTablesHashValue(IDataBase dataBase, TagInfo tags)
        {
            var tableInfoList = new List<Data.TableInfo>();
            foreach (var item in dataBase.TableContext.Tables.OrderBy(item => item.Name))
            {
                if ((item.TableInfo.Tags & tags) != TagInfo.Unused)
                {
                    var tableInfo = item.TableInfo.Filter(tags);
                    tableInfoList.Add(tableInfo);
                }
            }
            return CremaDataSet.GenerateHashValue(tableInfoList.ToArray());
        }

        private IEnumerable<string> GetItems(IDataBase dataBase, TagInfo tags)
        {
            yield return PathUtility.Separator;

            foreach (var item in dataBase.TypeContext)
            {
                if (item is IType type && (type.TypeInfo.Tags & tags) == TagInfo.Unused)
                {
                    continue;
                }
                yield return PathUtility.Separator + CremaSchema.TypeDirectory + item.Path;
            }

            foreach (var item in dataBase.TableContext)
            {
                if (item is ITable table && (table.TableInfo.DerivedTags & tags) == TagInfo.Unused)
                {
                    continue;
                }
                yield return PathUtility.Separator + CremaSchema.TableDirectory + item.Path;
            }
        }
    }
}
