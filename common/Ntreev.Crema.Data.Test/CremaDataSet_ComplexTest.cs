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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ntreev.Crema.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;
using Ntreev.Crema.Data.Xml;
using Ntreev.Crema.Data.Random;
using Ntreev.Library.Random;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.IO;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaDataSet_ComplexTest
    {
        private readonly CremaDataSet dataSet;

        public CremaDataSet_ComplexTest()
        {
            this.dataSet = CremaDataSetExtensions.CreateRandomSet();
        }

        [TestMethod]
        public void WriteToStream_ReadFromStream()
        {
            var schemaStream = new MemoryStream();
            var xmlStream = new MemoryStream();
            this.dataSet.WriteXmlSchema(schemaStream);
            this.dataSet.WriteXml(xmlStream);

            schemaStream.Position = 0;
            xmlStream.Position = 0;

            var dataSet = CremaDataSet.ReadSchema(schemaStream);
            dataSet.ReadXml(xmlStream);

            CremaComparer.CompareDataSet(this.dataSet, dataSet);
        }

        [TestMethod]
        public void WriteToTextWriter_ReadFromTextReader()
        {
            var schemaWriter = new StringWriter();
            var xmlWriter = new StringWriter();
            this.dataSet.WriteXmlSchema(schemaWriter);
            this.dataSet.WriteXml(xmlWriter);

            var schemaReader = new StringReader(schemaWriter.ToString());
            var dataSet = CremaDataSet.ReadSchema(schemaReader);
            var xmlReader = new StringReader(xmlWriter.ToString());
            dataSet.ReadXml(xmlReader);

            CremaComparer.CompareDataSet(this.dataSet, dataSet);
        }

        [TestMethod]
        public void WriteToXmlWriter_ReadFromXmlReader()
        {
            var schemaBuilder = new StringBuilder();
            var xmlBuilder = new StringBuilder();

            using (var schemaWriter = XmlWriter.Create(schemaBuilder))
            {
                this.dataSet.WriteXmlSchema(schemaWriter);
            }

            using (var xmlWriter = XmlWriter.Create(xmlBuilder))
            {
                this.dataSet.WriteXml(xmlWriter);
            }

            CremaDataSet dataSet = null;
            using (var sr = new StringReader(schemaBuilder.ToString()))
            using (var schemaReader = XmlReader.Create(sr))
            {
                dataSet = CremaDataSet.ReadSchema(schemaReader);
            }

            using (var sr = new StringReader(xmlBuilder.ToString()))
            using (var xmlReader = XmlReader.Create(sr))
            {
                dataSet.ReadXml(xmlReader);
            }

            CremaComparer.CompareDataSet(this.dataSet, dataSet);
        }


        [TestMethod]
        public void RemoveChild()
        {
            var tableCount = this.dataSet.Tables.Count;
            var table = this.dataSet.Tables.Random(item => item.Childs.Any() && item.TemplateNamespace == string.Empty);
            var child = table.Childs.Random();
            var childCount = table.Childs.Count;
            table.Childs.Remove(child);

            Assert.AreEqual(childCount - 1, table.Childs.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveType_Fail()
        {
            var dataSet = new CremaDataSet();
            var type = dataSet.Types.Add();
            type.AddRandomMembers(10);
            type.AcceptChanges();

            var table = dataSet.Tables.Add();
            var column = table.Columns.Add("Column1", type);

            foreach (var item in type.Members)
            {
                var row = table.NewRow();
                row[column] = item.Name;
                table.Rows.Add(row);
            }

            dataSet.Types.Remove(type);
        }

        [TestMethod]
        public void RemoveType()
        {
            var dataSet = new CremaDataSet();
            var type = dataSet.Types.Add();
            type.AddRandomMembers(10);
            type.AcceptChanges();

            var table = dataSet.Tables.Add();
            var column = table.Columns.Add("Column1", type);

            foreach (var item in type.Members)
            {
                var row = table.NewRow();
                row[column] = item.Name;
                table.Rows.Add(row);
            }

            table.Inherit("table1_ref", true);

            column.CremaType = null;
            dataSet.Types.Remove(type);
        }

        [TestMethod]
        public void Test1()
        {
            var dataSet = new CremaDataSet();
            var table = dataSet.Tables.Add("Table1");

            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Value", typeof(string));

            table.Columns["ID"].IsKey = true;

            table.ModifyRandomRows(30);

            var inheritTable = table.Inherit("InheritTable1");

            var child = table.Childs.Add("Child1");

            child.Columns.Add("ID", typeof(int));
            child.Columns.Add("Value", typeof(string));

            child.Columns["ID"].IsKey = true;

            child.AddRandomRows(30);

            table.TableName = "A1";
        }

        [TestMethod]
        public void Test2()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var childTable = dataTable.Childs.Add();
            var derivedTable = dataTable.Inherit();
            var derivedChildTable = derivedTable.Childs[childTable.TableName];
            var derivedName = derivedTable.Name;

            dataTable.TableName = "a";
            childTable.TableName = "b";

            Assert.AreEqual("a", dataTable.Name);
            Assert.AreEqual("a", dataTable.TableName);
            Assert.AreEqual("a.b", childTable.Name);
            Assert.AreEqual("b", childTable.TableName);

            Assert.AreEqual(derivedName, derivedTable.Name);
            Assert.AreEqual(derivedName, derivedTable.TableName);
            Assert.AreEqual($"{derivedName}.b", derivedChildTable.Name);
            Assert.AreEqual("b", derivedChildTable.TableName);
        }

        [TestMethod]
        public void Test3()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add("Table1", "/Test/");
            var childTable = dataTable.Childs.Add();
            var derivedTable = dataTable.Inherit("Derived1", "/Test1/");
            var derivedChildTable = derivedTable.Childs[childTable.TableName];
            var derivedName = derivedTable.Name;

            dataTable.TableName = "a";
            childTable.TableName = "b";

            Assert.AreEqual("/Test/", dataTable.CategoryPath);
            Assert.AreEqual($"{CremaSchema.TableNamespace}/Test/a", dataTable.Namespace);
            Assert.AreEqual("/Test/", childTable.CategoryPath);
            Assert.AreEqual($"{CremaSchema.TableNamespace}/Test/a.b", childTable.Namespace);

            Assert.AreEqual("/Test1/", derivedTable.CategoryPath);
            Assert.AreEqual($"{CremaSchema.TableNamespace}/Test1/{derivedName}", derivedTable.Namespace);
            Assert.AreEqual("/Test1/", derivedChildTable.CategoryPath);
            Assert.AreEqual($"{CremaSchema.TableNamespace}/Test1/{derivedName}.b", derivedChildTable.Namespace);
        }

        [TestMethod]
        public void Test4()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add("Table1", "/Test/");
            var childTable = dataTable.Childs.Add("Child1");
            var derivedTable = dataTable.Inherit("Derived1", "/Test1/");
            var derivedChildTable = derivedTable.Childs[childTable.TableName];
            var derivedName = derivedTable.Name;

            Assert.AreEqual("/Test/", dataTable.CategoryPath);
            Assert.AreEqual($"{CremaSchema.TableNamespace}/Test/Table1", dataTable.Namespace);
            Assert.AreEqual("/Test/", childTable.CategoryPath);
            Assert.AreEqual($"{CremaSchema.TableNamespace}/Test/Table1.Child1", childTable.Namespace);

            Assert.AreEqual("/Test1/", derivedTable.CategoryPath);
            Assert.AreEqual($"{CremaSchema.TableNamespace}/Test1/Derived1", derivedTable.Namespace);
            Assert.AreEqual("/Test1/", derivedChildTable.CategoryPath);
            Assert.AreEqual($"{CremaSchema.TableNamespace}/Test1/Derived1.Child1", derivedChildTable.Namespace);
        }

        [TestMethod]
        public void Test5()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var childTable = dataTable.Childs.Add();


            var dataColumn = dataTable.Columns.Add();

            var dataRow1 = dataTable.AddRow("0");
            var dataRow2 = dataTable.AddRow("1");

            dataRow2.Index = 0;
        }

        [TestMethod]
        public void RelationTestOnRenameChild()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var table = dataSet.Tables.Random(item => item.Childs.Any() && item.TemplatedParent == null);
            var child = table.Childs.Random();

            child.TableName = RandomUtility.NextIdentifier();

            var xml = table.GetXml();

        }

        [TestMethod]
        public void ChangeRelationName()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var table = dataSet.Tables.Random(item => item.Childs.Any() && item.TemplatedParent == null && item.DerivedTables.Any());
            table.TableName = RandomUtility.NextIdentifier();
            var xml = table.GetXml();
        }

        [TestMethod]
        public void TemplateNamespaceTest1()
        {
            var dataSet = new CremaDataSet();

            var dataTable1 = dataSet.Tables.Add();
            var childTable1 = dataTable1.Childs.Add();

            var dataTable2 = dataTable1.Inherit();
            var childTable2 = dataTable2.Childs[0];
            dataTable1.TableName = "CustomTable";

            Assert.AreEqual(dataTable1.Namespace, dataTable2.TemplateNamespace);
            Assert.AreEqual(childTable1.Namespace, childTable2.TemplateNamespace);
        }

        [TestMethod]
        public void TemplateNamespaceTest2()
        {
            var dataSet = new CremaDataSet();

            var dataTable1 = dataSet.Tables.Add();
            var childTable1 = dataTable1.Childs.Add();

            var dataTable2 = dataTable1.Inherit();
            var childTable2 = dataTable2.Childs[0];
            dataTable1.CategoryPath = "/test/";

            Assert.AreEqual(dataTable1.Namespace, dataTable2.TemplateNamespace);
            Assert.AreEqual(childTable1.Namespace, childTable2.TemplateNamespace);
        }

        [TestMethod]
        public void TemplateNamespaceTest3()
        {
            var dataSet = new CremaDataSet();

            var dataTable1 = dataSet.Tables.Add();
            var childTable1 = dataTable1.Childs.Add();

            var dataTable2 = dataTable1.Inherit();
            var childTable2 = dataTable2.Childs[0];
            childTable1.TableName = "CustomTable";

            Assert.AreEqual(dataTable1.Namespace, dataTable2.TemplateNamespace);
            Assert.AreEqual(childTable1.Namespace, childTable2.TemplateNamespace);
        }

        [TestMethod]
        public void RelationChangedTest()
        {
            Assert.Inconclusive("관계값을 인덱스로 이 테스트는 잘못된 테스트임");
            var dataSet = new CremaDataSet();

            var dataTable1 = dataSet.Tables.Add();
            var childTable1 = dataTable1.Childs.Add();

            var dataTable2 = dataTable1.Inherit();
            var childTable2 = dataTable2.Childs[0];

            var column1 = dataTable1.Columns.Add();
            var column2 = dataTable1.Columns.Add();

            column1.IsKey = true;

            var dataRow1 = dataTable1.NewRow();
            dataRow1[column1] = "a";
            dataRow1[column2] = "b";
            dataTable1.Rows.Add(dataRow1);

            var dataRow2 = dataTable1.NewRow();
            dataRow2[column1] = "b";
            dataRow2[column2] = "a";
            dataTable1.Rows.Add(dataRow2);

            var relationID1 = dataRow1.RelationID;
            var relationID2 = dataRow2.RelationID;
            column1.IsKey = false;
            column2.IsKey = true;

            Assert.AreEqual(dataRow1.RelationID, relationID2);
            Assert.AreEqual(dataRow2.RelationID, relationID1);
        }

        [TestMethod]
        public void ReadOnlyRelationAcceptChanges()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var childTable = dataTable.Childs.Add();

            var dataRow1 = dataTable.NewRow();
            dataTable.Rows.Add(dataRow1);

            var childRow1 = childTable.NewRow(dataRow1);
            childTable.Rows.Add(childRow1);

            var dataRow2 = dataTable.NewRow();
            dataRow2.Index = 0;
            dataTable.Rows.Add(dataRow2);

            dataTable.AcceptChanges();
        }

        [TestMethod]
        public void RelationTest1()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var childTable = dataTable.Childs.Add();
            var c1 = dataTable.Columns.Add();
            childTable.Columns.Add();

            var dataRow1 = dataTable.AddRow("q");
            var dataRow2 = dataTable.AddRow("q");
            childTable.AddRow(dataRow1, "a");

            dataTable.AcceptChanges();

            dataSet.DefaultViewManager.DataSet.EnforceConstraints = false;
            var row = dataTable.DefaultView[0].Row;
            row.BeginEdit();
            row[c1.ColumnName] = "dd";
            row[CremaSchema.Index] = 1;
            row.EndEdit();
            dataSet.DefaultViewManager.DataSet.EnforceConstraints = true;
        }

        /// <summary>
        /// 데이터가 있는 테이블에 자식 테이블 추가후 타입 변경시 관계값때문에 에러 발생이 되면 안됨
        /// </summary>
        [TestMethod]
        public void RelationTest2()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();

            var c1 = dataTable.Columns.Add();
            var dataRow1 = dataTable.AddRow("1");
            var dataRow2 = dataTable.AddRow("2");

            dataTable.AcceptChanges();

            var childTable = dataTable.Childs.Add();
            childTable.Columns.Add();
            childTable.AddRow(dataRow1, "a");

            c1.DataType = typeof(int);
        }

        /// <summary>
        /// 관계값 관련해서 임포트가 안되는 부분 수정
        /// </summary>
        [TestMethod]
        public void ImportTest()
        {
            var dataSet1 = new CremaDataSet();
            var dataTable1 = dataSet1.Tables.Add();
            var c1 = dataTable1.Columns.Add();
            c1.DefaultValue = "1";
            var dataRow1 = dataTable1.AddRow("1");
            var dataRow2 = dataTable1.AddRow("2");

            var childTable1 = dataTable1.Childs.Add();
            childTable1.Columns.Add();
            childTable1.AddRow(dataRow1, "a");

            //childTable.Childs.Add("w123qwerweqr");

            dataTable1.AcceptChanges();


            dataTable1.DefaultView[0][CremaSchema.Index] = 1;
            //dataRow1.Index = 1;
            dataTable1.AcceptChanges();

            var dataSet2 = new CremaDataSet();
            var dataTable2 = dataSet2.Tables.Add();
            var c2 = dataTable2.Columns.Add();
            c2.DefaultValue = "1";
            var childTable2 = dataTable2.Childs.Add();

            dataTable2.ImportRow(dataRow2);
        }

        [TestMethod]
        public void ImportTest1()
        {
            var dataSet1 = new CremaDataSet();
            var dataTable1 = dataSet1.Tables.Add();
            var c1 = dataTable1.Columns.Add();
            c1.DefaultValue = "1";
            //c1.IsKey = true;
            var dataRow1 = dataTable1.AddRow();
            var dataRow2 = dataTable1.AddRow("2");

            var childTable1 = dataTable1.Childs.Add();
            childTable1.Columns.Add();
            var childRow1 = childTable1.AddRow(dataRow1, "a");

            //childTable.Childs.Add("w123qwerweqr");

            dataTable1.AcceptChanges();



            //dataTable1.DefaultView[0][CremaSchema.Index] = 1;
            //dataRow1.Index = 1;
            dataTable1.AcceptChanges();

            var dataSet2 = new CremaDataSet();
            var dataTable2 = dataSet2.Tables.Add();
            var c2 = dataTable2.Columns.Add();
            //c2.IsKey = true;

            var childTable2 = dataTable2.Childs.Add();
            dataTable2.ImportRow(dataRow1);

            childTable2.ImportRow(childRow1);
        }

        [TestMethod]
        public void RemoveChildTest()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var derivedTable = dataTable.Inherit();

            var childTable = dataTable.Childs.Add();
            var derivedChildTable = derivedTable.Childs[childTable.TableName];

            dataSet.Tables.Remove(childTable);

            Assert.IsNull(childTable.DataSet);
            Assert.IsNull(derivedChildTable.DataSet);
        }

        [TestMethod]
        public void CanRemoveChildTest()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var derivedTable = dataTable.Inherit();

            var childTable = dataTable.Childs.Add();
            var derivedChildTable = derivedTable.Childs[childTable.TableName];

            var canRemove = dataSet.Tables.CanRemove(childTable);

            Assert.IsTrue(canRemove);
        }

        [TestMethod]
        [ExpectedException(typeof(CremaDataException))]
        public void RemoveDerivedChildTest_Fail()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var derivedTable = dataTable.Inherit();

            var childTable = dataTable.Childs.Add();
            var derivedChildTable = derivedTable.Childs[childTable.TableName];

            dataSet.Tables.Remove(derivedChildTable);
        }

        [TestMethod]
        public void CanRemoveDerivedChildTest()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var derivedTable = dataTable.Inherit();

            var childTable = dataTable.Childs.Add();
            var derivedChildTable = derivedTable.Childs[childTable.TableName];

            var canRemove = dataSet.Tables.CanRemove(derivedChildTable);

            Assert.IsFalse(canRemove);
        }

        [TestMethod]
        public void RemoveChildWithoutParentTest()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var derivedTable = dataTable.Inherit();

            var childTable = dataTable.Childs.Add();
            var derivedChildTable = derivedTable.Childs[childTable.TableName];

            var tempPath = PathUtility.GetTempPath(true);

            try
            {
                dataSet.WriteToDirectory(tempPath);

                var dataSet2 = new CremaDataSet();
                dataSet2.ReadTable(Path.Combine(tempPath, CremaSchema.TableDirectory, childTable.Name + CremaSchema.XmlExtension));
                dataSet2.ReadTable(Path.Combine(tempPath, CremaSchema.TableDirectory, derivedChildTable.Name + CremaSchema.XmlExtension));

                var childTable2 = dataSet2.Tables[childTable.Name];
                var derivedChildTable2 = dataSet2.Tables[derivedChildTable.Name];

                dataSet2.Tables.Remove(childTable2);

                Assert.IsNull(childTable2.DataSet);
                Assert.IsNull(derivedChildTable2.DataSet);
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CremaDataException))]
        public void RemoveChildWithoutParentTest_Fail()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var derivedTable = dataTable.Inherit();

            var childTable = dataTable.Childs.Add();
            var derivedChildTable = derivedTable.Childs[childTable.TableName];

            var tempPath = PathUtility.GetTempPath(true);

            try
            {
                dataSet.WriteToDirectory(tempPath);

                var dataSet2 = new CremaDataSet();
                dataSet2.ReadTable(Path.Combine(tempPath, CremaSchema.TableDirectory, childTable.Name + CremaSchema.XmlExtension));
                dataSet2.ReadTable(Path.Combine(tempPath, CremaSchema.TableDirectory, derivedChildTable.Name + CremaSchema.XmlExtension));

                var childTable2 = dataSet2.Tables[childTable.Name];
                var derivedChildTable2 = dataSet2.Tables[derivedChildTable.Name];

                dataSet2.Tables.Remove(derivedChildTable2);
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }
    }
}