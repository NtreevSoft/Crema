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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ntreev.Crema.Client.Converters.Properties;
using Ntreev.Crema.Client.Converters.Spreadsheet.Excel;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Extensions;
using Ntreev.Library.Linq;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;

namespace Ntreev.Crema.Client.Converters.Dialogs.ViewModels
{
    public class ExportTypeViewModel : ModalDialogAppBase
    {
        private readonly ITypeExportService exportService;
        private readonly Authentication authentication;
        private readonly IDataBase dataBase;
        private readonly IAppConfiguration configService;
        private readonly TypeRootTreeViewItemViewModel root;
        private readonly ObservableCollection<ExportTreeViewItemViewModel> categories;
        private readonly ObservableCollection<IExporter> exporters;
        private IExporter selectedExporter;
        private bool isExporting;

        private ExportTypeViewModel(Authentication authentication, IDataBase dataBase, string[] selectedPaths)
        {
            this.authentication = authentication;
            this.dataBase = dataBase;
            this.dataBase.Dispatcher.VerifyAccess();
            this.exportService = dataBase.GetService<ITypeExportService>();
            this.configService = dataBase.GetService<IAppConfiguration>();
            this.root = new TypeRootTreeViewItemViewModel(authentication, dataBase, this) {IsExpanded = true};
            this.categories = new ObservableCollection<ExportTreeViewItemViewModel> {this.root};
            this.SelectItems(this.root, selectedPaths);
            
            this.exporters = new ObservableCollection<IExporter>(this.exportService.Exporter);
            this.SelectedExporter = this.exporters.FirstOrDefault(item => item.Name == (string)this.configService[this.GetType(), nameof(SelectedExporter)]);
            this.configService?.Update(this);
            this.DisplayName = Resources.Title_ExportType;
        }

        public IExporter SelectedExporter
        {
            get => this.selectedExporter;
            set
            {
                if (this.selectedExporter != null && this.selectedExporter is INotifyPropertyChanged == true)
                {
                    ((INotifyPropertyChanged) this.selectedExporter).PropertyChanged -= SelectedExporter_PropertyChanged;
                }
                this.selectedExporter = value;
                if (this.selectedExporter != null && this.selectedExporter is INotifyPropertyChanged == true)
                {
                    ((INotifyPropertyChanged) this.selectedExporter).PropertyChanged += SelectedExporter_PropertyChanged;
                }

                this.NotifyOfPropertyChange(nameof(this.SelectedExporter));
                this.NotifyOfPropertyChange(nameof(this.CanExport));
            }
        }

        public IEnumerable Categories => this.categories;

        public static Task<ExportTypeViewModel> CreateInstanceAsync(Authentication authentication, IServiceProvider serviceProvider)
        {
            _ = authentication ?? throw new ArgumentNullException(nameof(authentication));
            _ = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            if (serviceProvider.GetService<IDataBase>() is IDataBase dataBase)
            {
                return dataBase.Dispatcher.InvokeAsync(() => new ExportTypeViewModel(authentication, dataBase, new string[] { }));
            }

            throw new ArgumentException("ServiceProvider does not provide IUserContext", nameof(serviceProvider));
        }

        public async Task ExportAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var progress = new Progress();
            this.CanExport = false;

            try
            {
                this.BeginProgress();
                var categories = this.EnumerateSelectedCategory(this.root).ToArray();

                if (this.SelectedExporter.Settings is ExcelTypeExporterSettings settings)
                {
                    await this.ExcelExportAsync(settings, categories);
                }
                else
                {
                    throw new NotSupportedException(this.SelectedExporter.Settings.GetType().ToString());
                }

                this.configService[this.GetType(), nameof(SelectedExporter)] = this.SelectedExporter.Name;
                this.configService.Commit(this);
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
            finally
            {
                this.EndProgress();
                this.CanExport = true;
                cancellationTokenSource.Dispose();
            }
        }

        private async Task ExcelExportAsync(ExcelTypeExporterSettings settings, TypeCategoryTreeViewItemViewModel[] categories)
        {
            this.ProgressMessage = string.Format(Resources.Message_ReceivingDataFormat, "(Metadata)");
            var metadata = this.dataBase.Dispatcher.Invoke(() => this.dataBase.GetMetaData(this.authentication));
            var typeNames = GetTypeNames(categories);
            var selectedMetaData = SelectedMetaData(metadata, typeNames).ToArray();
            this.ProgressMessage = Resources.Message_ExportingData;

            IEnumerable<(string Path, IEnumerable<TypeInfo> TypeInfos)> GetItems(Func<TypeInfo, string> func)
            {
                return selectedMetaData
                    .GroupBy(tuple => func(tuple.TypeInfo),
                        tuple => tuple.TypeInfo,
                        (path, infos) => (path, infos));
            }

            if (settings.IsSeparable)
            {
                var selectorFunc = settings.IsOneTableToOneFile
                    ? (Func<TypeInfo, string>) (info => info.Name)
                    : info => info.CategoryPath;

                foreach (var item in GetItems(selectorFunc))
                {
                    await Task.Run(() => ((ITypeExporter)this.SelectedExporter).Export(item.Path, item.TypeInfos.ToArray()));
                }
            }
            else
            {
                var typeInfos = selectedMetaData.Select(o => o.TypeInfo).ToArray();
                await Task.Run(() => ((ITypeExporter)this.SelectedExporter).Export(null, typeInfos));
            }
        }

