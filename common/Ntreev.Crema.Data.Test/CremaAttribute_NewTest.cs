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
    public class CremaAttribute_NewTest
    {
        [TestMethod]
        public void New()
        {
            var attribute = new CremaAttribute();

            Assert.AreEqual(string.Empty, attribute.AttributeName);
            Assert.AreEqual(typeof(string), attribute.DataType);
            Assert.AreEqual(true, attribute.AllowDBNull);
            Assert.AreEqual(false, attribute.AutoIncrement);
            Assert.AreEqual(string.Empty, attribute.Comment);
            Assert.AreEqual(DBNull.Value, attribute.DefaultValue);
        }

        [TestMethod]
        public void NewWithName()
        {
            var attributeName = RandomUtility.NextIdentifier();
            var attribute = new CremaAttribute(attributeName);

            Assert.AreEqual(attributeName, attribute.AttributeName);
            Assert.AreEqual(typeof(string), attribute.DataType);
            Assert.AreEqual(true, attribute.AllowDBNull);
            Assert.AreEqual(false, attribute.AutoIncrement);
            Assert.AreEqual(string.Empty, attribute.Comment);
            Assert.AreEqual(DBNull.Value, attribute.DefaultValue);
        }

        [TestMethod]
        public void NewWithNullName()
        {
            var attribute = new CremaAttribute(null);

            Assert.AreEqual(string.Empty, attribute.AttributeName);
            Assert.AreEqual(typeof(string), attribute.DataType);
            Assert.AreEqual(true, attribute.AllowDBNull);
            Assert.AreEqual(false, attribute.AutoIncrement);
            Assert.AreEqual(string.Empty, attribute.Comment);
            Assert.AreEqual(DBNull.Value, attribute.DefaultValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewWithName_Fail()
        {
            var attributeName = RandomUtility.NextInvalidIdentifier();
            new CremaAttribute(attributeName);
        }

        [TestMethod]
        public void NewWithNameAndType()
        {
            var attributeName = RandomUtility.NextIdentifier();
            var attributeType = CremaDataTypeUtility.GetBaseTypes().Random();
            var attribute = new CremaAttribute(attributeName, attributeType);

            Assert.AreEqual(attributeName, attribute.AttributeName);
            Assert.AreEqual(attributeType, attribute.DataType);
            Assert.AreEqual(true, attribute.AllowDBNull);
            Assert.AreEqual(false, attribute.AutoIncrement);
            Assert.AreEqual(string.Empty, attribute.Comment);
            Assert.AreEqual(DBNull.Value, attribute.DefaultValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewWithNameAndInvalidType_Fail()
        {
            var attributeName = RandomUtility.NextIdentifier();
            var attributeType = typeof(string).Assembly.GetTypes().Random(item => CremaDataTypeUtility.IsBaseType(item) == false);
            new CremaAttribute(attributeName, attributeType);
        }

        [TestMethod]
        public void NewWithNullNameAndType()
        {
            var attributeType = CremaDataTypeUtility.GetBaseTypes().Random();
            var attribute = new CremaAttribute(null, attributeType);

            Assert.AreEqual(string.Empty, attribute.AttributeName);
            Assert.AreEqual(attributeType, attribute.DataType);
            Assert.AreEqual(true, attribute.AllowDBNull);
            Assert.AreEqual(false, attribute.AutoIncrement);
            Assert.AreEqual(string.Empty, attribute.Comment);
            Assert.AreEqual(DBNull.Value, attribute.DefaultValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewWithNullNameAndInvalidType_Fail()
        {
            var attributeType = typeof(string).Assembly.GetTypes().Random(item => CremaDataTypeUtility.IsBaseType(item) == false);
            new CremaAttribute(null, attributeType);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NewWithNullNameAndNullType_Fail()
        {
            new CremaAttribute(null, null);
        }
    }
}