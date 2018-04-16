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

using Ntreev.Crema.Commands;
using Ntreev.Crema.Commands.Consoles;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.Commands.Consoles.TypeTemplate
{
    [Export(typeof(ITemplateCommand))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class SaveCommand : TemplateCommandBase
    {
        public SaveCommand()
            : base("save")
        {

        }

        public Action CloseAction
        {
            get; set;
        }

        protected override void OnExecute()
        {
            var terminal = new Terminal();
            var key = terminal.ReadKey("save template edit. do you proceed?(Y/N)", ConsoleKey.Y, ConsoleKey.N);
            if (key == ConsoleKey.Y)
            {
                this.Template.Dispatcher.Invoke(() => this.Template.EndEdit(this.Authentication));
                this.CloseAction?.Invoke();
            }
        }

        private ITypeTemplate Template { get { return this.CommandContext.Template; } }

        private Authentication Authentication { get { return this.CommandContext.Authentication; } }
    }
}
