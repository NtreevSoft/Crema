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
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Diff
{
    public class DiffTemplate : INotifyPropertyChanged
    {
        private readonly static DiffTemplateColumn[] emptyColumns = new DiffTemplateColumn[] { };
        private readonly CremaDataTable diffTable1;
        private readonly CremaDataTable diffTable2;
        private readonly CremaDataTable dataTable1;
        private readonly CremaDataTable dataTable2;
        private CremaTemplate diffSource1;
        private CremaTemplate diffSource2;
        private DiffMergeTypes mergeType;
        private bool dummy1;
        private bool dummy2;
        private readonly List<DiffTemplateColumn> itemList = new List<DiffTemplateColumn>();
        private DiffState diffState;
        private bool isResolved;
        private string header1;
        private string header2;
        private ObservableCollection<object> unresolvedItemList = new ObservableCollection<object>();
        private readonly HashSet<object> itemSet = new HashSet<object>();

        [Obsolete]
        public DiffTemplate(CremaDataTable table1, CremaDataTable table2, DiffMergeTypes mergeType)
        {
            this.diffTable1 = table1;
            this.diffTable2 = table2;
            this.diffSource1 = new CremaTemplate() { TargetTable = table1 };
            this.diffSource2 = new CremaTemplate() { TargetTable = table2 };
            this.mergeType = mergeType;
        }

        internal DiffTemplate(CremaDataTable diffTable1, CremaDataTable diffTable2, CremaDataTable dataTable1, CremaDataTable dataTable2)
        {
            this.diffTable1 = diffTable1.CloneTo(new CremaDataSet());
            this.diffTable2 = diffTable2.CloneTo(new CremaDataSet());

            foreach (var item in diffTable1.Columns)
            {
                var column = this.diffTable1.Columns[item.ColumnName];
                DiffUtility.SetDiffState(column, DiffUtility.GetDiffState(item));
            }

            foreach (var item in diffTable2.Columns)
            {
                var column = this.diffTable2.Columns[item.ColumnName];
                DiffUtility.SetDiffState(column, DiffUtility.GetDiffState(item));
            }

            this.dataTable1 = dataTable1;
            this.dataTable2 = dataTable2;

            this.diffSource1 = Create(this.diffTable1);
            this.diffSource2 = Create(this.diffTable2);
            this.diffSource1.ExtendedProperties[typeof(DiffTemplate)] = this;
            this.diffSource2.ExtendedProperties[typeof(DiffTemplate)] = this;
            this.dummy1 = this.diffSource1.TableName.StartsWith(DiffUtility.DiffDummyKey);
            this.dummy2 = this.diffSource2.TableName.StartsWith(DiffUtility.DiffDummyKey);

            this.AttachEventHandler(this.diffSource1, diffTable1);
            this.AttachEventHandler(this.diffSource2, diffTable2);
        }

        public override string ToString()
        {
            if (this.diffSource1.TableName != this.diffSource2.TableName)
                return $"{this.diffSource1.TableName.Replace(DiffUtility.DiffDummyKey, string.Empty)} => {this.diffSource2.TableName}";
            return this.diffSource1.TableName;
        }

        public void Resolve()
        {
            this.ValidateResolve();

            if (this.diffState == DiffState.Modified)
            {
                this.diffSource1.ReadOnly = false;
                this.diffSource2.ReadOnly = false;
                this.diffSource1.DeleteItems();
                this.diffSource2.DeleteItems();
                this.diffSource1.ColumnChanged -= DiffSource1_ColumnChanged;
                this.diffSource2.ColumnChanged -= DiffSource2_ColumnChanged;
                this.diffSource1.ColumnDeleted -= DiffSource1_ColumnDeleted;
                this.diffSource2.ColumnDeleted -= DiffSource2_ColumnDeleted;
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
            this.ValidateResolve();
            foreach (var item in this.DiffTable.Childs.Select(item => item.Template))
            {
                item.ValidateResolve();
            }
            this.Resolve();
            foreach (var item in this.DiffTable.Childs.Select(item => item.Template))
            {
                item.Resolve();
            }
        }

        public void Merge()
        {
            if (this.diffState == DiffState.Modified)
            {
                this.MergeModify();
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

        public void Refresh()
        {
            if (this.isResolved == true)
                return;
            this.diffSource1.ColumnChanged -= DiffSource1_ColumnChanged;
            this.diffSource2.ColumnChanged -= DiffSource2_ColumnChanged;
            this.diffSource1.ColumnDeleted -= DiffSource1_ColumnDeleted;
            this.diffSource2.ColumnDeleted -= DiffSource2_ColumnDeleted;
            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                item.Update();
            }
            this.diffSource1.ColumnChanged += DiffSource1_ColumnChanged;
            this.diffSource2.ColumnChanged += DiffSource2_ColumnChanged;
            this.diffSource1.ColumnDeleted += DiffSource1_ColumnDeleted;
            this.diffSource2.ColumnDeleted += DiffSource2_ColumnDeleted;
        }

        public void AcceptChanges()
        {
            this.Resolve();
        }

        public void RejectChanges()
        {
            this.SourceItem1.RejectChanges();
            this.SourceItem2.RejectChanges();
        }

        public bool HasChanges()
        {
            if (this.diffSource1.HasChanges(true) == true)
                return true;
            if (this.diffSource2.HasChanges(true) == true)
                return true;
            if (this.diffSource1.TableName != this.diffTable1.TableName)
                return true;
            if (this.diffSource2.TableName != this.diffTable2.TableName)
                return true;
            if (this.diffSource1.Tags != this.diffTable1.Tags)
                return true;
            if (this.diffSource2.Tags != this.diffTable2.Tags)
                return true;
            if (this.diffSource1.Comment != this.diffTable1.Comment)
                return true;
            if (this.diffSource2.Comment != this.diffTable2.Comment)
                return true;
            return false;
        }

        public CremaTemplate SourceItem1
        {
            get { return this.diffSource1; }
        }

        public CremaTemplate SourceItem2
        {
            get { return this.diffSource2; }
        }

        public string ItemName1
        {
            get
            {
                if (this.dummy1 == true)
                    return this.diffSource1.TableName.Replace(DiffUtility.DiffDummyKey, string.Empty);
                return this.diffSource1.TableName;
            }
            set
            {
                if (this.dummy1 == true && this.diffSource2.TableName != value)
                    this.diffSource1.TableName = DiffUtility.DiffDummyKey + value;
                else
                    this.diffSource1.TableName = value;
                this.InvokePropertyChangedEvent(nameof(this.ItemName1));
            }
        }

        public string ItemName2
        {
            get
            {
                if (this.dummy2 == true)
                    return this.diffSource2.TableName.Replace(DiffUtility.DiffDummyKey, string.Empty);
                return this.diffSource2.TableName;
            }
            set
            {
                if (this.dummy2 == true && this.diffSource1.TableName != value)
                    this.diffSource2.TableName = DiffUtility.DiffDummyKey + value;
                else
                    this.diffSource2.TableName = value;
                this.InvokePropertyChangedEvent(nameof(this.ItemName2));
            }
        }

        public TagInfo Tags1
        {
            get { return this.diffSource1.Tags; }
            set
            {
                this.diffSource1.Tags = value;
                this.InvokePropertyChangedEvent(nameof(this.Tags1));
            }
        }

        public TagInfo Tags2
        {
            get { return this.diffSource2.Tags; }
            set
            {
                this.diffSource2.Tags = value;
                this.InvokePropertyChangedEvent(nameof(this.Tags2));
            }
        }

        public string Comment1
        {
            get { return this.diffSource1.Comment; }
            set
            {
                this.diffSource1.Comment = value;
                this.InvokePropertyChangedEvent(nameof(this.Comment1));
            }
        }

        public string Comment2
        {
            get { return this.diffSource2.Comment; }
            set
            {
                this.diffSource2.Comment = value;
                this.InvokePropertyChangedEvent(nameof(this.Comment2));
            }
        }

        public string Header1
        {
            get
            {
                if (this.header1 == null && this.DiffTable != null)
                    return this.DiffTable.Header1;
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
                if (this.header2 == null && this.DiffTable != null)
                    return this.DiffTable.Header2;
                return this.header2 ?? string.Empty;
            }
            set
            {
                this.header2 = value;
            }
        }

        public DiffMergeTypes MergeType
        {
            get
            {
                if (this.DiffTable != null)
                    return this.DiffTable.MergeType;
                return this.mergeType;
            }
        }

        public DiffDataTable DiffTable
        {
            get;
            internal set;
        }

        public IReadOnlyList<DiffTemplateColumn> Items
        {
            get { return this.itemList; }
        }

        public bool IsResolved
        {
            get { return this.isResolved; }
        }

        public DiffState DiffState
        {
            get { return this.diffState; }
        }

        public IEnumerable<object> UnresolvedItems
        {
            get { return this.unresolvedItemList; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool VerifyModified()
        {
            if (this.diffSource1.TableName != this.diffSource2.TableName)
                return true;
            if (this.diffSource1.Tags != this.diffSource2.Tags)
                return true;
            if (this.diffSource1.Comment != this.diffSource2.Comment)
                return true;

            foreach (var item in this.diffSource1.Items)
            {
                if (DiffUtility.GetDiffState(item) != DiffState.Unchanged)
                    return true;
            }

            foreach (var item in this.diffSource2.Items)
            {
                if (DiffUtility.GetDiffState(item) != DiffState.Unchanged)
                    return true;
            }
            return false;
        }

        private void ValidateResolve()
        {
            if (this.unresolvedItemList.Any() == true)
            {
                throw new Exception();
            }
            if (this.diffState == DiffState.Imaginary)
            {
                throw new Exception();
            }
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
                        throw new Exception();
                    if (item.DiffState2 != DiffState.Unchanged)
                        throw new Exception();
                }
            }
        }

        private void ValidateAddToRightSide(object dataContext)
        {
            if (dataContext is DataRowView == false)
                throw new ArgumentException(nameof(dataContext));

            var dataRowView = dataContext as DataRowView;
            var dataRow = dataRowView.Row as InternalRowBase;
            if (dataRow.Table != this.diffSource1.InternalObject)
                throw new ArgumentException(nameof(dataContext));

            var item = this.itemList[dataRow.Index];
            if (item.DiffState1 != DiffState.Deleted)
                throw new ArgumentException(nameof(dataContext));
            var item1 = item.Item1;
            if (this.diffSource2.Items.Contains(item1.Name))
                throw new ArgumentException(string.Format(Resources.Exception_AlreadyExistedItem_Format, item1.Name));
        }

        private void ValidateCopyToRightSide(object dataContext)
        {
            if (dataContext is DataRowView == false)
                throw new ArgumentException(nameof(dataContext));

            var dataRowView = dataContext as DataRowView;
            var dataRow = dataRowView.Row as InternalRowBase;
            if (dataRow.Table != this.diffSource1.InternalObject)
                throw new ArgumentException(nameof(dataContext));

            var item = this.itemList[dataRow.Index];
            var diffState1 = item.DiffState1;
            if (diffState1 != DiffState.Modified && diffState1 != DiffState.Deleted && diffState1 != DiffState.Imaginary)
                throw new ArgumentException(nameof(dataContext));
            if (diffState1 == DiffState.Imaginary && item.DiffState2 == DiffState.Modified)
                throw new ArgumentException(nameof(dataContext));
            if (diffState1 == DiffState.Deleted && item.DiffState2 == DiffState.Modified)
                throw new ArgumentException(nameof(dataContext));
        }

        private void ValidateDeleteRightSide(object dataContext)
        {
            if (dataContext is DataRowView == false)
                throw new ArgumentException(nameof(dataContext));

            var dataRowView = dataContext as DataRowView;
            var dataRow = dataRowView.Row as InternalRowBase;
            if (dataRow.Table != this.diffSource2.InternalObject)
                throw new ArgumentException(nameof(dataContext));

            var item = this.itemList[dataRow.Index];
            if (item.DiffState2 != DiffState.Inserted)
                throw new ArgumentException(nameof(dataContext));
        }

        private void ValidateAddToLeftSide(object dataContext)
        {
            if (dataContext is DataRowView == false)
                throw new ArgumentException(nameof(dataContext));

            var dataRowView = dataContext as DataRowView;
            var dataRow = dataRowView.Row as InternalRowBase;
            if (dataRow.Table != this.diffSource2.InternalObject)
                throw new ArgumentException(nameof(dataContext));

            var item = this.itemList[dataRow.Index];
            if (item.DiffState2 != DiffState.Inserted)
                throw new ArgumentException(nameof(dataContext));
            var item2 = item.Item2;
            if (this.diffSource1.Items.Contains(item2.Name))
                throw new ArgumentException(string.Format(Resources.Exception_AlreadyExistedItem_Format, item2.Name));
        }

        private void ValidateCopyToLeftSide(object dataContext)
        {
            if (dataContext is DataRowView == false)
                throw new ArgumentException(nameof(dataContext));

            var dataRowView = dataContext as DataRowView;
            var dataRow = dataRowView.Row as InternalRowBase;
            if (dataRow.Table != this.diffSource2.InternalObject)
                throw new ArgumentException(nameof(dataContext));

            var item = this.itemList[dataRow.Index];
            var diffState2 = item.DiffState2;
            if (diffState2 != DiffState.Modified && diffState2 != DiffState.Inserted && diffState2 != DiffState.Imaginary)
                throw new ArgumentException(nameof(dataContext));
            if (diffState2 == DiffState.Imaginary && item.DiffState1 == DiffState.Modified)
                throw new ArgumentException(nameof(dataContext));
            if (diffState2 == DiffState.Inserted && item.DiffState1 == DiffState.Modified)
                throw new ArgumentException(nameof(dataContext));
        }

        private void ValidateDeleteLeftSide(object dataContext)
        {
            if (dataContext is DataRowView == false)
                throw new ArgumentException(nameof(dataContext));

            var dataRowView = dataContext as DataRowView;
            var dataRow = dataRowView.Row as InternalRowBase;
            if (dataRow.Table != this.diffSource1.InternalObject)
                throw new ArgumentException(nameof(dataContext));

            var item = this.itemList[dataRow.Index];
            if (item.DiffState1 != DiffState.Deleted)
                throw new ArgumentException(nameof(dataContext));
        }

        private void MergeDelete()
        {
            Validate();

            this.diffSource2.ReadOnly = false;
            for (var i = 0; i < this.diffSource1.Items.Count; i++)
            {
                var item1 = this.diffSource1.Items[i];
                var item2 = this.diffSource2.Items[i];
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
            for (var i = 0; i < this.diffSource2.Items.Count; i++)
            {
                var item1 = this.diffSource1.Items[i];
                var item2 = this.diffSource2.Items[i];
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

        private void MergeModify()
        {
            Validate();

            for (var i = this.itemList.Count - 1; i >= 0; i--)
            {
                var item = this.itemList[i];
                if (item.DiffState2 == DiffState.Modified)
                {
                    var item2 = item.Item2;
                    var item1 = item.GetTarget1();
                    item1.CopyFrom(item2);
                }
            }

            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                if (item.DiffState2 == DiffState.Inserted && item.DiffState1 == DiffState.Imaginary)
                {
                    var item2 = item.Item2;
                    var item1 = item.Item1;
                    item1.CopyFrom(item2);
                }
            }

            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                if (item.DiffState1 == DiffState.Deleted && item.DiffState2 == DiffState.Imaginary)
                {
                    var item1 = item.Item1;
                    item1.EmptyFields();
                }
            }

            foreach (var item in this.itemList)
            {
                if (item.DiffState1 != DiffState.Unchanged)
                    return;
                if (item.DiffState2 != DiffState.Unchanged)
                    return;
            }

            this.diffSource1.ReadOnly = false;
            this.diffSource2.ReadOnly = false;
            this.diffSource1.DeleteItems();
            this.diffSource2.DeleteItems();
            this.diffSource1.AcceptChanges();
            this.diffSource2.AcceptChanges();
            this.diffSource1.IsDiffMode = false;
            this.diffSource2.IsDiffMode = false;
            this.diffSource1.ReadOnly = true;
            this.diffSource2.ReadOnly = true;
            this.isResolved = true;

            void Validate()
            {
                if (this.diffState != DiffState.Modified)
                    throw new Exception();
            }
        }

        private void InitializeItems()
        {
            this.diffSource1.ColumnChanged -= DiffSource1_ColumnChanged;
            this.diffSource2.ColumnChanged -= DiffSource2_ColumnChanged;
            this.diffSource1.ColumnDeleted -= DiffSource1_ColumnDeleted;
            this.diffSource2.ColumnDeleted -= DiffSource2_ColumnDeleted;

            this.diffSource1.ReadOnly = false;
            this.diffSource2.ReadOnly = false;

            if (this.unresolvedItemList.Any() == false && (this.dataTable1 == null || this.dataTable2 == null))
            {
                FillColumns(this.diffSource1, this.dataTable1 ?? this.dataTable2);
                FillColumns(this.diffSource2, this.dataTable2 ?? this.dataTable1);
            }
            else
            {
                FillColumns(this.diffSource1, this.diffTable1);
                FillColumns(this.diffSource2, this.diffTable2);
            }

            this.itemList.Clear();
            this.itemList.Capacity = this.diffSource1.Items.Count;
            for (var i = 0; i < this.diffSource1.Items.Count; i++)
            {
                var item = new DiffTemplateColumn(this, i);
                this.itemList.Add(item);
            }

            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                item.Update();
            }

            this.diffSource1.AcceptChanges();
            this.diffSource2.AcceptChanges();

            this.diffSource1.ColumnChanged += DiffSource1_ColumnChanged;
            this.diffSource2.ColumnChanged += DiffSource2_ColumnChanged;
            this.diffSource1.ColumnDeleted += DiffSource1_ColumnDeleted;
            this.diffSource2.ColumnDeleted += DiffSource2_ColumnDeleted;

            void FillColumns(CremaTemplate template, CremaDataTable diffTable)
            {
                var index = 0;
                template.Columns.Clear();
                foreach (var item in diffTable.Columns)
                {
                    var templateColumn = template.NewColumn();
                    templateColumn.TargetColumn = item;
                    if (DiffUtility.GetDiffState(item) != DiffState.Imaginary)
                    {
                        templateColumn.CopyFrom(item);
                        templateColumn.SetAttribute(DiffUtility.DiffIDKey, item.ColumnID);
                    }
                    else
                    {
                        templateColumn.EmptyFields();
                        templateColumn.SetAttribute(DiffUtility.DiffEnabledKey, false);
                    }
                    templateColumn.Index = index++;
                    template.Columns.Add(templateColumn);
                    var diffState = DiffUtility.GetDiffState(item);
                    templateColumn.SetDiffState(diffState);
                }
            }
        }

        private void DiffSource1_ColumnChanged(object sender, CremaTemplateColumnChangeEventArgs e)
        {
            if (e.Item.ItemState == DataRowState.Detached)
                return;

            if (e.PropertyName == string.Empty)
            {
                var index = e.Item.Index;
                if (index >= this.itemList.Count)
                {
                    this.itemList.Add(new DiffTemplateColumn(this, index));
                }
                else
                {
                    var item = this.itemList[index];
                    item.Item1 = e.Item;
                }
            }
        }

        private void DiffSource2_ColumnChanged(object sender, CremaTemplateColumnChangeEventArgs e)
        {
            if (e.Item.ItemState == DataRowState.Detached)
                return;

            if (e.PropertyName == string.Empty)
            {
                var index = e.Item.Index;
                if (index >= this.itemList.Count)
                {
                    this.itemList.Add(new DiffTemplateColumn(this, index));
                }
                else
                {
                    var item = this.itemList[index];
                    item.Item2 = e.Item;
                }
            }
        }

        private void DiffSource1_ColumnDeleted(object sender, CremaTemplateColumnChangeEventArgs e)
        {
            for (var i = 0; i < this.itemList.Count; i++)
            {
                if (this.itemList[i].Item1 == e.Item && this.itemList.Count >= this.SourceItem1.Items.Count)
                {
                    this.itemList.RemoveAt(i);
                    break;
                }
            }
        }

        private void DiffSource2_ColumnDeleted(object sender, CremaTemplateColumnChangeEventArgs e)
        {
            for (var i = 0; i < this.itemList.Count; i++)
            {
                if (this.itemList[i].Item2 == e.Item && this.itemList.Count >= this.SourceItem2.Items.Count)
                {
                    this.itemList.RemoveAt(i);
                    break;
                }
            }
        }

        private void InvokePropertyChangedEvent(params string[] propertyNames)
        {
            foreach (var item in propertyNames)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(item));
            }
        }

        private void SyncType(CremaDataType sourceType, CremaDataType destType)
        {
            if (destType == null)
                return;
            destType.IsDiffMode = true;
            foreach (var item in sourceType.Members)
            {
                if (item.InternalObject[DiffUtility.DiffIDKey] is Guid memberID && object.Equals(memberID, item.MemberID) == false)
                {
                    var destColumn = destType.Members[memberID];
                    destColumn.CopyFrom(item);
                }
            }

            foreach (var item in destType.Members.ToArray())
            {
                if (sourceType.Members.Contains(item.MemberID) == false)
                {
                    item.Delete();
                }
            }

            foreach (var item in sourceType.Members)
            {
                if (destType.Members.Contains(item.MemberID) == false)
                {
                    var destColumn = destType.NewMember();
                    destColumn.CopyFrom(item);
                    destType.Members.Add(destColumn);
                }
                else if (item.InternalObject[DiffUtility.DiffIDKey] is Guid memberID && object.Equals(memberID, item.MemberID) == true)
                {
                    destType.Members[item.MemberID].CopyFrom(item);
                }
            }

            for (var i = sourceType.Members.Count - 1; i <= 0; i--)
            {
                destType.Members[sourceType.Members[i].MemberID].Index = i;
            }
            destType.IsDiffMode = false;
        }

        private void DiffDataType_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is DiffDataType dataType)
            {
                if (e.PropertyName == nameof(DiffDataType.IsResolved) && dataType.IsResolved == true)
                {
                    if (dataType.DiffState == DiffState.Inserted)
                    {
                        dataType.SourceItem1.CopyTo(this.diffTable1.DataSet);
                    }
                    else if (dataType.DiffState == DiffState.Deleted)
                    {
                        this.SyncType(dataType.SourceItem1, this.diffTable1.DataSet.Types[dataType.SourceItem1.TypeID]);
                        this.diffTable1.DataSet.Types.Remove(dataType.SourceItem1.TypeName);
                    }
                    else if (dataType.DiffState == DiffState.Modified)
                    {
                        this.SyncType(dataType.SourceItem1, this.diffTable1.DataSet.Types[dataType.SourceItem1.TypeID]);
                        this.SyncType(dataType.SourceItem2, this.diffTable2.DataSet.Types[dataType.SourceItem2.TypeID]);
                    }

                    this.unresolvedItemList.Remove(sender);
                    if (this.unresolvedItemList.Any() == false)
                    {
                        this.Diff();
                    }
                }
            }
        }

        private void AttachEventHandler(CremaTemplate template, CremaDataTable diffTable)
        {
            var query = from item in diffTable.Columns
                        where item.CremaType != null
                        let diffType = item.CremaType.ExtendedProperties[typeof(DiffDataType)] as DiffDataType
                        where diffType != null && diffType.IsResolved == false
                        select diffType;

            foreach (var item in query)
            {
                if (this.unresolvedItemList.Contains(item) == false)
                {
                    this.unresolvedItemList.Add(item);
                    item.PropertyChanged += DiffDataType_PropertyChanged;
                }
            }
        }

        private static CremaTemplate Create(CremaDataTable dataTable)
        {
            var template = new CremaTemplate() { TargetTable = dataTable };

            template.Attributes.Add(DiffUtility.DiffStateKey, typeof(string), DBNull.Value);
            template.Attributes.Add(DiffUtility.DiffFieldsKey, typeof(string), DBNull.Value);
            template.Attributes.Add(DiffUtility.DiffIDKey, typeof(Guid), DBNull.Value);
            template.Attributes.Add(DiffUtility.DiffEnabledKey, typeof(bool), true);

            template.Tags = dataTable.Tags;
            template.Comment = dataTable.Comment;
            template.IsDiffMode = true;

            return template;
        }

        internal void Diff()
        {
            this.InitializeItems();

            if (this.dataTable1 == null)
            {
                this.diffSource1.ReadOnly = true;
                this.diffSource2.ReadOnly = true;
                this.diffSource1.SetDiffState(DiffState.Imaginary);
                this.diffSource2.SetDiffState(DiffState.Inserted);
                this.diffState = DiffState.Inserted;
                this.isResolved = this.unresolvedItemList.Any() == false;
            }
            else if (this.dataTable2 == null)
            {
                this.diffSource1.ReadOnly = true;
                this.diffSource2.ReadOnly = true;
                this.diffSource1.SetDiffState(DiffState.Deleted);
                this.diffSource2.SetDiffState(DiffState.Imaginary);
                this.diffState = DiffState.Deleted;
                this.isResolved = this.unresolvedItemList.Any() == false;
            }
            else if (this.VerifyModified() == true)
            {
                this.diffSource1.ReadOnly = this.MergeType == DiffMergeTypes.ReadOnly1 || this.unresolvedItemList.Any();
                this.diffSource2.ReadOnly = this.MergeType == DiffMergeTypes.ReadOnly2 || this.unresolvedItemList.Any();
                this.diffSource1.SetDiffState(DiffState.Modified);
                this.diffSource2.SetDiffState(DiffState.Modified);
                this.diffState = DiffState.Modified;
                this.isResolved = false;
            }
            else
            {
                this.diffSource1.AcceptChanges();
                this.diffSource2.AcceptChanges();
                this.diffSource1.ReadOnly = true;
                this.diffSource2.ReadOnly = true;
                this.diffSource1.SetDiffState(DiffState.Unchanged);
                this.diffSource2.SetDiffState(DiffState.Unchanged);
                this.diffState = DiffState.Unchanged;
                this.isResolved = this.unresolvedItemList.Any() == false;
            }
            this.InvokePropertyChangedEvent(nameof(this.IsResolved), nameof(this.DiffState));
        }

        internal HashSet<object> ItemSet => this.itemSet;
    }
}
