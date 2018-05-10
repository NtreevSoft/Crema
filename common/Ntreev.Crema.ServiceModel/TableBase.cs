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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Schema;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.ServiceModel.Properties;
using System.Xml;
using System.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Data.Xml;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.Linq;
using System.Text.RegularExpressions;
using Ntreev.Library;
using Ntreev.Library.IO;
using System.ComponentModel;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.ServiceModel
{
    internal abstract class TableBase<_I, _C, _IC, _CC, _CT> : PermissionItemBase<_I, _C, _IC, _CC, _CT>, ITableInfoProvider
        where _I : TableBase<_I, _C, _IC, _CC, _CT>
        where _C : TableCategoryBase<_I, _C, _IC, _CC, _CT>, new()
        where _IC : ItemContainer<_I, _C, _IC, _CC, _CT>, new()
        where _CC : CategoryContainer<_I, _C, _IC, _CC, _CT>, new()
        where _CT : ItemContext<_I, _C, _IC, _CC, _CT>
    {
        private readonly InternalTableCollection<_I, _C, _IC, _CC, _CT> childs = new InternalTableCollection<_I, _C, _IC, _CC, _CT>(true);
        private readonly InternalTableCollection<_I, _C, _IC, _CC, _CT> derivedTables = new InternalTableCollection<_I, _C, _IC, _CC, _CT>();
        private _I parent;
        private _I templatedParent;
        private bool isLoaded;

        private TableInfo tableInfo = TableInfo.Default;
        private TableState tableState;

        private TableMetaData metaData = TableMetaData.Empty;

        public TableBase()
        {

        }

        public void UpdateTableInfo(TableInfo value)
        {
            this.tableInfo = value;
            this.OnTableInfoChanged(EventArgs.Empty);
        }

        public override void UpdateAccessInfo()
        {
            base.UpdateAccessInfo();
            foreach (var item in this.Childs)
            {
                item.UpdateAccessInfo();
            }
        }

        public override void UpdateLockInfo()
        {
            base.UpdateLockInfo();
            foreach (var item in this.Childs)
            {
                item.UpdateLockInfo();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateSetPublic(IAuthentication authentication, object target)
        {
            base.OnValidateSetPublic(authentication, target);

            foreach (var item in this.Childs)
            {
                item.OnValidateSetPublic(authentication, target);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateSetPrivate(IAuthentication authentication, object target)
        {
            base.OnValidateSetPrivate(authentication, target);

            foreach (var item in this.Childs)
            {
                item.OnValidateSetPrivate(authentication, target);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateAddAccessMember(IAuthentication authentication, object target, string memberID, AccessType accessType)
        {
            base.OnValidateAddAccessMember(authentication, target, memberID, accessType);

            foreach (var item in this.Childs)
            {
                item.OnValidateAddAccessMember(authentication, target, memberID, accessType);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateSetAccessMember(IAuthentication authentication, object target, string memberID, AccessType accessType)
        {
            base.OnValidateSetAccessMember(authentication, target, memberID, accessType);

            foreach (var item in this.Childs)
            {
                item.OnValidateSetAccessMember(authentication, target, memberID, accessType);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateRemoveAccessMember(IAuthentication authentication, object target)
        {
            base.OnValidateRemoveAccessMember(authentication, target);

            foreach (var item in this.Childs)
            {
                item.OnValidateRemoveAccessMember(authentication, target);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateLock(IAuthentication authentication, object target)
        {
            base.OnValidateLock(authentication, target);

            foreach (var item in this.Childs)
            {
                item.OnValidateLock(authentication, target);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateUnlock(IAuthentication authentication, object target)
        {
            base.OnValidateUnlock(authentication, target);

            foreach (var item in this.Childs)
            {
                item.OnValidateUnlock(authentication, target);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateRename(IAuthentication authentication, object target, string oldPath, string newPath)
        {
            base.OnValidateRename(authentication, target, oldPath, newPath);

            foreach (var item in this.Childs)
            {
                item.OnValidateRename(authentication, target, oldPath, newPath);
            }

            if (target == null)
                return;

            if (this.TemplatedParent != null)
            {
                this.TemplatedParent.OnValidateRename(authentication, null, oldPath, newPath);

                if (this == target && this.Parent != null)
                    throw new InvalidOperationException(Resources.Exception_InheritedChildCannotRename);
            }

            foreach (var item in this.DerivedTables)
            {
                item.OnValidateRename(authentication, null, oldPath, newPath);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateMove(IAuthentication authentication, object target, string oldPath, string newPath)
        {
            if (target == this && this.Parent != null)
                throw new NotImplementedException();
            base.OnValidateMove(authentication, target, oldPath, newPath);

            foreach (var item in this.Childs)
            {
                item.OnValidateMove(authentication, target, oldPath, newPath);
            }

            if (target == null)
                return;

            if (this.TemplatedParent != null)
            {
                this.TemplatedParent.OnValidateMove(authentication, null, oldPath, newPath);

                if (this == target && this.Parent != null)
                    throw new InvalidOperationException(Resources.Exception_InheritedChildCannotMove);
            }

            foreach (var item in this.DerivedTables)
            {
                item.OnValidateMove(authentication, null, oldPath, newPath);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnValidateDelete(IAuthentication authentication, object target)
        {
            base.OnValidateDelete(authentication, target);

            foreach (var item in this.Childs)
            {
                item.OnValidateDelete(authentication, target);
            }

            if (target == null)
                return;

            if (this.TemplatedParent != null)
            {
                this.TemplatedParent.OnValidateDelete(authentication, null);

                if (this == target && this.Parent != null)
                    throw new InvalidOperationException(Resources.Exception_InheritedChildCannotDelete);
            }

            foreach (var item in this.DerivedTables)
            {
                item.OnValidateDelete(authentication, null);
            }
        }

        public void UpdateTableInfo()
        {
            if (this.Category == null)
                return;

            this.tableInfo.Name = this.Name;
            this.tableInfo.CategoryPath = this.Category.Path;
            this.tableInfo.TemplatedParent = this.templatedParent == null ? string.Empty : this.templatedParent.Name;

            foreach (var item in this.childs)
            {
                item.UpdateTableInfo();
            }

            this.OnTableInfoChanged(EventArgs.Empty);

            foreach (var item in this.derivedTables)
            {
                item.UpdateTableInfo();
            }
        }

        public void Initialize(TableInfo tableInfo)
        {
            if (this.isLoaded == true)
                throw new InvalidOperationException();

            this.tableInfo = tableInfo;
            this.isLoaded = true;
            this.OnTableInfoChanged(EventArgs.Empty);
        }

        public void UpdateTemplate(TableInfo tableInfo)
        {
            var derivedTables = this.derivedTables.ToArray();

            foreach (var item in EnumerableUtility.Friends(this, derivedTables))
            {
                item.tableInfo.Columns = new ColumnInfo[tableInfo.Columns.Length];
                item.tableInfo.HashValue = tableInfo.HashValue;
                item.tableInfo.ModificationInfo = tableInfo.ModificationInfo;
                for (var i = 0; i < tableInfo.Columns.Length; i++)
                {
                    item.tableInfo.Columns[i] = tableInfo.Columns[i];
                }
                item.tableInfo.Comment = tableInfo.Comment;
                item.OnTableInfoChanged(EventArgs.Empty);
            }
        }

        public void UpdateTags(TagInfo tags)
        {
            if (this.tableInfo.Tags == tags)
                return;

            this.tableInfo.Tags = tags;
            this.tableInfo.DerivedTags = tags;
            for (var i = 0; i < this.tableInfo.Columns.Length; i++)
            {
                if (this.tableInfo.Columns[i].IsKey == true)
                {
                    this.tableInfo.Columns[i].DerivedTags = tags;
                }
                else
                {
                    this.tableInfo.Columns[i].DerivedTags = this.tableInfo.Columns[i].Tags & tags;
                }
            }

            this.OnTableInfoChanged(EventArgs.Empty);

            foreach (var item in this.childs)
            {
                item.UpdateDerivedTags(tags);
            }

            foreach (var item in this.derivedTables)
            {
                item.UpdateTags(tags);
            }
        }

        public void UpdateDerivedTags(TagInfo tags)
        {
            this.tableInfo.DerivedTags = this.tableInfo.Tags & tags;
            for (var i = 0; i < tableInfo.Columns.Length; i++)
            {
                if (this.tableInfo.Columns[i].IsKey == true)
                {
                    this.tableInfo.Columns[i].DerivedTags = this.tableInfo.DerivedTags;
                }
                else
                {
                    this.tableInfo.Columns[i].DerivedTags = this.tableInfo.Columns[i].Tags & this.tableInfo.DerivedTags;
                }
            }

            foreach (var item in this.DerivedTables)
            {
                item.UpdateDerivedTags(tags);
            }

            this.OnTableInfoChanged(EventArgs.Empty);
        }

        public void UpdateComment(string comment)
        {
            if (this.tableInfo.Comment == comment)
                return;

            this.tableInfo.Comment = comment;
            this.OnTableInfoChanged(EventArgs.Empty);
        }

        public void UpdateContent(TableInfo tableInfo)
        {
            this.tableInfo.ContentsInfo = tableInfo.ContentsInfo;
            this.OnTableInfoChanged(EventArgs.Empty);
        }

        public TableInfo TableInfo
        {
            get
            {
#if DEBUG
                if (this.tableInfo == TableInfo.Default)
                    throw new NotImplementedException();
#endif
                if (this.tableInfo.Name != this.Name)
                    throw new InvalidOperationException();
                return this.tableInfo;
            }
        }

        public TagInfo Tags
        {
            get
            {
                if (this.templatedParent != null)
                    return this.templatedParent.Tags;
                return this.tableInfo.DerivedTags;
            }
        }

        public TableState TableState
        {
            get { return this.tableState; }
            set
            {
                if (this.tableState == value)
                    return;
                this.tableState = value;
                this.OnTableStateChanged(EventArgs.Empty);
            }
        }

        public string TableName
        {
            get
            {
                if (this.parent == null)
                    return this.Name;
                return StringUtility.Split(this.Name, '.')[1];
            }
            set
            {
                if (this.parent != null)
                    this.Name = this.parent.TableName + "." + value;
                else
                    this.Name = value;
            }
        }

        public _I Parent
        {
            get { return this.parent; }
            internal set
            {
                if (this.parent != null)
                {
                    this.parent.childs.Remove(this as _I);
                    this.parent.Renamed -= Parent_Renamed;
                    this.parent.Moved -= Parent_Moved;
                    this.parent.Deleted -= Parent_Deleted;
                }
                this.parent = value;
                if (this.parent != null)
                {
                    this.parent.childs.Add(this as _I);
                    this.parent.Renamed += Parent_Renamed;
                    this.parent.Moved += Parent_Moved;
                    this.parent.Deleted += Parent_Deleted;
                }
            }
        }

        public _I TemplatedParent
        {
            get { return this.templatedParent; }
            internal set
            {
                if (this.templatedParent == value)
                    return;

                if (value == this)
                    throw new ArgumentException(Resources.Exception_CannotSetTemplateItself, nameof(value));

                if (value != null && value.TemplatedParent != null)
                    throw new ArgumentException(Resources.Exception_CannotSetTemplateToInherited, nameof(value));


                if (this.templatedParent != null)
                {
                    this.templatedParent.derivedTables.Remove(this as _I);
                    this.templatedParent.Renamed -= TemplatedParent_Renamed;
                    this.templatedParent.Deleted -= TemplatedParent_Deleted;
                }

                this.templatedParent = value;

                if (this.templatedParent != null)
                {
                    this.templatedParent.derivedTables.Add(this as _I);
                    this.templatedParent.Renamed += TemplatedParent_Renamed;
                    this.templatedParent.Deleted += TemplatedParent_Deleted;
                }
            }
        }

        public bool IsBaseTemplate
        {
            get { return this.derivedTables.Count > 0; }
        }

        public bool IsInherited
        {
            get { return string.IsNullOrEmpty(this.tableInfo.TemplatedParent) == false; }
        }

        public IContainer<_I> DerivedTables
        {
            get { return this.derivedTables; }
        }

        public InternalTableCollection<_I, _C, _IC, _CC, _CT> Childs
        {
            get { return this.childs; }
        }

        public TableMetaData MetaData
        {
            get { return this.metaData; }
        }

        public event EventHandler TableInfoChanged;

        public event EventHandler TableStateChanged;

        protected void Rename(IAuthentication authentication, string name)
        {
            this.TableName = name;
        }

        protected void Move(IAuthentication authentication, string categoryPath)
        {
            if (this.Parent != null)
                throw new InvalidOperationException("child table cannot move");
            this.Category = this.Context.Categories[categoryPath];
        }

        protected void Delete(IAuthentication authentication)
        {
            foreach (var item in this.derivedTables.ToArray())
            {
                item.Dispose();
            }
            base.Dispose();
        }

        protected override void OnRenamed(EventArgs e)
        {
            base.OnRenamed(e);
        }

        protected override void OnMoved(EventArgs e)
        {
            base.OnMoved(e);
        }

        protected override void OnDeleted(EventArgs e)
        {
            base.OnDeleted(e);
            this.Parent = null;
        }

        protected override void OnAccessChanged(EventArgs e)
        {
            this.metaData.AccessInfo = this.AccessInfo;
            base.OnAccessChanged(e);
        }

        protected override void OnLockChanged(EventArgs e)
        {
            this.metaData.LockInfo = this.LockInfo;
            base.OnLockChanged(e);
        }

        protected override void OnPathChanged(string oldPath, string newPath)
        {
            base.OnPathChanged(oldPath, newPath);

            this.tableInfo.Name = this.Name;
            this.tableInfo.CategoryPath = this.Category == null ? PathUtility.Separator : this.Category.Path;
            this.tableInfo.TemplatedParent = this.templatedParent == null ? string.Empty : this.templatedParent.Name;

            this.OnTableInfoChanged(EventArgs.Empty);

            foreach (var item in this.derivedTables)
            {
                item.OnPathChanged(oldPath, newPath);
            }
        }

        protected virtual void OnTableInfoChanged(EventArgs e)
        {
            this.metaData.Path = this.Path;
            this.metaData.TableInfo = this.TableInfo;
            this.TableInfoChanged?.Invoke(this, e);
        }

        protected virtual void OnTableStateChanged(EventArgs e)
        {
            this.metaData.TableState = this.TableState;
            this.TableStateChanged?.Invoke(this, e);
        }

        protected override void OnUpdateAccessParent(IAccessParent accessParent)
        {
            base.OnUpdateAccessParent(accessParent);

            foreach (var item in this.Childs)
            {
                item.AccessParent = accessParent;
                if (item.AccessInfo.UserID == string.Empty)
                {

                }
                else
                {
                    item.InvokeAccessChanged(EventArgs.Empty);
                }
            }
        }

        protected override void OnUpdateLockParent(ILockParent lockParent)
        {
            base.OnUpdateLockParent(lockParent);

            foreach (var item in this.Childs)
            {
                item.LockParent = lockParent;
                if (item.LockInfo.UserID == string.Empty)
                {

                }
            }
        }

        private void Parent_Renamed(object sender, EventArgs e)
        {
            var tableName = this.TableName;
            this.Name = this.parent.Name + "." + tableName;
        }

        private void Parent_Moved(object sender, EventArgs e)
        {
            this.Category = this.parent.Category;
        }

        private void Parent_Deleted(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void TemplatedParent_Renamed(object sender, EventArgs e)
        {
            if (this.parent != null)
            {
                this.TableName = this.templatedParent.TableName;
            }
        }

        private void TemplatedParent_Deleted(object sender, EventArgs e)
        {
            this.Dispose();
        }

        #region ITableInfoProvider

        TableInfo ITableInfoProvider.TableInfo
        {
            get { return this.TableInfo; }
            set
            {
                this.tableInfo = value;
                this.OnTableInfoChanged(EventArgs.Empty);
            }
        }

        #endregion
    }
}
