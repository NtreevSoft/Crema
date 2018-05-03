using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Windows.Threading;

namespace Ntreev.Crema.Javascript.Methods.Listeners.DataBases
{
    [Export(typeof(CremaDataBaseEventListenerBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class TableStateChangedEventListener : CremaDataBaseEventListenerBase
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public TableStateChangedEventListener(ICremaHost cremaHost)
            : base(CremaDataBaseEvents.TableStateChanged)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe(IDataBase dataBase)
        {
            dataBase.TableContext.Tables.TablesStateChanged += Tables_TablesStateChanged;
        }

        protected override void OnUnsubscribe(IDataBase dataBase)
        {
            dataBase.TableContext.Tables.TablesStateChanged -= Tables_TablesStateChanged;
        }

        private void Tables_TablesStateChanged(object sender, ItemsEventArgs<ITable> e)
        {
            this.Invoke(null);
        }
    }
}
