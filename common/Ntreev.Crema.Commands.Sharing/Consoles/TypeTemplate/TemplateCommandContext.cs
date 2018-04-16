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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Crema.Commands.Consoles.TypeTemplate
{
    class TemplateCommandContext : CommandContextBase
    {
        private readonly Authentication authentication;
        private readonly ITypeTemplate template;

        public TemplateCommandContext(Authentication authentication, ITypeTemplate template, IEnumerable<ITemplateCommand> commands) 
            : base(commands.Select(item => item.Command), Enumerable.Empty<ICommandProvider>())
        {
            this.authentication = authentication;
            this.template = template;

            foreach (var item in commands)
            {
                if (item is TemplateCommandBase command)
                {
                    command.CommandContext = this;
                }
                //else if (item is ConsoleCommandMethodBase commandMethod)
                //{
                //    commandMethod.CommandContext = this;
                //}
            }
        }

        public Authentication Authentication
        {
            get { return this.authentication; }
        }

        public ITypeTemplate Template
        {
            get { return this.template; }
        }
    }
}
