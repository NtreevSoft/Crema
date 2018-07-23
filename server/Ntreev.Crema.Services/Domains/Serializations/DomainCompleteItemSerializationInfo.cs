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

        public override string ToString()
        {
            return $"{this.ID}\t{this.DateTime:o}";
        }

        public static DomainCompleteItemSerializationInfo Parse(string text)
        {
            var items = StringUtility.Split(text, '\t');
            return new DomainCompleteItemSerializationInfo()
            {
                ID = long.Parse(items[0]),
                DateTime = DateTime.Parse(items[1])
            };
        }

        [DataMember]
        public long ID { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }
    }
}
