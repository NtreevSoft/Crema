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
using Ntreev.Crema.Spreadsheet.Excel;
using Ntreev.Library;

namespace Ntreev.Crema.Spreadsheet.Csv
{
    public class SpreadsheetCsvWriterSettings
    {
        public static SpreadsheetCsvWriterSettings Default = new SpreadsheetCsvWriterSettings();

        public SpreadsheetCsvWriterSettings()
        {
            this.OmitSignatureDate = true;
            this.Tags = TagInfo.All;
            this.NameEllipsis = SpreadsheetUtility.Ellipsis;
        }

        public bool OmitAttribute { get; set; }

        public bool OmitSignatureDate { get; set; }

        public TagInfo Tags { get; set; }

        public Func<string, string> NameEllipsis { get; set; }

        public string CategorySeperatorString { get; set; }= ".";

        public string Delimiter { get; set; } = ",";

        public string Extension { get; set; } = ".csv";

        public string FilenamePattern { get; set; }

        public bool CreateDirectoryIfNotExists { get; set; } = true;
    }
}
