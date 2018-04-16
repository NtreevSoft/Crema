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

namespace Ntreev.Crema.Comparer.Tables.ViewModels
{
    class TableDocumentItemViewModel : PropertyChangedBase
    {
        private readonly TableTreeViewItemViewModel viewModel;
        private bool isModified;

        public TableDocumentItemViewModel(TableTreeViewItemViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override string ToString()
        {
            return this.viewModel.ToString();
        }

        public void RefreshModifiedState()
        {
            this.IsModified = this.Source.HasChanges();
        }

        public bool IsModified
        {
            get { return this.isModified; }
            set
            {
                this.isModified = value;
                this.NotifyOfPropertyChange(nameof(this.IsModified));
                this.NotifyOfPropertyChange(nameof(this.DisplayName));
            }
        }

        public string DisplayName
        {
            get
            {
                if (this.isModified == true)
                    return this.viewModel.Source.ToString() + "*";
                return this.viewModel.Source.ToString();
            }
        }

        public bool IsResolved
        {
            get { return this.viewModel.IsResolved; }
        }

        public DiffDataTable Source
        {
            get { return this.viewModel.Source; }
        }

        public string Header1
        {
            get { return this.viewModel.Header1; }
        }

        public string Header2
        {
            get { return this.viewModel.Header2; }
        }
    }
}
