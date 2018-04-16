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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library.Random;
using Ntreev.Library;

namespace Ntreev.Crema.Data.Test
{
    public abstract class CremaDataColumn_ChangeTypeTestBase
    {
        protected CremaDataTable table = new CremaDataTable();
        protected CremaDataColumn column;

        protected CremaDataColumn_ChangeTypeTestBase(Type dataType)
        {
            this.column = this.table.Columns.Add("Column1", dataType);
        }
        
        protected void AddRows(params object[] fields)
        {
            foreach (var item in fields)
            {
                this.AddRow(item);
            }
        }

        private void AddRow(object field)
        {
            var r1 = this.table.NewRow();
            if (field == null || field == DBNull.Value)
                r1[this.column] = field;
            else if (field.GetType() == this.column.DataType)
                r1[this.column] = field;
            else
                Assert.Fail("invalid datatype");
            this.table.Rows.Add(r1);
        }

    }
}