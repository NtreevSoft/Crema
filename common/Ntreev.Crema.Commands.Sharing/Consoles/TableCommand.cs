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
            table.Dispatcher.Invoke(() => table.Copy(authentication, newTableName, categoryPath, this.CopyContent));
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(CategoryPath), nameof(CopyContent))]
        public void Inherit([CommandCompletion(nameof(GetTableNames))]string tableName, string newTableName)
        {
            var table = this.GetTable(tableName);
            var authentication = this.CommandContext.GetAuthentication(this);
            var categoryPath = this.CategoryPath ?? this.GetCurrentDirectory();
            table.Dispatcher.Invoke(() => table.Inherit(authentication, newTableName, categoryPath, this.CopyContent));
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(FormatProperties))]
        public void View([CommandCompletion(nameof(GetPaths))]string tableItemName, string revision = null)
        {
            var tableItem = this.GetTableItem(tableItemName);
            var authentication = this.CommandContext.GetAuthentication(this);
            var dataSet = tableItem.Dispatcher.Invoke(() => tableItem.GetDataSet(authentication, revision));
            var props = dataSet.ToDictionary(true, false);
            this.CommandContext.WriteObject(props, FormatProperties.Format);
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(FormatProperties))]
        public void Log([CommandCompletion(nameof(GetPaths))]string tableItemName, string revision = null)
        {
            var tableItem = this.GetTableItem(tableItemName);
            var authentication = this.CommandContext.GetAuthentication(this);
            var logs = tableItem.Dispatcher.Invoke(() => tableItem.GetLog(authentication, revision));

            foreach (var item in logs)
            {
                this.CommandContext.WriteObject(item.ToDictionary(), FormatProperties.Format);
                this.CommandContext.WriteLine();
            }
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(FilterProperties))]
        [CommandMethodStaticProperty(typeof(TagsProperties))]
        public void List()
        {
            var tableNames = this.GetTableNames((TagInfo)TagsProperties.Tags, FilterProperties.FilterExpression);
            this.CommandContext.WriteList(tableNames);
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(FormatProperties))]
        public void Info([CommandCompletion(nameof(GetTableNames))]string tableName)
        {
            var table = this.GetTable(tableName);
            var tableInfo = table.Dispatcher.Invoke(() => table.TableInfo);
            var props = tableInfo.ToDictionary(true);
            this.CommandContext.WriteObject(props, FormatProperties.Format);
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(FilterProperties))]
        [CommandMethodStaticProperty(typeof(FormatProperties))]
        public void ColumnList([CommandCompletion(nameof(GetTableNames))]string tableName)
        {
            var table = this.GetTable(tableName);
            var tableInfo = table.Dispatcher.Invoke(() => table.TableInfo);
            var columnList = tableInfo.Columns.Where(item => StringUtility.GlobMany(item.Name, FilterProperties.FilterExpression))
                                              .Select(item => new ColumnItem(item)).ToArray();
            this.CommandContext.WriteList(columnList);
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(FormatProperties))]
        [CommandMethodStaticProperty(typeof(FilterProperties))]
        public void ColumnInfo([CommandCompletion(nameof(GetTableNames))]string tableName)
        {
            var table = this.GetTable(tableName);
            var tableInfo = table.Dispatcher.Invoke(() => table.TableInfo);
            var columns = new Dictionary<string, object>(tableInfo.Columns.Length);
            foreach (var item in tableInfo.Columns)
            {
                if (StringUtility.GlobMany(item.Name, FilterProperties.FilterExpression))
                {
                    columns.Add(item.Name, item.ToDictionary());
                }
            }
            this.CommandContext.WriteObject(columns, FormatProperties.Format);
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
            return GetTableNames(TagInfo.All, null);
        }

        private string[] GetTableNames(TagInfo tags, string filterExpress)
        {
            return this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                var query = from item in dataBase.TableContext.Tables
                            where StringUtility.GlobMany(item.Name, filterExpress)
                            where (item.TableInfo.DerivedTags & tags) == tags
                            orderby item.Name
                            select item.Name;

                return query.ToArray();
            });
        }

        private string[] GetCategoryPaths()
        {
            return this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                var query = from item in dataBase.TableContext.Categories
                            orderby item.Path
                            select item.Path;
                return query.ToArray();
            });
        }

        private string[] GetPaths()
        {
            return this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                var query = from item in dataBase.TableContext.Categories
                            orderby item.Path
                            select item;

                var itemList = new List<string>(dataBase.TableContext.Count());
                foreach (var item in query)
                {
                    itemList.Add(item.Path);
                    itemList.AddRange(from table in item.Tables orderby table.Name select table.Name);
                }
                return itemList.ToArray();
            });
        }

        private string GetCurrentDirectory()
        {
            if (this.CommandContext.Drive is DataBasesConsoleDrive root)
            {
                var dataBasePath = new DataBasePath(this.CommandContext.Path);
                if (dataBasePath.ItemPath != string.Empty)
                    return dataBasePath.ItemPath;
            }
            return PathUtility.Separator;
        }

        private DataBasesConsoleDrive Drive => this.CommandContext.Drive as DataBasesConsoleDrive;

        private ICremaHost CremaHost => this.cremaHost.Value;

        #region classes

        class ColumnItem : TerminalTextItem
        {
            private readonly ColumnInfo columnInfo;

            public ColumnItem(ColumnInfo columnInfo)
                : base(columnInfo)
            {
                this.columnInfo = columnInfo;
            }

            public override string ToString()
            {
                return this.columnInfo.Name;
            }

            protected override void OnDraw(TextWriter writer, string text)
            {
                if (this.columnInfo.IsKey == true)
                {
                    using (TerminalColor.SetForeground(ConsoleColor.Cyan))
                    {
                        base.OnDraw(writer, text);
                    }
                }
                else
                {
                    base.OnDraw(writer, text);
                }
            }
        }

        #endregion
    }
}
