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
using Ntreev.Crema.Data.Xml.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Test
{
    static class CremaComparer
    {
        public static void CompareDataSet(CremaDataSet dataSet1, CremaDataSet dataSet2)
        {
            if (dataSet1 == null && dataSet2 == null)
                return;
            if (dataSet1 == null || dataSet2 == null)
                Assert.Fail("타입이 같지 않습니다.");

            CompareSchema(dataSet1, dataSet2);
            CompareXml(dataSet1, dataSet2);

            Assert.AreEqual(dataSet1.DataSetName, dataSet2.DataSetName, "DataSetName");
            Assert.AreEqual(dataSet1.Namespace, dataSet2.Namespace, "Namespace");
            Assert.AreEqual(dataSet1.TableNamespace, dataSet2.TableNamespace, "TableNamespace");
            Assert.AreEqual(dataSet1.TypeNamespace, dataSet2.TypeNamespace, "TypeNamespace");

            Assert.AreEqual(dataSet1.Tables.Count, dataSet2.Tables.Count, "Tables.Count");

            for (int i = 0; i < dataSet1.Tables.Count; i++)
            {
                var table1 = dataSet1.Tables[i];
                var table2 = dataSet2.Tables[table1.Name, table1.CategoryPath];
                CremaComparer.CompareTable(table1, table2);
            }

            Assert.AreEqual(dataSet1.Types.Count, dataSet2.Types.Count, "Types.Count");

            for (int i = 0; i < dataSet1.Types.Count; i++)
            {
                var type1 = dataSet1.Types[i];
                var type2 = dataSet2.Types[type1.TypeName];
                CremaComparer.CompareType(type1, type2);
            }
        }

        public static void CompareSchema(CremaDataSet dataSet1, CremaDataSet dataSet2)
        {
            try
            {
                Assert.AreEqual(dataSet1.GetXmlSchema(), dataSet2.GetXmlSchema(), "Schema");
            }
            catch
            {
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "dataSet1" + CremaSchema.SchemaExtension), dataSet1.GetXmlSchema());
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "dataSet2" + CremaSchema.SchemaExtension), dataSet2.GetXmlSchema());
                throw;
            }
        }

        public static void CompareXml(CremaDataSet dataSet1, CremaDataSet dataSet2)
        {
            try
            {
                Assert.AreEqual(dataSet1.GetXml(), dataSet2.GetXml(), "Xml");
            }
            catch
            {
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "dataSet1" + CremaSchema.XmlExtension), dataSet1.GetXml());
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "dataSet2" + CremaSchema.XmlExtension), dataSet2.GetXml());
                throw;
            }
        }

        public static void CompareTable(CremaDataTable table1, CremaDataTable table2)
        {
            if (table1 == null && table2 == null)
                return;
            if (table1 == null || table2 == null)
                Assert.Fail("테이블이 같지 않습니다.");

            

            if (table1.Parent == null)
            {
                CompareSchema(table1, table2);
                CompareXml(table1, table2);
            }

            Assert.AreEqual(table1.Name, table2.Name, "Name");
            Assert.AreEqual(table1.TableName, table2.TableName, "TableName");
            Assert.AreEqual(table1.TableID, table2.TableID, "TableID");
            Assert.AreEqual(table1.Comment, table2.Comment, "Comment");
            Assert.AreEqual(table1.CategoryPath, table2.CategoryPath, "CategoryPath");
            Assert.AreEqual(table1.TemplateNamespace, table2.TemplateNamespace, "TemplateNamespace");
            Assert.AreEqual(table1.CreationInfo, table2.CreationInfo, "CreationInfo");
            Assert.AreEqual(table1.ModificationInfo, table2.ModificationInfo, "ModificationInfo");
            Assert.AreEqual(table1.ContentsInfo, table2.ContentsInfo, "ContentsInfo");
            Assert.AreEqual(table1.Name, table2.Name, "Name");
            Assert.AreEqual(table1.Tags, table2.Tags, "Tags");
            Assert.AreEqual(table1.TableInfo, table2.TableInfo, "TableInfo");

            Assert.AreEqual(table1.Childs.Count, table2.Childs.Count, "Childs.Count");

            for (int i = 0; i < table1.Childs.Count; i++)
            {
                CompareTable(table1.Childs[i], table2.Childs[i]);
            }

            Assert.AreEqual(table1.Columns.Count, table2.Columns.Count, "Columns.Count");

            for (int i = 0; i < table1.Columns.Count; i++)
            {
                CompareColumn(table1.Columns[i], table2.Columns[i]);
            }
        }

        public static void CompareColumn(CremaDataColumn column1, CremaDataColumn column2)
        {
            if (column1 == null && column2 == null)
                return;
            if (column1 == null || column2 == null)
                Assert.Fail("컬럼이 같지 않습니다.");

            Assert.AreEqual(column1.AllowDBNull, column2.AllowDBNull, "AllowDBNull");
            Assert.AreEqual(column1.AutoIncrement, column2.AutoIncrement, "AutoIncrement");
            Assert.AreEqual(column1.ColumnName, column2.ColumnName, "ColumnName");
            Assert.AreEqual(column1.DataType, column2.DataType, "DataType");
            CremaComparer.CompareType(column1.CremaType, column2.CremaType);
            Assert.AreEqual(column1.DefaultValue, column2.DefaultValue, "DefaultValue");
            Assert.AreEqual(column1.Expression, column2.Expression, "Expression");
            Assert.AreEqual(column1.Validation, column2.Validation, "Validation");
            Assert.AreEqual(column1.Unique, column2.Unique, "Unique");
            Assert.AreEqual(column1.ReadOnly, column2.ReadOnly, "ReadOnly");
            Assert.AreEqual(column1.Comment, column2.Comment, "Comment");
            Assert.AreEqual(column1.IsKey, column2.IsKey, "IsKey");
            Assert.AreEqual(column1.CreationInfo, column2.CreationInfo, "CreationInfo");
            Assert.AreEqual(column1.ModificationInfo, column2.ModificationInfo, "ModificationInfo");
            Assert.AreEqual(column1.ColumnID, column2.ColumnID, "ColumnID");
            Assert.AreEqual(column1.Tags, column2.Tags, "Tags");
            Assert.AreEqual(column1.ColumnInfo, column2.ColumnInfo, "ColumnInfo");
            Assert.AreEqual(column1.Index, column2.Index, "Ordinal");
        }

        public static void CompareSchema(CremaDataTable table1, CremaDataTable table2)
        {
            if (table1.TemplateNamespace != string.Empty)
                return;

            try
            {
                Assert.AreEqual(table1.GetXmlSchema(), table2.GetXmlSchema(), "Schema");
            }
            catch
            {
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "table1" + CremaSchema.SchemaExtension), table1.GetXmlSchema());
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "table2" + CremaSchema.SchemaExtension), table2.GetXmlSchema());
                throw;
            }
        }

        public static void CompareXml(CremaDataTable table1, CremaDataTable table2)
        {
            try
            {
                Assert.AreEqual(table1.GetXml(), table2.GetXml(), "Xml");
            }
            catch
            {
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "table1" + CremaSchema.SchemaExtension), table1.GetXmlSchema());
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "table2" + CremaSchema.SchemaExtension), table2.GetXmlSchema());
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "table1" + CremaSchema.XmlExtension), table1.GetXml());
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "table2" + CremaSchema.XmlExtension), table2.GetXml());
                throw;
            }
        }

        public static void CompareType(CremaDataType type1, CremaDataType type2)
        {
            if (type1 == null && type2 == null)
                return;
            if (type1 == null || type2 == null)
                Assert.Fail("타입이 같지 않습니다.");

            CompareSchema(type1, type2);
            Assert.AreEqual(type1.TypeName, type2.TypeName, "TypeName");
            Assert.AreEqual(type1.CategoryPath, type2.CategoryPath, "CategoryPath");
            Assert.AreEqual(type1.Comment, type2.Comment, "Comment");
            Assert.AreEqual(type1.CreationInfo, type2.CreationInfo, "CreationInfo");
            Assert.AreEqual(type1.ModificationInfo, type2.ModificationInfo, "ModificationInfo");
            Assert.AreEqual(type1.TypeInfo, type2.TypeInfo, "TypeInfo");

            Assert.AreEqual(type1.Members.Count, type2.Members.Count, "Members.Count");

            for (int i = 0; i < type1.Members.Count; i++)
            {
                CompareMember(type1.Members[i], type2.Members[i]);
            }
        }

        public static void CompareMember(CremaDataTypeMember member1, CremaDataTypeMember member2)
        {
            if (member1 == null && member2 == null)
                return;
            if (member1 == null || member2 == null)
                Assert.Fail("멤버가 같지 않습니다.");

            Assert.AreEqual(member1.Index, member2.Index, "Index");
            Assert.AreEqual(member1.Name, member2.Name, "Name");
            Assert.AreEqual(member1.Value, member2.Value, "Value");
            Assert.AreEqual(member1.Comment, member2.Comment, "Comment");
            Assert.AreEqual(member1.CreationInfo, member2.CreationInfo, "CreationInfo");
            Assert.AreEqual(member1.ModificationInfo, member2.ModificationInfo, "ModificationInfo");
        }

        public static void CompareSchema(CremaDataType type1, CremaDataType type2)
        {
            try
            {
                Assert.AreEqual(type1.GetXmlSchema(), type2.GetXmlSchema(), "Schema");
            }
            catch
            {
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), type1.TypeName + "1" + CremaSchema.SchemaExtension), type1.GetXmlSchema());
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), type2.TypeName + "2" + CremaSchema.SchemaExtension), type2.GetXmlSchema());
                throw;
            }
        }

        
    }
}
