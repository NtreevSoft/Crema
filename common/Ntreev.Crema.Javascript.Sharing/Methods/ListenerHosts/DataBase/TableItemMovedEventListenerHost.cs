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
    class TableItemMovedEventListenerHost : DataBaseEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public TableItemMovedEventListenerHost(ICremaHost cremaHost)
            : base(DataBaseEvents.TableStateChanged)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe(IDataBase dataBase)
        {
            dataBase.Dispatcher.Invoke(() => dataBase.TableContext.ItemsMoved += TableContext_ItemsMoved);
        }

        protected override void OnUnsubscribe(IDataBase dataBase)
        {
            dataBase.Dispatcher.Invoke(() => dataBase.TableContext.ItemsMoved -= TableContext_ItemsMoved);
        }

        private void TableContext_ItemsMoved(object sender, ItemsMovedEventArgs<ITableItem> e)
        {
            if (sender is ITableContext context && context.GetService(typeof(IDataBase)) is IDataBase dataBase)
            {
                this.InvokeAsync(dataBase, null);
            }
        }
    }
}
