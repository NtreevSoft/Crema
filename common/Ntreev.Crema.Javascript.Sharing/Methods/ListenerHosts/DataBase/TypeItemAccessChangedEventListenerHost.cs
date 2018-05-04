using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Windows.Threading;

namespace Ntreev.Crema.Javascript.Methods.ListenerHosts.DataBases
{
    [Export(typeof(DataBaseEventListenerHost))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class TypeItemAccessChangedEventListenerHost : DataBaseEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public TypeItemAccessChangedEventListenerHost(ICremaHost cremaHost)
            : base(DataBaseEvents.TypeStateChanged)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe(IDataBase dataBase)
        {
            dataBase.Dispatcher.Invoke(() => dataBase.TypeContext.ItemsAccessChanged += TypeContext_ItemsAccessChanged);
        }

        protected override void OnUnsubscribe(IDataBase dataBase)
        {
            dataBase.Dispatcher.Invoke(() => dataBase.TypeContext.ItemsAccessChanged -= TypeContext_ItemsAccessChanged);
        }

        private void TypeContext_ItemsAccessChanged(object sender, ItemsEventArgs<ITypeItem> e)
        {
            if (sender is ITypeContext context && context.GetService(typeof(IDataBase)) is IDataBase dataBase)
            {
                foreach (var item in e.Items)
                {
                    var props = new Dictionary<string, object>()
                    {
                        { "Name", item.Name },
                    };
                    this.InvokeAsync(dataBase, null);
                }
            }
        }
    }
}
