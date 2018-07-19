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
    public sealed class CremaAttributeCollection : ICollection, IEnumerable<CremaAttribute>
    {
        private readonly InternalTableBase table;
        private readonly DataColumnCollection columns;
        private readonly IList<InternalAttribute> itemList;

        internal CremaAttributeCollection(InternalTableBase table)
        {
            this.table = table;
            this.columns = table.Columns;
            this.itemList = table.AttributeList;

            this.columns.CollectionChanged += Columns_CollectionChanged;
        }

        public CremaAttribute this[int index]
        {
            get { return this.itemList[index].Target; }
        }

        public CremaAttribute this[string attributeName]
        {
            get
            {
                var index = this.IndexOf(attributeName);
                if (index >= 0)
                    return this.itemList[index].Target;
                return null;
            }
        }

        public void Add(CremaAttribute attribute)
        {
            this.table.AddAttribute((InternalAttribute)attribute);
        }

        public CremaAttribute Add()
        {
            var attribute = this.table.AddAttribute();
            return attribute.Target;
        }

        public CremaAttribute Add(string attributeName)
        {
            var attribute = this.table.AddAttribute(attributeName);
            return attribute.Target;
        }

        public CremaAttribute Add(string attributeName, Type type)
        {
            var attribute = this.table.AddAttribute(attributeName, type);
            return attribute.Target;
        }

        public bool CanRemove(CremaAttribute attribute)
        {
            return this.table.CanRemoveAttribute((InternalAttribute)attribute);
        }

        public void Clear()
        {
            this.columns.CollectionChanged -= Columns_CollectionChanged;
            try
            {
                this.table.ClearAttributes();
            }
            finally
            {
                this.columns.CollectionChanged += Columns_CollectionChanged;
            }
            this.InvokeCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        public bool Contains(string attributeName)
        {
            return this.IndexOf(attributeName) >= 0;
        }

        public void CopyTo(CremaAttribute[] array, int index)
        {
            var items = this.itemList.Select(item => item.Target).ToArray();
            items.CopyTo(array, index);
        }

        public int IndexOf(CremaAttribute attribute)
        {
            return this.itemList.IndexOf(attribute.InternalAttribute);
        }

        public int IndexOf(string attributeName)
        {
            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                if (item.AttributeName == attributeName)
                    return i;
            }
            return -1;
        }

        public void Remove(CremaAttribute attribute)
        {
            this.table.RemoveAttribute(attribute.InternalAttribute);
        }

        public void Remove(string attributeName)
        {
            var index = this.IndexOf(attributeName);
            if (index >= 0)
            {
                var item = this.itemList[index];
                this.table.RemoveAttribute(item);
            }
        }

        public void RemoveAt(int index)
        {
            var item = this.itemList[index];
            this.table.RemoveAttribute(item);
        }

        public int Count
        {
            get { return this.itemList.Count; }
        }

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
                        if (e.Element is InternalAttribute attribute && attribute.IsInternalAction == false)
                        {
                            this.InvokeCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, attribute));
                        }
                    }
                    break;
                case CollectionChangeAction.Remove:
                    {
                        if (e.Element is InternalAttribute attribute && attribute.IsInternalAction == false)
                        {
                            this.InvokeCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, attribute));
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

        IEnumerator<CremaAttribute> IEnumerable<CremaAttribute>.GetEnumerator()
        {
            foreach (var item in this.itemList)
            {
                yield return item.Target;
            }
        }

        #endregion
    }
}
