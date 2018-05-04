using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.Users
{
    [Export(typeof(CremaEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class UserItemCreatedEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public UserItemCreatedEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.UserItemCreated)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.ItemsCreated += UserContext_ItemsCreated);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.ItemsCreated -= UserContext_ItemsCreated);
            }
        }

        private void UserContext_ItemsCreated(object sender, ItemsCreatedEventArgs<IUserItem> e)
        {
            this.Invoke(null);
        }
    }
}
