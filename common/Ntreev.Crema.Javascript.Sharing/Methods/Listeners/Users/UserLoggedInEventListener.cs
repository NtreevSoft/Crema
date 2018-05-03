using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.Listeners.Users
{
    [Export(typeof(CremaEventListenerBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class UserLoggedInEventListener : CremaEventListenerBase
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public UserLoggedInEventListener(ICremaHost cremaHost)
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
