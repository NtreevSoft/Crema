using Newtonsoft.Json.Linq;
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

namespace Ntreev.Crema.ObjectSerializer.Json
{
    [Export(typeof(IObjectSerializer))]
    class JsonObjectSerializer : IObjectSerializer
    {
        private const string extensions = ".json";

        public string Name => "json";

        public object Deserialize(string itemPath, Type type, IDictionary properties)
        {
            throw new NotImplementedException();
        }

        public string[] Serialize(string itemPath, object obj, IDictionary properties)
        {
            var filename = itemPath + extensions;
            var contents = JObject.FromObject(obj).ToString();
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
