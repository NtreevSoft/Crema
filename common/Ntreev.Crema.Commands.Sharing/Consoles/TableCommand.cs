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

using Ntreev.Crema.Commands.Consoles.Properties;
using Ntreev.Crema.Commands.Consoles.Serializations;
using Ntreev.Crema.Commands.Consoles.TableContent;
using Ntreev.Crema.Commands.Consoles.TableTemplate;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
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

namespace Ntreev.Crema.Commands.Consoles
{
    [Export(typeof(IConsoleCommand))]
    [ResourceDescription("Resources", IsShared = true)]
    class TableCommand : ConsoleCommandMethodBase
    {
        private const string tableNameString = "tableName";
        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        [ImportingConstructor]
        public TableCommand()
            : base("table")
        {

        }

        public override string[] GetCompletions(CommandMethodDescriptor methodDescriptor, CommandMemberDescriptor memberDescriptor, string find)
        {
            if (methodDescriptor.DescriptorName == nameof(View))
            {
                if (memberDescriptor.DescriptorName == tableNameString)
                {
                    return this.GetTableNames();
                }
            }
            else if (methodDescriptor.DescriptorName == nameof(Info))
            {
                if (memberDescriptor.DescriptorName == tableNameString)
                {
                    return this.GetTableNames();
                }
            }
            else if (methodDescriptor.DescriptorName == nameof(ColumnInfo))
            {
                if (memberDescriptor.DescriptorName == tableNameString)
                {
                    return this.GetTableNames();
                }
            }
            else if (methodDescriptor.DescriptorName == nameof(ColumnList))
            {
                if (memberDescriptor.DescriptorName == tableNameString)
                {
                    return this.GetTableNames();
                }
            }
            else if (methodDescriptor.DescriptorName == nameof(Log))
            {
                if (memberDescriptor.DescriptorName == tableNameString)
                {
                    return this.GetTableNames();
                }
            }
            else if (methodDescriptor.DescriptorName == nameof(EditTemplate))
            {
                if (memberDescriptor.DescriptorName == tableNameString)
                {
                    return this.GetTableNames();
                }
            }
            else if (methodDescriptor.DescriptorName == nameof(Edit))
            {
                if (memberDescriptor.DescriptorName == tableNameString)
                {
                    return this.GetTableNames();
                }
            }
            else if (methodDescriptor.DescriptorName == nameof(Create))
            {
                if (memberDescriptor.DescriptorName == nameof(CategoryPath))
                {
                    return this.GetCategoryPaths();
                }
            }
            else if (methodDescriptor.DescriptorName == nameof(Copy))
            {
                if (memberDescriptor.DescriptorName == "tableName")
                {
                    return this.GetTableNames();
                }
            }
            else if (methodDescriptor.DescriptorName == nameof(Inherit))
            {
                if (memberDescriptor.DescriptorName == "tableName")
                {
                    return this.GetTableNames();
                }
            }

            return base.GetCompletions(methodDescriptor, memberDescriptor, find);
        }

        [CommandMethod]
        public void Rename(string tableName, string newTableName)
        {
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            table.Dispatcher.Invoke(() => table.Rename(authentication, newTableName));
        }

        [CommandMethod]
        public void Move(string tableName, string categoryPath)
        {
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            table.Dispatcher.Invoke(() => table.Move(authentication, categoryPath));
        }

        [CommandMethod]
        public void Delete(string tableName, string deleteNow)
        {
            if (deleteNow != "DeleteNow")
                throw new ArgumentException("tpye: 'DeleteNow'", nameof(deleteNow));
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            table.Dispatcher.Invoke(() => table.Delete(authentication));
        }

