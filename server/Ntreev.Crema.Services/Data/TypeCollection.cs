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
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace Ntreev.Crema.Services.Data
{
    class TypeCollection : ItemContainer<Type, TypeCategory, TypeCollection, TypeCategoryCollection, TypeContext>, ITypeCollection
    {
        private ItemsCreatedEventHandler<IType> typesCreated;
        private ItemsRenamedEventHandler<IType> typesRenamed;
        private ItemsMovedEventHandler<IType> typesMoved;
        private ItemsDeletedEventHandler<IType> typesDeleted;
        private ItemsEventHandler<IType> typesStateChanged;
        private ItemsEventHandler<IType> typesChanged;

        public TypeCollection()
        {

        }

        public Type AddNew(Authentication authentication, string name, string categoryPath)
        {
            if (NameValidator.VerifyName(name) == false)
                throw new ArgumentException(string.Format(Resources.Exception_InvalidName_Format, name), nameof(name));
            return this.BaseAddNew(name, categoryPath, authentication);
        }

        public Type AddNew(Authentication authentication, CremaDataType dataType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(AddNew), dataType.Name, dataType.CategoryPath);
            this.ValidateAddNew(dataType.Name, dataType.CategoryPath, authentication);
            this.Sign(authentication);
            this.InvokeTypeCreate(authentication, dataType.Name, dataType.CategoryPath, dataType);
            var type = this.BaseAddNew(dataType.Name, dataType.CategoryPath, authentication);
            type.Initialize(dataType.TypeInfo);
            this.InvokeTypesCreatedEvent(authentication, new Type[] { type, }, dataType.DataSet);
            return type;
        }

        public Type Copy(Authentication authentication, string typeName, string newTypeName, string categoryPath)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(TypeCollection.Copy), typeName, newTypeName, categoryPath);
            this.ValidateCopy(authentication, typeName, newTypeName);
            var type = this[typeName];
            var dataType = type.ReadData(authentication);
            dataType.TypeName = newTypeName;
            dataType.CategoryPath = categoryPath;
            dataType.CreationInfo = authentication.SignatureDate;
            return this.AddNew(authentication, dataType);
        }

        public object GetService(System.Type serviceType)
        {
            return this.DataBase.GetService(serviceType);
        }

        public void InvokeTypeCreate(Authentication authentication, string typeName, string categoryPath, CremaDataType dataType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeCreate), typeName, categoryPath);
            var schemaPath = this.Context.GenerateTypePath(categoryPath, typeName);

            try
            {
                this.Repository.Add(schemaPath, dataType.GetXmlSchema());
                this.Context.InvokeTypeItemCreate(authentication, categoryPath + typeName);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }
        
        public void InvokeTypeRename(Authentication authentication, Type type, string newName, CremaDataSet dataSet)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeRename), type, newName);
            var dataTypes = new DataTypeCollection(dataSet, this.DataBase);
            var dataTables = new DataTableCollection(dataSet, this.DataBase);
            var dataType = dataSet.Types[type.Name, type.Category.Path];
            dataType.TypeName = newName;

            try
            {
                dataTypes.Modify(this.Repository, item => item == type);
                dataTables.Modify(this.Serializer);
                this.Context.InvokeTypeItemRename(authentication, type, newName);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeTypeMove(Authentication authentication, Type type, string newCategoryPath, CremaDataSet dataSet)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeMove), type, newCategoryPath);

            var dataTypes = new DataTypeCollection(dataSet, this.DataBase);
            var dataTables = new DataTableCollection(dataSet, this.DataBase);
            var dataType = dataSet.Types[type.Name, type.Category.Path];
            dataType.CategoryPath = newCategoryPath;

            try
            {
                dataTypes.Modify(this.Repository, item => item == type);
                dataTables.Modify(this.Serializer);
                this.Context.InvokeTypeItemMove(authentication, type, newCategoryPath);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeTypeDelete(Authentication authentication, Type type, CremaDataSet dataSet)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeDelete), type);

            var dataTypes = new DataTypeCollection(dataSet, this.DataBase);
            var dataTables = new DataTableCollection(dataSet, this.DataBase);
            var dataType = dataSet.Types[type.Name, type.Category.Path];

            var query = from table in dataSet.Tables
                        from column in table.Columns
                        where column.CremaType == dataType
                        select column;

            var columns = query.ToArray();

            foreach (var item in columns)
            {
                item.CremaType = null;
            }

            dataSet.Types.Remove(dataType);

            try
            {
                this.Repository.Delete(type.SchemaPath);
                dataTables.Modify(this.Serializer);
                this.Context.InvokeTypeItemDelete(authentication, type);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeTypeBeginTemplateEdit(Authentication authentication, Type type)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeBeginTemplateEdit), type);
        }

        public void InvokeTypeEndTemplateEdit(Authentication authentication, Type type, CremaDataType dataType)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeEndTemplateEdit), type);

            var dataTypes = new DataTypeCollection(dataType.DataSet, this.DataBase);
            var dataTables = new DataTableCollection(dataType.DataSet, this.DataBase);

            try
            {
                dataTypes.Modify(this.Repository, item => item == type);
                dataTables.Modify(this.Serializer);
                this.Context.InvokeTypeItemChange(authentication, type);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeTypeSetTags(Authentication authentication, Type type, TagInfo tags, CremaDataSet dataSet)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeSetTags), type, tags);

            var dataTypes = new DataTypeCollection(dataSet, this.DataBase);
            var dataType = dataSet.Types[type.Name, type.Category.Path];
            dataType.Tags = tags;

            try
            {
                dataTypes.Modify(this.Repository, item => item == type);
                this.Context.InvokeTypeItemChange(authentication, type);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                this.Repository.Revert();
                throw e;
            }
        }

        public void InvokeTypesCreatedEvent(Authentication authentication, Type[] types, CremaDataSet dataSet)
        {
            var args = types.Select(item => (object)item.TypeInfo).ToArray();
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTypesCreatedEvent), types);
            var comment = EventMessageBuilder.CreateType(authentication, types);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment);
            this.CremaHost.Info(comment);
            this.OnTypesCreated(new ItemsCreatedEventArgs<IType>(authentication, types, args, dataSet));
            this.Context.InvokeItemsCreatedEvent(authentication, types, args, dataSet);
        }

        public void InvokeTypesRenamedEvent(Authentication authentication, Type[] types, string[] oldNames, string[] oldPaths, CremaDataSet dataSet)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTypesRenamedEvent), types, oldNames, oldPaths);
            var comment = EventMessageBuilder.RenameType(authentication, types, oldNames);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment);
            this.CremaHost.Info(comment);
            this.OnTypesRenamed(new ItemsRenamedEventArgs<IType>(authentication, types, oldNames, oldPaths));
            this.Context.InvokeItemsRenamedEvent(authentication, types, oldNames, oldPaths, dataSet);
        }
        
        public void InvokeTypesMovedEvent(Authentication authentication, Type[] types, string[] oldPaths, string[] oldCategoryPaths, CremaDataSet dataSet)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTypesMovedEvent), types, oldPaths, oldCategoryPaths);
            var comment = EventMessageBuilder.MoveType(authentication, types, oldCategoryPaths);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment);
            this.CremaHost.Info(comment);
            this.OnTypesMoved(new ItemsMovedEventArgs<IType>(authentication, types, oldPaths, oldCategoryPaths));
            this.Context.InvokeItemsMovedEvent(authentication, types, oldPaths, oldCategoryPaths, dataSet);
        }
        
        public void InvokeTypesDeletedEvent(Authentication authentication, Type[] types, string[] oldPaths, CremaDataSet dataSet)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTypesDeletedEvent), oldPaths);
            var comment = EventMessageBuilder.DeleteType(authentication, types);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment);
            this.CremaHost.Info(comment);
            this.OnTypesDeleted(new ItemsDeletedEventArgs<IType>(authentication, types, oldPaths));
            this.Context.InvokeItemsDeleteEvent(authentication, types, oldPaths, dataSet);
        }
        
        public void InvokeTypesChangedEvent(Authentication authentication, Type[] types, CremaDataSet dataSet)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTypesChangedEvent), types);
            var comment = EventMessageBuilder.ChangeTypeTemplate(authentication, types);
            this.CremaHost.Debug(eventLog);
            this.Repository.Commit(authentication, comment);
            this.CremaHost.Info(comment);
            this.OnTypesChanged(new ItemsEventArgs<IType>(authentication, types));
            this.Context.InvokeItemsChangedEvent(authentication, types, dataSet);
        }

        public void InvokeTypesStateChangedEvent(Authentication authentication, Type[] types)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeTypesStateChangedEvent), types);
            this.OnTypesStateChanged(new ItemsEventArgs<IType>(authentication, types));
        }

        public DataBaseRepositoryHost Repository
        {
            get { return this.DataBase.Repository; }
        }

        public CremaHost CremaHost
        {
            get { return this.Context.CremaHost; }
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

        public new int Count
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return base.Count;
            }
        }

        public event ItemsCreatedEventHandler<IType> TypesCreated
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.typesCreated += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.typesCreated -= value;
            }
        }

        public event ItemsRenamedEventHandler<IType> TypesRenamed
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.typesRenamed += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.typesRenamed -= value;
            }
        }

        public event ItemsMovedEventHandler<IType> TypesMoved
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.typesMoved += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.typesMoved -= value;
            }
        }

        public event ItemsDeletedEventHandler<IType> TypesDeleted
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.typesDeleted += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.typesDeleted -= value;
            }
        }

        public event ItemsEventHandler<IType> TypesStateChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.typesStateChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.typesStateChanged -= value;
            }
        }

        public event ItemsEventHandler<IType> TypesChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                this.typesChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                this.typesChanged -= value;
            }
        }

        public new event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                this.Dispatcher?.VerifyAccess();
                base.CollectionChanged += value;
            }
            remove
            {
                this.Dispatcher?.VerifyAccess();
                base.CollectionChanged -= value;
            }
        }

        protected override void ValidateAddNew(string name, string categoryPath, object validation)
        {
            base.ValidateAddNew(name, categoryPath, validation);

            if (NameValidator.VerifyName(name) == false)
                throw new ArgumentException(string.Format(Resources.Exception_InvalidName_Format, name), nameof(name));

            if (validation is Authentication authentication)
            {
                var category = this.GetCategory(categoryPath);
                category.ValidateAccessType(authentication, AccessType.Master);
            }
        }

        protected virtual void OnTypesCreated(ItemsCreatedEventArgs<IType> e)
        {
            this.typesCreated?.Invoke(this, e);
        }

        protected virtual void OnTypesRenamed(ItemsRenamedEventArgs<IType> e)
        {
            this.typesRenamed?.Invoke(this, e);
        }

        protected virtual void OnTypesMoved(ItemsMovedEventArgs<IType> e)
        {
            this.typesMoved?.Invoke(this, e);
        }

        protected virtual void OnTypesDeleted(ItemsDeletedEventArgs<IType> e)
        {
            this.typesDeleted?.Invoke(this, e);
        }

        protected virtual void OnTypesStateChanged(ItemsEventArgs<IType> e)
        {
            this.typesStateChanged?.Invoke(this, e);
        }

        protected virtual void OnTypesChanged(ItemsEventArgs<IType> e)
        {
            this.typesChanged?.Invoke(this, e);
        }

        private void ValidateCopy(Authentication authentication, string typeName, string newTypeName)
        {
            if (this.Contains(typeName) == false)
                throw new TypeNotFoundException(typeName);

            if (this.Contains(newTypeName) == true)
                throw new ArgumentException(Resources.Exception_SameTypeNameExist, nameof(newTypeName));
        }

        private void Sign(Authentication authentication)
        {
            authentication.Sign();
        }

        #region ITypeCollection

        bool ITypeCollection.Contains(string typeName)
        {
            this.Dispatcher?.VerifyAccess();
            return this.Contains(typeName);
        }

        IType ITypeCollection.this[string typeName]
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this[typeName];
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator<IType> IEnumerable<IType>.GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.Dispatcher?.VerifyAccess();
            return this.GetEnumerator();
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
