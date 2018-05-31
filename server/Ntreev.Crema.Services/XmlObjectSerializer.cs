using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
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

        public string[] Serialize(object obj, string itemPath, object state)
        {
            if (obj is CremaDataTable dataTable)
            {
                var schemaPath = itemPath + CremaSchema.SchemaExtension;
                var xmlPath = itemPath + CremaSchema.XmlExtension;
                var ns = state as string;
                if (string.IsNullOrEmpty(ns) == true)
                {
                    File.WriteAllText(schemaPath, dataTable.GetXmlSchema(), Encoding.UTF8);
                    File.WriteAllText(xmlPath, dataTable.GetXml(), Encoding.UTF8);

                    return new string[] { schemaPath, xmlPath, };
                }
                else
                {
                    File.WriteAllText(xmlPath, dataTable.GetXml(), Encoding.UTF8);
                    return new string[] { xmlPath, };
                }
            }
            else
            {
                var filename = itemPath + extensions;
                DataContractSerializerUtility.Write(filename, obj, true);
                return new string[] { filename };
            }
        }

        public string[] VerifyPath(Type type, string itemPath, object state)
        {
            if (type == typeof(CremaDataTable))
            {
                var schemaPath = itemPath + CremaSchema.SchemaExtension;
                var xmlPath = itemPath + CremaSchema.XmlExtension;
                var ns = state as string;
                if (string.IsNullOrEmpty(ns) == true)
                {
                    return new string[] { schemaPath, xmlPath, };
                }
                else
                {
                    return new string[] { xmlPath, };
                }
            }
            else
            {
                var filename = itemPath + extensions;
                return new string[] { filename };
            }
        }

        public static readonly XmlObjectSerializer Default = new XmlObjectSerializer();
    }
}
