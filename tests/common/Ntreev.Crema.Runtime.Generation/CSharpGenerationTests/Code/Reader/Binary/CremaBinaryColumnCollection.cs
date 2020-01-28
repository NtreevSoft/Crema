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

using Ntreev.Crema.Code.Reader.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ntreev.Crema.Code.Reader.Binary
{
    class CremaBinaryColumnCollection : IColumnCollection
    {
        private readonly CremaBinaryTable table;
        private OrderedDictionary columns;

        public CremaBinaryColumnCollection(CremaBinaryTable table, int capacity, bool caseSensitive)
        {
            this.table = table;
            this.columns = new OrderedDictionary(capacity, StringResource.GetComparer(caseSensitive));
        }

        public void Add(CremaBinaryColumn item)
        {
            item.Index = this.columns.Count;
            this.columns.Add(item.Name, item);
        }

        public CremaBinaryColumn this[int index]
        {
            get { return this.columns[index] as CremaBinaryColumn; }
        }

        public CremaBinaryColumn this[string columnName]
        {
            get { return this.columns[columnName] as CremaBinaryColumn; }
        }

        private ICollection Collection
        {
            get { return this.columns.Values as ICollection; }
        }

        #region IColumnCollection

        IColumn IColumnCollection.this[int index]
        {
            get { return this[index]; }
        }

        IColumn IColumnCollection.this[string columnName]
        {
            get { return this[columnName]; }
        }

        ITable IColumnCollection.Table
        {
            get { return this.table; }
        }

        #endregion

        #region ICollection

        void ICollection.CopyTo(Array array, int index)
        {
            this.Collection.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.Collection.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return this.Collection.IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return this.Collection.SyncRoot; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Collection.GetEnumerator();
        }

        IEnumerator<IColumn> IEnumerable<IColumn>.GetEnumerator()
        {
            return this.Collection.Cast<IColumn>().GetEnumerator();
        }

        #endregion
    }
}
