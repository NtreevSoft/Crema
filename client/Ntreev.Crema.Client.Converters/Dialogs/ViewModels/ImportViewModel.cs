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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Caliburn.Micro;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library.IO;
using Ntreev.Crema.Data;
using Ntreev.ModernUI.Framework;
using Ntreev.Library;
using System.ComponentModel;
using Ntreev.Library.Linq;
using Ntreev.Crema.Client.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using Ntreev.Crema.Client.Converters.Properties;

namespace Ntreev.Crema.Client.Converters.Dialogs.ViewModels
{
    public class ImportViewModel : ModalDialogAppBase
    {
        private readonly IDataBase dataBase;
        private readonly IImportService importService;
        private readonly Authentication authentication;
        private readonly IAppConfiguration configService;
        private readonly ObservableCollection<IImporter> importers;
        private IImporter selectedImporter;
        private bool isImporting;
        private CancellationTokenSource cancelToken;
        private string comment;

        private ImportViewModel(Authentication authentication, IDataBase dataBase)
            : base(Resources.Title_Import)
        {
            this.authentication = authentication;
            this.dataBase = dataBase;
            this.dataBase.Dispatcher.VerifyAccess();
            this.importService = dataBase.GetService(typeof(IImportService)) as IImportService;
            this.configService = dataBase.GetService(typeof(IAppConfiguration)) as IAppConfiguration;
            this.importers = new ObservableCollection<IImporter>(this.importService.Importers);
            this.SelectedImporter = this.importers.FirstOrDefault(item => item.Name == (string)this.configService[this.GetType(), "SelectedImporter"]);
        }

        public static Task<ImportViewModel> CreateInstanceAsync(Authentication authentication, IDataBase dataBase)
        {
            return dataBase.Dispatcher.InvokeAsync(() =>
            {
                return new ImportViewModel(authentication, dataBase);
            });
        }

        [Obsolete("Progress")]
        public async void Import()
        {
            this.cancelToken = new CancellationTokenSource();
            this.CanImport = false;

            try
            {
                this.BeginProgress(Resources.Message_Importing);
                var tableNames = this.selectedImporter.GetTableNames();
                var dataSet = new CremaDataSet() { SignatureDateProvider = new SignatureDateProvider(this.authentication.ID), };
                await this.dataBase.Dispatcher.InvokeAsync(() => this.CreateTables(dataSet, tableNames));
                await Task.Run(() => this.selectedImporter.Import(dataSet));
                await this.dataBase.Dispatcher.InvokeAsync(() => dataBase.Import(this.authentication, dataSet, this.Comment));
                this.configService[this.GetType(), nameof(SelectedImporter)] = this.selectedImporter.Name;
                AppMessageBox.Show(Resources.Message_Imported);
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }
            finally
            {
                this.EndProgress();
                this.CanImport = true;
                this.cancelToken = null;
            }
        }

        public void Cancel()
        {
            this.cancelToken.Cancel();
        }

        public ObservableCollection<IImporter> Importers
        {
            get { return this.importers; }
        }

        public IImporter SelectedImporter
        {
            get { return this.selectedImporter; }
            set
            {
                if (this.selectedImporter != null && this.selectedImporter is INotifyPropertyChanged == true)
                {
                    (this.selectedImporter as INotifyPropertyChanged).PropertyChanged -= SelectedImporter_PropertyChanged;
                }
                this.selectedImporter = value;
                if (this.selectedImporter != null && this.selectedImporter is INotifyPropertyChanged == true)
                {
                    (this.selectedImporter as INotifyPropertyChanged).PropertyChanged += SelectedImporter_PropertyChanged;
                }

                this.NotifyOfPropertyChange(nameof(this.SelectedImporter));
                this.NotifyOfPropertyChange(nameof(this.CanImport));
            }
        }

        public string Comment
        {
            get { return this.comment ?? string.Empty; }
            set
            {
                this.comment = value;
                this.NotifyOfPropertyChange(nameof(this.Comment));
                this.NotifyOfPropertyChange(nameof(this.CanImport));
            }
        }

        public bool CanImport
        {
            get
            {
                if (this.selectedImporter == null)
                    return false;

                if (this.Comment == string.Empty)
                    return false;

                return this.selectedImporter.CanImport;
            }
            private set
            {
                this.isImporting = !value;
                this.NotifyOfPropertyChange(nameof(this.CanImport));
                this.NotifyOfPropertyChange(nameof(this.CanTryClose));
                this.NotifyOfPropertyChange(nameof(this.CanCancel));
                this.NotifyOfPropertyChange(nameof(this.IsImporting));
            }
        }

        public bool CanTryClose
        {
            get
            {
                return this.isImporting == false;
            }
        }

        public bool CanCancel
        {
            get { return this.isImporting == true; }
        }

        public bool IsImporting
        {
            get { return this.isImporting == true; }
        }

        private void SelectedImporter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IImporter.CanImport))
            {
                this.NotifyOfPropertyChange(nameof(this.CanImport));
            }
        }

        private void CreateTables(CremaDataSet dataSet, string[] names)
        {
            var tableCollection = this.dataBase.TableContext.Tables;
            var tableList = new List<ITable>(names.Length);

            foreach (var item in names)
            {
                if (tableCollection.Contains(item) == false)
                    throw new TableNotFoundException(item);
                tableList.Add(tableCollection[item]);
            }

            var tables = tableList.ToArray();

            var types = from item in tables
                        from i in EnumerableUtility.Friends(item, item.Childs)
                        from c in i.TableInfo.Columns
                        where c.DataType.StartsWith("/")
                        select c.DataType;

            var types1 = types.Distinct().ToArray();

            dataSet.BeginLoad();
            foreach (var item in types1)
            {
                var typeItem = dataBase.TypeContext[item];
                if (typeItem is IType == false)
                    continue;

                var type = typeItem as IType;

                dataSet.Types.Add(type.TypeInfo);
            }
            dataSet.EndLoad();
            foreach (var item in tables)
            {
                if (item.Parent != null)
                    continue;

                this.CreateTable(dataSet, item);
            }
        }

        private void CreateTable(CremaDataSet dataSet, ITable table)
        {
            var dataTable = dataSet.Tables.Add(GetTableInfo(table));
            foreach (var item in table.Childs)
            {
                dataTable.Childs.Add(GetTableInfo(item));
            }

            TableInfo GetTableInfo(ITable tableItem)
            {
                var tableInfo = tableItem.TableInfo;
                if (tableInfo.TemplatedParent != string.Empty)
                    tableInfo.TemplatedParent = string.Empty;
                return tableInfo;
            }
        }
    }
}
