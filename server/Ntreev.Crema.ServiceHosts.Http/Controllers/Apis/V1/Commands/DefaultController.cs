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
using System.Web.Http;
using Ntreev.Crema.ServiceHosts.Http.Requests.Commands;
using Ntreev.Crema.ServiceHosts.Http.Responses.Commands;
using Ntreev.Crema.Services;
using Ntreev.Library;

namespace Ntreev.Crema.ServiceHosts.Http.Controllers.Apis.V1.Commands
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/v1/commands")]
    public class DefaultController : CremaApiController
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DefaultController(ICremaHost cremaHost) : base(cremaHost)
        {
            this.cremaHost = cremaHost;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public LoginResponse Login([FromBody] LoginRequest request)
        {
            var authentication = this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Login(request.UserId, request.Password.ToSecureString()));
            return new LoginResponse
            {
                Token = authentication.Token.ToString()
            };
        }

        [HttpPost]
        [Route("logout")]
        public void Logout()
        {
            this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Logout(this.Authentication));
        }
    }
}
