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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Ntreev.Crema.Commands.Consoles.Properties;
using Ntreev.Library;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Commands.Consoles.Serializations
{
    class JsonEditorHost : IDisposable
    {
        private const string schemaKey = "$schema";
        private const string jsonExtension = ".json";
        private const string schemaExtension = ".schema.json";
        private readonly string schemaPath;
        private readonly string jsonPath;
        private readonly Type type;
        private readonly JSchema schema;
        private string hash;

        public JsonEditorHost(object obj)
            : this(obj, null)
        {

        }

        public JsonEditorHost(object obj, JSchema schema)
        {
            var path = PathUtility.GetTempPath(false);
            this.schemaPath = path + schemaExtension;
            this.jsonPath = path + jsonExtension;
            this.type = obj.GetType();
            this.schema = schema ?? JsonSchemaUtility.GetSchema(this.type);
            this.schema = JSchema.Parse(this.schema.ToString());
            this.schema.Properties.Add(schemaKey, JsonSchemaUtility.GetSchema(typeof(string)));

            this.WriteSchemaText();
            this.WriteText(obj);

            this.hash = HashUtility.GetHashValueFromFile(jsonPath);
        }

        public static bool TryEdit<T>(ref T obj)
        {
            return TryEdit<T>(ref obj, null);
        }

        public static bool TryEdit<T>(ref T obj, JSchema schema)
        {
            using (var editor = new JsonEditorHost(obj, schema))
            {
                if (editor.Execute() == false)
                    return false;

                obj = editor.Read<T>();
                return true;
            }
        }

        public bool Execute()
        {
            try
            {
                TextEditorHost.Execute(this.jsonPath);
            }
            catch (Exception e)
            {
                throw new Exception($"edit cancelled.", e);
            }
            return this.IsModified;
        }

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() => this.Execute());
        }

        public T Read<T>()
        {
            var serializer = new JsonSerializer()
            {
                Formatting = Formatting.Indented,
            };

            using (var stream = File.OpenText(this.jsonPath))
            using (var reader = new JsonTextReader(stream))
            using (var validatingReader = new JSchemaValidatingReader(reader))
            {
                var messages = new List<string>();
                validatingReader.Schema = this.schema;
                validatingReader.ValidationEventHandler += (o, a) => messages.Add(a.Message);
                var obj = serializer.Deserialize<T>(validatingReader);

                if (messages.Any() == true)
                {
                    throw new JsonSerializationException(string.Join(Environment.NewLine, messages));
                }

                return obj;
            }
        }

        public string Filename
        {
            get { return this.jsonPath; }
        }

        public bool IsModified
        {
            get
            {
                return this.hash != HashUtility.GetHashValueFromFile(jsonPath);
            }
        }

        public string ReadText()
        {
            return File.ReadAllText(this.jsonPath);
        }

        private void WriteSchemaText()
        {
            File.WriteAllText(this.schemaPath, $"{this.schema}", Encoding.UTF8);
        }

        private void WriteText(object obj)
        {
            var jobj2 = new JObject
            {
                { schemaKey, $"{new Uri(this.schemaPath)}" }
            };
            foreach (var item in JObject.FromObject(obj).Properties())
            {
                jobj2.Add(item);
            }
            File.WriteAllText(this.jsonPath, jobj2.ToString(Formatting.Indented) + Environment.NewLine, Encoding.UTF8);
        }

        #region IDisposable

        void IDisposable.Dispose()
        {
            FileUtility.Delete(this.schemaPath);
            FileUtility.Delete(this.jsonPath);
        }

        #endregion
    }
}
