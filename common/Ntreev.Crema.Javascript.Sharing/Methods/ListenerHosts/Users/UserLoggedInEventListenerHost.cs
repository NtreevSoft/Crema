using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.Users
{
    [Export(typeof(CremaEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class UserLoggedInEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public UserLoggedInEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.UserLoggedIn)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.UsersLoggedIn += UserContext_UsersLoggedIn);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.UsersLoggedIn -= UserContext_UsersLoggedIn);
            }
        }

        private void UserContext_UsersLoggedIn(object sender, ItemsEventArgs<IUser> e)
        {
            this.Invoke(null);
        }
    }
}
