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
    public class CremaDataColumn_ChangeDoubleToOtherTypeTest : CremaDataColumn_ChangeTypeTestBase
    {
        private const double minIntValue = -9999999.0f;
        private const double maxIntValue = 9999999.0f;
        private const double minLongValue = -999999999999999.0;
        private const double maxLongValue = 999999999999999.0;
        private static readonly double maxOADateValue = DateTime.MaxValue.ToOADate();

        public CremaDataColumn_ChangeDoubleToOtherTypeTest()
            : base(typeof(double))
        {

        }

        [TestMethod]
        public void DBNullDoubleToOther()
        {
            this.AddRows(DBNull.Value);
            foreach (var item in CremaDataTypeUtility.GetBaseTypes().Where(item => item != this.column.DataType))
            {
                try
                {
                    column.DataType = typeof(double);
                }
                catch (FormatException)
                {

                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DoubleToBoolean_Fail()
        {
            this.AddRows(0.0);
            column.DataType = typeof(bool);
        }

        [TestMethod]
        public void DoubleToSingle()
        {
            var value1 = Convert.ChangeType(float.MinValue, typeof(double));
            var value2 = Convert.ChangeType(float.MaxValue, typeof(double));
            var value3 = Convert.ChangeType(RandomUtility.Next<float>(), typeof(double));
            this.AddRows(value1, value2, value3);
            column.DataType = typeof(float);
        }

        [TestMethod]
        public void DoubleToInt8()
        {
            this.AddRows(-128.0, 127.0);
            column.DataType = typeof(sbyte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DoubleToInt8_Fail()
        {
            this.AddRows(double.MaxValue);
            column.DataType = typeof(sbyte);
        }

        [TestMethod]
        public void DoubleToUInt8()
        {
            this.AddRows(0.0, 255.0);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DoubleToUInt8_Fail()
        {
            this.AddRows(double.MaxValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        public void DoubleToInt16()
        {
            this.AddRows(-32768.0, 32767.0);
            column.DataType = typeof(short);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DoubleToInt16_Fail()
        {
            this.AddRows(double.MaxValue);
            column.DataType = typeof(short);
        }

        [TestMethod]
        public void DoubleToUInt16()
        {
            this.AddRows(0.0, 65535.0);
            column.DataType = typeof(ushort);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DoubleToUInt16_Fail()
        {
            this.AddRows(double.MinValue, double.MaxValue, RandomUtility.Next(typeof(double)));
            column.DataType = typeof(ushort);
        }

        [TestMethod]
        public void DoubleToInt32()
        {
            this.AddRows(minIntValue, maxIntValue);
            column.DataType = typeof(int);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DoubleToInt32Fail()
        {
            this.AddRows(maxLongValue + 1.0);
            column.DataType = typeof(int);
        }

        [TestMethod]
        public void DoubleToUInt32()
        {
            this.AddRows(0.0, maxIntValue);
            column.DataType = typeof(uint);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DoubleToUInt32_Fail()
        {
            this.AddRows(maxLongValue + 1.0);
            column.DataType = typeof(uint);
        }

        [TestMethod]
        public void DoubleToInt64()
        {
            this.AddRows(minLongValue, maxLongValue);
            column.DataType = typeof(long);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DoubleToInt64_Fail()
        {
            this.AddRows(maxLongValue + 1.0);
            column.DataType = typeof(long);
        }

        [TestMethod]
        public void DoubleToUInt64()
        {
            this.AddRows(0.0, maxLongValue);
            column.DataType = typeof(ulong);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DoubleToUInt64_Fail()
        {
            this.AddRows(double.MinValue, double.MaxValue, RandomUtility.Next(typeof(double)));
            column.DataType = typeof(ulong);
        }

        [TestMethod]
        public void DoubleToDateTime()
        {
            this.AddRows(0.0, maxOADateValue);
            column.DataType = typeof(DateTime);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DoubleToDateTime_Fail()
        {
            this.AddRows(maxLongValue + 1.0);
            column.DataType = typeof(DateTime);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DoubleToTimeSpan()
        {
            this.AddRows(minLongValue, maxLongValue);
            column.DataType = typeof(TimeSpan);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DoubleToTimeSpan_Fail()
        {
            this.AddRows(maxLongValue + 1.0);
            column.DataType = typeof(TimeSpan);
        }

        [TestMethod]
        public void DoubleToString()
        {
            this.AddRows(double.MinValue, double.MaxValue, RandomUtility.Next(typeof(double)));
            column.DataType = typeof(string);
        }
    }
}
