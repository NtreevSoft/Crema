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
using Ntreev.Crema.Data.Xml;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaDataColumn_MemberTest
    {
        private CremaDataColumn column = new CremaDataColumn();

        public CremaDataColumn_MemberTest()
        {

        }

        [TestMethod]
        public void SetNullName()
        {
            this.column.ColumnName = RandomUtility.NextIdentifier();
            this.column.ColumnName = null;
            Assert.AreEqual(string.Empty, this.column.ColumnName);
        }

        [TestMethod]
        public void SetName()
        {
            var newName = RandomUtility.NextIdentifier();
            this.column.ColumnName = newName;
            Assert.AreEqual(newName, this.column.ColumnName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetInvalidName_Fail()
        {
            var newName = RandomUtility.NextInvalidIdentifier();
            this.column.ColumnName = newName;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetCremaTypeNotInDataSet_Fail()
        {
            var columnType = CremaDataTypeExtensions.CreateRandomType();
            this.column.CremaType = columnType;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetCremaTypeNotInDataSetToNotStringTypeColumn_Fail()
        {
            var columnType = CremaDataTypeExtensions.CreateRandomType();
            this.column.DataType = typeof(int);
            this.column.CremaType = columnType;
        }

        [TestMethod]
        public void SetCremaType()
        {
            var dataSet = new CremaDataSet();
            var columnType = dataSet.AddRandomType();
            this.column.InitializeRandom();
            this.column.CremaType = columnType;

            Assert.AreEqual(typeof(string), this.column.DataType);
            Assert.AreEqual(DBNull.Value, this.column.DefaultValue);
            Assert.AreEqual(false, this.column.AutoIncrement);
            Assert.AreEqual(columnType, this.column.CremaType);
        }

        [TestMethod]
        public void SetCremaTypeToNotStringTypeColumn()
        {
            var dataSet = new CremaDataSet();
            var columnType = dataSet.AddRandomType();
            this.column.DataType = typeof(int);
            this.column.CremaType = columnType;

            Assert.AreEqual(typeof(string), this.column.DataType);
            Assert.AreEqual(columnType, this.column.CremaType);
            Assert.AreEqual(DBNull.Value, this.column.DefaultValue);
        }

        [TestMethod]
        public void SetNullCremaTypeToCremaTypeColumn()
        {
            var dataSet = new CremaDataSet();
            var columnType = dataSet.AddRandomType();
            this.column.CremaType = columnType;
            this.column.CremaType = null;

            Assert.AreEqual(typeof(string), this.column.DataType);
            Assert.IsNull(this.column.CremaType);
        }

        [TestMethod]
        public void SetKeyTrue()
        {
            this.column.IsKey = true;
            Assert.AreEqual(true, this.column.IsKey);
        }

        [TestMethod]
        public void SetKeyFalse()
        {
            this.column.IsKey = true;
            this.column.IsKey = false;
            Assert.AreEqual(false, this.column.IsKey);
        }

        [TestMethod]
        public void SetUniqueTrue()
        {
            this.column.Unique = true;
            Assert.AreEqual(true, this.column.Unique);
        }

        [TestMethod]
        public void SetUniqueFalse()
        {
            this.column.Unique = true;
            this.column.Unique = false;
            Assert.AreEqual(false, this.column.Unique);
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
        public void SetReadOnlyTrue()
        {
            this.column.ReadOnly = true;
            Assert.AreEqual(true, this.column.ReadOnly);
        }

        [TestMethod]
        public void SetReadOnlyFalse()
        {
            this.column.ReadOnly = true;
            this.column.ReadOnly = false;
            Assert.AreEqual(false, this.column.ReadOnly);
        }

        [TestMethod]
        public void SetDefaultValue()
        {
            this.column.DataType = CremaDataTypeUtility.GetBaseTypes().Random();
            var newDefaultValue = RandomUtility.Next(this.column.DataType);
            this.column.DefaultValue = newDefaultValue;
            Assert.AreEqual(newDefaultValue, this.column.DefaultValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetDefaultValueToAutoIncrementColumn_Fail()
        {
            this.column.DataType = CremaDataTypeUtility.GetBaseTypes().Random(item => CremaDataTypeUtility.CanUseAutoIncrement(item));
            this.column.AutoIncrement = true;
            var newDefaultValue = RandomUtility.Next(this.column.DataType);
            this.column.DefaultValue = newDefaultValue;
            Assert.AreEqual(newDefaultValue, this.column.DefaultValue);
        }

        [TestMethod]
        public void SetDefaultValueAsString()
        {
            this.column.DataType = CremaDataTypeUtility.GetBaseTypes().Random();
            var newDefaultValue = RandomUtility.Next(this.column.DataType);
            this.column.DefaultValue = CremaConvert.ToString(newDefaultValue);
            if (this.column.DataType == typeof(DateTime))
                Assert.AreEqual(newDefaultValue.ToString(), this.column.DefaultValue.ToString());
            else
                Assert.AreEqual(newDefaultValue, this.column.DefaultValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetDefaultValue_Fail()
        {
            var type1 = CremaDataTypeUtility.GetBaseTypes().Random(item => item != typeof(string));
            this.column.DataType = type1;
            this.column.DefaultValue = RandomUtility.NextWord();
        }

        [TestMethod]
        public void SetDefaultValueNull()
        {
            this.column.DefaultValue = RandomUtility.NextString();
            this.column.DefaultValue = null;
            Assert.AreEqual(DBNull.Value, this.column.DefaultValue);
        }

        [TestMethod]
        public void SetDefaultValueDBNull()
        {
            this.column.DefaultValue = RandomUtility.NextString();
            this.column.DefaultValue = DBNull.Value;
            Assert.AreEqual(DBNull.Value, this.column.DefaultValue);
        }

        [TestMethod]
        public void SetDefaultValueToCremaTypeColumn()
        {
            var dataSet = new CremaDataSet();
            var columnType = dataSet.AddRandomType();
            var defaultValue = columnType.GetRandomValue();
            this.column.CremaType = columnType;
            this.column.DefaultValue = defaultValue;
            Assert.AreEqual(defaultValue, this.column.DefaultValue);
        }

        [TestMethod]
        public void SetDefaultNumericValueToCremaTypeColumn()
        {
            var dataSet = new CremaDataSet();
            var columnType = dataSet.AddRandomType();
            var member = columnType.Members.Random();
            var defaultValue = columnType.ConvertFromString(member.Name);
            var defaultName = columnType.ConvertToString(defaultValue);
            this.column.CremaType = columnType;
            this.column.DefaultValue = defaultValue;
            Assert.AreEqual(defaultName, this.column.DefaultValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetDefaultValueToCremaTypeColumn_Fail()
        {
            var dataSet = new CremaDataSet();
            var columnType = dataSet.AddRandomType();
            this.column.CremaType = columnType;
            this.column.DefaultValue = RandomUtility.NextString();
        }

        [TestMethod]
        public void SetDefaultValueDBNullToCremaTypeColumn()
        {
            var dataSet = new CremaDataSet();
            var columnType = dataSet.AddRandomType();
            this.column.CremaType = columnType;
            this.column.DefaultValue = DBNull.Value;
            Assert.AreEqual(DBNull.Value, this.column.DefaultValue);
        }

        [TestMethod]
        public void SetDefaultValueNullToCremaTypeColumn()
        {
            var dataSet = new CremaDataSet();
            var columnType = dataSet.AddRandomType();
            this.column.CremaType = columnType;
            this.column.DefaultValue = null;
            Assert.AreEqual(DBNull.Value, this.column.DefaultValue);
        }

        [TestMethod]
        public void SetAutoIncrementTrue()
        {
            this.column.DataType = CremaDataTypeUtility.GetBaseTypes().Random(item => CremaDataTypeUtility.CanUseAutoIncrement(item));
            this.column.AutoIncrement = true;
            Assert.AreEqual(true, this.column.AutoIncrement);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetAutoIncrementTrueToDefaultValueColumn_Fail()
        {
            this.column.DataType = CremaDataTypeUtility.GetBaseTypes().Random(item => CremaDataTypeUtility.CanUseAutoIncrement(item));
            this.column.DefaultValue = RandomUtility.Next(this.column.DataType);
            this.column.AutoIncrement = true;
        }

        [TestMethod]
        public void SetAutoIncrementFalse()
        {
            this.column.DataType = CremaDataTypeUtility.GetBaseTypes().Random(item => CremaDataTypeUtility.CanUseAutoIncrement(item));
            this.column.AutoIncrement = true;
            this.column.AutoIncrement = false;
            Assert.AreEqual(false, this.column.AutoIncrement);
        }

        [TestMethod]
        public void SetAutoIncrementToInvalidIncrementType()
        {
            this.column.DataType = CremaDataTypeUtility.GetBaseTypes().Random(item => CremaDataTypeUtility.CanUseAutoIncrement(item) == false);
            this.column.AutoIncrement = true;
            Assert.AreEqual(typeof(int), this.column.DataType);
            Assert.AreEqual(true, this.column.AutoIncrement);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetAutoIncrementToCremaTypeColumn_Fail()
        {
            var dataSet = new CremaDataSet();
            var columnType = dataSet.AddRandomType();
            this.column.CremaType = columnType;
            this.column.AutoIncrement = true;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetOrdinal()
        {
            this.column.Index = 0;
        }
    }
}