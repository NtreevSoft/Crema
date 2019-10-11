using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.Users
{
    [Export(typeof(CremaEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class MessageReceivedEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public MessageReceivedEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.MessageReceived)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() =>
                {
                    userContext.MessageReceived += UserContext_UsersMessageReceived;
                    userContext.MessageReceived2 += UserContext_UsersMessageReceived2;
                });
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() =>
                {
                    userContext.MessageReceived -= UserContext_UsersMessageReceived;
                    userContext.MessageReceived2 -= UserContext_UsersMessageReceived2;
                });
            }
        }

        private void UserContext_UsersMessageReceived(object sender, MessageEventArgs e)
        {
            this.Invoke(null);
        }

        private void UserContext_UsersMessageReceived2(object sender, MessageEventArgs2 e)
        {
            this.Invoke(null);
        }
    }
}
