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

using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.DataGrid;

namespace Ntreev.Crema.Presentation.Controls.Actions
{
    class DetailsToggledAction : ActionBase
    {
        private readonly DataGridContext gridContext1;
        private readonly object item1;
        private readonly DataGridContext gridContext2;
        private readonly object item2;
        private readonly bool isExpanded;

        public DetailsToggledAction(DataGridContext gridContext1, object item1, DataGridContext gridContext2, object item2, bool isExpanded)
        {
            this.gridContext1 = gridContext1;
            this.item1 = item1;
            this.gridContext2 = gridContext2;
            this.item2 = item2;
            this.isExpanded = isExpanded;
        }

        protected override void OnRedo()
        {
            base.OnRedo();
            if (this.isExpanded == true)
            {
                this.gridContext1.ExpandDetails(this.item1);
                this.gridContext2.ExpandDetails(this.item2);
            }
            else
            {
                this.gridContext1.CollapseDetails(this.item1);
                this.gridContext2.CollapseDetails(this.item2);
            }

            (this.gridContext1.DataGridControl as DiffDataGridControl).InvokeDetailsToggledEvent();
            (this.gridContext2.DataGridControl as DiffDataGridControl).InvokeDetailsToggledEvent();
        }

        protected override void OnUndo()
        {
            base.OnUndo();
            if (this.isExpanded == true)
            {
                this.gridContext1.CollapseDetails(this.item1);
                this.gridContext2.CollapseDetails(this.item2);
            }
            else
            {
                this.gridContext1.ExpandDetails(this.item1);
                this.gridContext2.ExpandDetails(this.item2);
            }
            (this.gridContext1.DataGridControl as DiffDataGridControl).InvokeDetailsToggledEvent();
            (this.gridContext2.DataGridControl as DiffDataGridControl).InvokeDetailsToggledEvent();
        }
    }
}
