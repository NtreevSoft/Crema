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
using Ntreev.Crema.Services.Properties;
using Ntreev.Library;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        public Type AddNew(Authentication authentication, TypeInfo typeInfo)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(AddNew), typeInfo.Name, typeInfo.CategoryPath);
            this.InvokeTypeCreate(authentication, typeInfo.Name, typeInfo.CategoryPath);
            var type = this.BaseAddNew(typeInfo.Name, typeInfo.CategoryPath, authentication);
            type.Initialize(typeInfo);
            var items = EnumerableUtility.One(type).ToArray();
            this.InvokeTypesCreatedEvent(authentication, items);
            return type;
        }

        public Type Copy(Authentication authentication, Type type, string newTypeName, string categoryPath)
        {
            var result = this.Context.Service.CopyType(type.Name, newTypeName, categoryPath);
            this.Sign(authentication, result);
            return this.AddNew(authentication, result.Value);
        }

        public object GetService(System.Type serviceType)
        {
            return this.DataBase.GetService(serviceType);
        }

        public void InvokeTypeCreate(Authentication authentication, string typeName, string categoryPath)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeCreate), typeName, categoryPath);
        }

        public void InvokeTypeRename(Authentication authentication, Type type, string newName)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeRename), type, newName);
        }

        public void InvokeTypeMove(Authentication authentication, Type type, string newCategoryPath)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeMove), type, newCategoryPath);
        }

        public void InvokeTypeDelete(Authentication authentication, Type type)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeDelete), type);
        }

        public void InvokeTypeBeginTemplateEdit(Authentication authentication, Type type)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeBeginTemplateEdit), type);
        }

        public void InvokeTypeEndTemplateEdit(Authentication authentication, Type type, TypeInfo typeInfo)
        {
            this.CremaHost.DebugMethod(authentication, this, nameof(InvokeTypeEndTemplateEdit), type);
        }

        public void InvokeTypeSetTags(Authentication authentication, Type type, TagInfo tags)
        {
        }

        public void InvokeTypesCreatedEvent(Authentication authentication, Type[] types)
        {
            var args = types.Select(item => (object)item.TypeInfo).ToArray();
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTypesCreatedEvent), types);
            var comment = EventMessageBuilder.CreateType(authentication, types);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnTypesCreated(new ItemsCreatedEventArgs<IType>(authentication, types, args));
            this.Context.InvokeItemsCreatedEvent(authentication, types, args);
        }

        public void InvokeTypesRenamedEvent(Authentication authentication, Type[] types, string[] oldNames, string[] oldPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTypesRenamedEvent), types, oldNames, oldPaths);
            var comment = EventMessageBuilder.RenameType(authentication, types, oldNames);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnTypesRenamed(new ItemsRenamedEventArgs<IType>(authentication, types, oldNames, oldPaths));
            this.Context.InvokeItemsRenamedEvent(authentication, types, oldNames, oldPaths);
        }

        public void InvokeTypesMovedEvent(Authentication authentication, Type[] types, string[] oldPaths, string[] oldCategoryPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTypesMovedEvent), types, oldPaths, oldCategoryPaths);
            var comment = EventMessageBuilder.MoveType(authentication, types, oldCategoryPaths);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnTypesMoved(new ItemsMovedEventArgs<IType>(authentication, types, oldPaths, oldCategoryPaths));
            this.Context.InvokeItemsMovedEvent(authentication, types, oldPaths, oldCategoryPaths);
        }

        public void InvokeTypesDeletedEvent(Authentication authentication, Type[] types, string[] oldPaths)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTypesDeletedEvent), oldPaths);
            var comment = EventMessageBuilder.DeleteType(authentication, types);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnTypesDeleted(new ItemsDeletedEventArgs<IType>(authentication, types, oldPaths));
            this.Context.InvokeItemsDeleteEvent(authentication, types, oldPaths);
        }

        public void InvokeTypesChangedEvent(Authentication authentication, Type[] types)
        {
            var eventLog = EventLogBuilder.BuildMany(authentication, this, nameof(InvokeTypesChangedEvent), types);
            var comment = EventMessageBuilder.ChangeTypeTemplate(authentication, types);
            this.CremaHost.Debug(eventLog);
            this.CremaHost.Info(comment);
            this.OnTypesChanged(new ItemsEventArgs<IType>(authentication, types));
            this.Context.InvokeItemsChangedEvent(authentication, types);
        }

        public void InvokeTypesStateChangedEvent(Authentication authentication, Type[] types)
        {
            this.CremaHost.DebugMethodMany(authentication, this, nameof(InvokeTypesStateChangedEvent), types);
            this.OnTypesStateChanged(new ItemsEventArgs<IType>(authentication, types));
        }

        public IDataBaseService Service
        {
            get { return this.Context.Service; }
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

        private void Sign(Authentication authentication, ResultBase result)
        {
            result.Validate(authentication);
        }

        private void Sign<T>(Authentication authentication, ResultBase<T> result)
        {
            result.Validate(authentication);
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
