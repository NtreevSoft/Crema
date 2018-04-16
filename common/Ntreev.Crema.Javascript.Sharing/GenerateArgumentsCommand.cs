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

using Ntreev.Crema.Services;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;

namespace Ntreev.Crema.Javascript
{
    [Export(typeof(ICommand))]
    [Export]
    [ResourceDescription]
    class GenerateArgumentsCommand : CommandBase
    {
        [Import]
        private Lazy<ScriptContext> scriptContext = null;

        [ImportingConstructor]
        public GenerateArgumentsCommand()
            : base("gen-args")
        {
        }

        [CommandProperty(IsRequired = true)]
        public string Filename
        {
            get; set;
        }

        [CommandPropertyArray]
        [Description("a=number b=string c=boolean")]
        public string[] Arguments
        {
            get; set;
        }

        protected override void OnExecute()
        {
            CremaLog.Verbose = LogVerbose.Fatal;
            var code = this.ScriptContext.GenerateArguments(this.GetProperties());
            FileUtility.WriteAllText(code, this.Filename);
            Console.WriteLine(new FileInfo(this.Filename).FullName);
        }

        private Dictionary<string, Type> GetProperties()
        {
            var properties = new Dictionary<string, Type>(this.Arguments.Length);
            foreach (var item in this.Arguments)
            {
                if (CommandStringUtility.TryGetKeyValue(item, out var key, out var value) == true)
                {
                    var typeName = value;
                    if (CommandStringUtility.IsWrappedOfQuote(value))
                    {
                        value = CommandStringUtility.TrimQuot(value);
                    }

                    if (value == "number")
                    {
                        properties.Add(key, typeof(decimal));
                    }
                    else if (value == "boolean")
                    {
                        properties.Add(key, typeof(bool));
                    }
                    else if (value == "string")
                    {
                        properties.Add(key, typeof(string));
                    }
                    else
                    {
                        throw new ArgumentException(typeName);
                    }
                }
                else
                {
                    throw new ArgumentException(item);
                }
            }
            return properties;
        }

        private ScriptContext ScriptContext => this.scriptContext.Value;
    }
}