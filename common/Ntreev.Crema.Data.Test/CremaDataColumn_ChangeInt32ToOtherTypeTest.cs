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
    public class CremaDataColumn_ChangeInt32ToOtherTypeTest : CremaDataColumn_ChangeTypeTestBase
    {
        private const int maxOADateValue = 2958465;

        public CremaDataColumn_ChangeInt32ToOtherTypeTest()
            : base(typeof(int))
        {

        }

        [TestMethod]
        public void DBNullInt32ToOther()
        {
            this.AddRows(DBNull.Value);
            foreach (var item in CremaDataTypeUtility.GetBaseTypes().Where(item => item != this.column.DataType))
            {
                try
                {
                    column.DataType = typeof(int);
                }
                catch (FormatException)
                {

                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int32ToBoolean_Fail()
        {
            this.AddRows((int)0);
            column.DataType = typeof(bool);
        }

        [TestMethod]
        public void Int32ToSingle()
        {
            this.AddRows(int.MinValue, int.MaxValue);
            column.DataType = typeof(float);
        }

        [TestMethod]
        public void Int32ToDouble()
        {
            this.AddRows(int.MinValue, int.MaxValue);
            column.DataType = typeof(double);
        }

        [TestMethod]
        public void Int32ToInt8()
        {
            this.AddRows((int)sbyte.MinValue, (int)sbyte.MaxValue);
            column.DataType = typeof(sbyte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int32ToInt8_Fail1()
        {
            this.AddRows(int.MinValue);
            column.DataType = typeof(sbyte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int32ToInt8_Fail2()
        {
            this.AddRows(int.MaxValue);
            column.DataType = typeof(sbyte);
        }

        [TestMethod]
        public void Int32ToUInt8()
        {
            this.AddRows((int)byte.MinValue, (int)byte.MaxValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int32ToUInt8_Fail1()
        {
            this.AddRows(int.MinValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int32ToUInt8_Fail2()
        {
            this.AddRows(int.MaxValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        public void Int32ToUInt16()
        {
            this.AddRows((int)0, (int)ushort.MaxValue);
            column.DataType = typeof(ushort);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int32ToUInt16_Fail()
        {
            this.AddRows(int.MinValue);
            column.DataType = typeof(ushort);
        }

        [TestMethod]
        public void Int32ToUInt32()
        {
            this.AddRows((int)0, (int)ushort.MaxValue);
            column.DataType = typeof(uint);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int32ToUInt32_Fail()
        {
            this.AddRows(int.MinValue);
            column.DataType = typeof(uint);
        }

        [TestMethod]
        public void Int32ToInt64()
        {
            this.AddRows(int.MinValue, int.MaxValue);
            column.DataType = typeof(long);
        }

        [TestMethod]
        public void Int32ToUInt64()
        {
            this.AddRows((int)0, int.MaxValue);
            column.DataType = typeof(ulong);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int32ToUInt64_Fail()
        {
            this.AddRows(int.MinValue);
            column.DataType = typeof(ulong);
        }

        [TestMethod]
        public void Int32ToDateTime()
        {
            this.AddRows((int)0, maxOADateValue);
            column.DataType = typeof(DateTime);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int32ToDateTime_Fail()
        {
            this.AddRows(maxOADateValue + 1);
            column.DataType = typeof(DateTime);
        }

        [TestMethod]
        public void Int32ToTimeSpan()
        {
            this.AddRows(int.MinValue, int.MaxValue);
            column.DataType = typeof(TimeSpan);
        }

        [TestMethod]
        public void Int32ToString()
        {
            this.AddRows(int.MinValue, int.MaxValue);
            column.DataType = typeof(string);
        }
    }
}
