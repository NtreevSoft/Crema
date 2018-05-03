using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.Listeners.Domains
{
    [Export(typeof(CremaEventListenerBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class DomainDeletedEventListener : CremaEventListenerBase
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DomainDeletedEventListener(ICremaHost cremaHost)
            : base(CremaEvents.DomainDeleted)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDomainContext)) is IDomainContext domainContext)
            {
                domainContext.Dispatcher.Invoke(() => domainContext.Domains.DomainDeleted += Domains_DomainDeleted);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDomainContext)) is IDomainContext domainContext)
            {
                domainContext.Dispatcher.Invoke(() => domainContext.Domains.DomainDeleted -= Domains_DomainDeleted);
            }
        }

        private void Domains_DomainDeleted(object sender, DomainEventArgs e)
        {
            this.Invoke(null);
        }
    }
}
