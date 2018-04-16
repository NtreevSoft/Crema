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
    public class CremaDataColumn_ChangeInt16ToOtherTypeTest : CremaDataColumn_ChangeTypeTestBase
    {
        public CremaDataColumn_ChangeInt16ToOtherTypeTest()
            : base(typeof(short))
        {

        }

        [TestMethod]
        public void DBNullInt16ToOther()
        {
            this.AddRows(DBNull.Value);
            foreach (var item in CremaDataTypeUtility.GetBaseTypes().Where(item => item != this.column.DataType))
            {
                try
                {
                    column.DataType = typeof(short);
                }
                catch (FormatException)
                {

                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int16ToBoolean_Fail()
        {
            this.AddRows((short)0);
            column.DataType = typeof(bool);
        }

        [TestMethod]
        public void Int16ToSingle()
        {
            this.AddRows(short.MinValue, short.MaxValue);
            column.DataType = typeof(float);
        }

        [TestMethod]
        public void Int16ToDouble()
        {
            this.AddRows(short.MinValue, short.MaxValue);
            column.DataType = typeof(double);
        }

        [TestMethod]
        public void Int16ToInt8()
        {
            this.AddRows((short)sbyte.MinValue, (short)sbyte.MaxValue);
            column.DataType = typeof(sbyte);
        }

        [TestMethod]
        public void Int16ToUInt8()
        {
            this.AddRows((short)byte.MinValue, (short)byte.MaxValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int16ToUInt8_Fail1()
        {
            this.AddRows(short.MinValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int16ToUInt8_Fail2()
        {
            this.AddRows(short.MaxValue);
            column.DataType = typeof(byte);
        }

        [TestMethod]
        public void Int16ToUInt16()
        {
            this.AddRows((short)0, short.MaxValue);
            column.DataType = typeof(ushort);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int16ToUInt16_Fail()
        {
            this.AddRows(short.MinValue);
            column.DataType = typeof(ushort);
        }

        [TestMethod]
        public void Int16ToInt32()
        {
            this.AddRows(short.MinValue, short.MaxValue);
            column.DataType = typeof(int);
        }

        [TestMethod]
        public void Int16ToUInt32()
        {
            this.AddRows((short)0, short.MaxValue);
            column.DataType = typeof(uint);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int16ToUInt32_Fail()
        {
            this.AddRows(short.MinValue);
            column.DataType = typeof(uint);
        }

        [TestMethod]
        public void Int16ToInt64()
        {
            this.AddRows(short.MinValue, short.MaxValue);
            column.DataType = typeof(long);
        }

        [TestMethod]
        public void Int16ToUInt64()
        {
            this.AddRows((short)0, short.MaxValue);
            column.DataType = typeof(ulong);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Int16ToUInt64_Fail()
        {
            this.AddRows(short.MinValue);
            column.DataType = typeof(ulong);
        }

        [TestMethod]
        public void Int16ToDateTime()
        {
            this.AddRows(short.MinValue, short.MaxValue);
            column.DataType = typeof(DateTime);
        }

        [TestMethod]
        public void Int16ToTimeSpan()
        {
            this.AddRows(short.MinValue, short.MaxValue);
            column.DataType = typeof(TimeSpan);
        }

        [TestMethod]
        public void Int16ToString()
        {
            this.AddRows(short.MinValue, short.MaxValue);
            column.DataType = typeof(string);
        }
    }
}
