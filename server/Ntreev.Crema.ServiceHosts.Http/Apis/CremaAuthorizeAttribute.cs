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
using System.Security.Authentication;
using System.Web.Http;
using System.Web.Http.Controllers;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.ServiceHosts.Http.Apis
{
    class CremaAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var container = actionContext.RequestContext.Configuration.DependencyResolver;
            var cremaHost = container.GetService(typeof(ICremaHost)) as ICremaHost ?? throw new NullReferenceException("cremaHost");
            var userContext = container.GetService(typeof(IUserContext)) as IUserContext ?? throw new NullReferenceException("userContext");
            var token = "";

            if (actionContext.Request.Headers.Contains("token"))
            {
                token = actionContext.Request.Headers.GetValues("token").First();
            }
            else if (actionContext.Request.Headers.Contains("Token"))
            {
                token = actionContext.Request.Headers.GetValues("Token").First();
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                var authentication = cremaHost.Dispatcher.Invoke(() =>
                {
                    return userContext.Dispatcher.Invoke(() => userContext.Authenticate(Guid.Parse(token)));
                });

                if (authentication == null) return false;

                if (!string.IsNullOrWhiteSpace(this.Roles))
                {
                    var authority = (Authority)Enum.Parse(typeof(Authority), Roles, true);
                    if (authority > authentication.Authority)
                    {
                        throw new AuthenticationException("You do not have permission.");
                    }
                }

                return true;
            }

            return false;
        }
    }
}
