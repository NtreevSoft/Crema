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
using Ntreev.Library.IO;
using Ntreev.Crema.Data.Xml.Schema;
using System.Data;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaDataTable_MethodTest
    {
        private CremaDataTable table = new CremaDataTable();

        [TestMethod]
        public void BeginLoad()
        {
            var column1 = this.table.Columns.Add();
            column1.IsKey = true;

            this.table.BeginLoad();

            var row1 = this.table.NewRow();
            row1[0] = "a";
            this.table.Rows.Add(row1);

            var row2 = this.table.NewRow();
            row2[0] = "a";
            this.table.Rows.Add(row2);
        }

        [TestMethod]
        [ExpectedException(typeof(ConstraintException))]
        public void EndLoad_Fail()
        {
            var column1 = this.table.Columns.Add();
            column1.IsKey = true;

            this.table.BeginLoad();

            var row1 = this.table.NewRow();
            row1[0] = "a";
            this.table.Rows.Add(row1);

            var row2 = this.table.NewRow();
            row2[0] = "a";
            this.table.Rows.Add(row2);

            this.table.EndLoad();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InheritWithNullName_Fail()
        {
            this.table.Inherit(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InheritWithEmptyName_Fail()
        {
            this.table.Inherit(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InheritWithInvalidName_Fail()
        {
            var tableName = RandomUtility.NextInvalidIdentifier();
            this.table.Inherit(tableName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InheritWithNameAndNullCategoryPath_Fail()
        {
            var tableName = RandomUtility.NextIdentifier();
            this.table.Inherit(tableName, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InheritWithNameAndEmptyCategoryPath_Fail()
        {
            var tableName = RandomUtility.NextIdentifier();
            this.table.Inherit(tableName, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InheritWithNameAndInvalidCategoryPath_Fail()
        {
            var tableName = RandomUtility.NextIdentifier();
            var categoryPath = RandomUtility.NextInvalidCategoryPath();
            this.table.Inherit(tableName, categoryPath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InheritWithNameAndCategoryPath_Fail()
        {
            var tableName = RandomUtility.NextIdentifier();
            var categoryPath = RandomUtility.NextCategoryPath();
            this.table.Inherit(tableName, categoryPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyWithNullName_Fail()
        {
            this.table.Copy(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyWithEmptyName_Fail()
        {
            this.table.Copy(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyWithInvalidName_Fail()
        {
            var tableName = RandomUtility.NextInvalidIdentifier();
            this.table.Copy(tableName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyWithNameAndNullCategoryPath_Fail()
        {
            var tableName = RandomUtility.NextIdentifier();
            this.table.Copy(tableName, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyWithNameAndEmptyCategoryPath_Fail()
        {
            var tableName = RandomUtility.NextIdentifier();
            this.table.Copy(tableName, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyWithNameAndInvalidCategoryPath_Fail()
        {
            var tableName = RandomUtility.NextIdentifier();
            var categoryPath = RandomUtility.NextInvalidCategoryPath();
            this.table.Copy(tableName, categoryPath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CopyWithNameAndCategoryPath_Fail()
        {
            var tableName = RandomUtility.NextIdentifier();
            var categoryPath = RandomUtility.NextCategoryPath();
            this.table.Copy(tableName, categoryPath);
        }

        [TestMethod]
        public void CopySchemaOnly()
        {
            this.InitializeRandom(true);
            var table2 = this.table.Copy(this.table.TableName, this.table.CategoryPath);

            CremaComparer.CompareSchema(this.table, table2);
        }

        [TestMethod]
        public void Copy()
        {
            this.InitializeRandom(true);
            var table2 = this.table.Copy(this.table.TableName, this.table.CategoryPath, true);

            CremaComparer.CompareSchema(this.table, table2);
            CremaComparer.CompareXml(this.table, table2);
        }

        [TestMethod]
        public void CopyChildSchemaOnly()
        {
            this.InitializeRandom(true);

            var child1 = this.table.Childs.Random();
            var child2 = child1.Copy(child1.TableName, child1.CategoryPath);
        }

        [TestMethod]
        public void CopyChild()
        {
            this.InitializeRandom(true);

            var child1 = this.table.Childs.Random();
            var child2 = child1.Copy(child1.Name, child1.CategoryPath, true);
        }

        [TestMethod]
        public void AcceptChanges()
        {
            this.InitializeRandom();

            this.table.AcceptChanges();
            var rowCount = this.table.Rows.Count;
            this.table.RejectChanges();

            Assert.AreEqual(rowCount, this.table.Rows.Count);
        }

        [TestMethod]
        public void RejectChanges()
        {
            this.InitializeRandom();
            this.table.RejectChanges();

            Assert.AreEqual(0, this.table.Rows.Count);
        }

        [TestMethod]
        public void Clear()
        {
            this.InitializeRandom();

            this.table.AcceptChanges();
            this.table.Clear();

            Assert.AreEqual(0, this.table.Rows.Count);
        }

        [TestMethod]
        public void GetErrors()
        {
            this.InitializeRandom();

            this.table.Rows.Random().SetColumnError(this.table.Columns.Random(), "error");

            var errors = this.table.GetErrors();
            Assert.AreEqual(1, errors.Length);
        }

        [TestMethod]
        public void WriteAndReadXml_Stream()
        {
            this.InitializeRandom(true);

            var table2 = new CremaDataTable();
            table2.ReadXmlSchemaString(this.table.GetXmlSchema(true));
            table2.ReadXmlString(this.table.GetXml(true));

            CremaComparer.CompareTable(this.table, table2);
        }

        private void InitializeRandom()
        {
            this.InitializeRandom(false);
        }

        private void InitializeRandom(bool child)
        {
            var tableName = RandomUtility.NextIdentifier();
            var categoryPath = RandomUtility.NextCategoryPath();
            this.table.TableName = tableName;
            this.table.CategoryPath = categoryPath;
            this.table.AddRandomColumns(10);
            this.table.AddRandomRows(10);
            if (child == true)
                this.table.AddRandomChilds(10);
        }
    }
}