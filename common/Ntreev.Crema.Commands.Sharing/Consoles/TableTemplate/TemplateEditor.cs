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

using Ntreev.Crema.Commands.Consoles.Serializations;
using Ntreev.Crema.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.Commands.Consoles.TableTemplate
{
    static class TemplateEditor
    {
        public static bool EditColumns(ITableTemplate template, Authentication authentication)
        {
            var columnCount = template.Dispatcher.Invoke(() => template.Count);
            var dataTypes = template.Dispatcher.Invoke(() => template.SelectableTypes);
            var columnList = new List<JsonColumnInfos.ItemInfo>(columnCount);
            var idToColumn = new Dictionary<Guid, ITableColumn>(columnCount);

            template.Dispatcher.Invoke(() =>
            {
                foreach (var item in template)
                {
                    var column = new JsonColumnInfos.ItemInfo()
                    {
                        ID = Guid.NewGuid(),
                        Name = item.Name,
                        IsKey = item.IsKey,
                        DataType = item.DataType,
                        Comment = item.Comment,
                        IsUnique = item.IsUnique,
                        AutoIncrement = item.AutoIncrement,
                        DefaultValue = item.DefaultValue,
                        Tags = (string)item.Tags,
                        IsReadOnly = item.IsReadOnly,
                        DisallowNull = !item.AllowNull,
                    };
                    idToColumn.Add(column.ID, item);
                    columnList.Add(column);
                }
            });

            var schema = JsonSchemaUtility.CreateSchema(typeof(JsonColumnInfos));
            var itemsSchema = schema.Properties[nameof(JsonColumnInfos.Items)];
            var itemSchema = itemsSchema.Items.First();
            var dataTypeSchema = itemSchema.Properties[nameof(JsonColumnInfos.ItemInfo.DataType)];
            dataTypeSchema.SetEnums(dataTypes);
            var tagSchema = itemSchema.Properties[nameof(JsonColumnInfos.ItemInfo.Tags)];
            tagSchema.SetEnums(TagInfoUtility.Names);

            var columns = new JsonColumnInfos() { Items = columnList.ToArray() };

            using (var editor = new JsonEditorHost(columns, schema))
            {
                if (editor.Execute() == false)
                    return false;

                columns = editor.Read<JsonColumnInfos>();
            }

            template.Dispatcher.Invoke(() =>
            {
                foreach (var item in idToColumn.Keys.ToArray())
                {
                    if (columns.Items.Any(i => i.ID == item) == false)
                    {
                        var column = idToColumn[item];
                        column.Delete(authentication);
                        idToColumn.Remove(item);
                    }
                }

                for (var i = 0; i < columns.Items.Length; i++)
                {
                    var item = columns.Items[i];
                    if (item.ID == Guid.Empty)
                    {
                        var column = template.AddNew(authentication);
                        item = InitializeFields(authentication, item, column);
                        template.EndNew(authentication, column);
                        item.ID = Guid.NewGuid();
                        idToColumn.Add(item.ID, column);
                        columns.Items[i] = item;
                    }
                    else if (idToColumn.ContainsKey(item.ID) == true)
                    {
                        var column = idToColumn[item.ID];
                        SetFields(authentication, item, column);
                    }
                    else
                    {
                        throw new InvalidOperationException($"{item.ID} is not existed column.");
                    }
                }

                for (var i = 0; i < columns.Items.Length; i++)
                {
                    var item = columns.Items[i];
                    var column = idToColumn[item.ID];
                    column.SetIndex(authentication, i);
                }
            });

            return true;
        }

        private static JsonColumnInfos.ItemInfo InitializeFields(Authentication authentication, JsonColumnInfos.ItemInfo item, ITableColumn column)
        {
            column.SetName(authentication, item.Name);
            column.SetDataType(authentication, item.DataType);
            column.SetComment(authentication, item.Comment);
            column.SetTags(authentication, (TagInfo)item.Tags);
            column.SetIsReadOnly(authentication, item.IsReadOnly);
            column.SetIsUnique(authentication, item.IsUnique);
            column.SetAutoIncrement(authentication, item.AutoIncrement);
            column.SetDefaultValue(authentication, item.DefaultValue);
            column.SetAllowNull(authentication, !item.DisallowNull);
            return item;
        }

        private static void SetFields(Authentication authentication, JsonColumnInfos.ItemInfo item, ITableColumn column)
        {
            if (column.Name != item.Name)
                column.SetName(authentication, item.Name);
            if (column.DataType != item.DataType)
                column.SetDataType(authentication, item.DataType);
            if (column.Comment != item.Comment)
                column.SetComment(authentication, item.Comment);
            if (column.Tags != (TagInfo)item.Tags)
                column.SetTags(authentication, (TagInfo)item.Tags);
            if (column.IsReadOnly != item.IsReadOnly)
                column.SetIsReadOnly(authentication, item.IsReadOnly);
            if (column.IsUnique != item.IsUnique)
                column.SetIsUnique(authentication, item.IsUnique);
            if (column.AutoIncrement != item.AutoIncrement)
                column.SetAutoIncrement(authentication, item.AutoIncrement);
            if (column.DefaultValue != item.DefaultValue)
                column.SetDefaultValue(authentication, item.DefaultValue);
            if (column.AllowNull != !item.DisallowNull)
                column.SetAllowNull(authentication, !item.DisallowNull);
        }
    }
}
