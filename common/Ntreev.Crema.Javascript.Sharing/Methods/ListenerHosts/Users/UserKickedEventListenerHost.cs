using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.Users
{
    [Export(typeof(CremaEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class UserKickedEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public UserKickedEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.UserKicked)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.UsersKicked += UserContext_UsersKicked);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.UsersKicked -= UserContext_UsersKicked);
            }
        }

        private void UserContext_UsersKicked(object sender, ItemsEventArgs<IUser> e)
        {
            this.Invoke(null);
        }
    }
}
