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

namespace Ntreev.Crema.Client.Differences.Dialogs.ViewModels
{
    class DiffDataTableItemViewModel : PropertyChangedBase
    {
        private readonly DiffDataTableViewModel viewModel;
        private readonly DiffDataTable source;

        public DiffDataTableItemViewModel(DiffDataTableViewModel viewModel, DiffDataTable source)
        {
            this.viewModel = viewModel;
            this.source = source;
        }

        public override string ToString()
        {
            return this.source.ToString();
        }

        public string DisplayName
        {
            get { return this.Source.ToString(); }
        }

        public bool IsResolved
        {
            get { return this.source.IsResolved; }
        }

        public DiffDataTable Source
        {
            get { return this.source; }
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
