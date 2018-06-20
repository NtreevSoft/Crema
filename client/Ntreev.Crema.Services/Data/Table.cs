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
using Ntreev.Crema.Services.DataBaseService;
using Ntreev.Library;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Ntreev.Crema.Services.Data
{
    class Table : TableBase<Table, TableCategory, TableCollection, TableCategoryCollection, TableContext>,
        ITable, ITableItem, IInfoProvider, IStateProvider
    {
        public Table()
        {
            this.Template = new TableTemplate(this);
            this.Content = new TableContent(this);
        }

        public Table AddNew(Authentication authentication, TableInfo tableInfo)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.Container.InvokeChildTableCreate(authentication, this);
                var tableName = tableInfo.TableName;
                var childTable = this.Container.AddNew(authentication, tableInfo.Name, tableInfo.CategoryPath);
                childTable.Initialize(tableInfo);
                foreach (var item in this.DerivedTables)
                {
                    var derivedInfo = tableInfo;
                    derivedInfo.Name = CremaDataTable.GenerateName(item.Name, tableName);
                    derivedInfo.CategoryPath = item.Category.Path;
                    derivedInfo.TemplatedParent = childTable.Name;

                    var derivedChild = this.Container.AddNew(authentication, derivedInfo.Name, derivedInfo.CategoryPath);
                    derivedChild.TemplatedParent = childTable;
                    derivedChild.Initialize(derivedInfo);
                }

                var items = EnumerableUtility.Friends(childTable, childTable.DerivedTables).ToArray();
                this.Container.InvokeTablesCreatedEvent(authentication, items);
                return childTable;
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public AccessType GetAccessType(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            return base.GetAccessType(authentication);
        }

        public void SetPublic(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(SetPublic), this);
                var result = this.Service.SetPublicTableItem(base.Path);
                this.Sign(authentication, result);
                this.Context.InvokeTableItemSetPublic(authentication, this, this.AccessInfo);
                base.SetPublic(authentication);
                this.Context.InvokeItemsSetPublicEvent(authentication, new ITableItem[] { this });
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetPrivate(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(SetPrivate), this);
                var result = this.Service.SetPrivateTableItem(base.Path);
                this.Sign(authentication, result);
                this.Context.InvokeTableItemSetPrivate(authentication, this, AccessInfo.Empty);
                base.SetPrivate(authentication);
                this.Context.InvokeItemsSetPrivateEvent(authentication, new ITableItem[] { this });
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void AddAccessMember(Authentication authentication, string memberID, AccessType accessType)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(AddAccessMember), this, memberID, accessType);
                var result = this.Service.AddAccessMemberTableItem(base.Path, memberID, accessType);
                this.Sign(authentication, result);
                this.Context.InvokeTableItemAddAccessMember(authentication, this, this.AccessInfo, memberID, accessType);
                base.AddAccessMember(authentication, memberID, accessType);
                this.Context.InvokeItemsAddAccessMemberEvent(authentication, new ITableItem[] { this }, new string[] { memberID }, new AccessType[] { accessType });
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetAccessMember(Authentication authentication, string memberID, AccessType accessType)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(SetAccessMember), this, memberID, accessType);
                var result = this.Service.SetAccessMemberTableItem(base.Path, memberID, accessType);
                this.Sign(authentication, result);
                this.Context.InvokeTableItemSetAccessMember(authentication, this, this.AccessInfo, memberID, accessType);
                base.SetAccessMember(authentication, memberID, accessType);
                this.Context.InvokeItemsSetAccessMemberEvent(authentication, new ITableItem[] { this }, new string[] { memberID }, new AccessType[] { accessType });
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void RemoveAccessMember(Authentication authentication, string memberID)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(RemoveAccessMember), this, memberID);
                var result = this.Service.RemoveAccessMemberTableItem(base.Path, memberID);
                this.Sign(authentication, result);
                this.Context.InvokeTableItemRemoveAccessMember(authentication, this, this.AccessInfo, memberID);
                base.RemoveAccessMember(authentication, memberID);
                this.Context.InvokeItemsRemoveAccessMemberEvent(authentication, new ITableItem[] { this }, new string[] { memberID });
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Lock(Authentication authentication, string comment)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(Lock), this, comment);
                var result = this.Service.LockTableItem(base.Path, comment);
                this.Sign(authentication, result);
                this.Context.InvokeTableItemLock(authentication, this, comment);
                base.Lock(authentication, comment);
                this.Context.InvokeItemsLockedEvent(authentication, new ITableItem[] { this }, new string[] { comment });
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Unlock(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(Unlock), this);
                var result = this.Service.UnlockTableItem(base.Path);
                this.Sign(authentication, result);
                this.Context.InvokeTableItemUnlock(authentication, this);
                base.Unlock(authentication);
                this.Context.InvokeItemsUnlockedEvent(authentication, new ITableItem[] { this });
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Rename(Authentication authentication, string name)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(Rename), this, name);
                var result = this.Service.RenameTableItem(base.Path, name);
                this.Sign(authentication, result);
                var items = this.Parent == null ? EnumerableUtility.Friends(this, this.Childs).ToArray() : EnumerableUtility.Friends(this, this.DerivedTables).ToArray();
                var oldNames = items.Select(item => item.Name).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                this.Container.InvokeTableRename(authentication, this, name);
                base.Rename(authentication, name);
                this.Container.InvokeTablesRenamedEvent(authentication, items, oldNames, oldPaths);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Move(Authentication authentication, string categoryPath)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(Move), this, categoryPath);
                var result = this.Service.MoveTableItem(base.Path, categoryPath);
                this.Sign(authentication, result);
                var items = EnumerableUtility.Friends(this, this.Childs).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                var oldCategoryPaths = items.Select(item => item.Category.Path).ToArray();
                this.Container.InvokeTableMove(authentication, this, categoryPath);
                base.Move(authentication, categoryPath);
                this.Container.InvokeTablesMovedEvent(authentication, items, oldPaths, oldCategoryPaths);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Delete(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(Delete), this, base.Path);
                var result = this.Service.DeleteTableItem(base.Path);
                this.Sign(authentication, result);
                var items = this.Parent == null ? EnumerableUtility.Friends(this, this.Childs).ToArray() : EnumerableUtility.Friends(this, this.DerivedTables).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                var container = this.Container;
                container.InvokeTableDelete(authentication, this);
                base.Delete(authentication);
                container.InvokeTablesDeletedEvent(authentication, items, oldPaths);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetProperty(Authentication authentication, string propertyName, string value)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(SetProperty), this, propertyName, value);
                if (propertyName == CremaSchema.Tags)
                {
                    this.SetTags(authentication, (TagInfo)value);
                }
                else if (propertyName == CremaSchema.Comment)
                {
                    this.SetComment(authentication, value);
                }
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetTags(Authentication authentication, TagInfo tags)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(SetTags), this, tags);
                var result = this.Service.SetTableItemProperty(base.Path, CremaSchema.Tags, tags.ToString());
                this.Sign(authentication, result);
                var items = EnumerableUtility.Friends(this, this.Childs).SelectMany(item => item.DerivedTables).ToArray();
                this.Container.InvokeTableSetTags(authentication, this, tags);
                this.UpdateTags(tags);
                this.Container.InvokeTablesTemplateChangedEvent(authentication, items);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetComment(Authentication authentication, string comment)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(SetComment), this, comment);
                var result = this.Service.SetTableItemProperty(base.Path, CremaSchema.Comment, comment);
                this.Sign(authentication, result);
                var items = EnumerableUtility.Friends(this, this.Childs).SelectMany(item => item.DerivedTables).ToArray();
                this.Container.InvokeTableSetComment(authentication, this, comment);
                this.UpdateComment(comment);
                this.Container.InvokeTablesTemplateChangedEvent(authentication, items);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public Table Copy(Authentication authentication, string newTableName, string categoryPath, bool copyContent)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(Copy), this, newTableName, categoryPath, copyContent);
                return this.Container.Copy(authentication, this, newTableName, categoryPath, copyContent);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public Table Inherit(Authentication authentication, string newTableName, string categoryPath, bool copyContent)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(Inherit), this, newTableName, categoryPath, copyContent);
                return this.Container.Inherit(authentication, this, newTableName, categoryPath, copyContent);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public NewChildTableTemplate NewChild(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(NewChild), this);
                var template = new NewChildTableTemplate(this);
                template.BeginEdit(authentication);
                return template;
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public CremaDataSet GetDataSet(Authentication authentication, string revision)
        {
            try
            {
                this.DataBase.ValidateAsyncBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(GetDataSet), this, revision);
                var result = this.Service.GetTableItemDataSet(base.Path, revision);
                this.Sign(authentication, result);
                return result.Value;
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public LogInfo[] GetLog(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateAsyncBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(GetLog), this);
                var result = this.Service.GetTableItemLog(base.Path);
                this.Sign(authentication, result);
                return result.Value ?? new LogInfo[] { };
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public FindResultInfo[] Find(Authentication authentication, string text, FindOptions options)
        {
            try
            {
                this.DataBase.ValidateAsyncBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(Find), this, text, options);
                var result = this.Service.FindTableItem(base.Path, text, options);
                this.Sign(authentication, result);
                return result.Value ?? new FindResultInfo[] { };
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public object GetService(System.Type serviceType)
        {
            return this.DataBase.GetService(serviceType);
        }

        public TableTemplate Template { get; }

        public TableContent Content { get; }

        public CremaDispatcher Dispatcher => this.Context?.Dispatcher;

        public IDataBaseService Service => this.Context.Service;

        public CremaHost CremaHost => this.Context.CremaHost;

        public DataBase DataBase => this.Context.DataBase;

        public new string Name
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.Name;
            }
        }

        public new string TableName
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.TableName;
            }
        }

        public new string Path
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.Path;
            }
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

        public new TableInfo TableInfo
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.TableInfo;
            }
        }

        public new TableState TableState
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.TableState;
            }
        }

        public new TagInfo Tags
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.Tags;
            }
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

        public new event EventHandler Moved
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.Moved += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.Moved -= value;
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

        public new event EventHandler LockChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.LockChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.LockChanged -= value;
            }
        }

        public new event EventHandler AccessChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.AccessChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.AccessChanged -= value;
            }
        }

        public new event EventHandler TableInfoChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.TableInfoChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.TableInfoChanged -= value;
            }
        }

        public new event EventHandler TableStateChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.TableStateChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.TableStateChanged -= value;
            }
        }

        private void Sign(Authentication authentication, ResultBase result)
        {
            result.Validate(authentication);
        }

        private void Sign<T>(Authentication authentication, ResultBase<T> result)
        {
            result.Validate(authentication);
        }

        #region Invisibles

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetName(string name)
        {
            base.Name = name;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetParent(TableCategory parent)
        {
            base.Category = parent;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetTableInfo(TableInfo tableInfo)
        {
            this.UpdateTableInfo(tableInfo);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetTableState(TableState tableState)
        {
            base.TableState = tableState;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetAccessInfo(AccessChangeType changeType, AccessInfo accessInfo)
        {
            if (changeType != AccessChangeType.Public)
                base.AccessInfo = accessInfo;
            else
                base.AccessInfo = AccessInfo.Empty;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetAccessInfo(AccessInfo accessInfo)
        {
            base.AccessInfo = accessInfo;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetLockInfo(LockChangeType changeType, LockInfo lockInfo)
        {
            if (changeType == LockChangeType.Lock)
                base.LockInfo = lockInfo;
            else
                base.LockInfo = LockInfo.Empty;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetLockInfo(LockInfo lockInfo)
        {
            base.LockInfo = lockInfo;
        }

        #endregion

        #region ITable

        ITable ITable.Copy(Authentication authentication, string newTableName, string categoryPath, bool copyContent)
        {
            return this.Copy(authentication, newTableName, categoryPath, copyContent);
        }

        ITable ITable.Inherit(Authentication authentication, string newTableName, string categoryPath, bool copyContent)
        {
            return this.Inherit(authentication, newTableName, categoryPath, copyContent);
        }

        ITableTemplate ITable.NewTable(Authentication authentication)
        {
            return this.NewChild(authentication);
        }

        ITable ITable.TemplatedParent
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.TemplatedParent;
            }
        }

        ITableCategory ITable.Category
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Category;
            }
        }

        ITable ITable.Parent
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Parent;
            }
        }

        ITableTemplate ITable.Template
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Template;
            }
        }

        ITableContent ITable.Content
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Content;
            }
        }

        IContainer<ITable> ITable.Childs
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Childs;
            }
        }

        IContainer<ITable> ITable.DerivedTables
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.DerivedTables;
            }
        }

        #endregion

        #region ITableItem

        ITableItem ITableItem.Parent
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                if (this.Parent == null)
                    return this.Category;
                return this.Parent;
            }
        }

        IEnumerable<ITableItem> ITableItem.Childs
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Childs;
            }
        }

        #endregion

        #region IServiceProvider

        object IServiceProvider.GetService(System.Type serviceType)
        {
            return (this.DataBase as IDataBase).GetService(serviceType);
        }

        #endregion

        #region IInfoProvider

        IDictionary<string, object> IInfoProvider.Info => this.TableInfo.ToDictionary();

        #endregion

        #region IStateProvider

        object IStateProvider.State => this.TableState;

        #endregion
    }
}
