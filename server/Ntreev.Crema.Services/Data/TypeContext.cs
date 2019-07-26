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
using Ntreev.Crema.Services.Properties;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ntreev.Crema.Services.Data
{
    class TypeContext : TypeContextBase<Type, TypeCategory, TypeCollection, TypeCategoryCollection, TypeContext>,
        ITypeContext
    {
        private DataBase dataBase;
        private readonly DataBaseRepositoryHost repository;
        private readonly string basePath;

        private ItemsCreatedEventHandler<ITypeItem> itemsCreated;
        private ItemsRenamedEventHandler<ITypeItem> itemsRenamed;
        private ItemsMovedEventHandler<ITypeItem> itemsMoved;
        private ItemsDeletedEventHandler<ITypeItem> itemsDeleted;
        private ItemsEventHandler<ITypeItem> itemsChanged;
        private ItemsEventHandler<ITypeItem> itemsAccessChanged;
        private ItemsEventHandler<ITypeItem> itemsLockChanged;

        public TypeContext(DataBase dataBase, IEnumerable<TypeInfo> typeInfos)
        {
            this.dataBase = dataBase;
            this.repository = dataBase.Repository;
            this.basePath = Path.Combine(dataBase.BasePath, CremaSchema.TypeDirectory);
            this.Initialize(typeInfos);
            this.CremaHost.UserContext.Dispatcher.Invoke(() => this.CremaHost.UserContext.Users.UsersLoggedOut += Users_UsersLoggedOut);
            this.CremaHost.Debug("TypeContext is created.");
        }

        public void InvokeTypeItemLock(Authentication authentication, ITypeItem typeItem, string comment)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeItemLock), typeItem, comment);
        }

        public void InvokeTypeItemUnlock(Authentication authentication, ITypeItem typeItem)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeItemUnlock), typeItem);
        }

        public void InvokeTypeItemSetPrivate(Authentication authentication, ITypeItem typeItem, AccessInfo accessInfo)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeItemSetPrivate), typeItem);
            var accessInfoPath = typeItem.GetAccessInfoPath();
            try
            {
                accessInfo.SetPrivate(typeItem.GetType().Name, authentication.SignatureDate);
                typeItem.WriteAccessInfo(accessInfoPath, accessInfo);
                this.repository.Add(accessInfoPath);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert();
                throw e;
            }
        }

        public void InvokeTypeItemSetPublic(Authentication authentication, ITypeItem typeItem, AccessInfo accessInfo)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeItemSetPrivate), typeItem);
            var accessInfoPath = typeItem.GetAccessInfoPath();
            try
            {
                accessInfo.SetPublic();
                this.repository.Delete(accessInfoPath);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert();
                throw e;
            }
        }

        public void InvokeTypeItemAddAccessMember(Authentication authentication, ITypeItem typeItem, AccessInfo accessInfo, string memberID, AccessType accessType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeItemAddAccessMember), typeItem, memberID, accessType);
            var accessInfoPath = typeItem.GetAccessInfoPath();
            try
            {
                accessInfo.Add(authentication.SignatureDate, memberID, accessType);
                typeItem.WriteAccessInfo(accessInfoPath, accessInfo);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert();
                throw e;
            }
        }

        public void InvokeTypeItemSetAccessMember(Authentication authentication, ITypeItem typeItem, AccessInfo accessInfo, string memberID, AccessType accessType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeItemSetAccessMember), typeItem, memberID, accessType);
            var accessInfoPath = typeItem.GetAccessInfoPath();
            try
            {
                accessInfo.Set(authentication.SignatureDate, memberID, accessType);
                typeItem.WriteAccessInfo(accessInfoPath, accessInfo);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert();
                throw e;
            }
        }

        public void InvokeTypeItemRemoveAccessMember(Authentication authentication, ITypeItem typeItem, AccessInfo accessInfo, string memberID)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeItemRemoveAccessMember), typeItem, memberID);
            var accessInfoPath = typeItem.GetAccessInfoPath();
            try
            {
                accessInfo.Remove(authentication.SignatureDate, memberID);
                typeItem.WriteAccessInfo(accessInfoPath, accessInfo);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.repository.Revert();
                throw e;
            }
        }

        public void InvokeTypeItemCreate(Authentication authentication, string path)
        {

        }

        public void InvokeTypeItemRename(Authentication authentication, ITypeItem typeItem, string newName)
        {
            if (typeItem.AccessInfo.Path == typeItem.Path)
            {
                var accessInfoPath1 = typeItem.GetAccessInfoPath();
                var directoryName = Path.GetDirectoryName(accessInfoPath1);
                var extension = Path.GetExtension(accessInfoPath1);
                var accessInfoPath2 = Path.Combine(directoryName, newName + extension);
                if (File.Exists(accessInfoPath1) == true && accessInfoPath1 != accessInfoPath2)
                {
                    this.repository.Move(accessInfoPath1, accessInfoPath2);
                }
            }
        }

        public void InvokeTypeItemMove(Authentication authentication, ITypeItem typeItem, string newCategoryPath)
        {
            if (typeItem.AccessInfo.Path == typeItem.Path)
            {
                var accessInfoPath1 = typeItem.GetAccessInfoPath();
                var accessInfoPath2 = typeItem.GetAccessInfoPath(newCategoryPath);
                if (File.Exists(accessInfoPath1) == true)
                {
                    this.repository.Move(accessInfoPath1, accessInfoPath2);
                }
            }
        }

        public void InvokeTypeItemDelete(Authentication authentication, ITypeItem typeItem)
        {
            if (typeItem.AccessInfo.Path == typeItem.Path)
            {
                var accessInfoPath = typeItem.GetAccessInfoPath();
                if (File.Exists(accessInfoPath) == true)
                {
                    this.repository.Delete(accessInfoPath);
                }
            }
        }

        public void InvokeTypeItemChange(Authentication authentication, ITypeItem typeItem)
        {

        }

        public void Import(Authentication authentication, CremaDataSet dataSet, string comment)
        {
            this.Dispatcher?.VerifyAccess();
            this.CremaHost.DebugMethod(authentication, this, nameof(Import), comment);
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            var userContext = this.CremaHost.UserContext;
            this.dataBase = null;
            userContext.Dispatcher.Invoke(() => userContext.Users.UsersLoggedOut -= Users_UsersLoggedOut);
        }

        public object GetService(System.Type serviceType)
        {
            return this.DataBase.GetService(serviceType);
        }

        public void InvokeItemsSetPublicEvent(Authentication authentication, ITypeItem[] items)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsSetPublicEvent), items);
            var comment = EventMessageBuilder.SetPublicTypeItem(authentication, items);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Public);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITypeItem>(authentication, items, metaData));
        }

        public void InvokeItemsSetPrivateEvent(Authentication authentication, ITypeItem[] items)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsSetPrivateEvent), items);
            var comment = EventMessageBuilder.SetPrivateTypeItem(authentication, items);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Private);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITypeItem>(authentication, items, metaData));
        }

        public void InvokeItemsAddAccessMemberEvent(Authentication authentication, ITypeItem[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsAddAccessMemberEvent), items, memberIDs, accessTypes);
            var comment = EventMessageBuilder.AddAccessMemberToTypeItem(authentication, items, memberIDs, accessTypes);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Add, memberIDs, accessTypes);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITypeItem>(authentication, items, metaData));
        }

        public void InvokeItemsSetAccessMemberEvent(Authentication authentication, ITypeItem[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsSetAccessMemberEvent), items, memberIDs, accessTypes);
            var comment = EventMessageBuilder.SetAccessMemberOfTypeItem(authentication, items, memberIDs, accessTypes);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Set, memberIDs, accessTypes);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITypeItem>(authentication, items, metaData));
        }

        public void InvokeItemsRemoveAccessMemberEvent(Authentication authentication, ITypeItem[] items, string[] memberIDs)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsRemoveAccessMemberEvent), items, memberIDs);
            var comment = EventMessageBuilder.RemoveAccessMemberFromTypeItem(authentication, items, memberIDs);
            var metaData = EventMetaDataBuilder.Build(items, AccessChangeType.Remove, memberIDs);
            this.CremaHost.Debug(eventLog);
            this.repository.Commit(authentication, comment, eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsAccessChanged(new ItemsEventArgs<ITypeItem>(authentication, items, new object[] { AccessChangeType.Remove, memberIDs, }));
        }

        public void InvokeItemsLockedEvent(Authentication authentication, ITypeItem[] items, string[] comments)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsLockedEvent), items, comments);
            var comment = EventMessageBuilder.LockTypeItem(authentication, items, comments);
            var metaData = EventMetaDataBuilder.Build(items, LockChangeType.Lock, comments);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsLockChanged(new ItemsEventArgs<ITypeItem>(authentication, items, metaData));
        }

        public void InvokeItemsUnlockedEvent(Authentication authentication, ITypeItem[] items)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeItemsUnlockedEvent), items);
            var comment = EventMessageBuilder.UnlockTypeItem(authentication, items);
            var metaData = EventMetaDataBuilder.Build(items, LockChangeType.Unlock);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnItemsLockChanged(new ItemsEventArgs<ITypeItem>(authentication, items, metaData));
        }

        public void InvokeItemsCreatedEvent(Authentication authentication, ITypeItem[] items, object[] args, object metaData)
        {
            this.OnItemsCreated(new ItemsCreatedEventArgs<ITypeItem>(authentication, items, args, metaData));
        }

        public void InvokeItemsRenamedEvent(Authentication authentication, ITypeItem[] items, string[] oldNames, string[] oldPaths, object metaData)
        {
            this.OnItemsRenamed(new ItemsRenamedEventArgs<ITypeItem>(authentication, items, oldNames, oldPaths, metaData));
        }

        public void InvokeItemsMovedEvent(Authentication authentication, ITypeItem[] items, string[] oldPaths, string[] oldParentPaths, object metaData)
        {
            this.OnItemsMoved(new ItemsMovedEventArgs<ITypeItem>(authentication, items, oldPaths, oldParentPaths, metaData));
        }

        public void InvokeItemsDeleteEvent(Authentication authentication, ITypeItem[] items, string[] itemPaths, object metaData)
        {
            this.OnItemsDeleted(new ItemsDeletedEventArgs<ITypeItem>(authentication, items, itemPaths, metaData));
        }

        public void InvokeItemsChangedEvent(Authentication authentication, ITypeItem[] items, object metaData)
        {
            this.OnItemsChanged(new ItemsEventArgs<ITypeItem>(authentication, items, metaData));
        }

        public LogInfo[] GetLog(string schemaPath)
        {
            var revision = this.Repository.GetRevision(schemaPath);
            return this.Repository.GetLog(schemaPath, revision, 100);
        }

        public LogInfo[] GetCategoryLog(string localPath)
        {
            var revision = this.Repository.GetRevision(localPath);
            return this.Repository.GetLog(localPath, revision, 100);
        }

        public string GenerateTypePath(string categoryPath, string typeName)
        {
            NameValidator.ValidateCategoryPath(categoryPath);
            return PathUtility.ConvertFromUri(this.basePath + categoryPath + typeName + CremaSchema.SchemaExtension);
        }

        public string GenerateCategoryPath(string categoryPath)
        {
            NameValidator.ValidateCategoryPath(categoryPath);
            var baseUri = new Uri(this.basePath);
            var uri = new Uri(baseUri + categoryPath.TrimEnd(PathUtility.SeparatorChar));
            return uri.LocalPath;
        }

        public string GenerateCategoryPath(string parentPath, string name)
        {
            var categoryName = new CategoryName(parentPath, name);
            return this.GenerateCategoryPath(categoryName.Path);
        }

        public void ValidateTypePath(string categoryPath, string name)
        {
            this.DataBase.ValidateFileInfo(this.GenerateTypePath(categoryPath, name));
        }

        public void ValidateCategoryPath(string categoryPath)
        {
            this.DataBase.ValidateDirectoryInfo(this.GenerateCategoryPath(categoryPath));
        }

        public void UnlockItems(Authentication authentication, string userID)
        {
            this.ValidateUnlockItems(authentication, userID);

            var query = from ITypeItem item in this
                        where item.IsLocked
                        let lockInfo = item.LockInfo
                        where lockInfo.Path == item.Path && lockInfo.UserID == userID
                        select item;

            var items = query.ToArray();
            if (items.Any() == false)
                return;

            foreach (var item in items)
            {
                item.Unlock(authentication);
            }

            authentication.Sign();
            this.OnItemsLockChanged(new ItemsEventArgs<ITypeItem>(authentication, items));
        }

        public TypeCollection Types
        {
            get { return this.Items; }
        }

        public string BasePath
        {
            get { return this.basePath; }
        }

        public DataBaseRepositoryHost Repository
        {
            get { return this.DataBase.Repository; }
        }

        public CremaHost CremaHost
        {
            get { return this.DataBase.CremaHost; }
        }

        public DataBase DataBase
        {
            get
            {
                if (this.dataBase == null)
                    throw new InvalidOperationException(Resources.Exception_InvalidObject);
                return this.dataBase;
            }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.dataBase?.Dispatcher; }
        }

        public new ITypeItem this[string itemPath]
        {
            get { return base[itemPath] as ITypeItem; }
        }

        public event ItemsCreatedEventHandler<ITypeItem> ItemsCreated
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsCreated += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsCreated -= value;
            }
        }

        public event ItemsRenamedEventHandler<ITypeItem> ItemsRenamed
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsRenamed += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsRenamed -= value;
            }
        }

        public event ItemsMovedEventHandler<ITypeItem> ItemsMoved
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsMoved += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsMoved -= value;
            }
        }

        public event ItemsDeletedEventHandler<ITypeItem> ItemsDeleted
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsDeleted += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsDeleted -= value;
            }
        }

        public event ItemsEventHandler<ITypeItem> ItemsChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsChanged -= value;
            }
        }

        public event ItemsEventHandler<ITypeItem> ItemsAccessChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsAccessChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsAccessChanged -= value;
            }
        }

        public event ItemsEventHandler<ITypeItem> ItemsLockChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsLockChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.itemsLockChanged -= value;
            }
        }

        protected virtual void OnItemsCreated(ItemsCreatedEventArgs<ITypeItem> e)
        {
            this.itemsCreated?.Invoke(this, e);
        }

        protected virtual void OnItemsRenamed(ItemsRenamedEventArgs<ITypeItem> e)
        {
            this.itemsRenamed?.Invoke(this, e);
        }

        protected virtual void OnItemsMoved(ItemsMovedEventArgs<ITypeItem> e)
        {
            this.itemsMoved?.Invoke(this, e);
        }

        protected virtual void OnItemsDeleted(ItemsDeletedEventArgs<ITypeItem> e)
        {
            this.itemsDeleted?.Invoke(this, e);
        }

        protected virtual void OnItemsChanged(ItemsEventArgs<ITypeItem> e)
        {
            this.itemsChanged?.Invoke(this, e);
        }

        protected virtual void OnItemsAccessChanged(ItemsEventArgs<ITypeItem> e)
        {
            this.itemsAccessChanged?.Invoke(this, e);
        }

        protected virtual void OnItemsLockChanged(ItemsEventArgs<ITypeItem> e)
        {
            this.itemsLockChanged?.Invoke(this, e);
        }

        protected override IEnumerable<ITableInfoProvider> GetTables()
        {
            return this.DataBase.TableContext.Tables;
        }

        private void Users_UsersLoggedOut(object sender, ItemsEventArgs<AuthenticationInfo> e)
        {
            var userIDs = e.Items.Select(item => item.ID).ToArray();
            this.CremaHost.Dispatcher.InvokeAsync(() =>
            {
                if (this.dataBase == null)
                    return;
                foreach (var item in userIDs)
                {
                    this.UnlockItems(Authentication.System, item);
                }
            });
        }

        private void ValidateLockItems(Authentication authentication, string[] itemPaths)
        {
            var items = itemPaths.Select(item => this[item]);

            foreach (var item in items)
            {
                var permission = item as IPermission;

                if (permission.VerifyAccessType(authentication, AccessType.Master) == false)
                    throw new PermissionDeniedException();
            }
        }

        private void ValidateUnlockItems(Authentication authentication, string userID)
        {
            foreach (ITypeItem item in this)
            {
                if (item.LockInfo.UserID == userID && item.VerifyAccessType(authentication, AccessType.Master) == false)
                {
                    throw new PermissionDeniedException();
                }
            }
        }

        private void Initialize(IEnumerable<TypeInfo> typeInfos)
        {
            this.CremaHost.Debug("Load Types");
            var directories = DirectoryUtility.GetAllDirectories(this.basePath);
            foreach (var item in directories)
            {
                var categoryName = CategoryName.Create(UriUtility.MakeRelativeOfDirectory(this.basePath, item));
                this.Categories.Prepare(categoryName.Path);
            }
            foreach (var item in typeInfos)
            {
                var type = this.Types.AddNew(Authentication.System, item.Name, item.CategoryPath);
                type.Initialize(item);
            }
            foreach (ITypeItem item in this)
            {
                var accessInfoPath = item.GetAccessInfoPath();
                try
                {
                    item.ReadAccessInfo(accessInfoPath);
                }
                catch (Exception e)
                {
                    this.CremaHost.Error(e);
                }
            }
            this.CremaHost.Debug("TypeLoadingCompleted.");
        }

        //private void ReadAccessInfo()
        //{
        //    if (File.Exists(this.accessInfoPath) == false)
        //        return;

        //    try
        //    {
        //        var accessInfos = XmlSerializerUtility.Read<AccessInfo[]>(this.accessInfoPath);
        //        foreach (var item in accessInfos)
        //        {
        //            var typeItem = this[item.Path];
        //            if (typeItem is Type)
        //            {
        //                var type = typeItem as Type;
        //                type.SetAccessInfo(item);
        //            }
        //            else
        //            {
        //                var category = typeItem as TypeCategory;
        //                category.SetAccessInfo(item);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        this.CremaHost.Error(e);
        //    }
        //}

        //private void WriteAccessInfos()
        //{
        //    var query = from ITypeItem item in this
        //                let accessInfo = item.AccessInfo
        //                where accessInfo.UserID != string.Empty && accessInfo.Path == item.Path
        //                select accessInfo;

        //    XmlSerializerUtility.Write(this.accessInfoPath, query.ToArray());
        //}

        //private void WriteAccessInfo(string accessInfoPath, AccessInfo accessInfo, ITypeItem tableItem)
        //{
        //    var query = from ITypeItem item in this
        //                where item != tableItem
        //                let itemInfo = item.AccessInfo
        //                where itemInfo.UserID != string.Empty && itemInfo.Path == item.Path
        //                select (AccessSerializationInfo)itemInfo;

        //    var list = query.ToList();

        //    if (tableItem.IsPrivate == true)
        //        list.Add((AccessSerializationInfo)tableItem.AccessInfo);

        //    DataContractSerializerUtility.Write(accessInfoPath, list.ToArray(), true);
        //}

        #region ITypeContext

        bool ITypeContext.Contains(string itemPath)
        {
            this.Dispatcher?.VerifyAccess();
            return this.Contains(itemPath);
        }

        ITypeCollection ITypeContext.Types
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Types;
            }
        }

        ITypeCategoryCollection ITypeContext.Categories
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Categories;
            }
        }

        ITypeCategory ITypeContext.Root
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Root;
            }
        }

        ITypeItem ITypeContext.this[string itemPath]
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this[itemPath] as ITypeItem;
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator<ITypeItem> IEnumerable<ITypeItem>.GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            foreach (var item in this)
            {
                yield return item as ITypeItem;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            foreach (var item in this)
            {
                yield return item as ITypeItem;
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
