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
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Data
{
    public sealed class CremaDataRowCollection : IEnumerable<CremaDataRow>, ICollection
    {
        private readonly InternalDataTable table;
        private readonly DataRowCollection rows;
        private readonly IList<InternalDataRow> itemList;

        internal CremaDataRowCollection(InternalDataTable table)
        {
            this.table = table;
            this.rows = table.Rows;
            this.itemList = table.RowList;
        }

        public CremaDataRow this[int index] => this.itemList[index].Target;

        public void Add(CremaDataRow row)
        {
            this.table.AddRow((InternalDataRow)row);
        }

        public void Clear()
        {
            this.rows.Clear();
        }

        public bool Contains(object key)
        {
            return this.rows.Contains(key);
        }

        public bool Contains(object[] keys)
        {
            return this.rows.Contains(keys);
        }

        public void CopyTo(CremaDataRow[] array, int index)
        {
            var items = this.itemList.Select(item => item.Target).ToArray();
            items.CopyTo(array, index);
        }

        public CremaDataRow Find(object key)
        {
            if (this.rows.Find(key) is InternalDataRow rowItem)
                return rowItem.Target;
            return null;
        }

        public CremaDataRow Find(object[] keys)
        {
            if (this.rows.Find(keys) is InternalDataRow rowItem)
                return rowItem.Target;
            return null;
        }

        public int IndexOf(CremaDataRow row)
        {
            return this.itemList.IndexOf((InternalDataRow)row);
        }

        public void Remove(CremaDataRow row)
        {
            this.table.RemoveRow((InternalDataRow)row);
        }

        public void RemoveAt(int index)
        {
            this.table.RemoveRowAt(index);
        }

        public int Count => this.rows.Count;

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

        IEnumerator<CremaDataRow> IEnumerable<CremaDataRow>.GetEnumerator()
        {
            foreach (var item in this.itemList)
            {
                yield return item.Target;
            }
        }

        #endregion
    }
}