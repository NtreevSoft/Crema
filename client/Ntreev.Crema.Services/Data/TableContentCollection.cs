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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Data
{
    class TableContentCollection : ITableContentCollection
    {
        private readonly Table table;

        private TableContentCollection()
        {

        }

        public TableContentCollection(Table table)
        {
            this.table = table;
        }

        public bool Contains(string tableName)
        {
            if (this.table == null)
                return false;
            return this.table.Childs.ContainsKey(tableName);
        }

        public int Count
        {
            get
            {
                if (this.table == null)
                    return 0;
                return this.table.Childs.Count;
            }
        }

        public ITableContent this[string tableName]
        {
            get
            {
                if (this.table == null)
                    return null;

                if (this.table.Childs.ContainsKey(tableName) == false)
                    return null;

                return this.table.Childs[tableName].Content;
            }
        }

        public static readonly TableContentCollection Empty = new TableContentCollection();

        #region IEnumerable

        IEnumerator<ITableContent> IEnumerable<ITableContent>.GetEnumerator()
        {
            if (this.table != null)
            {
                foreach (var item in this.table.Childs)
                {
                    yield return item.Content;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (this.table != null)
            {
                foreach (var item in this.table.Childs)
                {
                    yield return item.Content;
                }
            }
        }

        #endregion
    }
}
