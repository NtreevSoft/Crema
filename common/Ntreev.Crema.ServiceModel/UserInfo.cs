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
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.IO;
using Ntreev.Library;
using System.Xml.Schema;
using System.Xml;
using Ntreev.Library.Serialization;
using Ntreev.Crema.Data.Xml.Schema;

namespace Ntreev.Crema.ServiceModel
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct UserInfo
    {
        [XmlElement]
        public string ID { get; set; }

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string CategoryName { get; set; }

        [XmlElement]
        public Authority Authority { get; set; }

        [XmlElement]
        public SignatureDate CreationInfo { get; set; }

        [XmlElement]
        public SignatureDate ModificationInfo { get; set; }

        [XmlElement]
        public AuthenticationInfo[] AuthenticationInfos { get; set; }

        [XmlIgnore]
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

        public IDictionary<string, object> ToDictionary()
        {
            var props = new Dictionary<string, object>
            {
                { nameof(this.ID), this.ID },
                { nameof(this.Name), this.Name },
                { nameof(this.CategoryName), this.CategoryName },
                { nameof(this.Authority), $"{this.Authority}" },
                { CremaSchema.Creator, this.CreationInfo.ID },
                { CremaSchema.CreatedDateTime, this.CreationInfo.DateTime },
                { CremaSchema.Modifier, this.ModificationInfo.ID },
                { CremaSchema.ModifiedDateTime, this.ModificationInfo.DateTime }
            };
            return props;
        }

        #region DataMember

        [DataMember]
        [XmlIgnore]
        private string Xml
        {
            get { return XmlSerializerUtility.GetString(this); }
            set { this = XmlSerializerUtility.ReadString(this, value); }
        }

        #endregion
    }
}
