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

using Ntreev.Crema.Commands.Consoles;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Javascript.Consoles
{
    [Export(typeof(IConsoleCommand))]
    [ResourceDescription("Resources", IsShared = true)]
    class RunFileCommand : ConsoleCommandBase
    {
        [Import]
        private Lazy<ScriptContext> scriptContext = null;

        public RunFileCommand()
            : base("run-file")
        {

        }

        [CommandProperty(IsRequired = true)]
        [DefaultValue("")]
        public string Filename
        {
            get; set;
        }

        [CommandProperty("async")]
        public bool IsAsync
        {
            get; set;
        }

        [CommandPropertyArray]
        public string[] Arguments
        {
            get;
            set;
        }

        protected override void OnExecute()
        {
            var script = File.ReadAllText(this.Filename);
            var authentication = this.CommandContext.GetAuthenticationInternal(this);
            if (this.IsAsync == false)
                this.ScriptContext.RunInternal(script, authentication, this.GetProperties());
            else
                this.ScriptContext.RunAsyncInternal(script, authentication, this.GetProperties());
        }

        private IDictionary<string, object> GetProperties()
        {
            return CommandStringUtility.ArgumentsToDictionary(this.Arguments);
        }

        private ScriptContext ScriptContext => this.scriptContext.Value;
    }
}
