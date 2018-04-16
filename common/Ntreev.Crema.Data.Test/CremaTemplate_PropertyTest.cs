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
using Ntreev.Library.IO;
using Ntreev.Crema.Data.Xml.Schema;
using System.Threading;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaTemplate_PropertyTest
    {
        private CremaDataTable table;
        private CremaTemplate template;

        public CremaTemplate_PropertyTest()
        {
            this.table = Random.CremaDataTableExtensions.CreateRandomTable();
            this.template = new CremaTemplate(this.table);
        }

        [TestMethod]
        public void SetName()
        {
            var tableName = RandomUtility.NextIdentifier();
            var modificationInfo = this.template.ModificationInfo;
            Thread.Sleep(1000);
            this.template.TableName = tableName;
            Assert.AreEqual(tableName, this.template.TableName);
            Assert.AreNotEqual(modificationInfo.DateTime, this.template.ModificationInfo.DateTime);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetInvalidName()
        {
            var tableName = RandomUtility.NextInvalidIdentifier();
            this.template.TableName = tableName;
        }

        [TestMethod]
        public void SetComment()
        {
            var comment = RandomUtility.NextString();
            var modificationInfo = this.template.ModificationInfo;
            Thread.Sleep(100);
            this.template.Comment = comment;
            Assert.AreEqual(comment, this.template.Comment);
            Assert.AreNotEqual(modificationInfo.DateTime, this.template.ModificationInfo.DateTime);
        }

        [TestMethod]
        public void SetNullComment()
        {
            var comment = RandomUtility.NextString();
            this.template.Comment = comment;
            this.template.Comment = null;
            Assert.AreEqual(string.Empty, this.template.Comment);
        }

        [TestMethod]
        public void SetTags()
        {
            var tags = RandomUtility.NextTags();
            var modificationInfo = this.template.ModificationInfo;
            Thread.Sleep(1000);
            if (this.template.Tags != tags)
            {
                this.template.Tags = tags;
                Assert.AreEqual(tags, this.template.Tags);
                Assert.IsTrue(this.template.ModificationInfo.DateTime - modificationInfo.DateTime > new TimeSpan(1000));
            }
        }
    }
}