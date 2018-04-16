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

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Commands.Consoles;
using Ntreev.Crema.Services;
using Ntreev.Library;

namespace Ntreev.Crema.ConsoleHost.Commands.Consoles
{
    [Export]
    class ConsoleTerminal : ConsoleTerminalBase
    {
        private readonly ConsoleCommandContext commandContext;

        [ImportingConstructor]
        public ConsoleTerminal(ConsoleCommandContext commandContext)
            : base(commandContext)
        {
            this.commandContext = commandContext;
        }

#if DEBUG
        public void Start(string authentication)
        {
            if (authentication != null)
            {
                var ss = StringUtility.Split(authentication, ':');
                
                this.commandContext.Login(this.commandContext.Address, ss[0], ss[1].ToSecureString());
            }
            this.SetPrompt();
            base.Start();
        }
#else
        public new void Start()
        {
            this.SetPrompt();
            base.Start();
        }
#endif

        public new void Cancel()
        {
            if (this.commandContext.IsOnline == true)
                this.commandContext.Logout();
            base.Cancel();
        }
    }
}
