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

using Ntreev.Library;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Library.ObjectModel;
using System;
using System.Linq;
using System.Collections.Generic;
using Ntreev.Crema.ServiceModel.Properties;
using System.Collections.Specialized;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.IO;

namespace Ntreev.Crema.ServiceModel
{
    internal abstract class TypeBase<_I, _C, _IC, _CC, _CT> : PermissionItemBase<_I, _C, _IC, _CC, _CT>
        where _I : TypeBase<_I, _C, _IC, _CC, _CT>
        where _C : TypeCategoryBase<_I, _C, _IC, _CC, _CT>, new()
        where _IC : ItemContainer<_I, _C, _IC, _CC, _CT>, new()
        where _CC : CategoryContainer<_I, _C, _IC, _CC, _CT>, new()
        where _CT : ItemContext<_I, _C, _IC, _CC, _CT>
    {
        private bool isLoaded;
        private TypeInfo typeInfo;
        private TypeState typeState;
        private TypeMetaData metaData = TypeMetaData.Empty;

        public TypeBase()
        {
            
        }

        public void UpdateTypeInfo(TypeInfo value)
        {
            this.typeInfo = value;
            this.OnTypeInfoChanged(EventArgs.Empty);
        }

        public void UpdateTags(TagInfo tags)
        {
            if (this.typeInfo.Tags == tags)
                return;

            this.typeInfo.Tags = tags;
            for (var i = 0; i < this.typeInfo.Members.Length; i++)
            {
                this.typeInfo.Members[i].DerivedTags = this.typeInfo.Members[i].Tags & tags;
            }

            this.OnTypeInfoChanged(EventArgs.Empty);
        }

        public void UpdateTypeInfo()
        {
            if (this.Category == null)
                return;
            if (this.typeInfo.Name == this.Name && this.typeInfo.CategoryPath == this.Category.Path)
                return;
            this.typeInfo.Name = this.Name;
            this.typeInfo.CategoryPath = this.Category.Path;
            this.OnTypeInfoChanged(EventArgs.Empty);
        }

        public void Initialize(TypeInfo typeInfo)
        {
            if (this.isLoaded == true)
                throw new InvalidOperationException();
            this.typeInfo = typeInfo;
            this.isLoaded = true;
            this.OnTypeInfoChanged(EventArgs.Empty);
        }

        public TypeInfo TypeInfo
        {
            get { return this.typeInfo; }
        }

        public TypeState TypeState
        {
            get { return this.typeState; }
            set
            {
                if (this.typeState == value)
                    return;
                this.typeState = value;
                this.OnTypeStateChanged(EventArgs.Empty);
            }
        }

        public TagInfo Tags
        {
            get
            {
                return this.typeInfo.Tags;
            }
        }

        public virtual bool IsBeingEdited
        {
            get { return this.typeState.HasFlag(TypeState.IsBeingEdited); }
            set
            {
                if (value == true)
                    this.typeState |= TypeState.IsBeingEdited;
                else
                    this.typeState &= ~TypeState.IsBeingEdited;
                this.OnTypeStateChanged(EventArgs.Empty);
            }
        }

        public TypeMetaData MetaData
        {
            get { return this.metaData; }
        }

        public event EventHandler TypeInfoChanged;

        public event EventHandler TypeStateChanged;

        protected void Rename(IAuthentication authentication, string name)
        {
            this.Name = name;
        }

        protected void Move(IAuthentication authentication, string categoryPath)
        {
            this.Category = this.Context.Categories[categoryPath];
        }

        protected void Delete(IAuthentication authentication)
        {
            base.Dispose();
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

        protected virtual void OnTypeInfoChanged(EventArgs e)
        {
            this.metaData.Path = this.Path;
            this.metaData.TypeInfo = this.TypeInfo;
            this.TypeInfoChanged?.Invoke(this, e);
        }

        protected virtual void OnTypeStateChanged(EventArgs e)
        {
            this.metaData.TypeState = this.TypeState;
            this.TypeStateChanged?.Invoke(this, e);
        }

        protected override void OnRenamed(EventArgs e)
        {
            base.OnRenamed(e);
        }

        protected override void OnMoved(EventArgs e)
        {
            base.OnMoved(e);
        }

        protected override void OnPathChanged(string oldPath, string newPath)
        {
            base.OnPathChanged(oldPath, newPath);

            this.typeInfo.Name = this.Name;
            this.typeInfo.CategoryPath = this.Category == null ? PathUtility.Separator : this.Category.Path;

            this.OnTypeInfoChanged(EventArgs.Empty);
        }
    }
}
