using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.ServiceHosts.Http.Controllers
{
    public class CremaApiController : ApiController
    {
        private readonly ICremaHost cremaHost;
        protected readonly IUserContext userContext;
        protected readonly ILogService logService;

        public CremaApiController(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
            this.logService = cremaHost.GetService(typeof(ILogService)) as ILogService;
            this.userContext = cremaHost.GetService(typeof(IUserContext)) as IUserContext;
        }

        protected Authentication Authentication
        {
            get
            {
                return this.cremaHost.Dispatcher.Invoke(() =>
                {
                    var token = Request.Headers.GetValues("token").First();
                    return this.userContext.Dispatcher.Invoke(() => this.userContext.Authenticate(Guid.Parse(token)));
                });
            }
        }
    }
}
