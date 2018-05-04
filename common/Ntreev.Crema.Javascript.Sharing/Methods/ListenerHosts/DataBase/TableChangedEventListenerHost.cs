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
    class TableChangedEventListenerHost : DataBaseEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public TableChangedEventListenerHost(ICremaHost cremaHost)
            : base(DataBaseEvents.TableChanged)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe(IDataBase dataBase)
        {
            dataBase.Dispatcher.Invoke(() => dataBase.TableContext.Tables.TablesChanged += Tables_TablesChanged);
        }

        protected override void OnUnsubscribe(IDataBase dataBase)
        {
            dataBase.Dispatcher.Invoke(() => dataBase.TableContext.Tables.TablesChanged -= Tables_TablesChanged);
        }

        private void Tables_TablesChanged(object sender, ItemsEventArgs<ITable> e)
        {
            if (sender is ITableCollection tables && tables.GetService(typeof(IDataBase)) is IDataBase dataBase)
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