        private static IEnumerable<(TypeInfo TypeInfo, string Path)> SelectedMetaData(DataBaseMetaData metadata, IEnumerable<string> typeNames)
        {
            var selectedMetaData = from m in metadata.Types
                join t in typeNames on m.TypeInfo.Name equals t
                select (m.TypeInfo, m.Path);

            return selectedMetaData;
        }

        private IEnumerable<string> GetTypeNames(TypeCategoryTreeViewItemViewModel[] categories)
        {
            var tableNames = new List<string>();

            foreach (var category in categories)
            {
                if (category is TypeCategoryTreeViewItemViewModel viewModel)
                {
                    GetTypeNames(viewModel, tableNames);
                }
            }

            return tableNames;
        }

        private void GetTypeNames(TypeCategoryTreeViewItemViewModel category, List<string> tableNames)
        {
            foreach (var item in category.Items)
            {
                if (item is TypeCategoryTreeViewItemViewModel categoryViewModel)
                {
                    if (categoryViewModel.IsChecked == false) continue;

                    GetTypeNames(categoryViewModel, tableNames);
                }
                else if (item is TypeTreeViewItemViewModel treeViewModel)
                {
                    if (treeViewModel.IsChecked == false) continue;

                    GetTypeNames(treeViewModel, tableNames);
                }
            }
        }

        private void GetTypeNames(TypeTreeViewItemViewModel treeView, List<string> tableNames)
        {
            foreach (var item in treeView.Items)
            {
                if (item is TypeTreeViewItemViewModel treeViewModel)
                {
                    if (treeViewModel.IsChecked == false) continue;

                    GetTypeNames(treeViewModel, tableNames);
                }
            }

            var tableName = treeView.TypeName;
            if (!tableNames.Contains(tableName))
            {
                tableNames.Add(tableName);
            }
        }

        private void SelectItems(ExportTreeViewItemViewModel viewModel, IEnumerable<string> selectedPaths)
        {
            var decendants = EnumerableUtility.Descendants(viewModel, item => item.Items.OfType<ExportTreeViewItemViewModel>());
            var items = EnumerableUtility.Friends(viewModel, decendants).ToArray();

            var query = from item in items
                join path in selectedPaths on item.Path equals path
                select item;

            foreach (var item in query)
            {
                item.IsChecked = true;
                item.IsExpanded = true;

                var parent = item.Parent;
                while (parent != null)
                {
                    parent.IsExpanded = true;
                    parent = parent.Parent;
                }
                item.IsSelected = true;
            }
        }

        public void Cancel()
        {
            this.ProgressMessage = Resources.Message_Canceling;
        }

        public bool CanExport
        {
            get
            {
                if (this.selectedExporter == null)
                    return false;

                return this.selectedExporter.CanExport;
            }
            protected set
            {
                this.isExporting = !value;
                this.NotifyOfPropertyChange(nameof(this.CanExport));
                this.NotifyOfPropertyChange(nameof(this.CanTryClose));
                this.NotifyOfPropertyChange(nameof(this.CanCancel));
                this.NotifyOfPropertyChange(nameof(this.IsExporting));
            }
        }

        public bool CanTryClose => this.isExporting == false;

        public bool CanCancel => this.isExporting == true;

        public bool IsExporting => this.isExporting == true;

        public ObservableCollection<IExporter> Exporters => this.exporters;

        private IEnumerable<TypeCategoryTreeViewItemViewModel> EnumerateSelectedCategory(TreeViewItemViewModel treeViewItem)
        {
            if (treeViewItem is TypeCategoryTreeViewItemViewModel == true)
                yield return treeViewItem as TypeCategoryTreeViewItemViewModel;

            var descendants = EnumerableUtility.Descendants<TreeViewItemViewModel, TypeCategoryTreeViewItemViewModel>(this.root, item => item.Items, item =>
            {
                if (item is TypeCategoryTreeViewItemViewModel == false)
                    return false;
                var viewModel = item as TypeCategoryTreeViewItemViewModel;
                return viewModel.IsChecked != false;
            });

            foreach (var item in descendants)
            {
                yield return item;
            }
        }

        private void SelectedExporter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IExporter.CanExport))
            {
                this.NotifyOfPropertyChange(nameof(this.CanExport));
            }
        }
    }
}
