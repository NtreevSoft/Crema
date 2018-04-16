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

namespace Ntreev.Crema.Reader.Binary
{
    class CremaBinaryColumn : IColumn
    {
        private string columnName;
        private Type type;
        private bool isKey;
        private int index;

        public CremaBinaryColumn(string columnName, Type type, bool isKey)
        {
            this.columnName = columnName;
            this.type = type;
            this.isKey = isKey;
        }

        public string Name
        {
            get { return this.columnName; }
        }

        public Type DataType
        {
            get { return this.type; }
        }

        public bool IsKey
        {
            get { return this.isKey; }
        }

        public int Index
        {
            get { return this.index; }
            internal set { this.index = value; }
        }

        public CremaBinaryTable Table
        {
            get;
            set;
        }

        #region IColumn

        ITable IColumn.Table
        {
            get { return this.Table; }
        }

        #endregion
    }
}
