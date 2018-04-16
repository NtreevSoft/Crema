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

using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.ModernUI.Framework.ViewModels;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using System.ComponentModel.Composition;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.SmartSet.Dialogs.ViewModels;

namespace Ntreev.Crema.Client.SmartSet.BrowserItems.ViewModels
{
    abstract class BookmarkCategoryTreeViewItemViewModel : TreeViewItemViewModel
    {
        private CategoryName categoryName;

        protected BookmarkCategoryTreeViewItemViewModel(string path, SmartSetBrowserViewModel browser)
        {
            NameValidator.ValidateCategoryPath(path);
            this.categoryName = new CategoryName(path);
            this.Target = path;
            this.Owner = browser;
        }

        public void NewFolder()
        {
            var query = from item in this.Items
                        where item is BookmarkCategoryTreeViewItemViewModel
                        let viewModel = item as BookmarkCategoryTreeViewItemViewModel
                        select viewModel.DisplayName;

            var dialog = new NewCategoryViewModel(PathUtility.Separator, query.ToArray());
            if (dialog.ShowDialog() != true)
                return;

            try
            {
                var viewModel = this.Browser.BookmarkCategory.CreateInstance(dialog.CategoryPath, this.Browser);
                this.Items.Add(viewModel);
                this.Items.Reposition(viewModel);
                this.IsExpanded = true;
                this.Browser.UpdateBookmarkItems();
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
        }

        // 북마크 폴더 삭제
        public void Delete()
        {
            var dialog = new DeleteViewModel();
            if (dialog.ShowDialog() == true)
            {
                this.Parent.Items.Remove(this);
                this.Browser.UpdateBookmarkItems();
            }
        }

        // 북마크 폴더 이름 변경
        public void Rename()
        {
            var categoryPaths = this.Browser.GetBookmarkCategoryPaths();
            var dialog = new RenameBookmarkCategoryViewModel(this.categoryName.Path, categoryPaths);
            if (dialog.ShowDialog() != true)
                return;

            try
            {
                this.categoryName.Name = dialog.NewName;
                this.NotifyOfPropertyChange(nameof(this.DisplayName));
                this.Parent.Items.Reposition(this);
                this.Browser.UpdateBookmarkItems();
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
        }

        // 북마크 폴더 이동
        public void Move()
        {
            var itemPaths = this.Browser.GetBookmarkItemPaths();
            var dialog = new MoveBookmarkItemViewModel(this.Path, itemPaths);
            if (dialog.ShowDialog() != true)
                return;

            try
            {
                var parentViewModel = this.Browser.GetBookmarkItem(dialog.TargetPath);
                this.Parent = parentViewModel;
                this.categoryName = new CategoryName(dialog.TargetPath, this.categoryName.Name);
                this.Browser.UpdateBookmarkItems();
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
        }

        public override string DisplayName
        {
            get { return this.categoryName.Name; }
        }

        public string DisplayPath
        {
            get { return this.categoryName.Path; }
        }

        public string Path
        {
            get { return this.categoryName.Path; }
        }

        public string Name
        {
            get { return categoryName.Name; }
        }

        public override int Order
        {
            get { return 1; }
        }

        public SmartSetBrowserViewModel Browser
        {
            get { return this.Owner as SmartSetBrowserViewModel; }
        }
    }
}
