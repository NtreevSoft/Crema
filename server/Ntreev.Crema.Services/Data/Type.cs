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
using Ntreev.Crema.Data.Xml;
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

namespace Ntreev.Crema.Services.Data
{
    class Type : TypeBase<Type, TypeCategory, TypeCollection, TypeCategoryCollection, TypeContext>,
        IType, ITypeItem, IInfoProvider, IStateProvider
    {
        private TypeTemplate template;
        

        public Type()
        {
            this.template = new TypeTemplate(this);
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
            this.Context.InvokeTypeItemSetPublic(authentication, this);
            base.SetPublic(authentication);
            this.Context.InvokeItemsSetPublicEvent(authentication, new ITypeItem[] { this });
        }

        public void SetPrivate(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(SetPrivate), this);
            base.ValidateSetPrivate(authentication);
            this.Sign(authentication);
            this.Context.InvokeTypeItemSetPrivate(authentication, this, AccessInfo.Empty);
            base.SetPrivate(authentication);
            this.Context.InvokeItemsSetPrivateEvent(authentication, new ITypeItem[] { this });
        }

        public void AddAccessMember(Authentication authentication, string memberID, AccessType accessType)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(AddAccessMember), this, memberID, accessType);
            base.ValidateAddAccessMember(authentication, memberID, accessType);
            this.Sign(authentication);
            this.Context.InvokeTypeItemAddAccessMember(authentication, this, this.AccessInfo, memberID, accessType);
            base.AddAccessMember(authentication, memberID, accessType);
            this.Context.InvokeItemsAddAccessMemberEvent(authentication, new ITypeItem[] { this }, new string[] { memberID }, new AccessType[] { accessType });
        }

        public void SetAccessMember(Authentication authentication, string memberID, AccessType accessType)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(SetAccessMember), this, memberID, accessType);
            base.ValidateSetAccessMember(authentication, memberID, accessType);
            this.Sign(authentication);
            this.Context.InvokeTypeItemSetAccessMember(authentication, this, this.AccessInfo, memberID, accessType);
            base.SetAccessMember(authentication, memberID, accessType);
            this.Context.InvokeItemsSetAccessMemberEvent(authentication, new ITypeItem[] { this }, new string[] { memberID }, new AccessType[] { accessType });
        }

        public void RemoveAccessMember(Authentication authentication, string memberID)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(RemoveAccessMember), this, memberID);
            base.ValidateRemoveAccessMember(authentication, memberID);
            this.Sign(authentication);
            this.Context.InvokeTypeItemRemoveAccessMember(authentication, this, this.AccessInfo, memberID);
            base.RemoveAccessMember(authentication, memberID);
            this.Context.InvokeItemsRemoveAccessMemberEvent(authentication, new ITypeItem[] { this }, new string[] { memberID });
        }

        public void Lock(Authentication authentication, string comment)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Lock), this, comment);
            base.ValidateLock(authentication);
            this.Sign(authentication);
            this.Context.InvokeTypeItemLock(authentication, this, comment);
            base.Lock(authentication, comment);
            this.Context.InvokeItemsLockedEvent(authentication, new ITypeItem[] { this }, new string[] { comment });
        }

        public void Unlock(Authentication authentication)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Unlock), this);
            base.ValidateUnlock(authentication);
            this.Sign(authentication);
            this.Context.InvokeTypeItemUnlock(authentication, this);
            base.Unlock(authentication);
            this.Context.InvokeItemsUnlockedEvent(authentication, new ITypeItem[] { this });
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
            var dataType = this.ReadAllData(authentication);
            var dataSet = dataType.DataSet;
            this.Container.InvokeTypeRename(authentication, this, name, dataSet);
            base.Rename(authentication, name);
            this.Container.InvokeTypesRenamedEvent(authentication, items, oldNames, oldPaths, dataSet);
        }

        public void Move(Authentication authentication, string categoryPath)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Move), this, categoryPath);
            base.ValidateMove(authentication, categoryPath);
            this.Sign(authentication);
            var items = EnumerableUtility.One(this).ToArray();
            var oldPaths = items.Select(item => item.Path).ToArray();
            var oldCategoryPaths = items.Select(item => item.Category.Path).ToArray();
            var dataType = this.ReadAllData(authentication);
            var dataSet = dataType.DataSet;
            this.Container.InvokeTypeMove(authentication, this, categoryPath, dataSet);
            base.Move(authentication, categoryPath);
            this.Container.InvokeTypesMovedEvent(authentication, items, oldPaths, oldCategoryPaths, dataSet);
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
            container.InvokeTypeDelete(authentication, this);
            base.Delete(authentication);
            container.InvokeTypesDeletedEvent(authentication, items, oldPaths);
        }

        public void SetProperty(Authentication authentication, string propertyName, string value)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(SetProperty), this, propertyName, value);
            if (propertyName == CremaSchema.Tags)
            {
                this.SetTags(authentication, (TagInfo)value);
            }
            else if (propertyName == CremaSchema.Comment)
            {
                //this.SetComment(authentication, value);
            }
        }

        public void SetTags(Authentication authentication, TagInfo tags)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(SetTags), this, tags);
            this.Template.ValidateBeginEdit(authentication);
            this.Sign(authentication);
            var dataType = this.ReadData(authentication);
            var dataSet = dataType.DataSet;
            this.Container.InvokeTypeSetTags(authentication, this, tags, dataSet);
            this.UpdateTags(tags);
            this.Container.InvokeTypesChangedEvent(authentication, new Type[] { this }, dataSet);
        }

        public Type Copy(Authentication authentication, string newTypeName, string categoryPath)
        {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(Copy), this, newTypeName, categoryPath);
            return this.Container.Copy(authentication, base.Name, newTypeName, categoryPath);
        }

        public LogInfo[] GetLog(Authentication authentication)
        {
            this.DataBase.ValidateAsyncBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(GetLog), this);
            var itemPaths = this.Dispatcher.Invoke(() =>
            {
                this.ValidateAccessType(authentication, AccessType.Guest);
                this.Sign(authentication);
                return this.Serializer.VerifyPath(typeof(CremaDataType), this.ItemPath, null);
            });
            var result = this.Context.GetLog(itemPaths);
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
                var descendants = EnumerableUtility.Descendants<ITypeItem>(this, item => item.Childs);
                return EnumerableUtility.Friends(this, descendants).Select(item => item.Path).ToArray();
            });
            var service = this.GetService(typeof(DataFindService)) as DataFindService;
            var result = service.Dispatcher.Invoke(() => service.FindFromType(this.DataBase.ID, items, text, options));
            return result;
        }

        public CremaDataSet GetDataSet(Authentication authentication, string revision)
        {
            this.DataBase.ValidateAsyncBeginInDataBase(authentication);
            this.CremaHost.DebugMethod(authentication, this, nameof(GetDataSet), this, revision);
            var info = this.Dispatcher.Invoke(() =>
            {
                this.ValidateAccessType(authentication, AccessType.Guest);
                this.Sign(authentication);
                return new Tuple<string, string>(this.DataBase.BasePath, this.SchemaPath);
            });
            var dataSet = this.Container.Repository.GetTypeData(info.Item1, info.Item2, revision);
            return dataSet;
        }

        public CremaDataType ReadData(Authentication authentication)
        {
            var dataSet = CremaDataSet.Create(new SignatureDateProvider(authentication.ID));
            dataSet.ReadType(this.SchemaPath);
            return dataSet.Types[base.Name];
        }

        public CremaDataType ReadAllData(Authentication authentication)
        {
            var tables = this.ReferencedTables.ToArray();
            var typeFiles = tables.SelectMany(item => item.GetTypes())
                                  .Concat(EnumerableUtility.One(this))
                                  .Select(item => item.ItemPath)
                                  .Distinct()
                                  .ToArray();
            var tableFiles = tables.Select(item => item.Parent ?? item)
                                   .Select(item => item.ItemPath)
                                   .Distinct()
                                   .ToArray();

            var info = new DataSetDeserializationInfo()
            {
                SignatureDateProvider = new SignatureDateProvider(authentication.ID),
                TypePaths = typeFiles,
                TablePaths = tableFiles,
            };
            var dataSet = this.Serializer.Deserialize(typeof(CremaDataSet), this.ItemPath, info) as CremaDataSet;
            return dataSet.Types[base.Name];
        }

        public object GetService(System.Type serviceType)
        {
            return this.DataBase.GetService(serviceType);
        }

        [Obsolete]
        public string SchemaPath
        {
            get { return this.Context.GenerateTypePath(this.Category.Path, base.Name); }
        }

        public string ItemPath
        {
            get { return this.Context.GenerateTypePath(this.Category.Path, base.Name); }
        }

        public IEnumerable<Table> ReferencedTables
        {
            get
            {
                var tables = this.GetService(typeof(TableCollection)) as TableCollection;
                foreach (var item in tables)
                {
                    if (item.IsTypeUsed(this.Path))
                    {
                        yield return item;
                        if (item.Parent != null)
                        {
                            yield return item.Parent;
                            foreach (var i in item.Parent.Childs)
                            {
                                yield return i;
                            }
                        }
                    }
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetAccessInfo(AccessInfo accessInfo)
        {
            accessInfo.Path = this.Path;
            base.AccessInfo = accessInfo;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetTypeState(TypeState typeState)
        {
            base.TypeState = typeState;
        }

        public TypeTemplate Template
        {
            get { return this.template; }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.Context?.Dispatcher; }
        }

        public IObjectSerializer Serializer => this.DataBase.Serializer;

        public CremaHost CremaHost
        {
            get { return this.Context.CremaHost; }
        }

        public DataBase DataBase
        {
            get { return this.Context.DataBase; }
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

        public new TypeInfo TypeInfo
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.TypeInfo;
            }
        }

        public new TypeState TypeState
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.TypeState;
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

        public new event EventHandler TypeInfoChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.TypeInfoChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.TypeInfoChanged -= value;
            }
        }

        public new event EventHandler TypeStateChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.TypeStateChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.TypeStateChanged -= value;
            }
        }

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
            this.ValidateNotBeingEdited();
            this.ValidateUsingTables(authentication);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateMove(IAuthentication authentication, object target, string oldPath, string newPath)
        {
            base.OnValidateMove(authentication, target, oldPath, newPath);
            this.ValidateNotBeingEdited();
            this.ValidateUsingTables(authentication);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateDelete(IAuthentication authentication, object target)
        {
            base.OnValidateDelete(authentication, target);
            this.ValidateNotBeingEdited();
            this.ValidateUsingTables(authentication);

            var tables = this.GetService(typeof(TableCollection)) as TableCollection;
            var query = from Table item in tables
                        where item.IsTypeUsed(base.Path)
                        select item;
            if (query.Any() == true)
                throw new InvalidOperationException(Resources.Exception_CannotDeleteTypeWithUsedTables);
        }

        public void ValidateUsingTables(IAuthentication authentication)
        {
            var tables = this.GetService(typeof(TableCollection)) as TableCollection;

            var query = from Table item in tables
                        where item.IsTypeUsed(base.Path)
                        select item;

            foreach (var item in query.Distinct())
            {
                this.ValidateUsingTable(authentication, item);
            }
        }

        public void ValidateNotBeingEdited()
        {
            if (this.template.Domain != null)
                throw new InvalidOperationException(string.Format(Resources.Exception_TypeIsBeingEdited_Format, base.Name));
        }

        private void ValidateUsingTable(IAuthentication authentication, Table table)
        {
            if (table.TableState.HasFlag(TableState.IsBeingEdited) == true)
                throw new InvalidOperationException(string.Format(Resources.Exception_TableIsBeingEdited_Format, table.Name));
            if (table.TableState.HasFlag(TableState.IsBeingSetup) == true)
                throw new InvalidOperationException(string.Format(Resources.Exception_TableIsBeingSetup_Format, table.Name));
            if (table.VerifyAccessType(authentication, AccessType.Master) == false)
                throw new PermissionException();
        }

        private IEnumerable<Table> FailmyTables(Table table)
        {
            var parent = table.Parent ?? table;

            yield return parent;

            foreach (var item in parent.Childs)
            {
                yield return item;
            }
        }

        private void Sign(Authentication authentication)
        {
            authentication.Sign();
        }

        #region IType

        ITypeCategory IType.Category
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Category;
            }
        }

        ITypeTemplate IType.Template
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Template;
            }
        }

        IType IType.Copy(Authentication authentication, string newTypeName, string categoryPath)
        {
            return this.Copy(authentication, newTypeName, categoryPath);
        }

        #endregion

        #region ITypeItem

        ITypeItem ITypeItem.Parent
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.Category;
            }
        }

        IEnumerable<ITypeItem> ITypeItem.Childs
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return Enumerable.Empty<ITypeItem>();
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

        IDictionary<string, object> IInfoProvider.Info => this.TypeInfo.ToDictionary();

        #endregion

        #region IStateProvider

        object IStateProvider.State => this.TypeState;

        #endregion
    }
}
