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

using Ntreev.Crema.Commands.Consoles;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.ConsoleHost.Commands.Consoles
{
    [Export(typeof(ConsoleCommandContext))]
    public class ConsoleCommandContext : ConsoleCommandContextBase
    {
        [Import]
        private Lazy<ICremaHost> cremaHost = null;
        [Import]
        private Lazy<CremaApplication> cremaApp = null;
        private Authentication authentication;

        [ImportingConstructor]
        public ConsoleCommandContext(ICremaHost cremaHost,
            [ImportMany]IEnumerable<IConsoleDrive> rootItems,
            [ImportMany]IEnumerable<IConsoleCommand> commands,
            [ImportMany]IEnumerable<IConsoleCommandProvider> commandProviders)
            : base(rootItems, commands, commandProviders)
        {
            this.BaseDirectory = DirectoryUtility.Prepare(cremaHost.WorkingPath, "console");
        }

        public void Login(string userID, SecureString password)
        {
            if (this.authentication != null)
                throw new Exception("이미 로그인되어 있습니다.");
            this.authentication = this.CremaHost.Login(userID, password);
            this.authentication.Expired += (s, e) => this.authentication = null;
            this.Initialize(authentication);
        }

#if DEBUG
        public void Login(string userID, string password)
        {
            var secureString = new SecureString();
            foreach (var item in password)
            {
                secureString.AppendChar(item);
            }
            this.Login(userID, secureString);
        }
#endif

        public void Logout()
        {
            if (this.authentication == null)
                throw new Exception("로그인되어 있지 않습니다.");
            this.CremaHost.Dispatcher.Invoke(() => this.CremaHost.Logout(this.authentication));
            this.authentication = null;
            this.Release();
        }

        public override ICremaHost CremaHost => this.cremaHost.Value;

        public override string Address
        {
            get
            {
                return AddressUtility.GetDisplayAddress($"localhost:{this.CremaApp.Port}");
            }
        }

        private CremaApplication CremaApp => this.cremaApp.Value;
    }
}
