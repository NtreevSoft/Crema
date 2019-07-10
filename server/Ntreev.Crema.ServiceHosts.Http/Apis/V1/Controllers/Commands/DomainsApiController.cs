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
        [Route("list")]
        [AllowAnonymous]
        public string[] GetDomainList()
        {
            return domainContext.Dispatcher.Invoke(() =>
            {
                return domainContext.Domains.Select(item => $"{item.ID}").ToArray();
            });
        }

        [HttpGet]
        [Route("{domainId}")]
        [AllowAnonymous]
        public IDictionary<string, object> GetDomainInfo(string domainId)
        {
            var domain = this.GetDomain(domainId);
            return domain.Dispatcher.Invoke(() =>
            {
                var domainInfo = domain.DomainInfo;
                var props = new Dictionary<string, object>
                {
                    {nameof(domainInfo.DomainID), $"{domainInfo.DomainID}"},
                    {nameof(domainInfo.DataBaseID), $"{domainInfo.DataBaseID}"},
                    {nameof(domainInfo.ItemPath), domainInfo.ItemPath},
                    {nameof(domainInfo.ItemType), $"{domainInfo.ItemType}"},
                    {nameof(domainInfo.DomainType), $"{domainInfo.DomainType}"},
                    {nameof(domainInfo.CategoryPath), $"{domainInfo.CategoryPath}"},
                    {CremaSchema.Creator, domainInfo.CreationInfo.ID},
                    {CremaSchema.CreatedDateTime, domainInfo.CreationInfo.DateTime},
                    {CremaSchema.Modifier, domainInfo.ModificationInfo.ID},
                    {CremaSchema.ModifiedDateTime, domainInfo.ModificationInfo.DateTime}
                };
                return props;
            });
        }

        [HttpGet]
        [Route("{domainId}/delete")]
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
                return domain.Users.Select(item => item.ID).ToArray();
            });
        }

        [HttpGet]
        [Route("{domainId}/users/{userId}/contains")]
        [AllowAnonymous]
        public ContainsDomainUserResponse ContainsDomainUser(string domainId, string userId)
        {
            var domain = this.GetDomain(domainId);
            var contains = domain.Dispatcher.Invoke(() => domain.Users.Contains(userId));
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
