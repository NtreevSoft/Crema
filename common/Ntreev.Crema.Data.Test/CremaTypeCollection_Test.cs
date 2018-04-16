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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ntreev.Crema.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;
using Ntreev.Library.IO;
using System.Xml.Serialization;
using System.Data;
using Ntreev.Crema.Data.Xml.Schema;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class Test_CremaDataTypeCollection
    {
        [TestMethod]
        public void AddTypes()
        {
            var dataSet = new CremaDataSet();

            var type1 = new CremaDataType("Type1");
            var type2 = new CremaDataType("Type1", "/A/");
            var type3 = new CremaDataType("Type1", "/B/");

            dataSet.Types.Add(type1);
            dataSet.Types.Add(type2);
            dataSet.Types.Add(type3);
        }

        [TestMethod]
        public void AddTypes_Fail()
        {
            var dataSet = new CremaDataSet();
            var type1 = new CremaDataType("Type1");
            var type2 = new CremaDataType("Type1");
            var type3 = new CremaDataType("Type1", "/A/");
            var type4 = new CremaDataType("Type1", "/A/");

            dataSet.Types.Add(type1);
            try
            {
                dataSet.Types.Add(type2);
                Assert.Inconclusive();
            }
            catch
            {

            }

            dataSet.Types.Add(type3);
            try
            {
                dataSet.Types.Add(type4);
                Assert.Inconclusive();
            }
            catch
            {

            }
        }

        [TestMethod]
        public void AddTypesByName()
        {
            var dataSet = new CremaDataSet();

            dataSet.Types.Add("Type1");
            dataSet.Types.Add("Type1", "/A/");
            dataSet.Types.Add("Type1", "/B/");
        }

        [TestMethod]
        public void AddTypesByName_Fail()
        {
            var dataSet = new CremaDataSet();

            dataSet.Types.Add("Type1");
            try
            {
                dataSet.Types.Add("Type1");
                Assert.Inconclusive();
            }
            catch
            {

            }

            dataSet.Types.Add("Type1", "/A/");
            try
            {
                dataSet.Types.Add("Type1", "/A/");
                Assert.Inconclusive();
            }
            catch
            {

            }

            dataSet.Types.Add("Type1", "/B/");
            try
            {
                dataSet.Types.Add("Type1", "/B/");
                Assert.Inconclusive();
            }
            catch
            {

            }
        }

        [TestMethod]
        public void Indexer()
        {
            CremaDataSet dataSet = new CremaDataSet();

            var type1 = dataSet.Types.Add("Type1");
            Assert.AreEqual(type1, dataSet.Types["Type1"]);
            Assert.AreEqual(type1, dataSet.Types["Type1", "/"]);

            var type2 = dataSet.Types.Add("Type1", "/A/");
            Assert.AreEqual(type2, dataSet.Types["Type1", "/A/"]);

            var type3 = dataSet.Types.Add("Type1", "/B/");
            Assert.AreEqual(type3, dataSet.Types["Type1", "/B/"]);
        }

        [TestMethod]
        public void Indexer_Fail()
        {
            var dataSet = new CremaDataSet();

            var type1 = dataSet.Types.Add("Type1");

            try
            {
                Assert.IsNull(dataSet.Types[null as string]);
                Assert.Inconclusive();
            }
            catch (ArgumentNullException)
            {
            }
            catch
            {
                Assert.Inconclusive();
            }

            try
            {
                Assert.AreEqual(type1, dataSet.Types["Type1", null]);
                Assert.Inconclusive();
            }
            catch (ArgumentNullException)
            {

            }
            catch
            {
                Assert.Inconclusive();
            }
        }

        [TestMethod]
        public void IndexerByIndex()
        {
            CremaDataSet dataSet = new CremaDataSet();

            var type1 = dataSet.Types.Add("Type1");
            var type2 = dataSet.Types.Add("Type1", "/A/");
            var type3 = dataSet.Types.Add("Type1", "/B/");

            Assert.AreEqual(dataSet.Types[0], type1);
            Assert.AreEqual(dataSet.Types[1], type2);
            Assert.AreEqual(dataSet.Types[2], type3);

            dataSet.Types.Remove(type2);

            Assert.AreEqual(dataSet.Types[1], type3);
        }

        [TestMethod]
        public void IndexerByIndex_Fail()
        {
            CremaDataSet dataSet = new CremaDataSet();

            var type1 = dataSet.Types.Add("Type1");
            var type2 = dataSet.Types.Add("Type1", "/A/");
            var type3 = dataSet.Types.Add("Type1", "/B/");

            try
            {
                var sss = dataSet.Types[-1];
                Assert.Inconclusive();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
            catch
            {
                Assert.Inconclusive();
            }

            try
            {
                var sss = dataSet.Types[3];
                Assert.Inconclusive();
            }
            catch (ArgumentOutOfRangeException)
            {

            }
            catch
            {
                Assert.Inconclusive();
            }
        }

        [TestMethod]
        public void Contains()
        {
            CremaDataSet dataSet = new CremaDataSet();

            Assert.IsFalse(dataSet.Types.Contains("Type1"));
            Assert.IsFalse(dataSet.Types.Contains("Type1", "/A/"));
            Assert.IsFalse(dataSet.Types.Contains("Type1", "/B/"));

            var type1 = dataSet.Types.Add("Type1");
            Assert.IsTrue(dataSet.Types.Contains("Type1"));
            Assert.IsTrue(dataSet.Types.Contains("Type1", "/"));

            Assert.IsFalse(dataSet.Types.Contains("Type1", "/A/"));
            Assert.IsFalse(dataSet.Types.Contains("Type1", "/B/"));

            var type2 = dataSet.Types.Add("Type1", "/A/");
            Assert.IsTrue(dataSet.Types.Contains("Type1", "/A/"));

            var type3 = dataSet.Types.Add("Type1", "/B/");
            Assert.IsTrue(dataSet.Types.Contains("Type1", "/B/"));
        }

        [TestMethod]
        public void Contains_Fail()
        {
            var dataSet = new CremaDataSet();
            var type1 = dataSet.Types.Add("Type1");

            Assert.IsFalse(dataSet.Types.Contains("Type1", "/A/"));
        }

        [TestMethod]
        public void Remove()
        {
            var dataSet = new CremaDataSet();

            var type1 = dataSet.Types.Add("Type1");
            var type2 = dataSet.Types.Add("Type1", "/A/");
            var type3 = dataSet.Types.Add("Type1", "/B/");

            dataSet.Types.Remove(type1);
            Assert.IsNull(type1.DataSet);

            dataSet.Types.Remove(type2);
            Assert.IsNull(type2.DataSet);

            dataSet.Types.Remove(type3);
            Assert.IsNull(type3.DataSet);

            Assert.AreEqual(dataSet.Types.Count, 0);
        }

        [TestMethod]
        public void Remove_Fail()
        {
            CremaDataSet dataSet = new CremaDataSet();

            var type1 = dataSet.Types.Add("Type1");
            var type2 = dataSet.Types.Add("Type1", "/A/");
            var type3 = dataSet.Types.Add("Type1", "/B/");

            try
            {
                dataSet.Types.Remove(null as CremaDataType);
                Assert.Inconclusive();
            }
            catch (ArgumentNullException)
            {

            }
            catch
            {
                Assert.Inconclusive();
            }

            dataSet.Types.Remove(type3);
            Assert.IsNull(type3.DataSet);

            try
            {
                dataSet.Types.Remove(type3);
                Assert.Inconclusive();
            }
            catch
            {

            }

            dataSet.Types.Remove(type2);
            Assert.IsNull(type2.DataSet);
            dataSet.Types.Remove(type1);
            Assert.IsNull(type1.DataSet);

            Assert.AreEqual(dataSet.Types.Count, 0);
        }

        [TestMethod]
        public void RemoveByName()
        {
            CremaDataSet dataSet = new CremaDataSet();

            var type1 = dataSet.Types.Add("Type1");
            var type2 = dataSet.Types.Add("Type1", "/A/");
            var type3 = dataSet.Types.Add("Type1", "/B/");

            dataSet.Types.Remove("Type1", "/B/");
            Assert.IsNull(type3.DataSet);

            dataSet.Types.Remove("Type1", "/A/");
            Assert.IsNull(type2.DataSet);

            dataSet.Types.Remove("Type1");
            Assert.IsNull(type1.DataSet);

            Assert.AreEqual(dataSet.Types.Count, 0);
        }

        [TestMethod]
        public void RemoveByName_Fail()
        {
            CremaDataSet dataSet = new CremaDataSet();

            var type1 = dataSet.Types.Add("Type1");
            var type2 = dataSet.Types.Add("Type1", "/A/");
            var type3 = dataSet.Types.Add("Type1", "/B/");

            var typeCount = dataSet.Types.Count;

            try
            {
                dataSet.Types.Remove(null as string);
                Assert.Inconclusive();
            }
            catch (ArgumentNullException)
            {

            }
            catch
            {
                Assert.Inconclusive();
            }

            try
            {
                dataSet.Types.Remove("Type123");
                Assert.Inconclusive();
            }
            catch
            {

            }

            try
            {
                dataSet.Types.Remove("Type1");
                Assert.Inconclusive();
            }
            catch
            {

            }
            Assert.IsNotNull(type1.DataSet);

            try
            {
                dataSet.Types.Remove("Type1", null);
                Assert.Inconclusive();
            }
            catch (ArgumentNullException)
            {

            }
            catch
            {
                Assert.Inconclusive();
            }

            Assert.AreEqual(dataSet.Types.Count, typeCount);
        }

        [TestMethod]
        public void CanRemove()
        {
            CremaDataSet dataSet = new CremaDataSet();

            var type1 = dataSet.Types.Add("Type1");
            var type2 = dataSet.Types.Add("Type1", "/A/");
            var type3 = dataSet.Types.Add("Type1", "/B/");

            var type4 = new CremaDataType();

            Assert.IsTrue(dataSet.Types.CanRemove(type1));
            Assert.IsTrue(dataSet.Types.CanRemove(type2));
            Assert.IsTrue(dataSet.Types.CanRemove(type3));

            Assert.IsFalse(dataSet.Types.CanRemove(type4));

            dataSet.Types.Remove(type1);
            dataSet.Types.Remove(type2);
            dataSet.Types.Remove(type3);

            Assert.IsFalse(dataSet.Types.CanRemove(type1));
            Assert.IsFalse(dataSet.Types.CanRemove(type2));
            Assert.IsFalse(dataSet.Types.CanRemove(type3));
        }

        [TestMethod]
        public void CanRemove_Fail()
        {
            CremaDataSet dataSet = new CremaDataSet();

            try
            {
                dataSet.Types.CanRemove(null);
                Assert.Inconclusive();
            }
            catch (ArgumentNullException)
            {

            }
            catch
            {
                Assert.Inconclusive();
            }
        }

        [TestMethod]
        public void Clear()
        {
            CremaDataSet dataSet = new CremaDataSet();

            var type1 = dataSet.Types.Add("Type1");
            var type2 = dataSet.Types.Add("Type1", "/A/");
            var type3 = dataSet.Types.Add("Type1", "/B/");

            dataSet.Types.Clear();
            Assert.IsNull(type1.DataSet);
            Assert.IsNull(type2.DataSet);
            Assert.IsNull(type3.DataSet);

            Assert.AreEqual(dataSet.Types.Count, 0);
        }

        [TestMethod]
        public void ViewSort()
        {
            var dataSet = new CremaDataSet();
            var dataType = dataSet.Types.Add();

            Assert.AreEqual($"{CremaSchema.Index} ASC", dataType.View.Sort);
        }
    }
}