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
using Ntreev.Library;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaAttribute_Test
    {
        private readonly string userID;
        private readonly CremaDataSet dataSet;
        private readonly CremaDataTable dataTable;
        private readonly CremaDataTable derivedTable;

        public CremaAttribute_Test()
        {
            this.userID = RandomUtility.NextWord();
            this.dataSet = new CremaDataSet() { SignatureDateProvider = new SignatureDateProvider(userID) };
            this.dataTable = this.dataSet.AddRandomTable();
            this.dataTable.AddRandomAttributes(10);
            this.derivedTable = this.dataTable.Inherit(RandomUtility.NextIdentifier());

            this.dataSet.AcceptChanges();
        }

        [TestMethod]
        public void Add_CompareWithDerived()
        {
            var attribute1 = this.dataTable.AddRandomAttribute();
            var attribute2 = this.derivedTable.Attributes[attribute1.AttributeName];

            Assert.AreEqual(attribute1.AttributeName, attribute2.AttributeName);
            Assert.AreEqual(attribute1.AllowDBNull, attribute2.AllowDBNull);
            Assert.AreEqual(attribute1.AutoIncrement, attribute2.AutoIncrement);
            Assert.AreEqual(attribute1.DataType, attribute2.DataType);
            Assert.AreEqual(attribute1.Comment, attribute2.Comment);
            Assert.AreEqual(attribute1.DefaultValue, attribute2.DefaultValue);
        }

        [TestMethod]
        public void Rename_CompareWithDerived()
        {
            var attribute1 = this.dataTable.Attributes.Random(item => item.DefaultValue == DBNull.Value);
            var attribute2 = this.derivedTable.Attributes[attribute1.AttributeName];

            Thread.Sleep(100);
            attribute1.AttributeName = RandomUtility.NextIdentifier();
            Assert.AreEqual(attribute1.AttributeName, attribute2.AttributeName);
        }
    }
}
