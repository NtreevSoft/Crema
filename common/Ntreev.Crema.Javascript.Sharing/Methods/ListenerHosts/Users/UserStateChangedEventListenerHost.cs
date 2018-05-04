using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.Users
{
    [Export(typeof(CremaEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class UserStateChangedEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public UserStateChangedEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.UserStateChanged)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.Users.UsersStateChanged += Users_UsersStateChanged);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.Users.UsersStateChanged -= Users_UsersStateChanged);
            }
        }

        private void Users_UsersStateChanged(object sender, ItemsEventArgs<IUser> e)
        {
            this.Invoke(null);
        }
    }
}
