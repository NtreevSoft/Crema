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
    class TableItemCreatedEventListenerHost : DataBaseEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public TableItemCreatedEventListenerHost(ICremaHost cremaHost)
            : base(DataBaseEvents.TableStateChanged)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe(IDataBase dataBase)
        {
            dataBase.Dispatcher.Invoke(() => dataBase.TableContext.ItemsCreated += TableContext_ItemsCreated);
        }

        protected override void OnUnsubscribe(IDataBase dataBase)
        {
            dataBase.Dispatcher.Invoke(() => dataBase.TableContext.ItemsCreated -= TableContext_ItemsCreated);
        }

        private void TableContext_ItemsCreated(object sender, ItemsCreatedEventArgs<ITableItem> e)
        {
            if (sender is ITableContext context && context.GetService(typeof(IDataBase)) is IDataBase dataBase)
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
