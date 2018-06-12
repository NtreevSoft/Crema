using Ntreev.Crema.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

namespace Ntreev.Crema.ObjectSerializer.Yaml
{
    [Export(typeof(IObjectSerializer))]
    class YamlObjectSerializer : IObjectSerializer
    {
        private const string extensions = ".yaml";

        private static readonly Serializer propertySerializer;
        private static readonly Deserializer propertyDeserializer = new Deserializer();

        static YamlObjectSerializer()
        {
            //propertySerializer = new SerializerBuilder().WithTypeInspector(item => new DataMemberInspector()).Build();
            propertySerializer = new SerializerBuilder().Build();
        }

        public string Name => "yaml";

        public object Deserialize(string itemPath, Type type, IDictionary properties)
        {
            throw new NotImplementedException();
        }

        public string[] Serialize(string itemPath, object obj, IDictionary properties)
        {
            var filename = itemPath + extensions;
            var contents = propertySerializer.Serialize(obj);
            File.WriteAllText(filename, contents, Encoding.UTF8);
            return new string[] { filename };
        }

        public string[] GetPath(string itemPath, Type type, IDictionary properties)
        {
            var filename = itemPath + extensions;
            return new string[] { filename };
        }

        public string[] GetItemPaths(string path, Type type, IDictionary properties)
        {
            //if (type == typeof(CremaDataTable))
            //{
            //    throw new NotImplementedException();
            //}
            //else
            //{
            //    var directories = DirectoryUtility.GetAllDirectories(path, "*", true);
            //    var files = DirectoryUtility.GetAllFiles(path, "*.xml");
            //    return directories.Concat(files).OrderBy(item => item).ToArray();
            //}
            throw new NotImplementedException();
        }

        public string[] GetReferencedPath(string itemPath, Type type, IDictionary properties)
        {
            throw new NotImplementedException();
        }

        public void Validate(string itemPath, Type type, IDictionary properties)
        {
            throw new NotImplementedException();
        }
    }
}
