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
using System.Web.Http;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.Responses.Commands;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.Controllers.Commands
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/v1/commands/domains")]
    public class DomainsApiController : CremaApiController
    {
        private readonly ICremaHost cremaHost;
        private readonly IDomainContext domainContext;

        [ImportingConstructor]
        public DomainsApiController(ICremaHost cremaHost) : base(cremaHost)
        {
            this.cremaHost = cremaHost;
            this.domainContext = cremaHost.GetService(typeof(IDomainContext)) as IDomainContext;
        }

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public string[] GetDomainList()
        {
            return domainContext.Dispatcher.Invoke(() =>
            {
                return domainContext.Domains.Select(item => $"{item.ID}").ToArray();
            });
        }

        [HttpGet]
        [Route("{domainId}/info")]
        [AllowAnonymous]
        public GetDomainInfoResponse GetDomainInfo(string domainId)
        {
            var domain = this.GetDomain(domainId);
            return domain.Dispatcher.Invoke(() =>
            {
                var domainInfo = domain.DomainInfo;
                return GetDomainInfoResponse.ConvertFrom(domainInfo);
            });
        }

        [HttpDelete]
        [Route("{domainId}")]
        public void DeleteDomain(string domainId, bool isCancel = false)
        {
            var domain = this.GetDomain(domainId);
            domain.Dispatcher.Invoke(() => domain.Delete(this.Authentication, isCancel));
        }

        [HttpGet]
        [Route("{domainId}/contains")]
        [AllowAnonymous]
        public ContainsDomainResponse ContainsDomain(string domainId)
        {
            var contains = false;
            if (Guid.TryParse(domainId, out var guid) == true)
            {
                contains = domainContext.Dispatcher.Invoke(() => domainContext.Domains.Contains(guid));
            }

            return new ContainsDomainResponse
            {
                Contains = contains
            };
        }

        [HttpGet]
        [Route("{domainId}/users")]
        [AllowAnonymous]
        public string[] GetDomainUserList(string domainId)
        {
            var domain = this.GetDomain(domainId);
            return domain.Dispatcher.Invoke(() =>
            {
                return domain.Users.Select(item => item.DomainUserInfo.UserID).Distinct().ToArray();
            });
        }

        [HttpGet]
        [Route("{domainId}/users/{userId}/contains")]
        [AllowAnonymous]
        public ContainsDomainUserResponse ContainsDomainUser(string domainId, string userId)
        {
            var domain = this.GetDomain(domainId);
            var contains = domain.Dispatcher.Invoke(() =>
            {
                var users = domain.Users.Select(o => o.DomainUserInfo.UserID);
                return users.Contains(userId);
            });
            return new ContainsDomainUserResponse
            {
                Contains = contains
            };
        }

        private IDomain GetDomain(string domainId)
        {
            if (domainId == null)
                throw new ArgumentNullException(nameof(domainId));

            if (!Guid.TryParse(domainId, out var guid))
                throw new ArgumentException("Invalid domainId", nameof(domainId));

            var domain = domainContext.Dispatcher.Invoke(() => domainContext.Domains[guid]);
            if (domain == null)
                throw new DomainNotFoundException(guid);

            return domain;
        }
    }
}
