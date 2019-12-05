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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Ntreev.Library;

namespace Ntreev.Crema.Client.Converters.Spreadsheet.Csv
{
    public class CsvExporterSettings : PropertyChangedBase
    {
        private bool omitAttribute;
        private bool omitSignatureDate;
        private string categorySeperatorString = ".";
        private string delimiter = ",";
        private string extension = ".csv";
        private string filenamePattern = "{categoryPath}{name}{extension}";
        private bool createDirectoryIfNotExists = true;
        private bool getDomainDataSetIfEditing = false;

        public static readonly CsvExporterSettings Default = new CsvExporterSettings();

        [ConfigurationProperty(nameof(omitAttribute))]
        public bool OmitAttribute
        {
            get => omitAttribute;
            set
            {
                if (value == omitAttribute) return;
                omitAttribute = value;
                NotifyOfPropertyChange();
            }
        }

        [ConfigurationProperty(nameof(omitSignatureDate))]
        public bool OmitSignatureDate
        {
            get => omitSignatureDate;
            set
            {
                if (value == omitSignatureDate) return;
                omitSignatureDate = value;
                NotifyOfPropertyChange();
            }
        }

        [ConfigurationProperty(nameof(categorySeperatorString))]
        public string CategorySeperatorString
        {
            get => categorySeperatorString;
            set
            {
                if (value == categorySeperatorString) return;
                categorySeperatorString = value;
                NotifyOfPropertyChange();
            }
        }

        [ConfigurationProperty(nameof(delimiter))]
        public string Delimiter
        {
            get => delimiter;
            set
            {
                if (value == delimiter) return;
                delimiter = value;
                NotifyOfPropertyChange();
            }
        }

        [ConfigurationProperty(nameof(extension))]
        public string Extension
        {
            get => extension;
            set
            {
                if (value == extension) return;
                extension = value;
                NotifyOfPropertyChange();
            }
        }

        [ConfigurationProperty]
        public string FilenamePattern
        {
            get => filenamePattern;
            set
            {
                if (value == filenamePattern) return;
                filenamePattern = value;
                NotifyOfPropertyChange();
            }
        }

        [ConfigurationProperty(nameof(createDirectoryIfNotExists))]
        public bool CreateDirectoryIfNotExists
        {
            get => createDirectoryIfNotExists;
            set
            {
                if (value == createDirectoryIfNotExists) return;
                createDirectoryIfNotExists = value;
                NotifyOfPropertyChange();
            }
        }

        [ConfigurationProperty(nameof(getDomainDataSetIfEditing))]
        public bool GetDomainDataSetIfEditing
        {
            get => getDomainDataSetIfEditing;
            set
            {
                if (value == getDomainDataSetIfEditing) return;
                getDomainDataSetIfEditing = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
