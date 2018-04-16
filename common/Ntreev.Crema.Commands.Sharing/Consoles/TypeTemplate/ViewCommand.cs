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

namespace Ntreev.Crema.Commands.Consoles.TypeTemplate
{
    [Export(typeof(ITemplateCommand))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class ViewCommand : TemplateCommandBase
    {
        public ViewCommand()
            : base("view")
        {

        }

        public ITypeTemplate Template { get => this.CommandContext.Template; }

        protected override void OnExecute()
        {
            var domain = this.Template.Dispatcher.Invoke(() => this.Template.Domain);
            var dataType = domain.Source as CremaDataType;

            this.Template.Dispatcher.Invoke(() => this.Draw(dataType));
        }

        private void Draw(CremaDataType dataType)
        {
            var members = GetMembers();
            var tableDataBuilder = new TableDataBuilder(members);
            var count = 0;

            foreach (var item in dataType.Members)
            {
                var fieldList = new List<string>
                {
                    item.Name,
                    $"{item.Value}",
                    item.Comment
                };
                tableDataBuilder.Add(fieldList.ToArray());
                count++;
            }

            this.Out.WriteLine();
            this.Out.PrintTableData(tableDataBuilder.Data, true);
            this.Out.WriteLine();

            string[] GetMembers()
            {
                var query = from item in this.GetMemberProperties(false)
                                //where item == nameof(CremaTemplateColumn.Name) || StringUtility.GlobMany(item, PreviewProperties.Columns)
                            select item;
                return query.ToArray();
            }
        }

        private IEnumerable<string> GetMemberProperties(bool detail)
        {
            yield return nameof(CremaDataTypeMember.Name);
            yield return nameof(CremaDataTypeMember.Value);
            yield return nameof(CremaTemplateColumn.Comment);
        }
    }
}