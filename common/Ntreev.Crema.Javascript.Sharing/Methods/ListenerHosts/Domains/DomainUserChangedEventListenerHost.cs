using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.Domains
{
    [Export(typeof(CremaEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class DomainUserChangedEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DomainUserChangedEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.DomainUserChanged)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDomainContext)) is IDomainContext domainContext)
            {
                domainContext.Dispatcher.Invoke(() => domainContext.Domains.DomainUserChanged += Domains_DomainUserChanged);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDomainContext)) is IDomainContext domainContext)
            {
                domainContext.Dispatcher.Invoke(() => domainContext.Domains.DomainUserChanged -= Domains_DomainUserChanged);
            }
        }

        private void Domains_DomainUserChanged(object sender, DomainUserEventArgs e)
        {
            this.Invoke(null);
        }
    }
}
