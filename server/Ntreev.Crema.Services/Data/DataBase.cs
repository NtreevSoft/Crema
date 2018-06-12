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
using Ntreev.Crema.Services.Domains;
using Ntreev.Crema.Services.Properties;
using Ntreev.Crema.Services.Users;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;

namespace Ntreev.Crema.Services.Data
{
    class DataBase : DataBaseBase<Type, TypeCategory, TypeCollection, TypeCategoryCollection, TypeContext, Table, TableCategory, TableCollection, TableCategoryCollection, TableContext>,
        IDataBase, IInfoProvider, IStateProvider
    {
        public static readonly Guid defaultID = Guid.Parse("9CAC6195-1F14-40D6-B56E-4747A67F5C8A");
        public const string defaultName = "default";

        private const string dataExtension = ".data";
        private readonly IRepositoryProvider repositoryProvider;
        private readonly IObjectSerializer serializer;
        private readonly string cachePath;
        private TypeContext typeContext;
        private TableContext tableContext;
        private UserContext userContext;
        private CremaDataSet dataSetCache;
        private DataBaseMetaData? metaData;

        private EventHandler<AuthenticationEventArgs> authenticationEntered;
        private EventHandler<AuthenticationEventArgs> authenticationLeft;

        private HashSet<AuthenticationToken> authentications = new HashSet<AuthenticationToken>();

        public DataBase(CremaHost cremaHost, string name)
        {
            this.CremaHost = cremaHost;
            this.repositoryProvider = cremaHost.RepositoryProvider;
            this.serializer = cremaHost.Serializer;
            this.Dispatcher = cremaHost.Dispatcher;
            base.Name = name;
            this.cachePath = cremaHost.GetPath(CremaPath.Caches, "databases");
            this.userContext = this.CremaHost.UserContext;
            this.userContext.Dispatcher.Invoke(() => this.userContext.Users.UsersLoggedOut += Users_UsersLoggedOut);
        }

        public DataBase(CremaHost cremaHost, string name, DataBaseSerializationInfo dataBaseInfo)
            : this(cremaHost, name)
        {
            base.DataBaseInfo = (DataBaseInfo)dataBaseInfo;
        }

        public override string ToString()
        {
            return base.Name;
        }

        public AccessType GetAccessType(Authentication authentication)
        {
            this.ValidateBeginInDataBase(authentication);
            return base.GetAccessType(authentication);
        }

        public void SetPublic(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(SetPublic), this);
            base.ValidateSetPublic(authentication);
            this.Sign(authentication);
            this.InvokeDataBaseSetPublic(authentication);
            base.SetPublic(authentication);
            this.metaData = null;
            this.DataBases.InvokeItemsSetPublicEvent(authentication, this.BasePath, new IDataBase[] { this });
        }

