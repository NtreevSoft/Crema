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

using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Runtime.Serialization
{
    public static class SerializationUtility
    {
        public static CremaDataSet Create(SerializationSet serializedSet)
        {
            var dataSet = new CremaDataSet();

            foreach (var item in serializedSet.Types)
            {
                var dataType = dataSet.Types.Add();
                Import(dataType, item);
            }

            var tables = serializedSet.Tables.Where(item => item.Parent == string.Empty && item.TemplatedParent == string.Empty);
            foreach (var item in tables)
            {
                var dataTable = dataSet.Tables.Add(item.TableName);
                Import(dataTable, item);
            }

            var childs = serializedSet.Tables.Where(item => item.Parent != string.Empty && item.TemplatedParent == string.Empty);
            foreach (var item in childs)
            {
                var dataTable = dataSet.Tables[item.Parent];
                var childTable = dataTable.Childs.Add(item.TableName);
                Import(childTable, item);
            }

            var derived = serializedSet.Tables.Where(item => item.Parent == string.Empty && item.TemplatedParent != string.Empty);
            foreach (var item in derived)
            {
                var dataTable = dataSet.Tables[item.TemplatedParent];
                dataTable.Inherit(item.TableName);
            }

            foreach (var item in serializedSet.Tables)
            {
                if (item.TemplatedParent != string.Empty)
                    continue;
                var dataTable = dataSet.Tables[item.Name];
            }

            foreach (var item in serializedSet.Tables)
            {
                var dataTable = dataSet.Tables[item.Name];
                ImportRow(dataTable, item);
            }

            return dataSet;
        }

        public static CremaDataType Create(SerializationType serializedType)
        {
            var dataType = new CremaDataType();
            Import(dataType, serializedType);
            return dataType;
        }

        public static CremaDataTable Create(SerializationTable serializedTable)
        {
            return Create(serializedTable, new SerializationType[] { });
        }

        public static CremaDataTable Create(SerializationTable serializedTable, SerializationType[] serializedTypes)
        {
            if (serializedTypes == null)
                throw new ArgumentNullException(nameof(serializedTypes));

            var dataTable = new CremaDataTable();
            Import(dataTable, serializedTable);
            return dataTable;
        }

        private static void Import(CremaDataType dataType, SerializationType serializedType)
        {
            //var itemName = new ItemName(serializedType.Name);

            dataType.TypeName = serializedType.Name;
            dataType.Tags = serializedType.Tags;
            dataType.CategoryPath = serializedType.CategoryPath;
            dataType.Comment = serializedType.Comment;
            dataType.IsFlag = serializedType.IsFlag;

            foreach (var item in serializedType.Members)
            {
                var typeMember = dataType.NewMember();
                Import(typeMember, item);
                dataType.Members.Add(typeMember);
            }

            dataType.AcceptChanges();
        }

        private static void Import(CremaDataTypeMember typeMember, SerializationTypeMember serializedTypeMember)
        {
            typeMember.Tags = serializedTypeMember.Tags;
            typeMember.Name = serializedTypeMember.Name;
            typeMember.Value = serializedTypeMember.Value;
            typeMember.Comment = serializedTypeMember.Comment;
        }


        private static void Import(CremaDataTable dataTable, SerializationTable serializedTable)
        {
            dataTable.Tags = serializedTable.Tags;
            dataTable.CategoryPath = serializedTable.CategoryPath;
            dataTable.TableName = serializedTable.TableName;
            dataTable.Comment = serializedTable.Comment;

            var columns = serializedTable.Columns.Where(item => item.Name != CremaSchema.__RelationID__ && item.Name != CremaSchema.__ParentID__);
            foreach (var item in columns)
            {
                var dataColumn = dataTable.Columns.Add(item.Name);
                Import(dataColumn, item);
            }

            foreach (var item in columns)
            {
                var dataColumn = dataTable.Columns[item.Name];
                dataColumn.IsKey = item.IsKey;
            }

            foreach (var item in columns)
            {
                var dataColumn = dataTable.Columns[item.Name];
                dataColumn.Unique = item.IsUnique;
            }
            dataTable.AcceptChanges();
        }

        private static void ImportRow(CremaDataTable dataTable, SerializationTable serializedTable)
        {
            dataTable.BeginLoad();
            foreach (var item in serializedTable.Rows)
            {
                if (dataTable.Parent == null)
                {
                    var dataRow = dataTable.NewRow();
                    ImportRow(dataRow, item, serializedTable.Columns);
                    dataTable.Rows.Add(dataRow);
                }
                else
                {
                    var parentRow = dataTable.Parent.Rows.First(i => i.RelationID == item.ParentID);
                    var dataRow = dataTable.NewRow(parentRow);
                    ImportRow(dataRow, item, serializedTable.Columns);
                    dataTable.Rows.Add(dataRow);
                }
            }
            dataTable.EndLoad();
        }

        private static void ImportRow(CremaDataRow dataRow, SerializationRow serializedRow, SerializationColumn[] serializedColumns)
        {
            for (var i = 0; i < serializedRow.Fields.Length; i++)
            {
                var serializedColumn = serializedColumns[i];
                var field = serializedRow.Fields[i];

                if (serializedColumn.Name == CremaSchema.__RelationID__)
                {
                    dataRow.RelationID = field as string;
                }
                else if (serializedColumn.Name == CremaSchema.__ParentID__)
                {
                    dataRow.ParentID = field as string;
                }
            }

            for (var i = 0; i < serializedRow.Fields.Length; i++)
            {
                var serializedColumn = serializedColumns[i];
                var isRelationColumn = serializedColumn.Name == CremaSchema.__RelationID__ || serializedColumn.Name == CremaSchema.__ParentID__;
                var field = serializedRow.Fields[i];

                if (field != null && isRelationColumn == false)
                {
                    dataRow[serializedColumn.Name] = field;
                }
            }
        }

        private static void Import(CremaDataColumn dataColumn, SerializationColumn serializedColumn)
        {
            dataColumn.ColumnName = serializedColumn.Name;
            dataColumn.Tags = serializedColumn.Tags;
            dataColumn.DataTypeName = serializedColumn.DataType;
            dataColumn.Comment = serializedColumn.Comment;
            dataColumn.AllowDBNull = serializedColumn.AllowNull;
            dataColumn.AutoIncrement = serializedColumn.AutoIncrement;
            dataColumn.ReadOnly = serializedColumn.ReadOnly;
        }
    }
}
