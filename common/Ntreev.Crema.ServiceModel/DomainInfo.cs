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
using System.Xml.Serialization;
using Ntreev.Crema.Data.Xml;
using System.Runtime.Serialization;
using System.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.IO;
using Ntreev.Library;
using System.Xml;
using System.Xml.Schema;
using Ntreev.Library.Serialization;
using Ntreev.Crema.Data.Xml.Schema;

namespace Ntreev.Crema.ServiceModel
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    [Serializable]
    public struct DomainInfo
    {
        [DataMember]
        public Guid DomainID { get; set; }

        [DataMember]
        public Guid DataBaseID { get; set; }

        [DataMember]
        public string ItemPath { get; set; }

        [DataMember]
        public string ItemType { get; set; }

        [DataMember]
        public string DomainType { get; set; }

        [DataMember]
        public string CategoryName { get; set; }

        [IgnoreDataMember]
        public string CategoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(this.CategoryName) == true)
                    return PathUtility.Separator;
                return PathUtility.Separator + this.CategoryName + PathUtility.Separator;
            }
            set
            {
                NameValidator.ValidateCategoryPath(value);
                if (value == PathUtility.Separator)
                    this.CategoryName = string.Empty;
                else
                    this.CategoryName = value.Trim(PathUtility.SeparatorChar);
            }
        }

        [DataMember]
        public SignatureDate CreationInfo { get; set; }

        [DataMember]
        public SignatureDate ModificationInfo { get; set; }

        public IDictionary<string, object> ToDictionary()
        {
            var props = new Dictionary<string, object>
            {
                { nameof(this.DomainID), $"{this.DomainID}" },
                { nameof(this.DataBaseID), $"{this.DataBaseID}" },
                { nameof(this.ItemPath), this.ItemPath },
                { nameof(this.ItemType), $"{this.ItemType}" },
                { nameof(this.DomainType), $"{this.DomainType}" },
                { nameof(this.CategoryPath), $"{this.CategoryPath}" },
                { CremaSchema.Creator, this.CreationInfo.ID },
                { CremaSchema.CreatedDateTime, this.CreationInfo.DateTime },
                { CremaSchema.Modifier, this.ModificationInfo.ID },
                { CremaSchema.ModifiedDateTime, this.ModificationInfo.DateTime }
            };
            return props;
        }
    }
}
