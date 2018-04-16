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

using Ntreev.Crema.Data.Xml.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Diff
{
    static class DiffExtensions
    {
        public static CremaDataTypeMember AddEmptyMember(this CremaDataType dataType)
        {
            dataType.BeginLoadData();
            var member = dataType.NewMember();
            member.EmptyFields();
            member.SetAttribute(DiffUtility.DiffEnabledKey, false);
            member.SetAttribute(DiffUtility.DiffStateKey, DiffState.Imaginary);
            dataType.Members.Add(member);
            dataType.EndLoadData();
            return member;
        }

        public static CremaTemplateColumn AddEmptyColumn(this CremaTemplate template)
        {
            template.BeginLoadData();
            var column = template.NewColumn();
            column.TargetColumn = template.TargetTable.Columns.Add();
            column.EmptyFields();
            column.SetAttribute(DiffUtility.DiffEnabledKey, false);
            column.SetAttribute(DiffUtility.DiffStateKey, DiffState.Imaginary);
            template.Columns.Add(column);
            template.EndLoadData();
            return column;
        }

        public static CremaDataRow AddEmptyRow(this CremaDataTable dataTable)
        {
            dataTable.InternalObject.BeginLoadData();
            var row = dataTable.NewRow();
            row.EmptyFields();
            dataTable.Rows.Add(row);
            dataTable.InternalObject.EndLoadData();
            return row;
        }

        public static void CopyFrom(this CremaDataTypeMember member, CremaDataTypeMember sourceMember)
        {
            CopyFields((InternalDataTypeMember)sourceMember, (InternalDataTypeMember)member);
        }

        public static void CopyFrom(this CremaTemplateColumn column, CremaTemplateColumn sourceColumn)
        {
            CopyFields((InternalTemplateColumn)sourceColumn, (InternalTemplateColumn)column);
        }

        public static void CopyFrom(this CremaDataRow row, CremaDataRow sourceRow)
        {
            CopyFields((InternalDataRow)sourceRow, (InternalDataRow)row);
        }

        public static void CopyFrom(this CremaDataColumn column, CremaDataColumn sourceColumn)
        {
            CopyProperties(sourceColumn, column);
        }

        public static void CopyFrom(this CremaTemplateColumn templateColumn, CremaDataColumn dataColumn)
        {
            templateColumn.ColumnID = dataColumn.ColumnID;
            templateColumn.Tags = dataColumn.Tags;
            templateColumn.Name = dataColumn.ColumnName;
            templateColumn.IsKey = dataColumn.IsKey;
            templateColumn.DataTypeName = dataColumn.DataTypeName;
            templateColumn.Comment = dataColumn.Comment;
            templateColumn.Unique = dataColumn.Unique;
            templateColumn.AutoIncrement = dataColumn.AutoIncrement;
            templateColumn.ReadOnly = dataColumn.ReadOnly;
            templateColumn.AllowNull = dataColumn.AllowDBNull;
            templateColumn.DefaultValue = dataColumn.DefaultValueString;
            templateColumn.CreationInfo = dataColumn.CreationInfo;
            templateColumn.ModificationInfo = dataColumn.ModificationInfo;
        }

        public static void EmptyFields(this CremaDataTypeMember typeMember)
        {
            EmptyFields((InternalDataTypeMember)typeMember);
        }

        public static void EmptyFields(this CremaTemplateColumn column)
        {
            EmptyFields((InternalTemplateColumn)column);
        }

        public static void EmptyFields(this CremaDataRow row)
        {
            EmptyFields((InternalDataRow)row);
        }

        public static void Add(this CremaAttributeCollection attributes, string attributeName, Type type, object defaultValue)
        {
            var attribute = attributes.Add(attributeName, type);
            attribute.IsVisible = false;
            attribute.DefaultValue = defaultValue;
        }

        public static void SetDiffState(this CremaDataType dataType, DiffState diffState)
        {
            DiffUtility.SetDiffState(dataType, diffState);
        }

        public static void SetDiffState(this CremaDataTypeMember typeMember, DiffState diffState)
        {
            DiffUtility.SetDiffState(typeMember, diffState);
        }

        public static void SetDiffState(this CremaTemplate template, DiffState diffState)
        {
            DiffUtility.SetDiffState(template, diffState);
        }

        public static void SetDiffState(this CremaTemplateColumn templateColumn, DiffState diffState)
        {
            DiffUtility.SetDiffState(templateColumn, diffState);
        }

        public static void DeleteItems(this CremaDataType dataType)
        {
            foreach (var item in dataType.Items.ToArray())
            {
                if ((bool)item.GetAttribute(DiffUtility.DiffEnabledKey) == false)
                {
                    item.Delete();
                }
            }
        }

        public static void DeleteItems(this CremaTemplate template)
        {
            foreach (var item in template.Items.ToArray())
            {
                if ((bool)item.GetAttribute(DiffUtility.DiffEnabledKey) == false)
                {
                    item.Delete();
                }
            }
        }

        public static void DeleteItems(this CremaDataTable dataTable)
        {
            foreach (var item in dataTable.Rows.ToArray())
            {
                if (DiffUtility.GetDiffState(item) == DiffState.Imaginary)
                {
                    item.Delete();
                }
            }
        }

        private static void CopyProperties(CremaDataColumn sourceColumn, CremaDataColumn destColumn)
        {
            destColumn.InternalAllowDBNull = sourceColumn.AllowDBNull;
            destColumn.InternalAutoIncrement = sourceColumn.AutoIncrement;
            destColumn.InternalColumnName = sourceColumn.ColumnName;
            destColumn.InternalDataType = sourceColumn.DataType;
            destColumn.InternalDefaultValue = sourceColumn.DefaultValue;
            destColumn.InternalExpression = sourceColumn.Expression;
            destColumn.InternalValidation = sourceColumn.Validation;
            destColumn.InternalUnique = sourceColumn.Unique;
            destColumn.InternalReadOnly = sourceColumn.ReadOnly;
            destColumn.InternalComment = sourceColumn.Comment;
            destColumn.InternalIsKey = sourceColumn.IsKey;
            destColumn.InternalCreationInfo = sourceColumn.CreationInfo;
            destColumn.InternalModificationInfo = sourceColumn.ModificationInfo;
            destColumn.InternalTags = sourceColumn.Tags;
            destColumn.InternalColumnID = sourceColumn.ColumnID;

            destColumn.InternalDataType = sourceColumn.DataType;
            if (sourceColumn.CremaType != null)
                destColumn.InternalCremaType = (InternalDataType)destColumn.Table.DataSet.Types[sourceColumn.CremaType.Name];
        }

        private static void CopyFields(DataRow sourceRow, DataRow destRow)
        {
            destRow.BeginEdit();
            try
            {
                foreach (var item in sourceRow.Table.Columns)
                {
                    if (item is DataColumn column)
                    {
                        var destColumn = destRow.Table.Columns[column.ColumnName];
                        if (destColumn == null || destColumn.ReadOnly == true)
                            continue;
                        var field2 = destRow[column.ColumnName];
                        var field1 = sourceRow[column.ColumnName];

                        if (object.Equals(field2, field1) == false)
                        {
                            destRow[column.ColumnName] = field1;
                        }
                    }
                }
                destRow.EndEdit();
            }
            catch
            {
                destRow.CancelEdit();
                throw;
            }
        }

        private static object[] EmptyFields(DataRow dataRow)
        {
            dataRow.BeginEdit();
            try
            {
                var itemArray = new object[dataRow.ItemArray.Length];
                for (var i = 0; i < dataRow.Table.Columns.Count; i++)
                {
                    var item = dataRow.Table.Columns[i];
                    if (item.ColumnMapping == MappingType.Hidden || item.ColumnName == CremaSchema.Index)
                        continue;
                    itemArray[i] = dataRow[i];
                    dataRow[i] = DBNull.Value;
                }
                dataRow.EndEdit();
                return itemArray;
            }
            catch
            {
                dataRow.CancelEdit();
                throw;
            }
        }
    }
}
