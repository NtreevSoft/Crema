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

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Ntreev.Crema.Data;
using Ntreev.Crema.Spreadsheet.Properties;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Ntreev.Crema.Data.Xml.Schema;
using ClosedXML.Excel;

namespace Ntreev.Crema.Spreadsheet
{
    public class SpreadsheetReader : IDisposable
    {
        private XLWorkbook workbook;

        public SpreadsheetReader(string filename)
            : this(new XLWorkbook(filename))
        {

        }

        public SpreadsheetReader(Stream stream)
            : this(new XLWorkbook(stream))
        {

        }

        private SpreadsheetReader(XLWorkbook workbook)
        {
            this.workbook = workbook;
        }

        public void Dispose()
        {
            this.workbook.Dispose();
            this.workbook = null;
        }

        public static string[] ReadSheetNames(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return SpreadsheetReader.ReadSheetNames(fs);
            }
        }

        public static string[] ReadSheetNames(Stream stream)
        {
            using (var doc = SpreadsheetDocument.Open(stream, false))
            {
                var sheets = doc.WorkbookPart.Workbook.GetFirstChild<Sheets>();

                var query = from item in sheets.Elements<Sheet>()
                            orderby item.Name.Value
                            select item.Name.Value;
                return query.ToArray();
            }
        }

        public void Read(CremaDataSet dataSet)
        {
            this.Read(dataSet, new Progress());
        }

        public void Read(CremaDataSet dataSet, IProgress progress)
        {
            var query = from sheet in this.workbook.Worksheets
                        join table in dataSet.Tables on sheet.Name equals SpreadsheetUtility.Ellipsis(table.Name)
                        orderby table.Name
                        select sheet;
            var items = query.ToArray();

            var step = new StepProgress(progress);
            step.Begin(items.Length);
            foreach (var item in query)
            {
                this.ReadSheet(item, dataSet);
                step.Next("read {0} : {1}", ConsoleProgress.GetProgressString(step.Step + 1, items.Length), item.Name);
            }
            step.Complete();
        }

        private void ReadSheet(IXLWorksheet worksheet, CremaDataSet dataSet)
        {
            var dataTable = dataSet.Tables.FirstOrDefault(item => item.Name == worksheet.Name);
            if (dataTable == null)
            {
                if (worksheet.Name.IndexOf('~') >= 0)
                {
                    dataTable = dataSet.Tables.FirstOrDefault(item => SpreadsheetUtility.Ellipsis(item.Name) == worksheet.Name);
                }
            }

            if (dataTable == null)
                return;

            var columns = this.ReadColumns(dataTable, worksheet.FirstRow());

            foreach (var item in worksheet.Rows().Skip(1))
            {
                if (item.Cells(true).Any() == false)
                    continue;
                this.ReadRow(dataTable, columns, item);
            }
        }

        private string[] ReadColumns(CremaDataTable dataTable, IXLRow row)
        {
            var columns = new List<string>();

            foreach (var item in row.Cells())
            {
                var text = item.GetString();

                if (text == string.Empty)
                {
                    var message = string.Format("'{0}!{1}'의 이름이 공백입니다. 열의 이름은 공백이 될 수 없습니다.", item.Address.Worksheet.Name, item.Address);
                    throw new CremaDataException(message);
                }

                if (text == CremaSchema.Tags || text == CremaSchema.Enable || text == CremaSchema.RelationID)
                {
                    columns.Add(text);
                    continue;
                }

                if (text == CremaSchema.Creator || text == CremaSchema.CreatedDateTime || text == CremaSchema.Modifier || text == CremaSchema.ModifiedDateTime)
                {
                    columns.Add(text);
                    continue;
                }

                if (dataTable.Columns.Contains(text) == false)
                {
                    var message = string.Format("'{0}!{1}'의 '{2}'열은 '{3}'테이블에 존재하지 않는 열입니다.", item.Address.Worksheet.Name, item.Address, item.Value, dataTable.TableName);
                    throw new CremaDataException(message);
                }

                if (columns.Contains(text) == true)
                {
                    var message = string.Format("'{0}!{1}'의 '{2}'열은 이미 존재합니다.", item.Address.Worksheet.Name, item.Address, item.Value);
                    throw new CremaDataException(message);
                }
                columns.Add(text);
            }

            return columns.ToArray();
        }

