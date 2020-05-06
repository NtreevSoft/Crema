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

using Caliburn.Micro;
using Ntreev.Library;

namespace Ntreev.Crema.Client.Converters.Spreadsheet.Excel
{
    public class ExcelTableTemplateExporterSettings : PropertyChangedBase
    {
        private static readonly ExcelTableTemplateExporterSettings Default = new ExcelTableTemplateExporterSettings();

        private bool omitAttribute;
        private bool omitSignatureDate;
        private bool isSeparable;
        private bool isOneTableToOneFile;
        private bool isIncludeDate;
        private string outputDateFormat = "yyyy-MM-dd_HH_mm";

        [ConfigurationProperty(nameof(isSeparable))]
        public bool IsSeparable
        {
            get => this.isSeparable;
            set
            {
                this.isSeparable = value;
                this.NotifyOfPropertyChange(nameof(this.IsSeparable));
            }
        }

        [ConfigurationProperty(nameof(isOneTableToOneFile))]
        public bool IsOneTableToOneFile
        {
            get => this.isOneTableToOneFile;
            set
            {
                this.isOneTableToOneFile = value;
                this.NotifyOfPropertyChange(nameof(this.IsOneTableToOneFile));
            }
        }

        [ConfigurationProperty(nameof(isIncludeDate))]
        public bool IsIncludeDate
        {
            get => this.isIncludeDate;
            set
            {
                this.isIncludeDate = value;
                this.NotifyOfPropertyChange(nameof(this.IsIncludeDate));
            }
        }

        [ConfigurationProperty(nameof(omitAttribute))]
        public bool OmitAttribute
        {
            get => this.omitAttribute;
            set
            {
                this.omitAttribute = value;
                this.NotifyOfPropertyChange(nameof(this.OmitAttribute));
            }
        }

        [ConfigurationProperty(nameof(omitSignatureDate))]
        public bool OmitSignatureDate
        {
            get => this.omitSignatureDate;
            set
            {
                this.omitSignatureDate = value;
                this.NotifyOfPropertyChange(nameof(this.OmitSignatureDate));
            }
        }

        [ConfigurationProperty(nameof(outputDateFormat))]
        public string OutputDateFormat
        {
            get => this.outputDateFormat;
            set
            {
                this.outputDateFormat = value;
                this.NotifyOfPropertyChange(nameof(this.OutputDateFormat));
            }
        }
    }
}
