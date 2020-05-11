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

using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using ClosedXML.Excel;
using Ntreev.Library;

namespace Ntreev.Crema.Spreadsheet.Excel
{
    public class ExcelWriter<TMaster, TDetail> : IDisposable
    {
        private readonly ExcelWriterSettings<TMaster, TDetail> settings;
        private (Func<TMaster, TDetail, object> Func, Action<IXLCell, TDetail> Action, Func<TMaster, bool> Enable)[] CompiledColumns { get; }
        public TMaster[] Master { get; }

        public ExcelWriter(TMaster[] master, ExcelWriterSettings<TMaster, TDetail> settings)
        {
            this.Master = master;
            this.settings = settings;
            this.CompiledColumns = this.settings
                .GetColumnsFunc
                .Select(o => (o.Expression.Compile(), o.Action, o.Enable)).ToArray();
        }

        public void Write(string filename, IProgress progress = null)
        {
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                this.Write(stream, progress);
            }
        }

        public void Write(Stream stream, IProgress progress = null)
        {
            var workbook = new XLWorkbook();
            this.WriteSheets(workbook, progress);
            workbook.SaveAs(stream);
        }

        private void WriteSheets(XLWorkbook workbook, IProgress progress)
        {
            if (progress == null)
            {
                progress = new Progress();
            }

            var step = new StepProgress(progress);

            try
            {
                step.Begin(this.Master.Length);

                foreach (var master in this.Master)
                {
                    var worksheet = workbook.AddWorksheet(SpreadsheetUtility.Ellipsis(this.settings.GetWorksheetNameFunc(master)));
                    this.settings.SetWorksheetStyleAction?.Invoke(worksheet, master);
                    this.WriteColumns(worksheet, master);
                    this.WriteRows(worksheet, master);
                    this.AdjustColumns(worksheet);

                    worksheet.SheetView.Freeze(1, 1);
                    step.Next("write {0} : {1}", ConsoleProgress.GetProgressString(step.Step + 1, this.Master.Length), master.ToString());
                }
            }
            finally
            {
                step.Complete();
            }
        }

        private void WriteColumns(IXLWorksheet worksheet, TMaster master)
        {
            var row = 1;
            var column = 1;
            foreach (var tuple in this.settings.GetColumnsFunc)
            {
                if (!tuple.Enable(master)) continue;

                var columnValue = tuple.Item1;
                if (string.IsNullOrWhiteSpace(columnValue))
                {
                    if (tuple.Item2.Body is MemberExpression member)
                    {
                        columnValue = member.Member.Name;
                    } 
                    else if (tuple.Item2.Body is UnaryExpression unary && unary.Operand is MemberExpression expression)
                    {
                        columnValue = expression.Member.Name;
                    }
                }

                var cell = worksheet.Cell(row, column++);
                cell.Value = columnValue;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
        }

        private void WriteRows(IXLWorksheet worksheet, TMaster master)
        {
            var row = 2;
            var details = this.settings.GetDetailFunc(master);
            foreach (var detail in details)
            {
                var column = 1;
                foreach (var c in this.CompiledColumns)
                {
                    if (!c.Enable(master)) continue;
                    
                    var columnValue = c.Func(master, detail);
                    var cell = WriteField(worksheet, row, column, columnValue);
                    c.Action?.Invoke(cell, detail);
                    column++;
                }

                row++;
            }
        }

        private void AdjustColumns(IXLWorksheet sheet)
        {
            for (var i = 0; i < this.settings.GetColumnsFunc.Length; i++)
            {
                sheet.Column(i + 1).AdjustToContents();
            }
        }

        private IXLCell WriteField(IXLWorksheet workSheet, int row, int column, object field)
        {
            if (field != null && field != DBNull.Value)
            {
                var cell = workSheet.Cell(row, column);

                if (field is byte)
                {
                    cell.SetValue((byte) field);
                }
                else if (field is sbyte)
                {
                    cell.SetValue((sbyte) field);
                }
                else if (field is short)
                {
                    cell.SetValue((short) field);
                }
                else if (field is ushort)
                {
                    cell.SetValue((ushort) field);
                }
                else if (field is int)
                {
                    cell.SetValue((int) field);
                }
                else if (field is uint)
                {
                    cell.SetValue((uint) field);
                }
                else if (field is float)
                {
                    cell.SetValue((float) field);
                }
                else if (field is double)
                {
                    cell.SetValue((double) field);
                }
                else if (field is char)
                {
                    cell.SetValue((int) field);
                }
                else if (field is bool)
                {
                    cell.SetValue((bool) field);
                }
                else if (field is TimeSpan)
                {
                    cell.SetValue((TimeSpan) field);
                }
                else if (field is DateTime)
                {
                    cell.SetValue((DateTime) field);
                }
                else
                {
                    cell.SetValue(Convert.ToString(field));
                }

                return cell;
            }

            return null;
        }

        public void Dispose()
        {
        }
    }
}
