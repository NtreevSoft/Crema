using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.EventHandlers.Users
{
    [Export(typeof(EventHandlerBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class UserStateChangedEventHandler : EventHandlerBase
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public UserStateChangedEventHandler(ICremaHost cremaHost)
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
