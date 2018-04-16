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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Types.Documents.ViewModels
{
    class SelectFieldStrategy
    {
        private readonly TypeViewModel document;
        private readonly string columnName;
        private readonly int row;

        public SelectFieldStrategy(TypeViewModel document, string typeName, string columnName, int row)
        {
            this.document = document;
            this.columnName = columnName;
            this.row = row;
            this.document.SelectedColumn = this.columnName;
            this.document.SelectedItemIndex = this.row;
            this.document.PropertyChanged += Document_PropertyChanged;
        }

        private void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TypeViewModel.Source))
            {
                this.document.SelectedColumn = this.columnName;
                this.document.SelectedItemIndex = this.row;
                this.document.PropertyChanged -= Document_PropertyChanged;
            }
        }
    }
}
