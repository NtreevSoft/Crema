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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Data
{
    public sealed class CremaDataColumnCollection : ICollection, IEnumerable<CremaDataColumn>
    {
        private readonly InternalDataTable table;
        private readonly DataColumnCollection columns;
        private readonly IList<InternalDataColumn> itemList;

        internal CremaDataColumnCollection(InternalDataTable table)
        {
            this.table = table;
            this.itemList = table.ColumnList;
            this.columns = table.Columns;
            this.columns.CollectionChanged += Columns_CollectionChanged;
        }

        public CremaDataColumn this[int index] => this.itemList[index].Target;

        public CremaDataColumn this[string columnName]
        {
            get
            {
                var index = this.IndexOf(columnName);
                if (index >= 0)
                    return this.itemList[index].Target;
                return null;
            }
        }

        public CremaDataColumn this[Guid columnID] => (CremaDataColumn)this.itemList.SingleOrDefault(item => item.ColumnID == columnID);

        public void Add(CremaDataColumn column)
        {
            this.table.AddColumn(column.InternalObject);
        }

        public CremaDataColumn Add()
        {
            var dataColumn = this.table.AddColumn();
            return dataColumn.Target;
        }

        public CremaDataColumn AddKey()
        {
            var dataColumn = this.Add();
            dataColumn.IsKey = true;
            return dataColumn;
        }
        
        public CremaDataColumn Add(string columnName)
        {
            var dataColumn = this.table.AddColumn(columnName);
            return dataColumn.Target;
        }

        public CremaDataColumn AddKey(string columnName)
        {
            var dataColumn = this.Add(columnName);
            dataColumn.IsKey = true;
            return dataColumn;
        }

        public CremaDataColumn Add(string columnName, Type type)
        {
            var dataColumn = this.table.AddColumn(columnName, type);
            return dataColumn.Target;
        }

        public CremaDataColumn AddKey(string columnName, Type type)
        {
            var dataColumn = this.Add(columnName, type);
            dataColumn.IsKey = true;
            return dataColumn;
        }

        public CremaDataColumn Add(string columnName, CremaDataType type)
        {
            var dataColumn = this.table.AddColumn(columnName, (InternalDataType)type);
            return dataColumn.Target;
        }

        public CremaDataColumn AddKey(string columnName, CremaDataType type)
        {
            var dataColumn = this.Add(columnName, type);
            dataColumn.IsKey = true;
            return dataColumn;
        }

        public bool CanRemove(CremaDataColumn column)
        {
            return this.table.CanRemoveColumn(column.InternalObject);
        }

        public void Clear()
        {
            this.columns.CollectionChanged -= Columns_CollectionChanged;
            try
            {
                this.table.ClearColumns();
            }
            finally
            {
                this.columns.CollectionChanged += Columns_CollectionChanged;
            }
            this.InvokeCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        public bool Contains(string columnName)
        {
            return this.IndexOf(columnName) >= 0;
        }

        public bool Contains(Guid columnID)
        {
            foreach (var item in this.itemList)
            {
                if (item.ColumnID == columnID)
                    return true;
            }
            return false;
        }

        public void CopyTo(CremaDataColumn[] array, int index)
        {
            var items = this.itemList.Select(item => item.Target).ToArray();
            items.CopyTo(array, index);
        }

        public int IndexOf(CremaDataColumn column)
        {
            return this.itemList.IndexOf(column.InternalObject);
        }

        public int IndexOf(string columnName)
        {
            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                if (item.ColumnName == columnName)
                    return i;
            }
            return -1;
        }

        public void Remove(CremaDataColumn column)
        {
            this.table.RemoveColumn((InternalDataColumn)column);
        }

        public void Remove(string columnName)
        {
            var index = this.IndexOf(columnName);
            if (index >= 0)
            {
                var item = this.itemList[index];
                this.table.RemoveColumn(item);
            }
        }

        public void RemoveAt(int index)
        {
            var item = this.itemList[index];
            this.table.RemoveColumn(item);
        }

        public int Count => this.itemList.Count;

        public event CollectionChangeEventHandler CollectionChanged;

        private void InvokeCollectionChanged(CollectionChangeEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }

        private void Columns_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    {
                        if (e.Element is InternalDataColumn dataColumn && dataColumn.IsInternalAction == false)
                        {
                            this.InvokeCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataColumn));
                        }
                    }
                    break;
                case CollectionChangeAction.Remove:
                    {
                        if (e.Element is InternalDataColumn dataColumn && dataColumn.IsInternalAction == false)
                        {
                            this.InvokeCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, dataColumn));
                        }
                    }
                    break;
                case CollectionChangeAction.Refresh:
                    {
                        this.InvokeCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
                    }
                    break;
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

        IEnumerator<CremaDataColumn> IEnumerable<CremaDataColumn>.GetEnumerator()
        {
            foreach (var item in this.itemList)
            {
                yield return item.Target;
            }
        }

        #endregion
    }
}