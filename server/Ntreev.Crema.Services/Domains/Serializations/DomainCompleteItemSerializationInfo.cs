using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Domains.Serializations
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    struct DomainCompleteItemSerializationInfo
    {
        public DomainCompleteItemSerializationInfo(long id)
        {
            this.ID = id;
            this.DateTime = DateTime.Now;
        }

        [DataMember]
        public long ID { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }
    }
}
