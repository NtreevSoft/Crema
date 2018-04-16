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
using Ntreev.Library.Linq;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaDataColumn_ChangeInt8ToOtherTypeTest : CremaDataColumn_ChangeTypeTestBase
    {
        public CremaDataColumn_ChangeInt8ToOtherTypeTest()
            : base(typeof(sbyte))
        {

        }

        [TestMethod]
        public void DBNullInt8ToOther()
        {
            this.AddRows(DBNull.Value);
            foreach (var item in CremaDataTypeUtility.GetBaseTypes().Where(item => item != this.column.DataType))
            {
                try
                {
                    column.DataType = typeof(sbyte);
                }
                catch (FormatException)
                {

                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int8ToBoolean_Fail()
        {
            this.AddRows((sbyte)0);
            column.DataType = typeof(bool);
        }

        [TestMethod]
        public void Int8ToSingle()
        {
            this.AddRows(sbyte.MinValue, sbyte.MaxValue);
            column.DataType = typeof(float);
        }

        [TestMethod]
        public void Int8ToDouble()
        {
            this.AddRows(sbyte.MinValue, sbyte.MaxValue);
            column.DataType = typeof(double);
        }

        [TestMethod]
        public void Int8ToUInt8()
        {
            this.AddRows((sbyte)0, sbyte.MaxValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int8ToUInt8_Fail()
        {
            this.AddRows(sbyte.MinValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        public void Int8ToInt16()
        {
            this.AddRows(sbyte.MinValue, sbyte.MaxValue);
            column.DataType = typeof(short);
        }

        [TestMethod]
        public void Int8ToUInt16()
        {
            this.AddRows((sbyte)0, sbyte.MaxValue);
            column.DataType = typeof(ushort);
        }

        [TestMethod]
        public void Int8ToInt32()
        {
            this.AddRows(sbyte.MinValue, sbyte.MaxValue);
            column.DataType = typeof(int);
        }

        [TestMethod]
        public void Int8ToUInt32()
        {
            this.AddRows((sbyte)0, sbyte.MaxValue);
            column.DataType = typeof(uint);
        }

        [TestMethod]
        public void Int8ToInt64()
        {
            this.AddRows(sbyte.MinValue, sbyte.MaxValue);
            column.DataType = typeof(long);
        }

        [TestMethod]
        public void Int8ToUInt64()
        {
            this.AddRows((sbyte)0, sbyte.MaxValue);
            column.DataType = typeof(ulong);
        }

        [TestMethod]
        public void Int8ToDateTime()
        {
            this.AddRows(sbyte.MinValue, sbyte.MaxValue);
            column.DataType = typeof(DateTime);
        }

        [TestMethod]
        public void Int8ToTimeSpan()
        {
            this.AddRows(sbyte.MinValue, sbyte.MaxValue);
            column.DataType = typeof(TimeSpan);
        }

        [TestMethod]
        public void Int8ToString()
        {
            this.AddRows(sbyte.MinValue, sbyte.MaxValue);
            column.DataType = typeof(string);
        }
    }
}
