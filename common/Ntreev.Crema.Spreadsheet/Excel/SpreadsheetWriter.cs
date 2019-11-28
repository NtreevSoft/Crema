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
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;

namespace Ntreev.Crema.Spreadsheet.Excel
{
    public class SpreadsheetWriter : IDisposable
    {
        private readonly CremaDataSet dataSet;
        private readonly SpreadsheetWriterSettings settings;

        public SpreadsheetWriter(CremaDataSet dataSet)
            : this(dataSet, SpreadsheetWriterSettings.Default)
        {

        }

        public SpreadsheetWriter(CremaDataSet dataSet, SpreadsheetWriterSettings settings)
        {
            this.dataSet = dataSet;
            this.settings = settings;
        }

        public void Write(string filename)
        {
            this.Write(filename, new Progress());
        }

        public void Write(string filename, IProgress progress)
        {
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                this.Write(stream, progress);
            }
        }

        public void Write(Stream stream)
        {
            this.Write(stream, new Progress());
        }

        public void Write(Stream stream, IProgress progress)
        {
            var workbook = new XLWorkbook();
            this.WriteSheets(workbook, progress);
            workbook.SaveAs(stream);
        }

        private void WriteSheets(XLWorkbook workbook, IProgress progress)
        {
            var step = new StepProgress(progress);
            step.Begin(this.dataSet.Tables.Count);
            foreach (var item in this.dataSet.Tables)
            {
                var worksheet = workbook.AddWorksheet(this.settings.NameEllipsis(item.Name));

                if (item.Tags.Color != null)
                    worksheet.SetTabColor(XLColor.FromHtml(item.Tags.Color));

                this.WriteColumns(item, worksheet);
                this.WriteRows(item, worksheet);
                this.AdjustColumns(item, worksheet);

                worksheet.SheetView.Freeze(1, item.PrimaryKey.Length);
                step.Next("write {0} : {1}", ConsoleProgress.GetProgressString(step.Step + 1, this.dataSet.Tables.Count), item.Name);
            }
            step.Complete();
        }

        private void WriteColumns(CremaDataTable dataTable, IXLWorksheet sheet)
        {
            var index = 1;
            var rowIndex = 1;
            if (this.settings.OmitAttribute == false)
            {
                sheet.Cell(rowIndex, index++).Value = CremaSchema.Tags;
                sheet.Cell(rowIndex, index++).Value = CremaSchema.Enable;
            }

            foreach (var item in this.GetSortedColumn(dataTable))
            {
                var column = sheet.Column(index);
                var cell = sheet.Cell(rowIndex, index++);
                cell.Value = item.ColumnName;

                if (item.Comment != string.Empty)
                    cell.Comment.AddText(item.Comment);
                if (item.IsKey == true)
                    cell.Style.Font.Bold = true;
                if (item.Unique == true)
                    cell.Style.Font.Italic = true;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                if (item.DerivedTags.Color != null)
                    column.Style.Fill.BackgroundColor = XLColor.FromHtml(item.DerivedTags.Color);
            }

            if (dataTable.Parent != null)
            {
                var column = sheet.Column(index);
                var cell = sheet.Cell(rowIndex, index++);
                cell.Value = CremaSchema.RelationID;
                cell.Comment.AddText("부모 자식 테이블의 관계값을 나타냅니다. 이 열의 값은 부모 시트의 행 번호를 나타냅니다.");
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                column.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                column.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            }

            if (this.settings.OmitSignatureDate == false)
            {
                sheet.Cell(rowIndex, index++).Value = CremaSchema.Creator;
                sheet.Cell(rowIndex, index++).Value = CremaSchema.CreatedDateTime;
                sheet.Cell(rowIndex, index++).Value = CremaSchema.Modifier;
                sheet.Cell(rowIndex, index++).Value = CremaSchema.ModifiedDateTime;
            }
        }

        private void WriteRows(CremaDataTable dataTable, IXLWorksheet workSheet)
        {
            int r = 2;
            foreach (var row in dataTable.Rows)
            {
                int c = 1;
                if (this.settings.OmitAttribute == false)
                {
                    this.WriteField(workSheet, r, c++, row.Tags.ToString());
                    this.WriteField(workSheet, r, c++, row.IsEnabled);
                }

                foreach (var column in this.GetSortedColumn(dataTable))
                {
                    var value = row[column];
                    this.WriteField(workSheet, r, c, value);
                    c++;
                }

                if (dataTable.Parent != null)
                {
                    var parentIndex = dataTable.Parent.Rows.IndexOf(row.Parent);
                    this.WriteField(workSheet, r, c++, parentIndex + 2);
                }

                if (this.settings.OmitSignatureDate == false)
                {
                    this.WriteField(workSheet, r, c++, row.CreationInfo.ID);
                    this.WriteField(workSheet, r, c++, row.CreationInfo.DateTime);
                    this.WriteField(workSheet, r, c++, row.ModificationInfo.ID);
                    this.WriteField(workSheet, r, c++, row.ModificationInfo.DateTime);
                }

                r++;
            }
        }

        private void WriteField(IXLWorksheet workSheet, int r, int c, object field)
        {
            if (field != null && field != DBNull.Value)
            {
                var cell = workSheet.Cell(r, c);

                if (field is byte)
                {
                    cell.SetValue((byte)field);
                }
                else if (field is sbyte)
                {
                    cell.SetValue((sbyte)field);
                }
                else if (field is short)
                {
                    cell.SetValue((short)field);
                }
                else if (field is ushort)
                {
                    cell.SetValue((ushort)field);
                }
                else if (field is int)
                {
                    cell.SetValue((int)field);
                }
                else if (field is uint)
                {
                    cell.SetValue((uint)field);
                }
                else if (field is float)
                {
                    cell.SetValue((float)field);
                }
                else if (field is double)
                {
                    cell.SetValue((double)field);
                }
                else if (field is char)
                {
                    cell.SetValue((int)field);
                }
                else if (field is bool)
                {
                    cell.SetValue((bool)field);
                }
                else if (field is TimeSpan)
                {
                    cell.SetValue((TimeSpan)field);
                }
                else if (field is DateTime)
                {
                    cell.SetValue((DateTime)field);
                }
                else
                {
                    cell.SetValue(Convert.ToString(field));
                }
            }
        }

        private void AdjustColumns(CremaDataTable dataTable, IXLWorksheet sheet)
        {
            var index = 1;
            if (this.settings.OmitAttribute == false)
            {
                sheet.Column(index++).AdjustToContents();
                sheet.Column(index++).AdjustToContents();
            }

            foreach (var item in this.GetSortedColumn(dataTable))
            {
                var column = sheet.Column(index++);
                column.AdjustToContents();
            }

            if (dataTable.Parent != null)
            {
                var column = sheet.Column(index++);
                column.AdjustToContents();
            }

            if (this.settings.OmitSignatureDate == false)
            {
                sheet.Column(index++).AdjustToContents();
                sheet.Column(index++).AdjustToContents();
                sheet.Column(index++).AdjustToContents();
                sheet.Column(index++).AdjustToContents();
            }
        }

        private IEnumerable<CremaDataColumn> GetSortedColumn(CremaDataTable dataTable)
        {
            foreach (var item in dataTable.PrimaryKey)
            {
                yield return item;
            }

            foreach (var item in dataTable.Columns)
            {
                if (item.IsKey == false)
                    yield return item;
            }
        }

        public void Dispose()
        {

        }
    }
}
