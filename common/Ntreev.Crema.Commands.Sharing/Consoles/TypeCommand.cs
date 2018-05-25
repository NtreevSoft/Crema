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
using Ntreev.Crema.Commands.Consoles.TypeTemplate;
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
    class TypeCommand : ConsoleCommandMethodBase
    {
        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        [ImportingConstructor]
        public TypeCommand()
            : base("type")
        {

        }

        public override string[] GetCompletions(CommandMethodDescriptor methodDescriptor, CommandMemberDescriptor memberDescriptor, string find)
        {
            if (methodDescriptor.DescriptorName == nameof(View))
            {
                if (memberDescriptor.DescriptorName == "typeName")
                {
                    return this.GetTypeNames();
                }
            }
            else if (methodDescriptor.DescriptorName == nameof(Info))
            {
                if (memberDescriptor.DescriptorName == "typeName")
                {
                    return this.GetTypeNames();
                }
            }
            else if (methodDescriptor.DescriptorName == nameof(Log))
            {
                if (memberDescriptor.DescriptorName == "typeName")
                {
                    return this.GetTypeNames();
                }
            }
            else if (methodDescriptor.DescriptorName == nameof(Edit))
            {
                if (memberDescriptor.DescriptorName == "typeName")
                {
                    return this.GetTypeNames();
                }
            }

            return base.GetCompletions(methodDescriptor, memberDescriptor, find);
        }

        [ConsoleModeOnly]
        [CommandMethod]
        [CommandMethodProperty(nameof(CategoryPath))]
        public void Create()
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            var category = this.GetCategory(this.CategoryPath ?? this.GetCurrentDirectory());
            var typeNames = this.GetTypeNames();
            var template = category.Dispatcher.Invoke(() => category.NewType(authentication));
            var typeName = NameUtility.GenerateNewName("Type", typeNames);
            var typeInfo = JsonTypeInfo.Default;
            typeInfo.TypeName = typeName;

            try
            {
                if (JsonEditorHost.TryEdit(ref typeInfo) == false)
                    return;
                if (this.CommandContext.ReadYesOrNo($"do you want to create type '{typeInfo.TypeName}'?") == false)
                    return;

                template.Dispatcher.Invoke(() =>
                {
                    template.SetTypeName(authentication, typeInfo.TypeName);
                    template.SetIsFlag(authentication, typeInfo.IsFlag);
                    template.SetComment(authentication, typeInfo.Comment);
                    foreach (var item in typeInfo.Members)
                    {
                        var member = template.AddNew(authentication);
                        member.SetName(authentication, item.Name);
                        member.SetValue(authentication, item.Value);
                        member.SetComment(authentication, item.Comment);
                        template.EndNew(authentication, member);
                    }
                    template.EndEdit(authentication);
                    template = null;
                });
            }
            finally
            {
                if (template != null)
                {
                    template.Dispatcher.Invoke(() => template.CancelEdit(authentication));
                }
            }
        }

        [ConsoleModeOnly]
        [CommandMethod]
        public void Edit(string typeName)
        {
            var type = this.GetType(typeName);
            var template = type.Dispatcher.Invoke(() => type.Template);
            var domain = template.Dispatcher.Invoke(() => template.Domain);
            var authentication = this.CommandContext.GetAuthentication(this);
            var contains = domain == null ? false : domain.Dispatcher.Invoke(() => domain.Users.Contains(authentication.ID));

            template.Dispatcher.Invoke(() =>
            {
                if (contains == false)
                    template.BeginEdit(authentication);
            });

            try
            {
                if (TemplateEditor.Edit(template, authentication) == true)
                {
                    template = null;
                }
            }
            finally
            {
                if (template != null)
                {
                    template.Dispatcher.Invoke(() => template.CancelEdit(authentication));
                }
            }
        }

        //[CommandMethod]
        //public void Rename(string typeName, string newTypeName)
        //{
        //    this.CremaHost.Dispatcher.Invoke(() =>
        //    {
        //        var type = this.GetType(typeName);
        //        type.Rename(authentication, newTypeName);
        //    });
        //}

        //[CommandMethod]
        //public void Move(string typeName, string categoryPath)
        //{
        //    this.CremaHost.Dispatcher.Invoke(() =>
        //    {
        //        var type = this.GetType(typeName);
        //        type.Move(authentication, categoryPath);
        //    });
        //}

        //[CommandMethod]
        //public void Delete(string typeName)
        //{
        //    this.CremaHost.Dispatcher.Invoke(() =>
        //    {
        //        var type = this.GetType(typeName);
        //        type.Delete(authentication);
        //    });
        //}

        //[CommandMethod]
        public void SetTags(string typeName, string tags)
        {
            var type = this.GetType(typeName);
            var authentication = this.CommandContext.GetAuthentication(this);
            type.Dispatcher.Invoke(() => type.SetTags(authentication, (TagInfo)tags));
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(CategoryPath))]
        public void Copy(string typeName, string newTypeName)
        {
            var type = this.GetType(typeName);
            type.Dispatcher.Invoke(() =>
            {
                var categoryPath = this.CategoryPath ?? this.GetCurrentDirectory();
                var authentication = this.CommandContext.GetAuthentication(this);
                type.Copy(authentication, newTypeName, categoryPath);
            });
        }

        [CommandMethod]
        public void View(string typeName, string revision = null)
        {
            var type = this.GetType(typeName);
            var tableData = type.Dispatcher.Invoke(() =>
            {
                var authentication = this.CommandContext.GetAuthentication(this);
                var dataSet = type.GetDataSet(authentication, revision);
                var dataType = dataSet.Types[type.Name, type.Category.Path];
                var tableDataBuilder = new TableDataBuilder(CremaSchema.Name, CremaSchema.Value, CremaSchema.Comment);
                foreach (var item in dataType.Members)
                {
                    tableDataBuilder.Add(item.Name, item.Value, item.Comment);
                }
                return tableDataBuilder;
            });

            this.Out.Print(tableData);
        }

        [CommandMethod]
        public void ViewCategory(string categoryPath, string revision = null)
        {
            var category = this.GetCategory(categoryPath);
            var builderList = new List<TableDataBuilder>();
            var authentication = this.CommandContext.GetAuthentication(this);
            category.Dispatcher.Invoke(() =>
            {
                var dataSet = category.GetDataSet(authentication, revision);
                foreach (var item in dataSet.Types)
                {
                    builderList.Add(BuildTableData(item));
                }
            });

            foreach (var item in builderList)
            {
                this.Out.Print(item);
            }

            TableDataBuilder BuildTableData(CremaDataType dataType)
            {
                var builder = new TableDataBuilder(CremaSchema.Name, CremaSchema.Value, CremaSchema.Comment);
                foreach (var item in dataType.Members)
                {
                    builder.Add(item.Name, item.Value, item.Comment);
                }
                return builder;
            }
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(LogProperties))]
        public void Log(string typeName)
        {
            var type = this.GetType(typeName);
            var authentication = this.CommandContext.GetAuthentication(this);
            var logs = type.Dispatcher.Invoke(() => type.GetLog(authentication));
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
        public void List()
        {
            var typeNames = GetTypeNames();
            this.Out.Print(typeNames);

            string[] GetTypeNames()
            {
                var dataBaseName = this.Drive.DataBaseName;
                return this.CremaHost.Dispatcher.Invoke(() =>
                {
                    var dataBase = this.CremaHost.DataBases[dataBaseName];
                    var query = from item in dataBase.TypeContext.Types
                                where StringUtility.GlobMany(item.Name, FilterProperties.FilterExpression)
                                select item.Name;
                    return query.ToArray();
                });
            }
        }

        [CommandMethod]
        public void Info(string typeName)
        {
            var type = this.GetType(typeName);
            var typeInfo = type.Dispatcher.Invoke(() => type.TypeInfo);
            var items = new Dictionary<string, object>
            {
                { $"{nameof(typeInfo.ID)}", typeInfo.ID },
                { $"{nameof(typeInfo.Name)}", typeInfo.Name },
                { $"{nameof(typeInfo.CategoryPath)}", typeInfo.CategoryPath },
                { $"{nameof(typeInfo.Tags)}", typeInfo.Tags},
                { $"{nameof(typeInfo.Comment)}", typeInfo.Comment },
                { $"{nameof(typeInfo.CreationInfo)}", typeInfo.CreationInfo.ToLocalValue() },
                { $"{nameof(typeInfo.ModificationInfo)}", typeInfo.ModificationInfo.ToLocalValue() },
            };
            this.Out.Print(items);
        }

        [CommandProperty]
        public string CategoryPath
        {
            get; set;
        }

        public ICremaHost CremaHost
        {
            get { return this.cremaHost.Value; }
        }

        public override bool IsEnabled => this.CommandContext.Drive is DataBasesConsoleDrive drive && drive.DataBaseName != string.Empty;

        private IType GetType(string typeName)
        {
            var type = this.CremaHost.Dispatcher.Invoke(GetType);
            if (type == null)
                throw new TypeNotFoundException(typeName);
            return type;

            IType GetType()
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                if (NameValidator.VerifyItemPath(typeName) == true)
                    return dataBase.TypeContext[typeName] as IType;
                return dataBase.TypeContext.Types[typeName];
            }
        }

        private ITypeCategory GetCategory(string categoryPath)
        {
            var category = this.CremaHost.Dispatcher.Invoke(GetCategory);
            if (category == null)
                throw new CategoryNotFoundException(categoryPath);
            return category;

            ITypeCategory GetCategory()
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                return dataBase.TypeContext.Categories[categoryPath];
            }
        }

        private string[] GetTypeNames()
        {
            return this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                return dataBase.TypeContext.Types.Select(item => item.Name).ToArray();
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
