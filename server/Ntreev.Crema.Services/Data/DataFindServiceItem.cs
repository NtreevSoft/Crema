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
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ntreev.Crema.Services.Data
{
    class DataFindServiceItem : DataServiceItemBase
    {
        private readonly CremaDispatcher dispatcher;
        private readonly Authentication authentication;
        private readonly BinaryFormatter formatter = new BinaryFormatter();

        private readonly List<FindResultInfo> findResults = new List<FindResultInfo>();

        public DataFindServiceItem(IDataBase dataBase, CremaDispatcher dispatcher, Authentication authentication)
            : base(dataBase)
        {
            this.dispatcher = dispatcher;
            this.authentication = authentication;
        }

        public FindResultInfo[] FindFromType(string[] itemPaths, string text, FindOptions options)
        {
            this.dispatcher.VerifyAccess();
            this.findResults.Clear();

            foreach (var item in itemPaths)
            {
                if (item.EndsWith(PathUtility.Separator) == true)
                    continue;
                var itemName = new ItemName(item);
                var cacheInfo = (FindTableCacheInfo)this.ReadType(itemName.Name, true);
                this.FindType(item, text, options, cacheInfo, this.findResults);
            }

            return this.findResults.ToArray();
        }

        public FindResultInfo[] FindFromTable(string[] itemPaths, string text, FindOptions options)
        {
            this.dispatcher.VerifyAccess();
            this.findResults.Clear();

            foreach (var item in itemPaths)
            {
                if (item.EndsWith(PathUtility.Separator) == true)
                    continue;

                var itemName = new ItemName(item);
                var cacheInfo = (FindTableCacheInfo)this.ReadTable(itemName.Name, true);
                this.FindTable(item, text, options, cacheInfo, this.findResults);
            }

            return this.findResults.ToArray();
        }

        public override CremaDispatcher Dispatcher => this.dispatcher;

        public override string Name => "find";

        protected override object GetObject(CremaDataTable dataTable)
        {
            return this.BuildCache(dataTable);
        }

        protected override object GetObject(CremaDataType dataType)
        {
            return this.BuildCache(dataType);
        }

        protected override void OnSerializeTable(Stream stream, object tableData)
        {
            this.formatter.Serialize(stream, tableData);
        }

        protected override void OnSerializeType(Stream stream, object typeData)
        {
            this.formatter.Serialize(stream, typeData);
        }

        protected override object OnDeserializeTable(Stream stream)
        {
            return this.formatter.Deserialize(stream);
        }

        protected override object OnDeserializeType(Stream stream)
        {
            return this.formatter.Deserialize(stream);
        }

        protected override Authentication Authentication
        {
            get { return this.authentication; }
        }

        private FindTableCacheInfo BuildCache(CremaDataType dataType)
        {
            var typeCache = new FindTableCacheInfo()
            {
                Columns = new string[] { CremaSchema.Name, CremaSchema.Value, CremaSchema.Comment }
            };
            var rowCaches = new List<FindRowCacheInfo>();
            foreach (var item in dataType.Members)
            {
                var rowCache = new FindRowCacheInfo()
                {
                    Values = new string[] { item.Name, item.Value.ToString(), item.Comment },
                    Tags = TagInfo.All.ToString(),
                    IsEnabled = true,
                    ModificationInfo = item.ModificationInfo
                };
                rowCaches.Add(rowCache);
            }
            typeCache.Rows = rowCaches.ToArray();

            return typeCache;
        }

        private FindTableCacheInfo BuildCache(CremaDataTable dataTable)
        {
            var columns = dataTable.Columns.ToArray();
            var tableCache = new FindTableCacheInfo()
            {
                Columns = columns.Select(i => i.ColumnName).ToArray(),
                Rows = this.BuildRowCache(dataTable, columns)
            };
            return tableCache;
        }

        private FindRowCacheInfo[] BuildRowCache(CremaDataTable table, CremaDataColumn[] columns)
        {
            var rowCaches = new List<FindRowCacheInfo>();

            foreach (var item in table.Rows)
            {
                rowCaches.Add(new FindRowCacheInfo(item, columns));
            }

            return rowCaches.ToArray();
        }

        private void FindType(string itemPath, string text, FindOptions options, FindTableCacheInfo cacheInfo, List<FindResultInfo> results)
        {
            int index = 0;
            foreach (var row in cacheInfo.Rows)
            {
                int c = 0;
                foreach (var value in row.Values)
                {
                    if (value != null && value.IndexOf(text) >= 0)
                    {
                        var resultItem = new FindResultInfo()
                        {
                            Path = itemPath,
                            Row = index,
                            ColumnName = cacheInfo.Columns[c],
                            Value = value,
                            Tags = row.Tags,
                            IsEnabled = row.IsEnabled,
                            ModificationInfo = row.ModificationInfo,
                        };
                        results.Add(resultItem);
                    }
                    c++;
                }
                index++;
            }
        }

        private void FindTable(string itemPath, string text, FindOptions options, FindTableCacheInfo cacheInfo, List<FindResultInfo> results)
        {
            int index = 0;
            foreach (var row in cacheInfo.Rows)
            {
                int c = 0;
                foreach (var value in row.Values)
                {
                    if (value != null && value.IndexOf(text) >= 0)
                    {
                        var resultItem = new FindResultInfo()
                        {
                            Path = itemPath,
                            Row = index,
                            ColumnName = cacheInfo.Columns[c],
                            Value = value,
                            Tags = row.Tags,
                            IsEnabled = row.IsEnabled,
                            ModificationInfo = row.ModificationInfo,
                        };
                        results.Add(resultItem);
                    }
                    c++;
                }
                index++;
            }
        }
    }
}
