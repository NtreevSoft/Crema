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
using Xceed.Wpf.DataGrid;

namespace Ntreev.Crema.Presentation.Controls.Actions
{
    class AddItemAction : ActionBase
    {
        private readonly DataGridContext gridContext;
        private readonly DataGridContext destContext;
        private readonly object dataItem;

        private object newItem;
        private object newDestItem;

        public AddItemAction(DataGridContext gridContext, DataGridContext destContext, object dataItem)
        {
            this.gridContext = gridContext;
            this.destContext = destContext;
            this.dataItem = dataItem;
        }

        protected override void OnRedo()
        {
            base.OnRedo();
            if (this.gridContext.Items.SourceCollection is DataGridCollectionView bindingList1)
            {
                var newItem = bindingList1.AddNew();
                bindingList1.CommitNew();
                bindingList1.Refresh();
                this.newItem = newItem;
            }

            if (this.destContext.Items.SourceCollection is DataGridCollectionView bindingList2)
            {
                var newItem = bindingList2.AddNew();
                DiffUtility.Copy(dataItem, newItem, true);
                bindingList2.CommitNew();
                bindingList2.Refresh();
                this.newDestItem = newItem;
            }
        }

        protected override void OnUndo()
        {
            base.OnUndo();
            if (this.gridContext.Items.SourceCollection is DataGridCollectionView bindingList1)
            {
                bindingList1.Remove(this.newItem);
                this.newItem = null;
                bindingList1.Refresh();
            }

            if (this.destContext.Items.SourceCollection is DataGridCollectionView bindingList2)
            {
                bindingList2.Remove(this.newDestItem);
                this.newDestItem = null;
                bindingList2.Refresh();
            }
        }
    }
}
