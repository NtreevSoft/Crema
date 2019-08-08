using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.Users
{
    [Export(typeof(CremaEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class UserAuthenticationKickedEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public UserAuthenticationKickedEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.UserKicked)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.UserAuthenticationsKicked += UserContext_UserAuthenticationsKicked);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.UserAuthenticationsKicked -= UserContext_UserAuthenticationsKicked);
            }
        }

        private void UserContext_UserAuthenticationsKicked(object sender, ItemsEventArgs<IUserAuthentication> e)
        {
            this.Invoke(null);
        }
    }
}
