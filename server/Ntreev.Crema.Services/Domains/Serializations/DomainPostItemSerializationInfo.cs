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
    struct DomainPostItemSerializationInfo
    {
        public DomainPostItemSerializationInfo(long id, Type type)
        {
            this.ID = id;
            this.Type = type.AssemblyQualifiedName;
            this.DateTime = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{this.ID}\t{this.DateTime:o}\t{this.Type}";
        }

        public static DomainPostItemSerializationInfo Parse(string text)
        {
            var items = StringUtility.Split(text, '\t');
            return new DomainPostItemSerializationInfo()
            {
                ID = long.Parse(items[0]),
                DateTime = DateTime.Parse(items[1]),
                Type = items[2],
            };
        }

        [DataMember]
        public long ID { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }

        [DataMember]
        public string Type { get; set; }
    }
}
