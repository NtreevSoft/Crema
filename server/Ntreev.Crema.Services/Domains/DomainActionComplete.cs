using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Domains
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    struct DomainActionComplete
    {
        public DomainActionComplete(long id)
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