        public void SetPrivate(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(SetPrivate), this);
            base.ValidateSetPrivate(authentication);
            this.Sign(authentication);
            this.InvokeDataBaseSetPrivate(authentication);
            base.SetPrivate(authentication);
            this.metaData = null;
            this.DataBases.InvokeItemsSetPrivateEvent(authentication, this.BasePath, new IDataBase[] { this });
        }

        public void AddAccessMember(Authentication authentication, string memberID, AccessType accessType)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(AddAccessMember), this, memberID, accessType);
            base.ValidateAddAccessMember(authentication, memberID, accessType);
            this.Sign(authentication);
            this.InvokeDataBaseAddAccessMember(authentication, memberID, accessType);
            base.AddAccessMember(authentication, memberID, accessType);
            this.metaData = null;
            this.DataBases.InvokeItemsAddAccessMemberEvent(authentication, this.BasePath, new IDataBase[] { this }, new string[] { memberID, }, new AccessType[] { accessType, });
        }

        public void SetAccessMember(Authentication authentication, string memberID, AccessType accessType)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(SetAccessMember), this, memberID, accessType);
            base.ValidateSetAccessMember(authentication, memberID, accessType);
            this.InvokeDataBaseSetAccessMember(authentication, memberID, accessType);
            base.SetAccessMember(authentication, memberID, accessType);
            this.Sign(authentication);
            this.metaData = null;
            this.DataBases.InvokeItemsSetAccessMemberEvent(authentication, this.BasePath, new IDataBase[] { this }, new string[] { memberID, }, new AccessType[] { accessType, });
        }

        public void RemoveAccessMember(Authentication authentication, string memberID)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(RemoveAccessMember), this, memberID);
            base.ValidateRemoveAccessMember(authentication, memberID);
            this.InvokeDataBaseRemoveAccessMember(authentication, memberID);
            base.RemoveAccessMember(authentication, memberID);
            this.Sign(authentication);
            this.metaData = null;
            this.DataBases.InvokeItemsRemoveAccessMemberEvent(authentication, this.BasePath, new IDataBase[] { this }, new string[] { memberID, });
        }

        public void Lock(Authentication authentication, string comment)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(Lock), this, comment);
            base.ValidateLock(authentication);
            this.DataBases.InvokeDataBaseLock(authentication, this, comment);
            base.Lock(authentication, comment);
            this.Sign(authentication);
            this.metaData = null;
            this.DataBases.InvokeItemsLockedEvent(authentication, new IDataBase[] { this }, new string[] { comment, });
        }

        public void Unlock(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(Unlock), this);
            base.ValidateUnlock(authentication);
            this.DataBases.InvokeDataBaseUnlock(authentication, this);
            base.Unlock(authentication);
            this.Sign(authentication);
            this.metaData = null;
            this.DataBases.InvokeItemsUnlockedEvent(authentication, new IDataBase[] { this });
        }

        public void Load(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(Load), this);
            this.ValidateLoad(authentication);
            this.DataBases.InvokeDataBaseLoad(authentication, this);
            this.Repository = new DataBaseRepositoryHost(this, this.repositoryProvider.CreateInstance(new RepositorySettings()
            {
                BasePath = this.DataBases.RemotePath,
                RepositoryName = this.Name,
                WorkingPath = this.BasePath,
                LogService = this.CremaHost,
            }));
            this.Repository.Changed += Repository_Changed;
            this.ReadCache();
            this.AttachDomainHost();
            this.metaData = null;
            base.DataBaseState = DataBaseState.IsLoaded;
            base.Load(authentication);
            base.UpdateLockParent();
            base.UpdateAccessParent();
            this.Sign(authentication);
            this.DataBases.InvokeItemsLoadedEvent(authentication, new IDataBase[] { this });
        }

        public void Unload(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(Unload), this);
            this.ValidateUnload(authentication);
            this.DataBases.InvokeDataBaseUnload(authentication, this);
            this.DetachDomainHost();
            this.WriteCache();
            this.tableContext.Dispose();
            this.tableContext = null;
            this.typeContext.Dispose();
            this.typeContext = null;
            this.metaData = null;
            this.DetachUsers();
            this.Repository.Changed -= Repository_Changed;
            this.Repository.Dispose();
            this.Repository = null;
            base.DataBaseState = DataBaseState.None;
            base.Unload(authentication);
            this.Sign(authentication);
            this.DataBases.InvokeItemsUnloadedEvent(authentication, new IDataBase[] { this });
        }

        public void Enter(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.ValidateEnter(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Enter), this);
            this.OnEnter(authentication);
            this.DataBases.InvokeItemsAuthenticationEnteredEvent(authentication, new IDataBase[] { this });
        }

        public void Leave(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.ValidateLeave(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Leave), this);
            this.OnLeave(authentication);
            this.DataBases.InvokeItemsAuthenticationLeftEvent(authentication, new IDataBase[] { this });
        }

        public void Rename(Authentication authentication, string name)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(Rename), this, name);
            this.DataBases.InvokeDataBaseRename(authentication, this, name);
            var oldName = base.Name;
            base.Name = name;
            this.Sign(authentication);
            this.DataBases.InvokeItemsRenamedEvent(authentication, new DataBase[] { this }, new string[] { oldName, });
        }

        public void Delete(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.CremaHost.DebugMethod(authentication, this, nameof(Delete), this);
            base.ValidateDelete(authentication);
            this.DataBases.InvokeDataBaseDelete(authentication, this);
            base.DataBaseState = DataBaseState.None;
            FileUtility.Delete(this.cachePath, this.ID.ToString() + dataExtension);
            this.Dispatcher = null;
            this.tableContext = null;
            this.typeContext = null;
            this.Sign(authentication);
            this.DataBases.InvokeItemsDeletedEvent(authentication, new DataBase[] { this }, new string[] { base.Name, });
        }

        public bool Contains(Authentication authentication)
        {
            return this.authentications.Contains(authentication);
        }

        public LogInfo[] GetLog(Authentication authentication)
        {
            this.ValidateDispatcher();

            var remotePath = this.CremaHost.GetPath(CremaPath.RemoteDataBases);
            var logs = this.repositoryProvider.GetLog(remotePath, this.Name, 100);
            return logs.OrderByDescending(item => item.Revision).Take(100).ToArray();
        }

        public void Revert(Authentication authentication, string revision)
        {
            this.ValidateDispatcher();
            this.ValidateRevert(authentication, revision);

            this.Repository.Revert(revision);
            try
            {
                var eventLog = EventLogBuilder.Build(authentication, this, nameof(Revert), this, revision);
                var comment = EventMessageBuilder.RevertDataBase(authentication, this, revision);
                var dataBaseInfo = base.DataBaseInfo;
                this.CremaHost.Debug(eventLog);
                this.Repository.Commit(authentication, comment);
                this.CremaHost.Info(comment);
                dataBaseInfo.Revision = this.Repository.RepositoryInfo.Revision;
                base.DataBaseInfo = dataBaseInfo;
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public DataBaseTransaction BeginTransaction(Authentication authentication)
        {
            this.ValidateDispatcher();
            this.ValidateBeginTransaction(authentication);
            this.Sign(authentication);
            if (this.IsLocked == false)
            {
                this.Lock(authentication, $"{this.ID}");
            }
            var transaction = new DataBaseTransaction(authentication, this, this.Repository);
            transaction.Disposed += (s, e) =>
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    if (this.LockInfo.Comment == $"{this.ID}" && this.IsLocked == true)
                    {
                        this.Unlock(authentication);
                    }
                });
            };
            return transaction;
        }

        public void ValidateBeginInDataBase(Authentication authentication)
        {
            this.ValidateDispatcher();
            authentication.Verify();
            if (authentication != Authentication.System && this.authentications.Contains(authentication) == false)
                throw new InvalidOperationException(Resources.Exception_NotInDataBase);
        }

        public void ValidateAsyncBeginInDataBase(Authentication authentication)
        {
            if (this.Dispatcher == null)
                throw new InvalidOperationException(Resources.Exception_InvalidObject);
            this.Dispatcher.Invoke(() =>
            {
                if (authentication != Authentication.System && this.authentications.Contains(authentication) == false)
                    throw new InvalidOperationException(Resources.Exception_NotInDataBase);
            });
        }

        public bool VerifyAccess(Authentication authentication)
        {
            return this.authentications.Contains(authentication);
        }

        public void ValidateDirectoryInfo(string path)
        {
            new DirectoryInfo(path);
        }

        public void ValidateFileInfo(string path)
        {
            new FileInfo(path);
        }

        public DataBaseMetaData GetMetaData(Authentication authentication)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            this.ValidateDispatcher();

            if (this.metaData == null)
            {
                var metaData = new DataBaseMetaData()
                {
                    DataBaseInfo = this.DataBaseInfo,
                    DataBaseState = this.DataBaseState,
                    AccessInfo = this.AccessInfo,
                    LockInfo = this.LockInfo,
                    Authentications = this.AuthenticationInfos
                };
                if (this.tableContext != null)
                {
                    var query = from TableCategory item in this.tableContext.Categories
                                orderby item.Path
                                select item.MetaData;

                    metaData.TableCategories = query.ToArray();
                }
                if (this.tableContext != null)
                {
                    var query = from Table item in this.tableContext.Tables
                                orderby item.Path
                                orderby item.TemplatedParent != null
                                select item.MetaData;

                    metaData.Tables = query.ToArray();
                }
                if (this.typeContext != null)
                {
                    var query = from TypeCategory item in this.typeContext.Categories
                                orderby item.Path
                                select item.MetaData;

                    metaData.TypeCategories = query.ToArray();
                }
                if (this.typeContext != null)
                {
                    var query = from Type item in this.typeContext.Types
                                orderby item.Path
                                select item.MetaData;

                    metaData.Types = query.ToArray();
                }
                this.metaData = metaData;
            }

            return this.metaData.Value;
        }

        public void Dispose()
        {
            if (this.IsLoaded == true)
                this.WriteCache();
            this.Dispatcher = null;
            base.DataBaseState = DataBaseState.None;
            this.tableContext = null;
            this.typeContext = null;
        }

        [Obsolete]
        public void DeleteInternal(Authentication authentication)
        {
            FileUtility.Delete(this.cachePath, this.ID.ToString() + dataExtension);
            this.Dispatcher = null;
            base.DataBaseState = DataBaseState.None;
            this.tableContext = null;
            this.typeContext = null;
            this.Delete(authentication);
        }

        public void ResettingDataBase(Authentication authentication)
        {
            if (this.GetService(typeof(DomainContext)) is DomainContext domainContext)
            {
                Trace.WriteLine("resetting");
                if (this.IsLoaded == true)
                    this.DetachDomainHost();

                var domains = domainContext.Domains.Where<Domain>(item => item.DataBaseID == this.ID).ToArray();
                foreach (var item in domains)
                {
                    item.Dispatcher.Invoke(() => item.Delete(authentication, true));
                }

                this.typeContext?.Dispose();
                this.tableContext?.Dispose();
                this.DataBases.InvokeDataBaseResetting(authentication, this);
                base.ResettingDataBase(authentication);
                this.DataBases.InvokeItemsResettingEvent(authentication, new IDataBase[] { this });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void ResetDataBase(Authentication authentication, IEnumerable<TypeInfo> typeInfos, IEnumerable<TableInfo> tableInfos)
        {
            this.DataBases.InvokeDataBaseReset(authentication, this);

            this.typeContext = new TypeContext(this, typeInfos);
            this.typeContext.ItemsLockChanged += (s, e) => this.metaData = null;
            this.typeContext.ItemsAccessChanged += (s, e) => this.metaData = null;
            this.tableContext = new TableContext(this, tableInfos);
            this.tableContext.ItemsLockChanged += (s, e) => this.metaData = null;
            this.tableContext.ItemsAccessChanged += (s, e) => this.metaData = null;
            base.ResetDataBase(authentication);
            base.UpdateLockParent();
            base.UpdateAccessParent();
            if (this.IsLoaded == true)
            {
                this.AttachDomainHost();
                foreach (var item in this.authentications)
                {
                    this.AttachUsers(authentication);
                }
            }
            var metaDataList = new List<DomainMetaData>();
            foreach (var item in this.CremaHost.DomainContext.Domains)
            {
                if (item.DomainInfo.DataBaseID == this.ID)
                {
                    var metaData = item.Dispatcher.Invoke(() => item.GetMetaData(authentication));
                    metaDataList.Add(metaData);
                }
            }
            Trace.WriteLine("reset");
            this.DataBases.InvokeItemsResetEvent(authentication, new IDataBase[] { this }, metaDataList.ToArray());
        }

        public IDomainHost FindDomainHost(Domain domain)
        {
            var domainInfo = domain.Dispatcher.Invoke(() => domain.DomainInfo);
            var itemPath = domainInfo.ItemPath;
            var itemType = domainInfo.ItemType;

            if (itemType == nameof(TableContent))
            {
                var table = this.tableContext[itemPath] as Table;
                return table.Content;
            }
            else if (itemType == nameof(NewTableTemplate))
            {
                var category = this.tableContext[itemPath] as TableCategory;
                return new NewTableTemplate(category);
            }
            else if (itemType == nameof(NewChildTableTemplate))
            {
                var table = this.tableContext[itemPath] as Table;
                return new NewChildTableTemplate(table);
            }
            else if (itemType == nameof(TableTemplate))
            {
                var table = this.tableContext[itemPath] as Table;
                return table.Template;
            }
            else if (itemType == nameof(NewTypeTemplate))
            {
                var category = this.typeContext[itemPath] as TypeCategory;
                return new NewTypeTemplate(category);
            }
            else if (itemType == nameof(TypeTemplate))
            {
                var type = this.typeContext[itemPath] as Type;
                return type.Template;
            }

            return null;
        }

        public object GetService(System.Type serviceType)
        {
            if (serviceType == typeof(TableContext))
                return this.tableContext;
            else if (serviceType == typeof(TableCategoryCollection))
                return this.tableContext.Categories;
            else if (serviceType == typeof(TableCollection))
                return this.tableContext.Tables;
            else if (serviceType == typeof(TypeContext))
                return this.typeContext;
            else if (serviceType == typeof(TypeCategoryCollection))
                return this.typeContext.Categories;
            else if (serviceType == typeof(TypeCollection))
                return this.typeContext.Types;
            else if (serviceType == typeof(DomainContext))
                return this.CremaHost.DomainContext;
            else
                return this.CremaHost.GetService(serviceType);
        }

        public string BasePath => this.CremaHost.GetPath(CremaPath.Working, "databases", $"{base.DataBaseInfo.ID}");

        public CremaDataSet GetDataSet(Authentication authentication, string revision)
        {
            this.Dispatcher.Invoke(() => this.ValidatePreview(authentication));

            if (revision == base.DataBaseInfo.Revision || revision == null)
            {
                if (this.dataSetCache == null)
                {
                    this.dataSetCache = this.Dispatcher.Invoke(() => CremaDataSet.ReadFromDirectory(this.BasePath));
                }
                return this.dataSetCache;
            }
            else
            {
                var tempPath = PathUtility.GetTempPath(false);
                var uri = this.Repository.GetUri(this.BasePath, revision);
                try
                {
                    var exportPath = this.Repository.Export(uri, tempPath);
                    return CremaDataSet.ReadFromDirectory(exportPath);
                }
                finally
                {
                    DirectoryUtility.Delete(tempPath);
                }
            }
        }

        public CremaDataSet GetDataSet(Authentication authentication, IEnumerable<Table> tables, bool schemaOnly)
        {
            var typePaths = tables.SelectMany(item => item.GetTypes())
                                  .Select(item => item.LocalPath)
                                  .Distinct()
                                  .ToArray();
            var tablePaths = tables.SelectMany(item => EnumerableUtility.Friends(item, item.DerivedTables))
                                   .Select(item => item.Parent ?? item)
                                   .Select(item => item.LocalPath)
                                   .Distinct()
                                   .ToArray();

            var props = new CremaDataSetPropertyCollection(authentication, typePaths, tablePaths)
            {
                SchemaOnly = schemaOnly,
            };
            return this.Serializer.Deserialize(this.BasePath, typeof(CremaDataSet), props) as CremaDataSet;
        }

        public CremaHost CremaHost { get; }

        public DataBaseCollection DataBases
        {
            get { return this.CremaHost.DataBases; }
        }

        public TableContext TableContext
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.tableContext;
            }
        }

        public TypeContext TypeContext
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.typeContext;
            }
        }

        public CremaDispatcher Dispatcher { get; private set; }

        public IObjectSerializer Serializer => this.serializer;

        public DataBaseRepositoryHost Repository { get; private set; }

        public new DataBaseInfo DataBaseInfo
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                if (base.DataBaseInfo.Paths == null)
                {
                    var remotePath = this.CremaHost.GetPath(CremaPath.RemoteDataBases);
                    var itemList = this.repositoryProvider.GetRepositoryItemList(remotePath, this.Name);
                    var categories = from item in itemList
                                     where item.EndsWith(PathUtility.Separator) == true
                                     select item;
                    var items = from item in itemList
                                where item.EndsWith(PathUtility.Separator) == false
                                where item.StartsWith(PathUtility.Separator + CremaSchema.TypeDirectory + PathUtility.Separator)
                                      || item.StartsWith(PathUtility.Separator + CremaSchema.TableDirectory + PathUtility.Separator)
                                select FileUtility.RemoveExtension(item);

                    var allItems = items.Distinct()
                                        .Concat(categories)
                                        .OrderBy(item => item)
                                        .ToArray();

                    var dataBaseInfo = base.DataBaseInfo;
                    //dataBaseInfo.Paths = EnumerableUtility.Friends(PathUtility.Separator, allItems).Distinct().ToArray();
                    dataBaseInfo.Paths = CategoryName.MakeItemList(allItems);
                    base.DataBaseInfo = dataBaseInfo;
                }
                return base.DataBaseInfo;
            }
        }

        public new DataBaseState DataBaseState
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.DataBaseState;
            }
        }

        public AuthenticationInfo[] AuthenticationInfos
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.authentications.Select(item => ((Authentication)item).AuthenticationInfo).ToArray();
            }
        }

        public override TypeCategoryBase<Type, TypeCategory, TypeCollection, TypeCategoryCollection, TypeContext> TypeCategory
        {
            get { return this.typeContext?.Root; }
        }

        public override TableCategoryBase<Table, TableCategory, TableCollection, TableCategoryCollection, TableContext> TableCategory
        {
            get { return this.tableContext?.Root; }
        }

        public new string Name
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.Name;
            }
        }

        public bool IsLoaded
        {
            get { return this.DataBaseState.HasFlag(DataBaseState.IsLoaded); }
        }

        public new bool IsLocked
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.IsLocked;
            }
        }

        public new bool IsPrivate
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.IsPrivate;
            }
        }

        public new AccessInfo AccessInfo
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.AccessInfo;
            }
        }

        public new LockInfo LockInfo
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.LockInfo;
            }
        }

        public Guid ID
        {
            get { return base.DataBaseInfo.ID; }
        }

        public new event EventHandler Renamed
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Renamed += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Renamed -= value;
            }
        }

        public new event EventHandler Deleted
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Deleted += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Deleted -= value;
            }
        }

        public new event EventHandler Loaded
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Loaded += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Loaded -= value;
            }
        }

        public new event EventHandler Unloaded
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Unloaded += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Unloaded -= value;
            }
        }

        public event EventHandler<AuthenticationEventArgs> AuthenticationEntered
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.authenticationEntered += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.authenticationEntered -= value;
            }
        }

        public event EventHandler<AuthenticationEventArgs> AuthenticationLeft
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.authenticationLeft += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.authenticationLeft -= value;
            }
        }

        public new event EventHandler Resetting
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Resetting += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Resetting -= value;
            }
        }

        public new event EventHandler Reset
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Reset += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Reset -= value;
            }
        }

        public new event EventHandler DataBaseInfoChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.DataBaseInfoChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.DataBaseInfoChanged -= value;
            }
        }

        public new event EventHandler DataBaseStateChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.DataBaseStateChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.DataBaseStateChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateSetPrivate(IAuthentication authentication, object target)
        {
            base.OnValidateSetPrivate(authentication, target);

            if (target == this)
            {
                var userID = authentication.ID;
                var userInfo = this.userContext.Dispatcher.Invoke(() => this.userContext.Users[userID].UserInfo);
                if (userInfo.Authority == Authority.Guest)
                {
                    throw new PermissionException();
                }

                if (this.IsLoaded == false)
                    throw new InvalidOperationException(Resources.Exception_DataBaseHasNotBeenLoaded);
            }
        }

        public override void OnValidateSetPublic(IAuthentication authentication, object target)
        {
            base.OnValidateSetPublic(authentication, target);

            if (target == this)
            {
                var userID = authentication.ID;
                var userInfo = this.userContext.Dispatcher.Invoke(() => this.userContext.Users[userID].UserInfo);
                if (userInfo.Authority == Authority.Guest)
                {
                    throw new PermissionException();
                }

                if (this.IsLoaded == false)
                    throw new InvalidOperationException(Resources.Exception_DataBaseHasNotBeenLoaded);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateAddAccessMember(IAuthentication authentication, object target, string memberID, AccessType accessType)
        {
            base.OnValidateAddAccessMember(authentication, target, memberID, accessType);

            if (target == this)
            {
                var userInfo = this.userContext.Dispatcher.Invoke(() => this.userContext.Users[memberID].UserInfo);
                if (userInfo.Authority == Authority.Guest && accessType != AccessType.Guest)
                {
                    throw new PermissionException($"'{memberID}' 은(는) '{Authority.Guest}' 계정이기 때문에 '{accessType}' 권한을 설정할 수 없습니다.");
                }

                if (this.IsLoaded == false)
                    throw new InvalidOperationException(Resources.Exception_DataBaseHasNotBeenLoaded);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateSetAccessMember(IAuthentication authentication, object target, string memberID, AccessType accessType)
        {
            base.OnValidateSetAccessMember(authentication, target, memberID, accessType);

            if (target == this)
            {
                var userInfo = this.userContext.Dispatcher.Invoke(() => this.userContext.Users[memberID].UserInfo);
                if (accessType >= AccessType.Editor && userInfo.Authority == Authority.Guest)
                {
                    throw new PermissionException($"'{memberID}' 은(는) '{Authority.Guest}' 계정이기 때문에 '{accessType}' 권한을 설정할 수 없습니다.");
                }

                if (this.IsLoaded == false)
                    throw new InvalidOperationException(Resources.Exception_DataBaseHasNotBeenLoaded);
            }
        }

        public override void OnValidateRemoveAccessMember(IAuthentication authentication, object target)
        {
            base.OnValidateRemoveAccessMember(authentication, target);

            if (target == this)
            {
                if (this.IsLoaded == false)
                    throw new InvalidOperationException(Resources.Exception_DataBaseHasNotBeenLoaded);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateDelete(IAuthentication authentication, object target)
        {
            base.OnValidateDelete(authentication, target);

            if (this.ID == DataBase.defaultID)
                throw new PermissionDeniedException(Resources.Exception_DefaultDataBaseCannotDelete);

            if (this.IsLoaded == true)
                throw new InvalidOperationException(Resources.Exception_DataBaseHasBeenLoaded);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetAccessInfo(AccessInfo accessInfo)
        {
            accessInfo.Path = this.Name;
            base.AccessInfo = accessInfo;
        }

        private void AttachDomainHost()
        {
            var domainContext = this.CremaHost.DomainContext;
            var domains = domainContext.Domains.Where<Domain>(item => item.DataBaseID == this.ID)
                                               .ToArray();

            foreach (var item in domains)
            {
                try
                {
                    var target = this.FindDomainHost(item);
                    target.Restore(item);
                    item.SetDomainHost(target);
                }
                catch (Exception e)
                {
                    this.CremaHost.Error(e);
                }
            }
        }

        private void DetachDomainHost()
        {
            var domainContext = this.CremaHost.DomainContext;
            var domains = domainContext.Domains.Where<Domain>(item => item.DataBaseID == this.ID)
                                               .ToArray();

            foreach (var item in domains)
            {
                if (item.Host != null)
                {
                    item.Host.Detach();
                }
                item.SetDomainHost(null);
            }
        }

        private void SetDataBaseState(Authentication authentication, DataBaseState dataBaseState)
        {
            this.Dispatcher?.VerifyAccess();
            if (base.DataBaseState == dataBaseState)
                return;
            base.DataBaseState = dataBaseState;
            this.DataBases.InvokeItemsStateChangedEvent(authentication, new IDataBase[] { this });
        }

        private void AttachUsers(Authentication authentication)
        {
            var dataBaseID = this.ID;
            var domainContext = this.CremaHost.DomainContext;

            var query = from Domain item in domainContext.Domains
                        where item.DataBaseID == dataBaseID
                        select item;

            var domains = query.ToArray();
            foreach (var item in domains)
            {
                item.Dispatcher.Invoke(() =>
                {
                    if (item.Users.Contains(authentication.ID) == true)
                        item.Attach(authentication);
                });
            }
        }

        private void DetachUsers()
        {
            foreach (var item in this.authentications.ToArray())
            {
                this.DetachUsers(item);
            }
            this.authentications.Clear();
            this.metaData = null;
        }

        private void DetachUsers(Authentication authentication)
        {
            var dataBaseID = this.ID;
            var domainContext = this.CremaHost.DomainContext;

            var query = from Domain item in domainContext.Domains
                        where item.DataBaseID == dataBaseID
                        select item;

            var domains = query.ToArray();
            foreach (var item in domains)
            {
                item.Dispatcher.Invoke(() =>
                {
                    if (item.Users.Contains(authentication.ID) == true)
                    {
                        var user = item.Users[authentication.ID];
                        if (user.IsOnline == true)
                        {
                            item.Detach(authentication);
                        }
                    }
                });
            }
        }

        private void WriteCache()
        {
            var filename = FileUtility.Prepare(this.cachePath, $"{this.ID}{dataExtension}");
            var dataInfo = new DataBaseDataSerializationInfo()
            {
                Revision = this.Repository.RepositoryInfo.Revision,
                TypeInfos = this.typeContext.Types.Select((Type item) => item.TypeInfo).ToArray(),
                TableInfos = this.tableContext.Tables.Select((Table item) => item.TableInfo).ToArray(),
            };
            JsonSerializerUtility.Write(filename, dataInfo, true);
        }

        public void Initialize()
        {
            //if (this.repositoryHost == null)
            //{
            //    this.repositoryHost = new DataBaseRepositoryHost(this, cremaHost.Repository);
            //    this.repositoryHost.Changed += Repository_Changed;
            //}

            var remotePath = this.CremaHost.GetPath(CremaPath.RemoteDataBases);
            var repositoryInfo = this.repositoryProvider.GetRepositoryInfo(remotePath, this.Name);

            if (base.DataBaseInfo.Revision != repositoryInfo.Revision)
            {
                this.CremaHost.Debug($"initialize database : {base.Name}");
                //var branchRevision = this.Repository.BranchRevision;
                //var branchLog = this.Repository.GetLog(this.basePath, branchRevision, 1).First();
                //var latestLog = this.Repository.GetLog(this.basePath, this.repositoryHost.Revision, 1).First();
                //var branchUserID = branchLog.GetPropertyString(LogPropertyInfo.UserIDKey) ?? string.Empty;
                //var latestUserID = latestLog.GetPropertyString(LogPropertyInfo.UserIDKey) ?? string.Empty;
                //var uri = this.Repository.GetUri(this.basePath, branchRevision);
                //var branchName = uri.Segments.Last();

                base.DataBaseInfo = new DataBaseInfo()
                {
                    ID = repositoryInfo.ID,
                    Name = repositoryInfo.Name,
                    Revision = repositoryInfo.Revision,
                    Comment = repositoryInfo.Comment,
                    BranchRevision = repositoryInfo.BranchRevision,
                    BranchSource = repositoryInfo.BranchSource,
                    BranchSourceRevision = repositoryInfo.BranchSourceRevision,
                    CreationInfo = repositoryInfo.CreationInfo,
                    ModificationInfo = repositoryInfo.ModificationInfo,
                };
            }

            this.ReadAccessInfo();
        }

        private void ReadCache()
        {
            var filename = FileUtility.Prepare(this.cachePath, $"{this.ID}{dataExtension}");

            if (this.CremaHost.NoCache == false && File.Exists(filename) == true)
            {
                try
                {
                    var dataInfo = JsonSerializerUtility.Read<DataBaseDataSerializationInfo>(filename);
                    if (this.Repository.RepositoryInfo.Revision == dataInfo.Revision)
                    {
                        //this.ResettingDataBase(Authentication.System);
                        this.ResetDataBase(Authentication.System, dataInfo.TypeInfos, dataInfo.TableInfos);
                        return;
                    }
                }
                catch (Exception e)
                {
                    this.CremaHost.Error(e);
                    this.CremaHost.Error($"'{this.Name}' cache is crashed.");
                }
            }

            {
                this.CremaHost.Debug($"begin read database : '{this.Name}'");
                var dataSet = CremaDataSet.ReadFromDirectory(this.BasePath);
                this.CremaHost.Debug($"end read database : '{this.Name}'");

                var typeInfos = dataSet.Types.Select(item => item.TypeInfo);
                var tableInfos = dataSet.Tables.Select(item => item.TableInfo);
                //this.ResettingDataBase(Authentication.System);
                this.ResetDataBase(Authentication.System, typeInfos, tableInfos);
                this.dataSetCache = dataSet;
            }
        }

        private void Users_UsersLoggedOut(object sender, ItemsEventArgs<IUser> e)
        {
            this.CremaHost.Dispatcher.InvokeAsync(() =>
            {
                if (e.UserID == this.LockInfo.UserID)
                {
                    this.Unlock(Authentication.System);
                }
            });
        }

        private void Repository_Changed(object sender, EventArgs e)
        {
            var dataBaseInfo = base.DataBaseInfo;
            dataBaseInfo.Revision = this.Repository.RepositoryInfo.Revision;
            dataBaseInfo.ModificationInfo = this.Repository.RepositoryInfo.ModificationInfo;
            dataBaseInfo.Paths = null;
            dataBaseInfo.TypesHashValue = CremaDataSet.GenerateHashValue((from Type item in this.TypeContext.Types select item.TypeInfo).ToArray());
            dataBaseInfo.TablesHashValue = CremaDataSet.GenerateHashValue((from Table item in this.TableContext.Tables select item.TableInfo).ToArray());
            this.dataSetCache = null;
            this.metaData = null;
            Authentication.System.Sign();
            base.DataBaseInfo = dataBaseInfo;

            this.Dispatcher.InvokeAsync(() =>
            {
                this.DataBases.InvokeItemsChangedEvent(Authentication.System, new IDataBase[] { this });
            });
        }

        private void Authentication_Expired(object sender, EventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                var authentication = sender as Authentication;
                if (this.IsLoaded == true && this.VerifyAccess(authentication) == true)
                {
                    this.OnLeave(authentication);
                    this.DataBases.InvokeItemsAuthenticationLeftEvent(authentication, new IDataBase[] { this });
                }
            });
        }

        private void InvokeDataBaseSetPublic(Authentication authentication)
        {
            var accessInfoPath = this.GetAccessInfoPath();
            var comment = EventMessageBuilder.SetPublicDataBase(authentication, new IDataBase[] { this, });
            try
            {
                this.Repository.Delete(accessInfoPath);
                this.Repository.Commit(authentication, comment);
                this.CremaHost.Info(comment);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        private void InvokeDataBaseSetPrivate(Authentication authentication)
        {
            var accessInfoPath = this.GetAccessInfoPath();
            var accessInfo = this.AccessInfo;
            var comment = EventMessageBuilder.SetPrivateDataBase(authentication, new IDataBase[] { this, });
            try
            {
                accessInfo.SetPrivate(this.GetType().Name, authentication.SignatureDate);
                this.WriteAccessInfo(accessInfoPath, accessInfo);
                this.Repository.Add(accessInfoPath);
                this.Repository.Commit(authentication, comment);
                this.CremaHost.Info(comment);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        private void InvokeDataBaseAddAccessMember(Authentication authentication, string memberID, AccessType accessType)
        {
            var accessInfoPath = this.GetAccessInfoPath();
            var accessInfo = this.AccessInfo;
            var comment = EventMessageBuilder.AddAccessMemberToDataBase(authentication, new IDataBase[] { this }, new string[] { memberID, }, new AccessType[] { accessType, });
            try
            {
                accessInfo.Add(authentication.SignatureDate, memberID, accessType);
                this.WriteAccessInfo(accessInfoPath, accessInfo);
                this.Repository.Commit(authentication, comment);
                this.CremaHost.Info(comment);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        private void InvokeDataBaseSetAccessMember(Authentication authentication, string memberID, AccessType accessType)
        {
            var accessInfoPath = this.GetAccessInfoPath();
            var accessInfo = this.AccessInfo;
            var comment = EventMessageBuilder.SetAccessMemberOfDataBase(authentication, new IDataBase[] { this }, new string[] { memberID, }, new AccessType[] { accessType, });
            try
            {
                accessInfo.Set(authentication.SignatureDate, memberID, accessType);
                this.WriteAccessInfo(accessInfoPath, accessInfo);
                this.Repository.Commit(authentication, comment);
                this.CremaHost.Info(comment);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        private void InvokeDataBaseRemoveAccessMember(Authentication authentication, string memberID)
        {
            var accessInfoPath = this.GetAccessInfoPath();
            var accessInfo = this.AccessInfo;
            var comment = EventMessageBuilder.RemoveAccessMemberFromDataBase(authentication, new IDataBase[] { this }, new string[] { memberID, });

            try
            {
                accessInfo.Remove(authentication.SignatureDate, memberID);
                this.WriteAccessInfo(accessInfoPath, accessInfo);
                this.Repository.Commit(authentication, comment);
                this.CremaHost.Info(comment);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        private void ValidateDispatcher()
        {
            if (this.Dispatcher == null)
                throw new InvalidOperationException(Resources.Exception_InvalidObject);
            this.Dispatcher?.VerifyAccess();
        }

        private void ValidatePreview(Authentication authentication)
        {
            if (this.IsLoaded == false)
                throw new NotImplementedException();
            this.ValidateBeginInDataBase(authentication);
        }

        private void ValidateEnter(Authentication authentication)
        {
            if (this.IsLoaded == false)
                throw new InvalidOperationException(Resources.Exception_DataBaseHasNotBeenLoaded);
            if (this.authentications.Contains(authentication) == true)
                throw new ArgumentException(Resources.Exception_AlreadyInDataBase, nameof(authentication));
            if (this.VerifyAccessType(authentication, AccessType.Guest) == false)
                throw new PermissionDeniedException();
        }

        private void ValidateLeave(Authentication authentication)
        {
            if (this.IsLoaded == false)
                throw new InvalidOperationException(Resources.Exception_DataBaseHasNotBeenLoaded);
            //if (this.authentications.Contains(authentication) == false)
            //    throw new NotImplementedException();
        }

        private void ValidateLoad(Authentication authentication)
        {
            if (this.IsLoaded == true)
                throw new InvalidOperationException(Resources.Exception_DataBaseHasBeenLoaded);
            if (authentication.IsSystem == false && authentication.IsAdmin == false)
                throw new PermissionDeniedException();
            if (this.VerifyAccessType(authentication, AccessType.Master) == false)
                throw new PermissionDeniedException();
        }

        private void ValidateUnload(Authentication authentication)
        {
            if (this.IsLoaded == false)
                throw new InvalidOperationException(Resources.Exception_DataBaseHasNotBeenLoaded);
            if (authentication.IsSystem == false && authentication.IsAdmin == false)
                throw new PermissionDeniedException();
            if (this.VerifyAccessType(authentication, AccessType.Master) == false)
                throw new PermissionDeniedException();
        }

        private void ValidateRevert(Authentication authentication, string revision)
        {
            if (authentication.IsSystem == false && authentication.IsAdmin == false)
                throw new PermissionDeniedException();
            if (this.IsLoaded == true)
                throw new InvalidOperationException(Resources.Exception_LoadedDataBaseCannotRevert);
            var logs = this.Repository.GetLog(new string[] { this.BasePath }, this.Repository.RepositoryInfo.Revision, 100);
            if (logs.Any(item => item.Revision == revision) == false)
                throw new ArgumentException(string.Format(Resources.Exception_NotFoundRevision_Format, revision), nameof(revision));
        }

        private void ValidateBeginTransaction(Authentication authentication)
        {
            if (this.IsLoaded == false)
                throw new InvalidOperationException(Resources.Exception_DataBaseHasNotBeenLoaded);
            if (authentication.IsSystem == false && authentication.IsAdmin == false)
                throw new PermissionDeniedException();
            if (this.VerifyAccessType(authentication, AccessType.Owner) == false)
                throw new PermissionDeniedException();
        }

        private void OnEnter(Authentication authentication)
        {
            this.Sign(authentication);
            this.authentications.Add(authentication);
            this.metaData = null;
            authentication.Expired += Authentication_Expired;
            this.SetDataBaseState(authentication, this.DataBaseState | DataBaseState.HasWorker);
            this.AttachUsers(authentication);
            this.authenticationEntered?.Invoke(this, new AuthenticationEventArgs(authentication.AuthenticationInfo));
        }

        private void OnLeave(Authentication authentication)
        {
            this.Sign(authentication);
            this.authentications.Remove(authentication);
            this.metaData = null;
            authentication.Expired -= Authentication_Expired;
            this.DetachUsers(authentication);
            if (this.authentications.Any() == false)
            {
                this.SetDataBaseState(authentication, this.DataBaseState & ~DataBaseState.HasWorker);
            }
            this.authenticationLeft?.Invoke(this, new AuthenticationEventArgs(authentication.AuthenticationInfo));
        }

        private void ReadAccessInfo()
        {
            var accessInfoPath = this.GetAccessInfoPath();
            try
            {
                this.ReadAccessInfo(accessInfoPath);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
            }
        }

        private void Sign(Authentication authentication)
        {
            authentication.Sign();
        }

        #region IDataBase

        IDataBase IDataBase.Copy(Authentication authentication, string newDataBaseName, string comment, bool force)
        {
            this.ValidateDispatcher();
            return this.DataBases.CopyDataBase(authentication, this, newDataBaseName, comment, force);
        }

        ITransaction IDataBase.BeginTransaction(Authentication authentication)
        {
            return this.BeginTransaction(authentication);
        }

        ITableContext IDataBase.TableContext
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                if (this.IsLoaded == false)
                    return null;
                return this.TableContext;
            }
        }

        ITypeContext IDataBase.TypeContext
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                if (this.IsLoaded == false)
                    return null;
                return this.TypeContext;
            }
        }

        #endregion

        #region IServiceProvider

        object IServiceProvider.GetService(System.Type serviceType)
        {
            if (serviceType == typeof(IDataBase))
                return this;

            if (base.DataBaseState.HasFlag(DataBaseState.IsLoaded) == true)
            {
                if (serviceType == typeof(ITableContext))
                    return this.tableContext;
                else if (serviceType == typeof(ITableCategoryCollection))
                    return this.tableContext.Categories;
                else if (serviceType == typeof(ITableCollection))
                    return this.tableContext.Tables;
                else if (serviceType == typeof(ITypeContext))
                    return this.typeContext;
                else if (serviceType == typeof(ITypeCategoryCollection))
                    return this.typeContext.Categories;
                else if (serviceType == typeof(ITypeCollection))
                    return this.typeContext.Types;
            }

            return this.CremaHost.GetService(serviceType);
        }

        #endregion

        #region IInfoProvider

        IDictionary<string, object> IInfoProvider.Info => this.DataBaseInfo.ToDictionary();

        #endregion

        #region IStateProvider

        object IStateProvider.State => this.DataBaseState;

        #endregion
    }
}
