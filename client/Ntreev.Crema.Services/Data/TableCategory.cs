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
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.DataBaseService;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Services.Data
{
    class TableCategory : TableCategoryBase<Table, TableCategory, TableCollection, TableCategoryCollection, TableContext>,
        ITableCategory, ITableItem
    {
        public TableCategory()
        {

        }

        public AccessType GetAccessType(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            return base.GetAccessType(authentication);
        }

        public void SetPublic(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(SetPublic), this);
            var result = this.Service.SetPublicTableItem(base.Path);
            this.Sign(authentication, result);
            this.Context.InvokeTableItemSetPublic(authentication, this, this.AccessInfo);
            base.SetPublic(authentication);
            this.Context.InvokeItemsSetPublicEvent(authentication, new ITableItem[] { this, });
        }

        public void SetPrivate(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(SetPrivate), this);
            var result = this.Service.SetPrivateTableItem(base.Path);
            this.Sign(authentication, result);
            this.Context.InvokeTableItemSetPrivate(authentication, this, AccessInfo.Empty);
            base.SetPrivate(authentication);
            this.Context.InvokeItemsSetPrivateEvent(authentication, new ITableItem[] { this, });
        }

        public void AddAccessMember(Authentication authentication, string memberID, AccessType accessType)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(AddAccessMember), this, memberID, accessType);
            var result = this.Service.AddAccessMemberTableItem(base.Path, memberID, accessType);
            this.Sign(authentication, result);
            this.Context.InvokeTableItemAddAccessMember(authentication, this, this.AccessInfo, memberID, accessType);
            base.AddAccessMember(authentication, memberID, accessType);
            this.Context.InvokeItemsAddAccessMemberEvent(authentication, new ITableItem[] { this, }, new string[] { memberID, }, new AccessType[] { accessType, });
        }

        public void SetAccessMember(Authentication authentication, string memberID, AccessType accessType)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(SetAccessMember), this, memberID, accessType);
            var result = this.Service.SetAccessMemberTableItem(base.Path, memberID, accessType);
            this.Sign(authentication, result);
            this.Context.InvokeTableItemSetAccessMember(authentication, this, this.AccessInfo, memberID, accessType);
            base.SetAccessMember(authentication, memberID, accessType);
            this.Context.InvokeItemsSetAccessMemberEvent(authentication, new ITableItem[] { this, }, new string[] { memberID, }, new AccessType[] { accessType, });
        }

        public void RemoveAccessMember(Authentication authentication, string memberID)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(RemoveAccessMember), this, memberID);
            var result = this.Service.RemoveAccessMemberTableItem(base.Path, memberID);
            this.Sign(authentication, result);
            this.Context.InvokeTableItemRemoveAccessMember(authentication, this, this.AccessInfo, memberID);
            base.RemoveAccessMember(authentication, memberID);
            this.Context.InvokeItemsRemoveAccessMemberEvent(authentication, new ITableItem[] { this, }, new string[] { memberID, });
        }

        public void Lock(Authentication authentication, string comment)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Lock), this, comment);
            var result = this.Service.LockTableItem(base.Path, comment);
            this.Sign(authentication, result);
            this.Context.InvokeTableItemLock(authentication, this, comment);
            base.Lock(authentication, comment);
            this.Context.InvokeItemsLockedEvent(authentication, new ITableItem[] { this, }, new string[] { comment });
        }

        public void Unlock(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Unlock), this);
            var result = this.Service.UnlockTableItem(base.Path);
            this.Sign(authentication, result);
            this.Context.InvokeTableItemUnlock(authentication, this);
            base.Unlock(authentication);
            this.Context.InvokeItemsUnlockedEvent(authentication, new ITableItem[] { this, });
        }

        public void Rename(Authentication authentication, string name)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Rename), this, name);
            var result = this.Service.RenameTableItem(base.Path, name);
            this.Sign(authentication, result);
            var items = EnumerableUtility.One(this).ToArray();
            var oldNames = items.Select(item => item.Name).ToArray();
            var oldPaths = items.Select(item => item.Path).ToArray();
            this.Container.InvokeCategoryRename(authentication, this, name);
            base.Rename(authentication, name);
            this.Container.InvokeCategoriesRenamedEvent(authentication, items, oldNames, oldPaths);
        }

        public void Move(Authentication authentication, string parentPath)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Move), this, parentPath);
            var result = this.Service.MoveTableItem(base.Path, parentPath);
            this.Sign(authentication, result);
            var items = EnumerableUtility.One(this).ToArray();
            var oldPaths = items.Select(item => item.Path).ToArray();
            var oldParentPaths = items.Select(item => item.Parent.Path).ToArray();
            this.Container.InvokeCategoryMove(authentication, this, parentPath);
            base.Move(authentication, parentPath);
            this.Container.InvokeCategoriesMovedEvent(authentication, items, oldPaths, oldParentPaths);
        }

        public void Delete(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Delete), this);
            var result = this.Service.DeleteTableItem(base.Path);
            this.Sign(authentication, result);
            var items = EnumerableUtility.One(this).ToArray();
            var oldPaths = items.Select(item => item.Path).ToArray();
            var container = this.Container;
            container.InvokeCategoryDelete(authentication, this);
            base.Delete(authentication);
            container.InvokeCategoriesDeletedEvent(authentication, items, oldPaths);
        }

        public void SetProperty(Authentication authentication, string propertyName, string value)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(SetProperty), this, propertyName, value);
            var result = this.Service.SetTableItemProperty(base.Path, propertyName, value);
            this.Sign(authentication, result);
        }
        
        public NewTableTemplate NewTable(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(NewTable), this);
            var template = new NewTableTemplate(this);
            template.BeginEdit(authentication);
            return template;
        }

        public CremaDataSet GetDataSet(Authentication authentication, string revision)
        {
            this.DataBase.ValidateAsyncBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(GetDataSet), this, revision);
            var result = this.Service.GetTableItemDataSet(base.Path, revision);
            this.Sign(authentication, result); 
            return result.Value;
        }

        public LogInfo[] GetLog(Authentication authentication)
        {
            this.DataBase.ValidateAsyncBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(GetLog), this);
            var result = this.Service.GetTableItemLog(base.Path);
            this.Sign(authentication, result);
            return result.Value ?? new LogInfo[] { };
        }

        public FindResultInfo[] Find(Authentication authentication, string text, FindOptions options)
        {
            this.DataBase.ValidateAsyncBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Find), this, text, options);
            var result = this.Context.Service.FindTableItem(base.Path, text, options);
            this.Sign(authentication, result);
            return result.Value ?? new FindResultInfo[] { };
        }

        public object GetService(System.Type serviceType)
        {
            return this.DataBase.GetService(serviceType);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetName(string name)
        {
            base.Name = name;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetParent(TableCategory parent)
        {
            base.Parent = parent;
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

        public CremaHost CremaHost
        {
            get { return this.Context.DataBase.CremaHost; }
        }

        public DataBase DataBase
        {
            get { return this.Context.DataBase; }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.Context?.Dispatcher; }
        }

        public IDataBaseService Service
        {
            get { return this.Context.Service; }
        }

        public new string Name
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.Name;
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

        private void Sign(Authentication authentication, ResultBase result)
        {
            result.Validate(authentication);
        }

        private void Sign<T>(Authentication authentication, ResultBase<T> result)
        {
            result.Validate(authentication);
        }

        #region ITableCategory

        ITableCategory ITableCategory.AddNewCategory(Authentication authentication, string name)
        {
            return this.Container.AddNew(authentication, name, base.Path);
        }

        ITableTemplate ITableCategory.NewTable(Authentication authentication)
        {
            return this.NewTable(authentication);
        }

        ITableCategory ITableCategory.Parent
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Parent;
            }
        }

        IContainer<ITableCategory> ITableCategory.Categories
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Categories;
            }
        }

        IContainer<ITable> ITableCategory.Tables
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Tables;
            }
        }

        #endregion

        #region ITableItem

        ITableItem ITableItem.Parent
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Parent;
            }
        }

        IEnumerable<ITableItem> ITableItem.Childs
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                foreach (var item in this.Categories)
                {
                    yield return item;
                }
                foreach (var item in this.Items)
                {
                    yield return item;
                }
            }
        }

        #endregion

        #region IServiceProvider

        object IServiceProvider.GetService(System.Type serviceType)
        {
            return (this.DataBase as IDataBase).GetService(serviceType);
        }

        #endregion
    }
}
