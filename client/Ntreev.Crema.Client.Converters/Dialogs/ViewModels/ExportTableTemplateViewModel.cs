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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shell;
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
    public class ExportTableTemplateViewModel : ExportViewModel
    {
        private readonly ITableTemplateExportService exportService;
        private readonly ObservableCollection<IExporter> exporters;

        private ExportTableTemplateViewModel(Authentication authentication, IDataBase dataBase, string[] selectedPaths)
            : base(Resources.Title_ExportTableTemplate, authentication, dataBase, selectedPaths)
        {
            this.exportService = dataBase.GetService<ITableTemplateExportService>();
            this.exporters = new ObservableCollection<IExporter>(this.exportService.Exporter);
            this.SelectedExporter = this.exporters.FirstOrDefault(item => item.Name == (string)this.configService[this.GetType(), nameof(SelectedExporter)]);
            
            this.configService?.Update(this);
        }

        public static Task<ExportTableTemplateViewModel> CreateInstanceAsync(Authentication authentication, IServiceProvider serviceProvider)
        {
            _ = authentication ?? throw new ArgumentNullException(nameof(authentication));
            _ = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            if (serviceProvider.GetService<IDataBase>() is IDataBase dataBase)
            {
                return dataBase.Dispatcher.InvokeAsync(() => new ExportTableTemplateViewModel(authentication, dataBase, new string[] { }));
            }

            throw new ArgumentException("ServiceProvider does not provide IUserContext", nameof(serviceProvider));
        }

        public override async Task ExportAsync(bool showExportCompletedMessageBox = true)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var progress = new Progress();
            this.CanExport = false;

            try
            {
                this.BeginProgress();
                var categories = this.EnumerateSelectedCategory(this.Root).ToArray();

                if (this.SelectedExporter.Settings is ExcelTableTemplateExporterSettings settings)
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

        private async Task ExcelExportAsync(ExcelTableTemplateExporterSettings settings, TableCategoryTreeViewItemViewModel[] categories)
        {
            this.ProgressMessage = string.Format(Resources.Message_ReceivingDataFormat, "(Metadata)");
            var metadata = this.DataBase.Dispatcher.Invoke(() => this.DataBase.GetMetaData(this.Authentication));
            var tableNames = GetTableNames(categories);
            var selectedMetaData = SelectedMetaData(metadata, tableNames).ToArray();
            this.LastExportedTables = selectedMetaData.Select(o => o.Path).ToArray();
            this.ProgressMessage = Resources.Message_ExportingData;

            if (settings.IsSeparable)
            {
                var tableInfos = new Dictionary<string, List<TableInfo>>();
                var keyPaths = new Dictionary<string, string>();

                if (settings.IsOneTableToOneFile)
                {
                    FillTableInfos(selectedMetaData, ref tableInfos, ref keyPaths, (tableInfo) => tableInfo.TableName, tableInfo => tableInfo.ParentName, item => item.Path);
                }
                else
                {
                    FillTableInfos(selectedMetaData, ref tableInfos, ref keyPaths, (tableInfo) => tableInfo.CategoryPath, tableInfo => tableInfo.CategoryPath, item => item.TableInfo.CategoryPath);
                }

                var tasks = new List<Task>();
                foreach (var item in tableInfos)
                {
                    tasks.Add(Task.Run(() => ((ITableTemplateExporter)this.SelectedExporter).Export(keyPaths[item.Key], item.Value.ToArray())));
                }

                await Task.WhenAll(tasks.ToArray());
            }
            else
            {
                var tableInfos = selectedMetaData.Select(o => o.TableInfo).ToArray();
                await Task.Run(() => ((ITableTemplateExporter)this.SelectedExporter).Export(null, tableInfos));
            }
        }

        private void FillTableInfos((TableInfo TableInfo, string Path)[] selectedMetaData, ref Dictionary<string, List<TableInfo>> tableInfos, ref Dictionary<string, string> keyPaths, Func<TableInfo, string> func, Func<TableInfo, string> keySelector, Func<(TableInfo TableInfo, string Path), string> funcPath)
        {
            foreach (var item in selectedMetaData.Where(item => string.IsNullOrWhiteSpace(item.TableInfo.ParentName)))
            {
                var key = func(item.TableInfo);

                if (!tableInfos.ContainsKey(key))
                {
                    tableInfos.Add(key, new List<TableInfo> { item.TableInfo });
                }
                else
                {
                    tableInfos[key].Add(item.TableInfo);
                }

                if (!keyPaths.ContainsKey(key))
                {
                    keyPaths.Add(key, funcPath(item));
                }
            }

            foreach (var item in selectedMetaData.Where(item => !string.IsNullOrWhiteSpace(item.TableInfo.ParentName)))
            {
                tableInfos[keySelector(item.TableInfo)].Add(item.TableInfo);
            }
        }

        private static IEnumerable<(TableInfo TableInfo, string Path)> SelectedMetaData(DataBaseMetaData metadata, IEnumerable<string> tableNames)
        {
            var selectedMetaData = from m in metadata.Tables
                join t in tableNames on m.TableInfo.TableName equals t
                select (m.TableInfo, m.Path);

            return selectedMetaData;
        }

        private IEnumerable<string> GetTableNames(TableCategoryTreeViewItemViewModel[] categories)
        {
            var tableNames = new List<string>();

            foreach (var category in categories)
            {
                if (category is TableCategoryTreeViewItemViewModel viewModel)
                {
                    GetTableNames(viewModel, tableNames);
                }
            }

            return tableNames;
        }

        private void GetTableNames(TableCategoryTreeViewItemViewModel category, List<string> tableNames)
        {
            foreach (var item in category.Items)
            {
                if (item is TableCategoryTreeViewItemViewModel categoryViewModel)
                {
                    if (categoryViewModel.IsChecked == false) continue;

                    GetTableNames(categoryViewModel, tableNames);
                }
                else if (item is TableTreeViewItemViewModel treeViewModel)
                {
                    if (treeViewModel.IsChecked == false) continue;

                    GetTableNames(treeViewModel, tableNames);
                }
            }
        }

        private void GetTableNames(TableTreeViewItemViewModel treeView, List<string> tableNames)
        {
            foreach (var item in treeView.Items)
            {
                if (item is TableTreeViewItemViewModel treeViewModel)
                {
                    if (treeViewModel.IsChecked == false) continue;

                    GetTableNames(treeViewModel, tableNames);
                }
            }

            var tableName = treeView.TableName;
            if (!tableNames.Contains(tableName))
            {
                tableNames.Add(tableName);
            }
        }

        public override ObservableCollection<IExporter> Exporters => this.exporters;

        private IEnumerable<TableCategoryTreeViewItemViewModel> EnumerateSelectedCategory(TreeViewItemViewModel treeViewItem)
        {
            if (treeViewItem is TableCategoryTreeViewItemViewModel == true)
                yield return treeViewItem as TableCategoryTreeViewItemViewModel;

            var descendants = EnumerableUtility.Descendants<TreeViewItemViewModel, TableCategoryTreeViewItemViewModel>(this.Root, item => item.Items, item =>
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
    }
}
