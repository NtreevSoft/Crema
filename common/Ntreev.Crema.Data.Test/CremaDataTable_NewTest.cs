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

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaDataTable_NewTest
    {
        [TestMethod]
        public void New()
        {
            var table = new CremaDataTable();

            Assert.AreNotEqual(Guid.Empty, table.TableID);
            Assert.AreEqual(string.Empty, table.Name);
            Assert.AreEqual(string.Empty, table.TableName);
            Assert.AreEqual(PathUtility.Separator, table.CategoryPath);
            Assert.AreEqual(CremaSchema.TableNamespace + table.CategoryPath + table.Name, table.Namespace);
            Assert.AreEqual(string.Empty, table.Comment);
            Assert.AreEqual(TagInfo.All, table.Tags);
            Assert.AreEqual(TagInfo.All, table.DerivedTags);
            Assert.AreEqual(string.Empty, table.TemplateNamespace);
            Assert.IsNull(table.Parent);
            Assert.IsNull(table.TemplatedParent);

            Assert.AreNotEqual(table.CreationInfo, SignatureDate.Empty);
            Assert.AreNotEqual(table.ModificationInfo, SignatureDate.Empty);
            Assert.AreEqual(table.CreationInfo, table.ModificationInfo);
            Assert.AreEqual(table.ContentsInfo, SignatureDate.Empty);
        }

        [TestMethod]
        public void NewWithName()
        {
            var tableName = RandomUtility.NextIdentifier();
            var table = new CremaDataTable(tableName);

            Assert.AreEqual(tableName, table.Name);
            Assert.AreEqual(tableName, table.TableName);
            Assert.AreEqual(PathUtility.Separator, table.CategoryPath);
            Assert.AreEqual(CremaSchema.TableNamespace + table.CategoryPath + table.Name, table.Namespace);
        }

        [TestMethod]
        public void NewWithNullName()
        {
            var table = new CremaDataTable(null);

            Assert.AreEqual(string.Empty, table.Name);
            Assert.AreEqual(string.Empty, table.TableName);
            Assert.AreEqual(PathUtility.Separator, table.CategoryPath);
            Assert.AreEqual(CremaSchema.TableNamespace + table.CategoryPath + table.Name, table.Namespace);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewWithInvalidName_Fail()
        {
            var tableName = RandomUtility.NextInvalidIdentifier();
            new CremaDataTable(tableName);
        }

        [TestMethod]
        public void NewWithNameAndCategoryPath()
        {
            var tableName = RandomUtility.NextIdentifier();
            var categoryPath = RandomUtility.NextCategoryPath();
            var table = new CremaDataTable(tableName, categoryPath);

            Assert.AreEqual(tableName, table.Name);
            Assert.AreEqual(tableName, table.TableName);
            Assert.AreEqual(categoryPath, table.CategoryPath);
            Assert.AreEqual(CremaSchema.TableNamespace + table.CategoryPath + table.Name, table.Namespace);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NewWithNameAndNullCategory_Fail()
        {
            var tableName = RandomUtility.NextIdentifier();
            var table = new CremaDataTable(tableName, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewWithNameAndInvalidCategory_Fail()
        {
            var tableName = RandomUtility.NextIdentifier();
            var categoryPath = RandomUtility.NextInvalidCategoryPath();
            var table = new CremaDataTable(tableName, categoryPath);
        }

        [TestMethod]
        public void NewWithNullNameAndType()
        {
            var categoryPath = RandomUtility.NextCategoryPath();
            var table = new CremaDataTable(null, categoryPath);

            Assert.AreEqual(string.Empty, table.Name);
            Assert.AreEqual(string.Empty, table.TableName);
            Assert.AreEqual(categoryPath, table.CategoryPath);
            Assert.AreEqual(CremaSchema.TableNamespace + table.CategoryPath + table.Name, table.Namespace);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NewWithNullNameAndNullCategory_Fail()
        {
            var table = new CremaDataTable(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewWithNullNameAndInvalidCategory_Fail()
        {
            var categoryPath = RandomUtility.NextInvalidCategoryPath();
            var table = new CremaDataTable(null, categoryPath);
        }
    }
}