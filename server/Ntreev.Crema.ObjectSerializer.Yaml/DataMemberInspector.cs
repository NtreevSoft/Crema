using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

namespace Ntreev.Crema.ObjectSerializer.Yaml
{
    class DataMemberInspector : TypeInspectorSkeleton
    {
        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
        {
            if (Attribute.GetCustomAttribute(type, typeof(DataContractAttribute)) is DataContractAttribute attribute)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
