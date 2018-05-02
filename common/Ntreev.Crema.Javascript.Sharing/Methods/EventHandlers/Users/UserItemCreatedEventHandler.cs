using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.EventHandlers.Users
{
    [Export(typeof(EventHandlerBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class UserItemCreatedEventHandler : EventHandlerBase
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public UserItemCreatedEventHandler(ICremaHost cremaHost)
            : base(CremaEvents.UserChanged)
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
