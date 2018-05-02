using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.EventHandlers.Users
{
    [Export(typeof(EventHandlerBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class UserChangedEventHandler : EventHandlerBase
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public UserChangedEventHandler(ICremaHost cremaHost)
            : base(CremaEvents.UserChanged)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.Users.UsersChanged += Users_UsersChanged);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.Users.UsersChanged -= Users_UsersChanged);
            }
        }

        private void Users_UsersChanged(object sender, ItemsEventArgs<IUser> e)
        {
            this.Invoke(null);
        }
    }
}
