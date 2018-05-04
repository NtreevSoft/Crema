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
    class DataBaseAuthenticationLeftEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DataBaseAuthenticationLeftEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.DataBaseAuthenticationLeft)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsAuthenticationLeft += DataBases_ItemsAuthenticationLeft);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsAuthenticationLeft -= DataBases_ItemsAuthenticationLeft);
            }
        }

        private void DataBases_ItemsAuthenticationLeft(object sender, ItemsEventArgs<IDataBase> e)
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
