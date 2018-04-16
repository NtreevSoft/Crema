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
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Comparer.Tables.ViewModels
{
    [Export(typeof(IContent))]
    class TablesViewModel : ViewModelBase, IContent
    {
        [Import]
        private Lazy<TableBrowserViewModel> browser = null;
        [Import]
        private Lazy<TableDocumentServiceViewModel> document = null;
        [Import]
        private Lazy<TablePropertyViewModel> properties = null;

        public TablesViewModel()
        {

        }

        public string DisplayName => "tables";

        public string Comment => string.Empty;

        private bool isVisible;
        public bool IsVisible
        {
            get { return this.isVisible; }
            set
            {
                this.isVisible = value;
                this.NotifyOfPropertyChange(nameof(this.IsVisible));
            }
        }

        public TableBrowserViewModel Browser => this.browser.Value;

        public TableDocumentServiceViewModel Document => this.document.Value;

        public TablePropertyViewModel Property => this.properties.Value;
    }
}
