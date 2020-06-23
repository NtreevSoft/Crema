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

namespace Ntreev.Crema.Code.Reader.Binary
{
    class CremaBinaryTable : ITable
    {
        private string tableName;
        private string categoryName;
        private int index;
        private string hashValue;

        private CremaBinaryColumnCollection columns;
        private readonly CremaBinaryRowCollection rows;
        private IColumn[] keys;
        private readonly CremaBinaryReader reader;

        public CremaBinaryTable(CremaBinaryReader reader, int rowCount, ReadOptions options)
        {
            this.reader = reader;
            this.rows = new CremaBinaryRowCollection(this, rowCount);
        }

        public override string ToString()
        {
            return this.tableName ?? base.ToString();
        }

        public string Category
        {
            get { return this.categoryName; }
            set { this.categoryName = value; }
        }

        public string Name
        {
            get { return this.tableName; }
            set { this.tableName = value; }
        }

        public int Index
        {
            get { return this.index; }
            internal set { this.index = value; }
        }

        public string HashValue
        {
            get { return this.hashValue; }
            set { this.hashValue = value; }
        }

        public CremaBinaryColumnCollection Columns
        {
            get { return this.columns; }
            set { this.columns = value; }
        }

        public CremaBinaryRowCollection Rows
        {
            get { return this.rows; }
        }

        public CremaBinaryReader Reader
        {
            get { return this.reader; }
        }

        public IColumn[] Keys
        {
            get { return this.keys; }
            set { this.keys = value; }
        }

        #region ITable

        IColumn[] ITable.Keys
        {
            get { return this.keys; }
        }

        IRowCollection ITable.Rows
        {
            get { return this.rows; }
        }

        IColumnCollection ITable.Columns
        {
            get { return this.columns; }
        }

        IDataSet ITable.DataSet
        {
            get { return this.reader; }
        }

        #endregion
    }
}
