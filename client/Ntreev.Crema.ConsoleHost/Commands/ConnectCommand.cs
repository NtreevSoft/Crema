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

using Ntreev.Crema.ConsoleHost.Commands.Consoles;
using Ntreev.Crema.Javascript;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ConsoleHost.Commands
{
    [Export(typeof(ICommand))]
    [Export]
    [ResourceDescription]
    class ConnectCommand : CommandBase
    {
        private readonly CremaBootstrapper application;
        private readonly ICremaHost cremaHost;

        [Import]
        private Lazy<ConsoleTerminal> terminal = null;
        [Import]
        private Lazy<ConsoleCommandContext> commandContext = null;

        [ImportingConstructor]
        public ConnectCommand(CremaBootstrapper application, ICremaHost cremaHost)
            : base("connect")
        {
            this.application = application;
            this.cremaHost = cremaHost;
        }

        public void Cancel()
        {
            var terminal = this.application.GetService(typeof(ConsoleTerminal)) as ConsoleTerminal;
            terminal.Cancel();
        }

        [CommandProperty(IsRequired = true)]
        public string Address
        {
            get;
            set;
        }

        [CommandProperty]
#if DEBUG
        [DefaultValue("en-US")]
#else
        [DefaultValue("")]
#endif
        public string Culture
        {
            get; set;
        }

#if DEBUG
        [CommandProperty('l', IsExplicit = true)]
        [DefaultValue("admin:admin")]
        public string LoginAuthentication
        {
            get;
            set;
        }
#endif

        protected override void OnExecute()
        {
            this.application.Culture = this.Culture;
            this.cremaHost.Closed += CremaHost_Closed;
            this.Wait();
        }

        private void CremaHost_Closed(object sender, ClosedEventArgs e)
        {
            if (e.Reason == CloseReason.Shutdown)
            {
                this.Cancel();
            }
        }

        private void Wait()
        {
            this.CommandContext.SetAddress(this.Address);

#if DEBUG
            this.Terminal.Start(this.LoginAuthentication);
#else
            this.Terminal.Start();
#endif
        }

        private ConsoleTerminal Terminal => this.terminal.Value;

        private ConsoleCommandContext CommandContext => this.commandContext.Value;
    }
}