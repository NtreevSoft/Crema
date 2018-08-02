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

using Ntreev.Crema.Commands.Consoles;
using Ntreev.Crema.Commands.Consoles.Properties;
using Ntreev.Crema.Commands.Consoles.Serializations;
using Ntreev.Crema.Commands.Consoles.TableContent;
using Ntreev.Crema.Commands.Consoles.TableTemplate;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Spreadsheet;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Ntreev.Crema.Commands.Spreadsheet
{
    [Export(typeof(IConsoleCommand))]
    [ResourceDescription("Resources", IsShared = true)]
    class SpreadSheetCommand : ConsoleCommandMethodBase
    {
        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        private string dataBaseName;

        [ImportingConstructor]
        public SpreadSheetCommand()
            : base(GetName())
        {

        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(FilterProperties))]
        [CommandMethodStaticProperty(typeof(DataSetTypeProperties))]
        [CommandMethodProperty(nameof(DataBase), nameof(OmitAttribute), nameof(OmitSignatureDate), nameof(Revision))]
        public void Export(string filename)
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            var dataBase = this.CremaHost.Dispatcher.Invoke(() => this.CremaHost.DataBases[this.DataBase]);
            var revision = dataBase.Dispatcher.Invoke(() => this.Revision ?? dataBase.DataBaseInfo.Revision);
            var dataSet = dataBase.Dispatcher.Invoke(() => dataBase.GetDataSet(authentication, DataSetTypeProperties.DataSetType, FilterProperties.FilterExpression, revision));
            var settings = new SpreadsheetWriterSettings()
            {
                OmitAttribute = this.OmitAttribute,
                OmitSignatureDate = this.OmitSignatureDate,
                OmitType = DataSetTypeProperties.TableOnly,
                OmitTable = DataSetTypeProperties.TypeOnly
            };
            settings.Properties.Add(nameof(Revision), revision);
            settings.Properties.Add(nameof(FilterProperties.FilterExpression), FilterProperties.FilterExpression);
            settings.Properties.Add(nameof(DataBase), this.DataBase);
            this.WriteDataSet(dataSet, filename, settings);
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(Message))]
        [CommandMethodStaticProperty(typeof(FilterProperties))]
        public void Import(string filename)
        {
            var path = Path.Combine(this.CommandContext.BaseDirectory, filename);
            var sheetNames = SpreadsheetReader.ReadTableNames(path);
            var authentication = this.CommandContext.GetAuthentication(this);
            var dataBase = this.CremaHost.Dispatcher.Invoke(() => this.CremaHost.DataBases[this.DataBase]);
            var tableNames = dataBase.Dispatcher.Invoke(() => dataBase.TableContext.Tables.Select(item => item.Name).ToArray());
            var query = from sheet in sheetNames
                        join table in tableNames on sheet equals SpreadsheetUtility.Ellipsis(table)
                        where StringUtility.GlobMany(table, FilterProperties.FilterExpression)
                        orderby table
                        select sheet;
            var filterExpression = string.Join(";", query);
            var revision = dataBase.Dispatcher.Invoke(() => dataBase.DataBaseInfo.Revision);
            var dataSet = dataBase.Dispatcher.Invoke(() => dataBase.GetDataSet(authentication, DataSetType.OmitContent, filterExpression, revision));
            this.ReadDataSet(dataSet, path);
            dataBase.Dispatcher.Invoke(() => dataBase.Import(authentication, dataSet, this.Message));
        }

        public override bool IsEnabled => this.CommandContext.IsOnline;

        [CommandProperty]
        public bool OmitAttribute
        {
            get;
            set;
        }

        [CommandProperty]
        public bool OmitSignatureDate
        {
            get;
            set;
        }

        [CommandProperty('m', true, IsRequired = true, IsExplicit = true)]
        public string Message
        {
            get; set;
        }

        [CommandProperty('r', true)]
        public string Revision
        {
            get; set;
        }

        [CommandProperty("database")]
        public string DataBase
        {
            get
            {
                if (this.dataBaseName != null)
                    return this.dataBaseName;
                if (this.CommandContext.Drive is DataBasesConsoleDrive drive)
                    return drive.DataBaseName;
                return null;
            }
            set
            {
                this.dataBaseName = value;
            }
        }

        private static string GetName()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                try
                {
                    var startInfo = new ProcessStartInfo()
                    {
                        RedirectStandardOutput = true,
                        FileName = "sw_vers",
                        Arguments = "-productName",
                        UseShellExecute = false,
                    };
                    var process = System.Diagnostics.Process.Start(startInfo);
                    var text = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return "numbers";
                }
                catch
                {
                    return "calc";
                }
            }
            else
            {
                return "excel";
            }
        }

        private void ImportTables(Authentication authentication, CremaDataSet dataSet, IDataBase dataBase)
        {
            this.CommandContext.WriteLine($"importing data");
            dataBase.Dispatcher.Invoke(() =>
            {
                dataBase.Import(authentication, dataSet, this.Message);
            });
            this.CommandContext.WriteLine($"importing data has been completed.");
        }

        //private void ReadTypes(Authentication authentication, CremaDataSet dataSet, IDataBase dataBase, string typeNames)
        //{
        //    var types = dataBase.Dispatcher.Invoke(() => this.GetFilterTypes(dataBase.TypeContext.Types, typeNames));
        //    if (types.Any() == false)
        //        throw new CremaException("조건에 맞는 타입이 존재하지 않습니다.");
        //    var step = new StepProgress(new ConsoleProgress(this.Out) { Style = ConsoleProgressStyle.None });
        //    step.Begin(types.Length);
        //    foreach (var item in types)
        //    {
        //        dataBase.Dispatcher.Invoke(() =>
        //        {
        //            var previewSet = item.GetDataSet(authentication, null);
        //            var previewType = previewSet.Types[item.Name];
        //            CreateTable(previewType);
        //            step.Next("reading {0}/{1} : {2}", step.Step + 1, types.Length, item.Name);
        //        });
        //    }

        //    step.Complete();

        //    void CreateTable(CremaDataType dataType)
        //    {
        //        var dataTable = dataSet.Tables.Add(dataType.Name);

        //        dataTable.Columns.Add(CremaSchema.Name);
        //        dataTable.Columns.Add(CremaSchema.Value, typeof(long));
        //        dataTable.Columns.Add(CremaSchema.Comment);

        //        dataTable.BeginLoad();
        //        foreach (var item in dataType.Members)
        //        {
        //            var row = dataTable.NewRow();

        //            row[CremaSchema.Name] = item.Name;
        //            row[CremaSchema.Value] = item.Value;
        //            row[CremaSchema.Comment] = item.Comment;

        //            row.IsEnabled = item.IsEnabled;
        //            row.Tags = item.Tags;
        //            row.SetAttribute(CremaSchema.Creator, item.CreationInfo.ID);
        //            row.SetAttribute(CremaSchema.CreatedDateTime, item.CreationInfo.DateTime);
        //            row.SetAttribute(CremaSchema.Modifier, item.ModificationInfo.ID);
        //            row.SetAttribute(CremaSchema.ModifiedDateTime, item.ModificationInfo.DateTime);

        //            dataTable.Rows.Add(row);
        //        }
        //        dataTable.EndLoad();
        //    }
        //}

        private void ReadDataSet(CremaDataSet dataSet, string filename)
        {
            using (var reader = new SpreadsheetReader(filename))
            {
                reader.Read(dataSet);
            }
        }

        //private void WriteTypes(CremaDataSet dataSet, string filename)
        //{
        //    var path = Path.Combine(this.CommandContext.BaseDirectory, filename);
        //    var settings = new SpreadsheetWriterSettings()
        //    {
        //        OmitAttribute = this.OmitAttribute,
        //        OmitSignatureDate = this.OmitSignatureDate,
        //        Tags = (TagInfo)TagsProperties.Tags,
        //    };
        //    var progress = new ConsoleProgress(this.Out) { Style = ConsoleProgressStyle.None };
        //    using (var writer = new SpreadsheetWriter(dataSet, settings))
        //    {
        //        writer.Write(path, progress);
        //    }
        //    this.Out.WriteLine($"export: \"{path}\"");
        //}

        private void WriteDataSet(CremaDataSet dataSet, string filename, SpreadsheetWriterSettings settings)
        {
            var path = Path.Combine(this.CommandContext.BaseDirectory, filename);
            using (var writer = new SpreadsheetWriter(dataSet, settings))
            {
                writer.Progress += Writer_Progress;
                writer.Write(path);
            }
            this.CommandContext.WriteLine($"export: \"{path}\"");
        }

        private void Writer_Progress(object sender, ProgressEventArgs e)
        {
            if (e.Target is CremaDataType dataType)
            {
                this.CommandContext.WriteLine($"write type {ConsoleProgress.GetProgressString(e.Index + 1, e.Count)} : {dataType.Name}");
            }
            else if (e.Target is CremaDataTable dataTable)
            {
                this.CommandContext.WriteLine($"write table {ConsoleProgress.GetProgressString(e.Index + 1, e.Count)} : {dataTable.Name}");
            }
        }

        private ICremaHost CremaHost => this.cremaHost.Value;
    }
}