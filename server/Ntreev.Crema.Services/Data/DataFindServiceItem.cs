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
using Ntreev.Crema.Services.Data.Serializations;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ntreev.Crema.Services.Data
{
    class DataFindServiceItem : DataServiceItemBase
    {
        private readonly CremaDispatcher dispatcher;
        private readonly Authentication authentication;

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
                var cacheInfo = (FindTableSerializationInfo)this.ReadType(itemName.Name, true);
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
                var cacheInfo = (FindTableSerializationInfo)this.ReadTable(itemName.Name, true);
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

        protected override System.Type TableDataType => typeof(FindTableSerializationInfo);

        protected override System.Type TypeDataType => typeof(FindTableSerializationInfo);

        protected override Authentication Authentication
        {
            get { return this.authentication; }
        }

        private FindTableSerializationInfo BuildCache(CremaDataType dataType)
        {
            var typeCache = new FindTableSerializationInfo()
            {
                Columns = new SerializationItemCollection<string>() { CremaSchema.Name, CremaSchema.Value, CremaSchema.Comment }
            };
            var rowCaches = new List<FindRowSerializationInfo>();
            foreach (var item in dataType.Members)
            {
                var rowCache = new FindRowSerializationInfo()
                {
                    Values = new SerializationItemCollection<string>() { item.Name, item.Value.ToString(), item.Comment },
                    Tags = TagInfo.All.ToString(),
                    IsEnabled = true,
                    ModificationInfo = item.ModificationInfo
                };
                rowCaches.Add(rowCache);
            }
            typeCache.Rows = rowCaches.ToArray();

            return typeCache;
        }

        private FindTableSerializationInfo BuildCache(CremaDataTable dataTable)
        {
            var columns = dataTable.Columns.ToArray();
            var tableCache = new FindTableSerializationInfo()
            {
                Columns = new SerializationItemCollection<string>(columns.Select(i => i.ColumnName)),
                Rows = this.BuildRowCache(dataTable, columns)
            };
            return tableCache;
        }

        private FindRowSerializationInfo[] BuildRowCache(CremaDataTable table, CremaDataColumn[] columns)
        {
            var rowCaches = new List<FindRowSerializationInfo>();

            foreach (var item in table.Rows)
            {
                rowCaches.Add(new FindRowSerializationInfo(item, columns));
            }

            return rowCaches.ToArray();
        }

        private void FindType(string itemPath, string text, FindOptions options, FindTableSerializationInfo cacheInfo, List<FindResultInfo> results)
        {
            var index = 0;
            foreach (var row in cacheInfo.Rows)
            {
                var c = 0;
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

        private void FindTable(string itemPath, string text, FindOptions options, FindTableSerializationInfo cacheInfo, List<FindResultInfo> results)
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
