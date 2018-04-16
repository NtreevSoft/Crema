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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Ntreev.Crema.Client.Converters.Properties;
using System.ComponentModel;

namespace Ntreev.Crema.Client.Converters
{
    public class ExporterSettings : PropertyChangedBase
    {
        public readonly static ExporterSettings Default = new ExporterSettings();

        private bool omitAttribute;
        private bool omitSignatureDate;
        private bool isSeparable;
        private bool isOneTableToOneFile;
        private bool isIncludeDate;
        private string outputDateFormat = "yyyy-MM-dd_HH_mm";  // isIncludeDate : true일때 출력하는 날짜 형식

        public ExporterSettings()
        {
        }

        [ConfigurationProperty(nameof(isSeparable))]
        public bool IsSeparable
        {
            get { return this.isSeparable; }
            set
            {
                this.isSeparable = value;
                this.NotifyOfPropertyChange(nameof(this.IsSeparable));
            }
        }

        [ConfigurationProperty(nameof(isOneTableToOneFile))]
        public bool IsOneTableToOneFile
        {
            get { return this.isOneTableToOneFile; }
            set
            {
                this.isOneTableToOneFile = value;
                this.NotifyOfPropertyChange(nameof(this.IsOneTableToOneFile));
            }
        }

        [ConfigurationProperty(nameof(isIncludeDate))]
        public bool IsIncludeDate
        {
            get { return this.isIncludeDate; }
            set
            {
                this.isIncludeDate = value;
                this.NotifyOfPropertyChange(nameof(this.IsIncludeDate));
            }
        }

        [ConfigurationProperty(nameof(omitAttribute))]
        public bool OmitAttribute
        {
            get { return this.omitAttribute; }
            set
            {
                this.omitAttribute = value;
                this.NotifyOfPropertyChange(nameof(this.OmitAttribute));
            }
        }

        [ConfigurationProperty(nameof(omitSignatureDate))]
        public bool OmitSignatureDate
        {
            get { return this.omitSignatureDate; }
            set
            {
                this.omitSignatureDate = value;
                this.NotifyOfPropertyChange(nameof(this.OmitSignatureDate));
            }
        }

        [ConfigurationProperty(nameof(outputDateFormat))]
        public string OutputDateFormat
        {
            get { return this.outputDateFormat; }
            set
            {
                this.outputDateFormat = value;
                this.NotifyOfPropertyChange(nameof(this.OutputDateFormat));
            }
        }
    }
}
