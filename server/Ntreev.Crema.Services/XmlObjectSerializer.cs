using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    [Export(typeof(IObjectSerializer))]
    class XmlObjectSerializer : IObjectSerializer
    {
        private const string extensions = ".xml";

        public string Name => "xml";

        public object Deserialize(Type type, string text)
        {
            throw new NotImplementedException();
        }

        public string[] Serialize(object obj, string itemPath)
        {
            var filename = itemPath + extensions;
            DataContractSerializerUtility.Write(filename, obj, true);
            return new string[] { filename };
        }

        public string[] VerifyPath(Type type, string itemPath)
        {
            var filename = itemPath + extensions;
            return new string[] { filename };
        }

        public static readonly XmlObjectSerializer Default = new XmlObjectSerializer();
    }
}
