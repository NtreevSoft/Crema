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

using Jint;
using Ntreev.Crema.Commands.Consoles;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Javascript.Methods
{
    [Export(typeof(IConsoleCommand))]
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class CallMethod : ConsoleCommandBase, IScriptMethod
    {
        [Import]
        private Lazy<ScriptContext> scriptContext = null;

        public CallMethod()
            : base("call-dummy")
        {

        }

        public Delegate Delegate => new Func<string, int>(this.Call);

        protected override void OnExecute()
        {
            throw new NotImplementedException();
        }

        public override bool IsEnabled => false;

        private int Call(string commandLine)
        {
            var error_message = string.Empty;
            var error_code = 0;
            try
            {
                this.Engine.SetValue("error_code", error_code);
                this.Engine.SetValue("error_message", error_message);
                this.CommandContext.Execute($"{this.CommandContext.Name} {commandLine}");
            }
            catch (TargetInvocationException e)
            {
                error_code = -1;
                if (e.InnerException != null)
                    error_message = e.InnerException.Message;
                else
                    error_message = e.Message;
            }
            catch (Exception e)
            {
                error_code = -1;
                error_message = e.Message;
            }
            this.Engine.SetValue("error_code", error_code);
            this.Engine.SetValue("error_message", error_message);
            if (error_message != string.Empty)
                this.CommandContext.Error.WriteLine(error_message);
            return error_code;
        }

        string IScriptMethod.Name
        {
            get { return "call"; }
        }

        private ScriptContext ScriptContext => this.scriptContext.Value;
    }
}
