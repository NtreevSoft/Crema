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

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaDataColumn_NewTest
    {
        [TestMethod]
        public void New()
        {
            var column = new CremaDataColumn();

            Assert.AreNotEqual(Guid.Empty, column.ColumnID);
            Assert.AreEqual(-1, column.Index);
            Assert.AreEqual(string.Empty, column.ColumnName);
            Assert.AreEqual(typeof(string), column.DataType);
            Assert.AreEqual(string.Empty, column.Comment);
            Assert.IsFalse(column.IsKey);
            Assert.IsFalse(column.Unique);
            Assert.IsFalse(column.ReadOnly);
            Assert.AreEqual(null, column.Table);
            Assert.AreEqual(TagInfo.All, column.Tags);
            Assert.AreEqual(TagInfo.All, column.DerivedTags);
            Assert.AreEqual(string.Empty, column.Validation);
            Assert.AreEqual(string.Empty, column.Expression);
            Assert.IsNull(column.CremaType);
            Assert.IsTrue(column.AllowDBNull);
            Assert.IsFalse(column.AutoIncrement);
            Assert.AreEqual(DBNull.Value, column.DefaultValue);

            Assert.AreEqual(column.CreationInfo, SignatureDate.Empty);
            Assert.AreEqual(column.ModificationInfo, SignatureDate.Empty);
        }

        [TestMethod]
        public void NewWithName()
        {
            var columnName = RandomUtility.NextIdentifier();
            var column = new CremaDataColumn(columnName);

            Assert.AreNotEqual(Guid.Empty, column.ColumnID);
            Assert.AreEqual(-1, column.Index);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(string), column.DataType);
            Assert.AreEqual(string.Empty, column.Comment);
            Assert.IsFalse(column.IsKey);
            Assert.IsFalse(column.Unique);
            Assert.IsFalse(column.ReadOnly);
            Assert.AreEqual(null, column.Table);
            Assert.AreEqual(TagInfo.All, column.Tags);
            Assert.AreEqual(TagInfo.All, column.DerivedTags);
            Assert.AreEqual(string.Empty, column.Validation);
            Assert.AreEqual(string.Empty, column.Expression);
            Assert.IsNull(column.CremaType);
            Assert.IsTrue(column.AllowDBNull);
            Assert.IsFalse(column.AutoIncrement);
            Assert.AreEqual(DBNull.Value, column.DefaultValue);

            Assert.AreEqual(column.CreationInfo, SignatureDate.Empty);
            Assert.AreEqual(column.ModificationInfo, SignatureDate.Empty);
        }

        [TestMethod]
        public void NewWithNullName()
        {
            var column = new CremaDataColumn(null);

            Assert.AreNotEqual(Guid.Empty, column.ColumnID);
            Assert.AreEqual(-1, column.Index);
            Assert.AreEqual(string.Empty, column.ColumnName);
            Assert.AreEqual(typeof(string), column.DataType);
            Assert.AreEqual(string.Empty, column.Comment);
            Assert.IsFalse(column.IsKey);
            Assert.IsFalse(column.Unique);
            Assert.IsFalse(column.ReadOnly);
            Assert.AreEqual(null, column.Table);
            Assert.AreEqual(TagInfo.All, column.Tags);
            Assert.AreEqual(TagInfo.All, column.DerivedTags);
            Assert.AreEqual(string.Empty, column.Validation);
            Assert.AreEqual(string.Empty, column.Expression);
            Assert.IsNull(column.CremaType);
            Assert.IsTrue(column.AllowDBNull);
            Assert.IsFalse(column.AutoIncrement);
            Assert.AreEqual(DBNull.Value, column.DefaultValue);

            Assert.AreEqual(column.CreationInfo, SignatureDate.Empty);
            Assert.AreEqual(column.ModificationInfo, SignatureDate.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewWithName_Fail()
        {
            var columnName = RandomUtility.NextInvalidIdentifier();
            new CremaDataColumn(columnName);
        }

        [TestMethod]
        public void NewWithNameAndType()
        {
            var columnName = RandomUtility.NextIdentifier();
            var columnType = CremaDataTypeUtility.GetBaseTypes().Random();
            var column = new CremaDataColumn(columnName, columnType);

            Assert.AreNotEqual(Guid.Empty, column.ColumnID);
            Assert.AreEqual(-1, column.Index);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(columnType, column.DataType);
            Assert.AreEqual(string.Empty, column.Comment);
            Assert.IsFalse(column.IsKey);
            Assert.IsFalse(column.Unique);
            Assert.IsFalse(column.ReadOnly);
            Assert.AreEqual(null, column.Table);
            Assert.AreEqual(TagInfo.All, column.Tags);
            Assert.AreEqual(TagInfo.All, column.DerivedTags);
            Assert.AreEqual(string.Empty, column.Validation);
            Assert.AreEqual(string.Empty, column.Expression);
            Assert.IsNull(column.CremaType);
            Assert.IsTrue(column.AllowDBNull);
            Assert.IsFalse(column.AutoIncrement);
            Assert.AreEqual(DBNull.Value, column.DefaultValue);

            Assert.AreEqual(column.CreationInfo, SignatureDate.Empty);
            Assert.AreEqual(column.ModificationInfo, SignatureDate.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewWithNameAndInvalidType_Fail()
        {
            var columnName = RandomUtility.NextIdentifier();
            var columnType = typeof(string).Assembly.GetTypes().Random(item => CremaDataTypeUtility.IsBaseType(item) == false);
            new CremaDataColumn(columnName, columnType);
        }

        [TestMethod]
        public void NewWithNullNameAndType()
        {
            var columnType = CremaDataTypeUtility.GetBaseTypes().Random();
            var column = new CremaDataColumn(null, columnType);

            Assert.AreNotEqual(Guid.Empty, column.ColumnID);
            Assert.AreEqual(-1, column.Index);
            Assert.AreEqual(string.Empty, column.ColumnName);
            Assert.AreEqual(columnType, column.DataType);
            Assert.AreEqual(string.Empty, column.Comment);
            Assert.IsFalse(column.IsKey);
            Assert.IsFalse(column.Unique);
            Assert.IsFalse(column.ReadOnly);
            Assert.AreEqual(null, column.Table);
            Assert.AreEqual(TagInfo.All, column.Tags);
            Assert.AreEqual(TagInfo.All, column.DerivedTags);
            Assert.AreEqual(string.Empty, column.Validation);
            Assert.AreEqual(string.Empty, column.Expression);
            Assert.IsNull(column.CremaType);
            Assert.IsTrue(column.AllowDBNull);
            Assert.IsFalse(column.AutoIncrement);
            Assert.AreEqual(DBNull.Value, column.DefaultValue);

            Assert.AreEqual(column.CreationInfo, SignatureDate.Empty);
            Assert.AreEqual(column.ModificationInfo, SignatureDate.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewWithNullNameAndInvalidType_Fail()
        {
            var columnType = typeof(string).Assembly.GetTypes().Random(item => CremaDataTypeUtility.IsBaseType(item) == false);
            new CremaDataColumn(null, columnType);
        }

        [TestMethod]
        public void NewWithNameAndCremaType()
        {
            var dataSet = new CremaDataSet();
            var columnName = RandomUtility.NextIdentifier();
            var columnType = dataSet.AddRandomType();
            var column = new CremaDataColumn(columnName, columnType);

            Assert.AreNotEqual(Guid.Empty, column.ColumnID);
            Assert.AreEqual(-1, column.Index);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(string), column.DataType);
            Assert.AreEqual(string.Empty, column.Comment);
            Assert.IsFalse(column.IsKey);
            Assert.IsFalse(column.Unique);
            Assert.IsFalse(column.ReadOnly);
            Assert.AreEqual(null, column.Table);
            Assert.AreEqual(TagInfo.All, column.Tags);
            Assert.AreEqual(TagInfo.All, column.DerivedTags);
            Assert.AreEqual(string.Empty, column.Validation);
            Assert.AreEqual(string.Empty, column.Expression);
            Assert.AreEqual(columnType, column.CremaType);
            Assert.IsTrue(column.AllowDBNull);
            Assert.IsFalse(column.AutoIncrement);
            Assert.AreEqual(DBNull.Value, column.DefaultValue);

            Assert.AreEqual(column.CreationInfo, SignatureDate.Empty);
            Assert.AreEqual(column.ModificationInfo, SignatureDate.Empty);
            Assert.AreEqual(0, columnType.ReferencedColumns.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewWithNameAndCremaTypeNotInDataSet()
        {
            var columnName = RandomUtility.NextIdentifier();
            var columnType = CremaDataTypeExtensions.CreateRandomType();
            new CremaDataColumn(columnName, columnType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NewWithNameAndNullCremaType()
        {
            var columnName = RandomUtility.NextIdentifier();
            new CremaDataColumn(columnName, null as CremaDataType);
        }
    }
}
