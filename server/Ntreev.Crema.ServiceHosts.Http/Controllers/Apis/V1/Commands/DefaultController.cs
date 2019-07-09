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
