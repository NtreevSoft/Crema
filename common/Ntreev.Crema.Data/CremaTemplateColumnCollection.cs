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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Data
{
    public sealed class CremaTemplateColumnCollection : IEnumerable<CremaTemplateColumn>, ICollection
    {
        private readonly InternalTemplate template;
        private readonly DataRowCollection rows;
        private readonly IList<InternalTemplateColumn> itemList;

        internal CremaTemplateColumnCollection(InternalTemplate template)
        {
            this.template = template;
            this.rows = template.Rows;
            this.itemList = template.RowList;
        }

        public void Add(CremaTemplateColumn column)
        {
            this.template.AddColumn((InternalTemplateColumn)column);
        }

        public void Clear()
        {
            this.template.ClearColumns();
        }

        public bool Contains(string columnName)
        {
            return this.IndexOf(columnName) >= 0;
        }

        public bool Contains(Guid columnID)
        {
            foreach (var item in this.itemList)
            {
                if (item[CremaSchema.ID] is Guid id && id == columnID)
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(CremaTemplateColumn[] array, int index)
        {
            var items = this.itemList.Select(item => item.Target).ToArray();
            items.CopyTo(array, index);
        }

        public int IndexOf(CremaTemplateColumn column)
        {
            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                if (item == column.InternalObject)
                    return i;
            }
            return -1;
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

        public void Remove(CremaTemplateColumn column)
        {
            this.template.RemoveColumn((InternalTemplateColumn)column);
        }

        public void RemoveAt(int index)
        {
            var item = this.itemList[index];
            this.template.RemoveColumn(item);
        }

        public int Count => this.itemList.Count;

        public CremaTemplateColumn this[int index] => this.itemList[index].Target;

        public CremaTemplateColumn this[string columnName]
        {
            get
            {
                var index = this.IndexOf(columnName);
                if (index >= 0)
                    return this.itemList[index].Target;
                return null;
            }
        }

        public CremaTemplateColumn this[Guid columnID]
        {
            get
            {
                foreach (var item in this.itemList)
                {
                    if (item[CremaSchema.ID] is Guid id && id == columnID)
                    {
                        return item.Target;
                    }
                }
                return null;
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

        IEnumerator<CremaTemplateColumn> IEnumerable<CremaTemplateColumn>.GetEnumerator()
        {
            foreach (var item in this.itemList)
            {
                yield return item.Target;
            }
        }

        #endregion
    }
}