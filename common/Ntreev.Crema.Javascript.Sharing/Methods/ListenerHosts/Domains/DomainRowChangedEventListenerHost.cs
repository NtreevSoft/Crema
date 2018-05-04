using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.Domains
{
    [Export(typeof(CremaEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class DomainRowChangedEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DomainRowChangedEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.DomainRowChanged)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDomainContext)) is IDomainContext domainContext)
            {
                domainContext.Dispatcher.Invoke(() => domainContext.Domains.DomainRowChanged += Domains_DomainRowChanged);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDomainContext)) is IDomainContext domainContext)
            {
                domainContext.Dispatcher.Invoke(() => domainContext.Domains.DomainRowChanged -= Domains_DomainRowChanged);
            }
        }

        private void Domains_DomainRowChanged(object sender, DomainRowEventArgs e)
        {
            this.Invoke(null);
        }
    }
}
