using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Ntreev.Crema.Services;
using Ntreev.Library.Extensions;

namespace Ntreev.Crema.ServiceHosts.Http
{
    public class CremaAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly ICremaHost cremaHost;
        private readonly IUserContext userContext;

        public CremaAuthorizeAttribute(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
            this.userContext = this.cremaHost.GetService<IUserContext>();
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Contains("token"))
            {
                var token = actionContext.Request.Headers.GetValues("token").First();
                var authentication = this.cremaHost.Dispatcher.Invoke(() =>
                {
                    return userContext.Dispatcher.Invoke(() => userContext.Authenticate(Guid.Parse(token)));
                });

                return authentication != null;
            }

            return false;
        }
    }
}
