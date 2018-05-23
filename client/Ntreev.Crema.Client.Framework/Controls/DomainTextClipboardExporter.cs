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

using Ntreev.Library;
using Ntreev.Crema.ServiceModel;
using Ntreev.ModernUI.Framework.DataGrid.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.DataGrid;
using Ntreev.Crema.Client.Framework.Properties;

namespace Ntreev.Crema.Client.Framework.Controls
{
    public class DomainTextClipboardExporter : ModernTextClipboardExporter
    {
        private readonly StringBuilder sb = new StringBuilder();
        private bool isFistColumn;
        private DataGridContext gridContext;

        protected override object ClipboardData
        {
            get { return this.sb.ToString(); }
        }

        protected override void ResetExporter()
        {
            this.sb.Clear();
            this.gridContext = null;
        }

        protected override void StartExporter(string dataFormat)
        {
            this.gridContext = null;
            base.StartExporter(dataFormat);
        }

        protected override void StartDataItem(DataGridContext dataGridContext, object dataItem)
        {
            if (this.gridContext != null && this.gridContext != dataGridContext)
                throw new InvalidOperationException(Resources.Exception_CannotSelectComplexRanges);
            this.isFistColumn = true;
            this.gridContext = dataGridContext;
        }

        protected override void StartDataItemField(DataGridContext dataGridContext, Column column, object fieldValue)
        {
            if (this.isFistColumn == true)
            {

            }
            else
            {
                this.sb.Append("\t");
            }

            if (fieldValue != null)
            {
                var converter = System.ComponentModel.TypeDescriptor.GetConverter(fieldValue);
                if (fieldValue is string)
                {
                    var text = fieldValue.ToString();

                    text = text.Replace("\"", "\"\"").Replace(Environment.NewLine, "\n");

                    if (text.IndexOf('\n') >= 0)
                    {
                        text = text.WrapQuot();
                    }

                    this.sb.Append(text);
                }
                else
                {
                    this.sb.Append(converter.ConvertToString(fieldValue));
                }
            }

            this.isFistColumn = false;
        }

        protected override void StartHeader(DataGridContext dataGridContext)
        {
            this.isFistColumn = true;
        }

        protected override void StartHeaderField(DataGridContext dataGridContext, Column column)
        {
            if (this.isFistColumn == true)
            {

            }
            else
            {
                this.sb.Append("\t");
            }

            if (column.Title == null)
                this.sb.Append(string.Format("\"{0}\"", column.FieldName));
            else
                this.sb.Append(string.Format("\"{0}\"", column.Title));
            this.isFistColumn = false;
        }


        protected override void EndDataItem(DataGridContext dataGridContext, object dataItem)
        {
            this.sb.AppendLine();
        }

        protected override void EndDataItemField(DataGridContext dataGridContext, Column column, object fieldValue)
        {

        }

        protected override void EndHeader(DataGridContext dataGridContext)
        {
            this.sb.AppendLine();
        }
    }
}
