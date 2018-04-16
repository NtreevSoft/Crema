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

using Ntreev.Crema.Runtime.Generation;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ntreev.Crema.Tools.Runtime.ViewModels
{
    public class GenerationItemViewModel : PropertyChangedBase
    {
        private GenerationItemCollection owner;

        private string name;
        private string address;
        private string outputPath;
        private string database;
        private string languageType;
        private string filterExpression;
        private string tags;
        private bool isDevmode;
        private string options;

        [XmlElement]
        public string Name
        {
            get { return this.name ?? string.Empty; }
            set
            {
                this.name = value;
                this.NotifyOfPropertyChange(nameof(this.Name));
            }
        }

        [XmlElement]
        public string Address
        {
            get { return this.address ?? string.Empty; }
            set
            {
                this.address = value;
                this.NotifyOfPropertyChange(nameof(this.Address));
            }
        }

        [XmlElement]
        public string OutputPath
        {
            get { return this.outputPath ?? string.Empty; }
            set
            {
                this.outputPath = value;
                this.NotifyOfPropertyChange(nameof(this.OutputPath));
            }
        }

        [XmlElement]
        public string DataBase
        {
            get { return this.database ?? string.Empty; }
            set
            {
                this.database = value;
                this.NotifyOfPropertyChange(nameof(this.DataBase));
            }
        }

        [XmlElement]
        public string LanguageType
        {
            get { return this.languageType ?? string.Empty; }
            set
            {
                this.languageType = value;
                this.NotifyOfPropertyChange(nameof(this.LanguageType));
            }
        }

        [XmlElement]
        public string FilterExpression
        {
            get { return this.filterExpression ?? string.Empty; }
            set
            {
                this.filterExpression = value;
                this.NotifyOfPropertyChange(nameof(this.FilterExpression));
            }
        }

        [XmlElement]
        public string Tags
        {
            get { return this.tags ?? string.Empty; }
            set
            {
                this.tags = value;
                this.NotifyOfPropertyChange(nameof(this.Tags));
            }
        }

        [XmlElement]
        public bool IsDevmode
        {
            get { return this.isDevmode; }
            set
            {
                this.isDevmode = value;
                this.NotifyOfPropertyChange(nameof(this.IsDevmode));
            }
        }

        [XmlElement]
        public string Options
        {
            get { return this.options ?? string.Empty; }
            set
            {
                this.options = value;
                this.NotifyOfPropertyChange(nameof(this.Options));
            }
        }

        public IEnumerable<IMenuItem> ContextMenus
        {
            get; set;
        }

        public GenerationItemCollection Owner
        {
            get { return this.owner; }
            set { this.owner = value; }
        }

        public static readonly GenerationItemViewModel Empty = new GenerationItemViewModel()
        {
            Tags = (string)TagInfo.All,
            LanguageType = "c#",
        };

        public bool Equals(GenerationItemViewModel dest)
        {
            if (this.Name != dest.Name)
                return false;
            if (this.Address != dest.Address)
                return false;
            if (this.OutputPath != dest.OutputPath)
                return false;
            if (this.DataBase != dest.DataBase)
                return false;
            if (this.LanguageType != dest.LanguageType)
                return false;
            if (this.FilterExpression != dest.FilterExpression)
                return false;
            if (this.Tags != dest.Tags)
                return false;
            if (this.IsDevmode != dest.IsDevmode)
                return false;
            if (this.Options != dest.Options)
                return false;
            return true;
        }

        public GenerationItemViewModel Clone()
        {
            return new GenerationItemViewModel()
            {
                name = this.name,
                address = this.address,
                outputPath = this.outputPath,
                database = this.database,
                languageType = this.languageType,
                filterExpression = this.filterExpression,
                tags = this.tags,
                isDevmode = this.isDevmode,
                options = this.options,
            };
        }
    }
}
