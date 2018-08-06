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
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ntreev.Crema.Services.Data
{
    class Table : TableBase<Table, TableCategory, TableCollection, TableCategoryCollection, TableContext>,
        ITable, ITableItem, IInfoProvider, IStateProvider
    {
        private readonly List<NewTableTemplate> templateList = new List<NewTableTemplate>();

        public Table()
        {
            this.Template = new TableTemplate(this);
            this.Content = new TableContent(this);
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
                base.ValidateSetPublic(authentication);
                this.Sign(authentication);
                this.Context.InvokeTableItemSetPublic(authentication, this);
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
                base.ValidateSetPrivate(authentication);
                this.Sign(authentication);
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
                base.ValidateAddAccessMember(authentication, memberID, accessType);
                this.Sign(authentication);
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
                base.ValidateSetAccessMember(authentication, memberID, accessType);
                this.Sign(authentication);
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
                base.ValidateRemoveAccessMember(authentication, memberID);
                this.Sign(authentication);
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
                base.ValidateLock(authentication);
                this.Sign(authentication);
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
                base.ValidateUnlock(authentication);
                this.Sign(authentication);
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
                base.ValidateRename(authentication, name);
                this.Sign(authentication);
                var items = this.Parent == null ? EnumerableUtility.Friends(this, this.Childs).ToArray() : EnumerableUtility.Friends(this, this.DerivedTables).ToArray();
                var oldNames = items.Select(item => item.Name).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                var dataSet = this.ReadAll(authentication);
                this.Container.InvokeTableRename(authentication, this, name, dataSet);
                base.Rename(authentication, name);
                this.Container.InvokeTablesRenamedEvent(authentication, items, oldNames, oldPaths, dataSet);
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
                base.ValidateMove(authentication, categoryPath);
                this.Sign(authentication);
                var items = EnumerableUtility.Friends(this, this.Childs).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                var oldCategoryPaths = items.Select(item => item.Category.Path).ToArray();
                var dataSet = this.ReadAll(authentication);
                this.Container.InvokeTableMove(authentication, this, categoryPath, dataSet);
                base.Move(authentication, categoryPath);
                this.Container.InvokeTablesMovedEvent(authentication, items, oldPaths, oldCategoryPaths, dataSet);
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
                base.ValidateDelete(authentication);
                this.Sign(authentication);
                var items = this.Parent == null ? EnumerableUtility.Friends(this, this.Childs).ToArray() : EnumerableUtility.Friends(this, this.DerivedTables).ToArray();
                var oldPaths = items.Select(item => item.Path).ToArray();
                var container = this.Container;
                var dataSet = this.ReadAll(authentication);
                container.InvokeTableDelete(authentication, this, dataSet);
                base.Delete(authentication);
                container.InvokeTablesDeletedEvent(authentication, items, oldPaths);
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

        public NewTableTemplate NewChild(Authentication authentication)
        {
            try
            {
                this.DataBase.ValidateBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(NewChild), this);
                this.ValidateNewChild(authentication);
                var template = new NewTableTemplate(this);
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
                this.ValidateAccessType(authentication, AccessType.Guest);
                this.Sign(authentication);
                return this.Repository.GetTableData(this.Serializer, this.ItemPath, this.TemplatedParent?.ItemPath, revision);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public LogInfo[] GetLog(Authentication authentication, string revision)
        {
            try
            {
                this.DataBase.ValidateAsyncBeginInDataBase(authentication);
                this.CremaHost.DebugMethod(authentication, this, nameof(GetLog), this);
                this.ValidateAccessType(authentication, AccessType.Guest);
                this.Sign(authentication);
                return this.Context.GetTableLog(this.ItemPath, revision);
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
                this.ValidateAccessType(authentication, AccessType.Guest);
                this.Sign(authentication);
                if (this.GetService(typeof(DataFindService)) is DataFindService service)
                {
                    return service.Dispatcher.Invoke(() => service.FindFromTable(this.DataBase.ID, new string[] { base.Path }, text, options));
                }
                throw new NotImplementedException();
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

        public string ItemPath => this.Context.GenerateTablePath(this.Category.BasePath, base.Name);

        public bool IsTypeUsed(string typePath)
        {
            foreach (var item in base.TableInfo.Columns)
            {
                if (item.DataType == typePath)
                    return true;
            }

            return false;
        }

        public IEnumerable<Type> GetTypes()
        {
            var types = this.GetService(typeof(TypeCollection)) as TypeCollection;
            var query = from item in base.TableInfo.Columns
                        where NameValidator.VerifyItemPath(item.DataType)
                        let itemName = new ItemName(item.DataType)
                        select types[itemName.Name, itemName.CategoryPath];
            return query.Distinct();
        }

        public IEnumerable<Table> GetRelations()
        {
            var table = this;
            while (table.Parent != null)
            {
                table = table.Parent;
            };

            return EnumerableUtility.FamilyTree(table, item => item.Childs);
        }

        public void ValidateNotBeingEdited()
        {
            if (this.Template.IsBeingEdited == true)
                throw new InvalidOperationException(string.Format(Resources.Exception_TableIsBeingSetup_Format, base.Name));
            if (this.Content.Domain != null)
                throw new InvalidOperationException(string.Format(Resources.Exception_TableIsBeingEdited_Format, base.Name));
        }

        public void ValidateHasNotBeingEditedType()
        {
            var typeContext = this.GetService(typeof(TypeContext)) as TypeContext;

            foreach (var item in base.TableInfo.Columns)
            {
                if (NameValidator.VerifyItemPath(item.DataType) == false)
                    continue;
                var type = typeContext[item.DataType] as Type;
                if (type.IsBeingEdited == true)
                    throw new InvalidOperationException(string.Format(Resources.Exception_TypeIsBeingEdited_Format, type.Name));
            }
        }

        public CremaDataSet ReadEditableData(Authentication authentication)
        {
            var tables = this.GetRelations().Distinct().OrderBy(item => item.Name).ToArray();
            var types = tables.SelectMany(item => item.GetTypes()).Distinct().ToArray();
            var typePaths = types.Select(item => item.ItemPath).ToArray();
            var tablePaths = tables.Select(item => item.ItemPath).ToArray();
            var props = new CremaDataSetSerializerSettings(authentication, typePaths, tablePaths);
            var dataSet = this.Serializer.Deserialize(this.ItemPath, typeof(CremaDataSet), props) as CremaDataSet;
            return dataSet;
        }

        public CremaDataSet ReadData(Authentication authentication)
        {
            var tables = this.CollectChilds().OrderBy(item => item.Name).ToArray();
            var types = tables.SelectMany(item => item.GetTypes()).Distinct().ToArray();
            var typePaths = types.Select(item => item.ItemPath).ToArray();
            var tablePaths = tables.Select(item => item.ItemPath).ToArray();
            var props = new CremaDataSetSerializerSettings(authentication, typePaths, tablePaths);
            var dataSet = this.Serializer.Deserialize(this.ItemPath, typeof(CremaDataSet), props) as CremaDataSet;
            return dataSet;
        }

        public CremaDataSet ReadAll(Authentication authentication)
        {
            return this.ReadAll(authentication, false);
        }

        public CremaDataSet ReadAll(Authentication authentication, bool allTypes)
        {
            var typeCollection = this.GetService(typeof(TypeCollection)) as TypeCollection;
            var tables = this.Collect().OrderBy(item => item.Name).ToArray();
            var types = allTypes == true ? typeCollection.ToArray<Type>() : tables.SelectMany(item => item.GetTypes()).Distinct().ToArray();
            var typePaths = types.Select(item => item.ItemPath).ToArray();
            var tablePaths = tables.Select(item => item.ItemPath).ToArray();
            var props = new CremaDataSetSerializerSettings(authentication, typePaths, tablePaths);
            var dataSet = this.Serializer.Deserialize(this.ItemPath, typeof(CremaDataSet), props) as CremaDataSet;
            return dataSet;
        }

        private IEnumerable<Table> Collect()
        {
            yield return this;

            foreach (var item in this.Childs)
            {
                foreach (var i in item.Collect())
                {
                    yield return i;
                }
            }

            foreach (var item in this.DerivedTables)
            {
                yield return item;
            }
        }

        private IEnumerable<Table> CollectChilds()
        {
            yield return this;

            foreach (var item in this.Childs)
            {
                foreach (var i in item.CollectChilds())
                {
                    yield return i;
                }
            }
        }

        public void ValidateNewChild(Authentication authentication)
        {
            if (this.TemplatedParent != null)
                throw new InvalidOperationException(Resources.Exception_InheritedTableCannotNewChild);
            this.ValidateAccessType(authentication, AccessType.Master);
            this.OnValidateNewChild(authentication, this);
        }

        public void ValidateLockInternal(Authentication authentication)
        {
            base.ValidateLock(authentication);
        }

        public void LockInternal(Authentication authentication, string comment)
        {
            base.Lock(authentication, comment);
        }

        public void UnlockInternal(Authentication authentication)
        {
            base.Unlock(authentication);
        }

        public void Attach(NewTableTemplate template)
        {
            template.EditCanceled += Template_EditCanceled;
            template.EditEnded += Template_EditEnded;
            this.templateList.Add(template);
        }

        public TableTemplate Template { get; }

        public TableContent Content { get; }

        public CremaDispatcher Dispatcher => this.Context?.Dispatcher;

        public IObjectSerializer Serializer => this.DataBase.Serializer;

        public DataBaseRepositoryHost Repository => this.DataBase.Repository;

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

        private void Template_EditEnded(object sender, EventArgs e)
        {
            this.templateList.Remove(sender as NewTableTemplate);
        }

        private void Template_EditCanceled(object sender, EventArgs e)
        {
            this.templateList.Remove(sender as NewTableTemplate);
        }

        private void Sign(Authentication authentication)
        {
            authentication.Sign();
        }

        #region Invisibles

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateLock(IAuthentication authentication, object target)
        {
            base.OnValidateLock(authentication, target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateUnlock(IAuthentication authentication, object target)
        {
            base.OnValidateUnlock(authentication, target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateSetPublic(IAuthentication authentication, object target)
        {
            base.OnValidateSetPublic(authentication, target);
            this.ValidateNotBeingEdited();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateSetPrivate(IAuthentication authentication, object target)
        {
            base.OnValidateSetPrivate(authentication, target);
            this.ValidateNotBeingEdited();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateAddAccessMember(IAuthentication authentication, object target, string memberID, AccessType accessType)
        {
            base.OnValidateAddAccessMember(authentication, target, memberID, accessType);
            this.ValidateNotBeingEdited();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateRemoveAccessMember(IAuthentication authentication, object target)
        {
            base.OnValidateRemoveAccessMember(authentication, target);
            this.ValidateNotBeingEdited();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateRename(IAuthentication authentication, object target, string oldPath, string newPath)
        {
            base.OnValidateRename(authentication, target, oldPath, newPath);
            if (target == this)
            {
                var itemName = new ItemName(newPath);
                if (this.TableName == itemName.Name)
                    throw new ArgumentException(Resources.Exception_SameName, nameof(newPath));
            }
            if (this.templateList.Any() == true)
                throw new InvalidOperationException(Resources.Exception_CannotRenameOnCreateChildTable);
            this.ValidateNotBeingEdited();
            this.ValidateHasNotBeingEditedType();

            if (this.Parent == null)
            {
                var itemName = new ItemName(Regex.Replace(this.Path, $"^{oldPath}", newPath));
                this.Context.ValidateTablePath(itemName.CategoryPath, itemName.Name, this.TemplatedParent);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateMove(IAuthentication authentication, object target, string oldPath, string newPath)
        {
            base.OnValidateMove(authentication, target, oldPath, newPath);
            if (this.templateList.Any() == true)
                throw new InvalidOperationException(Resources.Exception_CannotMoveOnCreateChildTable);
            this.ValidateNotBeingEdited();
            this.ValidateHasNotBeingEditedType();

            if (this.Parent == null)
            {
                var itemName = new ItemName(Regex.Replace(this.Path, $"^{oldPath}", newPath));
                this.Context.ValidateTablePath(itemName.CategoryPath, itemName.Name, this.TemplatedParent);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateDelete(IAuthentication authentication, object target)
        {
            base.OnValidateDelete(authentication, target);
            if (this.templateList.Any() == true)
                throw new InvalidOperationException(Resources.Exception_CannotDeleteOnCreateChildTable);
            this.ValidateNotBeingEdited();
            this.ValidateHasNotBeingEditedType();

            if (this.IsBaseTemplate == true && this.Parent == null && target == this)
                throw new InvalidOperationException(Resources.Exception_CannotDeleteBaseTemplateTable);
            if (this.Childs.Any() == true)
                throw new InvalidOperationException("Cannot delete table with childs");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateNewChild(IAuthentication authentication, object target)
        {
            this.ValidateNotBeingEdited();
            this.ValidateHasNotBeingEditedType();

            foreach (var item in this.Childs)
            {
                item.OnValidateNewChild(authentication, target);
            }

            foreach (var item in this.DerivedTables)
            {
                item.OnValidateNewChild(authentication, null);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateRevert(IAuthentication authentication, object target)
        {
            this.ValidateNotBeingEdited();
            this.ValidateHasNotBeingEditedType();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetAccessInfo(AccessInfo accessInfo)
        {
            accessInfo.Path = this.Path;
            base.AccessInfo = accessInfo;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetTableState(TableState tableState)
        {
            base.TableState = tableState;
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
