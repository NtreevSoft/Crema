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

using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.IO;

namespace Ntreev.Crema.Client.SmartSet.BrowserItems.ViewModels
{
    class InvalidTreeViewItemViewModel : TreeViewItemViewModel
    {
        private readonly string displayName;
        private readonly string displayPath;
        private readonly string path;
        private readonly Action<InvalidTreeViewItemViewModel> deleteAction;

        public InvalidTreeViewItemViewModel(string path, Action<InvalidTreeViewItemViewModel> deleteAction)
        {
            this.displayPath = path;
            this.path = path;

            var index = path.LastIndexOf(PathUtility.SeparatorChar);
            this.displayName = path.Substring(index + 1);
            
            this.deleteAction = deleteAction;
        }

        public void Delete()
        {
            this.deleteAction(this);
        }

        public override string DisplayName
        {
            get { return this.displayName; }
        }

        public string DisplayPath
        {
            get { return this.displayPath; }
        }

        public bool CanDelete
        {
            get { return this.deleteAction != null; }
        }

        public string Path => this.path;
    }
}
