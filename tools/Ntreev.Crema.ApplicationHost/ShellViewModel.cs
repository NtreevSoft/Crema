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

using Ntreev.Crema.ApplicationHost.ViewModels;
using Ntreev.Crema.Tools.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ApplicationHost
{
    [Export(typeof(IShell))]
    [Export(typeof(IContentService))]
    [InheritedExport(typeof(ShellViewModel))]
    class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell, IContentService
    {
        private readonly ObservableCollection<object> contents;

        private object selectedContent;

        [ImportingConstructor]
        public ShellViewModel([ImportMany]IEnumerable<IContent> contents)
        {
            this.contents = new ObservableCollection<object>();
            this.contents.Add(new ConsoleViewModel());
            foreach (var item in contents)
            {
                this.contents.Add(item);
            }

            this.selectedContent = this.contents.First();
        }

        public object SelectedContent
        {
            get { return this.selectedContent; }
            set
            {
                if (this.selectedContent == value)
                    return;
                this.selectedContent = this.contents.SingleOrDefault(item => item == value);
                this.NotifyOfPropertyChange(() => this.SelectedContent);
            }
        }

        public ObservableCollection<object> Contents
        {
            get { return this.contents; }
        }
    }
}
