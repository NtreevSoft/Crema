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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;
using Ntreev.Library.IO;

namespace Ntreev.Crema.Spreadsheet.Csv
{
    public class SpreadsheetCsvWriter : IDisposable
    {
        private readonly CremaDataSet dataSet;
        private readonly SpreadsheetCsvWriterSettings settings;

        private IDictionary<string, Func<CremaDataTable, string>> replaceDictionary;

        public SpreadsheetCsvWriter(CremaDataSet dataSet) : this(dataSet, SpreadsheetCsvWriterSettings.Default)
        {
        }

        public SpreadsheetCsvWriter(CremaDataSet dataSet, SpreadsheetCsvWriterSettings settings)
        {
            this.dataSet = dataSet;
            this.settings = settings;
            replaceDictionary = new Dictionary<string, Func<CremaDataTable, string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["{categoryPath}"] = table => table.CategoryPath == "/" ? "" : table.CategoryPath.Remove(0, 1).Replace("/", this.settings.CategorySeperatorString),
                ["{tableName}"] = table => table.TableName,
                ["{name}"] = table => table.Name,
                ["{extension}"] = table => this.settings.Extension
            };
        }

        public void Write(string filename)
        {
            this.WriteSheet(filename, new Progress());
        }

        private string GetFilename(string filenamePattern, CremaDataTable table)
        {
            foreach (var keyValuePair in this.replaceDictionary)
            {
                if (filenamePattern.Contains(keyValuePair.Key))
                {
                    filenamePattern = filenamePattern.Replace(keyValuePair.Key, keyValuePair.Value(table));
                }
            }

            return filenamePattern;
        }

        private void WriteSheet(string filenamePattern, IProgress progress)
        {
            var csvConfiguration = new Configuration
            {
                HasHeaderRecord = true,
                Encoding = Encoding.UTF8,
                Delimiter = settings.Delimiter
            };

            var step = new StepProgress(progress);
            step.Begin(this.dataSet.Tables.Count);

            foreach (var item in this.dataSet.Tables)
            {
                var filePath = this.GetFilename(filenamePattern, item);
                if (this.settings.CreateDirectoryIfNotExists)
                {
                    var directoryPath = Path.GetDirectoryName(filePath);
                    DirectoryUtility.Prepare(directoryPath);
                }

                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, csvConfiguration))
                {
                    this.WriteHeader(csv, item);

                    foreach (var row in item.Rows)
                    {
                        this.WriteRow(csv, item, item.Columns, row);
                        csv.NextRecord();
                    }
                }

                step.Next("write {0} : {1}", ConsoleProgress.GetProgressString(step.Step + 1, this.dataSet.Tables.Count), item.Name);
            }
            step.Complete();
        }

        private void WriteHeader(CsvWriter csv, CremaDataTable dataTable)
        {
            if (!this.settings.OmitAttribute)
            {
                csv.WriteField(CremaSchema.Tags);
                csv.WriteField(CremaSchema.Enable);
            }

            foreach (var column in dataTable.Columns)
            {
                csv.WriteField(column.ColumnName);
            }

            if (dataTable.Parent != null)
            {
                csv.WriteField(CremaSchema.RelationID);
            }

            if (!this.settings.OmitSignatureDate)
            {
                csv.WriteField(CremaSchema.Creator);
                csv.WriteField(CremaSchema.CreatedDateTime);
                csv.WriteField(CremaSchema.Modifier);
                csv.WriteField(CremaSchema.ModifiedDateTime);
            }

            csv.NextRecord();
        }

        private void WriteRow(CsvWriter csv, CremaDataTable dataTable, CremaDataColumnCollection columns, CremaDataRow row)
        {
            if (!this.settings.OmitAttribute)
            {
                csv.WriteField(row.Tags.ToString());
                csv.WriteField(row.IsEnabled.ToString());
            }

            foreach (var column in columns)
            {
                csv.WriteField(row[column].ToString());
            }

            if (dataTable.Parent != null)
            {
                var parentIndex = dataTable.Parent.Rows.IndexOf(row.Parent);
                csv.WriteField(parentIndex + 2);
            }

            if (!this.settings.OmitSignatureDate)
            {
                csv.WriteField(row.CreationInfo.ID);
                csv.WriteField(row.CreationInfo.DateTime);
                csv.WriteField(row.ModificationInfo.ID);
                csv.WriteField(row.ModificationInfo.DateTime);
            }
        }

        public void Dispose()
        {
        }
    }
}
