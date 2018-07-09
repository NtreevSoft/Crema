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

using Ntreev.Crema.Data;
using Ntreev.Library;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Ntreev.Crema.Commands.Consoles.Properties
{
    [ResourceDescription("../Resources", IsShared = true)]
    static class ViewProperties
    {
        [CommandProperty('r')]
        [DefaultValue(-1)]
        public static int Limit
        {
            get; set;
        }

        [CommandProperty("exp")]
        [DefaultValue("")]
        public static string Expression
        {
            get; set;
        }

        [CommandProperty("columns")]
        [DefaultValue("")]
        public static string Columns
        {
            get; set;
        }

        [CommandProperty('c')]
        [DefaultValue(-1)]
        public static int ColumnLimit
        {
            get; set;
        }

        [CommandProperty('s')]
        [DefaultValue("")]
        public static string Sort
        {
            get; set;
        }

        public static void View(CremaDataTable dataTable, TextWriter writer)
        {
            var columns = GetColumns();
            var tableDataBuilder = new TableDataBuilder(columns);
            var count = 0;
            var rows = ViewProperties.Sort == string.Empty ? dataTable.Select(ViewProperties.Expression) : dataTable.Select(ViewProperties.Expression, ViewProperties.Sort);
            foreach (var item in rows)
            {
                var items = columns.Select(i => $"{item[i]}").ToArray();
                if (ViewProperties.Limit >= 0 && ViewProperties.Limit <= count)
                    break;
                tableDataBuilder.Add(items);
                count++;
            }

            writer.PrintTableData(tableDataBuilder.Data, true);
            writer.WriteLine();

            string[] GetColumns()
            {
                var query = from item in dataTable.Columns
                            where item.IsKey || StringUtility.GlobMany(item.ColumnName, ViewProperties.Columns)
                            select item.ColumnName;

                if (ColumnLimit <= 0)
                    return query.ToArray();
                return query.Take(ColumnLimit).ToArray();
            }
        }
    }
}
