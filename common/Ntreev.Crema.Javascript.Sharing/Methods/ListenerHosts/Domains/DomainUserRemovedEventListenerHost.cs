using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.Domains
{
    [Export(typeof(CremaEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class DomainUserRemovedEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DomainUserRemovedEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.DomainUserRemoved)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDomainContext)) is IDomainContext domainContext)
            {
                domainContext.Dispatcher.Invoke(() => domainContext.Domains.DomainUserRemoved += Domains_DomainUserRemoved);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDomainContext)) is IDomainContext domainContext)
            {
                domainContext.Dispatcher.Invoke(() => domainContext.Domains.DomainUserRemoved -= Domains_DomainUserRemoved);
            }
        }

        private void Domains_DomainUserRemoved(object sender, DomainUserRemovedEventArgs e)
        {
            this.Invoke(null);
        }
    }
}
