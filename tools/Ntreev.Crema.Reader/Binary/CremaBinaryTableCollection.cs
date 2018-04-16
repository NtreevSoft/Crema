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

using Ntreev.Crema.Reader.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ntreev.Crema.Reader.Binary
{
    class CremaBinaryTableCollection : ITableCollection
    {
        private OrderedDictionary tables;
        private string[] tableNames;

        private readonly CremaBinaryReader reader;

        public CremaBinaryTableCollection(CremaBinaryReader reader, TableIndex[] tableIndexes)
        {
            this.reader = reader;

            this.tables = new OrderedDictionary(tableIndexes.Length, StringResource.GetComparer(this.reader.CaseSensitive));

            foreach (TableIndex item in tableIndexes)
            {
                this.tables.Add(StringResource.GetString(item.TableName), null);
            }

            this.tableNames = this.tables.Keys.Cast<string>().ToArray();
        }

        public CremaBinaryTable this[int index]
        {
            get
            {
                if (index >= this.tables.Count)
                    throw new ArgumentOutOfRangeException();

                if (this.tables[index] == null)
                {
                    return this.reader.ReadTable(index);
                }
                return this.tables[index] as CremaBinaryTable;
            }
            set
            {
                this.tables[index] = value;
                value.Index = index;
            }
        }

        public CremaBinaryTable this[string tableName]
        {
            get
            {
                if (this.tables.Contains(tableName) == false)
                {
                    throw new KeyNotFoundException(string.Format("'{0}'은(는) '{1}'에 존재하지 않는 테이블입니다.", tableName, this.reader.Name));
                }

                if (this.tables[tableName] == null)
                {
                    return this.reader.ReadTable(tableName);
                }
                return this.tables[tableName] as CremaBinaryTable;
            }
        }

        private ICollection Collection
        {
            get { return this.tables.Values as ICollection; }
        }

        #region ITableCollection

        bool ITableCollection.Contains(string tableName)
        {
            return this.tables.Contains(tableName);
        }

        bool ITableCollection.IsTableLoaded(string tableName)
        {
            return true;
        }

        ITable ITableCollection.this[int index]
        {
            get { return this[index] as ITable; }
        }

        ITable ITableCollection.this[string tableName]
        {
            get { return this[tableName] as ITable; }
        }

        string[] ITableCollection.TableNames
        {
            get { return this.tableNames; }
        }

        #endregion

        #region ICollection

        void ICollection.CopyTo(Array array, int index)
        {
            this.Collection.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.tables.Count; }
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

        IEnumerator<ITable> IEnumerable<ITable>.GetEnumerator()
        {
            return this.Collection.Cast<ITable>().GetEnumerator();
        }

        #endregion
    }
}
