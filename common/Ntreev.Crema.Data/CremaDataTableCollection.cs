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
    public sealed class CremaDataTableCollection : ICollection, IEnumerable<CremaDataTable>
    {
        private readonly InternalDataSet dataSet;
        private readonly DataTableCollection tables;
        private readonly List<InternalDataTable> itemList = new List<InternalDataTable>();
        private readonly Dictionary<string, InternalDataTable> itemsByName = new Dictionary<string, InternalDataTable>();
        private readonly Dictionary<string, InternalDataTable> itemsByNamespace = new Dictionary<string, InternalDataTable>();

        internal CremaDataTableCollection(InternalDataSet dataSet)
        {
            this.dataSet = dataSet;
            this.tables = dataSet.Tables;
            this.tables.CollectionChanged += Tables_CollectionChanged;
            this.dataSet.PropertyChanged += InternalDataSet_PropertyChanged;
        }

        public CremaDataTable this[int index] => this.itemList[index].Target;

        public CremaDataTable this[string name]
        {
            get
            {
                if (this.itemsByName.ContainsKey(name) == false)
                    return null;
                return this.itemsByName[name].Target;
            }
        }

        public CremaDataTable this[string name, string categoryPath]
        {
            get
            {
                var itemNamespace = UriUtility.Combine(this.dataSet.TableNamespace + categoryPath, name);
                if (this.itemsByNamespace.ContainsKey(itemNamespace) == false)
                    return null;
                return this.itemsByNamespace[itemNamespace].Target;
            }
        }

        public CremaDataTable Add()
        {
            var dataTable = this.dataSet.AddTable();
            return dataTable.Target;
        }

        public CremaDataTable Add(string tableName)
        {
            var dataTable = this.dataSet.AddTable(tableName);
            return dataTable.Target;
        }

        public CremaDataTable Add(string tableName, string categoryPath)
        {
            var dataTable = this.dataSet.AddTable(tableName, categoryPath);
            return dataTable.Target;
        }

        public void Add(CremaDataTable table)
        {
            this.dataSet.AddTable(table.InternalObject);
        }

        public CremaDataTable Add(TableInfo tableInfo)
        {
            var dataTable = this.dataSet.AddTable(tableInfo);
            return dataTable.Target;
        }

        public bool CanRemove(CremaDataTable table)
        {
            return this.dataSet.CanRemoveTable(table.InternalObject);
        }

        public void Clear()
        {
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

        public bool Contains(string name)
        {
            return this.itemsByName.ContainsKey(name);
            //return this.IndexOf(name) >= 0;
        }

        public bool Contains(string name, string categoryPath)
        {
            var itemNamespace = UriUtility.Combine(this.dataSet.TableNamespace + categoryPath, name);
            return this.itemsByNamespace.ContainsKey(name);
            //return this.IndexOf(name, categoryPath) >= 0;
        }

        public void CopyTo(CremaDataTable[] array, int index)
        {
            var items = this.itemList.Select(item => item.Target).ToArray();
            items.CopyTo(array, index);
        }

        public int IndexOf(CremaDataTable table)
        {
            return this.itemList.IndexOf(table.InternalObject);
        }

        public int IndexOf(string name)
        {
            var index = -1;
            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                var itemPath = item.Namespace.Substring(this.dataSet.TableNamespace.Length);
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
            var itemNamespace = UriUtility.Combine(this.dataSet.TableNamespace + categoryPath, name);
            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                if (item.Name == name && item.Namespace == itemNamespace)
                    return i;
            }
            return -1;
        }

        public void Remove(CremaDataTable table)
        {
            this.dataSet.RemoveTable(table.InternalObject);
        }

        public void Remove(string name)
        {
            var index = this.IndexOf(name);
            if (index >= 0)
            {
                var item = this.itemList[index];
                this.dataSet.RemoveTable(item);
            }
        }

        public void Remove(string name, string categoryPath)
        {
            var index = this.IndexOf(name, categoryPath);
            if (index >= 0)
            {
                var item = this.itemList[index];
                this.dataSet.RemoveTable(item);
            }
        }

        public void RemoveAt(int index)
        {
            var item = this.itemList[index];
            this.dataSet.RemoveTable(item);
        }

        public int Count => this.itemList.Count;

        public event CollectionChangeEventHandler CollectionChanged;

        private void InvokeCollectionChanged(CollectionChangeEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }

        private void Tables_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    {
                        if (e.Element is InternalDataTable dataTable)
                        {
                            this.itemList.Add(dataTable);
                            this.itemsByName.Add(dataTable.Name, dataTable);
                            this.itemsByNamespace.Add(dataTable.Namespace, dataTable);
                            dataTable.DefaultView.Sort = $"{CremaSchema.Index} ASC";
                            dataTable.BuildNamespace();
                            this.InvokeCollectionChanged(e);
                            dataTable.PropertyChanged += DataTable_PropertyChanged;
                        }
                    }
                    break;
                case CollectionChangeAction.Remove:
                    {
                        if (e.Element is InternalDataTable dataTable)
                        {
                            dataTable.PropertyChanged -= DataTable_PropertyChanged;
                            this.itemsByNamespace.Remove(dataTable.Namespace);
                            this.itemsByName.Remove(dataTable.Name);
                            this.itemList.Remove(dataTable);
                            dataTable.DefaultView.Sort = $"{CremaSchema.Index} ASC";
                            dataTable.BuildNamespace();
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

        private void DataTable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is InternalDataTable dataTable)
            {
                if (e.PropertyName == nameof(InternalDataTable.Name))
                {
                    var value = this.itemsByName.First(item => item.Value == dataTable);
                    this.itemsByName.Remove(value.Key);
                    this.itemsByName.Add(dataTable.Name, dataTable);
                }
                else if (e.PropertyName == nameof(InternalDataTable.Namespace))
                {
                    var value = this.itemsByNamespace.First(item => item.Value == dataTable);
                    this.itemsByNamespace.Remove(value.Key);
                    this.itemsByNamespace.Add(dataTable.Namespace, dataTable);

                }
            }
        }

        private void InternalDataSet_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(InternalDataSet.Namespace))
            {
                foreach (var item in this.tables)
                {
                    if (item is InternalDataTable dataTable)
                        dataTable.BuildNamespace();
                }
            }
        }

        internal void Sort()
        {
            this.itemList.Sort((x, y) => string.Compare(x.Name, y.Name));
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

        IEnumerator<CremaDataTable> IEnumerable<CremaDataTable>.GetEnumerator()
        {
            foreach (var item in this.itemList)
            {
                yield return item.Target;
            }
        }

        #endregion
    }
}
