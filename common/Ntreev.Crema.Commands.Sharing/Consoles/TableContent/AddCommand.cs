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

#pragma warning disable 0618
using Ntreev.Crema.Commands;
using Ntreev.Crema.Commands.Consoles;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Services;
using Ntreev.Crema.Commands.Consoles.Serializations;
using Ntreev.Crema.Commands.Consoles.Properties;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Newtonsoft.Json.Schema;
using Ntreev.Crema.Data;
using Newtonsoft.Json.Schema.Generation;

namespace Ntreev.Crema.Commands.Consoles.TableContent
{
    [Export(typeof(IConsoleCommand))]
    [Category(nameof(ITableContent))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ResourceDescription("../Resources", IsShared = true, Prefix = "TableContent")]
    class AddCommand : ContentCommandBase
    {
        public AddCommand()
            : base("add")
        {

        }

        protected override void OnExecute()
        {
            var tableInfo = this.Content.Dispatcher.Invoke(() => this.Content.Table.TableInfo);
            var schema = new JSchema();

            foreach (var item in tableInfo.Columns)
            {
                var itemSchema = this.CreateSchema(item.DataType);
                itemSchema.Description = item.Comment;
                schema.Properties.Add(item.Name, itemSchema);
            }

            var required = tableInfo.Columns.Where(item => item.AllowNull == false).Select(item => item.Name);
            foreach (var item in required)
            {
                schema.Required.Add(item);
            }

            var fields = new JsonPropertiesInfo();
            var authentication = this.CommandContext.GetAuthentication(this);
            var tableRow = this.Content.Dispatcher.Invoke(() => this.Content.AddNew(authentication, null));

            this.Content.Dispatcher.Invoke(() =>
            {
                foreach (var item in tableInfo.Columns)
                {
                    var field = tableRow[item.Name];
                    if (field != null || item.AllowNull == false)
                        fields.Add(item.Name, field);
                }
            });

            var result = GetResult();
            if (result == null)
                return;

            this.Content.Dispatcher.Invoke(() =>
            {
                foreach (var item in result)
                {
                    if (tableInfo.Columns.Any(i => i.Name == item.Key) == false)
                        continue;
                    if (item.Value == null)
                        continue;
                    tableRow.SetField(authentication, item.Key, item.Value);
                }

                this.Content.EndNew(authentication, tableRow);
            });

            JsonPropertiesInfo GetResult()
            {
                using (var editor = new JsonEditorHost(fields, schema))
                {
                    if (editor.Execute() == false)
                        return null;

                    return editor.Read<JsonPropertiesInfo>();
                }
            }
        }
    }
}