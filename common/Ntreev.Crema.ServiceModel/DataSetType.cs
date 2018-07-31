using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServiceModel
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public enum DataSetType
    {
        [EnumMember]
        All,

        [EnumMember]
        OmitContent,

        [EnumMember]
        TypeOnly,
    }
}
