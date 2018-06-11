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
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Properties;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Diff
{
    public class DiffDataTable : INotifyPropertyChanged
    {
        private CremaDataTable diffSource1;
        private CremaDataTable diffSource2;
        private readonly CremaDataTable dataTable1;
        private readonly CremaDataTable dataTable2;
        private DiffMergeTypes mergeType;
        private DiffState diffState;
        private DiffTemplate template;
        private readonly ObservableCollection<object> unresolvedItemList = new ObservableCollection<object>();
        private DiffDataTable[] childs = new DiffDataTable[] { };
        private DiffDataTable[] derivedTables = new DiffDataTable[] { };
        private DiffDataTable templatedParent;
        private DiffDataTable parent;
        private readonly List<DiffDataRow> itemList = new List<DiffDataRow>();
        private bool isResolved;
        private string header1;
        private string header2;
        private string[] filters = new string[] { };
        private readonly HashSet<object> itemSet = new HashSet<object>();

        [Obsolete]
        public DiffDataTable(CremaDataTable dataTable1, CremaDataTable dataTable2, DiffMergeTypes mergeType)
        {
            this.diffSource1 = dataTable1 == null ? new CremaDataTable() : new CremaDataTable(dataTable1.Name, dataTable1.CategoryPath);
            this.diffSource2 = dataTable2 == null ? new CremaDataTable() : new CremaDataTable(dataTable2.Name, dataTable2.CategoryPath);
            this.dataTable1 = dataTable1;
            this.dataTable2 = dataTable2;
            this.mergeType = mergeType;
        }

        internal DiffDataTable(CremaDataTable diffTable1, CremaDataTable diffTable2, CremaDataTable dataTable1, CremaDataTable dataTable2, DiffDataSet dataSet)
        {
            this.diffSource1 = diffTable1;
            this.diffSource2 = diffTable2;
            this.dataTable1 = dataTable1;
            this.dataTable2 = dataTable2;
            this.diffSource1.ExtendedProperties[typeof(DiffDataTable)] = this;
            this.diffSource2.ExtendedProperties[typeof(DiffDataTable)] = this;
            this.diffSource1.InternalComment = (dataTable1 ?? dataTable2).Comment;
            this.diffSource1.InternalTableID = (dataTable1 ?? dataTable2).TableID;
            this.diffSource1.InternalTags = (dataTable1 ?? dataTable2).Tags;
            this.diffSource1.InternalCreationInfo = (dataTable1 ?? dataTable2).CreationInfo;
            this.diffSource1.InternalModificationInfo = (dataTable1 ?? dataTable2).ModificationInfo;
            this.diffSource1.InternalContentsInfo = (dataTable1 ?? dataTable2).InternalContentsInfo;
            this.diffSource2.InternalComment = (dataTable2 ?? dataTable1).Comment;
            this.diffSource2.InternalTableID = (dataTable2 ?? dataTable1).TableID;
            this.diffSource2.InternalTags = (dataTable2 ?? dataTable1).Tags;
            this.diffSource2.InternalCreationInfo = (dataTable2 ?? dataTable1).CreationInfo;
            this.diffSource2.InternalModificationInfo = (dataTable2 ?? dataTable1).ModificationInfo;
            this.diffSource2.InternalContentsInfo = (dataTable2 ?? dataTable1).InternalContentsInfo;
            this.DiffSet = dataSet;
        }

        public void Resolve()
        {
            this.ValidateResolve();

            if (this.diffState == DiffState.Modified)
            {
                this.diffSource1.RowChanged -= DiffSource1_RowChanged;
                this.diffSource2.RowChanged -= DiffSource2_RowChanged;
                this.diffSource1.ReadOnly = false;
                this.diffSource2.ReadOnly = false;
                this.diffSource1.DeleteItems();
                this.diffSource2.DeleteItems();
                this.diffSource1.IsDiffMode = false;
                this.diffSource2.IsDiffMode = false;
                this.diffSource1.AcceptChanges();
                this.diffSource2.AcceptChanges();
                this.diffSource1.ReadOnly = true;
                this.diffSource2.ReadOnly = true;
                this.isResolved = true;
            }
            else if (this.diffState == DiffState.Deleted)
            {
                this.MergeDelete();
            }
            else if (this.diffState == DiffState.Inserted)
            {
                this.MergeInsert();
            }

            this.InvokePropertyChangedEvent(nameof(IsResolved));
        }

        public void ResolveAll()
        {
            this.ValidateResolveInternal();
            foreach (var item in this.childs)
            {
                item.ValidateResolveInternal();
            }
            foreach (var item in this.childs)
            {
                item.Resolve();
            }
            this.Resolve();
        }

        public void AcceptChanges()
        {
            this.Resolve();
        }

        public void RejectChanges()
        {
            this.diffSource1.RowChanged -= DiffSource1_RowChanged;
            this.diffSource2.RowChanged -= DiffSource2_RowChanged;
            this.itemList.Clear();
            this.SourceItem1.RejectChanges();
            this.SourceItem2.RejectChanges();
            this.itemList.Capacity = this.diffSource1.Rows.Count;
            for (var i = 0; i < this.diffSource1.Rows.Count; i++)
            {
                var item = new DiffDataRow(this, i);
                this.itemList.Add(item);
                item.UpdateValidation();
            }
            this.diffSource1.RowChanged += DiffSource1_RowChanged;
            this.diffSource2.RowChanged += DiffSource2_RowChanged;
        }

        public bool HasChanges()
        {
            if (this.diffSource1.HasChanges(true) == true)
                return true;
            if (this.diffSource2.HasChanges(true) == true)
                return true;
            return false;
        }

        public DiffMergeTypes MergeType
        {
            get
            {
                if (this.DiffSet != null)
                    return this.DiffSet.MergeType;
                return this.mergeType;
            }
        }

        public CremaDataTable ExportTable1()
        {
            return this.ExportTable1(new CremaDataSet());
        }

        public CremaDataTable ExportTable1(CremaDataSet exportSet)
        {
            var dataTable = CreateExportTable(exportSet, this.diffSource1);
            ExportTable(exportSet, this.diffSource1);
            return dataTable;
        }

        public CremaDataTable ExportTable2()
        {
            return this.ExportTable2(new CremaDataSet());
        }

        public CremaDataTable ExportTable2(CremaDataSet exportSet)
        {
            var dataTable = CreateExportTable(exportSet, this.diffSource2);
            ExportTable(exportSet, this.diffSource2);
            return dataTable;
        }

        public override string ToString()
        {
            return this.diffSource2.TableName;
        }

        public DiffDataSet DiffSet
        {
            get;
            internal set;
        }

        public CremaDataTable SourceItem1
        {
            get { return this.diffSource1; }
        }

        public CremaDataTable SourceItem2
        {
            get { return this.diffSource2; }
        }

        public TableInfo TableInfo1
        {
            get
            {
                var tableInfo = ((InternalDataTable)this.diffSource1).TableInfo;
                tableInfo.Name = DiffUtility.GetOriginalName(tableInfo.Name);
                return tableInfo;
            }
        }

        public TableInfo TableInfo2
        {
            get
            {
                var tableInfo = ((InternalDataTable)this.diffSource2).TableInfo;
                tableInfo.Name = DiffUtility.GetOriginalName(tableInfo.Name);
                return tableInfo;
            }
        }

        public string ItemName1
        {
            get { return DiffUtility.GetOriginalName(this.diffSource1.TableName); }
            set { this.diffSource1.TableName = value; }
        }

        public string ItemName2
        {
            get { return DiffUtility.GetOriginalName(this.diffSource2.TableName); }
            set { this.diffSource2.TableName = value; }
        }

        public string Header1
        {
            get
            {
                if (this.header1 == null && this.DiffSet != null)
                    return this.DiffSet.Header1;
                return this.header1 ?? string.Empty;
            }
            set
            {
                this.header1 = value;
            }
        }

        public string Header2
        {
            get
            {
                if (this.header2 == null && this.DiffSet != null)
                    return this.DiffSet.Header2;
                return this.header2 ?? string.Empty;
            }
            set
            {
                this.header2 = value;
            }
        }

        public DiffState DiffState
        {
            get { return this.diffState; }
        }

        public IReadOnlyList<DiffDataRow> Rows
        {
            get { return this.itemList; }
        }

        public bool IsResolved
        {
            get { return this.isResolved; }
        }

        public DiffTemplate Template
        {
            get { return this.template; }
            internal set
            {
                this.template = value;
            }
        }

        public DiffDataTable TemplatedParent
        {
            get { return this.templatedParent; }
        }

        public DiffDataTable[] DerivedTables
        {
            get { return this.derivedTables; }
        }

        public DiffDataTable[] Childs
        {
            get { return this.childs; }
        }

        public IEnumerable<object> UnresolvedItems
        {
            get { return this.unresolvedItemList; }
        }

        public string[] Filters
        {
            get { return this.filters ?? new string[] { }; }
            set
            {
                this.filters = value;
                for (var i = 0; i < this.itemList.Count; i++)
                {
                    var item = this.itemList[i];
                    item.Update();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void InitializeRows()
        {
            this.diffSource1.RowChanged -= DiffSource1_RowChanged;
            this.diffSource2.RowChanged -= DiffSource2_RowChanged;
            this.diffSource1.ReadOnly = false;
            this.diffSource2.ReadOnly = false;

            if (this.unresolvedItemList.Any() == true)
            {
                if (this.parent == null)
                {
                    DiffInternalUtility.InitializeRows(this.diffSource1, this.diffSource2, this.dataTable1, this.dataTable2);
                }
                else
                {
                    DiffInternalUtility.InitializeChildRows(this.diffSource1, this.diffSource2, this.dataTable1, this.dataTable2);
                }
            }
            else
            {
                if (this.parent == null)
                {
                    DiffInternalUtility.InitializeRows(this.diffSource1, this.diffSource2, this.dataTable1 ?? this.dataTable2, this.dataTable2 ?? this.dataTable1);
                }
                else
                {
                    DiffInternalUtility.InitializeChildRows(this.diffSource1, this.diffSource2, this.dataTable1 ?? this.dataTable2, this.dataTable2 ?? this.dataTable1);
                }
            }

            this.itemList.Clear();
            this.itemList.Capacity = this.diffSource1.Rows.Count;
            for (var i = 0; i < this.diffSource1.Rows.Count; i++)
            {
                var item = new DiffDataRow(this, i);
                this.itemList.Add(item);
            }

            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                item.Update();
            }

            this.diffSource1.AcceptChanges();
            this.diffSource2.AcceptChanges();
            this.diffSource1.RowChanged += DiffSource1_RowChanged;
            this.diffSource2.RowChanged += DiffSource2_RowChanged;
        }

        internal void InitializeChilds()
        {
            var tables1 = (this.dataTable1 != null ? this.dataTable1.Childs.OrderBy(item => item.TableName) : Enumerable.Empty<CremaDataTable>()).ToList();
            var tables2 = (this.dataTable2 != null ? this.dataTable2.Childs.OrderBy(item => item.TableName) : Enumerable.Empty<CremaDataTable>()).ToList();
            var tableList = new List<DiffDataTable>();

            foreach (var item in tables1.ToArray())
            {
                var count = tables2.Count(i => i.TableID == item.TableID);
                if (count == 1)
                {
                    var dataTable1 = item;
                    var dataTable2 = tables2.Single(i => i.TableID == item.TableID);
                    var diffTable1 = DiffDataTable.Create(this.diffSource1, dataTable1.TableName);
                    var diffTable2 = DiffDataTable.Create(this.diffSource2, dataTable2.TableName);
                    DiffInternalUtility.SyncColumns(diffTable1, diffTable2, dataTable1, dataTable2);
                    var diffTable = new DiffDataTable(diffTable1, diffTable2, dataTable1, dataTable2, this.DiffSet)
                    {
                        parent = this,
                    };
                    var diffTemplate = new DiffTemplate(diffTable1, diffTable2, dataTable1, dataTable2) { DiffTable = diffTable };
                    diffTable.template = diffTemplate;
                    tableList.Add(diffTable);
                    tables1.Remove(dataTable1);
                    tables2.Remove(dataTable2);
                }
            }

            foreach (var item in tables1)
            {
                var dataTable1 = item;
                if (this.dataTable2 != null && this.dataTable2.Childs.Contains(dataTable1.TableName) == true)
                {
                    var dataTable2 = this.dataTable2.Childs[dataTable1.TableName];
                    var diffTable1 = DiffDataTable.Create(this.diffSource1, dataTable1.TableName);
                    var diffTable2 = DiffDataTable.Create(this.diffSource2, dataTable2.TableName);
                    DiffInternalUtility.SyncColumns(diffTable1, diffTable2, dataTable1, dataTable2);
                    var diffTable = new DiffDataTable(diffTable1, diffTable2, dataTable1, dataTable2, this.DiffSet)
                    {
                        parent = this,
                    };
                    var diffTemplate = new DiffTemplate(diffTable1, diffTable2, dataTable1, dataTable2) { DiffTable = diffTable };
                    diffTable.template = diffTemplate;
                    tableList.Add(diffTable);
                    tables2.Remove(dataTable2);
                }
                else
                {
                    var diffTable1 = DiffDataTable.Create(this.diffSource1, dataTable1.TableName);
                    var diffTable2 = DiffDataTable.Create(this.diffSource2, dataTable1.TableName);
                    DiffInternalUtility.SyncColumns(diffTable1, diffTable2, dataTable1, null);
                    var diffTable = new DiffDataTable(diffTable1, diffTable2, dataTable1, null, this.DiffSet)
                    {
                        parent = this,
                    };
                    var diffTemplate = new DiffTemplate(diffTable1, diffTable2, dataTable1, null) { DiffTable = diffTable };
                    diffTable.template = diffTemplate;
                    tableList.Add(diffTable);
                }
            }

            foreach (var item in tables2)
            {
                var dataTable2 = item;
                var diffTable1 = DiffDataTable.Create(this.diffSource1, dataTable2.TableName);
                var diffTable2 = DiffDataTable.Create(this.diffSource2, dataTable2.TableName);
                DiffInternalUtility.SyncColumns(diffTable1, diffTable2, null, dataTable2);
                var diffTable = new DiffDataTable(diffTable1, diffTable2, null, dataTable2, this.DiffSet)
                {
                    parent = this,
                };
                var diffTemplate = new DiffTemplate(diffTable1, diffTable2, null, dataTable2) { DiffTable = diffTable };
                diffTable.template = diffTemplate;
                tableList.Add(diffTable);
            }

            this.childs = tableList.OrderBy(item => item.SourceItem1.TableName).ToArray();
        }

        internal void InitializeDerivedTables()
        {
            var tables1 = (this.dataTable1 != null ? this.dataTable1.DerivedTables.OrderBy(item => item.TableName) : Enumerable.Empty<CremaDataTable>()).ToList();
            var tables2 = (this.dataTable2 != null ? this.dataTable2.DerivedTables.OrderBy(item => item.TableName) : Enumerable.Empty<CremaDataTable>()).ToList();
            var tableList = new List<DiffDataTable>();

            foreach (var item in tables1.ToArray())
            {
                var count = tables2.Count(i => i.TableID == item.TableID);
                if (count == 1)
                {
                    var dataTable1 = item;
                    var dataTable2 = tables2.Single(i => i.TableID == item.TableID);
                    var diffTable1 = DiffDataTable.Inherit(this.diffSource1, dataTable1.TableName);
                    var diffTable2 = DiffDataTable.Inherit(this.diffSource2, dataTable2.TableName);
                    var diffTable = new DiffDataTable(diffTable1, diffTable2, dataTable1, dataTable2, this.DiffSet) { templatedParent = this };
                    var childList = new List<DiffDataTable>();
                    foreach (var i in this.childs)
                    {
                        var d1 = diffTable1.Childs[i.SourceItem1.TableName];
                        var d2 = diffTable2.Childs[i.SourceItem2.TableName];
                        var t1 = dataTable1.Childs[i.SourceItem1.TableName];
                        var t2 = dataTable2.Childs[i.SourceItem2.TableName];
                        childList.Add(new DiffDataTable(d1, d2, t1, t2, this.DiffSet) { parent = diffTable, templatedParent = i });
                    }
                    diffTable.childs = childList.ToArray();
                    tableList.Add(diffTable);
                    tables1.Remove(dataTable1);
                    tables2.Remove(dataTable2);
                }
            }

            foreach (var item in tables1)
            {
                var dataTable1 = item;
                var dataTable2 = this.dataTable2?.DerivedTables.FirstOrDefault(i => i.TableName == dataTable1.TableName);
                if (this.dataTable2 != null && dataTable2 != null)
                {
                    var diffTable1 = DiffDataTable.Inherit(this.diffSource1, dataTable1.TableName);
                    var diffTable2 = DiffDataTable.Inherit(this.diffSource2, dataTable2.TableName);
                    var diffTable = new DiffDataTable(diffTable1, diffTable2, dataTable1, dataTable2, this.DiffSet) { templatedParent = this };
                    var childList = new List<DiffDataTable>();
                    foreach (var i in this.childs)
                    {
                        var d1 = diffTable1.Childs[i.SourceItem1.TableName];
                        var d2 = diffTable2.Childs[i.SourceItem2.TableName];
                        var t1 = dataTable1.Childs[i.SourceItem1.TableName];
                        var t2 = dataTable2.Childs[i.SourceItem2.TableName];
                        childList.Add(new DiffDataTable(d1, d2, t1, t2, this.DiffSet) { parent = diffTable, templatedParent = i });
                    }
                    diffTable.childs = childList.ToArray();
                    tableList.Add(diffTable);
                    tables2.Remove(dataTable2);
                }
                else
                {
                    var diffTable1 = DiffDataTable.Inherit(this.diffSource1, DiffUtility.DiffDummyKey + dataTable1.TableName);
                    var diffTable2 = DiffDataTable.Inherit(this.diffSource2, DiffUtility.DiffDummyKey + dataTable1.TableName);
                    var diffTable = new DiffDataTable(diffTable1, diffTable2, dataTable1, null, this.DiffSet) { templatedParent = this };
                    var childList = new List<DiffDataTable>();
                    foreach (var i in this.childs)
                    {
                        var d1 = diffTable1.Childs[i.SourceItem1.TableName];
                        var d2 = diffTable2.Childs[i.SourceItem2.TableName];
                        var t1 = dataTable1.Childs[i.SourceItem1.TableName];
                        childList.Add(new DiffDataTable(d1, d2, t1, null, this.DiffSet) { parent = diffTable, templatedParent = i });
                    }
                    diffTable.childs = childList.ToArray();
                    tableList.Add(diffTable);
                }
            }

            foreach (var item in tables2)
            {
                var dataTable2 = item;
                var diffTable1 = DiffDataTable.Inherit(this.diffSource1, DiffUtility.DiffDummyKey + dataTable2.TableName);
                var diffTable2 = DiffDataTable.Inherit(this.diffSource2, DiffUtility.DiffDummyKey + dataTable2.TableName);
                var diffTable = new DiffDataTable(diffTable1, diffTable2, null, dataTable2, this.DiffSet) { templatedParent = this };
                var childList = new List<DiffDataTable>();
                foreach (var i in this.childs)
                {
                    var d1 = diffTable1.Childs[i.SourceItem1.TableName];
                    var d2 = diffTable2.Childs[i.SourceItem2.TableName];
                    var t2 = dataTable2.Childs[i.SourceItem2.TableName];
                    childList.Add(new DiffDataTable(d1, d2, null, t2, this.DiffSet) { parent = diffTable, templatedParent = i });
                }
                diffTable.childs = childList.ToArray();
                tableList.Add(diffTable);
            }

            this.derivedTables = tableList.OrderBy(item => item.SourceItem1.TableName).ToArray();
        }

        internal void InitializeTemplate()
        {
            if (this.templatedParent == null)
            {
                this.template = new DiffTemplate(this.diffSource1, this.diffSource2, this.dataTable1, this.dataTable2)
                {
                    DiffTable = this,
                };

                this.template.Diff();
            }
            else
            {
                this.template = this.templatedParent.template;
            }

            foreach (var item in this.childs)
            {
                item.InitializeTemplate();
            }

            foreach (var item in this.derivedTables)
            {
                item.InitializeTemplate();
            }

            if (this.template.IsResolved == false)
            {
                this.unresolvedItemList.Add(this.template);
                this.template.PropertyChanged += Template_PropertyChanged;
            }

            foreach (var item in this.childs)
            {
                if (item.template.IsResolved == false)
                {
                    this.unresolvedItemList.Add(item.template);
                    item.template.PropertyChanged += Template_PropertyChanged;
                }
            }

            if (this.parent != null && this.parent.template.IsResolved == false)
            {
                this.unresolvedItemList.Add(this.parent.template);
                this.parent.template.PropertyChanged += Template_PropertyChanged;
            }
        }

        private void InvokePropertyChangedEvent(params string[] propertyNames)
        {
            foreach (var item in propertyNames)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(item));
            }
        }

        private void Validate(CremaDataTable dataTable)
        {
            foreach (var item in dataTable.Rows)
            {
                this.Validate(dataTable, item);
            }
        }

        private void Validate(CremaDataTable dataTable, CremaDataRow dataRow)
        {
            foreach (var item in dataTable.Columns)
            {
                Validate(dataRow, item);
            }
        }

        public static void Validate(CremaDataRow dataRow, CremaDataColumn dataColumn)
        {
            var dataTypeName = dataColumn.DataTypeName;
            if (dataColumn.ExtendedProperties.Contains(nameof(CremaDataColumn.DataTypeName)) == true)
                dataTypeName = dataColumn.ExtendedProperties[nameof(CremaDataColumn.DataTypeName)] as string;

            if (CremaDataTypeUtility.IsBaseType(dataTypeName) == true)
            {
                var type = CremaDataTypeUtility.GetType(dataTypeName);
                var value = dataRow[dataColumn];
                if (value is string textValue && CremaConvert.VerifyChangeType(textValue, type) == false)
                {
                    dataRow.SetColumnError(dataColumn, $"'{value}' 은(는) {type.GetTypeName()} 으로 변경할 수 없습니다.");
                }
                else
                {
                    dataRow.SetColumnError(dataColumn, string.Empty);
                }
            }
            else
            {
                var dataType = GetCremaType();
                var value = dataRow[dataColumn];
                if (value is string textValue && dataType.VerifyValue(textValue) == false)
                {
                    dataRow.SetColumnError(dataColumn, $"'{value}' 은(는) {dataType.Name} 으로 변경할 수 없습니다.");
                }
                else
                {
                    dataRow.SetColumnError(dataColumn, string.Empty);
                }
            }

            CremaDataType GetCremaType()
            {
                var dataSet = dataRow.Table.DataSet;
                if (NameValidator.VerifyItemPath(dataTypeName) == true)
                {
                    var itemName = new ItemName(dataTypeName);
                    return dataSet.Types[itemName.Name, itemName.CategoryPath];
                }
                return dataSet.Types[dataTypeName];
            }
        }

        private void MergeDelete()
        {
            Validate();

            this.diffSource2.ReadOnly = false;
            for (var i = 0; i < this.diffSource1.Rows.Count; i++)
            {
                var item1 = this.diffSource1.Rows[i];
                var item2 = this.diffSource2.Rows[i];
                item2.CopyFrom(item1);
            }
            this.diffSource2.ReadOnly = true;
            this.isResolved = true;

            void Validate()
            {
                if (this.diffState != DiffState.Deleted)
                    throw new Exception();
            }
        }

        private void MergeInsert()
        {
            Validate();

            this.diffSource1.ReadOnly = false;
            this.diffSource2.ReadOnly = false;
            for (var i = 0; i < this.diffSource2.Rows.Count; i++)
            {
                var item1 = this.diffSource1.Rows[i];
                var item2 = this.diffSource2.Rows[i];
                item1.CopyFrom(item2);
            }
            this.diffSource1.ReadOnly = true;
            this.diffSource2.ReadOnly = true;
            this.isResolved = true;

            void Validate()
            {
                if (this.diffState != DiffState.Inserted)
                    throw new Exception();
            }
        }

        private void ValidateResolveInternal()
        {
            if (this.diffState == DiffState.Modified)
            {
                if (this.diffSource1.TableName != this.diffSource2.TableName)
                    throw new Exception();
                if (this.diffSource1.Tags != this.diffSource2.Tags)
                    throw new Exception();
                if (this.diffSource1.Comment != this.diffSource2.Comment)
                    throw new Exception();
                foreach (var item in this.itemList)
                {
                    if (item.DiffState1 == DiffState.Imaginary && item.DiffState2 == DiffState.Imaginary)
                        continue;
                    if (item.DiffState1 != DiffState.Unchanged)
                        throw new Exception(Resources.Exception_UnresolvedProblemsExist);
                    if (item.DiffState2 != DiffState.Unchanged)
                        throw new Exception(Resources.Exception_UnresolvedProblemsExist);
                }
            }
        }

        private void ValidateResolve()
        {
            this.ValidateResolveInternal();
            foreach (var item in this.childs)
            {
                if (item.IsResolved == false)
                    throw new Exception(Resources.Exception_UnresolvedChildTablesExist);
            }
        }

        private bool VerifyModified()
        {
            if (this.diffSource1.TableName != this.diffSource2.TableName)
                return true;
            if (this.diffSource1.Tags != this.diffSource2.Tags)
                return true;
            if (this.diffSource1.Comment != this.diffSource2.Comment)
                return true;

            foreach (var item in this.diffSource1.Rows)
            {
                if (DiffUtility.GetDiffState(item) != DiffState.Unchanged)
                    return true;
            }

            foreach (var item in this.diffSource2.Rows)
            {
                if (DiffUtility.GetDiffState(item) != DiffState.Unchanged)
                    return true;
            }
            return false;
        }

        private bool VerifyImaginary()
        {
            return this.unresolvedItemList.Any();
        }

        private void SyncTemplate(CremaTemplate template, CremaDataTable dataTable)
        {
            dataTable.TableName = template.TableName;
            dataTable.Tags = template.Tags;
            dataTable.Comment = template.Comment;
            foreach (var item in template.Columns)
            {
                if (item.InternalObject[DiffUtility.DiffIDKey] is Guid columnID && object.Equals(columnID, item.ColumnID) == false)
                {
                    var destColumn = dataTable.Columns[columnID];
                    destColumn.ColumnID = item.ColumnID;
                    destColumn.ColumnName = item.Name;
                    destColumn.ExtendedProperties[nameof(destColumn.IsKey)] = item.IsKey;
                    destColumn.ExtendedProperties[nameof(destColumn.DataTypeName)] = item.DataTypeName;
                    destColumn.ExtendedProperties[nameof(destColumn.DefaultValue)] = item.DefaultValue;
                    destColumn.ExtendedProperties[nameof(destColumn.AutoIncrement)] = item.AutoIncrement;
                    destColumn.ExtendedProperties[nameof(destColumn.AllowDBNull)] = item.AllowNull;
                    destColumn.ExtendedProperties[nameof(destColumn.ReadOnly)] = item.ReadOnly;
                    destColumn.ExtendedProperties[nameof(destColumn.Unique)] = item.Unique;
                    destColumn.Tags = item.Tags;
                    destColumn.Comment = item.Comment;
                    destColumn.ModificationInfo = item.ModificationInfo;
                    destColumn.CreationInfo = item.CreationInfo;
                }
            }

            foreach (var item in dataTable.Columns.ToArray())
            {
                if (template.Columns.Contains(item.ColumnID) == false)
                {
                    dataTable.Columns.Remove(item);
                }
            }

            foreach (var item in template.Columns)
            {
                if (dataTable.Columns.Contains(item.ColumnID) == false)
                {
                    var destColumn = dataTable.Columns.Add();
                    destColumn.ColumnID = item.ColumnID;
                    destColumn.ColumnName = item.Name;
                    destColumn.ExtendedProperties[nameof(destColumn.IsKey)] = item.IsKey;
                    destColumn.ExtendedProperties[nameof(destColumn.DataTypeName)] = item.DataTypeName;
                    destColumn.ExtendedProperties[nameof(destColumn.DefaultValue)] = item.DefaultValue;
                    destColumn.ExtendedProperties[nameof(destColumn.AutoIncrement)] = item.AutoIncrement;
                    destColumn.ExtendedProperties[nameof(destColumn.AllowDBNull)] = item.AllowNull;
                    destColumn.ExtendedProperties[nameof(destColumn.ReadOnly)] = item.ReadOnly;
                    destColumn.ExtendedProperties[nameof(destColumn.Unique)] = item.Unique;
                    destColumn.Tags = item.Tags;
                    destColumn.Comment = item.Comment;
                    destColumn.ModificationInfo = item.ModificationInfo;
                    destColumn.CreationInfo = item.CreationInfo;
                }
                else
                {
                    if (item.InternalObject[DiffUtility.DiffIDKey] is Guid columnID && object.Equals(columnID, item.ColumnID) == true)
                    {
                        var destColumn = dataTable.Columns[item.ColumnID];
                        destColumn.ColumnName = item.Name;
                        destColumn.ExtendedProperties[nameof(destColumn.IsKey)] = item.IsKey;
                        destColumn.ExtendedProperties[nameof(destColumn.DataTypeName)] = item.DataTypeName;
                        destColumn.ExtendedProperties[nameof(destColumn.DefaultValue)] = item.DefaultValue;
                        destColumn.ExtendedProperties[nameof(destColumn.AutoIncrement)] = item.AutoIncrement;
                        destColumn.ExtendedProperties[nameof(destColumn.AllowDBNull)] = item.AllowNull;
                        destColumn.ExtendedProperties[nameof(destColumn.ReadOnly)] = item.ReadOnly;
                        destColumn.ExtendedProperties[nameof(destColumn.Unique)] = item.Unique;
                        destColumn.Tags = item.Tags;
                        destColumn.Comment = item.Comment;
                        destColumn.ModificationInfo = item.ModificationInfo;
                        destColumn.CreationInfo = item.CreationInfo;
                    }
                }
            }

            for (var i = template.Columns.Count - 1; i >= 0; i--)
            {
                dataTable.Columns[template.Columns[i].ColumnID].Index = i;
            }

            var attributes = dataTable.Attributes.ToArray();
            var relationColumn = dataTable.ColumnRelation;
            var columns = dataTable.Columns.ToArray();

            this.Validate(dataTable);
        }

        private void SyncTemplate()
        {
            if (this.template.DiffState == DiffState.Inserted)
            {
                this.SyncTemplate(this.template.SourceItem1, this.diffSource1);
            }
            else if (this.template.DiffState == DiffState.Deleted)
            {
                this.SyncTemplate(this.template.SourceItem2, this.diffSource2);
            }
            else if (this.template.DiffState == DiffState.Modified)
            {
                this.SyncTemplate(this.template.SourceItem1, this.diffSource1);
                this.SyncTemplate(this.template.SourceItem2, this.diffSource2);
            }
        }

        private void Template_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is DiffTemplate diffTemplate)
            {
                if (e.PropertyName == nameof(this.template.IsResolved) && diffTemplate.IsResolved == true)
                {
                    if (this.template == diffTemplate)
                    {
                        this.SyncTemplate();
                    }

                    this.unresolvedItemList.Remove(sender);
                    if (this.unresolvedItemList.Any() == false)
                    {
                        if (this.parent == null)
                            this.Diff();
                    }
                }
            }
        }

        private void DiffSource1_RowChanged(object sender, CremaDataRowChangeEventArgs e)
        {
            if (e.Row.RowState == DataRowState.Detached)
                return;

            var index = e.Row.Index;
            if (index >= this.itemList.Count)
            {
                this.itemList.Add(new DiffDataRow(this, index));
            }
            else
            {
                var item = this.itemList[index];
                item.Item1 = e.Row;
                item.Update();
            }
        }

        private void DiffSource2_RowChanged(object sender, CremaDataRowChangeEventArgs e)
        {
            if (e.Row.RowState == DataRowState.Detached)
                return;

            var index = e.Row.Index;
            if (index >= this.itemList.Count)
            {
                this.itemList.Add(new DiffDataRow(this, index));
            }
            else
            {
                var item = this.itemList[index];
                item.Item2 = e.Row;
                item.Update();
            }
        }

        private static CremaDataTable CreateExportTable(CremaDataSet exportSet, CremaDataTable diffTable)
        {
            var dataSet = diffTable.DataSet;
            //var query = from item in diffTable.Columns
            //            where item.ExtendedProperties.Contains(nameof(item.DataTypeName)) == true
            //            let dataTypeName = item.ExtendedProperties[nameof(item.DataTypeName)] as string
            //            join type in dataSet.Types on dataTypeName equals type.Path
            //            select type;

            //var types = query.Distinct();
            //foreach (var item in types)
            //{
            //    if (exportSet.Types.Contains(item.Name, item.CategoryPath) == false)
            //    {
            //        item.CopyTo(exportSet);
            //    }
            //}

            var exportTable = diffTable.Parent == null ? exportSet.Tables.Add(diffTable.Name) : exportSet.Tables[diffTable.ParentName].Childs.Add(diffTable.TableName);
            exportTable.InternalTableID = diffTable.TableID;
            exportTable.InternalTags = diffTable.Tags;
            exportTable.InternalComment = diffTable.Comment;
            exportTable.InternalCreationInfo = diffTable.CreationInfo;
            exportTable.InternalModificationInfo = diffTable.ModificationInfo;
            exportTable.InternalContentsInfo = diffTable.ContentsInfo;

            var keyList = new List<CremaDataColumn>(diffTable.PrimaryKey.Length);
            foreach (var item in diffTable.Columns)
            {
                var exportColumn = exportTable.Columns.Add(item.ColumnName);
                if (item.ExtendedProperties[nameof(item.IsKey)] is bool isKey && isKey == true)
                {
                    keyList.Add(exportColumn);
                }
                exportColumn.InternalDataTypeName = (string)item.ExtendedProperties[nameof(item.DataTypeName)];
                exportColumn.InternalDefaultValue = item.ExtendedProperties[nameof(item.DefaultValue)];
                exportColumn.InternalAutoIncrement = (bool)item.ExtendedProperties[nameof(item.AutoIncrement)];
                exportColumn.InternalAllowDBNull = (bool)item.ExtendedProperties[nameof(item.AllowDBNull)];
                exportColumn.InternalReadOnly = (bool)item.ExtendedProperties[nameof(item.ReadOnly)];
                exportColumn.InternalUnique = (bool)item.ExtendedProperties[nameof(item.Unique)];
                exportColumn.InternalTags = item.Tags;
                exportColumn.InternalComment = item.Comment;
                exportColumn.InternalModificationInfo = item.ModificationInfo;
                exportColumn.InternalCreationInfo = item.CreationInfo;
            }

            exportTable.PrimaryKey = keyList.ToArray();

            foreach (var item in diffTable.Childs)
            {
                CreateExportTable(exportSet, item);
            }

            return exportTable;
        }

        private static void ExportTable(CremaDataSet dataSet, CremaDataTable diffTable)
        {
            var dataTable = dataSet.Tables[diffTable.Name];
            var internalTable = dataTable.InternalObject;
            foreach (var item in diffTable.Rows)
            {
                if (DiffUtility.GetDiffState(item) == DiffState.Imaginary)
                    continue;

                internalTable.OmitSignatureDate = true;
                var sourceRow = item.InternalObject;
                var dataRow = internalTable.NewRow();
                for (var i = 0; i < internalTable.Columns.Count; i++)
                {
                    var dataColumn = internalTable.Columns[i];
                    dataRow[dataColumn] = sourceRow[dataColumn.ColumnName];
                }
                internalTable.Rows.Add(dataRow);
                internalTable.OmitSignatureDate = false;
            }

            foreach (var item in diffTable.Childs)
            {
                ExportTable(dataSet, item);
            }
        }

        internal static CremaDataTable Create(CremaDataSet dataSet, string tableName)
        {
            var dataTable = dataSet.Tables.Add(tableName);

            dataTable.Attributes.Add(DiffUtility.DiffStateKey, typeof(string), DBNull.Value);
            dataTable.Attributes.Add(DiffUtility.DiffFieldsKey, typeof(string), DBNull.Value);
            dataTable.Attributes.Add(DiffUtility.DiffEnabledKey, typeof(bool), true);

            return dataTable;
        }

        internal static CremaDataTable Create(CremaDataTable dataTable, string childName)
        {
            var childTable = dataTable.Childs.Add(childName);

            childTable.Attributes.Add(DiffUtility.DiffStateKey, typeof(string), DBNull.Value);
            childTable.Attributes.Add(DiffUtility.DiffFieldsKey, typeof(string), DBNull.Value);
            childTable.Attributes.Add(DiffUtility.DiffEnabledKey, typeof(bool), true);

            return childTable;
        }

        internal static CremaDataTable Inherit(CremaDataTable dataTable, string tableName)
        {
            var dataSet = dataTable.DataSet;

            var derivedTable = dataSet.Tables.Add(tableName);

            derivedTable.Attributes.Add(DiffUtility.DiffStateKey, typeof(string), DBNull.Value);
            derivedTable.Attributes.Add(DiffUtility.DiffFieldsKey, typeof(string), DBNull.Value);
            derivedTable.Attributes.Add(DiffUtility.DiffEnabledKey, typeof(bool), true);

            derivedTable.InternalTableID = dataTable.TableID;
            derivedTable.Comment = dataTable.Comment;
            derivedTable.Tags = dataTable.Tags;

            foreach (var item in dataTable.Columns)
            {
                var derivedColumn = derivedTable.Columns.Add();
                derivedColumn.CopyFrom(item);
            }

            foreach (var item in dataTable.Childs)
            {
                var derivedChild = derivedTable.Childs.Add(item.TableName);
                derivedChild.Attributes.Add(DiffUtility.DiffStateKey, typeof(string), DBNull.Value);
                derivedChild.Attributes.Add(DiffUtility.DiffFieldsKey, typeof(string), DBNull.Value);
                derivedChild.Attributes.Add(DiffUtility.DiffEnabledKey, typeof(bool), true);
                derivedChild.InternalTableID = item.TableID;
                derivedChild.Comment = item.Comment;
                derivedChild.Tags = item.Tags;

                foreach (var i in item.Columns)
                {
                    var derivedColumn = derivedChild.Columns.Add();
                    derivedColumn.CopyFrom(i);
                }
            }

            derivedTable.TemplatedParent = dataTable;

            return derivedTable;
        }

        internal void DiffTemplate()
        {
            foreach (var item in this.Childs)
            {
                item.DiffTemplateInternal();

                if (this.templatedParent == null)
                {
                    if (item.template != null && item.template.IsResolved == false)
                    {
                        this.unresolvedItemList.Add(item.template);
                        item.template.PropertyChanged += Template_PropertyChanged;
                    }
                    else
                    {

                    }
                }
            }
            this.DiffTemplateInternal();
        }

        internal void DiffTemplateInternal()
        {
            if (this.templatedParent == null)
            {
                this.template.Diff();
                if (this.template.IsResolved == false)
                {
                    this.unresolvedItemList.Add(this.template);
                    this.template.PropertyChanged += Template_PropertyChanged;
                }
                else
                {
                    this.SyncTemplate();
                }

                if (this.parent != null && this.parent.template.IsResolved == false)
                {
                    this.unresolvedItemList.Add(this.parent.template);
                    this.parent.template.PropertyChanged += Template_PropertyChanged;
                }
            }
            else
            {
                if (this.templatedParent.template.IsResolved == false)
                {
                    this.unresolvedItemList.Add(this.templatedParent.template);
                    this.templatedParent.template.PropertyChanged += Template_PropertyChanged;
                }
            }
            this.diffSource1.IsDiffMode = true;
            this.diffSource2.IsDiffMode = true;
            this.diffSource1.InternalObject.OmitSignatureDate = true;
            this.diffSource2.InternalObject.OmitSignatureDate = true;
        }

        internal void Diff()
        {
            this.DiffInternal();
            foreach (var item in this.Childs)
            {
                item.DiffInternal();
            }
        }

        internal void DiffInternal()
        {
            this.InitializeRows();

            if (this.dataTable1 == null)
            {
                this.diffSource1.ReadOnly = true;
                this.diffSource2.ReadOnly = true;
                this.diffState = DiffState.Inserted;
                this.isResolved = this.unresolvedItemList.Any() == false;
            }
            else if (this.dataTable2 == null)
            {
                this.diffSource1.ReadOnly = true;
                this.diffSource2.ReadOnly = true;
                this.diffState = DiffState.Deleted;
                this.isResolved = this.unresolvedItemList.Any() == false;
            }
            else if (this.VerifyModified() == true)
            {
                this.diffSource1.ReadOnly = false;
                this.diffSource2.ReadOnly = false;
                this.diffSource1.IsDiffMode = true;
                this.diffSource2.IsDiffMode = true;
                this.diffState = DiffState.Modified;
                this.isResolved = false;
            }
            else
            {
                this.diffState = DiffState.Unchanged;
                this.isResolved = this.unresolvedItemList.Any() == false;
            }
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsResolved)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.DiffState)));
        }

        internal HashSet<object> ItemSet => this.itemSet;
    }
}
