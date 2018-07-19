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
using Ntreev.Library;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace Ntreev.Crema.Data
{
    public sealed class CremaChildTableCollection : ICollection, IEnumerable<CremaDataTable>
    {
        private readonly InternalDataTable table;
        private readonly IList<InternalDataTable> itemList;

        internal CremaChildTableCollection(InternalDataTable table)
        {
            this.table = table;
            this.itemList = table.ChildItems;

            if (this.itemList is INotifyCollectionChanged childs)
            {
                childs.CollectionChanged += Childs_CollectionChanged;
            }
        }

        public CremaDataTable this[int index] => this.itemList[index].Target;

        public CremaDataTable this[string tableName]
        {
            get
            {
                var index = this.IndexOf(tableName);
                if (index >= 0)
                    return this.itemList[index].Target;
                return null;
            }
        }

        public CremaDataTable this[Guid tableID] => (CremaDataTable)this.itemList.SingleOrDefault(item => item.TableID == tableID);

        public CremaDataTable Add()
        {
            var dataTable = this.table.AddChild();
            return dataTable.Target;
        }

        public CremaDataTable Add(string tableName)
        {
            var dataTable = this.table.AddChild(tableName);
            return dataTable.Target;
        }

        public CremaDataTable Add(TableInfo tableInfo)
        {
            var dataTable = this.table.AddChild(tableInfo);
            return dataTable.Target;
        }

        public bool CanRemove(CremaDataTable table)
        {
            return this.table.CanRemoveChild(table.InternalObject);
        }

        public void Clear()
        {
            {
                if (this.itemList is INotifyCollectionChanged childs)
                {
                    childs.CollectionChanged -= Childs_CollectionChanged;
                }
            }
            try
            {
                this.table.ClearChilds();
            }
            finally
            {
                if (this.itemList is INotifyCollectionChanged childs)
                {
                    childs.CollectionChanged += Childs_CollectionChanged;
                }
            }
            this.InvokeCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        public bool Contains(string tableName)
        {
            return this.IndexOf(tableName) >= 0;
        }

        public bool Contains(Guid tableID)
        {
            foreach (var item in this.itemList)
            {
                if (item.TableID == tableID)
                    return true;
            }
            return false;
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

        public int IndexOf(string tableName)
        {
            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                if (item.LocalName == tableName)
                    return i;
            }
            return -1;
        }

        public void Remove(CremaDataTable table)
        {
            this.table.RemoveChild(table.InternalObject);
        }

        public void Remove(string tableName)
        {
            var index = this.IndexOf(tableName);
            if (index >= 0)
            {
                var item = this.itemList[index];
                this.table.RemoveChild(item);
            }
        }

        public void RemoveAt(int index)
        {
            var item = this.itemList[index];
            this.table.RemoveChild(item);
        }

        public int Count => this.itemList.Count;

        public event CollectionChangeEventHandler CollectionChanged;

        private void InvokeCollectionChanged(CollectionChangeEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }

        private void Childs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            this.InvokeCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            this.InvokeCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.InvokeCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    {
                        throw new NotImplementedException();
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
