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

using Ntreev.Crema.Data.Diff;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Presentation.Controls.Actions
{
    class DeleteItemAction : ActionBase
    {
        private readonly ItemInfo item;

        public DeleteItemAction(object dataItem)
        {
            this.item = new ItemInfo(dataItem);
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
            private object[] fields;

            private readonly List<ItemInfo> childList = new List<ItemInfo>();

            public ItemInfo(object dataItem)
            {
                this.dataItem = dataItem;
                this.fields = DiffUtility.GetFields(dataItem);

                foreach (var item in DiffUtility.GetChilds(dataItem))
                {
                    this.childList.Add(new ItemInfo(item));
                }
            }

            public object Item { get => this.dataItem; }

            public object[] Fields { get => this.fields; }

            public void Redo()
            {
                DiffUtility.Empty(this.dataItem);
                foreach (var item in this.childList)
                {
                    item.Redo();
                }
            }

            public void Undo()
            {
                DiffUtility.Copy(this.dataItem, this.fields);
                foreach (var item in this.childList)
                {
                    item.Undo();
                }
            }
        }

        #endregion
    }
}
