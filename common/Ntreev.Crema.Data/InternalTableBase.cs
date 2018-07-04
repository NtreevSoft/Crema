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
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data
{
    abstract class InternalTableBase : DataTable, INotifyPropertyChanged
    {
        private const string attributeNameString = "Attribute";

        internal readonly InternalAttribute attributeCreator;
        internal readonly InternalAttribute attributeCreatedDateTime;
        internal readonly InternalAttribute attributeModifier;
        internal readonly InternalAttribute attributeModifiedDateTime;
        private object target;
        private string categoryPath;

        private readonly Stack<DataRow> rowEventStack = new Stack<DataRow>();
        private readonly FieldStack<bool> omitSignatureDate = new FieldStack<bool>();
        private bool isReadOnly;
        private bool isDiffMode;
        private bool isLoading;

        private string parentName;
        private string templateNamespace;
        private InternalTableBase parent;
        private InternalTableBase templatedParent;
        private readonly ObservableCollection<InternalTableBase> childItems = new ObservableCollection<InternalTableBase>();
        private readonly ObservableCollection<InternalTableBase> derivedItems = new ObservableCollection<InternalTableBase>();
        private readonly ReadOnlyObservableCollection<InternalTableBase> childItems2;
        private readonly ReadOnlyObservableCollection<InternalTableBase> derivedItems2;
        private readonly List<InternalAttribute> attributeList = new List<InternalAttribute>();
        private InternalRelation columnRelation;
        private InternalRelation parentRelation;

        private SignatureDateProvider signatureDateProvider;
        private SignatureDate signatureDate;

        private FieldStack<bool> acceptChangesStack = new FieldStack<bool>();

        protected InternalTableBase(string name, string categoryPath)
            : base(name)
        {
            Validate();
            this.CaseSensitive = true;
            this.childItems2 = new ReadOnlyObservableCollection<InternalTableBase>(this.childItems);
            this.derivedItems2 = new ReadOnlyObservableCollection<InternalTableBase>(this.derivedItems);
            this.Columns.CollectionChanged += Columns_CollectionChanged;

            this.attributeCreator = new InternalAttribute(CremaSchema.Creator, typeof(string))
            {
                DefaultValue = string.Empty,
                ReadOnly = true,
            };
            this.Columns.Add(this.attributeCreator);

            this.attributeCreatedDateTime = new InternalAttribute(CremaSchema.CreatedDateTime, typeof(DateTime))
            {
                DefaultValue = DateTime.MinValue,
                ReadOnly = true,
                DateTimeMode = DataSetDateTime.Utc,
            };
            this.Columns.Add(this.attributeCreatedDateTime);

            this.attributeModifier = new InternalAttribute(CremaSchema.Modifier, typeof(string))
            {
                DefaultValue = string.Empty,
                ReadOnly = true,
            };
            this.Columns.Add(attributeModifier);

            this.attributeModifiedDateTime = new InternalAttribute(CremaSchema.ModifiedDateTime, typeof(DateTime))
            {
                DefaultValue = DateTime.MinValue,
                ReadOnly = true,
                DateTimeMode = DataSetDateTime.Utc,
            };
            this.Columns.Add(this.attributeModifiedDateTime);

            this.categoryPath = categoryPath;
            this.BuildNamespace();

            void Validate()
            {
                if (string.IsNullOrEmpty(name) == false)
                    CremaDataSet.ValidateName(name);
                NameValidator.ValidateCategoryPath(categoryPath);
            }
        }

        public new void AcceptChanges()
        {
            using (this.acceptChangesStack.Set(true))
            {
                base.AcceptChanges();
            }
        }

        public new void RejectChanges()
        {
            using (this.acceptChangesStack.Set(true))
            {
                base.RejectChanges();
            }
        }

        public void RefreshRelationID()
        {
            if (this.ColumnRelation == null)
                return;

            this.ColumnRelation.InternalReadOnly = false;
            try
            {
                for (var i = this.Rows.Count - 1; i >= 0; i--)
                {
                    var item = this.Rows[i] as InternalRowBase;
                    if (item.RowState == DataRowState.Deleted || item.RelationID != null)
                        continue;
                    this.RowEventStack.Push(item);
                    if (this.PrimaryKey.Any() == true || item.RelationID == null)
                        item.RelationID = this.GenerateRelationID(item);
                    this.RowEventStack.Pop();
                }
            }
            finally
            {
                this.ColumnRelation.InternalReadOnly = true;
            }
        }

        public void AddAttribute(InternalAttribute attribute)
        {
            this.ValidateAddAttribute(attribute);
            this.Sign();
            this.AddAttributeInternal(attribute);
            foreach (var item in this.DerivedItems.ToArray())
            {
                var derivedAttribute = new InternalAttribute();
                attribute.CopyTo(derivedAttribute);
                item.AddAttributeInternal(derivedAttribute);
            }
        }

        public InternalAttribute AddAttribute()
        {
            var columnNames = from DataColumn item in this.Columns select item.ColumnName;
            var columnName = NameUtility.GenerateNewName(attributeNameString, columnNames.ToArray());
            return this.AddAttribute(columnName);
        }

        public InternalAttribute AddAttribute(string attributeName)
        {
            this.ValidateAddAttribute(attributeName);
            this.Sign();
            var attribute = this.AddAttributeInternal(attributeName);
            foreach (var item in this.DerivedItems.ToArray())
            {
                item.AddAttributeInternal(attributeName);
            }
            return attribute;
        }

        public InternalAttribute AddAttribute(string attributeName, Type dataType)
        {
            this.ValidateAddAttribute(attributeName, dataType);
            this.Sign();
            var attribute = this.AddAttributeInternal(attributeName, dataType);
            foreach (var item in this.DerivedItems.ToArray())
            {
                item.AddAttributeInternal(attributeName, dataType);
            }
            return attribute;
        }

        public void RemoveAttribute(InternalAttribute attribute)
        {
            this.ValidateRemoveAttribute(attribute);
            this.Sign();
            this.RemoveAttributeInternal(attribute);
            foreach (var item in this.DerivedItems.ToArray())
            {
                item.RemoveAttributeInternal(attribute.AttributeName);
            }
        }

        public void RemoveAttribute(string columnName)
        {
            this.ValidateRemoveAttribute(columnName);
            this.Sign();
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.RemoveAttributeInternal(columnName);
            }
        }

        public bool CanRemoveAttribute(InternalAttribute attribute)
        {
            if (this.TemplateNamespace != string.Empty)
                return false;
            return this.Columns.CanRemove(attribute);
        }

        public void ClearAttributes()
        {
            this.ValidateClearAttributes();
            this.Sign();
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.ClearAttributesInternal();
            }
        }

        public void ValidateSetLocalName(string value)
        {
            InternalSetBase.ValidateSetLocalName(this.DataSet, value);
            if (this.Parent != null && this.TemplateNamespace != string.Empty)
                throw new ArgumentException("템플릿 이름 변경 불가");
            if (this.Parent != null && this.Parent.LocalName == value)
                throw new ArgumentException($"'{value}'은(는) 부모 테이블의 이름이기 때문에 사용할 수 없습니다.", nameof(value));
            if (this.Parent != null && this.Parent.ChildItems.Any(item => item.LocalName == value) == true)
                throw new ArgumentException($"'{value}'은(는) 부모 테이블의 자식 테이블 이름으로 사용중이기 때문에 사용할 수 없습니다.", nameof(value));
            if (this.Parent == null && this.TemplateNamespace != string.Empty && this.TemplatedParentName == value)
                throw new ArgumentException($"'{value}'은(는) 기본 테이블의 이름으로 사용중이기 때문에 사용할 수 없습니다.", nameof(value));

            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateSetLocalName(value);
            }
        }

        public void ValidateAddAttribute(InternalAttribute attribute)
        {
            this.ValidateInvoke(nameof(this.AddAttribute));
            this.OnValidateAddAttribute(attribute);
        }

        public void ValidateAddAttribute(string columnName)
        {
            this.ValidateInvoke(nameof(this.AddAttribute));
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateAddAttribute(columnName);
            }
        }

        public void ValidateAddAttribute(string columnName, Type dataType)
        {
            this.ValidateInvoke(nameof(this.AddAttribute));
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateAddAttribute(columnName, dataType);
            }
        }

        public void ValidateRemoveAttribute(InternalAttribute attribute)
        {
            this.ValidateInvoke(nameof(this.RemoveAttribute));
            this.OnValidateRemoveAttribute(attribute);
        }

        public void ValidateRemoveAttribute(string columnName)
        {
            this.ValidateInvoke(nameof(this.RemoveAttribute));
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateRemoveAttribute(columnName);
            }
        }

        public void ValidateClearAttributes()
        {
            this.ValidateInvoke(nameof(this.ClearAttributes));
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.OnValidateClearAttributes();
            }
        }

        public new virtual void BeginLoadData()
        {
            if (this.isLoading == true)
                throw new InvalidOperationException(nameof(BeginLoadData));
            base.BeginLoadData();
            this.isLoading = true;
        }

        public new virtual void EndLoadData()
        {
            if (this.isLoading == false)
                throw new InvalidOperationException(nameof(EndLoadData));
            base.EndLoadData();
            this.isLoading = false;
        }

        private void AddAttributeInternal(InternalAttribute value)
        {
            this.Columns.Add(value);
        }

        private InternalAttribute AddAttributeInternal(string columnName)
        {
            var dataColumn = new InternalAttribute()
            {
                InternalColumnName = columnName,
            };
            this.Columns.Add(dataColumn);
            return dataColumn;
        }

        private InternalAttribute AddAttributeInternal(string columnName, Type dataType)
        {
            var dataColumn = new InternalAttribute()
            {
                InternalColumnName = columnName,
                InternalDataType = dataType,
            };
            this.Columns.Add(dataColumn);
            return dataColumn;
        }

        private void RemoveAttributeInternal(InternalAttribute attribute)
        {
            this.Columns.Remove(attribute);
        }

        private void RemoveAttributeInternal(string attributeName)
        {
            this.Columns.Remove(attributeName);
        }

        private void ClearAttributesInternal()
        {

        }

        public void ValidateSetCategoryPath(string value)
        {
            this.ValidateSetProperty(nameof(this.CategoryPath));
            foreach (var item in this.FamilyItems.ToArray())
            {
                item.OnValidateSetCategoryPath(value);
            }
        }

        public void BuildNamespace()
        {
            this.Namespace = this.BaseNamespace + this.CategoryPath + this.Name;
            this.InvokePropertyChangedEvent(nameof(this.Namespace));
        }

        public InternalTableBase GetChild(string localName)
        {
            foreach (var item in this.childItems)
            {
                if (item.LocalName == localName)
                    return item;
            }
            return null;
        }

        public string GenerateRelationID(InternalRowBase dataRow)
        {
            return Guid.NewGuid().ToString();
        }

        public void AddKey(InternalColumnBase dataColumn)
        {
            var keys = this.PrimaryKey.ToList();
            var oldKeys = this.PrimaryKey;

            keys.Add(dataColumn);

            try
            {
                this.PrimaryKey = keys.ToArray();
                if (this.parent == null)
                {
                    this.RefreshRelationID();
                }
            }
            catch
            {
                this.PrimaryKey = oldKeys;
                throw;
            }
        }

        public void RemoveKey(InternalColumnBase dataColumn)
        {
            var keys = this.PrimaryKey.ToList();
            var oldKeys = this.PrimaryKey;

            keys.Remove(dataColumn);

            try
            {
                this.PrimaryKey = keys.ToArray();
                if (this.parent == null)
                {
                    this.RefreshRelationID();
                }
            }
            catch
            {
                this.PrimaryKey = oldKeys;
                throw;
            }
        }

        public SignatureDateProvider SignatureDateProvider
        {
            get
            {
                if (this.DataSet is InternalSetBase dataSet)
                    return dataSet.SignatureDateProvider;
                return this.signatureDateProvider ?? SignatureDateProvider.Default;
            }
            set { this.signatureDateProvider = value; }
        }

        public virtual bool IsLoading
        {
            get
            {
                if (this.isLoading == true)
                    return true;
                if (this.Parent is InternalTableBase parent && parent.IsLoading == true)
                    return true;
                if (this.DataSet is InternalDataSet dataSet)
                    return dataSet.IsLoading;
                return false;
            }
        }

        public object Target
        {
            get { return this.target; }
            set { this.target = value; }
        }

        public bool HasChanges
        {
            get
            {
                for (var i = 0; i < this.Rows.Count; i++)
                {
                    var row = this.Rows[i];
                    if (row.RowState != DataRowState.Unchanged)
                        return true;
                }
                return false;
            }
        }

        public IEnumerable<DataRelation> GetChildRelations()
        {
            if (this.DataSet != null)
            {
                foreach (var item in this.DataSet.Relations)
                {
                    if (item is DataRelation relation && relation.ParentTable == this)
                        yield return relation;
                }
            }
        }

        public bool OmitSignatureDate
        {
            get
            {
                if (this.IsLoading == true)
                    return true;
                if (this.isDiffMode == true)
                    return true;
                if (this.templatedParent != null)
                    return this.templatedParent.OmitSignatureDate;
                return this.omitSignatureDate;
            }
            set
            {
                this.omitSignatureDate.Field = value;
            }
        }

        public bool ReadOnly
        {
            get { return this.isReadOnly; }
            set { this.isReadOnly = value; }
        }

        public bool IsDiffMode
        {
            get { return this.isDiffMode; }
            set
            {
                if (this.isDiffMode == value)
                    return;

                this.isDiffMode = value;
                if (this.isDiffMode == true)
                {
                    this.SetDiffMode();
                }
                else
                {
                    this.SetNormalMode();
                }
            }
        }

        public InternalTableBase Parent
        {
            get { return this.InternalParent; }
            set
            {
                if (this.InternalParent == value)
                    return;

                this.ValidateSetParent(value);

                this.InternalParent = value;
            }
        }

        public string ParentName
        {
            get { return this.parentName ?? string.Empty; }
        }

        public IList<InternalAttribute> AttributeList
        {
            get { return this.attributeList; }
        }

        public ReadOnlyObservableCollection<InternalTableBase> ChildItems
        {
            get { return this.childItems2; }
        }

        public IEnumerable<InternalTableBase> FamilyItems
        {
            get
            {
                yield return this;
                foreach (var item in this.ChildItems)
                {
                    yield return item;
                }
            }
        }

        public InternalTableBase TemplatedParent
        {
            get { return this.InternalTemplatedParent; }
            set
            {
                this.InternalTemplatedParent = value;

                foreach (var item in this.ChildItems)
                {
                    item.InternalTemplatedParent = value?.GetChild(item.LocalName);
                }
            }
        }

        public string TemplateNamespace
        {
            get { return this.templateNamespace ?? string.Empty; }
            set
            {
                this.templateNamespace = value;
                foreach (var item in this.ChildItems)
                {
                    item.templateNamespace = templateNamespace + "." + item.LocalName;
                }
            }
        }

        public string TemplatedParentName
        {
            get
            {
                if (this.TemplateNamespace == string.Empty)
                    return string.Empty;

                return InternalDataSet.GetTableName(this.BaseNamespace, this.TemplateNamespace);
            }
        }

        public ReadOnlyObservableCollection<InternalTableBase> DerivedItems
        {
            get { return this.derivedItems2; }
        }

        public IEnumerable<InternalTableBase> SiblingItems
        {
            get
            {
                yield return this;
                foreach (var item in this.derivedItems)
                {
                    yield return item;
                }
            }
        }

        public string CategoryPath
        {
            get { return this.InternalCategoryPath ?? PathUtility.Separator; }
            set
            {
                if (this.InternalCategoryPath == value)
                    return;
                this.ValidateSetCategoryPath(value);
                var signatureDate = this.Sign();
                foreach (var item in this.FamilyItems.ToArray())
                {
                    item.InternalCategoryPath = value;
                    item.InvokePropertyChangedEvent(nameof(this.CategoryPath));
                }
            }
        }

        public string Name
        {
            get { return base.TableName; }
        }

        [Obsolete]
        public new string TableName
        {
            get { return base.TableName; }
        }

        public string LocalName
        {
            get
            {
                var items = StringUtility.Split(this.Name, '.');
                return items.LastOrDefault() ?? string.Empty;
            }
            set
            {
                this.ValidateSetLocalName(value);
                var childs = this.childItems.ToArray();
                var derivedTable = this.parent != null ? this.derivedItems.ToArray() : new InternalTableBase[] { };
                if (this.parent != null)
                    this.InternalName = this.parent.Name + "." + value;
                else
                    this.InternalName = value;

                foreach (var item in childs)
                {
                    item.InternalName = base.TableName + "." + item.LocalName;
                }

                foreach (var item in derivedTable)
                {
                    item.InternalName = item.Parent.Name + "." + value;
                }
            }
        }

        public string InternalName
        {
            get { return base.TableName; }
            set
            {
                base.TableName = value;
                var items = value != null ? StringUtility.Split(value, '.') : new string[] { };
                if (items.Length > 1)
                {
                    this.parentName = string.Join(".", items.Take(items.Length - 1));
                }
                else
                {
                    this.parentName = null;
                }

                this.BuildNamespace();
                this.InvokePropertyChangedEvent(nameof(this.LocalName));
            }
        }

        public InternalRelation ColumnRelation
        {
            get { return this.columnRelation; }
        }

        public InternalRelation ParentRelation
        {
            get { return this.parentRelation; }
        }

        public new InternalSetBase DataSet
        {
            get { return base.DataSet as InternalSetBase; }
        }

        public string InternalCategoryPath
        {
            get { return this.categoryPath; }
            set
            {
                this.categoryPath = value;
                this.BuildNamespace();
            }
        }

        public InternalTableBase InternalTemplatedParent
        {
            get { return this.templatedParent; }
            set
            {
                if (this.templatedParent != null)
                {
                    this.templatedParent.derivedItems.Remove(this);
                    this.templatedParent.PropertyChanged -= TemplatedParent_PropertyChanged;
                }

                this.templatedParent = value;

                if (this.templatedParent != null)
                {
                    this.templatedParent.derivedItems.Add(this);
                    this.templateNamespace = this.templatedParent.Namespace;
                    this.templatedParent.PropertyChanged += TemplatedParent_PropertyChanged;
                }
                else
                {
                    this.templateNamespace = null;
                }
            }
        }

        public InternalTableBase InternalParent
        {
            get { return this.parent; }
            set
            {
                if (this.parent != null)
                {
                    this.parent.RemoveChild(this);
                    this.InternalName = this.LocalName;
                    if (this.parentRelation != null)
                    {
                        this.Columns.Remove(this.parentRelation);
                        this.parentRelation = null;
                    }

                    if (this.parent.TemplatedParent != null)
                        this.InternalTemplatedParent = null;
                }

                this.parent = value;

                if (this.parent != null)
                {
                    this.parent.AddChild(this);
                    this.InternalName = $"{this.parent.Name}.{this.LocalName}";

                    if (this.parentRelation == null)
                    {
                        this.parentRelation = new InternalRelation(CremaSchema.__ParentID__, typeof(string));
                        this.Columns.Add(this.parentRelation);
                    }

                    if (this.parent.TemplatedParent != null)
                        this.InternalTemplatedParent = this.parent.TemplatedParent.GetChild(this.LocalName);
                }
            }
        }

        public SignatureDate SignatureDate
        {
            get { return this.signatureDate; }
        }

        public object Editor
        {
            get; set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void AddChild(InternalTableBase child)
        {
            if (this.childItems.Any() == false)
            {
                this.columnRelation = new InternalRelation(CremaSchema.__RelationID__, typeof(string)) { ReadOnly = true, };
                this.Columns.Add(this.columnRelation);
                this.RefreshRelationID();
                this.columnRelation.InternalUnique = true;
                this.columnRelation.InternalAllowDBNull = false;
            }
            this.childItems.Add(child);
        }

        private void RemoveChild(InternalTableBase child)
        {
            this.childItems.Remove(child);

            if (this.childItems.Any() == false)
            {
                if (FindConstraint() is Constraint constraint)
                {
                    this.Constraints.Remove(constraint);
                }
                this.Columns.Remove(this.columnRelation);
                this.columnRelation = null;
            }

            Constraint FindConstraint()
            {
                foreach (var item in this.Constraints)
                {
                    if (item is UniqueConstraint uniqueConstraint)
                    {
                        for (var i = 0; i < uniqueConstraint.Columns.Length; i++)
                        {
                            if (uniqueConstraint.Columns[i] == this.columnRelation)
                            {
                                return uniqueConstraint;
                            }
                        }
                    }
                }
                return null;
            }
        }

        protected SignatureDate Sign()
        {
            this.ValidateSign();
            var signatureDate = this.SignatureDateProvider.Provide();
            foreach (var item in this.SiblingItems.ToArray())
            {
                item.signatureDate = signatureDate;
            }
            return signatureDate;
        }

        protected override void OnRowChanging(DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Commit && this.acceptChangesStack == false)
            {
                throw new InvalidOperationException();
            }
            if (this.RowEventStack.Any() == true)
            {
                if (this.RowEventStack.Peek() == e.Row)
                    base.OnRowChanging(new InternalDataRowChangeEventArgs(e));
                else
                    base.OnRowChanging(e);
            }
            else
            {
                base.OnRowChanging(e);
            }
        }

        protected override void OnRowChanged(DataRowChangeEventArgs e)
        {
            if (this.RowEventStack.Any() == false)
            {
                this.RowEventStack.Push(e.Row);
                var row = e.Row as InternalRowBase;

                try
                {
                    switch (e.Action)
                    {
                        case DataRowAction.Add:
                            {
                                if (this.OmitSignatureDate == false)
                                {
                                    row.BeginEdit();
                                    row.CreationInfo = row.ModificationInfo = this.Sign();
                                    row.EndEdit();
                                }
                            }
                            break;
                        case DataRowAction.Change:
                            {
                                if (this.OmitSignatureDate == false)
                                {
                                    row.ModificationInfo = this.Sign();
                                }

                                //if (this.Parent == null && this.ColumnRelation != null)
                                //{
                                //    if (this.IsLoading == true && row.HasVersion(DataRowVersion.Original) == true)
                                //    {
                                //        var relations = this.GetChildRelations().ToArray();
                                //        var oldRelationID = row.Field<string>(this.ColumnRelation, DataRowVersion.Original);
                                //        var newRelationID = this.GenerateRelationID(row);
                                //        var query = from item in relations
                                //                    from InternalDataRow childRow in item.ChildTable.Rows
                                //                    where childRow.RelationID == oldRelationID
                                //                    select childRow as InternalDataRow;
                                //        foreach (var item in query)
                                //        {
                                //            item.RelationID = newRelationID;
                                //        }
                                //        row.RelationID = newRelationID;
                                //    }
                                //    else
                                //    {
                                //        if (this.PrimaryKey.Any() == true || row.RelationID == null)
                                //            row.RelationID = this.GenerateRelationID(row);
                                //    }
                                //}
                            }
                            break;
                    }
                }
                finally
                {
                    this.RowEventStack.Pop();
                }
            }

            if (this.RowEventStack.Any() == true)
            {
                if (this.RowEventStack.Peek() == e.Row)
                    base.OnRowChanged(new InternalDataRowChangeEventArgs(e));
                else
                    base.OnRowChanged(e);
            }
            else
            {
                base.OnRowChanged(e);
            }
        }

        protected override void OnRowDeleted(DataRowChangeEventArgs e)
        {
            base.OnRowDeleted(e);
        }

        protected override void OnColumnChanging(DataColumnChangeEventArgs e)
        {
            if (this.isReadOnly == true)
                throw new ReadOnlyException();
            if (e.Column.DataType == typeof(string) && e.ProposedValue is string textValue && textValue == string.Empty)
                e.ProposedValue = DBNull.Value;

            //if (e.Column == this.columnRelation && e.Row[this.columnRelation] is DBNull == false)
            //{
            //    e.ProposedValue = e.Row[this.columnRelation];
            //}

            base.OnColumnChanging(e);
        }

        protected override void OnColumnChanged(DataColumnChangeEventArgs e)
        {
            //if (e.Column is InternalColumnBase column && column.IsKey == true)
            //{
            //    var row = e.Row as InternalRowBase;
            //    if (row.RowState == DataRowState.Detached && this.ColumnRelation != null)
            //    {
            //        if (this.IsLoading == false || row.RelationID == null)
            //        {
            //            row.RelationID = this.GenerateRelationID(row);
            //        }
            //    }
            //}

            base.OnColumnChanged(e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnSetNormalMode()
        {
            this.attributeCreator.InternalDefaultValue = string.Empty;
            this.attributeCreator.InternalReadOnly = true;

            this.attributeCreatedDateTime.InternalDefaultValue = DateTime.MinValue;
            this.attributeCreatedDateTime.InternalReadOnly = true;

            this.attributeModifier.InternalDefaultValue = string.Empty;
            this.attributeModifier.InternalReadOnly = true;

            this.attributeModifiedDateTime.InternalDefaultValue = DateTime.MinValue;
            this.attributeModifiedDateTime.InternalReadOnly = true;
        }

        protected virtual void OnSetDiffMode()
        {
            this.attributeCreator.InternalDefaultValue = DBNull.Value;
            this.attributeCreator.InternalReadOnly = false;

            this.attributeCreatedDateTime.InternalDefaultValue = DBNull.Value;
            this.attributeCreatedDateTime.InternalReadOnly = false;

            this.attributeModifier.InternalDefaultValue = DBNull.Value;
            this.attributeModifier.InternalReadOnly = false;

            this.attributeModifiedDateTime.InternalDefaultValue = DBNull.Value;
            this.attributeModifiedDateTime.InternalReadOnly = false;
        }

        protected void ValidateInvoke(string invokeName)
        {
            if (this.TemplateNamespace != string.Empty)
                throw new ArgumentException($"상속받은 테이블에서는 작업을 수행할 수 없습니다.: \"{invokeName}\"");
        }

        protected void ValidateChildInvoke(string invokeName)
        {
            if (this.ParentName != string.Empty)
                throw new ArgumentException($"자식 테이블에서는 작업을 수행할 수 없습니다.: \"{invokeName}\"");
        }

        protected void ValidateSetProperty(string propertyName)
        {
            if (this.ParentName != string.Empty)
                throw new ArgumentException($"자식 테이블에서는 해당 속성을 변경할 수 없습니다.: \"{propertyName}\"");
        }

        protected virtual void OnValidateSetLocalName(string value)
        {
            if (this.Parent != null)
            {
                foreach (var item in this.Parent.Columns)
                {
                    if (item is InternalAttribute attributeItem && attributeItem.AttributeName == value)
                    {
                        throw new ArgumentException($"'{value}' '{this.Parent.Name}'의 특성의 이름이기 때문에 사용할 수 없습니다.");
                    }

                    if (item is InternalDataColumn columnItem && columnItem.ColumnName == value)
                    {
                        throw new ArgumentException($"'{value}' '{this.Parent.Name}'의 열의 이름이기 때문에 사용할 수 없습니다.");
                    }

                    if (item is InternalRelation relationItem && relationItem.RelationName == value)
                    {
                        throw new ArgumentException($"'{value}' '{this.Parent.Name}'의 관계열의 이름이기 때문에 사용할 수 없습니다.");
                    }
                }

                foreach (var item in this.Parent.ChildItems)
                {
                    if (item.LocalName == value)
                    {
                        throw new ArgumentException($"'{value}' '{this.Parent.Name}'의 자식의 이름이기 때문에 사용할 수 없습니다.");
                    }
                }

                if (this.Parent.LocalName == value)
                {
                    if (this.Parent.TemplatedParentName != string.Empty)
                        throw new ArgumentException($"'{value}' 상속된 '{this.Parent.Name}'의 이름이기 때문에 사용할 수 없습니다.");
                    else
                        throw new ArgumentException($"'{value}' '{this.Parent.Name}'의 이름이기 때문에 사용할 수 없습니다.");
                }
            }
            else
            {
                foreach (var item in this.ChildItems)
                {
                    if (item.LocalName == value)
                    {
                        throw new ArgumentException($"'{value}' '{this.Name}'의 자식의 이름이기 때문에 사용할 수 없습니다.");
                    }
                }
            }
        }

        protected virtual void OnValidateSetCategoryPath(string value)
        {
            NameValidator.ValidateCategoryPath(value);
        }

        protected virtual void OnValidateAddAttribute(InternalAttribute attribute)
        {
            if (this.ChildItems.Where(item => item.LocalName == attribute.AttributeName).Any() == true)
                throw new CremaDataException("{0} 은(는) 자식 테이블의 이름으로 사용되고 있으므로 사용할 수 없습니다.", attribute.AttributeName);
            if (attribute.IsKey == true && this.Rows.Count > 0)
                throw new CremaDataException("데이터가 있을때는 키로 설정된 열을 추가할 수 없습니다.");
            if (attribute.AllowDBNull == false && this.Rows.Count > 0)
                throw new CremaDataException("데이터가 있을때는 null값을 허용하지 않는 열을 추가할 수 없습니다.");
            if (attribute.Unique == true && this.Rows.Count > 0)
                throw new CremaDataException("데이터가 있을때는 고유 속성인 열을 추가할 수 없습니다.");
        }

        protected virtual void OnValidateAddAttribute(string columnName)
        {
            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));
            if (columnName == CremaSchema.__RelationID__)
                throw new CremaDataException("'{0}'은(는) 예약되어 있는 이름이기 때문에 사용할 수 없습니다.");
        }

        protected virtual void OnValidateAddAttribute(string columnName, Type type)
        {
            this.OnValidateAddAttribute(columnName);
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (CremaDataTypeUtility.IsBaseType(type) == false)
                throw new NotSupportedException(string.Format("'{0}'은(는) 사용할 수 없는 타입입니다.", type.Name));
        }

        protected virtual void OnValidateRemoveAttribute(InternalAttribute attribute)
        {

        }

        protected virtual void OnValidateRemoveAttribute(string attributeName)
        {

        }

        protected virtual void OnValidateClearAttributes()
        {
            
        }

        protected void InvokePropertyChangedEvent(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected sealed override void OnPropertyChanging(PropertyChangedEventArgs pcevent)
        {
            base.OnPropertyChanging(pcevent);
        }

        protected abstract string BaseNamespace
        {
            get;
        }

        protected Stack<DataRow> RowEventStack
        {
            get { return this.rowEventStack; }
        }

        protected IFieldStack<bool> OmitSignatureDateStack
        {
            get { return this.omitSignatureDate; }
        }

        internal IFieldStack<bool> AcceptChangesStack
        {
            get { return this.acceptChangesStack; }
        }

        private void TemplatedParent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(InternalTableBase.Namespace))
            {
                this.templateNamespace = this.templatedParent.Namespace;
            }
        }

        private void Columns_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    {
                        if (e.Element is InternalAttribute attribute && attribute.IsInternalAction == false)
                        {
                            this.attributeList.Add(attribute);
                        }
                    }
                    break;
                case CollectionChangeAction.Remove:
                    {
                        if (e.Element is InternalAttribute attribute && attribute.IsInternalAction == false)
                        {
                            this.attributeList.Remove(attribute);
                        }
                    }
                    break;
                case CollectionChangeAction.Refresh:
                    {
                        this.attributeList.Clear();
                        foreach (var item in this.Columns)
                        {
                            if (item is InternalAttribute attribute)
                            {
                                this.attributeList.Add(attribute);
                            }
                        }
                    }
                    break;
            }
        }

        private void ValidateSetParent(InternalTableBase parent)
        {
            if (parent == null)
                return;

            if (this.DataSet != null && this.DataSet != parent.DataSet)
                throw new CremaDataException("서로 다른 DataSet에 있는 테이블을 부모로 설정할 수 없습니다.");

            foreach (var item in parent.Columns)
            {
                if (item is InternalColumnBase dataColumn && dataColumn.ColumnName == this.LocalName)
                    throw new CremaDataException("테이블의 이름 {0}이(가) 부모의 열 이름으로 사용되고 있습니다.", this.LocalName);
            }
        }

        private void ValidateSign()
        {
            //if (this.OmitSignatureDate == true)
            //    throw new InvalidOperationException();
        }

        private void SetNormalMode()
        {
            this.OnSetNormalMode();

            var columns = new DataColumn[this.Columns.Count];
            this.Columns.CopyTo(columns, 0);
            foreach (var item in columns)
            {
                if (item is InternalColumnBase column)
                {
                    column.SetNormalMode();
                }
            }
        }

        private void SetDiffMode()
        {
            this.OnSetDiffMode();

            var columns = new DataColumn[this.Columns.Count];
            this.Columns.CopyTo(columns, 0);

            foreach (var item in columns)
            {
                if (item is InternalColumnBase column)
                {
                    column.SetDiffMode();
                }
            }
        }
    }

    abstract class InternalTableBase<TableBase, RowBase> : InternalTableBase
        where TableBase : InternalTableBase<TableBase, RowBase>
        where RowBase : InternalRowBase<TableBase, RowBase>
    {
        internal readonly InternalAttribute attributeIndex;
        private bool updateIndex;

        private readonly List<RowBase> rowList = new List<RowBase>();

        protected InternalTableBase(string name, string categoryPath)
            : base(name, categoryPath)
        {
            this.attributeIndex = new InternalAttribute(CremaSchema.Index, typeof(int))
            {
                AllowDBNull = false,
                DefaultValue = 0,
                ColumnMapping = MappingType.Hidden,
            };
            this.Columns.Add(this.attributeIndex);
        }

        public IList<RowBase> RowList
        {
            get { return this.rowList; }
        }

        public new int MinimumCapacity
        {
            get { return base.MinimumCapacity; }
            set
            {
                base.MinimumCapacity = value;
                this.rowList.Capacity = value;
            }
        }

        protected override void OnColumnChanging(DataColumnChangeEventArgs e)
        {
            this.ValidateIndex(e);

            if (e.Column.DataType == typeof(DateTime))
            {
                if (e.ProposedValue is DateTime dateTime)
                {
                    if (dateTime.Ticks % 10000000 != 0)
                    {
                        //throw new FormatException("날짜 형식");
                    }
                }
            }

            base.OnColumnChanging(e);
        }

        protected override void OnColumnChanged(DataColumnChangeEventArgs e)
        {
            base.OnColumnChanged(e);

            if (this.RowEventStack.Any() == false && this.updateIndex == false && e.Column == this.attributeIndex && e.Row.RowState != DataRowState.Detached)
            {
                this.updateIndex = true;
            }
        }

        protected override void OnRowChanging(DataRowChangeEventArgs e)
        {
            base.OnRowChanging(e);

            if (this.RowEventStack.Any() == false)
            {
                if (e.Action == DataRowAction.Add)
                {
                    var index = (int)e.Row[this.attributeIndex];
                    if (index > this.rowList.Count)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        protected override void OnRowChanged(DataRowChangeEventArgs e)
        {
            if (this.updateIndex == true && this.RowEventStack.Any() == false)
            {
                this.UpdateIndex(e.Row);
                this.updateIndex = false;
            }

            if (this.RowEventStack.Any() == false)
            {
                var row = e.Row as RowBase;
                if (e.Action == DataRowAction.Add)
                {
                    var index = row.Index;
                    if (index < 0)
                        index = this.rowList.Count;
                    this.rowList.Insert(index, row);

                    this.RowEventStack.Push(row);
                    using (this.OmitSignatureDateStack.Set(true))
                    {
                        for (var i = index; i < this.rowList.Count; i++)
                        {
                            var item = this.rowList[i];
                            if (object.Equals(item[this.attributeIndex], i) == false)
                            {
                                item[this.attributeIndex] = i;
                            }
                        }
                    }
                    this.RowEventStack.Pop();
                    this.attributeIndex.InternalDefaultValue = this.Rows.Count;
                }
                else if (e.Action == DataRowAction.Commit)
                {
                    if (row.RowState == DataRowState.Detached)
                    {
                        this.rowList.Remove(row);
                    }
                }
                else if (e.Action == DataRowAction.Change)
                {

                }
                else if (e.Action == DataRowAction.Rollback)
                {
                    if (row.RowState == DataRowState.Detached)
                    {
                        this.rowList.Remove(row);
                    }
                    else if (row.RowState == DataRowState.Unchanged)
                    {
                        if (this.rowList.Contains(row) == false)
                            this.rowList.Add(row);
                    }

                    this.rowList.Sort((x, y) => x.Index.CompareTo(y.Index));
                    this.attributeIndex.InternalDefaultValue = this.rowList.Count;
                }
            }
            base.OnRowChanged(e);
        }

        protected override void OnRowDeleted(DataRowChangeEventArgs e)
        {
            base.OnRowDeleted(e);

            if (e.Row is RowBase rowItem)
            {
                this.rowList.Remove(rowItem);
            }
            this.UpdateIndex(null);
            this.attributeIndex.InternalDefaultValue = this.rowList.Count;
        }

        protected override void OnTableCleared(DataTableClearEventArgs e)
        {
            base.OnTableCleared(e);
            this.rowList.Clear();
            this.attributeIndex.InternalDefaultValue = (int)0;
        }

        private void UpdateIndex(DataRow row)
        {
            if (this.RowEventStack.Any() == true)
                throw new InvalidOperationException();

            this.RowEventStack.Push(row);

            using (this.OmitSignatureDateStack.Set(true))
            {
                var query = from DataRow item in this.Rows
                            where item.RowState != DataRowState.Deleted
                            orderby item.Field<int>(this.attributeIndex)
                            select item;

                var rows = query.ToList();
                if (row != null && row.RowState != DataRowState.Detached)
                {
                    var index = row.Field<int>(this.attributeIndex);
                    rows.Remove(row);
                    rows.Insert(index, row);
                }
                var rowList = new List<DataRow>(rows.Count);

                for (var i = rows.Count - 1; i >= 0; i--)
                {
                    var item = rows[i];
                    if (object.Equals(item[this.attributeIndex], i) == false)
                    {
                        item[this.attributeIndex] = i;
                        rowList.Add(item);
                    }
                    else if (item == row)
                    {
                        rowList.Add(item);
                    }
                }
            }
            this.RowEventStack.Pop();
            this.rowList.Sort((x, y) => x.Index.CompareTo(y.Index));
        }

        private void ValidateIndex(DataColumnChangeEventArgs e)
        {
            var column = e.Column;
            var row = e.Row;

            if (row.RowState == DataRowState.Detached)
            {
                //if (column == this.attributeIndex)
                //    throw new ArgumentException();
            }

            if (row.RowState != DataRowState.Detached && this.RowEventStack.Any() == false && this.updateIndex == false && column == this.attributeIndex && e.ProposedValue is int index)
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException();
                if (index >= GetRowCount())
                    throw new ArgumentOutOfRangeException();

                int GetRowCount()
                {
                    var rowCount = 0;
                    for (var i = 0; i < this.Rows.Count; i++)
                    {
                        var item = this.Rows[i];
                        if (item.RowState != DataRowState.Deleted && item.RowState != DataRowState.Detached)
                            rowCount++;
                    }
                    return rowCount;
                }
            }
        }
    }
}
