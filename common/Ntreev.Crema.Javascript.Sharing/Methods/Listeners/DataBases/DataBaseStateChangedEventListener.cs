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
    class DataBaseStateChangedEventListener : CremaEventListenerBase
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DataBaseStateChangedEventListener(ICremaHost cremaHost)
            : base(CremaEvents.DataBaseStateChanged)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsStateChanged += DataBases_ItemsStateChanged);
            }
        }

        protected override void OnUnsubscribe()
        {
            if (this.cremaHost.GetService(typeof(IDataBaseCollection)) is IDataBaseCollection dataBases)
            {
                dataBases.Dispatcher.Invoke(() => dataBases.ItemsStateChanged -= DataBases_ItemsStateChanged);
            }
        }

        private void DataBases_ItemsStateChanged(object sender, ItemsEventArgs<IDataBase> e)
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
