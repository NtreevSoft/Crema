using Newtonsoft.Json.Linq;
using Ntreev.Crema.Services;
using System;
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

        public object Deserialize(Type type, string itemPath, PropertyCollection properties)
        {
            throw new NotImplementedException();
        }

        public string[] Serialize(object obj, string itemPath, PropertyCollection properties)
        {
            var filename = itemPath + extensions;
            var contents = JObject.FromObject(obj).ToString();
            File.WriteAllText(filename, contents, Encoding.UTF8);
            return new string[] { filename };
        }

        public string[] VerifyPath(Type type, string itemPath, PropertyCollection properties)
        {
            var filename = itemPath + extensions;
            return new string[] { filename };
        }

        public string[] GetItemPaths(string path, Type type, PropertyCollection properties)
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
    }
}
