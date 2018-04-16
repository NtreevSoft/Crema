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

using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Diff;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Ntreev.Crema.Presentation.Controls.Actions
{
    class CopyItemAction : ActionBase
    {
        private readonly ItemInfo item;

        public CopyItemAction(object dataItem, object destItem)
            : this(dataItem, destItem, false)
        {

        }

        public CopyItemAction(object dataItem, object destItem, bool recursive)
        {
            this.item = new ItemInfo(dataItem, destItem);
        }

        protected override void OnRedo()
        {
            base.OnRedo();
            this.item.Redo();
        }

        protected override void OnUndo()
        {
            base.OnUndo();
            this.item.Undo();
        }

        #region classes

        class ItemInfo
        {
            private readonly object dataItem;
            private readonly object destItem;
            private object[] fields;

            private readonly List<ItemInfo> childList = new List<ItemInfo>();

            public ItemInfo(object dataItem, object destItem)
            {
                this.dataItem = dataItem;
                this.destItem = destItem;
                this.fields = DiffUtility.GetFields(destItem);

                var dataChildItems = new List<object>();
                var destChildItems = new List<object>();
                foreach (var item in DiffUtility.GetChilds(dataItem))
                {
                    dataChildItems.Add(item);
                }
                foreach (var item in DiffUtility.GetChilds(destItem))
                {
                    destChildItems.Add(item);
                }

                var query = from dataChild in dataChildItems
                            join destChild in destChildItems
                            on $"{DiffUtility.GetListName(dataChild)}{DiffUtility.GetItemKey(dataChild)}"
                            equals $"{DiffUtility.GetListName(destChild)}{DiffUtility.GetItemKey(destChild)}"
                            select new { DataItem = dataChild, DestItem = destChild };

                foreach (var item in query)
                {
                    this.childList.Add(new ItemInfo(item.DataItem, item.DestItem));
                }
            }

            public object Item { get => this.dataItem; }

            public object[] Fields { get => this.fields; }

            public void Redo()
            {
                DiffUtility.Copy(this.dataItem, this.destItem);
                foreach (var item in this.childList)
                {
                    item.Redo();
                }
            }

            public void Undo()
            {
                DiffUtility.Copy(this.destItem, this.fields);
                foreach (var item in this.childList)
                {
                    item.Undo();
                }
            }

            private void Obj_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == string.Empty)
                {
                    this.fields = DiffUtility.GetFields(this.dataItem);
                }
            }
        }

        #endregion
    }
}
