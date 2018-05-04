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
    class DataBaseResettingEventListenerHost : CremaEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DataBaseResettingEventListenerHost(ICremaHost cremaHost)
            : base(CremaEvents.DataBaseResetting)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsResetting += DataBases_ItemsResetting);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsResetting -= DataBases_ItemsResetting);
            }
        }

        private void DataBases_ItemsResetting(object sender, ItemsEventArgs<IDataBase> e)
        {
            this.Invoke(null);
        }
    }
}
