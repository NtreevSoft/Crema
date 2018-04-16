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
    public class CremaDataType_NewTest
    {
        [TestMethod]
        public void New()
        {
            var type = new CremaDataType();

            Assert.AreNotEqual(Guid.Empty, type.TypeID);
            Assert.AreEqual(string.Empty, type.TypeName);
            Assert.AreEqual(PathUtility.Separator, type.CategoryPath);
            Assert.AreEqual(CremaSchema.TypeNamespace + type.CategoryPath + type.TypeName, type.Namespace);
            Assert.AreEqual(string.Empty, type.Comment);
            Assert.AreEqual(TagInfo.All, type.Tags);

            Assert.AreNotEqual(type.CreationInfo, SignatureDate.Empty);
            Assert.AreNotEqual(type.ModificationInfo, SignatureDate.Empty);
            Assert.AreEqual(type.CreationInfo, type.ModificationInfo);
        }

        [TestMethod]
        public void NewWithName()
        {
            var typeName = RandomUtility.NextIdentifier();
            var type = new CremaDataType(typeName);

            Assert.AreEqual(typeName, type.TypeName);
            Assert.AreEqual(PathUtility.Separator, type.CategoryPath);
            Assert.AreEqual(CremaSchema.TypeNamespace + type.CategoryPath + type.TypeName, type.Namespace);
        }

        [TestMethod]
        public void NewWithNullName()
        {
            var type = new CremaDataType(null);

            Assert.AreEqual(string.Empty, type.TypeName);
            Assert.AreEqual(PathUtility.Separator, type.CategoryPath);
            Assert.AreEqual(CremaSchema.TypeNamespace + type.CategoryPath + type.TypeName, type.Namespace);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewWithInvalidName_Fail()
        {
            var typeName = RandomUtility.NextInvalidIdentifier();
            new CremaDataType(typeName);
        }

        [TestMethod]
        public void NewWithNameAndCategoryPath()
        {
            var typeName = RandomUtility.NextIdentifier();
            var categoryPath = RandomUtility.NextCategoryPath();
            var type = new CremaDataType(typeName, categoryPath);

            Assert.AreEqual(typeName, type.TypeName);
            Assert.AreEqual(categoryPath, type.CategoryPath);
            Assert.AreEqual(CremaSchema.TypeNamespace + type.CategoryPath + type.TypeName, type.Namespace);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NewWithNameAndNullCategory_Fail()
        {
            var typeName = RandomUtility.NextIdentifier();
            var type = new CremaDataType(typeName, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewWithNameAndInvalidCategory_Fail()
        {
            var typeName = RandomUtility.NextIdentifier();
            var categoryPath = RandomUtility.NextInvalidCategoryPath();
            var type = new CremaDataType(typeName, categoryPath);
        }

        [TestMethod]
        public void NewWithNullNameAndType()
        {
            var categoryPath = RandomUtility.NextCategoryPath();
            var type = new CremaDataType(null, categoryPath);

            Assert.AreEqual(string.Empty, type.TypeName);
            Assert.AreEqual(categoryPath, type.CategoryPath);
            Assert.AreEqual(CremaSchema.TypeNamespace + type.CategoryPath + type.TypeName, type.Namespace);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NewWithNullNameAndNullCategory_Fail()
        {
            var type = new CremaDataType(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewWithNullNameAndInvalidCategory_Fail()
        {
            var categoryPath = RandomUtility.NextInvalidCategoryPath();
            var type = new CremaDataType(null, categoryPath);
        }
    }
}