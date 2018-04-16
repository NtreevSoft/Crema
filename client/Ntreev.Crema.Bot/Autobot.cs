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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Services;
using System.Security;
using System.Reflection;
using Ntreev.Library;

namespace Ntreev.Crema.Bot
{
    class Autobot : AutobotBase
    {
        private readonly AutobotService service;
        private readonly string address;
        private AutobotCremaBootstrapper app = new AutobotCremaBootstrapper();
        private ICremaHost cremaHost;
        private Guid token;

        public Autobot(AutobotService service, string address, string autobotID)
            : base(autobotID)
        {
            this.service = service;
            this.address = address;
        }

        public override ICremaHost CremaHost => this.cremaHost;

        public override AutobotServiceBase Service => this.service;

        protected override Authentication OnLogin()
        {
            this.cremaHost = app.GetService(typeof(ICremaHost)) as ICremaHost;
            this.token = this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Open(this.address, this.AutobotID, StringUtility.ToSecureString("1111")));
            var autheticator = app.GetService(typeof(Authenticator)) as Authenticator;
            return autheticator;
        }

        protected override void OnLogout(Authentication authentication)
        {
            this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Close(this.token));
        }

        protected override void OnDisposed(EventArgs e)
        {
            base.OnDisposed(e);

            if (this.cremaHost != null && this.cremaHost.IsOpened == true)
            {
                this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Close(this.token));
            }
            this.token = Guid.Empty;
            this.app?.Dispose();
            this.app = null;
        }

        #region classes

        class AutobotCremaBootstrapper : CremaBootstrapper
        {
            public override IEnumerable<Tuple<Type, object>> GetParts()
            {
                foreach (var item in base.GetParts())
                {
                    yield return item;
                }
                var autheticator = new Authenticator();
                yield return new Tuple<Type, object>(typeof(IPlugin), autheticator);
                yield return new Tuple<Type, object>(typeof(Authenticator), autheticator);
            }

            public override IEnumerable<Assembly> GetAssemblies()
            {
                var itemList = base.GetAssemblies().ToList();

                return itemList;
            }
        }

        class Authenticator : AuthenticatorBase
        {
            
        }

        #endregion
    }
}
