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
using System.ComponentModel.Composition;
using System.Web.Http;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.Requests.Commands;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.Responses.Commands;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.Controllers.Commands
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/v1/commands")]
    public class DefaultApiController : CremaApiController
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DefaultApiController(ICremaHost cremaHost) : base(cremaHost)
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
        [Route("login/databases/{databaseName}/load-and-enter")]
        [AllowAnonymous]
        public LoginAndLoadEnterDataBaseResponse LoginAndLoadEnterDataBase(string databaseName, [FromBody] LoginRequest request)
        {
            var authentication = this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Login(request.UserId, request.Password.ToSecureString()));
            var database = this.GetDataBase(databaseName);
            var dataBaseStatus = database.Dispatcher.Invoke(() =>
            {
                if (database.IsLoaded)
                {
                    return LoadDataBaseStatus.AlreadyLoaded;
                }

                database.Load(authentication);
                return LoadDataBaseStatus.Loaded;
            });

            database.Dispatcher.Invoke(() => database.Enter(authentication));

            return new LoginAndLoadEnterDataBaseResponse
            {
                Token = authentication.Token.ToString(),
                DataBaseStatus = dataBaseStatus
            };
        }


        [HttpPost]
        [Route("logout")]
        public void Logout()
        {
            this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Logout(this.Authentication));
        }

        [HttpPost]
        [Route("logout/databases/{databaseName}/leave")]
        public void LogoutAndLeaveDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            database.Dispatcher.Invoke(() => database.Leave(this.Authentication));

            this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Logout(this.Authentication));
        }

        private IDataBase GetDataBase(string databaseName)
        {
            if (databaseName == null)
                throw new ArgumentNullException(nameof(databaseName));
            return this.cremaHost.Dispatcher.Invoke(() =>
            {
                if (this.cremaHost.DataBases.Contains(databaseName) == false)
                    throw new DataBaseNotFoundException(databaseName);
                return this.cremaHost.DataBases[databaseName];
            });
        }
    }
}
