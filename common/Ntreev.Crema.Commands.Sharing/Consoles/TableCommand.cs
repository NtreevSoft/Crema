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
        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        [ImportingConstructor]
        public TableCommand()
        {

        }

        public override string[] GetCompletions(CommandMethodDescriptor methodDescriptor, CommandMemberDescriptor memberDescriptor, string find)
        {
            return base.GetCompletions(methodDescriptor, memberDescriptor, find);
        }

        [CommandMethod]
        public void Rename([CommandCompletion(nameof(GetTableNames))]string tableName, string newTableName)
        {
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            table.Dispatcher.Invoke(() => table.Rename(authentication, newTableName));
        }

        [CommandMethod]
        public void Move([CommandCompletion(nameof(GetTableNames))]string tableName, [CommandCompletion(nameof(GetCategoryPaths))]string categoryPath)
        {
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            table.Dispatcher.Invoke(() => table.Move(authentication, categoryPath));
        }

        [CommandMethod]
        public void Delete([CommandCompletion(nameof(GetTableNames))]string tableName)
        {
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            if (this.CommandContext.ConfirmToDelete() == true)
            {
                table.Dispatcher.Invoke(() => table.Delete(authentication));
            }
        }

        [CommandMethod]
        public void SetTags([CommandCompletion(nameof(GetTableNames))]string tableName, string tags)
        {
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            table.Dispatcher.Invoke(() =>
            {
                var template = table.Template;
                template.BeginEdit(authentication);
                try
                {
                    template.SetTags(authentication, (TagInfo)tags);
                    template.EndEdit(authentication);
                }
                catch
                {
                    template.CancelEdit(authentication);
                    throw;
                }
            });
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(CategoryPath), nameof(CopyContent))]
        public void Copy([CommandCompletion(nameof(GetTableNames))]string tableName, string newTableName)
        {
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            var categoryPath = this.CategoryPath ?? this.GetCurrentDirectory();
            table.Dispatcher.Invoke(() =>
            {
                table.Copy(authentication, newTableName, categoryPath, this.CopyContent);
            });
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(CategoryPath), nameof(CopyContent))]
        public void Inherit([CommandCompletion(nameof(GetTableNames))]string tableName, string newTableName)
        {
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            var categoryPath = this.CategoryPath ?? this.GetCurrentDirectory();
            table.Dispatcher.Invoke(() =>
            {
                table.Inherit(authentication, newTableName, categoryPath, this.CopyContent);
            });
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(ViewProperties))]
        public void View([CommandCompletion(nameof(GetPaths))]string tableItemName, string revision = null)
        {
            var tableItem = this.GetTableItem(tableItemName);
            var authentication = this.CommandContext.GetAuthentication(this);
            var dataSet = tableItem.Dispatcher.Invoke(() =>
            {
                return tableItem.GetDataSet(authentication, revision);
            });

            foreach (var item in dataSet.Tables)
            {
                this.Out.WriteLine(item.Name);
                ViewProperties.View(item, this.Out);
            }
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(LogProperties))]
        public void Log([CommandCompletion(nameof(GetPaths))]string tableItemName)
        {
            var tableItem = this.GetTableItem(tableItemName);
            var authentication = this.CommandContext.GetAuthentication(this);
            var logs = tableItem.Dispatcher.Invoke(() => tableItem.GetLog(authentication));
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
                            orderby item.Name
                            select item.Name;

                return query.ToArray();
            }
        }

        [CommandMethod]
        public void Info([CommandCompletion(nameof(GetTableNames))]string tableName)
        {
            var table = this.GetTable(tableName);
            var tableInfo = table.Dispatcher.Invoke(() => table.TableInfo);
            var props = tableInfo.ToDictionary(true);
            var text = TextSerializer.Serialize(props);
            this.Out.WriteLine(text);
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(ColumnInfoProperties))]
        public void ColumnList([CommandCompletion(nameof(GetTableNames))]string tableName)
        {
            var table = this.GetTable(tableName);
            var tableInfo = table.Dispatcher.Invoke(() => table.TableInfo);
            var headerList = new List<string>(new string[] { CremaSchema.IsKey, CremaSchema.Name, CremaSchema.DataType, CremaSchema.Comment, });

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
        public void ColumnInfo([CommandCompletion(nameof(GetTableNames))]string tableName, string columnName)
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
        [CommandMethodProperty(nameof(ParentPath))]
        public void Create()
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            var tableNames = this.GetTableNames();
            var template = CreateTemplate();

            var dataTypes = template.Dispatcher.Invoke(() => template.SelectableTypes);
            var tableName = template.Dispatcher.Invoke(() => template.TableName);
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

            ITableTemplate CreateTemplate()
            {
                if (this.ParentPath == string.Empty)
                {
                    var category = this.GetCategory(this.GetCurrentDirectory());
                    return category.Dispatcher.Invoke(() => category.NewTable(authentication));
                }
                else if (NameValidator.VerifyCategoryPath(this.ParentPath) == true)
                {
                    var category = this.GetCategory(this.ParentPath);
                    return category.Dispatcher.Invoke(() => category.NewTable(authentication));
                }
                else if (this.GetTable(this.ParentPath) is ITable table)
                {
                    return table.Dispatcher.Invoke(() => table.NewTable(authentication));
                }
                else
                {
                    throw new NotImplementedException();
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
        public void EditTemplate([CommandCompletion(nameof(GetTableNames))]string tableName)
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
        public void Edit([CommandCompletion(nameof(GetTableNames))]string tableName)
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
            domain.Dispatcher.Invoke(() => domain.UserRemoved += Domain_UserRemoved);

            this.CommandContext.Category = nameof(ITableContent);
            this.CommandContext.Target = content;
        }

        [CommandProperty("force")]
        public bool IsForce
        {
            get; set;
        }

        [CommandProperty]
        [CommandCompletion(nameof(GetCategoryPaths))]
        public string CategoryPath
        {
            get; set;
        }

        [CommandProperty("parent")]
        [CommandCompletion(nameof(GetPaths))]
        [DefaultValue("")]
        public string ParentPath
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

        private ITable GetTable([CommandCompletion(nameof(GetTableNames))]string tableName)
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

        private ITableItem GetTableItem([CommandCompletion(nameof(GetPaths))]string tableItemName)
        {
            var tableItem = this.CremaHost.Dispatcher.Invoke(GetTableItem);
            if (tableItem == null)
                throw new TableNotFoundException(tableItemName);
            return tableItem;

            ITableItem GetTableItem()
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                if (NameValidator.VerifyItemPath(tableItemName) == true || NameValidator.VerifyCategoryPath(tableItemName) == true)
                    return dataBase.TableContext[tableItemName];
                return dataBase.TableContext.Tables[tableItemName] as ITableItem;
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

        private string[] GetPaths()
        {
            return this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                var itemList = new List<string>(dataBase.TableContext.Count());
                foreach (var item in dataBase.TableContext)
                {
                    if (item is ITable table)
                    {
                        itemList.Add(table.Name);
                    }
                    else if (item is ITableCategory category)
                    {
                        itemList.Add(category.Path);
                    }
                }
                return itemList.ToArray();
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
