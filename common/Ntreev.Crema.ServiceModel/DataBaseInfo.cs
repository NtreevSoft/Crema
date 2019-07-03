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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json;
using Ntreev.Library.Serialization;
using Ntreev.Crema.Data.Xml.Schema;

namespace Ntreev.Crema.ServiceModel
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct DataBaseInfo
    {
        [XmlElement]
        [JsonProperty]
        public Guid ID { get; set; }

        [XmlElement]
        [JsonProperty]
        public string Name { get; set; }

        [XmlElement]
        [JsonProperty]
        public string Comment { get; set; }

        [XmlElement]
        [JsonProperty]
        public long Revision { get; set; }

        [XmlElement]
        [JsonProperty]
        public TagInfo Tags { get; set; }

        [XmlElement]
        [JsonProperty]
        public long BranchRevision { get; set; }

        [XmlElement]
        [JsonProperty]
        public string BranchSource { get; set; }

        [XmlElement]
        [JsonProperty]
        public long BranchSourceRevision { get; set; }

        [XmlElement]
        [JsonProperty]
        public string TypesHashValue { get; set; }

        [XmlElement]
        [JsonProperty]
        public string TablesHashValue { get; set; }

        [XmlArray]
        [JsonProperty]
        public string[] Paths { get; set; }

        [XmlElement]
        [JsonProperty]
        public SignatureDate CreationInfo { get; set; }

        [XmlElement]
        [JsonProperty]
        public SignatureDate ModificationInfo { get; set; }

        public IDictionary<string, object> ToDictionary()
        {
            var props = new Dictionary<string, object>
            {
                { nameof(this.ID), this.ID },
                { nameof(this.Name), this.Name },
                { nameof(this.Comment), this.Comment },
                { nameof(this.Revision), this.Revision },
                { nameof(this.BranchRevision), this.BranchRevision},
                { nameof(this.BranchSource), this.BranchSource },
                { nameof(this.BranchSourceRevision), this.BranchSourceRevision},
                { nameof(this.Paths), this.Paths },
                { nameof(CremaSchema.Creator), this.CreationInfo.ID },
                { nameof(CremaSchema.CreatedDateTime), this.CreationInfo.DateTime },
                { nameof(CremaSchema.Modifier), this.ModificationInfo.ID },
                { nameof(CremaSchema.ModifiedDateTime), this.ModificationInfo.DateTime },
            };
            return props;
        }

        public readonly static DataBaseInfo Empty = new DataBaseInfo()
        {
            Name = string.Empty,
            Comment = string.Empty,
            BranchSource = string.Empty,
            Paths = new string[] { },
            CreationInfo = SignatureDate.Empty,
            ModificationInfo = SignatureDate.Empty,
        };


        #region DataMember

        [DataMember]
        [XmlIgnore]
        [JsonIgnore]
        private string Xml
        {
            get { return XmlSerializerUtility.GetString(this); }
            set { this = XmlSerializerUtility.ReadString(this, value); }
        }

        #endregion
    }
}
