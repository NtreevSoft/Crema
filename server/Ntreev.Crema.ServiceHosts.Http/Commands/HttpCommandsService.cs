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
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Ntreev.Crema.ServiceHosts.Http.Extensions;
using Ntreev.Crema.ServiceHosts.Http.Responses.Commands;
using Ntreev.Crema.ServiceHosts.Http.Users;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;

namespace Ntreev.Crema.ServiceHosts.Http.Commands
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    class HttpCommandsService : CremaServiceItemBase, IHttpCommandsService, ICremaServiceItem
    {
        private readonly ICremaHost cremaHost;
        private readonly CremaService cremaService;
        private readonly ILogService logService;
        private readonly IUserContext userContext;

        public HttpCommandsService(ICremaHost cremaHost, CremaService cremaService)
            : base(cremaHost.GetService(typeof(ILogService)) as ILogService)
        {
            this.cremaHost = cremaHost;
            this.cremaService = cremaService;
            this.logService = cremaHost.GetService(typeof(ILogService)) as ILogService;
            this.userContext = cremaHost.GetService(typeof(IUserContext)) as IUserContext;
            this.logService.Debug($"{nameof(HttpCommandsService)} Constructor");
        }

        protected override void OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo)
        {
        }

        public void Abort(bool disconnect)
        {
            CremaService.Dispatcher.Invoke(() =>
            {
                if (disconnect == false)
                {
                    try
                    {
                        this.Channel?.Close(TimeSpan.FromSeconds(10));
                    }
                    catch
                    {
                        this.Channel?.Abort();
                    }
                }
                else
                {
                    this.Channel?.Abort();
                }
            });
        }

        public Message Login(string userId, string password)
        {
            var authentication = this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Login(userId, password.ToSecureString()));
            return new LoginResponse
            {
                Token = authentication.Token.ToString()
            }.ToJsonMessage();
        }

        public void Logout(string token)
        {
            var authenticationToken = Guid.Parse(token);
            var authentication = this.cremaHost.Dispatcher.Invoke(() =>
            {
                return this.userContext.Dispatcher.Invoke(() => this.userContext.Authenticate(authenticationToken));
            });
            this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Logout(authentication));
        }
    }
}
