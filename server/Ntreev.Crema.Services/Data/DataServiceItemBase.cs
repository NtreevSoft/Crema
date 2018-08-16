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
using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ntreev.Crema.Services.Data
{
    public abstract class DataServiceItemBase : IDisposable
    {
        //private const string jsonExtension = ".json";
        private const string dataDirectory = "data";
        private const string infoDirectory = "info";
        private Guid dataBaseID;
        private readonly ILogService logService;
        private readonly IObjectSerializer serializer;
        private bool initialized;
        private string workingPath;
        private Dictionary<string, TableInfo> tableInfos = new Dictionary<string, TableInfo>();
        private Dictionary<string, TypeInfo> typeInfos = new Dictionary<string, TypeInfo>();
        private Dictionary<string, object> tableDatas = new Dictionary<string, object>();
        private Dictionary<string, object> typeDatas = new Dictionary<string, object>();
        private Dictionary<string, object> tableDomainDatas = new Dictionary<string, object>();
        private Dictionary<string, object> typeDomainDatas = new Dictionary<string, object>();
        private Dictionary<string, IDomain> domainItems = new Dictionary<string, IDomain>();
        private DataServiceItemInfo info;

        protected DataServiceItemBase(IDataBase dataBase)
        {
            this.DataBase = dataBase;
            this.DataBaseName = dataBase.Name;
            this.DataBase.Loaded += DataBase_Loaded;
            this.DataBase.Renamed += DataBase_Renamed;
            this.DataBase.Unloaded += DataBase_Unloaded;
            this.logService = dataBase.GetService(typeof(ILogService)) as ILogService;
            this.serializer = dataBase.GetService(typeof(IObjectSerializer)) as IObjectSerializer;

            if (dataBase is DataBase dataBaseInternal)
            {
                this.NoCache = dataBaseInternal.CremaHost.NoCache;
            }

            if (this.NoCache == false)
                this.ReadInfo();

            if (this.info.Revision != dataBase.DataBaseInfo.Revision)
                this.info = new DataServiceItemInfo();
        }

        public void Dispose()
        {
            DirectoryUtility.Delete(this.BasePath);
        }

        public void Commit()
        {
            if (this.initialized == false)
                return;

            DirectoryUtility.Backup(this.BasePath);

            DirectoryUtility.Prepare(this.BasePath);
            foreach (var item in this.tableInfos)
            {
                var itemPath = Path.Combine(this.BasePath, CremaSchema.TableDirectory, infoDirectory, item.Key);
                this.Serializer.Serialize(itemPath, item.Value, ObjectSerializerSettings.Empty);

            }
            foreach (var item in this.tableDatas)
            {
                var itemPath = Path.Combine(this.BasePath, CremaSchema.TableDirectory, dataDirectory, item.Key);
                this.Serializer.Serialize(itemPath, item.Value, ObjectSerializerSettings.Empty);
            }

            foreach (var item in this.typeInfos)
            {
                var itemPath = Path.Combine(this.BasePath, CremaSchema.TypeDirectory, infoDirectory, item.Key);
                this.Serializer.Serialize(itemPath, item.Value, ObjectSerializerSettings.Empty);
            }
            foreach (var item in this.typeDatas)
            {
                var itemPath = Path.Combine(this.BasePath, CremaSchema.TypeDirectory, dataDirectory, item.Key);
                this.Serializer.Serialize(itemPath, item.Value, ObjectSerializerSettings.Empty);
            }

            var tableInfos = this.tableInfos.Select(item => Path.Combine(CremaSchema.TableDirectory, item.Key));
            var typeInfos = this.typeInfos.Select(item => Path.Combine(CremaSchema.TypeDirectory, item.Key));

            this.info.ItemList = tableInfos.Concat(typeInfos).ToArray();
            this.info.Version = new Version(CremaSchema.MajorVersion, CremaSchema.MinorVersion);
            this.WriteInfo();
            DirectoryUtility.Clean(this.BasePath);
        }

        public void Reset()
        {
            this.DataBase.Dispatcher.Invoke(() =>
            {
                this.DataBase.Loaded -= DataBase_Loaded;
                this.DataBase.Renamed -= DataBase_Renamed;
                this.DataBase.Unloaded -= DataBase_Unloaded;
            });
            var dataSet = this.DataBase.Dispatcher.Invoke(() =>
            {
                if (this.DataBase.IsLoaded == false)
                    this.DataBase.Load(this.Authentication);
                var contains = this.DataBase.Contains(this.Authentication);
                if (contains == false)
                    this.DataBase.Enter(this.Authentication);
                try
                {
                    return this.DataBase.GetDataSet(this.Authentication, DataSetType.All, null, null);
                }
                finally
                {
                    if (contains == false)
                        this.DataBase.Leave(this.Authentication);
                }
            });

            DirectoryUtility.Delete(this.BasePath);
            this.Serialize(dataSet, this.info.Revision);
            this.initialized = true;
            this.DataBase.Dispatcher.Invoke(() =>
            {
                this.DataBase.Loaded += DataBase_Loaded;
                this.DataBase.Renamed += DataBase_Renamed;
                this.DataBase.Unloaded += DataBase_Unloaded;
            });
        }

        public abstract CremaDispatcher Dispatcher { get; }

        public IDataBase DataBase { get; }

        public abstract string Name { get; }

        public string BasePath
        {
            get
            {
                if (this.workingPath == null)
                {
                    if (this.DataBase.GetService(typeof(ICremaHost)) is CremaHost cremaHost)
                    {
                        this.workingPath = cremaHost.GetPath(CremaPath.Caches, this.Name, $"{this.DataBase.ID}");
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                return this.workingPath;
            }
        }

        public string DataBaseName { get; private set; }

        public string Revision => this.info.Revision;

        public bool NoCache { get; private set; }

        public DataServiceItemInfo DataServiceItemInfo => this.info;

        public IObjectSerializer Serializer => this.serializer;

        public event EventHandler Changed;

        protected virtual void OnChanged(EventArgs e)
        {
            this.Changed?.Invoke(this, e);
        }

        protected virtual bool CanSerialize(CremaDataTable dataTable)
        {
            return true;
        }

        protected virtual bool CanSerialize(CremaDataType dataType)
        {
            return true;
        }

        protected abstract object GetObject(CremaDataTable dataTable);

        protected abstract object GetObject(CremaDataType dataType);

        //protected abstract void OnSerializeTable(Stream stream, object tableData);

        //protected abstract void OnSerializeType(Stream stream, object tableData);

        //protected abstract object OnDeserializeTable(Stream stream);

        //protected abstract object OnDeserializeType(Stream stream);

        protected abstract System.Type TableDataType { get; }

        protected abstract System.Type TypeDataType { get; }

        protected object ReadTable(string name, bool isDevmode)
        {
            if (isDevmode == true && this.domainItems.ContainsKey(name) == true)
            {
                var domain = this.domainItems[name];
                if (this.tableDomainDatas.ContainsKey(name) == false)
                    this.Serialize(domain);
                return this.tableDomainDatas[name];
            }

            return this.tableDatas[name];
        }

        protected object ReadType(string name, bool isDevmode)
        {
            return this.typeDatas[name];
        }

        protected TableInfo GetTableInfo(string name)
        {
            return this.tableInfos[name];
        }

        protected TypeInfo GetTypeInfo(string name)
        {
            return this.typeInfos[name];
        }

        protected IEnumerable<string> GetTables()
        {
            return this.tableInfos.Keys;
        }

        protected IEnumerable<string> GetTypes()
        {
            return this.typeInfos.Keys;
        }

        protected abstract Authentication Authentication
        {
            get;
        }

        private void DataBase_Loaded(object sender, EventArgs e)
        {
            this.DataBase.TypeContext.ItemsCreated += TypeContext_ItemCreated;
            this.DataBase.TypeContext.ItemsRenamed += TypeContext_ItemRenamed;
            this.DataBase.TypeContext.ItemsMoved += TypeContext_ItemMoved;
            this.DataBase.TypeContext.ItemsDeleted += TypeContext_ItemDeleted;
            this.DataBase.TypeContext.ItemsChanged += TypeContext_ItemsChanged;

            this.DataBase.TableContext.ItemsCreated += TableContext_ItemCreated;
            this.DataBase.TableContext.ItemsRenamed += TableContext_ItemRenamed;
            this.DataBase.TableContext.ItemsMoved += TableContext_ItemMoved;
            this.DataBase.TableContext.ItemsDeleted += TableContext_ItemDeleted;
            this.DataBase.TableContext.ItemsChanged += TableContext_ItemsChanged;
            this.dataBaseID = this.DataBase.ID;

            if (this.DataBase is DataBase dataBaseInternal)
            {
                this.NoCache = dataBaseInternal.CremaHost.NoCache;
            }

            if (this.NoCache == true)
            {
                this.info.Revision = null;
            }

            var domainContext = this.DataBase.GetService(typeof(IDomainContext)) as IDomainContext;
            domainContext.Domains.DomainCreated += Domains_DomainCreated;
            domainContext.Domains.DomainDeleted += Domains_DomainDeleted;
            domainContext.Domains.DomainRowAdded += Domains_DomainRowAdded;
            domainContext.Domains.DomainRowChanged += Domains_DomainRowChanged;
            domainContext.Domains.DomainRowRemoved += Domains_DomainRowRemoved;

            var domainItems = new Dictionary<string, IDomain>();
            foreach (var item in domainContext.Domains)
            {
                if (item.DataBaseID != this.dataBaseID)
                    continue;
                Collect(item);
            }

            this.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in domainItems)
                {
                    this.domainItems.Add(item.Key, item.Value);
                }
                this.Initialize();
            });

            void Collect(IDomain domain)
            {
                if (domain.Host is TableContent content)
                {
                    var contents = EnumerableUtility.Friends(content, content.Childs);
                    var tableNames = contents.Select(item => item.Table.Name).ToArray();
                    foreach (var item in tableNames)
                    {
                        domainItems.Add(item, domain);
                    }
                }
            }
        }

        private void Domains_DomainRowRemoved(object sender, DomainRowEventArgs e)
        {
            this.DeleteDomainFiles(e.DomainInfo);
        }

        private void Domains_DomainRowChanged(object sender, DomainRowEventArgs e)
        {
            this.DeleteDomainFiles(e.DomainInfo);
        }

        private void Domains_DomainRowAdded(object sender, DomainRowEventArgs e)
        {
            this.DeleteDomainFiles(e.DomainInfo);
        }

        private void DataBase_Renamed(object sender, EventArgs e)
        {
            this.DataBaseName = this.DataBase.Name;
        }

        private void DataBase_Unloaded(object sender, EventArgs e)
        {
            var domainContext = this.DataBase.GetService(typeof(IDomainContext)) as IDomainContext;
            domainContext.Domains.DomainCreated -= Domains_DomainCreated;
            domainContext.Domains.DomainDeleted -= Domains_DomainDeleted;
            this.Dispatcher.InvokeAsync(() =>
            {
                this.domainItems.Clear();
                this.Commit();

                this.tableInfos.Clear();
                this.typeInfos.Clear();
                this.tableDatas.Clear();
                this.typeDatas.Clear();
                this.tableDomainDatas.Clear();
                this.typeDomainDatas.Clear();
                this.initialized = false;
            });
        }

        private void Domains_DomainDeleted(object sender, DomainEventArgs e)
        {
            if (e.DomainInfo.DataBaseID != this.dataBaseID)
                return;

            var domainInfo = e.DomainInfo;
            if (domainInfo.ItemType == nameof(TableContent))
            {
                this.Dispatcher?.InvokeAsync(() =>
                {
                    foreach (var item in this.domainItems.ToArray())
                    {
                        if (item.Value.DataBaseID == domainInfo.DataBaseID)
                        {
                            this.domainItems.Remove(item.Key);
                        }
                    }
                });
            }
        }

        private void Domains_DomainCreated(object sender, DomainEventArgs e)
        {
            var domains = sender as IDomainCollection;
            var domain = domains[e.DomainInfo.DomainID];
            if (domain == null || domain.DataBaseID != this.dataBaseID)
                return;

            if (domain.Host is TableContent content)
            {
                var contents = EnumerableUtility.Friends(content, content.Childs);
                var tableNames = contents.Select(item => item.Table.Name).ToArray();
                this.Dispatcher.InvokeAsync(() =>
                {
                    foreach (var item in tableNames)
                    {
                        this.domainItems.Add(item, domain);
                    }
                });
            }
        }

        private void DeleteDomainFiles(DomainInfo domainInfo)
        {

        }

        private void TypeContext_ItemCreated(object sender, Services.ItemsCreatedEventArgs<ITypeItem> e)
        {
            var dataSet = e.MetaData as CremaDataSet;
            var revision = this.DataBase.DataBaseInfo.Revision;

            this.Dispatcher.InvokeAsync(() => this.Serialize(dataSet, revision));
        }

        private void TypeContext_ItemRenamed(object sender, Services.ItemsRenamedEventArgs<ITypeItem> e)
        {
            var dataSet = e.MetaData as CremaDataSet;
            var revision = this.DataBase.DataBaseInfo.Revision;

            this.Dispatcher.InvokeAsync(() =>
            {
                for (var i = 0; i < e.Items.Length; i++)
                {
                    this.typeDatas.Remove(e.OldNames[i]);
                    this.typeInfos.Remove(e.OldNames[i]);
                }
                this.Serialize(dataSet, revision);
            });
        }

        private void TypeContext_ItemMoved(object sender, Services.ItemsMovedEventArgs<ITypeItem> e)
        {
            var dataSet = e.MetaData as CremaDataSet;
            var revision = this.DataBase.DataBaseInfo.Revision;

            this.Dispatcher.InvokeAsync(() => this.Serialize(dataSet, revision));
        }

        private void TypeContext_ItemDeleted(object sender, Services.ItemsDeletedEventArgs<ITypeItem> e)
        {
            var dataSet = e.MetaData as CremaDataSet;
            var revision = this.DataBase.DataBaseInfo.Revision;

            this.Dispatcher.InvokeAsync(() =>
            {
                for (var i = 0; i < e.Items.Length; i++)
                {
                    this.typeDatas.Remove(e.Items[i].Name);
                    this.typeInfos.Remove(e.Items[i].Name);
                }
                this.Serialize(dataSet, revision);
            });
        }

        private void TypeContext_ItemsChanged(object sender, Services.ItemsEventArgs<ITypeItem> e)
        {
            var dataSet = e.MetaData as CremaDataSet;
            var revision = this.DataBase.DataBaseInfo.Revision;

            this.Dispatcher.InvokeAsync(() => this.Serialize(dataSet, revision));
        }

        private void TableContext_ItemCreated(object sender, Services.ItemsCreatedEventArgs<ITableItem> e)
        {
            var dataSet = e.MetaData as CremaDataSet;
            var revision = this.DataBase.DataBaseInfo.Revision;

            this.Dispatcher.InvokeAsync(() => this.Serialize(dataSet, revision));
        }

        private void TableContext_ItemRenamed(object sender, Services.ItemsRenamedEventArgs<ITableItem> e)
        {
            var dataSet = e.MetaData as CremaDataSet;
            var revision = this.DataBase.DataBaseInfo.Revision;

            this.Dispatcher.InvokeAsync(() =>
            {
                for (var i = 0; i < e.Items.Length; i++)
                {
                    this.tableDatas.Remove(e.OldNames[i]);
                    this.tableInfos.Remove(e.OldNames[i]);
                }
                this.Serialize(dataSet, revision);
            });
        }

        private void TableContext_ItemMoved(object sender, Services.ItemsMovedEventArgs<ITableItem> e)
        {
            var dataSet = e.MetaData as CremaDataSet;
            var revision = this.DataBase.DataBaseInfo.Revision;

            this.Dispatcher.InvokeAsync(() => this.Serialize(dataSet, revision));
        }

        private void TableContext_ItemDeleted(object sender, Services.ItemsDeletedEventArgs<ITableItem> e)
        {
            var dataSet = e.MetaData as CremaDataSet;
            var revision = this.DataBase.DataBaseInfo.Revision;

            this.Dispatcher.InvokeAsync(() =>
            {
                for (var i = 0; i < e.Items.Length; i++)
                {
                    this.tableDatas.Remove(e.Items[i].Name);
                    this.tableInfos.Remove(e.Items[i].Name);
                }
                this.Serialize(dataSet, revision);
            });
        }

        private void TableContext_ItemsChanged(object sender, Services.ItemsEventArgs<ITableItem> e)
        {
            var dataSet = e.MetaData as CremaDataSet;
            var revision = this.DataBase.DataBaseInfo.Revision;

            this.Dispatcher.InvokeAsync(() => this.Serialize(dataSet, revision));
        }

        private void Initialize()
        {
            this.Dispatcher.CheckAccess();
            var error = false;
            try
            {
                if (this.info.Revision != null)
                {
                    foreach (var item in this.info.ItemList)
                    {
                        if (item.StartsWith(CremaSchema.TableDirectory) == true)
                        {
                            var tableName = Path.GetFileName(item);
                            {
                                var itemPath = Path.Combine(this.BasePath, CremaSchema.TableDirectory, infoDirectory, tableName);
                                var tableInfo = (TableInfo)this.Serializer.Deserialize(itemPath, typeof(TableInfo), ObjectSerializerSettings.Empty);
                                this.tableInfos.Add(tableName, tableInfo);
                            }
                            {
                                var itemPath = Path.Combine(this.BasePath, CremaSchema.TableDirectory, dataDirectory, tableName);
                                var tableData = this.Serializer.Deserialize(itemPath, this.TableDataType, ObjectSerializerSettings.Empty);
                                this.tableDatas.Add(tableName, tableData);
                            }
                        }
                        else if (item.StartsWith(CremaSchema.TypeDirectory) == true)
                        {
                            var typeName = Path.GetFileName(item);
                            {
                                var itemPath = Path.Combine(this.BasePath, CremaSchema.TypeDirectory, infoDirectory, typeName);
                                var typeInfo = (TypeInfo)this.Serializer.Deserialize(itemPath, typeof(TypeInfo), ObjectSerializerSettings.Empty);
                                this.typeInfos.Add(typeName, typeInfo);
                            }
                            {
                                var itemPath = Path.Combine(this.BasePath, CremaSchema.TypeDirectory, dataDirectory, typeName);
                                var typeData = this.Serializer.Deserialize(itemPath, this.TypeDataType, ObjectSerializerSettings.Empty);
                                this.typeDatas.Add(typeName, typeData);
                            }
                        }
                    }
                }
                else
                {
                    error = true;
                }
            }
            catch
            {
                error = true;
            }

            if (error == true)
            {
                var result = this.DataBase.Dispatcher.Invoke(() =>
                {
                    var revision = this.DataBase.DataBaseInfo.Revision;
                    var contains = this.DataBase.Contains(this.Authentication);
                    if (contains == false)
                        this.DataBase.Enter(this.Authentication);
                    var dataSet = this.DataBase.GetDataSet(this.Authentication, DataSetType.All, null, null);
                    if (contains == false)
                        this.DataBase.Leave(this.Authentication);
                    return new Tuple<string, CremaDataSet>(revision, dataSet);
                });
                this.Serialize(result.Item2, result.Item1);
            }
            this.initialized = true;
        }

        private void Serialize(CremaDataSet dataSet, string revision)
        {
            if (dataSet == null)
                return;

            this.SerializeTables(dataSet);
            this.SerializeTypes(dataSet);

            this.info.Revision = revision;
            this.info.Version = new Version(CremaSchema.MajorVersion, CremaSchema.MinorVersion);
            this.info.DateTime = DateTime.Now;

            this.OnChanged(EventArgs.Empty);
        }

        private void SerializeTables(CremaDataSet dataSet)
        {
            var tableInfos = new Dictionary<string, TableInfo>(this.tableInfos);
            var tableDatas = new Dictionary<string, object>(this.tableDatas);
            var tables = dataSet.Tables.OrderBy(item => item.Name);

            foreach (var item in tables)
            {
                if (this.CanSerialize(item) == true)
                {
                    tableDatas[item.Name] = this.GetObject(item);
                    tableInfos[item.Name] = item.TableInfo;
                }
                else
                {
                    tableInfos.Remove(item.Name);
                    tableDatas.Remove(item.Name);
                }
            }

            foreach (var item in this.tableInfos.Keys.Except(tableInfos.Keys).ToArray())
            {
                FileUtility.Delete(this.BasePath, item);
                tableInfos.Remove(item);
                tableDatas.Remove(item);
            }

            this.tableInfos = tableInfos;
            this.tableDatas = tableDatas;
        }

        private void SerializeTypes(CremaDataSet dataSet)
        {
            var typeInfos = new Dictionary<string, TypeInfo>(this.typeInfos);
            var typeDatas = new Dictionary<string, object>(this.typeDatas);
            var types = dataSet.Types.OrderBy(item => item.Name);

            foreach (var item in types)
            {
                if (this.CanSerialize(item) == true)
                {
                    typeDatas[item.Name] = this.GetObject(item);
                    typeInfos[item.Name] = item.TypeInfo;
                }
                else
                {
                    typeInfos.Remove(item.Name);
                    typeDatas.Remove(item.Name);
                }
            }

            foreach (var item in this.typeInfos.Keys.Except(typeInfos.Keys).ToArray())
            {
                FileUtility.Delete(this.BasePath, item);
                typeInfos.Remove(item);
                typeDatas.Remove(item);
            }

            this.typeInfos = typeInfos;
            this.typeDatas = typeDatas;
        }

        private void Serialize(IDomain domain)
        {
            this.Dispatcher.VerifyAccess();

            var dataSet = domain.Dispatcher.Invoke(() => (domain.Source as CremaDataSet).Copy());

            foreach (var item in dataSet.Tables.OrderBy(i => i.Name))
            {
                if (this.CanSerialize(item) == true)
                {
                    this.tableDomainDatas[item.Name] = this.GetObject(item);
                }
            }

            foreach (var item in dataSet.Types.OrderBy(i => i.Name))
            {
                if (this.CanSerialize(item) == true)
                {
                    this.typeDomainDatas[item.Name] = this.GetObject(item);
                }
            }
        }

        private void WriteInfo()
        {
            var itemPath = Path.Combine(this.BasePath, "info");
            this.Serializer.Serialize(itemPath, (DataServiceItemSerializationInfo)this.info, ObjectSerializerSettings.Empty);
        }

        private void ReadInfo()
        {
            var itemPath = Path.Combine(this.BasePath, "info");

            if (this.Serializer.Exists(itemPath, typeof(DataServiceItemSerializationInfo), ObjectSerializerSettings.Empty) == true)
            {
                try
                {
                    var value = (DataServiceItemSerializationInfo)this.Serializer.Deserialize(itemPath, typeof(DataServiceItemSerializationInfo), ObjectSerializerSettings.Empty);
                    this.info = (DataServiceItemInfo)value;
                }
                catch (Exception e)
                {
                    this.logService.Error(e);
                }
            }
        }
    }
}
