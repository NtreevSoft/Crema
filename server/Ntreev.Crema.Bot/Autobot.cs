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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library.Random;
using Ntreev.Library;
using Ntreev.Crema.ServiceModel;
using System.Threading;
using System.Reflection;
using Ntreev.Crema.Services;
using System.Security;

namespace Ntreev.Crema.Bot
{
    class Autobot : AutobotBase
    {
        private readonly ICremaHost cremaHost;
        private readonly SecureString password;
        private readonly AutobotService service;

        public Autobot(ICremaHost cremaHost, AutobotService service, string autobotID, SecureString password)
            : base(autobotID)
        {
            this.cremaHost = cremaHost;
            this.service = service;
            this.password = password;
        }

        public override ICremaHost CremaHost => this.cremaHost;

        public override AutobotServiceBase Service => this.service;

        protected override Authentication OnLogin()
        {
            return this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Login(this.AutobotID, this.password));
        }

        protected override void OnLogout(Authentication authentication)
        {
            this.cremaHost.Dispatcher.Invoke(() =>
            {
                this.cremaHost.Logout(authentication);
            });
        }
    }
}