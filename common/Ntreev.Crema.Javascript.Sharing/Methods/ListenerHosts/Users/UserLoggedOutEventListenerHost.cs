using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.Users
{
    [Export(typeof(CremaEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class UserLoggedOutEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public UserLoggedOutEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.UserLoggedOut)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.UsersLoggedOut += UserContext_UsersLoggedOut);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.UsersLoggedOut -= UserContext_UsersLoggedOut);
            }
        }

        private void UserContext_UsersLoggedOut(object sender, ItemsEventArgs<AuthenticationInfo> e)
        {
            this.Invoke(null);
        }
    }
}
