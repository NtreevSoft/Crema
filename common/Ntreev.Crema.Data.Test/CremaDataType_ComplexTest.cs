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
using Ntreev.Library.IO;
using System.Xml.Serialization;
using System.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Data.Xml;
using Ntreev.Library.ObjectModel;
using System.Threading;
using Ntreev.Library.Serialization;
using Ntreev.Library.Random;
using Ntreev.Library;
using Ntreev.Crema.Data.Random;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaDataType_ComplexTest
    {
        [TestMethod]
        public void RollbackTest()
        {
            var dataSet = new CremaDataSet();
            var type = dataSet.Types.Add();
            var member = type.NewMember();
            member.Name = "A";
            member.Value = 0;
            type.Members.Add(member);
            type.AcceptChanges();

            var table = dataSet.Tables.Add();

            var c1 = table.Columns.Add();
            c1.IsKey = true;

            var c2 = table.Columns.Add();
            c2.CremaType = type;

            var r1 = table.NewRow();
            r1[0] = "1";
            r1[1] = "A";
            table.Rows.Add(r1);

            type.RejectChanges();

        }

        [TestMethod]
        public void Create()
        {
            this.NewTest(null, "/");
            this.NewTest(string.Empty, "/");

            this.NewTest(RandomUtility.NextIdentifier(), "/");
            this.NewTest(null, "/" + RandomUtility.NextIdentifier() + "/");
            this.NewTest(string.Empty, "/" + RandomUtility.NextIdentifier() + "/");
            this.NewTest(RandomUtility.NextIdentifier(), "/" + RandomUtility.NextIdentifier() + "/");
        }

        [TestMethod]
        public void New_Fail()
        {
            try
            {
                this.NewTest("%", null);
                Assert.Fail();
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch
            {

            }
        }

        [TestMethod]
        public void WriteToString_ReadFromString()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var testSet = new CremaDataSet();

            foreach (var item in dataSet.Types)
            {
                string schema = item.GetXmlSchema();
                testSet.ReadTypeString(schema);
                var newType = testSet.Types[item.TypeName];

                CremaComparer.CompareType(item, newType);
            }
        }

        [TestMethod]
        public void WriteToFile_ReadFromFile()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var testSet = new CremaDataSet();

            foreach (var item in dataSet.Types)
            {
                string schemaPath = PathUtility.GetTempFileName();
                item.Write(schemaPath);

                try
                {
                    testSet.ReadType(schemaPath);
                }
                finally
                {
                    FileUtility.Delete(schemaPath);
                }

                var newType = testSet.Types[item.TypeName];
                CremaComparer.CompareType(item, newType);
            }
        }

        [TestMethod]
        public void WriteToStream_ReadFromStream()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var testSet = new CremaDataSet();

            foreach (var item in dataSet.Types)
            {
                MemoryStream ms = new MemoryStream();
                item.Write(ms);
                ms.Position = 0;

                testSet.ReadType(ms);
                var newType = testSet.Types[item.TypeName];

                CremaComparer.CompareType(item, newType);
            }
        }

        [TestMethod]
        public void WriteToTextWriter_ReaderFromTextReader()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var testSet = new CremaDataSet();

            foreach (var item in dataSet.Types)
            {
                StringWriter sw = new StringWriter();
                item.Write(sw);

                StringReader sr = new StringReader(sw.ToString());
                testSet.ReadType(sr);
                var newType = testSet.Types[item.TypeName];

                CremaComparer.CompareType(item, newType);
            }
        }

        [TestMethod]
        public void WriteToXmlWriter_ReaderFromXmlReader()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var testSet = new CremaDataSet();

            foreach (var item in dataSet.Types)
            {
                StringBuilder sb = new StringBuilder();
                XmlWriter writer = XmlWriter.Create(sb);
                item.Write(writer);
                writer.Close();

                StringReader sr = new StringReader(sb.ToString());
                XmlReader reader = XmlReader.Create(sr);
                testSet.ReadType(reader);
                var newType = testSet.Types[item.TypeName];

                CremaComparer.CompareType(item, newType);
            }
        }

        [TestMethod]
        public void XmlSerializing()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            foreach (var item in dataSet.Types)
            {
                string xml = XmlSerializerUtility.GetString(item);
                var newType = XmlSerializerUtility.ReadString<CremaDataType>(xml);

                CremaComparer.CompareType(item, newType);
            }
        }

        //[TestMethod]
        //public void MergeToEmpty()
        //{
        //    var dataSet = CremaDataSetExtensions.CreateRandomSet();
        //    var testSet = new CremaDataSet();

        //    foreach (var item in dataSet.Types)
        //    {
        //        var newType = testSet.Types.Add(item.Name);
        //        newType.IsFlag = item.IsFlag;
        //        newType.Merge(item);

        //        Assert.AreEqual(item.Members.Count, newType.Members.Count, "Members");

        //        for (int i = 0; i < item.Members.Count; i++)
        //        {
        //            var member1 = item.Members[i];
        //            var member2 = newType.Members[i];

        //            Assert.AreEqual(member1.Index, member2.Index, "Index");
        //            Assert.AreEqual(member1.Name, member2.Name, "Name");
        //            Assert.AreEqual(member1.Value, member2.Value, "Value");
        //            Assert.AreEqual(member1.Comment, member2.Comment, "Comment");
        //        }
        //    }
        //}

        //[TestMethod]
        //public void MergeDelete()
        //{
        //    var type1 = new CremaDataType("Type1");
        //    type1.FillRandom(5, 10);
        //    type1.AcceptChanges();

        //    StringReader sr = new StringReader(type1.GetXmlSchema());
        //    CremaDataType type2 = CremaDataType.ReadSchema(sr);
        //    sr.Close();

        //    CremaComparer.CompareType(type1, type2);
        //    type1.Members[3].Delete();
        //    type2.Merge(type1);

        //    type1.AcceptChanges();
        //    type2.AcceptChanges();
        //    CremaComparer.CompareType(type1, type2);
        //}

        [TestMethod]
        public void AddToDataSet_Fail()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var testSet = new CremaDataSet();

            foreach (var item in dataSet.Types)
            {
                try
                {
                    dataSet.Types.Add(item);
                    Assert.Fail("실패하지 않았습니다.");
                }
                catch
                {

                }
            }
        }

        [TestMethod]
        public void AddToAnotherDataSet_Fail()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            var testSet = new CremaDataSet();

            foreach (var item in dataSet.Types)
            {
                try
                {
                    testSet.Types.Add(item);
                    Assert.Fail("실패하지 않았습니다.");
                }
                catch
                {

                }
            }
        }

        [TestMethod]
        public void AddNullToDataSet_Fail()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();

            try
            {
                CremaDataType type = null;
                dataSet.Types.Add(type);
                Assert.Fail("실패하지 않았습니다.");
            }
            catch (ArgumentNullException)
            {

            }
            catch
            {
                Assert.Fail("잘못된 예외");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveUsingType_Fail()
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
            dataColumn.AllowDBNull = false;
            dataColumn.Unique = true;

            foreach (var item in dataType.Members)
            {
                var dataRow = dataTable.NewRow();
                dataRow[dataColumn] = item.Name;
                dataTable.Rows.Add(dataRow);
            }

            dataSet.Types.Remove(dataType);
        }

        [TestMethod]
        public void RemoveUsingType()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();

            var query = from table in dataSet.Tables
                        from column in table.Columns
                        where column.CremaType != null
                        select column;

            var types = query.Select(item => item.CremaType).Distinct().ToArray();
            var columns = query.ToArray();

            foreach (var item in columns)
            {
                item.CremaType = null;
            }

            foreach (var item in types)
            {
                dataSet.Types.Remove(item);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetIndex_Fail1()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);
            dataType.AddMember("B", 2);
            dataType.AddMember("C", 3);

            var member = dataType.Members.Random();
            member.Index = int.MaxValue;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetIndex_Fail2()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);
            dataType.AddMember("B", 2);
            dataType.AddMember("C", 3);

            var member = dataType.Members.Random();
            member.Index = -1;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConstraintException))]
        public void DeleteTypeMemberUsingUniqueNotAllowDBNull_Fail()
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
            dataColumn.AllowDBNull = false;
            dataColumn.Unique = true;

            foreach (var item in dataType.Members)
            {
                var dataRow = dataTable.NewRow();
                dataRow[dataColumn] = item.Name;
                dataTable.Rows.Add(dataRow);
            }

            var typeMember = dataType.Members.Random();
            typeMember.Delete();
        }

        [TestMethod]
        public void DeleteTypeMemberUsingUnique()
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
            dataColumn.Unique = true;

            foreach (var item in dataType.Members)
            {
                var dataRow = dataTable.NewRow();
                dataRow[dataColumn] = item.Name;
                dataTable.Rows.Add(dataRow);
            }
            dataTable.AcceptChanges();

            var typeMember = dataType.Members.Random();
            var index = dataType.Members.IndexOf(typeMember);
            typeMember.Delete();
            Assert.AreEqual(DBNull.Value, dataTable.Rows[index][dataColumn]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConstraintException))]
        public void DeleteFlagMemberUsingUniqueNotAllowDBNull_Fail()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.IsFlag = true;
            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);
            dataType.AddMember("B", 2);
            dataType.AddMember("C", 4);
            dataType.AcceptChanges();

            var dataTable = dataSet.Tables.Add();
            var dataColumn = dataTable.Columns.Add();
            dataColumn.CremaType = dataType;
            dataColumn.AllowDBNull = false;
            dataColumn.Unique = true;

            foreach (var item in dataType.Members)
            {
                var dataRow = dataTable.NewRow();
                dataRow[dataColumn] = item.Name;
                dataTable.Rows.Add(dataRow);
            }

            var typeMember = dataType.Members.Random();
            typeMember.Delete();
        }

        [TestMethod]
        public void DeleteFlagMemberUsingUnique()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.IsFlag = true;
            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);
            dataType.AddMember("B", 2);
            dataType.AddMember("C", 4);
            dataType.AcceptChanges();

            var dataTable = dataSet.Tables.Add();
            var dataColumn = dataTable.Columns.Add();
            dataColumn.CremaType = dataType;
            dataColumn.Unique = true;

            foreach (var item in dataType.Members)
            {
                var dataRow = dataTable.NewRow();
                dataRow[dataColumn] = item.Name;
                dataTable.Rows.Add(dataRow);
            }
            dataTable.AcceptChanges();

            var typeMember = dataType.Members.Random();
            var index = dataType.Members.IndexOf(typeMember);
            typeMember.Delete();
            Assert.AreEqual(DBNull.Value, dataTable.Rows[index][dataColumn]);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidConstraintException))]
        public void ChangeValueUsingUnique_Fail()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.IsFlag = true;
            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);
            dataType.AddMember("B", 2);
            dataType.AddMember("C", 4);
            dataType.AcceptChanges();

            var dataTable = dataSet.Tables.Add();
            var dataColumn = dataTable.Columns.Add();
            dataColumn.CremaType = dataType;
            dataColumn.Unique = true;

            foreach (var item in dataType.Members)
            {
                var dataRow = dataTable.NewRow();
                dataRow[dataColumn] = item.Name;
                dataTable.Rows.Add(dataRow);
            }
            dataTable.AcceptChanges();

            var typeMember1 = dataType.Members.Random();
            var typeMember2 = dataType.Members.Random(item => item != typeMember1);
            typeMember1.Value = typeMember2.Value;
        }

        [TestMethod]
        public void ModificationInfoAfterAddNew()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();

            var type = dataSet.Types.Random();
            var member = type.AddRandomMember();
            if (member == null)
                return;

            Assert.AreNotEqual(member.CreationInfo, SignatureDate.Empty);
            Assert.AreNotEqual(member.ModificationInfo, SignatureDate.Empty);
            Assert.AreEqual(member.ModificationInfo, type.ModificationInfo);
        }

        [TestMethod]
        public void ModificationInfoAfterSetFields()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();

            var type = dataSet.Types.Random();
            var member = type.AddRandomMember();
            if (member == null)
                return;

            Assert.AreEqual(member.CreationInfo, member.ModificationInfo);
            Assert.AreEqual(member.ModificationInfo, type.ModificationInfo);
        }

        [TestMethod]
        public void ModificationInfoAfterDelete()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);
            dataType.AcceptChanges();

            var dataTable = dataSet.Tables.Add();
            dataTable.Columns.Add("column1", dataType);

            var dataRow = dataTable.NewRow();
            dataRow[0] = "None";
            dataTable.Rows.Add(dataRow);

            dataSet.AcceptChanges();

            var member = dataType.Members[0];
            var modification = dataType.ModificationInfo;
            Thread.Sleep(1000);
            member.Delete();

            Assert.AreNotEqual(modification, dataType.ModificationInfo);
        }

        private void NewTest(string typeName, string categoryPath)
        {
            var typeName1 = typeName ?? string.Empty;
            var categoryName1 = categoryPath ?? string.Empty;

            var type = new CremaDataType(typeName, categoryPath);

            Assert.AreEqual(type.TypeName, typeName1);
            Assert.AreEqual(type.CategoryPath, categoryPath);
            Assert.AreEqual(type.Namespace, CremaSchema.TypeNamespace + categoryPath + typeName);
            Assert.IsNull(type.DataSet);
            Assert.AreEqual(type.Comment, string.Empty);
            Assert.IsFalse(type.IsFlag);
            Assert.AreEqual(TagInfo.All, type.Tags);
        }

        [TestMethod]
        public void SignatureDateTest()
        {
            var creator = RandomUtility.NextWord();
            var dataSet = new CremaDataSet() { SignatureDateProvider = new SignatureDateProvider(creator) };
            var dataType = dataSet.Types.Add();

            Assert.AreEqual(creator, dataType.CreationInfo.ID);
            Assert.AreEqual(creator, dataType.ModificationInfo.ID);
            Assert.AreEqual(dataType.CreationInfo, dataType.ModificationInfo);


        }

        [TestMethod]
        public void Test1()
        {
            CremaDataType type = new CremaDataType();
            var member = type.NewMember();
            member.Name = "A";
            member.Value = 0;

            type.Members.Add(member);
            type.AcceptChanges();

            member.BeginEdit();
            member.Name = "B";
            //type.AsDataView()[0].Row.SetReadOnlyField("Modifier", "werw");
            member.EndEdit();
        }

        [TestMethod]
        public void Test2()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);
            dataType.AcceptChanges();

            var dataTable = dataSet.Tables.Add();
            dataTable.Columns.AddKey("column1", dataType);

            var dataRow = dataTable.NewRow();
            dataRow[0] = "None";
            dataTable.Rows.Add(dataRow);

            dataSet.AcceptChanges();

            dataType.View[0].Row.BeginEdit();
            dataType.View[0].Row[CremaSchema.Name] = "none";
            dataType.View[0].Row[CremaSchema.Value] = 1;
            dataType.View[0].Row.EndEdit();

            Assert.AreEqual(dataRow[0], dataType.View[0].Row[CremaSchema.Name]);
            Assert.AreEqual(dataRow[0], dataType.Members[0].Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test2_Fail()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            dataType.AddMember("None", 0);
            dataType.AddMember("A", 1);

            var dataTable = dataSet.Tables.Add();
            dataTable.Columns.AddKey("column1", dataType);

            var dataRow = dataTable.NewRow();
            dataRow[0] = "None";
            dataTable.Rows.Add(dataRow);

            dataSet.AcceptChanges();

            dataType.View[0].Row.BeginEdit();
            dataType.View[0].Row[CremaSchema.Name] = "none";
            dataType.View[0].Row[CremaSchema.Value] = 1;
            dataType.View[0].Row.EndEdit();

            Assert.AreEqual(dataRow[0], dataType.View[0].Row[CremaSchema.Name]);
            Assert.AreEqual(dataRow[0], dataType.Members[0].Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test3()
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
            dataRow2[0] = "A";
            dataTable.Rows.Add(dataRow2);

            dataSet.AcceptChanges();

            dataTable.Columns[0].CremaType = dataType;
        }

        [TestMethod]
        public void Test4()
        {
            var dataSet1 = new CremaDataSet();

            var dataType1 = dataSet1.Types.Add();
            dataType1.AddMember("a", 0);
            dataType1.AddMember("b", 1);
            dataType1.AddMember("c", 2);
            dataType1.AcceptChanges();
            dataType1.Members["c"].Index = 1;
            dataType1.RejectChanges();

            Assert.AreEqual(0, dataType1.Members[0].Index);
            Assert.AreEqual(1, dataType1.Members[1].Index);
            Assert.AreEqual(2, dataType1.Members[2].Index);
        }

        [TestMethod]
        public void Test5()
        {
            var dataSet1 = new CremaDataSet();

            var dataType1 = dataSet1.Types.Add();
            dataType1.AddMember("a", 0);
            dataType1.AddMember("b", 1);
            dataType1.AddMember("c", 2);
            dataType1.AcceptChanges();
            dataType1.Members["b"].Delete();
            Assert.AreEqual(1, dataType1.Members["c"].Index);
            dataType1.RejectChanges();

            Assert.AreEqual(0, dataType1.Members["a"].Index);
            Assert.AreEqual(1, dataType1.Members["b"].Index);
            Assert.AreEqual(2, dataType1.Members["c"].Index);
        }

        [TestMethod]
        public void Test6()
        {
            var dataSet1 = new CremaDataSet();

            var dataType1 = dataSet1.Types.Add();
            var member1 = dataType1.AddMember("a", 0);
            var member2 = dataType1.AddMember("b", 1);
            var member3 = dataType1.AddMember("c", 2);

            member3.Index = 0;

            Assert.AreEqual(0, member3.Index);
            Assert.AreEqual(1, member1.Index);
            Assert.AreEqual(2, member2.Index);
        }

        [TestMethod]
        public void Test7()
        {
            var dataSet = new CremaDataSet();
            var dataType1 = dataSet.Types.Add();
            var member1 = dataType1.AddMember("member1", 0);
            var member2 = dataType1.AddMember("member2", 1);
            var member3 = dataType1.AddMember("member3", 2);

            var member4 = dataType1.NewMember();
            member4.Index = 0;
            member4.Name = "member4";
            dataType1.Members.Add(member4);

            Assert.AreEqual(1, member1.Index);
            Assert.AreEqual(2, member2.Index);
            Assert.AreEqual(3, member3.Index);
            Assert.AreEqual(0, member4.Index);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test8()
        {
            var dataSet = new CremaDataSet();
            var dataType1 = dataSet.Types.Add();
            var member1 = dataType1.AddMember("member1", 0);
            var member2 = dataType1.AddMember("member2", 1);
            var member3 = dataType1.AddMember("member3", 2);

            var member4 = dataType1.NewMember();
            member4.Index = 5;
            member4.Name = "member4";
            dataType1.Members.Add(member4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test9()
        {
            var dataSet = new CremaDataSet();
            var dataType1 = dataSet.Types.Add();
            var member1 = dataType1.AddMember("member1", 0);
            member1.Index = -1;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test10()
        {
            var dataSet = new CremaDataSet();
            var dataType1 = dataSet.Types.Add();
            var member1 = dataType1.AddMember("member1", 0);
            member1.Index = 1;
        }

        [TestMethod]
        public void Test11()
        {
            var dataSet1 = new CremaDataSet();
            var dataType1 = dataSet1.AddRandomType();
            var dataSet2 = new CremaDataSet();
            var dataType2 = dataSet2.Types.Add(dataType1.TypeInfo);

            CremaComparer.CompareType(dataType1, dataType2);
        }

        [TestMethod]
        public void ItemNameTest()
        {
            var dataSet1 = new CremaDataSet();
            var dataType1 = dataSet1.AddRandomType();

            var dataSet2 = new CremaDataSet();
            var typeName = RandomUtility.NextIdentifier();
            var categoryPath = RandomUtility.NextCategoryPath();
            dataSet2.ReadTypeString(dataType1.GetXmlSchema(), new ItemName(categoryPath, typeName));

            var dataType2 = dataSet2.Types.First();
            Assert.AreEqual(typeName, dataType2.Name);
            Assert.AreEqual(categoryPath, dataType2.CategoryPath);
        }
    }
}
