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

using Ntreev.Crema.Data.Properties;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Data
{
    public sealed class CremaDataTypeCollection : IEnumerable<CremaDataType>, ICollection
    {
        private readonly InternalDataSet dataSet;
        private readonly DataTableCollection tables;
        private readonly List<InternalDataType> itemList = new List<InternalDataType>();
        private readonly Dictionary<string, InternalDataType> itemsByName = new Dictionary<string, InternalDataType>();
        private readonly Dictionary<string, InternalDataType> itemsByNamespace = new Dictionary<string, InternalDataType>();

        internal CremaDataTypeCollection(InternalDataSet dataSet)
        {
            this.dataSet = dataSet;
            this.tables = dataSet.Tables;
            this.tables.CollectionChanged += Tables_CollectionChanged;
            //this.tables.CollectionChanging += Tables_CollectionChanging;
            this.dataSet.PropertyChanged += InternalDataSet_PropertyChanged;
        }

        public CremaDataType this[int index] => this.itemList[index].Target;

        public CremaDataType this[string name]
        {
            get
            {
                Validate();

                var index = -1;
                for (var i = 0; i < this.itemList.Count; i++)
                {
                    var item = this.itemList[i];
                    var itemPath = item.Namespace.Substring(this.dataSet.TypeNamespace.Length);
                    var itemName = new ItemName(itemPath);
                    if (itemName.Name == name)
                    {
                        if (index != -1)
                            return null;
                        index = i;
                    }
                }
                if (index >= 0)
                    return this.itemList[index].Target;
                return null;

                void Validate()
                {
                    if (name == null)
                        throw new ArgumentNullException(nameof(name));
                }
            }
        }

        public CremaDataType this[string name, string categoryPath]
        {
            get
            {
                Validate();
                var itemNamespace = UriUtility.Combine(this.dataSet.TypeNamespace + categoryPath, name);
                for (var i = 0; i < this.itemList.Count; i++)
                {
                    var item = this.itemList[i];
                    if (item.Name == name && item.Namespace == itemNamespace)
                        return item.Target;
                }
                return null;

                void Validate()
                {
                    if (name == null)
                        throw new ArgumentNullException(nameof(name));
                    if (categoryPath == null)
                        throw new ArgumentNullException(nameof(categoryPath));
                }
            }
        }

        public CremaDataType this[Guid typeID] => this.SingleOrDefault(item => item.TypeID == typeID);

        public CremaDataType Add()
        {
            var signatureDate = this.dataSet.SignatureDateProvider.Provide();
            var dataType = new CremaDataType()
            {
                InternalCreationInfo = signatureDate,
                InternalModificationInfo = signatureDate,
            };
            this.Add(dataType);
            return dataType;
        }

        public void Add(CremaDataType type)
        {
            this.ValidateAdd(type);
            if (type.TypeName == string.Empty)
                type.TypeName = NameUtility.GenerateNewName("Type", this.Select(item => item.TypeName).ToArray());
            this.tables.Add(type.InternalObject);
        }

        public CremaDataType Add(string name)
        {
            var signatureDate = this.dataSet.SignatureDateProvider.Provide();
            var dataType = new CremaDataType(name)
            {
                InternalCreationInfo = signatureDate,
                InternalModificationInfo = signatureDate,
            };
            this.Add(dataType);
            return dataType;
        }

        public CremaDataType Add(string name, string categoryPath)
        {
            var signatureDate = this.dataSet.SignatureDateProvider.Provide();
            var dataType = new CremaDataType(name, categoryPath)
            {
                InternalCreationInfo = signatureDate,
                InternalModificationInfo = signatureDate,
            };
            this.Add(dataType);
            return dataType;
        }

        public CremaDataType Add(TypeInfo typeInfo)
        {
            var dataType = this.dataSet.AddType(typeInfo);
            return dataType.Target;
        }

        public bool CanRemove(CremaDataType type)
        {
            if (type == null)
                throw new ArgumentNullException();
            if (type.DataSet != this.dataSet.Target)
                return false;
            if (type.HasReference == true)
                return false;
            return true;
        }

        public bool Contains(string name)
        {
            return this.IndexOf(name) >= 0;
        }

        public bool Contains(string name, string categoryPath)
        {
            NameValidator.ValidateName(name);
            NameValidator.ValidateCategoryPath(categoryPath);

            return this.IndexOf(name, categoryPath) >= 0;
        }

        public bool Contains(Guid typeID)
        {
            foreach (var item in this)
            {
                if (item.TypeID == typeID)
                    return true;
            }
            return false;
        }

        public void Remove(CremaDataType type)
        {
            this.Remove(type, false);
        }

        public void Remove(CremaDataType type, bool force)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (type.DataSet != this.dataSet.Target)
                throw new ArgumentException(Resources.Exception_NotIncludedInDataSet, nameof(type));
            if (force == false && type.HasReference == true)
                throw new InvalidOperationException("현재 타입이 사용되고 있는 테이블이 있기때문에 삭제할 수 없습니다");

            if (force == true)
            {
                var columns = type.ReferencedColumns;
                foreach (var item in columns)
                {
                    item.CremaType = null;
                }
            }
            this.tables.Remove(type.InternalObject);
        }

        public void Remove(string name)
        {
            Validate();
            var index = this.IndexOf(name);
            if (index >= 0)
            {
                var item = this.itemList[index];
                this.tables.Remove(item);
            }

            void Validate()
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));
            }
        }

        public void Remove(string name, string categoryPath)
        {
            Validate();
            var index = this.IndexOf(name, categoryPath);
            if (index >= 0)
            {
                var item = this.itemList[index];
                this.tables.Remove(item);
            }

            void Validate()
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));
                if (categoryPath == null)
                    throw new ArgumentNullException(nameof(categoryPath));
            }
        }

        public void RemoveAt(int index)
        {
            var item = this.itemList[index];
            this.tables.Remove(item);
        }

        public int IndexOf(string name)
        {
            var index = -1;
            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                var itemPath = item.Namespace.Substring(this.dataSet.TypeNamespace.Length);
                var itemName = new ItemName(itemPath);
                if (itemName.Name == name)
                {
                    if (index != -1)
                        return -1;
                    index = i;
                }
            }
            return index;
        }

        public int IndexOf(string name, string categoryPath)
        {
            var itemNamespace = UriUtility.Combine(this.dataSet.TypeNamespace + categoryPath, name);
            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                if (item.Name == name && item.Namespace == itemNamespace)
                    return i;
            }
            return -1;
        }

        public void Clear()
        {
            this.ValidateClear();
            var query = from item in this.itemList orderby item.Name select item;
            this.tables.CollectionChanged -= Tables_CollectionChanged;
            foreach (var item in query.Reverse())
            {
                this.tables.Remove(item);
            }
            this.itemList.Clear();
            this.tables.CollectionChanged += Tables_CollectionChanged;
            this.InvokeCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        public void CopyTo(Array array, int index)
        {
            var arrayList = new ArrayList(this.tables.Count);
            for (var i = 0; i < this.tables.Count; i++)
            {
                var item = this.tables[i] as InternalDataTable;
                arrayList.Add(item.Target);
            }
            arrayList.CopyTo(array, index);
        }

        public int Count => this.itemList.Count;

        public event CollectionChangeEventHandler CollectionChanged;

        private void InvokeCollectionChanged(CollectionChangeEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }

        private void ValidateAdd(string name, string categoryPath)
        {
            if (string.IsNullOrEmpty(name) == true)
                return;

            if (this.IndexOf(name, categoryPath) >= 0)
            {
                throw new CremaDataException(name);
            }
        }

        private void ValidateAdd(CremaDataType type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type.DataSet != null)
            {
                if (type.DataSet == this.dataSet.Target)
                {
                    throw new ArgumentException("CremaDataType이 이미 현재 DataSet에 속해 있습니다.");
                }
                else
                {
                    throw new ArgumentException("CremaDataType이 이미 다른 DataSet에 속해 있습니다");
                }
            }

            this.ValidateAdd(type.TypeName, type.CategoryPath);
        }

        private void ValidateClear()
        {
            foreach (var item in this)
            {
                if (item.HasReference == true)
                    throw new CremaDataException("타입이 참조되고 있기 때문에 삭제할 수 없습니다.");
            }
        }

        private void Tables_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    {
                        if (e.Element is InternalDataType dataType)
                        {
                            this.itemList.Add(dataType);
                            this.itemsByName.Add(dataType.Name, dataType);
                            this.itemsByNamespace.Add(dataType.Namespace, dataType);
                            dataType.DefaultView.Sort = $"{CremaSchema.Index} ASC";
                            dataType.BuildNamespace();
                            this.InvokeCollectionChanged(e);
                            dataType.PropertyChanged += DataType_PropertyChanged;
                        }
                    }
                    break;
                case CollectionChangeAction.Remove:
                    {
                        if (e.Element is InternalDataType dataType)
                        {
                            dataType.PropertyChanged -= DataType_PropertyChanged;
                            this.itemsByNamespace.Remove(dataType.Namespace);
                            this.itemsByName.Remove(dataType.Name);
                            this.itemList.Remove(dataType);
                            dataType.DefaultView.Sort = $"{CremaSchema.Index} ASC";
                            dataType.BuildNamespace();
                            this.InvokeCollectionChanged(e);
                        }
                    }
                    break;
                case CollectionChangeAction.Refresh:
                    {
                        this.itemList.Clear();
                    }
                    break;
            }
        }

        private void DataType_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is InternalDataType dataType)
            {
                if (e.PropertyName == nameof(InternalDataType.Name))
                {
                    var value = this.itemsByName.First(item => item.Value == dataType);
                    this.itemsByName.Remove(value.Key);
                    this.itemsByName.Add(dataType.Name, dataType);
                }
                else if (e.PropertyName == nameof(InternalDataType.Namespace))
                {
                    var value = this.itemsByNamespace.First(item => item.Value == dataType);
                    this.itemsByNamespace.Remove(value.Key);
                    this.itemsByNamespace.Add(dataType.Namespace, dataType);

                }
            }
        }

        private void InternalDataSet_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataSet.Namespace))
            {
                foreach (var item in this.tables)
                {
                    if (item is InternalDataType dataType)
                    {
                        dataType.BuildNamespace();
                    }
                }
            }
        }

        #region IEnumerable

        void ICollection.CopyTo(Array array, int index)
        {
            var items = this.itemList.Select(item => item.Target).ToArray();
            items.CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                if (this.itemList is ICollection collection)
                    return collection.IsSynchronized;
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (this.itemList is ICollection collection)
                    return collection.SyncRoot;
                return this.itemList;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in this.itemList)
            {
                yield return item.Target;
            }
        }

        IEnumerator<CremaDataType> IEnumerable<CremaDataType>.GetEnumerator()
        {
            foreach (var item in this.itemList)
            {
                yield return item.Target;
            }
        }

        #endregion
    }
}
