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
using Ntreev.Library;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Services.Data
{
    class TypeCategory : TypeCategoryBase<Type, TypeCategory, TypeCollection, TypeCategoryCollection, TypeContext>,
        ITypeCategory, ITypeItem
    {
        public TypeCategory()
        {

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
                var result = this.Context.Service.SetPublicTypeItem(base.Path);
                this.Sign(authentication, result);
                this.Context.InvokeTypeItemSetPublic(authentication, this, this.AccessInfo);
                base.SetPublic(authentication);
                this.Context.InvokeItemsSetPublicEvent(authentication, new ITypeItem[] { this });
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
                var result = this.Context.Service.SetPrivateTypeItem(base.Path);
                this.Sign(authentication, result);
                this.Context.InvokeTypeItemSetPrivate(authentication, this, AccessInfo.Empty);
                base.SetPrivate(authentication);
                this.Context.InvokeItemsSetPrivateEvent(authentication, new ITypeItem[] { this });
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
                var result = this.Context.Service.AddAccessMemberTypeItem(base.Path, memberID, accessType);
                this.Sign(authentication, result);
                this.Context.InvokeTypeItemAddAccessMember(authentication, this, this.AccessInfo, memberID, accessType);
                base.AddAccessMember(authentication, memberID, accessType);
                this.Context.InvokeItemsAddAccessMemberEvent(authentication, new ITypeItem[] { this }, new string[] { memberID }, new AccessType[] { accessType });
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
                var result = this.Context.Service.SetAccessMemberTypeItem(base.Path, memberID, accessType);
                this.Sign(authentication, result);
                this.Context.InvokeTypeItemSetAccessMember(authentication, this, this.AccessInfo, memberID, accessType);
                base.SetAccessMember(authentication, memberID, accessType);
                this.Context.InvokeItemsSetAccessMemberEvent(authentication, new ITypeItem[] { this }, new string[] { memberID }, new AccessType[] { accessType });
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
                var result = this.Context.Service.RemoveAccessMemberTypeItem(base.Path, memberID);
                this.Sign(authentication, result);
                this.Context.InvokeTypeItemRemoveAccessMember(authentication, this, this.AccessInfo, memberID);
                base.RemoveAccessMember(authentication, memberID);
                this.Context.InvokeItemsRemoveAccessMemberEvent(authentication, new ITypeItem[] { this }, new string[] { memberID });
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
                var result = this.Context.Service.LockTypeItem(base.Path, comment);
                this.Sign(authentication, result);
                this.Context.InvokeTypeItemLock(authentication, this, comment);
                base.Lock(authentication, comment);
                this.Context.InvokeItemsLockedEvent(authentication, new ITypeItem[] { this }, new string[] { comment });
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
                var result = this.Context.Service.UnlockTypeItem(base.Path);
                this.Sign(authentication, result);
                this.Context.InvokeTypeItemUnlock(authentication, this);
                base.Unlock(authentication);
                this.Context.InvokeItemsUnlockedEvent(authentication, new ITypeItem[] { this });
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
                var result = this.Service.RenameTypeItem(base.Path, name);
                this.Sign(authentication, result);
                var items = EnumerableUtility.One(this).ToArray();
                var oldNames = items.Select(item => item.Name).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                this.Container.InvokeCategoryRename(authentication, this, name);
                base.Rename(authentication, name);
                this.Container.InvokeCategoriesRenamedEvent(authentication, items, oldNames, oldPaths);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void Move(Authentication authentication, string parentPath)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(Move), this, parentPath);
                var result = this.Service.MoveTypeItem(base.Path, parentPath);
                this.Sign(authentication, result);
                var items = EnumerableUtility.One(this).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                var oldParentPaths = items.Select(item => item.Parent.Path).ToArray();
                this.Container.InvokeCategoryMove(authentication, this, parentPath);
                base.Move(authentication, parentPath);
                this.Container.InvokeCategoriesMovedEvent(authentication, items, oldPaths, oldParentPaths);
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
                this.CremaHost.DebugMethod(authentication, this, nameof(Delete), this);
                var result = this.Service.DeleteTypeItem(base.Path);
                this.Sign(authentication, result);
                var items = EnumerableUtility.One(this).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                var container = this.Container;
                container.InvokeCategoryDelete(authentication, this);
                base.Delete(authentication);
                container.InvokeCategoriesDeletedEvent(authentication, items, oldPaths);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public TypeCategory AddNewCategory(Authentication authentication, string name)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                return this.Container.AddNew(authentication, name, base.Path);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public NewTypeTemplate NewType(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(NewType), this);
                var template = new NewTypeTemplate(this);
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
                var result = this.Service.GetTypeItemDataSet(base.Path, revision);
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
                var result = this.Service.GetTypeItemLog(base.Path);
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
                var result = this.Context.Service.FindTypeItem(base.Path, text, options);
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

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetName(string name)
        {
            base.Name = name;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetParent(TypeCategory parent)
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

        public CremaHost CremaHost => this.Context.CremaHost;

        public DataBase DataBase => this.Context.DataBase;

        public CremaDispatcher Dispatcher => this.Context?.Dispatcher;

        public IDataBaseService Service => this.Context.Service;

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

        #region ITypeCategory

        ITypeCategory ITypeCategory.AddNewCategory(Authentication authentication, string name)
        {
            return this.AddNewCategory(authentication, name);
        }

        ITypeTemplate ITypeCategory.NewType(Authentication authentication)
        {
            return this.NewType(authentication);
        }

        ITypeCategory ITypeCategory.Parent
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Parent;
            }
        }

        IContainer<ITypeCategory> ITypeCategory.Categories
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Categories;
            }
        }

        IContainer<IType> ITypeCategory.Types
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Types;
            }
        }

        #endregion

        #region ITypeItem

        ITypeItem ITypeItem.Parent
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Parent;
            }
        }

        IEnumerable<ITypeItem> ITypeItem.Childs
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
