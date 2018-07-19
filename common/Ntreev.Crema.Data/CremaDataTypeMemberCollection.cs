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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ntreev.Crema.Data
{
    public sealed class CremaDataTypeMemberCollection : IEnumerable<CremaDataTypeMember>, ICollection
    {
        private readonly InternalDataType type;
        private readonly DataRowCollection rows;
        private readonly IList<InternalDataTypeMember> itemList;

        internal CremaDataTypeMemberCollection(InternalDataType type)
        {
            this.type = type;
            this.rows = type.Rows;
            this.itemList = type.RowList;
        }

        public void Add(CremaDataTypeMember member)
        {
            this.type.AddMember((InternalDataTypeMember)member);
        }

        public void Clear()
        {
            this.type.ClearMembers();
        }

        public bool Contains(string memberName)
        {
            return this.IndexOf(memberName) >= 0;
        }

        public bool Contains(Guid memberID)
        {
            foreach (var item in this.itemList)
            {
                if (item[CremaSchema.ID] is Guid id && id == memberID)
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(CremaDataTypeMember[] array, int index)
        {
            var items = this.itemList.Select(item => item.Target).ToArray();
            items.CopyTo(array, index);
        }

        public int IndexOf(CremaDataTypeMember member)
        {
            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                if (item == member.InternalObject)
                    return i;
            }
            return -1;
        }

        public int IndexOf(string memberName)
        {
            for (var i = 0; i < this.itemList.Count; i++)
            {
                var item = this.itemList[i];
                if (item.Name == memberName)
                    return i;
            }
            return -1;
        }

        public void Remove(CremaDataTypeMember member)
        {
            this.type.RemoveMember((InternalDataTypeMember)member);
        }

        public void RemoveAt(int index)
        {
            var item = this.itemList[index];
            this.type.RemoveMember(item);
        }

        public int Count => this.itemList.Count;

        public CremaDataTypeMember this[int index] => this.itemList[index].Target;

        public CremaDataTypeMember this[string memberName]
        {
            get
            {
                var index = this.IndexOf(memberName);
                if (index >= 0)
                    return this.itemList[index].Target;
                return null;
            }
        }

        public CremaDataTypeMember this[Guid memberID]
        {
            get
            {
                foreach (var item in this.itemList)
                {
                    if (item[CremaSchema.ID] is Guid id && id == memberID)
                    {
                        return item.Target;
                    }
                }
                return null;
            }
        }

        #region IEnumerable

        void ICollection.CopyTo(Array array, int index)
        {
            var items = this.itemList.Select(item => item.Target).ToArray();
            items.CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                if (this.itemList is ICollection collection)
                    return collection.IsSynchronized;
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (this.itemList is ICollection collection)
                    return collection.SyncRoot;
                return this.itemList;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in this.itemList)
            {
                yield return item.Target;
            }
        }

        IEnumerator<CremaDataTypeMember> IEnumerable<CremaDataTypeMember>.GetEnumerator()
        {
            foreach (var item in this.itemList)
            {
                yield return item.Target;
            }
        }

        #endregion
    }
}