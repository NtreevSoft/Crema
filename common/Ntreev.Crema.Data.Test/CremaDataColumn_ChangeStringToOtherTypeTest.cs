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
using System.Globalization;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaDataColumn_ChangeStringToOtherTypeTest : CremaDataColumn_ChangeTypeTestBase
    {
        public CremaDataColumn_ChangeStringToOtherTypeTest()
            : base(typeof(string))

        {

        }

        [TestMethod]
        public void EmptyStringToOther()
        {
            this.AddRows("");
            foreach (var item in CremaDataTypeUtility.GetBaseTypes().Where(item => item != this.column.DataType))
            {
                try
                {
                    column.DataType = typeof(float);
                }
                catch (FormatException)
                {

                }
            }
        }

        [TestMethod]
        public void NullStringToOther()
        {
            this.AddRows(new object[] { null });
            foreach (var item in CremaDataTypeUtility.GetBaseTypes().Where(item => item != this.column.DataType))
            {
                try
                {
                    column.DataType = typeof(float);
                }
                catch (FormatException)
                {

                }
            }
        }

        [TestMethod]
        public void StringToBoolean()
        {
            this.AddRows("true", "false", "True", "False");
            column.DataType = typeof(bool);
        }

        [TestMethod]
        public void StringToSingle()
        {
            var value1 = float.MinValue.ToString("R");
            var value2 = float.MaxValue.ToString("R");
            this.AddRows(value1, value2);
            column.DataType = typeof(float);
        }

        [TestMethod]
        public void StringToDouble()
        {
            var value1 = double.MinValue.ToString("R");
            var value2 = double.MaxValue.ToString("R");
            this.AddRows(value1, value2);
            column.DataType = typeof(double);
        }

        [TestMethod]
        public void StringToInt8()
        {
            this.AddRows("-128", "127");
            column.DataType = typeof(sbyte);
        }

        [TestMethod]
        public void StringToUInt8()
        {
            this.AddRows("0", "255");
            column.DataType = typeof(byte);
        }

        [TestMethod]
        public void StringToInt16()
        {
            this.AddRows("-32768", "32767");
            column.DataType = typeof(short);
        }

        [TestMethod]
        public void StringToUInt16()
        {
            this.AddRows("0", "65535");
            column.DataType = typeof(ushort);
        }

        [TestMethod]
        public void StringToInt32()
        {
            this.AddRows("-2147483648", "2147483647");
            column.DataType = typeof(int);
        }

        [TestMethod]
        public void StringToUInt32()
        {
            this.AddRows("0", "4294967295");
            column.DataType = typeof(uint);
        }

        [TestMethod]
        public void StringToInt64()
        {
            this.AddRows("-9223372036854775808", "9223372036854775807");
            column.DataType = typeof(long);
        }

        [TestMethod]
        public void StringToUInt64()
        {
            this.AddRows("0", "18446744073709551615");
            column.DataType = typeof(ulong);
        }

        [TestMethod]
        public void StringToDateTime()
        {
            var value1 = DateTime.MinValue.ToOADate().ToString();
            var value2 = DateTime.MaxValue.ToOADate().ToString();
            var value3 = RandomUtility.Next<DateTime>().ToOADate().ToString();
            this.AddRows(value1, value2, value3);
            column.DataType = typeof(DateTime);
        }

        [TestMethod]
        public void StringToTimeSpan()
        {
            var value1 = TimeSpan.MinValue.TotalSeconds.ToString("R");
            var value2 = TimeSpan.MaxValue.TotalSeconds.ToString("R");
            var value3 = RandomUtility.Next<TimeSpan>().TotalSeconds.ToString("R");
            this.AddRows(value1, value2, value3);
            column.DataType = typeof(TimeSpan);
        }
    }
}
