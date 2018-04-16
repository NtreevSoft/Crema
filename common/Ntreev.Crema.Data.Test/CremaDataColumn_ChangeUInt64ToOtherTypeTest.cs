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
    public class CremaDataColumn_ChangeUInt64ToOtherTypeTest : CremaDataColumn_ChangeTypeTestBase
    {
        private const ulong maxOADateValue = 2958465;

        public CremaDataColumn_ChangeUInt64ToOtherTypeTest()
            : base(typeof(ulong))
        {

        }

        [TestMethod]
        public void DBNullUInt64ToOther()
        {
            this.AddRows(DBNull.Value);
            foreach (var item in CremaDataTypeUtility.GetBaseTypes().Where(item => item != this.column.DataType))
            {
                try
                {
                    column.DataType = typeof(ulong);
                }
                catch (FormatException)
                {

                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void UInt64ToBoolean_Fail()
        {
            this.AddRows((ulong)0);
            column.DataType = typeof(bool);
        }

        [TestMethod]
        public void UInt64ToSingle()
        {
            this.AddRows(ulong.MinValue, ulong.MaxValue);
            column.DataType = typeof(float);
        }

        [TestMethod]
        public void UInt64ToDouble()
        {
            this.AddRows(ulong.MinValue, ulong.MaxValue);
            column.DataType = typeof(double);
        }

        [TestMethod]
        public void UInt64ToInt8()
        {
            this.AddRows((ulong)0, (ulong)sbyte.MaxValue);
            column.DataType = typeof(sbyte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void UInt64ToInt8_Fail()
        {
            this.AddRows(ulong.MaxValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        public void UInt64ToUInt8()
        {
            this.AddRows((ulong)byte.MinValue, (ulong)byte.MaxValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void UInt64ToUInt8_Fail()
        {
            this.AddRows(ulong.MaxValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        public void UInt64ToInt16()
        {
            this.AddRows(ulong.MinValue, (ulong)short.MaxValue);
            column.DataType = typeof(short);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void UInt64ToInt16_Fail()
        {
            this.AddRows(ulong.MaxValue);
            column.DataType = typeof(short);
        }

        [TestMethod]
        public void UInt64ToInt32()
        {
            this.AddRows(ulong.MinValue, (ulong)int.MaxValue);
            column.DataType = typeof(int);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void UInt64ToInt32_Fail()
        {
            this.AddRows(ulong.MaxValue);
            column.DataType = typeof(int);
        }

        [TestMethod]
        public void UInt64ToInt64()
        {
            this.AddRows((ulong)0, (ulong)long.MaxValue);
            column.DataType = typeof(long);
        }

        [TestMethod]
        public void UInt64ToUInt64()
        {
            this.AddRows(ulong.MinValue, ulong.MaxValue);
            column.DataType = typeof(ulong);
        }

        [TestMethod]
        public void UInt64ToDateTime()
        {
            this.AddRows((ulong)0, maxOADateValue);
            column.DataType = typeof(DateTime);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void UInt64ToDateTime_Fail()
        {
            this.AddRows(ulong.MaxValue);
            column.DataType = typeof(DateTime);
        }

        [TestMethod]
        public void UInt64ToTimeSpan()
        {
            this.AddRows((ulong)0, (ulong)TimeSpan.MaxValue.TotalSeconds);
            column.DataType = typeof(TimeSpan);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void UInt64ToTimeSpan_Fail()
        {
            this.AddRows(ulong.MaxValue);
            column.DataType = typeof(TimeSpan);
        }

        [TestMethod]
        public void UInt64ToString()
        {
            this.AddRows(ulong.MinValue, ulong.MaxValue);
            column.DataType = typeof(string);
        }
    }
}
