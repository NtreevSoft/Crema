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
    class TypeItemMovedEventListenerHost : DataBaseEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public TypeItemMovedEventListenerHost(ICremaHost cremaHost)
            : base(DataBaseEvents.TypeStateChanged)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe(IDataBase dataBase)
        {
            dataBase.Dispatcher.Invoke(() => dataBase.TypeContext.ItemsMoved += TypeContext_ItemsMoved);
        }

        protected override void OnUnsubscribe(IDataBase dataBase)
        {
            dataBase.Dispatcher.Invoke(() => dataBase.TypeContext.ItemsMoved -= TypeContext_ItemsMoved);
        }

        private void TypeContext_ItemsMoved(object sender, ItemsMovedEventArgs<ITypeItem> e)
        {
            if (sender is ITypeContext context && context.GetService(typeof(IDataBase)) is IDataBase dataBase)
            {
                this.InvokeAsync(dataBase, null);
            }
        }
    }
}
