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
using System.Data;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaDataColumn_Complex_Test
    {
        private CremaDataSet dataSet = new CremaDataSet();
        private CremaDataTable table;
        private CremaDataColumn[] columns;

        public CremaDataColumn_Complex_Test()
        {
            this.dataSet.AddRandoms(10);
            this.table = this.dataSet.Tables.Random();
            this.columns = this.dataSet.Tables.SelectMany(item => item.Columns).ToArray();
        }

        /// <summary>
        /// ///////////////////////////////////////////////////////
        /// </summary>
        [TestMethod]
        public void Add()
        {
            var column = this.table.Columns.Add();

            Assert.AreNotEqual(column.ColumnID, Guid.Empty);
            //Assert.AreEqual(column.Ordinal, 0);
            //Assert.AreEqual(column.ColumnName, "Column1");
            Assert.AreEqual(column.DataType, typeof(string));
            Assert.AreEqual(column.Comment, string.Empty);
            Assert.IsFalse(column.IsKey);
            Assert.IsFalse(column.Unique);
            Assert.IsFalse(column.ReadOnly);
            Assert.AreEqual(column.Table, table);
            Assert.AreEqual(column.Tags, TagInfo.All);
            Assert.AreEqual(column.Validation, string.Empty);
            Assert.AreEqual(column.Expression, string.Empty);
            Assert.IsNull(column.CremaType);
            Assert.IsTrue(column.AllowDBNull);
            Assert.AreEqual(column.DefaultValue, DBNull.Value);

            Assert.AreNotEqual(column.CreationInfo, SignatureDate.Empty);
            Assert.AreNotEqual(column.ModificationInfo, SignatureDate.Empty);
        }

        //[TestMethod]
        //public void SetCremaType()
        //{
        //    var dataSet = CremaDataSetExtensions.CreateRandomSet();
        //    var table = dataSet.Tables.Add();
        //    var column = new CremaDataColumn();
        //    column.CremaType = dataSet.Types.Random();
        //    table.Columns.Add(column);

        //    //Assert.IsTrue(column.CremaType.References.Contains(column));
        //    Assert.AreEqual(column.DataType, typeof(string));
        //}

        [TestMethod]
        public void SetCremaType_Fail1()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var table = new CremaDataTable();
            var column = new CremaDataColumn();
            column.CremaType = dataSet.Types.Random();

            try
            {
                table.Columns.Add(column);
            }
            catch (CremaDataException)
            {

            }
            catch
            {
                Assert.Inconclusive();
            }
        }

        [TestMethod]
        public void SetCremaType_Fail2()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var table = new CremaDataTable();
            var column = new CremaDataColumn("Column1", dataSet.Types.Random());

            try
            {
                table.Columns.Add(column);
            }
            catch (CremaDataException)
            {

            }
            catch
            {
                Assert.Inconclusive();
            }
        }

        [TestMethod]
        public void SetCremaType_Fail3()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var table = new CremaDataTable();

            try
            {
                var column = table.Columns.Add("Column1", dataSet.Types.Random());
            }
            catch (CremaDataException)
            {

            }
            catch
            {
                Assert.Inconclusive();
            }
        }

        [TestMethod]
        public void Remove()
        {
            var table = new CremaDataTable();
            var column = table.Columns.Add();
            table.Columns.Remove(column);

            Assert.AreEqual(column.Index, -1);
            Assert.AreEqual(column.DataType, typeof(string));
            Assert.AreEqual(column.Comment, string.Empty);
            Assert.IsFalse(column.IsKey);
            Assert.IsFalse(column.Unique);
            Assert.IsFalse(column.ReadOnly);
            Assert.IsNull(column.Table);
            Assert.AreEqual(column.Tags, TagInfo.All);
            Assert.AreEqual(column.Validation, string.Empty);
            Assert.AreEqual(column.Expression, string.Empty);
            Assert.IsNull(column.CremaType);
            Assert.IsTrue(column.AllowDBNull);
            Assert.AreEqual(column.DefaultValue, DBNull.Value);

            Assert.AreEqual(column.CreationInfo, SignatureDate.Empty);
            Assert.AreEqual(column.ModificationInfo, SignatureDate.Empty);
        }

        [TestMethod]
        public void Rename()
        {
            var table = new CremaDataTable();
            var column = table.Columns.Add(RandomUtility.NextIdentifier());

            var newName = RandomUtility.NextIdentifier();
            column.ColumnName = newName;

            Assert.AreEqual(column.ColumnName, newName);
        }

        [TestMethod]
        public void Rename_Fail()
        {
            var table = new CremaDataTable();
            var column = table.Columns.Add(RandomUtility.NextIdentifier());

            var oldName = column.ColumnName;
            var newName = RandomUtility.NextIdentifier() + " " + RandomUtility.NextIdentifier();
            try
            {
                column.ColumnName = newName;
                Assert.Inconclusive();
            }
            catch { }

            Assert.AreEqual(column.ColumnName, oldName);
        }

        [TestMethod]
        public void ChangeDataType_Complex()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();

            var query = from table in dataSet.Tables
                        where table.Parent == null && table.TemplateNamespace == string.Empty
                        from column in table.Columns
                        where column.DataType == typeof(int) && column.AutoIncrement == false
                        select column;

            var columns = query.ToArray();
            foreach (var item in columns)
            {
                item.DataType = typeof(string);
            }

            foreach (var item in columns)
            {
                item.DataType = typeof(int);
            }
        }

        [TestMethod]
        public void ChangeDataType_Fail()
        {
            var table = new CremaDataTable();

            try
            {
                table.Columns.Add(RandomUtility.NextIdentifier(), typeof(AttributeTargets));
                Assert.Inconclusive();
            }
            catch
            {

            }

            Assert.IsFalse(table.Columns.Any());

            var column = table.Columns.Add(RandomUtility.NextIdentifier());

            try
            {
                column.DataType = typeof(AttributeTargets);
                Assert.Inconclusive();
            }
            catch
            {

            }

            Assert.IsNull(column.CremaType);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetCremaType_Fail()
        {
            var type = new CremaDataType();
            var column = new CremaDataColumn();
            column.CremaType = type;
        }

        [TestMethod]
        public void ChangeCremaType_Fail()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var table = new CremaDataTable();

            try
            {
                table.Columns.Add(RandomUtility.NextIdentifier(), dataSet.Types.Random());
                Assert.Inconclusive();
            }
            catch
            {

            }

            Assert.IsFalse(table.Columns.Any());

            var column = table.Columns.Add(RandomUtility.NextIdentifier());

            try
            {
                column.CremaType = dataSet.Types.Random();
                Assert.Inconclusive();
            }
            catch
            {

            }

            Assert.IsNull(column.CremaType);
        }

        [TestMethod]
        public void Create()
        {
            foreach (var item in CremaDataTypeUtility.GetBaseTypes())
            {
                var column = new CremaDataColumn(RandomUtility.NextIdentifier(), item);
                Assert.IsNotNull(column);
            }
        }

        [TestMethod]
        public void CreateWithCremaType()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            foreach (var item in dataSet.Types)
            {
                var column = new CremaDataColumn(RandomUtility.NextIdentifier(), item);
                Assert.IsNotNull(column);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateWithCremaType_Fail()
        {
            var column = new CremaDataColumn("Column1", new CremaDataType());
        }

        [TestMethod]
        public void ChangeDataType1()
        {
            var dataSet = new CremaDataSet();
            var type = dataSet.Types.Add();
            type.AddRandomMembers(10);

            var table = dataSet.Tables.Add();
            var column = table.Columns.Add();

            foreach (var item in type.Members)
            {
                var row = table.NewRow();
                row[column] = item.Name;
                table.Rows.Add(row);
            }

            column.CremaType = type;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ChangeDataType1_Fail()
        {
            var dataSet = new CremaDataSet();
            var type = dataSet.Types.Add();
            type.AddRandomMembers(10);

            var table = dataSet.Tables.Add();
            var column = table.Columns.Add();

            foreach (var item in type.Members)
            {
                var row = table.NewRow();
                row[column] = item.Name + RandomUtility.NextWord();
                table.Rows.Add(row);
            }

            column.CremaType = type;
        }

        [TestMethod]
        public void Clear()
        {
            var table = new CremaDataTable();
            var column = table.Columns.Add();
            table.Columns.Clear();

            Assert.AreEqual(column.Index, -1);
            Assert.AreEqual(column.DataType, typeof(string));
            Assert.AreEqual(column.Comment, string.Empty);
            Assert.IsFalse(column.IsKey);
            Assert.IsFalse(column.Unique);
            Assert.IsFalse(column.ReadOnly);
            Assert.IsNull(column.Table);
            Assert.AreEqual(column.Tags, TagInfo.All);
            Assert.AreEqual(column.Validation, string.Empty);
            Assert.AreEqual(column.Expression, string.Empty);
            Assert.IsNull(column.CremaType);
            Assert.IsTrue(column.AllowDBNull);
            Assert.AreEqual(column.DefaultValue, DBNull.Value);

            Assert.AreEqual(column.CreationInfo, SignatureDate.Empty);
            Assert.AreEqual(column.ModificationInfo, SignatureDate.Empty);
        }

        [TestMethod]
        public void ChangeCremaTypeFromStringType()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);
            dataType.AddMember("B", 2);
            dataType.AddMember("C", 3);

            var dataTable = dataSet.Tables.Add();
            var dataColumn = dataTable.Columns.Add();
            var defaultValue = dataType.Members.Random().Name;
            dataColumn.DefaultValue = defaultValue;

            foreach (var item in dataType.Members)
            {
                var dataRow = dataTable.NewRow();
                dataRow[dataColumn] = item.Name;
                dataTable.Rows.Add(dataRow);
            }

            dataColumn.CremaType = dataType;

            Assert.AreEqual(dataType, dataColumn.CremaType);
            Assert.AreEqual(defaultValue, dataColumn.DefaultValue);
        }

        [TestMethod]
        public void CheckDefaultValueOnChangeMemberName()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);
            dataType.AddMember("B", 2);
            dataType.AddMember("C", 3);
            dataType.AcceptChanges();

            var dataTable = dataSet.Tables.Add();
            var dataColumn = dataTable.Columns.Add();
            dataColumn.CremaType = dataType;

            var typeMember = dataType.Members.Random();
            var defaultValue = typeMember.Name;
            dataColumn.DefaultValue = defaultValue;

            foreach (var item in dataType.Members)
            {
                var dataRow = dataTable.NewRow();
                dataRow[dataColumn] = item.Name;
                dataTable.Rows.Add(dataRow);
            }
            dataTable.AcceptChanges();

            var memberName = RandomUtility.NextIdentifier();
            typeMember.Name = memberName;
            Assert.AreEqual(memberName, dataColumn.DefaultValue);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Remove_Fail()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = new CremaDataColumn();
            dataTable.Columns.Remove(dataColumn);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Remove_Fail1()
        {
            var dataTable = new CremaDataTable();
            var dataColumn = null as CremaDataColumn;
            dataTable.Columns.Remove(dataColumn);
        }
    }
}
