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
using System.Linq;
using System.Text;

namespace Ntreev.Crema.Code.Reader.Binary
{
    class CremaBinaryRowCollection : IRowCollection
    {
        private readonly CremaBinaryTable table;
        private readonly List<CremaBinaryRow> rows;

        public CremaBinaryRowCollection(CremaBinaryTable table, int rowCount)
        {
            this.table = table;
            this.rows = new List<CremaBinaryRow>(rowCount);
            for (int i = 0; i < rowCount; i++)
            {
                this.rows.Add(new CremaBinaryRow());
            }
        }

        public CremaBinaryRow this[int index]
        {
            get { return this.rows[index]; }
        }

        #region IRowCollection

        IRow IRowCollection.this[int index]
        {
            get { return this.rows[index]; }
        }

        ITable IRowCollection.Table
        {
            get { return this.table; }
        }

        #endregion

        #region ICollection

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        int ICollection.Count
        {
            get { return this.rows.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return (this.rows as ICollection).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return (this.rows as ICollection).SyncRoot; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this.rows as ICollection).GetEnumerator();
        }

        IEnumerator<IRow> IEnumerable<IRow>.GetEnumerator()
        {
            return this.rows.Cast<IRow>().GetEnumerator();
        }

        #endregion
    }
}
