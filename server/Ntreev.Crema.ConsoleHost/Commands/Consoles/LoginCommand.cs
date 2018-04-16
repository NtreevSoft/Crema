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

using Ntreev.Library;
using Ntreev.Crema.Services;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Ntreev.Crema.Commands.Consoles;
using System.ComponentModel;

namespace Ntreev.Crema.ConsoleHost.Commands.Consoles
{
    [Export(typeof(IConsoleCommand))]
    [ResourceDescription("Resources", IsShared = true)]
    class LoginCommand : ConsoleCommandBase
    {
        public LoginCommand()
            : base("login")
        {

        }

        [CommandProperty(IsRequired = true)]
        [DefaultValue("")]
        public string UserID
        {
            get; set;
        }

        [CommandProperty(IsRequired = true)]
        [DefaultValue("")]
        public string Password
        {
            get; set;
        }

        public override bool IsEnabled => this.CommandContext.IsOnline == false;

        public new ConsoleCommandContext CommandContext => base.CommandContext as ConsoleCommandContext;

        protected override void OnExecute()
        {
            try
            {
                var userID = this.UserID == string.Empty ? this.CommandContext.ReadString("UserID:") : this.UserID;
                var password = this.Password == string.Empty ? this.CommandContext.ReadSecureString("Password:") : StringUtility.ToSecureString(this.Password);
                this.CommandContext.Login(userID, password);
            }
            catch (OperationCanceledException e)
            {
                throw new Exception("login is cancelled.", e);
            }
        }
    }
}
