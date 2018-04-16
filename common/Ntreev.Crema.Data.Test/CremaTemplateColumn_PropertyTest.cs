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

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaTemplateColumn_PropertyTest
    {
        private CremaTemplate template = new CremaTemplate(new CremaDataTable());
        private CremaTemplateColumn column;

        public CremaTemplateColumn_PropertyTest()
        {
            this.column = this.template.NewColumn();
        }

        [TestMethod]
        public void New()
        {
            Assert.AreEqual(-1, this.column.Index);
            Assert.AreEqual(false, this.column.IsKey);
            Assert.AreEqual("Column1", this.column.Name);
            Assert.AreEqual(typeof(string).GetTypeName(), this.column.DataTypeName);
            Assert.AreEqual(false, this.column.Unique);
            Assert.AreEqual(DBNull.Value, this.column.DefaultValue);
            Assert.AreEqual(false, this.column.AutoIncrement);
            Assert.AreEqual(string.Empty, this.column.Comment);
            Assert.AreEqual(TagInfo.All, this.column.Tags);
            Assert.AreEqual(false, this.column.ReadOnly);
        }

        [TestMethod]
        public void SetName()
        {
            var columnName = RandomUtility.NextIdentifier();
            this.column.Name = columnName;
            Assert.AreEqual(columnName, this.column.Name);
        }

        [TestMethod]
        public void SetInvalidName()
        {
            var columnName = RandomUtility.NextInvalidIdentifier();
            this.column.Name = columnName;
            Assert.AreEqual(columnName, this.column.Name);
        }

        [TestMethod]
        public void SetNullName()
        {
            this.column.Name = null;
            Assert.AreEqual(string.Empty, this.column.Name);
        }

        [TestMethod]
        public void SetEmptyName()
        {
            this.column.Name = string.Empty;
            Assert.AreEqual(string.Empty, this.column.Name);
        }

        [TestMethod]
        public void SetDataType()
        {
            var dataType = this.template.Types.Random();
            this.column.DataTypeName = dataType;
            Assert.AreEqual(dataType, this.column.DataTypeName);
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
        public void SetTrueIsKey()
        {
            this.column.IsKey = true;
            Assert.IsTrue(this.column.IsKey);
        }

        [TestMethod]
        public void SetFalseIsKey()
        {
            this.column.IsKey = true;
            this.column.IsKey = false;
            Assert.IsFalse(this.column.IsKey);
        }

        [TestMethod]
        public void SetTrueIsUnique()
        {
            this.column.Unique = true;
            Assert.IsTrue(this.column.Unique);
        }

        [TestMethod]
        public void SetFalseIsUnique()
        {
            this.column.Unique = true;
            this.column.Unique = false;
            Assert.IsFalse(this.column.Unique);
        }

        [TestMethod]
        public void SetTrueAutoIncrement()
        {
            this.column.AutoIncrement = true;
            Assert.IsTrue(this.column.AutoIncrement);
            Assert.AreEqual(typeof(string).GetTypeName(), this.column.DataTypeName);
        }

        [TestMethod]
        public void SetDefaultValue()
        {
            var defaultValue = RandomUtility.NextString();
            this.column.DefaultValue = defaultValue;
            if (defaultValue == string.Empty)
                Assert.AreEqual(DBNull.Value, this.column.DefaultValue);
            else
                Assert.AreEqual(defaultValue, this.column.DefaultValue); 
        }

        [TestMethod]
        public void SetDefaultValue2()
        {
            var dataType = CremaDataTypeUtility.GetBaseTypes().Random();
            var defaultValue = RandomUtility.Next(dataType);
            this.column.DataTypeName = dataType.GetTypeName();
            this.column.DefaultValue = defaultValue;
            if (dataType == typeof(DateTime))
                Assert.AreEqual(defaultValue.ToString(), this.column.DefaultValue.ToString());
            else
                Assert.AreEqual(defaultValue, this.column.DefaultValue);
        }

        [TestMethod]
        public void SetDefaultValue3()
        {
            var dataType = CremaDataTypeUtility.GetBaseTypes().Random();
            var defaultValue = RandomUtility.Next(dataType);
            this.column.DefaultValue = defaultValue;
            this.column.DataTypeName = dataType.GetTypeName();
            if (dataType == typeof(DateTime))
                Assert.AreEqual(defaultValue.ToString(), this.column.DefaultValue.ToString());
            else
                Assert.AreEqual(defaultValue, this.column.DefaultValue);
        }

        [TestMethod]
        public void SetNullDefaultValue()
        {
            var dataType = CremaDataTypeUtility.GetBaseTypes().Random();
            var defaultValue = CremaConvert.ToString(RandomUtility.Next(dataType));
            this.column.DefaultValue = defaultValue;
            this.column.DataTypeName = dataType.GetTypeName();
            this.column.DefaultValue = null;
            Assert.AreEqual(DBNull.Value, this.column.DefaultValue);
        }

        [TestMethod]
        public void SetEmptyDefaultValue()
        {
            var dataType = CremaDataTypeUtility.GetBaseTypes().Random();
            var defaultValue = CremaConvert.ToString(RandomUtility.Next(dataType));
            this.column.DefaultValue = defaultValue;
            this.column.DataTypeName = dataType.GetTypeName();
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