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
        {

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
        public void Edit([CommandCompletion(nameof(GetTypeNames))]string typeName)
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

        [CommandMethod]
        public void Rename([CommandCompletion(nameof(GetTypeNames))]string typeName, string newTypeName)
        {
            var type = this.GetType(typeName);
            var authentication = this.CommandContext.GetAuthentication(this);
            type.Dispatcher.Invoke(() => type.Rename(authentication, newTypeName));
        }

        [CommandMethod]
        public void Move([CommandCompletion(nameof(GetTypeNames))]string typeName, [CommandCompletion(nameof(GetCategoryPaths))]string categoryPath)
        {
            var type = this.GetType(typeName);
            var authentication = this.CommandContext.GetAuthentication(this);
            type.Dispatcher.Invoke(() => type.Move(authentication, categoryPath));
        }

        [CommandMethod]
        public void Delete([CommandCompletion(nameof(GetTypeNames))]string typeName)
        {
            var type = this.GetType(typeName);
            var authentication = this.CommandContext.GetAuthentication(this);
            if (this.CommandContext.ConfirmToDelete() == true)
            {
                type.Dispatcher.Invoke(() => type.Delete(authentication));
            }
        }

        [CommandMethod]
        public void SetTags([CommandCompletion(nameof(GetTypeNames))]string typeName, string tags)
        {
            var type = this.GetType(typeName);
            var authentication = this.CommandContext.GetAuthentication(this);
            type.Dispatcher.Invoke(() =>
            {
                var template = type.Template;
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
        [CommandMethodProperty(nameof(CategoryPath))]
        public void Copy([CommandCompletion(nameof(GetTypeNames))]string typeName, string newTypeName)
        {
            var type = this.GetType(typeName);
            var categoryPath = this.CategoryPath ?? this.GetCurrentDirectory();
            var authentication = this.CommandContext.GetAuthentication(this);
            type.Dispatcher.Invoke(() => type.Copy(authentication, newTypeName, categoryPath));
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(FormatProperties))]
        public void View([CommandCompletion(nameof(GetPaths))]string typeItemName, string revision = null)
        {
            var typeItem = this.GetTypeItem(typeItemName);
            var authentication = this.CommandContext.GetAuthentication(this);
            var dataSet = typeItem.Dispatcher.Invoke(() => typeItem.GetDataSet(authentication, revision));
            var props = dataSet.ToDictionary(false, true);
            this.CommandContext.WriteObject(props, FormatProperties.Format);
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(FormatProperties))]
        public void Log([CommandCompletion(nameof(GetPaths))]string typeItemName)
        {
            var typeItem = this.GetTypeItem(typeItemName);
            var authentication = this.CommandContext.GetAuthentication(this);
            var logs = typeItem.Dispatcher.Invoke(() => typeItem.GetLog(authentication));

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
            var typeNames = this.GetTypeNames((TagInfo)TagsProperties.Tags, FilterProperties.FilterExpression);
            this.CommandContext.WriteList(typeNames);
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(FormatProperties))]
        public void Info([CommandCompletion(nameof(GetTypeNames))]string typeName)
        {
            var type = this.GetType(typeName);
            var typeInfo = type.Dispatcher.Invoke(() => type.TypeInfo);
            this.CommandContext.WriteObject(typeInfo.ToDictionary(), FormatProperties.Format);
        }

        [CommandProperty]
        [CommandCompletion(nameof(GetCategoryPaths))]
        public string CategoryPath
        {
            get; set;
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

        private ITypeItem GetTypeItem([CommandCompletion(nameof(GetPaths))]string typeItemName)
        {
            var typeItem = this.CremaHost.Dispatcher.Invoke(GetTypeItem);
            if (typeItem == null)
                throw new TypeNotFoundException(typeItemName);
            return typeItem;

            ITypeItem GetTypeItem()
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                if (NameValidator.VerifyItemPath(typeItemName) == true || NameValidator.VerifyCategoryPath(typeItemName) == true)
                    return dataBase.TypeContext[typeItemName];
                return dataBase.TypeContext.Types[typeItemName] as ITypeItem;
            }
        }

        private string[] GetTypeNames()
        {
            return GetTypeNames(TagInfo.All, null);
        }

        private string[] GetTypeNames(TagInfo tags, string filterExpress)
        {
            return this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.CremaHost.DataBases[this.Drive.DataBaseName];
                var query = from item in dataBase.TypeContext.Types
                            where StringUtility.GlobMany(item.Name, filterExpress)
                            where (item.TypeInfo.DerivedTags & tags) == tags
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
                var query = from item in dataBase.TypeContext.Categories
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
                var query = from item in dataBase.TypeContext.Categories
                            orderby item.Path
                            select item;

                var itemList = new List<string>(dataBase.TypeContext.Count());
                foreach (var item in query)
                {
                    itemList.Add(item.Path);
                    itemList.AddRange(from type in item.Types orderby type.Name select type.Name);
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
    }
}
