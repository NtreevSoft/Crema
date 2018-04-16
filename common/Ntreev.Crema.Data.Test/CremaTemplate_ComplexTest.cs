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
using Ntreev.Library.Random;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaTemplate_ComplexTest
    {
        private readonly CremaDataSet dataSet;

        public CremaTemplate_ComplexTest()
        {
            this.dataSet = CremaDataSetExtensions.CreateRandomSet();
        }

        [TestMethod]
        public void Add()
        {
            var table = this.dataSet.Tables.First();
            var template = new CremaTemplate(table);
            var member = template.NewColumn();
            template.Columns.Add(member);
        }

        [TestMethod]
        public void Add_Fail()
        {
            var table = this.dataSet.Tables.First();
            var template = new CremaTemplate(table);
            var member = template.NewColumn();
            member.Name = RandomUtility.Random(table.Columns).ColumnName;

            try
            {
                template.Columns.Add(member);
                Assert.Inconclusive();
            }
            catch (ConstraintException)
            {

            }
            catch
            {
                Assert.Inconclusive();
            }
        }

        [TestMethod]
        public void AddEmpty()
        {
            var table = this.dataSet.Tables.First();
            var template = new CremaTemplate(table);

            var memberCount = template.Columns.Count;
            var member = template.NewColumn();
            template.Columns.Add(member);
            Assert.AreNotEqual(member.Name, string.Empty);
            Assert.AreEqual(member.Index + 1, template.Columns.Count);
            Assert.AreEqual(memberCount + 1, template.Columns.Count);
        }

        [TestMethod]
        public void SetIndex()
        {
            var table = this.dataSet.Tables.First();
            var template = new CremaTemplate(table);

            for (int i = 0; i < 100; i++)
            {
                var member = RandomUtility.Random(template.Columns);
                var index = RandomUtility.Next(template.Columns.Count);
                member.Index = index;
                Assert.AreEqual(member, template.Columns[index]);
            }
        }

        [TestMethod]
        public void SetIndex_Fail()
        {
            var table = this.dataSet.Tables.First();
            var template = new CremaTemplate(table);

            var member = RandomUtility.Random(template.Columns);
            var index = RandomUtility.Next(template.Columns.Count);
            try
            {
                member.Index = int.MaxValue;
                Assert.Inconclusive();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
            catch
            {
                Assert.Inconclusive();
            }
        }

        //[TestMethod]
        //public void Merge()
        //{
        //    var table = this.dataSet.Tables.Random(item => item.TemplatedParent == null);

        //    var template = new CremaTemplate(table);
        //    CremaRandomUtility.RandomTask(template, 30);

        //    string schema1 = template.GetXmlSchema();
        //    string xml1 = template.GetXml();

        //    template.MergeTo(table);
        //    string schema2 = table.DataSet.GetXmlSchema();
        //    string xml2 = table.DataSet.GetXml();

        //    Assert.AreEqual(schema1, schema2);
        //    Assert.AreEqual(xml1, xml2);
        //}

        [TestMethod]
        public void XmlSerializing()
        {
            var table = this.dataSet.Tables.Random(item => item.TemplatedParent == null);
            var template1 = new CremaTemplate(table);

            //CremaRandomUtility.RandomTask(template1, 30);

            var xml1 = XmlSerializerUtility.GetString(template1, true);
            var template2 = XmlSerializerUtility.ReadString<CremaTemplate>(xml1);
            var xml2 = XmlSerializerUtility.GetString(template2, true);

            try
            {
                Assert.AreEqual(xml1, xml2);
            }
            catch
            {
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), template1.TableName + "_template1" + CremaSchema.SchemaExtension), xml1);
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), template2.TableName + "_template2" + CremaSchema.SchemaExtension), xml2);
                throw;
            }
        }

        [TestMethod]
        public void Test1()
        {
            var dataTable = new CremaDataTable("Table1");
            var template = new CremaTemplate(dataTable);
            var column1 = template.NewColumn();
            column1.Comment = "12";
            Assert.AreEqual(DateTime.MinValue, column1.CreationInfo.DateTime);
            Assert.AreEqual(DateTime.MinValue, column1.ModificationInfo.DateTime);
            template.Columns.Add(column1);
            Assert.AreNotEqual(DateTime.MinValue, column1.CreationInfo.DateTime);
            Assert.AreNotEqual(DateTime.MinValue, column1.ModificationInfo.DateTime);
            Assert.AreEqual(column1.CreationInfo.DateTime, column1.ModificationInfo.DateTime);
            Thread.Sleep(100);
            column1.IsKey = true;
            Assert.IsTrue(column1.IsKey);
            Assert.IsTrue(column1.Unique);
            Assert.AreNotEqual(column1.CreationInfo.DateTime, column1.ModificationInfo.DateTime);
            var column2 = template.NewColumn();
            template.Columns.Add(column2);
            column2.IsKey = true;
            Assert.IsFalse(column1.Unique);
        }

        [TestMethod]
        public void Test2()
        {
            var dataTable = new CremaDataTable();
            var c1 = dataTable.Columns.Add("Column1", typeof(float));

            var template = new CremaTemplate(dataTable);
            template.Columns[0].DefaultValue = "1000000.0";
            template.Columns[0].DataTypeName = typeof(DateTime).GetTypeName();
            Assert.IsTrue(c1.DefaultValue is DateTime);
            template.Columns[0].DataTypeName = typeof(float).GetTypeName();
            Assert.IsTrue(c1.DefaultValue is float);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Test3_Fail()
        {
            var dataTable = new CremaDataTable();
            var c1 = dataTable.Columns.Add("Column1", typeof(float));

            var template = new CremaTemplate(dataTable);
            template.Columns[0].DefaultValue = "10000000.0";
            template.Columns[0].DataTypeName = typeof(DateTime).GetTypeName();
        }

        [TestMethod]
        public void Test4()
        {
            var dataTable = new CremaDataTable();
            var c1 = dataTable.Columns.Add("Column1", typeof(string));
            var template = new CremaTemplate(dataTable);
            var defaultString = "12.3";
            var defaultValue = CremaConvert.ChangeType(defaultString, typeof(float));
            template.Columns[0].DefaultValue = defaultString;
            template.Columns[0].SetDataType(typeof(float));
            template.Columns[0].SetDataType(typeof(DateTime));
            Assert.AreEqual(c1.DefaultValue, template.Columns[0].DefaultValue);
            template.Columns[0].SetDataType(typeof(float));
            Assert.AreEqual(defaultValue, template.Columns[0].DefaultValue);
        }

        [TestMethod]
        public void Test5()
        {
            var dataTable = new CremaDataTable();
            var c1 = dataTable.Columns.Add("Column1", typeof(string));
            var template = new CremaTemplate(dataTable);
            template.Columns[0].DefaultValue = "1";
            template.Columns[0].SetDataType(typeof(TimeSpan));
            template.Columns[0].DefaultValue = "62.9";
        }

        [TestMethod]
        public void Test6()
        {
            var dataTable = new CremaDataTable();
            var c1 = dataTable.Columns.Add("Column1", typeof(string));
            var template = new CremaTemplate(dataTable);
            template.Columns[0].DefaultValue = "1";
            template.Columns[0].SetDataType(typeof(TimeSpan));
            template.Columns[0].DefaultValue = TimeSpan.MinValue.ToString();
            template.Columns[0].DefaultValue = TimeSpan.MaxValue.ToString();
        }

        [TestMethod]
        public void Test7()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.AddRandomType();
            var dataTable = dataSet.AddRandomTable();
            var template = new CremaTemplate(dataTable);
            var column = template.NewColumn();
            column.DataTypeName = "/type1";
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test7_1()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.AddRandomType();
            var dataTable = dataSet.AddRandomTable();
            var template = new CremaTemplate(dataTable);
            var column = template.NewColumn();
            template.Columns.Add(column);
            column.DataTypeName = "/type1";
        }

        [TestMethod]
        public void Test8()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();
            var dataTable = dataSet.AddRandomTable();
            var derivedTable = dataSet.AddDerivedTable();
            var columnCount1 = dataTable.Columns.Count;

            var template = new CremaTemplate(dataTable);
            var columnCount2 = template.Columns.Count;
            var column = template.Columns.RandomOrDefault(item => item.IsKey == false && item.Unique == false);
            if (column == null)
                return;

            column.Delete();
            template.AcceptChanges();

            Assert.AreEqual(columnCount1 - 1, template.Columns.Count);
            Assert.AreEqual(columnCount1 - 1, dataTable.Columns.Count);
            Assert.AreEqual(columnCount1 - 1, derivedTable.Columns.Count);
        }

        [TestMethod]
        public void Test9()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.AddRandomType();
            var dataTable = dataSet.Tables.Add();

            var template = new CremaTemplate(dataTable);
            var column = template.NewColumn();

            column.DataTypeName = dataType.Path;

            template.Columns.Add(column);
            column.DataTypeName = typeof(int).GetTypeName();
            column.AutoIncrement = true;
            column.DataTypeName = dataType.Path;
            Assert.AreEqual(typeof(string), dataTable.Columns[0].DataType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test10()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.AddMember("None", 0);
            dataType.AddMember("A", 0);

            var dataTable = dataSet.Tables.Add();
            dataTable.Columns.AddKey("column1");

            var dataRow = dataTable.NewRow();
            dataRow[0] = "None";
            dataTable.Rows.Add(dataRow);

            var dataRow2 = dataTable.NewRow();
            dataRow2[0] = "B";
            dataTable.Rows.Add(dataRow2);

            dataSet.AcceptChanges();

            var template = new CremaTemplate(dataTable);
            template.Columns[0].DataTypeName = dataType.Path;
        }

        [TestMethod]
        public void Test11()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);

            var dataTable = dataSet.Tables.Add();
            dataTable.Columns.AddKey("column1");

            var dataRow = dataTable.NewRow();
            dataRow[0] = "None";
            dataTable.Rows.Add(dataRow);

            var dataRow2 = dataTable.NewRow();
            dataRow2[0] = "A";
            dataTable.Rows.Add(dataRow2);
            var dataTable2 = dataTable.Inherit("Derived1");

            dataSet.AcceptChanges();

            //dataTable.Columns[0].CremaType = dataType;
            var template = new CremaTemplate(dataTable);
            template.Columns[0].DataTypeName = dataType.Path;

            Assert.AreEqual(dataType, dataTable.Columns[0].CremaType);
            Assert.AreEqual(dataType, dataTable2.Columns[0].CremaType);
        }

        [TestMethod]
        public void UniqueTest1()
        {
            var dataSet = new CremaDataSet();

            var template = CremaTemplate.Create(dataSet, "table1", "/");
            var column1 = template.NewColumn();
            column1.IsKey = true;
            template.Columns.Add(column1);
            Assert.AreEqual(true, column1.IsKey);
            Assert.AreEqual(true, column1.Unique);

            var column2 = template.NewColumn();
            column2.IsKey = true;
            template.Columns.Add(column2);

            Assert.AreEqual(true, column1.IsKey);
            Assert.AreEqual(true, column2.IsKey);
            Assert.AreEqual(false, column1.Unique);
            Assert.AreEqual(false, column2.Unique);
        }

        [TestMethod]
        public void Test12()
        {
            var dataSet1 = new CremaDataSet();
            var template = CremaTemplate.Create(dataSet1, "table1", "/");
            var column1 = template.AddColumn("Column1");
            var column2 = template.AddColumn("Column2");
            var column3 = template.AddColumn("Column3");

            column3.Index = 0;

            Assert.AreEqual(0, column3.Index);
            Assert.AreEqual(1, column1.Index);
            Assert.AreEqual(2, column2.Index);
        }
    }
}
