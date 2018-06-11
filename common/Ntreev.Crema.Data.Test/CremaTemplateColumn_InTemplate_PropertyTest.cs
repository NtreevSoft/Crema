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
using Ntreev.Crema.Data.Xml;
using System.Data;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaTemplateColumn_InTemplate_PropertyTest
    {
        private CremaTemplate template;
        private CremaTemplateColumn column;

        public CremaTemplateColumn_InTemplate_PropertyTest()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            this.template = new CremaTemplate(dataSet.Tables.Random(item => item.TemplateNamespace == string.Empty));
            this.column = this.template.Columns.Random();
        }

        [TestMethod]
        public void SetName()
        {
            var columnName = RandomUtility.NextIdentifier();
            this.column.Name = columnName;
            Assert.AreEqual(columnName, this.column.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetExistedName()
        {
            var column = this.template.Columns.RandomOrDefault(item => item != this.column);
            if (column == null)
                Assert.Inconclusive();
            this.column.Name = column.Name;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetInvalidName()
        {
            var columnName = RandomUtility.NextInvalidIdentifier();
            this.column.Name = columnName;
            Assert.AreEqual(columnName, this.column.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetNullName()
        {
            this.column.Name = null;
            Assert.AreEqual(string.Empty, this.column.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetEmptyName()
        {
            this.column.Name = string.Empty;
            Assert.AreEqual(string.Empty, this.column.Name);
        }

        [TestMethod]
        public void SetDataType()
        {
            var dataTypeName = typeof(string).GetTypeName();
            this.column = this.template.Columns.RandomOrDefault(item => item.DefaultValue == DBNull.Value && item.AutoIncrement == false);
            if (this.column == null)
                return;
            this.column.DataTypeName = dataTypeName;
            Assert.AreEqual(dataTypeName, this.column.DataTypeName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetNullDataType()
        {
            this.column.DataTypeName = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetEmptyStringDataType()
        {
            this.column.DataTypeName = string.Empty;
        }

        [TestMethod]
        public void SetTrueIsUnique()
        {
            this.column = this.template.Columns.RandomOrDefault(item => item.Unique == false && item.VerifyUnique());
            if (this.column == null)
                return;
            this.column.Unique = true;
            Assert.IsTrue(this.column.Unique);
        }

        [TestMethod]
        public void SetFalseIsUnique()
        {
            this.column = this.template.Columns.RandomOrDefault(item => item.IsKey == false && item.Unique == true);
            if (this.column == null)
                return;
            this.column.Unique = false;
            Assert.IsFalse(this.column.Unique);
        }

        [TestMethod]
        public void SetNullDefaultValue()
        {
            this.column = this.template.Columns.RandomOrDefault(Predicate);
            if (this.column == null)
                return;
            var dataType = CremaDataTypeUtility.GetType(this.column.DataTypeName);
            var defaultValue = RandomUtility.Next(dataType);
            this.column.DefaultValue = defaultValue;
            this.column.DefaultValue = null;
            Assert.AreEqual(DBNull.Value, this.column.DefaultValue);

            bool Predicate(CremaTemplateColumn item)
            {
                if (CremaDataTypeUtility.IsBaseType(item.DataTypeName) == false)
                    return false;
                if (item.AutoIncrement == true)
                    return false;
                return true;
            }
        }

        [TestMethod]
        public void SetEmptyDefaultValue()
        {
            this.column.DefaultValue = string.Empty;
            Assert.AreEqual(DBNull.Value, this.column.DefaultValue);
        }

        [TestMethod]
        public void SetComment()
        {
            var comment = RandomUtility.NextString();
            this.column.Comment = comment;
            Assert.AreEqual(comment, this.column.Comment);
        }

        [TestMethod]
        public void SetNullComment()
        {
            this.column.Comment = RandomUtility.NextString();
            this.column.Comment = null;
            Assert.AreEqual(string.Empty, this.column.Comment);
        }

        [TestMethod]
        public void SetEmptyComment()
        {
            this.column.Comment = RandomUtility.NextString();
            this.column.Comment = string.Empty;
            Assert.AreEqual(string.Empty, this.column.Comment);
        }

        [TestMethod]
        public void SetRandomTags()
        {
            var tags = RandomUtility.NextTags();
            this.column.Tags = tags;
            Assert.AreEqual(tags, this.column.Tags);
        }

        [TestMethod]
        public void SetTrueReadOnly()
        {
            this.column.ReadOnly = true;
            Assert.AreEqual(true, this.column.ReadOnly);
        }

        [TestMethod]
        public void SetFalseReadOnly()
        {
            this.column.ReadOnly = true;
            this.column.ReadOnly = false;
            Assert.AreEqual(false, this.column.ReadOnly);
        }
    }
}