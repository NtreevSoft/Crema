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
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Diff
{
    public sealed class DiffDataType : INotifyPropertyChanged
    {
        private readonly CremaDataType diffSource1;
        private readonly CremaDataType diffSource2;
        private readonly CremaDataType dataType1;
        private readonly CremaDataType dataType2;
        private DiffMergeTypes mergeType;
        private bool dummy1;
        private bool dummy2;
        private readonly List<DiffDataTypeMember> itemList = new List<DiffDataTypeMember>();
        private DiffState diffState;
        private bool isResolved;
        private string header1;
        private string header2;
        private readonly HashSet<object> itemSet = new HashSet<object>();
        //private bool isSame;

        [Obsolete]
        public DiffDataType(CremaDataType dataType1, CremaDataType dataType2, DiffMergeTypes mergeType)
        {
            this.diffSource1 = dataType1 == null ? new CremaDataType() : new CremaDataType(dataType1.TypeName, dataType1.CategoryPath);
            this.diffSource2 = dataType2 == null ? new CremaDataType() : new CremaDataType(dataType2.TypeName, dataType2.CategoryPath);
            this.mergeType = mergeType;
            this.diffSource1.ExtendedProperties[typeof(DiffDataType)] = this;
            this.diffSource2.ExtendedProperties[typeof(DiffDataType)] = this;
            this.diffSource1.InternalIsFlag = dataType1.IsFlag;
            this.diffSource1.InternalComment = dataType1.Comment;
            this.diffSource1.InternalTypeID = dataType1.TypeID;
            this.diffSource1.Tags = dataType1.Tags;
            this.diffSource1.InternalCreationInfo = dataType1.CreationInfo;
            this.diffSource1.InternalModificationInfo = dataType1.ModificationInfo;
            this.diffSource2.InternalIsFlag = dataType2.IsFlag;
            this.diffSource2.InternalComment = dataType2.Comment;
            this.diffSource2.InternalTypeID = dataType2.TypeID;
            this.diffSource2.Tags = dataType2.Tags;
            this.diffSource2.InternalCreationInfo = dataType2.CreationInfo;
            this.diffSource2.InternalModificationInfo = dataType2.ModificationInfo;

            this.dataType1 = dataType1;
            this.dataType2 = dataType2;
            //this.isSame = dataType1.HashValue != null && dataType1.HashValue == dataType2.HashValue;
        }

        internal DiffDataType(CremaDataType diffType1, CremaDataType diffType2, CremaDataType dataType1, CremaDataType dataType2)
        {
            this.diffSource1 = diffType1;
            this.diffSource2 = diffType2;
            this.diffSource1.ExtendedProperties[typeof(DiffDataType)] = this;
            this.diffSource2.ExtendedProperties[typeof(DiffDataType)] = this;
            this.diffSource1.InternalIsFlag = dataType1 == null ? dataType2.IsFlag : dataType1.IsFlag;
            this.diffSource1.InternalComment = dataType1 == null ? dataType2.Comment : dataType1.Comment;
            this.diffSource1.InternalTypeID = dataType1 == null ? dataType2.TypeID : dataType1.TypeID;
            this.diffSource1.InternalCreationInfo = dataType1 == null ? dataType2.CreationInfo : dataType1.CreationInfo;
            this.diffSource1.InternalModificationInfo = dataType1 == null ? dataType2.ModificationInfo : dataType1.ModificationInfo;
            this.diffSource2.InternalIsFlag = dataType2 == null ? dataType1.IsFlag : dataType2.IsFlag;
            this.diffSource2.InternalComment = dataType2 == null ? dataType1.Comment : dataType2.Comment;
            this.diffSource2.InternalTypeID = dataType2 == null ? dataType1.TypeID : dataType2.TypeID;
            this.diffSource2.InternalCreationInfo = dataType2 == null ? dataType1.CreationInfo : dataType2.CreationInfo;
            this.diffSource2.InternalModificationInfo = dataType2 == null ? dataType1.ModificationInfo : dataType2.ModificationInfo;
            this.dummy1 = this.diffSource1.TypeName.StartsWith(DiffUtility.DiffDummyKey);
            this.dummy2 = this.diffSource2.TypeName.StartsWith(DiffUtility.DiffDummyKey);

            this.dataType1 = dataType1;
            this.dataType2 = dataType2;
            //this.isSame = dataType1.HashValue != null && dataType1.HashValue == dataType2.HashValue;
        }

        public override string ToString()
        {
            if (this.diffSource1.TypeName != this.diffSource2.TypeName)
                return $"{this.diffSource1.TypeName} => {this.diffSource2.TypeName}";
            return this.diffSource1.TypeName;
        }

        public void AcceptChanges()
        {
            this.Resolve();
        }

        public void RejectChanges()
        {
            this.diffSource1.RejectChanges();
            this.diffSource2.RejectChanges();
        }

        public bool HasChanges()
        {
            if (this.diffSource1.HasChanges(true) == true)
                return true;
            if (this.diffSource2.HasChanges(true) == true)
                return true;
            if (this.diffSource1.TypeName != this.dataType1.TypeName)
                return true;
            if (this.diffSource2.TypeName != this.dataType2.TypeName)
                return true;
            if (this.diffSource1.Tags != this.dataType1.Tags)
                return true;
            if (this.diffSource2.Tags != this.dataType2.Tags)
                return true;
            if (this.diffSource1.IsFlag != this.dataType1.IsFlag)
                return true;
            if (this.diffSource2.IsFlag != this.dataType2.IsFlag)
                return true;
            if (this.diffSource1.Comment != this.dataType1.Comment)
                return true;
            if (this.diffSource2.Comment != this.dataType2.Comment)
                return true;
            return false;
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
                this.diffSource1.MemberChanged -= DiffSource1_MemberChanged;
                this.diffSource2.MemberChanged -= DiffSource2_MemberChanged;
                this.diffSource1.MemberDeleted -= DiffSource1_MemberDeleted;
                this.diffSource2.MemberDeleted -= DiffSource2_MemberDeleted;
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
            this.diffSource1.MemberChanged -= DiffSource1_MemberChanged;
            this.diffSource2.MemberChanged -= DiffSource2_MemberChanged;
            this.diffSource1.MemberDeleted -= DiffSource1_MemberDeleted;
            this.diffSource2.MemberDeleted -= DiffSource2_MemberDeleted;
            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                item.Update();
            }
            this.diffSource1.MemberChanged += DiffSource1_MemberChanged;
            this.diffSource2.MemberChanged += DiffSource2_MemberChanged;
            this.diffSource1.MemberDeleted += DiffSource1_MemberDeleted;
            this.diffSource2.MemberDeleted += DiffSource2_MemberDeleted;
        }

        public CremaDataType SourceItem1
        {
            get { return this.diffSource1; }
        }

        public CremaDataType SourceItem2
        {
            get { return this.diffSource2; }
        }

        public string ItemName1
        {
            get
            {
                if (this.dummy1 == true)
                    return this.diffSource1.TypeName.Replace(DiffUtility.DiffDummyKey, string.Empty);
                return this.diffSource1.TypeName;
            }
            set
            {
                if (this.dummy1 == true && this.diffSource2.TypeName != value)
                    this.diffSource1.TypeName = DiffUtility.DiffDummyKey + value;
                else
                    this.diffSource1.TypeName = value;
                this.InvokePropertyChangedEvent(nameof(this.ItemName1));
            }
        }

        public string ItemName2
        {
            get
            {
                if (this.dummy2 == true)
                    return this.diffSource2.TypeName.Replace(DiffUtility.DiffDummyKey, string.Empty);
                return this.diffSource2.TypeName;
            }
            set
            {
                if (this.dummy2 == true && this.diffSource1.TypeName != value)
                    this.diffSource2.TypeName = DiffUtility.DiffDummyKey + value;
                else
                    this.diffSource2.TypeName = value;
                this.InvokePropertyChangedEvent(nameof(this.ItemName2));
            }
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

        public bool IsFlag1
        {
            get { return this.diffSource1.IsFlag; }
            set
            {
                this.diffSource1.IsFlag = value;
                this.InvokePropertyChangedEvent(nameof(this.IsFlag1));
            }
        }

        public bool IsFlag2
        {
            get { return this.diffSource2.IsFlag; }
            set
            {
                this.diffSource2.IsFlag = value;
                this.InvokePropertyChangedEvent(nameof(this.IsFlag2));
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

        public DiffDataSet DiffSet
        {
            get;
            internal set;
        }

        public IReadOnlyList<DiffDataTypeMember> Items
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

        public DiffMergeTypes MergeType
        {
            get
            {
                if (this.DiffSet != null)
                    return this.DiffSet.MergeType;
                return this.mergeType;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool VerifyModified()
        {
            if (this.diffSource1.TypeName != this.diffSource2.TypeName)
                return true;
            if (this.diffSource1.IsFlag != this.diffSource2.IsFlag)
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
            if (this.diffState == DiffState.Modified)
            {
                if (this.diffSource1.TypeName != this.diffSource2.TypeName)
                    throw new Exception();
                if (this.diffSource1.IsFlag != this.diffSource2.IsFlag)
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

        private void MergeDelete()
        {
            Validate();

            var dataSet = this.diffSource1.DataSet;
            foreach (var item in this.diffSource1.ReferencedColumns.ToArray())
            {
                item.CremaType = null;
            }

            this.diffSource2.ReadOnly = false;
            for (var i = 0; i < this.diffSource1.Items.Count; i++)
            {
                var item1 = this.diffSource1.Items[i];
                var item2 = this.diffSource2.Items[i];
                item2.CopyFrom(item1);
            }
            this.diffSource2.ReadOnly = true;
            this.isResolved = true;

            if (this.DiffSet != null && this.DiffSet.DataSet1.Types.Contains(this.diffSource1))
                this.DiffSet.DataSet1.Types.Remove(this.diffSource1);

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

            if (this.DiffSet != null)
            {
                var dataSet = this.DiffSet.DataSet1;
                this.diffSource1.CopyTo(this.DiffSet.DataSet1);
            }

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
            this.diffSource1.MemberChanged -= DiffSource1_MemberChanged;
            this.diffSource2.MemberChanged -= DiffSource2_MemberChanged;
            this.diffSource1.MemberDeleted -= DiffSource1_MemberDeleted;
            this.diffSource2.MemberDeleted -= DiffSource2_MemberDeleted;
            DiffInternalUtility.InitializeMembers(this.diffSource1, this.diffSource2, this.dataType1, this.dataType2);
            {
                this.itemList.Clear();
                this.itemList.Capacity = this.diffSource1.Items.Count;
                for (var i = 0; i < this.diffSource1.Items.Count; i++)
                {
                    var item = new DiffDataTypeMember(this, i);
                    this.itemList.Add(item);
                }

                for (var i = 0; i < this.itemList.Count; i++)
                {
                    var item = this.itemList[i];
                    item.Update();
                }

                this.diffSource1.AcceptChanges();
                this.diffSource2.AcceptChanges();
            }
            this.diffSource1.MemberChanged += DiffSource1_MemberChanged;
            this.diffSource2.MemberChanged += DiffSource2_MemberChanged;
            this.diffSource1.MemberDeleted += DiffSource1_MemberDeleted;
            this.diffSource2.MemberDeleted += DiffSource2_MemberDeleted;
        }

        private void DiffSource1_MemberChanged(object sender, CremaDataTypeMemberChangeEventArgs e)
        {
            if (e.Item.ItemState == DataRowState.Detached)
                return;

            if (e.PropertyName == string.Empty)
            {
                var index = e.Item.Index;
                if (index >= this.itemList.Count)
                {
                    this.itemList.Add(new DiffDataTypeMember(this, index));
                }
                else
                {
                    var item = this.itemList[index];
                    item.Item1 = e.Item;
                }
            }
        }

        private void DiffSource2_MemberChanged(object sender, CremaDataTypeMemberChangeEventArgs e)
        {
            if (e.Item.ItemState == DataRowState.Detached)
                return;

            if (e.PropertyName == string.Empty)
            {
                var index = e.Item.Index;
                if (index >= this.itemList.Count)
                {
                    this.itemList.Add(new DiffDataTypeMember(this, index));
                }
                else
                {
                    var item = this.itemList[index];
                    item.Item2 = e.Item;
                }
            }
        }

        private void DiffSource1_MemberDeleted(object sender, CremaDataTypeMemberChangeEventArgs e)
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

        private void DiffSource2_MemberDeleted(object sender, CremaDataTypeMemberChangeEventArgs e)
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

        internal static CremaDataType Create(CremaDataSet dataSet, string typeName)
        {
            var dataType = dataSet == null ? new CremaDataType(typeName) : dataSet.Types.Add(typeName);

            dataType.Attributes.Add(DiffUtility.DiffStateKey, typeof(string), $"{DiffState.Imaginary}");
            dataType.Attributes.Add(DiffUtility.DiffFieldsKey, typeof(string), DBNull.Value);
            dataType.Attributes.Add(DiffUtility.DiffIDKey, typeof(Guid), DBNull.Value);
            dataType.Attributes.Add(DiffUtility.DiffEnabledKey, typeof(bool), true);

            dataType.IsDiffMode = true;

            return dataType;
        }

        internal void Diff()
        {
            this.InitializeItems();

            if (this.dataType1 == null)
            {
                this.diffSource1.IsDiffMode = false;
                this.diffSource2.IsDiffMode = false;
                this.diffSource1.ReadOnly = true;
                this.diffSource2.ReadOnly = true;
                this.diffSource1.SetDiffState(DiffState.Imaginary);
                this.diffSource2.SetDiffState(DiffState.Inserted);
                this.diffState = DiffState.Inserted;
                this.isResolved = true;
            }
            else if (this.dataType2 == null)
            {
                this.diffSource1.IsDiffMode = false;
                this.diffSource2.IsDiffMode = false;
                this.diffSource1.ReadOnly = true;
                this.diffSource2.ReadOnly = true;
                this.diffSource1.SetDiffState(DiffState.Deleted);
                this.diffSource2.SetDiffState(DiffState.Imaginary);
                this.diffState = DiffState.Deleted;
                this.isResolved = true;
            }
            else if (this.VerifyModified() == true)
            {
                this.diffSource1.ReadOnly = this.MergeType == DiffMergeTypes.ReadOnly1;
                this.diffSource2.ReadOnly = this.MergeType == DiffMergeTypes.ReadOnly2;
                this.diffSource1.SetDiffState(DiffState.Modified);
                this.diffSource2.SetDiffState(DiffState.Modified);
                this.diffState = DiffState.Modified;
                this.isResolved = false;
            }
            else
            {
                this.diffSource1.IsDiffMode = false;
                this.diffSource2.IsDiffMode = false;
                this.diffSource1.ReadOnly = true;
                this.diffSource2.ReadOnly = true;
                this.diffSource1.SetDiffState(DiffState.Unchanged);
                this.diffSource2.SetDiffState(DiffState.Unchanged);
                this.diffState = DiffState.Unchanged;
                this.isResolved = true;
            }
            this.InvokePropertyChangedEvent(nameof(this.IsResolved), nameof(this.DiffState));
        }

        internal HashSet<object> ItemSet => this.itemSet;
    }
}
