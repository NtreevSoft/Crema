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
using System.Threading;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaDataTable_PropertyTest
    {
        private CremaDataTable table = new CremaDataTable();

        [TestMethod]
        public void SetName()
        {
            var tableName = RandomUtility.NextIdentifier();
            var modificationInfo = this.table.ModificationInfo;
            Thread.Sleep(100);
            this.table.TableName = tableName;
            Assert.AreEqual(tableName, this.table.TableName);
            Assert.AreNotEqual(modificationInfo.DateTime, this.table.ModificationInfo.DateTime);
        }

        [TestMethod]
        public void SetNullName()
        {
            this.table.TableName = RandomUtility.NextIdentifier();
            this.table.TableName = null;
            Assert.AreEqual(string.Empty, this.table.TableName);
        }

        [TestMethod]
        public void SetEmptyName()
        {
            this.table.TableName = RandomUtility.NextIdentifier();
            this.table.TableName = string.Empty;
            Assert.AreEqual(string.Empty, this.table.TableName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetInvalidName()
        {
            this.table.TableName = RandomUtility.NextInvalidIdentifier();
        }

        [TestMethod]
        public void SetCategoryPath()
        {
            var categoryPath = RandomUtility.NextCategoryPath(3);
            var modificationInfo = this.table.ModificationInfo;
            Thread.Sleep(1000);
            this.table.CategoryPath = categoryPath;
            Assert.AreEqual(string.Empty, this.table.TableName);
            Assert.AreEqual(categoryPath, this.table.CategoryPath);
            Assert.AreEqual(modificationInfo.DateTime, this.table.ModificationInfo.DateTime);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetInvalidCategoryPath()
        {
            var categoryPath = RandomUtility.NextInvalidCategoryPath();
            this.table.CategoryPath = categoryPath;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetNullCategoryPath()
        {
            this.table.CategoryPath = null;
        }

        [TestMethod]
        public void SetNameAndCategoryPath()
        {
            var tableName = RandomUtility.NextIdentifier();
            var categoryPath = RandomUtility.NextCategoryPath();
            this.table.TableName = tableName;
            this.table.CategoryPath = categoryPath;
            Assert.AreEqual(tableName, this.table.TableName);
            Assert.AreEqual(categoryPath, this.table.CategoryPath);
            Assert.AreEqual(CremaSchema.TableNamespace + categoryPath + tableName, table.Namespace);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetNameAndInvalidCategoryPath()
        {
            var tableName = RandomUtility.NextIdentifier();
            var categoryPath = RandomUtility.NextInvalidCategoryPath();
            this.table.TableName = tableName;
            this.table.CategoryPath = categoryPath;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetNameAndNullCategoryPath()
        {
            var tableName = RandomUtility.NextIdentifier();
            this.table.TableName = tableName;
            this.table.CategoryPath = null;
        }

        [TestMethod]
        public void SetTags()
        {
            var tag = RandomUtility.NextTags();
            var modificationInfo = this.table.ModificationInfo;
            Thread.Sleep(1000);
            if (this.table.Tags == tag)
                return;
            this.table.Tags = tag;
            Assert.AreEqual(tag, this.table.Tags);
            Assert.AreNotEqual(modificationInfo.DateTime, this.table.ModificationInfo.DateTime);
        }

        [TestMethod]
        public void SetComment()
        {
            var comment = RandomUtility.NextString();
            var modificationInfo = this.table.ModificationInfo;
            Thread.Sleep(1000);
            this.table.Comment = comment;
            Assert.AreEqual(comment, this.table.Comment);
            Assert.AreNotEqual(modificationInfo.DateTime, this.table.ModificationInfo.DateTime);
        }

        [TestMethod]
        public void SetEmptyComment()
        {
            this.table.Comment = RandomUtility.NextString();
            this.table.Comment = string.Empty;
            Assert.AreEqual(string.Empty, this.table.Comment);
        }

        [TestMethod]
        public void SetNullComment()
        {
            this.table.Comment = RandomUtility.NextString();
            this.table.Comment = null;
            Assert.AreEqual(string.Empty, this.table.Comment);
        }
    }
}