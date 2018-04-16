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
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.Crema.Comparer.Types.ViewModels
{
    [Export]
    class TypeBrowserViewModel : TreeViewViewModel
    {
        [Import]
        private Lazy<IServiceProvider> serviceProvider = null;
        private IShell shell;
        [Import]
        private Lazy<TypePropertyViewModel> property = null;

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
                });
            }
        }

        public void Merge()
        {
            var items = this.Items.SelectMany((TreeViewItemViewModel item) => EnumerableUtility.FamilyTree(item, i => i.Items))
                                  .OfType<TypeTreeViewItemViewModel>()
                                  .Select(item => item.Source)
                                  .Where(item => item.IsResolved == false);

            foreach (var item in items)
            {
                item.Merge();
            }
        }

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);

            this.property.Value.SelectedObject = this.SelectedItem;
        }

        private void UpdateItemsSource()
        {
            var dataSet = this.shell.DataSet;
            var compositionService = this.ServiceProvider.GetService(typeof(ICompositionService)) as ICompositionService;

            foreach (var item in dataSet.Types)
            {
                var viewModel = new TypeTreeViewItemViewModel(item)
                {
                    Header1 = dataSet.Header1,
                    Header2 = dataSet.Header2
                };
                compositionService.SatisfyImportsOnce(viewModel);
                this.Items.Add(viewModel);
            }
        }

        private void Shell_Loaded(object sender, EventArgs e)
        {
            this.UpdateItemsSource();
        }

        private void Shell_Unloaded(object sender, EventArgs e)
        {
            this.Items.Clear();
        }

        private IServiceProvider ServiceProvider => this.serviceProvider.Value;
    }
}