        [CommandMethod]
        public void SetTags(string tableName, string tags)
        {
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            table.Dispatcher.Invoke(() => table.SetTags(authentication, (TagInfo)tags));
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(CategoryPath), nameof(CopyContent))]
        public void Copy(string tableName, string newTableName)
        {
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            table.Dispatcher.Invoke(() =>
            {
                var categoryPath = this.CategoryPath ?? this.GetCurrentDirectory();
                table.Copy(authentication, newTableName, categoryPath, this.CopyContent);
            });
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(CategoryPath), nameof(CopyContent))]
        public void Inherit(string tableName, string newTableName)
        {
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            table.Dispatcher.Invoke(() =>
            {
                var categoryPath = this.CategoryPath ?? this.GetCurrentDirectory();
                table.Inherit(authentication, newTableName, categoryPath, this.CopyContent);
            });
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(ViewProperties))]
        public void View(string tableName, string revision = null)
        {
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            var dataTable = table.Dispatcher.Invoke(() =>
            {
                var dataSet = table.GetDataSet(authentication, revision);
                return dataSet.Tables[table.Name, table.Category.Path];
            });

            ViewProperties.View(dataTable, this.Out);
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(ViewProperties))]
        public void ViewCategory(string categoryPath, string revision = null)
        {
            var category = this.GetCategory(categoryPath);
            var authentication = this.CommandContext.GetAuthentication(this);
            var dataSet = category.Dispatcher.Invoke(() => category.GetDataSet(authentication, revision));

            foreach (var item in dataSet.Tables)
            {
                ViewProperties.View(item, this.Out);
            }
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(LogProperties))]
        public void Log(string tableName)
        {
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            var logs = table.Dispatcher.Invoke(() => table.GetLog(authentication));
            LogProperties.Print(this.Out, logs);
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(LogProperties))]
        public void LogCategory(string categoryPath)
        {
            var category = this.GetCategory(categoryPath);
            var authentication = this.CommandContext.GetAuthentication(this);
            var logs = category.Dispatcher.Invoke(() => category.GetLog(authentication));
            LogProperties.Print(this.Out, logs);
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(FilterProperties))]
        [CommandMethodStaticProperty(typeof(TagsProperties))]
        public void List()
        {
            var tableNames = this.CremaHost.Dispatcher.Invoke(GetTableNames);
            this.Out.Print(tableNames);

            string[] GetTableNames()
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                var tags = (TagInfo)TagsProperties.Tags;
                var query = from item in dataBase.TableContext.Tables
                            where StringUtility.GlobMany(item.Name, FilterProperties.FilterExpression)
                            where (item.TableInfo.DerivedTags & tags) == tags
                            select item.Name;

                return query.ToArray();
            }
        }

        [CommandMethod]
        public void Info(string tableName)
        {
            var table = this.GetTable(tableName);
            var tableInfo = table.Dispatcher.Invoke(() => table.TableInfo);
            var props = tableInfo.ToDictionary(true);
            var text = TextSerializer.Serialize(props);
            this.Out.WriteLine(text);
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(ColumnInfoProperties))]
        public void ColumnList(string tableName)
        {
            var table = this.GetTable(tableName);
            var tableInfo = table.Dispatcher.Invoke(() => table.TableInfo);
            var headerList = new List<string>(new string[] { "IsKey", CremaSchema.Name, "DataType", CremaSchema.Comment, });

            if (ColumnInfoProperties.ID == true || ColumnInfoProperties.All)
                headerList.Add(nameof(ColumnInfoProperties.ID));
            if (ColumnInfoProperties.AllowNull == true || ColumnInfoProperties.All)
                headerList.Add(nameof(ColumnInfoProperties.AllowNull));
            if (ColumnInfoProperties.ReadOnly == true || ColumnInfoProperties.All)
                headerList.Add(nameof(ColumnInfoProperties.ReadOnly));
            if (ColumnInfoProperties.Unique == true || ColumnInfoProperties.All)
                headerList.Add(nameof(ColumnInfoProperties.Unique));
            if (ColumnInfoProperties.AutoIncrement == true || ColumnInfoProperties.All)
                headerList.Add(nameof(ColumnInfoProperties.AutoIncrement));
            if (ColumnInfoProperties.Tags == true || ColumnInfoProperties.All)
                headerList.Add(nameof(ColumnInfoProperties.Tags));
            if (ColumnInfoProperties.DefaultValue == true || ColumnInfoProperties.All)
                headerList.Add(nameof(ColumnInfoProperties.DefaultValue));
            if (ColumnInfoProperties.SignatureDate == true || ColumnInfoProperties.All)
            {
                headerList.Add(nameof(Data.ColumnInfo.CreationInfo));
                headerList.Add(nameof(Data.ColumnInfo.ModificationInfo));
            }

            var tableDataBuilder = new TableDataBuilder(headerList.ToArray());

            foreach (var item in tableInfo.Columns)
            {
                var objectList = new List<object>(new object[] { item.IsKey ? "O" : string.Empty, item.Name, item.DataType, item.Comment });

                if (ColumnInfoProperties.ID == true || ColumnInfoProperties.All)
                    objectList.Add(item.ID);
                if (ColumnInfoProperties.AllowNull == true || ColumnInfoProperties.All)
                    objectList.Add(item.AllowNull);
                if (ColumnInfoProperties.ReadOnly == true || ColumnInfoProperties.All)
                    objectList.Add(item.ReadOnly);
                if (ColumnInfoProperties.Unique == true || ColumnInfoProperties.All)
                    objectList.Add(item.IsUnique);
                if (ColumnInfoProperties.AutoIncrement == true || ColumnInfoProperties.All)
                    objectList.Add(item.AutoIncrement);
                if (ColumnInfoProperties.Tags == true || ColumnInfoProperties.All)
                    objectList.Add(item.DerivedTags);
                if (ColumnInfoProperties.DefaultValue == true || ColumnInfoProperties.All)
                    objectList.Add(item.DefaultValue);
                if (ColumnInfoProperties.SignatureDate == true || ColumnInfoProperties.All)
                {
                    objectList.Add(item.CreationInfo);
                    objectList.Add(item.ModificationInfo);
                }

                tableDataBuilder.Add(objectList.ToArray());
            }

            this.Out.PrintTableData(tableDataBuilder.Data, true);
            this.Out.WriteLine();
        }

        [CommandMethod]
        public void ColumnInfo(string tableName, string columnName)
        {
            var table = this.GetTable(tableName);
            var tableInfo = table.Dispatcher.Invoke(() => table.TableInfo);
            var columnInfo = tableInfo.Columns.First(item => item.Name == columnName);

            var items = new Dictionary<string, object>
            {
                { $"{nameof(columnInfo.ID)}", columnInfo.ID },
                { $"{nameof(columnInfo.IsKey)}", columnInfo.IsKey },
                { $"{nameof(columnInfo.IsUnique)}", columnInfo.IsUnique },
                { $"{nameof(columnInfo.AllowNull)}", columnInfo.AllowNull },
                { $"{nameof(columnInfo.Name)}", columnInfo.Name },
                { $"{nameof(columnInfo.DataType)}", columnInfo.DataType },
                { $"{nameof(columnInfo.DefaultValue)}", columnInfo.DefaultValue },
                { $"{nameof(columnInfo.Comment)}", columnInfo.Comment },
                { $"{nameof(columnInfo.AutoIncrement)}", columnInfo.AutoIncrement},
                { $"{nameof(columnInfo.ReadOnly)}", columnInfo.ReadOnly},
                { $"{nameof(columnInfo.Tags)}", columnInfo.DerivedTags},
                { $"{nameof(columnInfo.CreationInfo)}", columnInfo.CreationInfo },
                { $"{nameof(columnInfo.ModificationInfo)}", columnInfo.ModificationInfo },
            };
            this.Out.Print<object>(items);
            this.Out.WriteLine();
        }

        [ConsoleModeOnly]
        [CommandMethod]
        [CommandMethodProperty(nameof(CategoryPath))]
        public void Create()
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            var category = this.GetCategory(this.CategoryPath ?? this.GetCurrentDirectory());
            var tableNames = this.GetTableNames();
            var template = category.Dispatcher.Invoke(() => category.NewTable(authentication));

            var dataTypes = template.Dispatcher.Invoke(() => template.SelectableTypes);
            var tableName = NameUtility.GenerateNewName("Table", tableNames);
            var tableInfo = JsonTableInfo.Default;
            if (tableInfo.TableName == string.Empty)
                tableInfo.TableName = tableName;

            var schema = JsonSchemaUtility.GetSchema(typeof(JsonTableInfo));
            var itemsSchema = schema.Properties[nameof(JsonTableInfo.Columns)];
            var itemSchema = itemsSchema.Items.First();
            itemSchema.SetEnums(nameof(JsonTableInfo.JsonTableColumnInfo.DataType), dataTypes);
            itemSchema.SetEnums(nameof(JsonTableInfo.JsonTableColumnInfo.Tags), TagInfoUtility.Names);

            try
            {
                if (JsonEditorHost.TryEdit(ref tableInfo, schema) == false)
                    return;
                if (this.CommandContext.ReadYesOrNo($"do you want to create table '{tableInfo.TableName}'?") == false)
                    return;

                template.Dispatcher.Invoke(SetData);
                template = null;
            }
            finally
            {
                if (template != null)
                {
                    template.Dispatcher.Invoke(() => template.CancelEdit(authentication));
                }
            }

            void SetData()
            {
                template.SetTableName(authentication, tableInfo.TableName);
                template.SetTags(authentication, (TagInfo)tableInfo.Tags);
                template.SetComment(authentication, tableInfo.Comment);
                foreach (var item in tableInfo.Columns)
                {
                    var column = template.AddNew(authentication);
                    column.SetName(authentication, item.Name);
                    column.SetIsKey(authentication, item.IsKey);
                    column.SetComment(authentication, item.Comment);
                    column.SetDataType(authentication, item.DataType);
                    column.SetIsUnique(authentication, item.IsUnique);
                    column.SetAutoIncrement(authentication, item.AutoIncrement);
                    column.SetDefaultValue(authentication, item.DefaultValue);
                    column.SetTags(authentication, (TagInfo)item.Tags);
                    column.SetIsReadOnly(authentication, item.IsReadOnly);
                    template.EndNew(authentication, column);
                }
                template.EndEdit(authentication);
            }
        }

        [ConsoleModeOnly]
        [CommandMethod]
        public void EditTemplate(string tableName)
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            var table = this.GetTable(tableName);
            var template = table.Dispatcher.Invoke(() => table.Template);
            var domain = template.Dispatcher.Invoke(() => template.Domain);
            var contains = domain == null ? false : domain.Dispatcher.Invoke(() => domain.Users.Contains(authentication.ID));

            template.Dispatcher.Invoke(() =>
            {
                if (contains == false)
                    template.BeginEdit(authentication);
            });

            if (TemplateEditor.EditColumns(template, authentication) == false)
            {
                template.Dispatcher.Invoke(() => template.CancelEdit(authentication));
            }
            else
            {
                template.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        template.EndEdit(authentication);
                    }
                    catch
                    {
                        template.CancelEdit(authentication);
                        throw;
                    }
                });
            }
        }

        [ConsoleModeOnly]
        [CommandMethod]
        public void Edit(string tableName)
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            var table = this.GetTable(tableName);
            var content = table.Dispatcher.Invoke(() => table.Content);
            var domain = content.Dispatcher.Invoke(() =>
            {
                if (content.Domain == null)
                    content.BeginEdit(authentication);
                return content.Domain;
            });
            var contains = domain.Dispatcher.Invoke(() => domain.Users.Contains(authentication.ID));
            content.Dispatcher.Invoke(() =>
            {
                if (contains == false)
                    content.EnterEdit(authentication);
            });
            domain.Dispatcher.Invoke(()=> domain.UserRemoved += Domain_UserRemoved);

            this.CommandContext.Category = nameof(ITableContent);
            this.CommandContext.Target = content;
        }
        
        [CommandProperty("force")]
        public bool IsForce
        {
            get; set;
        }

        [CommandProperty]
        public string CategoryPath
        {
            get; set;
        }

        [CommandProperty]
        public bool CopyContent
        {
            get; set;
        }

        [CommandProperty("quiet", 'q')]
        public bool IsQuiet
        {
            get; set;
        }

        [CommandProperty("complex")]
        public bool IsComplexMode
        {
            get; set;
        }

        public ICremaHost CremaHost
        {
            get { return this.cremaHost.Value; }
        }

        public override bool IsEnabled => this.CommandContext.Drive is DataBasesConsoleDrive drive && drive.DataBaseName != string.Empty;

        protected override bool IsMethodEnabled(CommandMethodDescriptor descriptor)
        {
            return base.IsMethodEnabled(descriptor);
        }

        private void Domain_UserRemoved(object sender, DomainUserRemovedEventArgs e)
        {
            if (sender is IDomain domain && e.RemoveInfo.Reason == RemoveReason.Kick && e.DomainUserInfo.UserID == this.CommandContext.UserID)
            {
                domain.UserRemoved -= Domain_UserRemoved;
                this.CommandContext.Category = null;
                this.CommandContext.Target = null;
            }
        }

        private ITable GetTable(string tableName)
        {
            var table = this.CremaHost.Dispatcher.Invoke(GetTable);
            if (table == null)
                throw new TableNotFoundException(tableName);
            return table;

            ITable GetTable()
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                if (NameValidator.VerifyItemPath(tableName) == true)
                    return dataBase.TableContext[tableName] as ITable;
                return dataBase.TableContext.Tables[tableName];
            }
        }

        private ITableCategory GetCategory(string categoryPath)
        {
            var category = this.CremaHost.Dispatcher.Invoke(GetCategory);
            if (category == null)
                throw new CategoryNotFoundException(categoryPath);
            return category;

            ITableCategory GetCategory()
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                return dataBase.TableContext.Categories[categoryPath];
            }
        }

        private string[] GetTableNames()
        {
            return this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                var query = from item in dataBase.TableContext.Tables
                            let name = item.Name
                            select name;
                return query.ToArray();
            });
        }

        private string[] GetCategoryPaths()
        {
            return this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                var query = from item in dataBase.TableContext.Categories
                            let path = item.Path
                            select path;
                return query.ToArray();
            });
        }

        private string GetCurrentDirectory()
        {
            if (this.CommandContext.Drive is DataBasesConsoleDrive root)
            {
                var dataBasePath = new DataBasePath(this.CommandContext.Path);
                return dataBasePath.ItemPath;
            }
            return PathUtility.Separator;
        }

        private DataBasesConsoleDrive Drive => this.CommandContext.Drive as DataBasesConsoleDrive;
    }
}
