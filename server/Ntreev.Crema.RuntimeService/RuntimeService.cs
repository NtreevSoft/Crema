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

using Ntreev.Crema.Runtime.Generation;
using Ntreev.Crema.Runtime.Serialization;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.ServiceHosts;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.RuntimeService
{
    [Export(typeof(IPlugin))]
    [Export(typeof(IRuntimeService))]
    [Export(typeof(RuntimeService))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    class RuntimeService : IPlugin, IRuntimeService
    {
        public const string ServiceID = "B8CD9F7C-58B8-4BDA-B6AE-B99ED216DA22";
        private readonly ICremaHost cremaHost;
        private readonly IEnumerable<Runtime.Serialization.IDataSerializer> serializers;
        private readonly Dictionary<Guid, RuntimeServiceItem> items = new Dictionary<Guid, RuntimeServiceItem>();
        private CremaDispatcher dispatcher;
        private Authentication authentication;

        [ImportingConstructor]
        public RuntimeService(ICremaHost cremaHost, [ImportMany]IEnumerable<Runtime.Serialization.IDataSerializer> serializers)
        {
            this.cremaHost = cremaHost;
            this.serializers = serializers;
            this.dispatcher = new CremaDispatcher(this);
            this.cremaHost.Opened += CremaHost_Opened;
            this.cremaHost.Closed += CremaHost_Closed;
            this.cremaHost.Disposed += (s, e) => this.dispatcher.Dispose();
        }

        public string Name
        {
            get { return nameof(RuntimeService); }
        }

        public Guid ID
        {
            get { return Guid.Parse(ServiceID); }
        }

        public void Initialize(Authentication authentication)
        {
            this.authentication = authentication;

            foreach (var item in this.cremaHost.DataBases)
            {
                var obj = new RuntimeServiceItem(item, this.dispatcher, authentication);
                this.items.Add(item.ID, obj);
            }
        }

        public void Release()
        {

        }

        public RuntimeServiceItem GetServiceItem(Guid dataBaseID)
        {
            if (this.items.ContainsKey(dataBaseID) == false)
                return null;
            return this.items[dataBaseID];
        }

        public IDataSerializer GetSerializer(string type)
        {
            return this.serializers.FirstOrDefault(item => item.Name == type);
        }

        public ResultBase<GenerationSet> GetCodeGenerationData(string dataBaseName, string tags, string filterExpression, string revision)
        {
            var result = new ResultBase<GenerationSet>();
            try
            {
                using (var dataBaseItem = UsingDataBase.Set(this.cremaHost, dataBaseName, this.authentication, true))
                {
                    var dataBaseID = dataBaseItem.DataBase.ID;
                    var metaSet = this.dispatcher.Invoke(() =>
                    {
                        var project = this.GetServiceItem(dataBaseID);
                        var tagInfo = new TagInfo(tags);
                        return project.Gerneration(tagInfo, filterExpression, false, revision);
                    });

                    result.Value = metaSet;
                }
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault(e);
            }

            return result;
        }

        public ResultBase<SerializationSet> GetDataGenerationData(string dataBaseName, string tags, string filterExpression, bool isDevmode, string revision)
        {
            var result = new ResultBase<SerializationSet>();
            try
            {
                using (var dataBaseItem = UsingDataBase.Set(this.cremaHost, dataBaseName, this.authentication, true))
                {
                    var dataBaseID = dataBaseItem.DataBase.ID;
                    var metaSet = this.dispatcher.Invoke(() =>
                    {
                        var project = this.GetServiceItem(dataBaseID);
                        var tagInfo = (TagInfo)tags;
                        return project.Serialize(tagInfo, filterExpression, isDevmode, revision);
                    });

                    result.Value = metaSet;
                }
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault(e);
            }

            return result;
        }

        public ResultBase<GenerationSet, SerializationSet> GetMetaData(string dataBaseName, string tags, string filterExpression, bool isDevmode, string revision)
        {
            var result = new ResultBase<GenerationSet, SerializationSet>();
            try
            {
                using (var dataBaseItem = UsingDataBase.Set(this.cremaHost, dataBaseName, this.authentication, true))
                {
                    var dataBaseID = dataBaseItem.DataBase.ID;
                    var metaSet = this.dispatcher.Invoke(() =>
                    {
                        var project = this.GetServiceItem(dataBaseID);
                        var tagInfo = (TagInfo)tags;
                        var codeMetaData = project.Gerneration(tagInfo, filterExpression, isDevmode, revision);
                        var dataMetaData = project.Serialize(tagInfo, filterExpression, isDevmode, revision);
                        return new Tuple<GenerationSet, SerializationSet>(codeMetaData, dataMetaData);
                    });

                    result.Value1 = metaSet.Item1;
                    result.Value2 = metaSet.Item2;
                }
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault(e);
            }

            return result;
        }

        public ResultBase ResetData(string dataBaseName)
        {
            var result = new ResultBase();

            try
            {
                using (var dataBaseItem = UsingDataBase.Set(this.cremaHost, dataBaseName, this.authentication, true))
                {
                    var dataBaseID = dataBaseItem.DataBase.ID;
                    this.dispatcher.Invoke(() =>
                    {
                        var project = this.GetServiceItem(dataBaseID);
                        project.Reset();
                    });
                }
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault(e);
            }

            return result;
        }

        public ResultBase<string> GetRevision(string dataBaseName)
        {
            var result = new ResultBase<string>();

            try
            {
                using (var dataBaseItem = UsingDataBase.Set(this.cremaHost, dataBaseName, this.authentication, true))
                {
                    var dataBase = dataBaseItem.DataBase;
                    result.Value = dataBase.Dispatcher.Invoke(() => dataBase.DataBaseInfo.Revision);
                }
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault(e);
            }

            return result;
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.dispatcher; }
        }

        private void DataBases_ItemCreated(object sender, ItemsCreatedEventArgs<IDataBase> e)
        {
            foreach (var item in e.Items)
            {
                var obj = new RuntimeServiceItem(item, this.dispatcher, authentication);
                this.items.Add(item.ID, obj);
            }
        }

        private void DataBases_ItemDeleted(object sender, ItemsDeletedEventArgs<IDataBase> e)
        {
            foreach (var item in e.Items)
            {
                var obj = this.items[item.ID];
                obj.Dispose();
                this.items.Remove(item.ID);
            }
        }

        private void CremaHost_Opened(object sender, EventArgs e)
        {
            this.cremaHost.DataBases.ItemsCreated += DataBases_ItemCreated;
            this.cremaHost.DataBases.ItemsDeleted += DataBases_ItemDeleted;
        }

        private void CremaHost_Closed(object sender, EventArgs e)
        {
            foreach (var item in this.items)
            {
                item.Value.Commit();
            }
            this.items.Clear();
        }

        private void Filter(ref DataBaseMetaData metaData, TagInfo tags, string filterExpression)
        {
            var tableList = new List<TableMetaData>();
            for (var i = 0; i < metaData.Tables.Length; i++)
            {
                var table = metaData.Tables[i];

                if (StringUtility.GlobMany(table.TableInfo.Name, filterExpression) == false)
                    continue;

                if ((table.TableInfo.DerivedTags & tags) != tags)
                    continue;

                var tableInfo = table.TableInfo;
                tableInfo.Columns = tableInfo.Columns.Where(item => (item.DerivedTags & tags) == tags).ToArray();
                table.TableInfo = tableInfo;

                tableList.Add(table);
            }

            metaData.Tables = tableList.ToArray();
        }
    }
}
