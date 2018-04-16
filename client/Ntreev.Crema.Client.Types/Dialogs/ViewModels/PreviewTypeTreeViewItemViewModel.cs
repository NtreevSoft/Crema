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

using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ntreev.Crema.Client.Types.Dialogs.ViewModels
{
    class PreviewTypeTreeViewItemViewModel : ItemTreeViewItemViewModel
    {
        private Action<PreviewTypeTreeViewItemViewModel> viewAction;
        private ICommand viewCommand;

        public PreviewTypeTreeViewItemViewModel(string path, Action<PreviewTypeTreeViewItemViewModel> viewAction)
            : base(path)
        {
            this.IsExpanded = true;
            this.viewAction = viewAction;
            this.viewCommand = new DelegateCommand(this.View);
        }

        public void View()
        {
            this.viewAction(this);
        }

        public ICommand ViewCommand
        {
            get { return this.viewCommand; }
        }

        public override string DisplayName => NameUtility.GetName(base.DisplayName);
    }
}
