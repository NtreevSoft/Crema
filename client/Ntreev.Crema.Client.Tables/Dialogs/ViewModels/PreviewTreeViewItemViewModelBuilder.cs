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
using Ntreev.Library;
using Ntreev.Library.ObjectModel;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ntreev.Crema.Client.Tables.Dialogs.ViewModels
{
    class PreviewTreeViewItemViewModelBuilder : TreeViewItemViewModelBuilder
    {
        private readonly Action<PreviewTableTreeViewItemViewModel> viewAction;

        public PreviewTreeViewItemViewModelBuilder(Action<PreviewTableTreeViewItemViewModel> viewAction)
        {
            this.viewAction = viewAction;
        }

        protected override TreeViewItemViewModel CreateCategory(string path)
        {
            return new PreviewTableCategoryTreeViewItemViewModel(path);
        }

        protected override TreeViewItemViewModel CreateItem(string path)
        {
            return new PreviewTableTreeViewItemViewModel(path,this.viewAction);
        }

        protected override string GetParentPath(string path)
        {
            if (NameValidator.VerifyItemPath(path) == true)
            {
                var itemName = new ItemName(path);
                var parentName = NameUtility.GetParentName(itemName.Name);
                if (parentName != string.Empty)
                {
                    return new ItemName(itemName.CategoryPath, parentName);
                }
            }
            return base.GetParentPath(path);
        }
    }
}
