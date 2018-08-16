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
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Library;
using Ntreev.Library.Linq;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ntreev.Crema.Client.Converters.Properties;

namespace Ntreev.Crema.Client.Converters.Dialogs.ViewModels
{
    public class ExportViewModel : ModalDialogAppBase
    {
        private readonly IDataBase dataBase;
        private readonly IExportService exportService;
        private readonly IAppConfiguration configs;
        private readonly Authentication authentication;
        private readonly ObservableCollection<ExportTreeViewItemViewModel> categories;
        private readonly ObservableCollection<IExporter> exporters;
        private IExporter selectedExporter;

        private bool isExporting;
        private CancellationTokenSource cancelToken;

        private TableRootTreeViewItemViewModel root;

        private ExportViewModel(Authentication authentication, IDataBase dataBase, string[] selectedPaths)
            : base(Resources.Title_Export)
        {
            this.authentication = authentication;
            this.dataBase = dataBase;
            this.dataBase.Dispatcher.VerifyAccess();
            this.exportService = dataBase.GetService(typeof(IExportService)) as IExportService;
            this.configs = dataBase.GetService(typeof(IAppConfiguration)) as IAppConfiguration;

            this.root = new TableRootTreeViewItemViewModel(authentication, this.dataBase, this) { IsExpanded = true, };
            this.categories = new ObservableCollection<ExportTreeViewItemViewModel>
            {
                this.root
            };
            this.SelectItems(this.root, selectedPaths);

            this.exporters = new ObservableCollection<IExporter>(this.exportService.Exporters);
            this.configs.Update(this);
        }

        public static Task<ExportViewModel> CreateInstanceAsync(Authentication authentication, ITableCategoryDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is ITableCategory category && category.GetService(typeof(IDataBase)) is IDataBase dataBase)
            {
                return dataBase.Dispatcher.InvokeAsync(() =>
                {
                    return new ExportViewModel(authentication, dataBase, new string[] { descriptor.Path });
                });
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        public static Task<ExportViewModel> CreateInstanceAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is ITable table && table.GetService(typeof(IDataBase)) is IDataBase dataBase)
            {
                return dataBase.Dispatcher.InvokeAsync(() =>
                {
                    var path = descriptor.TableInfo.CategoryPath + descriptor.TableInfo.Name;
                    return new ExportViewModel(authentication, dataBase, new string[] { path });
                });
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        public static Task<ExportViewModel> CreateInstanceAsync(Authentication authentication, IServiceProvider serviceProvider)
        {
            return CreateInstanceAsync(authentication, serviceProvider, new string[] { });
        }

        public static Task<ExportViewModel> CreateInstanceAsync(Authentication authentication, IServiceProvider serviceProvider, string[] selectedPaths)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (selectedPaths == null)
                throw new ArgumentNullException(nameof(selectedPaths));

            if (serviceProvider.GetService(typeof(IDataBase)) is IDataBase dataBase)
            {
                return dataBase.Dispatcher.InvokeAsync(() =>
                {
                    return new ExportViewModel(authentication, dataBase, selectedPaths);
                });
            }
            else
            {
                throw new ArgumentException("ServiceProvider does not provide IUserContext", nameof(serviceProvider));
            }
        }

        public ObservableCollection<IExporter> Exporters
        {
            get { return this.exporters; }
        }

        public IExporter SelectedExporter
        {
            get { return this.selectedExporter; }
            set
            {
                if (this.selectedExporter != null && this.selectedExporter is INotifyPropertyChanged == true)
                {
                    (this.selectedExporter as INotifyPropertyChanged).PropertyChanged -= SelectedExporter_PropertyChanged;
                }
                this.selectedExporter = value;
                if (this.selectedExporter != null && this.selectedExporter is INotifyPropertyChanged == true)
                {
                    (this.selectedExporter as INotifyPropertyChanged).PropertyChanged += SelectedExporter_PropertyChanged;
                }

                this.NotifyOfPropertyChange(nameof(this.SelectedExporter));
                this.NotifyOfPropertyChange(nameof(this.CanExport));
            }
        }

        public IEnumerable Categories
        {
            get { return this.categories; }
        }

