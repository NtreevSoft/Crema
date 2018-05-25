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

using Ntreev.Crema.Runtime.Serialization.Binary;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Services;
using Ntreev.Crema.Data;
using System.Runtime.Serialization.Formatters.Binary;
using Ntreev.Library.IO;
using System.Threading;
using Ntreev.Library;
using Ntreev.Crema.Services.Data;
using Ntreev.Crema.Runtime.Serialization;
using Ntreev.Crema.Runtime.Generation;
using Ntreev.Library.Serialization;

namespace Ntreev.Crema.RuntimeService
{
    class RuntimeServiceItem : DataServiceItemBase
    {
        private readonly Dictionary<string, SerializationSet> caches = new Dictionary<string, SerializationSet>();
        private readonly CremaDispatcher dispatcher;
        private readonly Authentication authentication;
        private readonly BinaryFormatter formatter = new BinaryFormatter();
        private ILogService logService;

        private readonly Dictionary<long, CremaDataSet> cachesByRevison = new Dictionary<long, CremaDataSet>();

        public RuntimeServiceItem(IDataBase dataBase, CremaDispatcher dispatcher, Authentication authentication)
            : base(dataBase)
        {
            this.dispatcher = dispatcher;
            this.authentication = authentication;
            this.logService = dataBase.GetService(typeof(ILogService)) as ILogService;
        }

        public GenerationSet Gerneration(TagInfo tags, string filterExpression, bool isDevmode, string revision)
        {
            this.Dispatcher.VerifyAccess();

            if (filterExpression == null)
                throw new ArgumentNullException(nameof(filterExpression));

            if (revision == null)
            {
                var tables = this.GetTables().Select(item => this.GetTableInfo(item))
                                             .ToArray();
                var types = this.GetTypes().Select(item => this.GetTypeInfo(item))
                                           .ToArray();

                var codeSet = new GenerationSet(types, tables)
                {
                    Name = this.DataBaseName,
                    Revision = this.Revision,
                };

                codeSet = codeSet.Filter(tags);
                if (filterExpression != string.Empty)
                    codeSet = codeSet.Filter(filterExpression);
                return codeSet;
            }
            else
            {
                var dataSet = this.DataBase.GetDataSet(this.authentication, revision);
                var tables = dataSet.Tables.Select(item => item.TableInfo).ToArray();
                var types = dataSet.Types.Select(item => item.TypeInfo).ToArray();
                var codeSet = new GenerationSet(types, tables)
                {
                    Name = this.DataBaseName,
                    Revision = this.Revision,
                };
                codeSet = codeSet.Filter(tags);
                if (filterExpression != string.Empty)
                    codeSet = codeSet.Filter(filterExpression);
                return codeSet;
            }
        }

        public SerializationSet Serialize(TagInfo tags, string filterExpression, bool isDevmode, string revision)
        {
            this.Dispatcher.VerifyAccess();

            if (filterExpression == null)
                throw new ArgumentNullException(nameof(filterExpression));

            if (revision == null)
            {
                var cacheKey = tags.ToString() + filterExpression;

                if (isDevmode == false && this.caches.ContainsKey(cacheKey) == true)
                {
                    return this.caches[cacheKey];
                }

                var dataSet = new SerializationSet()
                {
                    Name = this.DataBaseName,
                    Revision = this.Revision,
                };

                var tableItems = this.ReadTables(isDevmode);
                dataSet.Tables = tableItems.Cast<SerializationTable>().ToArray();

                var typeItems = this.ReadTypes(isDevmode);
                dataSet.Types = typeItems.Cast<SerializationType>().ToArray();

                dataSet = dataSet.Filter(tags);
                if (filterExpression != string.Empty)
                    dataSet = dataSet.Filter(filterExpression);

                if (isDevmode == false)
                {
                    this.caches[cacheKey] = dataSet;
                }

                return dataSet;
            }
            else
            {
                var dataSet = this.DataBase.GetDataSet(this.authentication, revision);
                var serializedSet = new SerializationSet(dataSet)
                {
                    Name = this.DataBaseName,
                    Revision = this.Revision,
                };
                serializedSet = serializedSet.Filter(tags);
                if (filterExpression != string.Empty)
                    serializedSet = serializedSet.Filter(filterExpression);
                return serializedSet;
            }
        }

        public override CremaDispatcher Dispatcher
        {
            get { return this.dispatcher; }
        }

        public override string Name
        {
            get { return "serialization"; }
        }

        protected override bool CanSerialize(CremaDataTable dataTable)
        {
            return dataTable.DerivedTags != TagInfo.Unused;
        }

        protected override object GetObject(CremaDataTable dataTable)
        {
            return new SerializationTable(dataTable);
        }

        protected override object GetObject(CremaDataType dataType)
        {
            return new SerializationType(dataType);
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

        protected override void OnChanged(EventArgs e)
        {
            base.OnChanged(e);
            this.caches.Clear();
        }

        protected override Authentication Authentication
        {
            get { return this.authentication; }
        }

        private object[] ReadTables(bool isDevmode)
        {
            var tableNames = this.GetTables().ToArray();
            var items = new List<object>(tableNames.Length);
            for (var i = 0; i < tableNames.Length; i++)
            {
                var tableName = tableNames[i];
                items.Add(this.ReadTable(tableName, isDevmode));
            }
            return items.ToArray();
        }

        private object[] ReadTypes(bool isDevmode)
        {
            var typeNames = this.GetTypes().ToArray();
            var items = new List<object>(typeNames.Length);
            for (var i = 0; i < typeNames.Length; i++)
            {
                var typeName = typeNames[i];
                items.Add(this.ReadType(typeName, isDevmode));
            }
            return items.ToArray();
        }

        private bool FilterTable(string tableName, string filterExpression)
        {
            if (string.IsNullOrEmpty(filterExpression) == true)
                return true;

            return tableName.GlobMany(filterExpression);
        }

        private bool FilterType(string typeName, string filterExpression, SerializationTable[] tables)
        {
            if (string.IsNullOrEmpty(filterExpression) == true)
                return true;

            var query = from table in tables
                        from column in table.Columns
                        where NameValidator.VerifyItemPath(column.DataType)
                        let itemName = new ItemName(column.DataType)
                        where itemName.Name == typeName
                        select table;

            return query.Any();
        }

        private bool FilterType(string typeName, string filterExpression, TableInfo[] tables)
        {
            if (string.IsNullOrEmpty(filterExpression) == true)
                return true;

            var query = from table in tables
                        from column in table.Columns
                        where NameValidator.VerifyItemPath(column.DataType)
                        let itemName = new ItemName(column.DataType)
                        where itemName.Name == typeName
                        select table;

            return query.Any();
        }
    }
}
