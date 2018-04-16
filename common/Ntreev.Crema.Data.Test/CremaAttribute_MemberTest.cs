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
using Ntreev.Library;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaAttribute_MemberTest
    {
        private CremaAttribute attribute = new CremaAttribute();

        public CremaAttribute_MemberTest()
        {

        }

        [TestMethod]
        public void Compare()
        {
            Assert.AreEqual(string.Empty, this.attribute.AttributeName);
            Assert.AreEqual(true, this.attribute.AllowDBNull);
            Assert.AreEqual(false, this.attribute.AutoIncrement);
            Assert.AreEqual(typeof(string), this.attribute.DataType);
            Assert.AreEqual(string.Empty, this.attribute.Comment);
            Assert.AreEqual(DBNull.Value, this.attribute.DefaultValue);
        }

        [TestMethod]
        public void SetNullName()
        {
            this.attribute.AttributeName = RandomUtility.NextIdentifier();
            this.attribute.AttributeName = null;
            Assert.AreEqual(string.Empty, this.attribute.AttributeName);
        }

        [TestMethod]
        public void SetName()
        {
            var newName = RandomUtility.NextIdentifier();
            this.attribute.AttributeName = newName;
            Assert.AreEqual(newName, this.attribute.AttributeName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetInvalidName_Fail()
        {
            var newName = RandomUtility.NextInvalidIdentifier();
            this.attribute.AttributeName = newName;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetNullDataType_Fail()
        {
            this.attribute.DataType = null;
        }

        [TestMethod]
        public void SetDataType()
        {
            var newType = CremaDataTypeUtility.GetBaseTypes().Random(item => item != this.attribute.DataType);
            this.attribute.DataType = newType;
            Assert.AreEqual(newType, this.attribute.DataType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetDataType_Fail()
        {
            var newType = typeof(string).Assembly.GetTypes().Random(item => CremaDataTypeUtility.IsBaseType(item) == false);
            this.attribute.DataType = newType;
        }

        [TestMethod]
        public void SetComment()
        {
            var newComment = RandomUtility.NextString();
            this.attribute.Comment = newComment;
            Assert.AreEqual(newComment, this.attribute.Comment);
        }

        [TestMethod]
        public void SetNullComment()
        {
            this.attribute.Comment = RandomUtility.NextString();
            this.attribute.Comment = null;
            Assert.AreEqual(string.Empty, this.attribute.Comment);
        }

        [TestMethod]
        public void SetDefaultValue()
        {
            this.attribute.DataType = CremaDataTypeUtility.GetBaseTypes().Random();
            var newDefaultValue = RandomUtility.Next(this.attribute.DataType);
            this.attribute.DefaultValue = newDefaultValue;
            Assert.AreEqual(newDefaultValue, this.attribute.DefaultValue);
        }

        [TestMethod]
        public void SetDefaultStringValue()
        {
            this.attribute.DataType = CremaDataTypeUtility.GetBaseTypes().Random();
            var newDefaultValue = RandomUtility.Next(this.attribute.DataType);
            this.attribute.DefaultValue = CremaConvert.ChangeType(newDefaultValue, typeof(string));
            Assert.AreEqual(newDefaultValue.GetType(), this.attribute.DefaultValue.GetType());
            if (this.attribute.DataType == typeof(DateTime))
                Assert.AreEqual(newDefaultValue.ToString(), this.attribute.DefaultValue.ToString());
            else
                Assert.AreEqual(newDefaultValue, this.attribute.DefaultValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetDefaultValue_Fail()
        {
            var type1 = CremaDataTypeUtility.GetBaseTypes().Random(item => item != typeof(string));
            this.attribute.DataType = type1;
            this.attribute.DefaultValue = RandomUtility.NextWord();
        }

        [TestMethod]
        public void SetNullDefaultValue()
        {
            this.attribute.DefaultValue = RandomUtility.NextString();
            this.attribute.DefaultValue = null;
            Assert.AreEqual(DBNull.Value, this.attribute.DefaultValue);
        }

        [TestMethod]
        public void SetAutoIncrementTrue()
        {
            this.attribute.DataType = CremaDataTypeUtility.GetBaseTypes().Random(item => CremaDataTypeUtility.CanUseAutoIncrement(item));
            this.attribute.AutoIncrement = true;
            Assert.AreEqual(true, this.attribute.AutoIncrement);
        }

        [TestMethod]
        public void SetAutoIncrementFalse()
        {
            this.attribute.DataType = CremaDataTypeUtility.GetBaseTypes().Random(item => CremaDataTypeUtility.CanUseAutoIncrement(item));
            this.attribute.AutoIncrement = true;
            this.attribute.AutoIncrement = false;
            Assert.AreEqual(false, this.attribute.AutoIncrement);
        }

        [TestMethod]
        public void SetAutoIncrementToInvalidIncrementType()
        {
            this.attribute.DataType = CremaDataTypeUtility.GetBaseTypes().Random(item => CremaDataTypeUtility.CanUseAutoIncrement(item) == false);
            this.attribute.AutoIncrement = true;
            Assert.AreEqual(typeof(int), this.attribute.DataType);
            Assert.AreEqual(true, this.attribute.AutoIncrement);
        }

        [TestMethod]
        public void SetAllowDBNullTrue()
        {
            this.attribute.AllowDBNull = true;
            Assert.AreEqual(true, this.attribute.AllowDBNull);
        }

        [TestMethod]
        public void SetAllowDBNullFalse()
        {
            this.attribute.AllowDBNull = true;
            this.attribute.AllowDBNull = false;
            Assert.AreEqual(false, this.attribute.AllowDBNull);
        }

        [TestMethod]
        public void SetReadOnlyTrue()
        {
            this.attribute.ReadOnly = true;
            Assert.AreEqual(true, this.attribute.ReadOnly);
        }

        [TestMethod]
        public void SetReadOnlyFalse()
        {
            this.attribute.ReadOnly = true;
            this.attribute.ReadOnly = false;
            Assert.AreEqual(false, this.attribute.ReadOnly);
        }

        [TestMethod]
        public void SetIsVisibleTrue()
        {
            this.attribute.IsVisible = true;
            Assert.AreEqual(true, this.attribute.IsVisible);
        }

        [TestMethod]
        public void SetIsVisibleFalse()
        {
            this.attribute.IsVisible = true;
            this.attribute.IsVisible = false;
            Assert.AreEqual(false, this.attribute.IsVisible);
        }

        [TestMethod]
        public void SetUniqueTrue()
        {
            this.attribute.Unique = true;
            Assert.AreEqual(true, this.attribute.Unique);
        }

        [TestMethod]
        public void SetUniqueFalse()
        {
            this.attribute.Unique = true;
            this.attribute.Unique = false;
            Assert.AreEqual(false, this.attribute.Unique);
        }
    }
}
