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

using Caliburn.Micro;
using Ntreev.Crema.Client.Converters.Spreadsheet.Views;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Crema.Spreadsheet;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.ComponentModel;
using Ntreev.Library.Linq;
using Ntreev.ModernUI.Framework.ViewModels;

namespace Ntreev.Crema.Client.Converters.Spreadsheet.ViewModels
{
    [Export(typeof(IImporter))]
    class ImporterViewModel : ViewAwareBase, IImporter
    {
        private readonly ICremaAppHost cremaAppHost;
        private readonly IAppConfiguration configs;
        private readonly ObservableCollection<SpreadsheetTreeViewItemViewModel> itemsSource = new ObservableCollection<SpreadsheetTreeViewItemViewModel>();
        private string inputPath;
        private string errorMessage;
        private bool? isAllSelected;
        private bool canImport;
        private string[] importedItems = new string[] { };

        [ImportingConstructor]
        public ImporterViewModel(ICremaAppHost cremaAppHost, IAppConfiguration configs)
        {
            this.cremaAppHost = cremaAppHost;
            this.configs = configs;
        }

        public void Import(CremaDataSet dataSet)
        {
            foreach (var item in this.itemsSource)
            {
                if (item.IsChecked == false)
                    continue;

                item.Read(dataSet);
            }

            
            this.importedItems = this.itemsSource.Descendants<TreeViewItemViewModel, SheetTreeViewItemViewModel>(item => item.Items)
                                     .Where(item => item.IsChecked == true)
                                     .Select(item => item.Path)
                                     .ToArray();
            this.configs.Commit(this);
        }

        public string Name
        {
            get { return "Excel Importer"; }
        }

        public IEnumerable ItemsSource
        {
            get
            {
                return this.itemsSource;
            }
        }

        [ConfigurationProperty("InputPath")]
        public string InputPath
        {
            get { return this.inputPath ?? string.Empty; }
            set
            {
                this.inputPath = value;
                this.NotifyOfPropertyChange(nameof(this.InputPath));
                this.NotifyOfPropertyChange(nameof(this.CanImport));
                this.Initialize();
            }
        }

        public bool? IsAllSelected
        {
            get
            {
                if (this.itemsSource.Count == 0)
                    return false;
                return this.isAllSelected;
            }
            set
            {
                this.isAllSelected = value ?? false;
                this.NotifyOfPropertyChange(nameof(this.IsAllSelected));

                foreach (SpreadsheetTreeViewItemViewModel item in this.itemsSource)
                {
                    item.PropertyChanged -= ViewModel_PropertyChanged;
                }

                foreach (SpreadsheetTreeViewItemViewModel item in this.itemsSource)
                {
                    item.IsChecked = this.isAllSelected;
                }

                foreach (SpreadsheetTreeViewItemViewModel item in this.itemsSource)
                {
                    item.PropertyChanged += ViewModel_PropertyChanged;
                }
                this.NotifyOfPropertyChange(nameof(this.CanImport));
            }
        }

        public void SelectPath()
        {
            var dialog = new CommonOpenFileDialog() { Multiselect = true, };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (dialog.FileNames.Count() > 1)
                    this.InputPath = string.Join(" ", dialog.FileNames.Select(item => item.WrapQuot()));
                else
                    this.InputPath = dialog.FileNames.First();
            }
        }

        public bool CanImport
        {
            get
            {
                if (this.InputPath == string.Empty)
                    return false;
                if (this.HasError == true)
                    return false;
                return this.canImport;
            }
            set
            {
                this.canImport = value;
                this.NotifyOfPropertyChange(nameof(this.CanImport));
            }
        }

        public string[] GetTableNames()
        {
            if (this.cremaAppHost.GetService(typeof(IDataBase)) is IDataBase dataBase)
            {
                var tableNames = dataBase.Dispatcher.Invoke(() =>
                {
                    return dataBase.TableContext.Tables.Select(item => item.Name).ToArray();
                });

                var sheetNames = this.itemsSource.SelectMany(item => item.GetSelectedSheetNames()).ToArray();

                var query = from tableName in tableNames
                            let tableName2 = SpreadsheetUtility.Ellipsis(tableName)
                            join sheetName in sheetNames on tableName2 equals sheetName
                            select tableName;

                return query.ToArray();
            }
            throw new NotImplementedException();
        }

        public bool HasError
        {
            get { return this.ErrorMessage != string.Empty; }
        }

        public string ErrorMessage
        {
            get { return this.errorMessage ?? string.Empty; }
            private set
            {
                this.errorMessage = value;
                this.NotifyOfPropertyChange(nameof(this.HasError));
                this.NotifyOfPropertyChange(nameof(this.ErrorMessage));
            }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.configs.Update(this);
        }

        private async void Initialize()
        {
            var items = StringUtility.SplitPath(this.InputPath);
            if (items.Any() == false)
                return;

            try
            {
                var viewModels = await Task.Run(() =>
                {
                    return items.Select(item => new SpreadsheetTreeViewItemViewModel(item) { IsChecked = true, }).ToArray();
                });
                this.itemsSource.Clear();
                foreach (var item in viewModels)
                {
                    item.PropertyChanged += ViewModel_PropertyChanged;
                    this.itemsSource.Add(item);
                }
            }
            catch (Exception e)
            {
                this.ErrorMessage = e.Message;
                return;
            }

            this.ErrorMessage = string.Empty;
            this.RefreshCanImport();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                this.RefreshCanImport();
            }
        }

        private void RefreshCanImport()
        {
            var items = this.itemsSource.FamilyTree<TreeViewItemViewModel, CheckableTreeViewItemViewModel>(item => item.Items).ToArray();
            var count = 0;
            foreach (var item in items)
            {
                if (item.IsChecked == true)
                {
                    count++;
                }
            }

            if (count == 0)
                this.isAllSelected = false;
            else if (count == items.Length)
                this.isAllSelected = true;
            else
                this.isAllSelected = null;

            this.CanImport = count > 0;
            this.NotifyOfPropertyChange(nameof(this.IsAllSelected));
        }

        [ConfigurationProperty]
        private string[] ImportedItems
        {
            get { return this.importedItems; }
            set
            {
                this.importedItems = value ?? new string[] { };
            }
        }
    }
}
