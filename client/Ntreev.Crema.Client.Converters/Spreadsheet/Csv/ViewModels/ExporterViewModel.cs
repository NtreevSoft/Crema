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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;
using Ntreev.Crema.Client.Converters.Spreadsheet.Excel;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Data;
using Ntreev.Crema.Spreadsheet.Csv;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.ModernUI.Framework;

namespace Ntreev.Crema.Client.Converters.Spreadsheet.Csv.ViewModels
{
    [Export(typeof(IExporter))]
    class ExporterViewModel : Screen, IExporter
    {
        private readonly ICremaAppHost cremaAppHost;
        private readonly IAppConfiguration configs;
        private ObservableCollection<string> outputPaths = new ObservableCollection<string> {""};

        [ImportingConstructor]
        public ExporterViewModel(ICremaAppHost cremaAppHost, IAppConfiguration configs)
        {
            this.cremaAppHost = cremaAppHost;
            this.configs = configs;
            this.Settings = new CsvExporterSettings();
            if (this.Settings is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged += NotifyPropertyChangedOnPropertyChanged;
            }
            this.configs.Update(this.Settings);
            this.configs.Update(this);
        }

        private void NotifyPropertyChangedOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyOfPropertyChange(nameof(CanExport));
        }

        public void Export(string itemPath, CremaDataSet dataSet)
        {
            if (!dataSet.Tables.Any()) return;

            var settings = this.Settings as CsvExporterSettings;
            if (settings == null) throw new NullReferenceException(nameof(settings));

            var spreadCsvWriterSettings = new SpreadsheetCsvWriterSettings
            {
                OmitAttribute = settings.OmitAttribute,
                OmitSignatureDate = settings.OmitSignatureDate,
                CategorySeperatorString = settings.CategorySeperatorString,
                Delimiter = settings.Delimiter,
                Extension = settings.Extension,
                FilenamePattern = settings.FilenamePattern,
                CreateDirectoryIfNotExists = settings.CreateDirectoryIfNotExists
            };

            var writer = new SpreadsheetCsvWriter(dataSet, spreadCsvWriterSettings);

            foreach (var outputPath in this.OutputPaths)
            {
                var filePath = Path.Combine(outputPath, spreadCsvWriterSettings.FilenamePattern);
                writer.Write(filePath);
            }
            this.configs.Commit(this.Settings);
            this.configs.Commit(this);
        }

        public void SelectPath(string path)
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var index = this.OutputPaths.IndexOf(path);
                if (index >= 0)
                {
                    this.OutputPaths[index] = dialog.FileName;
                }
            }

            this.NotifyOfPropertyChange(nameof(this.OutputPaths));
            this.NotifyOfPropertyChange(nameof(this.CanExport));
        }

        public void AddPath()
        {
            this.OutputPaths.Add("");
            this.NotifyOfPropertyChange(nameof(this.OutputPaths));
            this.NotifyOfPropertyChange(nameof(this.CanExport));
        }

        public void RemovePath(string path)
        {
            this.OutputPaths.Remove(path);
            this.NotifyOfPropertyChange(nameof(this.OutputPaths));
            this.NotifyOfPropertyChange(nameof(this.CanExport));
        }

        public string Name => "Csv Exporter";

        [ConfigurationProperty(nameof(outputPaths))]
        public ObservableCollection<string> OutputPaths
        {
            get
            {
                if (this.outputPaths == null || !this.outputPaths.Any())
                {
                    return (this.outputPaths = new ObservableCollection<string> {""});
                }

                return this.outputPaths;
            }
            set
            {
                this.outputPaths = value;
                this.NotifyOfPropertyChange(nameof(this.OutputPaths));
                this.NotifyOfPropertyChange(nameof(this.CanExport));
            }
        }

        public bool CanExport
        {
            get
            {
                var settings = this.Settings as CsvExporterSettings;
                if (settings == null) throw new NullReferenceException(nameof(settings));

                foreach (var outputPath in this.OutputPaths)
                {
                    if (string.IsNullOrEmpty(outputPath) == true)
                        return false;

                    if (settings.CreateDirectoryIfNotExists == false)
                    {
                        if (Directory.Exists(outputPath) == false)
                            return false;

                        if (DirectoryUtility.IsDirectory(outputPath) == true)
                            return false;
                    }
                }

                return true;
            }
        }

        public object Settings { get; }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.configs.Update(this);
        }
    }
}
