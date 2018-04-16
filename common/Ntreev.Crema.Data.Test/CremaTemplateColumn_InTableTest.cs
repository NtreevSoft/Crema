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
using Ntreev.Crema.Data.Xml;
using System.Data;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaTemplateColumn_InTableTest
    {
        private CremaTemplate template;
        private CremaTemplateColumn column;
        private CremaDataColumn targetColumn;

        public CremaTemplateColumn_InTableTest()
        {
            var dataSet = CremaDataSetExtensions.CreateRandomSet();
            this.template = new CremaTemplate(dataSet.Tables.Random(item => item.TemplateNamespace == string.Empty));
            this.column = this.template.Columns.Random();
            this.targetColumn = this.template.TargetTable.Columns[this.column.Name];
        }

        [TestMethod]
        public void Test()
        {
            Assert.AreEqual(this.targetColumn.Index, this.column.Index);
            Assert.AreEqual(this.targetColumn.IsKey , this.column.IsKey);
            Assert.AreEqual(this.targetColumn.Unique , this.column.Unique);
            Assert.AreEqual(this.targetColumn.ColumnName , this.column.Name);
            Assert.AreEqual(this.targetColumn.DataTypeName , this.column.DataTypeName);
            Assert.AreEqual(this.targetColumn.AutoIncrement , this.column.AutoIncrement);
            Assert.AreEqual(this.targetColumn.Comment , this.column.Comment);
            Assert.AreEqual(this.targetColumn.Tags , this.column.Tags);
            Assert.AreEqual(this.targetColumn.CreationInfo, this.column.CreationInfo);
            Assert.AreEqual(this.targetColumn.ModificationInfo, this.column.ModificationInfo);
        }
    }
}