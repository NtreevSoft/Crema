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

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Data.Xml;
using Ntreev.Crema.Data;
using System.Collections.Generic;
using Ntreev.Library.Linq;
using Ntreev.Library;
using Ntreev.Library.Serialization;
using System.IO;
using System.Text;

namespace Ntreev.Crema.ServiceHosts.Data
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    class DataBaseService : CremaServiceItemBase<IDataBaseEventCallback>, IDataBaseService, ICremaServiceItem
    {
        private readonly ICremaHost cremaHost;
        private readonly ILogService logService;

        private readonly IDomainContext domainContext;
        private readonly IUserContext userContext;

        private IDataBase dataBase;
        private Authentication authentication;
        private string dataBaseName;

        public DataBaseService(ICremaHost cremaHost)
            : base(cremaHost.GetService(typeof(ILogService)) as ILogService)
        {
            this.cremaHost = cremaHost;
            this.logService = cremaHost.GetService(typeof(ILogService)) as ILogService;
            this.userContext = cremaHost.GetService(typeof(IUserContext)) as IUserContext;
            this.domainContext = cremaHost.GetService(typeof(IDomainContext)) as IDomainContext;

            this.logService.Debug($"{nameof(DataBaseService)} Constructor");
        }

        public ResultBase DefinitionType(LogInfo[] param1, FindResultInfo[] param2)
        {
            return new ResultBase();
        }

        public ResultBase<DataBaseMetaData> Subscribe(Guid authenticationToken, string dataBaseName)
        {
            var result = new ResultBase<DataBaseMetaData>();
            try
            {
                this.userContext.Dispatcher.Invoke(() =>
                {
                    this.authentication = this.userContext.Authenticate(authenticationToken);
                    this.authentication.AddRef(this);
                    this.OwnerID = this.authentication.ID;
                    this.userContext.Users.UsersLoggedOut += Users_UsersLoggedOut;
                });
                result.Value = this.cremaHost.Dispatcher.Invoke(() =>
                {
                    this.dataBase = this.cremaHost.DataBases[dataBaseName];
                    this.dataBaseName = dataBaseName;
                    this.dataBase.Enter(this.authentication);
                    this.AttachEventHandlers();
                    this.logService.Debug($"[{this.OwnerID}] {nameof(DataBaseService)} {nameof(Subscribe)} : {dataBaseName}");
                    return this.dataBase.GetMetaData(this.authentication);
                });
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                this.dataBase = null;
                this.dataBaseName = null;
                result.Fault = new CremaFault(e);
            }
            return result;
        }

        public ResultBase Unsubscribe()
        {
            var result = new ResultBase();
            try
            {
                this.cremaHost.Dispatcher.Invoke(() =>
                {
                    this.DetachEventHandlers();
                    this.dataBase?.Leave(this.authentication);
                    this.dataBase = null;
                });
                this.userContext.Dispatcher.Invoke(() =>
                {
                    this.userContext.Users.UsersLoggedOut -= Users_UsersLoggedOut;
                    this.authentication.RemoveRef(this);
                    this.authentication = null;
                });
                result.SignatureDate = new SignatureDateProvider(this.OwnerID).Provide();
                this.logService.Debug($"[{this.OwnerID}] {nameof(DataBaseService)} {nameof(Unsubscribe)} : {this.dataBaseName}");
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault(e);
            }
            return result;
        }

        public ResultBase<DataBaseMetaData> GetMetaData()
        {
            return this.Invoke(() =>
            {
                return this.dataBase.GetMetaData(this.authentication);
            });
        }

        public ResultBase<CremaDataSet> GetDataSet(string revision, string filterExpression)
        {
            return this.InvokeImmediately(() =>
            {
                return this.dataBase.Dispatcher.Invoke(() => this.dataBase.GetDataSet(this.authentication, revision, filterExpression));
            });
        }

        public ResultBase NewTableCategory(string categoryPath)
        {
            return this.Invoke(() =>
            {
                var categoryName = new Ntreev.Library.ObjectModel.CategoryName(categoryPath);
                var category = this.GetTableCategory(categoryName.ParentPath);
                category.AddNewCategory(this.authentication, categoryName.Name);
            });
        }

        public ResultBase<CremaDataSet> GetTableItemDataSet(string itemPath, string revision)
        {
            return this.InvokeImmediately(() =>
            {
                var tableItem = this.dataBase.Dispatcher.Invoke(() => this.GetTableItem(itemPath));
                return tableItem.GetDataSet(this.authentication, revision);
            });
        }

        public ResultBase<CremaDataSet> GetTableDataSet(string revision, string filterExpression)
        {
            return this.InvokeImmediately(() =>
            {
                return this.TableContext.GetDataSet(authentication, revision, filterExpression);
            });
        }

        public ResultBase ImportTables(CremaDataSet dataSet, string comment)
        {
            return this.Invoke(() =>
            {
                this.TableContext.Import(this.authentication, dataSet, comment);
            });
        }

        public ResultBase RenameTableItem(string itemPath, string newName)
        {
            return this.Invoke(() =>
            {
                var tableItem = this.GetTableItem(itemPath);
                tableItem.Rename(this.authentication, newName);
            });
        }

        public ResultBase MoveTableItem(string itemPath, string parentPath)
        {
            return this.Invoke(() =>
            {
                var tableItem = this.GetTableItem(itemPath);
                tableItem.Move(this.authentication, parentPath);
            });
        }

        public ResultBase DeleteTableItem(string itemPath)
        {
            return this.Invoke(() =>
            {
                var tableItem = this.GetTableItem(itemPath);
                tableItem.Delete(this.authentication);
            });
        }

        public ResultBase SetPublicTableItem(string itemPath)
        {
            return this.Invoke(() =>
            {
                var tableItem = this.GetTableItem(itemPath);
                tableItem.SetPublic(this.authentication);
            });
        }

        public ResultBase<AccessInfo> SetPrivateTableItem(string itemPath)
        {
            return this.Invoke(() =>
            {
                var tableItem = this.GetTableItem(itemPath);
                tableItem.SetPrivate(this.authentication);
                return tableItem.AccessInfo;
            });
        }

        public ResultBase<AccessMemberInfo> AddAccessMemberTableItem(string itemPath, string memberID, AccessType accessType)
        {
            return this.Invoke(() =>
            {
                var tableItem = this.GetTableItem(itemPath);
                tableItem.AddAccessMember(this.authentication, memberID, accessType);
                return tableItem.AccessInfo.Members.Where(item => item.UserID == memberID).First();
            });
        }

        public ResultBase<AccessMemberInfo> SetAccessMemberTableItem(string itemPath, string memberID, AccessType accessType)
        {
            return this.Invoke(() =>
            {
                var tableItem = this.GetTableItem(itemPath);
                tableItem.SetAccessMember(this.authentication, memberID, accessType);
                return tableItem.AccessInfo.Members.Where(item => item.UserID == memberID).First();
            });
        }

        public ResultBase RemoveAccessMemberTableItem(string itemPath, string memberID)
        {
            return this.Invoke(() =>
            {
                var tableItem = this.GetTableItem(itemPath);
                tableItem.RemoveAccessMember(this.authentication, memberID);
            });
        }

        public ResultBase<LockInfo> LockTableItem(string itemPath, string comment)
        {
            return this.Invoke(() =>
            {
                var tableItem = this.GetTableItem(itemPath);
                tableItem.Lock(this.authentication, comment);
                return tableItem.LockInfo;
            });
        }

        public ResultBase UnlockTableItem(string itemPath)
        {
            return this.Invoke(() =>
            {
                var tableItem = this.GetTableItem(itemPath);
                tableItem.Unlock(this.authentication);
            });
        }

        public ResultBase<LogInfo[]> GetTableItemLog(string itemPath)
        {
            return this.InvokeImmediately(() =>
            {
                var tableItem = this.dataBase.Dispatcher.Invoke(() => this.GetTableItem(itemPath));
                return tableItem.GetLog(this.authentication);
            });
        }

        public ResultBase<FindResultInfo[]> FindTableItem(string itemPath, string text, FindOptions options)
        {
            return this.InvokeImmediately(() =>
            {
                var tableItem = this.dataBase.Dispatcher.Invoke(() => this.GetTableItem(itemPath));
                return tableItem.Find(this.authentication, text, options);
            });
        }

        public ResultBase<TableInfo[]> CopyTable(string tableName, string newTableName, string categoryPath, bool copyXml)
        {
            return this.Invoke(() =>
            {
                var table = this.GetTable(tableName);
                var newTable = table.Copy(this.authentication, newTableName, categoryPath, copyXml);
                return EnumerableUtility.Friends(newTable, newTable.Childs).Select(item => item.TableInfo).ToArray();
            });
        }

        public ResultBase<TableInfo[]> InheritTable(string tableName, string newTableName, string categoryPath, bool copyXml)
        {
            return this.Invoke(() =>
            {
                var table = this.GetTable(tableName);
                var newTable = table.Inherit(this.authentication, newTableName, categoryPath, copyXml);
                return EnumerableUtility.Friends(newTable, newTable.Childs).Select(item => item.TableInfo).ToArray();
            });
        }

        public ResultBase<DomainMetaData> EnterTableContentEdit(string tableName)
        {
            return this.Invoke(() =>
            {
                var table = this.GetTable(tableName);
                var content = table.Content;
                content.EnterEdit(this.authentication);
                var domain = content.Domain;
                return domain.Dispatcher.Invoke(() => domain.GetMetaData(this.authentication));
            });
        }

        public ResultBase<DomainMetaData> LeaveTableContentEdit(string tableName)
        {
            return this.Invoke(() =>
            {
                var table = this.GetTable(tableName);
                var content = table.Content;
                content.LeaveEdit(this.authentication);
                var domain = content.Domain;
                return domain.Dispatcher.Invoke(() => domain.GetMetaData(this.authentication));
            });
        }

        public ResultBase<DomainMetaData> BeginTableContentEdit(string tableName)
        {
            return this.Invoke(() =>
            {
                var table = this.GetTable(tableName);
                var content = table.Content;
                if (content.Domain == null)
                    content.BeginEdit(this.authentication);
                var domain = content.Domain;
                return domain.Dispatcher.Invoke(() => domain.GetMetaData(this.authentication));
            });
        }

        public ResultBase<TableInfo[]> EndTableContentEdit(string tableName)
        {
            return this.Invoke(() =>
            {
                var table = this.GetTable(tableName);
                var content = table.Content;
                var tables = content.Tables;
                content.EndEdit(this.authentication);
                return tables.Select(item => item.TableInfo).ToArray();
            });
        }

        public ResultBase CancelTableContentEdit(string tableName)
        {
            return this.Invoke(() =>
            {
                var table = this.GetTable(tableName);
                var content = table.Content;
                content.CancelEdit(this.authentication);
            });
        }

        public ResultBase<DomainMetaData> BeginTableTemplateEdit(string tableName)
        {
            return this.Invoke(() =>
            {
                var table = this.GetTable(tableName);
                var template = table.Template;
                template.BeginEdit(this.authentication);
                var domain = template.Domain;
                return domain.Dispatcher.Invoke(() => domain.GetMetaData(this.authentication));
            });
        }

        public ResultBase<DomainMetaData> BeginNewTable(string itemPath)
        {
            return this.Invoke(() =>
            {
                var tableItem = this.GetTableItem(itemPath);

                if (tableItem is ITableCategory category)
                {
                    var template = category.NewTable(this.authentication);
                    var domain = template.Domain;
                    return domain.Dispatcher.Invoke(() => domain.GetMetaData(this.authentication));
                }
                else if (tableItem is ITable table)
                {
                    var template = table.NewTable(this.authentication);
                    var domain = template.Domain;
                    return domain.Dispatcher.Invoke(() => domain.GetMetaData(this.authentication));
                }
                throw new NotImplementedException();
            });
        }

        public ResultBase<TableInfo[]> EndTableTemplateEdit(Guid domainID)
        {
            return this.Invoke(() =>
            {
                var domain = this.domainContext.Domains[domainID];
                var template = domain.Host as ITableTemplate;
                template.EndEdit(this.authentication);
                if (template.Target is ITable table)
                    return new TableInfo[] { table.TableInfo };
                else if (template.Target is ITable[] tables)
                    return tables.Select(item => item.TableInfo).ToArray();
                throw new NotImplementedException();
            });
        }

        public ResultBase CancelTableTemplateEdit(Guid domainID)
        {
            return this.Invoke(() =>
            {
                var domain = this.domainContext.Domains[domainID];
                var template = domain.Host as ITableTemplate;
                template.CancelEdit(this.authentication);
            });
        }

        public ResultBase NewTypeCategory(string categoryPath)
        {
            return this.Invoke(() =>
            {
                var categoryName = new Ntreev.Library.ObjectModel.CategoryName(categoryPath);
                var category = this.GetTypeCategory(categoryName.ParentPath);
                category.AddNewCategory(this.authentication, categoryName.Name);
            });
        }

        public ResultBase<CremaDataSet> GetTypeItemDataSet(string itemPath, string revision)
        {
            return this.InvokeImmediately(() =>
            {
                var typeItem = this.cremaHost.Dispatcher.Invoke(() => this.GetTypeItem(itemPath));
                return typeItem.GetDataSet(this.authentication, revision);
            });
        }

        public ResultBase<CremaDataSet> GetTypeDataSet(string revision, string filterExpression)
        {
            return this.InvokeImmediately(() =>
            {
                return this.TypeContext.GetDataSet(this.authentication, revision, filterExpression);
            });
        }

        public ResultBase ImportTypes(CremaDataSet dataSet, string comment)
        {
            return this.Invoke(() =>
            {
                this.TypeContext.Import(this.authentication, dataSet, comment);
            });
        }

        public ResultBase RenameTypeItem(string itemPath, string newName)
        {
            return this.Invoke(() =>
            {
                var typeItem = this.GetTypeItem(itemPath);
                typeItem.Rename(this.authentication, newName);
            });
        }

        public ResultBase MoveTypeItem(string itemPath, string parentPath)
        {
            return this.Invoke(() =>
            {
                var typeItem = this.GetTypeItem(itemPath);
                typeItem.Move(this.authentication, parentPath);
            });
        }

        public ResultBase DeleteTypeItem(string itemPath)
        {
            return this.Invoke(() =>
            {
                var typeItem = this.GetTypeItem(itemPath);
                typeItem.Delete(this.authentication);
            });
        }

        public ResultBase<TypeInfo> CopyType(string typeName, string newTypeName, string categoryPath)
        {
            return this.Invoke(() =>
            {
                var type = this.GetType(typeName);
                var newType = type.Copy(this.authentication, newTypeName, categoryPath);
                return newType.TypeInfo;
            });
        }

        public ResultBase<DomainMetaData> BeginTypeTemplateEdit(string typeName)
        {
            return this.Invoke(() =>
            {
                var type = this.GetType(typeName);
                var template = type.Template;
                template.BeginEdit(this.authentication);
                var domain = template.Domain;
                return domain.Dispatcher.Invoke(() => domain.GetMetaData(this.authentication));
            });
        }

        public ResultBase<DomainMetaData> BeginNewType(string categoryPath)
        {
            return this.Invoke(() =>
            {
                var category = this.GetTypeCategory(categoryPath);
                var template = category.NewType(this.authentication);
                var domain = template.Domain;
                return domain.Dispatcher.Invoke(() => domain.GetMetaData(this.authentication));
            });
        }

        public ResultBase<TypeInfo> EndTypeTemplateEdit(Guid domainID)
        {
            return this.Invoke(() =>
            {
                var domain = this.domainContext.Domains[domainID];
                var template = domain.Host as ITypeTemplate;
                template.EndEdit(this.authentication);
                return template.Type.TypeInfo;
            });
        }

        public ResultBase CancelTypeTemplateEdit(Guid domainID)
        {
            return this.Invoke(() =>
            {
                var domain = this.domainContext.Domains[domainID];
                var template = domain.Host as ITypeTemplate;
                template.CancelEdit(this.authentication);
            });
        }

        public ResultBase SetPublicTypeItem(string itemPath)
        {
            return this.Invoke(() =>
            {
                var typeItem = this.GetTypeItem(itemPath);
                typeItem.SetPublic(this.authentication);
            });
        }

        public ResultBase<AccessInfo> SetPrivateTypeItem(string itemPath)
        {
            return this.Invoke(() =>
            {
                var typeItem = this.GetTypeItem(itemPath);
                typeItem.SetPrivate(this.authentication);
                return typeItem.AccessInfo;
            });
        }

        public ResultBase<AccessMemberInfo> AddAccessMemberTypeItem(string itemPath, string memberID, AccessType accessType)
        {
            return this.Invoke(() =>
            {
                var typeItem = this.GetTypeItem(itemPath);
                typeItem.AddAccessMember(this.authentication, memberID, accessType);
                return typeItem.AccessInfo.Members.Where(item => item.UserID == memberID).First();
            });
        }

        public ResultBase<AccessMemberInfo> SetAccessMemberTypeItem(string itemPath, string memberID, AccessType accessType)
        {
            return this.Invoke(() =>
            {
                var typeItem = this.GetTypeItem(itemPath);
                typeItem.SetAccessMember(this.authentication, memberID, accessType);
                return typeItem.AccessInfo.Members.Where(item => item.UserID == memberID).First();
            });
        }

        public ResultBase RemoveAccessMemberTypeItem(string itemPath, string memberID)
        {
            return this.Invoke(() =>
            {
                var typeItem = this.GetTypeItem(itemPath);
                typeItem.RemoveAccessMember(this.authentication, memberID);
            });
        }

        public ResultBase<LockInfo> LockTypeItem(string itemPath, string comment)
        {
            return this.Invoke(() =>
            {
                var typeItem = this.GetTypeItem(itemPath);
                typeItem.Lock(this.authentication, comment);
                return typeItem.LockInfo;
            });
        }

        public ResultBase UnlockTypeItem(string itemPath)
        {
            return this.Invoke(() =>
            {
                var typeItem = this.GetTypeItem(itemPath);
                typeItem.Unlock(this.authentication);
            });
        }

        public ResultBase<LogInfo[]> GetTypeItemLog(string itemPath)
        {
            return this.InvokeImmediately(() =>
            {
                var typeItem = this.cremaHost.Dispatcher.Invoke(() => this.GetTypeItem(itemPath));
                return typeItem.GetLog(this.authentication);
            });
        }

        public ResultBase<FindResultInfo[]> FindTypeItem(string itemPath, string text, FindOptions options)
        {
            return this.InvokeImmediately(() =>
            {
                var typeItem = this.cremaHost.Dispatcher.Invoke(() => this.GetTypeItem(itemPath));
                return typeItem.Find(this.authentication, text, options);
            });
        }

        public bool IsAlive()
        {
            if (this.authentication == null)
                return false;
            this.logService.Debug($"[{this.authentication}] {nameof(DataBaseService)}.{nameof(IsAlive)} : {DateTime.Now}");
            this.authentication.Ping();
            return true;
        }

        protected override void OnDisposed(EventArgs e)
        {
            base.OnDisposed(e);
            this.cremaHost.Dispatcher.Invoke(() =>
            {
                if (this.authentication != null)
                {
                    this.DetachEventHandlers();
                }
                this.dataBase = null;
            });
            this.userContext.Dispatcher.Invoke(() =>
            {
                this.userContext.Users.UsersLoggedOut -= Users_UsersLoggedOut;
                if (this.authentication != null)
                {
                    if (this.authentication.RemoveRef(this) == 0)
                    {
                        this.userContext.Logout(this.authentication);
                    }
                    this.authentication = null;
                }
            });
        }

        protected override void OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo)
        {
            this.Callback.OnServiceClosed(signatureDate, closeInfo);
        }

        private void Users_UsersLoggedOut(object sender, ItemsEventArgs<IUser> e)
        {
            var actionUserID = e.UserID;
            var contains = e.Items.Any(item => item.ID == this.authentication.ID);
            var closeInfo = (CloseInfo)e.MetaData;
            if (actionUserID != this.authentication.ID && contains == true)
            {
                if (this.dataBase != null)
                {
                    this.InvokeEvent(null, null, () => this.Callback.OnServiceClosed(e.SignatureDate, (CloseInfo)e.MetaData));
                }
            }
        }

        private void Tables_TablesStateChanged(object sender, ItemsEventArgs<ITable> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var tableNames = e.Items.Select(item => item.Name).ToArray();
            var states = e.Items.Select(item => item.TableState).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTablesStateChanged(signatureDate, tableNames, states));
        }

        private void Tables_TablesChanged(object sender, ItemsEventArgs<ITable> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var values = e.Items.Select(item => item.TableInfo).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTablesChanged(signatureDate, values));
        }

        private void TableContext_ItemCreated(object sender, ItemsCreatedEventArgs<ITableItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var paths = e.Items.Select(item => item.Path).ToArray();
            var arguments = e.Arguments.Select(item => item is TableInfo tableInfo ? (TableInfo?)tableInfo : null).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTableItemsCreated(signatureDate, paths, arguments));
        }

        private void TableContext_ItemRenamed(object sender, ItemsRenamedEventArgs<ITableItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var oldPaths = e.OldPaths;
            var itemNames = e.Items.Select(item => item.Name).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTableItemsRenamed(signatureDate, oldPaths, itemNames));
        }

        private void TableContext_ItemMoved(object sender, ItemsMovedEventArgs<ITableItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var oldPaths = e.OldPaths;
            var parentPaths = e.Items.Select(item => item.Parent.Path).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTableItemsMoved(signatureDate, oldPaths, parentPaths));
        }

        private void TableContext_ItemDeleted(object sender, ItemsDeletedEventArgs<ITableItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var itemPaths = e.ItemPaths;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTableItemsDeleted(signatureDate, itemPaths));
        }

        private void TableContext_ItemsAccessChanged(object sender, ItemsEventArgs<ITableItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var values = new AccessInfo[e.Items.Length];
            for (var i = 0; i < e.Items.Length; i++)
            {
                var item = e.Items[i];
                var accessInfo = item.AccessInfo;
                if (item.AccessInfo.Path != item.Path)
                {
                    accessInfo = AccessInfo.Empty;
                    accessInfo.Path = item.Path;
                }
                values[i] = accessInfo;
            }
            var metaData = e.MetaData as object[];
            var changeType = (AccessChangeType)metaData[0];
            var memberIDs = metaData[1] as string[];
            var accessTypes = metaData[2] as AccessType[];

            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTableItemsAccessChanged(signatureDate, changeType, values, memberIDs, accessTypes));
        }

        private void TableContext_ItemsLockChanged(object sender, ItemsEventArgs<ITableItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var values = new LockInfo[e.Items.Length];
            for (var i = 0; i < e.Items.Length; i++)
            {
                var item = e.Items[i];
                var lockInfo = item.LockInfo;
                if (item.LockInfo.Path != item.Path)
                {
                    lockInfo = LockInfo.Empty;
                    lockInfo.Path = item.Path;
                }
                values[i] = lockInfo;
            }
            var metaData = e.MetaData as object[];
            var changeType = (LockChangeType)metaData[0];
            var comments = metaData[1] as string[];

            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTableItemsLockChanged(signatureDate, changeType, values, comments));
        }

        private void Types_TypesStateChanged(object sender, ItemsEventArgs<IType> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var typeNames = e.Items.Select(item => item.Name).ToArray();
            var states = e.Items.Select(item => item.TypeState).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTypesStateChanged(signatureDate, typeNames, states));
        }

        private void Types_TypesChanged(object sender, ItemsEventArgs<IType> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var values = e.Items.Select(item => item.TypeInfo).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTypesChanged(signatureDate, values));
        }

        private void TypeContext_ItemCreated(object sender, ItemsCreatedEventArgs<ITypeItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var itemPaths = e.Items.Select(item => item.Path).ToArray();
            var arguments = e.Arguments.Select(item => item is TypeInfo typeInfo ? (TypeInfo?)typeInfo : null).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTypeItemsCreated(signatureDate, itemPaths, arguments));
        }

        private void TypeContext_ItemRenamed(object sender, ItemsRenamedEventArgs<ITypeItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var oldPaths = e.OldPaths;
            var itemNames = e.Items.Select(item => item.Name).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTypeItemsRenamed(signatureDate, oldPaths, itemNames));
        }

        private void TypeContext_ItemMoved(object sender, ItemsMovedEventArgs<ITypeItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var oldPaths = e.OldPaths;
            var parentPaths = e.Items.Select(item => item.Parent.Path).ToArray();
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTypeItemsMoved(signatureDate, oldPaths, parentPaths));
        }

        private void TypeContext_ItemDeleted(object sender, ItemsDeletedEventArgs<ITypeItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var itemPaths = e.ItemPaths;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTypeItemsDeleted(signatureDate, itemPaths));
        }

        private void TypeContext_ItemsAccessChanged(object sender, ItemsEventArgs<ITypeItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var values = new AccessInfo[e.Items.Length];
            for (var i = 0; i < e.Items.Length; i++)
            {
                var item = e.Items[i];
                var accessInfo = item.AccessInfo;
                if (item.AccessInfo.Path != item.Path)
                {
                    accessInfo = AccessInfo.Empty;
                    accessInfo.Path = item.Path;
                }
                values[i] = accessInfo;
            }
            var metaData = e.MetaData as object[];
            var changeType = (AccessChangeType)metaData[0];
            var memberIDs = metaData[1] as string[];
            var accessTypes = metaData[2] as AccessType[];

            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTypeItemsAccessChanged(signatureDate, changeType, values, memberIDs, accessTypes));
        }

        private void TypeContext_ItemsLockChanged(object sender, ItemsEventArgs<ITypeItem> e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var values = new LockInfo[e.Items.Length];
            for (var i = 0; i < e.Items.Length; i++)
            {
                var item = e.Items[i];
                var lockInfo = item.LockInfo;
                if (item.LockInfo.Path != item.Path)
                {
                    lockInfo = LockInfo.Empty;
                    lockInfo.Path = item.Path;
                }
                values[i] = lockInfo;
            }
            var metaData = e.MetaData as object[];
            var changeType = (LockChangeType)metaData[0];
            var comments = metaData[1] as string[];
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnTypeItemsLockChanged(signatureDate, changeType, values, comments));
        }

        private void DataBase_Unloaded(object sender, EventArgs e)
        {
            this.dataBase = null;
        }

        private void AttachEventHandlers()
        {
            this.cremaHost.Dispatcher.VerifyAccess();

            this.TableContext.Tables.TablesStateChanged += Tables_TablesStateChanged;
            this.TableContext.Tables.TablesChanged += Tables_TablesChanged;
            this.TableContext.ItemsCreated += TableContext_ItemCreated;
            this.TableContext.ItemsRenamed += TableContext_ItemRenamed;
            this.TableContext.ItemsMoved += TableContext_ItemMoved;
            this.TableContext.ItemsDeleted += TableContext_ItemDeleted;
            this.TableContext.ItemsAccessChanged += TableContext_ItemsAccessChanged;
            this.TableContext.ItemsLockChanged += TableContext_ItemsLockChanged;

            this.TypeContext.Types.TypesStateChanged += Types_TypesStateChanged;
            this.TypeContext.Types.TypesChanged += Types_TypesChanged;
            this.TypeContext.ItemsCreated += TypeContext_ItemCreated;
            this.TypeContext.ItemsRenamed += TypeContext_ItemRenamed;
            this.TypeContext.ItemsMoved += TypeContext_ItemMoved;
            this.TypeContext.ItemsDeleted += TypeContext_ItemDeleted;
            this.TypeContext.ItemsAccessChanged += TypeContext_ItemsAccessChanged;
            this.TypeContext.ItemsLockChanged += TypeContext_ItemsLockChanged;

            this.dataBase.Unloaded += DataBase_Unloaded;

            this.logService.Debug($"[{this.OwnerID}] {nameof(DataBaseService)} {nameof(AttachEventHandlers)}");
        }

        private void DetachEventHandlers()
        {
            this.cremaHost.Dispatcher.VerifyAccess();

            if (this.dataBase == null || this.dataBase.IsLoaded == false)
                return;

            this.TableContext.Tables.TablesStateChanged -= Tables_TablesStateChanged;
            this.TableContext.Tables.TablesChanged -= Tables_TablesChanged;
            this.TableContext.ItemsCreated -= TableContext_ItemCreated;
            this.TableContext.ItemsRenamed -= TableContext_ItemRenamed;
            this.TableContext.ItemsMoved -= TableContext_ItemMoved;
            this.TableContext.ItemsDeleted -= TableContext_ItemDeleted;
            this.TableContext.ItemsAccessChanged -= TableContext_ItemsAccessChanged;
            this.TableContext.ItemsLockChanged -= TableContext_ItemsLockChanged;

            this.TypeContext.Types.TypesStateChanged -= Types_TypesStateChanged;
            this.TypeContext.Types.TypesChanged -= Types_TypesChanged;
            this.TypeContext.ItemsCreated -= TypeContext_ItemCreated;
            this.TypeContext.ItemsRenamed -= TypeContext_ItemRenamed;
            this.TypeContext.ItemsMoved -= TypeContext_ItemMoved;
            this.TypeContext.ItemsDeleted -= TypeContext_ItemDeleted;
            this.TypeContext.ItemsAccessChanged -= TypeContext_ItemsAccessChanged;
            this.TypeContext.ItemsLockChanged -= TypeContext_ItemsLockChanged;

            this.dataBase.Unloaded -= DataBase_Unloaded;

            this.logService.Debug($"[{this.OwnerID}] {nameof(DataBaseService)} {nameof(DetachEventHandlers)}");
        }

        private ResultBase Invoke(Action action)
        {
            var result = new ResultBase();
            try
            {
                this.cremaHost.Dispatcher.Invoke(action);
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault() { ExceptionType = e.GetType().Name, Message = e.Message };
            }
            return result;
        }

        private ResultBase<T> Invoke<T>(Func<T> func)
        {
            var result = new ResultBase<T>();
            try
            {
                result.Value = this.cremaHost.Dispatcher.Invoke(func);
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault() { ExceptionType = e.GetType().Name, Message = e.Message };
            }
            return result;
        }

        private ResultBase<T> InvokeImmediately<T>(Func<T> func)
        {
            var result = new ResultBase<T>();
            try
            {
                result.Value = func();
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault() { ExceptionType = e.GetType().Name, Message = e.Message };
            }
            return result;
        }

        private ITypeItem GetTypeItem(string itemPath)
        {
            var item = this.TypeContext[itemPath];
            if (item == null)
                throw new ItemNotFoundException(itemPath);
            return item;
        }

        private IType GetType(string typeName)
        {
            var type = this.TypeContext.Types[typeName];
            if (type == null)
                throw new TypeNotFoundException(typeName);
            return type;
        }

        private ITypeCategory GetTypeCategory(string categoryPath)
        {
            var category = this.TypeContext.Categories[categoryPath];
            if (category == null)
                throw new CategoryNotFoundException(categoryPath);
            return category;
        }

        private ITableItem GetTableItem(string itemPath)
        {
            var item = this.TableContext[itemPath];
            if (item == null)
                throw new ItemNotFoundException(itemPath);
            return item;
        }

        private ITable GetTable(string tableName)
        {
            var table = this.TableContext.Tables[tableName];
            if (table == null)
                throw new TableNotFoundException(tableName);
            return table;
        }

        private ITableCategory GetTableCategory(string categoryPath)
        {
            var category = this.TableContext.Categories[categoryPath];
            if (category == null)
                throw new CategoryNotFoundException(categoryPath);
            return category;
        }

        private ITableContext TableContext => this.dataBase.TableContext;

        private ITypeContext TypeContext => this.dataBase.TypeContext;

        #region ICremaServiceItem

        void ICremaServiceItem.Abort(bool disconnect)
        {
            this.cremaHost.Dispatcher.Invoke(() =>
            {
                this.DetachEventHandlers();
                this.dataBase = null;
            });
            this.userContext.Dispatcher.Invoke(() =>
            {
                this.userContext.Users.UsersLoggedOut -= Users_UsersLoggedOut;
                this.authentication = null;
            });
            CremaService.Dispatcher.Invoke(() =>
            {
                if (disconnect == false)
                {
                    this.Callback?.OnServiceClosed(SignatureDate.Empty, CloseInfo.Empty);
                    try
                    {
                        this.Channel?.Close(TimeSpan.FromSeconds(10));
                    }
                    catch
                    {
                        this.Channel?.Abort();
                    }
                }
                else
                {
                    this.Channel?.Abort();
                }
            });
        }

        #endregion
    }
}
