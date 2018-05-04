using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.Domains
{
    [Export(typeof(CremaEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class DomainStateChangedEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DomainStateChangedEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.DomainStateChanged)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDomainContext)) is IDomainContext domainContext)
            {
                domainContext.Dispatcher.Invoke(() => domainContext.Domains.DomainStateChanged += Domains_DomainStateChanged);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDomainContext)) is IDomainContext domainContext)
            {
                domainContext.Dispatcher.Invoke(() => domainContext.Domains.DomainStateChanged -= Domains_DomainStateChanged);
            }
        }

        private void Domains_DomainStateChanged(object sender, DomainEventArgs e)
        {
            this.Invoke(null);
        }
    }
}
