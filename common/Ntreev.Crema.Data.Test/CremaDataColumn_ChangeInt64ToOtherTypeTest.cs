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
    public class CremaDataColumn_ChangeInt64ToOtherTypeTest : CremaDataColumn_ChangeTypeTestBase
    {
        private const long minOADateValue = -(657435 - 1);
        private const long maxOADateValue = (2958466 - 1);

        public CremaDataColumn_ChangeInt64ToOtherTypeTest()
            : base(typeof(long))
        {

        }

        [TestMethod]
        public void DBNullInt64ToOther()
        {
            this.AddRows(DBNull.Value);
            foreach (var item in CremaDataTypeUtility.GetBaseTypes().Where(item => item != this.column.DataType))
            {
                try
                {
                    column.DataType = typeof(long);
                }
                catch (FormatException)
                {

                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int64ToBoolean_Fail()
        {
            this.AddRows((long)0);
            column.DataType = typeof(bool);
        }

        [TestMethod]
        public void Int64ToSingle()
        {
            this.AddRows(long.MinValue, long.MaxValue);
            column.DataType = typeof(float);
        }

        [TestMethod]
        public void Int64ToDouble()
        {
            this.AddRows(long.MinValue, long.MaxValue);
            column.DataType = typeof(double);
        }

        [TestMethod]
        public void Int64ToInt8()
        {
            this.AddRows((long)sbyte.MinValue, (long)sbyte.MaxValue);
            column.DataType = typeof(sbyte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int64ToInt8_Fail1()
        {
            this.AddRows(long.MinValue);
            column.DataType = typeof(sbyte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int64ToInt8_Fail2()
        {
            this.AddRows(long.MaxValue);
            column.DataType = typeof(sbyte);
        }

        [TestMethod]
        public void Int64ToUInt8()
        {
            this.AddRows((long)byte.MinValue, (long)byte.MaxValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int64ToUInt8_Fail1()
        {
            this.AddRows(long.MinValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int64ToUInt8_Fail2()
        {
            this.AddRows(long.MaxValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        public void Int64ToUInt16()
        {
            this.AddRows((long)0, (long)ushort.MaxValue);
            column.DataType = typeof(ushort);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int64ToUInt16_Fail1()
        {
            this.AddRows(long.MinValue);
            column.DataType = typeof(ushort);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int64ToUInt16_Fail2()
        {
            this.AddRows(long.MaxValue);
            column.DataType = typeof(ushort);
        }

        [TestMethod]
        public void Int64ToInt32()
        {
            this.AddRows((long)int.MinValue, (long)int.MaxValue);
            column.DataType = typeof(int);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int64ToInt32_Fail1()
        {
            this.AddRows(long.MinValue);
            column.DataType = typeof(uint);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int64ToInt32_Fail2()
        {
            this.AddRows(long.MaxValue);
            column.DataType = typeof(uint);
        }

        [TestMethod]
        public void Int64ToUInt32()
        {
            this.AddRows((long)0, (long)uint.MaxValue);
            column.DataType = typeof(uint);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int64ToUInt32_Fail()
        {
            this.AddRows(long.MinValue);
            column.DataType = typeof(uint);
        }

        [TestMethod]
        public void Int64ToInt64()
        {
            this.AddRows(long.MinValue, long.MaxValue);
            column.DataType = typeof(long);
        }

        [TestMethod]
        public void Int64ToUInt64()
        {
            this.AddRows((long)0, long.MaxValue);
            column.DataType = typeof(ulong);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int64ToUInt64_Fail()
        {
            this.AddRows(long.MinValue);
            column.DataType = typeof(ulong);
        }

        [TestMethod]
        public void Int64ToDateTime()
        {
            this.AddRows(minOADateValue);
            column.DataType = typeof(DateTime);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int64ToDateTime_Fail1()
        {
            this.AddRows(minOADateValue - 1);
            column.DataType = typeof(DateTime);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int64ToDateTime_Fail2()
        {
            this.AddRows(maxOADateValue + 1);
            column.DataType = typeof(DateTime);
        }

        [TestMethod]
        public void Int64ToTimeSpan1()
        {
            this.AddRows(minOADateValue);
            column.DataType = typeof(TimeSpan);
        }

        [TestMethod]
        public void Int64ToTimeSpan2()
        {
            this.AddRows(maxOADateValue);
            column.DataType = typeof(TimeSpan);
        }

        [TestMethod]
        public void Int64ToString()
        {
            this.AddRows(long.MinValue, long.MaxValue);
            column.DataType = typeof(string);
        }
    }
}
