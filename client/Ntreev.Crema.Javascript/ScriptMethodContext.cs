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
using Ntreev.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Javascript
{
    class ScriptMethodContext : Dictionary<object, object>, IScriptMethodContext
    {
        private readonly ScriptContext scriptContext;
        private readonly ICremaHost cremaHost;
        private Authentication authentication;
        private string token;

        public ScriptMethodContext(ScriptContext scriptContext, ICremaHost cremaHost, Authentication authentication)
        {
            this.scriptContext = scriptContext;
            this.cremaHost = cremaHost;
            this.authentication = authentication;
            if (this.authentication != null)
                this.token = $"{Guid.NewGuid()}";
        }

        public string Login(string address, string userID, string password)
        {
            if (this.token != null)
                throw new ArgumentException("이미 로그인되어 있습니다.");
            var token = this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Open(address, userID, StringUtility.ToSecureString(password)));
            var authenticator = this.cremaHost.GetService(typeof(Authenticator)) as Authenticator;
            this.Initialize(authenticator);
            this.token = $"{token}";
            return this.token;
        }

        public string Login(string address, string dataBase, string userID, string password)
        {
            if (this.token != null)
                throw new ArgumentException("이미 로그인되어 있습니다.");
            var token = this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Open(address, dataBase, userID, StringUtility.ToSecureString(password)));
            var authenticator = this.cremaHost.GetService(typeof(Authenticator)) as Authenticator;
            this.Initialize(authenticator);
            this.token = $"{token}";
            return this.token;
        }

        public void Logout(string token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));
            if (this.token != token)
                throw new ArgumentException("잘못된 토큰입니다.", nameof(token));
            this.cremaHost.Close(Guid.Parse(token));
            this.Release();
            this.token = null;
        }

        public Authentication GetAuthentication(IScriptMethod scriptMethod)
        {
            return this.authentication;
        }

        public IDictionary<object, object> Properties => this;

        public TextWriter Out => this.scriptContext.Out;

        protected void Initialize(Authentication authentication)
        {
            this.authentication = authentication;
        }

        protected void Release()
        {
            //#if SERVER
            //            if (this.authentication != null)
            //            {
            //                this.authentication.Expired -= Authentication_Expired;
            //            }
            //#endif
            this.authentication = null;
        }
    }
}
