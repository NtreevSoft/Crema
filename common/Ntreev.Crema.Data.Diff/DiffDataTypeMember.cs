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
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Diff
{
    public class DiffDataTypeMember
    {
        private readonly DiffDataType diffType;
        private readonly int index;
        private CremaDataTypeMember item1;
        private CremaDataTypeMember item2;

        internal DiffDataTypeMember(DiffDataType diffType, int index)
        {
            this.diffType = diffType;
            this.index = index;
            this.item1 = index < diffType.SourceItem1.Items.Count ? diffType.SourceItem1.Items[index] : null;
            this.item2 = index < diffType.SourceItem2.Items.Count ? diffType.SourceItem2.Items[index] : null;
        }

        /// <summary>
        /// Member2와 연결되는 실제 멤버
        /// </summary>
        /// <returns></returns>
        public CremaDataTypeMember GetTarget1()
        {
            var item2 = this.Item2;
            var diffSource1 = this.diffType.SourceItem1;
            if (item2.GetAttribute(CremaSchema.ID) is DBNull)
            {
                var item1 = diffSource1.Items[item2.Index];
                if (item1.GetAttribute(CremaSchema.ID) is DBNull)
                    return item1;
                return null;
            }

            if (diffSource1.Items.Contains(item2.MemberID) == false)
            {
                return diffSource1.Items[item2.Index];
            }

            return diffSource1.Items[item2.MemberID];
        }

        public CremaDataTypeMember GetTarget2()
        {
            var item1 = this.Item1;
            var diffSource2 = this.diffType.SourceItem2;
            if (item1.GetAttribute(CremaSchema.ID) is DBNull)
            {
                var member2 = diffSource2.Items[item1.Index];
                if (member2.GetAttribute(CremaSchema.ID) is DBNull)
                    return member2;
                return null;
            }

            if (diffSource2.Items.Contains(item1.MemberID) == false)
            {
                return diffSource2.Items[item1.Index];
            }

            return diffSource2.Items[item1.MemberID];
        }

        public CremaDataTypeMember Item1
        {
            get { return this.item1; }
            internal set { this.item1 = value; }
        }

        public CremaDataTypeMember Item2
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

        private static void Update(DataRow dataRow1, DataRow dataRow2, DiffState defaultState)
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
                    if (item is DataColumn column)
                    {
                        if (column.ColumnMapping == MappingType.Hidden)
                            continue;
                        var field1 = dataRow1[column.ColumnName];
                        var field2 = dataRow2[column.ColumnName];

                        if (object.Equals(field1, field2) == false)
                        {
                            fieldsList.Add(column.ColumnName);
                        }
                    }
                }

                if (object.Equals(dataRow1[CremaSchema.ID], dataRow2[CremaSchema.ID]) == true)
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

        internal void Update()
        {
            if (this.diffType.ItemSet.Contains(this) == false)
            {
                this.diffType.ItemSet.Add(this);
                Update((InternalDataTypeMember)this.Item1, (InternalDataTypeMember)this.GetTarget2(), DiffState.Deleted);
                Update((InternalDataTypeMember)this.Item2, (InternalDataTypeMember)this.GetTarget1(), DiffState.Inserted);
                this.diffType.ItemSet.Remove(this);
            }
        }

        internal void SetState(DiffState state)
        {
            DiffUtility.SetDiffState(this.Item1, state);
            DiffUtility.SetDiffState(this.Item2, state);
        }
    }
}
