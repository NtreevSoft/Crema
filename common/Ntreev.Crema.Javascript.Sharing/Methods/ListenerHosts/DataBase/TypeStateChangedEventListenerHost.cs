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
    class TypeStateChangedEventListenerHost : DataBaseEventListenerHost
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public TypeStateChangedEventListenerHost(ICremaHost cremaHost)
            : base(DataBaseEvents.TypeStateChanged)
        {
            this.cremaHost = cremaHost;
        }

        protected override void OnSubscribe(IDataBase dataBase)
        {
            dataBase.Dispatcher.Invoke(() => dataBase.TypeContext.Types.TypesStateChanged += Types_TypesStateChanged);
        }

        protected override void OnUnsubscribe(IDataBase dataBase)
        {
            dataBase.Dispatcher.Invoke(() => dataBase.TypeContext.Types.TypesStateChanged -= Types_TypesStateChanged);
        }

        private void Types_TypesStateChanged(object sender, ItemsEventArgs<IType> e)
        {
            if (sender is ITypeCollection types && types.GetService(typeof(IDataBase)) is IDataBase dataBase)
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
