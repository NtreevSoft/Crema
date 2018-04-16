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
using System.Timers;

namespace Ntreev.Crema.ConsoleHost.Commands.Consoles
{
    [Export(typeof(IConsoleCommand))]
    [ResourceDescription("Resources", IsShared = true)]
    class ShutdownCommand : ConsoleCommandBase
    {
        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        public ShutdownCommand()
            : base("shutdown")
        {

        }

        [CommandProperty(IsRequired = true)]
        [DefaultValue("")]
        public DateTimeValue Time
        {
            get; set;
        }

        [CommandProperty('r')]
        public bool IsRestart
        {
            get; set;
        }

        [CommandProperty]
        public bool NoCache
        {
            get; set;
        }

        [CommandProperty('c')]
        public bool IsCancelled
        {
            get; set;
        }

        [CommandProperty('m')]
        [DefaultValue("")]
        public string Message
        {
            get; set;
        }

        public override bool IsEnabled => this.CommandContext.IsOnline;

        protected override void OnExecute()
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            if (this.IsCancelled == true)
            {
                this.CremaHost.CancelShutdown(authentication);
            }
            else
            {
                this.CremaHost.Shutdown(authentication, this.Time.Milliseconds, this.ShutdownType, this.Message);
            }
        }

        private ICremaHost CremaHost => this.cremaHost.Value;

        private ShutdownType ShutdownType
        {
            get
            {
                var shutdownType = ShutdownType.None;

                if (this.IsRestart == true)
                    shutdownType |= ShutdownType.Restart;
                if (this.NoCache == true)
                    shutdownType |= ShutdownType.NoCache;

                return shutdownType;
            }
        }
    }
}
