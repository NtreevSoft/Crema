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
    class DataBaseCreatedEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DataBaseCreatedEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.DataBaseCreated)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsCreated += DataBases_ItemsCreated);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsCreated -= DataBases_ItemsCreated);
            }
        }

        private void DataBases_ItemsCreated(object sender, ItemsCreatedEventArgs<IDataBase> e)
        {
            this.Invoke(null);
        }
    }
}
