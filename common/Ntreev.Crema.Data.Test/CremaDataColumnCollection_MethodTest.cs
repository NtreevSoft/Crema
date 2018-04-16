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
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaDataColumnCollection_MethodTest
    {
        public CremaDataColumnCollection_MethodTest()
        {

        }

        [TestMethod]
        public void Add()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = dataTable.Columns.Add();
            Assert.AreEqual(1, dataTable.Columns.Count);
        }

        [TestMethod]
        public void AddMany()
        {
            var dataTable = new CremaDataTable();
            var columnCount = RandomUtility.Next(1, 10);
            for (var i = 0; i < columnCount; i++)
            {
                dataTable.Columns.Add();
            }
            Assert.AreEqual(columnCount, dataTable.Columns.Count);
        }

        [TestMethod]
        public void AddKey()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = dataTable.Columns.AddKey();
            Assert.AreEqual(1, dataTable.Columns.Count);
            Assert.AreEqual(1, dataTable.PrimaryKey.Length);
        }

        [TestMethod]
        public void AddKeyMany()
        {
            var dataTable = new CremaDataTable();
            var columnCount = RandomUtility.Next(1, 10);
            for (var i = 0; i < columnCount; i++)
            {
                dataTable.Columns.AddKey();
            }
            Assert.AreEqual(columnCount, dataTable.Columns.Count);
            Assert.AreEqual(columnCount, dataTable.PrimaryKey.Length);
        }

        [TestMethod]
        public void AddColumn()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = new CremaDataColumn();
            dataTable.Columns.Add(dataColumn);
            Assert.AreEqual(1, dataTable.Columns.Count);
        }

        [TestMethod]
        public void AddColumnMany()
        {
            var dataTable = new CremaDataTable();
            var columnCount = RandomUtility.Next(1, 10);
            for (var i = 0; i < columnCount; i++)
            {
                var dataColumn = new CremaDataColumn();
                dataTable.Columns.Add(dataColumn);
            }
            Assert.AreEqual(columnCount, dataTable.Columns.Count);
        }

        [TestMethod]
        public void AddKeyColumn()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = new CremaDataColumn() { IsKey = true };
            dataTable.Columns.Add(dataColumn);
            Assert.AreEqual(1, dataTable.Columns.Count);
            Assert.AreEqual(1, dataTable.PrimaryKey.Length);
        }

        [TestMethod]
        public void AddKeyColumnMany()
        {
            var dataTable = new CremaDataTable();
            var columnCount = RandomUtility.Next(1, 10);
            for (var i = 0; i < columnCount; i++)
            {
                var dataColumn = new CremaDataColumn() { IsKey = true };
                dataTable.Columns.Add(dataColumn);
            }
            Assert.AreEqual(columnCount, dataTable.Columns.Count);
            Assert.AreEqual(columnCount, dataTable.PrimaryKey.Length);
        }

        [TestMethod]
        public void AddColumnByName()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = dataTable.Columns.Add("Column1");
            Assert.AreEqual(1, dataTable.Columns.Count);
        }

        [TestMethod]
        public void AddColumnByNameMany()
        {
            var dataTable = new CremaDataTable();
            var columnCount = RandomUtility.Next(1, 10);
            for (var i = 0; i < columnCount; i++)
            {
                var dataColumn = dataTable.Columns.Add($"Column{i}");
            }
            Assert.AreEqual(columnCount, dataTable.Columns.Count);
        }

        [TestMethod]
        public void AddKeyColumnByName()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = dataTable.Columns.AddKey("Column1");
            Assert.AreEqual(1, dataTable.Columns.Count);
            Assert.AreEqual(1, dataTable.PrimaryKey.Length);
        }

        [TestMethod]
        public void AddKeyColumnByNameMany()
        {
            var dataTable = new CremaDataTable();
            var columnCount = RandomUtility.Next(1, 10);
            for (var i = 0; i < columnCount; i++)
            {
                var dataColumn = dataTable.Columns.AddKey($"Column{i}");
            }
            Assert.AreEqual(columnCount, dataTable.Columns.Count);
            Assert.AreEqual(columnCount, dataTable.PrimaryKey.Length);
        }

        [TestMethod]
        public void AddColumnByNameAndType()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = dataTable.Columns.Add("Column1", typeof(string));
            Assert.AreEqual(1, dataTable.Columns.Count);
        }

        [TestMethod]
        public void AddColumnByNameAndTypeMany()
        {
            var dataTable = new CremaDataTable();
            var dataTypes = CremaDataTypeUtility.GetBaseTypes();
            for (var i = 0; i < dataTypes.Length; i++)
            {
                var dataColumn = dataTable.Columns.Add($"Column{i}", dataTypes[i]);
            }
            Assert.AreEqual(dataTypes.Length, dataTable.Columns.Count);
        }

        [TestMethod]
        public void AddKeyColumnByNameAndType()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = dataTable.Columns.AddKey("Column1", typeof(string));
            Assert.AreEqual(1, dataTable.Columns.Count);
            Assert.AreEqual(1, dataTable.PrimaryKey.Length);
        }

        [TestMethod]
        public void AddKeyColumnByNameAndTypeMany()
        {
            var dataTable = new CremaDataTable();
            var dataTypes = CremaDataTypeUtility.GetBaseTypes();
            for (var i = 0; i < dataTypes.Length; i++)
            {
                var dataColumn = dataTable.Columns.AddKey($"Column{i}", dataTypes[i]);
            }
            Assert.AreEqual(dataTypes.Length, dataTable.Columns.Count);
            Assert.AreEqual(dataTypes.Length, dataTable.PrimaryKey.Length);
        }

        [TestMethod]
        public void AddColumnByNameAndCremaType()
        {
            var dataSet = new CremaDataSet();
            dataSet.AddRandomTypes(10);
            var dataTable = dataSet.Tables.Add();
            var dataColumn = dataTable.Columns.Add("Column1", dataSet.Types.Random());
            Assert.AreEqual(1, dataTable.Columns.Count);
        }

        [TestMethod]
        public void AddColumnByNameAndCremaTypeMany()
        {
            var dataSet = new CremaDataSet();
            dataSet.AddRandomTypes(10);
            var dataTable = dataSet.Tables.Add();
            for (var i = 0; i < dataSet.Types.Count; i++)
            {
                var dataColumn = dataTable.Columns.Add($"Column{i}", dataSet.Types[i]);
            }
            Assert.AreEqual(dataSet.Types.Count, dataTable.Columns.Count);
        }

        [TestMethod]
        public void AddKeyColumnByNameAndCremaType()
        {
            var dataSet = new CremaDataSet();
            dataSet.AddRandomTypes(10);
            var dataTable = dataSet.Tables.Add();
            var dataColumn = dataTable.Columns.AddKey("Column1", dataSet.Types.Random());
            Assert.AreEqual(1, dataTable.Columns.Count);
            Assert.AreEqual(1, dataTable.PrimaryKey.Length);
        }

        [TestMethod]
        public void AddKeyColumnByNameAndCremaTypeMany()
        {
            var dataSet = new CremaDataSet();
            dataSet.AddRandomTypes(10);
            var dataTable = dataSet.Tables.Add();
            for (var i = 0; i < dataSet.Types.Count; i++)
            {
                var dataColumn = dataTable.Columns.AddKey($"Column{i}", dataSet.Types[i]);
            }
            Assert.AreEqual(dataSet.Types.Count, dataTable.Columns.Count);
            Assert.AreEqual(dataSet.Types.Count, dataTable.PrimaryKey.Length);
        }

        [TestMethod]
        public void CanRemove()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = dataTable.Columns.Add();
            var result = dataTable.Columns.CanRemove(dataColumn);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanRemoveKey()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = dataTable.Columns.AddKey();
            var result = dataTable.Columns.CanRemove(dataColumn);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Clear()
        {
            var dataTable = new CremaDataTable();
            var columnCount = RandomUtility.Next(1, 10);
            for (var i = 0; i < columnCount; i++)
            {
                var dataColumn = dataTable.Columns.Add();
            }
            dataTable.Columns.Clear();
            Assert.AreEqual(0, dataTable.Columns.Count);
        }

        [TestMethod]
        public void Remove()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = dataTable.Columns.Add();
            dataTable.Columns.Remove(dataColumn);

            Assert.AreEqual(dataTable.Columns.Count, 0);
        }

        [TestMethod]
        public void RemoveMany()
        {
            var dataTable = new CremaDataTable();
            var columnCount = RandomUtility.Next(1, 10);
            for (var i = 0; i < columnCount; i++)
            {
                var dataColumn = dataTable.Columns.Add();
            }
            var columns = dataTable.Columns.ToArray();
            for (var i = 0; i < columns.Length; i++)
            {
                var dataColumn = columns[i];
                dataTable.Columns.Remove(dataColumn);
            }
            Assert.AreEqual(dataTable.Columns.Count, 0);
        }

        [TestMethod]
        public void RemoveByName()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = dataTable.Columns.Add();
            dataTable.Columns.Remove(dataColumn.ColumnName);
            Assert.AreEqual(dataTable.Columns.Count, 0);
        }

        [TestMethod]
        public void RemoveByNameMany()
        {
            var dataTable = new CremaDataTable();
            var columnCount = RandomUtility.Next(1, 10);
            for (var i = 0; i < columnCount; i++)
            {
                var dataColumn = dataTable.Columns.Add();
            }
            var columns = dataTable.Columns.ToArray();
            for (var i = 0; i < columns.Length; i++)
            {
                var dataColumn = columns[i];
                dataTable.Columns.Remove(dataColumn.ColumnName);
            }
            Assert.AreEqual(dataTable.Columns.Count, 0);
        }

        [TestMethod]
        public void RemoveByIndex()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = dataTable.Columns.Add();
            dataTable.Columns.RemoveAt(0);
            Assert.AreEqual(dataTable.Columns.Count, 0);
        }

        [TestMethod]
        public void RemoveByIndexMany()
        {
            var dataTable = new CremaDataTable();
            var columnCount = RandomUtility.Next(1, 10);
            for (var i = 0; i < columnCount; i++)
            {
                var dataColumn = dataTable.Columns.Add();
            }
            for (var i = columnCount - 1; i >= 0; i--)
            {
                dataTable.Columns.RemoveAt(i);
            }
            Assert.AreEqual(dataTable.Columns.Count, 0);
        }

        [TestMethod]
        public void RemoveByRandomIndex()
        {
            var dataTable = new CremaDataTable();
            var columnCount = RandomUtility.Next(1, 10);
            for (var i = 0; i < columnCount; i++)
            {
                dataTable.Columns.Add();
            }
            var dataColumn = dataTable.Columns.Random();
            var index = dataColumn.Index;
            dataTable.Columns.RemoveAt(index);
            Assert.AreEqual(-1, dataColumn.Index);
            Assert.AreEqual(columnCount - 1, dataTable.Columns.Count);
            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                Assert.AreEqual(i, dataTable.Columns[i].Index);
            }
        }

        [TestMethod]
        public void ContainsByName()
        {
            var dataTable = new CremaDataTable();
            var dataColumn1 = dataTable.Columns.Add();
            var result = dataTable.Columns.Contains(dataColumn1.ColumnName);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsByRandomName()
        {
            var dataTable = new CremaDataTable();
            dataTable.AddRandomColumns();
            var dataColumn1 = dataTable.Columns.Random();
            var result = dataTable.Columns.Contains(dataColumn1.ColumnName);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsByColumnID()
        {
            var dataTable = new CremaDataTable();
            var dataColumn1 = dataTable.Columns.Add();
            var result = dataTable.Columns.Contains(dataColumn1.ColumnID);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsByRandomColumnID()
        {
            var dataTable = new CremaDataTable();
            dataTable.AddRandomColumns();
            var dataColumn1 = dataTable.Columns.Random();
            var result = dataTable.Columns.Contains(dataColumn1.ColumnID);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IndexOf()
        {
            var dataTable = new CremaDataTable();
            var dataColumn1 = dataTable.Columns.Add();
            var result = dataTable.Columns.IndexOf(dataColumn1);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void IndexOfMany()
        {
            var dataTable = new CremaDataTable();
            dataTable.AddRandomColumns();
            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                var dataColumn = dataTable.Columns[i];
                var result = dataTable.Columns.IndexOf(dataColumn);
                Assert.AreEqual(i, result);
            }
        }

        [TestMethod]
        public void IndexOfByName()
        {
            var dataTable = new CremaDataTable();
            var dataColumn1 = dataTable.Columns.Add();
            var result = dataTable.Columns.IndexOf(dataColumn1.ColumnName);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void IndexOfByNameMany()
        {
            var dataTable = new CremaDataTable();
            dataTable.AddRandomColumns();
            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                var dataColumn = dataTable.Columns[i];
                var result = dataTable.Columns.IndexOf(dataColumn.ColumnName);
                Assert.AreEqual(i, result);
            }
        }

        [TestMethod]
        public void GetByIndex()
        {
            var dataTable = new CremaDataTable();
            var dataColumn1 = dataTable.Columns.Add();
            var dataColumn2 = dataTable.Columns[dataColumn1.Index];
            Assert.AreSame(dataColumn2, dataColumn1);
        }

        [TestMethod]
        public void GetByRandomIndex()
        {
            var dataTable = new CremaDataTable();
            dataTable.AddRandomColumns();

            var dataColumn1 = dataTable.Columns.Random();
            var dataColumn2 = dataTable.Columns[dataColumn1.Index];
            Assert.AreSame(dataColumn2, dataColumn1);
        }

        [TestMethod]
        public void GetByName()
        {
            var dataTable = new CremaDataTable();
            var dataColumn1 = dataTable.Columns.Add();
            var dataColumn2 = dataTable.Columns[dataColumn1.ColumnName];
            Assert.AreSame(dataColumn2, dataColumn1);
        }

        [TestMethod]
        public void GetByRandomName()
        {
            var dataTable = new CremaDataTable();
            dataTable.AddRandomColumns();
            var dataColumn1 = dataTable.Columns.Random();
            var dataColumn2 = dataTable.Columns[dataColumn1.ColumnName];
            Assert.AreSame(dataColumn2, dataColumn1);
        }

        [TestMethod]
        public void GetByColumnID()
        {
            var dataTable = new CremaDataTable();
            var dataColumn1 = dataTable.Columns.Add();
            var dataColumn2 = dataTable.Columns[dataColumn1.ColumnID];
            Assert.AreSame(dataColumn2, dataColumn1);
        }

        [TestMethod]
        public void GetByRandomColumnID()
        {
            var dataTable = new CremaDataTable();
            dataTable.AddRandomColumns();
            var dataColumn1 = dataTable.Columns.Random();
            var dataColumn2 = dataTable.Columns[dataColumn1.ColumnID];
            Assert.AreSame(dataColumn2, dataColumn1);
        }
    }
}
