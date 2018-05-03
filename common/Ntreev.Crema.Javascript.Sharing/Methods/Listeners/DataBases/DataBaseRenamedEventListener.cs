using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Windows.Threading;

namespace Ntreev.Crema.Javascript.Methods.Listeners.DataBases
{
    [Export(typeof(CremaEventListenerBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class DataBaseRenamedEventListener : CremaEventListenerBase
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DataBaseRenamedEventListener(ICremaHost cremaHost)
            : base(CremaEvents.DataBaseRenamed)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsRenamed += DataBases_ItemsRenamed);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsRenamed -= DataBases_ItemsRenamed);
            }
        }

        private void DataBases_ItemsRenamed(object sender, ItemsRenamedEventArgs<IDataBase> e)
        {
            this.Invoke(null);
        }
    }
}
