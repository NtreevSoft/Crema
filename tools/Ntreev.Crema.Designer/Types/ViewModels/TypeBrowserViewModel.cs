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
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.Crema.Designer.Types.ViewModels
{
    [Export]
    class TypeBrowserViewModel : TreeViewViewModel
    {
        [Import]
        private Lazy<IServiceProvider> serviceProvider = null;
        [Import]
        private IAppConfiguration configs = null;
        private IShell shell;

        [ImportingConstructor]
        public TypeBrowserViewModel(IShell shell)
        {
            this.shell = shell;
            this.shell.Loaded += Shell_Loaded;
            this.shell.Unloaded += Shell_Unloaded;

            if (this.shell.DataSet != null)
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    this.UpdateItemsSource();
                    this.LoadSettings();
                });
            }
        }

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);

            if (this.ServiceProvider.GetService(typeof(ITypePropertyService)) is ITypePropertyService propertyService)
            {
                propertyService.SelectedObject = this.SelectedItem;
            }
        }

        private void UpdateItemsSource()
        {
            var viewModels = new Dictionary<string, TreeViewItemViewModel>();
            var dataSet = this.shell.DataSet;
            var categories = dataSet.ExtendedProperties[CremaSchema.TypeDirectory] as string[];
            var compositionService = this.ServiceProvider.GetService(typeof(ICompositionService)) as ICompositionService;

            foreach (var item in categories)
            {
                if (item == PathUtility.Separator)
                {
                    viewModels.Add(item, new DataBaseTreeViewItemViewModel(dataSet, this));
                }
                else
                {
                    viewModels.Add(item, new CategoryTreeViewItemViewModel(dataSet, item, this));
                }
            }
            foreach (var item in dataSet.Types)
            {
                viewModels.Add(item.CategoryPath + item.Name, new TypeTreeViewItemViewModel(item, this));
            }

            foreach (var item in viewModels)
            {
                if (item.Key == PathUtility.Separator)
                {
                    compositionService.SatisfyImportsOnce(item.Value);
                }
                else
                {
                    if (NameValidator.VerifyCategoryPath(item.Key) == true)
                    {
                        var categoryName = new CategoryName(item.Key);
                        item.Value.Parent = viewModels[categoryName.ParentPath];
                    }
                    else
                    {
                        var itemName = new ItemName(item.Key);
                        item.Value.Parent = viewModels[itemName.CategoryPath];
                    }
                }
            }

            this.Items.Clear();
            this.Items.Add(viewModels[PathUtility.Separator]);
        }

        private void Shell_Loaded(object sender, EventArgs e)
        {
            this.UpdateItemsSource();
            this.LoadSettings();
        }

        private void Shell_Unloaded(object sender, EventArgs e)
        {
            this.SaveSettings();
        }

        private void SaveSettings()
        {
            var items = this.GetSettings();

            //this.configs[this.GetType(), $"{this.shell.DataSetPath}"] = items;
        }

        private void LoadSettings()
        {
            //if (this.shell.DataSetPath != null && this.configs.TryParse<string[]>(this.GetType(), $"{this.shell.DataSetPath}", out var savedItems) == true)
            //{
            //    this.SetSettings(savedItems);
            //}
        }

        private IServiceProvider ServiceProvider => this.serviceProvider.Value;
    }
}
