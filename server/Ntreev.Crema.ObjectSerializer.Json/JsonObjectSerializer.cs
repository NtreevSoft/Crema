//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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

        public object Deserialize(string itemPath, Type type, ObjectSerializerSettings settings)
        {
            throw new NotImplementedException();
        }

        public string[] Serialize(string itemPath, object obj, ObjectSerializerSettings settings)
        {
            var filename = itemPath + extensions;
            var contents = JObject.FromObject(obj).ToString();
            File.WriteAllText(filename, contents, Encoding.UTF8);
            return new string[] { filename };
        }

        public string[] GetPath(string itemPath, Type type, ObjectSerializerSettings settings)
        {
            var filename = itemPath + extensions;
            return new string[] { filename };
        }

        public string[] GetItemPaths(string path, Type type, ObjectSerializerSettings settings)
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

        public string[] GetReferencedPath(string itemPath, Type type, ObjectSerializerSettings settings)
        {
            throw new NotImplementedException();
        }

        public void Validate(string itemPath, Type type, ObjectSerializerSettings settings)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string itemPath, Type type, ObjectSerializerSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
