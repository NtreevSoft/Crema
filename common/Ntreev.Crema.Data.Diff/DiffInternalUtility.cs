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

using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Diff
{
    static class DiffInternalUtility
    {
        public static void InitializeMembers(CremaDataType diffSource1, CremaDataType diffSource2, CremaDataType dataType1, CremaDataType dataType2)
        {
            //var hashValue1 = dataType1?.HashValue;
            //var hashValue2 = dataType2?.HashValue;

            diffSource1.BeginLoadData();
            diffSource2.BeginLoadData();
            //if (dataType1.HashValue != null && dataType1.HashValue == dataType2.HashValue)
            {
                var inlineBuilder = new SideBySideDiffBuilder(new Differ());
                var memberText1 = dataType1 == null ? DiffInternalUtility.GetString(dataType2.Items) : DiffInternalUtility.GetString(dataType1.Items);
                var memberText2 = dataType2 == null ? DiffInternalUtility.GetString(dataType1.Items) : DiffInternalUtility.GetString(dataType2.Items);
                var memberDiff = inlineBuilder.BuildDiffModel(memberText1, memberText2);
                DiffInternalUtility.FillMember(diffSource1, dataType1 ?? dataType2, memberDiff.OldText.Lines);
                DiffInternalUtility.FillMember(diffSource2, dataType2 ?? dataType1, memberDiff.NewText.Lines);
            }
            //else
            //{

            //}
            diffSource1.EndLoadData();
            diffSource2.EndLoadData();
        }

        private static void FillMember(CremaDataType diffType, CremaDataType dataType, List<DiffPiece> lines)
        {
            var index = 0;
            diffType.Clear();
            foreach (var item in lines)
            {
                var diffMember = diffType.NewMember();
                if (item.Text != null)
                {
                    var typeMember = dataType.Members[index];
                    diffMember.CopyFrom(typeMember);
                    diffMember.SetAttribute(DiffUtility.DiffIDKey, typeMember.MemberID);
                    diffType.ExtendedProperties[diffMember] = typeMember;
                    index++;
                }
                diffMember.SetAttribute(DiffUtility.DiffEnabledKey, item.Text != null);
                diffMember.Index = diffType.Members.Count;
                diffType.Members.Add(diffMember);
            }
        }

        private static void FillColumns(CremaDataTable diffTable, CremaDataTable dataTable, List<DiffPiece> lines)
        {
            var index = 0;
            foreach (var item in lines)
            {
                var diffColumn = diffTable.Columns.Add();
                if (item.Text != null && dataTable != null)
                {
                    var dataColumn = dataTable.Columns[index];
                    diffColumn.ColumnName = dataColumn.ColumnName;
                    diffColumn.CopyFrom(dataColumn);
                    index++;
                }
                else
                {
                    diffColumn.ColumnName = GenerateDummyName();
                }
                DiffUtility.SetDiffState(diffColumn, (DiffState)item.Type);
            }

            string GenerateDummyName()
            {
                return NameUtility.GenerateNewName(DiffUtility.DiffDummyKey, diffTable.Columns.Select(item => item.ColumnName));
            }
        }

        internal static void SyncColumns(CremaDataTable diffTable1, CremaDataTable diffTable2, CremaDataTable dataTable1, CremaDataTable dataTable2)
        {
            var inlineBuilder = new SideBySideDiffBuilder(new Differ());
            var columnText1 = dataTable1 == null ? string.Empty : GetString(dataTable1.Columns);
            var columnText2 = dataTable2 == null ? string.Empty : GetString(dataTable2.Columns);
            var columnDiff = inlineBuilder.BuildDiffModel(columnText1, columnText2);

            diffTable1.Clear();
            diffTable2.Clear();
            diffTable1.Columns.Clear();
            diffTable2.Columns.Clear();
            FillColumns(diffTable1, dataTable1, columnDiff.OldText.Lines);
            FillColumns(diffTable2, dataTable2, columnDiff.NewText.Lines);

            if (dataTable1 == null)
            {
                foreach (var item in diffTable1.Columns)
                {
                    DiffUtility.SetDiffState(item, DiffState.Imaginary);
                }
            }

            if (dataTable2 == null)
            {
                foreach (var item in diffTable2.Columns)
                {
                    DiffUtility.SetDiffState(item, DiffState.Imaginary);
                }
            }
        }

        internal static void FillRows(CremaDataTable diffTable1, CremaDataTable diffTable2, CremaDataTable dataTable1, CremaDataTable dataTable2)
        {
            if (dataTable1 != null)
            {
                AddRows(dataTable1, diffTable1);
            }

            if (dataTable2 != null)
            {
                AddRows(dataTable2, diffTable2);
            }

            void AddRows(CremaDataTable sourceTable, CremaDataTable destTable)
            {
                foreach (var item in sourceTable.Rows)
                {
                    AddRow(item);
                }

                void AddRow(CremaDataRow sourceRow)
                {
                    var dataRow = destTable.NewRow();
                    foreach (var c in sourceTable.Columns)
                    {
                        dataRow[c.ColumnName] = sourceRow[c];
                    }
                    destTable.Rows.Add(dataRow);
                }
            }
        }

        internal static bool InitializeRows(CremaDataTable diffTable1, CremaDataTable diffTable2, CremaDataTable dataTable1, CremaDataTable dataTable2)
        {
            var differ = new Differ();
            var inlineBuilder = new SideBySideDiffBuilder(differ);

            var rowText1 = dataTable1 == null ? string.Empty : GetString(dataTable1.Rows);
            var rowText2 = dataTable2 == null ? string.Empty : GetString(dataTable2.Rows);

            var rowDiff = inlineBuilder.BuildDiffModel(rowText1, rowText2);
            var types1 = rowDiff.OldText.Lines.Select(item => item.Type).ToArray();
            var types2 = rowDiff.NewText.Lines.Select(item => item.Type).ToArray();

            foreach (var item in diffTable1.Childs)
            {
                item.Rows.Clear();
            }
            diffTable1.Rows.Clear();

            foreach (var item in diffTable2.Childs)
            {
                item.Rows.Clear();
            }
            diffTable2.Rows.Clear();

            diffTable1.BeginLoad();
            diffTable2.BeginLoad();
            FillRows(diffTable1, dataTable1, rowDiff.OldText.Lines);
            FillRows(diffTable2, dataTable2, rowDiff.NewText.Lines);
            diffTable1.EndLoad();
            diffTable2.EndLoad();
            return true;
        }

        internal static void InitializeChildRows(CremaDataTable diffTable1, CremaDataTable diffTable2, CremaDataTable dataTable1, CremaDataTable dataTable2)
        {
            var diffParent1 = diffTable1.Parent;
            var diffParent2 = diffTable2.Parent;

            diffTable1.Clear();
            diffTable2.Clear();

            for (var i = 0; i < diffParent1.Rows.Count; i++)
            {
                var dr1 = diffParent1.Rows[i];
                var dr2 = diffParent2.Rows[i];
                var sr1 = (CremaDataRow)diffParent1.ExtendedProperties[dr1];
                var sr2 = (CremaDataRow)diffParent2.ExtendedProperties[dr2];

                InitializeChildRows(dr1, dr2, sr1, sr2, diffTable1, diffTable2, dataTable1, dataTable2);
            }
        }

        private static void InitializeChildRows(CremaDataRow diffRow1, CremaDataRow diffRow2, CremaDataRow dataRow1, CremaDataRow dataRow2, CremaDataTable diffChildTable1, CremaDataTable diffChildTable2, CremaDataTable childTable1, CremaDataTable childTable2)
        {
            var emptyRows = new CremaDataRow[] { };
            var inlineBuilder = new SideBySideDiffBuilder(new Differ());

            var childRows1 = dataRow1 != null && childTable1 != null ? dataRow1.GetChildRows(childTable1) : emptyRows;
            var childRows2 = dataRow2 != null && childTable2 != null ? dataRow2.GetChildRows(childTable2) : emptyRows;

            var rowText1 = GetString(childRows1);
            var rowText2 = GetString(childRows2);
            var rowDiff = inlineBuilder.BuildDiffModel(rowText1, rowText2);

            FillChildRow(diffRow1, dataRow1, diffChildTable1, childTable1, childRows1, rowDiff.OldText.Lines);
            FillChildRow(diffRow2, dataRow2, diffChildTable2, childTable2, childRows2, rowDiff.NewText.Lines);
        }

        private static void FillRows(CremaDataTable diffTable, CremaDataTable dataTable, List<DiffPiece> lines)
        {
            var index = 0;
            diffTable.MinimumCapacity = lines.Count;
            foreach (var item in lines)
            {
                var diffRow = diffTable.NewRow();
                if (item.Text != null)
                {
                    var dataRow = dataTable.Rows[index];
                    CopyFields(dataRow, diffRow);
                    CopyAttributes(dataRow, diffRow);
                    diffRow.RelationID = null;
                    diffTable.ExtendedProperties[diffRow] = dataRow;
                    index++;
                }

                diffRow.SetAttribute(DiffUtility.DiffEnabledKey, item.Text != null);
                diffTable.Rows.Add(diffRow);
            }
        }

        private static void FillChildRow(CremaDataRow diffParentRow, CremaDataRow parentRow, CremaDataTable childDiffTable, CremaDataTable childTable, CremaDataRow[] childRows, List<DiffPiece> lines)
        {
            var index = 0;
            foreach (var item in lines)
            {
                var diffRow = childDiffTable.NewRow(diffParentRow);
                if (item.Text != null)
                {
                    var dataRow = childRows[index];
                    CopyFields(dataRow, diffRow);
                    CopyAttributes(dataRow, diffRow);
                    diffRow.RelationID = diffParentRow.RelationID;
                    childDiffTable.ExtendedProperties[diffRow] = dataRow;
                    index++;
                }
                diffRow.SetAttribute(DiffUtility.DiffEnabledKey, item.Text != null);
                childDiffTable.Rows.Add(diffRow);
            }
        }

        private static string GetFieldString(object field)
        {
            var escapeList = new List<char>() { '%', '-', '\r', '\n' };

            string text = null;
            if (field == DBNull.Value)
                text = "!dbnull!";
            else if (field == null)
                text = "!null!";
            else
                text = field.ToString();

            foreach (var item in escapeList)
            {
                text = text.Replace(item.ToString(), $"%{(int)item}");
            }

            foreach (var item in SideBySideDiffBuilder.WordSeparaters)
            {
                text = text.Replace(item.ToString(), $"%{(int)item}");
            }

            return text;
        }

        private static string GetString(CremaDataTypeMemberCollection members)
        {
            var lineList = new List<string>(members.Count);
            foreach (var item in members)
            {
                lineList.Add(GetString(item));
            }
            return string.Join(Environment.NewLine, lineList);
        }

        private static string GetString(CremaDataTypeMember member)
        {
            var fieldsText = new object[] { member.Name }.Select(item => GetFieldString(item));
            return string.Join("-", fieldsText);
        }

        private static string GetString(CremaDataColumnCollection columns)
        {
            var lineList = new List<string>(columns.Count);
            foreach (var item in columns)
            {
                lineList.Add(GetString(item));
            }
            return string.Join(Environment.NewLine, lineList);
        }

        private static string GetString(CremaDataColumn column)
        {
            var fieldsText = new object[] { column.ColumnName }.Select(item => GetFieldString(item));
            return string.Join("-", fieldsText);
        }

        private static string GetString(CremaDataRowCollection rows)
        {
            var lineList = new List<string>(rows.Count);
            foreach (var item in rows)
            {
                lineList.Add(GetString(item));
            }
            return string.Join(Environment.NewLine, lineList);
        }

        private static string GetString(CremaDataRow dataRow)
        {
            var query = from item in CollectFields()
                        select GetFieldString(item);
            return string.Join("-", query);

            IEnumerable<object> CollectFields()
            {
                foreach (var item in dataRow.Table.Columns)
                {
                    yield return dataRow[item];
                }
            }
        }

        private static string GetString(IEnumerable<CremaDataRow> rows)
        {
            if (rows == null)
                return string.Empty;

            var lineList = new List<string>();
            foreach (var item in rows)
            {
                lineList.Add(GetString(item));
            }
            return string.Join(Environment.NewLine, lineList);
        }

        private static void CopyAttributes(CremaDataRow sourceRow, CremaDataRow destRow)
        {
            var sourceTable = sourceRow.Table;
            foreach (var item in sourceTable.Attributes)
            {
                if (item.AttributeName == CremaSchema.Index)
                    continue;
                var sourceValue = sourceRow.GetAttribute(item.AttributeName);
                destRow.SetAttribute(item.AttributeName, sourceValue);
            }
        }

        private static void CopyFields(CremaDataRow sourceRow, CremaDataRow destRow)
        {
            foreach (var item in destRow.Table.Columns)
            {
                if (item.ExtendedProperties.ContainsKey(nameof(CremaDataColumn.ColumnName)))
                {
                    var columnName = item.ExtendedProperties[nameof(CremaDataColumn.ColumnName)] as string;
                    if (columnName.StartsWith(DiffUtility.DiffDummyKey) == true)
                        continue;
                    var sourceValue = sourceRow[columnName];
                    destRow[item.ColumnName] = sourceValue;
                }
                else if (sourceRow.Table.Columns.Contains(item.ColumnName))
                {
                    var sourceValue = sourceRow[item.ColumnName];
                    destRow[item.ColumnName] = sourceValue;
                }
            }
        }
    }
}
