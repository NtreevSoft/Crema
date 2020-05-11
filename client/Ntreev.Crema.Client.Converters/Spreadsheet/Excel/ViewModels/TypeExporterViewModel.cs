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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Caliburn.Micro;
using ClosedXML.Excel;
using Microsoft.WindowsAPICodePack.Dialogs;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Data;
using Ntreev.Crema.Spreadsheet.Excel;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.ModernUI.Framework;

namespace Ntreev.Crema.Client.Converters.Spreadsheet.Excel.ViewModels
{
    [Export(typeof(ITypeExporter))]
    class TypeExporterViewModel : Screen, ITypeExporter
    {
        private ICremaAppHost cremaAppHost;
        private IAppConfiguration configs;
        private string outputPath;
        private ExcelTypeExporterSettings settings;

        public string Name => "Excel Exporter";

        [ImportingConstructor]
        public TypeExporterViewModel(ICremaAppHost cremaAppHost, IAppConfiguration configs)
        {
            this.cremaAppHost = cremaAppHost;
            this.configs = configs;
            this.settings = new ExcelTypeExporterSettings();
            this.settings.PropertyChanged += Settings_PropertyChanged;
            this.configs.Update(this.settings);
        }

        public void Export(string itemPath, CremaDataSet dataSet)
        {
            throw new NotSupportedException();
        }

        public void Export(string itemPath, TypeInfo[] typeInfos)
        {
            if (!typeInfos.Any()) return;

            var writerSettings = new ExcelWriterSettings<TypeInfo, TypeMemberInfo>
            {
                GetWorksheetNameFunc = t => t.Name,
                GetDetailFunc = t => t.Members,
                GetColumnsFunc = new (string Name, Expression<Func<TypeInfo, TypeMemberInfo, object>> Expression, Action<IXLCell, TypeMemberInfo> Action, Func<TypeInfo, bool> Enable)[]
                {
                    ("", (t, d) => d.Name, null, (__) => true),
                    ("", (t, d) => d.Value, null, (__) => true),
                    ("", (t, d) => d.Comment, null, (__) => true),
                    ("", (t, d) => d.IsEnabled, null, (__) => !this.settings.OmitAttribute),
                    ("", (t, d) => d.Tags, (cell, d) =>
                    {
                        if (d.Tags.Color != null)
                            cell.WorksheetRow().Style.Fill.BackgroundColor = XLColor.FromHtml(d.Tags.Color);
                    }, (__) => !this.settings.OmitAttribute),
                    ("Modifier", (t, d) => d.ModificationInfo.ID, null, (__) => !this.settings.OmitSignatureDate),
                    ("ModifiedDateTime", (t, d) => d.ModificationInfo.DateTime, null, (__) => !this.settings.OmitSignatureDate),
                    ("Creator", (t, d) => d.CreationInfo.ID, null, (__) => !this.settings.OmitSignatureDate),
                    ("CreatedDateTime", (t, d) => d.CreationInfo.DateTime, null, (__) => !this.settings.OmitSignatureDate),
                    ("", (t, d) => d.ID, null, (__) => !this.settings.OmitAttribute),
                },
                SetWorksheetStyleAction = (worksheet, tableInfo) =>
                {
                    if (!string.IsNullOrWhiteSpace(tableInfo.Tags.Color))
                    {
                        worksheet.SetTabColor(XLColor.FromHtml(tableInfo.Tags.Color));
                    }
                }
            };
            
            var writer = new ExcelWriter<TypeInfo, TypeMemberInfo>(typeInfos, writerSettings);
            var filename = GetFileName(itemPath, OutputPath);
            writer.Write(filename);

            this.configs.Commit(this.settings);
            this.configs.Commit(this);
        }

        private string GetFileName(string itemPath, string filename)
        {
            if (this.settings.IsSeparable == true)
            {
                var outputPath = itemPath.Trim(PathUtility.SeparatorChar).Replace(PathUtility.SeparatorChar, '.');
                if (outputPath == string.Empty)
                    outputPath = this.cremaAppHost.DataBaseName;
                if (this.settings.IsIncludeDate)
                {
                    var szDate = DateTime.Now.ToString($"{this.settings.OutputDateFormat}");
                    return FileUtility.Prepare(this.outputPath, $"{outputPath}_{szDate}.xlsx");
                }
         
                return FileUtility.Prepare(this.outputPath, outputPath + ".xlsx");
            }

            return filename;
        }

        public void SelectPath()
        {
            if (this.settings.IsSeparable == true)
            {
                var dialog = new CommonOpenFileDialog()
                {
                    IsFolderPicker = true,
                };

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    this.OutputPath = dialog.FileName;
                }
            }
            else
            {
                var dialog = new CommonSaveFileDialog();
                dialog.Filters.Add(new CommonFileDialogFilter("excel file", "*.xlsx"));
                dialog.DefaultExtension = "xlsx";

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    this.OutputPath = dialog.FileName;
                }
            }
        }

        [ConfigurationProperty("outputPath")]
        public string OutputPath
        {
            get => this.outputPath;
            set
            {
                this.outputPath = value;
                this.NotifyOfPropertyChange(nameof(this.OutputPath));
                this.NotifyOfPropertyChange(nameof(this.CanExport));
            }
        }

        public bool CanExport
        {
            get
            {
                if (string.IsNullOrEmpty(this.OutputPath) == true)
                    return false;
                if (this.settings.IsSeparable == true)
                    return Directory.Exists(this.OutputPath);
                if (DirectoryUtility.IsDirectory(this.OutputPath) == true)
                    return false;
                return true;
            }
        }

        [ConfigurationProperty("omitSignatureDate")]
        public object Settings => this.settings;

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.configs.Update(this);
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.NotifyOfPropertyChange(nameof(CanExport));
        }
    }
}
