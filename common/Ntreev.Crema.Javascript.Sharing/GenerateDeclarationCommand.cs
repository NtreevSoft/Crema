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
    class GenerateDeclarationCommand : CommandBase
    {
        [Import]
        private Lazy<ScriptContext> scriptContext = null;
#if SERVER
        [Import]
        private CremaBootstrapper bootstrapper = null;
#endif

        [ImportingConstructor]
        public GenerateDeclarationCommand()
            : base("gen-decl")
        {
        }

        [CommandProperty(IsRequired = true)]
        public string Filename
        {
            get; set;
        }

        protected override void OnExecute()
        {
#if SERVER

            var tempPath = PathUtility.GetTempPath(true);
            this.bootstrapper.CreateRepository(tempPath, "crema", true);
            this.bootstrapper.BasePath = tempPath;
            this.bootstrapper.Disposed += (s, e) => DirectoryUtility.Delete(tempPath);
#endif
            CremaLog.Verbose = LogVerbose.Fatal;
            var code = this.ScriptContext.GenerateDeclaration();
            FileUtility.WriteAllText(code, this.Filename);
            Console.WriteLine(new FileInfo(this.Filename).FullName);
        }

        private ScriptContext ScriptContext => this.scriptContext.Value;
    }
}