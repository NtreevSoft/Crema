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
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Commands.Consoles.TableTemplate
{
    [Export(typeof(ITemplateCommand))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class ViewCommand : TemplateCommandBase
    {
        public ViewCommand()
            : base("view")
        {

        }

        public ITableTemplate Template { get => this.CommandContext.Template; }

        protected override void OnExecute()
        {
            var domain = this.Template.Dispatcher.Invoke(() => this.Template.Domain);
            var template = domain.Source as CremaTemplate;

            this.Template.Dispatcher.Invoke(() => this.Draw(template));
        }

        private void Draw(CremaTemplate template)
        {
            var columns = GetColumns();
            var tableDataBuilder = new TableDataBuilder(columns);
            var count = 0;

            foreach (var item in template.Columns)
            {
                var fieldList = new List<string>
                {
                    item.Name,
                    item.IsKey ? "O" : string.Empty,
                    item.DataTypeName,
                    item.Comment
                };
                tableDataBuilder.Add(fieldList.ToArray());
                count++;
            }

            this.Out.WriteLine();
            this.Out.PrintTableData(tableDataBuilder.Data, true);
            this.Out.WriteLine();

            string[] GetColumns()
            {
                var query = from item in this.GetColumnProperties(false)
                            //where item == nameof(CremaTemplateColumn.Name) || StringUtility.GlobMany(item, PreviewProperties.Columns)
                            select item;
                return query.ToArray();
            }
        }

        private IEnumerable<string> GetColumnProperties(bool detail)
        {
            //yield return nameof(CremaTemplateColumn.Index);
            yield return nameof(CremaTemplateColumn.Name);
            yield return nameof(CremaTemplateColumn.IsKey);
            yield return nameof(CremaTemplateColumn.DataTypeName);
            yield return nameof(CremaTemplateColumn.Comment);
            if (detail == true)
            {
                yield return nameof(CremaTemplateColumn.Unique);
                yield return nameof(CremaTemplateColumn.AutoIncrement);
                yield return nameof(CremaTemplateColumn.DefaultValue);
                yield return nameof(CremaTemplateColumn.Tags);
                yield return nameof(CremaTemplateColumn.ReadOnly);
            }
        }
    }
}