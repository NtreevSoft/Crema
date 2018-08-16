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
using System.Threading;
using Ntreev.Library;

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

        // 키 해제시 관계값을 갱신하는데 assert 형태로 코딩되어 있는 부분이 호출되지 않도록 처리함.
        // if (this.RowState != DataRowState.Detached)
        //     throw new InvalidOperationException();
        [TestMethod]
        public void RemoveKeyTest1()
        {
            var dataSet = new CremaDataSet();

            var dataTable = dataSet.Tables.Add();
            var childTable = dataTable.Childs.Add();

            var c1 = dataTable.Columns.Add();
            var c2 = dataTable.Columns.Add();
            var c3 = dataTable.Columns.Add();
            var c4 = dataTable.Columns.Add();

            c1.IsKey = true;
            c2.IsKey = true;
            c3.IsKey = true;

            childTable.Columns.Add();
            childTable.Columns.Add();

            var r1 = dataTable.AddRow("1", "1", "1");
            var r2 = dataTable.AddRow("2", "1", "2");
            var r3 = dataTable.AddRow("3", "1", "3");

            c3.IsKey = false;
        }

        [TestMethod]
        public void CreateChildOfChild()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var childTable = dataTable.Childs.Add();
            var ct = childTable.Childs.Add();
        }

        [TestMethod]
        public void InheritChildOfChild()
        {
            var dataSet = new CremaDataSet();
            var dataTable1 = dataSet.Tables.Add();
            var dataTable2 = dataTable1.Childs.Add();
            var dataTable3 = dataTable2.Childs.Add();

            var derivedTable1 = dataTable1.Inherit();
            var derivedTable2 = derivedTable1.Childs.First();
            var derivedTable3 = derivedTable2.Childs.First();

            Thread.Sleep(1000);

            Assert.AreNotEqual(dataTable1.TableName, derivedTable1.TableName);
            Assert.AreEqual(dataTable2.TableName, derivedTable2.TableName);
            Assert.AreEqual(dataTable3.TableName, derivedTable3.TableName);

            Assert.AreEqual(dataTable1.Name, derivedTable1.TemplatedParentName);
            Assert.AreEqual(dataTable2.Name, derivedTable2.TemplatedParentName);
            Assert.AreEqual(dataTable3.Name, derivedTable3.TemplatedParentName);

            Assert.AreNotEqual(dataTable1.TableID, derivedTable1.TableID);
            Assert.AreNotEqual(dataTable2.TableID, derivedTable2.TableID);
            Assert.AreNotEqual(dataTable3.TableID, derivedTable3.TableID);

            Assert.AreNotEqual(dataTable1.CreationInfo, derivedTable1.CreationInfo);
            Assert.AreEqual(dataTable2.CreationInfo, derivedTable2.CreationInfo);
            Assert.AreEqual(dataTable3.CreationInfo, derivedTable3.CreationInfo);
        }

        [TestMethod]
        public void InheritChildOfChild2()
        {
            var dataSet = new CremaDataSet();
            var dataTable1 = dataSet.Tables.Add();
            var dataTable2 = dataTable1.Childs.Add();
            var dataTable3 = dataTable2.Childs.Add();

            Thread.Sleep(1000);

            var derivedTable1 = dataTable1.Inherit();
            var derivedTable2 = derivedTable1.Childs.First();
            var derivedTable3 = derivedTable2.Childs.First();

            var identifier2 = RandomUtility.NextIdentifier();
            var identifier3 = RandomUtility.NextIdentifier();

            if (identifier2 == identifier3)
                return;

            dataTable2.TableName = identifier2;
            dataTable3.TableName = identifier3;

            Assert.AreNotEqual(dataTable1.TableName, derivedTable1.TableName);
            Assert.AreEqual(dataTable2.TableName, derivedTable2.TableName);
            Assert.AreEqual(dataTable3.TableName, derivedTable3.TableName);

            Assert.AreEqual(dataTable1.Name, derivedTable1.TemplatedParentName);
            Assert.AreEqual(dataTable2.Name, derivedTable2.TemplatedParentName);
            Assert.AreEqual(dataTable3.Name, derivedTable3.TemplatedParentName);
        }

        [TestMethod]
        public void CopyChildOfChild()
        {
            var dataSet = new CremaDataSet();
            var dataTable1 = dataSet.Tables.Add();
            var dataTable2 = dataTable1.Childs.Add();
            var dataTable3 = dataTable2.Childs.Add();

            Thread.Sleep(1000);

            var copiedTable1 = dataTable1.Copy();
            var copiedTable2 = copiedTable1.Childs.First();
            var copiedTable3 = copiedTable2.Childs.First();

            Assert.AreNotEqual(dataTable1.TableName, copiedTable1.TableName);
            Assert.AreEqual(dataTable2.TableName, copiedTable2.TableName);
            Assert.AreEqual(dataTable3.TableName, copiedTable3.TableName);

            Assert.AreEqual(copiedTable1.TemplatedParentName, string.Empty);
            Assert.AreEqual(copiedTable2.TemplatedParentName, string.Empty);
            Assert.AreEqual(copiedTable3.TemplatedParentName, string.Empty);

            Assert.AreNotEqual(dataTable1.TableID, copiedTable1.TableID);
            Assert.AreNotEqual(dataTable2.TableID, copiedTable2.TableID);
            Assert.AreNotEqual(dataTable3.TableID, copiedTable3.TableID);

            Assert.AreNotEqual(dataTable1.CreationInfo, copiedTable1.CreationInfo);
            Assert.AreNotEqual(dataTable2.CreationInfo, copiedTable2.CreationInfo);
            Assert.AreNotEqual(dataTable3.CreationInfo, copiedTable3.CreationInfo);
        }

        [TestMethod]
        public void CopyChildTable()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var childTable1 = dataTable.Childs.Add();
            var derivedTable = dataTable.Inherit();
            derivedTable.CategoryPath = RandomUtility.NextCategoryPath();
            var childTable2 = childTable1.Copy();

            var derivedChild1 = derivedTable.Childs[childTable1.TableName];
            var derivedChild2 = derivedTable.Childs[childTable2.TableName];

            Assert.AreEqual(derivedTable.CategoryPath, derivedChild1.CategoryPath);
            Assert.AreEqual(derivedTable.CategoryPath, derivedChild2.CategoryPath);

            Assert.AreEqual(dataTable.Childs.Count, derivedTable.Childs.Count);

            Assert.AreEqual(childTable1.TableName, derivedChild1.TableName);
            Assert.AreEqual(childTable2.TableName, derivedChild2.TableName);

            Assert.AreEqual(childTable1.Name, derivedChild1.TemplatedParentName);
            Assert.AreEqual(childTable2.Name, derivedChild2.TemplatedParentName);
        }

        [TestMethod]
        public void SignatureDateTest()
        {
            var dataSet = new CremaDataSet();
            var identifier = RandomUtility.NextIdentifier();
            dataSet.SignatureDateProvider = new SignatureDateProvider(identifier);
            var table1 = dataSet.Tables.Add();
            var derived1 = table1.Inherit();
            var child1 = table1.Childs.Add();
            var child2 = derived1.Childs[child1.TableName];
            var cc1 = child1.Childs.Add();
            var cc2 = child1.Childs[cc1.TableName];

            Assert.AreEqual(table1.CreationInfo.ID, identifier);
            Assert.AreEqual(derived1.CreationInfo.ID, identifier);
            Assert.AreEqual(child1.CreationInfo.ID, identifier);
            Assert.AreEqual(child2.CreationInfo.ID, identifier);
            Assert.AreEqual(cc1.CreationInfo.ID, identifier);
            Assert.AreEqual(cc2.CreationInfo.ID, identifier);

            Assert.AreNotEqual(table1.CreationInfo.DateTime, DateTime.MinValue);
            Assert.AreNotEqual(derived1.CreationInfo.DateTime, DateTime.MinValue);
            Assert.AreNotEqual(child1.CreationInfo.DateTime, DateTime.MinValue);
            Assert.AreNotEqual(child2.CreationInfo.DateTime, DateTime.MinValue);
            Assert.AreNotEqual(cc1.CreationInfo.DateTime, DateTime.MinValue);
            Assert.AreNotEqual(cc2.CreationInfo.DateTime, DateTime.MinValue);
        }
    }
}