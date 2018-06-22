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
    [DataContract(Name = "DomainActionPostInfo", Namespace = SchemaUtility.Namespace)]
    struct DomainPostSerializationInfo
    {
        [DataMember]
        public SerializationItemCollection<DomainPostItemSerializationInfo> Items { get; set; }
    }
}
