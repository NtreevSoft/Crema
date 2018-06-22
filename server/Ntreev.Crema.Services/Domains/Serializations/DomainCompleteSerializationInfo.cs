using Ntreev.Library;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Domains.Serializations
{
    [DataContract(Name = "DomainActionCompleteInfo", Namespace = SchemaUtility.Namespace)]
    struct DomainCompleteSerializationInfo
    {
        [DataMember]
        public SerializationItemCollection<DomainCompleteItemSerializationInfo> Items { get; set; }
    }
}
