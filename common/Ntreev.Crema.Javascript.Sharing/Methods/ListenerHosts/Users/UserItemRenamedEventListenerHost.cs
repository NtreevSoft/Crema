using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.Users
{
    [Export(typeof(CremaEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class UserItemRenamedEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public UserItemRenamedEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.UserItemRenamed)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.ItemsRenamed += UserContext_ItemsRenamed);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() => userContext.ItemsRenamed -= UserContext_ItemsRenamed);
            }
        }

        private void UserContext_ItemsRenamed(object sender, ItemsRenamedEventArgs<IUserItem> e)
        {
            this.Invoke(null);
        }
    }
}
