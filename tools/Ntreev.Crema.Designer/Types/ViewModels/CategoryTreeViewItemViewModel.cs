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
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Threading.Tasks;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Data;
using Ntreev.Library.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using Ntreev.ModernUI.Framework.ViewModels;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Data.Xml.Schema;
using System.Text.RegularExpressions;
using Ntreev.Library;
using Ntreev.Crema.Designer.Properties;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;

namespace Ntreev.Crema.Designer.Types.ViewModels
{
    public class CategoryTreeViewItemViewModel : TreeViewItemViewModel
    {
        private ISelector selector;

        private ICommand renameCommand;
        private ICommand deleteCommand;

        private readonly CremaDataSet dataSet;
        private string categoryName;
        private string categoryPath;

        public CategoryTreeViewItemViewModel(CremaDataSet dataSet, string categoryPath, ISelector selector)
        {
            this.dataSet = dataSet;
            this.categoryPath = categoryPath;
            this.categoryName = new CategoryName(categoryPath).Name;
            this.Target = categoryPath;
            this.selector = selector;

            this.renameCommand = new DelegateCommand(item => this.Rename());
            this.deleteCommand = new DelegateCommand(item => this.Delete());
        }

        public override string DisplayName
        {
            get { return this.categoryName ?? string.Empty; }
        }

        public string Name
        {
            get { return this.categoryName ?? string.Empty; }
        }

        public string Path
        {
            get { return this.categoryPath ?? string.Empty; }
        }

        public override int Order
        {
            get { return 1; }
        }

        public void NewType()
        {
            var dataSet = new CremaDataSet();
            foreach (var item in this.dataSet.Types)
            {
                item.CopyTo(dataSet);
            }
            var dataType = dataSet.Types.Add();
            var dialog = new NewTypeViewModel(dataType);

            if (dialog.ShowDialog() != true)
                return;

            var copiedType = dataType.CopyTo(this.dataSet);
            //var dataType = template.TargetType.CopyTo(this.dataSet);
            var viewModel = new TypeTreeViewItemViewModel(copiedType, this.selector)
            {
                Parent = this,
            };
        }

        public void NewFolder()
        {
            var categoryPaths = this.dataSet.ExtendedProperties[CremaSchema.TypeDirectory] as string[];
            var dialog = new NewCategoryViewModel(this.categoryPath, categoryPaths);
            if (dialog.ShowDialog() != true)
                return;

            var categoryName = new CategoryName(this.categoryPath, dialog.CategoryName);
            var viewModel = new CategoryTreeViewItemViewModel(this.dataSet, categoryName.Path, this.selector)
            {
                Parent = this,
            };
            var categoryPathList = categoryPaths.ToList();
            categoryPathList.Add(categoryName.Path);
            this.dataSet.ExtendedProperties[CremaSchema.TypeDirectory] = categoryPathList.OrderBy(item => item).ToArray();
            this.IsExpanded = true;
            viewModel.IsSelected = true;
        }

        public void Find()
        {
            //var service = this.category.GetService(typeof(ITypeDocumentService)) as ITypeDocumentService;
            //service?.AddFinder(this.categoryPath);
        }

        public void Rename()
        {
            var categoryPaths = this.dataSet.ExtendedProperties[CremaSchema.TypeDirectory] as string[];
            var dialog = new RenameCategoryViewModel(this.categoryPath, categoryPaths);
            if (dialog.ShowDialog() != true)
                return;

            var categoryName = new CategoryName(this.categoryPath)
            {
                Name = dialog.NewName,
            };

            for (var i = 0; i < categoryPaths.Length; i++)
            {
                var categoryPath = categoryPaths[i];
                if (categoryPath.StartsWith(this.categoryPath) == true)
                {
                    categoryPaths[i] = categoryName.Path + categoryPath.Substring(this.categoryPath.Length);
                }
            }

            foreach (var item in this.dataSet.Types)
            {
                var categoryPath = item.CategoryPath;
                if (categoryPath.StartsWith(this.categoryPath) == true)
                {
                    item.CategoryPath = categoryName.Path + categoryPath.Substring(this.categoryPath.Length);
                }
            }

            this.dataSet.ExtendedProperties[CremaSchema.TypeDirectory] = categoryPaths;
            this.categoryPath = categoryName.Path;
            this.categoryName = categoryName.Name;
            this.Refresh();
            if (this.selector != null)
                this.selector.SelectedItem = this;
        }

        public void Move()
        {
            var categoryPaths = this.dataSet.ExtendedProperties[CremaSchema.TypeDirectory] as string[];
            var dialog = new MoveViewModel(this.categoryPath, categoryPaths);
            if (dialog.ShowDialog() != true)
                return;

            //await this.UnlockAsync(comment);

            //if (this.selector != null)
            //    this.ExpandAncestors();
        }

        public void Delete()
        {
            var dialog = new DeleteViewModel()
            {
                DisplayName = Resources.Title_DeleteCategory
            };
            if (dialog.ShowDialog() != true)
                return;
        }

        public ICommand RenameCommand
        {
            get { return this.renameCommand; }
        }

        public ICommand DeleteCommand
        {
            get { return this.deleteCommand; }
        }
    }
}
