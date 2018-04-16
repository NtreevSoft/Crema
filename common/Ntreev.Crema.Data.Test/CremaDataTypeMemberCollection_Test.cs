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
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaDataTypeMemberCollection_Test
    {
        [TestMethod]
        public void Remove()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);
            dataType.AddMember("B", 2);
            dataType.AddMember("C", 3);

            var member = dataType.Members.Random();
            var memberCount = dataType.Members.Count;

            dataType.Members.Remove(member);

            Assert.AreEqual(memberCount - 1, dataType.Members.Count);
        }

        [TestMethod]
        public void Clear1()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);
            dataType.AddMember("B", 2);
            dataType.AddMember("C", 3);

            var member = dataType.Members.Random();
            var memberCount = dataType.Members.Count;

            dataType.Members.Clear();

            Assert.AreEqual(0, dataType.Members.Count);
        }

        [TestMethod]
        public void Clear2()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);
            dataType.AddMember("B", 2);
            dataType.AddMember("C", 3);
            dataType.AcceptChanges();

            var dataTable = dataSet.Tables.Add();
            var keyColumn = dataTable.Columns.AddKey("Key", typeof(int));
            var valueColumn = dataTable.Columns.Add("Value", dataType);

            var dataRow = dataTable.AddRow(0, "None");

            dataType.Members.Clear();
            Assert.AreEqual(null, dataRow.Field<string>(valueColumn));
            Assert.AreEqual(DBNull.Value, dataRow[valueColumn]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConstraintException))]
        public void Clear_Fail1()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);
            dataType.AddMember("B", 2);
            dataType.AddMember("C", 3);

            dataType.AcceptChanges();
            
            var dataTable = dataSet.Tables.Add();
            var keyColumn = dataTable.Columns.AddKey("Key", typeof(int));
            var valueColumn = dataTable.Columns.Add("Value", dataType);
            valueColumn.AllowDBNull = false;

            var dataRow = dataTable.AddRow(0, "None");

            var member = dataType.Members.Random();
            var memberCount = dataType.Members.Count;

            dataType.Members.Clear();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConstraintException))]
        public void Clear_Fail2()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);
            dataType.AddMember("B", 2);
            dataType.AddMember("C", 3);
            dataType.AcceptChanges();
            
            var dataTable = dataSet.Tables.Add();
            var keyColumn = dataTable.Columns.AddKey("Key", typeof(int));
            var valueColumn = dataTable.Columns.Add("Value", dataType);
            valueColumn.Unique = true;

            var dataRow1 = dataTable.AddRow(0, "None");
            var dataRow2 = dataTable.AddRow(1, "A");

            var member = dataType.Members.Random();
            var memberCount = dataType.Members.Count;

            dataType.Members.Clear();
        }
    }
}