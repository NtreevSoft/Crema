using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Windows.Threading;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.DataBases
{
    [Export(typeof(CremaEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class DataBaseAuthenticationEnteredEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DataBaseAuthenticationEnteredEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.DataBaseAuthenticationEntered)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsAuthenticationEntered += DataBases_ItemsAuthenticationEntered);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsAuthenticationEntered -= DataBases_ItemsAuthenticationEntered);
            }
        }

        private void DataBases_ItemsAuthenticationEntered(object sender, ItemsEventArgs<IDataBase> e)
        {
            foreach (var item in e.Items)
            {
                var props = new Dictionary<string, object>()
                {
                    { "Name", item.Name },
                };
                this.Invoke(props);
            }
        }
    }
}
