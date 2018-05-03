using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.Listeners.Users
{
    [Export(typeof(CremaEventListenerBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class UserLoggedOutEventListener : CremaEventListenerBase
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public UserLoggedOutEventListener(ICremaHost cremaHost)
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

        private void UserContext_UsersLoggedOut(object sender, ItemsEventArgs<IUser> e)
        {
            this.Invoke(null);
        }
    }
}
