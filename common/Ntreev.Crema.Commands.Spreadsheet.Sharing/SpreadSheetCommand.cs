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

        [ImportingConstructor]
        public SpreadSheetCommand()
            : base(GetName())
        {

        }

        public override string[] GetCompletions(CommandMethodDescriptor methodDescriptor, CommandMemberDescriptor memberDescriptor, string find)
        {
            if (methodDescriptor.DescriptorName == nameof(Export))
            {
                if (memberDescriptor.DescriptorName == "itemNames")
                {
                    return this.GetItemNames(find);
                }
            }
            return base.GetCompletions(methodDescriptor, memberDescriptor, find);
        }

        public override bool IsEnabled
        {
            get
            {
                if (this.CommandContext.IsOnline == false)
                    return false;
                if (this.CommandContext.Drive is DataBasesConsoleDrive root)
                    return root.Context != string.Empty;
                return false;
            }
        }

        [CommandMethod]
        public void Export(string itemNames, string filename)
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            var dataBase = this.CremaHost.Dispatcher.Invoke(() => this.CremaHost.DataBases[this.DataBaseName]);
            var dataSet = new CremaDataSet();

            using (DataBaseUsing.Set(dataBase, authentication, true))
            {
                if (this.Context == CremaSchema.TableDirectory)
                    this.ReadTables(dataSet, dataBase, itemNames);
                else
                    this.ReadTypes(dataSet, dataBase, itemNames);
            }

            if (this.Context == CremaSchema.TableDirectory)
                this.WriteTables(dataSet, filename);
            else
                this.WriteTypes(dataSet, filename);
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(Comment))]
        public void Import(string filename, string itemNames = null)
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            var path = Path.Combine(this.CommandContext.BaseDirectory, filename);
            var dataBase = this.CremaHost.Dispatcher.Invoke(() => this.CremaHost.DataBases[this.DataBaseName]);
            var dataSet = new CremaDataSet();

            using (DataBaseUsing.Set(dataBase, authentication, true))
            {
                if (this.Context == CremaSchema.TableDirectory)
                {
                    this.ReadTables(dataSet, dataBase, path, itemNames);
                    this.ImportTables(dataSet, dataBase);
                }
                else
                {
                    throw new NotImplementedException("타입 가져오기 기능은 구현되지 않았습니다.");
                }
            }

            dataBase.Dispatcher.Invoke(() =>
            {
                dataBase.TableContext.Import(authentication, dataSet, this.Comment);
            });
        }

        [CommandProperty('a')]
        public bool OmitAttribute
        {
            get;
            set;
        }

        [CommandProperty('s')]
        public bool OmitSignatureDate
        {
            get;
            set;
        }

        [CommandProperty('m', IsRequired = true)]
        public string Comment
        {
            get; set;
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

        private void ReadTables(CremaDataSet dataSet, IDataBase dataBase, string filename, string itemNames)
        {
            var sheetNames = SpreadsheetReader.ReadSheetNames(filename);
            var tableInfos = dataBase.Dispatcher.Invoke(() =>
            {
                var query = from table in dataBase.TableContext.Tables
                            let tableName2 = SpreadsheetUtility.Ellipsis(table.Name)
                            join sheetName in sheetNames on tableName2 equals sheetName
                            where table.Name.GlobMany(itemNames) || table.Path.GlobMany(itemNames)
                            orderby table.Name
                            select table.TableInfo;

                return query.ToArray();
            });

            var typeInfos = dataBase.Dispatcher.Invoke(() =>
            {
                var query = from table in dataBase.TableContext.Tables
                            let tableName2 = SpreadsheetUtility.Ellipsis(table.Name)
                            join sheetName in sheetNames on tableName2 equals sheetName
                            where table.Name.GlobMany(itemNames) || table.Path.GlobMany(itemNames)
                            from column in table.TableInfo.Columns
                            where CremaDataTypeUtility.IsBaseType(column.DataType) == false
                            let type = dataBase.TypeContext[column.DataType] as IType
                            where type != null
                            select type.TypeInfo;

                return query.Distinct().ToArray();
            });

            foreach (var item in typeInfos)
            {
                dataSet.Types.Add(item);
            }

            foreach (var item in tableInfos)
            {
                if (item.TemplatedParent != string.Empty)
                    continue;
                if (item.ParentName == string.Empty)
                    dataSet.Tables.Add(item);
                else
                    dataSet.Tables[item.ParentName].Childs.Add(item);
            }

            foreach (var item in tableInfos)
            {
                if (item.TemplatedParent != string.Empty && item.ParentName == string.Empty)
                {
                    var dataTable = dataSet.Tables[item.TemplatedParent];
                    dataTable.Inherit(item.TableName);
                }
            }

            var progress = new ConsoleProgress(this.Out) { Style = ConsoleProgressStyle.None };
            using (var reader = new SpreadsheetReader(filename))
            {
                reader.Read(dataSet, progress);
            }
        }

        private void ImportTables(CremaDataSet dataSet, IDataBase dataBase)
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            this.Out.WriteLine($"importing data");
            dataBase.Dispatcher.Invoke(() =>
            {
                dataBase.TableContext.Import(authentication, dataSet, this.Comment);
            });
            this.Out.WriteLine($"importing data has been completed.");
        }

        private void ReadTypes(CremaDataSet dataSet, IDataBase dataBase, string typeNames)
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            var types = dataBase.Dispatcher.Invoke(() => this.GetFilterTypes(dataBase.TypeContext.Types, typeNames));
            if (types.Any() == false)
                throw new CremaException("조건에 맞는 타입이 존재하지 않습니다.");
            var step = new StepProgress(new ConsoleProgress(this.Out) { Style = ConsoleProgressStyle.None });
            step.Begin(types.Length);
            foreach (var item in types)
            {
                dataBase.Dispatcher.Invoke(() =>
                {
                    var previewSet = item.GetDataSet(authentication, null);
                    var previewType = previewSet.Types[item.Name];
                    CreateTable(previewType);
                    step.Next("reading {0}/{1} : {2}", step.Step + 1, types.Length, item.Name);
                });
            }

            step.Complete();

            void CreateTable(CremaDataType dataType)
            {
                var dataTable = dataSet.Tables.Add(dataType.Name);

                dataTable.Columns.Add(CremaSchema.Name);
                dataTable.Columns.Add(CremaSchema.Value, typeof(long));
                dataTable.Columns.Add(CremaSchema.Comment);

                dataTable.BeginLoad();
                foreach (var item in dataType.Members)
                {
                    var row = dataTable.NewRow();

                    row[CremaSchema.Name] = item.Name;
                    row[CremaSchema.Value] = item.Value;
                    row[CremaSchema.Comment] = item.Comment;

                    row.IsEnabled = item.IsEnabled;
                    row.Tags = item.Tags;
                    row.SetAttribute(CremaSchema.Creator, item.CreationInfo.ID);
                    row.SetAttribute(CremaSchema.CreatedDateTime, item.CreationInfo.DateTime);
                    row.SetAttribute(CremaSchema.Modifier, item.ModificationInfo.ID);
                    row.SetAttribute(CremaSchema.ModifiedDateTime, item.ModificationInfo.DateTime);

                    dataTable.Rows.Add(row);
                }
                dataTable.EndLoad();
            }
        }

        private void ReadTables(CremaDataSet dataSet, IDataBase dataBase, string tableNames)
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            var tables = dataBase.Dispatcher.Invoke(() => this.GetFilterTables(dataBase.TableContext.Tables, tableNames));
            if (tables.Any() == false)
                throw new CremaException("조건에 맞는 테이블이 존재하지 않습니다.");
            var step = new StepProgress(new ConsoleProgress(this.Out) { Style = ConsoleProgressStyle.None });
            step.Begin(tables.Length);
            foreach (var item in tables)
            {
                dataBase.Dispatcher.Invoke(() =>
                {
                    var previewSet = item.GetDataSet(authentication, null);
                    var previewTable = previewSet.Tables[item.Name];
                    previewTable.CopyTo(dataSet);
                    var name = item.Name;
                    step.Next("read {0} : {1}", ConsoleProgress.GetProgressString(step.Step + 1, tables.Length), item.Name);
                });
            }
            step.Complete();
        }

        private void WriteTypes(CremaDataSet dataSet, string filename)
        {
            var path = Path.Combine(this.CommandContext.BaseDirectory, filename);
            var settings = new SpreadsheetWriterSettings()
            {
                OmitAttribute = this.OmitAttribute,
                OmitSignatureDate = this.OmitSignatureDate,
                Tags = (TagInfo)TagsProperties.Tags,
            };
            var progress = new ConsoleProgress(this.Out) { Style = ConsoleProgressStyle.None };
            using (var writer = new SpreadsheetWriter(dataSet, settings))
            {
                writer.Write(path, progress);
            }
            this.Out.WriteLine($"export: \"{path}\"");
        }

        private void WriteTables(CremaDataSet dataSet, string filename)
        {
            var path = Path.Combine(this.CommandContext.BaseDirectory, filename);
            var settings = new SpreadsheetWriterSettings()
            {
                OmitAttribute = this.OmitAttribute,
                OmitSignatureDate = this.OmitSignatureDate,
                Tags = (TagInfo)TagsProperties.Tags,
            };
            var progress = new ConsoleProgress(this.Out) { Style = ConsoleProgressStyle.None };
            using (var writer = new SpreadsheetWriter(dataSet, settings))
            {
                writer.Write(path, progress);
            }
            this.Out.WriteLine($"export: \"{path}\"");
        }

        private IType[] GetFilterTypes(ITypeCollection types, string typeNames)
        {
            var query = from item in types
                        where item.Name.GlobMany(typeNames) || item.Path.GlobMany(typeNames)
                        select item;

            return query.Distinct().ToArray();
        }

        private ITable[] GetFilterTables(ITableCollection tables, string tableNames)
        {
            var query = from item in tables
                        where item.Name.GlobMany(tableNames) || item.Path.GlobMany(tableNames)
                        select item.Parent ?? item;

            return query.Distinct().ToArray();
        }

        private string[] GetTypeNames()
        {
            return this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.CremaHost.DataBases[this.DataBaseName];
                var query = from item in dataBase.TypeContext.Types
                            let name = item.Name
                            select name;
                return query.ToArray();
            });
        }

        private string[] GetTypeCategoryPaths()
        {
            return this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.CremaHost.DataBases[this.DataBaseName];
                var query = from item in dataBase.TypeContext.Categories
                            let path = item.Path
                            select path;
                return query.ToArray();
            });
        }

        private string[] GetTableNames()
        {
            return this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.CremaHost.DataBases[this.DataBaseName];
                var query = from item in dataBase.TableContext.Tables
                            let name = item.Name
                            select name;
                return query.ToArray();
            });
        }

        private string[] GetTableCategoryPaths()
        {
            return this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.CremaHost.DataBases[this.DataBaseName];
                var query = from item in dataBase.TableContext.Categories
                            let path = item.Path
                            select path;
                return query.ToArray();
            });
        }

        private string[] GetItemNames(string find)
        {
            var ss = find.Split(';');
            var itemNames = GetItemNames().OrderBy(item => item).ToArray();
            if (ss.Length == 1)
            {
                var query = from item in itemNames
                            where item.StartsWith(ss.First())
                            select item;
                return query.ToArray();
            }
            else
            {
                var pre = string.Join(";", ss.Take(ss.Length - 1));
                var query = from item in itemNames
                            where item.StartsWith(ss.Last())
                            select $"{pre};{item}";
                return query.ToArray();
            }

            IEnumerable<string> GetItemNames()
            {
                if (this.Context == CremaSchema.TableDirectory)
                {
                    return this.GetTableNames().Concat(this.GetTableCategoryPaths());
                }
                else
                {
                    return this.GetTypeNames().Concat(this.GetTypeCategoryPaths());
                }
            }
        }

        private ICremaHost CremaHost => this.cremaHost.Value;

        private string DataBaseName => (this.CommandContext.Drive as DataBasesConsoleDrive).DataBaseName;

        private string Context => (this.CommandContext.Drive as DataBasesConsoleDrive).Context;
    }
}