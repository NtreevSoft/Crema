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

using ClosedXML.Excel;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ntreev.Crema.Spreadsheet
{
    public class SpreadsheetWriter : IDisposable
    {
        private readonly CremaDataType[] dataTypes;
        private readonly CremaDataTable[] dataTables;
        private readonly SpreadsheetWriterSettings settings;

        public SpreadsheetWriter(CremaDataSet dataSet)
            : this(dataSet, SpreadsheetWriterSettings.Default)
        {

        }

        public SpreadsheetWriter(CremaDataSet dataSet, SpreadsheetWriterSettings settings)
        {
            this.dataTypes = dataSet.Types.ToArray();
            this.dataTables = dataSet.Tables.ToArray();
            this.settings = settings;
        }

        public SpreadsheetWriter(CremaDataType dataType)
            : this(dataType, SpreadsheetWriterSettings.Default)
        {

        }

        public SpreadsheetWriter(CremaDataType dataType, SpreadsheetWriterSettings settings)
        {
            this.dataTypes = new CremaDataType[] { dataType };
            this.dataTables = new CremaDataTable[] { };
            this.settings = settings;
        }

        public SpreadsheetWriter(CremaDataType[] dataTypes)
            : this(dataTypes, SpreadsheetWriterSettings.Default)
        {

        }

        public SpreadsheetWriter(CremaDataType[] dataTypes, SpreadsheetWriterSettings settings)
        {
            this.dataTypes = dataTypes;
            this.dataTables = new CremaDataTable[] { };
            this.settings = settings;
        }

        public SpreadsheetWriter(CremaDataTable dataTable)
            : this(dataTable, SpreadsheetWriterSettings.Default)
        {

        }

        public SpreadsheetWriter(CremaDataTable dataTable, SpreadsheetWriterSettings settings)
        {
            this.dataTypes = new CremaDataType[] { };
            this.dataTables = new CremaDataTable[] { dataTable };
            this.settings = settings;
        }

        public SpreadsheetWriter(CremaDataTable[] dataTables)
            : this(dataTables, SpreadsheetWriterSettings.Default)
        {

        }

        public SpreadsheetWriter(CremaDataTable[] dataTables, SpreadsheetWriterSettings settings)
        {
            this.dataTypes = new CremaDataType[] { };
            this.dataTables = dataTables;
            this.settings = settings;
        }

        public void Write(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                this.Write(stream);
            }
        }

        public void Write(Stream stream)
        {
            var workbook = new XLWorkbook();
            this.WriteSheets(workbook);
            workbook.SaveAs(stream);
        }

        public void Dispose()
        {

        }

        public event ProgressEventHandler Progress;

        protected virtual void OnProgress(ProgressEventArgs e)
        {
            this.Progress?.Invoke(this, e);
        }

        private void WriteSheets(XLWorkbook workbook)
        {
            var list = new List<object>();
            if (this.settings.Properties.Count > 0)
            {
                list.Add(this.settings.Properties);
            }
            if (this.settings.OmitType == false)
            {
                list.AddRange(this.dataTypes);
            }
            if (this.settings.OmitTable == false)
            {
                list.AddRange(this.dataTables);
            }

            if (this.settings.Sort != null)
                list.Sort(this.settings.Sort);

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item is IDictionary)
                {
                    this.WriteInformation(workbook);
                }
                else if (item is CremaDataType dataType)
                {
                    this.WriteType(workbook, dataType);
                }
                else if (item is CremaDataTable dataTable)
                {
                    this.WriteTable(workbook, dataTable);
                }
                this.OnProgress(new ProgressEventArgs(item, i, list.Count));
            }
        }

        private void WriteType(XLWorkbook workbook, CremaDataType item)
        {

            var worksheet = workbook.AddWorksheet(this.settings.NameEllipsis($"${item.Name}"));

            if (item.Tags.Color != null)
                worksheet.SetTabColor(XLColor.FromHtml(item.Tags.Color));

            this.WriteMembers(item, worksheet);
            this.AdjustMembers(item, worksheet);

            worksheet.SheetView.Freeze(1, 1);
        }

        private void WriteTable(XLWorkbook workbook, CremaDataTable dataTable)
        {
            var worksheet = workbook.AddWorksheet(this.settings.NameEllipsis(dataTable.Name));

            if (dataTable.Tags.Color != null)
                worksheet.SetTabColor(XLColor.FromHtml(dataTable.Tags.Color));

            this.WriteColumns(dataTable, worksheet);
            this.WriteRows(dataTable, worksheet);
            this.AdjustColumns(dataTable, worksheet);

            worksheet.SheetView.Freeze(1, dataTable.PrimaryKey.Length);
        }

        private void WriteInformation(XLWorkbook workbook)
        {
            var worksheet = workbook.AddWorksheet(SpreadsheetUtility.InformationSheet);
            var properties = this.settings.Properties;
            var r = 1;
            foreach (var item in properties.Keys)
            {
                var c = 1;
                this.WriteField(worksheet, r, c++, item.ToString());
                this.WriteField(worksheet, r, c++, $"{properties[item]}");
                r++;
            }

            {
                var index = 1;
                worksheet.Column(index++).AdjustToContents();
                worksheet.Column(index++).AdjustToContents();
            }
        }

        private void WriteMembers(CremaDataType dataType, IXLWorksheet sheet)
        {
            var index = 1;
            var rowIndex = 1;
            if (this.settings.OmitAttribute == false)
            {
                sheet.Cell(rowIndex, index++).Value = CremaSchema.Tags;
                sheet.Cell(rowIndex, index++).Value = CremaSchema.Enable;
            }

            sheet.Cell(rowIndex, index++).Value = CremaSchema.Name;
            sheet.Cell(rowIndex, index++).Value = CremaSchema.Value;
            sheet.Cell(rowIndex, index++).Value = CremaSchema.Comment;

            if (this.settings.OmitSignatureDate == false)
            {
                sheet.Cell(rowIndex, index++).Value = CremaSchema.Creator;
                sheet.Cell(rowIndex, index++).Value = CremaSchema.CreatedDateTime;
                sheet.Cell(rowIndex, index++).Value = CremaSchema.Modifier;
                sheet.Cell(rowIndex, index++).Value = CremaSchema.ModifiedDateTime;
            }

            this.WriteMembers(dataType.Members, sheet);
        }

        private void WriteMembers(CremaDataTypeMemberCollection members, IXLWorksheet worksheet)
        {
            var r = 2;
            foreach (var item in members)
            {
                var c = 1;
                if (this.settings.OmitAttribute == false)
                {
                    this.WriteField(worksheet, r, c++, item.Tags.ToString());
                    this.WriteField(worksheet, r, c++, item.IsEnabled);
                }

                this.WriteField(worksheet, r, c++, item.Name);
                this.WriteField(worksheet, r, c++, item.Value);
                this.WriteField(worksheet, r, c++, item.Comment);

                if (this.settings.OmitSignatureDate == false)
                {
                    this.WriteField(worksheet, r, c++, item.CreationInfo.ID);
                    this.WriteField(worksheet, r, c++, item.CreationInfo.DateTime);
                    this.WriteField(worksheet, r, c++, item.ModificationInfo.ID);
                    this.WriteField(worksheet, r, c++, item.ModificationInfo.DateTime);
                }

                r++;
            }
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
                cell.Value = CremaSchema.__ParentID__;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                column.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                column.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            }

            if (dataTable.Childs.Any() == true)
            {
                var column = sheet.Column(index);
                var cell = sheet.Cell(rowIndex, index++);
                cell.Value = CremaSchema.__RelationID__;
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

        private void WriteRows(CremaDataTable dataTable, IXLWorksheet worksheet)
        {
            var r = 2;
            foreach (var row in dataTable.Rows)
            {
                var c = 1;
                if (this.settings.OmitAttribute == false)
                {
                    this.WriteField(worksheet, r, c++, row.Tags.ToString());
                    this.WriteField(worksheet, r, c++, row.IsEnabled);
                }

                foreach (var column in this.GetSortedColumn(dataTable))
                {
                    var value = row[column];
                    this.WriteField(worksheet, r, c, value);
                    c++;
                }

                if (dataTable.Parent != null)
                {
                    this.WriteField(worksheet, r, c++, row.ParentID);
                }

                if (dataTable.Childs.Any())
                {
                    this.WriteField(worksheet, r, c++, row.RelationID);
                }

                if (this.settings.OmitSignatureDate == false)
                {
                    this.WriteField(worksheet, r, c++, row.CreationInfo.ID);
                    this.WriteField(worksheet, r, c++, row.CreationInfo.DateTime);
                    this.WriteField(worksheet, r, c++, row.ModificationInfo.ID);
                    this.WriteField(worksheet, r, c++, row.ModificationInfo.DateTime);
                }

                r++;
            }
        }

        private void WriteField(IXLWorksheet worksheet, int r, int c, object field)
        {
            if (field != null && field != DBNull.Value)
            {
                var cell = worksheet.Cell(r, c);

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
                else if (field is long)
                {
                    cell.SetValue((long)field);
                }
                else if (field is ulong)
                {
                    cell.SetValue((ulong)field);
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

        private void AdjustColumns(CremaDataTable dataTable, IXLWorksheet worksheet)
        {
            var index = 1;
            if (this.settings.OmitAttribute == false)
            {
                worksheet.Column(index++).AdjustToContents();
                worksheet.Column(index++).AdjustToContents();
            }

            foreach (var item in this.GetSortedColumn(dataTable))
            {
                var column = worksheet.Column(index++);
                column.AdjustToContents();
            }

            if (dataTable.Parent != null)
            {
                var column = worksheet.Column(index++);
                column.AdjustToContents();
            }

            if (this.settings.OmitSignatureDate == false)
            {
                worksheet.Column(index++).AdjustToContents();
                worksheet.Column(index++).AdjustToContents();
                worksheet.Column(index++).AdjustToContents();
                worksheet.Column(index++).AdjustToContents();
            }
        }

        private void AdjustMembers(CremaDataType dataType, IXLWorksheet sheet)
        {
            var index = 1;
            if (this.settings.OmitAttribute == false)
            {
                sheet.Column(index++).AdjustToContents();
                sheet.Column(index++).AdjustToContents();
            }

            foreach (var item in dataType.Members)
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
    }
}
