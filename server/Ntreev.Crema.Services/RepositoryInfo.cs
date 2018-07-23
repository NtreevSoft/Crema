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

using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ntreev.Crema.Services
{
    [DataContract(Name = nameof(RepositoryInfo), Namespace = SchemaUtility.Namespace)]
    public struct RepositoryInfo
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public string Revision { get; set; }

        [DataMember]
        public SignatureDate CreationInfo { get; set; }

        [DataMember]
        public SignatureDate ModificationInfo { get; set; }

        public IDictionary<string, object> ToDictionary()
        {
            var props = new Dictionary<string, object>
            {
                { nameof(this.ID), this.ID },
                { nameof(this.Name), this.Name },
                { nameof(this.Comment), this.Comment },
                { nameof(this.Revision), this.Revision },
                { nameof(CremaSchema.Creator), this.CreationInfo.ID },
                { nameof(CremaSchema.CreatedDateTime), this.CreationInfo.DateTime },
                { nameof(CremaSchema.Modifier), this.ModificationInfo.ID },
                { nameof(CremaSchema.ModifiedDateTime), this.ModificationInfo.DateTime },
            };
            return props;
        }

        public readonly static RepositoryInfo Empty = new RepositoryInfo()
        {
            Name = string.Empty,
            Comment = string.Empty,
            CreationInfo = SignatureDate.Empty,
            ModificationInfo = SignatureDate.Empty,
        };
    }
}
