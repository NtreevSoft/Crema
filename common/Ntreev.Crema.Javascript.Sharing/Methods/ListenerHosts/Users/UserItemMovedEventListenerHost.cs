using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.Users
{
    [Export(typeof(CremaEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class UserItemMovedEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public UserItemMovedEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.UserItemMoved)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.ItemsMoved += UserContext_ItemsMoved);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.ItemsMoved -= UserContext_ItemsMoved);
            }
        }

        private void UserContext_ItemsMoved(object sender, ItemsMovedEventArgs<IUserItem> e)
        {
            this.Invoke(null);
        }
    }
}
