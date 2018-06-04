using Ntreev.Crema.Data;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Data
{
    class DataSetDeserializationInfo : PropertyCollection
    {
        public SignatureDateProvider SignatureDateProvider { get; set; }

        public string[] TypePaths { get; set; }

        public string[] TablePaths { get; set; }
    }
}
