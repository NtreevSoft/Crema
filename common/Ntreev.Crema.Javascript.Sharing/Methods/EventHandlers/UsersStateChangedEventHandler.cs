using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.EventHandlers
{
    class UsersStateChangedEventHandler : EventHandlerBase
    {
        private readonly IUserContext userContext;
        protected UsersStateChangedEventHandler(IUserContext userContext, Action<IDictionary<string, object>> action)
            : base(CremaEvents.UserStateChanged, action)
        {
            this.userContext = userContext;
        }

        public override void Subscribe()
        {
            this.userContext.Users.UsersStateChanged += Users_UsersStateChanged;
        }

        private void Users_UsersStateChanged(object sender, ItemsEventArgs<IUser> e)
        {
            this.Invoke(null);
        }

        public override void Unsubscribe()
        {
            this.userContext.Users.UsersStateChanged -= Users_UsersStateChanged;
        }
    }
}
