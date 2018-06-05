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
using Ntreev.Crema.Services.Properties;
using Ntreev.Library;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ntreev.Crema.Services.Data
{
    class TableCategory : TableCategoryBase<Table, TableCategory, TableCollection, TableCategoryCollection, TableContext>,
        ITableCategory, ITableItem
    {
        private readonly List<NewTableTemplate> templateList = new List<NewTableTemplate>();

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
            base.ValidateSetPublic(authentication);
            this.Sign(authentication);
            this.Context.InvokeTableItemSetPublic(authentication, this);
            base.SetPublic(authentication);
            this.Context.InvokeItemsSetPublicEvent(authentication, new ITableItem[] { this });
        }

        public void SetPrivate(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(SetPrivate), this);
            base.ValidateSetPrivate(authentication);
            this.Sign(authentication);
            this.Context.InvokeTableItemSetPrivate(authentication, this, AccessInfo.Empty);
            base.SetPrivate(authentication);
            this.Context.InvokeItemsSetPrivateEvent(authentication, new ITableItem[] { this });
        }

        public void AddAccessMember(Authentication authentication, string memberID, AccessType accessType)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(AddAccessMember), this, memberID, accessType);
            base.ValidateAddAccessMember(authentication, memberID, accessType);
            this.Sign(authentication);
            this.Context.InvokeTableItemAddAccessMember(authentication, this, this.AccessInfo, memberID, accessType);
            base.AddAccessMember(authentication, memberID, accessType);
            this.Context.InvokeItemsAddAccessMemberEvent(authentication, new ITableItem[] { this }, new string[] { memberID }, new AccessType[] { accessType });
        }

        public void SetAccessMember(Authentication authentication, string memberID, AccessType accessType)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(SetAccessMember), this, memberID, accessType);
            base.ValidateSetAccessMember(authentication, memberID, accessType);
            this.Sign(authentication);
            this.Context.InvokeTableItemSetAccessMember(authentication, this, this.AccessInfo, memberID, accessType);
            base.SetAccessMember(authentication, memberID, accessType);
            this.Context.InvokeItemsSetAccessMemberEvent(authentication, new ITableItem[] { this }, new string[] { memberID }, new AccessType[] { accessType });
        }

        public void RemoveAccessMember(Authentication authentication, string memberID)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(RemoveAccessMember), this, memberID);
            base.ValidateRemoveAccessMember(authentication, memberID);
            this.Sign(authentication);
            this.Context.InvokeTableItemRemoveAccessMember(authentication, this, this.AccessInfo, memberID);
            base.RemoveAccessMember(authentication, memberID);
            this.Context.InvokeItemsRemoveAccessMemberEvent(authentication, new ITableItem[] { this }, new string[] { memberID });
        }

        public void Lock(Authentication authentication, string comment)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Lock), this, comment);
            base.ValidateLock(authentication);
            this.Sign(authentication);
            this.Context.InvokeTableItemLock(authentication, this, comment);
            base.Lock(authentication, comment);
            this.Context.InvokeItemsLockedEvent(authentication, new ITableItem[] { this }, new string[] { comment });
        }

        public void Unlock(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Unlock), this);
            base.ValidateUnlock(authentication);
            this.Sign(authentication);
            this.Context.InvokeTableItemUnlock(authentication, this);
            base.Unlock(authentication);
            this.Context.InvokeItemsUnlockedEvent(authentication, new ITableItem[] { this });
        }

        public void Rename(Authentication authentication, string name)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Rename), this, name);
            base.ValidateRename(authentication, name);
            this.Sign(authentication);
            var items = EnumerableUtility.One(this).ToArray();
            var oldNames = items.Select(item => item.Name).ToArray();
            var oldPaths = items.Select(item => item.Path).ToArray();
            var dataSet = this.ReadAllData(authentication);
            this.Container.InvokeCategoryRename(authentication, this, name, dataSet);
            base.Rename(authentication, name);
            this.Container.InvokeCategoriesRenamedEvent(authentication, items, oldNames, oldPaths, dataSet);
        }

        public void Move(Authentication authentication, string parentPath)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Move), this, parentPath);
            base.ValidateMove(authentication, parentPath);
            this.Sign(authentication);
            var items = EnumerableUtility.One(this).ToArray();
            var oldPaths = items.Select(item => item.Path).ToArray();
            var oldParentPaths = items.Select(item => item.Parent.Path).ToArray();
            var dataSet = this.ReadAllData(authentication);
            this.Container.InvokeCategoryMove(authentication, this, parentPath, dataSet);
            base.Move(authentication, parentPath);
            this.Container.InvokeCategoriesMovedEvent(authentication, items, oldPaths, oldParentPaths, dataSet);
        }

        public void Delete(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Delete), this);
            base.ValidateDelete(authentication);
            this.Sign(authentication);
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
            this.Sign(authentication);
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
            var itemPath = this.Dispatcher.Invoke(() =>
            {
                this.ValidateAccessType(authentication, AccessType.Guest);
                this.Sign(authentication);
                return this.LocalPath;
            });
            var dataSet = this.Repository.GetTableCategoryData(this.Serializer, itemPath, revision);
            return dataSet;
        }

        public LogInfo[] GetLog(Authentication authentication)
        {
            this.DataBase.ValidateAsyncBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(GetLog), this);
            var localPath = this.Dispatcher.Invoke(() =>
            {
                this.ValidateAccessType(authentication, AccessType.Guest);
                this.Sign(authentication);
                return this.LocalPath;
            });
            var result = this.Context.GetCategoryLog(localPath);
            return result;
        }

        public FindResultInfo[] Find(Authentication authentication, string text, FindOptions options)
        {
            this.DataBase.ValidateAsyncBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Find), this, text, options);
            var items = this.Dispatcher.Invoke(() =>
            {
                this.ValidateAccessType(authentication, AccessType.Guest);
                this.Sign(authentication);
                var descendants = EnumerableUtility.Descendants<ITableItem>(this, item => item.Childs);
                return EnumerableUtility.Friends(this, descendants).Select(item => item.Path).ToArray();
            });
            var service = this.GetService(typeof(DataFindService)) as DataFindService;
            var result = service.Dispatcher.Invoke(() => service.FindFromTable(this.DataBase.ID, items, text, options));
            return result;
        }

        public object GetService(System.Type serviceType)
        {
            return this.DataBase.GetService(serviceType);
        }

        /// <summary>
        /// 폴더내에 모든 테이블과 상속된 테이블을 읽어들입니다.
        /// </summary>
        public CremaDataSet ReadAllData(Authentication authentication)
        {
            var tables = EnumerableUtility.Descendants<IItem, Table>(this as IItem, item => item.Childs).ToArray();
            var typePaths = tables.SelectMany(item => item.GetTypes())
                                  .Select(item => item.LocalPath)
                                  .Distinct()
                                  .ToArray();
            var tablePaths = tables.SelectMany(item => EnumerableUtility.Friends(item, item.DerivedTables))
                                   .Select(item => item.Parent ?? item)
                                   .Select(item => item.LocalPath)
                                   .Distinct()
                                   .ToArray();

            var props = new CremaDataSetPropertyCollection(authentication, typePaths, tablePaths);
            var dataSet = this.Serializer.Deserialize(this.LocalPath, typeof(CremaDataSet), props) as CremaDataSet;
            return dataSet;
        }

        /// <summary>
        /// 폴더내의 테이블을 읽어들입니다.
        /// </summary>
        public CremaDataSet ReadData(Authentication authentication)
        {
            return this.ReadData(authentication, this.Tables);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateRename(IAuthentication authentication, object target, string oldPath, string newPath)
        {
            base.OnValidateRename(authentication, target, oldPath, newPath);
            if (this.templateList.Any() == true)
                throw new InvalidOperationException(Resources.Exception_CannotRenameOnCreateTable);

            var categoryName = new CategoryName(Regex.Replace(this.Path, $"^{oldPath}", newPath));
            this.Context.ValidateCategoryPath(categoryName);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateMove(IAuthentication authentication, object target, string oldPath, string newPath)
        {
            base.OnValidateMove(authentication, target, oldPath, newPath);
            if (this.templateList.Any() == true)
                throw new InvalidOperationException(Resources.Exception_CannotMoveOnCreateTable);
            var tables = EnumerableUtility.Descendants<IItem, Table>(this as IItem, item => item.Childs);
            if (tables.Where(item => item.TableState != TableState.None).Any() == true)
                throw new InvalidOperationException(string.Format(Resources.Exception_TableIsBeingEdited_Format, string.Join(", ", tables.Select(item => item.Name))));
            var categoryName = new CategoryName(Regex.Replace(this.Path, $"^{oldPath}", newPath));
            this.Context.ValidateCategoryPath(categoryName);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateDelete(IAuthentication authentication, object target)
        {
            base.OnValidateDelete(authentication, target);
            if (this.templateList.Any() == true)
                throw new InvalidOperationException(Resources.Exception_CannotDeleteOnCreateTable);
            var tables = EnumerableUtility.Descendants<IItem, Table>(this as IItem, item => item.Childs).ToArray();
            if (tables.Any() == true)
                throw new InvalidOperationException(Resources.Exception_CannotDeletePathWithItems);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetAccessInfo(AccessInfo accessInfo)
        {
            accessInfo.Path = this.Path;
            base.AccessInfo = accessInfo;
        }

        public void Attach(NewTableTemplate template)
        {
            template.EditCanceled += (s, e)=> this.templateList.Remove(template); 
            template.EditEnded += (s, e) => this.templateList.Remove(template);
            this.templateList.Add(template);
        }

        public string LocalPath
        {
            get { return this.Context.GenerateCategoryPath(base.Path); }
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

        public IObjectSerializer Serializer => this.DataBase.Serializer;

        public DataBaseRepositoryHost Repository => this.DataBase.Repository;

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

        private CremaDataSet ReadData(Authentication authentication, IEnumerable<Table> tables)
        {
            var typePaths = tables.SelectMany(item => item.GetTypes())
                                  .Select(item => item.LocalPath)
                                  .Distinct()
                                  .ToArray();
            var tablePaths = tables.Select(item => item.LocalPath).Distinct().ToArray();
            var props = new CremaDataSetPropertyCollection(authentication, typePaths, tablePaths);
            var dataSet = this.Serializer.Deserialize(this.LocalPath, typeof(CremaDataSet), props) as CremaDataSet;
            return dataSet;
        }

        private void Sign(Authentication authentication)
        {
            authentication.Sign();
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
