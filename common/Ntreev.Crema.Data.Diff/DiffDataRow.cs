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
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Diff
{
    public class DiffDataRow
    {
        private const string rowError = "error";
        private readonly DiffDataTable diffTable;
        private readonly int index;
        private CremaDataRow item1;
        private CremaDataRow item2;

        internal DiffDataRow(DiffDataTable diffTable, int index)
        {
            this.diffTable = diffTable;
            this.index = index;
            this.item1 = index < diffTable.SourceItem1.Rows.Count ? diffTable.SourceItem1.Rows[index] : null;
            this.item2 = index < diffTable.SourceItem2.Rows.Count ? diffTable.SourceItem2.Rows[index] : null;
        }

        public CremaDataRow GetTarget1()
        {
            var item2 = this.Item2;
            var diffSource1 = this.diffTable.SourceItem1;
            return diffSource1.Rows[item2.Index];
        }

        public CremaDataRow GetTarget2()
        {
            var item1 = this.Item1;
            var diffSource2 = this.diffTable.SourceItem2;
            return diffSource2.Rows[item1.Index];
        }

        public CremaDataRow Item1
        {
            get { return this.item1; }
            internal set { this.item1 = value; }
        }

        public CremaDataRow Item2
        {
            get { return this.item2; }
            internal set { this.item2 = value; }
        }

        public int Index
        {
            get { return this.index; }
        }

        public DiffState DiffState1 => DiffUtility.GetDiffState(this.Item1);

        public DiffState DiffState2 => DiffUtility.GetDiffState(this.Item2);

        private static void Update(InternalDataRow dataRow1, InternalDataRow dataRow2, DiffState defaultState, string[] filters)
        {
            if (dataRow2 == null || (bool)dataRow1[DiffUtility.DiffEnabledKey] == false)
            {
                DiffUtility.SetDiffFields(dataRow1, null);
                DiffUtility.SetDiffState(dataRow1, DiffState.Imaginary);
            }
            else
            {
                var fieldsList = new List<string>(dataRow1.Table.Columns.Count);
                var diffState = defaultState;

                foreach (var item in dataRow1.Table.Columns)
                {
                    if (item is InternalDataColumn dataColumn)
                    {
                        if (filters.Contains(dataColumn.ColumnName) == true)
                            continue;
                        var field1 = dataRow1[dataColumn.ColumnName];
                        var dataColumn2 = GetColumn(dataRow2, dataColumn.Index);
                        var field2 = dataRow2[dataColumn2];

                        if (object.Equals(field1, field2) == false)
                        {
                            fieldsList.Add(dataColumn.ColumnName);
                        }

                        DiffDataTable.Validate((CremaDataRow)dataRow1, (CremaDataColumn)dataColumn);
                    }
                    else if (item is InternalAttribute attribute)
                    {
                        if (attribute.ColumnMapping == MappingType.Hidden || filters.Contains(attribute.AttributeName) == true)
                            continue;
                        var field1 = dataRow1[attribute.AttributeName];
                        var field2 = dataRow2[attribute.AttributeName];

                        if (object.Equals(field1, field2) == false)
                        {
                            fieldsList.Add(attribute.AttributeName);
                        }
                    }
                }

                if (dataRow1.GetColumnsInError().Any())
                    dataRow1.RowError = rowError;
                else
                    dataRow1.RowError = string.Empty;

                if ((bool)dataRow2[DiffUtility.DiffEnabledKey] == true)
                {
                    if (fieldsList.Any() == true || object.Equals(dataRow1[CremaSchema.Index], dataRow2[CremaSchema.Index]) == false)
                    {
                        diffState = DiffState.Modified;
                    }
                    else
                    {
                        diffState = DiffState.Unchanged;
                    }

                    DiffUtility.SetDiffFields(dataRow1, fieldsList);
                    DiffUtility.SetDiffState(dataRow1, diffState);
                }
                else
                {
                    DiffUtility.SetDiffFields(dataRow1, null);
                    DiffUtility.SetDiffState(dataRow1, diffState);
                }
            }
        }

        private static void UpdateValidation(InternalDataRow dataRow1, InternalDataRow dataRow2)
        {
            if (dataRow2 != null && (bool)dataRow1[DiffUtility.DiffEnabledKey] == true)
            {
                foreach (var item in dataRow1.Table.Columns)
                {
                    if (item is InternalDataColumn dataColumn)
                    {
                        var field1 = dataRow1[dataColumn.ColumnName];
                        var dataColumn2 = GetColumn(dataRow2, dataColumn.Index);
                        var field2 = dataRow2[dataColumn2];

                        DiffDataTable.Validate((CremaDataRow)dataRow1, (CremaDataColumn)dataColumn);
                    }
                }

                if (dataRow1.GetColumnsInError().Any())
                    dataRow1.RowError = rowError;
                else
                    dataRow1.RowError = string.Empty;
            }
        }

        private static InternalDataColumn GetColumn(DataRow dataRow, int index)
        {
            foreach (var item in dataRow.Table.Columns)
            {
                if (item is InternalDataColumn dataColumn && dataColumn.Index == index)
                {
                    return dataColumn;
                }
            }
            return null;
        }

        internal void UpdateValidation()
        {
            UpdateValidation((InternalDataRow)this.Item1, (InternalDataRow)this.Item2);
            UpdateValidation((InternalDataRow)this.Item2, (InternalDataRow)this.Item1);
        }

        internal void Update()
        {
            if (this.diffTable.ItemSet.Contains(this) == false)
            {
                this.diffTable.ItemSet.Add(this);
                Update((InternalDataRow)this.Item1, (InternalDataRow)this.Item2, DiffState.Deleted, this.diffTable.Filters);
                Update((InternalDataRow)this.Item2, (InternalDataRow)this.Item1, DiffState.Inserted, this.diffTable.Filters);
                this.diffTable.ItemSet.Remove(this);
            }
        }
    }
}
