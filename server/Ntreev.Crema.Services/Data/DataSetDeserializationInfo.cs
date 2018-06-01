using Ntreev.Crema.Data;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Data
{
    [DataContract(Name = "DataSet", Namespace = SchemaUtility.Namespace)]
    public struct DataSetDeserializationInfo
    {
        [DataMember]
        public SignatureDateProvider SignatureDateProvider { get; set; }

        [DataMember]
        public string[] TypePaths { get; set; }

        [DataMember]
        public string[] TablePaths { get; set; }
    }
}
