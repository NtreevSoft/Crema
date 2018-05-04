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
    class DataBaseUnloadedEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DataBaseUnloadedEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.DataBaseUnloaded)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsUnloaded += DataBases_ItemsUnloaded);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsUnloaded -= DataBases_ItemsUnloaded);
            }
        }

        private void DataBases_ItemsUnloaded(object sender, ItemsEventArgs<IDataBase> e)
        {
            this.Invoke(null);
        }
    }
}