        private void ReadRow(CremaDataTable dataTable, string[] columns, IXLRow row)
        {
            var fields = this.CollectFields(dataTable, columns, row);
            if (fields.Any() == false)
                return;

            var dataRow = this.NewRow(dataTable, fields, row);

            foreach (var item in dataTable.Columns)
            {
                if (fields.ContainsKey(item.ColumnName) == true)
                {
                    dataRow[item] = fields[item.ColumnName];
                }
            }

            foreach (var item in dataTable.Attributes)
            {
                if (fields.ContainsKey(item.AttributeName) == true)
                {
                    dataRow.SetAttribute(item.AttributeName, fields[item.AttributeName]);
                }
            }

            try
            {
                dataTable.Rows.Add(dataRow);
            }
            catch (Exception e)
            {
                throw new CremaDataException(string.Format("'{0}!{1}' 행 추가중에 문제가 발생했습니다.", row.Worksheet.Name, row.RangeAddress), e);
            }
        }

        private CremaDataRow NewRow(CremaDataTable dataTable, Dictionary<string, object> fields, IXLRow row)
        {
            if (dataTable.Parent == null)
                return dataTable.NewRow();

            if (fields.ContainsKey(CremaSchema.RelationID) == false)
            {
                throw new CremaDataException(string.Format("'{0}!{1}'에 부모 행 번호가 존재하지 않습니다.", row.Worksheet.Name, row.RangeAddress));
            }

            var rowIndex = (int)fields[CremaSchema.RelationID];
            var index = rowIndex - 2;
            var parentTable = dataTable.Parent;

            if (index < 0 || index >= parentTable.Rows.Count)
            {
                throw new CremaDataException(string.Format("'{0}!{1}'에 부모 행 번호'{2}'은(는) 잘못된 값입니다.", row.Worksheet.Name, row.RangeAddress, rowIndex));
            }
            var parentRow = parentTable.Rows[index];

            return dataTable.NewRow(parentRow);
        }

        private Dictionary<string, object> CollectFields(CremaDataTable dataTable, string[] columns, IXLRow row)
        {
            var fields = new Dictionary<string, object>(columns.Length);
            foreach (var item in row.Cells())
            {
                if (item.Value == null || item.Value.ToString() == string.Empty)
                    continue;

                var columnIndex = item.Address.ColumnNumber - 1;

                if (columnIndex >= columns.Length)
                    throw new CremaDataException("'{0}!{1}'의 값'{2}'은(는) 테이블 범위 밖에 있으므로 사용할 수 없습니다.", item.Address.Worksheet.Name, item.Address, item.Value);
                var columnName = columns[columnIndex];
                var column = dataTable.Columns[columnName];

                try
                {
                    if (columnName == CremaSchema.Creator || columnName == CremaSchema.CreatedDateTime || columnName == CremaSchema.Modifier || columnName == CremaSchema.ModifiedDateTime)
                    {
                        continue;
                    }
                    if (columnName == CremaSchema.Tags)
                    {
                        fields.Add(columnName, item.GetString());
                    }
                    else if (columnName == CremaSchema.Enable)
                    {
                        fields.Add(columnName, item.GetBoolean());
                    }
                    else if (columnName == CremaSchema.RelationID)
                    {
                        fields.Add(columnName, item.GetValue<int>());
                    }
                    else if (column.DataType == typeof(byte))
                    {
                        fields.Add(columnName, item.GetValue<byte>());
                    }
                    else if (column.DataType == typeof(sbyte))
                    {
                        fields.Add(columnName, item.GetValue<sbyte>());
                    }
                    else if (column.DataType == typeof(short))
                    {
                        fields.Add(columnName, item.GetValue<short>());
                    }
                    else if (column.DataType == typeof(ushort))
                    {
                        fields.Add(columnName, item.GetValue<ushort>());
                    }
                    else if (column.DataType == typeof(int))
                    {
                        fields.Add(columnName, item.GetValue<int>());
                    }
                    else if (column.DataType == typeof(uint))
                    {
                        fields.Add(columnName, item.GetValue<uint>());
                    }
                    else if (column.DataType == typeof(float))
                    {
                        fields.Add(columnName, item.GetValue<float>());
                    }
                    else if (column.DataType == typeof(double))
                    {
                        fields.Add(columnName, item.GetDouble());
                    }
                    else if (column.DataType == typeof(bool))
                    {
                        fields.Add(columnName, item.GetBoolean());
                    }
                    else if (column.DataType == typeof(TimeSpan))
                    {
                        fields.Add(columnName, item.GetTimeSpan());
                    }
                    else if (column.DataType == typeof(DateTime))
                    {
                        fields.Add(columnName, item.GetDateTime());
                    }
                    else
                    {
                        fields.Add(columnName, item.GetString());
                    }
                }
                catch (Exception e)
                {
                    throw new CremaDataException(string.Format("'{0}!{1}'의 값'{2}'은(는) '{3}'열의 적합한 값이 아닙니다.", item.Address.Worksheet.Name, item.Address, item.Value, columnName), e);
                }
            }
            return fields;
        }
    }
}
