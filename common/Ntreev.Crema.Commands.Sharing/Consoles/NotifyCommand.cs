﻿//Released under the MIT License.
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
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Commands.Consoles
{
    [Export(typeof(IConsoleCommand))]
    [ResourceDescription("Resources", IsShared = true)]
    class NotifyCommand : ConsoleCommandBase
    {
        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        public NotifyCommand()
            : base("notify")
        {
            this.Message = string.Empty;
        }

        [CommandProperty(IsRequired = true)]
        public string Message
        {
            get; set;
        }

        public override bool IsEnabled => this.CommandContext.IsOnline;

        public override string[] GetCompletions(CommandCompletionContext completionContext)
        {
            return this.UserContext.Dispatcher.Invoke(() =>
            {
                var query = from item in this.UserContext.Users
                            select item.ID;
                return query.ToArray();
            });
        }

        protected override void OnExecute()
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            this.UserContext.Dispatcher.Invoke(() => this.UserContext.NotifyMessage(authentication, this.Message));
        }

        private ICremaHost CremaHost => this.cremaHost.Value;

        private IUserContext UserContext => this.CremaHost.GetService(typeof(IUserContext)) as IUserContext;
    }
}
