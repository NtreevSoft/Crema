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
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ntreev.Crema.Javascript.Consoles
{
    [Export(typeof(IConsoleCommand))]
    [ResourceDescription("Resources", IsShared = true)]
    class RunCommand : ConsoleCommandBase
    {
        [Import]
        private Lazy<ScriptContext> scriptContext = null;

        public RunCommand()
            : base("run")
        {

        }

        [CommandProperty(IsRequired = true)]
        [CommandPropertyTrigger(nameof(Filename), "")]
        [CommandPropertyTrigger(nameof(List), false)]
        [DefaultValue("")]
        public string Scripts
        {
            get; set;
        }

        [CommandProperty()]
        [DefaultValue("")]
        [CommandPropertyTrigger(nameof(Scripts), "")]
        [CommandPropertyTrigger(nameof(List), false)]
        public string Filename
        {
            get; set;
        }

        [CommandProperty("list", 'l')]
        [CommandPropertyTrigger(nameof(Scripts), "")]
        [CommandPropertyTrigger(nameof(Filename), "")]
        [DefaultValue(false)]
        public bool List
        {
            get; set;
        }

        [CommandProperty("async")]
        [CommandPropertyTrigger(nameof(List), false)]
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
            if (this.List == true)
            {
                this.Out.Write(this.ScriptContext.GenerateDeclaration(ScriptContextBase.GetArgumentTypes(this.Arguments)));
            }
            else
            {
                var oldPath = Directory.GetCurrentDirectory();
                try
                {
                    DirectoryUtility.Prepare(this.CommandContext.BaseDirectory);
                    Directory.SetCurrentDirectory(this.CommandContext.BaseDirectory);
                    if (this.Filename != string.Empty)
                    {
                        this.Scripts = File.ReadAllText(this.Filename);
                    }

                    var authentication = this.CommandContext.GetAuthenticationInternal(this);
                    if (this.IsAsync == false)
                        this.ScriptContext.RunInternal(this.Scripts, authentication);
                    else
                        this.ScriptContext.RunAsyncInternal(this.Scripts, authentication);
                }
                finally
                {
                    Directory.SetCurrentDirectory(oldPath);
                }
            }
        }

        private ScriptContext ScriptContext => this.scriptContext.Value;
    }
}