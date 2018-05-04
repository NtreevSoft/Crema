using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Javascript.Methods
{
    abstract class DataBaseEventListenerHost
    {
        private readonly DataBaseEvents eventName;
        private readonly Dictionary<IDataBase, DataBaseEventListenerCollection> dataBaseToListeners = new Dictionary<IDataBase, DataBaseEventListenerCollection>();

        protected DataBaseEventListenerHost(DataBaseEvents eventName)
        {
            this.eventName = eventName;
        }

        public void Dispose()
        {
            foreach (var item in this.dataBaseToListeners)
            {
                this.OnUnsubscribe(item.Key);
            }
            this.dataBaseToListeners.Clear();
        }

        public void Subscribe(IDataBase dataBase, DataBaseEventListener listener)
        {
            if (this.dataBaseToListeners.ContainsKey(dataBase) == false)
            {
                this.dataBaseToListeners[dataBase] = new DataBaseEventListenerCollection();
            }
            var listeners = this.dataBaseToListeners[dataBase];
            if (listeners.Any() == false)
            {
                this.OnSubscribe(dataBase);
            }
            listeners.Add(listener);
        }

        public void Unsubscribe(IDataBase dataBase, DataBaseEventListener listener)
        {
            var listeners = this.dataBaseToListeners[dataBase];
            listeners.Remove(listener);
            if (listeners.Any() == false)
            {
                this.OnUnsubscribe(dataBase);
                this.dataBaseToListeners.Remove(dataBase);
            }
        }

        public void Subscribe(IDataBase dataBase, IEnumerable<DataBaseEventListener> listeners)
        {
            this.dataBaseToListeners[dataBase] = new DataBaseEventListenerCollection(listeners);
            this.OnSubscribe(dataBase);
        }

        public void Unsubscribe(IDataBase dataBase)
        {
            this.OnUnsubscribe(dataBase);
            this.dataBaseToListeners.Remove(dataBase);
        }

        public CremaDispatcher Dispatcher
        {
            get; set;
        }

        public DataBaseEvents EventName => this.eventName;

        protected void InvokeAsync(IDataBase dataBase, IDictionary<string, object> properties)
        {
            var dataBaseName = dataBase.Name;
            var listeners = this.dataBaseToListeners[dataBase].ToArray();
            this.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in listeners)
                {
                    item.Invoke(dataBaseName, properties);
                }
            });
        }

        protected abstract void OnSubscribe(IDataBase dataBase);

        protected abstract void OnUnsubscribe(IDataBase dataBase);

        
    }
}