        public async Task ExportAsync()
        {
            this.cancelToken = new CancellationTokenSource();

            var progress = new Progress();
            this.CanExport = false;

            try
            {
                this.BeginProgress();
                var categories = this.EnumerateSelectedCategory(this.root).ToArray();

                var descendants = EnumerableUtility.Descendants<TreeViewItemViewModel, TableCategoryTreeViewItemViewModel>(this.root, item => item.Items, item =>
                {
                    if (item is TableCategoryTreeViewItemViewModel == false)
                        return false;
                    var viewModel = item as TableCategoryTreeViewItemViewModel;
                    return viewModel.IsChecked != false;
                });

                if (this.selectedExporter.Settings.IsSeparable == true)
                {
                    var query = from item in categories
                                from tableItem in item.Items.OfType<TableTreeViewItemViewModel>()
                                where tableItem.IsChecked != false
                                select tableItem as ExportTreeViewItemViewModel;
                    var items = this.selectedExporter.Settings.IsOneTableToOneFile == true ? query : categories;
                    foreach (var item in items)
                    {
                        this.ProgressMessage = string.Format(Resources.Message_ReceivingDataFormat, item.Path);
                        var dataSet = new CremaDataSet();
                        await this.dataBase.Dispatcher.InvokeAsync(() => dataSet.ExtendedProperties[typeof(DataBaseInfo)] = this.dataBase.DataBaseInfo);
                        await item.PreviewAsync(dataSet);
                        if (this.cancelToken.IsCancellationRequested == true)
                            break;

                        this.ProgressMessage = string.Format(Resources.Message_ExportingDataFormat, item.Path);
                        await Task.Run(() => this.selectedExporter.Export(item.Path, dataSet));
                        if (this.cancelToken.IsCancellationRequested == true)
                            break;
                    }
                }
                else
                {
                    var dataSet = new CremaDataSet();
                    foreach (var item in categories)
                    {
                        this.ProgressMessage = string.Format(Resources.Message_ReceivingDataFormat, item.Path);
                        await this.dataBase.Dispatcher.InvokeAsync(() => dataSet.ExtendedProperties[typeof(DataBaseInfo)] = this.dataBase.DataBaseInfo);
                        await item.PreviewAsync(dataSet);
                        if (this.cancelToken.IsCancellationRequested == true)
                            break;
                    }

                    this.ProgressMessage = Resources.Message_ExportingData;
                    await Task.Run(() => this.selectedExporter.Export(null, dataSet));
                }
                this.configs.Commit(this);
                AppMessageBox.Show(Resources.Message_Exported);
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
            finally
            {
                this.EndProgress();
                this.CanExport = true;
                this.cancelToken = null;
            }
        }

        public void Cancel()
        {
            this.cancelToken.Cancel();
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
            private set
            {
                this.isExporting = !value;
                this.NotifyOfPropertyChange(nameof(this.CanExport));
                this.NotifyOfPropertyChange(nameof(this.CanTryClose));
                this.NotifyOfPropertyChange(nameof(this.CanCancel));
                this.NotifyOfPropertyChange(nameof(this.IsExporting));
            }
        }

        public bool CanTryClose
        {
            get { return this.isExporting == false; }
        }

        public bool CanCancel
        {
            get { return this.isExporting == true; }
        }

        public bool IsExporting
        {
            get { return this.isExporting == true; }
        }

        private IEnumerable<TableCategoryTreeViewItemViewModel> GetCategories(TreeViewItemViewModel treeViewItem)
        {
            if (treeViewItem is TableCategoryTreeViewItemViewModel == true)
            {
                yield return treeViewItem as TableCategoryTreeViewItemViewModel;
            }

            foreach (var item in treeViewItem.Items)
            {
                foreach (var i in this.GetCategories(item))
                {
                    yield return i;
                }
            }
        }

        private IEnumerable<TableCategoryTreeViewItemViewModel> EnumerateSelectedCategory(TreeViewItemViewModel treeViewItem)
        {
            if (treeViewItem is TableCategoryTreeViewItemViewModel == true)
                yield return treeViewItem as TableCategoryTreeViewItemViewModel;

            var descendants = EnumerableUtility.Descendants<TreeViewItemViewModel, TableCategoryTreeViewItemViewModel>(this.root, item => item.Items, item =>
            {
                if (item is TableCategoryTreeViewItemViewModel == false)
                    return false;
                var viewModel = item as TableCategoryTreeViewItemViewModel;
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

        [ConfigurationProperty("SelectedExporter")]
        private string SelectedExporterName
        {
            get { return this.selectedExporter?.Name; }
            set
            {
                if (value != null)
                {
                    this.SelectedExporter = this.exporters.FirstOrDefault(item => item.Name == (string)value);
                }
            }
        }
    }
}
