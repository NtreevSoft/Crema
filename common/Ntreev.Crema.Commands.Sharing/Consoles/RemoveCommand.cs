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

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;

namespace Ntreev.Crema.Commands.Consoles
{
    [Export(typeof(IConsoleCommand))]
    [ResourceDescription("Resources", IsShared = true)]
    class RemoveCommand : ConsoleCommandBase
    {
        public RemoveCommand()
            : base("rm")
        {

        }

        public override string[] GetCompletions(CommandCompletionContext completionContext)
        {
            if (completionContext.MemberDescriptor == null || completionContext.MemberDescriptor.DescriptorName == nameof(Path) == true)
                return this.CommandContext.GetCompletion(completionContext.Find, true);
            return base.GetCompletions(completionContext);
        }

        [CommandProperty(IsRequired = true)]
        public string Path
        {
            get; set;
        }

        [CommandProperty("dir", 'd')]
        public bool IsDirectory
        {
            get; set;
        }

        public override bool IsEnabled => this.CommandContext.IsOnline;

        protected override void OnExecute()
        {
            var path = this.CommandContext.GetAbsolutePath(this.Path);
            this.Remove(path);
        }

        private void Remove(string path)
        {
            var drive = this.CommandContext.GetDrive(path);
            var absolutePath = this.CommandContext.GetAbsolutePath(path);
            if (drive.GetPaths().Contains(absolutePath) == false)
                throw new ItemNotFoundException(path);
            if (this.CommandContext.ConfirmToDelete() == false)
                return;
            var authentication = this.CommandContext.GetAuthentication(this);
            drive.Delete(authentication, absolutePath);
        }
    }
}
