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
using Ntreev.Crema.Data.Random;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;
using Ntreev.Crema.Data.Xml;
using Ntreev.Library;
using Ntreev.Library.IO;
using System.Threading;
using Ntreev.Library.Random;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.ObjectModel;
using System.Xml.Serialization;
using Ntreev.Library.Serialization;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaDataTable_ComplexTest : IDisposable
    {
        public CremaDataTable_ComplexTest()
        {

        }

        [TestMethod]
        public void Create()
        {
            var dataTable = new CremaDataTable();

            Assert.AreEqual(string.Empty, dataTable.Name);
            Assert.AreEqual(string.Empty, dataTable.TableName);
            Assert.AreEqual(string.Empty, dataTable.Comment);
            Assert.AreEqual(string.Empty, dataTable.TemplateNamespace);
            Assert.AreEqual(PathUtility.Separator, dataTable.CategoryPath);
            Assert.AreEqual(TagInfo.All, dataTable.Tags);
            Assert.IsNull(dataTable.TemplatedParent);
            Assert.IsNull(dataTable.Parent);
            Assert.IsNotNull(dataTable.PrimaryKey);
            Assert.IsTrue(dataTable.PrimaryKey.Length == 0);
            Assert.AreEqual(CremaSchema.TableNamespace + dataTable.CategoryPath + dataTable.Name, dataTable.Namespace);
        }

        [TestMethod]
        public void CreateWithName()
        {
            var dataTable = new CremaDataTable(RandomUtility.NextIdentifier());

            Assert.AreNotEqual(string.Empty, dataTable.Name);
            Assert.AreNotEqual(string.Empty, dataTable.TableName);
            Assert.AreEqual(string.Empty, dataTable.TemplateNamespace);
            Assert.AreEqual(PathUtility.Separator, dataTable.CategoryPath);
            Assert.AreEqual(CremaSchema.TableNamespace + dataTable.CategoryPath + dataTable.Name, dataTable.Namespace);
        }

        [TestMethod]
        public void CreateWithNameAndCategoryPath()
        {
            var dataTable = new CremaDataTable(RandomUtility.NextIdentifier(), RandomUtility.NextCategoryPath(1));

            Assert.AreNotEqual(string.Empty, dataTable.Name);
            Assert.AreNotEqual(string.Empty, dataTable.TableName);
            Assert.AreEqual(string.Empty, dataTable.TemplateNamespace);
            Assert.AreNotEqual(PathUtility.Separator, dataTable.CategoryPath);
            Assert.AreEqual(CremaSchema.TableNamespace + dataTable.CategoryPath + dataTable.TableName, dataTable.Namespace);
        }

        [TestMethod]
        public void CreateChild()
        {
            var dataTable = new CremaDataTable("Table1");
            var childTable = dataTable.Childs.Add();

            Assert.AreEqual(dataTable.TableName + "." + childTable.TableName, childTable.Name);
            Assert.AreNotEqual(string.Empty, childTable.TableName);
            Assert.AreEqual(string.Empty, childTable.Comment);
            Assert.AreEqual(string.Empty, childTable.TemplateNamespace);
            Assert.AreEqual(dataTable.CategoryPath, childTable.CategoryPath);
            Assert.AreEqual(TagInfo.All, childTable.Tags);
            Assert.IsNull(childTable.TemplatedParent);
            Assert.IsNotNull(childTable.Parent);
            Assert.IsNotNull(childTable.PrimaryKey);
            Assert.IsTrue(childTable.PrimaryKey.Length == 0);
            Assert.AreEqual(CremaSchema.TableNamespace + childTable.CategoryPath + childTable.Name, childTable.Namespace);
        }

        [TestMethod]
        public void CreateFail()
        {
            this.InvokeFail(() => new CremaDataTable(null));
            this.InvokeFail(() => new CremaDataTable("."));
            this.InvokeFail(() => new CremaDataTable("/"));
            this.InvokeFail(() => new CremaDataTable(" "));
            this.InvokeFail(() => new CremaDataTable("# "));
            this.InvokeFail(() => new CremaDataTable("a", null));
            this.InvokeFail(() => new CremaDataTable("a", ""));
            this.InvokeFail(() => new CremaDataTable("a", "/e"));
            this.InvokeFail(() => new CremaDataTable("a", "/./"));
        }

        [TestMethod]
        public void ContentsInfoAfterAddNew()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            for (int i = 0; i < 10; i++)
            {
                var table = dataSet.Tables.Random();

                var row = table.AddRandomRow(false);
                if (row == null)
                    continue;

                Assert.AreNotEqual(row.CreationInfo, SignatureDate.Empty);
                Assert.AreNotEqual(row.ModificationInfo, SignatureDate.Empty);
                Assert.AreEqual(row.CreationInfo, row.ModificationInfo);
                Assert.AreEqual(row.ModificationInfo, table.ContentsInfo);
            }
        }

        [TestMethod]
        public void ContentsInfoAfterSetFields()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var row = dataSet.Tables.Random().Rows.RandomOrDefault();
            Thread.Sleep(1);

            if (row == null || row.SetRandomValue() == false)
                return;

            Assert.AreNotEqual(row.ModificationInfo, SignatureDate.Empty);
            Assert.AreNotEqual(row.CreationInfo, row.ModificationInfo);
            Assert.AreEqual(row.ModificationInfo, row.Table.ContentsInfo);
        }

        [TestMethod]
        public void ContentsInfoAfterDelete()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var dataRow = dataSet.Tables.Random().Rows.RandomOrDefault();
            if (dataRow == null)
                return;

            var table = dataRow.Table;
            var modificationInfo = table.ContentsInfo;
            Thread.Sleep(1000);
            dataRow.Delete();
            Assert.AreNotEqual(table.ContentsInfo, modificationInfo);
            table.AcceptChanges();
        }

        [TestMethod]
        public void WriteToFile_ReadFromFile()
        {
            var testSet = new CremaDataSet();
            var dataSet = CremaDataSetExtensions.CreateRandomSet();

            foreach (var item in dataSet.Tables)
            {
                var schemaPath = PathUtility.GetTempFileName();
                var xmlPath = PathUtility.GetTempFileName();

                try
                {
                    if (item.TemplateNamespace == string.Empty)
                        item.WriteXmlSchema(schemaPath);
                    else
                        item.TemplatedParent.WriteXmlSchema(schemaPath);
                    item.WriteXml(xmlPath);

                    try
                    {
                        if (item.TemplateNamespace == string.Empty)
                            testSet.ReadXmlSchema(schemaPath);
                        else
                            testSet.ReadXmlSchema(schemaPath, new ItemName(item.CategoryPath, item.Name));
                        if (item.Columns.Where(i => i.CremaType != null).Any() == true)
                            Assert.Fail("테이블이 사용자타입을 사용하는 경우 읽어들이는데 실패를 해야함.");
                    }
                    catch
                    {
                        if (item.TemplateNamespace == string.Empty)
                            testSet.ReadXmlSchema(schemaPath, new CremaTypeXmlResolver(dataSet, item.Namespace));
                        else
                            testSet.ReadXmlSchema(schemaPath, new ItemName(item.CategoryPath, item.Name), new CremaTypeXmlResolver(dataSet, item.Namespace));
                    }

                    if (item.TemplateNamespace == string.Empty)
                        testSet.ReadXml(xmlPath);
                    else
                        testSet.ReadXml(xmlPath, new ItemName(item.CategoryPath, item.Name));
                }
                finally
                {
                    FileUtility.Delete(schemaPath);
                    FileUtility.Delete(xmlPath);
                }
            }

            foreach (var item in dataSet.Tables)
            {
                var newTable = testSet.Tables[item.Name];
                CremaComparer.CompareTable(item, newTable);
            }
        }

        [TestMethod]
        public void WriteToStream_ReadFromStream()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var testSet = new CremaDataSet();
            foreach (var item in dataSet.Tables)
            {
                if (item.TemplateNamespace != string.Empty)
                    continue;

                var schemaStream = new MemoryStream();
                var xmlStream = new MemoryStream();

                item.WriteXmlSchema(schemaStream);
                item.WriteXml(xmlStream);

                //try
                //{
                //    schemaStream.Position = 0;
                //    testSet.ReadXmlSchema(schemaStream);
                //    if (item.Columns.Where(i => i.CremaType != null).Any() == true)
                //        Assert.Fail("테이블이 사용자타입을 사용하는 경우 읽어들이는데 실패를 해야함.");
                //}
                //catch
                //{
                //    if (item.Columns.Where(i => i.CremaType != null).Any() == false)
                //        Assert.Fail("스키마 읽어들이는데 실패함");

                schemaStream.Position = 0;
                testSet.ReadXmlSchema(schemaStream, new CremaTypeXmlResolver(dataSet, item.Namespace));
                //}

                xmlStream.Position = 0;
                testSet.ReadXml(xmlStream);
            }

            foreach (var item in dataSet.Tables)
            {
                if (item.TemplateNamespace != string.Empty)
                    continue;
                var newTable = testSet.Tables[item.Name];
                CremaComparer.CompareTable(item, newTable);
            }
        }

        [TestMethod]
        public void WriteToTextWriter_ReadFromTextReader()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var testSet = new CremaDataSet();
            foreach (var item in dataSet.Tables)
            {
                if (item.TemplateNamespace != string.Empty)
                    continue;

                StringWriter schemaWriter = new StringWriter();
                StringWriter xmlWriter = new StringWriter();

                item.WriteXmlSchema(schemaWriter);
                item.WriteXml(xmlWriter);

                try
                {
                    using (StringReader sr = new StringReader(schemaWriter.ToString()))
                    {
                        testSet.ReadXmlSchema(sr);
                    }
                    if (item.Columns.Where(i => i.CremaType != null).Any() == true)
                        Assert.Fail("테이블이 사용자타입을 사용하는 경우 읽어들이는데 실패를 해야함.");
                }
                catch
                {
                    using (StringReader sr = new StringReader(schemaWriter.ToString()))
                    {
                        testSet.ReadXmlSchema(sr, new CremaTypeXmlResolver(dataSet, item.Namespace));
                    }
                }

                using (StringReader sr = new StringReader(xmlWriter.ToString()))
                {
                    testSet.ReadXml(sr);
                }
            }

            foreach (var item in dataSet.Tables)
            {
                if (item.TemplateNamespace != string.Empty)
                    continue;
                var newTable = testSet.Tables[item.Name];
                CremaComparer.CompareTable(item, newTable);
            }
        }

        [TestMethod]
        public void WriteToXmlWriter_ReadFromXmlReader()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var testSet = new CremaDataSet();
            foreach (var item in dataSet.Tables)
            {
                if (item.TemplateNamespace != string.Empty)
                    continue;

                StringBuilder schemaBuilder = new StringBuilder();
                StringBuilder xmlBuilder = new StringBuilder();
                using (XmlWriter schemaWriter = XmlWriter.Create(schemaBuilder))
                {
                    item.WriteXmlSchema(schemaWriter);
                }
                using (XmlWriter xmlWriter = XmlWriter.Create(xmlBuilder))
                {
                    item.WriteXml(xmlWriter);
                }

                try
                {
                    using (StringReader sr = new StringReader(schemaBuilder.ToString()))
                    using (XmlReader reader = XmlReader.Create(sr))
                    {
                        testSet.ReadXmlSchema(reader);
                    }
                    if (item.Columns.Where(i => i.CremaType != null).Any() == true)
                        Assert.Fail("테이블이 사용자타입을 사용하는 경우 읽어들이는데 실패를 해야함.");
                }
                catch
                {
                    using (StringReader sr = new StringReader(schemaBuilder.ToString()))
                    using (XmlReader reader = XmlReader.Create(sr))
                    {
                        testSet.ReadXmlSchema(reader, new CremaTypeXmlResolver(dataSet, item.Namespace));
                    }
                }

                using (StringReader sr = new StringReader(xmlBuilder.ToString()))
                using (XmlReader reader = XmlReader.Create(sr))
                {
                    testSet.ReadXml(reader);
                }
            }

            foreach (var item in dataSet.Tables)
            {
                if (item.TemplateNamespace != string.Empty)
                    continue;
                var newTable = testSet.Tables[item.Name];
                CremaComparer.CompareTable(item, newTable);
            }
        }

        [TestMethod]
        public void ChangeCategory()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add("Table", "/A/b/");
            var childTable1 = dataTable.Childs.Add("child1");
            var childTable2 = dataTable.Childs.Add("child2");

            Assert.AreEqual(dataTable.CategoryPath, childTable1.CategoryPath);
            Assert.AreEqual(dataTable.CategoryPath, childTable2.CategoryPath);

            dataTable.CategoryPath = "/C/E/";
            Assert.AreEqual(dataTable.CategoryPath, childTable1.CategoryPath);
            Assert.AreEqual(dataTable.CategoryPath, childTable2.CategoryPath);
        }

        [TestMethod]
        public void AddWithChilds()
        {
            var dataSet = new CremaDataSet();
            var dataTable = new CremaDataTable("table1");
            var childTable1 = dataTable.Childs.Add("child1");
            var childTable2 = dataTable.Childs.Add("child2");

            dataSet.Tables.Add(dataTable);

            Assert.AreEqual(dataSet, dataTable.DataSet);
            Assert.AreEqual(dataSet, childTable1.DataSet);
            Assert.AreEqual(dataSet, childTable2.DataSet);
        }

        [TestMethod]
        public void AddWithChildsFail()
        {
            var dataSet = new CremaDataSet();
            var dataTable = new CremaDataTable("table1");
            var childTable1 = dataTable.Childs.Add("child1");
            var childTable2 = dataTable.Childs.Add("child2");

            dataSet.Tables.Add(dataTable);
            InvokeFail(() => dataSet.Tables.Add(dataTable));
        }

        [TestMethod]
        public void RenameChildFail1()
        {
            var dataTable = new CremaDataTable("table1");
            var childTable = dataTable.Childs.Add("child1");

            InvokeFail(() => childTable.TableName = "table1");
        }

        [TestMethod]
        public void RenameChildFail2()
        {
            var dataTable = new CremaDataTable("table1");
            var childTable1 = dataTable.Childs.Add("child1");
            var childTable2 = dataTable.Childs.Add("child2");

            InvokeFail(() => childTable1.TableName = "child2");
        }

        [TestMethod]
        public void AddChildFail1()
        {
            var dataSet = new CremaDataSet();
            var dataTable = new CremaDataTable("table1");
            var childTable1 = dataTable.Childs.Add("child1");
            var childTable2 = dataTable.Childs.Add("child2");

            InvokeFail(() => dataSet.Tables.Add(childTable1));
        }

        [TestMethod]
        public void DerivedTableCopyTo()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add("Table1");
            var childTable = dataTable.Childs.Add();
            var derivedTable = dataTable.Inherit("derived");
            var targetSet = new CremaDataSet();
            var targetTable = derivedTable.CopyTo(targetSet);
            CremaComparer.CompareTable(derivedTable, targetTable);

            var schema = targetSet.GetXmlSchema();
        }

        [TestMethod]
        public void DerivedTableCopyTo1()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add("Table1");
            var childTable = dataTable.Childs.Add();
            var derivedTable = dataTable.Inherit("derived");

            var tempPath = PathUtility.GetTempPath(true);
            try
            {
                dataSet.WriteToDirectory(tempPath);

                dataSet = new CremaDataSet();
                dataSet.ReadTable(Path.Combine(tempPath, CremaSchema.TableDirectory, derivedTable.Name + ".xml"));
                derivedTable = dataSet.Tables["derived"];

                var targetSet = new CremaDataSet();
                var targetTable = derivedTable.CopyTo(targetSet);
                CremaComparer.CompareTable(derivedTable, targetTable);

                var schema = targetSet.GetXmlSchema();
            }
            finally
            {
                DirectoryUtility.Delete(tempPath);
            }
        }

        [TestMethod]
        public void SignatureDateTest()
        {
            var creator = "Tester";
            var dataSet = new CremaDataSet() { SignatureDateProvider = new SignatureDateProvider(creator) };
            var dataTable = dataSet.Tables.Add();

            Assert.AreEqual(creator, dataTable.CreationInfo.ID);
            Assert.AreEqual(creator, dataTable.ModificationInfo.ID);
            Assert.AreEqual(dataTable.CreationInfo, dataTable.ModificationInfo);
        }

        [TestMethod]
        public void ComplexTest1()
        {
            Assert.Inconclusive("관계값을 인덱스로 바꾸면서 오류가 발생하는데 이테스트의 의도가 먼지 기억이 안남");
            var dataSet = new CremaDataSet();

            var table1 = dataSet.Tables.Add();
            var column1 = table1.Columns.AddKey();
            var child1 = table1.Childs.Add();
            child1.Columns.AddKey();
            var row1 = table1.NewRow();
            row1[0] = "1";
            table1.Rows.Add(row1);
            var relationID = row1.RelationID;

            var childCount = 4;
            for (var i = 0; i < childCount; i++)
            {
                var childRow1 = child1.NewRow(row1);
                childRow1[0] = i.ToString();
                child1.Rows.Add(childRow1);
            }

            dataSet.AcceptChanges();
            dataSet.BeginLoad();
            table1.DefaultView[0].BeginEdit();
            table1.DefaultView[0][7] = "2";
            table1.DefaultView[0][8] = "randomid";
            table1.DefaultView[0].EndEdit();
            dataSet.EndLoad();

            var sss = row1.GetChildRows(child1);
            Assert.AreEqual(childCount, sss.Length);
        }

        [TestMethod]
        public void Test1()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            dataTable.Columns.AddKey();
            dataTable.Columns.Add();

            var childTable = dataTable.Childs.Add();
            childTable.Columns.AddKey();
            childTable.Columns.Add();

            //var schema = childTable.GetXmlSchema();
            //var xml = childTable.GetXml();

            //File.WriteAllText($@"C:\Users\s2quake\Desktop\{childTable.Name}.xsd", schema);
            //File.WriteAllText($@"C:\Users\s2quake\Desktop\{childTable.Name}.xml", xml);
            //int qwre = 0;

            var destSet = new CremaDataSet();

            childTable.CloneTo(destSet);
            Assert.AreEqual(destSet.Tables[0].Name, childTable.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddChild_Fail1()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var dataColumn = dataTable.Columns.Add();
            var childTable = dataTable.Childs.Add(dataColumn.ColumnName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddChild_Fail2()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var attribute = dataTable.Attributes.Random();
            var childTable = dataTable.Childs.Add(attribute.AttributeName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddChild_Fail3()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var childTable = dataTable.Childs.Add();
            dataTable.Childs.Add(CremaSchema.__RelationID__);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddChild_Fail4()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var childTable = dataTable.Childs.Add();
            dataTable.Childs.Add(childTable.TableName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddChild_Fail5()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var childTable = dataTable.Childs.Add();
            dataTable.Childs.Add(dataTable.TableName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddChild_Fail6()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var derivedTable = dataTable.Inherit();
            dataTable.Childs.Add(derivedTable.TableName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RenameChild_Fail1()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var dataColumn = dataTable.Columns.Add();
            var childTable = dataTable.Childs.Add();
            childTable.TableName = dataColumn.ColumnName;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RenameChild_Fail2()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var attribute = dataTable.Attributes.Random();
            var childTable = dataTable.Childs.Add();
            childTable.TableName = attribute.AttributeName;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RenameChild_Fail3()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var childTable = dataTable.Childs.Add();
            childTable.TableName = CremaSchema.__RelationID__;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RenameChild_Fail4()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var childTable = dataTable.Childs.Add();
            var childTable1 = dataTable.Childs.Add();
            childTable1.TableName = childTable.TableName;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RenameChild_Fail5()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var childTable = dataTable.Childs.Add();
            childTable.TableName = dataTable.TableName;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RenameChild_Fail6()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var derivedTable = dataTable.Inherit();
            var childTable = dataTable.Childs.Add();
            childTable.TableName = derivedTable.TableName;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Rename_Fail1()
        {
            var dataSet = new CremaDataSet();
            var dataTable = dataSet.Tables.Add();
            var derivedTable = dataTable.Inherit();
            var childTable = dataTable.Childs.Add();
            derivedTable.TableName = childTable.TableName;
        }

        [TestMethod]
        public void Change_CategoryTest()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var dataTable1 = dataSet.Tables.Random(item => item.TemplatedParent == null && item.Parent == null);
            var modification = dataTable1.ModificationInfo;
            Thread.Sleep(1000);
            dataTable1.CategoryPath = RandomUtility.NextCategoryPath();

            Assert.AreEqual(modification, dataTable1.ModificationInfo);
        }

        public void Dispose()
        {

        }

        private void InvokeFail(Action action)
        {
            try
            {
                action();
                Assert.Inconclusive();
            }
            catch
            {

            }
        }
    }
}
