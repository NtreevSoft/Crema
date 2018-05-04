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
    class DataBaseResetEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DataBaseResetEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.DataBaseReset)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsReset += DataBases_ItemsReset);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsReset -= DataBases_ItemsReset);
            }
        }

        private void DataBases_ItemsReset(object sender, ItemsEventArgs<IDataBase> e)
        {
            this.Invoke(null);
        }
    }
}
