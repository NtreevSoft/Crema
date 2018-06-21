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
    struct DomainActionPost
    {
        public DomainActionPost(long id, Type type)
        {
            this.ID = id;
            this.Type = type.AssemblyQualifiedName;
            this.DateTime = DateTime.Now;
        }

        [DataMember]
        public long ID { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }

        [DataMember]
        public string Type { get; set; }
    }
}
